using UnityEngine;

namespace Naspey.SlickPainter.Blending
{
    public interface IBlendPixel
    {
        Color Blend(Color a, Color b);
    }
}