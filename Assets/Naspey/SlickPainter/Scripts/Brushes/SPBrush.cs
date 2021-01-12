using UnityEngine;

namespace Naspey.SlickPainter
{
    /// <summary>
    /// Base class describing a brush.
    /// </summary>
    [System.Serializable]
    public abstract class SPBrush
    {
        [SerializeField]
        protected int _size = 16;
        
        public int Size
        {
            get => _size;
            set
            {
                if (_size != value)
                    _cachedBrushTexture = null;

                _size = Mathf.Clamp(value, 0, 2048);
            }
        }

        [SerializeField, Range(1, 100)]
        protected float _hardness = 10;

        public float Hardness
        {
            get => _hardness;
            set
            {
                if (!Mathf.Approximately(_hardness, value))
                    _cachedBrushTexture = null;

                _hardness = Mathf.Clamp(value, 1, 100);
            }
        }

        protected Texture2D _cachedBrushTexture = null;

        public SPBrush() { }

        public SPBrush(int size, float hardness)
        {
            _size = size;
            _hardness = hardness;
            _cachedBrushTexture = null;
        }

        /// <summary>
        /// Returns all pixels that can be contained in the provided rect.
        /// </summary>
        public Color[] GetPixels(BrushRect rect, Color color)
        {
            if (_cachedBrushTexture == null)
                CreateBrushTextureCached();

            // Getting clipped pixels
            var brushPixels = _cachedBrushTexture.GetPixels(rect.BrushOffsetX, rect.BrushOffsetY, rect.Width, rect.Height);

            ColorPixels(brushPixels, color);

            return brushPixels;
        }

        /// <summary>
        /// Creates new brush texture for cache.
        /// </summary>
        protected abstract void CreateBrushTextureCached();

        /// <summary>
        /// Colors pixels of the brush.
        /// </summary>
        protected virtual void ColorPixels(Color[] pixels, Color color)
        {
            for (int i = 0; i < pixels.Length; i++)
                pixels[i] *= color;
        }

        /// <summary>
        /// Checks if brush cached texture has proper size and format, if not it creates new texture.
        /// </summary>
        protected void PrepareBrushCacheTexture()
        {
            if (_cachedBrushTexture != null && _cachedBrushTexture.width == Size && _cachedBrushTexture.height == Size &&
                _cachedBrushTexture.format == TextureFormat.RGBA32)
                return;

            if (_cachedBrushTexture != null)
                Object.Destroy(_cachedBrushTexture);

            _cachedBrushTexture = new Texture2D(Size, Size, TextureFormat.RGBA32, false);
        }
    }
}