using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Anarchy.Commands.Chat
{
    internal class ChangeTeamCommand : ChatCommand
    {
        public ChangeTeamCommand() : base("team", false, true, false)
        {
        }

        public override bool Execute(string[] args)
        {
            if(!GameModes.TeamMode.Enabled || GameModes.TeamMode.Selection > 1)
            {
                chatMessage = "Teams are locked or disabled";
                return false;
            }
            if(args.Length <= 0)
            {
                return false;
            }
            int team = 0;
            switch (args[0].ToLower())
            {
                case "0":
                case "individual":
                    break;

                case "1":
                case "cyan":
                    team = 1;
                    break;

                case "2":
                case "magenta":
                    team = 2;
                    break;

                default:
                    chatMessage = "Invalid team code or name (0,1,2 accepted)";
                    return false;
            }
            FengGameManagerMKII.FGM.BasePV.RPC("setTeamRPC", PhotonNetwork.player, new object[] { team });
            chatMessage = "Team changed to " + GetTeamName(team);
            if(PhotonNetwork.player.GameObject != null && PhotonNetwork.player.GameObject.GetComponent<HERO>() != null)
            {
                PhotonNetwork.player.GameObject.GetPhotonView().RPC("netDie2", PhotonTargets.All, new object[] { -1, "Team switch" });
            }
            return true;
        }

        private string GetTeamName(int team)
        {
            switch (team)
            {
                case 0:
                    return "individuals";

                case 1:
                    return "cyan";

                case 2:
                    return "magenta";

                default:
                    return $"unknown team {team}";
            }
        }
    }
}
