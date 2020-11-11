using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Naspey.SlickPainter
{
    public class StampBrush : SPBrush
    {
        [SerializeField] Sprite stamp;
        public Sprite Stamp
        {
            get => stamp;
            set
            {
                if (stamp != value)
                    cachedBrushTexture = null;

                stamp = value;
            }
        }

        public StampBrush(int size, float hardness, Sprite stamp) : base(size, hardness)
        {
            this.stamp = stamp;
        }

        protected override void CreateBrushTextureCached()
        {
            if (stamp != null)
            {
                var texCopy = TextureUtilities.CopyNotReadableTex(stamp.texture);
                cachedBrushTexture = SlickPainter.TextureScaler.Scale(texCopy, Size, Size);
                Object.Destroy(texCopy);
            }
        }

        // Stamps aren't supposed to be colored this way by design.
        // If you want them to be colored just comment/remove method below.
        protected override void ColorPixels(Color[] pixels, Color color) { }
    }
}