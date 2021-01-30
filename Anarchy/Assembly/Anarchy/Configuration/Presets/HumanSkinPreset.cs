using Anarchy.Configuration.Storage;
using Anarchy.Localization;
using Anarchy.UI;
using System.IO;
using UnityEngine;

namespace Anarchy.Configuration.Presets
{
    public class HumanSkinPreset : SkinPreset
    {
        public static string HumansPath = Application.dataPath + "/Configuration/HumanSkins/";
        public const int SkinDataLength = 13;

        private readonly string[] keys = new string[SkinDataLength] { "horse", "hair", "eyes", "glass", "face", "skin", "costume", "cape", "leftGear", "rightGear", "gas", "hoodie", "trail" };
        public string[] SkinData;

        public HumanSkinPreset(string name) : base(name, HumansPath)
        {
            SkinData = new string[SkinDataLength];
            for (int i = 0; i < SkinDataLength; i++)
            {
                SkinData[i] = string.Empty;
            }
        }

        public HumanSkinPreset(string name, HumanSkinPreset other) : this(name)
        {
            SkinData = new string[SkinDataLength];
            for (int i = 0; i < SkinDataLength; i++)
            {
                SkinData[i] = other.SkinData[i];
            }
        }

        public override void Draw(SmartRect rect, Locale locale)
        {
            string[] labels = locale.GetArray("humanLabels");
            for (int i = 0; i < labels.Length; i++)
            {
                SkinData[i] = UI.GUI.TextField(rect, SkinData[i], labels[i], Style.LabelOffset, true);
            }
        }

        public static SkinPreset[] GetAllPresets()
        {
            DirectoryInfo info = new DirectoryInfo(HumansPath);
            FileInfo[] files = info.GetFiles();
            if (files.Length == 0)
            {
                return null;
            }

            SkinPreset[] result = new SkinPreset[files.Length];
            for (int i = 0; i < files.Length; i++)
            {
                result[i] = new HumanSkinPreset(files[i].Name.Replace(Extension, string.Empty));
                result[i].Load();
            }
            return result;
        }

        public override void Load()
        {
            using (AnarchyStorage file = new AnarchyStorage(FullPath, '`', false))
            {
                file.Load();
                file.AutoSave = false;
                for (int i = 0; i < SkinDataLength; i++)
                {
                    SkinData[i] = file.GetString(keys[i], string.Empty);
                }
            }
        }

        public override void Save()
        {
            using (AnarchyStorage file = new AnarchyStorage(FullPath, '`', true))
            {
                for (int i = 0; i < SkinDataLength; i++)
                {
                    file.SetString(keys[i], SkinData[i]);
                }
            }
        }

        public override string[] ToSkinData()
        {
            string[] result = new string[SkinData.Length];
            for (int i = 0; i < SkinData.Length; i++)
            {
                result[i] = SkinData[i];
            }
            return result;
        }
    }
}