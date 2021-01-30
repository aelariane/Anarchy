using RC;
using System;

public class RCActionHelper
{
    public int helperClass;

    public int helperType;

    private RCActionHelper nextHelper;

    private object parameters;

    public RCActionHelper(int sentClass, int sentType, object options)
    {
        this.helperClass = sentClass;
        this.helperType = sentType;
        this.parameters = options;
    }

    public void callException(string str)
    {
        Anarchy.UI.Chat.Add(str);
    }

    public bool returnBool(object sentObject)
    {
        object parameters = sentObject;
        if (this.parameters != null)
        {
            parameters = this.parameters;
        }
        switch (this.helperClass)
        {
            case 0:
                return (bool)parameters;

            case 1:
                {
                    RCActionHelper helper = (RCActionHelper)parameters;
                    switch (this.helperType)
                    {
                        case 0:
                            return this.nextHelper.returnBool(RCManager.intVariables[helper.returnString(null)]);

                        case 1:
                            return (bool)RCManager.boolVariables[helper.returnString(null)];

                        case 2:
                            return this.nextHelper.returnBool(RCManager.stringVariables[helper.returnString(null)]);

                        case 3:
                            return this.nextHelper.returnBool(RCManager.floatVariables[helper.returnString(null)]);

                        case 4:
                            return this.nextHelper.returnBool(RCManager.playerVariables[helper.returnString(null)]);

                        case 5:
                            return this.nextHelper.returnBool(RCManager.titanVariables[helper.returnString(null)]);

                        default:
                            return false;
                    }
                }
            case 2:
                {
                    PhotonPlayer player = (PhotonPlayer)parameters;
                    if (player != null)
                    {
                        switch (this.helperType)
                        {
                            case 0:
                                return this.nextHelper.returnBool(player.Properties["team"]);

                            case 1:
                                return this.nextHelper.returnBool(player.Properties["RCteam"]);

                            case 2:
                                return !(bool)player.Properties["dead"];

                            case 3:
                                return this.nextHelper.returnBool(player.Properties["isTitan"]);

                            case 4:
                                return this.nextHelper.returnBool(player.Properties["kills"]);

                            case 5:
                                return this.nextHelper.returnBool(player.Properties["deaths"]);

                            case 6:
                                return this.nextHelper.returnBool(player.Properties["max_dmg"]);

                            case 7:
                                return this.nextHelper.returnBool(player.Properties["total_dmg"]);

                            case 8:
                                return this.nextHelper.returnBool(player.Properties["customInt"]);

                            case 9:
                                return (bool)player.Properties["customBool"];

                            case 10:
                                return this.nextHelper.returnBool(player.Properties["customString"]);

                            case 11:
                                return this.nextHelper.returnBool(player.Properties["customFloat"]);

                            case 12:
                                return this.nextHelper.returnBool(player.Properties["name"]);

                            case 13:
                                return this.nextHelper.returnBool(player.Properties["guildName"]);

                            case 14:
                                {
                                    int iD = player.ID;
                                    if (RCManager.heroHash.ContainsKey(iD))
                                    {
                                        HERO hero = (HERO)RCManager.heroHash[iD];
                                        return this.nextHelper.returnBool(hero.transform.position.x);
                                    }
                                    return false;
                                }
                            case 15:
                                {
                                    int key = player.ID;
                                    if (RCManager.heroHash.ContainsKey(key))
                                    {
                                        HERO hero2 = (HERO)RCManager.heroHash[key];
                                        return this.nextHelper.returnBool(hero2.transform.position.y);
                                    }
                                    return false;
                                }
                            case 16:
                                {
                                    int num6 = player.ID;
                                    if (RCManager.heroHash.ContainsKey(num6))
                                    {
                                        HERO hero3 = (HERO)RCManager.heroHash[num6];
                                        return this.nextHelper.returnBool(hero3.transform.position.z);
                                    }
                                    return false;
                                }
                            case 17:
                                {
                                    int num7 = player.ID;
                                    if (RCManager.heroHash.ContainsKey(num7))
                                    {
                                        HERO hero4 = (HERO)RCManager.heroHash[num7];
                                        return this.nextHelper.returnBool(hero4.rigidbody.velocity.magnitude);
                                    }
                                    return false;
                                }
                        }
                    }
                    return false;
                }
            case 3:
                {
                    TITAN titan = (TITAN)parameters;
                    if (titan != null)
                    {
                        switch (this.helperType)
                        {
                            case 0:
                                return this.nextHelper.returnBool(titan.abnormalType);

                            case 1:
                                return this.nextHelper.returnBool(titan.myLevel);

                            case 2:
                                return this.nextHelper.returnBool(titan.currentHealth);

                            case 3:
                                return this.nextHelper.returnBool(titan.transform.position.x);

                            case 4:
                                return this.nextHelper.returnBool(titan.transform.position.y);

                            case 5:
                                return this.nextHelper.returnBool(titan.transform.position.z);
                        }
                    }
                    return false;
                }
            case 4:
                {
                    RCActionHelper helper2 = (RCActionHelper)parameters;
                    RCRegion region = (RCRegion)RCManager.RCRegions[helper2.returnString(null)];
                    switch (this.helperType)
                    {
                        case 0:
                            return this.nextHelper.returnBool(region.GetRandomX());

                        case 1:
                            return this.nextHelper.returnBool(region.GetRandomY());

                        case 2:
                            return this.nextHelper.returnBool(region.GetRandomZ());

                        default:
                            return false;
                    }
                }
            case 5:
                switch (this.helperType)
                {
                    case 0:
                        return Convert.ToBoolean((int)parameters);

                    case 1:
                        return (bool)parameters;

                    case 2:
                        return Convert.ToBoolean((string)parameters);

                    case 3:
                        return Convert.ToBoolean((float)parameters);

                    default:
                        return false;
                }
            default:
                return false;
        }
    }

    public float returnFloat(object sentObject)
    {
        object parameters = sentObject;
        if (this.parameters != null)
        {
            parameters = this.parameters;
        }
        switch (this.helperClass)
        {
            case 0:
                return (float)parameters;

            case 1:
                {
                    RCActionHelper helper = (RCActionHelper)parameters;
                    switch (this.helperType)
                    {
                        case 0:
                            return this.nextHelper.returnFloat(RCManager.intVariables[helper.returnString(null)]);

                        case 1:
                            return this.nextHelper.returnFloat(RCManager.boolVariables[helper.returnString(null)]);

                        case 2:
                            return this.nextHelper.returnFloat(RCManager.stringVariables[helper.returnString(null)]);

                        case 3:
                            return (float)RCManager.floatVariables[helper.returnString(null)];

                        case 4:
                            return this.nextHelper.returnFloat(RCManager.playerVariables[helper.returnString(null)]);

                        case 5:
                            return this.nextHelper.returnFloat(RCManager.titanVariables[helper.returnString(null)]);

                        default:
                            return 0f;
                    }
                }
            case 2:
                {
                    PhotonPlayer player = (PhotonPlayer)parameters;
                    if (player != null)
                    {
                        switch (this.helperType)
                        {
                            case 0:
                                return this.nextHelper.returnFloat(player.Properties["team"]);

                            case 1:
                                return this.nextHelper.returnFloat(player.Properties["RCteam"]);

                            case 2:
                                return this.nextHelper.returnFloat(player.Properties["dead"]);

                            case 3:
                                return this.nextHelper.returnFloat(player.Properties["isTitan"]);

                            case 4:
                                return this.nextHelper.returnFloat(player.Properties["kills"]);

                            case 5:
                                return this.nextHelper.returnFloat(player.Properties["deaths"]);

                            case 6:
                                return this.nextHelper.returnFloat(player.Properties["max_dmg"]);

                            case 7:
                                return this.nextHelper.returnFloat(player.Properties["total_dmg"]);

                            case 8:
                                return this.nextHelper.returnFloat(player.Properties["customInt"]);

                            case 9:
                                return this.nextHelper.returnFloat(player.Properties["customBool"]);

                            case 10:
                                return this.nextHelper.returnFloat(player.Properties["customString"]);

                            case 11:
                                return (float)player.Properties["customFloat"];

                            case 12:
                                return this.nextHelper.returnFloat(player.Properties["name"]);

                            case 13:
                                return this.nextHelper.returnFloat(player.Properties["guildName"]);

                            case 14:
                                {
                                    int iD = player.ID;
                                    if (RCManager.heroHash.ContainsKey(iD))
                                    {
                                        return ((HERO)RCManager.heroHash[iD]).transform.position.x;
                                    }
                                    return 0f;
                                }
                            case 15:
                                {
                                    int key = player.ID;
                                    if (RCManager.heroHash.ContainsKey(key))
                                    {
                                        return ((HERO)RCManager.heroHash[key]).transform.position.y;
                                    }
                                    return 0f;
                                }
                            case 16:
                                {
                                    int num7 = player.ID;
                                    if (RCManager.heroHash.ContainsKey(num7))
                                    {
                                        return ((HERO)RCManager.heroHash[num7]).transform.position.z;
                                    }
                                    return 0f;
                                }
                            case 17:
                                {
                                    int num8 = player.ID;
                                    if (RCManager.heroHash.ContainsKey(num8))
                                    {
                                        return ((HERO)RCManager.heroHash[num8]).rigidbody.velocity.magnitude;
                                    }
                                    return 0f;
                                }
                        }
                    }
                    return 0f;
                }
            case 3:
                {
                    TITAN titan = (TITAN)parameters;
                    if (titan != null)
                    {
                        switch (this.helperType)
                        {
                            case 0:
                                return this.nextHelper.returnFloat(titan.abnormalType);

                            case 1:
                                return titan.myLevel;

                            case 2:
                                return this.nextHelper.returnFloat(titan.currentHealth);

                            case 3:
                                return titan.transform.position.x;

                            case 4:
                                return titan.transform.position.y;

                            case 5:
                                return titan.transform.position.z;
                        }
                    }
                    return 0f;
                }
            case 4:
                {
                    RCActionHelper helper2 = (RCActionHelper)parameters;
                    RCRegion region = (RCRegion)RCManager.RCRegions[helper2.returnString(null)];
                    switch (this.helperType)
                    {
                        case 0:
                            return region.GetRandomX();

                        case 1:
                            return region.GetRandomY();

                        case 2:
                            return region.GetRandomZ();

                        default:
                            return 0f;
                    };
                }
            case 5:
                switch (this.helperType)
                {
                    case 0:
                        return Convert.ToSingle((int)parameters);

                    case 1:
                        return Convert.ToSingle((bool)parameters);

                    case 2:
                        {
                            string text = (string)parameters;
                            float num9;
                            if (float.TryParse((string)parameters, out num9))
                            {
                                return num9;
                            }
                            return 0f;
                        }
                    case 3:
                        return (float)parameters;

                    default:
                        return (float)parameters;
                }
            default:
                return 0f;
        }
    }

    public int returnInt(object sentObject)
    {
        object parameters = sentObject;
        if (this.parameters != null)
        {
            parameters = this.parameters;
        }
        switch (this.helperClass)
        {
            case 0:
                return (int)parameters;

            case 1:
                {
                    RCActionHelper helper = (RCActionHelper)parameters;
                    switch (this.helperType)
                    {
                        case 0:
                            return (int)RCManager.intVariables[helper.returnString(null)];

                        case 1:
                            return this.nextHelper.returnInt(RCManager.boolVariables[helper.returnString(null)]);

                        case 2:
                            return this.nextHelper.returnInt(RCManager.stringVariables[helper.returnString(null)]);

                        case 3:
                            return this.nextHelper.returnInt(RCManager.floatVariables[helper.returnString(null)]);

                        case 4:
                            return this.nextHelper.returnInt(RCManager.playerVariables[helper.returnString(null)]);

                        case 5:
                            return this.nextHelper.returnInt(RCManager.titanVariables[helper.returnString(null)]);

                        default:
                            return 0;
                    }
                }
            case 2:
                {
                    PhotonPlayer player = (PhotonPlayer)parameters;
                    if (player != null)
                    {
                        switch (this.helperType)
                        {
                            case 0:
                                return (int)player.Properties["team"];

                            case 1:
                                return (int)player.Properties["RCteam"];

                            case 2:
                                return this.nextHelper.returnInt(player.Properties["dead"]);

                            case 3:
                                return (int)player.Properties["isTitan"];

                            case 4:
                                return (int)player.Properties["kills"];

                            case 5:
                                return (int)player.Properties["deaths"];

                            case 6:
                                return (int)player.Properties["max_dmg"];

                            case 7:
                                return (int)player.Properties["total_dmg"];

                            case 8:
                                return (int)player.Properties["customInt"];

                            case 9:
                                return this.nextHelper.returnInt(player.Properties["customBool"]);

                            case 10:
                                return this.nextHelper.returnInt(player.Properties["customString"]);

                            case 11:
                                return this.nextHelper.returnInt(player.Properties["customFloat"]);

                            case 12:
                                return this.nextHelper.returnInt(player.Properties["name"]);

                            case 13:
                                return this.nextHelper.returnInt(player.Properties["guildName"]);

                            case 14:
                                {
                                    int iD = player.ID;
                                    if (RCManager.heroHash.ContainsKey(iD))
                                    {
                                        HERO hero = (HERO)RCManager.heroHash[iD];
                                        return this.nextHelper.returnInt(hero.transform.position.x);
                                    }
                                    return 0;
                                }
                            case 15:
                                {
                                    int key = player.ID;
                                    if (RCManager.heroHash.ContainsKey(key))
                                    {
                                        HERO hero2 = (HERO)RCManager.heroHash[key];
                                        return this.nextHelper.returnInt(hero2.transform.position.y);
                                    }
                                    return 0;
                                }
                            case 16:
                                {
                                    int num7 = player.ID;
                                    if (RCManager.heroHash.ContainsKey(num7))
                                    {
                                        HERO hero3 = (HERO)RCManager.heroHash[num7];
                                        return this.nextHelper.returnInt(hero3.transform.position.z);
                                    }
                                    return 0;
                                }
                            case 17:
                                {
                                    int num8 = player.ID;
                                    if (RCManager.heroHash.ContainsKey(num8))
                                    {
                                        HERO hero4 = (HERO)RCManager.heroHash[num8];
                                        return this.nextHelper.returnInt(hero4.rigidbody.velocity.magnitude);
                                    }
                                    return 0;
                                }
                        }
                    }
                    return 0;
                }
            case 3:
                {
                    TITAN titan = (TITAN)parameters;
                    if (titan != null)
                    {
                        switch (this.helperType)
                        {
                            case 0:
                                return (int)titan.abnormalType;

                            case 1:
                                return this.nextHelper.returnInt(titan.myLevel);

                            case 2:
                                return titan.currentHealth;

                            case 3:
                                return this.nextHelper.returnInt(titan.transform.position.x);

                            case 4:
                                return this.nextHelper.returnInt(titan.transform.position.y);

                            case 5:
                                return this.nextHelper.returnInt(titan.transform.position.z);
                        }
                    }
                    return 0;
                }
            case 4:
                {
                    RCActionHelper helper2 = (RCActionHelper)parameters;
                    RCRegion region = (RCRegion)RCManager.RCRegions[helper2.returnString(null)];
                    switch (this.helperType)
                    {
                        case 0:
                            return this.nextHelper.returnInt(region.GetRandomX());

                        case 1:
                            return this.nextHelper.returnInt(region.GetRandomY());

                        case 2:
                            return this.nextHelper.returnInt(region.GetRandomZ());

                        default:
                            return 0;
                    }
                }
            case 5:
                switch (this.helperType)
                {
                    case 0:
                        return (int)parameters;

                    case 1:
                        return Convert.ToInt32((bool)parameters);

                    case 2:
                        {
                            string text = (string)parameters;
                            int num9;
                            if (int.TryParse((string)parameters, out num9))
                            {
                                return num9;
                            }
                            return 0;
                        }
                    case 3:
                        return Convert.ToInt32((float)parameters);

                    default:
                        return (int)parameters;
                }
            default:
                return 0;
        }
    }

    public PhotonPlayer returnPlayer(object objParameter)
    {
        object parameters = objParameter;
        if (this.parameters != null)
        {
            parameters = this.parameters;
        }
        int num = this.helperClass;
        if (num == 1)
        {
            RCActionHelper helper = (RCActionHelper)parameters;
            return (PhotonPlayer)RCManager.playerVariables[helper.returnString(null)];
        }
        return (PhotonPlayer)parameters;
    }

    public string returnString(object sentObject)
    {
        object parameters = sentObject;
        if (this.parameters != null)
        {
            parameters = this.parameters;
        }
        switch (this.helperClass)
        {
            case 0:
                return (string)parameters;

            case 1:
                {
                    RCActionHelper helper = (RCActionHelper)parameters;
                    switch (this.helperType)
                    {
                        case 0:
                            return this.nextHelper.returnString(RCManager.intVariables[helper.returnString(null)]);

                        case 1:
                            return this.nextHelper.returnString(RCManager.boolVariables[helper.returnString(null)]);

                        case 2:
                            return (string)RCManager.stringVariables[helper.returnString(null)];

                        case 3:
                            return this.nextHelper.returnString(RCManager.floatVariables[helper.returnString(null)]);

                        case 4:
                            return this.nextHelper.returnString(RCManager.playerVariables[helper.returnString(null)]);

                        case 5:
                            return this.nextHelper.returnString(RCManager.titanVariables[helper.returnString(null)]);

                        default:
                            return string.Empty;
                    }
                }
            case 2:
                {
                    PhotonPlayer player = (PhotonPlayer)parameters;
                    if (player != null)
                    {
                        switch (this.helperType)
                        {
                            case 0:
                                return this.nextHelper.returnString(player.Properties["team"]);

                            case 1:
                                return this.nextHelper.returnString(player.Properties["RCteam"]);

                            case 2:
                                return this.nextHelper.returnString(player.Properties["dead"]);

                            case 3:
                                return this.nextHelper.returnString(player.Properties["isTitan"]);

                            case 4:
                                return this.nextHelper.returnString(player.Properties["kills"]);

                            case 5:
                                return this.nextHelper.returnString(player.Properties["deaths"]);

                            case 6:
                                return this.nextHelper.returnString(player.Properties["max_dmg"]);

                            case 7:
                                return this.nextHelper.returnString(player.Properties["total_dmg"]);

                            case 8:
                                return this.nextHelper.returnString(player.Properties["customInt"]);

                            case 9:
                                return this.nextHelper.returnString(player.Properties["customBool"]);

                            case 10:
                                return (string)player.Properties["customString"];

                            case 11:
                                return this.nextHelper.returnString(player.Properties["customFloat"]);

                            case 12:
                                return (string)player.Properties["name"];

                            case 13:
                                return (string)player.Properties["guildName"];

                            case 14:
                                {
                                    int iD = player.ID;
                                    if (RCManager.heroHash.ContainsKey(iD))
                                    {
                                        HERO hero = (HERO)RCManager.heroHash[iD];
                                        return this.nextHelper.returnString(hero.transform.position.x);
                                    }
                                    return string.Empty;
                                }
                            case 15:
                                {
                                    int key = player.ID;
                                    if (RCManager.heroHash.ContainsKey(key))
                                    {
                                        HERO hero2 = (HERO)RCManager.heroHash[key];
                                        return this.nextHelper.returnString(hero2.transform.position.y);
                                    }
                                    return string.Empty;
                                }
                            case 16:
                                {
                                    int num6 = player.ID;
                                    if (RCManager.heroHash.ContainsKey(num6))
                                    {
                                        HERO hero3 = (HERO)RCManager.heroHash[num6];
                                        return this.nextHelper.returnString(hero3.transform.position.z);
                                    }
                                    return string.Empty;
                                }
                            case 17:
                                {
                                    int num7 = player.ID;
                                    if (RCManager.heroHash.ContainsKey(num7))
                                    {
                                        HERO hero4 = (HERO)RCManager.heroHash[num7];
                                        return this.nextHelper.returnString(hero4.rigidbody.velocity.magnitude);
                                    }
                                    return string.Empty;
                                }
                        }
                    }
                    return string.Empty;
                }
            case 3:
                {
                    TITAN titan = (TITAN)parameters;
                    if (titan != null)
                    {
                        switch (this.helperType)
                        {
                            case 0:
                                return this.nextHelper.returnString(titan.abnormalType);

                            case 1:
                                return this.nextHelper.returnString(titan.myLevel);

                            case 2:
                                return this.nextHelper.returnString(titan.currentHealth);

                            case 3:
                                return this.nextHelper.returnString(titan.transform.position.x);

                            case 4:
                                return this.nextHelper.returnString(titan.transform.position.y);

                            case 5:
                                return this.nextHelper.returnString(titan.transform.position.z);
                        }
                    }
                    return string.Empty;
                }
            case 4:
                {
                    RCActionHelper helper2 = (RCActionHelper)parameters;
                    RCRegion region = (RCRegion)RCManager.RCRegions[helper2.returnString(null)];
                    switch (this.helperType)
                    {
                        case 0:
                            return this.nextHelper.returnString(region.GetRandomX());

                        case 1:
                            return this.nextHelper.returnString(region.GetRandomY());

                        case 2:
                            return this.nextHelper.returnString(region.GetRandomZ());

                        default:
                            return string.Empty;
                    }
                }
            case 5:
                switch (this.helperType)
                {
                    case 0:
                        return ((int)parameters).ToString();

                    case 1:
                        return ((bool)parameters).ToString();

                    case 2:
                        return (string)parameters;

                    case 3:
                        return ((float)parameters).ToString();

                    default:
                        return string.Empty;
                }
            default:
                return string.Empty;
        }
    }

    public TITAN returnTitan(object objParameter)
    {
        object parameters = objParameter;
        if (this.parameters != null)
        {
            parameters = this.parameters;
        }
        int num = this.helperClass;
        if (num == 1)
        {
            RCActionHelper helper = (RCActionHelper)parameters;
            return (TITAN)RCManager.titanVariables[helper.returnString(null)];
        }
        return (TITAN)parameters;
    }

    public void setNextHelper(RCActionHelper sentHelper)
    {
        this.nextHelper = sentHelper;
    }

    public enum helperClasses
    {
        primitive,
        variable,
        player,
        titan,
        region,
        convert
    }

    public enum mathTypes
    {
        add,
        subtract,
        multiply,
        divide,
        modulo,
        power
    }

    public enum other
    {
        regionX,
        regionY,
        regionZ
    }

    public enum playerTypes
    {
        playerType,
        playerTeam,
        playerAlive,
        playerTitan,
        playerKills,
        playerDeaths,
        playerMaxDamage,
        playerTotalDamage,
        playerCustomInt,
        playerCustomBool,
        playerCustomString,
        playerCustomFloat,
        playerName,
        playerGuildName,
        playerPosX,
        playerPosY,
        playerPosZ,
        playerSpeed
    }

    public enum titanTypes
    {
        titanType,
        titanSize,
        titanHealth,
        positionX,
        positionY,
        positionZ
    }

    public enum variableTypes
    {
        typeInt,
        typeBool,
        typeString,
        typeFloat,
        typePlayer,
        typeTitan
    }
}