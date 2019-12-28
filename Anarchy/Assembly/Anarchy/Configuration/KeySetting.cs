using UnityEngine;

namespace Anarchy.Configuration
{
    public class KeySetting : Setting<KeyCode>
    {
        public KeySetting(string key, KeyCode defaultValue) : base(key, defaultValue, true)
        {
            Load();
            InputManager.AddToList(this);
        }

        public bool IsHolding()
        {
            return Input.GetKey(Value);
        }

        public bool IsPressed()
        {
            return Input.GetKeyDown(Value);
        }

        public bool IsUp()
        {
            return Input.GetKeyUp(Value);
        }

        public override void Load()
        {
            Value = (KeyCode)Settings.Storage.GetInt("KeyCode/" + Key, (int)DefaultValue);
        }

        public override void Save()
        {
            Settings.Storage.SetInt("KeyCode/" + Key, (int)Value);
        }
    }
}
