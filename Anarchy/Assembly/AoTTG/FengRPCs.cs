using Anarchy;
using Anarchy.Configuration;
using Anarchy.Localization;
using Anarchy.UI;
using Antis;
using ExitGames.Client.Photon;
using GameLogic;
using Optimization.Caching;
using RC;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using UnityEngine;

// ReSharper disable once CheckNamespace
[SuppressMessage("ReSharper", "UnusedMember.Local")]
[SuppressMessage("ReSharper", "InconsistentNaming")]
internal partial class FengGameManagerMKII
{
    [RPC]
    private void Chat(string content, string sender, PhotonMessageInfo info)
    {
        if (info != null && !info.Sender.IsLocal && info.Sender.Muted)
        {
            return;
        }

        string message;
        if (sender != string.Empty)
        {
            if (Settings.RemoveColors)
            {
                sender = sender.RemoveAll();
            }
            message = sender + ": " + content;
        }
        else
        {
            message = content;
        }

        message = AnarchyExtensions.ValidateUnityTags(message).RemoveSize();
        Anarchy.UI.Chat.Add(User.Chat(info.Sender.ID, message));
    }

    [RPC]
    private void ChatLocalized(string file, string key, string[] args, PhotonMessageInfo info)
    {
        if (!info.Sender.AnarchySync)
        {
            Log.AddLine("notAnarchyUser", MsgType.Error, "RPC", nameof(ChatLocalized), info.Sender.ID.ToString());
            return;
        }

        var locale = Anarchy.Localization.Language.Find(file);
        if (args == null)
        {
            Log.AddLine("RPCerror", MsgType.Error, info.Sender.ID.ToString(), nameof(ChatLocalized));
            info.Sender.AnarchySync = false;
            return;
        }

        if (locale == null)
        {
            locale = new Locale(file, true, ',');
            if (!File.Exists(locale.Path))
            {
                Log.AddLine("RPCerror", MsgType.Error, info.Sender.ID.ToString(), nameof(ChatLocalized));
                info.Sender.AnarchySync = false;
                return;
            }
        }

        if (!locale.IsOpen)
        {
            locale.KeepOpen(60);
        }

        if (args.Length > 0)
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
        if (info != null && !info.Sender.IsLocal && info.Sender.Muted)
        {
            return;
        }

        content = sender + ": " + content;
        content = AnarchyExtensions.ValidateUnityTags(content).RemoveSize();
        Anarchy.UI.Chat.Add(User.ChatPm(info.Sender.ID, content));
    }

    [RPC]
    private void clearlevel(string[] link, int gametype, PhotonMessageInfo info)
    {
        if (info != null && !info.Sender.IsMasterClient)
        {
            Log.AddLine("RPCerror", MsgType.Error, info.Sender.ID.ToString(), nameof(clearlevel));
            AntisManager.Response(info.Sender.ID, true, string.Empty);
            return;
        }

        switch (gametype)
        {
            case 0:
                IN_GAME_MAIN_CAMERA.GameMode = GameMode.KillTitan;
                if (!(logic is KillTitanLogic))
                {
                    logic = new KillTitanLogic(logic);
                }

                break;

            case 1:
                IN_GAME_MAIN_CAMERA.GameMode = GameMode.SurviveMode;
                if (!(logic is SurviveLogic))
                {
                    logic = new SurviveLogic(logic);
                }

                break;

            case 2:
                IN_GAME_MAIN_CAMERA.GameMode = GameMode.PvpAhss;
                if (!(logic is PVPLogic))
                {
                    logic = new PVPLogic(logic);
                }

                break;

            case 3:
                IN_GAME_MAIN_CAMERA.GameMode = GameMode.Racing;
                if (!(logic is RacingLogic))
                {
                    logic = new RacingLogic(logic);
                }

                break;

            default:
            case 4:
                IN_GAME_MAIN_CAMERA.GameMode = GameMode.None;
                if (logic == null || logic.GetType() != typeof(GameLogic.GameLogic))
                {
                    logic = new GameLogic.GameLogic(logic);
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
            AntisManager.Response(info.Sender.ID, true, string.Empty);
            return;
        }

        CustomLevel.RPC(content);
    }

    [RPC]
    private void getRacingResult(string player, float time, PhotonMessageInfo info = null)
    {
        if (info != null && (IN_GAME_MAIN_CAMERA.GameMode != GameMode.Racing || !PhotonNetwork.IsMasterClient))
        {
            Log.AddLine("RPCerror", MsgType.Error, info.Sender.ID.ToString(), nameof(getRacingResult));
            AntisManager.Response(info.Sender.ID, true, string.Empty);
            return;
        }

        (logic as RacingLogic)?.OnPlayerFinished(time, player);
        var result = new RacingResult(player, time);
        racingResult.Add(result);
        RefreshRacingResult();
    }

    [RPC]
    private void ignorePlayer(int ID, PhotonMessageInfo info)
    {
        if (!info.Sender.IsMasterClient)
        {
            Log.AddLine("RPCerror", MsgType.Error, info.Sender.ID.ToString(), nameof(ignorePlayer));
            AntisManager.Response(info.Sender.ID, true, string.Empty);
            return;
        }

        var photonPlayer = PhotonPlayer.Find(ID);
        if (photonPlayer != null && !photonPlayer.RCIgnored)
        {
            photonPlayer.RCIgnored = true;
        }

        PlayerList.Update();
    }

    [RPC]
    private void ignorePlayerArray(int[] IDS, PhotonMessageInfo info)
    {
        if (!info.Sender.IsMasterClient)
        {
            Log.AddLine("RPCerror", MsgType.Error, info.Sender.ID.ToString(), nameof(ignorePlayerArray));
            AntisManager.Response(info.Sender.ID, true, string.Empty);
            return;
        }

        foreach (var ID in IDS)
        {
            var photonPlayer = PhotonPlayer.Find(ID);
            if (photonPlayer != null && !photonPlayer.RCIgnored)
            {
                photonPlayer.RCIgnored = true;
            }
        }

        PlayerList.Update();
    }

    [RPC]
    private void labelRPC(int setting, PhotonMessageInfo info)
    {
        if (info != null)
        {
            var checkID = info.TimeInt - 1000000;
            checkID *= -1;
            if (checkID == info.Sender.ID)
            {
                if (!info.Sender.AnarchySync)
                {
                    info.Sender.AnarchySync = true;
                    PlayerList.Update();
                    PhotonNetwork.SendChekInfo(info.Sender);
                }
            }
        }
    }

    [RPC]
    public void loadskinRPC(string n, string urls, string str, string[] skybox, PhotonMessageInfo info = null)
    {
        if (info != null && !info.Sender.IsMasterClient)
        {
            Log.AddLine("RPCerror", MsgType.Error, info.Sender.ID.ToString(),
                nameof(FengGameManagerMKII) + "." + nameof(loadskinRPC));
            AntisManager.Response(info.Sender.ID, true, string.Empty);
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

        if (Level.MapName.Contains("Forest") && SkinSettings.ForestSkins.Value != 1)
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
            AntisManager.Response(info.Sender.ID, true, string.Empty);
            return;
        }

        logic.NetGameLose(score);
    }

    [RPC]
    private void netGameWin(int score, PhotonMessageInfo info)
    {
        if (info != null && !info.Sender.IsMasterClient && IN_GAME_MAIN_CAMERA.GameMode != GameMode.Racing)
        {
            Log.AddLine("RPCerror", MsgType.Error, info.Sender.ID.ToString(), nameof(netGameWin));
            AntisManager.Response(info.Sender.ID, true, string.Empty);
            return;
        }

        logic.NetGameWin(score);
    }

    [RPC]
    private void netRefreshRacingResult(string tmp, PhotonMessageInfo info)
    {
        if (info != null && (IN_GAME_MAIN_CAMERA.GameMode != GameMode.Racing || !info.Sender.IsMasterClient))
        {
            Log.AddLine("RPCerror", MsgType.Error, info.Sender.ID.ToString(), nameof(netRefreshRacingResult));
            AntisManager.Response(info.Sender.ID, true, string.Empty);
            return;
        }

        localRacingResult = tmp;
        if (logic is RacingLogic rac)
        {
            rac.OnUpdateRacingResult();
        }
    }

    [RPC]
    private void pauseRPC(bool pause, PhotonMessageInfo info = null)
    {
        if (info != null && !info.Sender.IsMasterClient)
        {
            Log.AddLine("RPCerror", MsgType.Error, info.Sender.ID.ToString(), nameof(pauseRPC));
            AntisManager.Response(info.Sender.ID, true, string.Empty);
            return;
        }

        if (pause)
        {
            AnarchyManager.PauseWindow.PauseWaitTime = 100000f;
            UnityEngine.Time.timeScale = 0.00001f;
            if (!AnarchyManager.PauseWindow.IsActive)
            {
                AnarchyManager.PauseWindow.EnableImmediate();
            }
        }
        else
        {
            AnarchyManager.PauseWindow.PauseWaitTime = 3f;
        }
    }

    [RPC]
    private void refreshPVPStatus(int score1, int score2, PhotonMessageInfo info)
    {
        if (info != null && !info.Sender.IsMasterClient && logic.Mode != GameMode.PVP_CAPTURE)
        {
            Log.AddLine("RPCerror", MsgType.Error, info.Sender.ID.ToString(), nameof(refreshPVPStatus));
            AntisManager.Response(info.Sender.ID, true, string.Empty);
            return;
        }

        if (logic is PVPCaptureLogic log)
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
            AntisManager.Response(info.Sender.ID, true, string.Empty);
            return;
        }

        if (logic is PVPLogic log)
        {
            log.Scores = score1;
        }
    }

    [RPC]
    private void refreshStatus(int score1, int score2, int wav, int highestWav, float time1, float time2,
        bool startRacin, bool endRacin, PhotonMessageInfo info)
    {
        if (info != null && !info.Sender.IsMasterClient)
        {
            Log.AddLine("RPCerror", MsgType.Error, info.Sender.ID.ToString(), nameof(refreshStatus));
            AntisManager.Response(info.Sender.ID, true, string.Empty);
            return;
        }

        logic.OnRefreshStatus(score1, score2, wav, highestWav, time1, time2, startRacin, endRacin);
    }

    [RPC]
    private void RequireStatus(PhotonMessageInfo info)
    {
        if (info != null && !PhotonNetwork.IsMasterClient)
        {
            Log.AddLine("RPCerror", MsgType.Error, info.Sender.ID.ToString(), nameof(RequireStatus));
            AntisManager.Response(info.Sender.ID, true, string.Empty);
            return;
        }

        logic.OnRequireStatus();
    }

    [RPC]
    private void respawnHeroInNewRound()
    {
        if (needChooseSide)
        {
            return;
        }

        if (IN_GAME_MAIN_CAMERA.MainCamera.gameOver)
        {
            SpawnPlayer(myLastHero);
            IN_GAME_MAIN_CAMERA.MainCamera.gameOver = false;
            ShowHUDInfoCenter(string.Empty);
        }
    }

    [RPC]
    private void restartGameByClient(PhotonMessageInfo info = null)
    {
    }

    [RPC]
    private void RPCLoadLevel(PhotonMessageInfo info = null)
    {
        if (info != null && !info.Sender.IsMasterClient)
        {
            Log.AddLine("RPCerror", MsgType.Error, info.Sender.ID.ToString(), nameof(RPCLoadLevel));
            AntisManager.Response(info.Sender.ID, true, string.Empty);
            return;
        }

        AnarchyManager.Log.Disable();
        AnarchyManager.Chat.Disable();
        DestroyAllExistingCloths();
        PhotonNetwork.LoadLevel(Level.MapName);
    }

    //En: Do not touch this. NEVER. To prevent erros from both sides, to you and from you.
    //Ru: Не трогать это. НИКОГДА. Во избежание ошибок с вашей стороны и стороны других.
    [RPC]
    private void SetAnarchyMod(bool isCustom, bool useSync, string customName, string version, PhotonMessageInfo info)
    {
        if (info.Sender.AnarchySync)
        {
            if (isCustom)
            {
                var customNameShow = customName == string.Empty ? "Custom" : customName;
                info.Sender.AnarchySync =
 version == AnarchyManager.AnarchyVersion.ToString() && (customName != string.Empty && customName == AnarchyManager.CustomName);

                info.Sender.ModName = string.Format(ModNames.AnarchyCustom, customNameShow);
                info.Sender.ModLocked = true;
                return;
            }

            if (version == AnarchyManager.AnarchyVersion.ToString())
            {
                return;
            }

            info.Sender.AnarchySync = false;
            info.Sender.ModName = string.Format(ModNames.AnarchyCustom, version);
            info.Sender.ModLocked = true;
        }
    }

    [RPC]
    private void setMasterRC(PhotonMessageInfo info)
    {
        if (!info.Sender.IsMasterClient)
        {
            Log.AddLine("RPCerror", MsgType.Error, info.Sender.ID.ToString(), nameof(setMasterRC));
            AntisManager.Response(info.Sender.ID, true, string.Empty);
        }
    }

    [RPC]
    private void setTeamRPC(int team, PhotonMessageInfo info)
    {
        if (info != null && !info.Sender.IsMasterClient && !info.Sender.IsLocal)
        {
            Log.AddLine("RPCerror", MsgType.Error, info.Sender.ID.ToString(), nameof(setTeamRPC));
            AntisManager.Response(info.Sender.ID, true, string.Empty);
            return;
        }

        SetTeam(team);
    }

    [RPC]
    private void settingRPC(Hashtable hash, PhotonMessageInfo info)
    {
        if (info != null && !info.Sender.IsMasterClient)
        {
            Log.AddLine("RPCerror", MsgType.Error, info.Sender.ID.ToString(), nameof(settingRPC));
            AntisManager.Response(info.Sender.ID, true, string.Empty);
            return;
        }

        GameModes.HandleRpc(hash);
    }

    [RPC]
    private void showResult(string text0, string text1, string text2, string text3, string text4, string text6,
        PhotonMessageInfo info)
    {
        if (info != null && !info.Sender.IsMasterClient)
        {
            Log.AddLine("RPCerror", MsgType.Error, info.Sender.ID.ToString(), nameof(showResult));
            AntisManager.Response(info.Sender.ID, true, string.Empty);
            return;
        }

        if (gameTimesUp)
        {
            return;
        }

        gameTimesUp = true;
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
        gameStart = false;
    }

    [RPC]
    private void spawnPlayerAtRPC(float posX, float posY, float posZ, PhotonMessageInfo info)
    {
        if (info.Sender.IsMasterClient && CustomLevel.logicLoaded && CustomLevel.customLevelLoaded && !needChooseSide &&
            IN_GAME_MAIN_CAMERA.MainCamera.gameOver)
        {
            var pos = new Vector3(posX, posY, posZ);
            var component = IN_GAME_MAIN_CAMERA.MainCamera;
            var id = myLastHero.ToUpper();
            myLastHero = id;
            component.SetMainObject(
                Pool.NetworkEnable("AOTTG_HERO 1", pos, Quaternion.identity).GetComponent<HERO>());
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
                IN_GAME_MAIN_CAMERA.MainT.position += new Vector3(Random.Range(-20, 20), 2f,
                    Random.Range(-20, 20));
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
            ShowHUDInfoCenter(string.Empty);
        }
    }

    [RPC]
    private void spawnTitanRPC(PhotonMessageInfo info)
    {
        if (info.Sender.IsMasterClient)
        {
            foreach (var obj in titans)
            {
                if (obj.BasePV.IsMine && (!PhotonNetwork.IsMasterClient || obj.nonAI))
                {
                    PhotonNetwork.Destroy(obj.BasePV);
                }
            }

            SpawnNonAiTitan(myLastHero);
        }
        else
        {
            Log.AddLine("RPCerror", MsgType.Error, info.Sender.ID.ToString(), nameof(spawnTitanRPC));
            AntisManager.Response(info.Sender.ID, true, string.Empty);
        }
    }

    [RPC]
    private void updateKillInfo(bool t1, string killer, bool t2, string victim, int dmg, PhotonMessageInfo info)
    {
        if (info != null && !info.Sender.IsMasterClient && dmg != 0 && !t1 && !info.Sender.IsTitan)
        {
            Log.AddLineRaw(
                $"t1:{t1},killer:{killer},t2:{t2},victim:{victim},dmg:{dmg},sender:{info.Sender.ID},isTitan:{info.Sender.IsTitan}",
                MsgType.Warning);
            Log.AddLine("RPCerror", MsgType.Error, info.Sender.ID.ToString(), nameof(updateKillInfo));
            return;
        }

        killer = killer.LimitToLengthStripped(50);
        victim = victim.LimitToLengthStripped(50);
        var killInfo = ((GameObject)Instantiate(CacheResources.Load("UI/KillInfo")))
            .GetComponent<KillInfoComponent>();
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
        if (!info.Sender.IsLocal)
        {
            AnarchyManager.Feed.Kill(killer.ToHTMLFormat(), victim.ToHTMLFormat(), dmg);
        }
    }

    [RPC]
    private void verifyPlayerHasLeft(int ID, PhotonMessageInfo info)
    {
        if (ID < 0)
        {
            var ver = ID.ToString().Replace("-", "");
            info.Sender.ModName = string.Format(ModNames.Cyan, $"{ver[0]}.{ver[1]}.{ver[2]}");
        }
    }

    [RPC]
    public void netShowDamage(int speed, PhotonMessageInfo info = null)
    {
        if (info != null && !info.Sender.IsMasterClient && !info.Sender.IsTitan)
        {
            Log.AddLine("RPCerror", MsgType.Error, info.Sender.ID.ToString(), nameof(netShowDamage));
            AntisManager.Response(info.Sender.ID, true, string.Empty);
            return;
        }

        if (Stylish != null)
        {
            Stylish.Style(speed);
        }
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
            iTween.ScaleTo(target.cachedGameObject,
                new System.Collections.Hashtable
                {
                    {"x", speed}, {"y", speed}, {"z", speed}, {"easetype", iTween.EaseType.easeOutElastic}, {"time", 1f}
                });
            iTween.ScaleTo(target.cachedGameObject, itweenHash);
        }
    }

    [RPC]
    public void oneTitanDown(string name1 = "", bool onPlayerLeave = false, PhotonMessageInfo info = null)
    {
        if (info != null && !PhotonNetwork.IsMasterClient)
        {
            Log.AddLine("RPCerror", MsgType.Error, info.Sender.ID.ToString(), nameof(oneTitanDown));
            AntisManager.Response(info.Sender.ID, true, string.Empty);
            return;
        }

        logic.OnTitanDown(name1, onPlayerLeave);
    }

    [RPC]
    public void someOneIsDead(int id, PhotonMessageInfo info = null)
    {
        if (info != null && !PhotonNetwork.IsMasterClient)
        {
            Log.AddLine("RPCerror", MsgType.Error, info.Sender.ID.ToString(), nameof(someOneIsDead));
            AntisManager.Response(info.Sender.ID, true, string.Empty);
            return;
        }

        logic.OnSomeOneIsDead(id);
    }
}
