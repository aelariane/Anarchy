namespace GameLogic
{
    internal class TrostLogic : GameLogic
    {
        public TrostLogic() : base()
        {

        }

        public override void OnSomeOneIsDead(int id)
        {
            if (!Round.IsWinning && !Round.IsLosing)
            {
                FengGameManagerMKII.FGM.StartCoroutine(Anarchy.GameModes.CheckGameEnd());
            }
        }
    }
}
