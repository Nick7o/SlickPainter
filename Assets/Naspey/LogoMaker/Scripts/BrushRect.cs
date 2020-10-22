using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Naspey.LogoMaker
{
    /// <summary>
    /// Represent brush position and offset relative to the canvas.
    /// </summary>
    public struct BrushRect
    {
        /// <summary>
        /// Brush coordinates relative to canvas. Can be negative. For finding starting position use X or Y properties.
        /// </summary>
        public int CanvasStartPixelX;

        /// <summary>
        /// Brush coordinates relative to canvas. Can be negative. For finding starting position use X or Y properties.
        /// </summary>
        public int CanvasStartPixelY;

        public int Width;
        public int Height;
        
        /// <summary>
        /// Defines the starting pixel of brush on X axis. Clips negative values.
        /// </summary>
        public int X => Mathf.Max(CanvasStartPixelX, 0);

        /// <summary>
        /// Defines the starting pixel of brush on Y axis. Clips negative values.
        /// </summary>
        public int Y => Mathf.Max(CanvasStartPixelY, 0);

        /// <summary>
        /// If canvas start pixel is negative (outside of canvas) then we need to clip that amount of pixels from brush texture. This property provides that number.
        /// </summary>
        public int BrushOffsetX => -Mathf.Min(CanvasStartPixelX, 0);

        /// <summary>
        /// If canvas start pixel is negative (outside of canvas) then we need to clip that amount of pixels from brush texture. This property provides that number.
        /// </summary>
        public int BrushOffsetY => -Mathf.Min(CanvasStartPixelY, 0);

        public BrushRect(int canvasStartPixelX, int canvasStartPixelY, int width, int height)
        {
            CanvasStartPixelX = canvasStartPixelX;
            CanvasStartPixelY = canvasStartPixelY;
            Width = width;
            Height = height;
        }
    }
}