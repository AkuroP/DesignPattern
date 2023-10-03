namespace Game.Script
{
    public interface ICommand
    {
        void Do();

        void Undo();
    }
}
