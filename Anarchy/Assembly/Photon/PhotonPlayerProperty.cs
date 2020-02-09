using System;
using System.Collections.Generic;
using System.Reflection;

public static class PhotonPlayerProperty
{
    public static readonly string beard_texture_id = "beard_texture_id";
    public static readonly string body_texture = "body_texture";
    public static readonly string cape = "cape";
    public static readonly string character = "character";
    public static readonly string costumeId = "costumeId";
    [RCProperty] public static readonly string currentLevel = "currentLevel";
    [RCProperty] public static readonly string customBool = "customBool";
    [RCProperty] public static readonly string customFloat = "customFloat";
    [RCProperty] public static readonly string customInt = "customInt";
    [RCProperty] public static readonly string customString = "customString";
    public static readonly string dead = "dead";
    public static readonly string deaths = "deaths";
    public static readonly string division = "division";
    public static readonly string eye_texture_id = "eye_texture_id";
    public static readonly string glass_texture_id = "glass_texture_id";
    public static readonly string guildName = "guildName";
    public static readonly string hair_color1 = "hair_color1";
    public static readonly string hair_color2 = "hair_color2";
    public static readonly string hair_color3 = "hair_color3";
    public static readonly string hairInfo = "hairInfo";
    public static readonly string heroCostumeId = "heroCostumeId";
    public static readonly string isTitan = "isTitan";
    public static readonly string kills = "kills";
    public static readonly string max_dmg = "max_dmg";
    public static readonly string name = "name";
    public static readonly string part_chest_1_object_mesh = "part_chest_1_object_mesh";
    public static readonly string part_chest_1_object_texture = "part_chest_1_object_texture";
    public static readonly string part_chest_object_mesh = "part_chest_object_mesh";
    public static readonly string part_chest_object_texture = "part_chest_object_texture";
    public static readonly string part_chest_skinned_cloth_mesh = "part_chest_skinned_cloth_mesh";
    public static readonly string part_chest_skinned_cloth_texture = "part_chest_skinned_cloth_texture";
    [RCProperty] public static readonly string RCBombA = "RCBombA";
    [RCProperty] public static readonly string RCBombB = "RCBombB";
    [RCProperty] public static readonly string RCBombG = "RCBombG";
    [RCProperty] public static readonly string RCBombR = "RCBombR";
    [RCProperty] public static readonly string RCBombRadius = "RCBombRadius";
    [RCProperty] public static readonly string RCteam = "RCteam";
    public static readonly string sex = "sex";
    public static readonly string skin_color = "skin_color";
    public static readonly string statACL = "statACL";
    public static readonly string statBLA = "statBLA";
    public static readonly string statGAS = "statGAS";
    public static readonly string statSKILL = "statSKILL";
    public static readonly string statSPD = "statSPD";
    public static readonly string team = "team";
    public static readonly string total_dmg = "total_dmg";
    public static readonly string uniform_type = "uniform_type";

    private static string[] allProperties;
    private static string[] rcProperties;
    private static string[] vanillaProperties;

    public static string[] AllProperties
    {
        get
        {
            if (allProperties == null)
            {
                InitAllProperties();
            }
            return allProperties;
        }
    }
    public static string[] RCProperties
    {
        get
        {
            if (rcProperties == null)
            {
                InitAllProperties();
            }
            return rcProperties;
        }
    }
    public static string[] VanillaProperties
    {
        get
        {
            if (vanillaProperties == null)
            {
                InitAllProperties();
            }
            return vanillaProperties;
        }
    }

    private static void InitAllProperties()
    {
        List<string> tmp = new List<string>();
        List<string> rcTemp = new List<string>();
        List<string> vanillaTemp = new List<string>();
        FieldInfo[] infos = typeof(PhotonPlayerProperty).GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.GetField);
        foreach (FieldInfo info in infos)
        {
            if (info.IsStatic && info.FieldType == typeof(string))
            {
                string value = info.GetValue(null).ToString();
                if (value.Length > 0)
                {
                    tmp.Add(value);
                }
                var atr = info.GetCustomAttributes(typeof(RCPropertyAttribute), false);
                if(atr.Length > 0)
                {
                    rcTemp.Add(value);
                }
                else
                {
                    vanillaTemp.Add(value);
                }
            }
        }
        allProperties = tmp.ToArray();
        rcProperties = rcTemp.ToArray();
        vanillaProperties = vanillaTemp.ToArray();
    }

    [AttributeUsage(AttributeTargets.Field)]
    private class RCPropertyAttribute : Attribute
    {
    }
}