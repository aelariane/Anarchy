using Anarchy.Configuration;
using Optimization.Caching;
using System.Collections;
using UnityEngine;

public sealed class AHSSShotGunCollider : MonoBehaviour
{
    private Transform baseT;
    private int count;
    private bool isLocal;
    private int myTeam = 1;
    private string ownerName = string.Empty;
    private int viewID = -1;
    public bool active_me;

    public GameObject currentCamera;
    public ArrayList currentHits = new ArrayList();
    public int dmg = 1;

    public float scoreMulti;

    private void Awake()
    {
        baseT = transform;
        isLocal = IN_GAME_MAIN_CAMERA.GameType == GameType.Single || baseT.root.gameObject.GetPhotonView().IsMine;
    }

    private bool checkIfBehind(GameObject titan)
    {
        Transform transform = titan.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/neck/head");
        Vector3 to = baseT.position - transform.transform.position;
        return Vector3.Angle(-transform.transform.Forward(), to) < 70f;
    }

    private void FixedUpdate()
    {
        if (this.count > 1)
        {
            this.active_me = false;
        }
        else
        {
            this.count++;
        }
    }

    private void OnEnable()
    {
        count = 0;
        dmg = 0;
        active_me = true;
        isLocal = IN_GAME_MAIN_CAMERA.GameType == GameType.Single || baseT.root.gameObject.GetPhotonView().IsMine;
    }

    private void OnTriggerStay(Collider other)
    {
        if (!isLocal)
            return;
        if (this.active_me)
        {
            switch (other.gameObject.tag)
            {
                case "playerHitbox":
                    if (!FengGameManagerMKII.Level.PVPEnabled)
                    {
                        return;
                    }
                    float num = 1f - Vector3.Distance(other.gameObject.transform.position, baseT.position) * 0.05f;
                    num = Mathf.Min(1f, num);
                    HitBox component = other.gameObject.GetComponent<HitBox>();
                    if (component != null && component.transform.root != null)
                    {
                        if (component.transform.root.GetComponent<HERO>().myTeam == this.myTeam)
                        {
                            return;
                        }
                        if (!component.transform.root.GetComponent<HERO>().IsInvincible())
                        {
                            if (IN_GAME_MAIN_CAMERA.GameType == GameType.Single)
                            {
                                if (!component.transform.root.GetComponent<HERO>().IsGrabbed)
                                {
                                    component.transform.root.GetComponent<HERO>().Die((component.transform.root.transform.position - baseT.position).normalized * num * 1000f + Vectors.up * 50f, false);
                                }
                            }
                            else if (IN_GAME_MAIN_CAMERA.GameType == GameType.MultiPlayer && !component.transform.root.GetComponent<HERO>().HasDied() && !component.transform.root.GetComponent<HERO>().IsGrabbed)
                            {
                                component.transform.root.GetComponent<HERO>().MarkDie();
                                component.transform.root.GetComponent<HERO>().BasePV.RPC("netDie", PhotonTargets.All, new object[]
                                {
                                (component.transform.root.position - baseT.position).normalized * num * 1000f + Vectors.up * 50f,
                                false,
                                this.viewID,
                                this.ownerName,
                                false
                                });
                            }
                        }
                    }
                    break;

                case "titanneck":
                    HitBox component2 = other.gameObject.GetComponent<HitBox>();
                    if (component2 != null && this.checkIfBehind(component2.transform.root.gameObject) && !this.currentHits.Contains(component2))
                    {
                        component2.hitPosition = (baseT.position + component2.transform.position) * 0.5f;
                        this.currentHits.Add(component2);
                        if (IN_GAME_MAIN_CAMERA.GameType == GameType.Single)
                        {
                            if (component2.transform.root.GetComponent<TITAN>() && !component2.transform.root.GetComponent<TITAN>().hasDie)
                            {
                                int num2 = (int)((IN_GAME_MAIN_CAMERA.MainR.velocity - component2.transform.root.rigidbody.velocity).magnitude * 10f * this.scoreMulti);
                                num2 = Mathf.Max(10, num2);
                                FengGameManagerMKII.FGM.netShowDamage(num2);
                                if ((float)num2 > component2.transform.root.GetComponent<TITAN>().myLevel * 100f)
                                {
                                    component2.transform.root.GetComponent<TITAN>().Die();
                                    if (Settings.Snapshots.ToValue() && Settings.SnapshotsDamage.Value <= num2)
                                    {
                                        IN_GAME_MAIN_CAMERA.MainCamera.startSnapShot(component2.transform.position, num2, component2.transform.root.gameObject, 0.02f);
                                    }
                                    FengGameManagerMKII.FGM.PlayerKillInfoSingleUpdate(num2);
                                }
                            }
                        }
                        else if (!PhotonNetwork.IsMasterClient || !component2.transform.root.GetComponent<PhotonView>().BasePV.IsMine)
                        {
                            if (component2.transform.root.GetComponent<TITAN>())
                            {
                                if (!component2.transform.root.GetComponent<TITAN>().hasDie)
                                {
                                    int num3 = (int)((IN_GAME_MAIN_CAMERA.MainR.velocity - component2.transform.root.rigidbody.velocity).magnitude * 10f * this.scoreMulti);
                                    num3 = Mathf.Max(10, num3);
                                    if ((float)num3 > component2.transform.root.GetComponent<TITAN>().myLevel * 100f)
                                    {
                                        if (Settings.Snapshots.ToValue() && Settings.SnapshotsDamage.Value <= num3)
                                        {
                                            IN_GAME_MAIN_CAMERA.MainCamera.startSnapShot(component2.transform.position, num3, component2.transform.root.gameObject, 0.02f);
                                            component2.transform.root.GetComponent<TITAN>().asClientLookTarget = false;
                                        }
                                        component2.transform.root.GetComponent<TITAN>().BasePV.RPC("titanGetHit", component2.transform.root.GetComponent<TITAN>().BasePV.owner, new object[]
                                        {
                                        baseT.root.gameObject.GetPhotonView().viewID,
                                        num3
                                        });
                                    }
                                }
                            }
                            else if (component2.transform.root.GetComponent<FEMALE_TITAN>())
                            {
                                int num4 = (int)((IN_GAME_MAIN_CAMERA.MainR.velocity - component2.transform.root.rigidbody.velocity).magnitude * 10f * this.scoreMulti);
                                num4 = Mathf.Max(10, num4);
                                if (!component2.transform.root.GetComponent<FEMALE_TITAN>().hasDie)
                                {
                                    component2.transform.root.GetComponent<FEMALE_TITAN>().BasePV.RPC("titanGetHit", component2.transform.root.GetComponent<FEMALE_TITAN>().BasePV.owner, new object[]
                                    {
                                    baseT.root.gameObject.GetPhotonView().viewID,
                                    num4
                                    });
                                }
                            }
                            else if (component2.transform.root.GetComponent<COLOSSAL_TITAN>() && !component2.transform.root.GetComponent<COLOSSAL_TITAN>().hasDie)
                            {
                                int num5 = (int)((IN_GAME_MAIN_CAMERA.MainR.velocity - component2.transform.root.rigidbody.velocity).magnitude * 10f * this.scoreMulti);
                                num5 = Mathf.Max(10, num5);
                                component2.transform.root.GetComponent<COLOSSAL_TITAN>().BasePV.RPC("titanGetHit", component2.transform.root.GetComponent<COLOSSAL_TITAN>().BasePV.owner, new object[]
                                {
                                baseT.root.gameObject.GetPhotonView().viewID,
                                num5
                                });
                            }
                        }
                        else if (component2.transform.root.GetComponent<TITAN>())
                        {
                            if (!component2.transform.root.GetComponent<TITAN>().hasDie)
                            {
                                int num6 = (int)((IN_GAME_MAIN_CAMERA.MainR.velocity - component2.transform.root.rigidbody.velocity).magnitude * 10f * this.scoreMulti);
                                num6 = Mathf.Max(10, num6);
                                if ((float)num6 > component2.transform.root.GetComponent<TITAN>().myLevel * 100f)
                                {
                                    if (Settings.Snapshots.ToValue() && Settings.SnapshotsDamage.Value <= num6)
                                    {
                                        IN_GAME_MAIN_CAMERA.MainCamera.startSnapShot(component2.transform.position, num6, component2.transform.root.gameObject, 0.02f);
                                    }
                                    component2.transform.root.GetComponent<TITAN>().titanGetHit(baseT.root.gameObject.GetPhotonView().viewID, num6);
                                }
                            }
                        }
                        else if (component2.transform.root.GetComponent<FEMALE_TITAN>())
                        {
                            if (!component2.transform.root.GetComponent<FEMALE_TITAN>().hasDie)
                            {
                                int num7 = (int)((IN_GAME_MAIN_CAMERA.MainR.velocity - component2.transform.root.rigidbody.velocity).magnitude * 10f * this.scoreMulti);
                                num7 = Mathf.Max(10, num7);
                                if (Settings.Snapshots.ToValue() && Settings.SnapshotsDamage.Value <= num7)
                                {
                                    IN_GAME_MAIN_CAMERA.MainCamera.startSnapShot(component2.transform.position, num7, null, 0.02f);
                                }
                                component2.transform.root.GetComponent<FEMALE_TITAN>().titanGetHit(baseT.root.gameObject.GetPhotonView().viewID, num7);
                            }
                        }
                        else if (component2.transform.root.GetComponent<COLOSSAL_TITAN>() && !component2.transform.root.GetComponent<COLOSSAL_TITAN>().hasDie)
                        {
                            int num8 = (int)((IN_GAME_MAIN_CAMERA.MainR.velocity - component2.transform.root.rigidbody.velocity).magnitude * 10f * this.scoreMulti);
                            num8 = Mathf.Max(10, num8);
                            if (Settings.Snapshots.ToValue() && Settings.SnapshotsDamage.Value <= num8)
                            {
                                IN_GAME_MAIN_CAMERA.MainCamera.startSnapShot(component2.transform.position, num8, null, 0.02f);
                            }
                            component2.transform.root.GetComponent<COLOSSAL_TITAN>().titanGetHit(baseT.root.gameObject.GetPhotonView().viewID, num8);
                        }
                        this.showCriticalHitFX(other.gameObject.transform.position);
                    }
                    break;

                case "erenHitbox":
                    if (this.dmg > 0 && !other.gameObject.transform.root.gameObject.GetComponent<TITAN_EREN>().isHit)
                    {
                        other.gameObject.transform.root.gameObject.GetComponent<TITAN_EREN>().hitByTitan();
                    }
                    break;

                case "titaneye":
                    if (!this.currentHits.Contains(other.gameObject))
                    {
                        this.currentHits.Add(other.gameObject);
                        GameObject gameObject = other.gameObject.transform.root.gameObject;
                        if (gameObject.GetComponent<FEMALE_TITAN>())
                        {
                            if (IN_GAME_MAIN_CAMERA.GameType == GameType.Single)
                            {
                                if (!gameObject.GetComponent<FEMALE_TITAN>().hasDie)
                                {
                                    gameObject.GetComponent<FEMALE_TITAN>().hitEye();
                                }
                            }
                            else if (!PhotonNetwork.IsMasterClient || !gameObject.GetPhotonView().IsMine)
                            {
                                if (!gameObject.GetComponent<FEMALE_TITAN>().hasDie)
                                {
                                    gameObject.GetComponent<FEMALE_TITAN>().BasePV.RPC("hitEyeRPC", PhotonTargets.MasterClient, new object[]
                                    {
                                    baseT.root.gameObject.GetPhotonView().viewID
                                    });
                                }
                            }
                            else if (!gameObject.GetComponent<FEMALE_TITAN>().hasDie)
                            {
                                gameObject.GetComponent<FEMALE_TITAN>().hitEyeRPC(baseT.root.gameObject.GetPhotonView().viewID);
                            }
                        }
                        else if (gameObject.GetComponent<TITAN>().abnormalType != AbnormalType.Crawler)
                        {
                            if (IN_GAME_MAIN_CAMERA.GameType == GameType.Single)
                            {
                                if (!gameObject.GetComponent<TITAN>().hasDie)
                                {
                                    gameObject.GetComponent<TITAN>().HitEye();
                                }
                            }
                            else if (!PhotonNetwork.IsMasterClient || !gameObject.GetPhotonView().IsMine)
                            {
                                if (!gameObject.GetComponent<TITAN>().hasDie)
                                {
                                    gameObject.GetComponent<TITAN>().BasePV.RPC("hitEyeRPC", PhotonTargets.MasterClient, new object[]
                                    {
                                    baseT.root.gameObject.GetPhotonView().viewID
                                    });
                                }
                            }
                            else if (!gameObject.GetComponent<TITAN>().hasDie)
                            {
                                gameObject.GetComponent<TITAN>().hitEyeRPC(baseT.root.gameObject.GetPhotonView().viewID);
                            }
                            this.showCriticalHitFX(other.gameObject.transform.position);
                        }
                    }
                    break;

                case "titanankle":
                    if (currentHits.Contains(other.gameObject)) return;
                    this.currentHits.Add(other.gameObject);
                    GameObject gameObject2 = other.gameObject.transform.root.gameObject;
                    int num9 = (int)((IN_GAME_MAIN_CAMERA.MainR.velocity - gameObject2.rigidbody.velocity).magnitude * 10f * this.scoreMulti);
                    num9 = Mathf.Max(10, num9);
                    if (gameObject2.GetComponent<TITAN>() && gameObject2.GetComponent<TITAN>().abnormalType != AbnormalType.Crawler)
                    {
                        if (IN_GAME_MAIN_CAMERA.GameType == GameType.Single)
                        {
                            if (!gameObject2.GetComponent<TITAN>().hasDie)
                            {
                                gameObject2.GetComponent<TITAN>().HitAnkle();
                            }
                        }
                        else
                        {
                            if (!PhotonNetwork.IsMasterClient || !gameObject2.GetPhotonView().IsMine)
                            {
                                if (!gameObject2.GetComponent<TITAN>().hasDie)
                                {
                                    gameObject2.GetComponent<TITAN>().BasePV.RPC("hitAnkleRPC", PhotonTargets.MasterClient, new object[]
                                    {
                                    baseT.root.gameObject.GetPhotonView().viewID
                                    });
                                }
                            }
                            else if (!gameObject2.GetComponent<TITAN>().hasDie)
                            {
                                gameObject2.GetComponent<TITAN>().HitAnkle();
                            }
                            this.showCriticalHitFX(other.gameObject.transform.position);
                        }
                    }
                    else if (gameObject2.GetComponent<FEMALE_TITAN>())
                    {
                        if (IN_GAME_MAIN_CAMERA.GameType == GameType.Single)
                        {
                            if (other.gameObject.name == "ankleR")
                            {
                                if (gameObject2.GetComponent<FEMALE_TITAN>() && !gameObject2.GetComponent<FEMALE_TITAN>().hasDie)
                                {
                                    gameObject2.GetComponent<FEMALE_TITAN>().hitAnkleR(num9);
                                }
                            }
                            else if (gameObject2.GetComponent<FEMALE_TITAN>() && !gameObject2.GetComponent<FEMALE_TITAN>().hasDie)
                            {
                                gameObject2.GetComponent<FEMALE_TITAN>().hitAnkleL(num9);
                            }
                        }
                        else if (other.gameObject.name == "ankleR")
                        {
                            if (!PhotonNetwork.IsMasterClient)
                            {
                                if (!gameObject2.GetComponent<FEMALE_TITAN>().hasDie)
                                {
                                    gameObject2.GetComponent<FEMALE_TITAN>().BasePV.RPC("hitAnkleRRPC", PhotonTargets.MasterClient, new object[]
                                    {
                                    baseT.root.gameObject.GetPhotonView().viewID,
                                    num9
                                    });
                                }
                            }
                            else if (!gameObject2.GetComponent<FEMALE_TITAN>().hasDie)
                            {
                                gameObject2.GetComponent<FEMALE_TITAN>().hitAnkleRRPC(baseT.root.gameObject.GetPhotonView().viewID, num9);
                            }
                        }
                        else if (!PhotonNetwork.IsMasterClient)
                        {
                            if (!gameObject2.GetComponent<FEMALE_TITAN>().hasDie)
                            {
                                gameObject2.GetComponent<FEMALE_TITAN>().BasePV.RPC("hitAnkleLRPC", PhotonTargets.MasterClient, new object[]
                                {
                                baseT.root.gameObject.GetPhotonView().viewID,
                                num9
                                });
                            }
                        }
                        else if (!gameObject2.GetComponent<FEMALE_TITAN>().hasDie)
                        {
                            gameObject2.GetComponent<FEMALE_TITAN>().hitAnkleLRPC(baseT.root.gameObject.GetPhotonView().viewID, num9);
                        }
                        this.showCriticalHitFX(other.gameObject.transform.position);
                    }
                    break;
            }
        }
    }

    private void showCriticalHitFX(Vector3 position)
    {
        IN_GAME_MAIN_CAMERA.MainCamera.startShake(0.2f, 0.3f, 0.95f);
        if (IN_GAME_MAIN_CAMERA.GameType != GameType.Single)
        {
            Optimization.Caching.Pool.NetworkEnable("redCross1", position, Quaternion.Euler(270f, 0f, 0f), 0);
        }
        else
        {
            Pool.Enable("redCross1", position, Quaternion.Euler(270f, 0f, 0f));//gameObject = (GameObject)UnityEngine.Object.Instantiate(CacheResources.Load("redCross1"));
        }
        //gameObject.transform.position = position;
    }

    private void Start()
    {
        if (IN_GAME_MAIN_CAMERA.GameType == GameType.MultiPlayer)
        {
            if (!baseT.root.gameObject.GetPhotonView().IsMine)
            {
                base.enabled = false;
                return;
            }
            if (baseT.root.gameObject.GetComponent<EnemyfxIDcontainer>() != null)
            {
                this.viewID = baseT.root.gameObject.GetComponent<EnemyfxIDcontainer>().myOwnerViewID;
                this.ownerName = baseT.root.gameObject.GetComponent<EnemyfxIDcontainer>().titanName;
                this.myTeam = PhotonView.Find(this.viewID).gameObject.GetComponent<HERO>().myTeam;
            }
        }
        else
        {
            this.myTeam = IN_GAME_MAIN_CAMERA.MainHERO.myTeam;
        }
        this.active_me = true;
        this.count = 0;
    }
}