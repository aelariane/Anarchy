namespace Anarchy.Commands.Chat
{
    internal class ChangeTeamCommand : ChatCommand
    {
        public ChangeTeamCommand() : base("team", false, true, false)
        {
        }

        public override bool Execute(string[] args)
        {
            if (!GameModes.TeamMode.Enabled || GameModes.TeamMode.Selection > 1)
            {
                chatMessage = Lang["errTeamsLocked"].HtmlColor("FF0000");
                return false;
            }
            if (args.Length <= 0)
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
                    chatMessage = Lang["errTeamsInvalid"].HtmlColor("FF0000");
                    return false;
            }
            FengGameManagerMKII.FGM.BasePV.RPC("setTeamRPC", PhotonNetwork.player, new object[] { team });
            chatMessage = Lang.Format("teamChanged", GetTeamName(team));
            if (PhotonNetwork.player.GameObject != null && PhotonNetwork.player.GameObject.GetComponent<HERO>() != null)
            {
                PhotonNetwork.player.GameObject.GetPhotonView().RPC("netDie2", PhotonTargets.All, new object[] { -1, "Team switch" });
            }
            return true;
        }

        private string GetTeamName(int team)
        {
            switch (team)
            {
                default:
                case 0:
                    return "<color=lime>" + Lang["teamIndividuals"] + "</color>";

                case 1:
                    return "<color=cyan>" + Lang["teamCyan"] + "</color>";

                case 2:
                    return "<color=magenta>" + Lang["teamMagenta"] + "</color>";
            }
        }
    }
}