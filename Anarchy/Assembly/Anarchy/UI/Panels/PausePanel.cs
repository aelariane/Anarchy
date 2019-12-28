using UnityEngine;

namespace Anarchy.UI
{
    public class PausePanel : GUIBase
    {
        const float Height = 385;
        const float Width = 300;

        private GUIBase activePanel;
        private Rect boxPosition;
        private bool leaving = false;
        private SmartRect pauseRect;
        private GUIStyle pauseStyle;

        public PausePanel() : base(nameof(PausePanel), 20)
        {
            animator = new Animation.CenterAnimation(this, Helper.GetScreenMiddle(Width, Height), 200f, 400f);
        }

        public void Continue()
        {
            leaving = false;
            if (activePanel != null)
                activePanel.DisableImmediate();
            Disable();
        }

        protected internal override void Draw()
        {
            if (activePanel != null && activePanel.Active)
                return;
            GUI.Box(boxPosition, locale["title"]);
            pauseRect.Reset();
            if (PauseButton(pauseRect, "btnContinue"))
            {
                Continue();
                return;
            }
            if (PauseButton(pauseRect, "profile"))
            {
                if (activePanel != null)
                {
                    activePanel.DisableImmediate();
                }
                activePanel = AnarchyManager.ProfilePanel;
                activePanel.Enable();
            }
            if (PauseButton(pauseRect, "skins"))
            {
                if (activePanel != null)
                {
                    activePanel.DisableImmediate();
                }
                activePanel = new SkinsPanel();
                activePanel.Enable();
            }
            if (PauseButton(pauseRect, "custom"))
            {
                if (IN_GAME_MAIN_CAMERA.GameType == GameType.Single || (IN_GAME_MAIN_CAMERA.GameType == GameType.Multi && PhotonNetwork.IsMasterClient))
                {
                    if (activePanel != null)
                    {
                        activePanel.DisableImmediate();
                    }
                    activePanel = new CustomPanel();
                    activePanel.Enable();
                }
            }
            if (PauseButton(pauseRect, "gameSettings"))
            {
                if (IN_GAME_MAIN_CAMERA.GameType == GameType.Single || (IN_GAME_MAIN_CAMERA.GameType == GameType.Multi && PhotonNetwork.IsMasterClient))
                {
                    if (activePanel != null)
                    {
                        activePanel.DisableImmediate();
                    }
                    activePanel = new GameModesPanel();
                    activePanel.Enable();
                }
            }
            if (PauseButton(pauseRect, "settings"))
            {
                if (activePanel != null)
                {
                    activePanel.DisableImmediate();
                }
                activePanel = AnarchyManager.SettingsPanel;
                activePanel.Enable();
            }
            if (PauseButton(pauseRect, "btnQuit"))
            {
                Quit();
                return;
            }
        }

        protected override void OnDisable()
        {
            if (activePanel != null)
                activePanel.DisableImmediate();
            activePanel = null;
            pauseStyle = null;
            pauseRect = null;
            Time.timeScale = 1f;
            Screen.lockCursor = leaving ? false : (IN_GAME_MAIN_CAMERA.CameraMode >= CameraType.TPS);
            Screen.showCursor = leaving;
            IN_GAME_MAIN_CAMERA.isPausing = false;
            InputManager.MenuOn = false;
            if (IN_GAME_MAIN_CAMERA.MainCamera != null && !IN_GAME_MAIN_CAMERA.MainCamera.enabled)
            {
                IN_GAME_MAIN_CAMERA.SpecMov.disable = false;
                IN_GAME_MAIN_CAMERA.Look.disable = false;
            }
            if (leaving)
            {
                if(IN_GAME_MAIN_CAMERA.GameType == GameType.Multi)
                {
                    PhotonNetwork.Disconnect();
                }
                IN_GAME_MAIN_CAMERA.GameType = GameType.Stop;
                FengGameManagerMKII.FGM.GameStart = false;
                Object.Destroy(FengGameManagerMKII.FGM);
                Application.LoadLevel("menu");
            }
            if (!leaving)
                Configuration.Settings.Apply();
        }

        protected override void OnEnable()
        {
            activePanel = null;
            boxPosition = Helper.GetScreenMiddle(Width, Height);
            pauseRect = new SmartRect(boxPosition.x + 10f, boxPosition.y + 25f, boxPosition.width - 20f, 50f - Style.VerticalMargin, Style.HorizontalMargin, Style.VerticalMargin);
            pauseStyle = new GUIStyle(Style.Button);
            pauseStyle.fontSize = Style.FontSize * 2;
            pauseStyle.fontStyle = FontStyle.Bold;
            leaving = false;
        }

        private bool PauseButton(SmartRect rect, string key)
        {
            bool res = UnityEngine.GUI.Button(rect.ToRect(), locale[key], pauseStyle);
            rect.MoveY(); 
            return res;
        }

        private void Quit()
        {
            leaving = true;
            DisableImmediate();
        }

        public override void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape) && (activePanel == null || !activePanel.Active))
            {
                Continue();
                return;
            }
        }
    }
}
