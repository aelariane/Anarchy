using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Anarchy.Localization
{
    public static class Language
    {
        public const string DefaultLanguage = "English";
        public static readonly string Path = Application.dataPath + "/Localization/";

        public static List<Locale> AllLocales => allLocales;
        private static List<Locale> allLocales;

        public static string[] AllLanguages
        {
            get
            {
                DirectoryInfo[] langs = new DirectoryInfo(Path).GetDirectories();
                string[] res = new string[langs.Length];
                for (int i = 0; i < langs.Length; i++)
                {
                    res[i] = langs[i].Name;
                }
                return res;
            }
        }

        public static string Directory { get; private set; } = "None";
        public static string SelectedLanguage { get; private set; } = DefaultLanguage;

        public static void AddLocale(Locale loc)
        {
            if (allLocales == null)
            {
                allLocales = new List<Locale>();
            }

            lock (allLocales)
            {
                if (!allLocales.Contains(loc))
                {
                    allLocales.Add(loc);
                }
            }
        }

        public static Locale Find(string name)
        {
            lock (allLocales)
            {
                foreach (Locale loc in allLocales)
                {
                    if (loc.Element == name && loc.MyLanguage == SelectedLanguage)
                    {
                        return loc;
                    }
                }
                return null;
            }
        }

        public static void Reload()
        {
            if (allLocales == null)
            {
                return;
            }

            lock (allLocales)
            {
                foreach (Locale loc in allLocales)
                {
                    loc.Reload();
                }
            }
        }

        public static void RemoveLocale(Locale loc)
        {
            if (allLocales == null)
            {
                return;
            }

            lock (allLocales)
            {
                if (allLocales.Contains(loc))
                {
                    allLocales.Remove(loc);
                }
            }
        }

        public static void SetLanguage(string lang)
        {
            if (AllLanguages.Contains(lang))
            {
                Directory = Path + lang + "/";
                SelectedLanguage = lang;
                Reload();
                return;
            }
            Debug.LogError("Not found language: " + lang);
            SetLanguage(DefaultLanguage);
        }

        public static void UpdateFormats()
        {
            if (allLocales == null)
            {
                return;
            }

            lock (allLocales)
            {
                foreach (Locale loc in allLocales)
                {
                    if (loc.Formateable)
                    {
                        loc.Reload();
                        loc.FormatColors();
                    }
                }
            }
        }
    }
}
