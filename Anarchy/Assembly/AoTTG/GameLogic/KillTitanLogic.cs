namespace GameLogic
{
    internal class KillTitanLogic : GameLogic
    {
        public KillTitanLogic() : base()
        {
        }

        public KillTitanLogic(GameLogic logic) : this()
        {
            CopyFrom(logic);
        }

        public override void OnSomeOneIsDead(int id)
        {
            if (!Round.IsWinning && !Round.IsLosing)
            {
                Anarchy.GameModes.CheckGameEnd();
            }
        }

        public override void OnTitanDown(string name, bool isLeaving)
        {
            if (!Round.IsWinning && !Round.IsLosing)
            {
                Anarchy.GameModes.CheckGameEnd();
            }
            if (CheckIsTitanAllDie())
            {
                GameWin();
                IN_GAME_MAIN_CAMERA.MainCamera.gameOver = true;
            }
        }

        protected override void UpdateLabels()
        {
            base.UpdateLabels();
            if (!Multiplayer)
            {
                Optimization.Labels.TopLeft = Lang.Format("singleState", FengGameManagerMKII.FGM.singleKills.ToString(), FengGameManagerMKII.FGM.singleMax.ToString(), FengGameManagerMKII.FGM.singleTotal.ToString());
            }
        }
    }
}