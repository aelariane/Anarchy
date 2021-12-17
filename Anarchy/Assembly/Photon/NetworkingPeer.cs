using Anarchy.Network;
using Anarchy.Network.Events;
using Anarchy.UI;
using ExitGames.Client.Photon;
using Optimization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

internal class NetworkingPeer : LoadbalancingPeer, IPhotonPeerListener
{
    internal static readonly Dictionary<ConnectionProtocol, int> ProtocolToNameServerPort;
    private readonly Dictionary<string, int> rpcShortcuts;
    public HashSet<int> allowedReceivingGroups;
    private HashSet<int> blockSendingGroups;
    private bool didAuthenticate;
    private IPhotonPeerListener externalListener;
    private string[] friendListRequested;
    private int friendListTimestamp;
    private bool isFetchingFriends;
    public JoinType mLastJoinType;
    internal Dictionary<Type, List<MethodInfo>> monoRPCMethodsCache;
    private bool mPlayernameHasToBeUpdated;
    private string playername;
    private Dictionary<int, object[]> tempInstantiationData;
    protected internal const string CurrentSceneProperty = "curScn";
    protected internal short currentLevelPrefix;
    protected internal bool loadingLevelAndPausedNetwork;
    protected internal string mAppId;
    protected internal string mAppVersion;
    internal static Dictionary<PhotonNetworkingMessage, AOTEvent> OnNetworkMessage = new Dictionary<PhotonNetworkingMessage, AOTEvent>();
    protected internal static Dictionary<int, PhotonView> photonViewList;
    public static Dictionary<string, GameObject> PrefabCache = new Dictionary<string, GameObject>();
    public static bool UsePrefabCache = true;
    public bool hasSwitchedMC;
    public bool insideLobby;
    public Dictionary<int, GameObject> instantiatedObjects;
    public bool IsInitialConnect;
    public static Dictionary<int, PhotonPlayer> mActors;
    public static Dictionary<string, RoomInfo> mGameList;
    public RoomInfo[] mGameListCopy;
    public static PhotonPlayer mMasterClient;
    public PhotonPlayer[] mOtherPlayerListCopy;
    public PhotonPlayer[] mPlayerListCopy;
    public string NameServerAddress;
    public bool requestSecurity;

    static NetworkingPeer()
    {
        Dictionary<ConnectionProtocol, int> dictionary = new Dictionary<ConnectionProtocol, int>();
        dictionary.Add(ConnectionProtocol.Udp, 5055);
        dictionary.Add(ConnectionProtocol.Tcp, 4530);
        dictionary.Add(ConnectionProtocol.WebSocket, 9090);
        dictionary.Add(ConnectionProtocol.WebSocketSecure, 19090);
        ProtocolToNameServerPort = dictionary;
    }

    public NetworkingPeer(IPhotonPeerListener listener, string playername, ConnectionProtocol connectionProtocol) : base(listener, connectionProtocol)
    {
        //DebugOut = DebugLevel.ALL;
        //PhotonNetwork.logLevel = PhotonLogLevel.Full;
        this.playername = string.Empty;
        mActors = new Dictionary<int, PhotonPlayer>();
        this.mOtherPlayerListCopy = new PhotonPlayer[0];
        this.mPlayerListCopy = new PhotonPlayer[0];
        this.requestSecurity = true;
        this.monoRPCMethodsCache = new Dictionary<System.Type, List<MethodInfo>>();
        mGameList = new Dictionary<string, RoomInfo>();
        this.mGameListCopy = new RoomInfo[0];
        this.instantiatedObjects = new Dictionary<int, GameObject>();
        this.allowedReceivingGroups = new HashSet<int>();
        this.blockSendingGroups = new HashSet<int>();
        photonViewList = new Dictionary<int, PhotonView>();
        this.NameServerAddress = "ns.exitgamescloud.com";
        this.tempInstantiationData = new Dictionary<int, object[]>();
        base.Listener = this;
        this.lobby = TypedLobby.Default;
        this.externalListener = listener;
        this.PlayerName = playername;
        this.mLocalActor = new PhotonPlayer(true, -1, this.playername);
        this.AddNewPlayer(this.mLocalActor.ID, this.mLocalActor);
        this.rpcShortcuts = new Dictionary<string, int>(PhotonNetwork.PhotonServerSettings.RpcList.Count);
        for (int i = 0; i < PhotonNetwork.PhotonServerSettings.RpcList.Count; i++)
        {
            string str = PhotonNetwork.PhotonServerSettings.RpcList[i];
            this.rpcShortcuts[str] = i;
        }
        base.SerializationProtocolType = Anarchy.Network.NetworkSettings.SerializationProtocol;
        base.ReuseEventInstance = true; //Well it doesn't change anything yet so
        this.State = global::PeerState.PeerCreated;

        //DebugOut = DebugLevel.ALL;
        //PhotonNetwork.logLevel = PhotonLogLevel.Full;
    }

    protected internal int FriendsListAge
    {
        get
        {
            return ((!this.isFetchingFriends && (this.friendListTimestamp != 0)) ? (Environment.TickCount - this.friendListTimestamp) : 0);
        }
    }

    protected internal string mAppVersionPun
    {
        get
        {
            return string.Format("{0}_{1}", this.mAppVersion, PhotonNetwork.versionPUN);
        }
    }

    protected internal ServerConnection server { get; private set; }
    internal RoomOptions mRoomOptionsForCreate { get; set; }
    internal TypedLobby mRoomToEnterLobby { get; set; }
    internal Room mRoomToGetInto { get; set; }
    public List<Region> AvailableRegions { get; protected internal set; }
    public CloudRegionCode CloudRegion { get; protected internal set; }
    public AuthenticationValues CustomAuthenticationValues { get; set; }

    public bool IsAuthorizeSecretAvailable
    {
        get
        {
            return false;
        }
    }

    public bool IsUsingNameServer { get; protected internal set; }
    public TypedLobby lobby { get; set; }
    public string MasterServerAddress { get; protected internal set; }

    public Room mCurrentGame
    {
        get
        {
            if ((this.mRoomToGetInto != null) && this.mRoomToGetInto.IsLocalClientInside)
            {
                return this.mRoomToGetInto;
            }
            return null;
        }
    }

    public int mGameCount { get; internal set; }
    public string mGameserver { get; internal set; }
    public PhotonPlayer mLocalActor { get; internal set; }
    public int mPlayersInRoomsCount { get; internal set; }
    public int mPlayersOnMasterCount { get; internal set; }
    public int mQueuePosition { get; internal set; }

    public string PlayerName
    {
        get
        {
            return this.playername;
        }
        set
        {
            if (!string.IsNullOrEmpty(value) && !value.Equals(this.playername))
            {
                if (this.mLocalActor != null)
                {
                    this.mLocalActor.FriendName = value;
                }
                this.playername = value;
                if (this.mCurrentGame != null)
                {
                    this.SendPlayerName();
                }
            }
        }
    }

    public PeerState State { get; internal set; }

    private static int ReturnLowestPlayerId(PhotonPlayer[] players, int playerIdToIgnore)
    {
        if ((players == null) || (players.Length == 0))
        {
            return -1;
        }
        int iD = int.MaxValue;
        for (int i = 0; i < players.Length; i++)
        {
            PhotonPlayer player = players[i];
            if ((player.ID != playerIdToIgnore) && (player.ID < iD))
            {
                iD = player.ID;
            }
        }
        return iD;
    }

    public void AddNewPlayer(int ID, PhotonPlayer player)
    {
        if (!mActors.ContainsKey(ID))
        {
            mActors[ID] = player;
            this.RebuildPlayerListCopies();
        }
        else
        {
            Debug.LogError("Adding player twice: " + ID);
        }
    }

    private bool AlmostEquals(object[] lastData, object[] currentContent)
    {
        if ((lastData != null) || (currentContent != null))
        {
            if (((lastData == null) || (currentContent == null)) || (lastData.Length != currentContent.Length))
            {
                return false;
            }
            for (int i = 0; i < currentContent.Length; i++)
            {
                object one = currentContent[i];
                object two = lastData[i];
                if (!this.ObjectIsSameWithInprecision(one, two))
                {
                    return false;
                }
            }
        }
        return true;
    }

    private void CheckMasterClient(int leavingPlayerId)
    {
        bool flag = (mMasterClient != null) && (mMasterClient.ID == leavingPlayerId);
        bool flag2 = leavingPlayerId > 0;
        if (!flag2 || flag)
        {
            if (mActors.Count <= 1)
            {
                mMasterClient = this.mLocalActor;
            }
            else
            {
                int num = int.MaxValue;
                foreach (int num2 in mActors.Keys)
                {
                    if ((num2 < num) && (num2 != leavingPlayerId))
                    {
                        num = num2;
                    }
                }
                mMasterClient = mActors[num];
            }
            if (flag2)
            {
                SendMonoMessage(PhotonNetworkingMessage.OnMasterClientSwitched, new object[] { mMasterClient });
            }
        }
    }

    internal bool CheckTypeMatch(ParameterInfo[] methodParameters, System.Type[] callParameterTypes)
    {
        if (methodParameters.Length < callParameterTypes.Length)
        {
            return false;
        }
        for (int i = 0; i < callParameterTypes.Length; i++)
        {
            System.Type parameterType = methodParameters[i].ParameterType;
            if ((callParameterTypes[i] != null) && !parameterType.Equals(callParameterTypes[i]))
            {
                return false;
            }
        }
        return true;
    }

    private bool DeltaCompressionRead(PhotonView view, ExitGames.Client.Photon.Hashtable data)
    {
        if (!data.ContainsKey((byte)1))
        {
            if (view.lastOnSerializeDataReceived == null)
            {
                return false;
            }
            object[] objArray = data[(byte)2] as object[];
            if (objArray == null)
            {
                return false;
            }
            int[] target = data[(byte)3] as int[];
            if (target == null)
            {
                target = new int[0];
            }
            object[] lastOnSerializeDataReceived = view.lastOnSerializeDataReceived;
            for (int i = 0; i < objArray.Length; i++)
            {
                if ((objArray[i] == null) && !target.Contains(i))
                {
                    objArray[i] = lastOnSerializeDataReceived[i];
                }
            }
            data[(byte)1] = objArray;
        }
        return true;
    }

    private bool DeltaCompressionWrite(PhotonView view, ExitGames.Client.Photon.Hashtable data)
    {
        if (view.lastOnSerializeDataSent != null)
        {
            object[] lastOnSerializeDataSent = view.lastOnSerializeDataSent;
            object[] objArray2 = data[(byte)1] as object[];
            if (objArray2 == null)
            {
                return false;
            }
            if (lastOnSerializeDataSent.Length != objArray2.Length)
            {
                return true;
            }
            object[] objArray3 = new object[objArray2.Length];
            int num = 0;
            List<int> list = new List<int>();
            for (int i = 0; i < objArray3.Length; i++)
            {
                object one = objArray2[i];
                object two = lastOnSerializeDataSent[i];
                if (this.ObjectIsSameWithInprecision(one, two))
                {
                    num++;
                }
                else
                {
                    objArray3[i] = objArray2[i];
                    if (one == null)
                    {
                        list.Add(i);
                    }
                }
            }
            if (num > 0)
            {
                data.Remove((byte)1);
                if (num == objArray2.Length)
                {
                    return false;
                }
                data[(byte)2] = objArray3;
                if (list.Count > 0)
                {
                    data[(byte)3] = list.ToArray();
                }
            }
        }
        return true;
    }

    private void DisconnectToReconnect()
    {
        if (NetworkSettings.CustomSettings.Value)
        {
            if(NetworkSettings.IsCustomPhotonServer.Value)
            {
                this.mGameserver = this.MasterServerAddress.Split(new char[] { ':' })[0] + ":" + this.mGameserver.Split(new char[] { ':' })[1];
            }
        }
        else
        {
            if(mGameserver.Contains("127.0.0.1"))
            {
                int addressIndex = TransportProtocol >= ConnectionProtocol.WebSocket ? 1 : 0;
                string actualAddress = MasterServerAddress.Split(':')[addressIndex];

                if (addressIndex > 0)
                {
                    actualAddress = actualAddress.Substring(2);
                }

                this.mGameserver = mGameserver.Replace("127.0.0.1", actualAddress);
            }
        }

        switch (this.server)
        {
            case ServerConnection.MasterServer:
                this.State = global::PeerState.DisconnectingFromMasterserver;
                base.Disconnect();
                break;

            case ServerConnection.GameServer:
                this.State = global::PeerState.DisconnectingFromGameserver;
                base.Disconnect();
                break;

            case ServerConnection.NameServer:
                this.State = global::PeerState.DisconnectingFromNameServer;
                base.Disconnect();
                break;
        }
    }

    private void GameEnteredOnGameServer(OperationResponse operationResponse)
    {
        if (operationResponse.ReturnCode == 0)
        {
            this.State = global::PeerState.Joined;
            this.mRoomToGetInto.IsLocalClientInside = true;
            ExitGames.Client.Photon.Hashtable pActorProperties = (ExitGames.Client.Photon.Hashtable)operationResponse[0xf9];
            ExitGames.Client.Photon.Hashtable gameProperties = (ExitGames.Client.Photon.Hashtable)operationResponse[0xf8];
            PropertiesChecker.CheckPlayersProperties(pActorProperties, 0, null);
            PropertiesChecker.CheckRoomProperties(gameProperties, null);
            int newID = (int)operationResponse[0xfe];
            this.ChangeLocalID(newID);
            this.CheckMasterClient(-1);
            if (this.mPlayernameHasToBeUpdated)
            {
                this.SendPlayerName();
            }
            if (operationResponse.OperationCode == OperationCode.CreateGame)
            {
                SendMonoMessage(PhotonNetworkingMessage.OnCreatedRoom, new object[0]);
            }
        }
        else
        {
            switch (operationResponse.OperationCode)
            {
                case OperationCode.JoinRandomGame:
                    {
                        if (PhotonNetwork.logLevel >= PhotonLogLevel.Informational)
                        {
                            Debug.Log("Join failed on GameServer. Changing back to MasterServer. Msg: " + operationResponse.DebugMessage);
                            if (operationResponse.ReturnCode == 0x7ff6)
                            {
                                Debug.Log("Most likely the game became empty during the switch to GameServer.");
                            }
                        }
                        object[] parameters = new object[] { operationResponse.ReturnCode, operationResponse.DebugMessage };
                        SendMonoMessage(PhotonNetworkingMessage.OnPhotonRandomJoinFailed, parameters);
                        break;
                    }
                case OperationCode.JoinGame:
                    {
                        if (PhotonNetwork.logLevel >= PhotonLogLevel.Informational)
                        {
                            Debug.Log("Join failed on GameServer. Changing back to MasterServer. Msg: " + operationResponse.DebugMessage);
                            if (operationResponse.ReturnCode == 0x7ff6)
                            {
                                Debug.Log("Most likely the game became empty during the switch to GameServer.");
                            }
                        }
                        SendMonoMessage(PhotonNetworkingMessage.OnPhotonJoinRoomFailed, new object[] { operationResponse.ReturnCode, operationResponse.DebugMessage });
                        break;
                    }
                case OperationCode.CreateGame:
                    {
                        if (PhotonNetwork.logLevel >= PhotonLogLevel.Informational)
                        {
                            Debug.Log("Create failed on GameServer. Changing back to MasterServer. Msg: " + operationResponse.DebugMessage);
                        }
                        SendMonoMessage(PhotonNetworkingMessage.OnPhotonCreateRoomFailed, new object[] { operationResponse.ReturnCode, operationResponse.DebugMessage });
                        break;
                    }
            }
            this.DisconnectToReconnect();
        }
    }

    internal ExitGames.Client.Photon.Hashtable GetActorPropertiesForActorNr(ExitGames.Client.Photon.Hashtable actorProperties, int actorNr)
    {
        if (actorProperties.ContainsKey(actorNr))
        {
            return (ExitGames.Client.Photon.Hashtable)actorProperties[actorNr];
        }
        return actorProperties;
    }

    private ExitGames.Client.Photon.Hashtable GetLocalActorProperties()
    {
        if (PhotonNetwork.player != null)
        {
            return PhotonNetwork.player.AllProperties;
        }
        ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable();
        hashtable[byte.MaxValue] = this.PlayerName;
        return hashtable;
    }

    private PhotonPlayer GetPlayerWithID(int number)
    {
        if ((mActors != null) && mActors.ContainsKey(number))
        {
            return mActors[number];
        }
        return null;
    }

    public void HandleEventLeave(int actorID)
    {
        if (PhotonNetwork.logLevel >= PhotonLogLevel.Informational)
        {
            Debug.Log("HandleEventLeave for player ID: " + actorID);
        }
        if ((actorID < 0) || !mActors.ContainsKey(actorID))
        {
            Debug.LogError(string.Format("Received event Leave for unknown player ID: {0}", actorID));
        }
        else
        {
            PhotonPlayer playerWithID = this.GetPlayerWithID(actorID);
            if (playerWithID == null)
            {
                Debug.LogError("HandleEventLeave for player ID: " + actorID + " has no PhotonPlayer!");
            }
            this.CheckMasterClient(actorID);
            if ((this.mCurrentGame != null) && this.mCurrentGame.AutoCleanUp)
            {
                this.DestroyPlayerObjects(actorID, true);
            }
            this.RemovePlayer(actorID, playerWithID);
            SendMonoMessage(PhotonNetworkingMessage.OnPhotonPlayerDisconnected, new object[] { playerWithID });
        }
    }

    private void LeftLobbyCleanup()
    {
        mGameList = new Dictionary<string, RoomInfo>();
        this.mGameListCopy = new RoomInfo[0];
        if (this.insideLobby)
        {
            this.insideLobby = false;
            SendMonoMessage(PhotonNetworkingMessage.OnLeftLobby, new object[0]);
        }
    }

    private void LeftRoomCleanup()
    {
        bool flag = this.mRoomToGetInto != null;
        bool flag2 = (this.mRoomToGetInto == null) ? PhotonNetwork.autoCleanUpPlayerObjects : this.mRoomToGetInto.AutoCleanUp;
        this.hasSwitchedMC = false;
        this.mRoomToGetInto = null;
        mActors = new Dictionary<int, PhotonPlayer>();
        this.mPlayerListCopy = new PhotonPlayer[0];
        this.mOtherPlayerListCopy = new PhotonPlayer[0];
        mMasterClient = null;
        this.allowedReceivingGroups = new HashSet<int>();
        this.blockSendingGroups = new HashSet<int>();
        mGameList = new Dictionary<string, RoomInfo>();
        this.mGameListCopy = new RoomInfo[0];
        this.isFetchingFriends = false;
        this.ChangeLocalID(-1);
        if (flag2)
        {
            this.LocalCleanupAnythingInstantiated(true);
            PhotonNetwork.manuallyAllocatedViewIds = new List<int>();
        }
        if (flag)
        {
            SendMonoMessage(PhotonNetworkingMessage.OnLeftRoom, new object[0]);
        }
    }

    private bool ObjectIsSameWithInprecision(object one, object two)
    {
        if ((one == null) || (two == null))
        {
            return ((one == null) && (two == null));
        }
        if (one.Equals(two))
        {
            return true;
        }
        if (one is Vector3)
        {
            Vector3 target = (Vector3)one;
            Vector3 second = (Vector3)two;
            if (target.AlmostEquals(second, PhotonNetwork.precisionForVectorSynchronization))
            {
                return true;
            }
        }
        else if (one is Vector2)
        {
            Vector2 vector3 = (Vector2)one;
            Vector2 vector4 = (Vector2)two;
            if (vector3.AlmostEquals(vector4, PhotonNetwork.precisionForVectorSynchronization))
            {
                return true;
            }
        }
        else if (one is Quaternion)
        {
            Quaternion quaternion = (Quaternion)one;
            Quaternion quaternion2 = (Quaternion)two;
            if (quaternion.AlmostEquals(quaternion2, PhotonNetwork.precisionForQuaternionSynchronization))
            {
                return true;
            }
        }
        else if (one is float)
        {
            float num = (float)one;
            float num2 = (float)two;
            if (num.AlmostEquals(num2, PhotonNetwork.precisionForFloatSynchronization))
            {
                return true;
            }
        }
        return false;
    }

    private void OnSerializeRead(ExitGames.Client.Photon.Hashtable data, PhotonPlayer sender, int networkTime, short correctPrefix)
    {
        int viewID = (int)data[(byte)0];
        PhotonView photonView = this.GetPhotonView(viewID);
        if (photonView == null)
        {
            Debug.LogWarning(string.Concat(new object[] { "Received OnSerialization for view ID ", viewID, ". We have no such PhotonView! Ignored this if you're leaving a room. State: ", this.State }));
        }
        else if ((photonView.prefix > 0) && (correctPrefix != photonView.prefix))
        {
            Debug.LogError(string.Concat(new object[] { "Received OnSerialization for view ID ", viewID, " with prefix ", correctPrefix, ". Our prefix is ", photonView.prefix }));
        }
        else if ((photonView.Group == 0) || this.allowedReceivingGroups.Contains(photonView.Group))
        {
            if (photonView.synchronization == ViewSynchronization.ReliableDeltaCompressed)
            {
                if (!this.DeltaCompressionRead(photonView, data))
                {
                    if (PhotonNetwork.logLevel >= PhotonLogLevel.Informational)
                    {
                        Debug.Log(string.Concat(new object[] { "Skipping packet for ", photonView.name, " [", photonView.viewID, "] as we haven't received a full packet for delta compression yet. This is OK if it happens for the first few frames after joining a game." }));
                    }
                    return;
                }
                photonView.lastOnSerializeDataReceived = data[(byte)1] as object[];
            }
            if (photonView.observed is MonoBehaviour)
            {
                object[] incomingData = data[(byte)1] as object[];
                PhotonStream pStream = new PhotonStream(false, incomingData);
                PhotonMessageInfo info = new PhotonMessageInfo(sender, networkTime, photonView);
                photonView.ExecuteOnSerialize(pStream, info);
            }
            else if (photonView.observed is Transform)
            {
                object[] objArray2 = data[(byte)1] as object[];
                Transform observed = (Transform)photonView.observed;
                if ((objArray2.Length >= 1) && (objArray2[0] != null))
                {
                    observed.localPosition = (Vector3)objArray2[0];
                }
                if ((objArray2.Length >= 2) && (objArray2[1] != null))
                {
                    observed.localRotation = (Quaternion)objArray2[1];
                }
                if ((objArray2.Length >= 3) && (objArray2[2] != null))
                {
                    observed.localScale = (Vector3)objArray2[2];
                }
            }
            else if (photonView.observed is Rigidbody)
            {
                object[] objArray3 = data[(byte)1] as object[];
                Rigidbody rigidbody = (Rigidbody)photonView.observed;
                if ((objArray3.Length >= 1) && (objArray3[0] != null))
                {
                    rigidbody.velocity = (Vector3)objArray3[0];
                }
                if ((objArray3.Length >= 2) && (objArray3[1] != null))
                {
                    rigidbody.angularVelocity = (Vector3)objArray3[1];
                }
            }
            else
            {
                Debug.LogError("Type of observed is unknown when receiving.");
            }
        }
    }

    private ExitGames.Client.Photon.Hashtable OnSerializeWrite(PhotonView view)
    {
        List<object> list = new List<object>();
        if (view.observed is MonoBehaviour)
        {
            PhotonStream pStream = new PhotonStream(true, null);
            PhotonMessageInfo info = new PhotonMessageInfo(this.mLocalActor, base.ServerTimeInMilliSeconds, view);
            view.ExecuteOnSerialize(pStream, info);
            if (pStream.Count == 0)
            {
                return null;
            }
            list = pStream.data;
        }
        else if (view.observed is Transform)
        {
            Transform observed = (Transform)view.observed;
            if (((view.onSerializeTransformOption == OnSerializeTransform.OnlyPosition) || (view.onSerializeTransformOption == OnSerializeTransform.PositionAndRotation)) || (view.onSerializeTransformOption == OnSerializeTransform.All))
            {
                list.Add(observed.localPosition);
            }
            else
            {
                list.Add(null);
            }
            if (((view.onSerializeTransformOption == OnSerializeTransform.OnlyRotation) || (view.onSerializeTransformOption == OnSerializeTransform.PositionAndRotation)) || (view.onSerializeTransformOption == OnSerializeTransform.All))
            {
                list.Add(observed.localRotation);
            }
            else
            {
                list.Add(null);
            }
            if ((view.onSerializeTransformOption == OnSerializeTransform.OnlyScale) || (view.onSerializeTransformOption == OnSerializeTransform.All))
            {
                list.Add(observed.localScale);
            }
        }
        else if (view.observed is Rigidbody)
        {
            Rigidbody rigidbody = (Rigidbody)view.observed;
            if (view.onSerializeRigidBodyOption != OnSerializeRigidBody.OnlyAngularVelocity)
            {
                list.Add(rigidbody.velocity);
            }
            else
            {
                list.Add(null);
            }
            if (view.onSerializeRigidBodyOption != OnSerializeRigidBody.OnlyVelocity)
            {
                list.Add(rigidbody.angularVelocity);
            }
        }
        else
        {
            Debug.LogError("Observed type is not serializable: " + view.observed.GetType());
            return null;
        }
        object[] lastData = list.ToArray();
        if (view.synchronization == ViewSynchronization.UnreliableOnChange)
        {
            if (this.AlmostEquals(lastData, view.lastOnSerializeDataSent))
            {
                if (view.mixedModeIsReliable)
                {
                    return null;
                }
                view.mixedModeIsReliable = true;
                view.lastOnSerializeDataSent = lastData;
            }
            else
            {
                view.mixedModeIsReliable = false;
                view.lastOnSerializeDataSent = lastData;
            }
        }
        ExitGames.Client.Photon.Hashtable data = new ExitGames.Client.Photon.Hashtable();
        data[(byte)0] = view.viewID;
        data[(byte)1] = lastData;
        if (view.synchronization == ViewSynchronization.ReliableDeltaCompressed)
        {
            bool flag = this.DeltaCompressionWrite(view, data);
            view.lastOnSerializeDataSent = lastData;
            if (!flag)
            {
                return null;
            }
        }
        return data;
    }

    private void OpRemoveFromServerInstantiationsOfPlayer(int actorNr)
    {
        this.OpRaiseEvent(0xca, null, true, new RaiseEventOptions() { CachingOption = EventCaching.RemoveFromRoomCache, TargetActors = new int[] { actorNr } });
    }

    private void RebuildPlayerListCopies()
    {
        this.mPlayerListCopy = new PhotonPlayer[mActors.Count];
        mActors.Values.CopyTo(this.mPlayerListCopy, 0);
        List<PhotonPlayer> list = new List<PhotonPlayer>();
        foreach (PhotonPlayer player in this.mPlayerListCopy)
        {
            if (!player.IsLocal)
            {
                list.Add(player);
            }
        }
        this.mOtherPlayerListCopy = list.ToArray();
    }

    private void RemoveCacheOfLeftPlayers()
    {
        Dictionary<byte, object> customOpParameters = new Dictionary<byte, object>();
        customOpParameters[ParameterCode.Code] = (byte)0;
        customOpParameters[ParameterCode.Cache] = (byte)EventCaching.RemoveFromRoomCacheForActorsLeft;
        SendOperation(OperationCode.RaiseEvent, customOpParameters, SendOptions.SendReliable);
    }

    internal void RemoveInstantiationData(int instantiationId)
    {
        this.tempInstantiationData.Remove(instantiationId);
    }

    private void RemovePlayer(int ID, PhotonPlayer player)
    {
        mActors.Remove(ID);
        if (!player.IsLocal)
        {
            this.RebuildPlayerListCopies();
        }
    }

    public void ResetPhotonViewsOnSerialize()
    {
        foreach (PhotonView view in photonViewList.Values)
        {
            view.lastOnSerializeDataSent = null;
        }
    }

    private void SendDestroyOfAll()
    {
        ExitGames.Client.Photon.Hashtable customEventContent = new ExitGames.Client.Photon.Hashtable();
        customEventContent[(byte)0] = -1;
        this.OpRaiseEvent(0xcf, customEventContent, true, null);
    }

    private void SendDestroyOfPlayer(int actorNr)
    {
        ExitGames.Client.Photon.Hashtable customEventContent = new ExitGames.Client.Photon.Hashtable();
        customEventContent[(byte)0] = actorNr;
        this.OpRaiseEvent(0xcf, customEventContent, true, null);
    }

    private void SendPlayerName()
    {
        if (this.State == global::PeerState.Joining)
        {
            this.mPlayernameHasToBeUpdated = true;
        }
        else if (this.mLocalActor != null)
        {
            this.mLocalActor.FriendName = this.PlayerName;
            ExitGames.Client.Photon.Hashtable actorProperties = new ExitGames.Client.Photon.Hashtable();
            actorProperties[(byte)0xff] = this.PlayerName;
            if (this.mLocalActor.ID > 0)
            {
                base.OpSetPropertiesOfActor(this.mLocalActor.ID, actorProperties, true, 0);
                this.mPlayernameHasToBeUpdated = false;
            }
        }
    }

    internal void ServerCleanInstantiateAndDestroy(int instantiateId, int actorNr)
    {
        ExitGames.Client.Photon.Hashtable customEventContent = new ExitGames.Client.Photon.Hashtable();
        customEventContent[(byte)7] = instantiateId;
        RaiseEventOptions options2 = new RaiseEventOptions
        {
            CachingOption = EventCaching.RemoveFromRoomCache
        };
        options2.TargetActors = new int[] { actorNr };
        RaiseEventOptions raiseEventOptions = options2;
        this.OpRaiseEvent(0xca, customEventContent, true, raiseEventOptions);
        ExitGames.Client.Photon.Hashtable hashtable2 = new ExitGames.Client.Photon.Hashtable();
        hashtable2[(byte)0] = instantiateId;
        this.OpRaiseEvent(0xcc, hashtable2, true, null);
    }

    internal void StoreInstantiationData(int instantiationId, object[] instantiationData)
    {
        this.tempInstantiationData[instantiationId] = instantiationData;
    }

    protected internal static bool GetMethod(MonoBehaviour monob, string methodType, out MethodInfo mi)
    {
        mi = null;
        if ((monob != null) && !string.IsNullOrEmpty(methodType))
        {
            List<MethodInfo> methods = SupportClass.GetMethods(monob.GetType(), null);
            for (int i = 0; i < methods.Count; i++)
            {
                MethodInfo info = methods[i];
                if (info.Name.Equals(methodType))
                {
                    mi = info;
                    return true;
                }
            }
        }
        return false;
    }

    protected internal void LoadLevelIfSynced()
    {
        if (((PhotonNetwork.automaticallySyncScene && !PhotonNetwork.IsMasterClient) && (PhotonNetwork.room != null)) && PhotonNetwork.room.CustomProperties.ContainsKey(CurrentSceneProperty))
        {
            object obj2 = PhotonNetwork.room.CustomProperties[CurrentSceneProperty];
            if (obj2 is int)
            {
                if (Application.loadedLevel != ((int)obj2))
                {
                    PhotonNetwork.LoadLevel((int)obj2);
                }
            }
            else if ((obj2 is string) && (Application.loadedLevelName != ((string)obj2)))
            {
                PhotonNetwork.LoadLevel((string)obj2);
            }
        }
    }

    protected internal void LocalCleanupAnythingInstantiated(bool destroyInstantiatedGameObjects)
    {
        if (this.tempInstantiationData.Count > 0)
        {
            Debug.LogWarning("It seems some instantiation is not completed, as instantiation data is used. You should make sure instantiations are paused when calling this method. Cleaning now, despite this.");
        }
        if (destroyInstantiatedGameObjects)
        {
            HashSet<GameObject> set = new HashSet<GameObject>(this.instantiatedObjects.Values);
            foreach (GameObject obj2 in set)
            {
                this.RemoveInstantiatedGO(obj2, true);
            }
        }
        this.tempInstantiationData.Clear();
        this.instantiatedObjects = new Dictionary<int, GameObject>();
        PhotonNetwork.lastUsedViewSubId = 0;
        PhotonNetwork.lastUsedViewSubIdStatic = 0;
    }

    protected internal void SetLevelInPropsIfSynced(object levelId)
    {
        if ((PhotonNetwork.automaticallySyncScene && PhotonNetwork.IsMasterClient) && (PhotonNetwork.room != null))
        {
            if (levelId == null)
            {
                Debug.LogError("Parameter levelId can't be null!");
            }
            else
            {
                if (PhotonNetwork.room.CustomProperties.ContainsKey(CurrentSceneProperty))
                {
                    object obj2 = PhotonNetwork.room.CustomProperties[CurrentSceneProperty];
                    if ((obj2 is int) && (Application.loadedLevel == ((int)obj2)))
                    {
                        return;
                    }
                    if ((obj2 is string) && Application.loadedLevelName.Equals((string)obj2))
                    {
                        return;
                    }
                }
                ExitGames.Client.Photon.Hashtable propertiesToSet = new ExitGames.Client.Photon.Hashtable();
                if (levelId is int)
                {
                    propertiesToSet[CurrentSceneProperty] = (int)levelId;
                }
                else if (levelId is string)
                {
                    propertiesToSet[CurrentSceneProperty] = (string)levelId;
                }
                else
                {
                    Debug.LogError("Parameter levelId must be int or string!");
                }
                PhotonNetwork.room.SetCustomProperties(propertiesToSet);
                this.SendOutgoingCommands();
            }
        }
    }

    protected internal bool SetMasterClient(int playerId, bool sync)
    {
        if (((mMasterClient == null) || (mMasterClient.ID == playerId)) || !mActors.ContainsKey(playerId))
        {
            return false;
        }
        if (sync)
        {
            ExitGames.Client.Photon.Hashtable customEventContent = new ExitGames.Client.Photon.Hashtable();
            customEventContent.Add((byte)1, playerId);
            if (!this.OpRaiseEvent(0xd0, customEventContent, true, null))
            {
                return false;
            }
        }
        this.hasSwitchedMC = true;
        mMasterClient = mActors[playerId];
        object[] parameters = new object[] { mMasterClient };
        SendMonoMessage(PhotonNetworkingMessage.OnMasterClientSwitched, parameters);
        return true;
    }

    internal GameObject DoInstantiate(ExitGames.Client.Photon.Hashtable evData, PhotonPlayer photonPlayer, GameObject resourceGameObject)
    {
        int[] viewIDs;
        object[] data = null;
        string name = (string)evData[(byte)0];
        int timestamp = (int)evData[(byte)6];
        int instantiationId = (int)evData[(byte)7];
        Vector3 position = Optimization.Caching.Vectors.zero;
        if (evData.ContainsKey((byte)1))
        {
            position = (Vector3)evData[(byte)1];
        }
        Quaternion identity = Quaternion.identity;
        if (evData.ContainsKey((byte)2))
        {
            identity = (Quaternion)evData[(byte)2];
        }
        int group = 0;
        if (evData.ContainsKey((byte)3))
        {
            group = (int)evData[(byte)3];
        }
        short prefix = 0;
        if (evData.ContainsKey((byte)8))
        {
            prefix = (short)evData[(byte)8];
        }
        if (evData.ContainsKey((byte)4))
        {
            viewIDs = (int[])evData[(byte)4];
        }
        else
        {
            viewIDs = new int[] { instantiationId };
        }
        if (evData.ContainsKey((byte)5))
        {
            data = (object[])evData[(byte)5];
        }
        if ((group != 0) && !this.allowedReceivingGroups.Contains(group))
        {
            return null;
        }
        return Optimization.Caching.Pool.NetworkInstantiate(name, position, identity, instantiationId, viewIDs, prefix, group, data);

#region Vanilla version

        //if (resourceGameObject == null)
        //{
        //    resourceGameObject = key.StartsWith("RCAsset/") ? Optimization.Caching.CacheResources.RCLoad(key) : (GameObject)Optimization.Caching.CacheResources.Load(key);
        //    if (resourceGameObject == null)
        //    {
        //        Debug.LogError("PhotonNetwork error: Could not Instantiate the prefab [" + key + "]. Please verify you have this gameobject in a Resources folder.");
        //        return null;
        //    }
        //}
        //PhotonView[] photonViewsInChildren = resourceGameObject.GetPhotonViewsInChildren();
        //if (photonViewsInChildren.Length != numArray.Length)
        //{
        //    throw new Exception("Error in Instantiation! The resource's PhotonView count is not the same as in incoming data.");
        //}
        //for (int i = 0; i < numArray.Length; i++)
        //{
        //    photonViewsInChildren[i].viewID = numArray[i];
        //    photonViewsInChildren[i].prefix = num4;
        //    photonViewsInChildren[i].instantiationId = instantiationId;
        //}
        //this.StoreInstantiationData(instantiationId, objArray);
        //GameObject obj2 = (GameObject)UnityEngine.Object.Instantiate(resourceGameObject, zero, identity);
        //for (int j = 0; j < numArray.Length; j++)
        //{
        //    photonViewsInChildren[j].viewID = 0;
        //    photonViewsInChildren[j].prefix = -1;
        //    photonViewsInChildren[j].prefixBackup = -1;
        //    photonViewsInChildren[j].instantiationId = -1;
        //}
        //this.RemoveInstantiationData(instantiationId);
        //if (this.instantiatedObjects.ContainsKey(instantiationId))
        //{
        //    GameObject go = this.instantiatedObjects[instantiationId];
        //    string str2 = string.Empty;
        //    if (go != null)
        //    {
        //        foreach (PhotonView view in go.GetPhotonViewsInChildren())
        //        {
        //            if (view != null)
        //            {
        //                str2 = str2 + view.ToString() + ", ";
        //            }
        //        }
        //    }
        //    object[] args = new object[] { obj2, instantiationId, this.instantiatedObjects.Count, go, str2, PhotonNetwork.lastUsedViewSubId, PhotonNetwork.lastUsedViewSubIdStatic, photonViewList.Count };
        //    Debug.LogError(string.Format("DoInstantiate re-defines a GameObject. Destroying old entry! New: '{0}' (instantiationID: {1}) Old: {3}. PhotonViews on old: {4}. instantiatedObjects.Count: {2}. PhotonNetwork.lastUsedViewSubId: {5} PhotonNetwork.lastUsedViewSubIdStatic: {6} photonViewList.Count {7}.)", args));
        //    this.RemoveInstantiatedGO(go, true);
        //}
        //this.instantiatedObjects.Add(instantiationId, obj2);
        ////obj2.SendMessage(PhotonNetworkingMessage.OnPhotonInstantiate.ToString(), new PhotonMessageInfo(photonPlayer, timestamp, null), SendMessageOptions.DontRequireReceiver);
        //return obj2;

#endregion Vanilla version
    }

    public static void RegisterEvent(PhotonNetworkingMessage msg, AOTEvent ev)
    {
        if (!OnNetworkMessage.ContainsKey(msg))
        {
            OnNetworkMessage.Add(msg, (smth) => { });
        }
        OnNetworkMessage[msg] += ev;
    }

    public static void RemoveEvent(PhotonNetworkingMessage msg, AOTEvent ev)
    {
        OnNetworkMessage[msg] -= ev;
    }

    internal void   RPC(PhotonView view, string methodName, PhotonPlayer player, params object[] parameters)
    {
        if (player == null)
        {
            return;
        }
        if (!this.blockSendingGroups.Contains(view.Group))
        {
            if (view.viewID < 1)
            {
                Debug.LogError(string.Concat(new object[] { "Illegal view ID:", view.viewID, " method: ", methodName, " GO:", view.gameObject.name }));
            }
            if (PhotonNetwork.logLevel >= PhotonLogLevel.Full)
            {
                Debug.Log(string.Concat(new object[] { "Sending RPC \"", methodName, "\" to player[", player, "]" }));
            }
            ExitGames.Client.Photon.Hashtable rpcData = new ExitGames.Client.Photon.Hashtable();
            rpcData[(byte)0] = view.viewID;
            if (view.prefix > 0)
            {
                rpcData[(byte)1] = (short)view.prefix;
            }
            rpcData[(byte)2] = base.ServerTimeInMilliSeconds;
            int num = 0;
            if (this.rpcShortcuts.TryGetValue(methodName, out num))
            {
                rpcData[(byte)5] = (byte)num;
            }
            else
            {
                rpcData[(byte)3] = methodName;
            }
            if ((parameters != null) && (parameters.Length > 0))
            {
                rpcData[(byte)4] = parameters;
            }
            if (this.mLocalActor == player)
            {
                this.ExecuteRPC(rpcData, player);
            }
            else
            {
                this.OpRaiseEvent(200, rpcData, true, player.ToOption());
            }
        }
    }

    internal void RPC(PhotonView view, string methodName, PhotonPlayer[] players, params object[] parameters)
    {
        if (players == null || players.Length <= 0)
        {
            return;
        }
        if (!this.blockSendingGroups.Contains(view.Group))
        {
            if (view.viewID < 1)
            {
                Debug.LogError(string.Concat(new object[] { "Illegal view ID:", view.viewID, " method: ", methodName, " GO:", view.gameObject.name }));
            }
            ExitGames.Client.Photon.Hashtable rpcData = new ExitGames.Client.Photon.Hashtable();
            rpcData[(byte)0] = view.viewID;
            if (view.prefix > 0)
            {
                rpcData[(byte)1] = (short)view.prefix;
            }
            rpcData[(byte)2] = base.ServerTimeInMilliSeconds;
            int num = 0;
            if (this.rpcShortcuts.TryGetValue(methodName, out num))
            {
                rpcData[(byte)5] = (byte)num;
            }
            else
            {
                rpcData[(byte)3] = methodName;
            }
            if ((parameters != null) && (parameters.Length > 0))
            {
                rpcData[(byte)4] = parameters;
            }
            bool needLocal = false;
            List<int> IDs = new List<int>();
            foreach (PhotonPlayer player in players)
            {
                if (player.IsLocal)
                {
                    needLocal = true;
                    continue;
                }
                IDs.Add(player.ID);
            }
            if (needLocal)
            {
                ExecuteRPC(rpcData, PhotonNetwork.player);
            }
            if (IDs.Count <= 0)
            {
                return;
            }
            RaiseEventOptions raiseEventOptions = new RaiseEventOptions() { TargetActors = IDs.ToArray() };
            this.OpRaiseEvent(200, rpcData, true, raiseEventOptions);
        }
    }

    internal void RPC(PhotonView view, string methodName, PhotonTargets target, params object[] parameters)
    {
        if (blockSendingGroups.Contains(view.Group))
        {
            return;
        }
        if (view.viewID < 1)
        {
            Debug.LogError(string.Concat(new object[] { "Illegal view ID:", view.viewID, " method: ", methodName, " GO:", view.gameObject.name }));
        }
        if (PhotonNetwork.logLevel >= PhotonLogLevel.Full)
        {
            Debug.Log(string.Concat(new object[] { "Sending RPC \"", methodName, "\" to ", target }));
        }
        ExitGames.Client.Photon.Hashtable customEventContent = new ExitGames.Client.Photon.Hashtable()
        {
            [(byte)0] = view.viewID,
            [(byte)2] = ServerTimeInMilliSeconds,
        };
        if (view.prefix > 0)
        {
            customEventContent[(byte)1] = (short)view.prefix;
        }
        int num = 0;
        if (this.rpcShortcuts.TryGetValue(methodName, out num))
        {
            customEventContent[(byte)5] = (byte)num;
        }
        else
        {
            customEventContent[(byte)3] = methodName;
        }
        if ((parameters != null) && (parameters.Length > 0))
        {
            customEventContent[(byte)4] = parameters;
        }
        RaiseEventOptions options = null;
        bool executeLocal = false;
        switch (target)
        {
            case PhotonTargets.All:
                options = new RaiseEventOptions { InterestGroup = (byte)view.Group };
                executeLocal = true;
                break;

            case PhotonTargets.Others:
                options = new RaiseEventOptions { InterestGroup = (byte)view.Group };
                break;

            case PhotonTargets.AllBuffered:
                options = new RaiseEventOptions { CachingOption = EventCaching.AddToRoomCache };
                executeLocal = true;
                break;

            case PhotonTargets.OthersBuffered:
                options = new RaiseEventOptions { CachingOption = EventCaching.AddToRoomCache };
                break;

            case PhotonTargets.MasterClient:
                if (mLocalActor.IsMasterClient)
                {
                    ExecuteRPC(customEventContent, this.mLocalActor);
                    return;
                }
                options = new RaiseEventOptions { Receivers = ReceiverGroup.MasterClient };
                break;

            case PhotonTargets.AllViaServer:
                options = new RaiseEventOptions { InterestGroup = (byte)view.Group, Receivers = ReceiverGroup.All };
                break;

            case PhotonTargets.AllBufferedViaServer:
                options = new RaiseEventOptions { Receivers = ReceiverGroup.All, InterestGroup = (byte)view.Group, CachingOption = EventCaching.AddToRoomCache };
                break;

            case PhotonTargets.AnarchyUsers:
                {
                    int[] ids = PhotonPlayer.GetAnarchyUsersID();
                    if (ids.Length <= 0)
                    {
                        ExecuteRPC(customEventContent, mLocalActor);
                        return;
                    }
                    options = new RaiseEventOptions() { TargetActors = ids };
                    executeLocal = true;
                }
                break;

            case PhotonTargets.AnarchyUsersOthers:
                {
                    int[] ids = PhotonPlayer.GetAnarchyUsersID();
                    if (ids.Length <= 0)
                    {
                        return;
                    }
                    options = new RaiseEventOptions() { TargetActors = ids };
                }
                break;

            case PhotonTargets.NotAnarchy:
                {
                    int[] ids = PhotonPlayer.GetNotAnarchyUsersID();
                    if (ids.Length <= 0)
                    {
                        return;
                    }
                    options = new RaiseEventOptions() { TargetActors = ids };
                }
                break;

            case PhotonTargets.RCUsers:
                {
                    int[] ids = PhotonPlayer.GetRCUsersID();
                    if (ids.Length <= 0)
                    {
                        return;
                    }
                    options = new RaiseEventOptions() { TargetActors = ids };
                }
                break;

            case PhotonTargets.VanillaUsers:
                {
                    int[] ids = PhotonPlayer.GetVanillaUsersID();
                    if (ids.Length <= 0)
                    {
                        return;
                    }
                    options = new RaiseEventOptions() { TargetActors = ids };
                }
                break;

            case PhotonTargets.AnarchyUsersBuffered:
            case PhotonTargets.AnarchyUsersOthersBuffered:
                throw new NotImplementedException($"PhotonTargets enum element: \"{target.ToString()}\" is not implemented yet");

            default:
                Debug.Log($"Unsupported target enum \"{target.ToString()}\"");
                return;
        }

        OpRaiseEvent(200, customEventContent, true, options);
        if (executeLocal)
        {
            ExecuteRPC(customEventContent, mLocalActor);
        }
    }

    internal ExitGames.Client.Photon.Hashtable SendInstantiate(string prefabName, Vector3 position, Quaternion rotation, int group, int[] viewIDs, object[] data, bool isGlobalObject)
    {
        int num = viewIDs[0];
        ExitGames.Client.Photon.Hashtable customEventContent = new ExitGames.Client.Photon.Hashtable();
        customEventContent[(byte)0] = prefabName;
        if (position != Vector3.zero)
        {
            customEventContent[(byte)1] = position;
        }
        if (rotation != Quaternion.identity)
        {
            customEventContent[(byte)2] = rotation;
        }
        if (group != 0)
        {
            customEventContent[(byte)3] = group;
        }
        if (viewIDs.Length > 1)
        {
            customEventContent[(byte)4] = viewIDs;
        }
        if (data != null)
        {
            customEventContent[(byte)5] = data;
        }
        if (this.currentLevelPrefix > 0)
        {
            customEventContent[(byte)8] = this.currentLevelPrefix;
        }
        customEventContent[(byte)6] = base.ServerTimeInMilliSeconds;
        customEventContent[(byte)7] = num;
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions
        {
            CachingOption = !isGlobalObject ? EventCaching.AddToRoomCache : EventCaching.AddToRoomCacheGlobal
        };
        this.OpRaiseEvent(0xca, customEventContent, true, raiseEventOptions);
        return customEventContent;
    }

    public static void SendMonoMessage(PhotonNetworkingMessage methodString, object[] parameters)
    {
        if (!OnNetworkMessage.ContainsKey(methodString))
        {
            return;
        }

        AOTEvent eventMessage = OnNetworkMessage[methodString];
        eventMessage(parameters.Length == 0 ? AOTEventArgs.Empty : new AOTEventArgs(parameters));
    }

    public void ChangeLocalID(int newID)
    {
        if (this.mLocalActor == null)
        {
            Debug.LogWarning(string.Format("Local actor is null or not in mActors! mLocalActor: {0} mActors==null: {1} newID: {2}", this.mLocalActor, mActors == null, newID));
        }
        if (mActors.ContainsKey(this.mLocalActor.ID))
        {
            mActors.Remove(this.mLocalActor.ID);
        }
        this.mLocalActor.InternalChangeLocalID(newID);
        mActors[this.mLocalActor.ID] = this.mLocalActor;
        this.RebuildPlayerListCopies();
    }

    public void CleanRpcBufferIfMine(PhotonView view)
    {
        if ((view.ownerId != this.mLocalActor.ID) && !this.mLocalActor.IsMasterClient)
        {
            Debug.LogError(string.Concat(new object[] { "Cannot remove cached RPCs on a PhotonView thats not ours! ", view.owner, " scene: ", view.isSceneView }));
        }
        else
        {
            this.OpCleanRpcBuffer(view);
        }
    }

    public bool Connect(string serverAddress, ServerConnection type)
    {
        if (PhotonHandler.AppQuits)
        {
            Debug.LogWarning("Ignoring Connect() because app gets closed. If this is an error, check PhotonHandler.AppQuits.");
            return false;
        }
        if (PhotonNetwork.connectionStateDetailed == global::PeerState.Disconnecting)
        {
            Debug.LogError("Connect() failed. Can't connect while disconnecting (still). Current state: " + PhotonNetwork.connectionStateDetailed);
            return false;
        }
        bool flag = base.Connect(serverAddress, string.Empty);
        if (flag)
        {
            switch (type)
            {
                case ServerConnection.MasterServer:
                    this.State = global::PeerState.ConnectingToMasterserver;
                    return flag;

                case ServerConnection.GameServer:
                    this.State = global::PeerState.ConnectingToGameserver;
                    return flag;

                case ServerConnection.NameServer:
                    this.State = global::PeerState.ConnectingToNameServer;
                    return flag;
            }
        }
        return flag;
    }

    public override bool Connect(string serverAddress, string applicationName)
    {
        Debug.LogError("Avoid using this directly. Thanks.");
        return false;
    }

    public bool ConnectToNameServer()
    {
        if (PhotonHandler.AppQuits)
        {
            Debug.LogWarning("Ignoring Connect() because app gets closed. If this is an error, check PhotonHandler.AppQuits.");
            return false;
        }
        this.IsUsingNameServer = true;
        this.CloudRegion = CloudRegionCode.none;
        if (this.State != global::PeerState.ConnectedToNameServer)
        {
            string nameServerAddress = this.NameServerAddress;
            if (!nameServerAddress.Contains(":"))
            {
                int num = 0;
                ProtocolToNameServerPort.TryGetValue(base.UsedProtocol, out num);
                nameServerAddress = string.Format("{0}:{1}", nameServerAddress, num);
                Debug.Log(string.Concat(new object[] { "Server to connect to: ", nameServerAddress, " settings protocol: ", PhotonNetwork.PhotonServerSettings.Protocol }));
            }
            if (!base.Connect(nameServerAddress, "ns"))
            {
                return false;
            }
            this.State = global::PeerState.ConnectingToNameServer;
        }
        return true;
    }

    public bool ConnectToRegionMaster(CloudRegionCode region)
    {
        if (PhotonHandler.AppQuits)
        {
            Debug.LogWarning("Ignoring Connect() because app gets closed. If this is an error, check PhotonHandler.AppQuits.");
            return false;
        }
        this.IsUsingNameServer = true;
        this.CloudRegion = region;
        if (this.State == global::PeerState.ConnectedToNameServer)
        {
            return this.OpAuthenticate(this.mAppId, this.mAppVersionPun, this.PlayerName, this.CustomAuthenticationValues, region.ToString());
        }
        string nameServerAddress = this.NameServerAddress;
        if (!nameServerAddress.Contains(":"))
        {
            int num = 0;
            ProtocolToNameServerPort.TryGetValue(base.UsedProtocol, out num);
            nameServerAddress = string.Format("{0}:{1}", nameServerAddress, num);
        }
        if (!base.Connect(nameServerAddress, "ns"))
        {
            return false;
        }
        this.State = global::PeerState.ConnectingToNameServer;
        return true;
    }

    public void DebugReturn(DebugLevel level, string message)
    {
        this.externalListener.DebugReturn(level, message);
    }

    public void DestroyAll(bool localOnly)
    {
        if (!localOnly)
        {
            this.OpRemoveCompleteCache();
            this.SendDestroyOfAll();
        }
        this.LocalCleanupAnythingInstantiated(true);
    }

    public void DestroyPlayerObjects(int playerId, bool localOnly)
    {
        if (playerId <= 0)
        {
            Debug.LogError("Failed to Destroy objects of playerId: " + playerId);
        }
        else
        {
            if (!localOnly)
            {
                this.OpRemoveFromServerInstantiationsOfPlayer(playerId);
                this.OpCleanRpcBuffer(playerId);
                this.SendDestroyOfPlayer(playerId);
            }
            Queue<GameObject> queue = new Queue<GameObject>();
            int num = playerId * PhotonNetwork.MAX_VIEW_IDS;
            int num2 = num + PhotonNetwork.MAX_VIEW_IDS;
            foreach (KeyValuePair<int, GameObject> pair in this.instantiatedObjects)
            {
                if ((pair.Key > num) && (pair.Key < num2))
                {
                    queue.Enqueue(pair.Value);
                }
            }
            foreach (GameObject obj2 in queue)
            {
                this.RemoveInstantiatedGO(obj2, true);
            }
        }
    }

    public override void Disconnect()
    {
        if (base.PeerState == PeerStateValue.Disconnected)
        {
            if (!PhotonHandler.AppQuits)
            {
                Debug.LogWarning(string.Format("Can't execute Disconnect() while not connected. Nothing changed. State: {0}", this.State));
            }
        }
        else
        {
            this.State = global::PeerState.Disconnecting;
            base.Disconnect();
        }
    }

    public void ExecuteRPC(ExitGames.Client.Photon.Hashtable rpcData, PhotonPlayer sender)
    {
        if ((rpcData == null) || !rpcData.ContainsKey((byte)0))
        {
            Debug.LogError("Malformed RPC; this should never occur. Content: " + SupportClass.DictionaryToString(rpcData));
        }
        else
        {
            string str;
            int viewID = (int)rpcData[(byte)0];
            int num2 = 0;
            if (rpcData.ContainsKey((byte)1))
            {
                num2 = (short)rpcData[(byte)1];
            }
            if (rpcData.ContainsKey((byte)5))
            {
                int num3 = (byte)rpcData[(byte)5];
                if (num3 > (PhotonNetwork.PhotonServerSettings.RpcList.Count - 1))
                {
                    Debug.LogError("Could not find RPC with index: " + num3 + ". Going to ignore! Check PhotonServerSettings.RpcList");
                    return;
                }
                str = PhotonNetwork.PhotonServerSettings.RpcList[num3];
            }
            else
            {
                str = (string)rpcData[(byte)3];
            }
            object[] parameters = null;
            if (rpcData.ContainsKey((byte)4))
            {
                parameters = (object[])rpcData[(byte)4];
            }
            if (parameters == null)
            {
                parameters = new object[0];
            }
            PhotonView photonView = this.GetPhotonView(viewID);
            if (photonView == null)
            {
                int num4 = viewID / PhotonNetwork.MAX_VIEW_IDS;
                bool flag = num4 == this.mLocalActor.ID;
                bool flag2 = num4 == sender.ID;
                if (flag)
                {
                    Debug.LogWarning(string.Concat(new object[] { "Received RPC \"", str, "\" for viewID ", viewID, " but this PhotonView does not exist! View was/is ours.", !flag2 ? " Remote called." : " Owner called." }));
                }
                else
                {
                    Debug.LogError(string.Concat(new object[] { "Received RPC \"", str, "\" for viewID ", viewID, " but this PhotonView does not exist! Was remote PV.", !flag2 ? " Remote called." : " Owner called." }));
                }
            }
            else if (photonView.prefix != num2)
            {
                Debug.LogError(string.Concat(new object[] { "Received RPC \"", str, "\" on viewID ", viewID, " with a prefix of ", num2, ", our prefix is ", photonView.prefix, ". The RPC has been ignored." }));
            }
            else if (str == string.Empty)
            {
                Debug.LogError("Malformed RPC; this should never occur. Content: " + SupportClass.DictionaryToString(rpcData));
            }
            else
            {
                if (PhotonNetwork.logLevel >= PhotonLogLevel.Full)
                {
                    Debug.Log("Received RPC: " + str);
                }
                if ((photonView.Group == 0) || this.allowedReceivingGroups.Contains(photonView.Group))
                {
                    System.Type[] callParameterTypes = new System.Type[0];
                    if (parameters.Length > 0)
                    {
                        callParameterTypes = new System.Type[parameters.Length];
                        int index = 0;
                        for (int i = 0; i < parameters.Length; i++)
                        {
                            object obj2 = parameters[i];
                            if (obj2 == null)
                            {
                                callParameterTypes[index] = null;
                            }
                            else
                            {
                                callParameterTypes[index] = obj2.GetType();
                            }
                            index++;
                        }
                    }
                    int num7 = 0;
                    int num8 = 0;
                    foreach (MonoBehaviour behaviour in photonView.GetComponents<MonoBehaviour>())
                    {
                        if (behaviour == null)
                        {
                            Debug.LogError("ERROR You have missing MonoBehaviours on your gameobjects!");
                        }
                        else
                        {
                            System.Type key = behaviour.GetType();
                            List<MethodInfo> list = null;
                            if (this.monoRPCMethodsCache.ContainsKey(key))
                            {
                                list = this.monoRPCMethodsCache[key];
                            }
                            if (list == null)
                            {
                                List<MethodInfo> methods = SupportClass.GetMethods(key, typeof(UnityEngine.RPC));
                                this.monoRPCMethodsCache[key] = methods;
                                list = methods;
                            }
                            if (list != null)
                            {
                                for (int j = 0; j < list.Count; j++)
                                {
                                    MethodInfo info = list[j];
                                    if (info.Name == str)
                                    {
                                        num8++;
                                        ParameterInfo[] methodParameters = info.GetParameters();
                                        if (methodParameters.Length == callParameterTypes.Length)
                                        {
                                            if (this.CheckTypeMatch(methodParameters, callParameterTypes))
                                            {
                                                num7++;
                                                object obj3 = info.Invoke(behaviour, parameters);
                                                if (info.ReturnType == typeof(IEnumerator))
                                                {
                                                    behaviour.StartCoroutine((IEnumerator)obj3);
                                                }
                                            }
                                        }
                                        else if ((methodParameters.Length - 1) == callParameterTypes.Length)
                                        {
                                            if (this.CheckTypeMatch(methodParameters, callParameterTypes) && (methodParameters[methodParameters.Length - 1].ParameterType == typeof(PhotonMessageInfo)))
                                            {
                                                num7++;
                                                int timestamp = (int)rpcData[(byte)2];
                                                object[] array = new object[parameters.Length + 1];
                                                parameters.CopyTo(array, 0);
                                                array[array.Length - 1] = new PhotonMessageInfo(sender, timestamp, photonView);
                                                object obj4 = info.Invoke(behaviour, array);
                                                if (info.ReturnType == typeof(IEnumerator))
                                                {
                                                    behaviour.StartCoroutine((IEnumerator)obj4);
                                                }
                                            }
                                        }
                                        else if ((methodParameters.Length == 1) && methodParameters[0].ParameterType.IsArray)
                                        {
                                            num7++;
                                            object[] objArray5 = new object[] { parameters };
                                            object obj5 = info.Invoke(behaviour, objArray5);
                                            if (info.ReturnType == typeof(IEnumerator))
                                            {
                                                behaviour.StartCoroutine((IEnumerator)obj5);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    if (num7 != 1)
                    {
                        string str2 = string.Empty;
                        for (int k = 0; k < callParameterTypes.Length; k++)
                        {
                            System.Type type2 = callParameterTypes[k];
                            if (str2 != string.Empty)
                            {
                                str2 = str2 + ", ";
                            }
                            if (type2 == null)
                            {
                                str2 = str2 + "null";
                            }
                            else
                            {
                                str2 = str2 + type2.Name;
                            }
                        }
                        if (num7 == 0)
                        {
                            if (num8 == 0)
                            {
                                Debug.LogError(string.Concat(new object[] { "PhotonView with ID ", viewID, " has no method \"", str, "\" marked with the [RPC](C#) or @RPC(JS) property! Args: ", str2 }));
                            }
                            else
                            {
                                Debug.LogError(string.Concat(new object[] { "PhotonView with ID ", viewID, " has no method \"", str, "\" that takes ", callParameterTypes.Length, " argument(s): ", str2 }));
                            }
                        }
                        else
                        {
                            Debug.LogError(string.Concat(new object[] { "PhotonView with ID ", viewID, " has ", num7, " methods \"", str, "\" that takes ", callParameterTypes.Length, " argument(s): ", str2, ". Should be just one?" }));
                        }
                    }
                }
            }
        }
    }

    public object[] FetchInstantiationData(int instantiationId)
    {
        object[] objArray = null;
        if (instantiationId == 0)
        {
            return null;
        }
        this.tempInstantiationData.TryGetValue(instantiationId, out objArray);
        return objArray;
    }

    public int GetInstantiatedObjectsId(GameObject go)
    {
        int num = -1;
        if (go == null)
        {
            Debug.LogError("GetInstantiatedObjectsId() for GO == null.");
            return num;
        }
        PhotonView[] photonViewsInChildren = go.GetPhotonViewsInChildren();
        if (((photonViewsInChildren != null) && (photonViewsInChildren.Length > 0)) && (photonViewsInChildren[0] != null))
        {
            return photonViewsInChildren[0].instantiationId;
        }
        if (PhotonNetwork.logLevel >= PhotonLogLevel.Informational)
        {
            Debug.Log("GetInstantiatedObjectsId failed for GO: " + go);
        }
        return num;
    }

    public PhotonView GetPhotonView(int viewID)
    {
        PhotonView view = null;
        photonViewList.TryGetValue(viewID, out view);
        if (view == null)
        {
            PhotonView[] viewArray = UnityEngine.Object.FindObjectsOfType(typeof(PhotonView)) as PhotonView[];
            foreach (PhotonView view2 in viewArray)
            {
                if (view2.viewID == viewID)
                {
                    if (view2.didAwake)
                    {
                        Debug.LogWarning("Had to lookup view that wasn't in dict: " + view2);
                    }
                    return view2;
                }
            }
        }
        return view;
    }

    public bool GetRegions()
    {
        if (this.server != ServerConnection.NameServer)
        {
            return false;
        }
        bool flag = this.OpGetRegions(this.mAppId);
        if (flag)
        {
            this.AvailableRegions = null;
        }
        return flag;
    }

    public void LocalCleanPhotonView(PhotonView view)
    {
        view.destroyedByPhotonNetworkOrQuit = true;
        photonViewList.Remove(view.viewID);
    }

    public void NewSceneLoaded()
    {
        if (this.loadingLevelAndPausedNetwork)
        {
            this.loadingLevelAndPausedNetwork = false;
            PhotonNetwork.isMessageQueueRunning = true;
        }
        List<int> list = new List<int>();
        foreach (KeyValuePair<int, PhotonView> pair in photonViewList)
        {
            if (pair.Value == null)
            {
                list.Add(pair.Key);
            }
        }
        for (int i = 0; i < list.Count; i++)
        {
            int key = list[i];
            photonViewList.Remove(key);
        }
        if ((list.Count > 0) && (PhotonNetwork.logLevel >= PhotonLogLevel.Informational))
        {
            Debug.Log("New level loaded. Removed " + list.Count + " scene view IDs from last level.");
        }
    }

    public void OnEvent(EventData photonEvent)
    {
        int key = -1;
        PhotonPlayer sender = null;
        if (photonEvent.Parameters.ContainsKey(0xfe))
        {
            key = (int)photonEvent[0xfe];
            if (mActors.ContainsKey(key))
            {
                sender = mActors[key];
                if (sender.RCIgnored && photonEvent.Code != 254)
                {
                    return;
                }
                sender.EventSpam.Count(photonEvent.Code);
            }
        }


        INetworkEvent curr = NetworkManager.GetEvent(photonEvent.Code);
        if (curr == null)
        {
            if (sender != null)
            {
                Antis.AntisManager.Response(sender.ID, true, $"Unknown event {photonEvent.Code}");
            }
            return;
        }
        if (!curr.CheckData(photonEvent, sender, out string reason))
        {
            if (PhotonNetwork.IsMasterClient)
            {
                Antis.AntisManager.Response(sender.ID, true, $"{Log.GetString("eventName", curr.Code.ToString())} {reason}");
            }
            else
            {
                Log.AddLineRaw($"{Log.GetString("eventName", curr.Code.ToString())} {reason} (ID: {sender.ID})", MsgType.Error);
            }
            return;
        }
        if (!curr.Handle())
        {
            curr.OnFailedHandle();
        }
    }

    public void OnOperationResponse(OperationResponse operationResponse)
    {
        if (PhotonNetwork.networkingPeer.State == global::PeerState.Disconnecting)
        {
            if (PhotonNetwork.logLevel >= PhotonLogLevel.Informational)
            {
                Debug.Log("OperationResponse ignored while disconnecting. Code: " + operationResponse.OperationCode);
            }
            return;
        }
        if (operationResponse.ReturnCode == 0)
        {
            if (PhotonNetwork.logLevel >= PhotonLogLevel.Informational)
            {
                Debug.Log(operationResponse.ToString());
            }
        }
        else if (operationResponse.ReturnCode == -3)
        {
            Debug.LogError("Operation " + operationResponse.OperationCode + " could not be executed (yet). Wait for state JoinedLobby or ConnectedToMaster and their callbacks before calling operations. WebRPCs need a server-side configuration. Enum OperationCode helps identify the operation.");
        }
        else if (operationResponse.ReturnCode == 0x7ff0)
        {
            Debug.LogError(string.Concat(new object[] { "Operation ", operationResponse.OperationCode, " failed in a server-side plugin. Check the configuration in the Dashboard. Message from server-plugin: ", operationResponse.DebugMessage }));
        }
        else if (PhotonNetwork.logLevel >= PhotonLogLevel.Informational)
        {
            Debug.LogError(string.Concat(new object[] { "Operation failed: ", operationResponse.ToStringFull(), " Server: ", this.server }));
        }
        if (operationResponse.Parameters.ContainsKey(0xdd))
        {
            if (this.CustomAuthenticationValues == null)
            {
                this.CustomAuthenticationValues = new AuthenticationValues();
            }
            this.CustomAuthenticationValues.Secret = operationResponse[0xdd] as string;
        }
        byte operationCode = operationResponse.OperationCode;
        switch (operationCode)
        {
            case OperationCode.WebRpc:
                {
                    object[] parameters = new object[] { operationResponse };
                    SendMonoMessage(PhotonNetworkingMessage.OnWebRpcResponse, parameters);
                    goto Label_0949;
                }
            case OperationCode.GetRegions:
                {
                    if (operationResponse.ReturnCode != 0x7fff)
                    {
                        string[] strArray = operationResponse[210] as string[];
                        string[] strArray2 = operationResponse[230] as string[];
                        if (((strArray == null) || (strArray2 == null)) || (strArray.Length != strArray2.Length))
                        {
                            Debug.LogError("The region arrays from Name Server are not ok. Must be non-null and same length.");
                        }
                        else
                        {
                            this.AvailableRegions = new List<Region>(strArray.Length);
                            for (int i = 0; i < strArray.Length; i++)
                            {
                                string str = strArray[i];
                                if (!string.IsNullOrEmpty(str))
                                {
                                    CloudRegionCode code = Region.Parse(str.ToLower());
                                    Region item = new Region
                                    {
                                        Code = code,
                                        HostAndPort = strArray2[i]
                                    };
                                    this.AvailableRegions.Add(item);
                                }
                            }
                        }
                        goto Label_0949;
                    }
                    Debug.LogError(string.Format("The appId this client sent is unknown on the server (Cloud). Check settings. If using the Cloud, check account.", new object[0]));
                    object[] objArray8 = new object[] { DisconnectCause.InvalidAuthentication };
                    SendMonoMessage(PhotonNetworkingMessage.OnFailedToConnectToPhoton, objArray8);
                    this.State = global::PeerState.Disconnecting;
                    this.Disconnect();
                    return;
                }
            case OperationCode.FindFriends:
                {
                    bool[] flagArray = operationResponse[1] as bool[];
                    string[] strArray3 = operationResponse[2] as string[];
                    if (((flagArray == null) || (strArray3 == null)) || ((this.friendListRequested == null) || (flagArray.Length != this.friendListRequested.Length)))
                    {
                        Debug.LogError("FindFriends failed to apply the result, as a required value wasn't provided or the friend list length differed from result.");
                    }
                    else
                    {
                        List<FriendInfo> list = new List<FriendInfo>(this.friendListRequested.Length);
                        for (int j = 0; j < this.friendListRequested.Length; j++)
                        {
                            FriendInfo info = new FriendInfo
                            {
                                Name = this.friendListRequested[j],
                                Room = strArray3[j],
                                IsOnline = flagArray[j]
                            };
                            list.Insert(j, info);
                        }
                        PhotonNetwork.Friends = list;
                    }
                    this.friendListRequested = null;
                    this.isFetchingFriends = false;
                    this.friendListTimestamp = Environment.TickCount;
                    if (this.friendListTimestamp == 0)
                    {
                        this.friendListTimestamp = 1;
                    }
                    SendMonoMessage(PhotonNetworkingMessage.OnUpdatedFriendList, new object[0]);
                    goto Label_0949;
                }


            case OperationCode.JoinRandomGame:
                if (operationResponse.ReturnCode == 0)
                {
                    string str3 = (string)operationResponse[0xff];
                    this.mRoomToGetInto.Name = str3;
                    this.mGameserver = (string)operationResponse[230];
                    this.DisconnectToReconnect();
                }
                else
                {
                    if (operationResponse.ReturnCode != 0x7ff8)
                    {
                        if (PhotonNetwork.logLevel >= PhotonLogLevel.Informational)
                        {
                            Debug.LogWarning(string.Format("JoinRandom failed: {0}.", operationResponse.ToStringFull()));
                        }
                    }
                    else if (PhotonNetwork.logLevel >= PhotonLogLevel.Full)
                    {
                        Debug.Log("JoinRandom failed: No open game. Calling: OnPhotonRandomJoinFailed() and staying on master server.");
                    }
                    SendMonoMessage(PhotonNetworkingMessage.OnPhotonRandomJoinFailed, new object[0]);
                }
                goto Label_0949;

            case OperationCode.JoinGame:
                if (this.server == ServerConnection.GameServer)
                {
                    this.GameEnteredOnGameServer(operationResponse);
                }
                else if (operationResponse.ReturnCode == 0)
                {
                    this.mGameserver = (string)operationResponse[230];
                    this.DisconnectToReconnect();
                }
                else
                {
                    if (PhotonNetwork.logLevel >= PhotonLogLevel.Informational)
                    {
                        Debug.Log(string.Format("JoinRoom failed (room maybe closed by now). Client stays on masterserver: {0}. State: {1}", operationResponse.ToStringFull(), this.State));
                    }
                    SendMonoMessage(PhotonNetworkingMessage.OnPhotonJoinRoomFailed, new object[0]);
                }
                goto Label_0949;

            case OperationCode.CreateGame:
                if (this.server != ServerConnection.GameServer)
                {
                    if (operationResponse.ReturnCode != 0)
                    {
                        if (PhotonNetwork.logLevel >= PhotonLogLevel.Informational)
                        {
                            Debug.LogWarning(string.Format("CreateRoom failed, client stays on masterserver: {0}.", operationResponse.ToStringFull()));
                        }
                        SendMonoMessage(PhotonNetworkingMessage.OnPhotonCreateRoomFailed, new object[0]);
                    }
                    else
                    {
                        string str2 = (string)operationResponse[0xff];
                        if (!string.IsNullOrEmpty(str2))
                        {
                            this.mRoomToGetInto.Name = str2;
                        }
                        this.mGameserver = (string)operationResponse[230];
                        this.DisconnectToReconnect();
                    }
                }
                else
                {
                    this.GameEnteredOnGameServer(operationResponse);
                }
                goto Label_0949;

            case OperationCode.LeaveLobby:
                this.State = global::PeerState.Authenticated;
                this.LeftLobbyCleanup();
                goto Label_0949;

            case OperationCode.JoinLobby:
                this.State = global::PeerState.JoinedLobby;
                this.insideLobby = true;
                SendMonoMessage(PhotonNetworkingMessage.OnJoinedLobby, new object[0]);
                goto Label_0949;

            case OperationCode.Authenticate:
                if (operationResponse.ReturnCode == 0)
                {
                    if (this.server == ServerConnection.NameServer)
                    {
                        this.MasterServerAddress = operationResponse[ParameterCode.Address] as string;
                        this.DisconnectToReconnect();
                    }
                    else if (this.server == ServerConnection.MasterServer)
                    {
                        if (PhotonNetwork.autoJoinLobby)
                        {
                            this.State = global::PeerState.Authenticated;
                            this.OpJoinLobby(this.lobby);
                        }
                        else
                        {
                            this.State = global::PeerState.ConnectedToMaster;
                            SendMonoMessage(PhotonNetworkingMessage.OnConnectedToMaster, new object[0]);
                        }
                    }
                    else if (this.server == ServerConnection.GameServer)
                    {
                        this.State = global::PeerState.Joining;
                        if (((this.mLastJoinType == JoinType.JoinGame) || (this.mLastJoinType == JoinType.JoinRandomGame)) || (this.mLastJoinType == JoinType.JoinOrCreateOnDemand))
                        {
                            this.OpJoinRoom(this.mRoomToGetInto.Name, this.mRoomOptionsForCreate, this.mRoomToEnterLobby, this.mLastJoinType == JoinType.JoinOrCreateOnDemand);
                        }
                        else if (this.mLastJoinType == JoinType.CreateGame)
                        {
                            this.OpCreateGame(this.mRoomToGetInto.Name, this.mRoomOptionsForCreate, this.mRoomToEnterLobby);
                        }
                    }
                    goto Label_0949;
                }
                if (operationResponse.ReturnCode != -2)
                {
                    if (operationResponse.ReturnCode == 0x7fff)
                    {
                        Debug.LogError(string.Format("The appId this client sent is unknown on the server (Cloud). Check settings. If using the Cloud, check account.", new object[0]));
                        object[] objArray3 = new object[] { DisconnectCause.InvalidAuthentication };
                        SendMonoMessage(PhotonNetworkingMessage.OnFailedToConnectToPhoton, objArray3);
                    }
                    else if (operationResponse.ReturnCode == 0x7ff3)
                    {
                        Debug.LogError(string.Format("Custom Authentication failed (either due to user-input or configuration or AuthParameter string format). Calling: OnCustomAuthenticationFailed()", new object[0]));
                        object[] objArray4 = new object[] { operationResponse.DebugMessage };
                        SendMonoMessage(PhotonNetworkingMessage.OnCustomAuthenticationFailed, objArray4);
                    }
                    else
                    {
                        Debug.LogError(string.Format("Authentication failed: '{0}' Code: {1}", operationResponse.DebugMessage, operationResponse.ReturnCode));
                    }
                }
                else
                {
                    Debug.LogError(string.Format("If you host Photon yourself, make sure to start the 'Instance LoadBalancing' " + base.ServerAddress, new object[0]));
                }
                break;

            default:
                switch (operationCode)
                {
                    case OperationCode.GetProperties:
                        {
                            ExitGames.Client.Photon.Hashtable pActorProperties = (ExitGames.Client.Photon.Hashtable)operationResponse[0xf9];
                            ExitGames.Client.Photon.Hashtable gameProperties = (ExitGames.Client.Photon.Hashtable)operationResponse[0xf8];
                            if (needCheck)
                            {
                                if (pActorProperties != null)
                                {
                                    var str = new System.Text.StringBuilder();
                                    foreach (var idk in pActorProperties)
                                    {
                                        if (idk.Value is ExitGames.Client.Photon.Hashtable hash)
                                        {
                                            str.Append("ID: " + idk.Key + "\n");
                                            foreach (var k in hash)
                                            {
                                                str.Append(k.Key + ": " + (k.Value == null ? "null" : k.Value.ToString()) + "\n");
                                            }
                                            str.AppendLine();
                                        }
                                    }
                                    new Anarchy.IO.LogFile("LogName").WriteLine(str.ToString());
                                    needCheck = false;
                                }
                            }
                            PropertiesChecker.CheckRoomProperties(gameProperties, null);
                            PropertiesChecker.CheckPlayersProperties(pActorProperties, 0, null);
                            goto Label_0949;
                        }
                    case OperationCode.SetProperties:
                    case OperationCode.RaiseEvent:
                        goto Label_0949;

                    case OperationCode.Leave:
                        this.DisconnectToReconnect();
                        goto Label_0949;

                    default:
                        Debug.LogWarning(string.Format("OperationResponse unhandled: {0}", operationResponse.ToString()));
                        goto Label_0949;
                }
        }
        this.State = global::PeerState.Disconnecting;
        this.Disconnect();
        if (operationResponse.ReturnCode == 0x7ff5)
        {
            if (PhotonNetwork.logLevel >= PhotonLogLevel.Informational)
            {
                Debug.LogWarning(string.Format("Currently, the limit of users is reached for this title. Try again later. Disconnecting", new object[0]));
            }
            SendMonoMessage(PhotonNetworkingMessage.OnPhotonMaxCccuReached, new object[0]);
            object[] objArray5 = new object[] { DisconnectCause.MaxCcuReached };
            SendMonoMessage(PhotonNetworkingMessage.OnConnectionFail, objArray5);
        }
        else if (operationResponse.ReturnCode == 0x7ff4)
        {
            if (PhotonNetwork.logLevel >= PhotonLogLevel.Informational)
            {
                Debug.LogError(string.Format("The used master server address is not available with the subscription currently used. Got to Photon Cloud Dashboard or change URL. Disconnecting.", new object[0]));
            }
            object[] objArray6 = new object[] { DisconnectCause.InvalidRegion };
            SendMonoMessage(PhotonNetworkingMessage.OnConnectionFail, objArray6);
        }
        else if (operationResponse.ReturnCode == 0x7ff1)
        {
            if (PhotonNetwork.logLevel >= PhotonLogLevel.Informational)
            {
                Debug.LogError(string.Format("The authentication ticket expired. You need to connect (and authenticate) again. Disconnecting.", new object[0]));
            }
            object[] objArray7 = new object[] { DisconnectCause.AuthenticationTicketExpired };
            SendMonoMessage(PhotonNetworkingMessage.OnConnectionFail, objArray7);
        }
        Label_0949:
        this.externalListener.OnOperationResponse(operationResponse);
    }

    public static bool needCheck;

    public void OnStatusChanged(StatusCode statusCode)
    {
        DisconnectCause cause;
        if (PhotonNetwork.logLevel >= PhotonLogLevel.Informational)
        {
            Debug.Log(string.Format("OnStatusChanged: {0}", statusCode.ToString()));
        }
        switch (statusCode)
        {
            case StatusCode.SecurityExceptionOnConnect:
            case StatusCode.ExceptionOnConnect:
                {
                    this.State = global::PeerState.PeerCreated;
                    if (this.CustomAuthenticationValues != null)
                    {
                        this.CustomAuthenticationValues.Secret = null;
                    }
                    cause = (DisconnectCause)statusCode;
                    object[] parameters = new object[] { cause };
                    SendMonoMessage(PhotonNetworkingMessage.OnFailedToConnectToPhoton, parameters);
                    goto Label_055E;
                }
            case StatusCode.Connect:
                if (this.State == global::PeerState.ConnectingToNameServer)
                {
                    if (PhotonNetwork.logLevel >= PhotonLogLevel.Full)
                    {
                        Debug.Log("Connected to NameServer.");
                    }
                    this.server = ServerConnection.NameServer;
                    if (this.CustomAuthenticationValues != null)
                    {
                        this.CustomAuthenticationValues.Secret = null;
                    }
                }
                if (this.State == global::PeerState.ConnectingToGameserver)
                {
                    if (PhotonNetwork.logLevel >= PhotonLogLevel.Full)
                    {
                        Debug.Log("Connected to gameserver.");
                    }
                    this.server = ServerConnection.GameServer;
                    this.State = global::PeerState.ConnectedToGameserver;
                }
                if (this.State == global::PeerState.ConnectingToMasterserver)
                {
                    if (PhotonNetwork.logLevel >= PhotonLogLevel.Full)
                    {
                        Debug.Log("Connected to masterserver.");
                    }
                    this.server = ServerConnection.MasterServer;
                    this.State = global::PeerState.ConnectedToMaster;
                    if (this.IsInitialConnect)
                    {
                        this.IsInitialConnect = false;
                        SendMonoMessage(PhotonNetworkingMessage.OnConnectedToPhoton, new object[0]);
                    }
                }
                base.EstablishEncryption();
                if (this.IsAuthorizeSecretAvailable)
                {
                    this.didAuthenticate = this.OpAuthenticate(this.mAppId, this.mAppVersionPun, this.PlayerName, this.CustomAuthenticationValues, this.CloudRegion.ToString());
                    if (this.didAuthenticate)
                    {
                        this.State = global::PeerState.Authenticating;
                    }
                }
                goto Label_055E;

            case StatusCode.Disconnect:
                this.didAuthenticate = false;
                this.isFetchingFriends = false;
                if (this.server == ServerConnection.GameServer)
                {
                    this.LeftRoomCleanup();
                }
                if (this.server == ServerConnection.MasterServer)
                {
                    this.LeftLobbyCleanup();
                }
                if (this.State == global::PeerState.DisconnectingFromMasterserver)
                {
                    if (this.Connect(this.mGameserver, ServerConnection.GameServer))
                    {
                        this.State = global::PeerState.ConnectingToGameserver;
                    }
                }
                else if ((this.State == global::PeerState.DisconnectingFromGameserver) || (this.State == global::PeerState.DisconnectingFromNameServer))
                {
                    if (this.Connect(this.MasterServerAddress, ServerConnection.MasterServer))
                    {
                        this.State = global::PeerState.ConnectingToMasterserver;
                    }
                }
                else
                {
                    if (this.CustomAuthenticationValues != null)
                    {
                        this.CustomAuthenticationValues.Secret = null;
                    }
                    this.State = global::PeerState.PeerCreated;
                    SendMonoMessage(PhotonNetworkingMessage.OnDisconnectedFromPhoton, new object[0]);
                }
                goto Label_055E;

            case StatusCode.Exception:
                {
                    if (!this.IsInitialConnect)
                    {
                        this.State = global::PeerState.PeerCreated;
                        cause = (DisconnectCause)statusCode;
                        object[] objArray3 = new object[] { cause };
                        SendMonoMessage(PhotonNetworkingMessage.OnConnectionFail, objArray3);
                        break;
                    }
                    Debug.LogError("Exception while connecting to: " + base.ServerAddress + ". Check if the server is available.");
                    if ((base.ServerAddress == null) || base.ServerAddress.StartsWith("127.0.0.1"))
                    {
                        Debug.LogWarning("The server address is 127.0.0.1 (localhost): Make sure the server is running on this machine. Android and iOS emulators have their own localhost.");
                        if (base.ServerAddress == this.mGameserver)
                        {
                            Debug.LogWarning("This might be a misconfiguration in the game server config. You need to edit it to a (public) address.");
                        }
                    }
                    this.State = global::PeerState.PeerCreated;
                    cause = (DisconnectCause)statusCode;
                    object[] objArray2 = new object[] { cause };
                    SendMonoMessage(PhotonNetworkingMessage.OnFailedToConnectToPhoton, objArray2);
                    break;
                }
            case StatusCode.SendError:
                goto Label_055E;

            case StatusCode.ExceptionOnReceive:
            case StatusCode.TimeoutDisconnect:
            case StatusCode.DisconnectByServerTimeout:
            case StatusCode.DisconnectByServerUserLimit:
            case StatusCode.DisconnectByServerLogic:
                if (!this.IsInitialConnect)
                {
                    cause = (DisconnectCause)statusCode;
                    object[] objArray6 = new object[] { cause };
                    SendMonoMessage(PhotonNetworkingMessage.OnConnectionFail, objArray6);
                }
                else
                {
                    Debug.LogWarning(string.Concat(new object[] { statusCode, " while connecting to: ", base.ServerAddress, ". Check if the server is available." }));
                    cause = (DisconnectCause)statusCode;
                    object[] objArray5 = new object[] { cause };
                    SendMonoMessage(PhotonNetworkingMessage.OnFailedToConnectToPhoton, objArray5);
                }
                if (this.CustomAuthenticationValues != null)
                {
                    this.CustomAuthenticationValues.Secret = null;
                }
                this.Disconnect();
                goto Label_055E;

            case StatusCode.EncryptionEstablished:
                if (this.server == ServerConnection.NameServer)
                {
                    this.State = global::PeerState.ConnectedToNameServer;
                    if (!this.didAuthenticate && (this.CloudRegion == CloudRegionCode.none))
                    {
                        this.OpGetRegions(this.mAppId);
                    }
                }
                if (!this.didAuthenticate && (!this.IsUsingNameServer || (this.CloudRegion != CloudRegionCode.none)))
                {
                    this.didAuthenticate = this.OpAuthenticate(this.mAppId, this.mAppVersionPun, this.PlayerName, this.CustomAuthenticationValues, this.CloudRegion.ToString());
                    if (this.didAuthenticate)
                    {
                        this.State = global::PeerState.Authenticating;
                    }
                }
                goto Label_055E;

            case StatusCode.EncryptionFailedToEstablish:
                Debug.LogError("Encryption wasn't established: " + statusCode + ". Going to authenticate anyways.");
                this.OpAuthenticate(this.mAppId, this.mAppVersionPun, this.PlayerName, this.CustomAuthenticationValues, this.CloudRegion.ToString());
                goto Label_055E;

            default:
                Debug.LogError("Received unknown status code: " + statusCode);
                goto Label_055E;
        }
        this.Disconnect();
        Label_055E:
        this.externalListener.OnStatusChanged(statusCode);
    }

    public void OpCleanRpcBuffer(PhotonView view)
    {
        ExitGames.Client.Photon.Hashtable customEventContent = new ExitGames.Client.Photon.Hashtable();
        customEventContent[(byte)0] = view.viewID;
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions
        {
            CachingOption = EventCaching.RemoveFromRoomCache
        };
        this.OpRaiseEvent(200, customEventContent, true, raiseEventOptions);
    }

    public void OpCleanRpcBuffer(int actorNumber)
    {
        RaiseEventOptions options2 = new RaiseEventOptions
        {
            CachingOption = EventCaching.RemoveFromRoomCache
        };
        options2.TargetActors = new int[] { actorNumber };
        RaiseEventOptions raiseEventOptions = options2;
        this.OpRaiseEvent(200, null, true, raiseEventOptions);
    }

    public bool OpCreateGame(string roomName, RoomOptions roomOptions, TypedLobby typedLobby)
    {
        bool onGameServer = this.server == ServerConnection.GameServer;
        if (!onGameServer)
        {
            this.mRoomOptionsForCreate = roomOptions;
            this.mRoomToGetInto = new Room(roomName, roomOptions);
            if (typedLobby == null)
            {
            }
            this.mRoomToEnterLobby = !this.insideLobby ? null : this.lobby;
            NetworkManager.RejoinRoom = roomName;
        }
        this.mLastJoinType = JoinType.CreateGame;
        return base.OpCreateRoom(roomName, roomOptions, this.mRoomToEnterLobby, this.GetLocalActorProperties(), onGameServer);
    }

    public override bool OpFindFriends(string[] friendsToFind)
    {
        if (this.isFetchingFriends)
        {
            return false;
        }
        this.friendListRequested = friendsToFind;
        this.isFetchingFriends = true;
        return base.OpFindFriends(friendsToFind);
    }

    public override bool OpJoinRandomRoom(ExitGames.Client.Photon.Hashtable expectedCustomRoomProperties, byte expectedMaxPlayers, ExitGames.Client.Photon.Hashtable playerProperties, MatchmakingMode matchingType, TypedLobby typedLobby, string sqlLobbyFilter)
    {
        this.mRoomToGetInto = new Room(null, null);
        this.mRoomToEnterLobby = null;
        this.mLastJoinType = JoinType.JoinRandomGame;
        return base.OpJoinRandomRoom(expectedCustomRoomProperties, expectedMaxPlayers, playerProperties, matchingType, typedLobby, sqlLobbyFilter);
    }

    public bool OpJoinRoom(string roomName, RoomOptions roomOptions, TypedLobby typedLobby, bool createIfNotExists)
    {
        bool onGameServer = this.server == ServerConnection.GameServer;
        if (!onGameServer)
        {
            this.mRoomOptionsForCreate = roomOptions;
            this.mRoomToGetInto = new Room(roomName, roomOptions);
            this.mRoomToEnterLobby = null;
            if (createIfNotExists)
            {
                if (typedLobby == null)
                {
                }
                this.mRoomToEnterLobby = !this.insideLobby ? null : this.lobby;
            }
            NetworkManager.RejoinRoom = roomName;
        }
        this.mLastJoinType = !createIfNotExists ? JoinType.JoinGame : JoinType.JoinOrCreateOnDemand;
        return base.OpJoinRoom(roomName, roomOptions, this.mRoomToEnterLobby, createIfNotExists, this.GetLocalActorProperties(), onGameServer);
    }

    public virtual bool OpLeave()
    {
        if (this.State != global::PeerState.Joined)
        {
            Debug.LogWarning("Not sending leave operation. State is not 'Joined': " + this.State);
            return false;
        }
        return SendOperation(OperationCode.Leave, null, SendOptions.SendReliable);
    }

    public override bool OpRaiseEvent(byte eventCode, object customEventContent, bool sendReliable, RaiseEventOptions raiseEventOptions)
    {
        if (PhotonNetwork.offlineMode)
        {
            return false;
        }
        return base.OpRaiseEvent(eventCode, customEventContent, sendReliable, raiseEventOptions);
    }

    public void OpRemoveCompleteCache()
    {
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions
        {
            CachingOption = EventCaching.RemoveFromRoomCache,
            Receivers = ReceiverGroup.MasterClient
        };
        this.OpRaiseEvent(0, null, true, raiseEventOptions);
    }

    public void OpRemoveCompleteCacheOfPlayer(int actorNumber)
    {
        RaiseEventOptions options2 = new RaiseEventOptions
        {
            CachingOption = EventCaching.RemoveFromRoomCache
        };
        options2.TargetActors = new int[] { actorNumber };
        RaiseEventOptions raiseEventOptions = options2;
        this.OpRaiseEvent(0, null, true, raiseEventOptions);
    }

    public void RegisterPhotonView(PhotonView netView)
    {
        if (!Application.isPlaying)
        {
            photonViewList = new Dictionary<int, PhotonView>();
        }
        else if (netView.subId != 0)
        {
            if (photonViewList.ContainsKey(netView.viewID))
            {
                if (netView != photonViewList[netView.viewID])
                {
                    Debug.LogError(string.Format("PhotonView ID duplicate found: {0}. New: {1} old: {2}. Maybe one wasn't destroyed on scene load?! Check for 'DontDestroyOnLoad'. Destroying old entry, adding new.", netView.viewID, netView, photonViewList[netView.viewID]));
                }
                this.RemoveInstantiatedGO(photonViewList[netView.viewID].gameObject, true);
            }
            photonViewList.Add(netView.viewID, netView);
            if (PhotonNetwork.logLevel >= PhotonLogLevel.Full)
            {
                Debug.Log("Registered PhotonView: " + netView.viewID);
            }
        }
    }

    public void RemoveAllInstantiatedObjects()
    {
        GameObject[] array = new GameObject[this.instantiatedObjects.Count];
        this.instantiatedObjects.Values.CopyTo(array, 0);
        for (int i = 0; i < array.Length; i++)
        {
            GameObject go = array[i];
            if (go != null)
            {
                this.RemoveInstantiatedGO(go, false);
            }
        }
        if (this.instantiatedObjects.Count > 0)
        {
            Debug.LogError("RemoveAllInstantiatedObjects() this.instantiatedObjects.Count should be 0 by now.");
        }
        this.instantiatedObjects = new Dictionary<int, GameObject>();
    }

    public void RemoveInstantiatedGO(GameObject go, bool localOnly)
    {
        if (go == null)
        {
            Debug.LogError("Failed to 'network-remove' GameObject because it's null.");
        }
        else
        {
            PhotonView[] componentsInChildren = go.GetComponentsInChildren<PhotonView>();
            if ((componentsInChildren == null) || (componentsInChildren.Length <= 0))
            {
                Debug.LogError("Failed to 'network-remove' GameObject because has no PhotonView components: " + go);
            }
            else
            {
                PhotonView view = componentsInChildren[0];
                int ownerActorNr = view.OwnerActorNr;
                int instantiationId = view.instantiationId;
                if (!localOnly)
                {
                    if (!view.IsMine && (!this.mLocalActor.IsMasterClient || mActors.ContainsKey(ownerActorNr)))
                    {
                        Debug.LogError("Failed to 'network-remove' GameObject. Client is neither owner nor masterClient taking over for owner who left: " + view);
                        return;
                    }
                    if (instantiationId < 1)
                    {
                        Debug.LogError("Failed to 'network-remove' GameObject because it is missing a valid InstantiationId on view: " + view + ". Not Destroying GameObject or PhotonViews!");
                        return;
                    }
                }
                if (!localOnly)
                {
                    this.ServerCleanInstantiateAndDestroy(instantiationId, ownerActorNr);
                }
                this.instantiatedObjects.Remove(instantiationId);
                for (int i = componentsInChildren.Length - 1; i >= 0; i--)
                {
                    PhotonView view2 = componentsInChildren[i];
                    if (view2 != null)
                    {
                        if (view2.instantiationId >= 1)
                        {
                            this.LocalCleanPhotonView(view2);
                        }
                        if (!localOnly)
                        {
                            this.OpCleanRpcBuffer(view2);
                        }
                    }
                }
                if (PhotonNetwork.logLevel >= PhotonLogLevel.Full)
                {
                    Debug.Log("Network destroy Instantiated GO: " + go.name);
                }
                Optimization.Caching.Pool.Disable(go);
            }
        }
    }

    public void RemoveRPCsInGroup(int group)
    {
        foreach (KeyValuePair<int, PhotonView> pair in photonViewList)
        {
            PhotonView view = pair.Value;
            if (view.Group == group)
            {
                this.CleanRpcBufferIfMine(view);
            }
        }
    }

    public void RunViewUpdate()
    {
        if ((PhotonNetwork.connected && !PhotonNetwork.offlineMode) && ((mActors != null) && (mActors.Count > 1)))
        {
            Dictionary<int, ExitGames.Client.Photon.Hashtable> dictionary = new Dictionary<int, ExitGames.Client.Photon.Hashtable>();
            Dictionary<int, ExitGames.Client.Photon.Hashtable> dictionary2 = new Dictionary<int, ExitGames.Client.Photon.Hashtable>();
            foreach (KeyValuePair<int, PhotonView> pair in photonViewList)
            {
                PhotonView view = pair.Value;
                if (((((view.observed != null) && (view.synchronization != ViewSynchronization.Off)) && ((view.ownerId == this.mLocalActor.ID) || (view.isSceneView && (mMasterClient == this.mLocalActor)))) && view.gameObject.activeInHierarchy) && !this.blockSendingGroups.Contains(view.Group))
                {
                    ExitGames.Client.Photon.Hashtable hashtable = this.OnSerializeWrite(view);
                    if (hashtable != null)
                    {
                        if ((view.synchronization == ViewSynchronization.ReliableDeltaCompressed) || view.mixedModeIsReliable)
                        {
                            if (hashtable.ContainsKey((byte)1) || hashtable.ContainsKey((byte)2))
                            {
                                if (!dictionary.ContainsKey(view.Group))
                                {
                                    dictionary[view.Group] = new ExitGames.Client.Photon.Hashtable();
                                    dictionary[view.Group][(byte)0] = base.ServerTimeInMilliSeconds;
                                    if (this.currentLevelPrefix >= 0)
                                    {
                                        dictionary[view.Group][(byte)1] = this.currentLevelPrefix;
                                    }
                                }
                                ExitGames.Client.Photon.Hashtable hashtable2 = dictionary[view.Group];
                                hashtable2.Add((short)hashtable2.Count, hashtable);
                            }
                        }
                        else
                        {
                            if (!dictionary2.ContainsKey(view.Group))
                            {
                                dictionary2[view.Group] = new ExitGames.Client.Photon.Hashtable();
                                dictionary2[view.Group][(byte)0] = base.ServerTimeInMilliSeconds;
                                if (this.currentLevelPrefix >= 0)
                                {
                                    dictionary2[view.Group][(byte)1] = this.currentLevelPrefix;
                                }
                            }
                            ExitGames.Client.Photon.Hashtable hashtable3 = dictionary2[view.Group];
                            hashtable3.Add((short)hashtable3.Count, hashtable);
                        }
                    }
                }
            }
            RaiseEventOptions raiseEventOptions = new RaiseEventOptions();
            foreach (KeyValuePair<int, ExitGames.Client.Photon.Hashtable> pair2 in dictionary)
            {
                raiseEventOptions.InterestGroup = (byte)pair2.Key;
                this.OpRaiseEvent(0xce, pair2.Value, true, raiseEventOptions);
            }
            foreach (KeyValuePair<int, ExitGames.Client.Photon.Hashtable> pair3 in dictionary2)
            {
                raiseEventOptions.InterestGroup = (byte)pair3.Key;
                this.OpRaiseEvent(0xc9, pair3.Value, false, raiseEventOptions);
            }
        }
    }

    public void SetApp(string appId, string gameVersion)
    {
        this.mAppId = appId.Trim();
        if (!string.IsNullOrEmpty(gameVersion))
        {
            this.mAppVersion = gameVersion.Trim();
        }
    }

    public void SetLevelPrefix(short prefix)
    {
        this.currentLevelPrefix = prefix;
    }

    public void SetReceivingEnabled(int group, bool enabled)
    {
        if (group <= 0)
        {
            Debug.LogError("Error: PhotonNetwork.SetReceivingEnabled was called with an illegal group number: " + group + ". The group number should be at least 1.");
        }
        else if (enabled)
        {
            if (!this.allowedReceivingGroups.Contains(group))
            {
                this.allowedReceivingGroups.Add(group);
                byte[] groupsToAdd = new byte[] { (byte)group };
                this.OpChangeGroups(null, groupsToAdd);
            }
        }
        else if (this.allowedReceivingGroups.Contains(group))
        {
            this.allowedReceivingGroups.Remove(group);
            byte[] groupsToRemove = new byte[] { (byte)group };
            this.OpChangeGroups(groupsToRemove, null);
        }
    }

    public void SetReceivingEnabled(int[] enableGroups, int[] disableGroups)
    {
        List<byte> list = new List<byte>();
        List<byte> list2 = new List<byte>();
        if (enableGroups != null)
        {
            for (int i = 0; i < enableGroups.Length; i++)
            {
                int item = enableGroups[i];
                if (item <= 0)
                {
                    Debug.LogError("Error: PhotonNetwork.SetReceivingEnabled was called with an illegal group number: " + item + ". The group number should be at least 1.");
                }
                else if (!this.allowedReceivingGroups.Contains(item))
                {
                    this.allowedReceivingGroups.Add(item);
                    list.Add((byte)item);
                }
            }
        }
        if (disableGroups != null)
        {
            for (int j = 0; j < disableGroups.Length; j++)
            {
                int num4 = disableGroups[j];
                if (num4 <= 0)
                {
                    Debug.LogError("Error: PhotonNetwork.SetReceivingEnabled was called with an illegal group number: " + num4 + ". The group number should be at least 1.");
                }
                else if (list.Contains((byte)num4))
                {
                    Debug.LogError("Error: PhotonNetwork.SetReceivingEnabled disableGroups contains a group that is also in the enableGroups: " + num4 + ".");
                }
                else if (this.allowedReceivingGroups.Contains(num4))
                {
                    this.allowedReceivingGroups.Remove(num4);
                    list2.Add((byte)num4);
                }
            }
        }
        this.OpChangeGroups((list2.Count <= 0) ? null : list2.ToArray(), (list.Count <= 0) ? null : list.ToArray());
    }

    public void SetSendingEnabled(int group, bool enabled)
    {
        if (!enabled)
        {
            this.blockSendingGroups.Add(group);
        }
        else
        {
            this.blockSendingGroups.Remove(group);
        }
    }

    public void SetSendingEnabled(int[] enableGroups, int[] disableGroups)
    {
        if (enableGroups != null)
        {
            foreach (int num in enableGroups)
            {
                if (this.blockSendingGroups.Contains(num))
                {
                    this.blockSendingGroups.Remove(num);
                }
            }
        }
        if (disableGroups != null)
        {
            foreach (int num3 in disableGroups)
            {
                if (!this.blockSendingGroups.Contains(num3))
                {
                    this.blockSendingGroups.Add(num3);
                }
            }
        }
    }

    public bool WebRpc(string uriPath, object parameters)
    {
        Dictionary<byte, object> customOpParameters = new Dictionary<byte, object>();
        customOpParameters.Add(0xd1, uriPath);
        customOpParameters.Add(0xd0, parameters);
        return SendOperation(0xdb, customOpParameters, SendOptions.SendReliable);
    }
}
