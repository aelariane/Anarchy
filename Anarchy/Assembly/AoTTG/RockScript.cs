using Optimization.Caching;
using UnityEngine;

public class RockScript : MonoBehaviour
{
    private Vector3 desPt = new Vector3(-200f, 0f, -280f);

    private bool disable;
    private float g = 500f;
    private float speed = 800f;
    private Vector3 vh;

    private Vector3 vv;

    private void Start()
    {
        base.transform.position = new Vector3(0f, 0f, 676f);
        this.vh = this.desPt - base.transform.position;
        this.vv = new Vector3(0f, this.g * this.vh.magnitude / (2f * this.speed), 0f);
        this.vh.Normalize();
        this.vh *= this.speed;
    }

    private void Update()
    {
        if (this.disable)
        {
            return;
        }
        this.vv += -Vectors.up * this.g * Time.deltaTime;
        base.transform.position += this.vv * Time.deltaTime;
        base.transform.position += this.vh * Time.deltaTime;
        if (Vector3.Distance(this.desPt, base.transform.position) < 20f || base.transform.position.y < 0f)
        {
            base.transform.position = this.desPt;
            if (IN_GAME_MAIN_CAMERA.GameType == GameType.Multi && PhotonNetwork.IsMasterClient)
            {
                if (FengGameManagerMKII.LAN)
                {
                    Network.Instantiate(CacheResources.Load("FX/boom1_CT_KICK"), base.transform.position + Vectors.up * 30f, Quaternion.Euler(270f, 0f, 0f), 0);
                }
                else
                {
                    Optimization.Caching.Pool.NetworkEnable("FX/boom1_CT_KICK", base.transform.position + Vectors.up * 30f, Quaternion.Euler(270f, 0f, 0f), 0);
                }
            }
            else
            {
                Pool.Enable("FX/boom1_CT_KICK", base.transform.position + Vectors.up * 30f, Quaternion.Euler(270f, 0f, 0f));//UnityEngine.Object.Instantiate(CacheResources.Load("FX/boom1_CT_KICK"), base.transform.position + Vectors.up * 30f, Quaternion.Euler(270f, 0f, 0f));
            }
            this.disable = true;
        }
    }
}