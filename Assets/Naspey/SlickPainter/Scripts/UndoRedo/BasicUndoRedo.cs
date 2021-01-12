using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Naspey.SlickPainter.UndoRedo
{
    public class BasicUndoRedo : IUndoRedo
    {
        public SlickPainter Painter { get; }

        public int MaxHistoryDepth { get; set; }

        private List<Texture2D> _undoTextures;
        private List<Texture2D> _redoTextures;

        public BasicUndoRedo(SlickPainter painter, int maxHistoryDepth = 10)
        {
            Painter = painter;
            MaxHistoryDepth = maxHistoryDepth;

            _undoTextures = new List<Texture2D>(MaxHistoryDepth);
            _redoTextures = new List<Texture2D>(MaxHistoryDepth);
        }

        public void RegisterState()
        {
            if (Painter == null)
                return;

            _undoTextures.Insert(0, TextureUtilities.CopyTexture(Painter.CanvasTexture));

            _redoTextures.DestroyAll();
            _redoTextures.Clear();

            CheckForMaxDepthExceed();
        }

        public void Undo() => UpdateState(_undoTextures, _redoTextures);

        public void Redo() => UpdateState(_redoTextures, _undoTextures);

        public void ClearHistory()
        {
            _undoTextures.DestroyAll();
            _undoTextures.Clear();

            _redoTextures.DestroyAll();
            _redoTextures.Clear();
        }

        private void UpdateState(List<Texture2D> source, List<Texture2D> copyHolder)
        {
            if (source.Count == 0)
                return;

            copyHolder.Insert(0, TextureUtilities.CopyTexture(Painter.CanvasTexture));
            TextureUtilities.CopyTexture(source.First(), Painter.CanvasTexture);

            source.DestroyAndRemove(0);

            CheckForMaxDepthExceed();
        }

        private void CheckForMaxDepthExceed()
        {
            _undoTextures.DestroyAndRemoveRange(MaxHistoryDepth, _undoTextures.Count - MaxHistoryDepth);
            _redoTextures.DestroyAndRemoveRange(MaxHistoryDepth, _redoTextures.Count - MaxHistoryDepth);
        }
    }
}