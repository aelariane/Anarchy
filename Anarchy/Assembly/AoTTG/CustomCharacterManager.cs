using UnityEngine;

public class CustomCharacterManager : MonoBehaviour
{
    private int capeId;
    private int[] capeOption;
    private int costumeId = 1;
    private HeroCostume[] costumeOption;
    private string currentSlot = "Set 1";
    private int divisionId;
    private DIVISION[] divisionOption;
    private int eyeId;
    private int[] eyeOption;
    private int faceId;
    private int[] faceOption;
    private int glassId;
    private int[] glassOption;
    private int hairId;
    private int[] hairOption;
    private int presetId;
    private HERO_SETUP setup;
    private int sexId;
    private Sex[] sexOption;
    private int skillId;
    private string[] skillOption;
    private int skinId;
    private int[] skinOption;
    public GameObject character;
    public GameObject hairB;
    public GameObject hairG;
    public GameObject hairR;
    public GameObject labelACL;
    public GameObject labelBLA;
    public GameObject labelCape;
    public GameObject labelCostume;
    public GameObject labelDivision;
    public GameObject labelEye;
    public GameObject labelFace;
    public GameObject labelGAS;
    public GameObject labelGlass;
    public GameObject labelHair;
    public GameObject labelPOINT;
    public GameObject labelPreset;
    public GameObject labelSex;
    public GameObject labelSKILL;
    public GameObject labelSkin;
    public GameObject labelSPD;

    private int calTotalPoints()
    {
        if (this.setup.myCostume != null)
        {
            int num = 0;
            num += this.setup.myCostume.stat.Spd;
            num += this.setup.myCostume.stat.Gas;
            num += this.setup.myCostume.stat.Bla;
            return num + this.setup.myCostume.stat.Acl;
        }
        return 400;
    }

    private void copyBodyCostume(HeroCostume from, HeroCostume to)
    {
        to.arm_l_mesh = from.arm_l_mesh;
        to.arm_r_mesh = from.arm_r_mesh;
        to.body_mesh = from.body_mesh;
        to.body_texture = from.body_texture;
        to.uniform_type = from.uniform_type;
        to.part_chest_1_object_mesh = from.part_chest_1_object_mesh;
        to.part_chest_1_object_texture = from.part_chest_1_object_texture;
        to.part_chest_object_mesh = from.part_chest_object_mesh;
        to.part_chest_object_texture = from.part_chest_object_texture;
        to.part_chest_skinned_cloth_mesh = from.part_chest_skinned_cloth_mesh;
        to.part_chest_skinned_cloth_texture = from.part_chest_skinned_cloth_texture;
        to.division = from.division;
        to.id = from.id;
        to.costumeId = from.costumeId;
    }

    private void copyCostume(HeroCostume from, HeroCostume to, bool init = false)
    {
        this.copyBodyCostume(from, to);
        to.sex = from.sex;
        to.hair_mesh = from.hair_mesh;
        to.hair_1_mesh = from.hair_1_mesh;
        to.hair_color = new Color(from.hair_color.r, from.hair_color.g, from.hair_color.b);
        to.hairInfo = from.hairInfo;
        to.cape = from.cape;
        to.cape_mesh = from.cape_mesh;
        to.cape_texture = from.cape_texture;
        to.brand1_mesh = from.brand1_mesh;
        to.brand2_mesh = from.brand2_mesh;
        to.brand3_mesh = from.brand3_mesh;
        to.brand4_mesh = from.brand4_mesh;
        to.brand_texture = from.brand_texture;
        to._3dmg_texture = from._3dmg_texture;
        to.face_texture = from.face_texture;
        to.eye_mesh = from.eye_mesh;
        to.glass_mesh = from.glass_mesh;
        to.beard_mesh = from.beard_mesh;
        to.eye_texture_id = from.eye_texture_id;
        to.beard_texture_id = from.beard_texture_id;
        to.glass_texture_id = from.glass_texture_id;
        to.skin_color = from.skin_color;
        to.skin_texture = from.skin_texture;
        to.beard_texture_id = from.beard_texture_id;
        to.hand_l_mesh = from.hand_l_mesh;
        to.hand_r_mesh = from.hand_r_mesh;
        to.mesh_3dmg = from.mesh_3dmg;
        to.mesh_3dmg_gas_l = from.mesh_3dmg_gas_l;
        to.mesh_3dmg_gas_r = from.mesh_3dmg_gas_r;
        to.mesh_3dmg_belt = from.mesh_3dmg_belt;
        to.weapon_l_mesh = from.weapon_l_mesh;
        to.weapon_r_mesh = from.weapon_r_mesh;
        if (init)
        {
            to.stat = new HeroStat();
            to.stat.Acl = 100;
            to.stat.Spd = 100;
            to.stat.Gas = 100;
            to.stat.Bla = 100;
            to.stat.skillID = "mikasa";
        }
        else
        {
            to.stat = new HeroStat();
            to.stat.Acl = from.stat.Acl;
            to.stat.Spd = from.stat.Spd;
            to.stat.Gas = from.stat.Gas;
            to.stat.Bla = from.stat.Bla;
            to.stat.skillID = from.stat.skillID;
        }
    }

    private void CostumeDataToMyID()
    {
        for (int i = 0; i < this.sexOption.Length; i++)
        {
            if (this.sexOption[i] == this.setup.myCostume.sex)
            {
                this.sexId = i;
                break;
            }
        }
        for (int i = 0; i < this.eyeOption.Length; i++)
        {
            if (this.eyeOption[i] == this.setup.myCostume.eye_texture_id)
            {
                this.eyeId = i;
                break;
            }
        }
        this.faceId = -1;
        for (int i = 0; i < this.faceOption.Length; i++)
        {
            if (this.faceOption[i] == this.setup.myCostume.beard_texture_id)
            {
                this.faceId = i;
                break;
            }
        }
        this.glassId = -1;
        for (int i = 0; i < this.glassOption.Length; i++)
        {
            if (this.glassOption[i] == this.setup.myCostume.glass_texture_id)
            {
                this.glassId = i;
                break;
            }
        }
        for (int i = 0; i < this.hairOption.Length; i++)
        {
            if (this.hairOption[i] == this.setup.myCostume.hairInfo.id)
            {
                this.hairId = i;
                break;
            }
        }
        for (int i = 0; i < this.skinOption.Length; i++)
        {
            if (this.skinOption[i] == this.setup.myCostume.skin_color)
            {
                this.skinId = i;
                break;
            }
        }
        if (this.setup.myCostume.cape)
        {
            this.capeId = 1;
        }
        else
        {
            this.capeId = 0;
        }
        for (int i = 0; i < this.divisionOption.Length; i++)
        {
            if (this.divisionOption[i] == this.setup.myCostume.division)
            {
                this.divisionId = i;
                break;
            }
        }
        this.costumeId = this.setup.myCostume.costumeId;
        float r = this.setup.myCostume.hair_color.r;
        float g = this.setup.myCostume.hair_color.g;
        float b = this.setup.myCostume.hair_color.b;
        this.hairR.GetComponent<UISlider>().sliderValue = r;
        this.hairG.GetComponent<UISlider>().sliderValue = g;
        this.hairB.GetComponent<UISlider>().sliderValue = b;
        for (int i = 0; i < this.skillOption.Length; i++)
        {
            if (this.skillOption[i] == this.setup.myCostume.stat.skillID)
            {
                this.skillId = i;
                break;
            }
        }
    }

    private void freshLabel()
    {
        this.labelSex.GetComponent<UILabel>().text = this.sexOption[this.sexId].ToString();
        this.labelEye.GetComponent<UILabel>().text = "eye_" + this.eyeId.ToString();
        this.labelFace.GetComponent<UILabel>().text = "face_" + this.faceId.ToString();
        this.labelGlass.GetComponent<UILabel>().text = "glass_" + this.glassId.ToString();
        this.labelHair.GetComponent<UILabel>().text = "hair_" + this.hairId.ToString();
        this.labelSkin.GetComponent<UILabel>().text = "skin_" + this.skinId.ToString();
        this.labelCostume.GetComponent<UILabel>().text = "costume_" + this.costumeId.ToString();
        this.labelCape.GetComponent<UILabel>().text = "cape_" + this.capeId.ToString();
        this.labelDivision.GetComponent<UILabel>().text = this.divisionOption[this.divisionId].ToString();
        this.labelPOINT.GetComponent<UILabel>().text = "Points: " + (400 - this.calTotalPoints()).ToString();
        this.labelSPD.GetComponent<UILabel>().text = "SPD " + this.setup.myCostume.stat.Spd.ToString();
        this.labelGAS.GetComponent<UILabel>().text = "GAS " + this.setup.myCostume.stat.Gas.ToString();
        this.labelBLA.GetComponent<UILabel>().text = "BLA " + this.setup.myCostume.stat.Bla.ToString();
        this.labelACL.GetComponent<UILabel>().text = "ACL " + this.setup.myCostume.stat.Acl.ToString();
        this.labelSKILL.GetComponent<UILabel>().text = "SKILL " + this.setup.myCostume.stat.skillID.ToString();
    }

    private void setHairColor()
    {
        if (this.setup.part_hair != null)
        {
            this.setup.part_hair.renderer.material.color = this.setup.myCostume.hair_color;
        }
        if (this.setup.part_hair_1 != null)
        {
            this.setup.part_hair_1.renderer.material.color = this.setup.myCostume.hair_color;
        }
    }

    private void setStatPoint(CreateStat type, int pt)
    {
        switch (type)
        {
            case CreateStat.SPD:
                this.setup.myCostume.stat.Spd += pt;
                break;

            case CreateStat.GAS:
                this.setup.myCostume.stat.Gas += pt;
                break;

            case CreateStat.BLA:
                this.setup.myCostume.stat.Bla += pt;
                break;

            case CreateStat.ACL:
                this.setup.myCostume.stat.Acl += pt;
                break;
        }
        this.setup.myCostume.stat.Spd = Mathf.Clamp(this.setup.myCostume.stat.Spd, 75, 125);
        this.setup.myCostume.stat.Gas = Mathf.Clamp(this.setup.myCostume.stat.Gas, 75, 125);
        this.setup.myCostume.stat.Bla = Mathf.Clamp(this.setup.myCostume.stat.Bla, 75, 125);
        this.setup.myCostume.stat.Acl = Mathf.Clamp(this.setup.myCostume.stat.Acl, 75, 125);
        this.freshLabel();
    }

    private void Start()
    {
        QualitySettings.SetQualityLevel(5, true);
        this.costumeOption = HeroCostume.costumeOption;
        this.setup = this.character.GetComponent<HERO_SETUP>();
        this.setup.Init();
        this.setup.myCostume = new HeroCostume();
        this.copyCostume(HeroCostume.costume[2], this.setup.myCostume, false);
        this.setup.myCostume.setMesh();
        this.setup.SetCharacterComponent();
        this.sexOption = new Sex[]
        {
            Sex.Male,
            Sex.Female
        };
        this.eyeOption = new int[28];
        for (int i = 0; i < 28; i++)
        {
            this.eyeOption[i] = i;
        }
        this.faceOption = new int[14];
        for (int i = 0; i < 14; i++)
        {
            this.faceOption[i] = i + 32;
        }
        this.glassOption = new int[10];
        for (int i = 0; i < 10; i++)
        {
            this.glassOption[i] = i + 48;
        }
        this.hairOption = new int[11];
        for (int i = 0; i < 11; i++)
        {
            this.hairOption[i] = i;
        }
        this.skinOption = new int[3];
        for (int i = 0; i < 3; i++)
        {
            this.skinOption[i] = i + 1;
        }
        this.capeOption = new int[2];
        for (int i = 0; i < 2; i++)
        {
            this.capeOption[i] = i;
        }
        this.divisionOption = new DIVISION[]
        {
            DIVISION.TraineesSquad,
            DIVISION.TheGarrison,
            DIVISION.TheMilitaryPolice,
            DIVISION.TheSurveryCorps
        };
        this.skillOption = new string[]
        {
            "mikasa",
            "levi",
            "sasha",
            "jean",
            "marco",
            "armin",
            "petra"
        };
        this.CostumeDataToMyID();
        this.freshLabel();
    }

    private int toNext(int id, int Count, int start = 0)
    {
        id++;
        if (id >= Count)
        {
            id = start;
        }
        id = Mathf.Clamp(id, start, start + Count - 1);
        return id;
    }

    private int toPrev(int id, int Count, int start = 0)
    {
        id--;
        if (id < start)
        {
            id = Count - 1;
        }
        id = Mathf.Clamp(id, start, start + Count - 1);
        return id;
    }

    public void LoadData()
    {
        HeroCostume heroCostume = CostumeConeveter.LocalDataToHeroCostume(this.currentSlot);
        if (heroCostume != null)
        {
            this.copyCostume(heroCostume, this.setup.myCostume, false);
            this.setup.DeleteCharacterComponent();
            this.setup.SetCharacterComponent();
        }
        this.CostumeDataToMyID();
        this.freshLabel();
    }

    public void nextOption(CreatePart part)
    {
        if (part == CreatePart.Preset)
        {
            this.presetId = this.toNext(this.presetId, HeroCostume.costume.Length, 0);
            this.copyCostume(HeroCostume.costume[this.presetId], this.setup.myCostume, true);
            this.CostumeDataToMyID();
            this.setup.DeleteCharacterComponent();
            this.setup.SetCharacterComponent();
            this.labelPreset.GetComponent<UILabel>().text = HeroCostume.costume[this.presetId].name;
            this.freshLabel();
        }
        else
        {
            this.toOption(part, true);
        }
    }

    public void nextStatOption(CreateStat type)
    {
        if (type == CreateStat.Skill)
        {
            this.skillId = this.toNext(this.skillId, this.skillOption.Length, 0);
            this.setup.myCostume.stat.skillID = this.skillOption[this.skillId];
            this.character.GetComponent<CharacterCreateAnimationControl>().playAttack(this.setup.myCostume.stat.skillID);
            this.freshLabel();
            return;
        }
        if (this.calTotalPoints() >= 400)
        {
            return;
        }
        this.setStatPoint(type, 1);
    }

    public void OnHairBChange(float value)
    {
        if (this.setup != null && this.setup.myCostume != null && this.setup.part_hair != null)
        {
            this.setup.myCostume.hair_color = new Color(this.setup.part_hair.renderer.material.color.r, this.setup.part_hair.renderer.material.color.g, value);
            this.setHairColor();
        }
    }

    public void OnHairGChange(float value)
    {
        if (this.setup.myCostume != null && this.setup.part_hair != null)
        {
            this.setup.myCostume.hair_color = new Color(this.setup.part_hair.renderer.material.color.r, value, this.setup.part_hair.renderer.material.color.b);
            this.setHairColor();
        }
    }

    public void OnHairRChange(float value)
    {
        if (this.setup.myCostume != null && this.setup.part_hair != null)
        {
            this.setup.myCostume.hair_color = new Color(value, this.setup.part_hair.renderer.material.color.g, this.setup.part_hair.renderer.material.color.b);
            this.setHairColor();
        }
    }

    public void OnSoltChange(string id)
    {
        this.currentSlot = id;
    }

    public void prevOption(CreatePart part)
    {
        if (part == CreatePart.Preset)
        {
            this.presetId = this.toPrev(this.presetId, HeroCostume.costume.Length, 0);
            this.copyCostume(HeroCostume.costume[this.presetId], this.setup.myCostume, true);
            this.CostumeDataToMyID();
            this.setup.DeleteCharacterComponent();
            this.setup.SetCharacterComponent();
            this.labelPreset.GetComponent<UILabel>().text = HeroCostume.costume[this.presetId].name;
            this.freshLabel();
        }
        else
        {
            this.toOption(part, false);
        }
    }

    public void prevStatOption(CreateStat type)
    {
        if (type == CreateStat.Skill)
        {
            this.skillId = this.toPrev(this.skillId, this.skillOption.Length, 0);
            this.setup.myCostume.stat.skillID = this.skillOption[this.skillId];
            this.character.GetComponent<CharacterCreateAnimationControl>().playAttack(this.setup.myCostume.stat.skillID);
            this.freshLabel();
            return;
        }
        this.setStatPoint(type, -1);
    }

    public void SaveData()
    {
        CostumeConeveter.HeroCostumeToLocalData(this.setup.myCostume, this.currentSlot);
    }

    public void toOption(CreatePart part, bool next)
    {
        switch (part)
        {
            case CreatePart.Sex:
                this.sexId = ((!next) ? this.toPrev(this.sexId, this.sexOption.Length, 0) : this.toNext(this.sexId, this.sexOption.Length, 0));
                if (this.sexId == 0)
                {
                    this.costumeId = 11;
                }
                else
                {
                    this.costumeId = 0;
                }
                this.copyCostume(this.costumeOption[this.costumeId], this.setup.myCostume, true);
                this.setup.myCostume.sex = this.sexOption[this.sexId];
                this.character.GetComponent<CharacterCreateAnimationControl>().toStand();
                this.CostumeDataToMyID();
                this.setup.DeleteCharacterComponent();
                this.setup.SetCharacterComponent();
                break;

            case CreatePart.Eye:
                this.eyeId = ((!next) ? this.toPrev(this.eyeId, this.eyeOption.Length, 0) : this.toNext(this.eyeId, this.eyeOption.Length, 0));
                this.setup.myCostume.eye_texture_id = this.eyeId;
                this.setup.SetFacialTexture(this.setup.part_eye, this.eyeOption[this.eyeId]);
                break;

            case CreatePart.Face:
                this.faceId = ((!next) ? this.toPrev(this.faceId, this.faceOption.Length, 0) : this.toNext(this.faceId, this.faceOption.Length, 0));
                this.setup.myCostume.beard_texture_id = this.faceOption[this.faceId];
                if (this.setup.part_face == null)
                {
                    this.setup.CreateFace();
                }
                this.setup.SetFacialTexture(this.setup.part_face, this.faceOption[this.faceId]);
                break;

            case CreatePart.Glass:
                this.glassId = ((!next) ? this.toPrev(this.glassId, this.glassOption.Length, 0) : this.toNext(this.glassId, this.glassOption.Length, 0));
                this.setup.myCostume.glass_texture_id = this.glassOption[this.glassId];
                if (this.setup.part_glass == null)
                {
                    this.setup.CreateGlass();
                }
                this.setup.SetFacialTexture(this.setup.part_glass, this.glassOption[this.glassId]);
                break;

            case CreatePart.Hair:
                this.hairId = ((!next) ? this.toPrev(this.hairId, this.hairOption.Length, 0) : this.toNext(this.hairId, this.hairOption.Length, 0));
                if (this.sexId == 0)
                {
                    this.setup.myCostume.hair_mesh = CostumeHair.hairsM[this.hairOption[this.hairId]].hair;
                    this.setup.myCostume.hair_1_mesh = CostumeHair.hairsM[this.hairOption[this.hairId]].hair_1;
                    this.setup.myCostume.hairInfo = CostumeHair.hairsM[this.hairOption[this.hairId]];
                }
                else
                {
                    this.setup.myCostume.hair_mesh = CostumeHair.hairsF[this.hairOption[this.hairId]].hair;
                    this.setup.myCostume.hair_1_mesh = CostumeHair.hairsF[this.hairOption[this.hairId]].hair_1;
                    this.setup.myCostume.hairInfo = CostumeHair.hairsF[this.hairOption[this.hairId]];
                }
                this.setup.CreateHair();
                this.setHairColor();
                break;

            case CreatePart.Skin:
                if (this.setup.myCostume.uniform_type == UNIFORM_TYPE.CasualAHSS)
                {
                    this.skinId = 2;
                }
                else
                {
                    this.skinId = ((!next) ? this.toPrev(this.skinId, 2, 0) : this.toNext(this.skinId, 2, 0));
                }
                this.setup.myCostume.skin_color = this.skinOption[this.skinId];
                this.setup.myCostume.setTexture();
                this.setup.SetSkin();
                break;

            case CreatePart.Costume:
                if (this.setup.myCostume.uniform_type == UNIFORM_TYPE.CasualAHSS)
                {
                    this.costumeId = 25;
                }
                else if (this.sexId == 0)
                {
                    this.costumeId = ((!next) ? this.toPrev(this.costumeId, 24, 10) : this.toNext(this.costumeId, 24, 10));
                }
                else
                {
                    this.costumeId = ((!next) ? this.toPrev(this.costumeId, 10, 0) : this.toNext(this.costumeId, 10, 0));
                }
                this.copyBodyCostume(this.costumeOption[this.costumeId], this.setup.myCostume);
                this.setup.myCostume.setMesh();
                this.setup.myCostume.setTexture();
                this.setup.CreateUpperBody();
                this.setup.CreateLeftArm();
                this.setup.CreateRightArm();
                this.setup.CreateLowerBody();
                break;

            case CreatePart.Cape:
                this.capeId = ((!next) ? this.toPrev(this.capeId, this.capeOption.Length, 0) : this.toNext(this.capeId, this.capeOption.Length, 0));
                this.setup.myCostume.cape = (this.capeId == 1);
                this.setup.myCostume.setCape();
                this.setup.myCostume.setTexture();
                this.setup.CreateCape();
                break;

            case CreatePart.Division:
                this.divisionId = ((!next) ? this.toPrev(this.divisionId, this.divisionOption.Length, 0) : this.toNext(this.divisionId, this.divisionOption.Length, 0));
                this.setup.myCostume.division = this.divisionOption[this.divisionId];
                this.setup.myCostume.setTexture();
                this.setup.CreateUpperBody();
                break;
        }
        this.freshLabel();
    }
}