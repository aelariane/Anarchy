using System.Collections.Generic;
using System.Text;
using Anarchy;
using Optimization;
using RC;
using UnityEngine;
using Setting = Anarchy.Configuration.GameModeSetting;

namespace GameLogic
{
    internal class RacingLogic : GameLogic
    {
        public const int MaxFinishers = 10;
        private const float ShowFinishedLabelTime = 7f;

        private bool customRestartTime;
        private float showFinishersTime = 0f;
        private bool needShowFinishers = false;
        private bool doorsDestroyed;
        private bool needDestroyDoors;
        private string topCenter = "";
        private string center = "";
        private float maxSpeed = 0f;
        private int finishersCount;
        private readonly List<string> finishers = new List<string>();

        public bool CustomRestartTime
        {
            get
            {
                return customRestartTime;
            }
            set
            {
                if (!value)
                {
                    RestartTime = 20f;
                }
                customRestartTime = value;
            }
        }

        public int FinishersCount
        {
            get => finishersCount;
            private set
            {
                finishersCount = value;
                if (PhotonNetwork.IsMasterClient && GameModes.RacingFinishersRestart.Enabled && value >= GameModes.RacingFinishersRestart.GetInt(0))
                {
                    FengGameManagerMKII.FGM.RestartGame(false, false);
                }
            }
        }

        public bool RaceStart { get; private set; }

        public float RestartTime
        {
            get => Round.GameEndTimer;
            set => Round.GameEndTimer = value;
        }
        public float StartTime { get; set; } = 20f;

        public RacingLogic() : base()
        {
            StartTime = IN_GAME_MAIN_CAMERA.GameType == GameType.Single ? 5f : (GameModes.RacingStartTime.Enabled ? GameModes.RacingStartTime.GetInt(0) : 20f);
            if (GameModes.ASORacing.Enabled)
            {
                CustomRestartTime = true;
                RestartTime = 999f;
            }
            else
            {
                RestartTime = 20f;
            }
        }

        public RacingLogic(GameLogic logic) : this()
        {
            CopyFrom(logic);
        }

        public static void ASORacingCheck(Setting set, bool state, bool rcv)
        {
            if (FengGameManagerMKII.FGM.Logic is RacingLogic log)
            {
                log.CustomRestartTime = state;
                if (state)
                {
                    log.RestartTime = 999f;
                }
                else
                {
                    if (!GameModes.RacingRestartTime.Enabled)
                    {
                        log.RestartTime = 20f;
                    }
                }
            }
        }

        private string GetFinishers()
        {
            if (Multiplayer && !PhotonNetwork.IsMasterClient)
            {
                if (FengGameManagerMKII.FGM.LocalRacingResult.Length <= 0)
                {
                    return string.Empty;
                }
                return "<color=#" + User.MainColor.ToValue() + ">" + FengGameManagerMKII.FGM.LocalRacingResult.ToHTMLFormat() + "</color>\n";
            }
            if (finishers.Count == 0)
            {
                return string.Empty;
            }
            StringBuilder bld = new StringBuilder();
            bld.Append(Lang["racingResult"] + "\n");
            lock (finishers)
            {
                foreach (string str in finishers)
                {
                    bld.Append(str + "\n");
                }
            }
            return bld.ToString();
        }

        protected override string GetShowResultTitle()
        {
            return $"Rounds finished {HumanScore}";
        }

        protected override void OnGameWin()
        {
            if (!Multiplayer)
            {
                OnPlayerFinishedSingle();
                needShowFinishers = true;
            }
        }

        public override void OnRefreshStatus(int score1, int score2, int wav, int highestWav, float time1, float time2, bool startRacin, bool endRacin)
        {
            HumanScore = score1;
            TitanScore = score2;
            if (!GameModes.RacingStartTime.Enabled)
            {
                Round.Time = time1;
            }
            else
            {
                Round.Time = time1 - (20f - GameModes.RacingStartTime.GetInt(0));
            }
            ServerTime = ServerTimeBase - time2;
            if (startRacin)
            {
                RaceStart = true;
                needDestroyDoors = true;
                doorsDestroyed = false;
                Labels.Center = "";
            }
        }

        public override void OnRequireStatus()
        {
            if (!GameModes.RacingStartTime.Enabled)
            {
                FengGameManagerMKII.FGM.BasePV.RPC("refreshStatus", PhotonTargets.Others, new object[]
                { 
                    HumanScore,
                    TitanScore,
                    0,
                    0,
                    Round.Time,
                    (ServerTimeBase - ServerTime),
                    RaceStart,
                    false
                });
            }
            else
            {
                FengGameManagerMKII.FGM.BasePV.RPC("refreshStatus", PhotonTargets.Others, new object[]
                {
                        HumanScore,
                        TitanScore,
                        0,
                        0,
                        20f - (float)GameModes.RacingStartTime.GetInt(0) + Round.Time,
                        (ServerTimeBase - ServerTime),
                        RaceStart,
                        false
                });
            }
        }

        public void OnUpdateRacingResult()
        {
            if (!needShowFinishers)
            {
                needShowFinishers = true;
            }
            showFinishersTime = 20f;
            FinishersCount++;
        }

        public void OnPlayerFinished(float time, string name)
        {
            if (finishers.Count >= MaxFinishers)
            {
                return;
            }
            if (!Round.IsWinning)
            {
                finishers.Clear();
                GameWin();
            }
            lock (finishers)
            {
                finishers.Add(Lang.Format("racingPlayerResult", (finishers.Count + 1).ToString(), name.ToHTMLFormat(), time.ToString("F2")));
                UpdateLabels();
                OnUpdateRacingResult();
            }
        }

        private void OnPlayerFinishedSingle()
        {
            finishers.Clear();
            lock (finishers)
            {
                finishers.Add(Lang.Format("racingPlayerResultSingle", (Round.Time - StartTime).ToString("F2"), Anarchy.InputManager.Settings[InputCode.Restart].ToString()));
                UpdateLabels();
                needShowFinishers = true;
            }
        }


        protected override void OnRestart()
        {
            needShowFinishers = false;
            RaceStart = false;
            FinishersCount = 0;
            if (FengGameManagerMKII.Level.Name.Contains("Custom"))
            {
                needDestroyDoors = true;
            }
            doorsDestroyed = false;
            if (PhotonNetwork.IsMasterClient && GameModes.RacingStartTime.Enabled)
            {
                OnRequireStatus();
            }
        }

        public static void RestartTimeCheck(Setting sender, bool state, bool rcv)
        {
            if (FengGameManagerMKII.FGM.Logic is RacingLogic log)
            {
                log.CustomRestartTime = state;
                if (state)
                {
                    if (sender.GetInt(0) == 999 || sender.GetInt(0) == 1000)
                    {
                        sender.State = false;
                        if (!GameModes.ASORacing.Enabled)
                        {
                            GameModes.ASORacing.State = true;
                        }
                        return;
                    }
                    else if (GameModes.ASORacing.Enabled)
                    {
                        GameModes.ASORacing.State = false;
                    }
                    Anarchy.Configuration.AnarchyGameModeSetting.AnarchySettingCallback(sender, true, false);
                    log.RestartTime = sender.GetInt(0);
                }
                else
                {
                    log.RestartTime = 20f;
                    Anarchy.Configuration.AnarchyGameModeSetting.AnarchySettingCallback(sender, false, false);
                }
            }
        }

        public static void StartTimeCheck(Setting set, bool state, bool rcv)
        {
            if (FengGameManagerMKII.FGM.Logic is RacingLogic log)
            {
                if (state)
                {
                    log.StartTime = set.GetInt(0);
                }
                else
                {
                    log.StartTime = 20f;
                }
            }
        }

        public void TryDestroyDoors()
        {
            GameObject obj = GameObject.Find("door");
            if (obj != null)
            {
                obj.SetActive(false);
            }
            if (CustomLevel.racingDoors != null && CustomLevel.customLevelLoaded)
            {
                foreach (GameObject gameObject in CustomLevel.racingDoors)
                {
                    gameObject.SetActive(false);
                }
                CustomLevel.racingDoors = null;
            }
        }

        protected override void UpdateLabels()
        {
            if (RaceStart)
            {
                topCenter = Lang.Format("time", (Round.Time - StartTime).ToString("F1"));
                if(PhotonNetwork.IsMasterClient && GameModes.RacingTimeLimit.Enabled)
                {
                    if((Round.Time - StartTime) >= GameModes.RacingTimeLimit.GetInt(0))
                    {
                        FengGameManagerMKII.FGM.RestartGame(false, false);
                        return;
                    }
                }
                if (!Multiplayer)
                {
                    float currentSpeed = IN_GAME_MAIN_CAMERA.MainR.velocity.magnitude;
                    maxSpeed = Mathf.Max(maxSpeed, currentSpeed);
                    Labels.TopLeft = Lang.Format("speedLabel", currentSpeed.ToString("F2"), maxSpeed.ToString("F2"));
                }
                center = "";
                if (Round.IsWinning)
                {
                    if (needShowFinishers)
                    {
                        center += GetFinishers() + "\n\n";
                    }
                    if (Multiplayer)
                    {
                        topCenter += "\n\n" + Lang.Format("racingRestart", Round.GameEndCD.ToString("F0"));
                    }
                }

            }
            else
            {
                topCenter = Lang["racingWaiting"] + "\n";
                center = "";
                if (Multiplayer && RoundsCount > 1 && GetFinishers().Length > 0)
                {
                    center += Lang["racingLastResult"] + "\n";
                    center += GetFinishers();
                    center += "\n\n";
                }
                topCenter += "\n" + Lang.Format("racingRemaining", (StartTime - Round.Time).ToString("F0"));
            }

            Labels.TopCenter = topCenter;
            Labels.Center = center;
            center = "";
            topCenter = "";
        }

        protected override void UpdateLogic()
        {
            if (!RaceStart)
            {
                if (Round.Time >= StartTime)
                {
                    RaceStart = true;
                    TryDestroyDoors();
                    Labels.TopCenter = string.Empty;
                    Labels.Center = string.Empty;
                }
                if (GameModes.RacingRestartTime.Enabled && PhotonNetwork.IsMasterClient)
                {
                    OnRequireStatus();
                }
            }
            if (needDestroyDoors && !doorsDestroyed && RaceStart && CustomLevel.customLevelLoaded)
            {
                TryDestroyDoors();
                doorsDestroyed = true;
            }
            if(IN_GAME_MAIN_CAMERA.GameType == GameType.Single)
            {
                UpdateRespawnTime();
            }
        }

        protected internal override void OnUpdate()
        {
            if (Multiplayer)
            {
                if (Input.GetKey(KeyCode.Tab))
                {
                    needShowFinishers = true;
                    UpdateLabels();
                    return;
                }
                if (needShowFinishers)
                {
                    showFinishersTime -= Time.unscaledDeltaTime;
                    if (showFinishersTime <= 0f)
                    {
                        needShowFinishers = false;
                        UpdateLabels();
                    }
                }
            }
        }

        protected override void UpdateRespawnTime()
        {
            if (IN_GAME_MAIN_CAMERA.MainCamera.gameOver && !FengGameManagerMKII.FGM.NeedChooseSide)
            {
                this.MyRespawnTime += UpdateInterval;
                if (this.MyRespawnTime > 1.5f)
                {
                    this.MyRespawnTime = 0f;
                    FengGameManagerMKII.FGM.SpawnPlayer(FengGameManagerMKII.FGM.myLastHero);
                    Labels.Center = string.Empty;
                }
            }
        }
    }
}
