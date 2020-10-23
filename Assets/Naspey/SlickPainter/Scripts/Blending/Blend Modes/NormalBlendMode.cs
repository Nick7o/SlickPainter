using UnityEngine;

namespace Naspey.SlickPainter.Blending
{
    /// <summary>
    /// Standard blend mode supporting alpha blending.
    /// </summary>
    public class NormalBlendMode : IBlendPixel
    {
        public Color Blend(Color a, Color b)
        {
            Color result;
            result.a = b.a + a.a * (1 - b.a);

            result.r = b.r * b.a + a.r * a.a * (1 - b.a) / result.a;
            result.g = b.g * b.a + a.g * a.a * (1 - b.a) / result.a;
            result.b = b.b * b.a + a.b * a.a * (1 - b.a) / result.a;

            return result;
        }
    }
}