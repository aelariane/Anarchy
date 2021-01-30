namespace Anarchy.Commands
{
    /// <summary>
    /// Interface that represents any type of command that can be executed
    /// </summary>
    internal interface ICommand
    {
        string CommandName { get; }

        bool Execute(string[] args);

        void OnFail();

        void OnSuccess();
    }
}