using System;
using System.Collections.Generic;
using System.Linq;
using Anarchy;
using Anarchy.Configuration;
using Anarchy.InputPos;
using Optimization;
using UnityEngine;

namespace Anarchy.UI
{
    internal class Chat : GUIBase
    {
        public const string ChatRPC = "Chat";
        public const string ChatPMRPC = "ChatPM";

        internal static FloatSetting BackgroundTransparency = new FloatSetting("ChatBackgroundTransparency", 0.15f);
        internal static IntSetting MessageCount = new IntSetting("ChatMessageCount", 10);
        internal static BoolSetting UseBackground = new BoolSetting("UseChatBackround", false);
        internal static IntSetting FontSize = new IntSetting("ChatFontSize", 15);
        internal static IntSetting ChatWidth = new IntSetting("ChatWidth", 330);
        internal static BoolSetting UseCustomChatSpace = new BoolSetting("UseCustomChatSpace", false);
        internal static IntSetting CustomChatSpaceUp = new IntSetting("CustomChatSpaceUp", 0);
        internal static IntSetting CustomChatSpaceDown = new IntSetting("CustomChatSpaceDown", 0);
        internal static IntSetting CustomChatSpaceLeft = new IntSetting("CustomChatSpaceLeft", 0);
        internal static IntSetting CustomChatSpaceRight = new IntSetting("CustomChatSpaceRight", 0);

        internal static Chat Instance;

        private Rect position;
        private Rect inputPosition;

        private GUIStyle ChatStyle;
        private GUIStyle textFieldStyle;

        private GUILayoutOption[] textFieldOptions;
        private GUILayoutOption labelOptions;

        private List<string> messages;
        private string inputLine = string.Empty;

        internal Commands.Chat.ChatCommandHandler CMDHandler = new Commands.Chat.ChatCommandHandler();

        internal Chat() : base("Chat", 5)
        {
            Instance = this;
            messages = new List<string>();
        }

        internal static void Add(string message)
        {
            if (Instance == null)
            {
                return;
            }
            List<string> messages = Instance.messages;
            if (message == null)
            {
                return;
            }
            if (messages.Count > MessageCount.Value)
            {
                messages.Remove(messages.First());
            }
            messages.Add(message);
        }

        public static void Clear()
        {
            if(Instance != null && Instance.messages != null)
            {
                Instance.messages.Clear();
            }
        }

        private void ResetInputline()
        {
            inputLine = string.Empty;
            UnityEngine.GUI.FocusControl(string.Empty);
        }

        public static void Send(string message)
        {
            FengGameManagerMKII.FGM.BasePV.RPC(ChatRPC, PhotonTargets.All, new object[] { User.ChatSend(message), "" });
        }

        protected override void OnEnable()
        {
            position = new Rect(0f, Style.ScreenHeight - 500, ChatWidth * (Style.WindowWidth / 700f), 470f);
            inputPosition = new Rect(30f, Style.ScreenHeight - 300 + 275, 300f * Style.WindowWidth / 700f + (30f * (Style.WindowWidth / 700f) - 30f), 25f);
            ChatStyle = new GUIStyle(Style.Label);
            textFieldStyle = new GUIStyle(Style.TextField);
            textFieldStyle.fontSize = FontSize;
            if (UseBackground)
            {
                Texture2D black = new Texture2D(1, 1, TextureFormat.ARGB32, false);
                black.SetPixel(0, 0, new Color(0f, 0f, 0f, BackgroundTransparency.Value));
                black.Apply();
                ChatStyle.normal.background = black;
            }
            ChatStyle.fontSize = FontSize;
            if (UseCustomChatSpace.Value)
            {
                ChatStyle.padding = new RectOffset(CustomChatSpaceLeft, CustomChatSpaceRight, CustomChatSpaceUp, CustomChatSpaceDown);
            }
            else
            {
                ChatStyle.padding = new RectOffset(0, 0, 0, 0);
            }
            ChatStyle.border = new RectOffset(0, 0, 0, 0);
            ChatStyle.margin = new RectOffset(0, 0, 0, 0);
            ChatStyle.overflow = new RectOffset(0, 0, 0, 0);
            //ChatStyle.fixedWidth = 330f * (Style.WindowWidth / 700f);
            labelOptions = UnityEngine.GUILayout.MinWidth(position.width);
            textFieldOptions = new GUILayoutOption[] { UnityEngine.GUILayout.Width(inputPosition.width) };
        }

        protected internal override void Draw()
        {
            Event ev = Event.current;
            if (ev.type == EventType.KeyDown && (ev.keyCode == KeyCode.Tab || ev.character == '\t'))
            {
                ev.Use();
            }
            if (ev.type == EventType.KeyDown && (ev.keyCode == KeyCode.Return || ev.keyCode == KeyCode.KeypadEnter || (InputManager.RebindKeyCodes[(int)InputRebinds.OpenChat].Value != KeyCode.None && ev.keyCode == InputManager.RebindKeyCodes[(int)InputRebinds.OpenChat].Value)))
            {
                if (!inputLine.IsNullOrWhiteSpace())
                {
                    if (inputLine == "\t")
                    {
                        ResetInputline();
                        return;
                    }
                    if (inputLine.StartsWith("/"))
                    {
                        CMDHandler.TryHandle(inputLine);
                    }
                    else
                    {
                        Send(inputLine);
                    }
                    ResetInputline();
                    return;
                }
                else
                {
                    inputLine = "\t";
                    UnityEngine.GUI.FocusControl("ChatInput");
                }   
            }
            UnityEngine.GUI.SetNextControlName(string.Empty);
            GUILayout.BeginArea(position);
            UnityEngine.GUILayout.FlexibleSpace();
            try
            {
                for (int i = 0; i < messages.Count; i++)
                {
                    string currentMessage = messages[i];
                    if (!currentMessage.IsNullOrEmpty())
                    {
                        UnityEngine.GUILayout.Label(currentMessage, ChatStyle, labelOptions);
                    }
                }
            }
            catch
            {
            }
            GUILayout.EndArea();
            UnityEngine.GUI.SetNextControlName("ChatInput");
            GUILayout.BeginArea(inputPosition);
            inputLine = UnityEngine.GUILayout.TextField(inputLine, textFieldStyle, textFieldOptions);
            GUILayout.EndArea();
        }

        protected override void OnDisable()
        {
        }
    }
}
