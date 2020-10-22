using UnityEngine;

namespace Naspey.LogoMaker.Blending
{
    public interface IBlendPixel
    {
        Color Blend(Color a, Color b);
    }
}