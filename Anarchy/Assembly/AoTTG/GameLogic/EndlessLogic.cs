using Optimization;

namespace GameLogic
{
    internal class EndlessLogic : GameLogic
    {
        public EndlessLogic() : base()
        {
        }

        public EndlessLogic(GameLogic logic) : this()
        {
            CopyFrom(logic);
        }

        public override void OnSomeOneIsDead(int id)
        {
            TitanScore++;
        }

        public override void OnTitanDown(string name, bool isLeaving)
        {
            if (!isLeaving)
            {
                HumanScore++;
                int rate2 = 90;
                if (FengGameManagerMKII.FGM.difficulty == 1)
                {
                    rate2 = 70;
                }
                FengGameManagerMKII.FGM.SpawnTitansCustom(rate2, 1, false);
            }
        }

        protected override void UpdateLabels()
        {
            Labels.Center = string.Empty;
            Labels.TopCenter = string.Empty;
            if (Round.IsWinning && Round.GameEndCD >= 0f)
            {
                if (Multiplayer)
                {
                    Labels.Center = Lang.Format("humanityWin", Round.GameEndCD.ToString("F0"));
                }
                else
                {
                    Labels.Center = Lang.Format("humanitySingleWin", Anarchy.InputManager.Settings[InputCode.Restart].ToString());
                }
            }
            else if (Round.IsLosing && Round.GameEndCD >= 0f)
            {
                if (Multiplayer)
                {
                    Labels.Center = Lang.Format("humanityFail", Round.GameEndCD.ToString("F0"));
                }
                else
                {
                    Labels.Center = Lang.Format("humanitySingleFail", Anarchy.InputManager.Settings[InputCode.Restart].ToString());
                }
            }
            Labels.TopCenter = Lang.Format("time", (IN_GAME_MAIN_CAMERA.GameType == GameType.Single ? (FengGameManagerMKII.FGM.logic.RoundTime).ToString("F0") : (FengGameManagerMKII.FGM.logic.ServerTime).ToString("F0")));
        }
    }
}