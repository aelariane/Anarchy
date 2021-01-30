namespace Anarchy.Configuration
{
    public class IntSetting : Setting<int>
    {
        public IntSetting(string key) : base(key)
        {
        }

        public IntSetting(string key, int defaultValue) : base(key, defaultValue)
        {
        }

        public override void Load()
        {
            Value = Settings.Storage.GetInt(Key, DefaultValue);
        }

        public override void Save()
        {
            Settings.Storage.SetInt(Key, Value);
        }
    }
}