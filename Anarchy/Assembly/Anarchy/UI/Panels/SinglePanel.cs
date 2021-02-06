using Anarchy.Configuration;
using Anarchy.UI.Animation;
using UnityEngine;
using static Anarchy.UI.GUI;

namespace Anarchy.UI
{
    internal class SinglePanel : GUIPanel
    {
        private string[] cameraList;
        private string[] costumeList;
        private readonly IntSetting costumeSelection = new IntSetting("SingleCostumeSelection");
        private string[] characterList;
        private readonly IntSetting characterSelection = new IntSetting("SingleCharacterSelection");
        private string[] dayLightList;
        private readonly IntSetting dayLightSelection = new IntSetting("SingleDayLightSelection");
        private string[] difficulityList;
        private readonly IntSetting difficulitySelection = new IntSetting("SingleDifficulitySelection");
        private SmartRect left;
        private string[] mapList;
        private readonly IntSetting mapSelection = new IntSetting("SingleMapSelection");
        private SmartRect right;

        public SinglePanel() : base(nameof(SinglePanel), GUILayers.SinglePanel)
        {
        }

        protected override void DrawMainPart()
        {
            DrawLeftColumn();
            if (!IsActive)
            {
                return;
            }

            DrawRightColumn();
        }

        private void DrawLeftColumn()
        {
            left.Reset();
            LabelCenter(left, locale["map"], true);
            SelectionGrid(left, mapSelection, mapList, 1, true);
            LabelCenter(left, locale["difficulity"], true);
            SelectionGrid(left, difficulitySelection, difficulityList, 3, true);
            LabelCenter(left, locale["daylight"], true);
            SelectionGrid(left, dayLightSelection, dayLightList, 3, true);
            LabelCenter(left, locale["camera"], true);
            SelectionGrid(left, Settings.CameraMode, cameraList, cameraList.Length, true);
            left.MoveToEndY(WindowPosition, new AutoScaleFloat(30f));
            left.height = new AutoScaleFloat(30f);
            if (Button(left, locale["start"], true))
            {
                PhotonNetwork.SetModProperties();
                OnButtonStartClick();
                DisableImmediate();
                return;
            }
        }

        private void DrawRightColumn()
        {
            right.Reset();
            LabelCenter(right, locale["character"], true);
            SelectionGrid(right, costumeSelection, costumeList, 3, true);
            DropdownMenu(this, right, characterSelection, characterList, true);
            right.MoveToEndY(WindowPosition, new AutoScaleFloat(30f));
            right.MoveOffsetX(new AutoScaleFloat(150f));
            right.height = new AutoScaleFloat(30f);
            if (Button(right, locale["back"], false))
            {
                Disable();
                return;
            }
        }

        private void OnButtonStartClick()
        {
            IN_GAME_MAIN_CAMERA.Difficulty = difficulitySelection;
            IN_GAME_MAIN_CAMERA.GameType = GameType.Single;
            IN_GAME_MAIN_CAMERA.singleCharacter = new string[] { "Mikasa", "Levi", "Armin", "Marco", "Jean", "Eren", "TITAN_EREN", "Petra", "Sasha", "Set 1", "Set 2", "Set 3" }[characterSelection.ToValue()].ToUpper();
            IN_GAME_MAIN_CAMERA.CameraMode = (CameraType)Settings.CameraMode.ToValue();
            IN_GAME_MAIN_CAMERA.DayLight = (DayLight)dayLightSelection.ToValue();
            Screen.lockCursor = IN_GAME_MAIN_CAMERA.CameraMode >= CameraType.TPS;
            Screen.showCursor = false;
            CheckBoxCostume.costumeSet = costumeSelection.ToValue() + 1;
            string map = mapList[mapSelection.ToValue()];
            FengGameManagerMKII.Level = LevelInfo.GetInfo(map);
            Application.LoadLevel(FengGameManagerMKII.Level.MapName);
        }

        protected override void OnPanelDisable()
        {
            left = null;
            right = null;
            mapList = null;
            cameraList = null;
            characterList = null;
            costumeList = null;
            dayLightList = null;
            difficulityList = null;
            if (Application.loadedLevelName == "menu")
            {
                AnarchyManager.MainMenu.EnableImmediate();
            }
        }

        protected override void OnPanelEnable()
        {
            SmartRect[] init = Helper.GetSmartRects(WindowPosition, 2);
            left = init[0];
            right = init[1];
            mapList = new string[] { "[S]Tutorial", "[S]Battle training", "[S]City", "[S]Forest", "[S]Forest Survive(no crawler)", "[S]Forest Survive(no crawler no punk)", "[S]Racing - Akina" };
            cameraList = new string[] { "ORIGINAL", "WOW", "TPS", "NewTPS" };
            characterList = locale.GetArray("characters");
            costumeList = new string[] { "Cos1", "Cos2", "Cos3" };
            dayLightList = locale.GetArray("daylights");
            difficulityList = locale.GetArray("difficulities");
            AnarchyManager.MainMenu.DisableImmediate();
        }

        public override void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Disable();
                return;
            }
        }
    }
}