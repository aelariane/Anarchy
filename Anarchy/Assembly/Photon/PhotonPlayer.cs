using System.Collections.Generic;
using Antis.Spam;
using ExitGames.Client.Photon;
using UnityEngine;

public class PhotonPlayer
{
    private const float TIMER_DELAY = 0.25f;
    private static List<PhotonPlayer> anarchyUsersList = new List<PhotonPlayer>();
    private static List<PhotonPlayer> rcUsersList = new List<PhotonPlayer>();
    private static List<PhotonPlayer> vanillaUsersList = new List<PhotonPlayer>();
    private static List<PhotonPlayer> notAnarchyUsersList = new List<PhotonPlayer>();

    private static PhotonPlayer[] anarchyUsersArray = new PhotonPlayer[0];
    private static PhotonPlayer[] rcUsersArray = new PhotonPlayer[0];
    private static PhotonPlayer[] vanillaUsersArray = new PhotonPlayer[0];
    private static PhotonPlayer[] notAnarchyUsersArray = new PhotonPlayer[0];

    private readonly Hashtable _localPropsToUpdate;
    private float _updateTimer;
    private bool anarchySync = false;
    public ICounter<byte> EventSpam;
    public readonly int ID;
    public ICounter<string> InstantiateSpam;
    public readonly bool IsLocal;
    private string modName = ModNames.Vanilla;
    private string nameField = string.Empty;
    private readonly RaiseEventOptions option = new RaiseEventOptions();
    private bool rcSync = false;
    public ICounter<string> RPCSpam;
    private readonly int[] targetArray;

    public bool ModLocked { get; set; } = false;

    public bool Muted { get; set; }

    public bool AnarchySync
    {
        get => anarchySync;
        set
        {
            if (!anarchySync && value)
            {
                anarchySync = true;
                rcSync = true;
                if (!anarchyUsersList.Contains(this))
                {
                    if (!IsLocal)
                    {
                        anarchyUsersList.Add(this);
                        anarchyUsersArray = anarchyUsersList.ToArray();
                    }
                }
                if (notAnarchyUsersList.Contains(this))
                {
                    notAnarchyUsersList.Remove(this);
                    notAnarchyUsersArray = notAnarchyUsersList.ToArray();
                }
                if (vanillaUsersList.Contains(this))
                {
                    vanillaUsersList.Remove(this);
                    vanillaUsersArray = vanillaUsersList.ToArray();
                }
                ModName = ModNames.Anarchy;
            }
            else if (anarchySync && !value && !IsLocal)
            {
                anarchySync = false;
                if (anarchyUsersList.Contains(this))
                {
                    anarchyUsersList.Remove(this);
                    anarchyUsersArray = anarchyUsersList.ToArray();
                }
                rcSync = false;
                RCSync = true;
            }
        }
    }

    public GameObject GameObject { get; set; } = null;

    public bool HasVoice { get; set; }

    public bool VCMuted { get; set; }

    public string ModName
    {
        get => modName;
        set
        {
            if (ModLocked || value == null)
            {
                return;
            }
            modName = value;
            if (FengGameManagerMKII.FGM && FengGameManagerMKII.FGM.PlayerList != null)
            {
                FengGameManagerMKII.FGM.PlayerList.Update();
            }
        }
    }


    private bool rcIgnored = false;

    public bool RCIgnored
    { 
        get => rcIgnored;
        set { if (!IsLocal) rcIgnored = value; }
    }

    public bool RCSync
    {
        get => rcSync;
        private set
        {
            if (!rcSync && value)
            {
                rcSync = true;
                if (vanillaUsersList.Contains(this))
                {
                    vanillaUsersList.Remove(this);
                    vanillaUsersArray = vanillaUsersList.ToArray();
                }
                if (!rcUsersList.Contains(this))
                {
                    rcUsersList.Add(this);
                    rcUsersArray = rcUsersList.ToArray();
                }
                if (!notAnarchyUsersList.Contains(this))
                {
                    notAnarchyUsersList.Add(this);
                    notAnarchyUsersArray = notAnarchyUsersList.ToArray();
                }
                ModName = ModNames.RC;
            }
        }
    }

    public float Volume { get; set; }

    static PhotonPlayer()
    {
        NetworkingPeer.RegisterEvent(PhotonNetworkingMessage.OnLeftRoom, OnLeftRoom);
        NetworkingPeer.RegisterEvent(PhotonNetworkingMessage.OnPhotonPlayerDisconnected, OnPhotonPlayerDisconnected);
    }

    private PhotonPlayer(int actorID, bool isLocal)
    {
        Properties = new Hashtable();
        ID = actorID;
        this.IsLocal = isLocal;
        if (isLocal)
        {
            _localPropsToUpdate = new Hashtable();
        }
        if (!isLocal)
        {
            vanillaUsersList.Add(this);
            vanillaUsersArray = vanillaUsersList.ToArray();
            notAnarchyUsersList.Add(this);
            notAnarchyUsersArray = notAnarchyUsersList.ToArray();
        }
        if (isLocal)
        {
            AnarchySync = true;
            HasVoice = true;
                ModName = string.Format(ModNames.AnarchyCustom, (Anarchy.AnarchyManager.CustomName != string.Empty ? Anarchy.AnarchyManager.CustomName : "Custom"));
        }
        targetArray = new int[] { ID };
        option = new RaiseEventOptions() { TargetActors = targetArray };
        InitializeCounters();
    }

    public PhotonPlayer(bool isLocal, int actorID, string name) : this(actorID, isLocal)
    {
        nameField = name;
    }

    protected internal PhotonPlayer(bool isLocal, int actorID, Hashtable properties) : this(actorID, isLocal)
    {
        InternalCacheProperties(properties);
        if (!isLocal)
        {
            RCSync = properties.ContainsKey("RCteam");
        }
    }

    public override bool Equals(object p) => (p as PhotonPlayer)?.ID == ID;

    public static PhotonPlayer Find(int ID)
    {
        NetworkingPeer.mActors.TryGetValue(ID, out PhotonPlayer player);
        return player;
    }

    public static PhotonPlayer[] GetAnarchyUsers()
    {
        return anarchyUsersArray;
    }

    public static PhotonPlayer[] GetNotAnarchyUsers()
    {
        return notAnarchyUsersArray;
    }

    public static PhotonPlayer[] GetRCUsers()
    {
        return rcUsersArray;
    }

    public static PhotonPlayer[] GetVanillaUsers()
    {
        return vanillaUsersArray;
    }

    public static int[] GetAnarchyUsersID()
    {
        int[] result = new int[anarchyUsersArray.Length];
        if (result.Length <= 0)
        {
            return result;
        }
        for (int i = 0; i < result.Length; i++)
        {
            result[i] = anarchyUsersArray[i].ID;
        }
        return result;
    }

    public static int[] GetNotAnarchyUsersID()
    {
        int[] result = new int[notAnarchyUsersArray.Length];
        if (result.Length <= 0)
        {
            return result;
        }
        for (int i = 0; i < result.Length; i++)
        {
            result[i] = notAnarchyUsersArray[i].ID;
        }
        return result;
    }

    public static int[] GetRCUsersID()
    {
        int[] result = new int[rcUsersArray.Length];
        if (result.Length <= 0)
        {
            return result;
        }
        for (int i = 0; i < result.Length; i++)
        {
            result[i] = rcUsersArray[i].ID;
        }
        return result;
    }

    public static int[] GetVanillaUsersID()
    {
        int[] result = new int[vanillaUsersArray.Length];
        if (result.Length <= 0)
        {
            return result;
        }
        for (int i = 0; i < result.Length; i++)
        {
            result[i] = vanillaUsersArray[i].ID;
        }
        return result;
    }

    public override int GetHashCode() => ID;

    public PhotonPlayer GetNext() => GetNextFor(ID);

    public PhotonPlayer GetNextFor(PhotonPlayer currentPlayer) => currentPlayer != null ? GetNextFor(currentPlayer.ID) : null;

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

    private void InitializeCounters()
    {
        EventSpam = new EventsCounter(ID);
        RPCSpam = new RPCCounter(ID);
        InstantiateSpam = new InstantiateCounter(ID);
    }

    internal void InternalChangeLocalID(int newID)
    {
        if (!this.IsLocal)
        {
            Debug.LogError("ERROR You should never change PhotonPlayer IDs!");
            return;
        }
        var info = GetType().GetField(nameof(ID), System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
        if (info != null)
        {
            info.SetValue(this, newID);
            InitializeCounters();
        }
        targetArray[0] = ID;

    }

    internal void InternalCacheProperties(Hashtable properties)
    {
        if (properties == null || properties.Count == 0 || this.Properties.Equals(properties))
        {
            return;
        }
        if (properties.ContainsKey("RCteam") && !RCSync)
        {
            RCSync = true;
        }
        if (properties.ContainsKey(byte.MaxValue))
        {
            this.nameField = (string)properties[byte.MaxValue];
        }
        this.Properties.MergeStringKeys(properties);
        this.Properties.StripKeysWithNullValues();
    }

    public void MuteVC()
    {
        VCMuted = true;
    }

    private static void OnLeftRoom(Optimization.AOTEventArgs args)
    {
        anarchyUsersList.Clear();
        anarchyUsersArray = anarchyUsersList.ToArray();
        rcUsersList.Clear();
        rcUsersArray = rcUsersList.ToArray();
        vanillaUsersList.Clear();
        vanillaUsersArray = vanillaUsersList.ToArray();
        notAnarchyUsersList.Clear();
        notAnarchyUsersArray = notAnarchyUsersList.ToArray();
    }

    private static void OnPhotonPlayerDisconnected(Optimization.AOTEventArgs args)
    {
        PhotonPlayer player = args.Player;
        if(player != null)
        {
            if (anarchyUsersList.Contains(player))
            {
                anarchyUsersList.Remove(player);
                anarchyUsersArray = anarchyUsersList.ToArray();
            }
            if (rcUsersList.Contains(player))
            {
                rcUsersList.Remove(player);
                rcUsersArray = rcUsersList.ToArray();
            }
            if (vanillaUsersList.Contains(player))
            {
                vanillaUsersList.Remove(player);
                vanillaUsersArray = vanillaUsersList.ToArray();
            }
            if (notAnarchyUsersList.Contains(player))
            {
                notAnarchyUsersList.Remove(player);
                notAnarchyUsersArray = notAnarchyUsersList.ToArray();
            }
        }
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
        if (propertiesToSet == null)
        {
            return;
        }
        this.Properties.MergeStringKeys(propertiesToSet);
        this.Properties.StripKeysWithNullValues();
        Hashtable actorProperties = propertiesToSet.StripToStringKeys();
        if (ID > 0 && !PhotonNetwork.offlineMode)
        {
            if (IsLocal)
            {
                _localPropsToUpdate.MergeStringKeys(propertiesToSet);
                return;
            }
            else
            {
                PhotonNetwork.networkingPeer.OpSetCustomPropertiesOfActor(ID, actorProperties, true, 0);
            }
        }
        NetworkingPeer.SendMonoMessage(PhotonNetworkingMessage.OnPhotonPlayerPropertiesChanged, new object[] { this, propertiesToSet });
    }

    public RaiseEventOptions ToOption()
    {
        return option;
    }

    public override string ToString()
    {
        return string.IsNullOrEmpty(nameField) ? $"#{this.ID:00}{((!IsMasterClient) ? string.Empty : "(master)")}" : $"'{nameField ?? "nameField"}'{((!IsMasterClient) ? string.Empty : "(master)")}";
    }

    public string ToStringFull()
    {
        return $"#{this.ID:00} '{nameField}' {this.Properties.ToStringFull()}";
    }

    public int[] ToTargetArray() 
    {
        return targetArray;
    }

    public void UpdateLocalProperties()
    {
        if (!IsLocal)
        {
            return;
        }
        _updateTimer -= Time.unscaledDeltaTime;
        if(_updateTimer < 0f)
        {
            if(_localPropsToUpdate.Count > 0 && PhotonNetwork.inRoom)
            {
                PhotonNetwork.networkingPeer.OpSetCustomPropertiesOfActor(ID, _localPropsToUpdate, true, 0);
                NetworkingPeer.SendMonoMessage(PhotonNetworkingMessage.OnPhotonPlayerPropertiesChanged, new object[] { this, _localPropsToUpdate});
                _localPropsToUpdate.Clear();
            }
            _updateTimer = TIMER_DELAY;
        }
    }

    public void Unmute()
    {
        VCMuted = false;
    }

    public Hashtable AllProperties
    {
        get
        {
            Hashtable hashtable = new Hashtable();
            hashtable.Merge(Properties);
            hashtable[byte.MaxValue] = nameField;
            return hashtable;
        }
    }

    public string Character
    {
        get => Properties[PhotonPlayerProperty.character] as string ?? "Levi";
        set
        {
            SetCustomProperties(new Hashtable() { { PhotonPlayerProperty.character, value } });
        }
    }

    public string CurrentLevel
    {
        get => Properties[PhotonPlayerProperty.currentLevel] as string ?? "";
        set
        {
            SetCustomProperties(new Hashtable() { { PhotonPlayerProperty.currentLevel, value } });
        }
    }

    public Hashtable Properties { get; private set; }

    public bool Dead
    {
        get => Properties != null && Properties[PhotonPlayerProperty.dead] is bool dead && dead;
        set
        {
            SetCustomProperties(new Hashtable() { { PhotonPlayerProperty.dead, value } });
        }
    }

    public int Deaths
    {
        get => Properties == null || !(Properties[PhotonPlayerProperty.deaths] is int deaths) ? -1 : deaths;
        set
        {
            SetCustomProperties(new Hashtable() { { PhotonPlayerProperty.deaths, value } });
        }
    }

    public string FriendName
    {
        get => nameField;
        set
        {
            if (!this.IsLocal)
            {
                Debug.LogError("Error: Cannot change the name of a remote player!");
                return;
            }
            this.nameField = value;
        }
    }

    public string GuildName
    {
        get => Properties == null ? "Unknown" : Properties[PhotonPlayerProperty.guildName] as string ?? "Unknown";
        set
        {
            SetCustomProperties(new Hashtable() { { PhotonPlayerProperty.guildName, value } });
        }
    }

    public bool IsMasterClient => NetworkingPeer.mMasterClient.ID == ID;

    public bool IsTitan
    {
        get => Properties != null && Properties[PhotonPlayerProperty.isTitan] is int tit && tit == 2;
        set
        {
            SetCustomProperties(new Hashtable() { { PhotonPlayerProperty.isTitan, value ? 2 : 1 } });
        }
    }

    public int Kills
    {
        get => Properties == null || !(Properties[PhotonPlayerProperty.kills] is int kills) ? -1 : kills;
        set
        {
            SetCustomProperties(new Hashtable() { { PhotonPlayerProperty.kills, value } });
        }
    }

    public int Max_Dmg
    {
        get => Properties == null || !(Properties[PhotonPlayerProperty.max_dmg] is int max) ? -1 : max;
        set
        {
            SetCustomProperties(new Hashtable() { { PhotonPlayerProperty.max_dmg, value } });
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
        get => Properties == null || !(Properties["RCteam"] is int team) ? 1 : team;
        set
        {
            SetCustomProperties(new Hashtable() { { "RCteam", value } });
        }
    }

    public int Team
    {
        get
        {
            if (Properties == null || !(Properties[PhotonPlayerProperty.team] is int team))
            {
                return 1;
            }
            return team;
        }
        set
        {
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
