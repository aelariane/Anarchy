using Anarchy.Configuration;
using Anarchy.IO;
using System.Collections.Generic;
using UnityEngine;
using static Optimization.Caching.Colors;

namespace Anarchy.UI
{
    /// <summary>
    /// Unified Anarchy mod style definitions
    /// </summary>
    //TODO: Add docs
    public static class Style
    {
        public static float ScreenWidthDefault { get; private set; }
        public static float ScreenHeightDefault { get; private set; }
        public static float ScreenWidth { get; set; }
        public static float ScreenHeight { get; set; }

        public static FloatSetting BigLabelOffsetSetting = new FloatSetting("GlobalBigLabelOffset", 320f);
        public static IntSetting FontSizeSetting = new IntSetting("GlobalFontSize", 14);
        public static FloatSetting HeightSetting = new FloatSetting("GlobalHeight", 22f);
        public static IntSetting HorizontalMarginSetting = new IntSetting("GlobalHorizontalMargin", 5);
        public static IntSetting VerticalMarginSetting = new IntSetting("GlobalVerticalMargin", 3);
        public static FloatSetting WindowHeightSetting = new FloatSetting("GlobalWindowHeight", 600);
        public static FloatSetting WindowWidthSetting = new FloatSetting("GlobalWindowWidth", 840f);
        public static FloatSetting WindowBottomOffsetSetting = new FloatSetting("GlobalWindowBottomOffset", 12f);
        public static FloatSetting WindowSideOffsetSetting = new FloatSetting("GlobalWindowSideOffset", 24f);
        public static FloatSetting WindowTopOffsetSetting = new FloatSetting("GlobalWindowTopOffset", 30f);
        public static FloatSetting LabelOffsetSetting = new FloatSetting("GlobalLabelOffset", 140f);
        public static FloatSetting LabelOffsetSliderSetting = new FloatSetting("LabelOffsetSlider", 180f);
        public static IntSetting LabelSpaceSetting = new IntSetting("GlobalLabelSpaceCount", 3);
        public static IntSetting FontSetting = new IntSetting("GlobalFontName", 3);
        public static FloatSetting BackgroundTransparencySetting = new FloatSetting("GlobalBackgroundTransparency", 250f);
        public static string[] PublicSettings;

        private static List<GUIStyle> allStyles;
        public static readonly GUIStyle Box = new GUIStyle();
        public static readonly GUIStyle Button = new GUIStyle();
        public static readonly GUIStyle Label = new GUIStyle();
        public static readonly GUIStyle LabelCenter = new GUIStyle();
        public static readonly GUIStyle ScrollView = GUIStyle.none;
        public static readonly GUIStyle SelectionGrid = new GUIStyle();
        public static readonly GUIStyle Slider = new GUIStyle();
        public static readonly GUIStyle SliderBody = new GUIStyle();
        public static readonly GUIStyle TextButton = new GUIStyle();
        private static string[] TextColors;
        public static readonly GUIStyle TextField = new GUIStyle();
        public static bool UseVectors = false;
        public static Vector3[] TextureDeltas;
        public static Color[] TextureColors;
        public static readonly GUIStyle Toggle = new GUIStyle();
        private static bool wasLoaded = false;

        /// <summary>
        /// Base colors for background of <seealso cref="GUIPanel"/>
        /// </summary>
        public static Color BackgroundColor => BackgroundHex.HexToColor((byte)BackgroundTransparency);
        public static string BackgroundHex { get; set; }
        public static int BackgroundTransparency { get; set; } = 250;
        public static float BigLabelOffset { get; private set; }
        public static Font Font { get; private set; }
        public static string FontName { get; private set; }
        public static int FontSize { get; private set; }
        public static float Height { get; private set; }
        public static int HorizontalMargin { get; private set; }
        public static int VerticalMargin { get; private set; }
        public static float WindowHeight { get; private set; }
        public static float WindowWidth { get; private set; }
        public static float WindowBottomOffset { get; private set; }
        public static float WindowSideOffset { get; private set; }
        public static float WindowTopOffset { get; private set; }
        public static float LabelOffset { get; private set; }
        public static float LabelOffsetSlider { get; private set; }
        public static string LabelSpace { get; private set; }

        public static void Initialize()
        {
            if (allStyles == null)
            {
                allStyles = new List<GUIStyle>();
            }
            lock (allStyles)
            {
                allStyles.Clear();
                allStyles.AddRange(new GUIStyle[] { Box, Button, Label, LabelCenter, SelectionGrid, TextButton, TextField, Toggle });
            }
            InitializeStyles();
        }

        private static void InitializeStyles()
        {
            Textures.Initialize();
            Color[] textColorsArray = new Color[6] { TextColors[0].HexToColor(), TextColors[1].HexToColor(), TextColors[2].HexToColor(), TextColors[3].HexToColor(), TextColors[4].HexToColor(), TextColors[5].HexToColor() };
            //Box
            Box.ApplyStyle(TextAnchor.UpperCenter, FontStyle.Bold, FontSize + 2, true, textColorsArray[0]);
            Box.richText = true;
            Box.normal.background = Textures.TextureCache[ElementType.Box][0];
            Box.hover.background = Textures.TextureCache[ElementType.Box][0];
            Box.active.background = Textures.TextureCache[ElementType.Box][0];
            //Button
            Button.ApplyStyle(TextAnchor.MiddleCenter, FontStyle.Normal, FontSize, true, textColorsArray);
            Button.richText = true;
            Button.normal.background = Textures.TextureCache[ElementType.Button][0];
            Button.hover.background = Textures.TextureCache[ElementType.Button][1];
            Button.active.background = Textures.TextureCache[ElementType.Button][2];
            //Label
            Label.ApplyStyle(TextAnchor.MiddleLeft, FontStyle.Normal, FontSize, true, textColorsArray);
            Label.richText = true;
            LabelCenter.ApplyStyle(TextAnchor.MiddleCenter, FontStyle.Normal, FontSize, true, textColorsArray);
            LabelCenter.richText = true;
            //SelectionGrid
            SelectionGrid.ApplyStyle(TextAnchor.MiddleCenter, FontStyle.Normal, FontSize, false, textColorsArray);
            SelectionGrid.richText = true;
            SelectionGrid.normal.background = Textures.TextureCache[ElementType.SelectionGrid][0];
            SelectionGrid.hover.background = Textures.TextureCache[ElementType.SelectionGrid][1];
            SelectionGrid.active.background = Textures.TextureCache[ElementType.SelectionGrid][2];
            SelectionGrid.onNormal.background = Textures.TextureCache[ElementType.SelectionGrid][3];
            SelectionGrid.onHover.background = Textures.TextureCache[ElementType.SelectionGrid][4];
            SelectionGrid.onActive.background = Textures.TextureCache[ElementType.SelectionGrid][5];
            //Slider
            Slider.normal.background = Textures.TextureCache[ElementType.Slider][0];
            Slider.hover.background = Textures.TextureCache[ElementType.Slider][1];
            Slider.active.background = Textures.TextureCache[ElementType.Slider][2];
            Slider.onNormal.background = Textures.TextureCache[ElementType.Slider][3];
            Slider.onHover.background = Textures.TextureCache[ElementType.Slider][4];
            Slider.onActive.background = Textures.TextureCache[ElementType.Slider][5];
            //SliderBody
            SliderBody.normal.background = Textures.TextureCache[ElementType.SliderBody][0];
            SliderBody.hover.background = Textures.TextureCache[ElementType.SliderBody][1];
            SliderBody.active.background = Textures.TextureCache[ElementType.SliderBody][2];
            SliderBody.onNormal.background = Textures.TextureCache[ElementType.SliderBody][3];
            SliderBody.onHover.background = Textures.TextureCache[ElementType.SliderBody][4];
            SliderBody.onActive.background = Textures.TextureCache[ElementType.SliderBody][5];
            SliderBody.fixedWidth = Height;
            SliderBody.fixedHeight = Height;
            //TextField
            TextField.ApplyStyle(TextAnchor.MiddleLeft, FontStyle.Normal, FontSize, false, textColorsArray);
            TextField.richText = false;
            TextField.clipping = TextClipping.Clip;
            TextField.normal.background = Textures.TextureCache[ElementType.TextField][0];
            TextField.hover.background = Textures.TextureCache[ElementType.TextField][1];
            TextField.active.background = Textures.TextureCache[ElementType.TextField][2];
            //Toggle
            Toggle.normal.background = Textures.TextureCache[ElementType.Toggle][0];
            Toggle.hover.background = Textures.TextureCache[ElementType.Toggle][1];
            Toggle.active.background = Textures.TextureCache[ElementType.Toggle][2];
            Toggle.onNormal.background = Textures.TextureCache[ElementType.Toggle][3];
            Toggle.onHover.background = Textures.TextureCache[ElementType.Toggle][4];
            Toggle.onActive.background = Textures.TextureCache[ElementType.Toggle][5];
            //ToggleButton
            TextButton.ApplyStyle(TextAnchor.MiddleRight, FontStyle.Normal, FontSize, true, new Color[6] { white, orange, yellow, white, white, white });
            TextButton.richText = true;
            TextButton.normal.background = GUIBase.EmptyTexture;
            TextButton.normal = TextButton.hover = TextButton.active;
            SetFont(Font);
        }

        public static void Load()
        {
            using (ConfigFile config = new ConfigFile(Application.dataPath + "/Configuration/Visuals.ini", ':', false))
            {
                using (ConfigFile settings = new ConfigFile(Application.dataPath + "/Configuration/Settings.ini", ' ', false))
                {
                    settings.Load();
                    settings.AutoSave = true;
                    string[] res = settings.GetString("resolution").Split('x');
                    ScreenWidthDefault = (float)System.Convert.ToInt32(res[0]);
                    ScreenHeightDefault = (float)System.Convert.ToInt32(res[1]);
                    if (PlayerPrefs.GetInt("AnarchyModLaunched") == 0)
                    {
                        PlayerPrefs.SetInt("AnarchyModLaunched", 1);
                        float xScale = ScreenWidthDefault / 1920f;
                        float yScale = ScreenHeightDefault / 1080f;
                        float totalScale = (xScale + yScale) / 2f;
                        UIManager.HUDScaleGUI.Value = Mathf.Clamp(totalScale, 0.75f, 1.5f);
                        UIManager.HUDScaleGUI.Save();
                    }
                    ResetScreenParameters();
                }
                config.Load();
                config.AutoSave = false;
                FontName = config.GetString("font");
                Font = AnarchyAssets.Load<Font>(FontName);
                BackgroundHex = config.GetString("background");
                BackgroundTransparency = config.GetInt("backgroundTransparency");
                TextColors = new string[6];
                TextColors[0] = config.GetString("textNormal");
                TextColors[1] = config.GetString("textHover");
                TextColors[2] = config.GetString("textActive");
                TextColors[3] = config.GetString("textOnNormal");
                TextColors[4] = config.GetString("textOnHover");
                TextColors[5] = config.GetString("textOnActive");
                TextureDeltas = new Vector3[6];
                TextureDeltas[0] = config.GetString("normalVector").ParseVector3();
                TextureDeltas[1] = config.GetString("hoverVector").ParseVector3();
                TextureDeltas[2] = config.GetString("activeVector").ParseVector3();
                TextureDeltas[3] = config.GetString("onNormalVector").ParseVector3();
                TextureDeltas[4] = config.GetString("onHoverVector").ParseVector3();
                TextureDeltas[5] = config.GetString("onActiveVector").ParseVector3();
                if (!config.AllValues.ContainsKey("useVectors"))
                {
                    UseVectors = false;
                }
                else
                {
                    UseVectors = config.GetBool("useVectors");
                }
                TextureColors = new Color[6];
                if (!config.AllValues.ContainsKey("colorNormal") || !config.AllValues.ContainsKey("colorHover") || !config.AllValues.ContainsKey("colorActive") ||
                    !config.AllValues.ContainsKey("colorOnNormal") || !config.AllValues.ContainsKey("colorOnHover") || !config.AllValues.ContainsKey("colorOnActive"))
                {
                    UseVectors = true;
                    Color[] colors = Helper.TextureColors(BackgroundColor, 6);
                    for (int i = 0; i < 6; i++)
                    {
                        TextureColors[i] = colors[i];
                    }
                    UseVectors = false;
                }
                else
                {
                    TextureColors[0] = config.GetString("colorNormal").HexToColor();
                    TextureColors[1] = config.GetString("colorHover").HexToColor();
                    TextureColors[2] = config.GetString("colorActive").HexToColor();
                    TextureColors[3] = config.GetString("colorOnNormal").HexToColor();
                    TextureColors[4] = config.GetString("colorOnHover").HexToColor();
                    TextureColors[5] = config.GetString("colorOnActive").HexToColor();
                }
                LoadPublicSettings();
            }
            wasLoaded = true;
            Initialize();
        }

        private static void LoadPublicSettings()
        {
            PublicSettings = new string[22];

            PublicSettings[0] = FontName;
            PublicSettings[1] = BackgroundHex;
            PublicSettings[2] = BackgroundTransparency.ToString();

            PublicSettings[3] = TextColors[0];
            PublicSettings[4] = TextColors[1];
            PublicSettings[5] = TextColors[2];
            PublicSettings[6] = TextColors[3];
            PublicSettings[7] = TextColors[4];
            PublicSettings[8] = TextColors[5];

            PublicSettings[9] = UseVectors.ToString();

            PublicSettings[10] = TextureDeltas[0].Vector3ToString();
            PublicSettings[11] = TextureDeltas[1].Vector3ToString();
            PublicSettings[12] = TextureDeltas[2].Vector3ToString();
            PublicSettings[13] = TextureDeltas[3].Vector3ToString();
            PublicSettings[14] = TextureDeltas[4].Vector3ToString();
            PublicSettings[15] = TextureDeltas[5].Vector3ToString();

            PublicSettings[16] = TextureColors[0].ColorToString();
            PublicSettings[17] = TextureColors[1].ColorToString();
            PublicSettings[18] = TextureColors[2].ColorToString();
            PublicSettings[19] = TextureColors[3].ColorToString();
            PublicSettings[20] = TextureColors[4].ColorToString();
            PublicSettings[21] = TextureColors[5].ColorToString();
        }

        public static void ResetScreenParameters()
        {
            ScreenHeight = ScreenHeightDefault;
            ScreenWidth = ScreenWidthDefault;
        }

        public static void Save()
        {
            if (!wasLoaded)
            {
                return;
            }
            TryApplyPublicSettings();
            using (ConfigFile config = new ConfigFile(Application.dataPath + "/Configuration/Visuals.ini", ':', false))
            {
                config.SetString("font", FontName);
                config.SetString("background", BackgroundHex);
                config.SetInt("backgroundTransparency", BackgroundTransparency);
                config.SetString("textNormal", TextColors[0]);
                config.SetString("textHover", TextColors[1]);
                config.SetString("textActive", TextColors[2]);
                config.SetString("textOnNormal", TextColors[3]);
                config.SetString("textOnHover", TextColors[4]);
                config.SetString("textOnActive", TextColors[5]);
                config.SetString("normalVector", TextureDeltas[0].Vector3ToString());
                config.SetString("hoverVector", TextureDeltas[1].Vector3ToString());
                config.SetString("activeVector", TextureDeltas[2].Vector3ToString());
                config.SetString("onNormalVector", TextureDeltas[3].Vector3ToString());
                config.SetString("onHoverVector", TextureDeltas[4].Vector3ToString());
                config.SetString("onActiveVector", TextureDeltas[5].Vector3ToString());
                config.SetBool("useVectors", UseVectors);
                config.SetString("colorNormal", TextureColors[0].ColorToString());
                config.SetString("colorHover", TextureColors[1].ColorToString());
                config.SetString("colorActive", TextureColors[2].ColorToString());
                config.SetString("colorOnNormal", TextureColors[3].ColorToString());
                config.SetString("colorOnHover", TextureColors[4].ColorToString());
                config.SetString("colorOnActive", TextureColors[5].ColorToString());
            }
        }

        public static void SetFont(Font font)
        {
            if (font == null)
            {
                return;
            }

            Font = font;
            for (int i = 0; i < allStyles.Count; i++)
            {
                allStyles[i].font = font;
            }
        }

        private static float SetScaling(FloatSetting set)
        {
            if (!UIManager.HUDAutoScaleGUI.Value)
            {
                return set.Value;
            }
            return (float)System.Math.Round(set.DefaultValue * UIManager.HUDScaleGUI.Value, 2);
        }

        private static int SetScaling(IntSetting set)
        {
            if (!UIManager.HUDAutoScaleGUI.Value)
            {
                return set.Value;
            }
            return Mathf.RoundToInt(set.DefaultValue * UIManager.HUDScaleGUI.Value);
        }

        private static void TryApplyPublicSettings()
        {
            FontName = PublicSettings[0];
            BackgroundHex = PublicSettings[1];
            BackgroundTransparency = System.Convert.ToInt32(PublicSettings[2]);
            UseVectors = System.Convert.ToBoolean(PublicSettings[9]);
            int j = 0;
            for (int i = 3; i < 9; i++)
            {
                TextColors[j++] = PublicSettings[i];
            }
            j = 0;
            for (int i = 10; i < 16; i++)
            {
                TextureDeltas[j++] = PublicSettings[i].ParseVector3();
            }
            j = 0;
            for (int i = 16; i < 22; i++)
            {
                TextureColors[j++] = PublicSettings[i].HexToColor();
            }
        }

        public static void UpdateScaling()
        {
            BigLabelOffset = SetScaling(BigLabelOffsetSetting);
            FontSize = SetScaling(FontSizeSetting);
            Height = SetScaling(HeightSetting);
            HorizontalMargin = SetScaling(HorizontalMarginSetting);
            VerticalMargin = SetScaling(VerticalMarginSetting);
            WindowHeight = SetScaling(WindowHeightSetting);
            WindowWidth = SetScaling(WindowWidthSetting);
            WindowBottomOffset = SetScaling(WindowBottomOffsetSetting);
            WindowSideOffset = SetScaling(WindowSideOffsetSetting);
            WindowTopOffset = SetScaling(WindowTopOffsetSetting);
            LabelOffset = SetScaling(LabelOffsetSetting);
            LabelOffsetSlider = SetScaling(LabelOffsetSliderSetting);
            LabelSpace = "";
            for (int i = 0; i < LabelSpaceSetting.Value; i++)
            {
                LabelSpace += " ";
            }
            Initialize();
        }
    }
}