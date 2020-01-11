using Anarchy;
using System.Text;

namespace Optimization
{
    class PlayerList
    {
        public PlayerList()
        {
            Update();
        }

        private string PlayerToString(PhotonPlayer player)
        {
            var bld = new StringBuilder();
            bld.Append($"[FFFFFF]# {player.ID} ");
            bld.Append(player.IsLocal ? "[FFCC00]>>[-] " : "");
            bld.Append(player.Anarchy ? "[00BBCC][A][-] ": (player.RCSync ? "[9999FF][RC][-] " : "[CCCCDD][V][-] "));
            bld.Append(player.RCIgnored ? "[FF0000][IGNORED] [-]" : string.Empty);
            bld.Append(player.IsMasterClient ? "[MC] " : "");
            bld.Append(player.Dead ? $"[{ColorSet.color_red}]*dead* " : "");
            bld.Append(player.IsTitan ? $"[{ColorSet.color_titan_player}][T] " : (player.Team == 2 ? $"[{ColorSet.color_human_1}][A] " : $"[{ColorSet.color_human}][H] "));
            bld.Append($"{player.UIName}[FFFFFF]: {player.Kills}/{player.Deaths}/{player.Max_Dmg}/{player.Total_Dmg}");
            return bld.ToString();
        }

        public void Update()
        {
            if (!PhotonNetwork.inRoom)
            {
                return;
            }
            var bld = new StringBuilder();
            if (GameModes.TeamMode.Enabled && !GameModes.InfectionMode.Enabled)
            {
                var individual = new StringBuilder();
                var cyan = new StringBuilder();
                var magenta = new StringBuilder();
                for (int i = 0; i < PhotonNetwork.playerList.Length; i++)
                {
                    StringBuilder local = null;
                    switch (PhotonNetwork.playerList[i].RCteam)
                    {
                        default:
                        case 0:
                            local = individual;
                            break;

                        case 1:
                            local = cyan;
                            break;

                        case 2:
                            local = magenta;
                            break;
                    }
                    local.AppendLine(PlayerToString(PhotonNetwork.playerList[i]));
                }
                bld.Append($"Individual\n{individual.ToString()}Team cyan\n{cyan.ToString()}Team magenta\n{magenta.ToString()}");
            }
            else
            {
                for (int i = 0; i < PhotonNetwork.playerList.Length; i++)
                {
                    bld.AppendLine(PlayerToString(PhotonNetwork.playerList[i]));
                }
            }
            Labels.TopLeft = bld.ToString().ToHTMLFormat();
        }

    }
}
