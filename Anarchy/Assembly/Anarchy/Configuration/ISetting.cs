namespace Anarchy.Configuration
{
    /// <summary>
    /// Base interface for setting
    /// </summary>
    public interface ISetting
    {
        /// <summary>
        /// Loads the Setting
        /// </summary>
        void Load();
        /// <summary>
        /// Saves the Setting
        /// </summary>
        void Save();
    }
}