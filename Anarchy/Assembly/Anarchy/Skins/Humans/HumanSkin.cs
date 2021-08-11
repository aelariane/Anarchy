using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Anarchy.Skins.Humans
{
    internal class HumanSkin : Skin
    {
        public override int DataLength => 13;

        internal static Dictionary<int, Dictionary<int, SkinElement>> Storage = new Dictionary<int, Dictionary<int, SkinElement>>();
        private Dictionary<int, Renderer[]> renderers;
        public SkinElement this[HumanParts part] => elements[(int)part];

        public HumanSkin(HERO owner, string[] data) : base(owner.gameObject, data)
        {
            renderers = new Dictionary<int, Renderer[]>();
            for (int i = 0; i < DataLength; i++)
            {
                renderers.Add(i, GetRenderers(i));
            }
            int key = IN_GAME_MAIN_CAMERA.GameType == GameType.MultiPlayer ? owner.BasePV.owner.ID : -1;
            if (Storage.ContainsKey(key))
            {
                elements = Storage[key];
            }
            else
            {
                Storage.Add(key, elements);
            }
        }

        public override void Apply()
        {
            HERO hero = Owner.GetComponent<HERO>();
            foreach (KeyValuePair<int, Renderer[]> pair in renderers)
            {
                if (pair.Key == (int)HumanParts.Gas && Configuration.SkinSettings.DisableCustomGas.Value)
                {
                    continue;
                }
                else if (pair.Key == (int)HumanParts.WeaponTrail)
                {
                    if (Configuration.VideoSettings.BladeTrails.Value == false || !hero.IsLocal)
                    {
                        continue;
                    }
                    var element = elements[pair.Key];
                    hero.leftbladetrail.MyMaterial.mainTexture = element.Texture;
                    hero.leftbladetrail2.MyMaterial.mainTexture = element.Texture;
                    hero.rightbladetrail.MyMaterial.mainTexture = element.Texture;
                    hero.rightbladetrail2.MyMaterial.mainTexture = element.Texture;
                    continue;
                }
                SkinElement skin = elements[pair.Key];
                if (skin.IsDone && pair.Value != null)
                {
                    foreach (Renderer render in pair.Value)
                    {
                        if (skin.IsTransparent)
                        {
                            render.enabled = false;
                            continue;
                        }
                        if (hero.Setup.myCostume.hairInfo.id >= 0)
                        {
                            render.material = CharacterMaterials.Materials[hero.Setup.myCostume.hairInfo.texture];
                        }
                        if (pair.Key == (int)HumanParts.Eyes || pair.Key == (int)HumanParts.Glass || pair.Key == (int)HumanParts.Face)
                        {
                            render.material.mainTextureScale *= 8f;
                            render.material.mainTextureOffset = new Vector2(0f, 0f);
                        }
                    }
                    TryApplyTextures(skin, pair.Value, true);
                }
            }
        }

        protected Renderer[] GetRenderers(int type)
        {
            HERO hero = Owner.GetComponent<HERO>();
            List<Renderer> tmp = new List<Renderer>();
            switch ((HumanParts)type)
            {
                case HumanParts.Hair:
                    if (hero.Setup.part_hair != null)
                    {
                        tmp.Add(hero.Setup.part_hair.renderer);
                    }
                    if (hero.Setup.part_hair_1 != null)
                    {
                        tmp.Add(hero.Setup.part_hair_1.renderer);
                    }
                    break;

                case HumanParts.Eyes:
                    tmp.Add(hero.Setup.part_eye.renderer);
                    break;

                case HumanParts.Glass:
                    if (hero.Setup.part_glass != null)
                    {
                        tmp.Add(hero.Setup.part_glass.renderer);
                    }
                    break;

                case HumanParts.Face:
                    if (hero.Setup.part_face != null)
                    {
                        tmp.Add(hero.Setup.part_face.renderer);
                    }
                    break;

                case HumanParts.Skin:
                    tmp.Add(hero.Setup.part_hand_l.renderer);
                    tmp.Add(hero.Setup.part_hand_r.renderer);
                    tmp.Add(hero.Setup.part_head.renderer);
                    break;

                case HumanParts.Costume:
                    if (hero.Setup.part_chest_3 != null)
                    {
                        tmp.Add(hero.Setup.part_chest_3.renderer);
                    }
                    tmp.Add(hero.Setup.part_upper_body.renderer);
                    tmp.Add(hero.Setup.part_arm_l.renderer);
                    tmp.Add(hero.Setup.part_arm_r.renderer);
                    tmp.Add(hero.Setup.part_leg.renderer);
                    break;

                case HumanParts.Cape:
                    if (hero.Setup.part_cape != null)
                    {
                        tmp.Add(hero.Setup.part_cape.renderer);
                    }
                    if (hero.Setup.part_brand_1 != null)
                    {
                        tmp.Add(hero.Setup.part_brand_1.renderer);
                    }
                    if (hero.Setup.part_brand_2 != null)
                    {
                        tmp.Add(hero.Setup.part_brand_2.renderer);
                    }
                    if (hero.Setup.part_brand_3 != null)
                    {
                        tmp.Add(hero.Setup.part_brand_3.renderer);
                    }
                    if (hero.Setup.part_brand_4 != null)
                    {
                        tmp.Add(hero.Setup.part_brand_4.renderer);
                    }
                    break;

                case HumanParts.Left3DMG:
                    if (hero.Setup.part_3dmg != null)
                    {
                        tmp.Add(hero.Setup.part_3dmg.renderer);
                    }
                    if (hero.Setup.part_3dmg_belt != null)
                    {
                        tmp.Add(hero.Setup.part_3dmg_belt.renderer);
                    }
                    if (hero.Setup.part_3dmg_gas_l != null)
                    {
                        tmp.Add(hero.Setup.part_3dmg_gas_l.renderer);
                    }
                    tmp.Add(hero.Setup.part_blade_l.renderer);
                    break;

                case HumanParts.Right3DMG:
                    if (hero.Setup.part_3dmg != null)
                    {
                        tmp.Add(hero.Setup.part_3dmg.renderer);
                    }
                    if (hero.Setup.part_3dmg_belt != null)
                    {
                        tmp.Add(hero.Setup.part_3dmg_belt.renderer);
                    }
                    if (hero.Setup.part_3dmg_gas_r != null)
                    {
                        tmp.Add(hero.Setup.part_3dmg_gas_r.renderer);
                    }
                    tmp.Add(hero.Setup.part_blade_r.renderer);
                    break;

                case HumanParts.Gas:
                    tmp.Add(hero.baseT.Find("3dmg_smoke").gameObject.renderer);
                    break;

                case HumanParts.Hoodie:
                    if (hero.Setup.part_chest_1 != null && hero.Setup.part_chest_1.name.Contains("character_cap_casual"))
                    {
                        tmp.Add(hero.Setup.part_chest_1.renderer);
                    }
                    break;

                case HumanParts.WeaponTrail:
                    break;

                case HumanParts.Horse:
                    GameObject horse = hero.myHorse;

                    if (horse == null)
                    {
                        return null;
                    }
                    foreach (Renderer renderer in horse.GetComponentsInChildren<Renderer>())
                    {
                        if (renderer.name == "HORSE")
                        {
                            tmp.Add(renderer);
                        }
                    }
                    break;

                default:
                    return null;
            }
            return tmp.ToArray();
        }

        public override string ToString()
        {
            StringBuilder str = new StringBuilder();
            foreach (KeyValuePair<int, SkinElement> pair in elements)
            {
                if (pair.Value.Path != string.Empty)
                {
                    str.AppendLine($"{((HumanParts)pair.Key).ToString()} = {pair.Value.Path}");
                }
            }
            return str.ToString();
        }
    }
}