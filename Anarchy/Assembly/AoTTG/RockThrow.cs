using Optimization.Caching;
using UnityEngine;

public class RockThrow : Photon.MonoBehaviour
{
    private Transform baseT;
    private bool launched;
    private Vector3 oldP;
    private Vector3 r;
    private Vector3 v;

    private void Awake()
    {
        baseT = transform;
    }

    private void explore()
    {
        GameObject gameObject;
        if (IN_GAME_MAIN_CAMERA.GameType == GameType.MultiPlayer && PhotonNetwork.IsMasterClient)
        {
            gameObject = Pool.NetworkEnable("FX/boom6", baseT.position, baseT.rotation, 0);
            if (baseT.root.gameObject.GetComponent<EnemyfxIDcontainer>() != null)
            {
                gameObject.GetComponent<EnemyfxIDcontainer>().myOwnerViewID = baseT.root.gameObject.GetComponent<EnemyfxIDcontainer>().myOwnerViewID;
                gameObject.GetComponent<EnemyfxIDcontainer>().titanName = baseT.root.gameObject.GetComponent<EnemyfxIDcontainer>().titanName;
            }
        }
        else
        {
            gameObject = Pool.Enable("FX/boom6", baseT.position, baseT.rotation);//(GameObject)UnityEngine.Object.Instantiate(CacheResources.Load("FX/boom6"), baseT.position, baseT.rotation);
        }
        gameObject.transform.localScale = baseT.localScale;
        float num = 1f - Vector3.Distance(IN_GAME_MAIN_CAMERA.BaseCamera.transform.position, gameObject.transform.position) * 0.05f;
        num = Mathf.Min(1f, num);
        IN_GAME_MAIN_CAMERA.MainCamera.startShake(num, num, 0.95f);
        if (IN_GAME_MAIN_CAMERA.GameType == GameType.Single)
        {
            Pool.Disable(gameObject);
        }
        else
        {
            PhotonNetwork.Destroy(BasePV);
        }
    }

    private void hitPlayer(GameObject go)
    {
        HERO hero = go.GetComponent<HERO>();
        if (hero != null && !hero.HasDied() && !hero.IsInvincible())
        {
            if (IN_GAME_MAIN_CAMERA.GameType == GameType.Single)
            {
                if (!hero.IsGrabbed)
                {
                    hero.Die(this.v.normalized * 1000f + Vectors.up * 50f, false);
                }
            }
            else if (IN_GAME_MAIN_CAMERA.GameType == GameType.MultiPlayer && !hero.HasDied() && !hero.IsGrabbed)
            {
                hero.MarkDie();
                int num = -1;
                string text = string.Empty;
                if (baseT.root.gameObject.GetComponent<EnemyfxIDcontainer>() != null)
                {
                    num = baseT.root.gameObject.GetComponent<EnemyfxIDcontainer>().myOwnerViewID;
                    text = baseT.root.gameObject.GetComponent<EnemyfxIDcontainer>().titanName;
                }
                hero.BasePV.RPC("netDie", PhotonTargets.All, new object[]
                {
                    this.v.normalized * 1000f + Vectors.up * 50f,
                    false,
                    num,
                    text,
                    true
                });
            }
        }
    }

    [RPC]
    private void initRPC(int viewID, Vector3 scale, Vector3 pos, float level)
    {
        GameObject gameObject = PhotonView.Find(viewID).gameObject;
        Transform parent = gameObject.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/shoulder_R/upper_arm_R/forearm_R/hand_R/hand_R_001");
        baseT.localScale = gameObject.transform.localScale;
        baseT.parent = parent;
        baseT.localPosition = pos;
    }

    [RPC]
    private void launchRPC(Vector3 v, Vector3 p)
    {
        this.launched = true;
        baseT.position = p;
        this.oldP = p;
        baseT.parent = null;
        this.launch(v);
    }

    private void OnDisable()
    {

    }

    private void OnEnable()
    {
        launched = false;
        this.r = new Vector3(UnityEngine.Random.Range(-5f, 5f), UnityEngine.Random.Range(-5f, 5f), UnityEngine.Random.Range(-5f, 5f));
    }

    private void Start()
    {
        this.r = new Vector3(UnityEngine.Random.Range(-5f, 5f), UnityEngine.Random.Range(-5f, 5f), UnityEngine.Random.Range(-5f, 5f));
    }

    private void Update()
    {
        if (this.launched)
        {
            baseT.Rotate(this.r);
            this.v -= 20f * Vectors.up * Time.deltaTime;
            baseT.position += this.v * Time.deltaTime;
            if (IN_GAME_MAIN_CAMERA.GameType == GameType.MultiPlayer && !PhotonNetwork.IsMasterClient)
            {
                return;
            }
            RaycastHit[] array = Physics.SphereCastAll(baseT.position, 2.5f * baseT.lossyScale.x, baseT.position - this.oldP, Vector3.Distance(baseT.position, this.oldP), Layers.PlayersEnemyAABGround);
            foreach (RaycastHit raycastHit in array)
            {
                if (raycastHit.collider.gameObject.layer == Layers.EnemyAABBN)
                {
                    GameObject gameObject = raycastHit.collider.gameObject.transform.root.gameObject;
                    if (gameObject.GetComponent<TITAN>() && !gameObject.GetComponent<TITAN>().hasDie)
                    {
                        gameObject.GetComponent<TITAN>().HitAnkle();
                        Vector3 position = baseT.position;
                        if (IN_GAME_MAIN_CAMERA.GameType == GameType.Single)
                        {
                            gameObject.GetComponent<TITAN>().HitAnkle();
                        }
                        else
                        {
                            if (baseT.root.gameObject.GetComponent<EnemyfxIDcontainer>() != null && PhotonView.Find(baseT.root.gameObject.GetComponent<EnemyfxIDcontainer>().myOwnerViewID))
                            {
                                position = PhotonView.Find(baseT.root.gameObject.GetComponent<EnemyfxIDcontainer>().myOwnerViewID).transform.position;
                            }
                            gameObject.GetComponent<HERO>().BasePV.RPC("hitAnkleRPC", PhotonTargets.All, new object[0]);
                        }
                    }
                    this.explore();
                }
                else if (raycastHit.collider.gameObject.layer == Layers.PlayersN)
                {
                    GameObject gameObject2 = raycastHit.collider.gameObject.transform.root.gameObject;
                    if (gameObject2.GetComponent<TITAN_EREN>())
                    {
                        if (!gameObject2.GetComponent<TITAN_EREN>().isHit)
                        {
                            gameObject2.GetComponent<TITAN_EREN>().hitByTitan();
                        }
                    }
                    else if (gameObject2.GetComponent<HERO>() && !gameObject2.GetComponent<HERO>().IsInvincible())
                    {
                        this.hitPlayer(gameObject2);
                    }
                }
                else if (raycastHit.collider.gameObject.layer == Layers.GroundN)
                {
                    this.explore();
                }
            }
            this.oldP = baseT.position;
        }
    }

    public void launch(Vector3 v1)
    {
        this.launched = true;
        this.oldP = baseT.position;
        this.v = v1;
        if (IN_GAME_MAIN_CAMERA.GameType == GameType.MultiPlayer && PhotonNetwork.IsMasterClient)
        {
            BasePV.RPC("launchRPC", PhotonTargets.Others, new object[]
            {
                this.v,
                this.oldP
            });
        }
    }
}