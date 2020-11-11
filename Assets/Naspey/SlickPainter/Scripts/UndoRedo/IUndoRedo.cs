namespace Naspey.SlickPainter.UndoRedo
{
    public interface IUndoRedo
    {
        void RegisterState();
        void Undo();
        void Redo();
        void ClearHistory();
    }
}