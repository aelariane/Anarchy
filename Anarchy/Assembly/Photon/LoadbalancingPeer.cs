using ExitGames.Client.Photon;
using System.Collections.Generic;

internal class LoadbalancingPeer : PhotonPeer
{

    public LoadbalancingPeer(IPhotonPeerListener listener, ConnectionProtocol protocolType) : base(listener, protocolType)
    {
    }

    protected bool OpSetPropertiesOfActor(int actorNr, Hashtable actorProperties, bool broadcast, byte channelId)
    {
        if (base.DebugOut >= DebugLevel.INFO)
        {
            base.Listener.DebugReturn(DebugLevel.INFO, "OpSetPropertiesOfActor()");
        }
        if (actorNr <= 0 || actorProperties == null)
        {
            if (base.DebugOut >= DebugLevel.INFO)
            {
                base.Listener.DebugReturn(DebugLevel.INFO, "OpSetPropertiesOfActor not sent. ActorNr must be > 0 and actorProperties != null.");
            }
            return false;
        }
        Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
        dictionary.Add(251, actorProperties);
        dictionary.Add(254, actorNr);
        if (broadcast)
        {
            dictionary.Add(250, broadcast);
        }
        return SendOperation(252, dictionary, broadcast ? SendOptions.SendReliable : SendOptions.SendUnreliable);
    }

    protected void OpSetPropertyOfRoom(byte propCode, object value)
    {
        Hashtable hashtable = new Hashtable();
        hashtable[propCode] = value;
        this.OpSetPropertiesOfRoom(hashtable, true, 0);
    }

    public virtual bool OpAuthenticate(string appId, string appVersion, string userId, AuthenticationValues authValues, string regionCode)
    {
        if (base.DebugOut >= DebugLevel.INFO)
        {
            base.Listener.DebugReturn(DebugLevel.INFO, "OpAuthenticate()");
        }
        Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
        if (authValues != null && authValues.Secret != null)
        {
            dictionary[221] = authValues.Secret;
            return SendOperation(230, dictionary, SendOptions.SendReliable);
        }
        dictionary[220] = appVersion;
        dictionary[224] = appId;
        if (!string.IsNullOrEmpty(regionCode))
        {
            dictionary[210] = regionCode;
        }
        if (!string.IsNullOrEmpty(userId))
        {
            dictionary[225] = userId;
        }
        if (authValues != null && authValues.AuthType != CustomAuthenticationType.None)
        {
            if (!base.IsEncryptionAvailable)
            {
                base.Listener.DebugReturn(DebugLevel.ERROR, "OpAuthenticate() failed. When you want Custom Authentication encryption is mandatory.");
                return false;
            }
            dictionary[217] = (byte)authValues.AuthType;
            if (!string.IsNullOrEmpty(authValues.Secret))
            {
                dictionary[221] = authValues.Secret;
            }
            if (!string.IsNullOrEmpty(authValues.AuthParameters))
            {
                dictionary[216] = authValues.AuthParameters;
            }
            if (authValues.AuthPostData != null)
            {
                dictionary[214] = authValues.AuthPostData;
            }
        }
        bool flag = SendOperation(230, dictionary, new SendOptions() { Channel = 0, DeliveryMode = DeliveryMode.Reliable, Encrypt = IsEncryptionAvailable });
        if (!flag)
        {
            base.Listener.DebugReturn(DebugLevel.ERROR, "Error calling OpAuthenticate! Did not work. Check log output, CustomAuthenticationValues and if you're connected.");
        }
        return flag;
    }

    public virtual bool OpChangeGroups(byte[] groupsToRemove, byte[] groupsToAdd)
    {
        if (base.DebugOut >= DebugLevel.ALL)
        {
            base.Listener.DebugReturn(DebugLevel.ALL, "OpChangeGroups()");
        }
        Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
        if (groupsToRemove != null)
        {
            dictionary[239] = groupsToRemove;
        }
        if (groupsToAdd != null)
        {
            dictionary[238] = groupsToAdd;
        }
        return SendOperation(248, dictionary, SendOptions.SendReliable);
    }

    public virtual bool OpCreateRoom(string roomName, RoomOptions roomOptions, TypedLobby lobby, Hashtable playerProperties, bool onGameServer)
    {
        if (base.DebugOut >= DebugLevel.INFO)
        {
            base.Listener.DebugReturn(DebugLevel.INFO, "OpCreateRoom()");
        }
        Dictionary<byte, object> customOpParameters = new Dictionary<byte, object>();
        if (!string.IsNullOrEmpty(roomName))
        {
            customOpParameters[0xff] = roomName;
            customOpParameters[0xec] = 3000;
        }
        if (lobby != null)
        {
            customOpParameters[0xd5] = lobby.Name;
            customOpParameters[0xd4] = (byte)lobby.Type;
        }
        if (onGameServer)
        {
            if ((playerProperties != null) && (playerProperties.Count > 0))
            {
                customOpParameters[0xf9] = playerProperties;
                customOpParameters[250] = true;
            }
            if (roomOptions == null)
            {
                roomOptions = new RoomOptions();
            }
            Hashtable target = new Hashtable();
            customOpParameters[0xf8] = target;
            target.MergeStringKeys(roomOptions.customRoomProperties);
            target[(byte)0xfd] = roomOptions.isOpen;
            target[(byte)0xfe] = roomOptions.isVisible;
            target[(byte)250] = roomOptions.customRoomPropertiesForLobby;
            if (roomOptions.maxPlayers > 0)
            {
                target[(byte)0xff] = roomOptions.maxPlayers;
            }
            if (roomOptions.cleanupCacheOnLeave)
            {
                customOpParameters[0xf1] = true;
                target[(byte)0xf9] = true;
            }
        }
        return SendOperation(227, customOpParameters, SendOptions.SendReliable);
        //return this.OpCustom(227, dictionary, true);
    }

    public virtual bool OpFindFriends(string[] friendsToFind)
    {
        Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
        if (friendsToFind != null && friendsToFind.Length > 0)
        {
            dictionary[1] = friendsToFind;
        }
        return SendOperation(222, dictionary, SendOptions.SendReliable);
    }

    public virtual bool OpGetRegions(string appId)
    {
        Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
        dictionary[224] = appId;
        return SendOperation(220, dictionary, new SendOptions() { DeliveryMode = DeliveryMode.Reliable, Channel = 0, Encrypt = true });
    }

    public virtual bool OpJoinLobby(TypedLobby lobby)
    {
        if (base.DebugOut >= DebugLevel.INFO)
        {
            base.Listener.DebugReturn(DebugLevel.INFO, "OpJoinLobby()");
        }
        Dictionary<byte, object> dictionary = null;
        if (lobby != null && !lobby.IsDefault)
        {
            dictionary = new Dictionary<byte, object>();
            dictionary[213] = lobby.Name;
            dictionary[212] = (byte)lobby.Type;
        }
        return SendOperation(229, dictionary, SendOptions.SendReliable);
    }

    public virtual bool OpJoinRandomRoom(Hashtable expectedCustomRoomProperties, byte expectedMaxPlayers, Hashtable playerProperties, MatchmakingMode matchingType, TypedLobby typedLobby, string sqlLobbyFilter)
    {
        if (base.DebugOut >= DebugLevel.INFO)
        {
            base.Listener.DebugReturn(DebugLevel.INFO, "OpJoinRandomRoom()");
        }
        Hashtable hashtable = new Hashtable();
        hashtable.MergeStringKeys(expectedCustomRoomProperties);
        if (expectedMaxPlayers > 0)
        {
            hashtable[byte.MaxValue] = expectedMaxPlayers;
        }
        Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
        if (hashtable.Count > 0)
        {
            dictionary[248] = hashtable;
        }
        if (playerProperties != null && playerProperties.Count > 0)
        {
            dictionary[249] = playerProperties;
        }
        if (matchingType != MatchmakingMode.FillRoom)
        {
            dictionary[223] = (byte)matchingType;
        }
        if (typedLobby != null)
        {
            dictionary[213] = typedLobby.Name;
            dictionary[212] = (byte)typedLobby.Type;
        }
        if (!string.IsNullOrEmpty(sqlLobbyFilter))
        {
            dictionary[245] = sqlLobbyFilter;
        }
        return SendOperation(225, dictionary, SendOptions.SendReliable);
    }

    public virtual bool OpJoinRoom(string roomName, RoomOptions roomOptions, TypedLobby lobby, bool createIfNotExists, Hashtable playerProperties, bool onGameServer)
    {
        Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
        if (!string.IsNullOrEmpty(roomName))
        {
            dictionary[byte.MaxValue] = roomName;
        }
        if (createIfNotExists)
        {
            dictionary[215] = true;
            if (lobby != null)
            {
                dictionary[213] = lobby.Name;
                dictionary[212] = (byte)lobby.Type;
            }
        }
        if (onGameServer)
        {
            if (playerProperties != null && playerProperties.Count > 0)
            {
                dictionary[249] = playerProperties;
                dictionary[250] = true;
            }
            if (createIfNotExists)
            {
                if (roomOptions == null)
                {
                    roomOptions = new RoomOptions();
                }
                Hashtable hashtable = new Hashtable();
                dictionary[248] = hashtable;
                hashtable.MergeStringKeys(roomOptions.customRoomProperties);
                hashtable[253] = roomOptions.isOpen;
                hashtable[254] = roomOptions.isVisible;
                hashtable[250] = roomOptions.customRoomPropertiesForLobby;
                if (roomOptions.maxPlayers > 0)
                {
                    hashtable[byte.MaxValue] = roomOptions.maxPlayers;
                }
                if (roomOptions.cleanupCacheOnLeave)
                {
                    dictionary[241] = true;
                    hashtable[249] = true;
                }
            }
        }
        return SendOperation(226, dictionary, SendOptions.SendReliable);
    }

    public virtual bool OpLeaveLobby()
    {
        if (base.DebugOut >= DebugLevel.INFO)
        {
            base.Listener.DebugReturn(DebugLevel.INFO, "OpLeaveLobby()");
        }
        return SendOperation(228, null, SendOptions.SendReliable);
    }

    public virtual bool OpRaiseEvent(byte eventCode, object customEventContent, bool sendReliable, RaiseEventOptions raiseEventOptions)
    {
        Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
        dictionary[244] = eventCode;
        if (customEventContent != null)
        {
            dictionary[245] = customEventContent;
        }
        if (raiseEventOptions == null)
        {
            raiseEventOptions = RaiseEventOptions.Default;
        }
        else
        {
            if (raiseEventOptions.CachingOption != EventCaching.DoNotCache)
            {
                dictionary[247] = (byte)raiseEventOptions.CachingOption;
            }
            if (raiseEventOptions.Receivers != ReceiverGroup.Others)
            {
                dictionary[246] = (byte)raiseEventOptions.Receivers;
            }
            if (raiseEventOptions.InterestGroup != 0)
            {
                dictionary[240] = raiseEventOptions.InterestGroup;
            }
            if (raiseEventOptions.TargetActors != null)
            {
                dictionary[252] = raiseEventOptions.TargetActors;
            }
            if (raiseEventOptions.ForwardToWebhook)
            {
                dictionary[234] = true;
            }
        }
        return SendOperation(253, dictionary, new SendOptions() { DeliveryMode = sendReliable ? DeliveryMode.Reliable : DeliveryMode.Unreliable, Channel = raiseEventOptions.SequenceChannel, Encrypt = false });
    }

    public bool OpSetCustomPropertiesOfActor(int actorNr, Hashtable actorProperties, bool broadcast, byte channelId)
    {
        return this.OpSetPropertiesOfActor(actorNr, actorProperties.StripToStringKeys(), broadcast, channelId);
    }

    public bool OpSetCustomPropertiesOfRoom(Hashtable gameProperties, bool broadcast, byte channelId)
    {
        return this.OpSetPropertiesOfRoom(gameProperties.StripToStringKeys(), broadcast, channelId);
    }

    public bool OpSetPropertiesOfRoom(Hashtable gameProperties, bool broadcast, byte channelId)
    {
        if (base.DebugOut >= DebugLevel.INFO)
        {
            base.Listener.DebugReturn(DebugLevel.INFO, "OpSetPropertiesOfRoom()");
        }
        Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
        dictionary.Add(251, gameProperties);
        if (broadcast)
        {
            dictionary.Add(250, true);
        }
        return SendOperation(252, dictionary, new SendOptions() { Channel = channelId, DeliveryMode = broadcast ? DeliveryMode.Reliable : DeliveryMode.Unreliable, Encrypt = false });
    }
}