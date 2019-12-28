using UnityEngine;
using static Anarchy.UI.GUI;

namespace Anarchy.UI
{
    internal class LoadingMapScreen : GUIBase
    {
        private static LoadingMapScreen instance;


        private Rect screenRect = new Rect(0f, 0f, Style.ScreenWidth, Style.ScreenHeight);
        private SmartRect rect;

        public LoadingMapScreen() : base(nameof(LoadingMapScreen))
        {
            if(instance != null)
            {
                instance.DisableImmediate();
            }
            instance = this;
        }

        ~LoadingMapScreen()
        {
            instance = null;
        }

        protected internal override void Draw()
        {
            Box(screenRect, string.Empty);
            rect.Reset();
            LabelCenter(rect, locale["loadingLbl"], true);
            LabelCenter(rect, locale.Format("loadingProgress", PhotonNetwork.player.CurrentLevel.Length.ToString(), PhotonNetwork.masterClient.CurrentLevel.Length.ToString()), true);
            if(Button(rect, locale["disconnect"], false))
            {
                if (IN_GAME_MAIN_CAMERA.GameType == GameType.Multi)
                {
                    PhotonNetwork.Disconnect();
                }
                IN_GAME_MAIN_CAMERA.GameType = GameType.Stop;
                FengGameManagerMKII.FGM.GameStart = false;
                UnityEngine.Object.Destroy(FengGameManagerMKII.FGM);
                Application.LoadLevel("menu");
                DisableImmediate();
            }
            if(PhotonNetwork.player.CurrentLevel.Length >= PhotonNetwork.masterClient.CurrentLevel.Length)
            {
                DisableImmediate();
                RC.CustomLevel.customLevelLoaded = true;
                if (!FengGameManagerMKII.FGM.NeedChooseSide)
                {
                    if (!PhotonNetwork.player.IsTitan)
                    {
                        FengGameManagerMKII.FGM.SpawnPlayer(FengGameManagerMKII.FGM.myLastHero);
                    }
                    else
                    {
                        FengGameManagerMKII.FGM.SpawnNonAITitan("RANDOM");
                    }
                    FengGameManagerMKII.FGM.ShowHUDInfoCenter(string.Empty);
                }                GameLogic.RacingLogic log = FengGameManagerMKII.FGM.Logic as GameLogic.RacingLogic;
                if(log != null)
                {
                    if (log.RaceStart)
                    {
                        log.TryDestroyDoors();
                    }
                }
            }
        }

        protected override void OnDisable()
        {
            Screen.lockCursor = true;
            Screen.showCursor = true;
            IN_GAME_MAIN_CAMERA.SpecMov.disable = false;
            IN_GAME_MAIN_CAMERA.Look.disable = false;
            rect = null;
        }

        protected override void OnEnable()
        {
            screenRect = new Rect(0f, 0f, Style.ScreenWidth, Style.ScreenHeight);
            Rect center = Helper.GetScreenMiddle(new AutoScaleFloat(300f), new AutoScaleFloat(100f));
            rect = new SmartRect(center.x, center.y, center.width, new AutoScaleFloat(30f));
            Screen.lockCursor = false;
            Screen.showCursor = true;
            IN_GAME_MAIN_CAMERA.SpecMov.disable = true;
            IN_GAME_MAIN_CAMERA.Look.disable = true;
        }

        public override void OnUpdateScaling()
        {
            screenRect = new Rect(0f, 0f, Style.ScreenWidth, Style.ScreenHeight);
            rect = new SmartRect(Helper.GetScreenMiddle(new AutoScaleFloat(300f), new AutoScaleFloat(100f)));
        }
    }
}
