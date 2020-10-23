using System.Collections.Generic;
using UnityEngine;

namespace Naspey.SlickPainter
{
    public static class TextureUtilities
    {
        private static readonly Dictionary<string, Texture2D> _tempTextures = new Dictionary<string, Texture2D>();

        /// <summary>
        /// Frees memory occupied by temporary textures used by TextureUtilities.
        /// </summary>
        public static void FreeTempTextures()
        {
            foreach (var texture in _tempTextures)
                Object.Destroy(texture.Value);

            _tempTextures.Clear();
        }

        /// <summary>
        /// Clears texture pixel data. Optimized for clearing multiple textures of the same dimensions and format.
        /// </summary>
        public static void ClearTexture(Texture2D texture)
        {
            var clearTexture = CreateOrGetColorTexture("clearTex", texture.width, texture.height, texture.format, new Color32());
            CopyTexture(clearTexture, texture);
        }

        /// <summary>
        /// Changes all texture pixels to the specified color.
        /// </summary>
        public static void ColorTexture(Texture2D texture, Color32 color)
        {
            var colorTexture = CreateOrGetColorTexture("colorTex", texture.width, texture.height, texture.format, color);
            CopyTexture(colorTexture, texture);
        }

        /// <summary>
        /// Copies texture data from source to the destination. Both must have the same dimensions and format.
        /// </summary>
        public static void CopyTexture(Texture2D source, Texture2D destination)
        {
            if (SystemInfo.copyTextureSupport == UnityEngine.Rendering.CopyTextureSupport.None)
            {
                // Fall back for devices that doesn't support Graphics.CopyTexture()
                Color[] pixels = source.GetPixels();
                destination.SetPixels(pixels);
                destination.Apply();
            }
            else
                Graphics.CopyTexture(source, 0, 0, destination, 0, 0);
        }

        /// <summary>
        /// Gets already created texture or create new one if destination texture is null or if it doesn't meet
        /// the criteria of width, height, format and color of the first pixel (compared to the one passed in the parameter).
        /// </summary>
        private static Texture2D CreateOrGetColorTexture(string tempTextureName, int width, int height, TextureFormat format, Color32 color)
        {
            // Creating a key in the dictionary if it doesn't exist
            if (!_tempTextures.TryGetValue(tempTextureName, out var destination))
            {
                _tempTextures.Add(tempTextureName, null);
                destination = _tempTextures[tempTextureName];
            }

            // Destroying object from memory if it needs no longer needed
            if(destination != null && !CompareSizeAndFormat(destination, width, height, format))
                Object.Destroy(destination);

            if(destination == null)
                destination = new Texture2D(width, height, format, false);

            _tempTextures[tempTextureName] = destination;

            var colors = new Color32[width * height];

            // Default instance of Color32 is transparent, so if the 'color' parameter is transparent
            // we don't want to spend time on changing colors.
            if (!color.Equals(default))
            {
                for (int i = 0; i < colors.Length; i++)
                    colors[i] = color;
            }

            destination.SetPixels32(colors);
            destination.Apply();

            return destination;
        }

        /// <summary>
        /// Scales texture using provided scaling algorithm implementation.
        /// </summary>
        /// <returns>Scaled texture.</returns>
        public static Texture2D Scale(ITextureScaler scaler, Texture2D texture, int newWidth, int newHeight)
        {
            return scaler.Scale(texture, newWidth, newHeight);
        }

        /// <summary>
        /// This method can copy textures marked as not readable in the import settings.
        /// </summary>
        public static Texture2D CopyNotReadableTex(Texture2D sourceTexture)
        {
            RenderTexture tmp = RenderTexture.GetTemporary(sourceTexture.width, sourceTexture.height, 0,
                    RenderTextureFormat.Default, RenderTextureReadWrite.Linear);
            Graphics.Blit(sourceTexture, tmp);

            RenderTexture previous = RenderTexture.active;
            RenderTexture.active = tmp;

            Texture2D texCopy = new Texture2D(sourceTexture.width, sourceTexture.height);
            texCopy.ReadPixels(new Rect(0, 0, tmp.width, tmp.height), 0, 0);
            texCopy.Apply();

            RenderTexture.active = previous;
            RenderTexture.ReleaseTemporary(tmp);

            return texCopy;
        }

        /// <summary>
        /// Checks if the texture has the same size and format.
        /// </summary>
        public static bool CompareSizeAndFormat(Texture2D src, int width, int height, TextureFormat format)
        {
            return width != src.width || height != src.height || format != src.format;
        }
    }
}