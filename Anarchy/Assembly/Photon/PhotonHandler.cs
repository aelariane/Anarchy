using ExitGames.Client.Photon;
using Optimization;
using System;
using UnityEngine;

internal class PhotonHandler : Photon.MonoBehaviour, IPhotonPeerListener
{
    private const string PlayerPrefsKey = "PUNCloudBestRegion";
    private static bool sendThreadShouldRun;
    private int nextSendTickCount;
    private int nextSendTickCountOnSerialize;
    internal static CloudRegionCode BestRegionCodeCurrently = CloudRegionCode.none;
    public static bool AppQuits;
    public static Type PingImplementation = null;
    public static PhotonHandler SP;
    public int updateInterval;
    public int updateIntervalOnSerialize;

    internal static CloudRegionCode BestRegionCodeInPreferences
    {
        get
        {
            string @string = PlayerPrefs.GetString("PUNCloudBestRegion", string.Empty);
            if (!string.IsNullOrEmpty(@string))
            {
                return Region.Parse(@string);
            }
            return CloudRegionCode.none;
        }
        set
        {
            if (value == CloudRegionCode.none)
            {
                PlayerPrefs.DeleteKey("PUNCloudBestRegion");
            }
            else
            {
                PlayerPrefs.SetString("PUNCloudBestRegion", value.ToString());
            }
        }
    }

    protected void Awake()
    {
        if (PhotonHandler.SP != null && PhotonHandler.SP != this && PhotonHandler.SP.gameObject != null)
        {
            UnityEngine.Object.DestroyImmediate(PhotonHandler.SP.gameObject);
        }
        PhotonHandler.SP = this;
        UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
        this.updateInterval = 1000 / PhotonNetwork.sendRate;
        this.updateIntervalOnSerialize = 1000 / PhotonNetwork.sendRateOnSerialize;
        PhotonHandler.StartFallbackSendAckThread();
        NetworkingPeer.RegisterEvent(PhotonNetworkingMessage.OnCreatedRoom, OnCreatedRoom);
        NetworkingPeer.RegisterEvent(PhotonNetworkingMessage.OnJoinedRoom, OnJoinedRoom);
    }

    protected void OnApplicationQuit()
    {
        PhotonHandler.AppQuits = true;
        PhotonHandler.StopFallbackSendAckThread();
        PhotonNetwork.Disconnect();
    }

    protected void OnCreatedRoom(AOTEventArgs args)
    {
        PhotonNetwork.networkingPeer.SetLevelInPropsIfSynced(Application.loadedLevelName);
    }

    private void OnDestroy()
    {
        NetworkingPeer.RemoveEvent(PhotonNetworkingMessage.OnJoinedRoom, OnJoinedRoom);
        NetworkingPeer.RemoveEvent(PhotonNetworkingMessage.OnCreatedRoom, OnCreatedRoom);
    }

    protected void OnJoinedRoom(AOTEventArgs args)
    {
        PhotonNetwork.networkingPeer.LoadLevelIfSynced();
    }

    protected void OnLevelWasLoaded(int level)
    {
        PhotonNetwork.networkingPeer.NewSceneLoaded();
        PhotonNetwork.networkingPeer.SetLevelInPropsIfSynced(Application.loadedLevelName);
    }

    protected void Update()
    {
        if (PhotonNetwork.player != null)
        {
            PhotonNetwork.player.UpdateLocalProperties();
        }
        if (PhotonNetwork.connectionStateDetailed == PeerState.PeerCreated || PhotonNetwork.connectionStateDetailed == PeerState.Disconnected || PhotonNetwork.offlineMode || !PhotonNetwork.isMessageQueueRunning)
        {
            return;
        }

        while (PhotonNetwork.isMessageQueueRunning && PhotonNetwork.networkingPeer.DispatchIncomingCommands()) { }

        int num = (int)(Time.time * 1000f);
        if (PhotonNetwork.isMessageQueueRunning && num > this.nextSendTickCountOnSerialize)
        {
            PhotonNetwork.networkingPeer.RunViewUpdate();
            this.nextSendTickCountOnSerialize = num + this.updateIntervalOnSerialize;
            this.nextSendTickCount = 0;
        }

        if (num > this.nextSendTickCount)
        {
            while (PhotonNetwork.isMessageQueueRunning && PhotonNetwork.networkingPeer.SendOutgoingCommands()) { }
            this.nextSendTickCount = num + this.updateInterval;
        }
    }

    public static bool FallbackSendAckThread()
    {
        if (PhotonHandler.sendThreadShouldRun && PhotonNetwork.networkingPeer != null)
        {
            PhotonNetwork.networkingPeer.SendAcksOnly();
        }
        return PhotonHandler.sendThreadShouldRun;
    }

    public static void StartFallbackSendAckThread()
    {
        if (PhotonHandler.sendThreadShouldRun)
        {
            return;
        }
        PhotonHandler.sendThreadShouldRun = true;
        SupportClass.StartBackgroundCalls(new Func<bool>(FallbackSendAckThread));
    }

    public static void StopFallbackSendAckThread()
    {
        PhotonHandler.sendThreadShouldRun = false;
    }

    public void DebugReturn(DebugLevel level, string message)
    {
        if (level == DebugLevel.ERROR)
        {
            Debug.LogError(message);
        }
        else if (level == DebugLevel.WARNING)
        {
            Debug.LogWarning(message);
        }
        else if (level == DebugLevel.INFO && PhotonNetwork.logLevel >= PhotonLogLevel.Informational)
        {
            Debug.Log(message);
        }
        else if (level == DebugLevel.ALL && PhotonNetwork.logLevel == PhotonLogLevel.Full)
        {
            Debug.Log(message);
        }
    }

    public void OnEvent(EventData photonEvent)
    {
    }

    public void OnOperationResponse(OperationResponse operationResponse)
    {
    }

    public void OnStatusChanged(StatusCode statusCode)
    {
    }
}