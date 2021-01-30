using System.Collections.Generic;
using UnityEngine;

namespace Anarchy.IO
{
    public class ConfigFile : File
    {
        private readonly char Separator = '`';

        protected readonly Dictionary<string, string> allValues = new Dictionary<string, string>();
        protected readonly Dictionary<string, bool> booleans = new Dictionary<string, bool>();
        protected readonly Dictionary<string, float> floats = new Dictionary<string, float>();
        protected readonly Dictionary<string, int> integers = new Dictionary<string, int>();
        protected readonly Dictionary<string, string> strings = new Dictionary<string, string>();

        public bool AutoSave { get; set; } = true;

        public Dictionary<string, string> AllValues
        {
            get
            {
                return allValues;
            }
        }

        public ConfigFile(string path, char separator = '`') : this(path, separator, true)
        {
        }

        public ConfigFile(string path, char separator, bool autocreate) : base(path, false, autocreate)
        {
            Separator = separator;
        }

        public override void Dispose()
        {
            if (AutoSave)
            {
                Save();
            }

            base.Dispose();
        }

        #region Get variables

        public bool GetBool(string key)
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
                }
            }
            return result;
        }

        public float GetFloat(string key)
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
                }
            }
            return result;
        }

        public int GetInt(string key)
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
                }
            }
            return result;
        }

        public string GetString(string key)
        {
            string result = string.Empty;
            if (strings.TryGetValue(key, out result))
            {
                return result;
            }
            if (allValues.TryGetValue(key, out result))
            {
                strings.Add(key, result);
            }
            else
            {
                result = string.Empty;
            }
            return result;
        }

        #endregion Get variables

        public void Load()
        {
            if (!System.IO.File.Exists(Path))
            {
                Debug.LogError($"ConfigFile Error: There is no file {Path}.");
                return;
            }

            allValues.Clear();
            booleans.Clear();
            floats.Clear();
            integers.Clear();
            strings.Clear();
            string[] allStrings = System.IO.File.ReadAllLines(Path);
            foreach (string str in allStrings)
            {
                if (str.StartsWith("#") || str.Equals(string.Empty))
                {
                    continue;
                }
                string[] add = ParseString(str);
                if (add == null)
                {
                    continue;
                }
                allValues.Add(add[0], add[1]);
            }
        }

        private string[] ParseString(string val)
        {
            string[] parse = val.Split(Separator);
            if (parse.Length < 2)
            {
                Debug.LogError("Config line without separator found. " + val + ". Path: " + Path);
                return null;
            }
            if (parse.Length == 2)
            {
                return parse;
            }
            string[] result = new string[2];
            result[0] = parse[0];
            result[1] = val.Substring(parse[0].Length + 1);
            return result;
        }

        private void RefreshValue(string key, string newValue)
        {
            if (allValues.ContainsKey(key))
            {
                allValues[key] = newValue;
                return;
            }
            allValues.Add(key, newValue);
        }

        public void Save()
        {
            StoreValues();
            using (textWriter = info.CreateText())
            {
                foreach (var pair in allValues)
                {
                    textWriter.Write(pair.Key);
                    textWriter.Write(Separator);
                    textWriter.WriteLine(pair.Value);
                    textWriter.Flush();
                }
            }
        }

        #region Set variables

        public void SetBool(string key, bool val)
        {
            if (booleans.ContainsKey(key))
            {
                booleans[key] = val;
                return;
            }
            booleans.Add(key, val);
        }

        public void SetFloat(string key, float val)
        {
            if (floats.ContainsKey(key))
            {
                floats[key] = val;
                return;
            }
            floats.Add(key, val);
        }

        public void SetInt(string key, int val)
        {
            if (integers.ContainsKey(key))
            {
                integers[key] = val;
                return;
            }
            integers.Add(key, val);
        }

        public void SetString(string key, string val)
        {
            if (strings.ContainsKey(key))
            {
                strings[key] = val;
                return;
            }
            strings.Add(key, val);
        }

        public void StoreValues()
        {
            foreach (var pair in booleans)
            {
                RefreshValue(pair.Key, pair.Value.ToString());
            }
            foreach (var pair in floats)
            {
                RefreshValue(pair.Key, pair.Value.ToString());
            }
            foreach (var pair in integers)
            {
                RefreshValue(pair.Key, pair.Value.ToString());
            }
            foreach (var pair in strings)
            {
                RefreshValue(pair.Key, pair.Value.ToString());
            }
        }

        #endregion Set variables

        public void Unload()
        {
            allValues.Clear();
            booleans.Clear();
            floats.Clear();
            integers.Clear();
            strings.Clear();
        }
    }
}