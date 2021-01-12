using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using Naspey.SlickPainter.Blending;
using Naspey.SlickPainter.UndoRedo;

namespace Naspey.SlickPainter
{
    /// <summary>
    /// Main component of the painter. Handles input and events.
    /// </summary>
    [RequireComponent(typeof(RawImage))]
    public class SlickPainter : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        /// <summary>
        /// Implementation of texture scaling algorithm.
        /// </summary>
        public static ITextureScaler TextureScaler { get; set; }

        public event System.Action<SPBrush> BrushChangedEvent;
        public event System.Action<Color> ColorChangedEvent;

        public SlickPainterInput PainterInput;

        [Header("General")]
        [SerializeField] private Vector2Int _canvasSize = new Vector2Int(512, 512);
        [SerializeField] private Color _backgroundColor = Color.white;

        // Alpha channel support
        [SerializeField] private bool _useAlphaChannel = true;

        [Space]
        [SerializeField] private InterpolationSettings _interpolationSettings;

        [SerializeField] private SPBrush _brush;
        [Header("Brush")]
        [SerializeField] private Color _brushColor = Color.black;

        [Header("Eraser")]
        [SerializeField] private bool _isErasing = false;

        [Space]
        [SerializeField] private BlendModes _blendMode = BlendModes.Normal;

        [Header("Undo/Redo")]
        [SerializeField] private int _maxHistoryDepth = 15;

        ///////////// NOT SERIALIZED MEMBERS /////////////
        // Unity components used for displaying textures
        private RawImage _canvasImage;
        private RawImage _brushCanvasImage;

        // Textures storing the data
        private Texture2D _canvasTex;
        private Texture2D _brushCanvasTex;

        private Vector2 _canvasMousePosition;

        private InterpolationFramedata? _lastInterpolationFramedata = null;


        public Vector2Int CanvasSize => _canvasSize;

        public Color BackgroundColor { get => _backgroundColor; set => _backgroundColor = value; }

        public bool UseAlphaChannel { get => _useAlphaChannel; set => _useAlphaChannel = value; }

        public bool IsErasing { get => _isErasing; set => _isErasing = value; }

        public int MaxHistoryDepth => _maxHistoryDepth;

        public IUndoRedo UndoRedo { get; private set; }

        public InterpolationSettings InterpolationSettings => _interpolationSettings;

        /// <summary>
        /// Current blend mode in use.
        /// </summary>
        public BlendModes BlendMode { get => IsErasing && _useAlphaChannel ? BlendModes.Eraser : _blendMode; set => _blendMode = value; }

        public SPBrush Brush
        {
            get => _brush;
            set
            {
                if (value != null)
                {
                    _brush = value;
                    BrushChangedEvent?.Invoke(_brush);
                }
            }
        }

        /// <summary>
        /// Gets current color in usage. Returns background color if erasing is enabled. Setter changes only brush color.
        /// </summary>
        public Color Color
        {
            get
            {
                return !_isErasing ? _brushColor :
                    _useAlphaChannel ? _backgroundColor :
                    new Color(_backgroundColor.r, _backgroundColor.g, _backgroundColor.b);
            }
            set
            {
                _brushColor = value;
                ColorChangedEvent?.Invoke(_brushColor);
            }
        }

        /// <summary>
        /// Stores normalized position of mouse relative to canvas rect transform.
        /// </summary>
        public Vector2 CanvasMousePosition
        {
            get => _canvasMousePosition;
            set => _canvasMousePosition = value;
        }

        /// <summary>
        /// Points to the texture displayed on the canvas.
        /// </summary>
        public Texture2D CanvasTexture => _canvasTex;

        private void Awake()
        {
            if (PainterInput == null)
                PainterInput = new SlickPainterInput();

            if (InterpolationSettings == null)
                _interpolationSettings = new InterpolationSettings();

            UndoRedo = new BasicUndoRedo(this, MaxHistoryDepth);
            TextureScaler = new LMBilinearScaling();

            if (Brush == null)
                Brush = new CircleBrush();
        }

        private void Start()
        {
            _canvasImage = GetComponent<RawImage>();

            // Preparing brush canvas
            if (_brushCanvasImage == null)
            {
                _brushCanvasImage = new GameObject("Brush Canvas").AddComponent<RawImage>();
                _brushCanvasImage.transform.parent = transform;

                var rt = _brushCanvasImage.rectTransform;
                rt.anchorMin = Vector2.zero;
                rt.anchorMax = Vector2.one;
                rt.offsetMin = Vector2.zero;
                rt.offsetMax = Vector2.zero;
            }

            // Creating empty textures
            Clear();
        }

        private int _paintFramesCount = 0;
        private void Update()
        {
            if (isMouseOverCanvas)
            {
                Vector2 lastFrameMousePos = CanvasMousePosition;
                GetCanvasMousePosition(ref _canvasMousePosition);
                float deltaMouseDistance = Vector2.Distance(lastFrameMousePos, CanvasMousePosition);

                // Only updates canvas graphics when user moved their mouse or click it
                if (deltaMouseDistance > Mathf.Epsilon || Input.GetKeyDown(KeyCode.Mouse0))
                {
                    if (Input.GetKey(KeyCode.Mouse0))
                    {
                        if (_paintFramesCount == 0)
                            UndoRedo.RegisterState();

                        Paint(CanvasMousePosition);
                        _paintFramesCount++;
                    }

                    DrawBrushPreview();
                }
            }

            if (Input.GetKeyUp(KeyCode.Mouse0))
            {
                _paintFramesCount = 0;
                // Reseting, we don't want to have a "stained" screen
                _lastInterpolationFramedata = null;
            }

            if (Input.GetKeyDown(PainterInput.UndoKey) && (!PainterInput.UseUndoRedoCommonKey || Input.GetKey(PainterInput.UndoRedoCommonKey)))
                UndoRedo.Undo();

            if (Input.GetKeyDown(PainterInput.RedoKey) && (!PainterInput.UseUndoRedoCommonKey || Input.GetKey(PainterInput.UndoRedoCommonKey)))
                UndoRedo.Redo();
        }

        /// <summary>
        /// Draws brush preview over mouse position on brush canvas texture.
        /// </summary>
        private void DrawBrushPreview()
        {
            TextureUtilities.ClearTexture(_brushCanvasTex);
            PaintNotInterpolated(CanvasMousePosition, _brushCanvasTex);
        }

        /// <summary>
        /// Gets rectangle that is contained inside canvas and clipped outside, so the pixels can be correctly collected from the brush texture.
        /// </summary>
        private BrushRect GetBrushRect(Vector2 normalizedCanvasPosition)
        {
            // Calculating position of the first brush pixel ([0, 0] position is top left corner)
            var brushStartPixel = new Vector2(normalizedCanvasPosition.x * _canvasSize.x, normalizedCanvasPosition.y * _canvasSize.y);
            brushStartPixel -= new Vector2(Brush.Size, Brush.Size) * 0.5f;

            int width = Brush.Size;
            int height = Brush.Size;

            // Handling cases when brush is outside of canvas
            if (brushStartPixel.x > _brushCanvasTex.width - Brush.Size)
                width = _brushCanvasTex.width - (int)brushStartPixel.x;
            else if (brushStartPixel.x <= 0)
                width = Brush.Size + (int)brushStartPixel.x;

            if (brushStartPixel.y > _brushCanvasTex.height - Brush.Size)
                height = _brushCanvasTex.height - (int)brushStartPixel.y;
            else if (brushStartPixel.y <= 0)
                height = Brush.Size + (int)brushStartPixel.y;

            return new BrushRect((int)brushStartPixel.x, (int)brushStartPixel.y, width, height);
        }

        /// <summary>
        /// Paints current brush inside rect on the texture.
        /// </summary>
        private void Paint(Vector2 normalizedCanvasPosition, Texture2D texture, BlendModes blendMode = BlendModes.Normal)
        {
            var interpolationFramedata = new InterpolationFramedata(normalizedCanvasPosition);

            if (_lastInterpolationFramedata == null)
                _lastInterpolationFramedata = interpolationFramedata;

            var densityFactor = CalculateDensityFactor(Brush);
            var interpolatedPoints = _lastInterpolationFramedata.Value.GetInterpolatedPoints(interpolationFramedata, densityFactor);
            foreach (var point in interpolatedPoints)
                PaintNotInterpolated(point, texture, blendMode);

            _lastInterpolationFramedata = interpolationFramedata;
        }

        /// <summary>
        /// Paints current brush in specified brush rect on main canvas texture.
        /// </summary>
        public void Paint(Vector2 normalizedCanvasPosition)
        {
            Paint(normalizedCanvasPosition, CanvasTexture, BlendMode);
        }

        /// <summary>
        /// Paints current brush in specified brush rect on main canvas texture.
        /// </summary>
        public void PaintNotInterpolated(Vector2 normalizedCanvasPosition, Texture2D texture, BlendModes blendMode = BlendModes.Normal)
        {
            var rect = GetBrushRect(normalizedCanvasPosition);
            texture.Blend(Brush.GetPixels(rect, Color), rect, blendMode);
            texture.Apply();
        }

        /// <summary>
        /// Clears or creates (if they are null) canvas textures.
        /// </summary>
        public void Clear()
        {
            if (_canvasTex != null)
                Destroy(_canvasTex);
            if (_brushCanvasTex != null)
                Destroy(_brushCanvasTex);

            // Main canvas texture initialization
            _canvasTex = new Texture2D(_canvasSize.x, _canvasSize.y, TextureFormat.RGBA32, false)
            {
                wrapMode = TextureWrapMode.Clamp
            };
            TextureUtilities.ColorTexture(_canvasTex, _backgroundColor);
            _canvasTex.Apply();
            _canvasImage.texture = _canvasTex;

            // Brush canvas texture initialization
            _brushCanvasTex = new Texture2D(_canvasSize.x, _canvasSize.y, TextureFormat.RGBA32, false)
            {
                wrapMode = TextureWrapMode.Clamp
            };
            TextureUtilities.ClearTexture(_brushCanvasTex);
            _brushCanvasTex.Apply();
            _brushCanvasImage.texture = _brushCanvasTex;
        }

        /// <summary>
        /// Loads texture data into canvas texture.
        /// </summary>
        public void Load(byte[] textureData)
        {
            CanvasTexture.LoadImage(textureData);
        }

        /// <summary>
        /// Resizes canvas to specified size. Content gets scaled using current TextureScaler.
        /// </summary>
        public void Resize(Vector2Int size)
        {
            _canvasSize = size;
            
            if(_canvasTex != null)
                TextureScaler.Scale(_canvasTex, size.x, size.y);

            if (_brushCanvasTex != null)
                TextureScaler.Scale(_brushCanvasTex, size.x, size.y);
        }

        /// <summary>
        /// Calculates normalized mouse position inside painting canvas.
        /// </summary>
        public void GetCanvasMousePosition(ref Vector2 result)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(_canvasImage.rectTransform, Input.mousePosition, null, out var localPoint);
            // Normalizing result position and correcting pivot alignment
            result = new Vector2(localPoint.x / _canvasImage.rectTransform.sizeDelta.x, localPoint.y / _canvasImage.rectTransform.sizeDelta.y) + _canvasImage.rectTransform.pivot;
        }

        /// <summary>
        /// Calculates density factor for brush using current interpolation settings.
        /// </summary>
        public float CalculateDensityFactor(SPBrush brush)
        {
            return (192f / brush.Size) * InterpolationSettings.PointsDensity;
        }

        // IPointerEnterHandler & IPointerExitHandler implementation
        bool isMouseOverCanvas = false;
        public void OnPointerEnter(PointerEventData eventData) => isMouseOverCanvas = true;
        public void OnPointerExit(PointerEventData eventData) => isMouseOverCanvas = false;
    }
}