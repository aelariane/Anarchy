using System;
using ExitGames.Client.Photon;
using RC;

public class RCAction
{
    private int actionClass;

    private int actionType;

    private RCEvent nextEvent;

    private RCActionHelper[] parameters;

    public RCAction(int category, int type, RCEvent next, RCActionHelper[] helpers)
    {
        this.actionClass = category;
        this.actionType = type;
        this.nextEvent = next;
        this.parameters = helpers;
    }

    public void callException(string str)
    {
        Anarchy.UI.Chat.Add(str);
    }

    public void doAction()
    {
        switch (this.actionClass)
        {
            case 0:
                this.nextEvent.checkEvent();
                return;
            case 1:
                {
                    string key = this.parameters[0].returnString(null);
                    int num2 = this.parameters[1].returnInt(null);
                    switch (this.actionType)
                    {
                        case 0:
                            if (!RCManager.intVariables.ContainsKey(key))
                            {
                                RCManager.intVariables.Add(key, num2);
                                return;
                            }
                            RCManager.intVariables[key] = num2;
                            return;
                        case 1:
                            if (!RCManager.intVariables.ContainsKey(key))
                            {
                                this.callException("Variable not found: " + key);
                                return;
                            }
                            RCManager.intVariables[key] = (int)RCManager.intVariables[key] + num2;
                            return;
                        case 2:
                            if (!RCManager.intVariables.ContainsKey(key))
                            {
                                this.callException("Variable not found: " + key);
                                return;
                            }
                            RCManager.intVariables[key] = (int)RCManager.intVariables[key] - num2;
                            return;
                        case 3:
                            if (!RCManager.intVariables.ContainsKey(key))
                            {
                                this.callException("Variable not found: " + key);
                                return;
                            }
                            RCManager.intVariables[key] = (int)RCManager.intVariables[key] * num2;
                            return;
                        case 4:
                            if (!RCManager.intVariables.ContainsKey(key))
                            {
                                this.callException("Variable not found: " + key);
                                return;
                            }
                            RCManager.intVariables[key] = (int)RCManager.intVariables[key] / num2;
                            return;
                        case 5:
                            if (!RCManager.intVariables.ContainsKey(key))
                            {
                                this.callException("Variable not found: " + key);
                                return;
                            }
                            RCManager.intVariables[key] = (int)RCManager.intVariables[key] % num2;
                            return;
                        case 6:
                            if (!RCManager.intVariables.ContainsKey(key))
                            {
                                this.callException("Variable not found: " + key);
                                return;
                            }
                            RCManager.intVariables[key] = (int)Math.Pow((double)((int)RCManager.intVariables[key]), (double)num2);
                            return;
                        case 7:
                        case 8:
                        case 9:
                        case 10:
                        case 11:
                            break;
                        case 12:
                            if (!RCManager.intVariables.ContainsKey(key))
                            {
                                RCManager.intVariables.Add(key, UnityEngine.Random.Range(num2, this.parameters[2].returnInt(null)));
                                return;
                            }
                            RCManager.intVariables[key] = UnityEngine.Random.Range(num2, this.parameters[2].returnInt(null));
                            return;
                        default:
                            return;
                    }
                    break;
                }
            case 2:
                {
                    string str2 = this.parameters[0].returnString(null);
                    bool flag2 = this.parameters[1].returnBool(null);
                    int num3 = this.actionType;
                    if (num3 != 0)
                    {
                        if (num3 != 11)
                        {
                            if (num3 != 12)
                            {
                                return;
                            }
                            if (!RCManager.boolVariables.ContainsKey(str2))
                            {
                                RCManager.boolVariables.Add(str2, Convert.ToBoolean(UnityEngine.Random.Range(0, 2)));
                                return;
                            }
                            RCManager.boolVariables[str2] = Convert.ToBoolean(UnityEngine.Random.Range(0, 2));
                            return;
                        }
                        else
                        {
                            if (!RCManager.boolVariables.ContainsKey(str2))
                            {
                                this.callException("Variable not found: " + str2);
                                return;
                            }
                            RCManager.boolVariables[str2] = !(bool)RCManager.boolVariables[str2];
                            return;
                        }
                    }
                    else
                    {
                        if (!RCManager.boolVariables.ContainsKey(str2))
                        {
                            RCManager.boolVariables.Add(str2, flag2);
                            return;
                        }
                        RCManager.boolVariables[str2] = flag2;
                        return;
                    }
                }
            case 3:
                {
                    string str3 = this.parameters[0].returnString(null);
                    int num4 = this.actionType;
                    if (num4 != 0)
                    {
                        switch (num4)
                        {
                            case 7:
                                {
                                    string str4 = string.Empty;
                                    for (int i = 1; i < this.parameters.Length; i++)
                                    {
                                        str4 += this.parameters[i].returnString(null);
                                    }
                                    if (!RCManager.stringVariables.ContainsKey(str3))
                                    {
                                        RCManager.stringVariables.Add(str3, str4);
                                        return;
                                    }
                                    RCManager.stringVariables[str3] = str4;
                                    return;
                                }
                            case 8:
                                {
                                    string str5 = this.parameters[1].returnString(null);
                                    if (!RCManager.stringVariables.ContainsKey(str3))
                                    {
                                        this.callException("No Variable");
                                        return;
                                    }
                                    RCManager.stringVariables[str3] = (string)RCManager.stringVariables[str3] + str5;
                                    return;
                                }
                            case 9:
                                this.parameters[1].returnString(null);
                                if (!RCManager.stringVariables.ContainsKey(str3))
                                {
                                    this.callException("No Variable");
                                    return;
                                }
                                RCManager.stringVariables[str3] = ((string)RCManager.stringVariables[str3]).Replace(this.parameters[1].returnString(null), this.parameters[2].returnString(null));
                                return;
                            default:
                                return;
                        }
                    }
                    else
                    {
                        string str6 = this.parameters[1].returnString(null);
                        if (!RCManager.stringVariables.ContainsKey(str3))
                        {
                            RCManager.stringVariables.Add(str3, str6);
                            return;
                        }
                        RCManager.stringVariables[str3] = str6;
                        return;
                    }
                }
            case 4:
                {
                    string str7 = this.parameters[0].returnString(null);
                    float num5 = this.parameters[1].returnFloat(null);
                    switch (this.actionType)
                    {
                        case 0:
                            if (!RCManager.floatVariables.ContainsKey(str7))
                            {
                                RCManager.floatVariables.Add(str7, num5);
                                return;
                            }
                            RCManager.floatVariables[str7] = num5;
                            return;
                        case 1:
                            if (!RCManager.floatVariables.ContainsKey(str7))
                            {
                                this.callException("No Variable");
                                return;
                            }
                            RCManager.floatVariables[str7] = (float)RCManager.floatVariables[str7] + num5;
                            return;
                        case 2:
                            if (!RCManager.floatVariables.ContainsKey(str7))
                            {
                                this.callException("No Variable");
                                return;
                            }
                            RCManager.floatVariables[str7] = (float)RCManager.floatVariables[str7] - num5;
                            return;
                        case 3:
                            if (!RCManager.floatVariables.ContainsKey(str7))
                            {
                                this.callException("No Variable");
                                return;
                            }
                            RCManager.floatVariables[str7] = (float)RCManager.floatVariables[str7] * num5;
                            return;
                        case 4:
                            if (!RCManager.floatVariables.ContainsKey(str7))
                            {
                                this.callException("No Variable");
                                return;
                            }
                            RCManager.floatVariables[str7] = (float)RCManager.floatVariables[str7] / num5;
                            return;
                        case 5:
                            if (!RCManager.floatVariables.ContainsKey(str7))
                            {
                                this.callException("No Variable");
                                return;
                            }
                            RCManager.floatVariables[str7] = (float)RCManager.floatVariables[str7] % num5;
                            return;
                        case 6:
                            if (!RCManager.floatVariables.ContainsKey(str7))
                            {
                                this.callException("No Variable");
                                return;
                            }
                            RCManager.floatVariables[str7] = (float)Math.Pow((double)((int)RCManager.floatVariables[str7]), (double)num5);
                            return;
                        case 7:
                        case 8:
                        case 9:
                        case 10:
                        case 11:
                            break;
                        case 12:
                            if (!RCManager.floatVariables.ContainsKey(str7))
                            {
                                RCManager.floatVariables.Add(str7, UnityEngine.Random.Range(num5, this.parameters[2].returnFloat(null)));
                                return;
                            }
                            RCManager.floatVariables[str7] = UnityEngine.Random.Range(num5, this.parameters[2].returnFloat(null));
                            return;
                        default:
                            return;
                    }
                    break;
                }
            case 5:
                {
                    string str8 = this.parameters[0].returnString(null);
                    PhotonPlayer player = this.parameters[1].returnPlayer(null);
                    if (this.actionType == 0)
                    {
                        if (!RCManager.playerVariables.ContainsKey(str8))
                        {
                            RCManager.playerVariables.Add(str8, player);
                            return;
                        }
                        RCManager.playerVariables[str8] = player;
                        return;
                    }
                    break;
                }
            case 6:
                {
                    string str9 = this.parameters[0].returnString(null);
                    TITAN titan = this.parameters[1].returnTitan(null);
                    if (this.actionType == 0)
                    {
                        if (!RCManager.titanVariables.ContainsKey(str9))
                        {
                            RCManager.titanVariables.Add(str9, titan);
                            return;
                        }
                        RCManager.titanVariables[str9] = titan;
                        return;
                    }
                    break;
                }
            case 7:
                {
                    PhotonPlayer targetPlayer = this.parameters[0].returnPlayer(null);
                    switch (this.actionType)
                    {
                        case 0:
                            {
                                int iD = targetPlayer.ID;
                                if (RCManager.heroHash.ContainsKey(iD))
                                {
                                    HERO hero = (HERO)RCManager.heroHash[iD];
                                    hero.MarkDie();
                                    hero.BasePV.RPC("netDie2", PhotonTargets.All, new object[]
                                    {
                        -1,
                        this.parameters[1].returnString(null) + " "
                                    });
                                    return;
                                }
                                this.callException("Player Not Alive");
                                return;
                            }
                        case 1:
                            FengGameManagerMKII.FGM.BasePV.RPC("respawnHeroInNewRound", targetPlayer, new object[0]);
                            return;
                        case 2:
                            FengGameManagerMKII.FGM.BasePV.RPC("spawnPlayerAtRPC", targetPlayer, new object[]
                            {
                    this.parameters[1].returnFloat(null),
                    this.parameters[2].returnFloat(null),
                    this.parameters[3].returnFloat(null)
                            });
                            return;
                        case 3:
                            {
                                int num6 = targetPlayer.ID;
                                if (RCManager.heroHash.ContainsKey(num6))
                                {
                                    ((HERO)RCManager.heroHash[num6]).BasePV.RPC("moveToRPC", targetPlayer, new object[]
                                    {
                        this.parameters[1].returnFloat(null),
                        this.parameters[2].returnFloat(null),
                        this.parameters[3].returnFloat(null)
                                    });
                                    return;
                                }
                                this.callException("Player Not Alive");
                                return;
                            }
                        case 4:
                            targetPlayer.SetCustomProperties(new Hashtable
                {
                    {
                        "kills",
                        this.parameters[1].returnInt(null)
                    }
                });
                            return;
                        case 5:
                            targetPlayer.SetCustomProperties(new Hashtable
                {
                    {
                        "deaths",
                        this.parameters[1].returnInt(null)
                    }
                });
                            return;
                        case 6:
                            targetPlayer.SetCustomProperties(new Hashtable
                {
                    {
                        "max_dmg",
                        this.parameters[1].returnInt(null)
                    }
                });
                            return;
                        case 7:
                            targetPlayer.SetCustomProperties(new Hashtable
                {
                    {
                        "total_dmg",
                        this.parameters[1].returnInt(null)
                    }
                });
                            return;
                        case 8:
                            targetPlayer.SetCustomProperties(new Hashtable
                {
                    {
                        "name",
                        this.parameters[1].returnString(null)
                    }
                });
                            return;
                        case 9:
                            targetPlayer.SetCustomProperties(new Hashtable
                {
                    {
                        "guildName",
                        this.parameters[1].returnString(null)
                    }
                });
                            return;
                        case 10:
                            targetPlayer.SetCustomProperties(new Hashtable
                {
                    {
                        "RCteam",
                        this.parameters[1].returnInt(null)
                    }
                });
                            return;
                        case 11:
                            targetPlayer.SetCustomProperties(new Hashtable
                {
                    {
                        "customInt",
                        this.parameters[1].returnInt(null)
                    }
                });
                            return;
                        case 12:
                            targetPlayer.SetCustomProperties(new Hashtable
                {
                    {
                        "customBool",
                        this.parameters[1].returnBool(null)
                    }
                });
                            return;
                        case 13:
                            targetPlayer.SetCustomProperties(new Hashtable
                {
                    {
                        "customString",
                        this.parameters[1].returnString(null)
                    }
                });
                            return;
                        case 14:
                            targetPlayer.SetCustomProperties(new Hashtable
                {
                    {
                        "RCteam",
                        this.parameters[1].returnFloat(null)
                    }
                });
                            return;
                        default:
                            return;
                    }
                }
            case 8:
                switch (this.actionType)
                {
                    case 0:
                        {
                            TITAN titan2 = this.parameters[0].returnTitan(null);
                            object[] parameters = new object[]
                            {
                    this.parameters[1].returnPlayer(null).ID,
                    this.parameters[2].returnInt(null)
                            };
                            titan2.BasePV.RPC("titanGetHit", titan2.BasePV.owner, parameters);
                            return;
                        }
                    case 1:
                        FengGameManagerMKII.FGM.SpawnTitanAction(this.parameters[0].returnInt(null), this.parameters[1].returnFloat(null), this.parameters[2].returnInt(null), this.parameters[3].returnInt(null));
                        return;
                    case 2:
                        FengGameManagerMKII.FGM.SpawnTitanAtAction(this.parameters[0].returnInt(null), this.parameters[1].returnFloat(null), this.parameters[2].returnInt(null), this.parameters[3].returnInt(null), this.parameters[4].returnFloat(null), this.parameters[5].returnFloat(null), this.parameters[6].returnFloat(null));
                        return;
                    case 3:
                        {
                            TITAN titan3 = this.parameters[0].returnTitan(null);
                            int num7 = this.parameters[1].returnInt(null);
                            titan3.currentHealth = num7;
                            if (titan3.maxHealth == 0)
                            {
                                titan3.maxHealth = titan3.currentHealth;
                            }
                            titan3.BasePV.RPC("labelRPC", PhotonTargets.AllBuffered, new object[]
                            {
                    titan3.currentHealth,
                    titan3.maxHealth
                            });
                            return;
                        }
                    case 4:
                        {
                            TITAN titan4 = this.parameters[0].returnTitan(null);
                            if (titan4.BasePV.IsMine)
                            {
                                titan4.MoveTo(this.parameters[1].returnFloat(null), this.parameters[2].returnFloat(null), this.parameters[3].returnFloat(null));
                                return;
                            }
                            titan4.BasePV.RPC("moveToRPC", titan4.BasePV.owner, new object[]
                            {
                    this.parameters[1].returnFloat(null),
                    this.parameters[2].returnFloat(null),
                    this.parameters[3].returnFloat(null)
                            });
                            return;
                        }
                    default:
                        return;
                }

            case 9:
                switch (this.actionType)
                {
                    case 0:
                        FengGameManagerMKII.FGM.BasePV.RPC("Chat", PhotonTargets.All, new object[]
                        {
                    this.parameters[0].returnString(null),
                    string.Empty
                        });
                        return;
                    case 1:
                        FengGameManagerMKII.FGM.GameWin();
                        if (this.parameters[0].returnBool(null))
                        {
                            RCManager.intVariables.Clear();
                            RCManager.boolVariables.Clear();
                            RCManager.stringVariables.Clear();
                            RCManager.floatVariables.Clear();
                            RCManager.playerVariables.Clear();
                            RCManager.titanVariables.Clear();
                        }
                        return;
                    case 2:
                        FengGameManagerMKII.FGM.GameLose();
                        if (this.parameters[0].returnBool(null))
                        {
                            RCManager.intVariables.Clear();
                            RCManager.boolVariables.Clear();
                            RCManager.stringVariables.Clear();
                            RCManager.floatVariables.Clear();
                            RCManager.playerVariables.Clear();
                            RCManager.titanVariables.Clear();
                        }
                        return;
                    case 3:
                        if (this.parameters[0].returnBool(null))
                        {
                            RCManager.intVariables.Clear();
                            RCManager.boolVariables.Clear();
                            RCManager.stringVariables.Clear();
                            RCManager.floatVariables.Clear();
                            RCManager.playerVariables.Clear();
                            RCManager.titanVariables.Clear();
                        }
                        FengGameManagerMKII.FGM.RestartGame(false, false);
                        return;
                    default:
                        return;
                }
            default:
                return;
        }
    }

    public enum actionClasses
    {
        typeVoid,
        typeVariableInt,
        typeVariableBool,
        typeVariableString,
        typeVariableFloat,
        typeVariablePlayer,
        typeVariableTitan,
        typePlayer,
        typeTitan,
        typeGame
    }

    public enum gameTypes
    {
        printMessage,
        winGame,
        loseGame,
        restartGame
    }

    public enum playerTypes
    {
        killPlayer,
        spawnPlayer,
        spawnPlayerAt,
        movePlayer,
        setKills,
        setDeaths,
        setMaxDmg,
        setTotalDmg,
        setName,
        setGuildName,
        setTeam,
        setCustomInt,
        setCustomBool,
        setCustomString,
        setCustomFloat
    }

    public enum titanTypes
    {
        killTitan,
        spawnTitan,
        spawnTitanAt,
        setHealth,
        moveTitan
    }

    public enum varTypes
    {
        set,
        add,
        subtract,
        multiply,
        divide,
        modulo,
        power,
        concat,
        append,
        remove,
        replace,
        toOpposite,
        setRandom
    }
}
