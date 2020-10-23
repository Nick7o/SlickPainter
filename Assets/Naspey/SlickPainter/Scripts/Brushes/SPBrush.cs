using UnityEngine;

namespace Naspey.SlickPainter
{
    /// <summary>
    /// Class describing brush.
    /// </summary>
    [System.Serializable]
    public abstract class SPBrush
    {
        [SerializeField] int size = 16;
        public int Size
        {
            get => size;
            set
            {
                if (size != value)
                    cachedBrushTexture = null;

                size = Mathf.Clamp(value, 0, 2048);
            }
        }

        [SerializeField, Range(1, 100)] float hardness = 10;
        public float Hardness
        {
            get => hardness;
            set
            {
                if (!Mathf.Approximately(hardness, value))
                    cachedBrushTexture = null;

                hardness = Mathf.Clamp(value, 1, 100);
            }
        }

        protected Texture2D cachedBrushTexture = null;

        public SPBrush() { }

        public SPBrush(int size, float hardness)
        {
            this.size = size;
            this.hardness = hardness;
            cachedBrushTexture = null;
        }

        /// <summary>
        /// Returns all pixels that can be contained in the provided rect.
        /// </summary>
        public Color[] GetPixels(BrushRect rect, Color color)
        {
            if (cachedBrushTexture == null)
                CreateBrushTextureCached();

            // Getting clipped pixels
            var brushPixels = cachedBrushTexture.GetPixels(rect.BrushOffsetX, rect.BrushOffsetY, rect.Width, rect.Height);

            ColorPixels(brushPixels, color);

            return brushPixels;
        }

        /// <summary>
        /// Creates new brush texture for cachedBrushTexture field.
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
            if (cachedBrushTexture != null && cachedBrushTexture.width == Size && cachedBrushTexture.height == Size &&
                cachedBrushTexture.format == TextureFormat.RGBA32)
                return;

            if (cachedBrushTexture != null)
                Object.Destroy(cachedBrushTexture);

            cachedBrushTexture = new Texture2D(Size, Size, TextureFormat.RGBA32, false);
        }
    }
}