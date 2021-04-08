using ExitGames.Client.Photon;
using System;
using System.Collections.Generic;
using UnityEngine;

using Anarchy.Network;
using Anarchy.Configuration;

public static class PhotonNetwork
{
    private static bool _mAutomaticallySyncScene = false;
    private static bool autoJoinLobbyField = true;
    private static bool isOfflineMode = false;
    private static bool m_autoCleanUpPlayerObjects = true;
    private static bool m_isMessageQueueRunning = true;
    private static Room offlineModeRoom = null;
    private static int sendInterval = 50;
    private static int sendIntervalOnSerialize = 100;
    internal static readonly PhotonHandler photonMono;
    internal static int lastUsedViewSubId = 0;
    internal static int lastUsedViewSubIdStatic = 0;
    internal static List<int> manuallyAllocatedViewIds = new List<int>();
    internal static NetworkingPeer networkingPeer;
    public const string serverSettingsAssetFile = "PhotonServerSettings";
    public const string serverSettingsAssetPath = "Assets/Photon Unity Networking/Resources/PhotonServerSettings.asset";
    public const string versionPUN = "1.28";
    public static readonly int MAX_VIEW_IDS = 1000;
    public static bool InstantiateInRoomOnly = true;
    public static PhotonLogLevel logLevel = PhotonLogLevel.ErrorsOnly;
    public static EventCallback OnEventCall;
    public static ServerSettings PhotonServerSettings = (ServerSettings)Resources.Load("PhotonServerSettings", typeof(ServerSettings));
    public static float precisionForFloatSynchronization = 0.01f;
    public static float precisionForQuaternionSynchronization = 1f;
    public static float precisionForVectorSynchronization = 9.9E-05f;
    public static Dictionary<string, GameObject> PrefabCache = new Dictionary<string, GameObject>();
    public static HashSet<GameObject> SendMonoMessageTargets;
    public static bool UseNameServer = true;
    public static bool UsePrefabCache = true;

    static PhotonNetwork()
    {
        Application.runInBackground = true;
        GameObject gameObject = new GameObject();
        photonMono = gameObject.AddComponent<PhotonHandler>();
        gameObject.name = "PhotonMono";
        gameObject.hideFlags = HideFlags.HideInHierarchy;
        networkingPeer = new NetworkingPeer(photonMono, string.Empty, NetworkSettings.ConnectProtocol);
        CustomTypes.Register();
    }

    public delegate void EventCallback(byte eventCode, object content, int senderId);

    public static void SetModProperties()
    {
        int anarchyFlags;
        int abuseFlags = 0;
        player.Properties.TryGetValue(PhotonPlayerProperty.anarchyAbuseFlags, out object val);
        if(val is int)
        {
            abuseFlags = (int) val;
        }
        try
        {
            anarchyFlags = (int)player.Properties[PhotonPlayerProperty.anarchyFlags];
        }
        catch
        {
            anarchyFlags = 0;
        }

        //Check abuse
        if(Anarchy.GameModes.BombMode.Enabled && Settings.InfiniteGasPvp.Value && Anarchy.GameModes.InfiniteGasPvp.Enabled)
        {
            abuseFlags |= (int)AbuseFlags.InfiniteGasInPvp;
        }


        if(Settings.BodyLeanEnabled.Value == false)
        {
            anarchyFlags |= (int)AnarchyFlags.DisableBodyLean;
        }

        if (Anarchy.InputManager.LegacyGasRebind.Value)
        {
            anarchyFlags |= (int)AnarchyFlags.LegacyBurst;
        }

        if (Settings.CameraMode.Value == 3)
        {
            anarchyFlags |= (int)AnarchyFlags.NewTPSCamera;
        }

        player.SetCustomProperties(new Hashtable() { { PhotonPlayerProperty.anarchyFlags, anarchyFlags }, {  PhotonPlayerProperty.anarchyAbuseFlags, abuseFlags} });
    }

    public static AuthenticationValues AuthValues
    {
        get
        {
            return (networkingPeer == null) ? null : networkingPeer.CustomAuthenticationValues;
        }
        set
        {
            if (networkingPeer != null)
            {
                networkingPeer.CustomAuthenticationValues = value;
            }
        }
    }

    public static bool autoCleanUpPlayerObjects
    {
        get
        {
            return m_autoCleanUpPlayerObjects;
        }
        set
        {
            if (room != null)
            {
                Debug.LogError("Setting autoCleanUpPlayerObjects while in a room is not supported.");
            }
            else
            {
                m_autoCleanUpPlayerObjects = value;
            }
        }
    }

    public static bool autoJoinLobby
    {
        get
        {
            return autoJoinLobbyField;
        }
        set
        {
            autoJoinLobbyField = value;
        }
    }

    public static bool automaticallySyncScene
    {
        get
        {
            return _mAutomaticallySyncScene;
        }
        set
        {
            _mAutomaticallySyncScene = value;
            if (_mAutomaticallySyncScene && room != null)
            {
                networkingPeer.LoadLevelIfSynced();
            }
        }
    }

    public static bool connected
    {
        get
        {
            return offlineMode || (networkingPeer != null && (!networkingPeer.IsInitialConnect && networkingPeer.State != PeerState.PeerCreated && networkingPeer.State != PeerState.Disconnected && networkingPeer.State != PeerState.Disconnecting) && networkingPeer.State != PeerState.ConnectingToNameServer);
        }
    }

    public static bool connectedAndReady
    {
        get
        {
            if (!connected)
            {
                return false;
            }
            if (offlineMode)
            {
                return true;
            }
            PeerState connectionStateDetailed = PhotonNetwork.connectionStateDetailed;
            switch (connectionStateDetailed)
            {
                case PeerState.ConnectingToGameserver:
                case PeerState.Joining:
                case PeerState.Leaving:
                case PeerState.ConnectingToMasterserver:
                case PeerState.Disconnecting:
                case PeerState.Disconnected:
                case PeerState.ConnectingToNameServer:
                case PeerState.Authenticating:
                    break;

                default:
                    if (connectionStateDetailed != PeerState.PeerCreated)
                    {
                        return true;
                    }
                    break;
            }
            return false;
        }
    }

    public static bool connecting
    {
        get
        {
            return networkingPeer.IsInitialConnect && !offlineMode;
        }
    }

    public static ConnectionState connectionState
    {
        get
        {
            if (offlineMode)
            {
                return ConnectionState.Connected;
            }
            if (networkingPeer == null)
            {
                return ConnectionState.Disconnected;
            }
            PeerStateValue peerState = networkingPeer.PeerState;
            switch (peerState)
            {
                case PeerStateValue.Disconnected:
                    return ConnectionState.Disconnected;

                case PeerStateValue.Connecting:
                    return ConnectionState.Connecting;

                default:
                    if (peerState != PeerStateValue.InitializingApplication)
                    {
                        return ConnectionState.Disconnected;
                    }
                    return ConnectionState.InitializingApplication;

                case PeerStateValue.Connected:
                    return ConnectionState.Connected;

                case PeerStateValue.Disconnecting:
                    return ConnectionState.Disconnecting;
            }
        }
    }

    public static PeerState connectionStateDetailed
    {
        get
        {
            if (offlineMode)
            {
                return (offlineModeRoom == null) ? PeerState.ConnectedToMaster : PeerState.Joined;
            }
            if (networkingPeer == null)
            {
                return PeerState.Disconnected;
            }
            return networkingPeer.State;
        }
    }

    public static int countOfPlayers
    {
        get
        {
            return networkingPeer.mPlayersInRoomsCount + networkingPeer.mPlayersOnMasterCount;
        }
    }

    public static int countOfPlayersInRooms
    {
        get
        {
            return networkingPeer.mPlayersInRoomsCount;
        }
    }

    public static int countOfPlayersOnMaster
    {
        get
        {
            return networkingPeer.mPlayersOnMasterCount;
        }
    }

    public static int countOfRooms
    {
        get
        {
            return networkingPeer.mGameCount;
        }
    }

    public static bool CrcCheckEnabled
    {
        get
        {
            return networkingPeer.CrcEnabled;
        }
        set
        {
            if (!connected && !connecting)
            {
                networkingPeer.CrcEnabled = value;
            }
            else
            {
                Debug.Log("Can't change CrcCheckEnabled while being connected. CrcCheckEnabled stays " + networkingPeer.CrcEnabled);
            }
        }
    }

    public static List<FriendInfo> Friends { get; internal set; }

    public static int FriendsListAge
    {
        get
        {
            return (networkingPeer == null) ? 0 : networkingPeer.FriendsListAge;
        }
    }

    public static string gameVersion
    {
        get
        {
            return networkingPeer.mAppVersion;
        }
        set
        {
            networkingPeer.mAppVersion = value;
        }
    }

    public static bool inRoom
    {
        get
        {
            return connectionStateDetailed == PeerState.Joined;
        }
    }

    public static bool InsideLobby
    {
        get
        {
            return networkingPeer.insideLobby;
        }
    }

    public static bool IsMasterClient
    {
        get
        {
            return offlineMode || NetworkingPeer.mMasterClient == networkingPeer.mLocalActor;
        }
    }

    public static bool isMessageQueueRunning
    {
        get
        {
            return m_isMessageQueueRunning;
        }
        set
        {
            if (value)
            {
                PhotonHandler.StartFallbackSendAckThread();
            }
            networkingPeer.IsSendingOnlyAcks = !value;
            m_isMessageQueueRunning = value;
        }
    }

    public static bool isNonMasterClientInRoom
    {
        get
        {
            return !IsMasterClient && room != null;
        }
    }

    public static TypedLobby lobby
    {
        get
        {
            return networkingPeer.lobby;
        }
        set
        {
            networkingPeer.lobby = value;
        }
    }

    public static PhotonPlayer masterClient
    {
        get
        {
            if (networkingPeer == null)
            {
                return null;
            }
            return NetworkingPeer.mMasterClient;
        }
    }

    [Obsolete("Used for compatibility with Unity networking only.")]
    public static int maxConnections
    {
        get
        {
            if (room == null)
            {
                return 0;
            }
            return room.MaxPlayers;
        }
        set
        {
            room.MaxPlayers = value;
        }
    }

    public static int MaxResendsBeforeDisconnect
    {
        get
        {
            return networkingPeer.SentCountAllowance;
        }
        set
        {
            if (value < 3)
            {
                value = 3;
            }
            if (value > 10)
            {
                value = 10;
            }
            networkingPeer.SentCountAllowance = value;
        }
    }

    public static bool NetworkStatisticsEnabled
    {
        get
        {
            return networkingPeer.TrafficStatsEnabled;
        }
        set
        {
            networkingPeer.TrafficStatsEnabled = value;
        }
    }

    public static bool offlineMode
    {
        get
        {
            return isOfflineMode;
        }
        set
        {
            if (value == isOfflineMode)
            {
                return;
            }
            if (value && connected)
            {
                Debug.LogError("Can't start OFFLINE mode while connected!");
            }
            else
            {
                if (networkingPeer.PeerState != PeerStateValue.Disconnected)
                {
                    networkingPeer.Disconnect();
                }
                isOfflineMode = value;
                if (isOfflineMode)
                {
                    NetworkingPeer.SendMonoMessage(PhotonNetworkingMessage.OnConnectedToMaster, new object[0]);
                    networkingPeer.ChangeLocalID(1);
                    NetworkingPeer.mMasterClient = player;
                }
                else
                {
                    offlineModeRoom = null;
                    networkingPeer.ChangeLocalID(-1);
                    NetworkingPeer.mMasterClient = null;
                }
            }
        }
    }

    public static PhotonPlayer[] otherPlayers
    {
        get
        {
            if (networkingPeer == null)
            {
                return new PhotonPlayer[0];
            }
            return networkingPeer.mOtherPlayerListCopy;
        }
    }

    public static int PacketLossByCrcCheck
    {
        get
        {
            return networkingPeer.PacketLossByCrc;
        }
    }

    public static PhotonPlayer player
    {
        get
        {
            if (networkingPeer == null)
            {
                return null;
            }
            return networkingPeer.mLocalActor;
        }
    }

    public static PhotonPlayer[] playerList
    {
        get
        {
            if (networkingPeer == null)
            {
                return new PhotonPlayer[0];
            }
            return networkingPeer.mPlayerListCopy;
        }
    }

    public static string playerName
    {
        get
        {
            return networkingPeer.PlayerName;
        }
        set
        {
            networkingPeer.PlayerName = value;
        }
    }

    public static int ResentReliableCommands
    {
        get
        {
            return networkingPeer.ResentReliableCommands;
        }
    }

    public static Room room
    {
        get
        {
            if (isOfflineMode)
            {
                return offlineModeRoom;
            }
            return networkingPeer.mCurrentGame;
        }
    }

    public static int sendRate
    {
        get
        {
            return 1000 / sendInterval;
        }
        set
        {
            sendInterval = 1000 / value;
            if (photonMono != null)
            {
                photonMono.updateInterval = sendInterval;
            }
            if (value < sendRateOnSerialize)
            {
                sendRateOnSerialize = value;
            }
        }
    }

    public static int sendRateOnSerialize
    {
        get
        {
            return 1000 / sendIntervalOnSerialize;
        }
        set
        {
            if (value > sendRate)
            {
                Debug.LogError("Error, can not set the OnSerialize SendRate more often then the overall SendRate");
                value = sendRate;
            }
            sendIntervalOnSerialize = 1000 / value;
            if (photonMono != null)
            {
                photonMono.updateIntervalOnSerialize = sendIntervalOnSerialize;
            }
        }
    }

    public static ServerConnection Server
    {
        get
        {
            return networkingPeer.server;
        }
    }

    public static string ServerAddress
    {
        get
        {
            return (networkingPeer == null) ? "<not connected>" : networkingPeer.ServerAddress;
        }
    }

    public static double time
    {
        get
        {
            if (offlineMode)
            {
                return (double)Time.time;
            }
            return networkingPeer.ServerTimeInMilliSeconds / 1000.0;
        }
    }

    public static int[] AllocateSceneViewIDs(int countOfNewViews)
    {
        int[] array = new int[countOfNewViews];
        for (int i = 0; i < countOfNewViews; i++)
        {
            array[i] = AllocateViewID(0);
        }
        return array;
    }

    public static int AllocateViewID(int ownerId)
    {
        if (ownerId == 0)
        {
            int num = lastUsedViewSubIdStatic;
            int num2 = ownerId * MAX_VIEW_IDS;
            for (int i = 1; i < MAX_VIEW_IDS; i++)
            {
                num = (num + 1) % MAX_VIEW_IDS;
                if (num != 0)
                {
                    int num3 = num + num2;
                    if (!NetworkingPeer.photonViewList.ContainsKey(num3))
                    {
                        lastUsedViewSubIdStatic = num;
                        return num3;
                    }
                }
            }
            throw new Exception(string.Format("AllocateViewID() failed. Room (user {0}) is out of subIds, as all room viewIDs are used.", ownerId));
        }
        int num4 = lastUsedViewSubId;
        int num5 = ownerId * MAX_VIEW_IDS;
        for (int j = 1; j < MAX_VIEW_IDS; j++)
        {
            num4 = (num4 + 1) % MAX_VIEW_IDS;
            if (num4 != 0)
            {
                int num6 = num4 + num5;
                if (!NetworkingPeer.photonViewList.ContainsKey(num6) && !manuallyAllocatedViewIds.Contains(num6))
                {
                    lastUsedViewSubId = num4;
                    return num6;
                }
            }
        }
        throw new Exception(string.Format("AllocateViewID() failed. User {0} is out of subIds, as all viewIDs are used.", ownerId));
    }

    private static bool VerifyCanUseNetwork()
    {
        if (connected)
        {
            return true;
        }
        Debug.LogError("Cannot send messages when not connected. Either connect to Photon OR use offline mode!");
        return false;
    }

    internal static void RPC(PhotonView view, string methodName, PhotonTargets target, params object[] parameters)
    {
        if (!VerifyCanUseNetwork())
        {
            return;
        }
        if (room == null)
        {
            Debug.LogWarning("Cannot send RPCs in Lobby! RPC dropped.");
            return;
        }
        if (networkingPeer != null)
        {
            networkingPeer.RPC(view, methodName, target, parameters);
        }
        else
        {
            Debug.LogWarning("Could not execute RPC " + methodName + ". Possible scene loading in progress?");
        }
    }

    internal static void RPC(PhotonView view, string methodName, PhotonPlayer targetPlayer, params object[] parameters)
    {
        if (!VerifyCanUseNetwork())
        {
            return;
        }
        if (room == null)
        {
            Debug.LogWarning("Cannot send RPCs in Lobby, only processed locally");
            return;
        }
        if (player == null)
        {
            Debug.LogError("Error; Sending RPC to player null! Aborted \"" + methodName + "\"");
        }
        if (networkingPeer != null)
        {
            networkingPeer.RPC(view, methodName, targetPlayer, parameters);
        }
        else
        {
            Debug.LogWarning("Could not execute RPC " + methodName + ". Possible scene loading in progress?");
        }
    }

    internal static void RPC(PhotonView view, string methodName, PhotonPlayer[] targets, params object[] parameters)
    {
        if (!VerifyCanUseNetwork())
        {
            return;
        }
        if (room == null)
        {
            Debug.LogWarning("Cannot send RPCs in Lobby! RPC dropped.");
            return;
        }
        if (networkingPeer != null)
        {
            networkingPeer.RPC(view, methodName, targets, parameters);
        }
        else
        {
            Debug.LogWarning("Could not execute RPC " + methodName + ". Possible scene loading in progress?");
        }
    }

    public static int AllocateViewID()
    {
        int num = AllocateViewID(player.ID);
        manuallyAllocatedViewIds.Add(num);
        return num;
    }

    public static bool CloseConnection(PhotonPlayer kickPlayer)
    {
        if (!VerifyCanUseNetwork())
        {
            return false;
        }
        if (!player.IsMasterClient)
        {
            Debug.LogError("CloseConnection: Only the masterclient can kick another player.");
            return false;
        }
        if (kickPlayer == null)
        {
            Debug.LogError("CloseConnection: No such player connected!");
            return false;
        }
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions
        {
            TargetActors = new int[]
            {
                kickPlayer.ID
            }
        };
        return networkingPeer.OpRaiseEvent(203, null, true, raiseEventOptions);
    }

    public static bool ConnectToBestCloudServer(string gameVersion)
    {
        if (PhotonServerSettings == null)
        {
            Debug.LogError("Can't connect: Loading settings failed. ServerSettings asset must be in any 'Resources' folder as: PhotonServerSettings");
            return false;
        }
        if (PhotonServerSettings.HostType == ServerSettings.HostingOption.OfflineMode)
        {
            return ConnectUsingSettings(gameVersion);
        }
        networkingPeer.IsInitialConnect = true;
        networkingPeer.SetApp(PhotonServerSettings.AppID, gameVersion);
        CloudRegionCode bestRegionCodeInPreferences = PhotonHandler.BestRegionCodeInPreferences;
        if (bestRegionCodeInPreferences != CloudRegionCode.none)
        {
            Debug.Log("Best region found in PlayerPrefs. Connecting to: " + bestRegionCodeInPreferences);
            return networkingPeer.ConnectToRegionMaster(bestRegionCodeInPreferences);
        }
        return networkingPeer.ConnectToNameServer();
    }

    public static bool ConnectToMaster(string masterServerAddress, int port, string appID, string gameVersion)
    {
        string url = string.Empty;
        if (networkingPeer.TransportProtocol >= ConnectionProtocol.WebSocket)
        {
            url = "ws";
            if (networkingPeer.UsedProtocol == ConnectionProtocol.WebSocketSecure)
            {
                url += "s";
            }

            url += "://";
        }
        masterServerAddress = url + masterServerAddress;
        if (networkingPeer.PeerState != PeerStateValue.Disconnected)
        {
            Debug.LogWarning("ConnectToMaster() failed. Can only connect while in state 'Disconnected'. Current state: " + networkingPeer.PeerState);
            return false;
        }
        if (offlineMode)
        {
            offlineMode = false;
            Debug.LogWarning("ConnectToMaster() disabled the offline mode. No longer offline.");
        }
        if (!isMessageQueueRunning)
        {
            isMessageQueueRunning = true;
            Debug.LogWarning("ConnectToMaster() enabled isMessageQueueRunning. Needs to be able to dispatch incoming messages.");
        }
        networkingPeer.SetApp(appID, gameVersion);
        networkingPeer.IsUsingNameServer = false;
        networkingPeer.IsInitialConnect = true;
        networkingPeer.MasterServerAddress = masterServerAddress + ":" + port;
        return networkingPeer.Connect(networkingPeer.MasterServerAddress, ServerConnection.MasterServer);
    }

    public static bool ConnectUsingSettings(string gameVersion)
    {
        if (PhotonServerSettings == null)
        {
            Debug.LogError("Can't connect: Loading settings failed. ServerSettings asset must be in any 'Resources' folder as: PhotonServerSettings");
            return false;
        }
        SwitchToProtocol(PhotonServerSettings.Protocol);
        networkingPeer.SetApp(PhotonServerSettings.AppID, gameVersion);
        if (PhotonServerSettings.HostType == ServerSettings.HostingOption.OfflineMode)
        {
            offlineMode = true;
            return true;
        }
        if (offlineMode)
        {
            Debug.LogWarning("ConnectUsingSettings() disabled the offline mode. No longer offline.");
        }
        offlineMode = false;
        isMessageQueueRunning = true;
        networkingPeer.IsInitialConnect = true;
        if (PhotonServerSettings.HostType == ServerSettings.HostingOption.SelfHosted)
        {
            networkingPeer.IsUsingNameServer = false;
            networkingPeer.MasterServerAddress = PhotonServerSettings.ServerAddress + ":" + PhotonServerSettings.ServerPort;
            return networkingPeer.Connect(networkingPeer.MasterServerAddress, ServerConnection.MasterServer);
        }
        if (PhotonServerSettings.HostType == ServerSettings.HostingOption.BestRegion)
        {
            return ConnectToBestCloudServer(gameVersion);
        }
        return networkingPeer.ConnectToRegionMaster(PhotonServerSettings.PreferredRegion);
    }

    [Obsolete("Use overload with RoomOptions and TypedLobby parameters.")]
    public static bool CreateRoom(string roomName, bool isVisible, bool isOpen, int maxPlayers)
    {
        return CreateRoom(roomName, new RoomOptions
        {
            isVisible = isVisible,
            isOpen = isOpen,
            maxPlayers = maxPlayers
        }, null);
    }

    [Obsolete("Use overload with RoomOptions and TypedLobby parameters.")]
    public static bool CreateRoom(string roomName, bool isVisible, bool isOpen, int maxPlayers, Hashtable customRoomProperties, string[] propsToListInLobby)
    {
        return CreateRoom(roomName, new RoomOptions
        {
            isVisible = isVisible,
            isOpen = isOpen,
            maxPlayers = maxPlayers,
            customRoomProperties = customRoomProperties,
            customRoomPropertiesForLobby = propsToListInLobby
        }, null);
    }

    public static bool CreateRoom(string roomName)
    {
        return CreateRoom(roomName, null, null);
    }

    public static bool CreateRoom(string roomName, RoomOptions roomOptions, TypedLobby typedLobby)
    {
        if (offlineMode)
        {
            if (offlineModeRoom != null)
            {
                Debug.LogError("CreateRoom failed. In offline mode you still have to leave a room to enter another.");
                return false;
            }
            offlineModeRoom = new Room(roomName, roomOptions);
            NetworkingPeer.SendMonoMessage(PhotonNetworkingMessage.OnJoinedRoom, new object[0]);
            NetworkingPeer.SendMonoMessage(PhotonNetworkingMessage.OnCreatedRoom, new object[0]);
            return true;
        }
        else
        {
            if (networkingPeer.server != ServerConnection.MasterServer || !connectedAndReady)
            {
                Debug.LogError("CreateRoom failed. Client is not on Master Server or not yet ready to call operations. Wait for callback: OnJoinedLobby or OnConnectedToMaster.");
                return false;
            }
            return networkingPeer.OpCreateGame(roomName, roomOptions, typedLobby);
        }
    }

    public static void Destroy(PhotonView targetView)
    {
        if (targetView != null)
        {
            networkingPeer.RemoveInstantiatedGO(targetView.gameObject, !inRoom);
        }
        else
        {
            Debug.LogError("Destroy(targetPhotonView) failed, cause targetPhotonView is null.");
        }
    }

    public static void Destroy(GameObject targetGo)
    {
        networkingPeer.RemoveInstantiatedGO(targetGo, !inRoom);
    }

    public static void DestroyAll()
    {
        if (IsMasterClient)
        {
            networkingPeer.DestroyAll(false);
        }
        else
        {
            Debug.LogError("Couldn't call DestroyAll() as only the master client is allowed to call this.");
        }
    }

    public static void DestroyPlayerObjects(PhotonPlayer targetPlayer)
    {
        if (targetPlayer == null)
        {
            Debug.LogError("DestroyPlayerObjects() failed, cause parameter 'targetPlayer' was null.");
        }
        DestroyPlayerObjects(targetPlayer.ID);
    }

    public static void DestroyPlayerObjects(int targetPlayerId)
    {
        if (!VerifyCanUseNetwork())
        {
            return;
        }
        if (player.IsMasterClient || targetPlayerId == player.ID)
        {
            networkingPeer.DestroyPlayerObjects(targetPlayerId, false);
        }
        else
        {
            Debug.LogError("DestroyPlayerObjects() failed, cause players can only destroy their own GameObjects. A Master Client can destroy anyone's. This is master: " + IsMasterClient);
        }
    }

    public static void Disconnect()
    {
        if (offlineMode)
        {
            offlineMode = false;
            offlineModeRoom = null;
            networkingPeer.State = PeerState.Disconnecting;
            networkingPeer.OnStatusChanged(StatusCode.Disconnect);
            return;
        }
        if (networkingPeer == null)
        {
            return;
        }
        networkingPeer.Disconnect();
    }

    public static void FetchServerTimestamp()
    {
        if (networkingPeer != null)
        {
            networkingPeer.FetchServerTimestamp();
        }
    }

    public static bool FindFriends(string[] friendsToFind)
    {
        return networkingPeer != null && !isOfflineMode && networkingPeer.OpFindFriends(friendsToFind);
    }

    public static int GetPing()
    {
        return networkingPeer.RoundTripTime;
    }

    public static RoomInfo[] GetRoomList()
    {
        if (offlineMode || networkingPeer == null)
        {
            return new RoomInfo[0];
        }
        return networkingPeer.mGameListCopy;
    }

    [Obsolete("Used for compatibility with Unity networking only. Encryption is automatically initialized while connecting.")]
    public static void InitializeSecurity()
    {
    }

    public static GameObject Instantiate(string prefabName, Vector3 position, Quaternion rotation, int group)
    {
        return Instantiate(prefabName, position, rotation, group, null);
    }

    public static GameObject Instantiate(string prefabName, Vector3 position, Quaternion rotation, int group, object[] data)
    {
        if (!connected || (InstantiateInRoomOnly && !inRoom))
        {
            Debug.LogError(string.Concat(new object[]
            {
                "Failed to Instantiate prefab: ",
                prefabName,
                ". Client should be in a room. Current connectionStateDetailed: ",
                connectionStateDetailed
            }));
            return null;
        }
        GameObject gameObject = prefabName.StartsWith("RCAsset/") ? Optimization.Caching.CacheResources.RCLoad(prefabName) : (GameObject)Optimization.Caching.CacheResources.Load(prefabName);
        //      if (!UsePrefabCache || !PrefabCache.TryGetValue(prefabName, out gameObject))
        //{
        //	gameObject =
        //	if (UsePrefabCache)
        //	{
        //		PrefabCache.Add(prefabName, gameObject);
        //	}
        //}
        if (gameObject == null)
        {
            Debug.LogError("Failed to Instantiate prefab: " + prefabName + ". Verify the Prefab is in a Resources folder (and not in a subfolder)");
            return null;
        }
        if (gameObject.GetComponent<PhotonView>() == null)
        {
            Debug.LogError("Failed to Instantiate prefab:" + prefabName + ". Prefab must have a PhotonView component.");
            return null;
        }
        Component[] photonViewsInChildren = gameObject.GetPhotonViewsInChildren();
        int[] array = new int[photonViewsInChildren.Length];
        for (int i = 0; i < array.Length; i++)
        {
            array[i] = AllocateViewID(player.ID);
        }
        networkingPeer.SendInstantiate(prefabName, position, rotation, group, array, data, false);
        return Optimization.Caching.Pool.NetworkInstantiate(prefabName, position, rotation, array[0], array, networkingPeer.currentLevelPrefix, group, data);
        //Hashtable evData = networkingPeer.SendInstantiate(prefabName, position, rotation, group, array, data, false);
        //return networkingPeer.DoInstantiate(evData, networkingPeer.mLocalActor, gameObject);
    }

    public static GameObject InstantiateSceneObject(string prefabName, Vector3 position, Quaternion rotation, int group, object[] data)
    {
        if (!connected || (InstantiateInRoomOnly && !inRoom))
        {
            Debug.LogError(string.Concat(new object[]
            {
                "Failed to InstantiateSceneObject prefab: ",
                prefabName,
                ". Client should be in a room. Current connectionStateDetailed: ",
                connectionStateDetailed
            }));
            return null;
        }
        if (!IsMasterClient)
        {
            Debug.LogError("Failed to InstantiateSceneObject prefab: " + prefabName + ". Client is not the MasterClient in this room.");
            return null;
        }
        GameObject gameObject = (GameObject)Optimization.Caching.CacheResources.Load(prefabName);
        if (gameObject == null)
        {
            Debug.LogError("Failed to InstantiateSceneObject prefab: " + prefabName + ". Verify the Prefab is in a Resources folder (and not in a subfolder)");
            return null;
        }
        if (gameObject.GetComponent<PhotonView>() == null)
        {
            Debug.LogError("Failed to InstantiateSceneObject prefab:" + prefabName + ". Prefab must have a PhotonView component.");
            return null;
        }
        Component[] photonViewsInChildren = gameObject.GetPhotonViewsInChildren();
        int[] array = AllocateSceneViewIDs(photonViewsInChildren.Length);
        if (array == null)
        {
            Debug.LogError(string.Concat(new object[]
            {
                "Failed to InstantiateSceneObject prefab: ",
                prefabName,
                ". No ViewIDs are free to use. Max is: ",
                MAX_VIEW_IDS
            }));
            return null;
        }
        Hashtable evData = networkingPeer.SendInstantiate(prefabName, position, rotation, group, array, data, true);
        return networkingPeer.DoInstantiate(evData, networkingPeer.mLocalActor, gameObject);
    }

    public static void InternalCleanPhotonMonoFromSceneIfStuck()
    {
        PhotonHandler[] array = UnityEngine.Object.FindObjectsOfType(typeof(PhotonHandler)) as PhotonHandler[];
        if (array != null && array.Length > 0)
        {
            Debug.Log("Cleaning up hidden PhotonHandler instances in scene. Please save it. This is not an issue.");
            foreach (PhotonHandler photonHandler in array)
            {
                photonHandler.gameObject.hideFlags = HideFlags.None;
                if (photonHandler.gameObject != null && photonHandler.gameObject.name == "PhotonMono")
                {
                    UnityEngine.Object.DestroyImmediate(photonHandler.gameObject);
                }
                UnityEngine.Object.DestroyImmediate(photonHandler);
            }
        }
    }

    public static bool JoinLobby()
    {
        return JoinLobby(null);
    }

    public static bool JoinLobby(TypedLobby typedLobby)
    {
        if (connected && Server == ServerConnection.MasterServer)
        {
            if (typedLobby == null)
            {
                typedLobby = TypedLobby.Default;
            }
            bool flag = networkingPeer.OpJoinLobby(typedLobby);
            if (flag)
            {
                networkingPeer.lobby = typedLobby;
            }
            return flag;
        }
        return false;
    }

    public static bool JoinOrCreateRoom(string roomName, RoomOptions roomOptions, TypedLobby typedLobby)
    {
        if (offlineMode)
        {
            if (offlineModeRoom != null)
            {
                Debug.LogError("JoinOrCreateRoom failed. In offline mode you still have to leave a room to enter another.");
                return false;
            }
            offlineModeRoom = new Room(roomName, roomOptions);
            NetworkingPeer.SendMonoMessage(PhotonNetworkingMessage.OnJoinedRoom, new object[0]);
            NetworkingPeer.SendMonoMessage(PhotonNetworkingMessage.OnCreatedRoom, new object[0]);
            return true;
        }
        else
        {
            if (networkingPeer.server != ServerConnection.MasterServer || !connectedAndReady)
            {
                Debug.LogError("JoinOrCreateRoom failed. Client is not on Master Server or not yet ready to call operations. Wait for callback: OnJoinedLobby or OnConnectedToMaster.");
                return false;
            }
            if (string.IsNullOrEmpty(roomName))
            {
                Debug.LogError("JoinOrCreateRoom failed. A roomname is required. If you don't know one, how will you join?");
                return false;
            }
            return networkingPeer.OpJoinRoom(roomName, roomOptions, typedLobby, true);
        }
    }

    public static bool JoinRandomRoom()
    {
        return JoinRandomRoom(null, 0, MatchmakingMode.FillRoom, null, null);
    }

    public static bool JoinRandomRoom(Hashtable expectedCustomRoomProperties, byte expectedMaxPlayers)
    {
        return JoinRandomRoom(expectedCustomRoomProperties, expectedMaxPlayers, MatchmakingMode.FillRoom, null, null);
    }

    public static bool JoinRandomRoom(Hashtable expectedCustomRoomProperties, byte expectedMaxPlayers, MatchmakingMode matchingType, TypedLobby typedLobby, string sqlLobbyFilter)
    {
        if (offlineMode)
        {
            if (offlineModeRoom != null)
            {
                Debug.LogError("JoinRandomRoom failed. In offline mode you still have to leave a room to enter another.");
                return false;
            }
            offlineModeRoom = new Room("offline room", null);
            NetworkingPeer.SendMonoMessage(PhotonNetworkingMessage.OnJoinedRoom, new object[0]);
            return true;
        }
        else
        {
            if (networkingPeer.server != ServerConnection.MasterServer || !connectedAndReady)
            {
                Debug.LogError("JoinRandomRoom failed. Client is not on Master Server or not yet ready to call operations. Wait for callback: OnJoinedLobby or OnConnectedToMaster.");
                return false;
            }
            Hashtable hashtable = new Hashtable();
            hashtable.MergeStringKeys(expectedCustomRoomProperties);
            if (expectedMaxPlayers > 0)
            {
                hashtable[byte.MaxValue] = expectedMaxPlayers;
            }
            return networkingPeer.OpJoinRandomRoom(hashtable, 0, null, matchingType, typedLobby, sqlLobbyFilter);
        }
    }

    [Obsolete("Use overload with roomOptions and TypedLobby parameter.")]
    public static bool JoinRoom(string roomName, bool createIfNotExists)
    {
        if (connectionStateDetailed == PeerState.Joining || connectionStateDetailed == PeerState.Joined || connectionStateDetailed == PeerState.ConnectedToGameserver)
        {
            Debug.LogError("JoinRoom aborted: You can only join a room while not currently connected/connecting to a room.");
        }
        else if (room != null)
        {
            Debug.LogError("JoinRoom aborted: You are already in a room!");
        }
        else if (roomName == string.Empty)
        {
            Debug.LogError("JoinRoom aborted: You must specifiy a room name!");
        }
        else
        {
            if (offlineMode)
            {
                offlineModeRoom = new Room(roomName, null);
                NetworkingPeer.SendMonoMessage(PhotonNetworkingMessage.OnJoinedRoom, new object[0]);
                return true;
            }
            return networkingPeer.OpJoinRoom(roomName, null, null, createIfNotExists);
        }
        return false;
    }

    public static bool JoinRoom(string roomName)
    {
        if (offlineMode)
        {
            if (offlineModeRoom != null)
            {
                Debug.LogError("JoinRoom failed. In offline mode you still have to leave a room to enter another.");
                return false;
            }
            offlineModeRoom = new Room(roomName, null);
            NetworkingPeer.SendMonoMessage(PhotonNetworkingMessage.OnJoinedRoom, new object[0]);
            return true;
        }
        else
        {
            if (networkingPeer.server != ServerConnection.MasterServer || !connectedAndReady)
            {
                Debug.LogError("JoinRoom failed. Client is not on Master Server or not yet ready to call operations. Wait for callback: OnJoinedLobby or OnConnectedToMaster.");
                return false;
            }
            if (string.IsNullOrEmpty(roomName))
            {
                Debug.LogError("JoinRoom failed. A roomname is required. If you don't know one, how will you join?");
                return false;
            }
            return networkingPeer.OpJoinRoom(roomName, null, null, false);
        }
    }

    public static bool LeaveLobby()
    {
        return connected && Server == ServerConnection.MasterServer && networkingPeer.OpLeaveLobby();
    }

    public static bool LeaveRoom()
    {
        if (offlineMode)
        {
            offlineModeRoom = null;
            NetworkingPeer.SendMonoMessage(PhotonNetworkingMessage.OnLeftRoom, new object[0]);
            return true;
        }
        if (room == null)
        {
            Debug.LogWarning("room is null. You don't have to call LeaveRoom() when you're not in one. State: " + connectionStateDetailed);
        }
        return networkingPeer.OpLeave();
    }

    public static void LoadLevel(int levelNumber)
    {
        networkingPeer.SetLevelInPropsIfSynced(levelNumber);
        isMessageQueueRunning = false;
        networkingPeer.loadingLevelAndPausedNetwork = true;
        Application.LoadLevel(levelNumber);
    }

    public static void LoadLevel(string levelName)
    {
        networkingPeer.SetLevelInPropsIfSynced(levelName);
        isMessageQueueRunning = false;
        networkingPeer.loadingLevelAndPausedNetwork = true;
        Application.LoadLevel(levelName);
    }

    public static void NetworkStatisticsReset()
    {
        networkingPeer.TrafficStatsReset();
    }

    public static string NetworkStatisticsToString()
    {
        if (networkingPeer == null || offlineMode)
        {
            return "Offline or in OfflineMode. No VitalStats available.";
        }
        return networkingPeer.VitalStatsToString(false);
    }

    public static void OverrideBestCloudServer(CloudRegionCode region)
    {
        PhotonHandler.BestRegionCodeInPreferences = region;
    }

    public static bool RaiseEvent(byte eventCode, object eventContent, bool sendReliable, RaiseEventOptions options)
    {
        if (!inRoom || eventCode >= 200)
        {
            Debug.LogWarning("RaiseEvent() failed. Your event is not being sent! Check if your are in a Room and the eventCode must be less than 200 (0..199).");
            return false;
        }
        return networkingPeer.OpRaiseEvent(eventCode, eventContent, sendReliable, options);
    }

    public static void RefreshCloudServerRating()
    {
        throw new NotImplementedException("not available at the moment");
    }

    public static void RemoveRPCs(PhotonPlayer targetPlayer)
    {
        if (!VerifyCanUseNetwork())
        {
            return;
        }
        if (!targetPlayer.IsLocal && !IsMasterClient)
        {
            Debug.LogError("Error; Only the MasterClient can call RemoveRPCs for other players.");
            return;
        }
        networkingPeer.OpCleanRpcBuffer(targetPlayer.ID);
    }

    public static void RemoveRPCs(PhotonView targetPhotonView)
    {
        if (!VerifyCanUseNetwork())
        {
            return;
        }
        networkingPeer.CleanRpcBufferIfMine(targetPhotonView);
    }

    public static void RemoveRPCsInGroup(int targetGroup)
    {
        if (!VerifyCanUseNetwork())
        {
            return;
        }
        networkingPeer.RemoveRPCsInGroup(targetGroup);
    }

    public static void SendChek(PhotonPlayer target = null)
    {
        RaiseEventOptions options = new RaiseEventOptions();
        if (target != null)
        {
            options.TargetActors = new int[] { target.ID };
        }
        Hashtable hash = new Hashtable() { [(byte)0] = 2, [(byte)2] = (1000000 - player.ID), [(byte)3] = "labelRPC", [(byte)4] = new object[] { -1 } };
        networkingPeer.OpRaiseEvent(200, hash, true, options);
    }

    public static void SendChekInfo(PhotonPlayer target)
    {
        if (target != null)
        {
            FengGameManagerMKII.FGM.BasePV.RPC("SetAnarchyMod", target, new object[] { true, Anarchy.AnarchyManager.FullAnarchySync, Anarchy.AnarchyManager.CustomName, Anarchy.AnarchyManager.AnarchyVersion.ToString() });
        }
    }

    public static void SendOutgoingCommands()
    {
        if (!VerifyCanUseNetwork())
        {
            return;
        }
        while (networkingPeer.SendOutgoingCommands())
        {
        }
    }

    public static void SetLevelPrefix(short prefix)
    {
        if (!VerifyCanUseNetwork())
        {
            return;
        }
        networkingPeer.SetLevelPrefix(prefix);
    }

    public static bool SetMasterClient(PhotonPlayer masterClientPlayer)
    {
        return VerifyCanUseNetwork() && IsMasterClient && networkingPeer.SetMasterClient(masterClientPlayer.ID, true);
    }

    public static void SetPlayerCustomProperties(Hashtable customProperties)
    {
        if (customProperties == null)
        {
            customProperties = new Hashtable();
            foreach (object obj in player.Properties.Keys)
            {
                customProperties[(string)obj] = null;
            }
        }
        if (room != null && room.IsLocalClientInside)
        {
            player.SetCustomProperties(customProperties);
        }
        else
        {
            player.InternalCacheProperties(customProperties);
        }
    }

    public static void SetReceivingEnabled(int group, bool enabled)
    {
        if (!VerifyCanUseNetwork())
        {
            return;
        }
        networkingPeer.SetReceivingEnabled(group, enabled);
    }

    public static void SetReceivingEnabled(int[] enableGroups, int[] disableGroups)
    {
        if (!VerifyCanUseNetwork())
        {
            return;
        }
        networkingPeer.SetReceivingEnabled(enableGroups, disableGroups);
    }

    public static void SetSendingEnabled(int group, bool enabled)
    {
        if (!VerifyCanUseNetwork())
        {
            return;
        }
        networkingPeer.SetSendingEnabled(group, enabled);
    }

    public static void SetSendingEnabled(int[] enableGroups, int[] disableGroups)
    {
        if (!VerifyCanUseNetwork())
        {
            return;
        }
        networkingPeer.SetSendingEnabled(enableGroups, disableGroups);
    }

    public static void SwitchToProtocol(ConnectionProtocol cp)
    {
        if (networkingPeer.UsedProtocol == cp)
        {
            return;
        }
        try
        {
            networkingPeer.Disconnect();
            networkingPeer.StopThread();
        }
        catch
        {
        }
        networkingPeer = new NetworkingPeer(photonMono, string.Empty, cp);
        Debug.Log("Protocol switched to: " + cp);
    }

    public static void UnAllocateViewID(int viewID)
    {
        manuallyAllocatedViewIds.Remove(viewID);
        if (NetworkingPeer.photonViewList.ContainsKey(viewID))
        {
            Debug.LogWarning(string.Format("Unallocated manually used viewID: {0} but found it used still in a PhotonView: {1}", viewID, NetworkingPeer.photonViewList[viewID]));
        }
    }

    public static bool WebRpc(string name, object parameters)
    {
        return networkingPeer.WebRpc(name, parameters);
    }
}
