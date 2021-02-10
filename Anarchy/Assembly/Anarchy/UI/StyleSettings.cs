using Anarchy.Configuration;

namespace Anarchy.UI
{
    /// <summary>
    /// Contains settings for Anarchy Style
    /// </summary>
    public class StyleSettings
    {
        /// <summary>
        /// Biggest label offset
        /// </summary>
        public static readonly FloatSetting BigLabelOffset = new FloatSetting("GlobalBigLabelOffset", StyleDefaults.BigLabelOffset);
        /// <summary>
        /// Size of GUI Font
        /// </summary>
        public static readonly IntSetting FontSize = new IntSetting("GlobalFontSize", StyleDefaults.FontSize);
        /// <summary>
        /// Unified Height of GUI elements
        /// </summary>
        public static readonly FloatSetting Height = new FloatSetting("GlobalHeight", StyleDefaults.ElementHeight);
        /// <summary>
        /// Horizontal distance between elements
        /// </summary>
        public static readonly IntSetting HorizontalMargin = new IntSetting("GlobalHorizontalMargin", StyleDefaults.HorizontalMargin);
        /// <summary>
        /// Vertical distance between elements
        /// </summary>
        public static readonly IntSetting VerticalMargin = new IntSetting("GlobalVerticalMargin", StyleDefaults.VerticalMargin);
        /// <summary>
        /// Height of unified window
        /// </summary>
        /// <remarks><seealso cref="GUIPanel"/> applies this settings to its Window by default</remarks>
        public static readonly FloatSetting WindowHeight = new FloatSetting("GlobalWindowHeight", StyleDefaults.WindowHeight);
        /// <summary>
        /// Width of unified window
        /// </summary>
        /// <remarks><seealso cref="GUIPanel"/> applies this settings to its Window by default</remarks>
        public static readonly FloatSetting WindowWidth = new FloatSetting("GlobalWindowWidth", StyleDefaults.WindowWidth);
        /// <summary>
        /// Distance between bottom Window border and content inside the Window
        /// </summary>
        /// <remarks><seealso cref="GUIPanel"/> applies this settings to its Window by default</remarks>
        public static readonly FloatSetting WindowBottomOffset = new FloatSetting("GlobalWindowBottomOffset", StyleDefaults.WindowBottom);
        /// <summary>
        /// Distance between Window sides and content inside the Window
        /// </summary>
        /// <remarks><seealso cref="GUIPanel"/> applies this settings to its Window by default</remarks>
        public static readonly FloatSetting WindowSideOffset = new FloatSetting("GlobalWindowSideOffset", StyleDefaults.WindowSideOffset);
        /// <summary>
        /// Distance between top border of Window and content inside the Window
        /// </summary>
        /// <remarks><seealso cref="GUIPanel"/> applies this settings to its Window by default</remarks>
        public static readonly FloatSetting WindowTopOffset = new FloatSetting("GlobalWindowTopOffset", StyleDefaults.WindowTopOffset);
        /// <summary>
        /// Width of Label when used in combination with element
        /// </summary>
        public static readonly FloatSetting LabelOffset = new FloatSetting("GlobalLabelOffset", StyleDefaults.LabelOffset);
        /// <summary>
        /// Width of label when used with Slider in one line
        /// </summary>
        public static readonly FloatSetting LabelOffsetSlider = new FloatSetting("LabelOffsetSlider", StyleDefaults.LabelOffsetSlider);
        /// <summary>
        /// Count of space symbols for "daughter" string (Placed below main content and a bit further, to specify that this is just additional text)
        /// </summary>
        public static readonly IntSetting LabelSpace = new IntSetting("GlobalLabelSpaceCount", StyleDefaults.LabelSpace);
        /// <summary>
        /// Selected font
        /// </summary>
        public static readonly IntSetting FontSelection = new IntSetting("GlobalFontName", StyleDefaults.FontIndex);
        /// <summary>
        /// Tramsparency of background
        /// </summary>
        /// <remarks>By Background Color it means default color of Windows used by <seealso cref="GUIPanel"/> by default</remarks>
        public static readonly FloatSetting BackgroundTransparency = new FloatSetting("GlobalBackgroundTransparency", StyleDefaults.BackgroundTransparency);
    }
}
