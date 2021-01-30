using UnityEngine;

public class EnemyCheckCollider : Photon.MonoBehaviour
{
    private void OnEnable()
    {
        active_me = true;
        count = 0;
    }

    private void FixedUpdate()
    {
        if (this.count > 1)
        {
            this.active_me = false;
            return;
        }
        this.count++;
    }

    private void OnTriggerStay(Collider other)
    {
        if ((IN_GAME_MAIN_CAMERA.GameType == GameType.Single || base.transform.root.gameObject.GetPhotonView().IsMine) && this.active_me)
        {
            if (other.gameObject.tag == "playerHitbox")
            {
                float b = 1f - Vector3.Distance(other.gameObject.transform.position, base.transform.position) * 0.05f;
                b = Mathf.Min(1f, b);
                HitBox component = other.gameObject.GetComponent<HitBox>();
                if (component != null && component.transform.root != null)
                {
                    if (this.dmg == 0)
                    {
                        Vector3 vector = component.transform.root.transform.position - base.transform.position;
                        float num2 = 0f;
                        if (base.gameObject.GetComponent<SphereCollider>() != null)
                        {
                            num2 = base.transform.localScale.x * base.gameObject.GetComponent<SphereCollider>().radius;
                        }
                        if (base.gameObject.GetComponent<CapsuleCollider>() != null)
                        {
                            num2 = base.transform.localScale.x * base.gameObject.GetComponent<CapsuleCollider>().height;
                        }
                        float num3 = 5f;
                        if (num2 > 0f)
                        {
                            num3 = Mathf.Max(5f, num2 - vector.magnitude);
                        }
                        if (IN_GAME_MAIN_CAMERA.GameType == GameType.Single)
                        {
                            component.transform.root.GetComponent<HERO>().blowAway(vector.normalized * num3 + Vector3.up * 1f);
                            return;
                        }
                        if (IN_GAME_MAIN_CAMERA.GameType == GameType.MultiPlayer)
                        {
                            object[] parameters = new object[]
                            {
                                vector.normalized * num3 + Vector3.up * 1f
                            };
                            component.transform.root.GetComponent<HERO>().BasePV.RPC("blowAway", PhotonTargets.All, parameters);
                            return;
                        }
                    }
                    else if (!component.transform.root.GetComponent<HERO>().IsInvincible())
                    {
                        if (IN_GAME_MAIN_CAMERA.GameType == GameType.Single)
                        {
                            if (!component.transform.root.GetComponent<HERO>().IsGrabbed)
                            {
                                Vector3 vector2 = component.transform.root.transform.position - base.transform.position;
                                component.transform.root.GetComponent<HERO>().Die(vector2.normalized * b * 1000f + Vector3.up * 50f, this.isThisBite);
                                return;
                            }
                        }
                        else if (IN_GAME_MAIN_CAMERA.GameType == GameType.MultiPlayer && !component.transform.root.GetComponent<HERO>().HasDied() && !component.transform.root.GetComponent<HERO>().IsGrabbed)
                        {
                            component.transform.root.GetComponent<HERO>().MarkDie();
                            int myOwnerViewID = -1;
                            string titanName = string.Empty;
                            if (base.transform.root.gameObject.GetComponent<EnemyfxIDcontainer>() != null)
                            {
                                myOwnerViewID = base.transform.root.gameObject.GetComponent<EnemyfxIDcontainer>().myOwnerViewID;
                                titanName = base.transform.root.gameObject.GetComponent<EnemyfxIDcontainer>().titanName;
                            }
                            object[] objArray2 = new object[]
                            {
                                (component.transform.root.position - base.transform.position).normalized * b * 1000f + Vector3.up * 50f,
                                this.isThisBite,
                                myOwnerViewID,
                                titanName,
                                true
                            };
                            component.transform.root.GetComponent<HERO>().BasePV.RPC("netDie", PhotonTargets.All, objArray2);
                            return;
                        }
                    }
                }
            }
            else if (other.gameObject.tag == "erenHitbox" && this.dmg > 0 && !other.gameObject.transform.root.gameObject.GetComponent<TITAN_EREN>().isHit)
            {
                other.gameObject.transform.root.gameObject.GetComponent<TITAN_EREN>().hitByTitan();
            }
        }
    }

    private void Start()
    {
        this.active_me = true;
        this.count = 0;
    }

    public bool active_me;

    private int count;

    public int dmg = 1;

    public bool isThisBite;
}