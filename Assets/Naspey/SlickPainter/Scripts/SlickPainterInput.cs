using UnityEngine;

namespace Naspey.SlickPainter
{
    /// <summary>
    /// Describes SlickPainter's input.
    /// </summary>
    [System.Serializable]
    public class SlickPainterInput
    {
        // Key mapping for undo/redo
        public bool UseUndoRedoCommonKey => UndoRedoCommonKey != KeyCode.None;
        public KeyCode UndoRedoCommonKey = KeyCode.LeftControl;
        public KeyCode UndoKey = KeyCode.Z;
        public KeyCode RedoKey = KeyCode.Y;
    }
}