using System.Collections.Generic;

namespace Anarchy.Commands.Chat
{
    internal class ChatCommandHandler
    {
        private Dictionary<string, ChatCommand> allCommands;

        public ChatCommandHandler()
        {
            allCommands = new Dictionary<string, ChatCommand>();
            ChatCommand.LoadLocale();
            InitCommands();
        }

        private void InitCommands()
        {
            allCommands.Clear();
            allCommands.Add("restart", new RestartCommand());
            allCommands.Add("pm", new PMCommand());
            allCommands.Add("kick", new KickCommand(false));
            allCommands.Add("ban", new KickCommand(true));
            allCommands.Add("room", new RoomSettingCommands());
            allCommands.Add("pause", new PauseCommand(true));
            allCommands.Add("unpause", new PauseCommand(false));
            allCommands.Add("light", new DaylightChangeCommand());
            allCommands.Add("resetkd", new ResetKDCommand());
            allCommands.Add("revive", new ReviveCommand());
            allCommands.Add("spectate", new SpectateCommand());
            allCommands.Add("leave", new LeaveCommand());
            allCommands.Add("asoracing", new ASORacingCommand());
        }

        private void NotFound(string name)
        {
            string message = User.FormatColors(ChatCommand.Lang.Format("cmdNotFound", name));
            if (AnarchyManager.Log.Active)
            {
                UI.Log.AddLineRaw(message, UI.MsgType.Error);
            }
            else
            {
                UI.Chat.Add(message);
            }
        }

        public void TryHandleCommand(ICommand cmd, string inputLine)
        {
            if(cmd.Execute(inputLine.Split(' ')))
            {
                cmd.OnSuccess();
            }
            else
            {
                cmd.OnFail();
            }
        }

        public void TryHandle(string inputLine)
        {
            string[] strArray = inputLine.Substring(1).Split(' ');
            if(!allCommands.TryGetValue(strArray[0].ToLower(), out ChatCommand cmd))
            {
                NotFound(strArray[0].ToLower());
                return;
            }
            if(cmd.ReqiresMC && !PhotonNetwork.IsMasterClient)
            {
                UI.Chat.Add(ChatCommand.Lang["errMC"]);
                return;
            }
            string[] args = new string[strArray.Length - 1];
            for(int i = 1; i < strArray.Length; i++)
            {
                args[i - 1] = strArray[i];
            }
            if (cmd.Execute(args))
            {
                cmd.OnSuccess();
            }
            else
            {
                cmd.OnFail();
            }
            cmd.OnFinalize();
        }
    }
}
