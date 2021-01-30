namespace Anarchy.Commands.Chat
{
    internal class KillCommand : ChatCommand
    {
        public KillCommand() : base("kill", true, true, false)
        {
        }

        public override bool Execute(string[] args)
        {
            if (args.Length <= 0)
            {
                chatMessage = Lang["errArg"];
                return false;
            }
            int ID;
            if (!int.TryParse(args[0], out ID))
            {
                chatMessage = Lang["errArg"];
                return false;
            }
            PhotonPlayer target = PhotonPlayer.Find(ID);
            if (target == null)
            {
                chatMessage = Lang["errArg"];
                return false;
            }
            string killer = args.Length > 1 ? args[1] : "Kill";
            return Abuse.Kill(target, killer);
        }
    }
}
