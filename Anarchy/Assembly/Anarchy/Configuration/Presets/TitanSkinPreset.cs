using Anarchy.Configuration.Storage;
using Anarchy.Localization;
using Anarchy.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using static Anarchy.UI.GUI;

namespace Anarchy.Configuration.Presets
{
    public class TitanSkinPreset : SkinPreset
    {
        public static readonly string TitansPath = Application.dataPath + "/Configuration/TitanSkins/";

        private const int Length = 5;
        private const int HairTypeLength = 11;

        public string Annie = string.Empty;
        public string[] Bodies;
        public string Colossal = string.Empty;
        public string[] Eyes;
        public string Eren = string.Empty;
        public string[] Hairs;
        public HairType[] HairTypes;

        public bool RandomizePairs { get; set; }

        public TitanSkinPreset(string name) : base(name, TitansPath)
        {
            Name = name;
            Bodies = new string[Length].Select(x => string.Empty).ToArray();
            Eyes = new string[Length].Select(x => string.Empty).ToArray();
            Hairs = new string[Length].Select(x => string.Empty).ToArray();
            HairTypes = new HairType[Length].Select(x => HairType.Random).ToArray();
        }

        public override void Draw(SmartRect rect, Locale locale)
        {
            RandomizePairs = ToggleButton(rect, RandomizePairs, locale["forestRandomize"], true);
            for (int i = 0; i < Length; i++)
            {
                rect.width = rect.width - 100f - Style.HorizontalMargin;
                Hairs[i] = TextField(rect, Hairs[i], locale["titanHair"] + (i + 1).ToString() + ":", 80f, false);
                rect.MoveX();
                rect.width = 100;
                if (Button(rect, HairTypes[i].ToString(), true))
                {
                    int val = (int)HairTypes[i];
                    val++;
                    if (val >= (int)HairType.Count)
                    {
                        val = -1;
                    }

                    HairTypes[i] = (HairType)val;
                }
                rect.ResetX();
            }
            for (int i = 0; i < Length; i++)
            {
                Eyes[i] = TextField(rect, Eyes[i], locale["titanEye"] + (i + 1).ToString() + ":", 80f, true);
            }
            for (int i = 0; i < Length; i++)
            {
                Bodies[i] = TextField(rect, Bodies[i], locale["titanBody"] + (i + 1).ToString() + ":", 80f, true);
            }
            Annie = TextField(rect, Annie, locale["annie"], 80f, true);
            Eren = TextField(rect, Eren, locale["eren"], 80f, true);
            Colossal = TextField(rect, Colossal, locale["colossal"], 80f, true);
        }

        public static SkinPreset[] GetAllPresets()
        {
            DirectoryInfo info = new DirectoryInfo(TitansPath);
            FileInfo[] files = info.GetFiles();
            if (files.Length == 0)
            {
                return null;
            }

            SkinPreset[] result = new SkinPreset[files.Length];
            for (int i = 0; i < files.Length; i++)
            {
                result[i] = new TitanSkinPreset(files[i].Name.Replace(Extension, string.Empty));
                result[i].Load();
            }
            return result;
        }

        public override void Load()
        {
            using (AnarchyStorage storage = new AnarchyStorage(FullPath, '`', false))
            {
                storage.Load();
                storage.AutoSave = false;
                RandomizePairs = storage.GetBool(nameof(RandomizePairs));
                Annie = storage.GetString(nameof(Annie));
                Colossal = storage.GetString(nameof(Colossal));
                Eren = storage.GetString(nameof(Eren));
                Bodies = storage.GetString(nameof(Bodies)).Split(',');
                Eyes = storage.GetString(nameof(Eyes)).Split(',');
                Hairs = storage.GetString(nameof(Hairs)).Split(',');
                string[] tmp = storage.GetString(nameof(HairTypes), "-1,-1,-1,-1,-1").Split(',');
                HairTypes = new HairType[Length].Select(x => HairType.Random).ToArray();
                for (int i = 0; i < tmp.Length; i++)
                {
                    HairTypes[i] = (HairType)Convert.ToInt32(tmp[i]);
                }
            }
        }

        public override void Save()
        {
            using (AnarchyStorage storage = new AnarchyStorage(FullPath, '`', true))
            {
                storage.SetBool(nameof(RandomizePairs), RandomizePairs);
                storage.SetString(nameof(Annie), Annie);
                storage.SetString(nameof(Colossal), Colossal);
                storage.SetString(nameof(Eren), Eren);
                storage.SetString(nameof(Bodies), string.Join(",", Bodies));
                storage.SetString(nameof(Eyes), string.Join(",", Eyes));
                storage.SetString(nameof(Hairs), string.Join(",", Hairs));
                string[] types = new string[Length];
                for (int i = 0; i < HairTypes.Length; i++)
                {
                    types[i] = ((int)HairTypes[i]).ToString();
                }
                storage.SetString(nameof(HairTypes), string.Join(",", types));
                storage.Save();
            }
        }

        public override string[] ToSkinData()
        {
            List<string> temp = new List<string>();
            for (int i = 0; i < Hairs.Length; i++)
            {
                temp.Add(Hairs[i]);
            }
            for (int i = 0; i < Eyes.Length; i++)
            {
                temp.Add(Eyes[i]);
            }
            for (int i = 0; i < Bodies.Length; i++)
            {
                temp.Add(Bodies[i]);
            }
            temp.Add(Eren);
            temp.Add(Annie);
            temp.Add(Colossal);
            return temp.ToArray();
        }

        public enum HairType : int
        {
            Count = 10,
            Male0 = 0,
            Male1,
            Male2,
            Male3,
            Male4,
            Male5,
            Male6,
            Male7,
            Male8,
            Male9,
            Random = -1
        }
    }
}