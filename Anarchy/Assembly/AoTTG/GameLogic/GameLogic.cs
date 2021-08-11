using Anarchy;
using Anarchy.Commands.Chat;
using Optimization;
using RC;
using System;
using UnityEngine;

namespace GameLogic
{
    internal class GameLogic
    {
        public const float UpdateInterval = 0.05f;

        public readonly Anarchy.Localization.Locale Lang;
        protected internal float MyRespawnTime;
        public readonly Round Round;
        private float timeToUpdate;

        public float LifeTime { get; private set; }
        public GameMode Mode => IN_GAME_MAIN_CAMERA.GameMode;
        public bool Multiplayer => IN_GAME_MAIN_CAMERA.GameType == GameType.MultiPlayer;
        public float ServerTime { get; set; }
        public int CurrentRound { get; private set; } = 1;
        public float RoundTime { get => Round.Time; set => Round.Time = value; }
        public int HumanScore { get; set; }
        public int TitanScore { get; set; }
        public bool Restarting { get; set; } = false;
        public float ServerTimeBase { get; set; }
        public bool Stop { get; private set; } = false;

        public GameLogic()
        {
            LifeTime = 0f;
            Lang = new Anarchy.Localization.Locale("GameLogic", true, ',');
            Lang.Load();
            Round = new Round();
        }

        public GameLogic(GameLogic logic) : this()
        {
            CopyFrom(logic);
        }

        public bool CheckIsTitanAllDie()
        {
            foreach (TITAN tit in FengGameManagerMKII.Titans)
            {
                if (!tit.hasDie)
                {
                    return false;
                }
            }
            return FengGameManagerMKII.Annie == null;
        }

        public virtual void CopyFrom(GameLogic other)
        {
            if (other == null)
            {
                return;
            }
            ServerTime = other.ServerTime;
            ServerTimeBase = other.ServerTimeBase;
            HumanScore = other.HumanScore;
            TitanScore = other.TitanScore;
            RoundTime = other.RoundTime;
            CurrentRound = other.CurrentRound;
        }

        public void GameLose()
        {
            if (Stop || Round.IsWinning || Round.IsLosing)
            {
                return;
            }
            Round.IsLosing = true;
            TitanScore++;
            Round.GameEndCD = Round.GameEndTimer;
            OnGameLoose();
            AnarchyManager.Feed.RoundLoose();
            if (Multiplayer)
            {
                FengGameManagerMKII.FGM.BasePV.RPC("netGameLose", PhotonTargets.Others, new object[] { TitanScore });
            }
        }

        public void GameWin()
        {
            if (Stop || Round.IsWinning || Round.IsLosing)
            {
                return;
            }
            Round.IsWinning = true;
            HumanScore++;
            Round.GameEndCD = Round.GameEndTimer;
            OnGameWin();
            AnarchyManager.Feed.RoundWin();
            if (Multiplayer)
            {
                FengGameManagerMKII.FGM.BasePV.RPC("netGameWin", PhotonTargets.Others, new object[] { GetNetGameWinData() });
            }
        }

        protected virtual int GetNetGameWinData()
        {
            return HumanScore;
        }

        protected virtual string GetShowResultTitle()
        {
            return $"Humanity {HumanScore} : Titan {TitanScore}";
        }

        public bool IsPlayerAllDead()
        {
            foreach (PhotonPlayer player in PhotonNetwork.playerList)
            {
                if (player.IsTitan || player.Dead)
                {
                    continue;
                }

                return false;
            }
            return true;
        }

        public bool IsTeamAllDead(int team)
        {
            foreach (PhotonPlayer player in PhotonNetwork.playerList)
            {
                if (player.IsTitan)
                {
                    continue;
                }

                if (player.Team == team && !player.Dead)
                {
                    return false;
                }
            }
            return true;
        }

        public void NetGameLose(int score)
        {
            Round.IsLosing = true;
            TitanScore = score;
            Round.GameEndCD = Round.GameEndTimer;
            OnNetGameLose(score);
            AnarchyManager.Feed.RoundLoose();
        }

        public void NetGameWin(int score)
        {
            Round.IsWinning = true;
            HumanScore = score;
            Round.GameEndCD = Round.GameEndTimer;
            OnNetGameWin(score);
            AnarchyManager.Feed.RoundWin();
        }

        protected virtual void OnGameLoose()
        {
        }

        public void OnGameRestart()
        {
            if (Stop)
            {
                return;
            }
            Round.Reset();
            Lang.Reload();
            CurrentRound++;
            OnRestart();
            GameModes.AntiReviveClear();
            Restarting = false;
            if (IN_GAME_MAIN_CAMERA.GameType == GameType.MultiPlayer)
            {
                if (PhotonNetwork.IsMasterClient)
                {
                    OnRequireStatus();
                }
                AnarchyManager.Feed.RoundStart();
            }
        }

        protected virtual void OnGameWin()
        {
        }

        public void OnLateUpdate()
        {
            if (Stop)
            {
                return;
            }
            float unscaled = Time.unscaledDeltaTime;
            LifeTime += unscaled;
            timeToUpdate -= unscaled;
            ServerTime -= Time.deltaTime;
            UpdateInputs();
            Round.OnLateUpdate();
            if (timeToUpdate <= 0f)
            {
                Labels.TopCenter = "";
                UpdateLogic();
                UpdateLabels();
                if (Multiplayer)
                {
                    UpdateTeams();
                    UpdateCustomLogic();
                    if (FengGameManagerMKII.FGM.needChooseSide)
                    {
                        Labels.TopCenter += "\n\n" + Lang.Format("pressToJoin", 1.ToString());
                    }
                    FengGameManagerMKII.FGM.KillInfoUpdate();
                    UpdateRespawnTime();
                    if (PhotonNetwork.IsMasterClient)
                    {
                        if (ServerTime <= 0f)
                        {
                            Stop = true;
                            ShowResult();
                        }
                        if (Round.GameEndCD <= 0f)
                        {
                            FengGameManagerMKII.FGM.RestartGame(false, false);
                        }
                    }
                }
                timeToUpdate = UpdateInterval;
            }
        }

        protected virtual void OnNetGameLose(int score)
        {
        }

        protected virtual void OnNetGameWin(int score)
        {
        }

        public virtual void OnRefreshStatus(int score1, int score2, int wav, int highestWav, float time1, float time2, bool startRacin, bool endRacin)
        {
            HumanScore = score1;
            TitanScore = score2;
            Round.Time = time1;
            ServerTime = ServerTimeBase - time2;
        }

        protected virtual void OnRestart()
        {
        }

        public virtual void OnRequireStatus()
        {
            FengGameManagerMKII.FGM.BasePV.RPC("refreshStatus", PhotonTargets.Others, new object[]
            {
                HumanScore,
                TitanScore,
                0,
                0,
                Round.Time,
                (ServerTimeBase - ServerTime),
                false,
                false
            });
        }

        public virtual void OnSomeOneIsDead(int id)
        {
        }

        public virtual void OnTitanDown(string name, bool isLeaving)
        {
        }

        protected internal virtual void OnUpdate()
        {
        }

        private void ShowResult()
        {
            IN_GAME_MAIN_CAMERA.GameType = GameType.Stop;
            FengGameManagerMKII.FGM.gameStart = false;
            Screen.lockCursor = false;
            Screen.showCursor = true;
            string names = string.Empty;
            string kills = string.Empty;
            string deaths = string.Empty;
            string maxs = string.Empty;
            string totals = string.Empty;
            string title = GetShowResultTitle();
            foreach (PhotonPlayer player in PhotonNetwork.playerList)
            {
                names += player.UIName + "\n";
                kills += player.Kills + "\n";
                deaths += player.Deaths + "\n";
                maxs += player.MaximumDamage + "\n";
                totals += player.TotalDamage + "\n";
            }
            FengGameManagerMKII.FGM.BasePV.RPC("showResult", PhotonTargets.AllBuffered, new object[] { names, kills, deaths, maxs, totals, title });
        }

        private void UpdateCustomLogic()
        {
            if (PhotonNetwork.IsMasterClient && !Restarting)
            {
                CustomLevel.OnUpdate();
                if (CustomLevel.logicLoaded)
                {
                    for (int i = 0; i < CustomLevel.titanSpawners.Count; i++)
                    {
                        var item = CustomLevel.titanSpawners[i];
                        item.time -= UpdateInterval;
                        if (item.time < 0f && FengGameManagerMKII.Titans.Count + (FengGameManagerMKII.Annie != null ? 1 : 0) < Math.Min(RC.RCManager.SpawnCapCustom, 80))
                        {
                            if (item.name == "spawnannie" && FengGameManagerMKII.Annie == null)
                            {
                                Optimization.Caching.Pool.NetworkEnable("FEMALE_TITAN", item.location, Quaternion.identity, 0);
                            }
                            else
                            {
                                TITAN tit = Optimization.Caching.Pool.NetworkEnable("TITAN_VER3.1", item.location, Quaternion.identity, 0).GetComponent<TITAN>();
                                AbnormalType type = AbnormalType.Normal;
                                switch (item.name)
                                {
                                    case "spawnAbnormal":
                                        type = AbnormalType.Aberrant;
                                        break;

                                    case "spawnJumper":
                                        type = AbnormalType.Jumper;
                                        break;

                                    case "spawnCrawler":
                                        type = AbnormalType.Crawler;
                                        break;

                                    case "spawnPunk":
                                        type = AbnormalType.Punk;
                                        break;

                                    default:
                                        break;
                                }
                                tit.SetAbnormalType(type, type == AbnormalType.Crawler);
                                if (item.endless)
                                {
                                    item.time = item.delay;
                                }
                                else
                                {
                                    CustomLevel.titanSpawners.Remove(item);
                                }
                            }
                        }
                    }
                }
            }
        }

        private void UpdateInputs()
        {
            if (IN_GAME_MAIN_CAMERA.GameType != GameType.Single && FengGameManagerMKII.FGM.needChooseSide)
            {
                if (InputManager.IsInputDown[InputCode.Flare1] && !AnarchyManager.Pause.IsActive)
                {
                    if (AnarchyManager.CharacterSelectionPanel.IsActive)
                    {
                        Screen.lockCursor = true;
                        Screen.showCursor = true;
                        IN_GAME_MAIN_CAMERA.SpecMov.disable = false;
                        IN_GAME_MAIN_CAMERA.Look.disable = false;
                        AnarchyManager.CharacterSelectionPanel.Disable();
                    }
                    else
                    {
                        Screen.lockCursor = false;
                        Screen.showCursor = true;
                        IN_GAME_MAIN_CAMERA.SpecMov.disable = true;
                        IN_GAME_MAIN_CAMERA.Look.disable = true;
                        AnarchyManager.CharacterSelectionPanel.Enable();
                    }
                }
                if (InputManager.IsInputDown[InputCode.Pause] && !AnarchyManager.CharacterSelectionPanel.IsActive)
                {
                    if (!AnarchyManager.Pause.IsActive)
                    {
                        AnarchyManager.Pause.Enable();
                        Screen.showCursor = true;
                        Screen.lockCursor = false;
                        IN_GAME_MAIN_CAMERA.SpecMov.disable = true;
                        IN_GAME_MAIN_CAMERA.Look.disable = true;
                        InputManager.MenuOn = true;
                        IN_GAME_MAIN_CAMERA.isPausing = true;
                    }
                }
            }
        }

        protected virtual void UpdateLabels()
        {
            Labels.Center = string.Empty;
            if (Round.IsWinning && Round.GameEndCD >= 0f)
            {
                if (Multiplayer)
                {
                    Labels.Center = Lang.Format("humanityWin", Round.GameEndCD.ToString("F0")) + "\n\n";
                }
                else
                {
                    Labels.Center = Lang.Format("humanitySingleWin", Anarchy.InputManager.Settings[InputCode.Restart].ToString()) + "\n\n";
                }
            }
            else if (Round.IsLosing && Round.GameEndCD >= 0f)
            {
                if (Multiplayer)
                {
                    Labels.Center = Lang.Format("humanityFail", Round.GameEndCD.ToString("F0")) + "\n\n";
                }
                else
                {
                    Labels.Center = Lang.Format("humanitySingleFail", Anarchy.InputManager.Settings[InputCode.Restart].ToString()) + "\n\n";
                }
            }
            string top = "";
            if (FengGameManagerMKII.Titans.Count > 0)
            {
                top += Lang.Format("titans", FengGameManagerMKII.Titans.Count.ToString());
            }
            if (top != "")
            {
                top += " ";
            }
            top += Lang.Format("time", (IN_GAME_MAIN_CAMERA.GameType == GameType.Single ? (FengGameManagerMKII.FGM.logic.RoundTime).ToString("F0") : (FengGameManagerMKII.FGM.logic.ServerTime).ToString("F0")));
            Labels.TopCenter = top;
        }

        protected virtual void UpdateLogic()
        {
        }

        protected virtual void UpdateRespawnTime()
        {
            if ((IN_GAME_MAIN_CAMERA.MainCamera.gameOver && !FengGameManagerMKII.FGM.needChooseSide) && (FengGameManagerMKII.Level.RespawnMode == RespawnMode.DEATHMATCH || GameModes.EndlessRespawn.Enabled || ((GameModes.BombMode.Enabled || GameModes.BladePvp.Enabled) && GameModes.PointMode.Enabled)) && !SpectateCommand.IsInSpecMode)
            {
                this.MyRespawnTime += UpdateInterval;
                int num = 10;
                if (PhotonNetwork.player.IsTitan)
                {
                    num = 15;
                }
                if (GameModes.EndlessRespawn.Enabled)
                {
                    num = GameModes.EndlessRespawn.GetInt(0);
                }
                Labels.Center += "\n" + Lang.Format("respawnTime", (num - MyRespawnTime).ToString("F0")) + "\n\n";
                if (this.MyRespawnTime > (float)num)
                {
                    this.MyRespawnTime = 0f;
                    IN_GAME_MAIN_CAMERA.MainCamera.gameOver = false;
                    if (PhotonNetwork.player.IsTitan)
                    {
                        FengGameManagerMKII.FGM.SpawnNonAiTitan(FengGameManagerMKII.FGM.myLastHero, "titanRespawn");
                    }
                    else
                    {
                        FengGameManagerMKII.FGM.SpawnPlayer(FengGameManagerMKII.FGM.myLastHero);
                    }
                    IN_GAME_MAIN_CAMERA.MainCamera.gameOver = false;
                    Labels.Center = string.Empty;
                }
            }
        }

        private void UpdateTeams()
        {
            if (GameModes.TeamMode.Enabled)
            {
                Labels.TopCenter = Labels.TopCenter.AppendLine(Anarchy.Configuration.GameModeSetting.Lang.Format("cyanScore", AnarchyExtensions.GetCyanPoints().ToString()) + "      " + Anarchy.Configuration.GameModeSetting.Lang.Format("magentaScore", AnarchyExtensions.GetMagentaPoints().ToString()));
            }
        }
    }
}