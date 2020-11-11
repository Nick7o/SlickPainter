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

        private List<Texture2D> undoTextures { get; set; }
        private List<Texture2D> redoTextures { get; set; }

        public BasicUndoRedo(SlickPainter painter, int maxHistoryDepth = 10)
        {
            Painter = painter;
            MaxHistoryDepth = maxHistoryDepth;

            undoTextures = new List<Texture2D>(MaxHistoryDepth);
            redoTextures = new List<Texture2D>(MaxHistoryDepth);
        }

        public void RegisterState()
        {
            if (Painter == null)
                return;

            undoTextures.Insert(0, TextureUtilities.CopyTexture(Painter.Result));

            redoTextures.DestroyAll();
            redoTextures.Clear();

            CheckForMaxDepthExceed();
        }

        public void Undo() => UpdateState(undoTextures, redoTextures);

        public void Redo() => UpdateState(redoTextures, undoTextures);

        public void ClearHistory()
        {
            undoTextures.DestroyAll();
            undoTextures.Clear();

            redoTextures.DestroyAll();
            redoTextures.Clear();
        }

        private void UpdateState(List<Texture2D> source, List<Texture2D> copyHolder)
        {
            if (source.Count == 0)
                return;

            copyHolder.Insert(0, TextureUtilities.CopyTexture(Painter.Result));
            TextureUtilities.CopyTexture(source.First(), Painter.Result);

            source.DestroyAndRemove(0);

            CheckForMaxDepthExceed();
        }

        private void CheckForMaxDepthExceed()
        {
            undoTextures.DestroyAndRemoveRange(MaxHistoryDepth, undoTextures.Count - MaxHistoryDepth);
            redoTextures.DestroyAndRemoveRange(MaxHistoryDepth, redoTextures.Count - MaxHistoryDepth);
        }
    }
}