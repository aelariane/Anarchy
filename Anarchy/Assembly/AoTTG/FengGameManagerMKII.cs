using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Anarchy;
using Anarchy.Configuration;
using Anarchy.UI;
using Optimization;
using Optimization.Caching;
using RC;
using UnityEngine;

internal class FengGameManagerMKII : Photon.MonoBehaviour
{
    private static readonly Hashtable itweenHash = new Hashtable() { { "x", 0 }, { "y", 0 }, { "z", 0 }, { "easetype", iTween.EaseType.easeInBounce }, { "time", 0.5f }, { "delay", 2f } };
    private static Queue<KillInfoComponent> killInfoList = new Queue<KillInfoComponent>();
    private FEMALE_TITAN annie;
    private COLOSSAL_TITAN colossal;
    private TITAN_EREN eren;
    private bool gameTimesUp;
    private List<HERO> heroes;
    private List<Bullet> hooks;
    private Anarchy.Skins.Maps.LevelSkin levelSkin;
    public string LocalRacingResult;
    private IN_GAME_MAIN_CAMERA mainCamera;
    public string myLastHero;
    private string myLastRespawnTag = "";
    private PlayerList playerList;
    private RoomInformation roomInformation = new RoomInformation();
    private ArrayList racingResult;
    internal List<TITAN> titans;

    public const string ApplicationId = "f1f6195c-df4a-40f9-bae5-4744c32901ef";
    public static FengGameManagerMKII FGM;
    public static FPSCounter FPS = new FPSCounter();
    public static bool LAN;
    public static LevelInfo Level;
    public static StylishComponent Stylish;
    public GameObject checkpoint;
    public int Difficulty;
    public bool GameStart;
    public bool JustSuicide;
    public GameLogic.GameLogic Logic;
    public bool NeedChooseSide;
    public int SingleKills;
    public int SingleMax;
    public int SingleTotal;
    public int Time = 600;

    public static FEMALE_TITAN Annie => FGM.annie;
    public static COLOSSAL_TITAN Colossal => FGM.colossal;
    public static TITAN_EREN Eren => FGM.eren;
    public static List<HERO> Heroes => FGM.heroes;
    public bool IsLosing => Logic.Round.IsLosing;
    public bool IsWinning => Logic.Round.IsWinning;
    public static List<TITAN> Titans => FGM.titans;
    public static UIReferArray UIRefer { get; private set; }

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
        NetworkingPeer.RegisterEvent(PhotonNetworkingMessage.OnPhotonPlayerPropertiesChanged, OnPhotonPlayerPropertiesChanged);
    }

    

    [RPC]
    private void Chat(string content, string sender, PhotonMessageInfo info)
    {
        //TODO: Add antis here
        string message;
        if (sender != string.Empty) message = sender + ": " + content;
        else message = content;
        Anarchy.UI.Chat.Add(User.Chat(info.Sender.ID, message).RemoveSize());
    }

    [RPC]
    private void ChatLocalized(string file, string key, string[] args, PhotonMessageInfo info)
    {
        if (!info.Sender.Anarchy)
        {
            Log.AddLine("notAnarchyUser", MsgType.Error, "RPC", nameof(ChatLocalized), info.Sender.ID.ToString());
            return;
        }
        var locale = Anarchy.Localization.Language.Find(file);
        if (locale == null || args == null)
        {
            Log.AddLine("RPCerror", MsgType.Error, info.Sender.ID.ToString(), nameof(ChatLocalized));
            return;
        }
        if (!locale.IsOpen)
        {
            locale.KeepOpen(60);
        }
        if(args.Length > 0)
        {
            Anarchy.UI.Chat.Add(locale.Format(key, args));
        }
        else
        {
            Anarchy.UI.Chat.Add(locale[key]);
        }
    }

    [RPC]
    private void ChatPM(string sender, string content, PhotonMessageInfo info)
    {
        //TODO: Add antis here
        content = sender + ":" + content;
        Anarchy.UI.Chat.Add(User.ChatPM(info.Sender.ID, content).RemoveSize());
    }

    private bool CheckIsTitanAllDie()
    {
        foreach(TITAN tit in titans)
        {
            if (!tit.hasDie)
            {
                return false;
            }
        }
        return annie == null;
    }

    [RPC]
    private void clearlevel(string[] link, int gametype, PhotonMessageInfo info)
    {
        if(info != null && !info.Sender.IsMasterClient)
        {
            Log.AddLine("RPCerror", MsgType.Error, info.Sender.ID.ToString(), nameof(clearlevel));
            return;
        }
        switch (gametype)
        {
            case 0:
                IN_GAME_MAIN_CAMERA.GameMode = GameMode.KILL_TITAN;
                if (Logic == null || !(Logic is GameLogic.KillTitanLogic))
                {
                    Logic = new GameLogic.KillTitanLogic(Logic);
                }
                break;

            case 1:
                IN_GAME_MAIN_CAMERA.GameMode = GameMode.SURVIVE_MODE;
                if (Logic == null || !(Logic is GameLogic.SurviveLogic))
                {
                    Logic = new GameLogic.SurviveLogic(Logic);
                }
                break;

            case 2:
                IN_GAME_MAIN_CAMERA.GameMode = GameMode.PVP_AHSS;
                if (Logic == null || !(Logic is GameLogic.PVPLogic))
                {
                    Logic = new GameLogic.PVPLogic(Logic);
                }
                break;

            case 3:
                IN_GAME_MAIN_CAMERA.GameMode = GameMode.RACING;
                if (Logic == null || !(Logic is GameLogic.RacingLogic))
                {
                    Logic = new GameLogic.RacingLogic(Logic);
                }
                break;

            default:
            case 4:
                IN_GAME_MAIN_CAMERA.GameMode = GameMode.None;
                if (Logic == null || !Logic.GetType().Equals(typeof(GameLogic.GameLogic)))
                {
                    Logic = new GameLogic.GameLogic(Logic);
                }
                break;
        }
        if (SkinSettings.CustomSkins.Value != 1)
        {
            return;
        }
        CustomLevel.LoadSkin(link, info);
    }


    [RPC]
    private void customlevelRPC(string[] content, PhotonMessageInfo info = null)
    {
        if (info != null && !info.Sender.IsMasterClient)
        {
            Log.AddLine("RPCerror", MsgType.Error, info.Sender.ID.ToString(), nameof(customlevelRPC));
            return;
        }
        CustomLevel.RPC(content);
    }

    [RPC]
    private void getRacingResult(string player, float time, PhotonMessageInfo info)
    {
        if (info != null && (IN_GAME_MAIN_CAMERA.GameMode != GameMode.RACING || !PhotonNetwork.IsMasterClient))
        {
            Log.AddLine("RPCerror", MsgType.Error, info.Sender.ID.ToString(), nameof(getRacingResult));
            return;
        }
        (Logic as GameLogic.RacingLogic).OnPlayerFinished(time, player);
        RacingResult racingResult = new RacingResult(player, time);
        this.racingResult.Add(racingResult);
        this.RefreshRacingResult();
    }

    [RPC]
    private void ignorePlayer(int ID, PhotonMessageInfo info)
    {
        if (!info.Sender.IsMasterClient)
        {
            Log.AddLine("RPCerror", MsgType.Error, info.Sender.ID.ToString(), nameof(ignorePlayerArray));
            return;
        }
        PhotonPlayer photonPlayer = PhotonPlayer.Find(ID);
        if (photonPlayer != null && !photonPlayer.RCIgnored)
        {
            photonPlayer.RCIgnored = true;
        }
        playerList.Update();
    }

    [RPC]
    private void ignorePlayerArray(int[] IDS, PhotonMessageInfo info)
    {
        if (!info.Sender.IsMasterClient)
        {
            Log.AddLine("RPCerror", MsgType.Error, info.Sender.ID.ToString(), nameof(ignorePlayerArray));
            return;
        }
        foreach (int ID in IDS)
        {
            PhotonPlayer photonPlayer = PhotonPlayer.Find(ID);
            if (photonPlayer != null && !photonPlayer.RCIgnored)
            {
                photonPlayer.RCIgnored = true;
            }
        }
        playerList.Update();
    }

    [RPC]
    private void labelRPC(int setting, PhotonMessageInfo info)
    {
        if (info != null)
        {
            int checkID = info.TimeInt - 1000000;
            checkID *= -1;
            if(checkID == info.Sender.ID)
            {
                if (!info.Sender.Anarchy)
                {
                    info.Sender.Anarchy = true;
                    playerList.Update();
                }
            }
        }
    }

    private void LateUpdate()
    {
        if (this.GameStart)
        {
            foreach (var h in heroes)
            {
                h.lateUpdate();
            }
            foreach (var t in titans)
            {
                t.lateUpdate();
            }
            Logic.OnLateUpdate();
            roomInformation.Update();
        }
    }

    private void LoadMapSkin(string n, string urls, string str, string[] skybox, PhotonMessageInfo info = null)
    {
        string[] checkUrls = urls.Split(',');
        string[] checkStr = str.Split(',');
        string[] checkData = new string[1 + 8 + checkStr.Length + 6];
        checkData[0] = n;
        int i = 1;
        for (int j = 0; i < 1 + 8; i++, j++)
        {
            checkData[i] = checkUrls[j];
        }
        for (int j = 0; i < 1 + 8 + checkStr.Length; i++, j++)
        {
            checkData[i] = checkStr[j];
        }
        for (int j = 0; i < checkData.Length; i++, j++)
        {
            checkData[i] = skybox[j];
        }
        if (levelSkin == null)
        {
            if (Level.Name.Contains("City"))
            {
                levelSkin = new Anarchy.Skins.Maps.CitySkin(checkData);
            }
            else if (Level.Name.Contains("Forest"))
            {
                levelSkin = new Anarchy.Skins.Maps.ForestSkin(checkData);
            }
        }
        if (levelSkin == null)
        {
            return;
        }
        Anarchy.Skins.Skin.Check(levelSkin, checkData);
    }

    private void LoadSkinCheck()
    {
        if (Level.Name.Contains("City"))
        {
            if (SkinSettings.SkinsCheck(SkinSettings.CitySkins))
            {
                if (SkinSettings.CitySet.Value != "$Not define$")
                {
                    var set = new Anarchy.Configuration.Presets.CityPreset(SkinSettings.CitySet);
                    set.Load();
                    string n = "";
                    for (int i = 0; i < 250; i++)
                    {
                        int val = UnityEngine.Random.Range(0, 8);
                        n += val.ToString();
                    }
                    string urls = string.Join(",", set.Houses) + ",";
                    string urls2 = $"{set.Ground},{set.Wall},{set.Gate}";
                    string[] box = new string[6].Select(x => "").ToArray();
                    if (set.LinkedSkybox != "$Not define$")
                    {
                        var boxSet = new Anarchy.Configuration.Presets.SkyboxPreset(set.LinkedSkybox);
                        boxSet.Load();
                        box = boxSet.ToSkinData();
                    }
                    LoadMapSkin(n, urls, urls2, box);
                    if (IN_GAME_MAIN_CAMERA.GameType == GameType.Multi && PhotonNetwork.IsMasterClient)
                    {
                        BasePV.RPC("loadskinRPC", PhotonTargets.OthersBuffered, new object[] { n, urls, urls2, box });
                    }
                }
            }
        }
        else if (Level.MapName.Contains("Forest"))
        {
            if (SkinSettings.SkinsCheck(SkinSettings.ForestSkins))
            {
                if (SkinSettings.ForestSet.Value != "$Not define$")
                {
                    var set = new Anarchy.Configuration.Presets.ForestPreset(SkinSettings.ForestSet);
                    set.Load();
                    string n = "";
                    for (int i = 0; i < 150; i++)
                    {
                        int val = UnityEngine.Random.Range(0, 8);
                        n += val.ToString();
                        if (set.RandomizePairs)
                        {
                            n += UnityEngine.Random.Range(0, 8).ToString();
                        }
                        else
                        {
                            n += val.ToString();
                        }
                    }
                    string urls = string.Join(",", set.Trees) + ",";
                    string urls2 = string.Join(",", set.Leaves);
                    urls2 += "," + set.Ground;
                    string[] box = new string[6].Select(x => "").ToArray();
                    if (set.LinkedSkybox != "$Not define$")
                    {
                        var boxSet = new Anarchy.Configuration.Presets.SkyboxPreset(set.LinkedSkybox);
                        boxSet.Load();
                        box = boxSet.ToSkinData();
                    }
                    LoadMapSkin(n, urls, urls2, box);
                    if (IN_GAME_MAIN_CAMERA.GameType == GameType.Multi && PhotonNetwork.IsMasterClient)
                    {
                        BasePV.RPC("loadskinRPC", PhotonTargets.OthersBuffered, new object[] { n, urls, urls2, box });
                    }
                }
            }
        }
    }

    [RPC]
    public void loadskinRPC(string n, string urls, string str, string[] skybox, PhotonMessageInfo info = null)
    {
        if(info != null && !info.Sender.IsMasterClient)
        {
            Log.AddLine("RPCerror", MsgType.Error, info.Sender.ID.ToString(), nameof(FengGameManagerMKII) + "." + nameof(loadskinRPC));
            return;
        }
        if (!Level.MapName.Contains("City") && !Level.MapName.Contains("Forest"))
        {
            return;
        }
        if (Level.MapName.Contains("City") && SkinSettings.CitySkins.Value != 1)
        {
            return;
        }
        else if (Level.MapName.Contains("Forest") && SkinSettings.ForestSkins.Value != 1)
        {
            return;
        }
        LoadMapSkin(n, urls, str, skybox);
    }

    [RPC]
    private void netGameLose(int score, PhotonMessageInfo info)
    {
        if (info != null && !info.Sender.IsMasterClient)
        {
            Log.AddLine("RPCerror", MsgType.Error, info.Sender.ID.ToString(), nameof(netGameLose));
            return;
        }
        Logic.NetGameLose(score);
    }

    [RPC]
    private void netGameWin(int score, PhotonMessageInfo info)
    {
        if (info != null && !info.Sender.IsMasterClient && IN_GAME_MAIN_CAMERA.GameMode != GameMode.RACING)
        {
            Log.AddLine("RPCerror", MsgType.Error, info.Sender.ID.ToString(), nameof(netGameWin));
            return;
        }
        Logic.NetGameWin(score);
    }

    [RPC]
    private void netRefreshRacingResult(string tmp, PhotonMessageInfo info)
    {
        if (info != null && (IN_GAME_MAIN_CAMERA.GameMode != GameMode.RACING || !info.Sender.IsMasterClient))
        {
            Log.AddLine("RPCerror", MsgType.Error, info.Sender.ID.ToString(), nameof(netRefreshRacingResult));
            return;
        }
        this.LocalRacingResult = tmp;
        if(Logic is GameLogic.RacingLogic rac)
        {
            rac.OnUpdateRacingResult();
        }
    }

    private void OnLevelWasLoaded(int level)
    {
        if (level == 0)
        {
            return;
        }
        if (Application.loadedLevelName == "characterCreation" || Application.loadedLevelName == "SnapShot")
        {
            return;
        }
        GameObject[] array = GameObject.FindGameObjectsWithTag("titan");
        foreach (GameObject gameObject in array)
        {
            if (gameObject.GetPhotonView() == null || !gameObject.GetPhotonView().owner.IsMasterClient)
            {
                UnityEngine.Object.Destroy(gameObject);
            }
        }
        this.GameStart = true;
        Pool.Clear();
        RespawnPositions.Dispose();
        this.ShowHUDInfoCenter(string.Empty);
        GameObject gameObject2 = (GameObject)UnityEngine.Object.Instantiate(CacheResources.Load("MainCamera_mono"), CacheGameObject.Find("cameraDefaultPosition").transform.position, CacheGameObject.Find("cameraDefaultPosition").transform.rotation);
        UnityEngine.Object.Destroy(CacheGameObject.Find("cameraDefaultPosition"));
        gameObject2.name = "MainCamera";
        Screen.lockCursor = true;
        Screen.showCursor = true;
        var ui = (GameObject)Instantiate(CacheResources.Load("UI_IN_GAME"));
        ui.name = "UI_IN_GAME";
        ui.SetActive(true);
        UIRefer = ui.GetComponent<UIReferArray>();
        NGUITools.SetActive(UIRefer.panels[0], true);
        NGUITools.SetActive(UIRefer.panels[1], false);
        NGUITools.SetActive(UIRefer.panels[2], false);
        NGUITools.SetActive(UIRefer.panels[3], false);
        IN_GAME_MAIN_CAMERA.MainCamera.setHUDposition();
        IN_GAME_MAIN_CAMERA.MainCamera.setDayLight(IN_GAME_MAIN_CAMERA.DayLight);
        LevelInfo info = Level;
        ClothFactory.ClearClothCache();
        Logic.OnGameRestart();
        playerList = new PlayerList();
        if (IN_GAME_MAIN_CAMERA.GameType == GameType.Single)
        {
            LoadSkinCheck();
            CustomLevel.OnLoadLevel();
            this.SingleKills = 0;
            this.SingleMax = 0;
            this.SingleTotal = 0;
            IN_GAME_MAIN_CAMERA.MainCamera.enabled = true;
            IN_GAME_MAIN_CAMERA.SpecMov.disable = true;
            IN_GAME_MAIN_CAMERA.Look.disable = true;
            IN_GAME_MAIN_CAMERA.GameMode = FengGameManagerMKII.Level.Mode;
            this.SpawnPlayer(IN_GAME_MAIN_CAMERA.singleCharacter.ToUpper());
            Screen.lockCursor = IN_GAME_MAIN_CAMERA.CameraMode >= CameraType.TPS;
            Screen.showCursor = false;
            int rate = 90;
            if (this.Difficulty == 1)
            {
                rate = 70;
            }
            SpawnTitansCustom(rate, info.EnemyNumber, false);
            return;
        }
        PVPcheckPoint.chkPts = new ArrayList();
        IN_GAME_MAIN_CAMERA.MainCamera.enabled = false;
        IN_GAME_MAIN_CAMERA.BaseCamera.GetComponent<CameraShake>().enabled = false;
        IN_GAME_MAIN_CAMERA.GameType = GameType.Multi;
        LoadSkinCheck();
        CustomLevel.OnLoadLevel();
        if (info.Mode == GameMode.TROST)
        {
            CacheGameObject.Find("playerRespawn").SetActive(false);
            UnityEngine.Object.Destroy(CacheGameObject.Find("playerRespawn"));
            GameObject gameObject3 = CacheGameObject.Find("rock");
            gameObject3.animation["lift"].speed = 0f;
            CacheGameObject.Find("door_fine").SetActive(false);
            CacheGameObject.Find("door_broke").SetActive(true);
            UnityEngine.Object.Destroy(CacheGameObject.Find("ppl"));
        }
        else if (info.Mode == GameMode.BOSS_FIGHT_CT)
        {
            CacheGameObject.Find("playerRespawnTrost").SetActive(false);
            UnityEngine.Object.Destroy(CacheGameObject.Find("playerRespawnTrost"));
        }
        if (this.NeedChooseSide)
        {
            this.ShowHUDInfoTopCenterADD("\n\nPRESS 1 TO ENTER GAME");
        }
        else
        {
            Screen.lockCursor = IN_GAME_MAIN_CAMERA.CameraMode >= CameraType.TPS;
            if (IN_GAME_MAIN_CAMERA.GameMode == GameMode.PVP_CAPTURE)
            {
                if ((int)PhotonNetwork.player.Properties[PhotonPlayerProperty.isTitan] == 2)
                {
                    this.checkpoint = CacheGameObject.Find("PVPchkPtT");
                }
                else
                {
                    this.checkpoint = CacheGameObject.Find("PVPchkPtH");
                }
            }
            if ((int)PhotonNetwork.player.Properties[PhotonPlayerProperty.isTitan] == 2)
            {
                this.SpawnNonAITitan(this.myLastHero, "titanRespawn");
            }
            else
            {
                this.SpawnPlayer(this.myLastHero);
            }
        }
        if (info.Mode == GameMode.BOSS_FIGHT_CT)
        {
            UnityEngine.Object.Destroy(CacheGameObject.Find("rock"));
        }
        if (PhotonNetwork.IsMasterClient)
        {
            if (info.Mode == GameMode.TROST)
            {
                if (!this.IsPlayerAllDead())
                {
                    GameObject gameObject4 = Optimization.Caching.Pool.NetworkEnable("TITAN_EREN_trost", new Vector3(-200f, 0f, -194f), Quaternion.Euler(0f, 180f, 0f), 0);
                    gameObject4.GetComponent<TITAN_EREN>().rockLift = true;
                    int rate2 = 90;
                    if (this.Difficulty == 1)
                    {
                        rate2 = 70;
                    }
                    GameObject[] array3 = GameObject.FindGameObjectsWithTag("titanRespawn");
                    GameObject gameObject5 = CacheGameObject.Find("titanRespawnTrost");
                    if (gameObject5 != null)
                    {
                        foreach (GameObject gameObject6 in array3)
                        {
                            if (gameObject6.transform.parent.gameObject == gameObject5)
                            {
                                this.SpawnTitan(rate2, gameObject6.transform.position, gameObject6.transform.rotation, false);
                            }
                        }
                    }
                }
            }
            else if (info.Mode == GameMode.BOSS_FIGHT_CT)
            {
                if (!this.IsPlayerAllDead())
                {
                    Optimization.Caching.Pool.NetworkEnable("COLOSSAL_TITAN", -Vectors.up * 10000f, Quaternion.Euler(0f, 180f, 0f), 0);
                }
            }
            else if (info.Mode == GameMode.KILL_TITAN || info.Mode == GameMode.ENDLESS_TITAN || info.Mode == GameMode.SURVIVE_MODE)
            {
                if (info.Name == "Annie" || info.Name == "Annie II")
                {
                    Optimization.Caching.Pool.NetworkEnable("FEMALE_TITAN", CacheGameObject.Find("titanRespawn").transform.position, CacheGameObject.Find("titanRespawn").transform.rotation, 0);
                }
                else
                {
                    int rate3 = 90;
                    if (this.Difficulty == 1)
                    {
                        rate3 = 70;
                    }
                    SpawnTitansCustom( rate3, info.EnemyNumber, false);
                }
            }
            else if (info.Mode != GameMode.TROST)
            {
                if (info.Mode == GameMode.PVP_CAPTURE && FengGameManagerMKII.Level.MapName == "OutSide")
                {
                    GameObject[] array5 = GameObject.FindGameObjectsWithTag("titanRespawn");
                    if (array5.Length <= 0)
                    {
                        return;
                    }
                    for (int k = 0; k < array5.Length; k++)
                    {
                        this.spawnTitanRaw(array5[k].transform.position, array5[k].transform.rotation).setAbnormalType(AbnormalType.Crawler, true);
                    }
                }
            }
        }
        if (!info.Supply)
        {
            UnityEngine.Object.Destroy(CacheGameObject.Find("aot_supply"));
        }
        if (!PhotonNetwork.IsMasterClient)
        {
            BasePV.RPC("RequireStatus", PhotonTargets.MasterClient, new object[0]);
        }
        if (Stylish != null)
        {
            Stylish.enabled = true;
        }
        if (FengGameManagerMKII.Level.LavaMode)
        {
            UnityEngine.Object.Instantiate(CacheResources.Load("levelBottom"), new Vector3(0f, -29.5f, 0f), Quaternion.Euler(0f, 0f, 0f));
            CacheGameObject.Find("aot_supply").transform.position = CacheGameObject.Find("aot_supply_lava_position").transform.position;
            CacheGameObject.Find("aot_supply").transform.rotation = CacheGameObject.Find("aot_supply_lava_position").transform.rotation;
        }
        roomInformation.UpdateLabels();
        Resources.UnloadUnusedAssets();
    }

    [RPC]
    private void pauseRPC(bool pause, PhotonMessageInfo info = null)
    {
        if(info != null && info.Sender.IsMasterClient)
        {
            if (pause)
            {
                AnarchyManager.PauseWindow.PauseWaitTime = 100000f;
                UnityEngine.Time.timeScale = 1E-06f;
                if (!AnarchyManager.PauseWindow.Active)
                {
                    AnarchyManager.PauseWindow.EnableImmediate();
                }
            }
            else
            {
                AnarchyManager.PauseWindow.PauseWaitTime = 3f;
            }
        }

    }

    [RPC]
    private void refreshPVPStatus(int score1, int score2, PhotonMessageInfo info)
    {
        if (info != null && !info.Sender.IsMasterClient && Logic.Mode != GameMode.PVP_CAPTURE)
        {
            Log.AddLine("RPCerror", MsgType.Error, info.Sender.ID.ToString(), nameof(refreshPVPStatus));
            return;
        }
        if (Logic is GameLogic.PVPCaptureLogic log)
        {
            log.PVPHumanScore = score1;
            log.PVPTitanScore = score2;
        }
    }

    [RPC]
    private void refreshPVPStatus_AHSS(int[] score1, PhotonMessageInfo info)
    {
        if (info != null && !info.Sender.IsMasterClient)
        {
            Log.AddLine("RPCerror", MsgType.Error, info.Sender.ID.ToString(), nameof(refreshPVPStatus_AHSS));
            return;
        }
        if (Logic is GameLogic.PVPLogic log)
        {
            log.Scores = score1;
        }
    }

    private void RefreshRacingResult()
    {
        this.LocalRacingResult = "Result\n";
        IComparer comparer = new IComparerRacingResult();
        this.racingResult.Sort(comparer);
        int num = this.racingResult.Count;
        num = Mathf.Min(num, 6);
        for (int i = 0; i < num; i++)
        {
            string text = this.LocalRacingResult;
            this.LocalRacingResult = string.Concat(new object[]
            {
                text,
                "Rank ",
                i + 1,
                " : "
            });
            this.LocalRacingResult += (this.racingResult[i] as RacingResult).Name;
            this.LocalRacingResult = this.LocalRacingResult + "   " + ((float)((int)((this.racingResult[i] as RacingResult).Time * 100f)) * 0.01f).ToString() + "s";
            this.LocalRacingResult += "\n";
        }
        BasePV.RPC("netRefreshRacingResult", PhotonTargets.All, new object[]
        {
            this.LocalRacingResult
        });
    }

    [RPC]
    private void refreshStatus(int score1, int score2, int wav, int highestWav, float time1, float time2, bool startRacin, bool endRacin, PhotonMessageInfo info)
    {
        if (info != null && !info.Sender.IsMasterClient)
        {
            Log.AddLine("RPCerror", MsgType.Error, info.Sender.ID.ToString(), nameof(refreshStatus));
            return;
        }
        Logic.OnRefreshStatus(score1, score2, wav, highestWav, time1, time2, startRacin, endRacin);
    }

    [RPC]
    private void RequireStatus(PhotonMessageInfo info)
    {
        if (info != null && !PhotonNetwork.IsMasterClient)
        {
            Log.AddLine("RPCerror", MsgType.Error, info.Sender.ID.ToString(), nameof(RequireStatus));
            return;
        }
        Logic.OnRequireStatus();
    }

    [RPC]
    private void respawnHeroInNewRound()
    {
        if (this.NeedChooseSide)
        {
            return;
        }
        if (IN_GAME_MAIN_CAMERA.MainCamera.gameOver)
        {
            this.SpawnPlayer(this.myLastHero);
            IN_GAME_MAIN_CAMERA.MainCamera.gameOver = false;
            this.ShowHUDInfoCenter(string.Empty);
        }
    }

    [RPC]
    private void restartGameByClient(PhotonMessageInfo info = null)
    {
    }

    [RPC]
    private void RPCLoadLevel(PhotonMessageInfo info = null)
    {
        if (info.Sender.IsMasterClient)
        {
            AnarchyManager.Log.Disable();
            AnarchyManager.Chat.Disable();
            DestroyAllExistingCloths();
            PhotonNetwork.LoadLevel(FengGameManagerMKII.Level.MapName);
        }
    }

    [RPC]
    private void setMasterRC()
    {

    }

    private void setTeam(int team)
    {
        string nameColor = string.Empty;
        switch (team)
        {
            case 3:
                int cyans = 0;
                int magentas = 0;
                foreach(PhotonPlayer player in PhotonNetwork.playerList)
                {
                    if (player.RCteam == 1)
                        cyans++;
                    else if (player.RCteam == 2)
                        magentas++;
                }
                setTeam(cyans > magentas ? 1 : 2);
                return;

            case 1:
                nameColor = Colors.cyan.ColorToString();
                break;

            case 2:
                nameColor = Colors.magenta.ColorToString();
                break;

            default:
                break;
        }
        if(nameColor != string.Empty)
        {
            PhotonNetwork.player.UIName = $"[{nameColor}]{PhotonNetwork.player.UIName.RemoveHex()}";
        }
        else
        {
            PhotonNetwork.player.UIName = User.Name.Value;
        }
        PhotonNetwork.player.RCteam = team;
        if (team > 0 && team < 3)
        {
            foreach (HERO hero in heroes)
            {
                if (hero.IsLocal)
                {
                    BasePV.RPC("labelRPC", PhotonTargets.All, new object[] { hero.BasePV.viewID });
                }
            }
        }
    }

    [RPC]
    private void setTeamRPC(int team, PhotonMessageInfo info)
    {
        if(info != null && !info.Sender.IsMasterClient)
        {
            Log.AddLine("RPCerror", MsgType.Error, info.Sender.ID.ToString(), nameof(setTeamRPC));
            return;
        }
        setTeam(team);
    }

    [RPC]
    private void settingRPC(ExitGames.Client.Photon.Hashtable hash, PhotonMessageInfo info)
    {
        if (info != null && !info.Sender.IsMasterClient)
        {
            Log.AddLine("RPCerror", MsgType.Error, info.Sender.ID.ToString(), nameof(settingRPC));
            return;
        }
        if (info.Sender.IsMasterClient)
        {
            GameModes.HandleRPC(hash);
        }
    }

    #region Show UILabels
    internal void ShowHUDInfoCenter(string content)
    {
        Labels.Center = content.ToHTMLFormat();
    }

    internal void ShowHUDInfoCenterADD(string content)
    {
        Labels.Center = content.ToHTMLFormat();
    }

    internal void ShowHUDInfoTopCenter(string content)
    {
        Labels.TopCenter = content.ToHTMLFormat();
    }

    internal void ShowHUDInfoTopCenterADD(string content)
    {
        Labels.TopCenter = content.ToHTMLFormat();
    }

    internal void ShowHUDInfoTopLeft(string content)
    {
        Labels.TopLeft= content.ToHTMLFormat();
    }

    internal void ShowHUDInfoTopRight(string content)
    {
        Labels.TopRight = content.ToHTMLFormat();
    }

    internal void ShowHUDInfoTopRightMAPNAME(string content)
    {
        Labels.TopRight += content.ToHTMLFormat();
    }
    #endregion

    [RPC]
    private void showResult(string text0, string text1, string text2, string text3, string text4, string text6, PhotonMessageInfo info)
    {
        if (info != null && !info.Sender.IsMasterClient)
        {
            Log.AddLine("RPCerror", MsgType.Error, info.Sender.ID.ToString(), nameof(showResult));
            return;
        }
        if (this.gameTimesUp)
        {
            return;
        }
        this.gameTimesUp = true;
        NGUITools.SetActive(UIRefer.panels[0], false);
        NGUITools.SetActive(UIRefer.panels[1], false);
        NGUITools.SetActive(UIRefer.panels[2], true);
        NGUITools.SetActive(UIRefer.panels[3], false);
        CacheGameObject.Find<UILabel>("LabelName").text = text0;
        CacheGameObject.Find<UILabel>("LabelKill").text = text1;
        CacheGameObject.Find<UILabel>("LabelDead").text = text2;
        CacheGameObject.Find<UILabel>("LabelMaxDmg").text = text3;
        CacheGameObject.Find<UILabel>("LabelTotalDmg").text = text4;
        CacheGameObject.Find<UILabel>("LabelResultTitle").text = text6;
        Screen.lockCursor = false;
        Screen.showCursor = true;
        IN_GAME_MAIN_CAMERA.GameType = GameType.Stop;
        this.GameStart = false;
    }

    [RPC]
    private void spawnPlayerAtRPC(float posX, float posY, float posZ, PhotonMessageInfo info)
    {
        if(info.Sender.IsMasterClient && CustomLevel.logicLoaded && CustomLevel.customLevelLoaded && !NeedChooseSide && IN_GAME_MAIN_CAMERA.MainCamera.gameOver)
        {
            Vector3 pos = new Vector3(posX, posY, posZ);
            IN_GAME_MAIN_CAMERA component = IN_GAME_MAIN_CAMERA.MainCamera;
            string id = myLastHero.ToUpper();
            this.myLastHero = id;
            component.SetMainObject(Pool.NetworkEnable("AOTTG_HERO 1", pos, Quaternion.identity, 0).GetComponent<HERO>(), true, false);
            id = id.ToUpper();
            if (id == "SET 1" || id == "SET 2" || id == "SET 3")
            {
                HeroCostume heroCostume2 = CostumeConeveter.LocalDataToHeroCostume(id);
                heroCostume2.Checkstat();
                CostumeConeveter.HeroCostumeToLocalData(heroCostume2, id);
                IN_GAME_MAIN_CAMERA.MainHERO.Setup.Init();
                if (heroCostume2 != null)
                {
                    IN_GAME_MAIN_CAMERA.MainHERO.Setup.myCostume = heroCostume2;
                    IN_GAME_MAIN_CAMERA.MainHERO.Setup.myCostume.stat = heroCostume2.stat;
                }
                else
                {
                    heroCostume2 = HeroCostume.costumeOption[3];
                    IN_GAME_MAIN_CAMERA.MainHERO.Setup.myCostume = heroCostume2;
                    IN_GAME_MAIN_CAMERA.MainHERO.Setup.myCostume.stat = HeroStat.getInfo(heroCostume2.name.ToUpper());
                }
                IN_GAME_MAIN_CAMERA.MainHERO.Setup.SetCharacterComponent();
                IN_GAME_MAIN_CAMERA.MainHERO.setStat();
                IN_GAME_MAIN_CAMERA.MainHERO.setSkillHUDPosition();
            }
            else
            {
                for (int j = 0; j < HeroCostume.costume.Length; j++)
                {
                    if (HeroCostume.costume[j].name.ToUpper() == id.ToUpper())
                    {
                        int num2 = HeroCostume.costume[j].id;
                        if (id.ToUpper() != "AHSS")
                        {
                            num2 += CheckBoxCostume.costumeSet - 1;
                        }
                        if (HeroCostume.costume[num2].name != HeroCostume.costume[j].name)
                        {
                            num2 = HeroCostume.costume[j].id + 1;
                        }
                        IN_GAME_MAIN_CAMERA.MainHERO.Setup.Init();
                        IN_GAME_MAIN_CAMERA.MainHERO.Setup.myCostume = HeroCostume.costume[num2];
                        IN_GAME_MAIN_CAMERA.MainHERO.Setup.myCostume.stat = HeroStat.getInfo(HeroCostume.costume[num2].name.ToUpper());
                        IN_GAME_MAIN_CAMERA.MainHERO.Setup.SetCharacterComponent();
                        IN_GAME_MAIN_CAMERA.MainHERO.setStat();
                        IN_GAME_MAIN_CAMERA.MainHERO.setSkillHUDPosition();
                        break;
                    }
                }
            }
            CostumeConeveter.HeroCostumeToPhotonData(IN_GAME_MAIN_CAMERA.MainHERO.Setup.myCostume, PhotonNetwork.player);
            if (IN_GAME_MAIN_CAMERA.GameMode == GameMode.PVP_CAPTURE)
            {
                IN_GAME_MAIN_CAMERA.MainT.position += new Vector3((float)UnityEngine.Random.Range(-20, 20), 2f, (float)UnityEngine.Random.Range(-20, 20));
            }
            PhotonNetwork.player.Dead = false;
            PhotonNetwork.player.IsTitan = false;
            component.enabled = true;
            IN_GAME_MAIN_CAMERA.MainCamera.setHUDposition();
            IN_GAME_MAIN_CAMERA.SpecMov.disable = true;
            IN_GAME_MAIN_CAMERA.Look.disable = true;
            component.gameOver = false;
            Screen.lockCursor = IN_GAME_MAIN_CAMERA.CameraMode >= CameraType.TPS;
            Screen.showCursor = false;
            this.ShowHUDInfoCenter(string.Empty);
        }
    }

    private TITAN spawnTitanRaw(Vector3 position, Quaternion rotation)
    {
        if (IN_GAME_MAIN_CAMERA.GameType == GameType.Single)
        {
            return ((GameObject)UnityEngine.Object.Instantiate(CacheResources.Load("TITAN_VER3.1"), position, rotation)).GetComponent<TITAN>();
        }
        else
        {
            return Optimization.Caching.Pool.NetworkEnable("TITAN_VER3.1", position, rotation, 0).GetComponent<TITAN>();
        }
    }

    [RPC]
    private void spawnTitanRPC(PhotonMessageInfo info)
    {
        if (info.Sender.IsMasterClient)
        {
            foreach (TITAN obj in titans)
            {
                if(obj.BasePV.IsMine && (!PhotonNetwork.IsMasterClient || obj.nonAI))
                {
                    PhotonNetwork.Destroy(obj.BasePV);
                }
            }
            SpawnNonAITitan(this.myLastHero, "titanRespawn");
        }
    }

    private void Start()
    {
        ChangeQuality.setCurrentQuality();
        name = "MultiplayerManager";
        DontDestroyOnLoad(this);
        HeroCostume.Init();
        CharacterMaterials.Init();
        RCManager.Clear();
        this.heroes = new List<HERO>();
        this.titans = new List<TITAN>();
        this.hooks = new List<Bullet>();
        AnarchyManager.DestroyMainScene();
    }

    private void Update()
    {
        FPS.FPSUpdate();
        if (IN_GAME_MAIN_CAMERA.GameType != GameType.Single)
        {
            Labels.NetworkStatus = PhotonNetwork.connectionStateDetailed.ToString() + (PhotonNetwork.connected ? " ping: " + PhotonNetwork.GetPing() : "");
        }
        if (!GameStart)
            return;
        foreach (var h in heroes)
        {
            h.update();
        }
        foreach (var b in hooks)
        {
            b.update();
        }
        if (IN_GAME_MAIN_CAMERA.GameType == GameType.Single || PhotonNetwork.IsMasterClient)
        {
            foreach (var t in titans)
            {
                t.update();
            }
        }
        if (mainCamera != null)
        {
            mainCamera.update();
            mainCamera.snapShotUpdate();
        }
        Logic.OnUpdate();
    }

    [RPC]
    private void updateKillInfo(bool t1, string killer, bool t2, string victim, int dmg, PhotonMessageInfo info)
    {
        if (info != null && !info.Sender.IsMasterClient && dmg != 0 && !t1)
        {
            Log.AddLineRaw($"t1:{t1},killer:{killer},t2:{t2},victim:{victim},dmg:{dmg}", MsgType.Warning);
            Log.AddLine("RPCerror", MsgType.Error, info.Sender.ID.ToString(), nameof(updateKillInfo));
            return;
        }
        killer = killer.LimitToLengthStripped(50);
        victim = victim.LimitToLengthStripped(50);
        var killInfo = ((GameObject)Instantiate(CacheResources.Load("UI/KillInfo"))).GetComponent<KillInfoComponent>();
        using (var ien = killInfoList.GetEnumerator())
        {
            while (ien.MoveNext())
            {
                if (ien.Current != null)
                {
                    ien.Current.moveOn();
                }
            }
        }
        if (killInfoList.Count > 4)
        {
            killInfoList.Dequeue().destroy();
        }
        killInfo.SetParent(UIRefer.panels[0].transform);
        killInfo.Show(t1, killer, t2, victim, dmg);
        killInfoList.Enqueue(killInfo);
        if (Settings.GameFeed)
        {
            Anarchy.UI.Chat.Add($"<color=#FFC000>({Logic.RoundTime.ToString("F2")})</color> {killer.ToHTMLFormat()} killed {victim.ToHTMLFormat()} for {dmg} damage.");
        }
    }

    [RPC]
    private void verifyPlayerHasLeft(int ID, PhotonMessageInfo info)
    {
        if (ID < 0)
        {
            //Newest cyan detection
        }
    }

    public void AddCamera(IN_GAME_MAIN_CAMERA c)
    {
        this.mainCamera = c;
    }

    public void AddCT(COLOSSAL_TITAN titan)
    {
        colossal = titan;
    }

    public void AddET(TITAN_EREN hero)
    {
        eren = hero;
    }

    public void AddFT(FEMALE_TITAN titan)
    {
        annie = titan;
    }

    public void AddHero(HERO hero)
    {
        this.heroes.Add(hero);
    }

    public void AddHook(Bullet h)
    {
        this.hooks.Add(h);
    }

    public void AddTitan(TITAN titan)
    {
        this.titans.Add(titan);
    }

    public void CheckPVPpts()
    {
        if(Logic is GameLogic.PVPCaptureLogic cap)
        {
            cap.CheckPVPpts();
        }
    }

    public void DestroyAllExistingCloths()
    {
        Cloth[] array = UnityEngine.Object.FindObjectsOfType<Cloth>();
        bool flag = array.Length != 0;
        if (flag)
        {
            for (int i = 0; i < array.Length; i++)
            {
                ClothFactory.DisposeObject(array[i].gameObject);
            }
        }
    }

    public void GameLose()
    {
        Logic.GameLose();
    }

    public void GameWin()
    {
        Logic.GameWin();
    }

    public bool IsPlayerAllDead()
    {
        foreach(PhotonPlayer player in PhotonNetwork.playerList)
        {
            if (player.IsTitan || player.Dead) continue;
            return false;
        }
        return true;
    }

    public bool IsTeamAllDead(int team)
    {
        foreach(PhotonPlayer player in PhotonNetwork.playerList)
        {
            if (player.IsTitan) continue;
            if (player.Team == team && !player.Dead) return false;

        }
        return true;
    }

    public void KillInfoUpdate()
    {
        if (killInfoList.Count > 0 && killInfoList.Peek() == null)
            killInfoList.Dequeue();
    }

    public void MultiplayerRacingFinsih()
    {
        float num = Logic.RoundTime - (Logic as GameLogic.RacingLogic).StartTime;
        if (PhotonNetwork.IsMasterClient)
        {
            this.getRacingResult(User.RaceName, num, new PhotonMessageInfo());
        }
        else
        {
            BasePV.RPC("getRacingResult", PhotonTargets.MasterClient, new object[]
            {
                User.RaceName,
                num
            });
        }
        this.GameWin();
    }

    [RPC]
    public void netShowDamage(int speed, PhotonMessageInfo info)
    {
        if (info != null && !info.Sender.IsMasterClient)
        {
            Log.AddLine("RPCerror", MsgType.Error, info.Sender.ID.ToString(), nameof(netShowDamage));
            return;
        }
        Stylish?.Style(speed);
        //CacheGameObject.Find<StylishComponent>("Stylish").Style(speed);
        var target = CacheGameObject.Find<UILabel>("LabelScore");
        if (target != null)
        {
            target.text = speed.ToString();
            target.transform.localScale = Vectors.zero;
            speed = (int)(speed * 0.1f);
            speed = Mathf.Max(40, speed);
            speed = Mathf.Min(150, speed);
            iTween.Stop(target.cachedGameObject);
            iTween.ScaleTo(target.cachedGameObject, new System.Collections.Hashtable() { { "x", speed }, { "y", speed }, { "z", speed }, { "easetype", iTween.EaseType.easeOutElastic }, { "time", 1f } });
            iTween.ScaleTo(target.cachedGameObject, itweenHash);
        }
    }

    public void NOTSpawnNonAITitan(string id)
    {
        this.myLastHero = id.ToUpper();
        PhotonNetwork.player.Dead = true;
        PhotonNetwork.player.IsTitan = true;
        Screen.lockCursor = IN_GAME_MAIN_CAMERA.CameraMode >= CameraType.TPS;
        Screen.showCursor = true;
        this.ShowHUDInfoCenter("the game has started for 60 seconds.\n please wait for next round.\n Click Right Mouse Key to Enter or Exit the Spectator Mode.");
        IN_GAME_MAIN_CAMERA.MainCamera.enabled = true;
        IN_GAME_MAIN_CAMERA.MainCamera.SetMainObject(null, true, false);
        IN_GAME_MAIN_CAMERA.MainCamera.setSpectorMode(true);
        IN_GAME_MAIN_CAMERA.MainCamera.gameOver = true;
    }

    public void NOTSpawnPlayer(string id)
    {
        this.myLastHero = id.ToUpper();
        PhotonNetwork.player.Dead = true;
        PhotonNetwork.player.IsTitan = false;
        if (!Level.Name.StartsWith("Custom"))
        {
            Screen.lockCursor = IN_GAME_MAIN_CAMERA.CameraMode >= CameraType.TPS;
            Screen.showCursor = false;
            IN_GAME_MAIN_CAMERA.MainCamera.setSpectorMode(true);
        }
        IN_GAME_MAIN_CAMERA.MainCamera.enabled = true;
        IN_GAME_MAIN_CAMERA.MainCamera.SetMainObject(null, true, false);
        IN_GAME_MAIN_CAMERA.MainCamera.gameOver = true;
    }

    public void OnConnectedToMaster(AOTEventArgs args)
    {
        UnityEngine.MonoBehaviour.print("OnConnectedToMaster");
    }

    public void OnConnectedToPhoton(AOTEventArgs args)
    {
        UnityEngine.MonoBehaviour.print("OnConnectedToPhoton");
    }

    public void OnConnectionFail(AOTEventArgs args)
    {

        UnityEngine.MonoBehaviour.print("OnConnectionFail : " + args.DisconnectCause.ToString());
        Screen.lockCursor = false;
        Screen.showCursor = true;
        IN_GAME_MAIN_CAMERA.GameType = GameType.Stop;
        this.GameStart = false;
        NGUITools.SetActive(UIRefer.panels[0], false);
        NGUITools.SetActive(UIRefer.panels[1], false);
        NGUITools.SetActive(UIRefer.panels[2], false);
        NGUITools.SetActive(UIRefer.panels[3], false);
        NGUITools.SetActive(UIRefer.panels[4], true);
        CacheGameObject.Find("LabelDisconnectInfo").GetComponent<UILabel>().text = "OnConnectionFail : " + args.DisconnectCause.ToString();
    }

    public void OnCreatedRoom(AOTEventArgs args)
    {
        this.racingResult = new ArrayList();
        GameModes.Load();
        GameModes.ForceChange();
        GameModes.SendRPC();
    }

    public void OnCustomAuthenticationFailed()
    {
        UnityEngine.MonoBehaviour.print("OnCustomAuthenticationFailed");
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
        NetworkingPeer.RemoveEvent(PhotonNetworkingMessage.OnPhotonPlayerPropertiesChanged, OnPhotonPlayerPropertiesChanged);
        levelSkin = null;
    }

    public void OnDisconnectedFromPhoton(AOTEventArgs args)
    {
        UnityEngine.MonoBehaviour.print("OnDisconnectedFromPhoton");
        Screen.lockCursor = false;
        Screen.showCursor = true;
    }

    [RPC]
    public void oneTitanDown(string name1 = "", bool onPlayerLeave = false, PhotonMessageInfo info = null)
    {
        if(info != null && !PhotonNetwork.IsMasterClient)
        {
            Log.AddLine("RPCerror", MsgType.Error, info.Sender.ID.ToString(), nameof(oneTitanDown));
            return;
        }
        Logic.OnTitanDown(name1, onPlayerLeave);
    }

    public void OnFailedToConnectToPhoton(AOTEventArgs args)
    {
        UnityEngine.MonoBehaviour.print("OnFailedToConnectToPhoton");
    }

    public void OnJoinedLobby(AOTEventArgs args)
    {

    }

    public void OnJoinedRoom(AOTEventArgs args)
    {
        string[] strArray = PhotonNetwork.room.name.Split('`');
        gameTimesUp = false;
        Level = LevelInfo.GetInfo(strArray[1]);
        switch (strArray[2].ToLower())
        {
            case "normal":  
                Difficulty = 0;
                break;
            case "hard":
                Difficulty = 1;
                break;
            case "abnormal":
                Difficulty = 2;
                break;
            default:
                Difficulty = 1;
                break;
        }
        IN_GAME_MAIN_CAMERA.Difficulty = this.Difficulty;
        Time = int.Parse(strArray[3]) * 60;
        Logic.ServerTimeBase = (float)Time;
        Logic.ServerTime = Time;
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
        PhotonPlayer player = PhotonNetwork.player;
        player.RCIgnored = false;
        player.UIName = User.Name;
        player.GuildName = User.AllGuildNames;
        player.Kills = player.Deaths = player.Max_Dmg = player.Total_Dmg = 0;
        player.RCteam = 0;
        player.Dead = true;
        player.IsTitan = false;
        LocalRacingResult = string.Empty;
        NeedChooseSide = true;
        foreach(var info in killInfoList)
        {
            info.destroy();
        }
        killInfoList.Clear();
        RCManager.racingSpawnPointSet = false;
        if (!PhotonNetwork.IsMasterClient)
        {
            BasePV.RPC("RequireStatus", PhotonTargets.MasterClient, new object[0]);
        }
    }

    public void OnLeftLobby(AOTEventArgs args)
    {
        UnityEngine.MonoBehaviour.print("OnLeftLobby");
    }

    public void OnLeftRoom(AOTEventArgs args)
    {
        UnityEngine.MonoBehaviour.print("OnLeftRoom");
        if (Application.loadedLevel != 0)
        {
            UnityEngine.Time.timeScale = 1f;
            if (PhotonNetwork.connected)
            {
                PhotonNetwork.Disconnect();
            }
            IN_GAME_MAIN_CAMERA.GameType = GameType.Stop;
            this.GameStart = false;
            Screen.lockCursor = false;
            Screen.showCursor = true;
            InputManager.MenuOn = false;
            levelSkin = null;
            DestroyAllExistingCloths();
            UnityEngine.Object.Destroy(FengGameManagerMKII.FGM);
            Application.LoadLevel("menu");
        }
        RCManager.racingSpawnPointSet = false;
    }

    public void OnMasterClientSwitched(AOTEventArgs args)
    {
        UnityEngine.MonoBehaviour.print("OnMasterClientSwitched");
        if (this.gameTimesUp)
        {
            return;
        }
        if (PhotonNetwork.IsMasterClient)
        {
            GameModes.Load();
            GameModes.ForceChange();
            this.RestartGame(true);
        }
    }

    public void OnPhotonCreateRoomFailed(AOTEventArgs args)
    {
        UnityEngine.Debug.LogError("OnPhotonCreateRoomFailed");
    }

    public void OnPhotonCustomRoomPropertiesChanged(AOTEventArgs args)
    {
        UnityEngine.MonoBehaviour.print("OnPhotonCustomRoomPropertiesChanged");
    }

    public void OnPhotonJoinRoomFailed(AOTEventArgs args)
    {
        UnityEngine.MonoBehaviour.print("OnPhotonJoinRoomFailed");
    }

    public void OnPhotonPlayerConnected(AOTEventArgs args)
    {
        Log.AddLine("playerConnected", MsgType.Info, args.Player.ID.ToString(), args.Player.UIName.ToHTMLFormat());
        playerList?.Update();
        if (PhotonNetwork.IsMasterClient)
        {
            if (Level.Name.StartsWith("Custom"))
            {
                StartCoroutine(CustomLevel.SendRPCToPlayer(args.Player));
            }
            GameModes.SendRPCToPlayer(args.Player);
        }
    }

    public void OnPhotonPlayerDisconnected(AOTEventArgs args)
    {
        PhotonPlayer player = args.Player;
        if (player != null)
        {
            if (player.RCIgnored)
            {
                Log.AddLine("playerKicked", MsgType.Info, player.ID.ToString(), player.UIName.ToHTMLFormat());
            }
            else
            {
                Log.AddLine("playerLeft", MsgType.Info, player.ID.ToString(), player.UIName.ToHTMLFormat());
            }
        }
        playerList?.Update();
        if (!this.gameTimesUp)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                this.oneTitanDown(string.Empty, true);
                this.someOneIsDead(0, null);
            }
        }
    }

    public void OnPhotonPlayerPropertiesChanged(AOTEventArgs args)
    {
        playerList?.Update();
    }

    public void OnPhotonRandomJoinFailed(AOTEventArgs args)
    {
        UnityEngine.MonoBehaviour.print("OnPhotonRandomJoinFailed");
    }

    public void OnPhotonSerializeView(AOTEventArgs args)
    {
        UnityEngine.MonoBehaviour.print("OnPhotonSerializeView");
    }

    public void OnReceivedRoomListUpdate(AOTEventArgs args)
    {
    }

    public void OnUpdatedFriendList(AOTEventArgs args)
    {
        UnityEngine.MonoBehaviour.print("OnUpdatedFriendList");
    }

    public void PlayerKillInfoSingleUpdate(int dmg)
    {
        this.SingleKills++;
        this.SingleMax = Mathf.Max(dmg, this.SingleMax);
        this.SingleTotal += dmg;
    }

    public void PlayerKillInfoUpdate(PhotonPlayer player, int dmg)
    {
        player.Kills++;
        player.Max_Dmg = Mathf.Max(dmg, player.Max_Dmg);
        player.Total_Dmg += dmg;
    }

    public TITAN RandomSpawnOneTitan(int rate)
    {
        return this.SpawnTitan(rate, RespawnPositions.RandomTitanPos, Quaternion.identity, false);
    }

    public void RandomSpawnTitans(string place, int rate, int num, bool punk = false)
    {
        if (num == -1)
        {
            num = 1;
        }
        List<Vector3> list = new List<Vector3>(RespawnPositions.TitanPositions);
        if (list.Count <= 0)
        {
            return;
        }
        for (int i = 0; i < num; i++)
        {
            if (list.Count <= 0)
            {
                return;
            }
            int pos = Random.Range(0, list.Count);
            this.SpawnTitan(rate, list[pos], Quaternion.identity, punk);
            list.RemoveAt(pos);
        }
    }

    public void RemoveCT(COLOSSAL_TITAN titan)
    {
        titan = null;
    }

    public void RemoveET(TITAN_EREN hero)
    {
        eren = null;
    }

    public void RemoveFT(FEMALE_TITAN titan)
    {
        titan = null;
    }

    public void RemoveHero(HERO hero)
    {
        this.heroes.Remove(hero);
    }

    public void RemoveHook(Bullet h)
    {
        this.hooks.Remove(h);
    }

    public void RemoveTitan(TITAN titan)
    {
        this.titans.Remove(titan);
    }

    public void RestartGame(bool masterclientSwitched = false)
    {
        if (this.gameTimesUp)
        {
            return;
        }
        GameModes.OnRestart();
        this.checkpoint = null;
        Logic.RoundTime = 0f;
        Logic.MyRespawnTime = 0f;
        foreach (var info in killInfoList)
        {
            info.destroy();
        }
        killInfoList.Clear();
        this.racingResult = new ArrayList();
        this.ShowHUDInfoCenter(string.Empty);
        DestroyAllExistingCloths();
        PhotonNetwork.DestroyAll();
        BasePV.RPC("RPCLoadLevel", PhotonTargets.All, new object[0]);
        if (masterclientSwitched)
        {
            this.SendChatContentInfo(User.MasterClientSwitch);
        }
        else
        {
            this.SendChatContentInfo(User.MsgRestart);
        }
        GameModes.SendRPC();
    }

    public void RestartGameSingle()
    {
        this.checkpoint = null;
        this.SingleKills = 0;
        this.SingleMax = 0;
        this.SingleTotal = 0;
        Logic.RoundTime = 0f;
        Logic.MyRespawnTime = 0f;
        this.ShowHUDInfoCenter(string.Empty);
        DestroyAllExistingCloths();
        Application.LoadLevel(Application.loadedLevel);
    }

    public void SendChatContentInfo(string content)
    {
        BasePV.RPC("Chat", PhotonTargets.All, new object[]
        {
            content,
            string.Empty
        });
    }

    public void SendKillInfo(bool t1, string killer, bool t2, string victim, int dmg = 0)
    {
        BasePV.RPC("updateKillInfo", PhotonTargets.All, new object[]
        {
            t1,
            killer,
            t2,
            victim,
            dmg
        });
    }

    [RPC]
    public void someOneIsDead(int id, PhotonMessageInfo info)
    {
        if (info != null && !PhotonNetwork.IsMasterClient)
        {
            Log.AddLine("RPCerror", MsgType.Error, info.Sender.ID.ToString(), nameof(someOneIsDead));
            return;
        }
        Logic.OnSomeOneIsDead(id);
    }

    public void SpawnNonAITitan(string id, string tag = "titanRespawn")
    {
        GameObject[] array = GameObject.FindGameObjectsWithTag(tag);
        GameObject gameObject = array[UnityEngine.Random.Range(0, array.Length)];
        this.myLastHero = id.ToUpper();
        GameObject gameObject2;
        if (IN_GAME_MAIN_CAMERA.GameMode == GameMode.PVP_CAPTURE)
        {
            gameObject2 = Optimization.Caching.Pool.NetworkEnable("TITAN_VER3.1", this.checkpoint.transform.position + new Vector3((float)UnityEngine.Random.Range(-20, 20), 2f, (float)UnityEngine.Random.Range(-20, 20)), this.checkpoint.transform.rotation, 0);
        }
        else
        {
            gameObject2 = Optimization.Caching.Pool.NetworkEnable("TITAN_VER3.1", gameObject.transform.position, gameObject.transform.rotation, 0);
        }
        IN_GAME_MAIN_CAMERA.MainCamera.SetMainObject(gameObject2.GetComponent<TITAN>());
        gameObject2.GetComponent<TITAN>().nonAI = true;
        gameObject2.GetComponent<TITAN>().speed = 30f;
        gameObject2.GetComponent<TITAN_CONTROLLER>().enabled = true;
        if (id == "RANDOM" && UnityEngine.Random.Range(0, 100) < 7)
        {
            gameObject2.GetComponent<TITAN>().setAbnormalType(AbnormalType.Crawler, true);
        }
        IN_GAME_MAIN_CAMERA.MainCamera.enabled = true;
        IN_GAME_MAIN_CAMERA.SpecMov.disable = true;
        IN_GAME_MAIN_CAMERA.Look.disable = true;
        IN_GAME_MAIN_CAMERA.MainCamera.gameOver = false;
        ExitGames.Client.Photon.Hashtable customProperties = new ExitGames.Client.Photon.Hashtable
        {
            {
                "dead",
                false
            }
        };
        PhotonNetwork.player.SetCustomProperties(customProperties);
        customProperties = new ExitGames.Client.Photon.Hashtable
        {
            {
                PhotonPlayerProperty.isTitan,
                2
            }
        };
        PhotonNetwork.player.SetCustomProperties(customProperties);
        Screen.lockCursor = IN_GAME_MAIN_CAMERA.CameraMode >= CameraType.TPS;
        Screen.showCursor = true;
        this.ShowHUDInfoCenter(string.Empty);
    }

    public void SpawnPlayer(string id)
    {
        this.SpawnPlayerAt(id, myLastRespawnTag);
    }

    public void SpawnPlayerAt(string id, string tag = "")
    {
        if(!CustomLevel.logicLoaded || !CustomLevel.customLevelLoaded)
        {
            NOTSpawnPlayer(id);
            return;
        }
        myLastRespawnTag = tag;
        Vector3 pos;
        Quaternion rot = Quaternion.identity;
        if(tag != string.Empty)
        {
            GameObject[] positions = GameObject.FindGameObjectsWithTag(tag);
            if(positions.Length > 0)
            {
                pos = positions[UnityEngine.Random.Range(0, positions.Length)].transform.position;
            }
            else
            {
                pos = RespawnPositions.RandomHeroPos;
            }
        }
        else if (IN_GAME_MAIN_CAMERA.GameMode == GameMode.PVP_CAPTURE)
        {
            pos = this.checkpoint.transform.position;
        }
        else if (RCManager.racingSpawnPointSet)
        {
            pos = RCManager.racingSpawnPoint;
            rot = RCManager.racingSpawnPointRotation;
        }
        else if (Level.Name.StartsWith("Custom"))
        {
            List<Vector3> list = new List<Vector3>();
            switch (PhotonNetwork.player.RCteam)
            {
                case 0:
                    for (int i = 0; i < 2; i++)
                    {
                        string type = (i == 0) ? "C" : "M";
                        foreach (Vector3 vec in CustomLevel.spawnPositions["Player" + type])
                        {
                            list.Add(vec);
                        }
                    }
                    break;
                case 1:
                    using (List<Vector3>.Enumerator enumerator = CustomLevel.spawnPositions["PlayerC"].GetEnumerator())
                    {
                        while (enumerator.MoveNext())
                        {
                            Vector3 vec2 = enumerator.Current;
                            list.Add(vec2);
                        }
                    }
                    break;
                case 2:
                    using (List<Vector3>.Enumerator enumerator = CustomLevel.spawnPositions["PlayerM"].GetEnumerator())
                    {
                        while (enumerator.MoveNext())
                        {
                            Vector3 vec2 = enumerator.Current;
                            list.Add(vec2);
                        }
                    }
                    break;
                default:
                    foreach (Vector3 vec3 in CustomLevel.spawnPositions["PlayerM"])
                    {
                        list.Add(vec3);
                    }
                    break;
            }
            if (list.Count > 0)
            {
                pos = list[UnityEngine.Random.Range(0, list.Count)];
            }
            else
            {
                pos = RespawnPositions.RandomHeroPos;
            }
        }
        else
        {
            pos = RespawnPositions.RandomHeroPos;
        }
        IN_GAME_MAIN_CAMERA component = IN_GAME_MAIN_CAMERA.MainCamera;
        this.myLastHero = id.ToUpper();
        if (IN_GAME_MAIN_CAMERA.GameType == GameType.Single)
        {
            if (IN_GAME_MAIN_CAMERA.singleCharacter == "TITAN_EREN")
            {
                component.SetMainObject(((GameObject)UnityEngine.Object.Instantiate(CacheResources.Load("TITAN_EREN"), pos, Quaternion.identity)).GetComponent<TITAN_EREN>(), true, false);
            }
            else
            {
                component.SetMainObject(((GameObject)UnityEngine.Object.Instantiate(CacheResources.Load("AOTTG_HERO 1"), pos, rot)).GetComponent<HERO>(), true, false);
                if (IN_GAME_MAIN_CAMERA.singleCharacter == "SET 1" || IN_GAME_MAIN_CAMERA.singleCharacter == "SET 2" || IN_GAME_MAIN_CAMERA.singleCharacter == "SET 3")
                {
                    HeroCostume heroCostume = CostumeConeveter.LocalDataToHeroCostume(IN_GAME_MAIN_CAMERA.singleCharacter);
                    heroCostume.Checkstat();
                    CostumeConeveter.HeroCostumeToLocalData(heroCostume, IN_GAME_MAIN_CAMERA.singleCharacter);
                    IN_GAME_MAIN_CAMERA.MainHERO.Setup.Init();
                    if (heroCostume != null)
                    {
                        IN_GAME_MAIN_CAMERA.MainHERO.Setup.myCostume = heroCostume;
                        IN_GAME_MAIN_CAMERA.MainHERO.Setup.myCostume.stat = heroCostume.stat;
                    }
                    else
                    {
                        heroCostume = HeroCostume.costumeOption[3];
                        IN_GAME_MAIN_CAMERA.MainHERO.Setup.myCostume = heroCostume;
                        IN_GAME_MAIN_CAMERA.MainHERO.Setup.myCostume.stat = HeroStat.getInfo(heroCostume.name.ToUpper());
                    }
                    IN_GAME_MAIN_CAMERA.MainHERO.Setup.SetCharacterComponent();
                    IN_GAME_MAIN_CAMERA.MainHERO.setStat();
                    IN_GAME_MAIN_CAMERA.MainHERO.setSkillHUDPosition();
                }
                else
                {
                    for (int i = 0; i < HeroCostume.costume.Length; i++)
                    {
                        if (HeroCostume.costume[i].name.ToUpper() == IN_GAME_MAIN_CAMERA.singleCharacter.ToUpper())
                        {
                            int num = HeroCostume.costume[i].id + CheckBoxCostume.costumeSet - 1;
                            if (HeroCostume.costume[num].name != HeroCostume.costume[i].name)
                            {
                                num = HeroCostume.costume[i].id + 1;
                            }
                            IN_GAME_MAIN_CAMERA.MainHERO.Setup.Init();
                            IN_GAME_MAIN_CAMERA.MainHERO.Setup.myCostume = HeroCostume.costume[num];
                            IN_GAME_MAIN_CAMERA.MainHERO.Setup.myCostume.stat = HeroStat.getInfo(HeroCostume.costume[num].name.ToUpper());
                            IN_GAME_MAIN_CAMERA.MainHERO.Setup.SetCharacterComponent();
                            IN_GAME_MAIN_CAMERA.MainHERO.setStat();
                            IN_GAME_MAIN_CAMERA.MainHERO.setSkillHUDPosition();
                            break;
                        }
                    }
                }
            }
        }
        else
        {
            component.SetMainObject(Optimization.Caching.Pool.NetworkEnable("AOTTG_HERO 1", pos, Quaternion.identity, 0).GetComponent<HERO>(), true, false);
            id = id.ToUpper();
            if (id == "SET 1" || id == "SET 2" || id == "SET 3")
            {
                HeroCostume heroCostume2 = CostumeConeveter.LocalDataToHeroCostume(id);
                heroCostume2.Checkstat();
                CostumeConeveter.HeroCostumeToLocalData(heroCostume2, id);
                IN_GAME_MAIN_CAMERA.MainHERO.Setup.Init();
                if (heroCostume2 != null)
                {
                    IN_GAME_MAIN_CAMERA.MainHERO.Setup.myCostume = heroCostume2;
                    IN_GAME_MAIN_CAMERA.MainHERO.Setup.myCostume.stat = heroCostume2.stat;
                }
                else
                {
                    heroCostume2 = HeroCostume.costumeOption[3];
                    IN_GAME_MAIN_CAMERA.MainHERO.Setup.myCostume = heroCostume2;
                    IN_GAME_MAIN_CAMERA.MainHERO.Setup.myCostume.stat = HeroStat.getInfo(heroCostume2.name.ToUpper());
                }
                IN_GAME_MAIN_CAMERA.MainHERO.Setup.SetCharacterComponent();
                IN_GAME_MAIN_CAMERA.MainHERO.setStat();
                IN_GAME_MAIN_CAMERA.MainHERO.setSkillHUDPosition();
            }
            else
            {
                for (int j = 0; j < HeroCostume.costume.Length; j++)
                {
                    if (HeroCostume.costume[j].name.ToUpper() == id.ToUpper())
                    {
                        int num2 = HeroCostume.costume[j].id;
                        if (id.ToUpper() != "AHSS")
                        {
                            num2 += CheckBoxCostume.costumeSet - 1;
                        }
                        if (HeroCostume.costume[num2].name != HeroCostume.costume[j].name)
                        {
                            num2 = HeroCostume.costume[j].id + 1;
                        }
                        IN_GAME_MAIN_CAMERA.MainHERO.Setup.Init();
                        IN_GAME_MAIN_CAMERA.MainHERO.Setup.myCostume = HeroCostume.costume[num2];
                        IN_GAME_MAIN_CAMERA.MainHERO.Setup.myCostume.stat = HeroStat.getInfo(HeroCostume.costume[num2].name.ToUpper());
                        IN_GAME_MAIN_CAMERA.MainHERO.Setup.SetCharacterComponent();
                        IN_GAME_MAIN_CAMERA.MainHERO.setStat();
                        IN_GAME_MAIN_CAMERA.MainHERO.setSkillHUDPosition();
                        break;
                    }
                }
            }
            CostumeConeveter.HeroCostumeToPhotonData(IN_GAME_MAIN_CAMERA.MainHERO.Setup.myCostume, PhotonNetwork.player);
            if (IN_GAME_MAIN_CAMERA.GameMode == GameMode.PVP_CAPTURE)
            {
                IN_GAME_MAIN_CAMERA.MainT.position += new Vector3((float)UnityEngine.Random.Range(-20, 20), 2f, (float)UnityEngine.Random.Range(-20, 20));
            }
            PhotonNetwork.player.Dead = false;
            PhotonNetwork.player.IsTitan = false;
        }
        component.enabled = true;
        IN_GAME_MAIN_CAMERA.MainCamera.setHUDposition();
        IN_GAME_MAIN_CAMERA.SpecMov.disable = true;
        IN_GAME_MAIN_CAMERA.Look.disable = true;
        component.gameOver = false;
        Screen.lockCursor = IN_GAME_MAIN_CAMERA.CameraMode >= CameraType.TPS;
        Screen.showCursor = false;
        this.ShowHUDInfoCenter(string.Empty);
    }

    public TITAN SpawnTitan(int rate, Vector3 position, Quaternion rotation, bool punk = false)
    {
        TITAN tit = this.spawnTitanRaw(position, rotation);
        if (punk)
        {
            tit.setAbnormalType(AbnormalType.Punk, false);
        }
        else if (UnityEngine.Random.Range(0, 100) < rate)
        {
            if (IN_GAME_MAIN_CAMERA.Difficulty == 2)
            {
                if (UnityEngine.Random.Range(0f, 1f) < 0.7f || FengGameManagerMKII.Level.NoCrawler)
                {
                    tit.setAbnormalType(AbnormalType.Jumper, false);
                }
                else
                {
                    tit.setAbnormalType(AbnormalType.Crawler, false);
                }
            }
        }
        else if (IN_GAME_MAIN_CAMERA.Difficulty == 2)
        {
            if (UnityEngine.Random.Range(0f, 1f) < 0.7f || FengGameManagerMKII.Level.NoCrawler)
            {
                tit.setAbnormalType(AbnormalType.Jumper, false);
            }
            else
            {
                tit.setAbnormalType(AbnormalType.Crawler, false);
            }
        }
        else if (UnityEngine.Random.Range(0, 100) < rate)
        {
            if (UnityEngine.Random.Range(0f, 1f) < 0.8f || FengGameManagerMKII.Level.NoCrawler)
            {
                tit.setAbnormalType(AbnormalType.Aberrant, false);
            }
            else
            {
                tit.setAbnormalType(AbnormalType.Crawler, false);
            }
        }
        else if (UnityEngine.Random.Range(0f, 1f) < 0.8f || FengGameManagerMKII.Level.NoCrawler)
        {
            tit.setAbnormalType(AbnormalType.Jumper, false);
        }
        else
        {
            tit.setAbnormalType(AbnormalType.Crawler, false);
        }
        GameObject gameObject2;
        if (IN_GAME_MAIN_CAMERA.GameType == GameType.Single)
        {
            gameObject2 = Pool.Enable("FX/FXtitanSpawn", tit.transform.position, Quaternion.Euler(-90f, 0f, 0f));//(GameObject)UnityEngine.Object.Instantiate(CacheResources.Load("FX/FXtitanSpawn"), tit.transform.position, Quaternion.Euler(-90f, 0f, 0f));
        }
        else
        {
            gameObject2 = Optimization.Caching.Pool.NetworkEnable("FX/FXtitanSpawn", tit.transform.position, Quaternion.Euler(-90f, 0f, 0f), 0);
        }
        gameObject2.transform.localScale = tit.transform.localScale;
        return tit;
    }

    public void SpawnTitanAction(int type, float size, int health, int number)
    {
        Vector3 position = new Vector3(Random.Range(-400f, 400f), 0f, Random.Range(-400f, 400f));
        Quaternion rotation = new Quaternion(0f, 0f, 0f, 1f);
        if (CustomLevel.spawnPositions["Titan"].Count > 0)
        {
            position = CustomLevel.spawnPositions["Titan"][Random.Range(0, CustomLevel.spawnPositions["Titan"].Count)];
        }
        else
        {
            GameObject[] objArray = GameObject.FindGameObjectsWithTag("titanRespawn");
            if (objArray.Length != 0)
            {
                int index = UnityEngine.Random.Range(0, objArray.Length);
                GameObject obj2 = objArray[index];
                while (objArray[index] == null)
                {
                    index = UnityEngine.Random.Range(0, objArray.Length);
                    obj2 = objArray[index];
                }
                objArray[index] = null;
                position = obj2.transform.position;
                rotation = obj2.transform.rotation;
            }
        }
        for (int i = 0; i < number; i++)
        {
            TITAN titan = this.spawnTitanRaw(position, rotation);
            titan.hasSetLevel = true;
            titan.resetLevel(size);
            if ((float)health > 0f)
            {
                titan.armor = health;
            }
            titan.setAbnormalType((AbnormalType)type);
        }
    }

    public void SpawnTitanAtAction(int type, float size, int health, int number, float posX, float posY, float posZ)
    {
        Vector3 position = new Vector3(posX, posY, posZ);
        Quaternion rotation = new Quaternion(0f, 0f, 0f, 1f);
        for (int i = 0; i < number; i++)
        {
            TITAN obj2 = this.spawnTitanRaw(position, rotation);
            obj2.resetLevel(size);
            obj2.hasSetLevel = true;
            if ((float)health > 0f)
            {
                obj2.armor = health;
            }
            obj2.setAbnormalType((AbnormalType)type);
        }
    }

    public void SpawnTitansCustom(int rate, int count, bool punk = false)
    {
        int num = count;
        if (Level.Name.Contains("Custom"))
        {
            num = 5;
            if (RCManager.GameType.Value == 1)
            {
                num = 3;
            }
            else if(RCManager.GameType.Value == 2 || RCManager.GameType.Value == 3)
            {
                num = 0;
            }
        }
        else if (IN_GAME_MAIN_CAMERA.GameMode == GameMode.SURVIVE_MODE)
        {
            if (count == 3 && !punk && GameModes.CustomAmount.Enabled)
            {
                 num = GameModes.CustomAmount.GetInt(0);
            }
            else
            {
                if (punk)
                {
                    num = count;
                }
                else
                {
                    if (GameModes.TitansWaveAmount.Enabled)
                    {
                        num = Logic.Round.Wave * GameModes.TitansWaveAmount.GetInt(0);
                    }
                }
            }
        }
        else if (GameModes.CustomAmount.Enabled)
        {
            num = GameModes.CustomAmount.GetInt(0);
        }
        num = Mathf.Min(50, num);
        if (GameModes.SpawnRate.Enabled)
        {
            float[] rates = new float[5];
            for (int i = 0; i < rates.Length; i++)
            {
                rates[i] = GameModes.SpawnRate.GetFloat(i);
            }
            if (punk && GameModes.PunkOverride.Enabled)
            {
                rates = new float[5] { 0f, 0f, 0f, 0f, 100f };
            }
            List<Vector3> positions;
            if (CustomLevel.spawnPositions["Titan"].Count > 0)
            {
                positions = new List<Vector3>(CustomLevel.spawnPositions["Titan"]);
            }
            else
            {
                if(RespawnPositions.TitanPositions.Length > 0)
                {
                    positions = new List<Vector3>(RespawnPositions.TitanPositions);
                }
                else
                {
                    positions = new List<Vector3>(new Vector3[num].Select(x => new Vector3(Random.Range(-400f, 400f), 0f, Random.Range(-400f, 400f))).ToArray());
                }
            }
            float summ = rates[0] + rates[1] + rates[2] + rates[3] + rates[4];
            for (int i = 0; i < num; i++)
            {
                Vector3 position;
                if (positions.Count == 0)
                {
                    position = new Vector3(Random.Range(-400f, 400f), 0f, Random.Range(-400f, 400f));
                }
                else
                {
                    int index = Random.Range(0, positions.Count);
                    position = positions[index];
                    positions.RemoveAt(index);
                }
                float cRate = Random.Range(0, 100f);
                if(cRate <= summ)
                {
                    float startRate = 0f;
                    float endRate = 0f;
                    int type = 0;
                    for(int j = 0; j < 5; j++)
                    {
                        endRate += rates[j];
                        if(cRate >= startRate && cRate < endRate)
                        {
                            type = j;
                            break;
                        }
                        startRate += rates[j];
                    }
                    spawnTitanRaw(position, Quaternion.identity).setAbnormalType((AbnormalType)type, false);
                }
                else
                {
                    SpawnTitan(rate, position, Quaternion.identity, punk);
                }
            }
        }
        else
        {
            if (Level.Name.Contains("Custom"))
            {
                List<Vector3> positions;
                if (CustomLevel.spawnPositions["Titan"].Count > 0)
                {
                    positions = new List<Vector3>(CustomLevel.spawnPositions["Titan"]);
                }
                else
                {
                    if (RespawnPositions.TitanPositions.Length > 0)
                    {
                        positions = new List<Vector3>(RespawnPositions.TitanPositions);
                    }
                    else
                    {
                        positions = new List<Vector3>(new Vector3[num].Select(x => new Vector3(Random.Range(-400f, 400f), 0f, Random.Range(-400f, 400f))).ToArray());
                    }
                }
                for (int i = 0; i < num; i++)
                {
                    Vector3 position;
                    if (positions.Count == 0)
                    {
                        position = new Vector3(Random.Range(-400f, 400f), 0f, Random.Range(-400f, 400f));
                    }
                    else
                    {
                        int index = Random.Range(0, positions.Count);
                        position = positions[index];
                        positions.RemoveAt(index);
                    }
                    SpawnTitan(rate, position, Quaternion.identity, punk);
                }
            }
            else
            {
                RandomSpawnTitans("titanRespawn", rate, num, punk);
            }
        }
    }

    public void TitanGetKill(PhotonPlayer player, int Damage, string name)
    {
        Damage = Mathf.Max(10, Damage);
        BasePV.RPC("netShowDamage", player, new object[]
        {
            Damage
        });
        BasePV.RPC("oneTitanDown", PhotonTargets.MasterClient, new object[]
        {
            name,
            false
        });
        this.SendKillInfo(false, player.UIName, true, name, Damage);
        this.PlayerKillInfoUpdate(player, Damage);
    }

    public void TitanGetKillbyServer(int Damage, string name)
    {
        Damage = Mathf.Max(10, Damage);
        this.SendKillInfo(false, LoginFengKAI.player.name, true, name, Damage);
        this.netShowDamage(Damage, new PhotonMessageInfo());
        this.oneTitanDown(name, false);
        this.PlayerKillInfoUpdate(PhotonNetwork.player, Damage);
    }
}