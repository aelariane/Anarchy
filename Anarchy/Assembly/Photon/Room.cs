using ExitGames.Client.Photon;
using UnityEngine;

public class Room : RoomInfo
{
    internal Room(string roomName, RoomOptions options) : base(roomName, null)
    {
        if (options == null)
        {
            options = new RoomOptions();
        }
        this.visibleField = options.isVisible;
        this.openField = options.isOpen;
        this.maxPlayersField = (byte)options.maxPlayers;
        this.autoCleanUpField = false;
        base.CacheProperties(options.customRoomProperties);
        this.PropertiesListedInLobby = options.customRoomPropertiesForLobby;
    }

    public bool AutoCleanUp
    {
        get
        {
            return this.autoCleanUpField;
        }
    }


    public new int MaxPlayers
    {
        get
        {
            return (int)this.maxPlayersField;
        }
        set
        {
            if (!this.Equals(PhotonNetwork.room))
            {
                Debug.LogWarning("Can't set maxPlayers when not in that room.");
            }
            if (value > 255)
            {
                Debug.LogWarning("Can't set Room.maxPlayers to: " + value + ". Using max value: 255.");
                value = 255;
            }
            if (value != (int)this.maxPlayersField && !PhotonNetwork.offlineMode)
            {
                PhotonNetwork.networkingPeer.OpSetPropertiesOfRoom(new Hashtable() { { RoomProperty.MaxPlayers, (byte)value } }, true, 0);
            }
            this.maxPlayersField = (byte)value;
        }
    }


    public new string Name
    {
        get
        {
            return this.nameField;
        }
        internal set
        {
            this.nameField = value;
        }
    }

    public new bool Open
    {
        get
        {
            return this.openField;
        }
        set
        {
            if (!this.Equals(PhotonNetwork.room))
            {
                Debug.LogWarning("Can't set open when not in that room.");
            }
            if (value != this.openField && !PhotonNetwork.offlineMode)
            {
                PhotonNetwork.networkingPeer.OpSetPropertiesOfRoom(new Hashtable() { { RoomProperty.Joinable, value } }, true, 0);
            }
            this.openField = value;
        }
    }

    public new int PlayerCount
    {
        get
        {
            if (PhotonNetwork.playerList != null)
            {
                return PhotonNetwork.playerList.Length;
            }
            return 0;
        }
    }


    public string[] PropertiesListedInLobby { get; private set; }


    public new bool Visible
    {
        get
        {
            return this.visibleField;
        }
        set
        {
            if (!this.Equals(PhotonNetwork.room))
            {
                Debug.LogWarning("Can't set visible when not in that room.");
            }
            if (value != this.visibleField && !PhotonNetwork.offlineMode)
            {
                PhotonNetwork.networkingPeer.OpSetPropertiesOfRoom(new Hashtable() { { RoomProperty.Visibility, value } }, true, 0);
            }
            this.visibleField = value;
        }
    }

    public RC.Bombs.BombStatsCalculator BombStatsCalculator
    {
        get
        {
            string name = CustomProperties["BombCalculator"] as string;

            if (name == null)
            {
                return new RC.Bombs.DefaultBombStatsCalculator();
            }
            switch (name)
            {
                case "TLW v1":
                    return new TLW.TLWBombCalculatorV1();

                case "RC":
                default:
                    return new RC.Bombs.DefaultBombStatsCalculator();
            }
        }
        set
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                throw new System.Exception("Not masterclient");
            }
            string name = "";
            if(value == null)
            {
                name = "RC";
            }
            var type = value.GetType();
            if(type == typeof(TLW.TLWBombCalculatorV1))
            {
                name = "TLW v1";
            }
            else if(type == typeof(RC.Bombs.BombStatsCalculator))
            {
                name = "RC";
            }
            else
            {
                name = "RC";
            }

            SetCustomProperties(new Hashtable() { { "BombCalculator", name } });
        }
    }

    public void SetCustomProperties(Hashtable propertiesToSet)
    {
        if (propertiesToSet == null)
        {
            return;
        }
        base.CustomProperties.MergeStringKeys(propertiesToSet);
        base.CustomProperties.StripKeysWithNullValues();
        Hashtable gameProperties = propertiesToSet.StripToStringKeys();
        if (!PhotonNetwork.offlineMode)
        {
            PhotonNetwork.networkingPeer.OpSetCustomPropertiesOfRoom(gameProperties, true, 0);
        }
        NetworkingPeer.SendMonoMessage(PhotonNetworkingMessage.OnPhotonCustomRoomPropertiesChanged, new object[] { propertiesToSet });
    }

    public void SetPropertiesListedInLobby(string[] propsListedInLobby)
    {
        Hashtable hashtable = new Hashtable();
        hashtable[RoomProperty.PropertiesListedInLobby] = propsListedInLobby;
        PhotonNetwork.networkingPeer.OpSetPropertiesOfRoom(hashtable, false, 0);
        this.PropertiesListedInLobby = propsListedInLobby;
    }

    public override string ToString()
    {
        return string.Format("Room: '{0}' {1},{2} {4}/{3} players.", new object[]
        {
            this.nameField,
            (!this.visibleField) ? "hidden" : "visible",
            (!this.openField) ? "closed" : "open",
            this.maxPlayersField,
            this.PlayerCount
        });
    }

    public new string ToStringFull()
    {
        return string.Format("Room: '{0}' {1},{2} {4}/{3} players.\ncustomProps: {5}", new object[]
        {
            this.nameField,
            (!this.visibleField) ? "hidden" : "visible",
            (!this.openField) ? "closed" : "open",
            this.maxPlayersField,
            this.PlayerCount,
            base.CustomProperties.ToStringFull()
        });
    }
}
