using Anarchy.Configuration;
using ExitGames.Client.Photon;
using System;
using System.Collections.Generic;
using UnityEngine;
using static Anarchy.UI.GUI;

namespace Anarchy.UI
{
    public class DebugPanel : GUIPanel
    {
        private const int Debug = 0;
        private const int Terminal = 1;
        private const int Players = 2;
        private const int Traffic = 3;

        internal static IntSetting DebugLevel = new IntSetting(nameof(DebugLevel), 0);
        internal static IntSetting LogLevel = new IntSetting(nameof(LogLevel), 0);
        internal static IntSetting LogMessagesCount = new IntSetting(nameof(LogMessagesCount), 50);
        public static BoolSetting PlayersColor = new BoolSetting(nameof(PlayersColor), true);
        internal static StringSetting ColorKey = new StringSetting(nameof(ColorKey), "FFFF00");
        internal static StringSetting ColorValue = new StringSetting(nameof(ColorValue), "FF5900");
        internal static BoolSetting NetworkStats = new BoolSetting(nameof(NetworkStats), false);

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
        private SmartRect left;
        private SmartRect right;

        private Vector2 scroll;
        private Rect scrollArea;
        private Rect scrollAreaView;
        private SmartRect scrollRect;

        private Vector2 scrollName;
        private Rect scrollAreaName;
        private Rect scrollAreaViewName;
        private SmartRect scrollRectName;

        private Vector2 scrollProperties;
        private Rect scrollAreaProperties;
        private Rect scrollAreaViewProperties;
        private SmartRect scrollRectProperties;

        private Rect pagePosition;
        private string[] pagesSelection;

        private int selectedPlayer = 0;
        private GUIStyle scrollStyle;

        public DebugPanel() : base(nameof(DebugPanel), GUILayers.DebugPanel)
        {
            PhotonNetwork.networkingPeer.TrafficStatsEnabled = NetworkStats.Value;
            animator = new Animation.NoneAnimation(this);
            //animator = new Animation.CenterAnimation(this, Helper.GetScreenMiddle(Style.WindowWidth, Style.WindowHeight));
            messages = new List<LogMessage>();
            Application.RegisterLogCallback(Log);
        }

        public void Log(string message, string trace, LogType type)
        {
            messages.Add(new LogMessage(message, trace, type));
            //scrollAreaView.height = messages.Count * Style.Height + (messages.Count + 1) * Style.VerticalMargin;
        }

        [GUIPage(Debug)]
        private void DebugPage()
        {
            rect.Reset();
            rect.MoveY();
            scrollRect.Reset();
            scrollArea.y = rect.y;

            if (messages.Count > 0)
            {
                scroll = BeginScrollView(scrollArea, scroll, scrollAreaView);
                var options = new GUILayoutOption[0];
                foreach (var msg in messages)
                {
                    GUILayout.Label(msg.ToString(), options);
                }
                EndScrollView();
            }
        }

        [GUIPage(Players)]
        private void PlayerPage()
        {
            left.Reset();
            LabelCenter(left, locale["players"], true);
            scrollRectName.Reset();
            scrollAreaName.x = left.x;
            scrollAreaName.y = left.y;
            scrollName = BeginScrollView(scrollAreaName, scrollName, scrollAreaViewName);
            for (int i = 0; i < PhotonNetwork.playerList.Length; i++)
            {
                var player = PhotonNetwork.playerList[i];
                if (Button(left, player.UIName.ToHTMLFormat(), true))
                {
                    selectedPlayer = i;
                }
            }
            EndScrollView();

            right.Reset();
            LabelCenter(right, locale["properties"], true);
            scrollRectProperties.Reset();
            scrollAreaProperties.x = right.x;
            scrollAreaProperties.y = right.y;
            scrollProperties = BeginScrollView(scrollAreaProperties, scrollProperties, scrollAreaViewProperties);
            foreach (var test in PhotonNetwork.playerList[selectedPlayer].AllProperties)
            {
                string val = test.Value == null ? "<color=lightblue>Null</color>" : test.Value.ToString();
                if (PlayersColor)
                {
                    val = val.ToHTMLFormat();
                }
                GUILayout.Label($"<b><color=#{ColorKey}>{test.Key}</color>: <color=#{ColorValue}>{val}</color></b>", new GUILayoutOption[] { UnityEngine.GUILayout.MaxWidth(right.width) });
            }
            EndScrollView();
        }

        private void DrawSelectionButtons()
        {
            rect.Reset();
            pageSelection = SelectionGrid(rect, pageSelection, pagesSelection, pagesSelection.Length, true);
        }

        private void DrawLowerButtons()
        {
            rect.MoveToEndY(WindowPosition, Style.Height);
            if (pageSelection == Players)
            {
                rect.width = left.width;
                ToggleButton(rect, PlayersColor, "Use coloring:", false);
            }
            rect.MoveToEndX(WindowPosition, Style.LabelOffset);
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
            animator = new Animation.NoneAnimation(this);
            //animator = new Animation.CenterAnimation(this, Helper.GetScreenMiddle(Style.WindowWidth, Style.WindowHeight));
        }

        protected override void DrawMainPart()
        {
            DrawSelectionButtons();
            DrawLowerButtons();
        }

        protected override void OnPanelDisable()
        {
            Screen.lockCursor = Application.loadedLevelName == "menu" ? false : (IN_GAME_MAIN_CAMERA.CameraMode >= CameraType.TPS);
            Screen.showCursor = Application.loadedLevelName == "menu";
            if (!AnarchyManager.Pause.IsActive)
            {
                IN_GAME_MAIN_CAMERA.isPausing = false;
                InputManager.MenuOn = false;
            }
            if (IN_GAME_MAIN_CAMERA.GameType == GameType.Single)
            {
                Time.timeScale = 1f;
            }
            //Anarchy.Configuration.Settings.Apply();
        }

        protected override void OnPanelEnable()
        {
            if (IN_GAME_MAIN_CAMERA.GameType == GameType.Single)
            {
                Time.timeScale = 0f;
            }
            if (!AnarchyManager.Pause.IsActive)
            {
                IN_GAME_MAIN_CAMERA.isPausing = true;
                InputManager.MenuOn = true;
                if (Screen.lockCursor)
                {
                    Screen.lockCursor = false;
                }
                if (!Screen.showCursor)
                {
                    Screen.showCursor = true;
                }
            }
            rect = Helper.GetSmartRects(WindowPosition, 1)[0];

            scroll = Optimization.Caching.Vectors.v2zero;
            scrollRect = new SmartRect(0f, 0f, rect.width, rect.height, 0f, Style.VerticalMargin);
            scrollArea = new Rect(rect.x, rect.y, rect.width, WindowPosition.height - (4 * (Style.Height + Style.VerticalMargin)) - (Style.WindowTopOffset + Style.WindowBottomOffset) - 10f);
            scrollAreaView = new Rect(0f, 0f, rect.width, int.MaxValue);

            head = locale["title"];
            pagesSelection = locale.GetArray("buttons");
            pagePosition = new Rect(WindowPosition.x, WindowPosition.y + ((rect.height + Style.VerticalMargin) * 2f), WindowPosition.width, WindowPosition.height - (rect.y + rect.height + Style.VerticalMargin) - Style.WindowBottomOffset - Style.WindowTopOffset);
            SmartRect[] rects = Helper.GetSmartRects(pagePosition, 2);
            left = rects[0];
            right = rects[1];

            scrollName = Optimization.Caching.Vectors.v2zero;
            scrollRectName = new SmartRect(left.x, left.y, left.width, left.height, 0f, Style.VerticalMargin);
            scrollAreaName = new Rect(left.x, left.y, left.width, WindowPosition.height - (4 * (Style.Height + Style.VerticalMargin)) - (Style.WindowTopOffset + Style.WindowBottomOffset) - 10f);
            scrollAreaViewName = new Rect(left.x, left.y, left.width, int.MaxValue);

            scrollProperties = Optimization.Caching.Vectors.v2zero;
            scrollRectProperties = new SmartRect(right.x, right.y, right.width, right.height, 0f, Style.VerticalMargin);
            scrollAreaProperties = new Rect(right.x, right.y, right.width, WindowPosition.height - (4 * (Style.Height + Style.VerticalMargin)) - (Style.WindowTopOffset + Style.WindowBottomOffset) - 10f);
            scrollAreaViewProperties = new Rect(0, 0, right.width, int.MaxValue);


            scrollStyle = new GUIStyle();
            scrollStyle.normal.background = Textures.TextureCache[ElementType.Button][0];
            scrollStyle.hover.background = Textures.TextureCache[ElementType.SelectionGrid][0];
            scrollStyle.active.background = Textures.TextureCache[ElementType.SelectionGrid][1];
            scrollStyle.focused.background = Textures.TextureCache[ElementType.SelectionGrid][2];
            //scrollStyle.fixedHeight = 200f;
            scrollStyle.fixedWidth = 25f;

            scrollStyle.onNormal.background = Textures.TextureCache[ElementType.Button][0];
            scrollStyle.onHover.background = Textures.TextureCache[ElementType.SelectionGrid][3];
            scrollStyle.onActive.background = Textures.TextureCache[ElementType.SelectionGrid][4];
            scrollStyle.onFocused.background = Textures.TextureCache[ElementType.SelectionGrid][5];
        }

        [GUIPage(Traffic)]
        private void TrafficPage()
        {
            TrafficStatsGameLevel trafficStatsGameLevel = PhotonNetwork.networkingPeer.TrafficStatsGameLevel;
            long num = PhotonNetwork.networkingPeer.TrafficStatsElapsedMs / 1000L;
            if (num == 0L)
            {
                num = 1L;
            }
            left.Reset();
            right.Reset();
            bool cache = NetworkStats.Value;
            ToggleButton(left, NetworkStats, locale["statsOn"], true);
            if (NetworkStats.Value != cache)
            {
                PhotonNetwork.networkingPeer.TrafficStatsEnabled = NetworkStats.Value;
                return;
            }
            if (!NetworkStats.Value)
            {
                return;
            }
            Label(left, locale.Format("statString", trafficStatsGameLevel.TotalOutgoingMessageCount.ToString(), trafficStatsGameLevel.TotalIncomingMessageCount.ToString(), trafficStatsGameLevel.TotalMessageCount.ToString()), true);
            Label(left, locale.Format("avgSec", num.ToString()), true);
            Label(left, locale.Format("statString", (trafficStatsGameLevel.TotalOutgoingMessageCount / num).ToString(), (trafficStatsGameLevel.TotalIncomingMessageCount / num).ToString(), (trafficStatsGameLevel.TotalMessageCount / num).ToString()), true);

            Label(left, locale.Format("ping", PhotonNetwork.networkingPeer.RoundTripTime.ToString(), PhotonNetwork.networkingPeer.RoundTripTimeVariance.ToString()), true);
            Label(left, locale.Format("deltaSend", trafficStatsGameLevel.LongestDeltaBetweenSending.ToString()), true);
            Label(left, locale.Format("deltaDispatch", trafficStatsGameLevel.LongestDeltaBetweenDispatching.ToString()), true);
            Label(left, locale.Format("deltaEvent", trafficStatsGameLevel.LongestEventCallbackCode.ToString(), trafficStatsGameLevel.LongestEventCallback.ToString()), true);
            Label(left, locale.Format("deltaOperation", trafficStatsGameLevel.LongestOpResponseCallbackOpCode.ToString(), trafficStatsGameLevel.LongestOpResponseCallback.ToString()), true);

            left.MoveToEndY(WindowPosition, Style.Height);
            if (Button(left, locale["btnReset"]))
            {
                PhotonNetwork.networkingPeer.TrafficStatsReset();
                PhotonNetwork.networkingPeer.TrafficStatsEnabled = true;
            }

            LabelCenter(right, locale["in"], true);
            var stats = PhotonNetwork.networkingPeer.TrafficStatsIncoming;
            Label(right, locale.Format("bytesCountPackets", stats.TotalPacketBytes.ToString()), true);
            Label(right, locale.Format("bytesCountCmd", stats.TotalCommandBytes.ToString()), true);
            Label(right, locale.Format("packetsCount", stats.TotalPacketCount.ToString()), true);
            Label(right, locale.Format("cmdCount", stats.TotalCommandsInPackets.ToString()), true);

            LabelCenter(right, locale["out"], true);
            stats = PhotonNetwork.networkingPeer.TrafficStatsOutgoing;
            Label(right, locale.Format("bytesCountPackets", stats.TotalPacketBytes.ToString()), true);
            Label(right, locale.Format("bytesCountCmd", stats.TotalCommandBytes.ToString()), true);
            Label(right, locale.Format("packetsCount", stats.TotalPacketCount.ToString()), true);
            Label(right, locale.Format("cmdCount", stats.TotalCommandsInPackets.ToString()), true);
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