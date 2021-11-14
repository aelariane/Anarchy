using ExitGames.Client.Photon;
using System.Collections.Generic;

internal class LoadbalancingPeer : PhotonPeer
{
    public LoadbalancingPeer(IPhotonPeerListener listener, ConnectionProtocol protocolType) : base(listener, protocolType)
    {
    }

    internal bool OpSetPropertiesOfActor(int actorNr, Hashtable actorProperties, bool broadcast, byte channelId)
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
        dictionary.Add(ParameterCode.Properties, actorProperties);
        dictionary.Add(ParameterCode.ActorNr, actorNr);
        if (broadcast)
        {
            dictionary.Add(ParameterCode.Broadcast, broadcast);
        }
        return SendOperation(OperationCode.SetProperties, dictionary, broadcast ? SendOptions.SendReliable : SendOptions.SendUnreliable);
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
            dictionary[ParameterCode.Secret] = authValues.Secret;
            return SendOperation(OperationCode.Authenticate, dictionary, SendOptions.SendReliable);
        }
        dictionary[ParameterCode.AppVersion] = appVersion;
        dictionary[ParameterCode.ApplicationId] = appId;
        if (!string.IsNullOrEmpty(regionCode))
        {
            dictionary[ParameterCode.Region] = regionCode;
        }
        if (!string.IsNullOrEmpty(userId))
        {
            dictionary[ParameterCode.UserId] = userId;
        }
        if (authValues != null && authValues.AuthType != CustomAuthenticationType.None)
        {
            if (!base.IsEncryptionAvailable)
            {
                base.Listener.DebugReturn(DebugLevel.ERROR, "OpAuthenticate() failed. When you want Custom Authentication encryption is mandatory.");
                return false;
            }
            dictionary[ParameterCode.ClientAuthenticationType] = (byte)authValues.AuthType;
            if (!string.IsNullOrEmpty(authValues.Secret))
            {
                dictionary[ParameterCode.Secret] = authValues.Secret;
            }
            if (!string.IsNullOrEmpty(authValues.AuthParameters))
            {
                dictionary[ParameterCode.ClientAuthenticationParams] = authValues.AuthParameters;
            }
            if (authValues.AuthPostData != null)
            {
                dictionary[ParameterCode.ClientAuthenticationData] = authValues.AuthPostData;
            }
        }
        bool flag = SendOperation(OperationCode.Authenticate, dictionary, new SendOptions() { Channel = 0, DeliveryMode = DeliveryMode.Reliable, Encrypt = IsEncryptionAvailable });
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
            dictionary[ParameterCode.Remove] = groupsToRemove;
        }
        if (groupsToAdd != null)
        {
            dictionary[ParameterCode.Add] = groupsToAdd;
        }
        return SendOperation(OperationCode.ChangeGroups, dictionary, SendOptions.SendReliable);
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
            customOpParameters[ParameterCode.RoomName] = roomName;
        }
        if (lobby != null)
        {
            customOpParameters[ParameterCode.LobbyName] = lobby.Name;
            customOpParameters[ParameterCode.LobbyType] = (byte)lobby.Type;
        }
        if (onGameServer)
        {
            if ((playerProperties != null) && (playerProperties.Count > 0))
            {
                customOpParameters[ParameterCode.PlayerProperties] = playerProperties;
                customOpParameters[ParameterCode.Broadcast] = true;
            }
            if (roomOptions == null)
            {
                roomOptions = new RoomOptions();
            }
            Hashtable target = new Hashtable();
            customOpParameters[ParameterCode.GameProperties] = target;
            target.MergeStringKeys(roomOptions.customRoomProperties);
            target[RoomProperty.Joinable] = roomOptions.isOpen;
            target[RoomProperty.Visibility] = roomOptions.isVisible;
            target[RoomProperty.PropertiesListedInLobby] = roomOptions.customRoomPropertiesForLobby;
            if (roomOptions.maxPlayers > 0)
            {
                target[RoomProperty.MaxPlayers] = roomOptions.maxPlayers;
            }
            if (roomOptions.cleanupCacheOnLeave)
            {
                customOpParameters[ParameterCode.CleanupCacheOnLeave] = true;
                target[RoomProperty.CleanUpCacheOnLeave] = true;
            }
        }
        return SendOperation(OperationCode.CreateGame, customOpParameters, SendOptions.SendReliable);
    }

    public virtual bool OpFindFriends(string[] friendsToFind)
    {
        Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
        if (friendsToFind != null && friendsToFind.Length > 0)
        {
            dictionary[ParameterCode.FindFriendsRequestList] = friendsToFind;
        }
        return SendOperation(OperationCode.FindFriends, dictionary, SendOptions.SendReliable);
    }

    public virtual bool OpGetRegions(string appId)
    {
        Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
        dictionary[ParameterCode.ApplicationId] = appId;
        return SendOperation(OperationCode.GetRegions, dictionary, new SendOptions() { DeliveryMode = DeliveryMode.Reliable, Channel = 0, Encrypt = true });
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
            dictionary[ParameterCode.LobbyName] = lobby.Name;
            dictionary[ParameterCode.LobbyType] = (byte)lobby.Type;
        }
        return SendOperation(OperationCode.JoinLobby, dictionary, SendOptions.SendReliable);
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
            dictionary[ParameterCode.GameProperties] = hashtable;
        }
        if (playerProperties != null && playerProperties.Count > 0)
        {
            dictionary[ParameterCode.PlayerProperties] = playerProperties;
        }
        if (matchingType != MatchmakingMode.FillRoom)
        {
            dictionary[ParameterCode.MatchMakingType] = (byte)matchingType;
        }
        if (typedLobby != null)
        {
            dictionary[ParameterCode.LobbyName] = typedLobby.Name;
            dictionary[ParameterCode.LobbyType] = (byte)typedLobby.Type;
        }
        if (!string.IsNullOrEmpty(sqlLobbyFilter))
        {
            dictionary[ParameterCode.Data] = sqlLobbyFilter;
        }
        return SendOperation(OperationCode.JoinRandomGame, dictionary, SendOptions.SendReliable);
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
            dictionary[ParameterCode.JoinMode] = true;
            if (lobby != null)
            {
                dictionary[ParameterCode.LobbyName] = lobby.Name;
                dictionary[ParameterCode.LobbyType] = (byte)lobby.Type;
            }
        }
        if (onGameServer)
        {
            if (playerProperties != null && playerProperties.Count > 0)
            {
                dictionary[ParameterCode.PlayerProperties] = playerProperties;
                dictionary[ParameterCode.Broadcast] = true;
            }
            if (createIfNotExists)
            {
                if (roomOptions == null)
                {
                    roomOptions = new RoomOptions();
                }
                Hashtable hashtable = new Hashtable();
                dictionary[ParameterCode.GameProperties] = hashtable;
                hashtable.MergeStringKeys(roomOptions.customRoomProperties);
                hashtable[RoomProperty.Joinable] = roomOptions.isOpen;
                hashtable[RoomProperty.Visibility] = roomOptions.isVisible;
                hashtable[RoomProperty.PropertiesListedInLobby] = roomOptions.customRoomPropertiesForLobby;
                if (roomOptions.maxPlayers > 0)
                {
                    hashtable[RoomProperty.MaxPlayers] = roomOptions.maxPlayers;
                }
                if (roomOptions.cleanupCacheOnLeave)
                {
                    dictionary[ParameterCode.CleanupCacheOnLeave] = true;
                    hashtable[RoomProperty.CleanUpCacheOnLeave] = true;
                }
            }
        }
        return SendOperation(OperationCode.JoinGame, dictionary, SendOptions.SendReliable);
    }

    public virtual bool OpLeaveLobby()
    {
        if (base.DebugOut >= DebugLevel.INFO)
        {
            base.Listener.DebugReturn(DebugLevel.INFO, "OpLeaveLobby()");
        }
        return SendOperation(OperationCode.LeaveLobby, null, SendOptions.SendReliable);
    }

    public virtual bool OpRaiseEvent(byte eventCode, object customEventContent, bool sendReliable, RaiseEventOptions raiseEventOptions)
    {
        Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
        dictionary[ParameterCode.Code] = eventCode;
        if (raiseEventOptions == null)
        {
            raiseEventOptions = RaiseEventOptions.Default;
        }
        else
        {
            if (raiseEventOptions.CachingOption != EventCaching.DoNotCache)
            {
                dictionary[ParameterCode.Cache] = (byte)raiseEventOptions.CachingOption;
            }
            if (raiseEventOptions.Receivers != ReceiverGroup.Others)
            {
                dictionary[ParameterCode.ReceiverGroup] = (byte)raiseEventOptions.Receivers;
            }
            if (raiseEventOptions.InterestGroup != 0)
            {
                dictionary[ParameterCode.Group] = raiseEventOptions.InterestGroup;
            }
            if (raiseEventOptions.TargetActors != null)
            {
                dictionary[ParameterCode.ActorList] = raiseEventOptions.TargetActors;
            }
            if (raiseEventOptions.ForwardToWebhook)
            {
                dictionary[ParameterCode.EventForward] = true;
            }
        }
        if (customEventContent != null)
        {
            dictionary[ParameterCode.CustomEventContent] = customEventContent;
        }
        return SendOperation(OperationCode.RaiseEvent, dictionary, new SendOptions() { DeliveryMode = sendReliable ? DeliveryMode.Reliable : DeliveryMode.Unreliable, Channel = raiseEventOptions.SequenceChannel, Encrypt = false });
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
        dictionary.Add(ParameterCode.Properties, gameProperties);
        if (broadcast)
        {
            dictionary.Add(ParameterCode.Broadcast, true);
        }
        return SendOperation(OperationCode.SetProperties, dictionary, new SendOptions() { Channel = channelId, DeliveryMode = DeliveryMode.Reliable, Encrypt = false });
    }
}
