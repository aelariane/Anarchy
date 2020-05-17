using Anarchy.Configuration;
using Optimization.Caching;
using System.Collections;
using UnityEngine;

public class TriggerColliderWeapon : MonoBehaviour
{

    private bool active_me;
    private Transform baseT;
    private GameObject baseG;
    public ArrayList currentHits = new ArrayList();
    public ArrayList currentHitsII = new ArrayList();
    public AudioSource meatDie;
    public int myTeam = 1;
    public float scoreMulti = 1f;

    public bool Active
    {
        get
        {
            return active_me;
        }
        set
        {
            baseG.SetActive(value);
            active_me = value;
        }
    }

    private void Awake()
    {
        baseT = transform;
        baseG = base.gameObject;
        active_me = baseG.activeInHierarchy;
    }

    private bool checkIfBehind(GameObject titan)
    {
        Transform transform = titan.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/neck/head");
        Vector3 to = baseT.position - transform.transform.position;
        return Vector3.Angle(-transform.transform.Forward(), to) < 70f;
    }

    private void napeMeat(Vector3 vkill, Transform titan)
    {
        Transform transform = titan.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/neck");
        GameObject gameObject = Pool.Enable("titanNapeMeat", transform.position, transform.rotation);//(GameObject)UnityEngine.Object.Instantiate(CacheResources.Load("titanNapeMeat"), transform.position, transform.rotation);
        gameObject.transform.localScale = titan.localScale;
        gameObject.rigidbody.AddForce(vkill.normalized * 15f, ForceMode.Impulse);
        gameObject.rigidbody.AddForce(-titan.forward * 10f, ForceMode.Impulse);
        gameObject.rigidbody.AddTorque(new Vector3((float)UnityEngine.Random.Range(-100, 100), (float)UnityEngine.Random.Range(-100, 100), (float)UnityEngine.Random.Range(-100, 100)), ForceMode.Impulse);
    }

    private void OnTriggerStay(Collider other)
    {
        //if (this.ActiveMe)
        //{
        if (!this.currentHitsII.Contains(other.gameObject))
        {
            this.currentHitsII.Add(other.gameObject);
            IN_GAME_MAIN_CAMERA.MainCamera.startShake(0.1f, 0.1f, 0.95f);
            if (other.gameObject.transform.root.gameObject.CompareTag("titan"))
            {
                IN_GAME_MAIN_CAMERA.MainHERO.slashHit.Play();
                if (IN_GAME_MAIN_CAMERA.GameType != GameType.Single)
                {
                    Optimization.Caching.Pool.NetworkEnable("hitMeat", baseT.position, Quaternion.Euler(270f, 0f, 0f), 0);
                }
                else
                {
                    Pool.Enable("hitMeat", baseT.position, Quaternion.Euler(270f, 0f, 0f));//(GameObject)UnityEngine.Object.Instantiate(CacheResources.Load("hitMeat"));
                }
                //gameObject.transform.position = baseT.position;
                baseT.root.GetComponent<HERO>().useBlade(0);
            }
        }
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
                                baseT.root.gameObject.GetPhotonView().viewID,
                                PhotonView.Find(baseT.root.gameObject.GetPhotonView().viewID).owner.Properties[PhotonPlayerProperty.name],
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
                    this.meatDie.Play();
                    if (IN_GAME_MAIN_CAMERA.GameType == GameType.Single)
                    {
                        if (component2.transform.root.GetComponent<TITAN>() && !component2.transform.root.GetComponent<TITAN>().hasDie)
                        {
                            int num2 = (int)((IN_GAME_MAIN_CAMERA.MainR.velocity - component2.transform.root.rigidbody.velocity).magnitude * 10f * this.scoreMulti);
                            num2 = Mathf.Max(10, num2);
                            if (Settings.Snapshots.ToValue() && Settings.SnapshotsDamage.Value <= num2)
                            {
                                IN_GAME_MAIN_CAMERA.MainCamera.startSnapShot(component2.transform.position, num2, component2.transform.root.gameObject, 0.02f);
                            }
                            component2.transform.root.GetComponent<TITAN>().Die();
                            this.napeMeat(IN_GAME_MAIN_CAMERA.MainR.velocity, component2.transform.root);
                            FengGameManagerMKII.FGM.netShowDamage(num2);
                            FengGameManagerMKII.FGM.PlayerKillInfoSingleUpdate(num2);
                        }
                    }
                    else if (!PhotonNetwork.IsMasterClient || !component2.transform.root.gameObject.GetPhotonView().IsMine)
                    {
                        if (component2.transform.root.GetComponent<TITAN>())
                        {
                            if (!component2.transform.root.GetComponent<TITAN>().hasDie)
                            {
                                int num3 = (int)((IN_GAME_MAIN_CAMERA.MainR.velocity - component2.transform.root.rigidbody.velocity).magnitude * 10f * this.scoreMulti);
                                num3 = Mathf.Max(10, num3);
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
                        else if (component2.transform.root.GetComponent<FEMALE_TITAN>())
                        {
                            baseT.root.GetComponent<HERO>().useBlade(int.MaxValue);
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
                        else if (component2.transform.root.GetComponent<COLOSSAL_TITAN>())
                        {
                            baseT.root.GetComponent<HERO>().useBlade(int.MaxValue);
                            if (!component2.transform.root.GetComponent<COLOSSAL_TITAN>().hasDie)
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
                    }
                    else if (component2.transform.root.GetComponent<TITAN>())
                    {
                        if (!component2.transform.root.GetComponent<TITAN>().hasDie)
                        {
                            int num6 = (int)((IN_GAME_MAIN_CAMERA.MainR.velocity - component2.transform.root.rigidbody.velocity).magnitude * 10f * this.scoreMulti);
                            num6 = Mathf.Max(10, num6);
                            if (Settings.Snapshots.ToValue() && Settings.SnapshotsDamage.Value <= num6)
                            {
                                IN_GAME_MAIN_CAMERA.MainCamera.startSnapShot(component2.transform.position, num6, component2.transform.root.gameObject, 0.02f);
                            }
                            component2.transform.root.GetComponent<TITAN>().titanGetHit(baseT.root.gameObject.GetPhotonView().viewID, num6);
                        }
                    }
                    else if (component2.transform.root.GetComponent<FEMALE_TITAN>())
                    {
                        baseT.root.GetComponent<HERO>().useBlade(int.MaxValue);
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
                    else if (component2.transform.root.GetComponent<COLOSSAL_TITAN>())
                    {
                        baseT.root.GetComponent<HERO>().useBlade(int.MaxValue);
                        if (!component2.transform.root.GetComponent<COLOSSAL_TITAN>().hasDie)
                        {
                            int num8 = (int)((IN_GAME_MAIN_CAMERA.MainR.velocity - component2.transform.root.rigidbody.velocity).magnitude * 10f * this.scoreMulti);
                            num8 = Mathf.Max(10, num8);
                            if (Settings.Snapshots.ToValue() && Settings.SnapshotsDamage.Value <= num8)
                            {
                                IN_GAME_MAIN_CAMERA.MainCamera.startSnapShot(component2.transform.position, num8, null, 0.02f);
                            }
                            component2.transform.root.GetComponent<COLOSSAL_TITAN>().titanGetHit(baseT.root.gameObject.GetPhotonView().viewID, num8);
                        }
                    }
                    this.showCriticalHitFX();
                }
                break;

            case "titaneye":
                if (!this.currentHits.Contains(other.gameObject))
                {
                    this.currentHits.Add(other.gameObject);
                    GameObject gameObject2 = other.gameObject.transform.root.gameObject;
                    if (gameObject2.GetComponent<FEMALE_TITAN>())
                    {
                        if (IN_GAME_MAIN_CAMERA.GameType == GameType.Single)
                        {
                            if (!gameObject2.GetComponent<FEMALE_TITAN>().hasDie)
                            {
                                gameObject2.GetComponent<FEMALE_TITAN>().hitEye();
                            }
                        }
                        else if (!PhotonNetwork.IsMasterClient)
                        {
                            if (!gameObject2.GetComponent<FEMALE_TITAN>().hasDie)
                            {
                                gameObject2.GetComponent<FEMALE_TITAN>().BasePV.RPC("hitEyeRPC", PhotonTargets.MasterClient, new object[]
                                {
                                    baseT.root.gameObject.GetPhotonView().viewID
                                });
                            }
                        }
                        else if (!gameObject2.GetComponent<FEMALE_TITAN>().hasDie)
                        {
                            gameObject2.GetComponent<FEMALE_TITAN>().hitEyeRPC(baseT.root.gameObject.GetPhotonView().viewID);
                        }
                    }
                    else if (gameObject2.GetComponent<TITAN>().abnormalType != AbnormalType.Crawler)
                    {
                        if (IN_GAME_MAIN_CAMERA.GameType == GameType.Single)
                        {
                            if (!gameObject2.GetComponent<TITAN>().hasDie)
                            {
                                gameObject2.GetComponent<TITAN>().HitEye();
                            }
                        }
                        else if (!PhotonNetwork.IsMasterClient || !gameObject2.GetPhotonView().IsMine)
                        {
                            if (!gameObject2.GetComponent<TITAN>().hasDie)
                            {
                                gameObject2.GetComponent<TITAN>().BasePV.RPC("hitEyeRPC", PhotonTargets.MasterClient, new object[]
                                {
                                    baseT.root.gameObject.GetPhotonView().viewID
                                });
                            }
                        }
                        else if (!gameObject2.GetComponent<TITAN>().hasDie)
                        {
                            gameObject2.GetComponent<TITAN>().hitEyeRPC(baseT.root.gameObject.GetPhotonView().viewID);
                        }
                        this.showCriticalHitFX();
                    }
                }
                break;

            case "titanankle":
                if (currentHits.Contains(other.gameObject)) return;
                this.currentHits.Add(other.gameObject);
                GameObject gameObject3 = other.gameObject.transform.root.gameObject;
                int num9 = (int)((IN_GAME_MAIN_CAMERA.MainR.velocity - gameObject3.rigidbody.velocity).magnitude * 10f * this.scoreMulti);
                num9 = Mathf.Max(10, num9);
                if (gameObject3.GetComponent<TITAN>() && gameObject3.GetComponent<TITAN>().abnormalType != AbnormalType.Crawler)
                {
                    if (IN_GAME_MAIN_CAMERA.GameType == GameType.Single)
                    {
                        if (!gameObject3.GetComponent<TITAN>().hasDie)
                        {
                            gameObject3.GetComponent<TITAN>().HitAnkle();
                        }
                    }
                    else
                    {
                        if (!PhotonNetwork.IsMasterClient || !gameObject3.GetPhotonView().IsMine)
                        {
                            if (!gameObject3.GetComponent<TITAN>().hasDie)
                            {
                                gameObject3.GetComponent<TITAN>().BasePV.RPC("hitAnkleRPC", PhotonTargets.MasterClient, new object[]
                                {
                                    baseT.root.gameObject.GetPhotonView().viewID
                                });
                            }
                        }
                        else if (!gameObject3.GetComponent<TITAN>().hasDie)
                        {
                            gameObject3.GetComponent<TITAN>().HitAnkle();
                        }
                        this.showCriticalHitFX();
                    }
                }
                else if (gameObject3.GetComponent<FEMALE_TITAN>())
                {
                    if (IN_GAME_MAIN_CAMERA.GameType == GameType.Single)
                    {
                        if (other.gameObject.name == "ankleR")
                        {
                            if (gameObject3.GetComponent<FEMALE_TITAN>() && !gameObject3.GetComponent<FEMALE_TITAN>().hasDie)
                            {
                                gameObject3.GetComponent<FEMALE_TITAN>().hitAnkleR(num9);
                            }
                        }
                        else if (gameObject3.GetComponent<FEMALE_TITAN>() && !gameObject3.GetComponent<FEMALE_TITAN>().hasDie)
                        {
                            gameObject3.GetComponent<FEMALE_TITAN>().hitAnkleL(num9);
                        }
                    }
                    else if (other.gameObject.name == "ankleR")
                    {
                        if (!PhotonNetwork.IsMasterClient)
                        {
                            if (!gameObject3.GetComponent<FEMALE_TITAN>().hasDie)
                            {
                                gameObject3.GetComponent<FEMALE_TITAN>().BasePV.RPC("hitAnkleRRPC", PhotonTargets.MasterClient, new object[]
                                {
                                    baseT.root.gameObject.GetPhotonView().viewID,
                                    num9
                                });
                            }
                        }
                        else if (!gameObject3.GetComponent<FEMALE_TITAN>().hasDie)
                        {
                            gameObject3.GetComponent<FEMALE_TITAN>().hitAnkleRRPC(baseT.root.gameObject.GetPhotonView().viewID, num9);
                        }
                    }
                    else if (!PhotonNetwork.IsMasterClient)
                    {
                        if (!gameObject3.GetComponent<FEMALE_TITAN>().hasDie)
                        {
                            gameObject3.GetComponent<FEMALE_TITAN>().BasePV.RPC("hitAnkleLRPC", PhotonTargets.MasterClient, new object[]
                            {
                                baseT.root.gameObject.GetPhotonView().viewID,
                                num9
                            });
                        }
                    }
                    else if (!gameObject3.GetComponent<FEMALE_TITAN>().hasDie)
                    {
                        gameObject3.GetComponent<FEMALE_TITAN>().hitAnkleLRPC(baseT.root.gameObject.GetPhotonView().viewID, num9);
                    }
                    this.showCriticalHitFX();
                }
                break;
        }
        //}
    }

    private void showCriticalHitFX()
    {
        IN_GAME_MAIN_CAMERA.MainCamera.startShake(0.2f, 0.3f, 0.95f);
        //GameObject gameObject;
        if (IN_GAME_MAIN_CAMERA.GameType != GameType.Single)
        {
            Optimization.Caching.Pool.NetworkEnable("redCross", baseT.position, Quaternion.Euler(270f, 0f, 0f), 0);
        }
        else
        {
            Pool.Enable("redCross", baseT.position, Quaternion.Euler(270f, 0f, 0f));//gameObject = //(GameObject)UnityEngine.Object.Instantiate(CacheResources.Load("redCross"));
        }
        //gameObject.transform.position = baseT.position;
    }

    public void clearHits()
    {
        this.currentHitsII = new ArrayList();
        this.currentHits = new ArrayList();
    }
}