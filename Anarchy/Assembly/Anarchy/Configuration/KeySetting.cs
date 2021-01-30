using UnityEngine;

namespace Anarchy.Configuration
{
    public class KeySetting : Setting<KeyCode>
    {
        public static readonly KeyCode AxisUp = KeyCode.JoystickButton14;
        public static readonly KeyCode AxisDown = KeyCode.JoystickButton15;

        private bool isAxis = false;
        private float modifier = 0f;

        public readonly string LanguageFileKey;

        public KeySetting(string key, KeyCode defaultValue) : base(key, defaultValue, true)
        {
            Load();
            LanguageFileKey = key.Replace(" ", string.Empty);
            InputManager.AddToList(this);
        }

        private bool CheckAxis()
        {
            return Input.GetAxis("Mouse ScrollWheel") * modifier > 0f;
        }

        public bool IsKeyHolding()
        {
            if (Value == KeyCode.None)
            {
                return false;
            }
            if (isAxis)
            {
                return CheckAxis();
            }
            return Input.GetKey(Value);
        }

        public bool IsKeyDown()
        {
            if (Value == KeyCode.None)
            {
                return false;
            }
            if (isAxis)
            {
                return CheckAxis();
            }
            return Input.GetKeyDown(Value);
        }

        public bool IsKeyUp()
        {
            if (Value == KeyCode.None)
            {
                return false;
            }
            if (isAxis)
            {
                return CheckAxis();
            }
            return Input.GetKeyUp(Value);
        }

        public override void Load()
        {
            Value = (KeyCode)Settings.Storage.GetInt("KeyCode/" + Key, (int)DefaultValue);
            if (Value == AxisUp || Value == AxisDown)
            {
                isAxis = true;
                if (Value == AxisUp)
                {
                    modifier = 1f;
                }
                else
                {
                    modifier = -1f;
                }
            }
        }

        public override void Save()
        {
            Settings.Storage.SetInt("KeyCode/" + Key, (int)Value);
        }

        public void SetAsAxis(bool up)
        {
            isAxis = true;
            if (up)
            {
                Value = AxisUp;
                modifier = 1f;
            }
            else
            {
                Value = AxisDown;
                modifier = -1f;
            }
        }

        public void SetValue(KeyCode val)
        {
            Value = val;
            isAxis = false;
            modifier = 0f;
        }

        public override string ToString()
        {
            if (isAxis)
            {
                if (modifier > 0f)
                {
                    return "Scroll Up";
                }
                else
                {
                    return "Scroll Down";
                }
            }
            return Value.ToString();
        }
    }
}