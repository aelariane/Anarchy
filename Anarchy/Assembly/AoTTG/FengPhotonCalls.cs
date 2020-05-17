using System.Collections;
using System.Collections.Generic;
using Anarchy;
using Anarchy.UI;
using Optimization;
using Optimization.Caching;
using RC;
using UnityEngine;

// ReSharper disable once InconsistentNaming
internal partial class FengGameManagerMKII
{
    private void Awake()
    {
        FGM = this;
        NetworkingPeer.RegisterEvent(PhotonNetworkingMessage.OnConnectionFail, OnConnectionFail);
        NetworkingPeer.RegisterEvent(PhotonNetworkingMessage.OnConnectedToPhoton, OnConnectedToPhoton);
        NetworkingPeer.RegisterEvent(PhotonNetworkingMessage.OnCreatedRoom, OnCreatedRoom);
        NetworkingPeer.RegisterEvent(PhotonNetworkingMessage.OnDisconnectedFromPhoton, OnDisconnectedFromPhoton);
        NetworkingPeer.RegisterEvent(PhotonNetworkingMessage.OnJoinedLobby, OnJoinedLobby);
        NetworkingPeer.RegisterEvent(PhotonNetworkingMessage.OnJoinedRoom, OnJoinedRoom);
        NetworkingPeer.RegisterEvent(PhotonNetworkingMessage.OnLeftRoom, OnLeftRoom);
        NetworkingPeer.RegisterEvent(PhotonNetworkingMessage.OnMasterClientSwitched, OnMasterClientSwitched);
        NetworkingPeer.RegisterEvent(PhotonNetworkingMessage.OnPhotonPlayerConnected, OnPhotonPlayerConnected);
        NetworkingPeer.RegisterEvent(PhotonNetworkingMessage.OnPhotonPlayerDisconnected, OnPhotonPlayerDisconnected);
        NetworkingPeer.RegisterEvent(PhotonNetworkingMessage.OnPhotonPlayerPropertiesChanged,
            OnPhotonPlayerPropertiesChanged);
    }

    private void LateUpdate()
    {
        if (gameStart)
        {
            foreach (var h in heroes) h.lateUpdate();
            foreach (var t in titans) t.lateUpdate();
            logic.OnLateUpdate();
            roomInformation.Update();
        }
    }

    private void OnApplicationFocus(bool focus)
    {
        if (focus)
        {
            if (Application.loadedLevelName == "menu" || AnarchyManager.SettingsPanel.Active ||
                AnarchyManager.CharacterSelectionPanel.Active || IN_GAME_MAIN_CAMERA.isPausing)
            {
                Screen.showCursor = true;
                Screen.lockCursor = false;
            }
            else if (IN_GAME_MAIN_CAMERA.GameType == GameType.Single ||
                     IN_GAME_MAIN_CAMERA.GameType == GameType.MultiPlayer && !PhotonNetwork.player.Dead)
            {
                Screen.showCursor = false;
                Screen.lockCursor = IN_GAME_MAIN_CAMERA.CameraMode >= CameraType.TPS;
                if (GameObject.Find("cross1"))
                    GameObject.Find("cross1").GetComponent<Transform>().position = Input.mousePosition;
            }
        }
    }

    private void OnLevelWasLoaded(int level)
    {
        if (level == 0) return;
        if (Application.loadedLevelName == "characterCreation" || Application.loadedLevelName == "SnapShot") return;
        var array = GameObject.FindGameObjectsWithTag("titan");
        foreach (var go in array)
            if (go.GetPhotonView() == null || !go.GetPhotonView().owner.IsMasterClient)
                Destroy(go);
        gameStart = true;
        Pool.Clear();
        RespawnPositions.Dispose();
        ShowHUDInfoCenter(string.Empty);
        var gameObject2 = (GameObject) Instantiate(CacheResources.Load("MainCamera_mono"),
            CacheGameObject.Find("cameraDefaultPosition").transform.position,
            CacheGameObject.Find("cameraDefaultPosition").transform.rotation);
        Destroy(CacheGameObject.Find("cameraDefaultPosition"));
        gameObject2.name = "MainCamera";
        Screen.lockCursor = true;
        Screen.showCursor = true;
        var ui = (GameObject) Instantiate(CacheResources.Load("UI_IN_GAME"));
        ui.name = "UI_IN_GAME";
        ui.SetActive(true);
        UIRefer = ui.GetComponent<UIReferArray>();
        NGUITools.SetActive(UIRefer.panels[0], true);
        NGUITools.SetActive(UIRefer.panels[1], false);
        NGUITools.SetActive(UIRefer.panels[2], false);
        NGUITools.SetActive(UIRefer.panels[3], false);
        IN_GAME_MAIN_CAMERA.MainCamera.setHUDposition();
        IN_GAME_MAIN_CAMERA.MainCamera.setDayLight(IN_GAME_MAIN_CAMERA.DayLight);
        var info = Level;
        ClothFactory.ClearClothCache();
        logic.OnGameRestart();
        PlayerList = new PlayerList();
        if (IN_GAME_MAIN_CAMERA.GameType == GameType.Single)
        {
            LoadSkinCheck();
            CustomLevel.OnLoadLevel();
            singleKills = 0;
            singleMax = 0;
            singleTotal = 0;
            IN_GAME_MAIN_CAMERA.MainCamera.enabled = true;
            IN_GAME_MAIN_CAMERA.SpecMov.disable = true;
            IN_GAME_MAIN_CAMERA.Look.disable = true;
            IN_GAME_MAIN_CAMERA.GameMode = Level.Mode;
            SpawnPlayer(IN_GAME_MAIN_CAMERA.singleCharacter.ToUpper());
            Screen.lockCursor = IN_GAME_MAIN_CAMERA.CameraMode >= CameraType.TPS;
            Screen.showCursor = false;
            var rate = 90;
            if (difficulty == 1) rate = 70;
            SpawnTitansCustom(rate, info.EnemyNumber);
            return;
        }

        PVPcheckPoint.chkPts = new ArrayList();
        IN_GAME_MAIN_CAMERA.MainCamera.enabled = false;
        IN_GAME_MAIN_CAMERA.BaseCamera.GetComponent<CameraShake>().enabled = false;
        IN_GAME_MAIN_CAMERA.GameType = GameType.MultiPlayer;
        LoadSkinCheck();
        CustomLevel.OnLoadLevel();
        switch (info.Mode)
        {
            case GameMode.TROST:
            {
                CacheGameObject.Find("playerRespawn").SetActive(false);
                Destroy(CacheGameObject.Find("playerRespawn"));
                var gameObject3 = CacheGameObject.Find("rock");
                gameObject3.animation["lift"].speed = 0f;
                CacheGameObject.Find("door_fine").SetActive(false);
                CacheGameObject.Find("door_broke").SetActive(true);
                Destroy(CacheGameObject.Find("ppl"));
                break;
            }
            case GameMode.BOSS_FIGHT_CT:
                CacheGameObject.Find("playerRespawnTrost").SetActive(false);
                Destroy(CacheGameObject.Find("playerRespawnTrost"));
                break;
        }

        if (needChooseSide)
        {
            ShowHUDInfoTopCenterADD("\n\nPRESS 1 TO ENTER GAME");
        }
        else
        {
            Screen.lockCursor = IN_GAME_MAIN_CAMERA.CameraMode >= CameraType.TPS;
            if (IN_GAME_MAIN_CAMERA.GameMode == GameMode.PVP_CAPTURE)
            {
                if ((int) PhotonNetwork.player.Properties[PhotonPlayerProperty.isTitan] == 2)
                    checkpoint = CacheGameObject.Find("PVPchkPtT");
                else
                    checkpoint = CacheGameObject.Find("PVPchkPtH");
            }

            if ((int) PhotonNetwork.player.Properties[PhotonPlayerProperty.isTitan] == 2)
                SpawnNonAiTitan(myLastHero);
            else
                SpawnPlayer(myLastHero);
        }

        if (info.Mode == GameMode.BOSS_FIGHT_CT) Destroy(CacheGameObject.Find("rock"));
        if (PhotonNetwork.IsMasterClient)
        {
            switch (info.Mode)
            {
                case GameMode.TROST:
                {
                    if (!IsPlayerAllDead())
                    {
                        var gameObject4 = Pool.NetworkEnable("TITAN_EREN_trost", new Vector3(-200f, 0f, -194f),
                            Quaternion.Euler(0f, 180f, 0f));
                        gameObject4.GetComponent<TITAN_EREN>().rockLift = true;
                        var rate2 = 90;
                        if (difficulty == 1) rate2 = 70;
                        var array3 = GameObject.FindGameObjectsWithTag("titanRespawn");
                        var gameObject5 = CacheGameObject.Find("titanRespawnTrost");
                        if (gameObject5 != null)
                            foreach (var gameObject6 in array3)
                                if (gameObject6.transform.parent.gameObject == gameObject5)
                                    SpawnTitan(rate2, gameObject6.transform.position, gameObject6.transform.rotation);
                    }

                    break;
                }
                case GameMode.BOSS_FIGHT_CT:
                {
                    if (!IsPlayerAllDead())
                        Pool.NetworkEnable("COLOSSAL_TITAN", -Vectors.up * 10000f, Quaternion.Euler(0f, 180f, 0f));
                    break;
                }
                case GameMode.KILL_TITAN:
                case GameMode.ENDLESS_TITAN:
                case GameMode.SURVIVE_MODE:
                {
                    if (info.Name == "Annie" || info.Name == "Annie II")
                    {
                        Pool.NetworkEnable("FEMALE_TITAN", CacheGameObject.Find("titanRespawn").transform.position,
                            CacheGameObject.Find("titanRespawn").transform.rotation);
                    }
                    else
                    {
                        var rate3 = 90;
                        if (difficulty == 1) rate3 = 70;
                        SpawnTitansCustom(rate3, info.EnemyNumber);
                    }

                    break;
                }
                default:
                {
                    if (info.Mode != GameMode.TROST)
                    {
                        if (info.Mode == GameMode.PVP_CAPTURE && Level.MapName == "OutSide")
                        {
                            var array5 = GameObject.FindGameObjectsWithTag("titanRespawn");
                            if (array5.Length <= 0) return;
                            for (var k = 0; k < array5.Length; k++)
                                SpawnTitanRaw(array5[k].transform.position, array5[k].transform.rotation)
                                    .SetAbnormalType(AbnormalType.Crawler, true);
                        }
                    }

                    break;
                }
            }
        }

        if (!info.Supply) Destroy(CacheGameObject.Find("aot_supply"));
        if (!PhotonNetwork.IsMasterClient) BasePV.RPC("RequireStatus", PhotonTargets.MasterClient);
        if (Stylish != null) Stylish.enabled = true;
        if (Level.LavaMode)
        {
            Instantiate(CacheResources.Load("levelBottom"), new Vector3(0f, -29.5f, 0f), Quaternion.Euler(0f, 0f, 0f));
            CacheGameObject.Find("aot_supply").transform.position =
                CacheGameObject.Find("aot_supply_lava_position").transform.position;
            CacheGameObject.Find("aot_supply").transform.rotation =
                CacheGameObject.Find("aot_supply_lava_position").transform.rotation;
        }

        roomInformation.UpdateLabels();
        Resources.UnloadUnusedAssets();
    }

    private void Start()
    {
        ChangeQuality.setCurrentQuality();
        name = "MultiplayerManager";
        DontDestroyOnLoad(this);
        HeroCostume.Init();
        CharacterMaterials.Init();
        RCManager.ClearAll();
        heroes = new List<HERO>();
        titans = new List<TITAN>();
        hooks = new List<Bullet>();
        AnarchyManager.DestroyMainScene();
    }

    private void Update()
    {
        FPS.FPSUpdate();
        if (IN_GAME_MAIN_CAMERA.GameType != GameType.Single)
            Labels.NetworkStatus = PhotonNetwork.connectionStateDetailed +
                                   (PhotonNetwork.connected ? " ping: " + PhotonNetwork.GetPing() : "");
        if (!gameStart)
            return;

        int i;
        for (i = 0; i < hooks.Count; i++) hooks[i].update();
        for (i = 0; i < heroes.Count; i++) heroes[i].update();
        if (IN_GAME_MAIN_CAMERA.GameType == GameType.Single || PhotonNetwork.IsMasterClient)
            for (i = 0; i < titans.Count; i++)
                titans[i].update();
        else if (IN_GAME_MAIN_CAMERA.GameType == GameType.MultiPlayer && PhotonNetwork.player.IsTitan)
            for (i = 0; i < titans.Count; i++)
                if (titans[i].IsLocal)
                    titans[i].update();
        if (mainCamera != null)
        {
            mainCamera.update();
            mainCamera.snapShotUpdate();
        }

        logic.OnUpdate();
    }

    public void OnConnectedToMaster(AOTEventArgs args)
    {
        print("OnConnectedToMaster");
    }

    public void OnConnectedToPhoton(AOTEventArgs args)
    {
        print("OnConnectedToPhoton");
    }

    public void OnConnectionFail(AOTEventArgs args)
    {
        print("OnConnectionFail : " + args.DisconnectCause);
        Screen.lockCursor = false;
        Screen.showCursor = true;
        IN_GAME_MAIN_CAMERA.GameType = GameType.Stop;
        gameStart = false;
        NGUITools.SetActive(UIRefer.panels[0], false);
        NGUITools.SetActive(UIRefer.panels[1], false);
        NGUITools.SetActive(UIRefer.panels[2], false);
        NGUITools.SetActive(UIRefer.panels[3], false);
        NGUITools.SetActive(UIRefer.panels[4], true);
        CacheGameObject.Find("LabelDisconnectInfo").GetComponent<UILabel>().text =
            "OnConnectionFail : " + args.DisconnectCause;
    }

    public void OnCreatedRoom(AOTEventArgs args)
    {
        racingResult = new ArrayList();
    }

    public void OnCustomAuthenticationFailed()
    {
        print("OnCustomAuthenticationFailed");
    }

    private void OnDestroy()
    {
        NetworkingPeer.RemoveEvent(PhotonNetworkingMessage.OnConnectionFail, OnConnectionFail);
        NetworkingPeer.RemoveEvent(PhotonNetworkingMessage.OnConnectedToPhoton, OnConnectedToPhoton);
        NetworkingPeer.RemoveEvent(PhotonNetworkingMessage.OnCreatedRoom, OnCreatedRoom);
        NetworkingPeer.RemoveEvent(PhotonNetworkingMessage.OnDisconnectedFromPhoton, OnDisconnectedFromPhoton);
        NetworkingPeer.RemoveEvent(PhotonNetworkingMessage.OnJoinedLobby, OnJoinedLobby);
        NetworkingPeer.RemoveEvent(PhotonNetworkingMessage.OnJoinedRoom, OnJoinedRoom);
        NetworkingPeer.RemoveEvent(PhotonNetworkingMessage.OnLeftRoom, OnLeftRoom);
        NetworkingPeer.RemoveEvent(PhotonNetworkingMessage.OnMasterClientSwitched, OnMasterClientSwitched);
        NetworkingPeer.RemoveEvent(PhotonNetworkingMessage.OnPhotonPlayerConnected, OnPhotonPlayerConnected);
        NetworkingPeer.RemoveEvent(PhotonNetworkingMessage.OnPhotonPlayerDisconnected, OnPhotonPlayerDisconnected);
        NetworkingPeer.RemoveEvent(PhotonNetworkingMessage.OnPhotonPlayerPropertiesChanged,
            OnPhotonPlayerPropertiesChanged);
        levelSkin = null;
    }

    public void OnDisconnectedFromPhoton(AOTEventArgs args)
    {
        print("OnDisconnectedFromPhoton");
        Screen.lockCursor = false;
        Screen.showCursor = true;
    }

    public void OnFailedToConnectToPhoton(AOTEventArgs args)
    {
        print("OnFailedToConnectToPhoton");
    }

    public void OnJoinedLobby(AOTEventArgs args)
    {
    }

    public void OnJoinedRoom(AOTEventArgs args)
    {
        Debug.Log("OnJoinedRoom >> " + PhotonNetwork.room.Name);
        var strArray = PhotonNetwork.room.Name.Split('`');
        gameTimesUp = false;
        Level = LevelInfo.GetInfo(strArray[1]);
        switch (strArray[2].ToLower())
        {
            case "normal":
                difficulty = 0;
                break;
            case "hard":
                difficulty = 1;
                break;
            case "abnormal":
                difficulty = 2;
                break;
            default:
                difficulty = 1;
                break;
        }

        IN_GAME_MAIN_CAMERA.Difficulty = difficulty;
        time = int.Parse(strArray[3]) * 60;
        logic.ServerTimeBase = time;
        logic.ServerTime = time;
        switch (strArray[4].ToLower())
        {
            case "day":
            case "день":
                IN_GAME_MAIN_CAMERA.DayLight = DayLight.Day;
                break;
            case "dawn":
            case "вечер":
                IN_GAME_MAIN_CAMERA.DayLight = DayLight.Dawn;
                break;
            case "night":
            case "ночь":
                IN_GAME_MAIN_CAMERA.DayLight = DayLight.Night;
                break;
            default:
                IN_GAME_MAIN_CAMERA.DayLight = DayLight.Dawn;
                break;
        }

        IN_GAME_MAIN_CAMERA.GameMode = Level.Mode;
        PhotonNetwork.LoadLevel(Level.MapName);
        if (PhotonNetwork.IsMasterClient)
        {
            GameModes.Load();
            GameModes.ForceChange();
            GameModes.SendRpc();
        }

        var player = PhotonNetwork.player;
        player.RCIgnored = false;
        player.UIName = User.Name;
        player.GuildName = User.AllGuildNames;
        player.Kills = player.Deaths = player.Max_Dmg = player.Total_Dmg = 0;
        player.RCteam = 0;
        player.Dead = true;
        player.IsTitan = false;
        localRacingResult = string.Empty;
        needChooseSide = true;
        foreach (var info in killInfoList) info.destroy();
        killInfoList.Clear();
        RCManager.racingSpawnPointSet = false;
        if (!PhotonNetwork.IsMasterClient) BasePV.RPC("RequireStatus", PhotonTargets.MasterClient);
        foreach (var her in heroes)
            if (her.BasePV != null && her.BasePV.owner.GameObject == null)
                her.BasePV.owner.GameObject = her.baseG;
    }

    public void OnLeftLobby(AOTEventArgs args)
    {
        print("OnLeftLobby");
    }

    public void OnLeftRoom(AOTEventArgs args)
    {
        print("OnLeftRoom");
        if (Application.loadedLevel != 0)
        {
            Time.timeScale = 1f;
            if (PhotonNetwork.connected) PhotonNetwork.Disconnect();
            IN_GAME_MAIN_CAMERA.GameType = GameType.Stop;
            gameStart = false;
            Screen.lockCursor = false;
            Screen.showCursor = true;
            InputManager.MenuOn = false;
            levelSkin = null;
            DestroyAllExistingCloths();
            Destroy(FGM);
            Application.LoadLevel("menu");
        }

        RespawnPositions.Dispose();
        RCManager.racingSpawnPointSet = false;
    }

    public void OnMasterClientSwitched(AOTEventArgs args)
    {
        print("OnMasterClientSwitched");
        if (gameTimesUp) return;
        if (PhotonNetwork.IsMasterClient)
        {
            GameModes.Load();
            GameModes.ForceChange();
            RCManager.ClearAll();
            RestartGame(true, false);
        }
    }

    public void OnPhotonCreateRoomFailed(AOTEventArgs args)
    {
        Debug.LogError("OnPhotonCreateRoomFailed");
    }

    public void OnPhotonCustomRoomPropertiesChanged(AOTEventArgs args)
    {
        print("OnPhotonCustomRoomPropertiesChanged");
    }

    public void OnPhotonJoinRoomFailed(AOTEventArgs args)
    {
        print("OnPhotonJoinRoomFailed");
    }

    public void OnPhotonPlayerConnected(AOTEventArgs args)
    {
        StartCoroutine(OnPhotonPlayerConnectedE(args.Player));
    }

    public void OnPhotonPlayerDisconnected(AOTEventArgs args)
    {
        var player = args.Player;
        if (player != null)
        {
            if (player.RCIgnored)
                Log.AddLine("playerKicked", MsgType.Info, player.ID.ToString(), player.UIName.ToHTMLFormat());
            else
                Log.AddLine("playerLeft", MsgType.Info, player.ID.ToString(), player.UIName.ToHTMLFormat());
        }

        PlayerList?.Update();
        if (!gameTimesUp)
            if (PhotonNetwork.IsMasterClient)
            {
                oneTitanDown(string.Empty, true);
                someOneIsDead(0);
            }
    }

    public void OnPhotonPlayerPropertiesChanged(AOTEventArgs args)
    {
        PlayerList?.Update();
    }

    public void OnPhotonRandomJoinFailed(AOTEventArgs args)
    {
        print("OnPhotonRandomJoinFailed");
    }

    public void OnPhotonSerializeView(AOTEventArgs args)
    {
        print("OnPhotonSerializeView");
    }

    public void OnReceivedRoomListUpdate(AOTEventArgs args)
    {
    }

    public void OnUpdatedFriendList(AOTEventArgs args)
    {
        print("OnUpdatedFriendList");
    }
}