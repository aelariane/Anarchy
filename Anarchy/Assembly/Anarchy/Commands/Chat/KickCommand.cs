using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Anarchy.Commands.Chat
{
    internal class KickCommand : ChatCommand
    {
        private bool ban;

        public KickCommand(bool ban) : base(ban ? "ban" : "kick", true, true, false)
        {
            this.ban = ban;
        }

        public override bool Execute(string[] args)
        {
            if(args.Length <= 0)
            {
                chatMessage = Lang.Format("errArg", CommandName);
                return false;
            }
            int[] IDs = new int[args.Length];
            for(int i = 0; i < args.Length; i++)
            {
                int.TryParse(args[i], out IDs[i]);
            }
            string send = "";
            for(int i = 0; i < IDs.Length; i++)
            {
                PhotonPlayer target = PhotonPlayer.Find(IDs[i]);
                if(target == null)
                {
                    if (chatMessage.Length > 0)
                        chatMessage += "\n";
                    chatMessage += Lang.Format("kickIgnore", IDs[i].ToString());
                    continue;
                }
                if (chatMessage.Length > 0)
                    chatMessage += "\n";
                if (send.Length > 0)
                    send += "\n";
                chatMessage += string.Format(Lang["kickSuccess"], new object[] { target.ID.ToString(), target.UIName.ToHTMLFormat() });
                send += string.Format(English["kickSuccess"], new object[] { target.ID.ToString(), target.UIName.ToHTMLFormat() });
                PhotonNetwork.CloseConnection(target);
            }
            FengGameManagerMKII.FGM.BasePV.RPC("Chat", PhotonTargets.Others, new object[] { send, string.Empty });
            return true;
        }
    }
}
