using ExitGames.Client.Photon;
using UnityEngine;

public class PhotonPlayer
{
    //
    private string _nameField = string.Empty;
    public readonly bool IsLocal;

    public bool IsMuted { get; set; }

    public float Volume { get; set; }

    public void Mute()
    {
        IsMuted = true;
    }

    public void Unmute()
    {
        IsMuted = false;
    }

    public bool HasVoice { get; set; }

    public bool Anarchy { get; set; }

    public bool RCIgnored { get; set; }

    public PhotonPlayer(bool isLocal, int actorID, string name)
    {
        Properties = new Hashtable();
        this.IsLocal = isLocal;
        ID = actorID;
        _nameField = name;
        if (isLocal)
        {
            Anarchy = true;
            HasVoice = true;
        }
    }

    protected internal PhotonPlayer(bool isLocal, int actorID, Hashtable properties)
    {
        Properties = new Hashtable();
        this.IsLocal = isLocal;
        ID = actorID;
        InternalCacheProperties(properties);
        if (isLocal)
        {
            Anarchy = true;
            HasVoice = true;
        }
    }

    //Methods
    public override bool Equals(object p) => (p as PhotonPlayer)?.ID == ID;

    public static PhotonPlayer Find(int ID)
    {
        NetworkingPeer.mActors.TryGetValue(ID, out PhotonPlayer player);
        return player;
    }

    public static int[] GetAnarchyUserIDs()
    {
        System.Collections.Generic.List<int> result = new System.Collections.Generic.List<int>();
        foreach (PhotonPlayer player in PhotonNetwork.playerList)
        {
            if (player.IsLocal || !player.Anarchy)
            {
                continue;
            }
            result.Add(player.ID);
        }
        return result.ToArray();
    }

    public override int GetHashCode() => ID;

    public PhotonPlayer GetNext() => GetNextFor(ID);

    public PhotonPlayer GetNextFor(PhotonPlayer currentPlayer) => currentPlayer == null ? GetNextFor(currentPlayer.ID) : null;

    public PhotonPlayer GetNextFor(int currentPlayerId)
    {
        if (PhotonNetwork.networkingPeer == null || NetworkingPeer.mActors == null || NetworkingPeer.mActors.Count < 2)
        {
            return null;
        }
        System.Collections.Generic.Dictionary<int, PhotonPlayer> mActors = NetworkingPeer.mActors;
        int num = int.MaxValue;
        int num2 = currentPlayerId;
        foreach (int num3 in mActors.Keys)
        {
            if (num3 < num2)
            {
                num2 = num3;
            }
            else if (num3 > currentPlayerId && num3 < num)
            {
                num = num3;
            }
        }
        return (num == int.MaxValue) ? mActors[num2] : mActors[num];
    }

    internal void InternalChangeLocalID(int newID)
    {
        if (!this.IsLocal)
        {
            Debug.LogError("ERROR You should never change PhotonPlayer IDs!");
            return;
        }
        ID = newID;
    }

    internal void InternalCacheProperties(Hashtable properties)
    {
        if (properties == null || properties.Count == 0 || this.Properties.Equals(properties))
        {
            return;
        }
        if (properties.ContainsKey(byte.MaxValue))
        {
            this._nameField = (string)properties[byte.MaxValue];
        }
        this.Properties.MergeStringKeys(properties);
        this.Properties.StripKeysWithNullValues();
    }

    public bool RemoveProperty(string property)
    {
        if (!Properties.ContainsKey(property))
        {
            return false;
        }
        Properties[property] = null;
        SetCustomProperties(Properties);
        return true;
    }

    public void SetCustomProperties(Hashtable propertiesToSet)
    {
        if (propertiesToSet == null) return;
        this.Properties.MergeStringKeys(propertiesToSet);
        this.Properties.StripKeysWithNullValues();
        Hashtable actorProperties = propertiesToSet.StripToStringKeys();
        if (ID > 0 && !PhotonNetwork.offlineMode)
        {
            PhotonNetwork.networkingPeer.OpSetCustomPropertiesOfActor(ID, actorProperties, true, 0);
        }
        NetworkingPeer.SendMonoMessage(PhotonNetworkingMessage.OnPhotonPlayerPropertiesChanged, new object[] { this, propertiesToSet });
    }

    public override string ToString()
    {
        return string.IsNullOrEmpty(_nameField) ? $"#{this.ID:00}{((!IsMasterClient) ? string.Empty : "(master)")}" : $"'{_nameField}'{((!IsMasterClient) ? string.Empty : "(master)")}";
    }

    public string ToStringFull()
    {
        return $"#{this.ID:00} '{_nameField}' {this.Properties.ToStringFull()}";
    }

    //Properties
    public Hashtable AllProperties
    {
        get
        {
            Hashtable hashtable = new Hashtable();
            hashtable.Merge(Properties);
            hashtable[byte.MaxValue] = _nameField;
            return hashtable;
        }
    }

    public string Character
    {
        get => Properties[PhotonPlayerProperty.character] as string ?? "Levi";
        set
        {
            if (Properties == null) return;
            SetCustomProperties(new Hashtable() { { PhotonPlayerProperty.character, value } });
        }
    }

    public string CurrentLevel
    {
        get => Properties[PhotonPlayerProperty.currentLevel] as string ?? "";
        set
        {
            if (Properties == null) return;
            SetCustomProperties(new Hashtable() { { PhotonPlayerProperty.currentLevel, value } });
        }
    }

    public Hashtable Properties { get; private set; }

    public bool Dead
    {
        get => Properties != null && Properties[PhotonPlayerProperty.dead] is bool dead && dead;
        set
        {
            if (Properties == null) return;
            SetCustomProperties(new Hashtable() { { PhotonPlayerProperty.dead, value } });
        }
    }

    public int Deaths
    {
        get => Properties == null || !(Properties[PhotonPlayerProperty.deaths] is int deaths) ? -1 : deaths;
        set
        {
            if (Properties == null) return;
            SetCustomProperties(new Hashtable() { { PhotonPlayerProperty.deaths, value } });
        }
    }

    public string GuildName
    {
        get => Properties == null ? "Unknown" : Properties[PhotonPlayerProperty.guildName] as string ?? "Unknown";
        set
        {
            if (Properties == null) return;
            SetCustomProperties(new Hashtable() { { PhotonPlayerProperty.guildName, value } });
        }
    }

    public int ID { get; private set; }

    public bool IsMasterClient => NetworkingPeer.mMasterClient.ID == ID;

    public bool IsTitan
    {
        get => Properties != null && Properties[PhotonPlayerProperty.isTitan] is int tit && tit == 2;
        set
        {
            if (Properties == null) return;
            SetCustomProperties(new Hashtable() { { PhotonPlayerProperty.isTitan, value ? 2 : 1 } });
        }
    }

    public int Kills
    {
        get => Properties == null || !(Properties[PhotonPlayerProperty.kills] is int kills) ? -1 : kills;
        set
        {
            if (Properties == null) return;
            SetCustomProperties(new Hashtable() { { PhotonPlayerProperty.kills, value } });
        }
    }

    public int Max_Dmg
    {
        get => Properties == null || !(Properties[PhotonPlayerProperty.max_dmg] is int max) ? -1 : max;
        set
        {
            if (Properties == null) return;
            SetCustomProperties(new Hashtable() { { PhotonPlayerProperty.max_dmg, value } });
        }
    }

    public string Name
    {
        get => _nameField;
        set
        {
            if (!this.IsLocal)
            {
                Debug.LogError("Error: Cannot change the name of a remote player!");
                return;
            }
            this._nameField = value;
        }
    }

    public float RCBombA
    {
        get
        {
            object obj;
            if ((obj = this.Properties["RCBombA"]) is float)
            {
                return (float)obj;
            }
            return 0f;
        }
        set
        {
            this.SetCustomProperties(new Hashtable
            {
                {
                    "RCBombA",
                    value
                }
            });
        }
    }

    public float RCBombB
    {
        get
        {
            object obj;
            if ((obj = this.Properties["RCBombB"]) is float)
            {
                return (float)obj;
            }
            return 0f;
        }
        set
        {
            this.SetCustomProperties(new Hashtable
            {
                {
                    "RCBombB",
                    value
                }
            });
        }
    }

    public float RCBombG
    {
        get
        {
            object obj;
            if ((obj = this.Properties["RCBombG"]) is float)
            {
                return (float)obj;
            }
            return 0f;
        }
        set
        {
            this.SetCustomProperties(new Hashtable
            {
                {
                    "RCBombG",
                    value
                }
            });
        }
    }

    public float RCBombR
    {
        get
        {
            object obj;
            if ((obj = this.Properties["RCBombR"]) is float)
            {
                return (float)obj;
            }
            return 0f;
        }
        set
        {
            this.SetCustomProperties(new Hashtable
            {
                {
                    "RCBombR",
                    value
                }
            });
        }
    }

    public int RCteam
    {
        get => Properties == null || !(Properties["RCteam"] is int team) ? 1 : team; set
        {
            if (Properties == null) return;
            SetCustomProperties(new Hashtable() { { "RCteam", value } });
        }
    }

    public int Team
    {
        get
        {
            if(Properties == null || !(Properties[PhotonPlayerProperty.team] is int team))
            {
                return 1;
            }
            return team;
        }
        set
        {
            if (Properties == null) return;
            SetCustomProperties(new Hashtable() { { PhotonPlayerProperty.team, value } });
        }
    }

    public int Total_Dmg
    {
        get => Properties == null || !(Properties[PhotonPlayerProperty.total_dmg] is int total) ? -1 : total;
        set
        {
            if (Properties == null) return;
            SetCustomProperties(new Hashtable() { { PhotonPlayerProperty.total_dmg, value } });
        }
    }

    public string UIName
    {
        get => Properties == null ? "Unknown" : Properties[PhotonPlayerProperty.name] as string ?? "Unknown";
        set
        {
            if (Properties == null) return;
            SetCustomProperties(new Hashtable() { { PhotonPlayerProperty.name, value } });
        }
    }

}
