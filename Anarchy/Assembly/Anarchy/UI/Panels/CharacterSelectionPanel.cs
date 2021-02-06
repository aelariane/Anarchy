using Anarchy.Configuration;
using Anarchy.UI.Animation;
using Optimization.Caching;
using UnityEngine;
using static Anarchy.UI.GUI;

namespace Anarchy.UI
{
    internal class CharacterSelectionPanel : GUIPanel
    {
        private string[] charactersList;
        private string[] cameraList;
        private string[] costumeList;
        private readonly IntSetting costumeSelection = new IntSetting("MultiCostumeSelection");
        private string[] characterList;
        private readonly IntSetting characterSelection = new IntSetting("MultiCharacterSelection");
        private SmartRect left;
        private SmartRect right;
        private HeroStat stats;
        private string character;
        private Texture2D avatar = null;

        public CharacterSelectionPanel() : base(nameof(CharacterSelectionPanel), GUILayers.CharacterSelection)
        {
        }

        protected override void DrawMainPart()
        {
            left.Reset();
            LabelCenter(left, locale["character"], true);
            SelectionGrid(left, costumeSelection, costumeList, 3, true);
            bool updateChar = avatar == null;
            int oldChar = characterSelection.Value;
            SelectionGrid(left, characterSelection, characterList, 1, true);
            if (oldChar != characterSelection.Value)
            {
                updateChar = true;
            }
            character = charactersList[characterSelection].ToUpper();
            var set = CostumeConeveter.LocalDataToHeroCostume(character);
            stats = character.Contains("SET") ? (set == null ? new HeroStat() : set.stat) : HeroStat.getInfo(character);

            LabelCenter(left, locale["camera"], true);
            SelectionGrid(left, Settings.CameraMode, cameraList, cameraList.Length, true);
            float height = Style.Height * (FengGameManagerMKII.Level.PVPEnabled ? 3f : 2f) + (Style.VerticalMargin * (FengGameManagerMKII.Level.PVPEnabled ? 2f : 1f)) + (Style.Height + Style.VerticalMargin);
            left.MoveToEndY(WindowPosition, height);
            if (Button(left, locale["humanStart"], true))
            {
                SpawnHero();
                DisableImmediate();
                return;
            }

            if (FengGameManagerMKII.Level.TeamTitan && IN_GAME_MAIN_CAMERA.GameMode != GameMode.PvpAhss)
            {
                if (Button(left, locale["titanStart"], true))
                {
                    SpawnTitan();
                    DisableImmediate();
                    return;
                }
            }
            if (FengGameManagerMKII.Level.PVPEnabled)
            {
                if (Button(left, locale["ahssStart"], true))
                {
                    SpawnAHSS();
                    DisableImmediate();
                    return;
                }
            }
            if (Button(left, locale["back"], true))
            {
                Screen.lockCursor = true;
                Screen.showCursor = true;
                IN_GAME_MAIN_CAMERA.SpecMov.disable = false;
                IN_GAME_MAIN_CAMERA.Look.disable = false;
                Disable();
                return;
            }
            right.Reset();
            LabelCenter(right, locale["avatar"], true);
            right.height = right.width;
            if (updateChar)
            {
                avatar = LoadTexture(character.Contains("SET") ? "CUSTOM" : character, "png");
            }
            DrawTexture(right, avatar, true);
            right.height = Style.Height;
            LabelCenter(right, locale["stats"], true);
            Label(right, locale.Format("speed", stats.Spd.ToString()), true);
            Label(right, locale.Format("acceleration", stats.Acl.ToString()), true);
            Label(right, locale.Format("gas", stats.Gas.ToString()), true);
            Label(right, locale.Format("blade", stats.Bla.ToString()), true);
        }

        private void SpawnAHSS()
        {
            if (character.Contains("SET"))
            {
                var set = CostumeConeveter.LocalDataToHeroCostume(character);
                if (set.costumeId != 25 && set.costumeId != 26)
                {
                    character = "AHSS";
                }
            }
            else
            {
                character = "AHSS";
            }
            FengGameManagerMKII.FGM.needChooseSide = false;
            if (!PhotonNetwork.IsMasterClient && FengGameManagerMKII.FGM.logic.RoundTime > 60f)
            {
                FengGameManagerMKII.FGM.NotSpawnPlayer(character);
                FengGameManagerMKII.FGM.BasePV.RPC("restartGameByClient", PhotonTargets.MasterClient, new object[0]);
            }
            else
            {
                FengGameManagerMKII.FGM.SpawnPlayerAt(character, FengGameManagerMKII.Level.Name.Contains("Custom") ? string.Empty : "playerRespawn2");
            }
            IN_GAME_MAIN_CAMERA.usingTitan = false;
            IN_GAME_MAIN_CAMERA.MainCamera.setHUDposition();
            PhotonNetwork.player.Character = character;
        }

        private void SpawnHero()
        {
            CheckBoxCostume.costumeSet = costumeSelection.ToValue() + 1;
            FengGameManagerMKII.FGM.needChooseSide = false;
            if (IN_GAME_MAIN_CAMERA.GameMode == GameMode.PVP_CAPTURE)
            {
                FengGameManagerMKII.FGM.checkpoint = CacheGameObject.Find("PVPchkPtH");
            }
            if (!PhotonNetwork.IsMasterClient && FengGameManagerMKII.FGM.logic.RoundTime > 60f)
            {
                if (!FengGameManagerMKII.IsPlayerAllDead())
                {
                    FengGameManagerMKII.FGM.NotSpawnPlayer(character);
                }
                else
                {
                    FengGameManagerMKII.FGM.NotSpawnPlayer(character);
                    FengGameManagerMKII.FGM.BasePV.RPC("restartGameByClient", PhotonTargets.MasterClient, new object[0]);
                }
            }
            else if (IN_GAME_MAIN_CAMERA.GameMode == GameMode.BossFightCT || IN_GAME_MAIN_CAMERA.GameMode == GameMode.Trost || IN_GAME_MAIN_CAMERA.GameMode == GameMode.PVP_CAPTURE)
            {
                if (FengGameManagerMKII.IsPlayerAllDead())
                {
                    FengGameManagerMKII.FGM.NotSpawnPlayer(character);
                    FengGameManagerMKII.FGM.BasePV.RPC("restartGameByClient", PhotonTargets.MasterClient, new object[0]);
                }
                else
                {
                    FengGameManagerMKII.FGM.SpawnPlayer(character);
                }
            }
            else
            {
                FengGameManagerMKII.FGM.SpawnPlayer(character);
            }
            IN_GAME_MAIN_CAMERA.usingTitan = false;
            IN_GAME_MAIN_CAMERA.MainCamera.setHUDposition();
            PhotonNetwork.player.Character = character;
        }

        private void SpawnTitan()
        {
            if (IN_GAME_MAIN_CAMERA.GameMode == GameMode.PVP_CAPTURE)
            {
                FengGameManagerMKII.FGM.checkpoint = CacheGameObject.Find("PVPchkPtT");
            }
            if ((!PhotonNetwork.IsMasterClient && FengGameManagerMKII.FGM.logic.RoundTime > 60f) || FengGameManagerMKII.FGM.justSuicide)
            {
                FengGameManagerMKII.FGM.justSuicide = false;
                FengGameManagerMKII.FGM.NotSpawnNonAiTitan(character);
            }
            else
            {
                FengGameManagerMKII.FGM.SpawnNonAiTitan(character, "titanRespawn");
            }
            FengGameManagerMKII.FGM.needChooseSide = false;
            IN_GAME_MAIN_CAMERA.usingTitan = true;
            IN_GAME_MAIN_CAMERA.MainCamera.setHUDposition();
        }

        protected override void OnPanelDisable()
        {
            character = "";
            right = null;
            cameraList = null;
            characterList = null;
            charactersList = null;
            costumeList = null;
            avatar = null;
        }

        protected override void OnPanelEnable()
        {
            SmartRect[] init = Helper.GetSmartRects(WindowPosition, 2);
            left = init[0];
            right = init[1];
            cameraList = new string[] { "ORIGINAL", "WOW", "TPS" };
            characterList = locale.GetArray("characters");
            costumeList = new string[] { "Cos1", "Cos2", "Cos3" };
            charactersList = new string[] { "Mikasa", "Levi", "Armin", "Marco", "Jean", "Eren", "Petra", "Sasha", "Set 1", "Set 2", "Set 3" };
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
