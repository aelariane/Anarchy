namespace Anarchy.Commands.Chat
{
    internal class MuteCommand : ChatCommand
    {
        private readonly bool value;
        private readonly string key;

        public MuteCommand(bool val) : base(val ? "mute" : "unmute", false, true, false)
        {
            value = val;
            key = val ? "mute" : "unmute";
        }

        public override bool Execute(string[] args)
        {
            if (args.Length == 0)
            {
                chatMessage = Lang["errArg"];
                return false;
            }
            for (int i = 0; i < args.Length; i++)
            {
                if (int.TryParse(args[i], out int ID))
                {
                    PhotonPlayer target = PhotonPlayer.Find(ID);
                    if (target == null)
                    {
                        chatMessage = chatMessage.AppendLine(Lang.Format("errPlayer", ID.ToString()));
                        continue;
                    }
                    logMessage = logMessage.AppendLine(Lang.Format(key + (target.Muted == value ? "Failed" : "Succeed"), ID.ToString(), target.UIName.ToHTMLFormat()));
                    target.Muted = value;
                    FengGameManagerMKII.FGM.PlayerList.Update();
                }
            }
            return true;
        }
    }
}