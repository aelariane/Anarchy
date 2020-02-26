using System;

namespace Anarchy.Commands.Chat
{
    internal abstract class ChatCommand : ICommand
    {
        protected string chatMessage = string.Empty;
        protected string logMessage = string.Empty;
        protected bool useChat = true;
        protected bool useLog = false;
        public bool RequireMC { get; private set; }

        public static Localization.Locale English;
        public static Localization.Locale Lang;

        public string CommandName { get; }

        protected ChatCommand(string key, bool needmc) : this(key, needmc, true, false)
        {
        }

        protected ChatCommand(string key, bool needmc, bool chat,  bool log)
        {
            CommandName = key;
            useChat = chat;
            useLog = log;
            RequireMC = needmc;
        }

        public abstract bool Execute(string[] args);

        public static void LoadLocale()
        {
            English = new Localization.Locale(Localization.Language.DefaultLanguage, "ChatCommands", true, ',');
            English.Reload();
            English.FormatColors();
            Lang = new Localization.Locale("ChatCommands");
            Lang.Formateable = true;
            Lang.Reload();
            Lang.FormatColors();
        }

        protected void PrintMessage()
        {
            if (useChat && chatMessage != string.Empty)
            {
                UI.Chat.Add(chatMessage);
            }
            if (useLog && AnarchyManager.Log.Active && logMessage != string.Empty)
            {
                UI.Log.AddLineRaw(logMessage);
            }
        }

        public virtual void OnFail()
        {
            PrintMessage();
        }

        public virtual void OnFinalize()
        {
            ResetMessages();
        }

        public virtual void OnSuccess()
        {
            PrintMessage();
        }

        private void ResetMessages()
        {
            if (chatMessage.Length > 0)
            {
                chatMessage = string.Empty;
            }
            if (logMessage.Length > 0)
            {
                logMessage = string.Empty;
            }
        }


        public static void SendLocalizedText(string key, string[] args)
        {
            UI.Chat.SendLocalizedText("ChatCommands", key, args);
        }

        public static void SendLocalizedText(PhotonPlayer target, string key, string[] args)
        {
            if (target != null)
            {
                UI.Chat.SendLocalizedText(target, "ChatCommands", key, args);
            }
        }
    }
}
