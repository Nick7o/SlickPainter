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
        public static ITextureScaler TextureScaler { get; set; } = new LMBilinearScaling();

        [Header("General")]
        [SerializeField] Vector2Int canvasSize = new Vector2Int(128, 128);
        public Vector2Int CanvasSize => canvasSize;

        [SerializeField] Color backgroundColor = Color.white;
        public Color BackgroundColor { get => backgroundColor; set => backgroundColor = value; }

        // Alpha channel support
        [SerializeField] bool useAlphaChannel = true;
        public bool UseAlphaChannel { get => useAlphaChannel; set => useAlphaChannel = value; }

        [Space]
        [SerializeField] SPBrush brush = new CircleBrush();
        [SerializeField] Color brushColor = Color.black;

        [Header("Eraser")]
        [SerializeField] bool isErasing = false;
        public bool IsErasing { get => isErasing; set => isErasing = value; }

        [Space]
        [SerializeField] BlendModes blendMode = BlendModes.Normal;
        /// <summary> Current blend mode in use. </summary>
        public BlendModes BlendMode { get => IsErasing && useAlphaChannel ? BlendModes.Eraser : blendMode; set => blendMode = value; }

        [Header("Undo/Redo")]
        [SerializeField] int maxHistoryDepth = 15;
        public int MaxHistoryDepth => maxHistoryDepth;

        // Key mapping for undo/redo
        public bool UseUndoRedoCommonKey => UndoRedoCommonKey != KeyCode.None;
        public KeyCode UndoRedoCommonKey = KeyCode.LeftControl;
        public KeyCode UndoKey = KeyCode.Z;
        public KeyCode RedoKey = KeyCode.Y;

        public IUndoRedo UndoRedo { get; private set; }

        // ________ PRIVATE MEMBERS ________
        // Unity components used for displaying textures
        RawImage canvasImage;
        RawImage brushCanvasImage;

        // Actual textures holding the data
        Texture2D canvasTex;
        Texture2D brushCanvasTex;

        Vector2 canvasMousePosition;

        // Events
        public event System.Action<SPBrush> BrushChangedEvent;
        public event System.Action<Color> ColorChangedEvent;

        #region Properties
        public SPBrush Brush
        {
            get => brush;
            set
            {
                if (!value.Equals(default))
                {
                    brush = value;
                    BrushChangedEvent?.Invoke(brush);
                }
            }
        }

        /// <summary>
        /// Gets current color in usage. Returns background color if erasing is enabled.
        /// Setter changes brush color while background color stays the same.
        /// </summary>
        public Color Color
        {
            get
            {
                return !isErasing ? brushColor :
                    useAlphaChannel ? backgroundColor :
                    new Color(backgroundColor.r, backgroundColor.g, backgroundColor.b);
            }
            set
            {
                brushColor = value;
                ColorChangedEvent?.Invoke(brushColor);
            }
        }

        /// <summary>
        /// Stores normalized position of mouse relative to canvas rect transform.
        /// </summary>
        public Vector2 CanvasMousePosition
        {
            get => canvasMousePosition;
            set => canvasMousePosition = value;
        }

        /// <summary>
        /// Points to the texture displayed on the canvas.
        /// </summary>
        public Texture2D Result => canvasTex;
        #endregion

        private void Awake()
        {
            UndoRedo = new BasicUndoRedo(this, MaxHistoryDepth);
        }

        private void Start()
        {
            canvasImage = GetComponent<RawImage>();

            // Preparing brush canvas
            if (brushCanvasImage == null)
            {
                brushCanvasImage = new GameObject("Brush Canvas").AddComponent<RawImage>();
                brushCanvasImage.transform.parent = transform;

                var rt = brushCanvasImage.rectTransform;
                rt.anchorMin = Vector2.zero;
                rt.anchorMax = Vector2.one;
                rt.offsetMin = Vector2.zero;
                rt.offsetMax = Vector2.zero;
            }

            // Creating empty textures
            Clear();
        }

        int paintFramesCount = 0;
        private void Update()
        {
            if (isMouseOverCanvas)
            {
                Vector2 lastFrameMousePos = CanvasMousePosition;
                GetCanvasMousePosition(ref canvasMousePosition);
                float deltaMouseDistance = Vector2.Distance(lastFrameMousePos, CanvasMousePosition);

                // Optimization - only update canvas graphics when user moved their mouse or click it
                if (deltaMouseDistance > Mathf.Epsilon || Input.GetKeyDown(KeyCode.Mouse0))
                {
                    if (Input.GetKey(KeyCode.Mouse0))
                    {
                        if (paintFramesCount == 0)
                            UndoRedo.RegisterState();

                        Paint(GetBrushRect());
                        paintFramesCount++;
                    }

                    DrawBrushPreview();
                }
            }

            if (Input.GetKeyUp(KeyCode.Mouse0))
                paintFramesCount = 0;

            if (Input.GetKeyDown(UndoKey) && (!UseUndoRedoCommonKey || Input.GetKey(UndoRedoCommonKey)))
                UndoRedo.Undo();

            if (Input.GetKeyDown(RedoKey) && (!UseUndoRedoCommonKey || Input.GetKey(UndoRedoCommonKey)))
                UndoRedo.Redo();
        }

        #region Mouse Detection
        bool isMouseOverCanvas = false;
        public void OnPointerEnter(PointerEventData eventData) => isMouseOverCanvas = true;
        public void OnPointerExit(PointerEventData eventData) => isMouseOverCanvas = false;
        #endregion Mouse Detection

        /// <summary>
        /// Draws brush preview over mouse position on brush canvas texture.
        /// </summary>
        private void DrawBrushPreview()
        {
            TextureUtilities.ClearTexture(brushCanvasTex);
            var rect = GetBrushRect();
            Paint(rect, brushCanvasTex);
        }

        /// <summary>
        /// Gets rectangle that is contained inside canvas and clipped outside, so the pixels can be correctly collected from the brush texture.
        /// </summary>
        private BrushRect GetBrushRect()
        {
            // Calculating position of the first brush pixel ([0, 0] position is top left corner)
            var brushStartPixel = new Vector2(CanvasMousePosition.x * canvasSize.x, CanvasMousePosition.y * canvasSize.y);
            brushStartPixel -= new Vector2(Brush.Size, Brush.Size) * 0.5f;

            int width = Brush.Size;
            int height = Brush.Size;

            // Handling cases when brush is outside of canvas
            if (brushStartPixel.x > brushCanvasTex.width - Brush.Size)
                width = brushCanvasTex.width - (int)brushStartPixel.x;
            else if (brushStartPixel.x <= 0)
                width = Brush.Size + (int)brushStartPixel.x;

            if (brushStartPixel.y > brushCanvasTex.height - Brush.Size)
                height = brushCanvasTex.height - (int)brushStartPixel.y;
            else if (brushStartPixel.y <= 0)
                height = Brush.Size + (int)brushStartPixel.y;

            return new BrushRect((int)brushStartPixel.x, (int)brushStartPixel.y, width, height);
        }

        /// <summary>
        /// Paints current brush inside rect on the texture.
        /// </summary>
        private void Paint(BrushRect rect, Texture2D texture, BlendModes blendMode = BlendModes.Normal)
        {
            texture.Blend(Brush.GetPixels(rect, Color), rect, blendMode);
            texture.Apply();
        }

        /// <summary>
        /// Paints current brush in specified brush rect on main canvas texture.
        /// </summary>
        public void Paint(BrushRect rect)
        {
            Paint(rect, canvasTex, BlendMode);
        }

        /// <summary>
        /// Clears or creates (if they are null) canvas textures.
        /// </summary>
        public void Clear()
        {
            if (canvasTex != null)
                Destroy(canvasTex);
            if (brushCanvasTex != null)
                Destroy(brushCanvasTex);

            // Main canvas texture initialization
            canvasTex = new Texture2D(canvasSize.x, canvasSize.y, TextureFormat.RGBA32, false)
            {
                wrapMode = TextureWrapMode.Clamp
            };
            TextureUtilities.ColorTexture(canvasTex, backgroundColor);
            canvasTex.Apply();
            canvasImage.texture = canvasTex;

            // Brush canvas texture initialization
            brushCanvasTex = new Texture2D(canvasSize.x, canvasSize.y, TextureFormat.RGBA32, false)
            {
                wrapMode = TextureWrapMode.Clamp
            };
            TextureUtilities.ClearTexture(brushCanvasTex);
            brushCanvasTex.Apply();
            brushCanvasImage.texture = brushCanvasTex;
        }

        /// <summary>
        /// Loads texture data into canvas texture.
        /// </summary>
        public void Load(byte[] textureData)
        {
            canvasTex.LoadImage(textureData);
        }

        /// <summary>
        /// Resizes canvas to specified size. Content gets scaled using current TextureScaler.
        /// </summary>
        public void Resize(Vector2Int size)
        {
            canvasSize = size;
            
            if(canvasTex != null)
                TextureScaler.Scale(canvasTex, size.x, size.y);

            if (brushCanvasTex != null)
                TextureScaler.Scale(brushCanvasTex, size.x, size.y);
        }

        /// <summary>
        /// Calculates normalized mouse position inside painting canvas.
        /// </summary>
        public void GetCanvasMousePosition(ref Vector2 result)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasImage.rectTransform, Input.mousePosition, null, out var localPoint);
            // Normalizing result position and correcting pivot alignment
            result = new Vector2(localPoint.x / canvasImage.rectTransform.sizeDelta.x, localPoint.y / canvasImage.rectTransform.sizeDelta.y) + canvasImage.rectTransform.pivot;
        }
    }
}