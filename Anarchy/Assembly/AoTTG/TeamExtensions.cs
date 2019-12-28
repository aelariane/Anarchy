using ExitGames.Client.Photon;
using UnityEngine;

internal static class TeamExtensions
{
    public static PunTeams.Team GetTeam(this PhotonPlayer player)
    {
        object obj;
        if (player.Properties.TryGetValue("team", out obj))
        {
            return (PunTeams.Team)((byte)obj);
        }
        return PunTeams.Team.none;
    }

    public static void SetTeam(this PhotonPlayer player, PunTeams.Team team)
    {
        if (!PhotonNetwork.connectedAndReady)
        {
            Debug.LogWarning("JoinTeam was called in state: " + PhotonNetwork.connectionStateDetailed + ". Not connectedAndReady.");
        }
        PunTeams.Team team2 = PhotonNetwork.player.GetTeam();
        if (team2 != team)
        {
            PhotonNetwork.player.SetCustomProperties(new Hashtable
            {
                {
                    "team",
                    (byte)team
                }
            });
        }
    }
}