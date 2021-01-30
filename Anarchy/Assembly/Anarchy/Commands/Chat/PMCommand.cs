namespace Anarchy.Commands.Chat
{
    internal class PMCommand : ChatCommand
    {
        private int pmID = -1;

        public PMCommand() : base("pm", false, true, false)
        {
        }

        public override bool Execute(string[] args)
        {
            if (args.Length <= 0)
            {
                chatMessage = Lang.Format("errArg", CommandName);
                return false;
            }
            if (args[0].ToLower() == "setid")
            {
                if (!int.TryParse(args[1], out int id))
                {
                    chatMessage = Lang.Format("errArg", CommandName);
                    return false;
                }
                pmID = id;
                chatMessage = Lang.Format("pmSetID", pmID.ToString());
                return true;
            }
            else if (!int.TryParse(args[0], out int ID))
            {
                if (pmID <= 0)
                {
                    chatMessage = Lang.Format("errArg", CommandName);
                    return false;
                }
                else if (PhotonPlayer.Find(pmID) == null)
                {
                    chatMessage = Lang.Format("errPM", pmID.ToString());
                    return false;
                }
                string message = string.Join(" ", args, 0, args.Length);
                chatMessage = string.Format(Lang["pmSent"], new object[] { pmID.ToString(), message });
                FengGameManagerMKII.FGM.BasePV.RPC("Chat", PhotonPlayer.Find(pmID), new object[] { User.ChatPmSend(PhotonNetwork.player.ID, message), "" });
            }
            else
            {
                if (PhotonPlayer.Find(ID) == null)
                {
                    chatMessage = Lang.Format("errPM", pmID.ToString());
                    return false;
                }
                string message = string.Join(" ", args, 1, args.Length - 1);
                chatMessage = string.Format(Lang["pmSent"], new object[] { ID.ToString(), message });
                FengGameManagerMKII.FGM.BasePV.RPC("Chat", PhotonPlayer.Find(ID), new object[] { User.ChatPmSend(PhotonNetwork.player.ID, message), "" });
            }
            return true;
        }
    }
}