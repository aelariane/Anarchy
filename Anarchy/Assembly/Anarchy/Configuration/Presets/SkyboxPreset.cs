using Anarchy.Configuration.Storage;
using Anarchy.Localization;
using Anarchy.UI;
using System.IO;
using System.Linq;
using UnityEngine;
using static Anarchy.UI.GUI;

namespace Anarchy.Configuration.Presets
{
    public class SkyboxPreset : SkinPreset
    {
        public static readonly string SkyboxPath = Application.dataPath + "/Configuration/SkyboxSkins/";
        private const int Length = 6;
        private static readonly string[] labels = new string[Length] { "Front", "Back", "Left", "Right", "Up", "Down" };

        private string[] data;

        public string SkyboxUp => data[0];
        public string SkyboxDown => data[1];
        public string SkyboxLeft => data[2];
        public string SkyboxRight => data[3];
        public string SkyboxFront => data[4];
        public string SkyboxBack => data[5];

        public SkyboxPreset(string name) : base(name, SkyboxPath)
        {
            data = new string[Length].Select(x => string.Empty).ToArray();
        }

        public override void Draw(SmartRect rect, Locale locale)
        {
            for (int i = 0; i < Length; i++)
            {
                LabelCenter(rect, locale["skybox" + labels[i]], true);
                data[i] = TextField(rect, data[i], string.Empty, 0f, true);
            }
            rect.MoveY();
            if (SkinSettings.CitySet.Value != Anarchy.Configuration.StringSetting.NotDefine)
            {
                if (Button(rect, locale.Format("btnLinkCity", SkinSettings.CitySet.Value), true))
                {
                    LinkToCitySet(SkinSettings.CitySet.Value);
                }
            }
            if (SkinSettings.ForestSet.Value != Anarchy.Configuration.StringSetting.NotDefine)
            {
                if (Button(rect, locale.Format("btnLinkForest", SkinSettings.ForestSet.Value), true))
                {
                    LinkToForestSet(SkinSettings.ForestSet.Value);
                }
            }
        }

        public void LinkToCitySet(string setName)
        {
            CityPreset set = new CityPreset(setName);
            if (set.Exists())
            {
                set.Load();
                set.LinkedSkybox = Name;
            }
            set.Save();
        }

        public void LinkToForestSet(string setName)
        {
            ForestPreset set = new ForestPreset(setName);
            if (set.Exists())
            {
                set.Load();
                set.LinkedSkybox = Name;
            }
            set.Save();
        }

        public override void Load()
        {
            using (var file = new AnarchyStorage(FullPath, '`', false))
            {
                file.Load();
                file.AutoSave = false;
                data[0] = file.GetString("skyboxUp", string.Empty);
                data[1] = file.GetString("skyboxDown", string.Empty);
                data[2] = file.GetString("skyboxLeft", string.Empty);
                data[3] = file.GetString("skyboxRight", string.Empty);
                data[4] = file.GetString("skyboxFront", string.Empty);
                data[5] = file.GetString("skyboxBack", string.Empty);
            }
        }

        public override void Save()
        {
            using (var file = new AnarchyStorage(FullPath, '`', true))
            {
                file.SetString("skyboxUp", data[0]);
                file.SetString("skyboxDown", data[1]);
                file.SetString("skyboxLeft", data[2]);
                file.SetString("skyboxRight", data[3]);
                file.SetString("skyboxFront", data[4]);
                file.SetString("skyboxBack", data[5]);
            }
        }

        public static SkinPreset[] GetAllPresets()
        {
            DirectoryInfo info = new DirectoryInfo(SkyboxPath);
            FileInfo[] files = info.GetFiles();
            if (files.Length == 0)
            {
                return null;
            }

            SkinPreset[] result = new SkinPreset[files.Length];
            for (int i = 0; i < files.Length; i++)
            {
                result[i] = new SkyboxPreset(files[i].Name.Replace(Extension, string.Empty));
                result[i].Load();
            }
            return result;
        }

        public override string[] ToSkinData()
        {
            string[] result = new string[data.Length];
            for (int i = 0; i < data.Length; i++)
            {
                result[i] = data[i];
            }
            return result;
        }
    }
}