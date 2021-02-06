namespace Anarchy.UI
{
    /// <summary>
    /// Type of GUIPage method
    /// </summary>
    public enum GUIPageType
    {
        /// <summary>
        /// Calls when page was disabled
        /// </summary>
        DisableMethod = 1,
        /// <summary>
        /// Draws the page
        /// </summary>
        DrawMethod = 0,
        /// <summary>
        /// Calls when page was enabled
        /// </summary>
        EnableMethod = 2
    }
}