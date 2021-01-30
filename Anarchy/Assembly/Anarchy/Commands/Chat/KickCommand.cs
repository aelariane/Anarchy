namespace Anarchy.Commands.Chat
{
    internal class KickCommand : ChatCommand
    {
        private bool ban;
        private bool super;

        public KickCommand(bool ban, bool superBan) : base(ban ? "ban" : "kick", true, true, false)
        {
            this.ban = ban;
            this.super = superBan;
        }

        public override bool Execute(string[] args)
        {
            if (args.Length <= 0)
            {
                chatMessage = Lang.Format("errArg", CommandName);
                return false;
            }
            int[] IDs = new int[args.Length];
            bool permaBan = false;
            for (int i = 0; i < args.Length; i++)
            {
                if (!int.TryParse(args[i], out IDs[i]))
                {
                    if (args[i] == "perma" || args[i] == "--perma" || args[i] == "-p")
                    {
                        permaBan = true;
                        IDs[i] = -128;
                    }
                }
            }
            string send = "";
            for (int i = 0; i < IDs.Length; i++)
            {
                PhotonPlayer target = PhotonPlayer.Find(IDs[i]);
                if (target == null)
                {
                    if (IDs[i] != -128)
                    {
                        if (chatMessage.Length > 0)
                        {
                            chatMessage += "\n";
                        }

                        chatMessage += Lang.Format("kickIgnore", IDs[i].ToString());
                    }
                    continue;
                }
                if (chatMessage.Length > 0)
                {
                    chatMessage += "\n";
                }

                if (send.Length > 0)
                {
                    send += "\n";
                }

                chatMessage += string.Format(Lang["kickSuccess"], new object[] { target.ID.ToString(), target.UIName.ToHTMLFormat() });
                SendLocalizedText("kickSuccess", new string[] { target.ID.ToString(), target.UIName.ToHTMLFormat() });
                if (ban)
                {
                    if (permaBan)
                    {
                        Network.BanList.PermaBan(target);
                    }
                    else
                    {
                        Network.BanList.Ban(target);
                    }
                }
                if (super)
                {
                    Network.Antis.SuperKick(target, ban, string.Empty);
                }
                else
                {
                    Network.Antis.Kick(target, ban, string.Empty);
                }
            }
            return true;
        }
    }
}