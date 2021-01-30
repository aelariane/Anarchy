namespace Anarchy.Configuration
{
    public class BoolSetting : Setting<bool>
    {
        public BoolSetting(string key) : base(key)
        {
        }

        public BoolSetting(string key, bool defaultValue) : base(key, defaultValue)
        {
        }

        public override void Load()
        {
            Value = Settings.Storage.GetBool(Key, DefaultValue);
        }

        public override void Save()
        {
            Settings.Storage.SetBool(Key, Value);
        }
    }
}