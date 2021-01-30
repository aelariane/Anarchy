using Anarchy;
using Optimization;

namespace GameLogic
{
    internal class SurviveLogic : GameLogic
    {
        public int MaxWave { get; private set; } = 1;

        public SurviveLogic() : base()
        {
        }

        public SurviveLogic(GameLogic logic) : this()
        {
            CopyFrom(logic);
        }

        public override void OnTitanDown(string name, bool isLeaving)
        {
            if (!Round.IsWinning && !Round.IsLosing)
            {
                GameModes.CheckGameEnd();
            }
            if (this.CheckIsTitanAllDie())
            {
                GameModes.AntiReviveClear();
                Round.Wave++;
                if (FengGameManagerMKII.Level.RespawnMode == RespawnMode.NEWROUND && IN_GAME_MAIN_CAMERA.GameType == GameType.MultiPlayer)
                {
                    FengGameManagerMKII.FGM.BasePV.RPC("respawnHeroInNewRound", PhotonTargets.All, new object[0]);
                }
                if (IN_GAME_MAIN_CAMERA.GameType == GameType.MultiPlayer)
                {
                    FengGameManagerMKII.FGM.SendChatContentInfo(User.Wave(Round.Wave));
                }
                if (Round.Wave > MaxWave)
                {
                    MaxWave = Round.Wave;
                }
                if (PhotonNetwork.IsMasterClient)
                {
                    OnRequireStatus();
                }
                if (GameModes.MaxWave.Enabled ? Round.Wave > GameModes.MaxWave.GetInt(0) : Round.Wave > 20)
                {
                    GameWin();
                }
                else
                {
                    int rate = 90;
                    if (FengGameManagerMKII.FGM.difficulty == 1)
                    {
                        rate = 70;
                    }
                    if (!FengGameManagerMKII.Level.PunksEnabled)
                    {
                        FengGameManagerMKII.FGM.SpawnTitansCustom(rate, Round.Wave + 2, false);
                    }
                    else
                    {
                        if (Round.Wave % 5 == 0)
                        {
                            FengGameManagerMKII.FGM.SpawnTitansCustom(rate, Round.Wave / 5, true);
                        }
                        else
                        {
                            FengGameManagerMKII.FGM.SpawnTitansCustom(rate, Round.Wave + 2, false);
                        }
                    }
                }
            }
        }

        public override void OnRefreshStatus(int score1, int score2, int wav, int highestWav, float time1, float time2, bool startRacin, bool endRacin)
        {
            HumanScore = score1;
            TitanScore = score2;
            Round.Wave = wav;
            MaxWave = highestWav;
            Round.Time = time1;
            ServerTime = ServerTimeBase - time2;
        }

        public override void OnRequireStatus()
        {
            FengGameManagerMKII.FGM.BasePV.RPC("refreshStatus", PhotonTargets.Others, new object[]
            {
                HumanScore,
                TitanScore,
                Round.Wave,
                MaxWave,
                Round.Time,
                (ServerTimeBase - ServerTime),
                false,
                false
            });
        }

        public override void OnSomeOneIsDead(int id)
        {
            if (!Round.IsWinning && !Round.IsLosing)
            {
                GameModes.CheckGameEnd();
            }
        }

        protected override void UpdateLabels()
        {
            string center = string.Empty;
            string top = string.Empty;
            top += Lang.Format("titans", FengGameManagerMKII.Titans.Count.ToString());
            top += " " + Lang.Format("wave", Round.Wave.ToString());
            if (!Multiplayer)
            {
                Labels.TopLeft = Lang.Format("singleState", FengGameManagerMKII.FGM.singleKills.ToString(), FengGameManagerMKII.FGM.singleMax.ToString(), FengGameManagerMKII.FGM.singleTotal.ToString());
            }
            if (Round.IsWinning && Round.GameEndCD >= 0f)
            {
                if (Multiplayer)
                {
                    center = Lang.Format("surviveWin", Round.Wave.ToString(), Round.GameEndCD.ToString("F0"));
                }
                else
                {
                    center = Lang.Format("surviveSingleWin", Round.Wave.ToString(), Anarchy.InputManager.Settings[InputCode.Restart].ToString());
                }
            }
            else if (Round.IsLosing && Round.GameEndCD >= 0f)
            {
                if (Multiplayer)
                {
                    center = Lang.Format("surviveFail", Round.Wave.ToString(), Round.GameEndCD.ToString("F0"));
                }
                else
                {
                    center = Lang.Format("surviveSingleFail", Round.Wave.ToString(), Anarchy.InputManager.Settings[InputCode.Restart].ToString());
                }
            }
            if (center != "")
            {
                center += "\n\n";
            }
            Labels.Center = center;
            Labels.TopCenter = top;
        }
    }
}