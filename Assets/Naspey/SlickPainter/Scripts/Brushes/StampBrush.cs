using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Naspey.SlickPainter
{
    public class StampBrush : SPBrush
    {
        [SerializeField]
        private Sprite _stamp;

        public Sprite Stamp
        {
            get => _stamp;
            set
            {
                if (_stamp != value)
                    _cachedBrushTexture = null;

                _stamp = value;
            }
        }

        public StampBrush(int size, float hardness, Sprite stamp) : base(size, hardness)
        {
            _stamp = stamp;
        }

        protected override void CreateBrushTextureCached()
        {
            if (_stamp != null)
            {
                var texCopy = TextureUtilities.CopyNotReadableTex(_stamp.texture);
                _cachedBrushTexture = SlickPainter.TextureScaler.Scale(texCopy, Size, Size);
                Object.Destroy(texCopy);
            }
        }

        // Stamps aren't supposed to be colored this way by design.
        // If you want them to be colored just comment/remove method below.
        protected override void ColorPixels(Color[] pixels, Color color) { }
    }
}