namespace Anarchy.Commands.Chat
{
    internal class RestartCommand : ChatCommand
    {
        public RestartCommand() : base("restart", true, true, false)
        {
        }

        public override bool Execute(string[] args)
        {
            FengGameManagerMKII.FGM.RestartGame(false, true);
            chatMessage = Lang["restartMessage"];
            SendLocalizedText("restartMessage", null);
            return true;
        }
    }
}
