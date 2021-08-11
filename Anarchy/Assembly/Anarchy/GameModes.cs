using Anarchy.Configuration;
using Anarchy.UI;
using GameLogic;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace Anarchy
{
    public static class GameModes
    {
        public static StringSetting DisabledColor = new StringSetting(nameof(DisabledColor), "FFAACC");
        public static StringSetting EnabledColor = new StringSetting(nameof(EnabledColor), "CCFFCC");

        private static List<GameModeSetting> allGameSettings;
        private static readonly List<int> antiReviveList = new List<int>();

        public static Hashtable oldHash = new Hashtable();
        private static Hashtable infection = new Hashtable();

        public static readonly GameModeSetting CustomAmount = new GameModeSetting("titanc", new[] { 5 });

        public static readonly GameModeSetting SpawnRate =
            new GameModeSetting("spawnMode,nRate,aRate,jRate,cRate,pRate",
                new float[] { 20f, 20f, 20f, 20f, 20f }).AddApplyCallback(CheckCustomSpawn);

        public static readonly GameModeSetting SizeMode =
            new GameModeSetting("sizeMode,sizeLower,sizeUpper", new[] { 0.7f, 3f });

        public static readonly GameModeSetting HealthMode =
            new GameModeSetting("healthMode,healthLower,healthUpper", 0, new[] { 200, 500 });

        public static readonly GameModeSetting DamageMode = new GameModeSetting("damage", new[] { 500 });
        public static readonly GameModeSetting ExplodeMode = new GameModeSetting("explode", new[] { 30 });
        public static readonly GameModeSetting NoRocks = new GameModeSetting("rock");

        public static readonly GameModeSetting TitansWaveAmount =
            new GameModeSetting("waveModeOn,waveModeNum", new[] { 5 });

        public static readonly GameModeSetting PointMode = new GameModeSetting("point", new[] { 50 });
        public static readonly GameModeSetting BombMode = new GameModeSetting("bomb");

        public static readonly GameModeSetting TeamMode = new GameModeSetting("team", 0).AddApplyCallback(
            (a, b, c, d, e) =>
            {
                if (!b)
                {
                    if (PhotonNetwork.inRoom)
                    {
                        FengGameManagerMKII.FGM.BasePV.RPC("setTeamRPC",
                            PhotonTargets.All, 0);
                    }
                    return;
                }

                SendTeamInfo();
            });

        public static readonly GameModeSetting InfectionMode = new GameModeSetting("infection", new[] { 1 });
        public static readonly GameModeSetting FriendlyMode = new GameModeSetting("friendly");
        public static readonly GameModeSetting BladePvp = new GameModeSetting("pvp", 0);
        public static readonly GameModeSetting NoAhssReload = new GameModeSetting("ahssReload");
        public static readonly GameModeSetting CannonsKillHumans = new GameModeSetting("deadlycannons");
        public static readonly GameModeSetting MaxWave = new GameModeSetting("maxwave", new[] { 20 });
        public static readonly GameModeSetting PunkOverride = new GameModeSetting("punkWaves");
        public static readonly GameModeSetting MinimapDisable = new GameModeSetting("globalDisableMinimap");
        public static readonly GameModeSetting EndlessRespawn = new GameModeSetting("endless", new[] { 5 });
        public static readonly GameModeSetting KickEren = new GameModeSetting("eren");
        public static readonly GameModeSetting AllowHorses = new GameModeSetting("horse");
        public static readonly StringSetting Motd = new StringSetting("motd", string.Empty);

        public static readonly AnarchyGameModeSetting RacingStartTime =
            (AnarchyGameModeSetting)new AnarchyGameModeSetting("startTime,startTimeValue", new int[] { 20 })
                .AddChangedCallback(RacingLogic.StartTimeCheck);

        public static readonly AnarchyGameModeSetting RacingFinishersRestart =
            new AnarchyGameModeSetting("restartOnFinishers,finishersCount", new int[] { 5 });

        public static readonly AnarchyGameModeSetting RacingTimeLimit =
            new AnarchyGameModeSetting("racingTimeLimit,racingTimeLimitValue", new int[] { 500 });

        public static readonly AnarchyGameModeSetting RacingRestartTime =
            (AnarchyGameModeSetting)new AnarchyGameModeSetting("racingRestartTime,restartTimeValue", new int[] { 999 })
                .RemoveChangedCallback(AnarchyGameModeSetting.AnarchySettingCallback)
                .AddChangedCallback(RacingLogic.RestartTimeCheck);

        public static readonly AnarchyGameModeSetting NoGuest = new AnarchyGameModeSetting("noGuest");
        public static readonly AnarchyGameModeSetting AntiRevive = new AnarchyGameModeSetting("antiRevive");

        public static readonly AnarchyGameModeSetting AfkKill =
            new AnarchyGameModeSetting("afkKill,afkKillTime", new[] { 20 });

        public static readonly GameModeSetting AsoRacing =
            new GameModeSetting("asoracing").AddChangedCallback(RacingLogic.ASORacingCheck);

        public static readonly AnarchyGameModeSetting AutoPickNextMap = new AnarchyGameModeSetting("autoPickMap,autoPickMapRounds", new int[] { 5 });
        public static readonly StringSetting AutoPickNextMapFilter = new StringSetting("autoPickMapFilter", string.Empty);
        public static readonly AnarchyGameModeSetting InfiniteGasPvp = new AnarchyGameModeSetting("infiniteGasPvp");
        public static readonly BoolSetting AnnounceMapSwitch = new BoolSetting(nameof(AnnounceMapSwitch), false);
        public static readonly AnarchyGameModeSetting MaximumSpeedLimit = new AnarchyGameModeSetting("maximumSpeedLimit,maximumAllowedSpeed", new int[] { 250 });
        public static readonly AnarchyGameModeSetting NonStopRacing = new AnarchyGameModeSetting("nonStopRacing,nonStopMinimumSpeed,nonStopGainTimer", new int[] { 100, 10 });
        public static readonly AnarchyGameModeSetting OneLifeMode = new AnarchyGameModeSetting("oneLifeRacing,olrAmount,olrTimer", new int[] { 1, 5 });

        public static bool AntiReviveAdd(int id)
        {
            if (AntiReviveEnabled() && IN_GAME_MAIN_CAMERA.GameMode != GameMode.Racing &&
                FengGameManagerMKII.Level.RespawnMode != RespawnMode.DEATHMATCH &&
                FengGameManagerMKII.FGM.logic.RoundTime > 10f)
            {
                if (antiReviveList.Contains(id))
                {
                    return false;
                }

                antiReviveList.Add(id);
                return true;
            }

            return false;
        }

        public static bool AntiReviveCheck(int id, HERO hero)
        {
            if (AntiReviveEnabled())
            {
                if (antiReviveList.Contains(id))
                {
                    hero.BasePV.RPC("netDie2", PhotonTargets.All, -1, "Anti-Revive ");
                    return true;
                }

                return false;
            }

            return false;
        }

        public static void AntiReviveClear()
        {
            if (AntiReviveEnabled())
            {
                antiReviveList.Clear();
            }
        }

        private static bool AntiReviveEnabled() => IN_GAME_MAIN_CAMERA.GameType == GameType.MultiPlayer &&
                                                   PhotonNetwork.IsMasterClient &&
                                                   AntiRevive.Enabled &&
                                                   !EndlessRespawn.Enabled;

        public static bool AntiReviveRemove(int id) => AntiReviveEnabled() && antiReviveList.Remove(id);

        public static void AddSetting(GameModeSetting set)
        {
            if (allGameSettings == null)
            {
                allGameSettings = new List<GameModeSetting>();
            }

            if (!allGameSettings.Contains(set))
            {
                allGameSettings.Add(set);
            }
        }

        private static void CheckCustomSpawn(GameModeSetting set, bool state, int selection, float[] floats,
            int[] integers)
        {
            if (state)
            {
                float summ = 0f;
                for (int i = 0; i < 5; i++)
                {
                    summ += floats[i];
                }

                if (summ > 100f)
                {
                    for (int i = 0; i < 5; i++)
                    {
                        floats[i] = 20f;
                    }
                }
            }
        }

        private static IEnumerator CheckEndless(int id, float time)
        {
            yield return new WaitForSeconds(time);
            PhotonPlayer player = PhotonPlayer.Find(id);
            if (player != null && player.Dead && !player.IsTitan)
            {
                FengGameManagerMKII.FGM.BasePV.RPC("respawnHeroInNewRound", player);
            }
        }

        public static void CheckGameEnd()
        {
            FengGameManagerMKII.FGM.StartCoroutine(CheckGameEndI());
        }

        private static IEnumerator CheckGameEndI()
        {
            yield return new WaitForSeconds(0.05f);
            if (PhotonNetwork.IsMasterClient && CheckPvpWinner())
            {
                yield break;
            }

            if (PhotonNetwork.IsMasterClient && FengGameManagerMKII.IsPlayerAllDead())
            {
                FengGameManagerMKII.FGM.GameLose();
            }
        }

        private static bool CheckPvpWinner()
        {
            if ((!BombMode.Enabled && !BladePvp.Enabled && !TeamMode.Enabled && !PointMode.Enabled) || FengGameManagerMKII.FGM.IsWinning)
            {
                return false;
            }
            if (!TeamMode.Enabled)
            {
                if (!PointMode.Enabled)
                {
                    List<PhotonPlayer> alives = PhotonNetwork.playerList.Where(player => !player.Dead).ToList();
                    if (alives.Count > 1)
                    {
                        return false;
                    }
                    FengGameManagerMKII.FGM.GameWin();
                    if (alives.Count > 0)
                    {
                        Chat.SendLocalizedTextAll("GameModes", "playerWin", new[] { alives[0].UIName.ToHTMLFormat() });
                    }
                    else
                    {
                        Chat.SendLocalizedTextAll("GameModes", "nobodyWin", new string[0]);
                    }
                    if (alives.Count > 0)
                    {
                        alives[0].Kills += 5;
                    }
                }
                else
                {
                    if (FengGameManagerMKII.IsPlayerAllDead() && !EndlessRespawn.Enabled)
                    {
                        FengGameManagerMKII.FGM.GameLose();
                        return true;
                    }
                    foreach (PhotonPlayer player in PhotonNetwork.playerList)
                    {
                        if (player.Kills >= PointMode.GetInt(0))
                        {
                            FengGameManagerMKII.FGM.GameWin();
                            Chat.SendLocalizedTextAll("GameModes", "playerWin", new[] { player.UIName.ToHTMLFormat() });
                            break;
                        }
                    }
                }
                return true;
            }
            bool teamCyanWin = false;
            bool teamMagentaWin = false;
            if (PointMode.Enabled)
            {
                int cyanKills = 0;
                int magentaKills = 0;
                foreach (PhotonPlayer player in PhotonNetwork.playerList)
                {
                    switch (player.RCteam)
                    {
                        case 1:
                            cyanKills += player.Kills;
                            break;

                        case 2:
                            magentaKills += player.Kills;
                            break;
                    }
                }
                if (cyanKills >= PointMode.GetInt(0))
                {
                    teamCyanWin = true;
                }
                if (magentaKills >= PointMode.GetInt(0))
                {
                    teamMagentaWin = true;
                }
                if (FengGameManagerMKII.IsPlayerAllDead() && !EndlessRespawn.Enabled)
                {
                    FengGameManagerMKII.FGM.GameLose();
                    return true;
                }
            }
            else
            {
                List<PhotonPlayer> cyan = new List<PhotonPlayer>();
                List<PhotonPlayer> magenta = new List<PhotonPlayer>();
                foreach (PhotonPlayer player in PhotonNetwork.playerList)
                {
                    if (player.Dead)
                    {
                        continue;
                    }
                    int rcteam = player.RCteam;
                    switch (rcteam)
                    {
                        case 1:
                            cyan.Add(player);
                            break;

                        case 2:
                            magenta.Add(player);
                            break;
                    }
                }
                if (BladePvp.Enabled || BombMode.Enabled)
                {
                    if (magenta.Count < 1)
                    {
                        teamCyanWin = true;
                    }
                    else if (cyan.Count < 1)
                    {
                        teamMagentaWin = true;
                    }
                }
            }
            if (teamMagentaWin ^ teamCyanWin)
            {
                FengGameManagerMKII.FGM.GameWin();
                Chat.SendLocalizedTextAll("GameModes", (teamCyanWin ? "cyan" : "magenta") + "Win", new string[0]);
                return true;
            }

            if (teamMagentaWin && teamCyanWin)
            {
                FengGameManagerMKII.FGM.GameWin();
                Chat.SendLocalizedTextAll("GameModes", "nobodyWin", new string[0]);
                return true;
            }
            return false;
        }

        public static void DisableAll()
        {
            foreach (GameModeSetting set in allGameSettings)
            {
                set.State = false;
            }
        }

        public static void EndlessMode(int id)
        {
            if (!EndlessRespawn.Enabled)
            {
                return;
            }

            FengGameManagerMKII.FGM.StartCoroutine(CheckEndless(id, EndlessRespawn.GetInt(0)));
        }

        public static void ForceChange()
        {
            foreach (GameModeSetting set in allGameSettings)
            {
                set.ForceChange();
            }
        }

        public static string GetGameModesInfo()
        {
            StringBuilder bld = new StringBuilder();
            int count = 0;
            foreach (var set in allGameSettings.Where(set => set.Enabled))
            {
                bld.Append((count > 0 ? "\n" : string.Empty) + set.ToStringLocal());
                count++;
            }

            return count == 0 ? string.Empty : bld.ToString();
        }

        public static void HandleRpc(Hashtable hash)
        {
            if (oldHash.Equals(hash))
            {
                return;
            }

            int count = 0;
            StringBuilder bld = new StringBuilder();
            foreach (var set in allGameSettings)
            {
                set.ReadFromHashtable(hash);
                if (set.HasChangedReceived)
                {
                    set.ApplyReceived();
                    if (count > 0)
                    {
                        bld.Append("\n");
                    }

                    bld.Append(set.ToStringLocal());
                    count++;
                }
            }

            if (hash.ContainsKey("motd") && oldHash.ContainsKey("motd") &&
                oldHash["motd"] as string != hash["motd"] as string)
            {
                if (count > 0)
                {
                    bld.Append("\n");
                }

                bld.Append("MOTD: " + hash["motd"]);
                oldHash["motd"] = hash["motd"];
            }
            else if(hash.ContainsKey("motd") && oldHash.ContainsKey("motd") == false)
            {
                bld.AppendLine("MOTD: " + hash["motd"]);
                oldHash.Add("motd", hash["motd"]);
            }

            oldHash = new Hashtable();
            Dictionary<object, object> clone = (Dictionary<object, object>)hash.Clone();
            foreach (KeyValuePair<object, object> pair in clone)
            {
                oldHash.Add(pair.Key, pair.Value);
            }

            Chat.Add(bld.ToString());
            PhotonNetwork.SetModProperties();
        }

        public static void InfectionOnDeath(PhotonPlayer owner)
        {
            if (PhotonNetwork.IsMasterClient && InfectionMode.Enabled && !FengGameManagerMKII.FGM.IsLosing &&
                !FengGameManagerMKII.FGM.IsWinning && FengGameManagerMKII.FGM.logic.RoundTime > 5f)
            {
                if (!infection.ContainsKey(owner.ID))
                {
                    infection.Add(owner, owner.ID);
                    owner.IsTitan = true;
                    FengGameManagerMKII.FGM.BasePV.RPC("spawnTitanRPC", owner);
                }
            }
        }

        public static void InfectionOnSpawn(HERO hero)
        {
            if (InfectionMode.Enabled && PhotonNetwork.IsMasterClient && !FengGameManagerMKII.FGM.IsLosing &&
                !FengGameManagerMKII.FGM.IsWinning && FengGameManagerMKII.FGM.logic.RoundTime > 5f)
            {
                if (infection.ContainsKey(hero.BasePV.owner.ID))
                {
                    hero.MarkDie();
                    hero.BasePV.RPC("netDie2", PhotonTargets.All, -1, "noswitchingfgt");
                }
            }
        }

        private static void InfectionUpdate()
        {
            int count = InfectionMode.GetInt(0);
            if (count < 0 || count > PhotonNetwork.playerList.Length)
            {
                count = 1;
            }

            infection.Clear();
            foreach (PhotonPlayer player in PhotonNetwork.playerList)
            {
                player.IsTitan = false;
            }

            int players = PhotonNetwork.playerList.Length;
            foreach (var player in PhotonNetwork.playerList)
            {
                if (Random.Range(0f, 1f) <= count / (float)players)
                {
                    player.IsTitan = true;
                    infection.Add(player.ID, 2);
                    if (--count == 0)
                    {
                        break;
                    }
                }

                players--;
            }
        }

        public static void Load()
        {
            foreach (GameModeSetting set in allGameSettings)
            {
                set.Load();
            }
        }

        public static void OnRestart()
        {
            if (InfectionMode.Enabled)
            {
                return;
            }

            //else if (TeamMode.Enabled)
            //{
            //    SendTeamInfo();
            //}
            if (PointMode.Enabled)
            {
                foreach (PhotonPlayer player in PhotonNetwork.playerList)
                {
                    player.Kills = player.Deaths = player.TotalDamage = player.MaximumDamage = 0;
                }
            }
        }

        public static void ResetOnLoad()
        {
            foreach (GameModeSetting set in allGameSettings)
            {
                set.ForceDisable();
                set.Apply();
            }

            oldHash = new Hashtable();
        }

        public static void Save()
        {
            foreach (GameModeSetting set in allGameSettings)
            {
                set.Save();
            }
        }

        private static void SendTeamInfo()
        {
            int team = 1;
            foreach (var player in PhotonNetwork.playerList)
            {
                FengGameManagerMKII.FGM.BasePV.RPC("setTeamRPC", player,
                    (team++ % 2 == 0 ? 2 : 1));
            }
        }

        public static void SendRpc()
        {
            StringBuilder bld = new StringBuilder();
            Hashtable hash = new Hashtable();
            int count = 0;
            int countSend = 0;
            foreach (var set in allGameSettings)
            {
                if (set.Enabled && !set.HasChanged)
                {
                    set.WriteToHashtable(hash);
                    count++;
                    continue;
                }

                if (set.HasChanged)
                {
                    bool oldstate = set.State;
                    set.Apply();
                    if (oldstate != set.State && set.State == false)
                    {
                        set.Save();
                        continue;
                    }

                    set.WriteToHashtable(hash);
                    if (countSend > 0)
                    {
                        bld.Append("\n");
                    }

                    bld.Append(set.ToStringLocal());
                    set.Save();
                    count++;
                    countSend++;
                }
            }

            if (count <= 0)
            {
                if(Motd.Value.Trim().Length > 0 && oldHash["motd"] as string != Motd.Value.Trim())
                {
                    oldHash["motd"] = Motd.Value;
                    FengGameManagerMKII.FGM.BasePV.RPC("Chat", PhotonTargets.All, "MOTD: " + Motd.Value, string.Empty);
                }
                return;
            }

            if (InfectionMode.Enabled)
            {
                InfectionUpdate();
            }

            //if (TeamMode.Enabled)
            //{
            //    SendTeamInfo();
            //}
            FengGameManagerMKII.FGM.BasePV.RPC("settingRPC", PhotonTargets.Others, hash);

            if (Motd.Value.Trim().Length > 0 && oldHash["motd"] as string != Motd.Value.Trim())
            {
                oldHash["motd"] = Motd.Value;
                FengGameManagerMKII.FGM.BasePV.RPC("Chat", PhotonTargets.Others, "MOTD: " + Motd.Value, string.Empty);
            }
            if (bld.ToString() != string.Empty)
            {
                Chat.Add(bld.ToString());
            }
        }

        public static void SendRpcToPlayer(PhotonPlayer player)
        {
            Hashtable hash = new Hashtable();
            string vanillaString = string.Empty;
            string anarchyString = string.Empty;
            int count = 0;
            for (int i = 0; i < allGameSettings.Count; i++)
            {
                GameModeSetting set = allGameSettings[i];
                if (set.Enabled)
                {
                    set.WriteToHashtable(hash);
                    count++;
                    if (!player.AnarchySync && set is AnarchyGameModeSetting setting)
                    {
                        if (anarchyString.Length > 0)
                        {
                            anarchyString += "\n";
                        }

                        anarchyString += setting.ToString();
                    }
                    else if (!player.RCSync)
                    {
                        if (vanillaString.Length > 0)
                        {
                            vanillaString += "\n";
                        }

                        vanillaString += set.ToString();
                    }
                }
            }

            if (count <= 0)
            {
                if (Motd.Value.Trim().Length > 0)
                {
                    FengGameManagerMKII.FGM.BasePV.RPC("Chat", player, "MOTD: " + Motd.Value, string.Empty);
                }

                return;
            }

            FengGameManagerMKII.FGM.BasePV.RPC("settingRPC", player, hash);
            if (!player.RCSync & vanillaString.Length > 0)
            {
                FengGameManagerMKII.FGM.BasePV.RPC("Chat", player, vanillaString, string.Empty);
            }

            if (!player.AnarchySync && anarchyString.Length > 0)
            {
                FengGameManagerMKII.FGM.BasePV.RPC("Chat", player, anarchyString, string.Empty);
            }

            if (Motd.Value != string.Empty)
            {
                FengGameManagerMKII.FGM.BasePV.RPC("Chat", player, "MOTD: " + Motd.Value, string.Empty);
            }
        }
    }
}