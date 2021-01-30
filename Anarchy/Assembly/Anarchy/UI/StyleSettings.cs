using Anarchy.Configuration;

namespace Anarchy.UI
{
    public class StyleSettings
    {
        public static FloatSetting BigLabelOffsetSetting = new FloatSetting("GlobalBigLabelOffset", StyleDefaults.BigLabelOffset);
        public static IntSetting FontSizeSetting = new IntSetting("GlobalFontSize", StyleDefaults.FontSize);
        public static FloatSetting HeightSetting = new FloatSetting("GlobalHeight", StyleDefaults.ElementHeight);
        public static IntSetting HorizontalMarginSetting = new IntSetting("GlobalHorizontalMargin", StyleDefaults.HorizontalMargin);
        public static IntSetting VerticalMarginSetting = new IntSetting("GlobalVerticalMargin", StyleDefaults.VerticalMargin);
        public static FloatSetting WindowHeightSetting = new FloatSetting("GlobalWindowHeight", StyleDefaults.WindowHeight);
        public static FloatSetting WindowWidthSetting = new FloatSetting("GlobalWindowWidth", StyleDefaults.WindowWidth);
        public static FloatSetting WindowBottomOffsetSetting = new FloatSetting("GlobalWindowBottomOffset", StyleDefaults.WindowBottom);
        public static FloatSetting WindowSideOffsetSetting = new FloatSetting("GlobalWindowSideOffset", StyleDefaults.WindowSideOffset);
        public static FloatSetting WindowTopOffsetSetting = new FloatSetting("GlobalWindowTopOffset", StyleDefaults.WindowTopOffset);
        public static FloatSetting LabelOffsetSetting = new FloatSetting("GlobalLabelOffset", StyleDefaults.LabelOffset);
        public static FloatSetting LabelOffsetSliderSetting = new FloatSetting("LabelOffsetSlider", StyleDefaults.LabelOffsetSlider);
        public static IntSetting LabelSpaceSetting = new IntSetting("GlobalLabelSpaceCount", StyleDefaults.LabelSpace);
        public static IntSetting FontSetting = new IntSetting("GlobalFontName", StyleDefaults.FontIndex);
        public static FloatSetting BackgroundTransparencySetting = new FloatSetting("GlobalBackgroundTransparency", StyleDefaults.BackgroundTransparency);
    }
}
