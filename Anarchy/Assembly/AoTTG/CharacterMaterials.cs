using System.Collections.Generic;
using UnityEngine;

internal static class CharacterMaterials
{
    private static bool initialized = false;
    public static Dictionary<string, Material> Materials;

    private static void NewMaterial(string pref)
    {
        Texture mainTexture = (Texture)Object.Instantiate(Resources.Load("NewTexture/" + pref));
        Material material = (Material)Object.Instantiate(Resources.Load("NewTexture/MaterialCharacter"));
        material.mainTexture = mainTexture;
        Materials.Add(pref, material);
    }

    public static Material Get3DMG(string name)
    {
        if(Materials.TryGetValue(name, out Material mat))
        {
            return mat;
        }
        return Materials["AOTTG_HERO_3DMG"];
    }

    public static Material GetSkin(string name)
    {
        if (Materials.TryGetValue(name, out Material mat))
        {
            return mat;
        }
        return Materials["aottg_hero_skin_1"];
    }

    public static void Init()   
    {
        if (initialized)
        {
            return;
        }
        initialized = true;
        Materials = new Dictionary<string, Material>();
        NewMaterial("AOTTG_HERO_3DMG");
        NewMaterial("aottg_hero_AHSS_3dmg");
        NewMaterial("aottg_hero_annie_cap_causal");
        NewMaterial("aottg_hero_annie_cap_uniform");
        NewMaterial("aottg_hero_brand_sc");
        NewMaterial("aottg_hero_brand_mp");
        NewMaterial("aottg_hero_brand_g");
        NewMaterial("aottg_hero_brand_ts");
        NewMaterial("aottg_hero_skin_1");
        NewMaterial("aottg_hero_skin_2");
        NewMaterial("aottg_hero_skin_3");
        NewMaterial("aottg_hero_casual_fa_1");
        NewMaterial("aottg_hero_casual_fa_2");
        NewMaterial("aottg_hero_casual_fa_3");
        NewMaterial("aottg_hero_casual_fb_1");
        NewMaterial("aottg_hero_casual_fb_2");
        NewMaterial("aottg_hero_casual_ma_1");
        NewMaterial("aottg_hero_casual_ma_1_ahss");
        NewMaterial("aottg_hero_casual_ma_2");
        NewMaterial("aottg_hero_casual_ma_3");
        NewMaterial("aottg_hero_casual_mb_1");
        NewMaterial("aottg_hero_casual_mb_2");
        NewMaterial("aottg_hero_casual_mb_3");
        NewMaterial("aottg_hero_casual_mb_4");
        NewMaterial("aottg_hero_uniform_fa_1");
        NewMaterial("aottg_hero_uniform_fa_2");
        NewMaterial("aottg_hero_uniform_fa_3");
        NewMaterial("aottg_hero_uniform_fb_1");
        NewMaterial("aottg_hero_uniform_fb_2");
        NewMaterial("aottg_hero_uniform_ma_1");
        NewMaterial("aottg_hero_uniform_ma_2");
        NewMaterial("aottg_hero_uniform_ma_3");
        NewMaterial("aottg_hero_uniform_mb_1");
        NewMaterial("aottg_hero_uniform_mb_2");
        NewMaterial("aottg_hero_uniform_mb_3");
        NewMaterial("aottg_hero_uniform_mb_4");
        NewMaterial("hair_annie");
        NewMaterial("hair_armin");
        NewMaterial("hair_boy1");
        NewMaterial("hair_boy2");
        NewMaterial("hair_boy3");
        NewMaterial("hair_boy4");
        NewMaterial("hair_eren");
        NewMaterial("hair_girl1");
        NewMaterial("hair_girl2");
        NewMaterial("hair_girl3");
        NewMaterial("hair_girl4");
        NewMaterial("hair_girl5");
        NewMaterial("hair_hanji");
        NewMaterial("hair_jean");
        NewMaterial("hair_levi");
        NewMaterial("hair_marco");
        NewMaterial("hair_mike");
        NewMaterial("hair_petra");
        NewMaterial("hair_rico");
        NewMaterial("hair_sasha");
        NewMaterial("hair_mikasa");
        Texture mainTexture = (Texture)Object.Instantiate(Resources.Load("NewTexture/aottg_hero_eyes"));
        Material material = (Material)Object.Instantiate(Resources.Load("NewTexture/MaterialGLASS"));
        material.mainTexture = mainTexture;
        Materials.Add("aottg_hero_eyes", material);
    }
}