using UnityEngine;

namespace Anarchy.Configuration.Storage
{
    public class PrefStorage : IDataStorage
    {
        public void Clear()
        {
            PlayerPrefs.DeleteAll();
        }

        public void Dispose()
        {
        }

        public bool GetBool(string key, bool def)
        {
            return PlayerPrefs.GetInt(key, def ? 1 : 0) == 1;
        }

        public float GetFloat(string key, float def)
        {
            return PlayerPrefs.GetFloat(key, def);
        }

        public int GetInt(string key, int def)
        {
            return PlayerPrefs.GetInt(key, def);
        }

        public string GetString(string key, string def)
        {
            return PlayerPrefs.GetString(key, def);
        }

        public void Save()
        {
        }

        public void SetBool(string key, bool value)
        {
            PlayerPrefs.SetInt(key, value ? 1 : 0);
        }

        public void SetFloat(string key, float value)
        {
            PlayerPrefs.SetFloat(key, value);
        }

        public void SetInt(string key, int value)
        {
            PlayerPrefs.SetInt(key, value);
        }

        public void SetString(string key, string value)
        {
            PlayerPrefs.SetString(key, value);
        }
    }
}