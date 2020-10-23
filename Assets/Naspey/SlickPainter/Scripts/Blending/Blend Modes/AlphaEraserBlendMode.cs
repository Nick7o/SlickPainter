using UnityEngine;

namespace Naspey.SlickPainter.Blending
{
    /// <summary>
    /// Blends alphas of color a and b. Meant to be used as a eraser for alpha backgrounds.
    /// </summary>
    public class AlphaEraserBlendMode : IBlendPixel
    {
        public Color Blend(Color a, Color b)
        {
            Color result = a;
            result.a = a.a * (1 - b.a);

            return result;
        }
    }
}