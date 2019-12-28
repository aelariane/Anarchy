using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace GameLogic
{
    internal class Round
    {
        public float GameEndCD { get; set; } = 10f;
        public float GameEndTimer { get; set; } = 10f;
        public bool GameStart { get; set; }
        public bool IsLosing { get; set; }
        public bool IsWinning { get; set; }
        public float Time { get; set; } = 0f;
        public int TeamWinner { get; set; }
        public int Wave { get; set; } = 1;

        public Round()
        {
        }

        public void OnLateUpdate()
        {
            float unscaled = UnityEngine.Time.deltaTime;
            Time += unscaled;
            if(IN_GAME_MAIN_CAMERA.GameType == GameType.Multi && (IsWinning || IsLosing))
            {
                GameEndCD -= UnityEngine.Time.deltaTime;
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
