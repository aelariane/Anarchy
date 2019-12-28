using System;
using System.Collections.Generic;
using UnityEngine;

public class PunTeams : MonoBehaviour
{
    public const string TeamPlayerProp = "team";

    public static Dictionary<PunTeams.Team, List<PhotonPlayer>> PlayersPerTeam;

    public enum Team : byte
    {
        none,
        red,
        blue
    }

    public void OnJoinedRoom()
    {
        this.UpdateTeams();
    }

    public void OnPhotonPlayerPropertiesChanged(object[] playerAndUpdatedProps)
    {
        this.UpdateTeams();
    }

    public void Start()
    {
        PunTeams.PlayersPerTeam = new Dictionary<PunTeams.Team, List<PhotonPlayer>>();
        Array values = Enum.GetValues(typeof(PunTeams.Team));
        foreach (object obj in values)
        {
            PunTeams.PlayersPerTeam[(PunTeams.Team)((byte)obj)] = new List<PhotonPlayer>();
        }
    }

    public void UpdateTeams()
    {
        Array values = Enum.GetValues(typeof(PunTeams.Team));
        foreach (object obj in values)
        {
            PunTeams.PlayersPerTeam[(PunTeams.Team)((byte)obj)].Clear();
        }
        for (int i = 0; i < PhotonNetwork.playerList.Length; i++)
        {
            PhotonPlayer photonPlayer = PhotonNetwork.playerList[i];
            PunTeams.Team team = photonPlayer.GetTeam();
            PunTeams.PlayersPerTeam[team].Add(photonPlayer);
        }
    }
}