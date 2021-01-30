namespace Anarchy.Commands.Chat
{
    internal class LeaveCommand : ChatCommand
    {
        public LeaveCommand() : base("leave", false, false, false)
        {
        }

        public override bool Execute(string[] args)
        {
            if (args.Length <= 0)
            {
                PhotonNetwork.LeaveRoom();
                return true;
            }
            string message = string.Join(" ", args, 0, args.Length);
            FengGameManagerMKII.FGM.BasePV.RPC("Chat", PhotonTargets.All, new object[] { User.Chat(PhotonNetwork.player.ID, message), "" });
            PhotonNetwork.LeaveRoom();
            return true;
        }
    }
}