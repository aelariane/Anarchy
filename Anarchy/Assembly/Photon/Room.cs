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
        this.propertiesListedInLobby = options.customRoomPropertiesForLobby;
    }

    public bool autoCleanUp
    {
        get
        {
            return this.autoCleanUpField;
        }
    }

    public new int maxPlayers
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
                PhotonNetwork.networkingPeer.OpSetPropertiesOfRoom(new Hashtable
                {
                    {
                        byte.MaxValue,
                        (byte)value
                    }
                }, true, 0);
            }
            this.maxPlayersField = (byte)value;
        }
    }

    public new string name
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

    public new bool open
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
                PhotonNetwork.networkingPeer.OpSetPropertiesOfRoom(new Hashtable
                {
                    {
                        253,
                        value
                    }
                }, true, 0);
            }
            this.openField = value;
        }
    }

    public new int playerCount
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

    public string[] propertiesListedInLobby { get; private set; }

    public new bool visible
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
                PhotonNetwork.networkingPeer.OpSetPropertiesOfRoom(new Hashtable
                {
                    {
                        254,
                        value
                    }
                }, true, 0);
            }
            this.visibleField = value;
        }
    }

    public void SetCustomProperties(Hashtable propertiesToSet)
    {
        if (propertiesToSet == null)
        {
            return;
        }
        base.customProperties.MergeStringKeys(propertiesToSet);
        base.customProperties.StripKeysWithNullValues();
        Hashtable gameProperties = propertiesToSet.StripToStringKeys();
        if (!PhotonNetwork.offlineMode)
        {
            PhotonNetwork.networkingPeer.OpSetCustomPropertiesOfRoom(gameProperties, true, 0);
        }
        NetworkingPeer.SendMonoMessage(PhotonNetworkingMessage.OnPhotonCustomRoomPropertiesChanged, new object[]
        {
            propertiesToSet
        });
    }

    public void SetPropertiesListedInLobby(string[] propsListedInLobby)
    {
        Hashtable hashtable = new Hashtable();
        hashtable[250] = propsListedInLobby;
        PhotonNetwork.networkingPeer.OpSetPropertiesOfRoom(hashtable, false, 0);
        this.propertiesListedInLobby = propsListedInLobby;
    }

    public override string ToString()
    {
        return string.Format("Room: '{0}' {1},{2} {4}/{3} players.", new object[]
        {
            this.nameField,
            (!this.visibleField) ? "hidden" : "visible",
            (!this.openField) ? "closed" : "open",
            this.maxPlayersField,
            this.playerCount
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
            this.playerCount,
            base.customProperties.ToStringFull()
        });
    }
}