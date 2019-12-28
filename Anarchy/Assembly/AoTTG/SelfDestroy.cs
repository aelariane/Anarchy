using Optimization.Caching;
using UnityEngine;

public class SelfDestroy : Photon.MonoBehaviour
{
    public float CountDown = 5f;

    private void OnDisable()
    {

    }

    private void OnEnable()
    {
        CountDown = 5f;
    }

    private void Start()
    {
    }

    private void Update()
    {
        this.CountDown -= Time.deltaTime;
        if (this.CountDown <= 0f)
        {
            if (IN_GAME_MAIN_CAMERA.GameType == GameType.Single)
            {
                Pool.Disable(gameObject);
                return;
            }
            else if (IN_GAME_MAIN_CAMERA.GameType == GameType.Multi)
            {
                if (BasePV != null)
                {
                    if (BasePV.viewID == 0)
                    {
                        Pool.Disable(base.gameObject);
                    }
                    else if (BasePV.IsMine)
                    {
                        PhotonNetwork.Destroy(base.gameObject);
                    }
                }
                else
                {
                    Pool.Disable(base.gameObject);
                }
            }
        }
    }
}