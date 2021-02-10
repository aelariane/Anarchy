using Anarchy;
using Anarchy.Inputs;
using Optimization.Caching;
using RC;
using System;
using UnityEngine;

public class Cannon : Photon.MonoBehaviour
{
    public Transform ballPoint;
    public Transform barrel;
    private Quaternion correctBarrelRot = Quaternion.identity;
    private Vector3 correctPlayerPos = Vector3.zero;
    private Quaternion correctPlayerRot = Quaternion.identity;
    public float currentRot = 0f;
    public Transform firingPoint;
    public bool isCannonGround;
    public GameObject myCannonBall;
    public LineRenderer myCannonLine;
    public HERO myHero;
    public string settings;
    public float SmoothingDelay = 5f;

    public void Awake()
    {
        bool flag = BasePV != null;
        if (flag)
        {
            BasePV.observed = this;
            this.barrel = base.transform.Find("Barrel");
            this.correctPlayerPos = base.transform.position;
            this.correctPlayerRot = base.transform.rotation;
            this.correctBarrelRot = this.barrel.rotation;
            bool isMine = BasePV.IsMine;
            if (isMine)
            {
                this.firingPoint = this.barrel.Find("FiringPoint");
                this.ballPoint = this.barrel.Find("BallPoint");
                this.myCannonLine = this.ballPoint.GetComponent<LineRenderer>();
                bool flag2 = base.gameObject.name.Contains("CannonGround");
                if (flag2)
                {
                    this.isCannonGround = true;
                }
            }
            bool isMasterClient = PhotonNetwork.IsMasterClient;
            if (isMasterClient)
            {
                PhotonPlayer owner = BasePV.owner;
                bool flag3 = RCManager.allowedToCannon.ContainsKey(owner.ID);
                if (flag3)
                {
                    this.settings = RCManager.allowedToCannon[owner.ID].settings;
                    BasePV.RPC("SetSize", PhotonTargets.All, new object[]
                    {
                        this.settings
                    });
                    int viewID = RCManager.allowedToCannon[owner.ID].viewID;
                    RCManager.allowedToCannon.Remove(owner.ID);
                    CannonPropRegion component = PhotonView.Find(viewID).gameObject.GetComponent<CannonPropRegion>();
                    bool flag4 = component != null;
                    if (flag4)
                    {
                        component.disabled = true;
                        component.destroyed = true;
                        PhotonNetwork.Destroy(component.gameObject);
                    }
                }
                else
                {
                    bool flag5 = !owner.IsLocal;// && !FengGameManagerMKII.instance.restartingMC;
                    if (flag5)
                    {
                        //TO DO
                        //FengGameManagerMKII.instance.kickPlayerRC(owner, false, "spawning cannon without request.");
                    }
                }
            }
        }
    }

    public void Fire()
    {
        bool flag = this.myHero.skillCDDuration <= 0f;
        if (flag)
        {
            GameObject gameObject = Pool.NetworkEnable("FX/boom2", this.firingPoint.position, this.firingPoint.rotation, 0);
            foreach (EnemyCheckCollider enemyCheckCollider in gameObject.GetComponentsInChildren<EnemyCheckCollider>())
            {
                enemyCheckCollider.dmg = 0;
            }
            this.myCannonBall = Pool.NetworkEnable("RCAsset/CannonBallObject", this.ballPoint.position, this.firingPoint.rotation, 0);
            this.myCannonBall.rigidbody.velocity = this.firingPoint.forward * 300f;
            this.myCannonBall.GetComponent<CannonBall>().myHero = this.myHero;
            this.myHero.skillCDDuration = 3.5f;
        }
    }

    public void OnDestroy()
    {
        bool flag = PhotonNetwork.IsMasterClient;// && !FengGameManagerMKII.instance.isRestarting;
        if (flag)
        {
            string[] array = this.settings.Split(new char[]
            {
                ','
            });
            bool flag2 = array[0] == "photon";
            if (flag2)
            {
                bool flag3 = array.Length > 15;
                if (flag3)
                {
                    GameObject gameObject = Pool.NetworkEnable("RCAsset/" + array[1] + "Prop", new Vector3(Convert.ToSingle(array[12]), Convert.ToSingle(array[13]), Convert.ToSingle(array[14])), new Quaternion(Convert.ToSingle(array[15]), Convert.ToSingle(array[16]), Convert.ToSingle(array[17]), Convert.ToSingle(array[18])), 0);
                    gameObject.GetComponent<CannonPropRegion>().settings = this.settings;
                    gameObject.GetPhotonView().RPC("SetSize", PhotonTargets.AllBuffered, new object[]
                    {
                        this.settings
                    });
                }
                else
                {
                    Pool.NetworkEnable("RCAsset/" + array[1] + "Prop", new Vector3(Convert.ToSingle(array[2]), Convert.ToSingle(array[3]), Convert.ToSingle(array[4])), new Quaternion(Convert.ToSingle(array[5]), Convert.ToSingle(array[6]), Convert.ToSingle(array[7]), Convert.ToSingle(array[8])), 0).GetComponent<CannonPropRegion>().settings = this.settings;
                }
            }
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        bool isWriting = stream.isWriting;
        if (isWriting)
        {
            stream.SendNext(base.transform.position);
            stream.SendNext(base.transform.rotation);
            stream.SendNext(this.barrel.rotation);
        }
        else
        {
            this.correctPlayerPos = (Vector3)stream.ReceiveNext();
            this.correctPlayerRot = (Quaternion)stream.ReceiveNext();
            this.correctBarrelRot = (Quaternion)stream.ReceiveNext();
        }
    }

    [RPC]
    public void SetSize(string settings, PhotonMessageInfo info)
    {
        bool isMasterClient = info.Sender.IsMasterClient;
        if (isMasterClient)
        {
            string[] array = settings.Split(new char[]
            {
                ','
            });
            bool flag = array.Length > 15;
            if (flag)
            {
                float a = 1f;
                GameObject gameObject = base.gameObject;
                bool flag2 = array[2] != "default";
                if (flag2)
                {
                    bool flag3 = array[2].StartsWith("transparent");
                    if (flag3)
                    {
                        float num;
                        bool flag4 = float.TryParse(array[2].Substring(11), out num);
                        if (flag4)
                        {
                            a = num;
                        }
                        foreach (Renderer renderer in gameObject.GetComponentsInChildren<Renderer>())
                        {
                            renderer.material = (Material)RCManager.Load("transparent");
                            bool flag5 = Convert.ToSingle(array[10]) != 1f || Convert.ToSingle(array[11]) != 1f;
                            if (flag5)
                            {
                                renderer.material.mainTextureScale = new Vector2(renderer.material.mainTextureScale.x * Convert.ToSingle(array[10]), renderer.material.mainTextureScale.y * Convert.ToSingle(array[11]));
                            }
                        }
                    }
                    else
                    {
                        foreach (Renderer renderer2 in gameObject.GetComponentsInChildren<Renderer>())
                        {
                            bool flag6 = !renderer2.name.Contains("Line Renderer");
                            if (flag6)
                            {
                                renderer2.material = (Material)RCManager.Load(array[2]);
                                bool flag7 = Convert.ToSingle(array[10]) != 1f || Convert.ToSingle(array[11]) != 1f;
                                if (flag7)
                                {
                                    renderer2.material.mainTextureScale = new Vector2(renderer2.material.mainTextureScale.x * Convert.ToSingle(array[10]), renderer2.material.mainTextureScale.y * Convert.ToSingle(array[11]));
                                }
                            }
                        }
                    }
                }
                float num2 = gameObject.transform.localScale.x * Convert.ToSingle(array[3]);
                num2 -= 0.001f;
                float y = gameObject.transform.localScale.y * Convert.ToSingle(array[4]);
                float z = gameObject.transform.localScale.z * Convert.ToSingle(array[5]);
                gameObject.transform.localScale = new Vector3(num2, y, z);
                bool flag8 = array[6] != "0";
                if (flag8)
                {
                    Color color = new Color(Convert.ToSingle(array[7]), Convert.ToSingle(array[8]), Convert.ToSingle(array[9]), a);
                    foreach (MeshFilter meshFilter in gameObject.GetComponentsInChildren<MeshFilter>())
                    {
                        Mesh mesh = meshFilter.mesh;
                        Color[] array2 = new Color[mesh.vertexCount];
                        for (int l = 0; l < mesh.vertexCount; l++)
                        {
                            array2[l] = color;
                        }
                        mesh.colors = array2;
                    }
                }
            }
        }
    }

    public void Update()
    {
        bool flag = !BasePV.IsMine;
        if (flag)
        {
            base.transform.position = Vector3.Lerp(base.transform.position, this.correctPlayerPos, Time.deltaTime * this.SmoothingDelay);
            base.transform.rotation = Quaternion.Lerp(base.transform.rotation, this.correctPlayerRot, Time.deltaTime * this.SmoothingDelay);
            this.barrel.rotation = Quaternion.Lerp(this.barrel.rotation, this.correctBarrelRot, Time.deltaTime * this.SmoothingDelay);
        }
        else
        {
            Vector3 a = new Vector3(0f, -30f, 0f);
            Vector3 vector = this.ballPoint.position;
            Vector3 a2 = this.ballPoint.forward * 300f;
            float d = 40f / a2.magnitude;
            this.myCannonLine.SetWidth(0.5f, 40f);
            this.myCannonLine.SetVertexCount(100);
            for (int i = 0; i < 100; i++)
            {
                this.myCannonLine.SetPosition(i, vector);
                vector += a2 * d + 0.5f * a * d * d;
                a2 += a * d;
            }
            float num = 30f;
            bool flag2 = InputManager.IsInputCannonHolding((int)InputCannon.CannonSlow);
            if (flag2)
            {
                num = 5f;
            }
            bool flag3 = this.isCannonGround;
            if (flag3)
            {
                bool flag4 = InputManager.IsInputCannonHolding((int)InputCannon.CannonUp);
                if (flag4)
                {
                    bool flag5 = this.currentRot <= 32f;
                    if (flag5)
                    {
                        this.currentRot += Time.deltaTime * num;
                        this.barrel.Rotate(new Vector3(0f, 0f, Time.deltaTime * num));
                    }
                }
                else
                {
                    bool flag6 = InputManager.IsInputCannonHolding((int)InputCannon.CannonDown) && this.currentRot >= -18f;
                    if (flag6)
                    {
                        this.currentRot += Time.deltaTime * -num;
                        this.barrel.Rotate(new Vector3(0f, 0f, Time.deltaTime * -num));
                    }
                }
                bool flag7 = InputManager.IsInputCannonHolding((int)InputCannon.CannonLeft);
                if (flag7)
                {
                    base.transform.Rotate(new Vector3(0f, Time.deltaTime * -num, 0f));
                }
                else
                {
                    bool flag8 = InputManager.IsInputCannonHolding((int)InputCannon.CannonRight);
                    if (flag8)
                    {
                        base.transform.Rotate(new Vector3(0f, Time.deltaTime * num, 0f));
                    }
                }
            }
            else
            {
                bool flag9 = InputManager.IsInputCannonHolding((int)InputCannon.CannonUp);
                if (flag9)
                {
                    bool flag10 = this.currentRot >= -50f;
                    if (flag10)
                    {
                        this.currentRot += Time.deltaTime * -num;
                        this.barrel.Rotate(new Vector3(Time.deltaTime * -num, 0f, 0f));
                    }
                }
                else
                {
                    bool flag11 = InputManager.IsInputCannonHolding((int)InputCannon.CannonDown) && this.currentRot <= 40f;
                    if (flag11)
                    {
                        this.currentRot += Time.deltaTime * num;
                        this.barrel.Rotate(new Vector3(Time.deltaTime * num, 0f, 0f));
                    }
                }
                bool flag12 = InputManager.IsInputCannonHolding((int)InputCannon.CannonLeft);
                if (flag12)
                {
                    base.transform.Rotate(new Vector3(0f, Time.deltaTime * -num, 0f));
                }
                else
                {
                    bool flag13 = InputManager.IsInputCannonHolding((int)InputCannon.CannonRight);
                    if (flag13)
                    {
                        base.transform.Rotate(new Vector3(0f, Time.deltaTime * num, 0f));
                    }
                }
            }
            bool flag14 = InputManager.IsInputCannonDown((int)InputCannon.CannonFire);
            if (flag14)
            {
                this.Fire();
            }
            else
            {
                bool flag15 = InputManager.IsInputCannonDown((int)InputCannon.CannonMount);
                if (flag15)
                {
                    bool flag16 = this.myHero != null;
                    if (flag16)
                    {
                        this.myHero.isCannon = false;
                        this.myHero.myCannonRegion = null;
                        IN_GAME_MAIN_CAMERA.MainCamera.SetMainObject(this.myHero, true, false);
                        this.myHero.baseR.velocity = Vector3.zero;
                        this.myHero.BasePV.RPC("ReturnFromCannon", PhotonTargets.Others, new object[0]);
                        this.myHero.skillCDLast = this.myHero.skillCDLastCannon;
                        this.myHero.skillCDDuration = this.myHero.skillCDLast;
                    }
                    PhotonNetwork.Destroy(base.gameObject);
                }
            }
        }
    }
}