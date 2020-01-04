using Anarchy.Configuration;
using Anarchy.UI.Animation;
using UnityEngine;
using static Anarchy.UI.GUI;

namespace Anarchy.UI
{
    public class SettingsPanel : GUIPanel
    {
        const int General = 0;
        const int Video = 1;
        const int Effects = 2;
        const int Rebinds = 3;
        const int Anarchy = 4;
        const int Abilities = 5;

        const int RebindsHuman = 0;
        const int RebindsTitan = 1;
        const int RebindsHorse = 2;
        const int RebindsCannon = 3;

        const int Color = 0;
        const int NameAnimation = 1;
        const int ChatAndConsole = 2;

        const int Bombs = 0;

        private SmartRect left;
        private Rect pagePosition;
        private string[] pagesSelection;
        private SmartRect rect;
        private SmartRect right;
        private bool rebindWait = false;
        private KeySetting waitSetting = null;

        private int RebindPage;
        private string[] RebindLabels;

        private int ModPage;
        private string[] ModLabels;

        private int AbilityPage;
        private string[] AbilityLabels;

        private float[] localBombStats;

        public SettingsPanel() : base(nameof(SettingsPanel), 10)
        {
            animator = new CenterAnimation(this, Helper.GetScreenMiddle(Style.WindowWidth, Style.WindowHeight));
        }

        [GUIPage(Anarchy, GUIPageType.DisableMethod)]
        private void AnarchyPageDisabled()
        {
            UIManager.HUDScaleGUI.Value = (float)System.Math.Round(UIManager.HUDScaleGUI.Value, 2);
        }

        protected override void DrawMainPart()
        {
            DrawSelectionButtons();
            DrawLowerButtons();
        }

        [GUIPage(Abilities)]
        private void DrawAbilityPage()
        {
            rect.Reset();
            rect.MoveY();
            AbilityPage = SelectionGrid(rect, AbilityPage, AbilityLabels, AbilityLabels.Length, true);
            SmartRect[] rects = Helper.GetSmartRects(pagePosition, 2);
            switch (AbilityPage)
            {
                case 0:
                    LabelCenter(rects[0], locale["bombStats"], true);
                    localBombStats[0] = (int)HorizontalSlider(rects[0], localBombStats[0], locale.Format("bombRad", Mathf.RoundToInt(localBombStats[0]).ToString()), 0f, 10f, Style.LabelOffsetSlider, true);
                    localBombStats[1] = (int)HorizontalSlider(rects[0], localBombStats[1], locale.Format("bombRange", Mathf.RoundToInt(localBombStats[1]).ToString()), 0f, 10f, Style.LabelOffsetSlider, true);
                    localBombStats[2] = (int)HorizontalSlider(rects[0], localBombStats[2], locale.Format("bombSpeed", Mathf.RoundToInt(localBombStats[2]).ToString()), 0f, 10f, Style.LabelOffsetSlider, true);
                    localBombStats[3] = (int)HorizontalSlider(rects[0], localBombStats[3], locale.Format("bombCd", Mathf.RoundToInt(localBombStats[3]).ToString()), 0f, 10f, Style.LabelOffsetSlider, true);
                    TextField(rects[0], Bomb.BombNameSetting, locale["bombName"], Style.LabelOffset, true);

                    LabelCenter(rects[1], locale["bombColor"], true);
                    HorizontalSlider(rects[1], Bomb.MyBombColorR, locale["bombColorR"], 0f, 1f, Style.LabelOffsetSlider, true);
                    HorizontalSlider(rects[1], Bomb.MyBombColorG, locale["bombColorG"], 0f, 1f, Style.LabelOffsetSlider, true);
                    HorizontalSlider(rects[1], Bomb.MyBombColorB, locale["bombColorB"], 0f, 1f, Style.LabelOffsetSlider, true);
                    HorizontalSlider(rects[1], Bomb.MyBombColorA, locale["bombColorA"], 0.5f, 1f, Style.LabelOffsetSlider, true);
                    break;
            }
        }

        [GUIPage(Abilities, GUIPageType.DisableMethod)]
        private void DrawAbilityPageDisabled()
        {
            if (localBombStats != null)
            {
                Bomb.MyBombRad.Value = Mathf.RoundToInt(localBombStats[0]);
                Bomb.MyBombRange.Value = Mathf.RoundToInt(localBombStats[1]);
                Bomb.MyBombSpeed.Value = Mathf.RoundToInt(localBombStats[2]);
                Bomb.MyBombCD.Value = Mathf.RoundToInt(localBombStats[3]);
                if (Bomb.MyBombRad.Value + Bomb.MyBombRange.Value + Bomb.MyBombSpeed.Value + Bomb.MyBombCD.Value > 25)
                {
                    Bomb.MyBombRad.Value = 5;
                    Bomb.MyBombCD.Value = 5;
                    Bomb.MyBombSpeed.Value = 5;
                    Bomb.MyBombRange.Value = 5;
                }
                localBombStats = null;
            }
        }

        [GUIPage(Abilities, GUIPageType.EnableMethod)]
        private void DrawAbilityPageEnabled()
        {
            localBombStats = new float[4];
            localBombStats[0] = Bomb.MyBombRad;
            localBombStats[1] = Bomb.MyBombRange;
            localBombStats[2] = Bomb.MyBombSpeed;
            localBombStats[3] = Bomb.MyBombCD;
        }

        [GUIPage(Anarchy)]
        private void DrawAnarchyPage()
        {
            rect.Reset();
            rect.MoveY();
            ModPage = SelectionGrid(rect, ModPage, ModLabels, ModLabels.Length, true);
            SmartRect[] rects = Helper.GetSmartRects(pagePosition, 2);
            switch (ModPage)
            {
                case 0:
                    LabelCenter(rects[0], locale["windowOffset"], true);
                    ToggleButton(rects[0], UIManager.HUDAutoScaleGUI, locale["guiAutoScale"], true);
                    if (UIManager.HUDAutoScaleGUI.Value)
                    {
                        HorizontalSlider(rects[0], UIManager.HUDScaleGUI, locale.Format("hudScale", (UIManager.HUDScaleGUI.Value * 100f).ToString("F0")), 0.75f, 1.5f, Style.LabelOffsetSlider, true);
                    }
                    else
                    {
                        Label(rects[0], locale["guiScaleWarning"], true);
                        Label(rects[0], locale["guiScaleWarning1"], true);
                        TextField(rects[0], Style.FontSizeSetting, locale["fontSize"], Style.BigLabelOffset, true);
                        TextField(rects[0], Style.WindowWidthSetting, locale["windowWidth"], Style.BigLabelOffset, true);
                        TextField(rects[0], Style.WindowHeightSetting, locale["windowHeight"], Style.BigLabelOffset, true);
                        TextField(rects[0], Style.HeightSetting, locale["height"], Style.BigLabelOffset, true);
                        LabelCenter(rects[0], string.Empty, true);
                        TextField(rects[0], Style.WindowBottomOffsetSetting, locale["windowBottomOffset"], Style.BigLabelOffset, true);
                        TextField(rects[0], Style.WindowSideOffsetSetting, locale["windowSideOffset"], Style.BigLabelOffset, true);
                        TextField(rects[0], Style.WindowTopOffsetSetting, locale["windowTopOffset"], Style.BigLabelOffset, true);
                        TextField(rects[0], Style.LabelOffsetSetting, locale["labelOffset"], Style.BigLabelOffset, true);
                        TextField(rects[0], Style.LabelOffsetSliderSetting, locale["labelOffsetSlider"], Style.BigLabelOffset, true);
                        TextField(rects[0], Style.BigLabelOffsetSetting, locale["bigLabelOffset"], Style.BigLabelOffset, true);
                        TextField(rects[0], Style.HorizontalMarginSetting, locale["horizontalMargin"], Style.BigLabelOffset, true);
                        TextField(rects[0], Style.VerticalMarginSetting, locale["verticalMargin"], Style.BigLabelOffset, true);
                        TextField(rects[0], Style.LabelSpaceSetting, locale["labelSpace"], Style.BigLabelOffset, true);
                    }
                    LabelCenter(rects[1], locale["styleColors"], true);
                    Style.PublicSettings[0] = TextField(rects[1], Style.PublicSettings[0], locale["fontName"], Style.LabelOffset, true);
                    Style.PublicSettings[1] = TextField(rects[1], Style.PublicSettings[1], locale["background"], Style.LabelOffset, true);
                    Style.PublicSettings[2] = TextField(rects[1], Style.PublicSettings[2], locale["backgroundTransparency"], Style.BigLabelOffset, true);
                    Label(rects[1], "", true);
                    Style.PublicSettings[3] = TextField(rects[1], Style.PublicSettings[3], locale["text"] + " " + locale["normal"], Style.LabelOffset, true);
                    Style.PublicSettings[4] = TextField(rects[1], Style.PublicSettings[4], locale["text"] + " " + locale["hover"], Style.LabelOffset, true);
                    Style.PublicSettings[5] = TextField(rects[1], Style.PublicSettings[5], locale["text"] + " " + locale["active"], Style.LabelOffset, true);
                    Style.PublicSettings[6] = TextField(rects[1], Style.PublicSettings[6], locale["text"] + " " + locale["onNormal"], Style.LabelOffset, true);
                    Style.PublicSettings[7] = TextField(rects[1], Style.PublicSettings[7], locale["text"] + " " + locale["onHover"], Style.LabelOffset, true);
                    Style.PublicSettings[8] = TextField(rects[1], Style.PublicSettings[8], locale["text"] + " " + locale["onActive"], Style.LabelOffset, true);
                    Label(rects[1], "", true);
                    Style.UseVectors = ToggleButton(rects[1], Style.UseVectors, locale["useVectors"], true);
                    if (Style.UseVectors)
                    {
                        Style.PublicSettings[10] = TextField(rects[1], Style.PublicSettings[10], locale["vector"] + " " + locale["normal"], Style.LabelOffset, true);
                        Style.PublicSettings[11] = TextField(rects[1], Style.PublicSettings[11], locale["vector"] + " " + locale["hover"], Style.LabelOffset, true);
                        Style.PublicSettings[12] = TextField(rects[1], Style.PublicSettings[12], locale["vector"] + " " + locale["active"], Style.LabelOffset, true);
                        Style.PublicSettings[13] = TextField(rects[1], Style.PublicSettings[13], locale["vector"] + " " + locale["onNormal"], Style.LabelOffset, true);
                        Style.PublicSettings[14] = TextField(rects[1], Style.PublicSettings[14], locale["vector"] + " " + locale["onHover"], Style.LabelOffset, true);
                        Style.PublicSettings[15] = TextField(rects[1], Style.PublicSettings[15], locale["vector"] + " " + locale["onActive"], Style.LabelOffset, true);
                    }
                    else
                    {
                        Style.PublicSettings[16] = TextField(rects[1], Style.PublicSettings[16], locale["color"] + " " + locale["normal"], Style.LabelOffset, true);
                        Style.PublicSettings[17] = TextField(rects[1], Style.PublicSettings[17], locale["color"] + " " + locale["hover"], Style.LabelOffset, true);
                        Style.PublicSettings[18] = TextField(rects[1], Style.PublicSettings[18], locale["color"] + " " + locale["active"], Style.LabelOffset, true);
                        Style.PublicSettings[19] = TextField(rects[1], Style.PublicSettings[19], locale["color"] + " " + locale["onNormal"], Style.LabelOffset, true);
                        Style.PublicSettings[20] = TextField(rects[1], Style.PublicSettings[20], locale["color"] + " " + locale["onHover"], Style.LabelOffset, true);
                        Style.PublicSettings[21] = TextField(rects[1], Style.PublicSettings[21], locale["color"] + " " + locale["onActive"], Style.LabelOffset, true);
                    }
                    break;

                case 1:
                    break;

                case 2:
                    LabelCenter(rects[0], locale["chat"], true);
                    ToggleButton(rects[0], Chat.UseBackground, locale["chatBack"], true);
                    if (Chat.UseBackground.Value)
                    {
                        HorizontalSlider(rects[0], Chat.BackgroundTransparency, Style.LabelSpace + locale.Format("chatBackVal", Chat.BackgroundTransparency.Value.ToString("F2")), Style.LabelOffsetSlider, true);
                    }
                    TextField(rects[0], Chat.MessageCount, locale["chatCount"], Style.BigLabelOffset, true);
                    TextField(rects[0], Chat.FontSize, locale["chatSize"], Style.BigLabelOffset, true);
                    TextField(rects[0], Chat.ChatWidth, locale["chatWidth"], Style.BigLabelOffset, true);
                    ToggleButton(rects[0], Chat.UseCustomChatSpace, locale["chatUseCustomSpace"], true);
                    if (Chat.UseCustomChatSpace.Value)
                    {
                        TextField(rects[0], Chat.CustomChatSpaceUp, Style.LabelSpace + locale["chatSpaceUp"], Style.BigLabelOffset, true);
                        TextField(rects[0], Chat.CustomChatSpaceDown, Style.LabelSpace + locale["chatSpaceDown"], Style.BigLabelOffset, true);
                        TextField(rects[0], Chat.CustomChatSpaceLeft, Style.LabelSpace + locale["chatSpaceLeft"], Style.BigLabelOffset, true);
                        TextField(rects[0], Chat.CustomChatSpaceRight, Style.LabelSpace + locale["chatSpaceRight"], Style.BigLabelOffset, true);
                    }

                    LabelCenter(rects[1], locale["console"], true);
                    ToggleButton(rects[1], Log.UseBackground, locale["consoleBack"], true);
                    if (Log.UseBackground.Value)
                    {
                        HorizontalSlider(rects[1], Log.BackgroundTransparency, Style.LabelSpace + locale.Format("consoleBackVal", Log.BackgroundTransparency.Value.ToString("F2")), Style.LabelOffsetSlider, true);
                    }
                    TextField(rects[1], Log.MessageCount, locale["consoleCount"], Style.BigLabelOffset, true);
                    TextField(rects[1], Log.FontSize, locale["consoleSize"], Style.BigLabelOffset, true);
                    TextField(rects[1], Log.LogWidth, locale["consoleWidth"], Style.BigLabelOffset, true);
                    ToggleButton(rects[1], Log.UseCustomLogSpace, locale["consoleUseCustomSpace"], true);
                    if (Log.UseCustomLogSpace.Value)
                    {
                        TextField(rects[1], Log.CustomLogSpaceUp, Style.LabelSpace + locale["consoleSpaceUp"], Style.BigLabelOffset, true);
                        TextField(rects[1], Log.CustomLogSpaceDown, Style.LabelSpace + locale["consoleSpaceDown"], Style.BigLabelOffset, true);
                        TextField(rects[1], Log.CustomLogSpaceLeft, Style.LabelSpace + locale["consoleSpaceLeft"], Style.BigLabelOffset, true);
                        TextField(rects[1], Log.CustomLogSpaceRight, Style.LabelSpace + locale["consoleSpaceRight"], Style.BigLabelOffset, true);
                    }
                    break;
            }
        }

        [GUIPage(Effects)]
        private void DrawEffectsPage()
        {
            left.Reset();
            LabelCenter(left, locale["effects"], true);
            ToggleButton(left, VideoSettings.WindEffect, locale["windEffect"], true);
            ToggleButton(left, VideoSettings.BladeTrails, locale["bladeTrails"], true);
            ToggleButton(left, VideoSettings.CameraTilt, locale["tilt"], true);
            ToggleButton(left, VideoSettings.Blur, locale["blur"], true);
        }

        [GUIPage(General)]
        private void DrawGeneralPage()
        {
            //Left
            left.Reset();
            LabelCenter(left, locale["game"], true);
            ToggleButton(left, Settings.InvertY, locale["inverty"], true);
            HorizontalSlider(left, Settings.MouseSensivity, locale.Format("sensivity", (Settings.MouseSensivity.Value * 100f).ToString("F0") + "%"), 0.01f, 1f, Style.LabelOffsetSlider, true);
            HorizontalSlider(left, Settings.CameraDistance, locale.Format("distance", Settings.CameraDistance.Value.ToString("F2")), Style.LabelOffsetSlider, true);
            HorizontalSlider(left, Settings.CameraDistanceMore, locale.Format("moreDistance", Settings.CameraDistanceMore.Value.ToString("F2")), Style.LabelOffsetSlider, true);
            ToggleButton(left, Settings.StaticFOVEnabled, locale["staticFOV"], true);
            if (Settings.StaticFOVEnabled.Value)
            {
                HorizontalSlider(left, Settings.StaticFOV, Style.LabelSpace + locale.Format("fovValue", Settings.StaticFOV.Value.ToString("F0")), 60f, 120f, Style.LabelOffsetSlider, true);
            }

            ToggleButton(left, Settings.Minimap, locale["minimap"], true);
            ToggleButton(left, Settings.GameFeed, locale["gameFeed"], true);
            left.MoveY();
            LabelCenter(left, locale["audio"], true);
            HorizontalSlider(left, Settings.SoundLevel, locale.Format("overallAudio", (Settings.SoundLevel.Value * 100f).ToString("F0") + "%"), 0f, 1f, Style.LabelOffsetSlider, true);
            left.MoveY();
            LabelCenter(left, locale["snapshots"], true);
            ToggleButton(left, Settings.Snapshots, locale["snapshotsEnabled"], true);
            if (Settings.Snapshots.Value)
            {
                ToggleButton(left, Settings.SnapshotsInGame, Style.LabelSpace + locale["snapshotsShow"], true);
                TextField(left, Settings.SnapshotsDamage, Style.LabelSpace + locale["snapshotsDmg"], Style.BigLabelOffset, true);
            }

            //Right
            right.Reset();
            LabelCenter(right, locale["others"], true);
            ToggleButton(right, Settings.Speedometer, locale["speedometer"], true);
            if (Settings.Speedometer.Value)
            {
                SelectionGrid(right, Settings.SpeedometerType, locale.GetArray("speedometerTypes"), 2, true);
            }
            ToggleButton(right, Settings.BodyLean, locale["bodylean"], true);

        }

        private void DrawLowerButtons()
        {
            rect.MoveToEndY(BoxPosition, Style.Height);
            rect.MoveToEndX(BoxPosition, Style.LabelOffset);
            rect.width = Style.LabelOffset;
            if (Button(rect, locale["btnBack"]))
            {
                Disable();
                return;
            }
        }

        [GUIPage(Rebinds)]
        private void DrawRebindsPage()
        {
            //To do
            rect.Reset();
            rect.MoveY();
            RebindPage = SelectionGrid(rect, RebindPage, RebindLabels, RebindLabels.Length, true);
            SmartRect[] rects = Helper.GetSmartRects(pagePosition, 2);
            int RebindCannon = (int)InputCodes.DefaultsCount + InputManager.RebindKeyCodes.Length;
            int CannonTitan = RebindCannon + InputManager.CannonKeyCodes.Length;
            int TitanHorse = CannonTitan + InputManager.TitanKeyCodes.Length;
            switch (RebindPage)
            {
                case RebindsHuman:
                    LabelCenter(rects[0], locale["mainRebinds"], true);
                    for (int i = 0; i < 17; i++)
                    {
                        RebindButton(rects[0], InputManager.AllKeys[i]);
                    }
                    rects[1].MoveY();
                    for (int i = 17; i < (int)InputCodes.DefaultsCount; i++)
                    {
                        RebindButton(rects[1], InputManager.AllKeys[i]);
                    }
                    LabelCenter(rects[1], locale["rebinds"], true);
                    for (int i = (int)InputCodes.DefaultsCount; i < RebindCannon; i++)
                    {
                        RebindButton(rects[1], InputManager.AllKeys[i]);
                    }
                    break;
                case RebindsCannon:
                    rects[0].MoveY();
                    for (int i = RebindCannon; i < CannonTitan; i++)
                    {
                        RebindButton(rects[0], InputManager.AllKeys[i]);
                    }
                    break;
                case RebindsTitan:
                    rects[0].MoveY();
                    for (int i = CannonTitan; i < TitanHorse; i++)
                    {
                        RebindButton(rects[0], InputManager.AllKeys[i]);
                    }
                    break;
                case RebindsHorse:
                    rects[0].MoveY();
                    for (int i = TitanHorse; i < InputManager.AllKeys.Count; i++)
                    {
                        RebindButton(rects[0], InputManager.AllKeys[i]);
                    }
                    break;
                default:
                    break;
            }
        }

        private void DrawSelectionButtons()
        {
            rect.Reset();
            pageSelection = SelectionGrid(rect, pageSelection, pagesSelection, pagesSelection.Length, true);
        }

        [GUIPage(Video)]
        private void DrawVideoPage()
        {
            //left
            left.Reset();
            LabelCenter(left, locale["graphics"], true);
            HorizontalSlider(left, VideoSettings.DrawDistance, locale.Format("drawDistance", VideoSettings.DrawDistance.Value.ToString("F0")), 1000f, 10000f, Style.LabelOffsetSlider, true);
            ToggleButton(left, VideoSettings.Mipmap, locale["mipmap"], true);
            ToggleButton(left, VideoSettings.VSync, locale["vsync"], true);
            LabelCenter(left, locale["texturesQuality"], true);
            SelectionGrid(left, VideoSettings.TextureQuality, locale.GetArray("texturesLevels"), 4, true);
            LabelCenter(left, locale.Format("quality", VideoSettings.Quality.Value.ToString("F2")), true);
            HorizontalSlider(left, VideoSettings.Quality, string.Empty, 0f, 1f, 0f, true);
            left.MoveY();
            LabelCenter(left, locale["advancedVideo"], true);
            ToggleButton(left, VideoSettings.OcclusionCulling, locale["occlusion"], true);
            LabelCenter(left, locale["aniso"], true);
            SelectionGrid(left, VideoSettings.AnisotropicFiltering, locale.GetArray("anisoLevels"), 3, true);
            Label(left, locale["antiAliasing"], false);
            left.MoveOffsetX(Style.LabelOffset);
            SelectionGrid(left, VideoSettings.AntiAliasing, locale.GetArray("antiAliasings"), 4, true);
            left.ResetX();
            Label(left, locale["bWeight"], false);
            left.MoveOffsetX(Style.LabelOffset);
            SelectionGrid(left, VideoSettings.BlendWeight, locale.GetArray("blendWeights"), 3, true);
            left.ResetX();
            HorizontalSlider(left, VideoSettings.LODBias, locale.Format("lodBias", VideoSettings.LODBias.Value.ToString("F2")), 0f, 2f, Style.LabelOffsetSlider, true);
            HorizontalSlider(left, VideoSettings.MaxLODLevel, locale.Format("maxLOD", VideoSettings.MaxLODLevel.Value.ToString("F0")), 0f, 7f, Style.LabelOffsetSlider, true);


            //right
            right.Reset();
            LabelCenter(right, locale["shadows"], true);
            Label(right, locale["shadowNote"], true);
            ToggleButton(right, VideoSettings.UseShadows, locale["useShadows"], true);
            HorizontalSlider(right, VideoSettings.ShadowDistance, locale.Format("shadowDist", VideoSettings.ShadowDistance.Value.ToString("F0")), 0f, 1000f, Style.LabelOffsetSlider, true);
            Label(right, locale["shadowCascade"], false);
            right.MoveOffsetX(Style.LabelOffset);
            SelectionGrid(right, VideoSettings.ShadowCascades, locale.GetArray("shadowCascades"), 3, true);
            right.ResetX();
            Label(right, locale["shadowProjection"], false);
            right.MoveOffsetX(Style.LabelOffset);
            SelectionGrid(right, VideoSettings.ShadowProjection, locale.GetArray("shadowProjections"), 2, true);
            right.ResetX();
        }

        protected override void OnPanelDisable()
        {
            pagesSelection = null;
            head = null;
            left = null;
            rect = null;
            right = null;
            RebindPage = 0;
            RebindLabels = null;
            Settings.Apply();
            VideoSettings.Apply();
        }

        protected override void OnPanelEnable()
        {
            rect = Helper.GetSmartRects(BoxPosition, 1)[0];
            head = locale["title"];
            pagePosition = new Rect(BoxPosition.x, BoxPosition.y + ((rect.height + Style.VerticalMargin) * 2f), BoxPosition.width, BoxPosition.height - (rect.y + rect.height + Style.VerticalMargin) - Style.WindowBottomOffset - Style.WindowTopOffset);
            SmartRect[] rects = Helper.GetSmartRects(pagePosition, 2);
            left = rects[0];
            right = rects[1];
            pagesSelection = locale.GetArray("buttons");
            RebindPage = 0;
            RebindLabels = locale.GetArray("buttonsRebinds");
            ModPage = 0;
            ModLabels = locale.GetArray("modMenu");
            AbilityPage = 0;
            AbilityLabels = locale.GetArray("abilityNames");
        }

        private void RebindButton(SmartRect rect, KeySetting set)
        {
            Label(rect, set.Key + ":");
            rect.MoveOffsetX(Style.LabelOffset * 2f);
            if (Button(rect, set == waitSetting ? locale["waiting"] : set.ToString()) && !rebindWait)
            {
                rebindWait = true;
                waitSetting = set;
            }
            rect.ResetX();
            if (set != waitSetting || !rebindWait)
                return;
            var curr = Event.current;
            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.LeftShift))
            {
                set.SetValue(KeyCode.LeftShift);
                rebindWait = false;
                waitSetting = null;
                return;
            }
            else if (Input.GetKey(KeyCode.RightShift) || Input.GetKeyDown(KeyCode.RightShift))
            {
                set.SetValue(KeyCode.RightShift);
                rebindWait = false;
                waitSetting = null;
                return;
            }
            if(Input.GetAxis("Mouse ScrollWheel") != 0f)
            {
                set.SetAsAxis(Input.GetAxis("Mouse ScrollWheel") > 0f);
                rebindWait = false;
                waitSetting = null;
            }
            if ((Input.anyKey || curr.functionKey) && curr.keyCode != KeyCode.None)
            {
                set.SetValue(curr.keyCode);
                for (int i = 0; i < 7; i++)
                {
                    if (Input.GetKeyDown(KeyCode.Mouse0 + i))
                    {
                        set.SetValue((KeyCode.Mouse0 + i));
                        waitSetting = null;
                        rebindWait = false;
                        return;
                    }
                }
                rebindWait = false;
                waitSetting = null;
                return;
            }
            for (int i = 0; i < 7; i++)
            {
                if (Input.GetKeyDown(KeyCode.Mouse0 + i))
                {
                    set.SetValue((KeyCode.Mouse0 + i));
                    waitSetting = null;
                    rebindWait = false;
                    return;
                }
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
    }
}