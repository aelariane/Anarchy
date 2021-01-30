using Anarchy;
using Optimization;
using Optimization.Caching;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonBall : Photon.MonoBehaviour
{
    private Vector3 correctPos;

    private Vector3 correctVelocity;

    public bool disabled;

    public Transform firingPoint;

    public bool isCollider;

    public HERO myHero;

    public List<TitanTrigger> myTitanTriggers;

    public float SmoothingDelay = 10f;

    private void Awake()
    {
        bool flag = BasePV != null;
        if (flag)
        {
            BasePV.observed = this;
            this.correctPos = base.transform.position;
            this.correctVelocity = Vector3.zero;
            base.GetComponent<SphereCollider>().enabled = false;
            if (BasePV.IsMine)
            {
                base.StartCoroutine(this.WaitAndDestroy(10f));
                this.myTitanTriggers = new List<TitanTrigger>();
            }
        }
    }

    public void destroyMe()
    {
        bool flag = !this.disabled;
        if (flag)
        {
            this.disabled = true;
            GameObject gameObject = Pool.NetworkEnable("FX/boom4", base.transform.position, base.transform.rotation, 0);
            foreach (EnemyCheckCollider enemyCheckCollider in gameObject.GetComponentsInChildren<EnemyCheckCollider>())
            {
                enemyCheckCollider.dmg = 0;
            }
            bool flag2 = GameModes.CannonsKillHumans.Enabled;
            if (flag2)
            {
                foreach (HERO hero in FengGameManagerMKII.Heroes)
                {
                    bool flag3 = hero != null && Vector3.Distance(hero.transform.position, base.transform.position) <= 20f && !hero.BasePV.IsMine;
                    if (flag3)
                    {
                        GameObject gameObject2 = hero.gameObject;
                        PhotonPlayer owner = gameObject2.GetPhotonView().owner;

                        bool flag4 = GameModes.TeamMode.Enabled;
                        if (flag4)
                        {
                            int num = PhotonNetwork.player.RCteam;
                            int num2 = owner.RCteam; ;
                            bool flag5 = num == 0 || num != num2;
                            if (flag5)
                            {
                                gameObject2.GetComponent<HERO>().MarkDie();
                                gameObject2.GetComponent<HERO>().BasePV.RPC("netDie2", PhotonTargets.All, new object[]
                                {
                                    -1,
                                    PhotonNetwork.player.UIName + " "
                                });
                                FengGameManagerMKII.FGM.PlayerKillInfoUpdate(PhotonNetwork.player, 0);
                            }
                        }
                        else
                        {
                            gameObject2.GetComponent<HERO>().MarkDie();
                            gameObject2.GetComponent<HERO>().BasePV.RPC("netDie2", PhotonTargets.All, new object[]
                            {
                                -1,
                               PhotonNetwork.player.UIName + " "
                            });
                            FengGameManagerMKII.FGM.PlayerKillInfoUpdate(PhotonNetwork.player, 0);
                        }
                    }
                }
            }
            bool flag6 = this.myTitanTriggers != null;
            if (flag6)
            {
                for (int j = 0; j < this.myTitanTriggers.Count; j++)
                {
                    bool flag7 = this.myTitanTriggers[j] != null;
                    if (flag7)
                    {
                        this.myTitanTriggers[j].IsCollide = false;
                    }
                }
            }
            PhotonNetwork.Destroy(base.gameObject);
        }
    }

    public void FixedUpdate()
    {
        if (BasePV.IsMine && !disabled)
        {
            LayerMask mask = Layers.PlayerAttackBox | Layers.EnemyBox;
            if (!isCollider)
            {
                mask |= Layers.Ground;
            }
            Collider[] array = Physics.OverlapSphere(base.transform.position, 0.6f, mask.value);
            bool flag3 = false;
            for (int i = 0; i < array.Length; i++)
            {
                GameObject gameObject = array[i].gameObject;
                bool flag4 = gameObject.layer == 16;
                if (flag4)
                {
                    TitanTrigger component = gameObject.GetComponent<TitanTrigger>();
                    bool flag5 = !(component == null) && !this.myTitanTriggers.Contains(component);
                    if (flag5)
                    {
                        component.IsCollide = true;
                        this.myTitanTriggers.Add(component);
                    }
                }
                else
                {
                    bool flag6 = gameObject.layer == 10;
                    if (flag6)
                    {
                        TITAN component2 = gameObject.transform.root.gameObject.GetComponent<TITAN>();
                        bool flag7 = component2 != null;
                        if (flag7)
                        {
                            bool flag8 = component2.abnormalType == AbnormalType.Crawler;
                            if (flag8)
                            {
                                bool flag9 = gameObject.name == "head";
                                if (flag9)
                                {
                                    component2.BasePV.RPC("DieByCannon", component2.BasePV.owner, new object[]
                                    {
                                        this.myHero.BasePV.viewID
                                    });
                                    component2.DieBlow(base.transform.position, 0.2f);
                                    i = array.Length;
                                }
                            }
                            else
                            {
                                bool flag10 = gameObject.name == "head";
                                if (flag10)
                                {
                                    component2.BasePV.RPC("DieByCannon", component2.BasePV.owner, new object[]
                                    {
                                        this.myHero.BasePV.viewID
                                    });
                                    component2.DieHeadBlow(base.transform.position, 0.2f);
                                    i = array.Length;
                                }
                                else
                                {
                                    bool flag11 = UnityEngine.Random.Range(0f, 1f) < 0.5f;
                                    if (flag11)
                                    {
                                        component2.HitL(base.transform.position, 0.05f);
                                    }
                                    else
                                    {
                                        component2.HitR(base.transform.position, 0.05f);
                                    }
                                }
                            }
                            this.destroyMe();
                        }
                    }
                    else
                    {
                        bool flag12 = gameObject.layer == 9 && (gameObject.transform.root.name.Contains("CannonWall") || gameObject.transform.root.name.Contains("CannonGround"));
                        if (flag12)
                        {
                            flag3 = true;
                        }
                    }
                }
            }
            bool flag13 = !this.isCollider && !flag3;
            if (flag13)
            {
                this.isCollider = true;
                base.GetComponent<SphereCollider>().enabled = true;
            }
        }
    }

    public void OnCollisionEnter(Collision myCollision)
    {
        bool isMine = BasePV.IsMine;
        if (isMine)
        {
            this.destroyMe();
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        bool isWriting = stream.isWriting;
        if (isWriting)
        {
            stream.SendNext(base.transform.position);
            stream.SendNext(base.rigidbody.velocity);
        }
        else
        {
            this.correctPos = (Vector3)stream.ReceiveNext();
            this.correctVelocity = (Vector3)stream.ReceiveNext();
        }
    }

    public void Update()
    {
        if (!BasePV.IsMine)
        {
            base.transform.position = Vector3.Lerp(base.transform.position, this.correctPos, Time.deltaTime * this.SmoothingDelay);
            base.rigidbody.velocity = this.correctVelocity;
        }
    }

    public IEnumerator WaitAndDestroy(float time)
    {
        yield return new WaitForSeconds(time);
        this.destroyMe();
        yield break;
    }
}