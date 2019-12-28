using System;
using UnityEngine;

class RacingKillTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        HERO hero = other.gameObject.GetComponent<HERO>();
        if(hero != null && hero.IsLocal && !hero.HasDied())
        {
            hero.markDie();
            hero.BasePV.RPC("netDie2", PhotonTargets.All, new object[]
            {
                        -1,
                         Anarchy.User.RacingKillTrigger.PickRandomString() + " "
            });
        }
    }
}
