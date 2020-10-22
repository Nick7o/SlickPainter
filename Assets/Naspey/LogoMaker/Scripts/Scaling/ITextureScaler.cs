using UnityEngine;

namespace Naspey.LogoMaker
{
    public interface ITextureScaler
    {
        Texture2D Scale(Texture2D src, int newWidth, int newHeight);
    }
}