using Anarchy;
using Optimization.Caching;
using UnityEngine;

public class BombExplode : Photon.MonoBehaviour
{
    private ParticleSystem myParticle;

    private void Awake()
    {
        if (myParticle == null)
        {
            myParticle = GetComponent<ParticleSystem>();
        }
    }

    public void OnEnable()
    {
        if (IN_GAME_MAIN_CAMERA.GameType == GameType.MultiPlayer)
        {
            if (BasePV != null && BasePV.owner != null)
            {
                PhotonPlayer owner = BasePV.owner;
                Color startColor = new Color(owner.RCBombR, owner.RCBombG, owner.RCBombB, Mathf.Max(0.5f, owner.RCBombA));
                if (GameModes.TeamMode.Enabled)
                {
                    switch (owner.RCteam)
                    {
                        case 1:
                            startColor = Colors.cyan;
                            break;

                        case 2:
                            startColor = Colors.magenta;
                            break;
                    }
                }
                myParticle.startColor = startColor;
                float num6 = AnarchyExtensions.GetFloat(owner.Properties["RCBombRadius"]) * 2f;
                num6 = Mathf.Clamp(num6, 40f, 120f);
                myParticle.startSize = num6;
            }
        }
    }
}