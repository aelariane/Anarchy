using Anarchy.Configuration.Storage;
using System.IO;
using UnityEngine;

namespace Anarchy.Configuration.Presets
{
    public class ServerPreset : ISetting
    {
        private const string Extension = ".serverpreset";
        private static readonly string Path = Application.dataPath + "/Configuration/ServerPresets/";

        public int Daylight;
        public int Difficulity;
        public int Map;
        public string Name;
        public string Password;
        public string Players;
        public string ServerName;
        public string Time;

        public ServerPreset(string name)
        {
            Name = name;
        }

        public void Delete()
        {
            FileInfo info = new FileInfo(Path + Name + Extension);
            if (info.Exists)
            {
                info.Delete();
            }
        }

        public void Load()
        {
            using (var storage = new AnarchyStorage(Path + Name + Extension, '`', true))
            {
                storage.AutoSave = false;
                storage.Load();
                Daylight = storage.GetInt(nameof(Daylight), 0);
                Difficulity = storage.GetInt(nameof(Difficulity), 0);
                Map = storage.GetInt(nameof(Map), 0);
                Password = storage.GetString(nameof(Password), string.Empty);
                Players = storage.GetString(nameof(Players), "5");
                ServerName = storage.GetString(nameof(ServerName), "Food for fate");
                Time = storage.GetString(nameof(Time), "60");
            }
        }

        public static ServerPreset[] LoadPresets()
        {
            DirectoryInfo info = new DirectoryInfo(Path);
            FileInfo[] files = info.GetFiles();
            if (files.Length == 0)
            {
                return null;
            }

            ServerPreset[] result = new ServerPreset[files.Length];
            for (int i = 0; i < files.Length; i++)
            {
                result[i] = new ServerPreset(files[i].Name.Replace(Extension, string.Empty));
                result[i].Load();
            }
            return result;
        }

        public void Save()
        {
            using (var storage = new AnarchyStorage(Path + Name + Extension, '`', true))
            {
                storage.SetInt(nameof(Daylight), Daylight);
                storage.SetInt(nameof(Difficulity), Difficulity);
                storage.SetInt(nameof(Map), Map);
                storage.SetString(nameof(Password), Password);
                storage.SetString(nameof(Players), Players);
                storage.SetString(nameof(ServerName), ServerName);
                storage.SetString(nameof(Time), Time);
                storage.Save();
            }
        }
    }
}