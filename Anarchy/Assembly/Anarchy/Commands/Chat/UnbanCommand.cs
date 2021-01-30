namespace Anarchy.Commands.Chat
{
    internal class UnbanCommand : ChatCommand
    {
        public UnbanCommand() : base("unban", true, true, false)
        {
        }

        public override bool Execute(string[] args)
        {
            if (args.Length < 1)
            {
                chatMessage = Lang["errArg"];
                return false;
            }
            bool unban;
            if (int.TryParse(args[0], out int ID))
            {
                unban = Network.BanList.Unban(ID);
            }
            else
            {
                unban = Network.BanList.Unban(args[0]);
            }
            chatMessage = Lang["unban" + (unban ? "Succeed" : "Failed")];
            return true;
        }
    }
}