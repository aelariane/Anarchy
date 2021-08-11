using Anarchy;
using System.Text;

namespace Optimization
{
    internal class PlayerList
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
            bld.Append("[" + player.ModName + "] ");
            bld.Append(player.RCIgnored ? "[FF0000][IGNORED] [-]" : string.Empty);
            bld.Append(player.Muted ? "[FFFF00][MUTED] [-]" : string.Empty);
            bld.Append(player.IsMasterClient ? "[MC] " : "");
            bld.Append(player.Dead ? $"[{ColorSet.color_red}]*dead* " : "");
            bld.Append(player.IsTitan ? $"[{ColorSet.color_titan_player}][T] " : (player.Team == 2 ? $"[{ColorSet.color_human_1}][A] " : $"[{ColorSet.color_human}][H] "));
            bld.Append($"{(player.UIName.RemoveHTML())}[FFFFFF]: {player.Kills}/{player.Deaths}/{player.MaximumDamage}/{player.TotalDamage} ");
            //if (player.AbuseInformation.InfiniteGas)
            //{
            //    bld.Append("[CB0000][GAS][-] ");
            //}
            //if (player.AbuseInformation.InfiniteBlades)
            //{
            //    bld.Append("[CB0000][BLA][-] ");
            //}
            //if (player.AbuseInformation.InfiniteBullets)
            //{
            //    bld.Append("[CB0000][BUL][-] ");
            //}
            if (player.AbuseInformation.CharacterUnusualStats)
            {
                bld.Append("[CB0000][STATS][-] ");
            }

            if (player.Properties.ContainsKey(PhotonPlayerProperty.anarchyFlags))
            {
                int? anarchyFlag = player.Properties[PhotonPlayerProperty.anarchyFlags] as int?;
                if(anarchyFlag.HasValue && anarchyFlag.Value > 0)
                {
                    bld.Append("[CCCCDD][?][-] ");
                }
                int? abuseFlag = player.Properties[PhotonPlayerProperty.anarchyAbuseFlags] as int?;
                if (abuseFlag.HasValue && abuseFlag.Value > 0)
                {
                    bld.Append("[FF0000][A][-] ");
                }

            }
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
                int[] cyanStats = new int[4];
                int[] magentaStats = new int[4];
                //int[] individualStats = new int[4];
                for (int i = 0; i < PhotonNetwork.playerList.Length; i++)
                {
                    StringBuilder local = null;
                    PhotonPlayer player = PhotonNetwork.playerList[i];
                    int[] teamArray = null;
                    switch (PhotonNetwork.playerList[i].RCteam)
                    {
                        default:
                        case 0:
                            local = individual;
                            //teamArray = individualStats;
                            break;

                        case 1:
                            local = cyan;
                            teamArray = cyanStats;
                            break;

                        case 2:
                            local = magenta;
                            teamArray = magentaStats;
                            break;
                    }
                    if (teamArray != null)
                    {
                        teamArray[0] += player.Kills;
                        teamArray[1] += player.Deaths;
                        teamArray[2] += player.MaximumDamage;
                        teamArray[3] += player.TotalDamage;
                    }
                    local.AppendLine(PlayerToString(PhotonNetwork.playerList[i]));
                }
                bld.AppendLine($"\n<color=cyan>Team cyan</color> {GetStatString(cyanStats)}\n{cyan.ToString()}");
                bld.AppendLine($"<color=magenta>Team magenta</color> {GetStatString(magentaStats)}\n{magenta.ToString()}");
                bld.Append($"<color=lime>Individuals</color>\n{individual.ToString()}");
            }
            else
            {
                for (int i = 0; i < PhotonNetwork.playerList.Length; i++)
                {
                    if (i >= 25)
                    {
                        bld.Append("And " + (PhotonNetwork.playerList.Length - 25) + " players...");
                        break;
                    }
                    bld.AppendLine(PlayerToString(PhotonNetwork.playerList[i]));
                }
            }
            Labels.TopLeft = bld.ToString().ToHTMLFormat();
        }

        private string GetStatString(int[] stats)
        {
            return $"{stats[0]}/{stats[1]}/{stats[2]}/{stats[3]}";
        }
    }
}