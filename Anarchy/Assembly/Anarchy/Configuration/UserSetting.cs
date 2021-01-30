using Anarchy.Configuration.Storage;
using System.Collections.Generic;

namespace Anarchy.Configuration
{
    public class UserSetting : Setting<string>
    {
        private const char separator = ',';

        public static AnarchyStorage Storage => storage;

        private static object locker = new object();
        private static AnarchyStorage storage;
        private static List<UserSetting> settings;

        public UserSetting(string key) : this(key, string.Empty)
        {
        }

        public UserSetting(string key, string defaultValue) : base(key, defaultValue, false)
        {
            if (settings == null)
            {
                settings = new List<UserSetting>();
            }

            lock (locker)
            {
                settings.Add(this);
            }
        }

        ~UserSetting()
        {
            lock (locker)
            {
                if (settings.Contains(this))
                {
                    settings.Remove(this);
                }
            }
        }

        public static void Change(string newPath)
        {
            if (storage != null)
            {
                SaveSettings();
                storage.Dispose();
            }
            storage = new AnarchyStorage(newPath, '`', false);
            LoadSettings();
        }

        public string[] GetArray()
        {
            if (Value == null)
            {
                return new string[0];
            }

            return Value.Split(separator);
        }

        public override void Load()
        {
            if (storage == null)
            {
                return;
            }

            Value = storage.GetString(Key, DefaultValue);
        }

        public static void LoadSettings()
        {
            storage.Load();
            lock (locker)
            {
                for (int i = 0; i < settings.Count; i++)
                {
                    settings[i].Load();
                }
            }
            storage.Unload();
        }

        public string PickRandomString()
        {
            string[] res = GetArray();
            if (res.Length > 0)
            {
                return res[UnityEngine.Random.Range(0, res.Length)];
            }
            return null;
        }

        public override void Save()
        {
            if (storage == null)
            {
                return;
            }

            storage.SetString(Key, Value);
        }

        public static void SaveSettings()
        {
            lock (locker)
            {
                for (int i = 0; i < settings.Count; i++)
                {
                    settings[i].Save();
                }
            }
            storage.Save();
        }
    }
}