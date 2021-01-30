namespace GameLogic
{
    internal class Round
    {
        public float GameEndCD = 10f;
        public float GameEndTimer = 10f;
        public bool GameStart;
        public bool IsLosing;
        public bool IsWinning;
        public float Time = 0f;
        public int TeamWinner;
        public int Wave = 1;

        public Round()
        {
        }

        public void OnLateUpdate()
        {
            float time = UnityEngine.Time.deltaTime;
            if (IN_GAME_MAIN_CAMERA.GameType == GameType.Single)
            {
                if (IsWinning || IsLosing)
                {
                    return;
                }
                Time += time;
            }
            else
            {
                Time += time;
                if (IsWinning || IsLosing)
                {
                    GameEndCD -= time;
                }
            }
        }

        public void Reset()
        {
            Wave = 1;
            Time = 0f;
            IsWinning = false;
            IsLosing = false;
            GameEndCD = GameEndTimer;
        }
    }
}