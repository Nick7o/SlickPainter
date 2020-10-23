using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Naspey.SlickPainter
{
    public class LMBilinearScaling : ITextureScaler
    {
        /// <summary>
        /// Scales provided texture to specified new width and height using bilinear scaling.
        /// </summary>
        /// <returns>Rescaled texture.</returns>
        public Texture2D Scale(Texture2D src, int newWidth, int newHeight)
        {
            Color32[] result = new Color32[newWidth * newHeight];

            float tx = src.width / (float)newWidth;
            float ty = src.height / (float)newHeight;

            for (int i = 0; i < newHeight; i++)
            {
                for (int j = 0; j < newWidth; j++)
                {
                    var x = (tx * j);
                    var y = (ty * i);

                    result[i * newWidth + j] = src.GetPixelBilinear(x / (float)src.width, y / (float)src.height);
                }
            }

            src.Resize(newWidth, newHeight);
            src.SetPixels32(result);
            src.Apply();
            return src;
        }
    }
}