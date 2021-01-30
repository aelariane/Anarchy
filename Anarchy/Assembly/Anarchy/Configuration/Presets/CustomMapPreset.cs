using Anarchy.Configuration.Storage;
using Anarchy.Localization;
using Anarchy.UI;
using System.IO;
using System.Linq;
using UnityEngine;
using static Anarchy.UI.GUI;

namespace Anarchy.Configuration.Presets
{
    public class CustomMapPreset : SkinPreset
    {
        public static readonly string CustomPath = Application.dataPath + "/Configuration/CustomMapSkins/";
        private static readonly string[] labels = new string[6] { "Front", "Back", "Left", "Right", "Up", "Down" };
        private const int Length = 7;

        private string[] data;

        public string Ground
        {
            get
            {
                return data[0] ?? string.Empty;
            }
            set
            {
                data[0] = value ?? string.Empty;
            }
        }

        public string SkyboxUp => data[1];

        public string SkyboxDown => data[2];

        public string SkyboxLeft => data[3];

        public string SkyboxRight => data[4];

        public string SkyboxFront => data[5];

        public string SkyboxBack => data[6];

        public CustomMapPreset(string name) : base(name, CustomPath)
        {
            data = new string[Length].Select(x => string.Empty).ToArray();
        }

        public override void Draw(SmartRect rect, Locale locale)
        {
            LabelCenter(rect, locale["ground"], true);
            data[0] = TextField(rect, data[0], string.Empty, 0f, true);
            rect.MoveY();
            for (int i = 1; i < Length; i++)
            {
                LabelCenter(rect, locale["skybox" + labels[i - 1]], true);
                data[i] = TextField(rect, data[i], string.Empty, 0f, true);
            }
        }

        public override void Load()
        {
            using (var file = new AnarchyStorage(FullPath, '`', false))
            {
                file.Load();
                file.AutoSave = false;
                data[0] = file.GetString("ground", string.Empty);
                data[1] = file.GetString("skyboxUp", string.Empty);
                data[2] = file.GetString("skyboxDown", string.Empty);
                data[3] = file.GetString("skyboxLeft", string.Empty);
                data[4] = file.GetString("skyboxRight", string.Empty);
                data[5] = file.GetString("skyboxFront", string.Empty);
                data[6] = file.GetString("skyboxBack", string.Empty);
            }
        }

        public override void Save()
        {
            using (var file = new AnarchyStorage(FullPath, '`', true))
            {
                file.SetString("ground", data[0]);
                file.SetString("skyboxUp", data[1]);
                file.SetString("skyboxDown", data[2]);
                file.SetString("skyboxLeft", data[3]);
                file.SetString("skyboxRight", data[4]);
                file.SetString("skyboxFront", data[5]);
                file.SetString("skyboxBack", data[6]);
            }
        }

        public static SkinPreset[] GetAllPresets()
        {
            DirectoryInfo info = new DirectoryInfo(CustomPath);
            FileInfo[] files = info.GetFiles();
            if (files.Length == 0)
            {
                return null;
            }

            SkinPreset[] result = new SkinPreset[files.Length];
            for (int i = 0; i < files.Length; i++)
            {
                result[i] = new CustomMapPreset(files[i].Name.Replace(Extension, string.Empty));
                result[i].Load();
            }
            return result;
        }

        public override string[] ToSkinData()
        {
            string[] result = new string[data.Length];
            for (int i = 1; i < data.Length; i++)
            {
                result[i - 1] = data[i];
            }
            result[6] = data[0];
            return result;
        }
    }
}