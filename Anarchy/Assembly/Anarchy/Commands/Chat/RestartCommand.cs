namespace Anarchy.Commands.Chat
{
    internal class RestartCommand : ChatCommand
    {
        public RestartCommand() : base("restart", true, true, false)
        {
        }

        public override bool Execute(string[] args)
        {
            if (args.Length > 0 && args[0] == "-r")
            {
                foreach (PhotonPlayer player in PhotonNetwork.playerList)
                {
                    player.SetCustomProperties(ResetKDCommand.ResetHash);
                }
            }
            FengGameManagerMKII.FGM.RestartGame(false, true);
            chatMessage = Lang["restartMessage"];
            SendLocalizedText("restartMessage", null);
            return true;
        }
    }
}