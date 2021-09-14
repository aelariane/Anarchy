namespace Anarchy.Commands.Chat
{
    internal class ClearCommand : ChatCommand
    {
        public ClearCommand() : base("clear", false, false, false)
        {
        }

        public override bool Execute(string[] args)
        {
            if (args.Length == 0)
            {
                UI.Chat.Clear();
            }
            else if (args[0] == "-c")
            {
                UI.Log.Clear();
            }
            else if(args[0] == "-h")
            {
                AnarchyManager.ChatHistory.ClearMessages();
            }
            return true;
        }
    }
}