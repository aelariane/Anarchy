using Anarchy;
using Optimization;

namespace GameLogic
{
    internal class PVPCaptureLogic : GameLogic
    {
        public int PVPHumanScore { get; set; }
        public int PVPHumanScoreMax { get; set; } = 200;
        public int PVPTitanScore { get; set; }
        public int PVPTitanScoreMax { get; set; } = 200;

        public PVPCaptureLogic() : base()
        {
        }

        public void CheckPVPpts()
        {
            if (this.PVPHumanScore >= this.PVPHumanScoreMax)
            {
                this.PVPHumanScore = this.PVPHumanScoreMax;
                if (PhotonNetwork.IsMasterClient)
                {
                    this.GameWin();
                }
            }
            else if (this.PVPTitanScore >= this.PVPTitanScoreMax)
            {
                this.PVPTitanScore = this.PVPTitanScoreMax;
                if (PhotonNetwork.IsMasterClient)
                {
                    this.GameLose();
                }
            }
        }

        public override void OnRequireStatus()
        {
            base.OnRequireStatus();
            FengGameManagerMKII.FGM.BasePV.RPC("refreshPVPStatus", PhotonTargets.Others, new object[]
            {
                this.PVPHumanScore,
                this.PVPTitanScore
            });
        }

        public override void OnSomeOneIsDead(int id)
        {
            PVPTitanScore = ((id != 0) ? (PVPTitanScore + 2) : PVPTitanScore);
            CheckPVPpts();
            FengGameManagerMKII.FGM.BasePV.RPC("refreshPVPStatus", PhotonTargets.Others, new object[]
            {
                this.PVPHumanScore,
                this.PVPTitanScore
            });
        }

        protected override void OnRestart()
        {
            PVPHumanScore = 0;
            PVPTitanScore = 0;
        }

        public override void OnTitanDown(string name, bool isLeaving)
        {
            if (name != string.Empty)
            {
                if (name == "Titan")
                {
                    this.PVPHumanScore++;
                }
                else if (name == "Aberrant")
                {
                    this.PVPHumanScore += 2;
                }
                else if (name == "Jumper")
                {
                    this.PVPHumanScore += 3;
                }
                else if (name == "Crawler")
                {
                    this.PVPHumanScore += 4;
                }
                else if (name == "Female Titan")
                {
                    this.PVPHumanScore += 10;
                }
                else
                {
                    this.PVPHumanScore += 3;
                }
            }
            this.CheckPVPpts();
            FengGameManagerMKII.FGM.BasePV.RPC("refreshPVPStatus", PhotonTargets.Others, new object[]
            {
                this.PVPHumanScore,
                this.PVPTitanScore
            });
        }

        protected override void UpdateLabels()
        {
            Labels.Center = string.Empty;
            if (Round.IsWinning && Round.GameEndCD >= 0f)
            {
                Labels.Center = Lang.Format("humanityWin", Round.GameEndCD.ToString("F0")) + "\n\n";
            }
            else if (Round.IsLosing && Round.GameEndCD >= 0f)
            {
                Labels.Center = Lang.Format("humanityFail", Round.GameEndCD.ToString("F0")) + "\n\n";
            }
            string top = "";
            string info = "| ";
            for (int j = 0; j < PVPcheckPoint.chkPts.Count; j++)
            {
                info = info + (PVPcheckPoint.chkPts[j] as PVPcheckPoint).getStateString() + " ";
            }
            info += " |";
            top = $"<color=#{User.MainColor.ToValue()}>{PVPTitanScoreMax - PVPTitanScore} {info.ToHTMLFormat()} {PVPHumanScoreMax - PVPHumanScore}</color>";

            top += "\n" + Lang.Format("time", (IN_GAME_MAIN_CAMERA.GameType == GameType.Single ? FengGameManagerMKII.FGM.logic.RoundTime : ServerTime).ToString("F0"));
            Labels.TopCenter = top;
        }

        protected override void UpdateLogic()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                CheckPVPpts();
            }
        }
    }
}