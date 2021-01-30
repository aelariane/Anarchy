using ExitGames.Client.Photon;

namespace Anarchy.Commands.Chat
{
    internal class ResetKDCommand : ChatCommand
    {
        internal static readonly Hashtable ResetHash = new Hashtable() { { PhotonPlayerProperty.kills, 0 }, { PhotonPlayerProperty.max_dmg, 0 }, { PhotonPlayerProperty.total_dmg, 0 }, { PhotonPlayerProperty.deaths, 0 } };

        public ResetKDCommand() : base("resetkd", false, true, false)
        {
        }

        public override bool Execute(string[] args)
        {
            if (args.Length <= 0)
            {
                ResetProps(PhotonNetwork.player);
                chatMessage = Lang["resetkdLocal"];
                return true;
            }
            if (!PhotonNetwork.IsMasterClient)
            {
                chatMessage = Lang["errMC"];
                return false;
            }
            if (args[0] == "all")
            {
                foreach (PhotonPlayer player in PhotonNetwork.playerList)
                {
                    ResetProps(player);
                }
                chatMessage = Lang["resetkdAll"];
                if (PhotonNetwork.IsMasterClient)
                {
                    SendLocalizedText("resetkdAll", null);
                }
                return true;
            }
            int[] IDs = new int[args.Length];
            for (int i = 0; i < args.Length; i++)
            {
                int.TryParse(args[i], out IDs[i]);
            }
            for (int i = 0; i < IDs.Length; i++)
            {
                PhotonPlayer player = PhotonPlayer.Find(IDs[i]);
                if (player != null)
                {
                    ResetProps(player);
                    if (chatMessage != string.Empty)
                    {
                        chatMessage += "\n";
                    }
                    chatMessage += Lang.Format("resetkdPlayer", player.ID.ToString(), player.UIName.ToHTMLFormat());
                }
            }
            return true;
        }

        private void ResetProps(PhotonPlayer target)
        {
            if (target != null)
            {
                target.SetCustomProperties(ResetHash);
            }
        }
    }
}