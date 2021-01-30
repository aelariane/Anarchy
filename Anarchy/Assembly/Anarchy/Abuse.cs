using System.Linq;
using UnityEngine;

namespace Anarchy
{
    internal static class Abuse
    {
        public static bool Kill(PhotonPlayer player, string killer)
        {
            if(!PhotonNetwork.IsMasterClient || player == null || player.GameObject == null)
            {
                return false;
            }
            if (player.GameObject.GetComponent<HERO>() != null)
            {
                player.GameObject.GetComponent<HERO>().BasePV
                    .RPC("netDie2", PhotonTargets.All, new object[] { -1, killer });
                return true;
            }
            return false;
        }

    }
}
