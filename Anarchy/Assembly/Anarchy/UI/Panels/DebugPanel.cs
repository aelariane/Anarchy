using System;
using System.Collections.Generic;
using System.Text;
using Anarchy.Configuration;
using Optimization;
using UnityEngine;
using GUI = Anarchy.UI.GUI;
using GUILayout = Anarchy.UI.GUILayout;
using static Anarchy.UI.GUI;


namespace Anarchy.UI
{
    public class DebugPanel : GUIBase
    {
        private const int Debug = 0;
        private const int Terminal = 1;
        private const int Players = 3;
        private const int Traffic = 2;

        internal static IntSetting DebugLevel = new IntSetting(nameof(DebugLevel), 0);
        internal static IntSetting LogLevel = new IntSetting(nameof(LogLevel), 0);
        internal static IntSetting LogMessagesCount = new IntSetting(nameof(LogMessagesCount), 50);

        private static readonly Dictionary<LogType, string> TypeColors = new Dictionary<LogType, string>
        {
            { LogType.Assert, "<color=white>" },
            { LogType.Error, "<color=red>" },
            { LogType.Exception, "<color=red>" },
            { LogType.Log, "<color=white>" },
            { LogType.Warning, "<color=yellow>" }
        };

        private List<LogMessage> messages;
        private SmartRect rect;

        private Vector2 scroll;
        private Rect scrollArea;
        private Rect scrollAreaView;
        private SmartRect scrollRect;

        public DebugPanel() : base(nameof(DebugPanel), 369)
        {
            animator = new Animation.CenterAnimation(this, Helper.GetScreenMiddle(Style.WindowWidth, Style.WindowHeight));
            messages = new List<LogMessage>();
            Application.RegisterLogCallback(Log);
        }

        public void Log(string message, string trace, LogType type)
        {

            messages.Add(new LogMessage(message, trace, type));
        }

        protected override void OnEnable()
        {
            //rect = Helper.GetSmartRects(BoxPosition, 1)[0];
            //scroll = Optimization.Caching.Vectors.v2zero;
            //scrollRect = new SmartRect(0f, 0f, rect.width, rect.height, 0f, Style.VerticalMargin);
            //scrollArea = new Rect(rect.x, rect.y, rect.width, BoxPosition.height - (4 * (Style.Height + Style.VerticalMargin)) - (Style.WindowTopOffset + Style.WindowBottomOffset) - 10f);
            //scrollAreaView = new Rect(0f, 0f, rect.width, 5000f);
            //head = locale["title"];
        }

        [GUIPage(Debug)]
        private void DebugPage()
        {
            rect.Reset();
            rect.MoveY(Style.VerticalMargin);
            scrollRect.Reset();
            scrollArea.y = rect.y;
            scroll = BeginScrollView(scrollArea, scroll, scrollAreaView);
            foreach (var msg in messages)
            {
                GUILayout.Label(msg.ToString(), new GUILayoutOption[0]);
            }
            EndScrollView();
        }

        protected internal override void Draw()
        {
        }

        //protected override void DrawMainPart()
        //{
        //    DrawLowerButtons();
        //}

        protected override void OnDisable()
        {
        }

        private void DrawLowerButtons()
        {
            //rect.MoveToEndY(BoxPosition, Style.Height);
            //rect.MoveToEndX(BoxPosition, Style.LabelOffset);
            //rect.width = Style.LabelOffset;
            //if (Button(rect, locale["btnClose"]))
            //{
            //    Disable();
            //    return;
            //}
        }

        public override void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Disable();
                return;
            }
        }

        public override void OnUpdateScaling()
        {
            animator = new Animation.CenterAnimation(this, Helper.GetScreenMiddle(Style.WindowWidth, Style.WindowHeight));
        }

        internal class LogMessage
        {
            internal string Content;
            internal DateTime TimeStamp;
            internal LogType Type;
            internal string Stacktrace;

            internal LogMessage(string content, string trace, LogType type)
            {
                TimeStamp = DateTime.Now;
                Type = type;
                Content = content;
                Stacktrace = trace;
            }

            public override string ToString()
            {
                return $"<color=orange>[{TimeStamp.ToLongTimeString()}]</color> {TypeColors[Type]}{Content}</color> {(Stacktrace == string.Empty ? string.Empty : $"\n<color=lightblue>Stacktrace: {Stacktrace}</color>")}";
            }
        }
    }
}