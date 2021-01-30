using ExitGames.Client.Photon;

namespace Anarchy.Commands.Chat
{
    internal class ReviveCommand : ChatCommand
    {
        private static readonly Hashtable HashRevive = new Hashtable() { [(byte)0] = 2, [(byte)2] = PhotonNetwork.networkingPeer.ServerTimeInMilliSeconds, [(byte)3] = "respawnHeroInNewRound" };

        public ReviveCommand() : base("revive", true, true, false)
        {
        }

         public override bool Execute(string[] args)
        {
            if(args.Length <= 0)
            {
                logMessage = Lang["revived"];
                PhotonNetwork.networkingPeer.OpRaiseEvent(200, HashRevive, true, new RaiseEventOptions { TargetActors = new int[] { PhotonNetwork.player.ID } });
                return true;
            }
            if (args[0].ToLower() == "all")
            {
                FengGameManagerMKII.FGM.BasePV.RPC("respawnHeroInNewRound", PhotonTargets.All, new object[0]);
                SendLocalizedText("revivedAll", null);
                chatMessage = Lang["revivedAll"];
                return true;
            }
            HashRevive[(byte)2] = PhotonNetwork.networkingPeer.ServerTimeInMilliSeconds;
            int[] IDs = new int[args.Length];
            for (int i = 0; i < args.Length; i++)
            {
                int.TryParse(args[i], out IDs[i]);
            }
            for (int i = 0; i < IDs.Length; i++)
            {
                PhotonPlayer target = PhotonPlayer.Find(IDs[i]);
                if (target == null)
                {
                    continue;
                }
                PhotonNetwork.networkingPeer.OpRaiseEvent(200, HashRevive, true, new RaiseEventOptions { TargetActors = new int[] { target.ID } });

                SendLocalizedText(target, "revived", null);
                if(chatMessage.Length > 0)
                {
                    chatMessage += "\n";
                }
                chatMessage += Lang.Format("playerRevived", target.ID.ToString());
            }
            return true;
        }
    }
}
