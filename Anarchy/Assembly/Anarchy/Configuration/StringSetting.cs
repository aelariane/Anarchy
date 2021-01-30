namespace Anarchy.Configuration
{
    public class StringSetting : Setting<string>
    {
        public const string NotDefine = "$Not define$";

        public StringSetting(string key) : base(key)
        {
        }

        public StringSetting(string key, string defaultValue) : base(key, defaultValue)
        {
        }

        public override void Load()
        {
            Value = Settings.Storage.GetString(Key, DefaultValue);
            if (Value.Equals("$null"))
            {
                Value = null;
            }
        }

        public override void Save()
        {
            Settings.Storage.SetString(Key, Value ?? "$null");
        }
    }
}