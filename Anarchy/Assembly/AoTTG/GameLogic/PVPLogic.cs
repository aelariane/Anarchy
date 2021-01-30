namespace GameLogic
{
    internal class PVPLogic : GameLogic
    {
        public int[] Scores { get; set; } = new int[2];
        private int teamWinner;

        public PVPLogic() : base()
        {
        }

        public PVPLogic(GameLogic logic) : this()
        {
            CopyFrom(logic);
        }

        public override void CopyFrom(GameLogic other)
        {
            base.CopyFrom(other);
            if (other == null || !(other is PVPLogic pvplog))
            {
                return;
            }
            Scores = pvplog.Scores;
        }

        protected override int GetNetGameWinData()
        {
            return teamWinner;
        }

        protected override string GetShowResultTitle()
        {
            return $"Team1 {Scores[0]} : Team2 {Scores[1]}";
        }

        protected override void OnGameWin()
        {
            Scores[teamWinner - 1]++;
        }

        protected override void OnNetGameWin(int score)
        {
            teamWinner = score;
            Scores[teamWinner - 1]++;
        }

        public override void OnRequireStatus()
        {
            base.OnRequireStatus();
            FengGameManagerMKII.FGM.BasePV.RPC("refreshPVPStatus_AHSS", PhotonTargets.Others, new object[] { Scores });
        }

        protected override void OnRestart()
        {
            teamWinner = 0;
        }

        public override void OnSomeOneIsDead(int id)
        {
            if (IsPlayerAllDead())
            {
                teamWinner = 0;
                GameLose();
            }
            else if (IsTeamAllDead(2))
            {
                teamWinner = 1;
                GameWin();
            }
            else if (IsTeamAllDead(1))
            {
                teamWinner = 2;
                GameWin();
            }
        }

        protected override void UpdateLabels()
        {
            string center = "";
            if (Round.IsWinning && Round.GameEndCD >= 0f)
            {
                center = Lang.Format("pvpWin", teamWinner.ToString(), Round.GameEndCD.ToString("F0")) + "\n\n";
            }
            else if (Round.IsLosing && Round.GameEndCD >= 0f)
            {
                center = Lang.Format("humanityFail", Round.GameEndCD.ToString("F0")) + "\n\n";
            }
            if (center != "")
            {
                center += "\n\n";
            }
            //TODO: Top center label
            Optimization.Labels.Center = center;
        }
    }
}