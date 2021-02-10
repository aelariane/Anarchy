namespace Anarchy.Commands
{
    /// <summary>
    /// Interface that represents any type of command that can be executed
    /// </summary>
    internal interface ICommand
    {
        /// <summary>
        /// Name of <see cref="ICommand"/>
        /// </summary>
        string CommandName { get; }
        /// <summary>
        /// Executes <see cref="ICommand"/> with given arguments
        /// </summary>
        /// <param name="args">Arguments</param>
        /// <returns><seealso cref="true"/> if execution completed successfully. <seealso cref="false"/> otherwise.</returns>
        bool Execute(string[] args);
        /// <summary>
        /// Calls if <see cref="Execute(string[])"/> was not successful
        /// </summary>
        void OnFail();
        /// <summary>
        /// Calls if <see cref="Execute(string[])"/> was successful
        /// </summary>
        void OnSuccess();
    }
}