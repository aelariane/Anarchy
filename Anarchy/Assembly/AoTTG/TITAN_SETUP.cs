using Anarchy.Configuration;
using Optimization.Caching;
using UnityEngine;

public class TITAN_SETUP : Photon.MonoBehaviour
{
    private CostumeHair hair;
    private GameObject hair_go_ref;
    private int hairType;
    private bool hasEye;
    private int skin;
    private GameObject part_hair;
    public GameObject eye;
    private Anarchy.Skins.Titans.TitanSkinHair hairSkin;

    private void ApplyHairSkin(int hair, int eye, string hairlink)
    {
        Destroy(this.part_hair);
        this.hair = CostumeHair.hairsM[hair];
        this.hairType = hair;
        if (this.hair.hair != string.Empty)
        {
            part_hair = (GameObject)UnityEngine.Object.Instantiate(CacheResources.Load("Character/" + this.hair.hair));
            part_hair.transform.parent = this.hair_go_ref.transform.parent;
            part_hair.transform.position = this.hair_go_ref.transform.position;
            part_hair.transform.rotation = this.hair_go_ref.transform.rotation;
            part_hair.transform.localScale = this.hair_go_ref.transform.localScale;
            part_hair.renderer.material = CharacterMaterials.Materials[this.hair.texture];
        }
        if (hairSkin != null)
        {
            hairSkin = new Anarchy.Skins.Titans.TitanSkinHair(part_hair, hairlink);
        }
        Anarchy.Skins.Skin.Check(hairSkin, new string[] { hairlink });
        this.setFacialTexture(this.eye, eye);
    }

    private void Awake()
    {
        CostumeHair.init();
        CharacterMaterials.Init();
        HeroCostume.Init();
        this.hair_go_ref = new GameObject();
        this.eye.transform.parent = base.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/neck/head").transform;
        this.hair_go_ref.transform.position = this.eye.transform.position + Vectors.up * 3.5f + base.transform.Forward() * 5.2f;
        this.hair_go_ref.transform.rotation = this.eye.transform.rotation;
        this.hair_go_ref.transform.RotateAround(this.eye.transform.position, base.transform.right, -20f);
        this.hair_go_ref.transform.localScale = new Vector3(210f, 210f, 210f);
        this.hair_go_ref.transform.parent = base.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/neck/head").transform;
    }

    [RPC]
    private void setHairPRC(int type, int eye_type, float c1, float c2, float c3)
    {
        UnityEngine.Object.Destroy(this.part_hair);
        this.hair = CostumeHair.hairsM[type];
        this.hairType = type;
        if (this.hair.hair != string.Empty)
        {
            GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(CacheResources.Load("Character/" + this.hair.hair));
            gameObject.transform.parent = this.hair_go_ref.transform.parent;
            gameObject.transform.position = this.hair_go_ref.transform.position;
            gameObject.transform.rotation = this.hair_go_ref.transform.rotation;
            gameObject.transform.localScale = this.hair_go_ref.transform.localScale;
            gameObject.renderer.material = CharacterMaterials.Materials[this.hair.texture];
            gameObject.renderer.material.color = new Color(c1, c2, c3);
            this.part_hair = gameObject;
        }
        this.setFacialTexture(this.eye, eye_type);
    }

    [RPC]
    public void setHairRPC2(int hair, int eye, string hairlink, PhotonMessageInfo info = null)
    {
        if(SkinSettings.TitanSkins.Value == 1 && info != null && info.Sender.IsMasterClient)
        {
            ApplyHairSkin(hair, eye, hairlink);
        }
    }

    public void setFacialTexture(GameObject go, int id)
    {
        if (id < 0)
        {
            return;
        }
        float num = 0.25f;
        float num2 = 0.125f;
        float x = num2 * (float)((int)((float)id / 8f));
        float y = -num * (float)(id % 4);
        go.renderer.material.mainTextureOffset = new Vector2(x, y);
    }

    public void setHair()
    {
        if (SkinSettings.SkinsCheck(SkinSettings.TitanSkins) && SkinSettings.TitanSet.Value != Anarchy.Configuration.StringSetting.NotDefine)
        {
            Anarchy.Configuration.Presets.TitanSkinPreset set = new Anarchy.Configuration.Presets.TitanSkinPreset(SkinSettings.TitanSet.Value);
            set.Load();
            
            int num = UnityEngine.Random.Range(0, 9);
            if (num == 3)
            {
                num = 9;
            }
            skin = set.RandomizePairs ? Random.Range(0, 5) : skin;
            if(set.HairTypes[skin] >= 0)
            {
                num = (int)set.HairTypes[skin];
            }
            string hair = set.Hairs[skin];
            int num3 = Random.Range(1, 8);
            if (hasEye)
            {
                num3 = 0;
            }
            if(hair.EndsWith(".png") || hair.EndsWith(".jpg") || hair.EndsWith(".jpeg"))
            {
                ApplyHairSkin(num, num3, hair);
                if(IN_GAME_MAIN_CAMERA.GameType == GameType.MultiPlayer && PhotonNetwork.IsMasterClient && SkinSettings.TitanSkins.Value != 2)
                {
                    BasePV.RPC("setHairRPC2", PhotonTargets.OthersBuffered, new object[] { num, num3, hair });
                }
            }
            else
            {
                if (IN_GAME_MAIN_CAMERA.GameType == GameType.MultiPlayer)
                {
                    Color hair_color = HeroCostume.costume[UnityEngine.Random.Range(0, HeroCostume.costume.Length - 5)].hair_color;
                    object[] parameters = new object[]
                    {
                        num,
                        num3,
                        hair_color.r,
                        hair_color.g,
                        hair_color.b
                    };
                    BasePV.RPC("setHairPRC", PhotonTargets.AllBuffered, parameters);
                }
                else
                {
                    Color hair_color = HeroCostume.costume[UnityEngine.Random.Range(0, HeroCostume.costume.Length - 5)].hair_color;
                    this.setHairPRC(num, num3, hair_color.r, hair_color.g, hair_color.b);
                }
            }
        }
        else if(IN_GAME_MAIN_CAMERA.GameType == GameType.Single || BasePV.IsMine)
        {
            setHairVanilla();
        }
    }

    private void setHairVanilla()
    {
        UnityEngine.Object.Destroy(this.part_hair);
        int num = UnityEngine.Random.Range(0, CostumeHair.hairsM.Length);
        if (num == 3)
        {
            num = 9;
        }
        this.hairType = num;
        this.hair = CostumeHair.hairsM[num];
        if (this.hair.hair == string.Empty)
        {
            this.hair = CostumeHair.hairsM[9];
            this.hairType = 9;
        }
        this.part_hair = (GameObject)UnityEngine.Object.Instantiate(CacheResources.Load("Character/" + this.hair.hair));
        this.part_hair.transform.parent = this.hair_go_ref.transform.parent;
        this.part_hair.transform.position = this.hair_go_ref.transform.position;
        this.part_hair.transform.rotation = this.hair_go_ref.transform.rotation;
        this.part_hair.transform.localScale = this.hair_go_ref.transform.localScale;
        this.part_hair.renderer.material = CharacterMaterials.Materials[this.hair.texture];
        this.part_hair.renderer.material.color = HeroCostume.costume[UnityEngine.Random.Range(0, HeroCostume.costume.Length - 5)].hair_color;
        int num2 = UnityEngine.Random.Range(1, 8);
        this.setFacialTexture(this.eye, num2);
        if (IN_GAME_MAIN_CAMERA.GameType == GameType.MultiPlayer && BasePV.IsMine)
        {
            BasePV.RPC("setHairPRC", PhotonTargets.OthersBuffered, new object[]
            {
                this.hairType,
                num2,
                this.part_hair.renderer.material.color.r,
                this.part_hair.renderer.material.color.g,
                this.part_hair.renderer.material.color.b
            });
        }
    }

    public void setPunkHair()
    {
        UnityEngine.Object.Destroy(this.part_hair);
        this.hair = CostumeHair.hairsM[3];
        this.hairType = 3;
        GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(CacheResources.Load("Character/" + this.hair.hair));
        gameObject.transform.parent = this.hair_go_ref.transform.parent;
        gameObject.transform.position = this.hair_go_ref.transform.position;
        gameObject.transform.rotation = this.hair_go_ref.transform.rotation;
        gameObject.transform.localScale = this.hair_go_ref.transform.localScale;
        gameObject.renderer.material = CharacterMaterials.Materials[this.hair.texture];
        int num = UnityEngine.Random.Range(1, 4);
        if (num == 1)
        {
            gameObject.renderer.material.color = FengColor.hairPunk1;
        }
        if (num == 2)
        {
            gameObject.renderer.material.color = FengColor.hairPunk2;
        }
        if (num == 3)
        {
            gameObject.renderer.material.color = FengColor.hairPunk3;
        }
        this.part_hair = gameObject;
        this.setFacialTexture(this.eye, 0);
        if (IN_GAME_MAIN_CAMERA.GameType == GameType.MultiPlayer && BasePV.IsMine)
        {
            BasePV.RPC("setHairPRC", PhotonTargets.OthersBuffered, new object[]
            {
                this.hairType,
                0,
                this.part_hair.renderer.material.color.r,
                this.part_hair.renderer.material.color.g,
                this.part_hair.renderer.material.color.b
            });
        }
    }

    public void SetVar(int skin, bool eye)
    {
        this.skin = skin;
        hasEye = eye;
    }
}