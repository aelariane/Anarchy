using System;
using System.Collections.Generic;
using System.Linq;
using Anarchy;
using Anarchy.Configuration;
using Optimization;
using UnityEngine;

namespace Anarchy.UI
{

    public class Log : GUIBase
    {
        internal static FloatSetting BackgroundTransparency = new FloatSetting("LogBackgroundTransparency", 0.15f);
        internal static IntSetting MessageCount = new IntSetting("LogMessageCount", 10);
        internal static BoolSetting UseBackground = new BoolSetting("UseLogBackround", false);
        internal static IntSetting FontSize = new IntSetting("LogFontSize", 15);
        internal static IntSetting LogWidth = new IntSetting("LogWidth", 330);
        internal static BoolSetting UseCustomLogSpace = new BoolSetting("UseCustomLogSpace", false);
        internal static IntSetting CustomLogSpaceUp = new IntSetting("CustomLogSpaceUp", 0);
        internal static IntSetting CustomLogSpaceDown = new IntSetting("CustomLogSpaceDown", 0);
        internal static IntSetting CustomLogSpaceLeft = new IntSetting("CustomLogSpaceLeft", 0);
        internal static IntSetting CustomLogSpaceRight = new IntSetting("CustomLogSpaceRight", 0);

        private static readonly Dictionary<MsgType, string> TypeDict = new Dictionary<MsgType, string>()
        {
            { MsgType.Ban, "<color=magenta>[>]</color> " },
            { MsgType.Error, "<color=red>[-]</color> " },
            { MsgType.Info, "<color=#CCCCDD>[?]</color> " },
            { MsgType.None, "" },
            { MsgType.Warning, "<color=#FFCC00>[!]</color> " }
        };

        internal static Log Instance;

        private GUILayoutOption labelOptions;
        private GUIStyle LogStyle;
        private OrderedDictionary<string, int> messages;
        private Rect position;


        internal Log() : base("Log", 4)
        {
            Instance = this;
            messages = new OrderedDictionary<string, int>();
            locale.Formateable = true;
        }

        private static void AddLine(string message)
        {
            if (Instance == null || !Instance.Active)
            {
                return;
            }
            OrderedDictionary<string, int> messages = Instance.messages;
            if (messages == null)
            {
                return;
            }
            if (!messages.ContainsKey(message))
            {
                if (messages.Count > MessageCount.Value)
                {
                    messages.Remove(messages.Keys.First());
                }
                messages.Add(message, 1);
            }
            else
            {
                messages[message]++;
            }
        }

        public static void AddLine(string key, MsgType type = MsgType.None)
        {
            if (Instance == null || !Instance.Active)
            {
                return;
            }
            AddLineRaw(Instance.locale[key], type);
        }

        public static void AddLine(string key, params string[] values)
        {
            if (Instance == null || !Instance.Active)
            {
                return;
            }
            AddLineRaw(Instance.locale.Format(key, values), MsgType.None);
        }

        public static void AddLine(string key, MsgType type, params string[] values)
        {
            if(Instance == null || !Instance.Active)
            {
                return;
            }
            AddLineRaw(Instance.locale.Format(key, values), type);
        }

        public static void AddLineRaw(string message, MsgType type = MsgType.None)
        {
            if (Instance == null || !Instance.Active)
            {
                return;
            }
            AddLine(TypeDict[type] + message);
        }

        internal static void Clear()
        {
            if (Instance == null)
            {
                return;
            }
            Instance.messages.Clear();
        }

        protected override void OnEnable()
        {
            locale.FormatColors();
            var cache = LogWidth * (Style.WindowWidth / 700f);
            position = new Rect(Style.ScreenWidth - cache, Style.ScreenHeight - 500, cache, 470f);
            LogStyle = new GUIStyle(Style.Label);
            if (UseBackground)
            {
                Texture2D black = new Texture2D(1, 1, TextureFormat.ARGB32, false);
                black.SetPixel(0, 0, new Color(0f, 0f, 0f, BackgroundTransparency.Value));
                black.Apply();
                LogStyle.normal.background = black;
            }
            LogStyle.fontSize = FontSize;
            if (UseCustomLogSpace.Value)
            {
                LogStyle.padding = new RectOffset(CustomLogSpaceLeft, CustomLogSpaceRight, CustomLogSpaceUp, CustomLogSpaceDown);
            }
            else
            {
                LogStyle.padding = new RectOffset(0, 0, 0, 0);
            }
            LogStyle.border = new RectOffset(0, 0, 0, 0);
            LogStyle.margin = new RectOffset(0, 0, 0, 0);
            LogStyle.overflow = new RectOffset(0, 0, 0, 0);
            labelOptions = UnityEngine.GUILayout.Width(position.width);
        }

        protected internal override void Draw()
        {
            List<string> message = messages.Keys.ToList();
            List<int> count = messages.Values.ToList();
            GUILayout.BeginArea(position);
            UnityEngine.GUILayout.FlexibleSpace();
            try
            {
                for (int i = 0; i < messages.Count; i++)
                {
                    string currentMessage = message[i];
                    int currentCount = count[i];
                    if (!currentMessage.IsNullOrEmpty())
                    {
                        UnityEngine.GUILayout.Label(currentMessage + (currentCount > 1 ? $" <color=red>[x{currentCount}]</color>" : string.Empty), LogStyle, labelOptions);
                    }
                }
            }
            catch
            { 
            }
            GUILayout.EndArea();
        }

        public static string GetString(string key)
        {
            if(Instance != null)
            {
                return Instance.locale[key];
            }
            return "";
        }

        public static string GetString(string key, params string[] args)
        {
            if (Instance != null)
            {
                return Instance.locale.Format(key, args);
            }
            return "";
        }

        protected override void OnDisable()
        {
            //messages.Clear();
        }
    }

    public enum MsgType : byte
    {
        None = 0,
        Ban = 1,
        Info = 2,
        Warning = 3,
        Error = 4
    }
}
