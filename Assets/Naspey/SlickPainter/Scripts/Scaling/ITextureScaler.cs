using UnityEngine;

namespace Naspey.SlickPainter
{
    public interface ITextureScaler
    {
        Texture2D Scale(Texture2D src, int newWidth, int newHeight);
    }
}