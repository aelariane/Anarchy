//TO DO: Rewrite
using ExitGames.Client.Photon;
using UnityEngine;

public class CostumeConeveter
{
    private static int DivisionToInt(DIVISION id)
    {
        if (id == DIVISION.TheGarrison)
        {
            return 0;
        }
        if (id == DIVISION.TheMilitaryPolice)
        {
            return 1;
        }
        if (id == DIVISION.TheSurveryCorps)
        {
            return 2;
        }
        if (id == DIVISION.TraineesSquad)
        {
            return 3;
        }
        return 2;
    }

    private static DIVISION IntToDivision(int id)
    {
        if (id == 0)
        {
            return DIVISION.TheGarrison;
        }
        if (id == 1)
        {
            return DIVISION.TheMilitaryPolice;
        }
        if (id == 2)
        {
            return DIVISION.TheSurveryCorps;
        }
        if (id == 3)
        {
            return DIVISION.TraineesSquad;
        }
        return DIVISION.TheSurveryCorps;
    }

    private static Sex IntToSex(int id)
    {
        if (id == 0)
        {
            return Sex.Female;
        }
        if (id == 1)
        {
            return Sex.Male;
        }
        return Sex.Male;
    }

    private static UNIFORM_TYPE IntToUniformType(int id)
    {
        if (id == 0)
        {
            return UNIFORM_TYPE.CasualA;
        }
        if (id == 1)
        {
            return UNIFORM_TYPE.CasualB;
        }
        if (id == 2)
        {
            return UNIFORM_TYPE.UniformA;
        }
        if (id == 3)
        {
            return UNIFORM_TYPE.UniformB;
        }
        if (id == 4)
        {
            return UNIFORM_TYPE.CasualAHSS;
        }
        return UNIFORM_TYPE.UniformA;
    }

    private static int SexToInt(Sex id)
    {
        if (id == Sex.Female)
        {
            return 0;
        }
        if (id == Sex.Male)
        {
            return 1;
        }
        return 1;
    }

    private static int UniformTypeToInt(UNIFORM_TYPE id)
    {
        if (id == UNIFORM_TYPE.CasualA)
        {
            return 0;
        }
        if (id == UNIFORM_TYPE.CasualB)
        {
            return 1;
        }
        if (id == UNIFORM_TYPE.UniformA)
        {
            return 2;
        }
        if (id == UNIFORM_TYPE.UniformB)
        {
            return 3;
        }
        if (id == UNIFORM_TYPE.CasualAHSS)
        {
            return 4;
        }
        return 2;
    }

    public static void HeroCostumeToLocalData(HeroCostume costume, string slot)
    {
        slot = slot.ToUpper();
        PlayerPrefs.SetInt(slot + PhotonPlayerProperty.sex, CostumeConeveter.SexToInt(costume.sex));
        PlayerPrefs.SetInt(slot + PhotonPlayerProperty.costumeId, costume.costumeId);
        PlayerPrefs.SetInt(slot + PhotonPlayerProperty.heroCostumeId, costume.id);
        PlayerPrefs.SetInt(slot + PhotonPlayerProperty.cape, (!costume.cape) ? 0 : 1);
        PlayerPrefs.SetInt(slot + PhotonPlayerProperty.hairInfo, costume.hairInfo.id);
        PlayerPrefs.SetInt(slot + PhotonPlayerProperty.eye_texture_id, costume.eye_texture_id);
        PlayerPrefs.SetInt(slot + PhotonPlayerProperty.beard_texture_id, costume.beard_texture_id);
        PlayerPrefs.SetInt(slot + PhotonPlayerProperty.glass_texture_id, costume.glass_texture_id);
        PlayerPrefs.SetInt(slot + PhotonPlayerProperty.skin_color, costume.skin_color);
        PlayerPrefs.SetFloat(slot + PhotonPlayerProperty.hair_color1, costume.hair_color.r);
        PlayerPrefs.SetFloat(slot + PhotonPlayerProperty.hair_color2, costume.hair_color.g);
        PlayerPrefs.SetFloat(slot + PhotonPlayerProperty.hair_color3, costume.hair_color.b);
        PlayerPrefs.SetInt(slot + PhotonPlayerProperty.division, CostumeConeveter.DivisionToInt(costume.division));
        PlayerPrefs.SetInt(slot + PhotonPlayerProperty.statSPD, costume.stat.Spd);
        PlayerPrefs.SetInt(slot + PhotonPlayerProperty.statGAS, costume.stat.Gas);
        PlayerPrefs.SetInt(slot + PhotonPlayerProperty.statBLA, costume.stat.Bla);
        PlayerPrefs.SetInt(slot + PhotonPlayerProperty.statACL, costume.stat.Acl);
        PlayerPrefs.SetString(slot + PhotonPlayerProperty.statSKILL, costume.stat.skillID);
    }

    public static void HeroCostumeToPhotonData(HeroCostume costume, PhotonPlayer player)
    {
        Hashtable hash = new Hashtable
        {
            {
                PhotonPlayerProperty.sex,
                CostumeConeveter.SexToInt(costume.sex)
            },
            {
                PhotonPlayerProperty.costumeId,
                costume.costumeId
            },
            {
                PhotonPlayerProperty.heroCostumeId,
                costume.id
            },
            {
                PhotonPlayerProperty.cape,
                costume.cape
            },
            {
                PhotonPlayerProperty.hairInfo,
                costume.hairInfo.id
            },
            {
                PhotonPlayerProperty.eye_texture_id,
                costume.eye_texture_id
            },
            {
                PhotonPlayerProperty.beard_texture_id,
                costume.beard_texture_id
            },
            {
                PhotonPlayerProperty.glass_texture_id,
                costume.glass_texture_id
            },
            {
                PhotonPlayerProperty.skin_color,
                costume.skin_color
            },
            {
                PhotonPlayerProperty.hair_color1,
                costume.hair_color.r
            },
            {
                PhotonPlayerProperty.hair_color2,
                costume.hair_color.g
            },
            {
                PhotonPlayerProperty.hair_color3,
                costume.hair_color.b
            },
            {
                PhotonPlayerProperty.division,
                CostumeConeveter.DivisionToInt(costume.division)
            },
            {
                PhotonPlayerProperty.statSPD,
                costume.stat.Spd
            },
            {
                PhotonPlayerProperty.statGAS,
                costume.stat.Gas
            },
            {
                PhotonPlayerProperty.statBLA,
                costume.stat.Bla
            },
            {
                PhotonPlayerProperty.statACL,
                costume.stat.Acl
            },
            {
                PhotonPlayerProperty.statSKILL,
                costume.stat.skillID
            }
        };
        PhotonNetwork.player.SetCustomProperties(hash);
    }

    public static HeroCostume LocalDataToHeroCostume(string slot)
    {
        slot = slot.ToUpper();
        if (!PlayerPrefs.HasKey(slot + PhotonPlayerProperty.sex))
        {
            return HeroCostume.costume[0];
        }
        HeroCostume heroCostume = new HeroCostume();
        heroCostume.sex = IntToSex(PlayerPrefs.GetInt(slot + PhotonPlayerProperty.sex));
        heroCostume.id = PlayerPrefs.GetInt(slot + PhotonPlayerProperty.heroCostumeId);
        heroCostume.costumeId = PlayerPrefs.GetInt(slot + PhotonPlayerProperty.costumeId);
        heroCostume.cape = (PlayerPrefs.GetInt(slot + PhotonPlayerProperty.cape) == 1);
        heroCostume.hairInfo = ((heroCostume.sex != Sex.Male) ? CostumeHair.hairsF[PlayerPrefs.GetInt(slot + PhotonPlayerProperty.hairInfo)] : CostumeHair.hairsM[PlayerPrefs.GetInt(slot + PhotonPlayerProperty.hairInfo)]);
        heroCostume.eye_texture_id = PlayerPrefs.GetInt(slot + PhotonPlayerProperty.eye_texture_id);
        heroCostume.beard_texture_id = PlayerPrefs.GetInt(slot + PhotonPlayerProperty.beard_texture_id);
        heroCostume.glass_texture_id = PlayerPrefs.GetInt(slot + PhotonPlayerProperty.glass_texture_id);
        heroCostume.skin_color = PlayerPrefs.GetInt(slot + PhotonPlayerProperty.skin_color);
        heroCostume.hair_color = new Color(PlayerPrefs.GetFloat(slot + PhotonPlayerProperty.hair_color1), PlayerPrefs.GetFloat(slot + PhotonPlayerProperty.hair_color2), PlayerPrefs.GetFloat(slot + PhotonPlayerProperty.hair_color3));
        heroCostume.division = CostumeConeveter.IntToDivision(PlayerPrefs.GetInt(slot + PhotonPlayerProperty.division));
        heroCostume.stat = new HeroStat();
        heroCostume.stat.Spd = PlayerPrefs.GetInt(slot + PhotonPlayerProperty.statSPD);
        heroCostume.stat.Gas = PlayerPrefs.GetInt(slot + PhotonPlayerProperty.statGAS);
        heroCostume.stat.Bla = PlayerPrefs.GetInt(slot + PhotonPlayerProperty.statBLA);
        heroCostume.stat.Acl = PlayerPrefs.GetInt(slot + PhotonPlayerProperty.statACL);
        heroCostume.stat.skillID = PlayerPrefs.GetString(slot + PhotonPlayerProperty.statSKILL);
        heroCostume.setBodyByCostumeId(-1);
        heroCostume.setMesh();
        heroCostume.setTexture();
        return heroCostume;
    }

    public static HeroCostume PhotonDataToHeroCostume(PhotonPlayer player)
    {
        HeroCostume heroCostume = new HeroCostume();
        Hashtable props = player.Properties;
        heroCostume.sex = IntToSex(props.Get<int>(PhotonPlayerProperty.sex));
        heroCostume.costumeId = props.Get<int>(PhotonPlayerProperty.costumeId);
        heroCostume.id = props.Get<int>(PhotonPlayerProperty.heroCostumeId);
        heroCostume.cape = props.Get<bool>(PhotonPlayerProperty.cape);
        CostumeHair[] hairs = IntToSex(props.Get<int>(PhotonPlayerProperty.sex)) == Sex.Male ? CostumeHair.hairsM : CostumeHair.hairsF;
        int hairId  = props.Get<int>(PhotonPlayerProperty.hairInfo);
        heroCostume.hairInfo = hairs[hairId < hairs.Length ? hairId : 0];
        heroCostume.eye_texture_id = props.Get<int>(PhotonPlayerProperty.eye_texture_id);
        heroCostume.beard_texture_id = props.Get<int>(PhotonPlayerProperty.beard_texture_id);
        heroCostume.glass_texture_id = props.Get<int>(PhotonPlayerProperty.glass_texture_id);
        heroCostume.skin_color = props.Get<int>(PhotonPlayerProperty.skin_color);
        heroCostume.hair_color = new Color(props.Get<float>(PhotonPlayerProperty.hair_color1), props.Get<float>(PhotonPlayerProperty.hair_color2), props.Get<float>(PhotonPlayerProperty.hair_color3));
        heroCostume.division = IntToDivision(props.Get<int>(PhotonPlayerProperty.division));
        HeroStat stat = new HeroStat();
        stat.Spd = props.Get<int>(PhotonPlayerProperty.statSPD);
        stat.Gas = props.Get<int>(PhotonPlayerProperty.statGAS);
        stat.Bla = props.Get<int>(PhotonPlayerProperty.statBLA);
        stat.Acl = props.Get<int>(PhotonPlayerProperty.statACL);
        string skill = props.Get<string>(PhotonPlayerProperty.statSKILL);
        stat.skillID = skill ?? "levi";
        heroCostume.stat = stat;
        heroCostume.setBodyByCostumeId(-1);
        heroCostume.setMesh();
        heroCostume.setTexture();
        return heroCostume;
    }
}