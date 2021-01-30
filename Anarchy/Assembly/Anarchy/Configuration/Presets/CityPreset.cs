using Anarchy.Configuration.Storage;
using Anarchy.Localization;
using Anarchy.UI;
using System.IO;
using System.Linq;
using UnityEngine;
using static Anarchy.UI.GUI;

namespace Anarchy.Configuration.Presets
{
    public class CityPreset : SkinPreset
    {
        public static readonly string CityPath = Application.dataPath + "/Configuration/CitySkins/";
        private const int Length = 11;

        private string[] data;

        public string Gate => data[2];
        public string Ground => data[0];
        public string LinkedSkybox { get; set; }
        public string Wall => data[1];

        public string[] Houses
        {
            get
            {
                string[] result = new string[data.Length - 3];
                for (int i = 3; i < data.Length; i++)
                {
                    result[i - 3] = data[i];
                }
                return result;
            }
        }

        public CityPreset(string name) : base(name, CityPath)
        {
            data = new string[Length].Select(x => string.Empty).ToArray();
            LinkedSkybox = Anarchy.Configuration.StringSetting.NotDefine;
        }

        public override void Draw(SmartRect rect, Locale locale)
        {
            int index = 0;
            if (LinkedSkybox != Anarchy.Configuration.StringSetting.NotDefine)
            {
                Label(rect, locale.Format("linkedBox", LinkedSkybox), true);
            }
            LabelCenter(rect, locale["ground"], true);
            data[index] = TextField(rect, data[index++], string.Empty, 0f, true);
            LabelCenter(rect, locale["wall"], true);
            data[index] = TextField(rect, data[index++], string.Empty, 0f, true);
            LabelCenter(rect, locale["gate"], true);
            data[index] = TextField(rect, data[index++], string.Empty, 0f, true);
            rect.MoveY();
            LabelCenter(rect, locale["houses"], true);
            for (int i = index; i < data.Length; i++)
            {
                data[i] = TextField(rect, data[i], string.Empty, 0f, true);
            }
        }

        public static SkinPreset[] GetAllPresets()
        {
            DirectoryInfo info = new DirectoryInfo(CityPath);
            FileInfo[] files = info.GetFiles();
            if (files.Length == 0)
            {
                return null;
            }

            SkinPreset[] result = new SkinPreset[files.Length];
            for (int i = 0; i < files.Length; i++)
            {
                result[i] = new CityPreset(files[i].Name.Replace(Extension, string.Empty));
                result[i].Load();
            }
            return result;
        }

        public override void Load()
        {
            using (var file = new AnarchyStorage(FullPath, '`', false))
            {
                file.Load();
                file.AutoSave = false;
                data[0] = file.GetString("ground", string.Empty);
                data[1] = file.GetString("wall", string.Empty);
                data[2] = file.GetString("gate", string.Empty);
                for (int i = 3; i < Length; i++)
                {
                    data[i] = file.GetString("house" + (i - 3).ToString(), string.Empty);
                }
                LinkedSkybox = file.GetString("skybox", Anarchy.Configuration.StringSetting.NotDefine);
                if (LinkedSkybox != Anarchy.Configuration.StringSetting.NotDefine)
                {
                    SkyboxPreset set = new SkyboxPreset(LinkedSkybox);
                    if (!set.Exists())
                    {
                        LinkedSkybox = Anarchy.Configuration.StringSetting.NotDefine;
                    }
                }
            }
        }

        public override void Save()
        {
            using (var file = new AnarchyStorage(FullPath, '`', true))
            {
                file.SetString("ground", data[0]);
                file.SetString("wall", data[1]);
                file.SetString("gate", data[2]);
                for (int i = 3; i < data.Length; i++)
                {
                    file.SetString("house" + (i - 3).ToString(), data[i]);
                }
                file.SetString("skybox", LinkedSkybox);
            }
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