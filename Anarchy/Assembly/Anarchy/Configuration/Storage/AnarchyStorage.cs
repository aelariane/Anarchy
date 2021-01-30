using Anarchy.IO;
using UnityEngine;

namespace Anarchy.Configuration.Storage
{
    public class AnarchyStorage : ConfigFile, IDataStorage
    {
        public AnarchyStorage() : base(Application.dataPath + "/Configuration/Settings.cfg", '`', true)
        {
            Load();
        }

        public AnarchyStorage(string path) : this(path, '`', true)
        {
        }

        public AnarchyStorage(string path, char separator) : this(path, separator, true)
        {
        }

        public AnarchyStorage(string path, char separator, bool autocreate) : base(path, separator, autocreate)
        {
        }

        public void Clear()
        {
            Delete();
            Create();
        }

        public bool GetBool(string key, bool def)
        {
            bool result;
            if (booleans.TryGetValue(key, out result))
            {
                return result;
            }
            string val;
            if (allValues.TryGetValue(key, out val))
            {
                if (bool.TryParse(val, out result))
                {
                    booleans.Add(key, result);
                    return result;
                }
                return def;
            }
            else
            {
                SetBool(key, def);
                return def;
            }
        }

        public float GetFloat(string key, float def)
        {
            float result;
            if (floats.TryGetValue(key, out result))
            {
                return result;
            }
            string val;
            if (allValues.TryGetValue(key, out val))
            {
                if (float.TryParse(val, out result))
                {
                    floats.Add(key, result);
                    return result;
                }
                return def;
            }
            else
            {
                SetFloat(key, def);
                return def;
            }
        }

        public int GetInt(string key, int def)
        {
            int result;
            if (integers.TryGetValue(key, out result))
            {
                return result;
            }
            string val;
            if (allValues.TryGetValue(key, out val))
            {
                if (int.TryParse(val, out result))
                {
                    integers.Add(key, result);
                    return result;
                }
                return def;
            }
            else
            {
                SetInt(key, def);
                return def;
            }
        }

        public string GetString(string key, string def)
        {
            string result = string.Empty;
            if (strings.TryGetValue(key, out result))
            {
                return result;
            }
            if (allValues.TryGetValue(key, out result))
            {
                strings.Add(key, result);
                return result;
            }
            else
            {
                SetString(key, def);
                return def;
            }
        }
    }
}