namespace Anarchy.Commands
{
    internal interface ICommand
    {
        string CommandName { get; }

        bool Execute(string[] args);

        void OnFail();

        void OnSuccess();
    }
}
