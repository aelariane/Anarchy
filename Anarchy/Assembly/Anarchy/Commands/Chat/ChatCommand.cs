namespace Anarchy.Commands.Chat
{
    /// <summary>
    /// Base class for Chat commands
    /// </summary>
    public abstract class ChatCommand : ICommand
    {
        /// <summary>
        /// Message that will be printed in <seealso cref="UI.Chat"/> after command executed
        /// </summary>
        protected string chatMessage = string.Empty;
        /// <summary>
        /// Message that will left in <seealso cref="UI.Log"/> after command excuted
        /// </summary>
        protected string logMessage = string.Empty;
        protected bool useChat = true;
        protected bool useLog = false;

        /// <summary>
        /// If command requires player to be <seealso cref="PhotonNetwork.masterClient"/> to execute command
        /// </summary>
        public bool RequireMC { get; private set; }

        public static Localization.Locale English { get; private set; }

        public static Localization.Locale Lang { get; private set; }

        /// <summary>
        /// Name of command
        /// </summary>
        public string CommandName { get; }

        protected ChatCommand(string key, bool needmc) : this(key, needmc, true, false)
        {
        }

        protected ChatCommand(string key, bool needmc, bool chat, bool log)
        {
            CommandName = key;
            useChat = chat;
            useLog = log;
            RequireMC = needmc;
        }

        /// <summary>
        /// Executes command
        /// </summary>
        /// <param name="args">Provided arguments</param>
        /// <returns>If command was executed successfully</returns>
        public abstract bool Execute(string[] args);

        /// <summary>
        /// Loads language files
        /// </summary>
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

        /// <summary>
        /// Prints <seealso cref="chatMessage"/> and <seealso cref="logMessage"/>
        /// </summary>
        protected void PrintMessage()
        {
            if (useChat && chatMessage != string.Empty)
            {
                UI.Chat.Add(chatMessage);
            }
            if (useLog && AnarchyManager.Log.IsActive && logMessage != string.Empty)
            {
                UI.Log.AddLineRaw(logMessage);
            }
        }

        /// <summary>
        /// Calls if <seealso cref="Execute(string[])"/> failed
        /// </summary>
        public virtual void OnFail()
        {
            PrintMessage();
        }

        /// <summary>
        /// Always after <seealso cref="Execute(string[])"/>
        /// </summary>
        public virtual void OnFinalize()
        {
            ResetMessages();
        }
        
        /// <summary>
        /// Calls if <seealso cref="Execute(string[])"/> finished successfully
        /// </summary>
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

        /// <summary>
        /// Sends localized text in Chat
        /// </summary>
        /// <param name="key"></param>
        /// <param name="args"></param>
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