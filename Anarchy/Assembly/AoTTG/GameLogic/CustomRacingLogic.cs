using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ExitGames.Client.Photon;

namespace GameLogic
{
    internal class NonStopRacingOptions
    {
        public float MinimalSpeed = 150f;
        public float RecoverTimeSeconds = 10f;
    }

    internal class LimitedLifeOptions
    {
        public int LivesAmount = 1;
        public int RestartTimerSeconds = 5;
        public int AfkTimerSeconds = 20;
    }

    internal class RacingRules
    {
        public bool DifferentiateTime = false;
        public LimitedLifeOptions LifeModeOptions;
        public float RestartTimer = 20f;
        public float FinishersAmountRestart = 10;
        public float StartTimer = 20f;
        public bool DisableCheckPointsRespawn = false;
        public float MaximumSpeedLimit = 0;
        public NonStopRacingOptions NonStopOption;
    }

    internal class CustomRacingLogicProperties 
    {
        public const string LastRoundFinishers = "LastRoundFinishers";
        public const string CurrentRound = "CurrentRound";
        public const string MapName = "MapName";
        public const string MapAuthor = "MapAuthor";
        public const string RacingRules = "RacingRules";
        public const string ShadowName = "ShadowName";
        public const string ShadowTime = "ShadowTime";
    }

    internal class CustomRacingLogic : GameLogic
    {
        public CustomRacingLogic() : base()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                UpdateCurrentRound();
            }
        }

        public void SetMapName(string mapName)
        {
            PhotonNetwork.room.SetCustomProperties(new Hashtable()
            {
                { CustomRacingLogicProperties.MapName, mapName }
            });
        }

        public void UpdateCurrentRound()
        {
            PhotonNetwork.room.SetCustomProperties(new Hashtable() 
            {
                { CustomRacingLogicProperties.CurrentRound, CurrentRound }
            });
        }

        
    }
}
