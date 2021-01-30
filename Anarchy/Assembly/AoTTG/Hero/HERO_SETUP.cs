using Optimization.Caching;
using UnityEngine;
using Xft;

public class HERO_SETUP : MonoBehaviour
{
    private Transform baseT;
    private GameObject mount_3dmg;
    private GameObject mount_3dmg_gas_l;
    private GameObject mount_3dmg_gas_r;
    private GameObject mount_3dmg_gun_mag_l;
    private GameObject mount_3dmg_gun_mag_r;
    private GameObject mount_weapon_l;
    private GameObject mount_weapon_r;
    public GameObject chest_info;
    public bool IsDeadBody;
    public HeroCostume myCostume;
    public GameObject part_3dmg;
    public GameObject part_3dmg_belt;
    public GameObject part_3dmg_gas_l;
    public GameObject part_3dmg_gas_r;
    public GameObject part_arm_l;
    public GameObject part_arm_r;
    public GameObject part_asset_1;
    public GameObject part_asset_2;
    public GameObject part_blade_l;
    public GameObject part_blade_r;
    public GameObject part_brand_1;
    public GameObject part_brand_2;
    public GameObject part_brand_3;
    public GameObject part_brand_4;
    public GameObject part_cape;
    public GameObject part_chest;
    public GameObject part_chest_1;
    public GameObject part_chest_2;
    public GameObject part_chest_3;
    public GameObject part_eye;
    public GameObject part_face;
    public GameObject part_glass;
    public GameObject part_hair;
    public GameObject part_hair_1;
    public GameObject part_hair_2;
    public GameObject part_hand_l;
    public GameObject part_hand_r;
    public GameObject part_head;
    public GameObject part_leg;
    public GameObject part_upper_body;
    public GameObject reference;

    private void Awake()
    {
        baseT = transform;
        this.part_head.transform.parent = baseT.Find("Amarture/Controller_Body/hip/spine/chest/neck/head").transform;
        this.mount_3dmg = new GameObject();
        this.mount_3dmg_gas_l = new GameObject();
        this.mount_3dmg_gas_r = new GameObject();
        this.mount_3dmg_gun_mag_l = new GameObject();
        this.mount_3dmg_gun_mag_r = new GameObject();
        this.mount_weapon_l = new GameObject();
        this.mount_weapon_r = new GameObject();
        this.mount_3dmg.transform.position = baseT.position;
        this.mount_3dmg.transform.rotation = Quaternion.Euler(270f, baseT.rotation.eulerAngles.y, 0f);
        this.mount_3dmg.transform.parent = baseT.Find("Amarture/Controller_Body/hip/spine/chest").transform;
        this.mount_3dmg_gas_l.transform.position = baseT.position;
        this.mount_3dmg_gas_l.transform.rotation = Quaternion.Euler(270f, baseT.rotation.eulerAngles.y, 0f);
        this.mount_3dmg_gas_l.transform.parent = baseT.Find("Amarture/Controller_Body/hip/spine").transform;
        this.mount_3dmg_gas_r.transform.position = baseT.position;
        this.mount_3dmg_gas_r.transform.rotation = Quaternion.Euler(270f, baseT.rotation.eulerAngles.y, 0f);
        this.mount_3dmg_gas_r.transform.parent = baseT.Find("Amarture/Controller_Body/hip/spine").transform;
        this.mount_3dmg_gun_mag_l.transform.position = baseT.position;
        this.mount_3dmg_gun_mag_l.transform.rotation = Quaternion.Euler(270f, baseT.rotation.eulerAngles.y, 0f);
        this.mount_3dmg_gun_mag_l.transform.parent = baseT.Find("Amarture/Controller_Body/hip/thigh_L").transform;
        this.mount_3dmg_gun_mag_r.transform.position = baseT.position;
        this.mount_3dmg_gun_mag_r.transform.rotation = Quaternion.Euler(270f, baseT.rotation.eulerAngles.y, 0f);
        this.mount_3dmg_gun_mag_r.transform.parent = baseT.Find("Amarture/Controller_Body/hip/thigh_R").transform;
        this.mount_weapon_l.transform.position = baseT.position;
        this.mount_weapon_l.transform.rotation = Quaternion.Euler(270f, baseT.rotation.eulerAngles.y, 0f);
        this.mount_weapon_l.transform.parent = baseT.Find("Amarture/Controller_Body/hip/spine/chest/shoulder_L/upper_arm_L/forearm_L/hand_L").transform;
        this.mount_weapon_r.transform.position = baseT.position;
        this.mount_weapon_r.transform.rotation = Quaternion.Euler(270f, baseT.rotation.eulerAngles.y, 0f);
        this.mount_weapon_r.transform.parent = baseT.Find("Amarture/Controller_Body/hip/spine/chest/shoulder_R/upper_arm_R/forearm_R/hand_R").transform;
    }

    private GameObject GenerateCloth(GameObject go, string res)
    {
        if (!go.GetComponent<SkinnedMeshRenderer>())
        {
            go.AddComponent<SkinnedMeshRenderer>();
        }
        SkinnedMeshRenderer component = go.GetComponent<SkinnedMeshRenderer>();
        Transform[] bones = component.bones;
        SkinnedMeshRenderer component2 = ((GameObject)UnityEngine.Object.Instantiate(CacheResources.Load(res))).GetComponent<SkinnedMeshRenderer>();
        component2.gameObject.transform.parent = component.gameObject.transform.parent;
        component2.transform.localPosition = Vectors.zero;
        component2.transform.localScale = Vectors.one;
        component2.bones = bones;
        component2.quality = SkinQuality.Auto;
        return component2.gameObject;
    }

    public void Create3DMG()
    {
        UnityEngine.Object.Destroy(this.part_3dmg);
        UnityEngine.Object.Destroy(this.part_3dmg_belt);
        UnityEngine.Object.Destroy(this.part_3dmg_gas_l);
        UnityEngine.Object.Destroy(this.part_3dmg_gas_r);
        UnityEngine.Object.Destroy(this.part_blade_l);
        UnityEngine.Object.Destroy(this.part_blade_r);
        if (this.myCostume.mesh_3dmg.Length > 0)
        {
            this.part_3dmg = (GameObject)UnityEngine.Object.Instantiate(CacheResources.Load("Character/" + this.myCostume.mesh_3dmg));
            this.part_3dmg.transform.position = this.mount_3dmg.transform.position;
            this.part_3dmg.transform.rotation = this.mount_3dmg.transform.rotation;
            this.part_3dmg.transform.parent = this.mount_3dmg.transform.parent;
            this.part_3dmg.renderer.material = CharacterMaterials.Materials[this.myCostume._3dmg_texture];
        }
        if (this.myCostume.mesh_3dmg_belt.Length > 0)
        {
            this.part_3dmg_belt = this.GenerateCloth(this.reference, "Character/" + this.myCostume.mesh_3dmg_belt);
            this.part_3dmg_belt.renderer.material = CharacterMaterials.Materials[this.myCostume._3dmg_texture];
        }
        if (this.myCostume.mesh_3dmg_gas_l.Length > 0)
        {
            this.part_3dmg_gas_l = (GameObject)UnityEngine.Object.Instantiate(CacheResources.Load("Character/" + this.myCostume.mesh_3dmg_gas_l));
            if (this.myCostume.uniform_type != UNIFORM_TYPE.CasualAHSS)
            {
                this.part_3dmg_gas_l.transform.position = this.mount_3dmg_gas_l.transform.position;
                this.part_3dmg_gas_l.transform.rotation = this.mount_3dmg_gas_l.transform.rotation;
                this.part_3dmg_gas_l.transform.parent = this.mount_3dmg_gas_l.transform.parent;
            }
            else
            {
                this.part_3dmg_gas_l.transform.position = this.mount_3dmg_gun_mag_l.transform.position;
                this.part_3dmg_gas_l.transform.rotation = this.mount_3dmg_gun_mag_l.transform.rotation;
                this.part_3dmg_gas_l.transform.parent = this.mount_3dmg_gun_mag_l.transform.parent;
            }
            this.part_3dmg_gas_l.renderer.material = CharacterMaterials.Materials[this.myCostume._3dmg_texture];
        }
        if (this.myCostume.mesh_3dmg_gas_r.Length > 0)
        {
            this.part_3dmg_gas_r = (GameObject)UnityEngine.Object.Instantiate(CacheResources.Load("Character/" + this.myCostume.mesh_3dmg_gas_r));
            if (this.myCostume.uniform_type != UNIFORM_TYPE.CasualAHSS)
            {
                this.part_3dmg_gas_r.transform.position = this.mount_3dmg_gas_r.transform.position;
                this.part_3dmg_gas_r.transform.rotation = this.mount_3dmg_gas_r.transform.rotation;
                this.part_3dmg_gas_r.transform.parent = this.mount_3dmg_gas_r.transform.parent;
            }
            else
            {
                this.part_3dmg_gas_r.transform.position = this.mount_3dmg_gun_mag_r.transform.position;
                this.part_3dmg_gas_r.transform.rotation = this.mount_3dmg_gun_mag_r.transform.rotation;
                this.part_3dmg_gas_r.transform.parent = this.mount_3dmg_gun_mag_r.transform.parent;
            }
            this.part_3dmg_gas_r.renderer.material = CharacterMaterials.Materials[this.myCostume._3dmg_texture];
        }
        if (this.myCostume.weapon_l_mesh.Length > 0)
        {
            this.part_blade_l = (GameObject)UnityEngine.Object.Instantiate(CacheResources.Load("Character/" + this.myCostume.weapon_l_mesh));
            this.part_blade_l.transform.position = this.mount_weapon_l.transform.position;
            this.part_blade_l.transform.rotation = this.mount_weapon_l.transform.rotation;
            this.part_blade_l.transform.parent = this.mount_weapon_l.transform.parent;
            this.part_blade_l.renderer.material = CharacterMaterials.Materials[this.myCostume._3dmg_texture];
            if (this.part_blade_l.transform.Find("X-WeaponTrailA"))
            {
                this.part_blade_l.transform.Find("X-WeaponTrailA").GetComponent<XWeaponTrail>().Deactivate();
                this.part_blade_l.transform.Find("X-WeaponTrailB").GetComponent<XWeaponTrail>().Deactivate();
                if (base.gameObject.GetComponent<HERO>())
                {
                    base.gameObject.GetComponent<HERO>().leftbladetrail = this.part_blade_l.transform.Find("X-WeaponTrailA").GetComponent<XWeaponTrail>();
                    base.gameObject.GetComponent<HERO>().leftbladetrail2 = this.part_blade_l.transform.Find("X-WeaponTrailB").GetComponent<XWeaponTrail>();
                }
            }
        }
        if (this.myCostume.weapon_r_mesh.Length > 0)
        {
            this.part_blade_r = (GameObject)UnityEngine.Object.Instantiate(CacheResources.Load("Character/" + this.myCostume.weapon_r_mesh));
            this.part_blade_r.transform.position = this.mount_weapon_r.transform.position;
            this.part_blade_r.transform.rotation = this.mount_weapon_r.transform.rotation;
            this.part_blade_r.transform.parent = this.mount_weapon_r.transform.parent;
            this.part_blade_r.renderer.material = CharacterMaterials.Materials[this.myCostume._3dmg_texture];
            if (this.part_blade_r.transform.Find("X-WeaponTrailA"))
            {
                this.part_blade_r.transform.Find("X-WeaponTrailA").GetComponent<XWeaponTrail>().Deactivate();
                this.part_blade_r.transform.Find("X-WeaponTrailB").GetComponent<XWeaponTrail>().Deactivate();
                if (base.gameObject.GetComponent<HERO>())
                {
                    base.gameObject.GetComponent<HERO>().rightbladetrail = this.part_blade_r.transform.Find("X-WeaponTrailA").GetComponent<XWeaponTrail>();
                    base.gameObject.GetComponent<HERO>().rightbladetrail2 = this.part_blade_r.transform.Find("X-WeaponTrailB").GetComponent<XWeaponTrail>();
                }
            }
        }
    }

    public void CreateCape()
    {
        if (!this.IsDeadBody)
        {
            ClothFactory.DisposeObject(this.part_cape);
            if (this.myCostume.cape_mesh.Length > 0)
            {
                this.part_cape = ClothFactory.GetCape(this.reference, "Character/" + this.myCostume.cape_mesh, CharacterMaterials.Materials[this.myCostume.brand_texture]);
            }
        }
    }

    public void CreateFace()
    {
        this.part_face = (GameObject)UnityEngine.Object.Instantiate(CacheResources.Load("Character/character_face"));
        this.part_face.transform.position = this.part_head.transform.position;
        this.part_face.transform.rotation = this.part_head.transform.rotation;
        this.part_face.transform.parent = baseT.Find("Amarture/Controller_Body/hip/spine/chest/neck/head").transform;
    }

    public void CreateGlass()
    {
        this.part_glass = (GameObject)UnityEngine.Object.Instantiate(CacheResources.Load("Character/glass"));
        this.part_glass.transform.position = this.part_head.transform.position;
        this.part_glass.transform.rotation = this.part_head.transform.rotation;
        this.part_glass.transform.parent = baseT.Find("Amarture/Controller_Body/hip/spine/chest/neck/head").transform;
    }

    public void CreateHair()
    {
        UnityEngine.Object.Destroy(this.part_hair);
        if (!this.IsDeadBody)
        {
            ClothFactory.DisposeObject(this.part_hair_1);
        }
        if (this.myCostume.hair_mesh != string.Empty)
        {
            this.part_hair = (GameObject)UnityEngine.Object.Instantiate(CacheResources.Load("Character/" + this.myCostume.hair_mesh));
            this.part_hair.transform.position = this.part_head.transform.position;
            this.part_hair.transform.rotation = this.part_head.transform.rotation;
            this.part_hair.transform.parent = base.transform.Find("Amarture/Controller_Body/hip/spine/chest/neck/head").transform;
            this.part_hair.renderer.material = CharacterMaterials.Materials[this.myCostume.hairInfo.texture];
            this.part_hair.renderer.material.color = this.myCostume.hair_color;
        }
        if (this.myCostume.hair_1_mesh.Length > 0 && !this.IsDeadBody)
        {
            string name = "Character/" + this.myCostume.hair_1_mesh;
            Material material = CharacterMaterials.Materials[this.myCostume.hairInfo.texture];
            this.part_hair_1 = ClothFactory.GetHair(this.reference, name, material, this.myCostume.hair_color);
        }
    }

    public void CreateHead()
    {
        UnityEngine.Object.Destroy(this.part_eye);
        UnityEngine.Object.Destroy(this.part_face);
        UnityEngine.Object.Destroy(this.part_glass);
        UnityEngine.Object.Destroy(this.part_hair);
        if (!this.IsDeadBody)
        {
            ClothFactory.DisposeObject(this.part_hair_1);
        }
        this.CreateHair();
        if (this.myCostume.eye_mesh.Length > 0)
        {
            this.part_eye = (GameObject)UnityEngine.Object.Instantiate(CacheResources.Load("Character/" + this.myCostume.eye_mesh));
            this.part_eye.transform.position = this.part_head.transform.position;
            this.part_eye.transform.rotation = this.part_head.transform.rotation;
            this.part_eye.transform.parent = base.transform.Find("Amarture/Controller_Body/hip/spine/chest/neck/head").transform;
            this.SetFacialTexture(this.part_eye, this.myCostume.eye_texture_id);
        }
        if (this.myCostume.beard_texture_id >= 0)
        {
            this.CreateFace();
            this.SetFacialTexture(this.part_face, this.myCostume.beard_texture_id);
        }
        if (this.myCostume.glass_texture_id >= 0)
        {
            this.CreateGlass();
            this.SetFacialTexture(this.part_glass, this.myCostume.glass_texture_id);
        }
        string indexHead = myCostume.skin_texture;
        string indexChest = myCostume.skin_texture;
        if (!CharacterMaterials.Materials.ContainsKey(indexHead))
        {
            indexHead = "hair_annie";
        }
        if (!CharacterMaterials.Materials.ContainsKey(indexChest))
        {
            indexChest = "aottg_hero_skin_1";
        }
        this.part_head.renderer.material = CharacterMaterials.Materials[indexHead];
        this.part_chest.renderer.material = CharacterMaterials.Materials[indexChest];
    }

    public void CreateLeftArm()
    {
        UnityEngine.Object.Destroy(this.part_arm_l);
        if (this.myCostume.arm_l_mesh.Length > 0)
        {
            this.part_arm_l = this.GenerateCloth(this.reference, "Character/" + this.myCostume.arm_l_mesh);
            this.part_arm_l.renderer.material = CharacterMaterials.Materials[this.myCostume.body_texture];
        }
        UnityEngine.Object.Destroy(this.part_hand_l);
        if (this.myCostume.hand_l_mesh.Length > 0)
        {
            this.part_hand_l = this.GenerateCloth(this.reference, "Character/" + this.myCostume.hand_l_mesh);
            this.part_hand_l.renderer.material = CharacterMaterials.Materials[this.myCostume.skin_texture];
        }
    }

    public void CreateLowerBody()
    {
        this.part_leg.renderer.material = CharacterMaterials.Materials[this.myCostume.body_texture];
    }

    public void CreateRightArm()
    {
        UnityEngine.Object.Destroy(this.part_arm_r);
        if (this.myCostume.arm_r_mesh.Length > 0)
        {
            this.part_arm_r = this.GenerateCloth(this.reference, "Character/" + this.myCostume.arm_r_mesh);
            this.part_arm_r.renderer.material = CharacterMaterials.Materials[this.myCostume.body_texture];
        }
        UnityEngine.Object.Destroy(this.part_hand_r);
        if (this.myCostume.hand_r_mesh.Length > 0)
        {
            this.part_hand_r = this.GenerateCloth(this.reference, "Character/" + this.myCostume.hand_r_mesh);
            this.part_hand_r.renderer.material = CharacterMaterials.Materials[this.myCostume.skin_texture];
        }
    }

    public void CreateUpperBody()
    {
        UnityEngine.Object.Destroy(this.part_upper_body);
        UnityEngine.Object.Destroy(this.part_brand_1);
        UnityEngine.Object.Destroy(this.part_brand_2);
        UnityEngine.Object.Destroy(this.part_brand_3);
        UnityEngine.Object.Destroy(this.part_brand_4);
        UnityEngine.Object.Destroy(this.part_chest_1);
        UnityEngine.Object.Destroy(this.part_chest_2);
        if (!this.IsDeadBody)
        {
            ClothFactory.DisposeObject(this.part_chest_3);
        }
        this.CreateCape();
        if (this.myCostume.part_chest_object_mesh.Length > 0)
        {
            this.part_chest_1 = (GameObject)UnityEngine.Object.Instantiate(CacheResources.Load("Character/" + this.myCostume.part_chest_object_mesh));
            this.part_chest_1.transform.position = this.chest_info.transform.position;
            this.part_chest_1.transform.rotation = this.chest_info.transform.rotation;
            this.part_chest_1.transform.parent = base.transform.Find("Amarture/Controller_Body/hip/spine/chest").transform;
            this.part_chest_1.renderer.material = CharacterMaterials.Materials[this.myCostume.part_chest_object_texture];
        }
        if (this.myCostume.part_chest_1_object_mesh.Length > 0)
        {
            this.part_chest_2 = (GameObject)UnityEngine.Object.Instantiate(CacheResources.Load("Character/" + this.myCostume.part_chest_1_object_mesh));
            this.part_chest_2.transform.position = this.chest_info.transform.position;
            this.part_chest_2.transform.rotation = this.chest_info.transform.rotation;
            this.part_chest_2.transform.parent = base.transform.Find("Amarture/Controller_Body/hip/spine/chest").transform;
            this.part_chest_2.transform.parent = base.transform.Find("Amarture/Controller_Body/hip/spine/chest").transform;
            this.part_chest_2.renderer.material = CharacterMaterials.Materials[this.myCostume.part_chest_1_object_texture];
        }
        if (this.myCostume.part_chest_skinned_cloth_mesh.Length > 0 && !this.IsDeadBody)
        {
            this.part_chest_3 = ClothFactory.GetCape(this.reference, "Character/" + this.myCostume.part_chest_skinned_cloth_mesh, CharacterMaterials.Materials[this.myCostume.part_chest_skinned_cloth_texture]);
        }
        if (this.myCostume.body_mesh.Length > 0)
        {
            this.part_upper_body = this.GenerateCloth(this.reference, "Character/" + this.myCostume.body_mesh);
            this.part_upper_body.renderer.material = CharacterMaterials.Materials[this.myCostume.body_texture];
        }
        if (this.myCostume.brand1_mesh.Length > 0)
        {
            this.part_brand_1 = this.GenerateCloth(this.reference, "Character/" + this.myCostume.brand1_mesh);
            this.part_brand_1.renderer.material = CharacterMaterials.Materials[this.myCostume.brand_texture];
        }
        if (this.myCostume.brand2_mesh.Length > 0)
        {
            this.part_brand_2 = this.GenerateCloth(this.reference, "Character/" + this.myCostume.brand2_mesh);
            this.part_brand_2.renderer.material = CharacterMaterials.Materials[this.myCostume.brand_texture];
        }
        if (this.myCostume.brand3_mesh.Length > 0)
        {
            this.part_brand_3 = this.GenerateCloth(this.reference, "Character/" + this.myCostume.brand3_mesh);
            this.part_brand_3.renderer.material = CharacterMaterials.Materials[this.myCostume.brand_texture];
        }
        if (this.myCostume.brand4_mesh.Length > 0)
        {
            this.part_brand_4 = this.GenerateCloth(this.reference, "Character/" + this.myCostume.brand4_mesh);
            this.part_brand_4.renderer.material = CharacterMaterials.Materials[this.myCostume.brand_texture];
        }
        this.part_head.renderer.material = CharacterMaterials.Materials[this.myCostume.skin_texture];
        this.part_chest.renderer.material = CharacterMaterials.Materials[this.myCostume.skin_texture];
    }

    public void DeleteCharacterComponent()
    {
        UnityEngine.Object.Destroy(this.part_eye);
        UnityEngine.Object.Destroy(this.part_face);
        UnityEngine.Object.Destroy(this.part_glass);
        UnityEngine.Object.Destroy(this.part_hair);
        bool flag = !this.IsDeadBody;
        if (flag)
        {
            ClothFactory.DisposeObject(this.part_hair_1);
        }
        UnityEngine.Object.Destroy(this.part_upper_body);
        UnityEngine.Object.Destroy(this.part_arm_l);
        UnityEngine.Object.Destroy(this.part_arm_r);
        bool flag2 = !this.IsDeadBody;
        if (flag2)
        {
            ClothFactory.DisposeObject(this.part_hair_2);
            ClothFactory.DisposeObject(this.part_cape);
        }
        UnityEngine.Object.Destroy(this.part_brand_1);
        UnityEngine.Object.Destroy(this.part_brand_2);
        UnityEngine.Object.Destroy(this.part_brand_3);
        UnityEngine.Object.Destroy(this.part_brand_4);
        UnityEngine.Object.Destroy(this.part_chest_1);
        UnityEngine.Object.Destroy(this.part_chest_2);
        UnityEngine.Object.Destroy(this.part_chest_3);
        UnityEngine.Object.Destroy(this.part_3dmg);
        UnityEngine.Object.Destroy(this.part_3dmg_belt);
        UnityEngine.Object.Destroy(this.part_3dmg_gas_l);
        UnityEngine.Object.Destroy(this.part_3dmg_gas_r);
        UnityEngine.Object.Destroy(this.part_blade_l);
        UnityEngine.Object.Destroy(this.part_blade_r);
    }

    public void Init()
    {
        CharacterMaterials.Init();
    }

    private void OnDestroy()
    {
    }

    public void SetCharacterComponent()
    {
        this.CreateHead();
        this.CreateUpperBody();
        this.CreateLeftArm();
        this.CreateRightArm();
        this.CreateLowerBody();
        this.Create3DMG();
    }

    public void SetFacialTexture(GameObject go, int id)
    {
        if (id < 0)
        {
            return;
        }
        go.renderer.material = CharacterMaterials.Materials[this.myCostume.face_texture];
        float num = 0.125f;
        float x = num * (float)((int)((float)id / 8f));
        float y = -num * (float)(id % 8);
        go.renderer.material.mainTextureOffset = new Vector2(x, y);
    }

    public void SetSkin()
    {
        this.part_head.renderer.material = CharacterMaterials.Materials[this.myCostume.skin_texture];
        this.part_chest.renderer.material = CharacterMaterials.Materials[this.myCostume.skin_texture];
        this.part_hand_l.renderer.material = CharacterMaterials.Materials[this.myCostume.skin_texture];
        this.part_hand_r.renderer.material = CharacterMaterials.Materials[this.myCostume.skin_texture];
    }
}