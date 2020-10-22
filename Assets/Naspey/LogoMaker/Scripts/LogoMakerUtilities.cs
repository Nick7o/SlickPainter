using UnityEngine;
using Naspey.LogoMaker.Blending;

namespace Naspey.LogoMaker
{
    public static class LogoMakerUtilities
    {
        public static void Blend(this Texture2D sourceTex, Color[] brushPixels, BrushRect rect, BlendModes blendMode = BlendModes.Normal)
        {
            if (!IsBrushRectCorrect(rect, sourceTex))
            {
                Debug.LogError("Couldn't blend textures! Brush rectangle was outside of bounds.");
                return;
            }

            var canvasPixels = sourceTex.GetPixels(rect.X, rect.Y, rect.Width, rect.Height);
            var brushColors = brushPixels;

            var blending = BlendingManager.GetBlendingService(blendMode);
            for (int i = 0; i < canvasPixels.Length; i++)
            {
                canvasPixels[i] = blending.Blend(canvasPixels[i], brushColors[i]);
            }

            // Setting source texture pixels. BrushRect coordinates are relative to canvas, 
            // so we can use it to know which pixels to set on canvas.
            sourceTex.SetPixels(rect.X, rect.Y, rect.Width, rect.Height, canvasPixels);
        }

        /// <summary>
        /// Checks if brush rect is contained inside texture.
        /// </summary>
        public static bool IsBrushRectCorrect(BrushRect rect, Texture2D texture)
        {
            return rect.X + rect.Width <= texture.width &&
                rect.Y + rect.Height <= texture.height;
        }
    }
}