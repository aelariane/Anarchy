using Anarchy;
using Anarchy.Configuration;
using Optimization.Caching;
using System.Collections;
using UnityEngine;

public class Bomb : Photon.MonoBehaviour
{
    //!!!!NOT TESTED!!!!

    //internal static IntSetting MyBombCD = new IntSetting("MyBombCD", 5);
    //internal static IntSetting MyBombRad = new IntSetting("MyBombRad", 5);
    //internal static IntSetting MyBombRange = new IntSetting("MyBombRange", 5);
   // internal static IntSetting MyBombSpeed = new IntSetting("MyBombSpeed", 5);
    internal static FloatSetting MyBombColorR = new FloatSetting("MyBombColorR", 1f);
    internal static FloatSetting MyBombColorG = new FloatSetting("MyBombColorG", 1f);
    internal static FloatSetting MyBombColorB = new FloatSetting("MyBombColorB", 1f);
    internal static FloatSetting MyBombColorA = new FloatSetting("MyBombColorA", 1f);
    internal static StringSetting BombNameSetting = new StringSetting("bombName", "Bomb");
    internal static string BombName => User.FormatColors(BombNameSetting.Value).Replace("$name$", User.Name.Value).Replace("$chatName$", User.ChatName.Value);

    public Rigidbody baseR;
    public Transform baseT;
    private Vector3 correctPlayerPos = Vectors.zero;
    private Quaternion correctPlayerRot = Quaternion.identity;
    private Vector3 correctPlayerVelocity = Vectors.zero;
    public bool disabled;
    public BombExplode myExplosion;
    private float myRad;
    public float SmoothingDelay = 10f;
    private HERO owner;

    public void Awake()
    {
        this.baseT = transform;
        this.baseR = rigidbody;
    }

    public void destroyMe()
    {
        if (this.BasePV.IsMine)
        {
            if (this.myExplosion != null)
            {
                PhotonNetwork.Destroy(this.myExplosion.gameObject);
            }
            PhotonNetwork.Destroy(base.gameObject);
        }
    }

    public void Explode(float radius)
    {
        this.disabled = true;
        this.baseR.velocity = Vectors.zero;
        this.myExplosion = Pool.NetworkEnable("RCAsset/BombExplodeMain", this.baseT.position, Quaternion.Euler(0f, 0f, 0f), 0).GetComponent<BombExplode>();
        foreach (HERO hero in FengGameManagerMKII.Heroes)
        {
            if (Vector3.Distance(hero.baseT.position, this.baseT.position) < radius && !hero.BasePV.IsMine && !hero.BombImmune)
            {
                PhotonPlayer owner = hero.BasePV.owner;
                if (PhotonNetwork.player.RCteam > 0)
                {
                    int num = PhotonNetwork.player.RCteam;
                    int num2 = owner.RCteam;
                    if (num == 0 || num != num2)
                    {
                        hero.MarkDie();
                        hero.BasePV.RPC("netDie2", PhotonTargets.All, new object[]
                        {
                            -1,
                            BombName
                        }); ;
                        FengGameManagerMKII.FGM.PlayerKillInfoUpdate(PhotonNetwork.player, 0);
                    }
                }
                else
                {
                    hero.MarkDie();
                    hero.BasePV.RPC("netDie2", PhotonTargets.All, new object[]
                    {
                        -1,
                       BombName
                    });
                    FengGameManagerMKII.FGM.PlayerKillInfoUpdate(PhotonNetwork.player, 0);
                }
            }
        }
        base.StartCoroutine(this.WaitAndFade(1.5f));
    }

    private void OnDisable()
    {
        if (owner != null)
        {
            owner.myBomb = null;
            owner = null;
        }
        myExplosion = null;
    }

    private void OnEnable()
    {
        disabled = false;
        if (IN_GAME_MAIN_CAMERA.GameType == GameType.MultiPlayer)
        {
            if (this.BasePV != null && BasePV.owner != null)
            {
                this.BasePV.observed = this;
                this.correctPlayerPos = this.baseT.position;
                this.correctPlayerRot = Quaternion.identity;
                PhotonPlayer owner = this.BasePV.owner;
                if (owner.IsLocal)
                {
                    var stats = RC.Bombs.BombSettings.Load(typeof(TLW.TLWBombCalculatorV1));
                    this.myRad = stats.Radius * 4f + 20f;
                }
                Color startColor = new Color(owner.RCBombR, owner.RCBombG, owner.RCBombB, Mathf.Max(0.5f, owner.RCBombA));
                if (Anarchy.GameModes.TeamMode.Enabled)
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
                GetComponent<ParticleSystem>().startColor = startColor;
            }
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            stream.SendNext(this.baseT.position);
            stream.SendNext(this.baseT.rotation);
            stream.SendNext(this.baseR.velocity);
            return;
        }
        if (stream.data.Count != 3)
        {
            return;
        }
        object obj = stream.ReceiveNext();
        this.correctPlayerPos = ((obj is Vector3) ? ((Vector3)obj) : Vector3.zero);
        obj = stream.ReceiveNext();
        this.correctPlayerRot = ((obj is Quaternion) ? ((Quaternion)obj) : Quaternion.identity);
        obj = stream.ReceiveNext();
        this.correctPlayerVelocity = ((obj is Vector3) ? ((Vector3)obj) : Vector3.zero);
    }

    public void SetMyOwner(HERO hero)
    {
        owner = hero;
    }

    public void Update()
    {
        if (!this.disabled)
        {
            if (!this.BasePV.IsMine)
            {
                float dt = Time.deltaTime;
                this.baseT.position = Vector3.Lerp(this.baseT.position, this.correctPlayerPos, dt * this.SmoothingDelay);
                this.baseT.rotation = Quaternion.Lerp(this.baseT.rotation, this.correctPlayerRot, dt * this.SmoothingDelay);
                this.baseR.velocity = this.correctPlayerVelocity;
            }
        }
    }

    private IEnumerator WaitAndFade(float time)
    {
        yield return new WaitForSeconds(time);
        PhotonNetwork.Destroy(this.myExplosion.gameObject);
        PhotonNetwork.Destroy(base.gameObject);
        yield break;
    }
}