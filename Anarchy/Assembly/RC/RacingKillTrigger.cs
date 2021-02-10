using UnityEngine;

internal class RacingKillTrigger : MonoBehaviour
{
    public bool Disabled { get; set; } = false;

    private void OnTriggerEnter(Collider other)
    {
        HERO hero = other.gameObject.GetComponent<HERO>();
        if (hero != null && hero.IsLocal && !hero.IsDead)
        {
            if (Disabled)
            {
                return;
            }
            hero.MarkDie();
            hero.BasePV.RPC("netDie2", PhotonTargets.All, new object[]
            {
                        -1,
                         Anarchy.User.RacingKillTrigger.PickRandomString() + " "
            });
        }
    }
}