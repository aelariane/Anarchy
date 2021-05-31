using Anarchy;
using Anarchy.Commands.Chat;
using Anarchy.Configuration;
using Anarchy.Configuration.Presets;
using Anarchy.Network;
using Anarchy.Skins;
using Anarchy.Skins.Maps;
using Anarchy.UI;
using Antis;
using GameLogic;
using Optimization;
using Optimization.Caching;
using RC;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using UnityEngine;
using MonoBehaviour = Photon.MonoBehaviour;

[SuppressMessage("ReSharper", "CheckNamespace")]
[SuppressMessage("ReSharper", "UnusedMember.Local")]
[SuppressMessage("ReSharper", "InconsistentNaming")]
internal partial class FengGameManagerMKII : MonoBehaviour
{
    //Basic AoTTG ApplicationID
    //public const string ApplicationId = "f1f6195c-df4a-40f9-bae5-4744c32901ef";
    public const string ApplicationId = "5578b046-8264-438c-99c5-fb15c71b6744";
    //public const string ApplicationId = "";

    private static readonly Hashtable itweenHash = new Hashtable
        {{"x", 0}, {"y", 0}, {"z", 0}, {"easetype", iTween.EaseType.easeInBounce}, {"time", 0.5f}, {"delay", 2f}};

    private static readonly Queue<KillInfoComponent> killInfoList = new Queue<KillInfoComponent>();
    public static FengGameManagerMKII FGM;
    public static FPSCounter FPS = new FPSCounter();
    public static bool LAN;
    public static LevelInfo Level;
    public static StylishComponent Stylish;
    private readonly RoomInformation roomInformation = new RoomInformation();
    private FEMALE_TITAN annie;
    public GameObject checkpoint;
    private COLOSSAL_TITAN colossal;
    public int difficulty;
    private TITAN_EREN eren;
    public bool gameStart;
    private bool gameTimesUp;
    private List<HERO> heroes;
    private List<Bullet> hooks;
    public bool justSuicide;
    private LevelSkin levelSkin;
    public string localRacingResult;
    public GameLogic.GameLogic logic;
    private IN_GAME_MAIN_CAMERA mainCamera;
    public string myLastHero;
    private string myLastRespawnTag = "";
    public bool needChooseSide;
    internal PlayerList PlayerList;
    private ArrayList racingResult;
    public int singleKills;
    public int singleMax;
    public int singleTotal;
    public int time = 600;
    private List<TITAN> titans;

    public static FEMALE_TITAN Annie => FGM.annie;
    public static COLOSSAL_TITAN Colossal => FGM.colossal;
    public static TITAN_EREN Eren => FGM.eren;
    public static List<HERO> Heroes => FGM.heroes;
    public bool IsLosing => logic.Round.IsLosing;
    public bool IsWinning => logic.Round.IsWinning;
    public static List<TITAN> Titans => FGM.titans;
    public static UIReferArray UIRefer { get; private set; }

    private bool CheckIsTitanAllDie()
    {
        if (titans.Any(tit => !tit.hasDie))
        {
            return false;
        }

        return annie == null;
    }

    [RPC]
    private void RequestAnarchyVersion(PhotonMessageInfo info)
    {
        if (info.Sender.CanRequestVersion)
        {
            BasePV.RPC(nameof(ReceiveAnarchyVersion), info.Sender, new object[] { AnarchyManager.AnarchyVersion.ToString(), true, AnarchyManager.CustomName });
        }
    }

    [RPC]
    private void ReceiveAnarchyVersion(string version, bool custom, string customVersion, PhotonMessageInfo info)
    {

    }

    [RPC]
    private void MarkMeAsNotAnarchy(PhotonMessageInfo info)
    {
        if (info.Sender.AnarchySync)
        {
            info.Sender.ModLocked = false;
            info.Sender.AnarchySync = false;
        }
    }

    private void LoadMapSkin(string n, string urls, string str, IList<string> skybox, PhotonMessageInfo info = null)
    {
        var checkUrls = urls.Split(',');
        var checkStr = str.Split(',');
        var checkData = new string[1 + 8 + checkStr.Length + 6];
        checkData[0] = n;
        var i = 1;
        for (var j = 0; i < 1 + 8; i++, j++)
        {
            checkData[i] = checkUrls[j];
        }

        for (var j = 0; i < 1 + 8 + checkStr.Length; i++, j++)
        {
            checkData[i] = checkStr[j];
        }

        for (var j = 0; i < checkData.Length; i++, j++)
        {
            checkData[i] = skybox[j];
        }

        if (levelSkin == null)
        {
            if (Level.Name.Contains("City"))
            {
                levelSkin = new CitySkin(checkData);
            }
            else if (Level.Name.Contains("Forest"))
            {
                levelSkin = new ForestSkin(checkData);
            }
        }

        if (levelSkin == null)
        {
            return;
        }

        Skin.Check(levelSkin, checkData);
    }

    private void LoadSkinCheck()
    {
        if (Level.Name.Contains("City"))
        {
            if (SkinSettings.SkinsCheck(SkinSettings.CitySkins))
            {
                if (SkinSettings.CitySet.Value != StringSetting.NotDefine)
                {
                    var set = new CityPreset(SkinSettings.CitySet);
                    set.Load();
                    var n = "";
                    for (var i = 0; i < 250; i++)
                    {
                        var val = Random.Range(0, 8);
                        n += val.ToString();
                    }

                    var urls = string.Join(",", set.Houses) + ",";
                    var urls2 = $"{set.Ground},{set.Wall},{set.Gate}";
                    var box = new string[6].Select(x => "").ToArray();
                    if (set.LinkedSkybox != StringSetting.NotDefine)
                    {
                        var boxSet = new SkyboxPreset(set.LinkedSkybox);
                        boxSet.Load();
                        box = boxSet.ToSkinData();
                    }

                    LoadMapSkin(n, urls, urls2, box);
                    if (IN_GAME_MAIN_CAMERA.GameType == GameType.MultiPlayer && PhotonNetwork.IsMasterClient &&
                        SkinSettings.CitySkins.Value != 2)
                    {
                        BasePV.RPC("loadskinRPC", PhotonTargets.OthersBuffered, n, urls, urls2, box);
                    }
                }
            }
        }
        else if (Level.MapName.Contains("Forest"))
        {
            if (SkinSettings.SkinsCheck(SkinSettings.ForestSkins))
            {
                if (SkinSettings.ForestSet.Value != StringSetting.NotDefine)
                {
                    var set = new ForestPreset(SkinSettings.ForestSet);
                    set.Load();
                    var n = "";
                    for (var i = 0; i < 150; i++)
                    {
                        var val = Random.Range(0, 8);
                        n += val.ToString();
                        if (set.RandomizePairs)
                        {
                            n += Random.Range(0, 8).ToString();
                        }
                        else
                        {
                            n += val.ToString();
                        }
                    }

                    var urls = string.Join(",", set.Trees) + ",";
                    var urls2 = string.Join(",", set.Leaves);
                    urls2 += "," + set.Ground;
                    var box = new string[6].Select(x => "").ToArray();
                    if (set.LinkedSkybox != StringSetting.NotDefine)
                    {
                        var boxSet = new SkyboxPreset(set.LinkedSkybox);
                        boxSet.Load();
                        box = boxSet.ToSkinData();
                    }

                    LoadMapSkin(n, urls, urls2, box);
                    if (IN_GAME_MAIN_CAMERA.GameType == GameType.MultiPlayer && PhotonNetwork.IsMasterClient &&
                        SkinSettings.ForestSkins.Value != 2)
                    {
                        BasePV.RPC("loadskinRPC", PhotonTargets.OthersBuffered, n, urls, urls2, box);
                    }
                }
            }
        }
    }

    private void RefreshRacingResult()
    {
        localRacingResult = "Result\n";
        IComparer comparer = new IComparerRacingResult();
        racingResult.Sort(comparer);
        var num = racingResult.Count;
        //num = Mathf.Min(num, RacingLogic.MaxFinishers);
        for (var i = 0; i < num; i++)
        {
            var text = localRacingResult;
            localRacingResult = string.Concat(text, "Rank ", i + 1, " : ");
            localRacingResult += (racingResult[i] as RacingResult)?.Name;
            localRacingResult = localRacingResult + "   " +
                                (int)(((RacingResult)racingResult[i]).Time * 100f) * 0.01f + "s";
            localRacingResult += "\n";
        }

        BasePV.RPC("netRefreshRacingResult", PhotonTargets.All, localRacingResult);
    }

    private void SetTeam(int team)
    {
        var nameColor = string.Empty;
        switch (team)
        {
            case 0:
                PhotonNetwork.player.RCteam = 0;
                PhotonNetwork.player.UIName = User.Name.Value;
                return;

            case 1:
                nameColor = Colors.cyan.ColorToString();
                break;

            case 2:
                nameColor = Colors.magenta.ColorToString();
                break;

            case 3:
                var cyans = 0;
                var magentas = 0;
                foreach (var player in PhotonNetwork.playerList)
                {
                    switch (player.RCteam)
                    {
                        case 1:
                            cyans++;
                            break;

                        case 2:
                            magentas++;
                            break;
                    }
                }

                SetTeam(cyans > magentas ? 1 : 2);
                return;
        }

        PhotonNetwork.player.UIName = nameColor != string.Empty ? $"[{nameColor}]{PhotonNetwork.player.UIName.RemoveHex()}" : User.Name.Value;
        PhotonNetwork.player.RCteam = team;
        if (team >= 0 && team < 3)
        {
            foreach (var hero in heroes.Where(hero => hero.IsLocal))
            {
                BasePV.RPC("labelRPC", PhotonTargets.All, hero.BasePV.viewID);
            }
        }
    }

    private static TITAN SpawnTitanRaw(Vector3 position, Quaternion rotation)
    {
        if (IN_GAME_MAIN_CAMERA.GameType == GameType.Single)
        {
            return ((GameObject)Instantiate(CacheResources.Load("TITAN_VER3.1"), position, rotation))
                .GetComponent<TITAN>();
        }

        return Pool.NetworkEnable("TITAN_VER3.1", position, rotation).GetComponent<TITAN>();
    }

    public void AddCamera(IN_GAME_MAIN_CAMERA c)
    {
        mainCamera = c;
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
        heroes.Add(hero);
    }

    public void AddHook(Bullet h)
    {
        hooks.Add(h);
    }

    public void AddTitan(TITAN titan)
    {
        titans.Add(titan);
    }

    public void CheckPVPpts()
    {
        if (logic is PVPCaptureLogic cap)
        {
            cap.CheckPVPpts();
        }
    }

    public void DestroyAllExistingCloths()
    {
        var array = FindObjectsOfType<Cloth>();
        var flag = array.Length != 0;
        if (flag)
        {
            for (var i = 0; i < array.Length; i++)
            {
                ClothFactory.DisposeObject(array[i].gameObject);
            }
        }
    }

    public void GameLose()
    {
        logic.GameLose();
    }

    public void GameWin()
    {
        logic.GameWin();
    }

    public static bool IsPlayerAllDead()
    {
        return PhotonNetwork.playerList.All(player => player.IsTitan || player.Dead);
    }

    public bool IsTeamAllDead(int team)
    {
        return PhotonNetwork.playerList.Where(player => !player.IsTitan).All(player => player.Team != team || player.Dead);
    }

    public void KillInfoUpdate()
    {
        if (killInfoList.Count > 0 && killInfoList.Peek() == null)
        {
            killInfoList.Dequeue();
        }
    }

    public void MultiplayerRacingFinish()
    {
        Debug.LogError(nameof(MultiplayerRacingFinish));
        var num = logic.RoundTime - ((RacingLogic)logic).StartTime;
        if (PhotonNetwork.IsMasterClient)
        {
            getRacingResult(User.RaceName, num);
        }
        else
        {
            BasePV.RPC("getRacingResult", PhotonTargets.MasterClient, User.RaceName, num);
        }

        GameWin();
    }

    public void NotSpawnNonAiTitan(string id)
    {
        myLastHero = id.ToUpper();
        PhotonNetwork.player.Dead = true;
        PhotonNetwork.player.IsTitan = true;
        Screen.lockCursor = IN_GAME_MAIN_CAMERA.CameraMode >= CameraType.TPS;
        Screen.showCursor = true;
        ShowHUDInfoCenter(
            "the game has started for 60 seconds.\n please wait for next round.\n Click Right Mouse Key to Enter or Exit the Spectator Mode.");
        IN_GAME_MAIN_CAMERA.MainCamera.enabled = true;
        IN_GAME_MAIN_CAMERA.MainCamera.SetMainObject(null);
        IN_GAME_MAIN_CAMERA.MainCamera.setSpectorMode(true);
        IN_GAME_MAIN_CAMERA.MainCamera.gameOver = true;
    }

    public void NotSpawnPlayer(string id)
    {
        myLastHero = id.ToUpper();
        PhotonNetwork.player.Dead = true;
        PhotonNetwork.player.IsTitan = false;
        Screen.lockCursor = IN_GAME_MAIN_CAMERA.CameraMode >= CameraType.TPS;
        Screen.showCursor = false;
        IN_GAME_MAIN_CAMERA.MainCamera.enabled = true;
        IN_GAME_MAIN_CAMERA.MainCamera.SetMainObject(null);
        IN_GAME_MAIN_CAMERA.MainCamera.setSpectorMode(true);
        IN_GAME_MAIN_CAMERA.MainCamera.gameOver = true;
    }

    private IEnumerator OnPhotonPlayerConnectedE(PhotonPlayer player)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (AnarchyManager.PauseWindow.IsActive)
            {
                BasePV.RPC("pauseRPC", player, true);
            }

            if (Level.Name.StartsWith("Custom"))
            {
                StartCoroutine(CustomLevel.SendRPCToPlayer(player));
            }
        }

        yield return new WaitForSeconds(1f);
        if (player.Properties[PhotonPlayerProperty.name] == null)
        {
            yield return new WaitForSeconds(0.15f);
        }

        Log.AddLine("playerConnected", MsgType.Info, player.ID.ToString(), player.UIName.ToHTMLFormat());
        PlayerList?.Update();
        if (PhotonNetwork.IsMasterClient)
        {
            yield return new WaitForSeconds(0.5f);
            GameModes.SendRpcToPlayer(player);
            if (GameModes.NoGuest.Enabled && player.UIName.RemoveHex().ToUpper().StartsWith("GUEST"))
            {
                AntisManager.Response(player.ID, false, "Anti-Guest");
            }
            else if (BanList.Banned(player.UIName.RemoveHex()))
            {
                AntisManager.Response(player.ID, false, "Banned");
            }
        }
    }

    public void PlayerKillInfoSingleUpdate(int dmg)
    {
        singleKills++;
        singleMax = Mathf.Max(dmg, singleMax);
        singleTotal += dmg;

        if (IN_GAME_MAIN_CAMERA.GameType == GameType.Single)
        {
            SingleRunStats.OnKill();
        }
    }

    public void PlayerKillInfoUpdate(PhotonPlayer player, int dmg)
    {
        player.Kills++;
        player.Max_Dmg = Mathf.Max(dmg, player.Max_Dmg);
        player.Total_Dmg += dmg;
    }

    public TITAN RandomSpawnOneTitan(int rate)
    {
        return SpawnTitan(rate, RespawnPositions.RandomTitanPos, Quaternion.identity);
    }

    public void RandomSpawnTitans(string place, int rate, int num, bool punk = false)
    {
        if (num == -1)
        {
            num = 1;
        }

        var list = new List<Vector3>(RespawnPositions.TitanPositions);
        if (list.Count <= 0)
        {
            return;
        }

        for (var i = 0; i < num; i++)
        {
            if (list.Count <= 0)
            {
                return;
            }

            var pos = Random.Range(0, list.Count);
            SpawnTitan(rate, list[pos], Quaternion.identity, punk);
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
        heroes.Remove(hero);
    }

    public void RemoveHook(Bullet h)
    {
        hooks.Remove(h);
    }

    public void RemoveTitan(TITAN titan)
    {
        titans.Remove(titan);
    }

    public void RestartGame(bool masterclientSwitched, bool restartManually)
    {
        if (gameTimesUp || logic.Restarting)
        {
            return;
        }

        GameModes.OnRestart();
        checkpoint = null;
        logic.Restarting = true;
        logic.RoundTime = 0f;
        logic.MyRespawnTime = 0f;
        foreach (var info in killInfoList)
        {
            info.destroy();
        }

        killInfoList.Clear();
        racingResult = new ArrayList();
        RCManager.ClearVariables();
        ShowHUDInfoCenter(string.Empty);
        DestroyAllExistingCloths();
        GameModes.SendRpc();
        PhotonNetwork.DestroyAll();
        BasePV.RPC("RPCLoadLevel", PhotonTargets.All);
        if (masterclientSwitched)
        {
            SendChatContentInfo(User.MasterClientSwitch);
        }
        else
        {
            if (!restartManually && User.MsgRestart.Length > 0)
            {
                SendChatContentInfo(User.MsgRestart);
            }
        }
    }

    public void RestartGameSingle()
    {
        checkpoint = null;
        singleKills = 0;
        singleMax = 0;
        singleTotal = 0;
        logic.RoundTime = 0f;
        logic.MyRespawnTime = 0f;
        ShowHUDInfoCenter(string.Empty);
        DestroyAllExistingCloths();
        Application.LoadLevel(Application.loadedLevel);
    }

    public void SendChatContentInfo(string content)
    {
        BasePV.RPC("Chat", PhotonTargets.All, content, string.Empty);
    }

    public void SendKillInfo(bool t1, string killer, bool t2, string victim, int dmg = 0)
    {
        BasePV.RPC("updateKillInfo", PhotonTargets.All, t1, killer, t2, victim, dmg);
    }

    public void SpawnNonAiTitan(string id, string find = "titanRespawn")
    {
        var array = GameObject.FindGameObjectsWithTag(find);
        var go = array[Random.Range(0, array.Length)];
        myLastHero = id.ToUpper();
        GameObject gameObject2;
        if (IN_GAME_MAIN_CAMERA.GameMode == GameMode.PVP_CAPTURE)
        {
            gameObject2 = Pool.NetworkEnable("TITAN_VER3.1",
                checkpoint.transform.position + new Vector3(Random.Range(-20, 20), 2f, Random.Range(-20, 20)),
                checkpoint.transform.rotation);
        }
        else
        {
            gameObject2 = Pool.NetworkEnable("TITAN_VER3.1", go.transform.position,
                go.transform.rotation);
        }

        IN_GAME_MAIN_CAMERA.MainCamera.SetMainObject(gameObject2.GetComponent<TITAN>());
        gameObject2.GetComponent<TITAN>().nonAI = true;
        gameObject2.GetComponent<TITAN>().speed = 30f;
        gameObject2.GetComponent<TITAN_CONTROLLER>().enabled = true;
        if (id == "RANDOM" && Random.Range(0, 100) < 7)
        {
            gameObject2.GetComponent<TITAN>().SetAbnormalType(AbnormalType.Crawler, true);
        }

        IN_GAME_MAIN_CAMERA.MainCamera.enabled = true;
        IN_GAME_MAIN_CAMERA.SpecMov.disable = true;
        IN_GAME_MAIN_CAMERA.Look.disable = true;
        IN_GAME_MAIN_CAMERA.MainCamera.gameOver = false;
        PhotonNetwork.player.Dead = false;
        PhotonNetwork.player.IsTitan = true;
        Screen.lockCursor = IN_GAME_MAIN_CAMERA.CameraMode >= CameraType.TPS;
        Screen.showCursor = true;
        ShowHUDInfoCenter(string.Empty);
    }

    public void SpawnPlayer(string id)
    {
        SpawnPlayerAt(id, myLastRespawnTag);
    }

    public void SpawnPlayerAt(string id, string find = "")
    {
        if (!CustomLevel.logicLoaded || !CustomLevel.customLevelLoaded)
        {
            NotSpawnPlayer(id);
            return;
        }

        myLastRespawnTag = find;
        Vector3 pos;
        var rot = Quaternion.identity;
        if (find != string.Empty)
        {
            var positions = GameObject.FindGameObjectsWithTag(find);
            if (positions.Length > 0)
            {
                pos = positions[Random.Range(0, positions.Length)].transform.position;
            }
            else
            {
                pos = RespawnPositions.RandomHeroPos;
            }
        }
        else if (IN_GAME_MAIN_CAMERA.GameMode == GameMode.PVP_CAPTURE)
        {
            pos = checkpoint.transform.position;
        }
        else if (RCManager.racingSpawnPointSet)
        {
            pos = RCManager.racingSpawnPoint;
            rot = RCManager.racingSpawnPointRotation;
        }
        else if (Level.Name.StartsWith("Custom"))
        {
            var list = new List<Vector3>();
            switch (PhotonNetwork.player.RCteam)
            {
                case 0:
                    for (var i = 0; i < 2; i++)
                    {
                        var type = i == 0 ? "C" : "M";
                        foreach (var vec in CustomLevel.spawnPositions["Player" + type])
                        {
                            list.Add(vec);
                        }
                    }

                    break;

                case 1:
                    using (var enumerator = CustomLevel.spawnPositions["PlayerC"].GetEnumerator())
                    {
                        while (enumerator.MoveNext())
                        {
                            var vec2 = enumerator.Current;
                            list.Add(vec2);
                        }
                    }

                    break;

                case 2:
                    using (var enumerator = CustomLevel.spawnPositions["PlayerM"].GetEnumerator())
                    {
                        while (enumerator.MoveNext())
                        {
                            var vec2 = enumerator.Current;
                            list.Add(vec2);
                        }
                    }

                    break;

                default:
                    foreach (var vec3 in CustomLevel.spawnPositions["PlayerM"])
                    {
                        list.Add(vec3);
                    }

                    break;
            }

            if (list.Count > 0)
            {
                pos = list[Random.Range(0, list.Count)];
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

        var component = IN_GAME_MAIN_CAMERA.MainCamera;
        myLastHero = id.ToUpper();
        if (IN_GAME_MAIN_CAMERA.GameType == GameType.Single)
        {
            if (IN_GAME_MAIN_CAMERA.singleCharacter == "TITAN_EREN")
            {
                component.SetMainObject(
                    ((GameObject)Instantiate(CacheResources.Load("TITAN_EREN"), pos, Quaternion.identity))
                    .GetComponent<TITAN_EREN>());
            }
            else
            {
                component.SetMainObject(((GameObject)Instantiate(CacheResources.Load("AOTTG_HERO 1"), pos, rot))
                    .GetComponent<HERO>());
                if (IN_GAME_MAIN_CAMERA.singleCharacter == "SET 1" || IN_GAME_MAIN_CAMERA.singleCharacter == "SET 2" ||
                    IN_GAME_MAIN_CAMERA.singleCharacter == "SET 3")
                {
                    var heroCostume = CostumeConeveter.LocalDataToHeroCostume(IN_GAME_MAIN_CAMERA.singleCharacter);
                    heroCostume.Checkstat();
                    CostumeConeveter.HeroCostumeToLocalData(heroCostume, IN_GAME_MAIN_CAMERA.singleCharacter);
                    IN_GAME_MAIN_CAMERA.MainHERO.Setup.Init();

                    IN_GAME_MAIN_CAMERA.MainHERO.Setup.myCostume = heroCostume;
                    IN_GAME_MAIN_CAMERA.MainHERO.Setup.myCostume.stat = heroCostume.stat;

                    IN_GAME_MAIN_CAMERA.MainHERO.Setup.SetCharacterComponent();
                    IN_GAME_MAIN_CAMERA.MainHERO.SetStat();
                    IN_GAME_MAIN_CAMERA.MainHERO.SetSkillHudPosition();
                }
                else
                {
                    for (var i = 0; i < HeroCostume.costume.Length; i++)
                    {
                        if (HeroCostume.costume[i].name.ToUpper() == IN_GAME_MAIN_CAMERA.singleCharacter.ToUpper())
                        {
                            var num = HeroCostume.costume[i].id + CheckBoxCostume.costumeSet - 1;
                            if (HeroCostume.costume[num].name != HeroCostume.costume[i].name)
                            {
                                num = HeroCostume.costume[i].id + 1;
                            }

                            IN_GAME_MAIN_CAMERA.MainHERO.Setup.Init();
                            IN_GAME_MAIN_CAMERA.MainHERO.Setup.myCostume = HeroCostume.costume[num];
                            IN_GAME_MAIN_CAMERA.MainHERO.Setup.myCostume.stat =
                                HeroStat.getInfo(HeroCostume.costume[num].name.ToUpper());
                            IN_GAME_MAIN_CAMERA.MainHERO.Setup.SetCharacterComponent();
                            IN_GAME_MAIN_CAMERA.MainHERO.SetStat();
                            IN_GAME_MAIN_CAMERA.MainHERO.SetSkillHudPosition();
                            break;
                        }
                    }
                }
            }
        }
        else
        {
            var hero = Pool.NetworkEnable("AOTTG_HERO 1", pos, Quaternion.identity).GetComponent<HERO>();
            component.SetMainObject(hero);
            id = id.ToUpper();
            if (id == "SET 1" || id == "SET 2" || id == "SET 3")
            {
                var heroCostume2 = CostumeConeveter.LocalDataToHeroCostume(id);
                heroCostume2.Checkstat();
                CostumeConeveter.HeroCostumeToLocalData(heroCostume2, id);
                IN_GAME_MAIN_CAMERA.MainHERO.Setup.Init();

                IN_GAME_MAIN_CAMERA.MainHERO.Setup.myCostume = heroCostume2;
                IN_GAME_MAIN_CAMERA.MainHERO.Setup.myCostume.stat = heroCostume2.stat;

                IN_GAME_MAIN_CAMERA.MainHERO.Setup.SetCharacterComponent();
                IN_GAME_MAIN_CAMERA.MainHERO.SetStat();
                IN_GAME_MAIN_CAMERA.MainHERO.SetSkillHudPosition();
            }
            else
            {
                for (var j = 0; j < HeroCostume.costume.Length; j++)
                {
                    if (HeroCostume.costume[j].name.ToUpper() == id.ToUpper())
                    {
                        var num2 = HeroCostume.costume[j].id;
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
                        IN_GAME_MAIN_CAMERA.MainHERO.Setup.myCostume.stat =
                            HeroStat.getInfo(HeroCostume.costume[num2].name.ToUpper());
                        IN_GAME_MAIN_CAMERA.MainHERO.Setup.SetCharacterComponent();
                        IN_GAME_MAIN_CAMERA.MainHERO.SetStat();
                        IN_GAME_MAIN_CAMERA.MainHERO.SetSkillHudPosition();
                        break;
                    }
                }
            }

            CostumeConeveter.HeroCostumeToPhotonData(IN_GAME_MAIN_CAMERA.MainHERO.Setup.myCostume,
                PhotonNetwork.player);
            if (IN_GAME_MAIN_CAMERA.GameMode == GameMode.PVP_CAPTURE)
            {
                IN_GAME_MAIN_CAMERA.MainT.position += new Vector3(Random.Range(-20, 20), 2f, Random.Range(-20, 20));
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
        ShowHUDInfoCenter(string.Empty);
    }

    public TITAN SpawnTitan(int rate, Vector3 position, Quaternion rotation, bool punk = false)
    {
        var tit = SpawnTitanRaw(position, rotation);
        if (punk)
        {
            tit.SetAbnormalType(AbnormalType.Punk);
        }
        else if (Random.Range(0, 100) < rate)
        {
            if (IN_GAME_MAIN_CAMERA.Difficulty == 2)
            {
                if (Random.Range(0f, 1f) < 0.7f || Level.NoCrawler)
                {
                    tit.SetAbnormalType(AbnormalType.Jumper);
                }
                else
                {
                    tit.SetAbnormalType(AbnormalType.Crawler);
                }
            }
        }
        else if (IN_GAME_MAIN_CAMERA.Difficulty == 2)
        {
            if (Random.Range(0f, 1f) < 0.7f || Level.NoCrawler)
            {
                tit.SetAbnormalType(AbnormalType.Jumper);
            }
            else
            {
                tit.SetAbnormalType(AbnormalType.Crawler);
            }
        }
        else if (Random.Range(0, 100) < rate)
        {
            if (Random.Range(0f, 1f) < 0.8f || Level.NoCrawler)
            {
                tit.SetAbnormalType(AbnormalType.Aberrant);
            }
            else
            {
                tit.SetAbnormalType(AbnormalType.Crawler);
            }
        }
        else if (Random.Range(0f, 1f) < 0.8f || Level.NoCrawler)
        {
            tit.SetAbnormalType(AbnormalType.Jumper);
        }
        else
        {
            tit.SetAbnormalType(AbnormalType.Crawler);
        }

        GameObject gameObject2;
        if (IN_GAME_MAIN_CAMERA.GameType == GameType.Single)
        {
            gameObject2 =
                Pool.Enable("FX/FXtitanSpawn", tit.transform.position,
                    Quaternion.Euler(-90f, 0f,
                        0f)); //(GameObject)UnityEngine.Object.Instantiate(CacheResources.Load("FX/FXtitanSpawn"), tit.transform.position, Quaternion.Euler(-90f, 0f, 0f));
        }
        else
        {
            gameObject2 = Pool.NetworkEnable("FX/FXtitanSpawn", tit.transform.position, Quaternion.Euler(-90f, 0f, 0f));
        }

        gameObject2.transform.localScale = tit.transform.localScale;
        return tit;
    }

    public void SpawnTitanAction(int type, float size, int health, int number)
    {
        var position = new Vector3(Random.Range(-400f, 400f), 0f, Random.Range(-400f, 400f));
        var rotation = new Quaternion(0f, 0f, 0f, 1f);
        if (Level.Name.StartsWith("Custom") && CustomLevel.spawnPositions["Titan"].Count > 0)
        {
            position = CustomLevel.spawnPositions["Titan"][Random.Range(0, CustomLevel.spawnPositions["Titan"].Count)];
        }
        else
        {
            var objArray = GameObject.FindGameObjectsWithTag("titanRespawn");
            if (objArray.Length != 0)
            {
                var index = Random.Range(0, objArray.Length);
                var obj2 = objArray[index];
                while (objArray[index] == null)
                {
                    index = Random.Range(0, objArray.Length);
                    obj2 = objArray[index];
                }

                objArray[index] = null;
                position = obj2.transform.position;
                rotation = obj2.transform.rotation;
            }
        }

        for (var i = 0; i < number; i++)
        {
            var titan = SpawnTitanRaw(position, rotation);
            titan.hasSetLevel = true;
            titan.ResetLevel(size);
            if (health > 0f)
            {
                titan.armor = health;
            }

            titan.SetAbnormalType((AbnormalType)type);
        }
    }

    public void SpawnTitanAtAction(int type, float size, int health, int number, float posX, float posY, float posZ)
    {
        var position = new Vector3(posX, posY, posZ);
        var rotation = new Quaternion(0f, 0f, 0f, 1f);
        for (var i = 0; i < number; i++)
        {
            var obj2 = SpawnTitanRaw(position, rotation);
            obj2.ResetLevel(size);
            obj2.hasSetLevel = true;
            if (health > 0f)
            {
                obj2.armor = health;
            }

            obj2.SetAbnormalType((AbnormalType)type);
        }
    }

    public void SpawnTitansCustom(int rate, int count, bool punk = false)
    {
        var num = count;
        if (Level.Name.Contains("Custom"))
        {
            num = 5;
            if (RCManager.GameType.Value == 1)
            {
                num = 3;
            }
            else if (RCManager.GameType.Value == 2 || RCManager.GameType.Value == 3)
            {
                num = 0;
            }
        }
        if (IN_GAME_MAIN_CAMERA.GameMode == GameMode.SurviveMode)
        {
            if (count == 3 && !punk && GameModes.CustomAmount.Enabled)
            {
                num = GameModes.CustomAmount.GetInt(0);
            }
            else
            {
                num = GameModes.CustomAmount.Enabled ? GameModes.CustomAmount.GetInt(0) : count;
                if (punk)
                {
                    num = count;
                }
                else
                {
                    if (GameModes.TitansWaveAmount.Enabled)
                    {
                        num += GameModes.TitansWaveAmount.GetInt(0);
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
            var rates = new float[5];
            for (var i = 0; i < rates.Length; i++)
            {
                rates[i] = GameModes.SpawnRate.GetFloat(i);
            }

            if (punk && GameModes.PunkOverride.Enabled)
            {
                rates = new[] { 0f, 0f, 0f, 0f, 100f };
            }

            List<Vector3> positions;
            if (Level.Name.StartsWith("Custom") && CustomLevel.spawnPositions["Titan"].Count > 0)
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
                    positions = new List<Vector3>(new Vector3[num].Select(x => new Vector3(Random.Range(-100f, 100f), 0f, Random.Range(-100f, 100f))).ToArray());
                }
            }

            var summ = rates[0] + rates[1] + rates[2] + rates[3] + rates[4];
            for (var i = 0; i < num; i++)
            {
                Vector3 position;
                if (positions.Count == 0)
                {
                    position = RespawnPositions.RandomTitanPos;
                }
                else
                {
                    var index = Random.Range(0, positions.Count);
                    position = positions[index];
                    positions.RemoveAt(index);
                }

                var cRate = Random.Range(0, 100f);
                if (cRate <= summ)
                {
                    var startRate = 0f;
                    var endRate = 0f;
                    var type = 0;
                    for (var j = 0; j < 5; j++)
                    {
                        endRate += rates[j];
                        if (cRate >= startRate && cRate < endRate)
                        {
                            type = j;
                            break;
                        }

                        startRate += rates[j];
                    }

                    SpawnTitanRaw(position, Quaternion.identity).SetAbnormalType((AbnormalType)type);
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
                        positions = new List<Vector3>(new Vector3[num].Select(x =>
                            new Vector3(Random.Range(-400f, 400f), 0f, Random.Range(-400f, 400f))).ToArray());
                    }
                }

                for (var i = 0; i < num; i++)
                {
                    Vector3 position;
                    if (positions.Count == 0)
                    {
                        position = new Vector3(Random.Range(-400f, 400f), 0f, Random.Range(-400f, 400f));
                    }
                    else
                    {
                        var index = Random.Range(0, positions.Count);
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

    public void TitanGetKill(PhotonPlayer player, int damage, string titanName)
    {
        damage = Mathf.Max(10, damage);
        BasePV.RPC("netShowDamage", player, damage);
        BasePV.RPC("oneTitanDown", PhotonTargets.MasterClient, titanName, false);
        SendKillInfo(false, player.UIName, true, titanName, damage);
        PlayerKillInfoUpdate(player, damage);
        AnarchyManager.Feed.Kill(player.UIName.ToHTMLFormat(), titanName, damage);
    }

    public void TitanGetKillbyServer(int Damage, string titanName)
    {
        Damage = Mathf.Max(10, Damage);
        SendKillInfo(false, LoginFengKAI.player.name, true, titanName, Damage);
        netShowDamage(Damage);
        oneTitanDown(titanName);
        PlayerKillInfoUpdate(PhotonNetwork.player, Damage);
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
        Labels.TopLeft = content.ToHTMLFormat();
    }

    internal void ShowHUDInfoTopRight(string content)
    {
        Labels.TopRight = content.ToHTMLFormat();
    }

    internal void ShowHUDInfoTopRightMAPNAME(string content)
    {
        Labels.TopRight += content.ToHTMLFormat();
    }

#endregion Show UILabels
}
