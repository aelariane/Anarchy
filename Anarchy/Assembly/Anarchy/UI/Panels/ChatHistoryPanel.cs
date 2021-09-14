using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using static Anarchy.UI.GUI;

namespace Anarchy.UI
{
    public class ChatHistoryPanel : GUIPanel
    {
        private Vector2 scroll;
        private Rect scrollArea;
        private Rect scrollAreaView;
        private SmartRect rect;
        private SmartRect scrollRect;
        private string showString = string.Empty;
        private readonly List<string> messages = new List<string>();

        public ChatHistoryPanel() : base(nameof(ChatHistoryPanel))
        {
            NetworkingPeer.RegisterEvent(PhotonNetworkingMessage.OnJoinedRoom, (args) => { messages.Clear(); showString = string.Empty; });
        }

        public void AddMessage(string message)
        {
            messages.Add(message.RemoveAll());
            if (IsActive)
            {
                showString += "\n" + message;
            }
        }

        public void ClearMessages()
        {
            messages.Clear();
        }

        protected override void DrawMainPart()
        {
            rect.Reset();
            rect.MoveY();
            scrollRect.Reset();
            scrollArea.y = rect.y;

            if (messages.Count > 0)
            {
                scroll = BeginScrollView(scrollArea, scroll, scrollAreaView);
                UnityEngine.GUILayout.TextArea(showString, Style.Label, new GUILayoutOption[] { UnityEngine.GUILayout.Width(scrollArea.width) });
                //var options = new GUILayoutOption[0];
                //foreach (var msg in messages)
                //{
                //    GUILayout.Label(msg.ToString(), options);
                //}
                EndScrollView();
            }
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
            showString = string.Empty;
        }

        protected override void OnPanelEnable()
        {
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

            var bld = new StringBuilder();
            foreach(string str in messages)
            {
                bld.AppendLine(str);
            }
            showString = bld.ToString();
        }

        public override void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Disable();
            }
        }
    }
}
