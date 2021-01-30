using Anarchy.IO;
using System.Collections.Generic;

namespace Anarchy.Localization
{
    public class Locale
    {
        public const string Extension = ".lang";

        private bool custom = false;
        public readonly string Element;
        public readonly string MyLanguage;
        private Dictionary<string, string> localizedText = new Dictionary<string, string>();
        private readonly Dictionary<string, string[]> localizedTextArrayCache = new Dictionary<string, string[]>();
        private readonly object locker = new object();
        private bool notAllowClose = false;
        public readonly char Separator;

        public bool AlwaysOpen { get; set; } = false;
        public bool Formateable { get; set; }
        public bool IsOpen { get; private set; }
        public string Path { get; private set; }

        public Locale(string element) : this(element, false)
        {
        }

        public Locale(string element, bool format) : this(element, format, ',')
        {
        }

        public Locale(string element, bool format, char separator)
        {
            Formateable = format;
            Separator = separator;
            Element = element;
            Path = Language.Directory + Element + Extension;
            Load();
            if (format)
            {
                FormatColors();
            }
            MyLanguage = Language.SelectedLanguage;
            Language.AddLocale(this);
        }

        public Locale(string lang, string element, bool format, char separator)
        {
            Formateable = format;
            Separator = separator;
            Element = element;
            MyLanguage = lang;
            Path = Language.Path + lang + "/" + Element + Extension;
            Load();
            if (format)
            {
                FormatColors();
            }
            custom = true;
            Language.AddLocale(this);
        }

        ~Locale()
        {
            Unload();
            Language.RemoveLocale(this);
        }

        public string Format(string key, params string[] values)
        {
            return string.Format(this[key], values);
        }

        public string Format(string key, string str)
        {
            return string.Format(this[key], str);
        }

        public void FormatColors()
        {
            if (!Formateable)
            {
                return;
            }
            var temp = new Dictionary<string, string>();
            foreach (KeyValuePair<string, string> pair in localizedText)
            {
                temp.Add(pair.Key, User.FormatColors(pair.Value));
            }
            localizedText = temp;
        }

        public string Get(string key)
        {
            return this[key];
        }

        public string[] GetArray(string key)
        {
            string[] result;
            if (localizedTextArrayCache.TryGetValue(key, out result))
            {
                return result;
            }
            try
            {
                result = localizedText[key].Split(Separator);
                localizedTextArrayCache.Add(key, result);
            }
            catch
            {
                UnityEngine.Debug.Log("Invalid key: " + key);
                throw;
            }
            return result;
        }

        public void KeepOpen(int seconds)
        {
            if (!AlwaysOpen)
            {
                new System.Threading.Thread(() =>
                {
                    if (!IsOpen)
                    {
                        Load();
                    }
                    notAllowClose = true;
                    System.Threading.Thread.Sleep(seconds * 1000);
                    notAllowClose = false;
                    Unload();
                })
                { IsBackground = true }.Start();
            }
        }

        public void Load()
        {
            if (!System.IO.File.Exists(Path))
            {
                return;
            }
            lock (locker)
            {
                localizedText.Clear();
                localizedTextArrayCache.Clear();
                IsOpen = false;
                using (ConfigFile config = new ConfigFile(Path, ':', false))
                {
                    config.AutoSave = false;
                    config.Load();
                    foreach (KeyValuePair<string, string> pair in config.AllValues)
                    {
                        localizedText.Add(pair.Key, pair.Value.Replace(@"\n", System.Environment.NewLine));
                    }
                    if (Formateable)
                    {
                        FormatColors();
                    }
                }
                IsOpen = true;
            }
        }

        public void Reload()
        {
            if (!custom)
            {
                Path = Language.Directory + Element + Extension;
            }
            Load();
        }

        public void Unload()
        {
            if (AlwaysOpen || notAllowClose)
            {
                return;
            }
            lock (locker)
            {
                localizedText.Clear();
                localizedTextArrayCache.Clear();
                IsOpen = false;
            }
        }

        public string this[string key]
        {
            get
            {
                if (localizedText.ContainsKey(key))
                {
                    return localizedText[key];
                }

                return $"?{key}?";
            }
        }
    }
}