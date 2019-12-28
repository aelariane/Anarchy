namespace Anarchy.Commands.Chat
{
    internal class RestartCommand : ChatCommand
    {
        public RestartCommand() : base("restart", true, true, false)
        {

        }

        public override bool Execute(string[] args)
        {
            FengGameManagerMKII.FGM.RestartGame();
            return false;
        }
    }
}
