using System.Collections.Generic;
using System.Text;
using Anarchy;
using Optimization;
using RC;
using UnityEngine;

namespace GameLogic
{
    internal class RacingLogic : GameLogic
    {
        const int MaxFinishers = 10;
        private bool customRestartTime;
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

        private const float ShowFinishedLabelTime = 7f;
        public float RestartTime
        {
            get => Round.GameEndTimer;
            set => Round.GameEndTimer = value;
        }
        private float showFinishersTime = 0f;
        private bool needShowFinishers = false;
        public float StartTime { get; set; } = 20f;
        public bool RaceStart { get; private set; }
        private bool doorsDestroyed;
        private bool needDestroyDoors;
        private string topCenter = "";
        private string center = "";
        private float maxSpeed = 0f;
        public int FinishersCount { get; private set; }
        private readonly List<string> finishers = new List<string>();

        public RacingLogic() : base()
        {
            OnRestart += () =>
            {
                needShowFinishers = false;
                RaceStart = false;
                if (FengGameManagerMKII.Level.Name.Contains("Custom"))
                {
                    needDestroyDoors = true;
                }
                doorsDestroyed = false;
            };
            StartTime = IN_GAME_MAIN_CAMERA.GameType == GameType.Single ? 5f : 20f; 
            if (GameModes.ASORacing.Enabled)
            {
                CustomRestartTime = true;
                RestartTime = 999f;
            }
        }

        public RacingLogic(GameLogic logic) : this()
        {
            CopyFrom(logic);
        }

        private string GetFinishers()
        {
            if (Multiplayer && !PhotonNetwork.IsMasterClient)
            {
                if(FengGameManagerMKII.FGM.LocalRacingResult.Length <= 0)
                {
                    return string.Empty;
                }
                return "<color=#" + User.MainColor.ToValue() + ">" + FengGameManagerMKII.FGM.LocalRacingResult.ToHTMLFormat() + "</color>\n";
            }
            if(finishers.Count == 0)
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
            base.OnRefreshStatus(score1, score2, wav, highestWav, time1, time2, startRacin, endRacin);
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

        public void OnUpdateRacingResult()
        {
            if (!needShowFinishers)
            {
                needShowFinishers = true;
            }
            showFinishersTime = 20f;
        }

        public void OnPlayerFinished(float time, string name)
        {
            if(finishers.Count >= MaxFinishers)
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
                topCenter +=  "\n" + Lang.Format("racingRemaining", (StartTime - Round.Time).ToString("F0"));

            }

            Labels.TopCenter = topCenter;
            Labels.Center = center;
            center = "";
            topCenter = "";
        }

        protected override void UpdateLogic()
        {
            if (!RaceStart && Round.Time >= StartTime)
            {
                RaceStart = true;
                TryDestroyDoors();
                Labels.TopCenter = string.Empty;
                Labels.Center = string.Empty;
            }
            if (needDestroyDoors && !doorsDestroyed && RaceStart && CustomLevel.customLevelLoaded)
            {
                TryDestroyDoors();
                doorsDestroyed = true;
            }
        }

        protected internal override void OnUpdate()
        {
            if (needShowFinishers && Multiplayer)
            {
                showFinishersTime -= Time.unscaledDeltaTime;
                if (showFinishersTime <= 0f)
                {
                    needShowFinishers = false;
                    UpdateLabels();
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
                    IN_GAME_MAIN_CAMERA.MainCamera.gameOver = false;
                    FengGameManagerMKII.FGM.SpawnPlayer(FengGameManagerMKII.FGM.myLastHero);
                    IN_GAME_MAIN_CAMERA.MainCamera.gameOver = false;
                    Labels.Center = string.Empty;
                }
            }
        }
    }
}
