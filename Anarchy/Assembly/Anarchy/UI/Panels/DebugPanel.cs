using Optimization;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using static Anarchy.UI.GUI;

namespace Anarchy.UI
{
    public class DebugPanel : GUIPanel
    {
        private const int Debug = 0;
        private const int Terminal = 1;
        private const int Players = 3;
        private const int Traffic = 2;

        private static readonly Dictionary<LogType, string> TypeColors = new Dictionary<LogType, string>
        {
            { LogType.Assert, "<color=white>" },
            { LogType.Error, "<color=red>" },
            { LogType.Exception, "<color=red>" },
            { LogType.Log, "<color=white>" },
            { LogType.Warning, "<color=yellow>" }
        };

        private List<LogMsg> messages;
        private SmartRect rect;

        private Vector2 scroll;
        private Rect scrollArea;
        private Rect scrollAreaView;
        private SmartRect scrollRect;

        public DebugPanel() : base(nameof(DebugPanel), 369)
        {
            animator = new Animation.CenterAnimation(this, Helper.GetScreenMiddle(Style.WindowWidth, Style.WindowHeight));
            messages = new List<LogMsg>();
            Application.RegisterLogCallback(Log);
        }

        public void Log(string message, string trace, LogType type)
        {
            var msg = new LogMsg(message, trace, type);
            messages.Add(msg);
        }

        protected override void OnPanelEnable()
        {
            rect = Helper.GetSmartRects(BoxPosition, 1)[0];
            scroll = Optimization.Caching.Vectors.v2zero;
            scrollRect = new SmartRect(0f, 0f, rect.width, rect.height, 0f, Style.VerticalMargin);
            scrollArea = new Rect(rect.x, rect.y, rect.width, BoxPosition.height - (4 * (Style.Height + Style.VerticalMargin)) - (Style.WindowTopOffset + Style.WindowBottomOffset) - 10f);
            scrollAreaView = new Rect(0f, 0f, rect.width, 1000f);
            head = locale["title"];
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
                Label(scrollRect, $"{msg.time} {msg.Content}: {msg.Stacktrace}", true);
            }
            EndScrollView();
        }

        protected override void DrawMainPart()
        {
            DrawLowerButtons();
        }

        protected override void OnPanelDisable()
        {
        }

        private void DrawLowerButtons()
        {
            rect.MoveToEndY(BoxPosition, Style.Height);
            rect.MoveToEndX(BoxPosition, Style.LabelOffset);
            rect.width = Style.LabelOffset;
            if (Button(rect, locale["btnClose"]))
            {
                Disable();
                return;
            }
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

        private struct LogMsg
        {
            internal string Content;
            internal string time;
            internal string Stacktrace;

            internal LogMsg(string content, string trace, LogType type)
            {
                time = "<color=orange>[" + DateTime.Now.ToLongTimeString() + "]</color>";
                Content = TypeColors[type] + content + "</color>";
                Stacktrace = trace == "" ? "" : "<color=lightblue>" + trace + "</color>";
            }

            public override string ToString()
            {
                return time + " " + Content + (Stacktrace == "" ? "" : "\nStackTrace: " + Stacktrace);
            }

        }
    }
}