using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Naspey.SlickPainter
{
    public class CircleBrush : SPBrush
    {
        public CircleBrush() { }
        public CircleBrush(int size, float hardness) : base(size, hardness) { }

        protected override void CreateBrushTextureCached()
        {
            PrepareBrushCacheTexture();

            var colors = new Color32[Size * Size];
            Vector2 normalizedCenter = Vector2.one * 0.5f;

            for (int y = 0; y < _cachedBrushTexture.height; y++)
            {
                for (int x = 0; x < _cachedBrushTexture.width; x++)
                {
                    Vector2 pixelPosition = new Vector2(x / (float)Size, y / (float)Size);

                    float distFromCenter = Vector2.Distance(pixelPosition, normalizedCenter);
                    float value = Mathf.Clamp01((1 - distFromCenter * 2) * Hardness);

                    // Converting to 1 dimensional array
                    int pixelIndex = y * _cachedBrushTexture.width + x;
                    // Creating new white pixel with calculated transparency and setting it
                    colors[pixelIndex] = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, (byte)(value * byte.MaxValue));
                }
            }

            _cachedBrushTexture.SetPixels32(colors);
            _cachedBrushTexture.Apply();
        }
    }
}