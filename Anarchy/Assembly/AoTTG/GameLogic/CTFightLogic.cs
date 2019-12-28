using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Optimization;

namespace GameLogic
{
    internal class CTFightLogic : GameLogic
    {
        public CTFightLogic() : base()
        {

        }

        public override void OnSomeOneIsDead(int id)
        {
            if (!Round.IsWinning && !Round.IsLosing)
            {
                FengGameManagerMKII.FGM.StartCoroutine(Anarchy.GameModes.CheckGameEnd());
            }
        }

        protected override void UpdateLabels()
        {
            Labels.Center = string.Empty;
            Labels.TopCenter = string.Empty;
            if (Round.IsWinning)
            {
                Labels.Center = Lang.Format("humanityWin", Round.GameEndCD.ToString("F0")) + "\n\n";
            }
            else if (Round.IsLosing)
            {
                Labels.Center = Lang.Format("humanityFail", Round.GameEndCD.ToString("F0")) + "\n\n";
            }
            Labels.TopCenter = Lang.Format("time", (IN_GAME_MAIN_CAMERA.GameType == GameType.Single ? (FengGameManagerMKII.FGM.Logic.RoundTime).ToString("F0") : (FengGameManagerMKII.FGM.Logic.ServerTime).ToString("F0")));
            Labels.TopCenter += "\n" + Lang["colossalInfo"];
        }
    }
}
