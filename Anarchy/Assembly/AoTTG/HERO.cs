using System;
using System.Collections.Generic;
using Anarchy;
using Anarchy.Configuration;
using Anarchy.Configuration.Presets;
using Anarchy.InputPos;
using Anarchy.Network;
using Anarchy.Skins.Humans;
using ExitGames.Client.Photon;
using Optimization;
using Optimization.Caching;
using RC;
using UnityEngine;
using Xft;
using Random = UnityEngine.Random;

public class HERO : Optimization.Caching.Bases.HeroBase
{
    private HeroState _state;
    private bool almostSingleHook;
    private string attackAnimation;
    private int attackLoop;
    private bool attackMove;
    private bool attackReleased;
    private GameObject badGuy;
    private float buffTime;
    private int bulletMAX = 7;
    private bool buttonAttackRelease;
    private Transform crossT1;
    private Transform crossT2;
    private GameObject crossL1;
    private GameObject crossL2;
    private GameObject crossR1;
    private GameObject crossR2;
    private Transform crossL1T;
    private Transform crossL2T;
    private Transform crossR1T;
    private Transform crossR2T;
    private int currentBladeNum = 5;
    private float currentBladeSta = 100f;
    private Buff currentBuff;
    private float currentGas = 100f;
    private Vector3 currentV;
    private Vector3 dashDirection;
    private float dashTime;
    private Vector3 dashV;
    private bool detonate;
    private float dTapTime = -1f;
    private bool EHold;
    public GameObject eren;
    private int escapeTimes = 1;
    private float facingDirection;
    private float flare1CD;
    private float flare2CD;
    private float flare3CD;
    private float flareTotalCD = 30f;
    private float gravity = 20f;
    private bool grounded;
    private GameObject gunDummy;
    private Vector3 gunTarget;
    private bool hasDied;
    private bool hookBySomeOne = true;
    private bool hookSomeOne;
    private GameObject hookTarget;
    private float invincible = 3f;
    private bool isLaunchLeft;
    private bool isLaunchRight;
    private bool isLeftHandHooked;
    private bool isMounted;
    private bool isRightHandHooked;
    private bool justGrounded;
    private UILabel LabelDistance;
    private Transform labelT;
    private float launchElapsedTimeL;
    private float launchElapsedTimeR;
    private Vector3 launchForce;
    private Vector3 launchPointLeft;
    private Vector3 launchPointRight;
    private bool leanLeft;
    private bool leftArmAim;
    private int leftBulletLeft = 7;
    private bool leftGunHasBullet = true;
    private float lTapTime = -1f;
    private GameObject myHorse;
    private GameObject myNetWorkName;
    private bool needLean;
    private Quaternion oldHeadRotation;
    private float originVM;
    private bool QHold;
    private float reelAxis = 0f;
    private string reloadAnimation = string.Empty;
    private bool rightArmAim;
    private int rightBulletLeft = 7;
    private bool rightGunHasBullet = true;
    private float rTapTime = -1f;
    private GameObject skillCD;
    private string skillID;
    private string skillIDHUD;
    private ParticleSystem smoke_3dmg;
    private ParticleSystem sparks;
    private bool spawned = false;
    private ParticleSystem speedFXPS;
    private string standAnimation = "stand";
    private Quaternion targetHeadRotation;
    private Quaternion targetRotation;
    private Vector3 targetV;
    private bool throwedBlades;
    private GameObject titanWhoGrabMe;
    private int titanWhoGrabMeID;
    private int totalBladeNum = 5;
    private float useGasSpeed = 0.2f;
    private float uTapTime = -1f;
    private bool wallJump;
    private float wallRunTime;
    private TriggerColliderWeapon wLeft;
    private TriggerColliderWeapon wRight;

    public static List<TITAN> myTitans = new List<TITAN>();
    public AudioSource audio_ally;
    public AudioSource audio_hitwall;
    public float bombCD;
    public bool BombImmune;
    public float bombRadius;
    public float bombSpeed;
    public float bombTime;
    public float bombTimeMax;
    public Bullet bulletLeft;
    public Bullet bulletRight;
    public float CameraMultiplier;
    public bool canJump = true;
    public GameObject checkBoxLeft;
    public GameObject checkBoxRight;
    public string currentAnimation;
    public Camera currentCamera;
    public float currentSpeed;
    public GameObject hookRefL1;
    public GameObject hookRefL2;
    public GameObject hookRefR1;
    public GameObject hookRefR2;
    public bool isCannon;
    public bool IsPhotonCamera;
    public float jumpHeight = 2f;
    public Bullet LastHook;
    public XWeaponTrail leftbladetrail;
    public XWeaponTrail leftbladetrail2;
    public float maxVelocityChange = 10f;
    public AudioSource meatDie;
    internal Bomb myBomb;
    public GameObject myCannon;
    public Transform myCannonBase;
    public Transform myCannonPlayer;
    public CannonPropRegion myCannonRegion;
    public Group myGroup;
    public float myScale = 1f;
    private static string mySkinURL = "";
    public int myTeam = 1;
    public XWeaponTrail rightbladetrail;
    public XWeaponTrail rightbladetrail2;
    public AudioSource rope;
    public HERO_SETUP Setup;
    public float skillCDDuration;
    public float skillCDLast;
    public float skillCDLastCannon;
    internal HumanSkin Skin = null;
    public string[] SkinData = null;
    public AudioSource slash;
    public AudioSource slashHit;
    public float speed = 10f;
    public GameObject speedFX;
    public GameObject speedFX1;
    public bool titanForm;
    public float totalBladeSta = 100f;
    public float totalGas = 100f;
    public bool Gunner;

    private HeroState State
    {
        get
        {
            return this._state;
        }
        set
        {
            if (this._state == HeroState.AirDodge || this._state == HeroState.GroundDodge)
            {
                this.dashTime = 0f;
            }
            this._state = value;
        }
    }

    public bool isGrabbed
    {
        get
        {
            return this.State == HeroState.Grab;
        }
    }

    private void applyForceToBody(GameObject GO, Vector3 v)
    {
        GO.rigidbody.AddForce(v);
        GO.rigidbody.AddTorque(UnityEngine.Random.Range(-10f, 10f), UnityEngine.Random.Range(-10f, 10f), UnityEngine.Random.Range(-10f, 10f));
    }

    private void Awake()
    {
        base.Cache();
        this.Setup = baseG.GetComponent<HERO_SETUP>();
        baseR.freezeRotation = true;
        baseR.useGravity = false;
        if (IsLocal)
        {
            wLeft = checkBoxLeft.GetComponent<TriggerColliderWeapon>();
            wRight = checkBoxRight.GetComponent<TriggerColliderWeapon>();
            this.crossT1 = GameObject.Find("cross1").GetComponent<Transform>();
            this.crossT2 = GameObject.Find("cross2").GetComponent<Transform>();
            this.crossL1 = GameObject.Find("crossL1");
            this.crossL2 = GameObject.Find("crossL2");
            this.crossR1 = GameObject.Find("crossR1");
            this.crossR2 = GameObject.Find("crossR2");
            this.crossL1T = this.crossL1.transform;
            this.crossL2T = this.crossL2.transform;
            this.crossR1T = this.crossR1.transform;
            this.crossR2T = this.crossR2.transform;
            this.LabelDistance = CacheGameObject.Find<UILabel>("LabelDistance");
            this.labelT = this.LabelDistance.transform;
        }
        else
        {
            if (IN_GAME_MAIN_CAMERA.GameType == GameType.Multi)
            {
                baseG.layer = Layers.NetworkObjectN;
                GetComponent<CapsuleCollider>().isTrigger = false;
            }
        }
    }

    [RPC]
    private void backToHumanRPC()
    {
        this.titanForm = false;
        this.eren = null;
        baseG.GetComponent<SmoothSyncMovement>().Disabled = false;
    }

    private void bodyLean()
    {
        if (IsLocal && !grounded)
        {
            float z = 0f;
            this.needLean = false;
            if (!grounded && !this.Gunner && this.State == HeroState.Attack && this.attackAnimation != "attack3_1" && this.attackAnimation != "attack3_2")
            {
                float y = baseR.velocity.y;
                float x = baseR.velocity.x;
                float z2 = baseR.velocity.z;
                float x2 = Mathf.Sqrt(x * x + z2 * z2);
                float num = Mathf.Atan2(y, x2) * 57.29578f;
                this.targetRotation = Quaternion.Euler(-num * (1f - Vector3.Angle(baseR.velocity, baseT.Forward()) / 90f), this.facingDirection, 0f);
                if ((this.isLeftHandHooked && this.bulletLeft != null) || (this.isRightHandHooked && this.bulletRight != null))
                {
                    baseT.rotation = this.targetRotation;
                }
                return;
            }
            if (this.isLeftHandHooked && this.bulletLeft != null && this.isRightHandHooked && this.bulletRight != null)
            {
                if (this.almostSingleHook)
                {
                    this.needLean = true;
                    z = this.getLeanAngle(this.bulletRight.transform.position, true);
                }
            }
            else if (this.isLeftHandHooked && this.bulletLeft != null)
            {
                this.needLean = true;
                z = this.getLeanAngle(this.bulletLeft.transform.position, true);
            }
            else if (this.isRightHandHooked && this.bulletRight != null)
            {
                this.needLean = true;
                z = this.getLeanAngle(this.bulletRight.transform.position, false);
            }
            if (this.needLean)
            {
                float num2 = 0f;
                if (!this.Gunner && this.State != HeroState.Attack)
                {
                    num2 = this.currentSpeed * 0.1f;
                    num2 = Mathf.Min(num2, 20f);
                }
                this.targetRotation = Quaternion.Euler(-num2, this.facingDirection, z);
            }
            else if (this.State != HeroState.Attack)
            {
                this.targetRotation = Quaternion.Euler(0f, this.facingDirection, 0f);
            }
        }
    }

    private void bombInit()
    {
        this.skillCDDuration = this.skillCDLast;
        skillIDHUD = skillID;
        if (GameModes.BombMode.Enabled)
        {
            int rad = Bomb.MyBombRad.Value;
            int range = Bomb.MyBombRange.Value;
            int speed = Bomb.MyBombSpeed.Value;
            int cd = Bomb.MyBombCD.Value;
            Hashtable hash = new Hashtable();
            this.bombCD = (float)cd * -0.4f + 5f;
            this.bombTimeMax = ((float)range * 60f + 200f) / ((float)speed * 60f + 200f);
            this.bombRadius = (float)rad * 4f + 20f;
            this.bombSpeed = (float)speed * 60f + 200f;
            hash.Add("RCBombR", Bomb.MyBombColorR.ToValue());
            hash.Add("RCBombG", Bomb.MyBombColorG.ToValue());
            hash.Add("RCBombB", Bomb.MyBombColorB.ToValue());
            hash.Add("RCBombA", Bomb.MyBombColorA.ToValue());
            hash.Add("RCBombRadius", this.bombRadius);
            PhotonNetwork.player.SetCustomProperties(hash);
            this.skillID = "bomb";
            skillIDHUD = "armin";
            this.skillCDLast = this.bombCD;
            skillCDDuration = 10f;
            if (FengGameManagerMKII.FGM.Logic.RoundTime > 10f)
            {
                skillCDDuration = 5f;
            }
        }
    }

    private void bombUpdate()
    {
        if (InputManager.IsInputDown[InputCode.Attack1] && this.skillCDDuration <= 0f)
        {
            if (!(this.myBomb == null) && !this.myBomb.disabled)
            {
                this.myBomb.Explode(this.bombRadius);
            }
            Color color = Colors.white;
            ExitGames.Client.Photon.Hashtable hash = new ExitGames.Client.Photon.Hashtable();
            hash.Add("RCBombR", Bomb.MyBombColorR.ToValue());
            hash.Add("RCBombG", Bomb.MyBombColorG.ToValue());
            hash.Add("RCBombB", Bomb.MyBombColorB.ToValue());
            hash.Add("RCBombA", Bomb.MyBombColorA.ToValue());
            PhotonNetwork.player.SetCustomProperties(hash);
            this.detonate = false;
            this.skillCDDuration = this.bombCD;
            RaycastHit hitInfo = default(RaycastHit);
            Ray ray = IN_GAME_MAIN_CAMERA.BaseCamera.ScreenPointToRay(Input.mousePosition);
            this.currentV = this.baseT.position;
            this.targetV = this.currentV + Vectors.forward * 200f;
            if (Physics.Raycast(ray, out hitInfo, 1000000f, Layers.EnemyGround.value))
            {
                this.targetV = hitInfo.point;
            }
            Vector3 vector = Vector3.Normalize(this.targetV - this.currentV);
            this.myBomb = Pool.NetworkEnable("RCAsset/BombMain", this.currentV + vector * 4f, new Quaternion(0f, 0f, 0f, 1f), 0).GetComponent<Bomb>();
            myBomb.SetMyOwner(this);
            this.myBomb.rigidbody.velocity = vector * this.bombSpeed;
            this.bombTime = 0f;
            return;
        }
        if (this.myBomb != null && !this.myBomb.disabled)
        {
            this.bombTime += Time.deltaTime;
            bool flag2 = false;
            if (InputManager.Settings[InputCode.Attack1].IsUp())
            {
                this.detonate = true;
            }
            else if (InputManager.IsInputDown[InputCode.Attack1] && this.detonate)
            {
                this.detonate = false;
                flag2 = true;
            }
            if (this.bombTime >= this.bombTimeMax)
            {
                flag2 = true;
            }
            if (flag2)
            {
                this.myBomb.Explode(this.bombRadius);
                this.detonate = false;
            }
        }
    }

    private void breakApart(Vector3 v, bool isBite)
    {
        GameObject go = (GameObject)UnityEngine.Object.Instantiate(CacheResources.Load("Character_parts/AOTTG_HERO_body"), baseT.position, baseT.rotation);
        if (go == null)
        {
            return;
        }
        go.gameObject.GetComponent<HERO_SETUP>().myCostume = this.Setup.myCostume;
        go.gameObject.GetComponent<HERO_SETUP>().IsDeadBody = true;
        go.GetComponent<HERO_DEAD_BODY_SETUP>().init(this.currentAnimation, baseA[this.currentAnimation].normalizedTime, BodyParts.ARM_R);
        if (!isBite)
        {
            GameObject gameObject2 = (GameObject)UnityEngine.Object.Instantiate(CacheResources.Load("Character_parts/AOTTG_HERO_body"), baseT.position, baseT.rotation);
            GameObject gameObject3 = (GameObject)UnityEngine.Object.Instantiate(CacheResources.Load("Character_parts/AOTTG_HERO_body"), baseT.position, baseT.rotation);
            GameObject gameObject4 = (GameObject)UnityEngine.Object.Instantiate(CacheResources.Load("Character_parts/AOTTG_HERO_body"), baseT.position, baseT.rotation);
            gameObject2.gameObject.GetComponent<HERO_SETUP>().myCostume = this.Setup.myCostume;
            gameObject3.gameObject.GetComponent<HERO_SETUP>().myCostume = this.Setup.myCostume;
            gameObject4.gameObject.GetComponent<HERO_SETUP>().myCostume = this.Setup.myCostume;
            gameObject2.gameObject.GetComponent<HERO_SETUP>().IsDeadBody = true;
            gameObject3.gameObject.GetComponent<HERO_SETUP>().IsDeadBody = true;
            gameObject4.gameObject.GetComponent<HERO_SETUP>().IsDeadBody = true;
            gameObject2.GetComponent<HERO_DEAD_BODY_SETUP>().init(this.currentAnimation, baseA[this.currentAnimation].normalizedTime, BodyParts.UPPER);
            gameObject3.GetComponent<HERO_DEAD_BODY_SETUP>().init(this.currentAnimation, baseA[this.currentAnimation].normalizedTime, BodyParts.LOWER);
            gameObject4.GetComponent<HERO_DEAD_BODY_SETUP>().init(this.currentAnimation, baseA[this.currentAnimation].normalizedTime, BodyParts.ARM_L);
            this.applyForceToBody(gameObject2, v);
            this.applyForceToBody(gameObject3, v);
            this.applyForceToBody(gameObject4, v);
            if (IN_GAME_MAIN_CAMERA.GameType == GameType.Single || BasePV.IsMine)
            {
                IN_GAME_MAIN_CAMERA.MainCamera.SetMainObject(gameObject2, false, false);
            }
        }
        else if (IN_GAME_MAIN_CAMERA.GameType == GameType.Single || BasePV.IsMine)
        {
            IN_GAME_MAIN_CAMERA.MainCamera.SetMainObject(go, false, false);
        }
        this.applyForceToBody(go, v);
        GameObject gameObject5;
        GameObject gameObject6;
        GameObject gameObject7;
        GameObject gameObject8;
        GameObject gameObject9;
        if (this.Gunner)
        {
            gameObject5 = (GameObject)UnityEngine.Object.Instantiate(CacheResources.Load("Character_parts/character_gun_l"), Hand_L.position, Hand_L.rotation);
            gameObject6 = (GameObject)UnityEngine.Object.Instantiate(CacheResources.Load("Character_parts/character_gun_r"), Hand_R.position, Hand_R.rotation);
            gameObject7 = (GameObject)UnityEngine.Object.Instantiate(CacheResources.Load("Character_parts/character_3dmg_2"), baseT.position, baseT.rotation);
            gameObject8 = (GameObject)UnityEngine.Object.Instantiate(CacheResources.Load("Character_parts/character_gun_mag_l"), baseT.position, baseT.rotation);
            gameObject9 = (GameObject)UnityEngine.Object.Instantiate(CacheResources.Load("Character_parts/character_gun_mag_r"), baseT.position, baseT.rotation);
        }
        else
        {
            gameObject5 = (GameObject)UnityEngine.Object.Instantiate(CacheResources.Load("Character_parts/character_blade_l"), Hand_L.position, Hand_L.rotation);
            gameObject6 = (GameObject)UnityEngine.Object.Instantiate(CacheResources.Load("Character_parts/character_blade_r"), Hand_R.position, Hand_R.rotation);
            gameObject7 = (GameObject)UnityEngine.Object.Instantiate(CacheResources.Load("Character_parts/character_3dmg"), baseT.position, baseT.rotation);
            gameObject8 = (GameObject)UnityEngine.Object.Instantiate(CacheResources.Load("Character_parts/character_3dmg_gas_l"), baseT.position, baseT.rotation);
            gameObject9 = (GameObject)UnityEngine.Object.Instantiate(CacheResources.Load("Character_parts/character_3dmg_gas_r"), baseT.position, baseT.rotation);
        }
        gameObject5.renderer.material = CharacterMaterials.Materials[this.Setup.myCostume._3dmg_texture];
        gameObject6.renderer.material = CharacterMaterials.Materials[this.Setup.myCostume._3dmg_texture];
        gameObject7.renderer.material = CharacterMaterials.Materials[this.Setup.myCostume._3dmg_texture];
        gameObject8.renderer.material = CharacterMaterials.Materials[this.Setup.myCostume._3dmg_texture];
        gameObject9.renderer.material = CharacterMaterials.Materials[this.Setup.myCostume._3dmg_texture];
        this.applyForceToBody(gameObject5, v);
        this.applyForceToBody(gameObject6, v);
        this.applyForceToBody(gameObject7, v);
        this.applyForceToBody(gameObject8, v);
        this.applyForceToBody(gameObject9, v);
    }

    private void bufferUpdate()
    {
        if (this.buffTime > 0f)
        {
            this.buffTime -= Time.deltaTime;
            if (this.buffTime <= 0f)
            {
                this.buffTime = 0f;
                if (this.currentBuff == Buff.SpeedUp && baseA.IsPlaying("run_sasha"))
                {
                    this.crossFade("run", 0.1f);
                }
                this.currentBuff = Buff.NoBuff;
            }
        }
    }

    private void calcFlareCD()
    {
        if (this.flare1CD > 0f)
        {
            this.flare1CD -= Time.deltaTime;
            if (this.flare1CD < 0f)
            {
                this.flare1CD = 0f;
            }
        }
        if (this.flare2CD > 0f)
        {
            this.flare2CD -= Time.deltaTime;
            if (this.flare2CD < 0f)
            {
                this.flare2CD = 0f;
            }
        }
        if (this.flare3CD > 0f)
        {
            this.flare3CD -= Time.deltaTime;
            if (this.flare3CD < 0f)
            {
                this.flare3CD = 0f;
            }
        }
    }

    private void calcSkillCD()
    {
        if (this.skillCDDuration > 0f)
        {
            this.skillCDDuration -= Time.deltaTime;
            if (this.skillCDDuration < 0f)
            {
                this.skillCDDuration = 0f;
            }
        }
    }

    private float CalculateJumpVerticalSpeed()
    {
        return Mathf.Sqrt(2f * this.jumpHeight * this.gravity);
    }

    private void changeBlade()
    {
        if (this.Gunner && !this.grounded && FengGameManagerMKII.Level.Mode == GameMode.PVP_AHSS)
        {
            return;
        }
        this.State = HeroState.ChangeBlade;
        this.throwedBlades = false;
        if (this.Gunner)
        {
            if (!this.leftGunHasBullet && !this.rightGunHasBullet)
            {
                if (this.grounded)
                {
                    this.reloadAnimation = "AHSS_gun_reload_both";
                }
                else
                {
                    this.reloadAnimation = "AHSS_gun_reload_both_air";
                }
            }
            else if (!this.leftGunHasBullet)
            {
                if (this.grounded)
                {
                    this.reloadAnimation = "AHSS_gun_reload_l";
                }
                else
                {
                    this.reloadAnimation = "AHSS_gun_reload_l_air";
                }
            }
            else if (!this.rightGunHasBullet)
            {
                if (this.grounded)
                {
                    this.reloadAnimation = "AHSS_gun_reload_r";
                }
                else
                {
                    this.reloadAnimation = "AHSS_gun_reload_r_air";
                }
            }
            else
            {
                if (this.grounded)
                {
                    this.reloadAnimation = "AHSS_gun_reload_both";
                }
                else
                {
                    this.reloadAnimation = "AHSS_gun_reload_both_air";
                }
                this.leftGunHasBullet = (this.rightGunHasBullet = false);
            }
            this.crossFade(this.reloadAnimation, 0.05f);
        }
        else
        {
            if (!this.grounded)
            {
                this.reloadAnimation = "changeBlade_air";
            }
            else
            {
                this.reloadAnimation = "changeBlade";
            }
            this.crossFade(this.reloadAnimation, 0.1f);
        }
    }

    private void checkDashDoubleTap()
    {
        if (this.uTapTime >= 0f)
        {
            this.uTapTime += Time.deltaTime;
            if (this.uTapTime > 0.2f)
            {
                this.uTapTime = -1f;
            }
        }
        if (this.dTapTime >= 0f)
        {
            this.dTapTime += Time.deltaTime;
            if (this.dTapTime > 0.2f)
            {
                this.dTapTime = -1f;
            }
        }
        if (this.lTapTime >= 0f)
        {
            this.lTapTime += Time.deltaTime;
            if (this.lTapTime > 0.2f)
            {
                this.lTapTime = -1f;
            }
        }
        if (this.rTapTime >= 0f)
        {
            this.rTapTime += Time.deltaTime;
            if (this.rTapTime > 0.2f)
            {
                this.rTapTime = -1f;
            }
        }
        if (InputManager.IsInputDown[InputCode.Up])
        {
            if (this.uTapTime == -1f)
            {
                this.uTapTime = 0f;
            }
            if (this.uTapTime != 0f)
            {
                dash(0f, 1f);
            }
        }
        if (InputManager.IsInputDown[InputCode.Down])
        {
            if (this.dTapTime == -1f)
            {
                this.dTapTime = 0f;
            }
            if (this.dTapTime != 0f)
            {
                dash(0f, -1f);
            }
        }
        if (InputManager.IsInputDown[InputCode.Left])
        {
            if (this.lTapTime == -1f)
            {
                this.lTapTime = 0f;
            }
            if (this.lTapTime != 0f)
            {
                dash(-1f, 0f);
            }
        }
        if (InputManager.IsInputDown[InputCode.Right])
        {
            if (this.rTapTime == -1f)
            {
                this.rTapTime = 0f;
            }
            if (this.rTapTime != 0f)
            {
                dash(1f, 0f);
            }
        }
    }

    private void CheckTitan()
    {
        RaycastHit[] hits = Physics.RaycastAll(IN_GAME_MAIN_CAMERA.BaseCamera.ScreenPointToRay(Input.mousePosition), 180f, Layers.EnemyGroundPlayerAttack.value);
        List<RaycastHit> sortedHits = new List<RaycastHit>();
        List<TITAN> currentTitans = new List<TITAN>();
        foreach (RaycastHit hit in hits)
        {
            sortedHits.Add(hit);
        }
        sortedHits.Sort((x, y) => x.distance.CompareTo(y.distance));
        float maxDistance = 180f;
        for (int i = 0; i < hits.Length; i++)
        {
            GameObject hitObject = hits[i].collider.gameObject;
            if (hitObject.layer == 16)
            {
                if (hitObject.name.Contains("PlayerDetectorRC") && hits[i].distance < maxDistance)
                {
                    hitObject.transform.root.gameObject.GetComponent<TITAN>().IsLook = true;
                    maxDistance -= 60f;
                    if (maxDistance <= 60f)
                    {
                        i = sortedHits.Count;
                    }
                    TITAN titan = hitObject.transform.root.gameObject.GetComponent<TITAN>();
                    if (titan != null)
                    {
                        currentTitans.Add(titan);
                    }
                }
            }
            else
            {
                i = sortedHits.Count;
            }
        }
        foreach (TITAN oldTitan in myTitans)
        {
            if (!currentTitans.Contains(oldTitan))
            {
                oldTitan.IsLook = false;
            }
        }
        foreach (TITAN newTitan in currentTitans)
        {
            newTitan.IsLook = true;
        }
        myTitans = currentTitans;
        //foreach (TITAN tit in FengGameManagerMKII.Titans)
        //{
        //    tit.IsLook = false;
        //    if (Vector3.Distance(baseT.position, tit.baseT.position) < 150f)
        //    {
        //        tit.IsLook = true;
        //    }
        //}
    }

    private void customAnimationSpeed()
    {
        baseA["attack5"].speed = 1.85f;
        baseA["changeBlade"].speed = 1.2f;
        baseA["air_release"].speed = 0.6f;
        baseA["changeBlade_air"].speed = 0.8f;
        baseA["AHSS_gun_reload_both"].speed = 0.38f;
        baseA["AHSS_gun_reload_both_air"].speed = 0.5f;
        baseA["AHSS_gun_reload_l"].speed = 0.4f;
        baseA["AHSS_gun_reload_l_air"].speed = 0.5f;
        baseA["AHSS_gun_reload_r"].speed = 0.4f;
        baseA["AHSS_gun_reload_r_air"].speed = 0.5f;
    }

    private void dash(float horizontal, float vertical)
    {
        if (this.dashTime > 0f)
        {
            return;
        }
        if (this.currentGas <= 0f)
        {
            return;
        }
        if (this.isMounted)
        {
            return;
        }
        this.useGas(this.totalGas * 0.04f);
        this.facingDirection = this.getGlobalFacingDirection(horizontal, vertical);
        this.dashV = this.getGlobaleFacingVector3(this.facingDirection);
        this.originVM = this.currentSpeed;
        Quaternion rotation = Quaternion.Euler(0f, this.facingDirection, 0f);
        baseR.rotation = rotation;
        this.targetRotation = rotation;
        if (IN_GAME_MAIN_CAMERA.GameType == GameType.Single)
        {
            Pool.Enable("FX/boost_smoke", baseT.position, baseT.rotation);
        }
        else
        {
            Pool.NetworkEnable("FX/boost_smoke", baseT.position, baseT.rotation, 0);
        }
        this.dashTime = 0.5f;
        this.crossFade("dash", 0.1f);
        baseA["dash"].time = 0.1f;
        this.State = HeroState.AirDodge;
        this.falseAttack();
        baseR.AddForce(this.dashV * 40f, ForceMode.VelocityChange);
    }

    private void dodge(bool offTheWall = false)
    {
        if (this.myHorse != null && !this.isMounted && Vector3.Distance(this.myHorse.transform.position, baseT.position) < 15f)
        {
            this.getOnHorse();
            return;
        }
        this.State = HeroState.GroundDodge;
        if (!offTheWall)
        {
            float num;
            if (InputManager.IsInput[InputCode.Up])
            {
                num = 1f;
            }
            else if (InputManager.IsInput[InputCode.Down])
            {
                num = -1f;
            }
            else
            {
                num = 0f;
            }
            float num2;
            if (InputManager.IsInput[InputCode.Left])
            {
                num2 = -1f;
            }
            else if (InputManager.IsInput[InputCode.Right])
            {
                num2 = 1f;
            }
            else
            {
                num2 = 0f;
            }
            float globalFacingDirection = this.getGlobalFacingDirection(num2, num);
            if (num2 != 0f || num != 0f)
            {
                this.facingDirection = globalFacingDirection + 180f;
                this.targetRotation = Quaternion.Euler(0f, this.facingDirection, 0f);
            }
            this.crossFade("dodge", 0.1f);
        }
        else
        {
            this.playAnimation("dodge");
            this.playAnimationAt("dodge", 0.2f);
        }
        this.sparks.enableEmission = false;
    }

    private void erenTransform()
    {
        this.skillCDDuration = this.skillCDLast;
        if (this.bulletLeft)
        {
            bulletLeft.RemoveMe();
        }
        if (this.bulletRight)
        {
            bulletRight.RemoveMe();
        }
        if (IN_GAME_MAIN_CAMERA.GameType == GameType.Single)
        {
            this.eren = (GameObject)UnityEngine.Object.Instantiate(CacheResources.Load("TITAN_EREN"), baseT.position, baseT.rotation);
        }
        else
        {
            this.eren = Optimization.Caching.Pool.NetworkEnable("TITAN_EREN", baseT.position, baseT.rotation, 0);
        }
        this.eren.GetComponent<TITAN_EREN>().realBody = baseG;
        IN_GAME_MAIN_CAMERA.MainCamera.flashBlind();
        IN_GAME_MAIN_CAMERA.MainCamera.SetMainObject(this.eren.GetComponent<TITAN_EREN>(), true, false);
        this.eren.GetComponent<TITAN_EREN>().born();
        this.eren.rigidbody.velocity = baseR.velocity;
        baseR.velocity = Vectors.zero;
        baseT.position = this.eren.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/neck").position;
        this.titanForm = true;
        if (IN_GAME_MAIN_CAMERA.GameType != GameType.Single)
        {
            BasePV.RPC("whoIsMyErenTitan", PhotonTargets.Others, new object[]
            {
                this.eren.GetPhotonView().viewID
            });
        }
        if (this.smoke_3dmg.enableEmission && IN_GAME_MAIN_CAMERA.GameType != GameType.Single && BasePV.IsMine)
        {
            BasePV.RPC("net3DMGSMOKE", PhotonTargets.Others, new object[]
            {
                false
            });
        }
        this.smoke_3dmg.enableEmission = false;
    }

    private void escapeFromGrab()
    {
    }

    private TITAN FindNearestTitan()
    {
        TITAN res = null;
        float positiveInfinity = float.PositiveInfinity;
        Vector3 position = baseT.position;
        foreach (var tit in FengGameManagerMKII.Titans)
        {
            var inf = (tit.baseT.position - position).sqrMagnitude;
            if (inf < positiveInfinity)
            {
                res = tit;
                positiveInfinity = inf;
            }
        }
        return res;
    }

    private void FixedUpdate()
    {
        if (isCannon || this.titanForm || (IN_GAME_MAIN_CAMERA.isPausing && IN_GAME_MAIN_CAMERA.GameType == GameType.Single))
        {
            return;
        }
        this.currentSpeed = baseR.velocity.magnitude;
        if (!IsLocal) return;
        CheckTitan();
        if (this.State == HeroState.Grab)
        {
            baseR.AddForce(-baseR.velocity, ForceMode.VelocityChange);
            return;
        }
        if (this.IsGrounded())
        {
            if (!this.grounded)
            {
                this.justGrounded = true;
            }
            this.grounded = true;
        }
        else
        {
            this.grounded = false;
        }
        if (this.hookSomeOne)
        {
            if (this.hookTarget != null)
            {
                Vector3 vector = this.hookTarget.transform.position - baseT.position;
                float magnitude = vector.magnitude;
                if (magnitude > 2f)
                {
                    baseR.AddForce(vector.normalized * Mathf.Pow(magnitude, 0.15f) * 30f - baseR.velocity * 0.95f, ForceMode.VelocityChange);
                }
            }
            else
            {
                this.hookSomeOne = false;
            }
        }
        else if (this.hookBySomeOne && this.badGuy != null)
        {
            if (this.badGuy != null)
            {
                Vector3 vector2 = this.badGuy.transform.position - baseT.position;
                float magnitude2 = vector2.magnitude;
                if (magnitude2 > 5f)
                {
                    baseR.AddForce(vector2.normalized * Mathf.Pow(magnitude2, 0.15f) * 0.2f, ForceMode.Impulse);
                }
            }
            else
            {
                this.hookBySomeOne = false;
            }
        }
        float num = 0f;
        float num2 = 0f;
        if (!IN_GAME_MAIN_CAMERA.isPausing && !IN_GAME_MAIN_CAMERA.isTyping)
        {
            if (InputManager.IsInput[InputCode.Up])
            {
                num2 = 1f;
            }
            else if (InputManager.IsInput[InputCode.Down])
            {
                num2 = -1f;
            }
            if (InputManager.IsInput[InputCode.Left])
            {
                num = -1f;
            }
            else if (InputManager.IsInput[InputCode.Right])
            {
                num = 1f;
            }
        }
        bool flag = false;
        bool flag2 = false;
        bool flag3 = false;
        this.isLeftHandHooked = false;
        this.isRightHandHooked = false;
        if (this.isLaunchLeft)
        {
            if (this.bulletLeft != null && bulletLeft.IsHooked())
            {
                this.isLeftHandHooked = true;
                Vector3 vector3 = this.bulletLeft.transform.position - baseT.position;
                vector3.Normalize();
                vector3 *= 10f;
                if (!this.isLaunchRight)
                {
                    vector3 *= 2f;
                }
                if (Vector3.Angle(baseR.velocity, vector3) > 90f && InputManager.IsInput[InputCode.Gas])
                {
                    flag2 = true;
                    flag = true;
                }
                if (!flag2)
                {
                    baseR.AddForce(vector3);
                    if (Vector3.Angle(baseR.velocity, vector3) > 90f)
                    {
                        baseR.AddForce(-baseR.velocity * 2f, ForceMode.Acceleration);
                    }
                }
            }
            this.launchElapsedTimeL += Time.deltaTime;
            if (this.QHold && this.currentGas > 0f)
            {
                this.useGas(this.useGasSpeed * Time.deltaTime);
            }
            else if (this.launchElapsedTimeL > 0.3f)
            {
                this.isLaunchLeft = false;
                if (this.bulletLeft != null)
                {
                    Bullet component = bulletLeft;
                    component.Disable();
                    this.releaseIfIHookSb();
                    this.bulletLeft = null;
                    flag2 = false;
                }
            }
        }
        if (this.isLaunchRight)
        {
            if (this.bulletRight != null && bulletRight.IsHooked())
            {
                this.isRightHandHooked = true;
                Vector3 vector4 = this.bulletRight.transform.position - baseT.position;
                vector4.Normalize();
                vector4 *= 10f;
                if (!this.isLaunchLeft)
                {
                    vector4 *= 2f;
                }
                if (Vector3.Angle(baseR.velocity, vector4) > 90f && InputManager.IsInput[InputCode.Gas])
                {
                    flag3 = true;
                    flag = true;
                }
                if (!flag3)
                {
                    baseR.AddForce(vector4);
                    if (Vector3.Angle(baseR.velocity, vector4) > 90f)
                    {
                        baseR.AddForce(-baseR.velocity * 2f, ForceMode.Acceleration);
                    }
                }
            }
            this.launchElapsedTimeR += Time.deltaTime;
            if (this.EHold && this.currentGas > 0f)
            {
                this.useGas(this.useGasSpeed * Time.deltaTime);
            }
            else if (this.launchElapsedTimeR > 0.3f)
            {
                this.isLaunchRight = false;
                if (this.bulletRight != null)
                {
                    Bullet component2 = bulletRight;
                    component2.Disable();
                    this.releaseIfIHookSb();
                    this.bulletRight = null;
                    flag3 = false;
                }
            }
        }
        if (this.grounded)
        {
            Vector3 a = Vectors.zero;
            if (this.State == HeroState.Attack)
            {
                if (this.attackAnimation == "attack5")
                {
                    if (baseA[this.attackAnimation].normalizedTime > 0.4f && baseA[this.attackAnimation].normalizedTime < 0.61f)
                    {
                        baseR.AddForce(baseGT.forward * 200f);
                    }
                }
                else if (this.attackAnimation == "special_petra")
                {
                    if (baseA[this.attackAnimation].normalizedTime > 0.35f && baseA[this.attackAnimation].normalizedTime < 0.48f)
                    {
                        baseR.AddForce(baseGT.forward * 200f);
                    }
                }
                else if (baseA.IsPlaying("attack3_2"))
                {
                    a = Vectors.zero;
                }
                else if (baseA.IsPlaying("attack1") || baseA.IsPlaying("attack2"))
                {
                    baseR.AddForce(baseGT.forward * 200f);
                }
                if (baseA.IsPlaying("attack3_2"))
                {
                    a = Vectors.zero;
                }
            }
            if (this.justGrounded)
            {
                if (this.State != HeroState.Attack || (!(this.attackAnimation == "attack3_1") && !(this.attackAnimation == "attack5") && !(this.attackAnimation == "special_petra")))
                {
                    if (this.State != HeroState.Attack && num == 0f && num2 == 0f && !this.bulletLeft && !this.bulletRight && this.State != HeroState.FillGas)
                    {
                        this.State = HeroState.Land;
                        this.crossFade("dash_land", 0.01f);
                    }
                    else
                    {
                        this.buttonAttackRelease = true;
                        if (this.State != HeroState.Attack && baseR.velocity.x * baseR.velocity.x + baseR.velocity.z * baseR.velocity.z > this.speed * this.speed * 1.5f && this.State != HeroState.FillGas)
                        {
                            this.State = HeroState.Slide;
                            this.crossFade("slide", 0.05f);
                            this.facingDirection = Mathf.Atan2(baseR.velocity.x, baseR.velocity.z) * 57.29578f;
                            this.targetRotation = Quaternion.Euler(0f, this.facingDirection, 0f);
                            this.sparks.enableEmission = true;
                        }
                    }
                }
                this.justGrounded = false;
                a = baseR.velocity;
            }
            if (this.State == HeroState.Attack && this.attackAnimation == "attack3_1" && baseA[this.attackAnimation].normalizedTime >= 1f)
            {
                this.playAnimation("attack3_2");
                this.resetAnimationSpeed();
                Vector3 zero = Vectors.zero;
                baseR.velocity = zero;
                a = zero;
                IN_GAME_MAIN_CAMERA.MainCamera.startShake(0.2f, 0.3f, 0.95f);
            }
            if (this.State == HeroState.GroundDodge)
            {
                if (baseA["dodge"].normalizedTime >= 0.2f && baseA["dodge"].normalizedTime < 0.8f)
                {
                    a = -baseT.Forward() * 2.4f * this.speed;
                }
                if (baseA["dodge"].normalizedTime > 0.8f)
                {
                    a = baseR.velocity;
                    a *= 0.9f;
                }
            }
            else if (this.State == HeroState.Idle)
            {
                Vector3 vector5 = new Vector3(num, 0f, num2);
                float num3 = this.getGlobalFacingDirection(num, num2);
                a = this.getGlobaleFacingVector3(num3);
                float d = (vector5.magnitude <= 0.95f) ? ((vector5.magnitude >= 0.25f) ? vector5.magnitude : 0f) : 1f;
                a *= d;
                a *= this.speed;
                if (this.buffTime > 0f && this.currentBuff == Buff.SpeedUp)
                {
                    a *= 4f;
                }
                if (num != 0f || num2 != 0f)
                {
                    if (!baseA.IsPlaying("run") && !baseA.IsPlaying("jump") && !baseA.IsPlaying("run_sasha") && (!baseA.IsPlaying("horse_geton") || baseA["horse_geton"].normalizedTime >= 0.5f))
                    {
                        if (this.buffTime > 0f && this.currentBuff == Buff.SpeedUp)
                        {
                            this.crossFade("run_sasha", 0.1f);
                        }
                        else
                        {
                            this.crossFade("run", 0.1f);
                        }
                    }
                }
                else
                {
                    if (!baseA.IsPlaying(this.standAnimation) && this.State != HeroState.Land && !baseA.IsPlaying("jump") && !baseA.IsPlaying("horse_geton") && !baseA.IsPlaying("grabbed"))
                    {
                        this.crossFade(this.standAnimation, 0.1f);
                        a *= 0f;
                    }
                    num3 = -874f;
                }
                if (num3 != -874f)
                {
                    this.facingDirection = num3;
                    this.targetRotation = Quaternion.Euler(0f, this.facingDirection, 0f);
                }
            }
            else if (this.State == HeroState.Land)
            {
                a = baseR.velocity;
                a *= 0.96f;
            }
            else if (this.State == HeroState.Slide)
            {
                a = baseR.velocity;
                a *= 0.99f;
                if (this.currentSpeed < this.speed * 1.2f)
                {
                    this.idle();
                    this.sparks.enableEmission = false;
                }
            }
            Vector3 velocity = baseR.velocity;
            Vector3 vector6 = a - velocity;
            vector6.x = Mathf.Clamp(vector6.x, -this.maxVelocityChange, this.maxVelocityChange);
            vector6.z = Mathf.Clamp(vector6.z, -this.maxVelocityChange, this.maxVelocityChange);
            vector6.y = 0f;
            if (baseA.IsPlaying("jump") && baseA["jump"].normalizedTime > 0.18f)
            {
                vector6.y += 8f;
            }
            if (baseA.IsPlaying("horse_geton") && baseA["horse_geton"].normalizedTime > 0.18f && baseA["horse_geton"].normalizedTime < 1f)
            {
                float num4 = 6f;
                vector6 = -baseR.velocity;
                vector6.y = num4;
                float num5 = Vector3.Distance(this.myHorse.transform.position, baseT.position);
                float d2 = 0.6f * this.gravity * num5 / (2f * num4);
                vector6 += d2 * (this.myHorse.transform.position - baseT.position).normalized;
            }
            if (this.State != HeroState.Attack || !this.Gunner)
            {
                baseR.AddForce(vector6, ForceMode.VelocityChange);
                baseR.rotation = Quaternion.Lerp(baseGT.rotation, Quaternion.Euler(0f, this.facingDirection, 0f), Time.deltaTime * 10f);
            }
        }
        else
        {
            if (this.sparks.enableEmission)
            {
                this.sparks.enableEmission = false;
            }
            if (this.myHorse != null && (baseA.IsPlaying("horse_geton") || baseA.IsPlaying("air_fall")) && baseR.velocity.y < 0f && Vector3.Distance(this.myHorse.transform.position + Vectors.up * 1.65f, baseT.position) < 0.5f)
            {
                baseT.position = this.myHorse.transform.position + Vectors.up * 1.65f;
                baseT.rotation = this.myHorse.transform.rotation;
                this.isMounted = true;
                this.crossFade("horse_idle", 0.1f);
                this.myHorse.GetComponent<Horse>().mounted();
            }
            if ((this.State == HeroState.Idle && !baseA.IsPlaying("dash") && !baseA.IsPlaying("wallrun") && !baseA.IsPlaying("toRoof") && !baseA.IsPlaying("horse_geton") && !baseA.IsPlaying("horse_getoff") && !baseA.IsPlaying("air_release") && !this.isMounted && (!baseA.IsPlaying("air_hook_l_just") || baseA["air_hook_l_just"].normalizedTime >= 1f) && (!baseA.IsPlaying("air_hook_r_just") || baseA["air_hook_r_just"].normalizedTime >= 1f)) || baseA["dash"].normalizedTime >= 0.99f)
            {
                if (!this.isLeftHandHooked && !this.isRightHandHooked && (baseA.IsPlaying("air_hook_l") || baseA.IsPlaying("air_hook_r") || baseA.IsPlaying("air_hook")) && baseR.velocity.y > 20f)
                {
                    baseA.CrossFade("air_release");
                }
                else
                {
                    bool flag4 = Mathf.Abs(baseR.velocity.x) + Mathf.Abs(baseR.velocity.z) > 25f;
                    bool flag5 = baseR.velocity.y < 0f;
                    if (!flag4)
                    {
                        if (flag5)
                        {
                            if (!baseA.IsPlaying("air_fall"))
                            {
                                this.crossFade("air_fall", 0.2f);
                            }
                        }
                        else if (!baseA.IsPlaying("air_rise"))
                        {
                            this.crossFade("air_rise", 0.2f);
                        }
                    }
                    else if (!this.isLeftHandHooked && !this.isRightHandHooked)
                    {
                        float cr = -Mathf.Atan2(baseR.velocity.z, baseR.velocity.x) * 57.29578f;
                        float num6 = -Mathf.DeltaAngle(cr, baseT.rotation.eulerAngles.y - 90f);
                        if (Mathf.Abs(num6) < 45f)
                        {
                            if (!baseA.IsPlaying("air2"))
                            {
                                this.crossFade("air2", 0.2f);
                            }
                        }
                        else if (num6 < 135f && num6 > 0f)
                        {
                            if (!baseA.IsPlaying("air2_right"))
                            {
                                this.crossFade("air2_right", 0.2f);
                            }
                        }
                        else if (num6 > -135f && num6 < 0f)
                        {
                            if (!baseA.IsPlaying("air2_left"))
                            {
                                this.crossFade("air2_left", 0.2f);
                            }
                        }
                        else if (!baseA.IsPlaying("air2_backward"))
                        {
                            this.crossFade("air2_backward", 0.2f);
                        }
                    }
                    else if (this.Gunner)
                    {
                        if (!this.isRightHandHooked)
                        {
                            if (!baseA.IsPlaying("AHSS_hook_forward_l"))
                            {
                                this.crossFade("AHSS_hook_forward_l", 0.1f);
                            }
                        }
                        else if (!this.isLeftHandHooked)
                        {
                            if (!baseA.IsPlaying("AHSS_hook_forward_r"))
                            {
                                this.crossFade("AHSS_hook_forward_r", 0.1f);
                            }
                        }
                        else if (!baseA.IsPlaying("AHSS_hook_forward_both"))
                        {
                            this.crossFade("AHSS_hook_forward_both", 0.1f);
                        }
                    }
                    else if (!this.isRightHandHooked)
                    {
                        if (!baseA.IsPlaying("air_hook_l"))
                        {
                            this.crossFade("air_hook_l", 0.1f);
                        }
                    }
                    else if (!this.isLeftHandHooked)
                    {
                        if (!baseA.IsPlaying("air_hook_r"))
                        {
                            this.crossFade("air_hook_r", 0.1f);
                        }
                    }
                    else if (!baseA.IsPlaying("air_hook"))
                    {
                        this.crossFade("air_hook", 0.1f);
                    }
                }
            }
            if (this.State == HeroState.Idle && baseA.IsPlaying("air_release") && baseA["air_release"].normalizedTime >= 1f)
            {
                this.crossFade("air_rise", 0.2f);
            }
            if (baseA.IsPlaying("horse_getoff") && baseA["horse_getoff"].normalizedTime >= 1f)
            {
                this.crossFade("air_rise", 0.2f);
            }
            if (baseA.IsPlaying("toRoof"))
            {
                if (baseA["toRoof"].normalizedTime < 0.22f)
                {
                    baseR.velocity = Vectors.zero;
                    baseR.AddForce(new Vector3(0f, this.gravity * baseR.mass, 0f));
                }
                else
                {
                    if (!this.wallJump)
                    {
                        this.wallJump = true;
                        baseR.AddForce(Vectors.up * 8f, ForceMode.Impulse);
                    }
                    baseR.AddForce(baseT.Forward() * 0.05f, ForceMode.Impulse);
                }
                if (baseA["toRoof"].normalizedTime >= 1f)
                {
                    this.playAnimation("air_rise");
                }
            }
            else if (this.State == HeroState.Idle && this.isPressDirectionTowardsHero(num, num2) && !InputManager.IsInput[InputCode.Gas] && !InputManager.IsInput[InputCode.LeftRope] && !InputManager.IsInput[InputCode.RightRope] && !InputManager.IsInput[InputCode.BothRope] && this.IsFrontGrounded() && !baseA.IsPlaying("wallrun") && !baseA.IsPlaying("dodge"))
            {
                this.crossFade("wallrun", 0.1f);
                this.wallRunTime = 0f;
            }
            else if (baseA.IsPlaying("wallrun"))
            {
                baseR.AddForce(Vectors.up * this.speed - baseR.velocity, ForceMode.VelocityChange);
                this.wallRunTime += Time.deltaTime;
                if (this.wallRunTime > 1f || (num2 == 0f && num == 0f))
                {
                    baseR.AddForce(-baseT.Forward() * this.speed * 0.75f, ForceMode.Impulse);
                    this.dodge(true);
                }
                else if (!this.IsUpFrontGrounded())
                {
                    this.wallJump = false;
                    this.crossFade("toRoof", 0.1f);
                }
                else if (!this.IsFrontGrounded())
                {
                    this.crossFade("air_fall", 0.1f);
                }
            }
            else if (!baseA.IsPlaying("attack5") && !baseA.IsPlaying("special_petra") && !baseA.IsPlaying("dash") && !baseA.IsPlaying("jump"))
            {
                Vector3 vector7 = new Vector3(num, 0f, num2);
                float num7 = this.getGlobalFacingDirection(num, num2);
                Vector3 vector8 = this.getGlobaleFacingVector3(num7);
                float d3 = (vector7.magnitude <= 0.95f) ? ((vector7.magnitude >= 0.25f) ? vector7.magnitude : 0f) : 1f;
                vector8 *= d3;
                vector8 *= (float)this.Setup.myCostume.stat.Acl / 10f * 2f;
                if (num == 0f && num2 == 0f)
                {
                    if (this.State == HeroState.Attack)
                    {
                        vector8 *= 0f;
                    }
                    num7 = -874f;
                }
                if (num7 != -874f)
                {
                    this.facingDirection = num7;
                    this.targetRotation = Quaternion.Euler(0f, this.facingDirection, 0f);
                }
                if (!flag2 && !flag3 && !this.isMounted && InputManager.IsInput[InputCode.Gas] && this.currentGas > 0f)
                {
                    if (num != 0f || num2 != 0f)
                    {
                        baseR.AddForce(vector8, ForceMode.Acceleration);
                    }
                    else
                    {
                        baseR.AddForce(baseT.Forward() * vector8.magnitude, ForceMode.Acceleration);
                    }
                    flag = true;
                }
            }
            if (baseA.IsPlaying("air_fall") && this.currentSpeed < 0.2f && this.IsFrontGrounded())
            {
                this.crossFade("onWall", 0.3f);
            }
        }
        Vector3 current = Vectors.zero;
        if (flag2 && flag3)
        {
            current = (this.bulletRight.baseT.position + this.bulletLeft.baseT.position) * 0.5f - this.baseT.position;
        }
        else if (flag2 && !flag3)
        {
            current = this.bulletLeft.baseT.position - this.baseT.position;
        }
        else if (flag3 && !flag2)
        {
            current = this.bulletRight.baseT.position - this.baseT.position;
        }
        if (flag2 || flag3)
        {
            this.baseR.AddForce(-this.baseR.velocity, ForceMode.VelocityChange);
            if (InputManager.IsInputRebindHolding((int)InputRebinds.ReelIn))
            {
                reelAxis = -1f;
            }
            else if (InputManager.IsInputRebindHolding((int)InputRebinds.ReelOut))
            {
                reelAxis = 1f;
            }
            else
            {
                reelAxis = Input.GetAxis("Mouse ScrollWheel") * 5555f;
            }
            float idk = 1.53938f * (1f + Mathf.Clamp(reelAxis, -0.8f, 0.8f));
            reelAxis = 0f;
            this.baseR.velocity = Vector3.RotateTowards(current, this.baseR.velocity, idk, idk).normalized * (this.currentSpeed + 0.1f);
        }
        if (this.State == HeroState.Attack && (this.attackAnimation == "attack5" || this.attackAnimation == "special_petra") && baseA[this.attackAnimation].normalizedTime > 0.4f && !this.attackMove)
        {
            this.attackMove = true;
            if (this.launchPointRight.magnitude > 0f)
            {
                Vector3 vector9 = this.launchPointRight - baseT.position;
                vector9.Normalize();
                vector9 *= 13f;
                baseR.AddForce(vector9, ForceMode.Impulse);
            }
            if (this.attackAnimation == "special_petra" && this.launchPointLeft.magnitude > 0f)
            {
                Vector3 vector10 = this.launchPointLeft - baseT.position;
                vector10.Normalize();
                vector10 *= 13f;
                baseR.AddForce(vector10, ForceMode.Impulse);
                if (this.bulletRight)
                {
                    bulletRight.Disable();
                    this.releaseIfIHookSb();
                }
                if (this.bulletLeft)
                {
                    bulletLeft.Disable();
                    this.releaseIfIHookSb();
                }
            }
            baseR.AddForce(Vectors.up * 2f, ForceMode.Impulse);
        }
        bool flag6 = false;
        if (this.bulletLeft != null || this.bulletRight != null)
        {
            if (this.bulletLeft && this.bulletLeft.transform.position.y > baseGT.position.y && this.isLaunchLeft && bulletLeft.IsHooked())
            {
                flag6 = true;
            }
            if (this.bulletRight && this.bulletRight.transform.position.y > baseGT.position.y && this.isLaunchRight && bulletRight.IsHooked())
            {
                flag6 = true;
            }
        }
        if (flag6)
        {
            baseR.AddForce(new Vector3(0f, -10f * baseR.mass, 0f));
        }
        else
        {
            baseR.AddForce(new Vector3(0f, -this.gravity * baseR.mass, 0f));
        }
        if (!Settings.StaticFOVEnabled)
        {
            if (this.currentSpeed > 10f)
            {
                IN_GAME_MAIN_CAMERA.BaseCamera.fieldOfView = Mathf.Lerp(IN_GAME_MAIN_CAMERA.BaseCamera.fieldOfView, Mathf.Min(100f, this.currentSpeed + 40f), 0.1f);
            }
            else
            {
                IN_GAME_MAIN_CAMERA.BaseCamera.fieldOfView = Mathf.Lerp(IN_GAME_MAIN_CAMERA.BaseCamera.fieldOfView, 50f, 0.1f);
            }
        }
        else
        {
            IN_GAME_MAIN_CAMERA.BaseCamera.fieldOfView = Settings.StaticFOV.Value;
        }
        if (flag)
        {
            this.useGas(this.useGasSpeed * Time.deltaTime);
            if (!this.smoke_3dmg.enableEmission && IN_GAME_MAIN_CAMERA.GameType != GameType.Single && BasePV.IsMine)
            {
                BasePV.RPC("net3DMGSMOKE", PhotonTargets.Others, new object[]
                {
                    true
                });
            }
            this.smoke_3dmg.enableEmission = true;
        }
        else
        {
            if (this.smoke_3dmg.enableEmission && IN_GAME_MAIN_CAMERA.GameType != GameType.Single && BasePV.IsMine)
            {
                BasePV.RPC("net3DMGSMOKE", PhotonTargets.Others, new object[]
                {
                    false
                });
            }
            this.smoke_3dmg.enableEmission = false;
        }
        if (VideoSettings.WindEffect.Value)
        {
            if (this.currentSpeed > 80f)
            {
                if (!this.speedFXPS.enableEmission)
                {
                    this.speedFXPS.enableEmission = true;
                }
                this.speedFXPS.startSpeed = this.currentSpeed;
                this.speedFX.transform.LookAt(baseT.position + baseR.velocity);
            }
            else if (this.speedFXPS.enableEmission)
            {
                this.speedFXPS.enableEmission = false;
            }
        }
        this.setHookedPplDirection();
        this.bodyLean();
        if (!baseA.IsPlaying("attack3_2") && !baseA.IsPlaying("attack5") && !baseA.IsPlaying("special_petra"))
        {
            baseR.rotation = Quaternion.Lerp(baseGT.rotation, this.targetRotation, 0.02f * 6f);
        }
    }

    private Vector3 getGlobaleFacingVector3(float horizontal, float vertical)
    {
        float num = -this.getGlobalFacingDirection(horizontal, vertical) + 90f;
        float x = Mathf.Cos(num * 0.0174532924f);
        float z = Mathf.Sin(num * 0.0174532924f);
        return new Vector3(x, 0f, z);
    }

    private Vector3 getGlobaleFacingVector3(float resultAngle)
    {
        float num = -resultAngle + 90f;
        float x = Mathf.Cos(num * 0.0174532924f);
        float z = Mathf.Sin(num * 0.0174532924f);
        return new Vector3(x, 0f, z);
    }

    private float getGlobalFacingDirection(float horizontal, float vertical)
    {
        if (vertical == 0f && horizontal == 0f)
        {
            return baseT.rotation.eulerAngles.y;
        }
        float y = IN_GAME_MAIN_CAMERA.MainCamera.transform.rotation.eulerAngles.y;
        float num = Mathf.Atan2(vertical, horizontal) * 57.29578f;
        num = -num + 90f;
        return y + num;
    }

    private float getLeanAngle(Vector3 p, bool left)
    {
        if (!this.Gunner && this.State == HeroState.Attack)
        {
            return 0f;
        }
        float num = p.y - baseT.position.y;
        float num2 = Vector3.Distance(p, baseT.position);
        float num3 = Mathf.Acos(num / num2) * 57.29578f;
        num3 *= 0.1f;
        num3 *= 1f + Mathf.Pow(baseR.velocity.magnitude, 0.2f);
        Vector3 vector = p - baseT.position;
        float current = Mathf.Atan2(vector.x, vector.z) * 57.29578f;
        float target = Mathf.Atan2(baseR.velocity.x, baseR.velocity.z) * 57.29578f;
        float num4 = Mathf.DeltaAngle(current, target);
        num3 += Mathf.Abs(num4 * 0.5f);
        if (this.State != HeroState.Attack)
        {
            num3 = Mathf.Min(num3, 80f);
        }
        if (num4 > 0f)
        {
            this.leanLeft = true;
        }
        else
        {
            this.leanLeft = false;
        }
        if (this.Gunner)
        {
            return num3 * (float)((num4 >= 0f) ? 1 : -1);
        }
        float num5;
        if ((left && num4 < 0f) || (!left && num4 > 0f))
        {
            num5 = 0.1f;
        }
        else
        {
            num5 = 0.5f;
        }
        return num3 * ((num4 >= 0f) ? num5 : (-num5));
    }

    private void getOffHorse()
    {
        this.playAnimation("horse_getoff");
        baseR.AddForce(Vectors.up * 10f - baseT.Forward() * 2f - baseT.Right() * 1f, ForceMode.VelocityChange);
        this.unmounted();
    }

    private void getOnHorse()
    {
        this.playAnimation("horse_geton");
        this.facingDirection = this.myHorse.transform.rotation.eulerAngles.y;
        this.targetRotation = Quaternion.Euler(0f, this.facingDirection, 0f);
    }

    private void headMovement()
    {
        float x = Mathf.Sqrt((this.gunTarget.x - baseT.position.x) * (this.gunTarget.x - baseT.position.x) + (this.gunTarget.z - baseT.position.z) * (this.gunTarget.z - baseT.position.z));
        this.targetHeadRotation = Head.rotation;
        Vector3 vector = this.gunTarget - baseT.position;
        float current = -Mathf.Atan2(vector.z, vector.x) * 57.29578f;
        float num = -Mathf.DeltaAngle(current, baseT.rotation.eulerAngles.y - 90f);
        num = Mathf.Clamp(num, -40f, 40f);
        float y = Neck.position.y - this.gunTarget.y;
        float num2 = Mathf.Atan2(y, x) * 57.29578f;
        num2 = Mathf.Clamp(num2, -40f, 30f);
        this.targetHeadRotation = Quaternion.Euler(Head.rotation.eulerAngles.x + num2, Head.rotation.eulerAngles.y + num, Head.rotation.eulerAngles.z);
        this.oldHeadRotation = Quaternion.Lerp(this.oldHeadRotation, this.targetHeadRotation, Time.deltaTime * 60f);
        Head.rotation = this.oldHeadRotation;
    }

    private void idle()
    {
        if (this.State == HeroState.Attack)
        {
            this.falseAttack();
        }
        this.State = HeroState.Idle;
        this.crossFade(this.standAnimation, 0.1f);
    }

    private bool IsFrontGrounded()
    {
        return Physics.Raycast(baseGT.position + baseGT.up * 1f, baseGT.forward, 1f, Layers.EnemyGround.value);
    }

    private bool isPressDirectionTowardsHero(float h, float v)
    {
        if (h == 0f && v == 0f)
        {
            return false;
        }
        float globalFacingDirection = this.getGlobalFacingDirection(h, v);
        return Mathf.Abs(Mathf.DeltaAngle(globalFacingDirection, baseT.rotation.eulerAngles.y)) < 45f;
    }

    private bool IsUpFrontGrounded()
    {
        return Physics.Raycast(baseGT.position + baseGT.up * 3f, baseGT.forward, 1.2f, Layers.EnemyGround.value);
    }

    [RPC]
    private void killObject(PhotonMessageInfo info)
    {
        Anarchy.UI.Log.AddLineRaw($"HERO.killObjectRPC by ID {info.Sender.ID}", Anarchy.UI.MsgType.Warning);
    }

    private void launchLeftRope(RaycastHit hit, bool single, int mode = 0)
    {
        if (this.currentGas == 0f)
        {
            return;
        }
        this.useGas(0f);
        if (IN_GAME_MAIN_CAMERA.GameType == GameType.Single)
        {
            this.bulletLeft = ((GameObject)UnityEngine.Object.Instantiate(CacheResources.Load("hook"))).GetComponent<Bullet>();
        }
        else if (BasePV.IsMine)
        {
            this.bulletLeft = Optimization.Caching.Pool.NetworkEnable("hook", baseT.position, baseT.rotation, 0).GetComponent<Bullet>();
        }
        GameObject gameObject = (!this.Gunner) ? this.hookRefL1 : this.hookRefL2;
        string launcher_ref = (!this.Gunner) ? "hookRefL1" : "hookRefL2";
        this.bulletLeft.transform.position = gameObject.transform.position;
        Bullet component = bulletLeft;
        float d = (!single) ? ((hit.distance <= 50f) ? (hit.distance * 0.05f) : (hit.distance * 0.3f)) : 0f;
        Vector3 a = hit.point - baseT.Right() * d - this.bulletLeft.transform.position;
        a.Normalize();
        if (mode == 1)
        {
            component.Launch(a * 3f, baseR.velocity, launcher_ref, true, this, true);
        }
        else
        {
            component.Launch(a * 3f, baseR.velocity, launcher_ref, true, this, false);
        }
        this.launchPointLeft = Vectors.zero;
    }

    private void launchRightRope(RaycastHit hit, bool single, int mode = 0)
    {
        if (this.currentGas == 0f)
        {
            return;
        }
        this.useGas(0f);
        if (IN_GAME_MAIN_CAMERA.GameType == GameType.Single)
        {
            this.bulletRight = ((GameObject)UnityEngine.Object.Instantiate(CacheResources.Load("hook"))).GetComponent<Bullet>();
        }
        else if (BasePV.IsMine)
        {
            this.bulletRight = Optimization.Caching.Pool.NetworkEnable("hook", baseT.position, baseT.rotation, 0).GetComponent<Bullet>();
        }
        GameObject gameObject = (!this.Gunner) ? this.hookRefR1 : this.hookRefR2;
        string launcher_ref = (!this.Gunner) ? "hookRefR1" : "hookRefR2";
        this.bulletRight.transform.position = gameObject.transform.position;
        Bullet component = bulletRight;
        float d = (!single) ? ((hit.distance <= 50f) ? (hit.distance * 0.05f) : (hit.distance * 0.3f)) : 0f;
        Vector3 a = hit.point + baseT.Right() * d - this.bulletRight.transform.position;
        a.Normalize();
        if (mode == 1)
        {
            component.Launch(a * 5f, baseR.velocity, launcher_ref, false, this, true);
        }
        else
        {
            component.Launch(a * 3f, baseR.velocity, launcher_ref, false, this, false);
        }
        this.launchPointRight = Vectors.zero;
    }

    private System.Collections.IEnumerator loadskinRPCE(int horse, string urls, PhotonMessageInfo info = null)
    {
        while (!spawned)
        {
            yield return null;
        }
        if (Skin != null)
        {
            Debug.Log("loadskinRPC(HERO) but skin is already loaded");
            yield break;
        }
        SkinData = urls.Split(',');
        try
        {
            Skin = new HumanSkin(this, SkinData);
        }
        catch
        {
            Debug.Log($"Could not load skin for ID: {info.Sender.ID}, HERO skin is null");
            yield break;
        }
        if (SkinData != null && Skin.NeedReload(SkinData))
        {
            yield return StartCoroutine(Skin.Reload(SkinData));
        }
        Skin.Apply();
    }

    private void leftArmAimTo(Vector3 target)
    {
        float num = target.x - this.Upper_Arm_L.position.x;
        float y = target.y - this.Upper_Arm_L.position.y;
        float num2 = target.z - this.Upper_Arm_L.position.z;
        float x = Mathf.Sqrt(num * num + num2 * num2);
        this.Hand_L.localRotation = Quaternion.Euler(90f, 0f, 0f);
        this.Forearm_L.localRotation = Quaternion.Euler(-90f, 0f, 0f);
        this.Upper_Arm_L.rotation = Quaternion.Euler(0f, 90f + Mathf.Atan2(num, num2) * 57.29578f, -Mathf.Atan2(y, x) * 57.29578f);
    }

    [RPC]
    private void net3DMGSMOKE(bool ifON)
    {
        if (this.smoke_3dmg != null)
        {
            this.smoke_3dmg.enableEmission = ifON;
        }
    }

    [RPC]
    private void netContinueAnimation()
    {
        foreach (object obj in baseA)
        {
            AnimationState animationState = (AnimationState)obj;
            if (animationState.speed == 1f)
            {
                return;
            }
            animationState.speed = 1f;
        }
        this.playAnimation(this.currentPlayingClipName());
    }

    [RPC]
    private void netCrossFade(string aniName, float time)
    {
        this.currentAnimation = aniName;
        if (baseA != null)
        {
            baseA.CrossFade(aniName, time);
        }
    }

    [RPC]
    private void netDie2(int viewID = -1, string titanName = "", PhotonMessageInfo info = null)
    {
        if (BasePV.IsMine)
        {
            if (myBomb != null)
            {
                myBomb.destroyMe();
            }
            if (this.myCannon != null)
            {
                PhotonNetwork.Destroy(this.myCannon);
            }
            PhotonNetwork.RemoveRPCs(BasePV);
            if (this.titanForm && this.eren != null)
            {
                this.eren.GetComponent<TITAN_EREN>().lifeTime = 0.1f;
            }
            if (skillCD != null)
            {
                skillCD.transform.localPosition = Vectors.up * 5000f;
            }
        }
        this.meatDie.Play();
        if (this.bulletLeft != null)
        {
            bulletLeft.RemoveMe();
        }
        if (this.bulletRight != null)
        {
            bulletRight.RemoveMe();
        }
        Transform audioTf = baseT.Find("audio_die");
        if (audioTf != null)
        {
            audioTf.parent = null;
            audioTf.GetComponent<AudioSource>().Play();
        }
        if (BasePV.IsMine)
        {
            IN_GAME_MAIN_CAMERA.MainCamera.SetMainObject(null, true, false);
            IN_GAME_MAIN_CAMERA.MainCamera.setSpectorMode(true);
            IN_GAME_MAIN_CAMERA.MainCamera.gameOver = true;
            FengGameManagerMKII.FGM.Logic.MyRespawnTime = 0f;
        }
        this.falseAttack();
        this.hasDied = true;
        baseG.GetComponent<SmoothSyncMovement>().Disabled = true;
        if (IN_GAME_MAIN_CAMERA.GameType == GameType.Multi && BasePV.IsMine)
        {
            PhotonNetwork.RemoveRPCs(BasePV);
            PhotonNetwork.player.Dead = true;
            PhotonNetwork.player.Deaths++;
            if (viewID != -1)
            {
                PhotonView photonView = PhotonView.Find(viewID);
                if (photonView != null)
                {
                    FengGameManagerMKII.FGM.SendKillInfo(true, User.DeathFormat(info.Sender.ID, info.Sender.UIName) + " ", false, User.DeathName, 0);
                    photonView.owner.Kills++;
                }
            }
            else
            {
                FengGameManagerMKII.FGM.SendKillInfo(true, User.DeathFormat(info.Sender.ID, titanName) + " ", false, User.DeathName, 0);
            }
            FengGameManagerMKII.FGM.BasePV.RPC("someOneIsDead", PhotonTargets.MasterClient, new object[] { titanName != string.Empty ? 1 : 0 });
        }
        if (IN_GAME_MAIN_CAMERA.GameType == GameType.Multi && BasePV.IsMine)
        {
            Pool.NetworkEnable("hitMeat2", baseT.position, Quaternion.Euler(270f, 0f, 0f), 0);
        }
        else
        {
            Pool.Enable("hitMeat2", baseT.position, Quaternion.identity);
        }
        if (PhotonNetwork.IsMasterClient)
        {
            onDeathEvent(viewID, true);
            if (RCManager.heroHash.ContainsKey(viewID))
                RCManager.heroHash.Remove(viewID);
        }
        if (BasePV.IsMine)
        {
            PhotonNetwork.Destroy(BasePV);
        }
    }

    [RPC]
    private void netGrabbed(int id, bool leftHand)
    {
        this.titanWhoGrabMeID = id;
        this.grabbed(PhotonView.Find(id).gameObject, leftHand);
    }

    [RPC]
    private void netlaughAttack()
    {
        GameObject[] array = GameObject.FindGameObjectsWithTag("titan");
        foreach (GameObject gameObject in array)
        {
            if (Vector3.Distance(gameObject.transform.position, baseT.position) < 50f && Vector3.Angle(gameObject.transform.Forward(), baseT.position - gameObject.transform.position) < 90f && gameObject.GetComponent<TITAN>())
            {
                gameObject.GetComponent<TITAN>().beLaughAttacked();
            }
        }
    }

    [RPC]
    private void netPauseAnimation()
    {
        foreach (object obj in baseA)
        {
            AnimationState animationState = (AnimationState)obj;
            animationState.speed = 0f;
        }
    }

    [RPC]
    private void netPlayAnimation(string aniName)
    {
        this.currentAnimation = aniName;
        if (baseA != null)
        {
            baseA.Play(aniName);
        }
    }

    [RPC]
    private void netPlayAnimationAt(string aniName, float normalizedTime)
    {
        this.currentAnimation = aniName;
        if (baseA != null)
        {
            baseA.Play(aniName);
            baseA[aniName].normalizedTime = normalizedTime;
        }
    }

    [RPC]
    private void netSetIsGrabbedFalse()
    {
        this.State = HeroState.Idle;
    }

    [RPC]
    private void netTauntAttack(float tauntTime, float distance = 100f)
    {
        GameObject[] array = GameObject.FindGameObjectsWithTag("titan");
        foreach (GameObject gameObject in array)
        {
            if (Vector3.Distance(gameObject.transform.position, baseT.position) < distance && gameObject.GetComponent<TITAN>())
            {
                gameObject.GetComponent<TITAN>().beTauntedBy(baseG, tauntTime);
            }
        }
    }

    [RPC]
    private void netUngrabbed()
    {
        this.ungrabbed();
        this.netPlayAnimation(this.standAnimation);
        this.falseAttack();
    }

    public void onDeathEvent(int viewID, bool isTitan)
    {
        if (PhotonNetwork.player.IsMasterClient)
        {
            if (BasePV != null && BasePV.owner != null)
            {
                GameModes.EndlessMode(BasePV.owner.ID);
            }
        }
        if (isTitan)
        {
            if (RCManager.RCEvents.ContainsKey("OnPlayerDieByTitan"))
            {
                RCEvent event2 = (RCEvent)RCManager.RCEvents["OnPlayerDieByTitan"];
                string[] strArray = (string[])RCManager.RCVariableNames["OnPlayerDieByTitan"];
                if (RCManager.playerVariables.ContainsKey(strArray[0]))
                {
                    RCManager.playerVariables[strArray[0]] = BasePV.owner;
                }
                else
                {
                    RCManager.playerVariables.Add(strArray[0], BasePV.owner);
                }
                if (RCManager.titanVariables.ContainsKey(strArray[1]))
                {
                    RCManager.titanVariables[strArray[1]] = PhotonView.Find(viewID).gameObject.GetComponent<TITAN>();
                }
                else
                {
                    RCManager.titanVariables.Add(strArray[1], PhotonView.Find(viewID).gameObject.GetComponent<TITAN>());
                }
                event2.checkEvent();
            }
        }
        else if (RCManager.RCEvents.ContainsKey("OnPlayerDieByPlayer"))
        {
            RCEvent event2 = (RCEvent)RCManager.RCEvents["OnPlayerDieByPlayer"];
            string[] strArray = (string[])RCManager.RCVariableNames["OnPlayerDieByPlayer"];
            if (RCManager.playerVariables.ContainsKey(strArray[0]))
            {
                RCManager.playerVariables[strArray[0]] = BasePV.owner;
            }
            else
            {
                RCManager.playerVariables.Add(strArray[0], BasePV.owner);
            }
            if (RCManager.playerVariables.ContainsKey(strArray[1]))
            {
                RCManager.playerVariables[strArray[1]] = PhotonView.Find(viewID).owner;
            }
            else
            {
                RCManager.playerVariables.Add(strArray[1], PhotonView.Find(viewID).owner);
            }
            event2.checkEvent();
        }
    }

    private void OnDestroy()
    {
        if (FengGameManagerMKII.FGM != null)
        {
            FengGameManagerMKII.FGM.RemoveHero(this);
        }
        if (this.myNetWorkName != null)
        {
            Destroy(this.myNetWorkName);
        }
        if (this.gunDummy != null)
        {
            Destroy(this.gunDummy);
        }
        if (IN_GAME_MAIN_CAMERA.GameType == GameType.Multi)
        {
            this.releaseIfIHookSb();
        }
        if (this.Setup.part_cape != null)
        {
            ClothFactory.DisposeObject(this.Setup.part_cape);
        }
        if (this.Setup.part_hair_1 != null)
        {
            ClothFactory.DisposeObject(this.Setup.part_hair_1);
        }
        if (this.Setup.part_hair_2 != null)
        {
            ClothFactory.DisposeObject(this.Setup.part_hair_2);
        }
        if (IN_GAME_MAIN_CAMERA.GameType == GameType.Multi && BasePV != null)
        {
            GameModes.InfectionOnDeath(BasePV.owner);
        }
    }

    private void playAnimationAt(string aniName, float normalizedTime)
    {
        this.currentAnimation = aniName;
        baseA.Play(aniName);
        baseA[aniName].normalizedTime = normalizedTime;
        if (!PhotonNetwork.connected)
        {
            return;
        }
        if (BasePV.IsMine)
        {
            BasePV.RPC("netPlayAnimationAt", PhotonTargets.Others, new object[]
            {
                aniName,
                normalizedTime
            });
        }
    }

    private void releaseIfIHookSb()
    {
        if (this.hookSomeOne && this.hookTarget != null)
        {
            this.hookTarget.GetPhotonView().RPC("badGuyReleaseMe", this.hookTarget.GetPhotonView().owner, new object[0]);
            this.hookTarget = null;
            this.hookSomeOne = false;
        }
    }

    private void rightArmAimTo(Vector3 target)
    {
        float num = target.x - this.Upper_Arm_R.position.x;
        float y = target.y - this.Upper_Arm_R.position.y;
        float num2 = target.z - this.Upper_Arm_R.position.z;
        float x = Mathf.Sqrt(num * num + num2 * num2);
        this.Hand_R.localRotation = Quaternion.Euler(-90f, 0f, 0f);
        this.Forearm_R.localRotation = Quaternion.Euler(90f, 0f, 0f);
        this.Upper_Arm_R.rotation = Quaternion.Euler(180f, 90f + Mathf.Atan2(num, num2) * 57.29578f, Mathf.Atan2(y, x) * 57.29578f);
    }

    [RPC]
    private void RPCHookedByHuman(int hooker, Vector3 hookPosition)
    {
        this.hookBySomeOne = true;
        this.badGuy = PhotonView.Find(hooker).gameObject;
        if (Vector3.Distance(hookPosition, baseT.position) < 15f)
        {
            this.launchForce = PhotonView.Find(hooker).gameObject.transform.position - baseT.position;
            baseR.AddForce(-baseR.velocity * 0.9f, ForceMode.VelocityChange);
            float d = Mathf.Pow(this.launchForce.magnitude, 0.1f);
            if (this.grounded)
            {
                baseR.AddForce(Vectors.up * Mathf.Min(this.launchForce.magnitude * 0.2f, 10f), ForceMode.Impulse);
            }
            baseR.AddForce(this.launchForce * d * 0.1f, ForceMode.Impulse);
            if (this.State != HeroState.Grab)
            {
                this.dashTime = 1f;
                this.crossFade("dash", 0.05f);
                baseA["dash"].time = 0.1f;
                this.State = HeroState.AirDodge;
                this.falseAttack();
                this.facingDirection = Mathf.Atan2(this.launchForce.x, this.launchForce.z) * 57.29578f;
                Quaternion quaternion = Quaternion.Euler(0f, this.facingDirection, 0f);
                baseGT.rotation = quaternion;
                baseR.rotation = quaternion;
                this.targetRotation = quaternion;
            }
        }
        else
        {
            this.hookBySomeOne = false;
            this.badGuy = null;
            PhotonView.Find(hooker).RPC("hookFail", PhotonView.Find(hooker).owner, new object[0]);
        }
    }

    private void salute()
    {
        this.State = HeroState.Salute;
        this.crossFade("salute", 0.1f);
    }

    private void setHookedPplDirection()
    {
        this.almostSingleHook = false;
        if (this.isRightHandHooked && this.isLeftHandHooked)
        {
            if (this.bulletLeft != null && this.bulletRight != null)
            {
                Vector3 vector = this.bulletLeft.transform.position - this.bulletRight.transform.position;
                if (vector.sqrMagnitude < 4f)
                {
                    Vector3 vector2 = (this.bulletLeft.transform.position + this.bulletRight.transform.position) * 0.5f - baseT.position;
                    this.facingDirection = Mathf.Atan2(vector2.x, vector2.z) * 57.29578f;
                    if (this.Gunner && this.State != HeroState.Attack)
                    {
                        float current = -Mathf.Atan2(baseR.velocity.z, baseR.velocity.x) * 57.29578f;
                        float target = -Mathf.Atan2(vector2.z, vector2.x) * 57.29578f;
                        float num = -Mathf.DeltaAngle(current, target);
                        this.facingDirection += num;
                    }
                    this.almostSingleHook = true;
                }
                else
                {
                    Vector3 to = baseT.position - this.bulletLeft.transform.position;
                    Vector3 to2 = baseT.position - this.bulletRight.transform.position;
                    Vector3 vector3 = (this.bulletLeft.transform.position + this.bulletRight.transform.position) * 0.5f;
                    Vector3 from = baseT.position - vector3;
                    if (Vector3.Angle(from, to) < 30f && Vector3.Angle(from, to2) < 30f)
                    {
                        this.almostSingleHook = true;
                        Vector3 vector4 = vector3 - baseT.position;
                        this.facingDirection = Mathf.Atan2(vector4.x, vector4.z) * 57.29578f;
                    }
                    else
                    {
                        this.almostSingleHook = false;
                        Vector3 forward = baseT.Forward();
                        Vector3.OrthoNormalize(ref vector, ref forward);
                        this.facingDirection = Mathf.Atan2(forward.x, forward.z) * 57.29578f;
                        float current2 = Mathf.Atan2(to.x, to.z) * 57.29578f;
                        float num2 = Mathf.DeltaAngle(current2, this.facingDirection);
                        if (num2 > 0f)
                        {
                            this.facingDirection += 180f;
                        }
                    }
                }
            }
        }
        else
        {
            this.almostSingleHook = true;
            Vector3 vector5 = Vectors.zero;
            if (this.isRightHandHooked && this.bulletRight != null)
            {
                vector5 = this.bulletRight.transform.position - baseT.position;
            }
            else
            {
                if (!this.isLeftHandHooked || !(this.bulletLeft != null))
                {
                    return;
                }
                vector5 = this.bulletLeft.transform.position - baseT.position;
            }
            this.facingDirection = Mathf.Atan2(vector5.x, vector5.z) * 57.29578f;
            if (this.State != HeroState.Attack)
            {
                float current3 = -Mathf.Atan2(baseR.velocity.z, baseR.velocity.x) * 57.29578f;
                float target2 = -Mathf.Atan2(vector5.z, vector5.x) * 57.29578f;
                float num3 = -Mathf.DeltaAngle(current3, target2);
                if (this.Gunner)
                {
                    this.facingDirection += num3;
                }
                else
                {
                    float num4;
                    if ((this.isLeftHandHooked && num3 < 0f) || (this.isRightHandHooked && num3 > 0f))
                    {
                        num4 = -0.1f;
                    }
                    else
                    {
                        num4 = 0.1f;
                    }
                    this.facingDirection += num3 * num4;
                }
            }
        }
    }

    [RPC]
    private void setMyTeam(int val)
    {
        this.myTeam = val;
        if (IsLocal)
        {
            this.wLeft.myTeam = val;
            this.wRight.myTeam = val;
        }
        if (IN_GAME_MAIN_CAMERA.GameType == GameType.Multi && PhotonNetwork.IsMasterClient)
        {
            if (GameModes.FriendlyMode.Enabled)
            {
                if (val != 1)
                {
                    BasePV.RPC("setMyTeam", PhotonTargets.AllBuffered, new object[] { 1 });
                }
            }
            else
            {
                if (GameModes.BladePVP.Enabled)
                {
                    int team = 0;
                    switch (GameModes.BladePVP.Selection)
                    {
                        case 1:
                            team = BasePV.owner.RCteam;
                            break;

                        case 2:
                            team = BasePV.owner.ID;
                            break;
                    }
                    if (val == team)
                    {
                        return;
                    }
                    BasePV.RPC("setMyTeam", PhotonTargets.AllBuffered, new object[] { (int)team });
                }
            }
        }
    }

    private void ShowAimUI()
    {
        if (Screen.showCursor)
        {
            Vector3 localPosition = Vectors.up * 10000f;
            this.crossT1.localPosition = localPosition;
            this.crossT2.localPosition = localPosition;
            this.crossR2T.localPosition = localPosition;
            this.crossR1T.localPosition = localPosition;
            this.crossL2T.localPosition = localPosition;
            this.crossL1T.localPosition = localPosition;
            this.labelT.localPosition = localPosition;
            return;
        }
        Vector3 position = Input.mousePosition;
        Ray ray = IN_GAME_MAIN_CAMERA.BaseCamera.ScreenPointToRay(position);
        if (Physics.Raycast(ray, out RaycastHit raycastHit, 1E+07f, Layers.EnemyGround.value))
        {
            this.crossT1.localPosition = position;
            this.crossT1.localPosition -= new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0f);
            this.crossT2.localPosition = this.crossT1.localPosition;
            float magnitude = (raycastHit.point - this.baseT.position).magnitude;
            string text = (magnitude <= 1000f) ? ((int)magnitude).ToString() : "???";
            if (Settings.Speedometer.Value)
            {
                if (Settings.SpeedometerType.Value == 0)
                {
                    text += "\n" + baseR.velocity.magnitude.ToString("F0") + " u/s";
                }
                else
                {
                    text += "\n" + (baseR.velocity.magnitude / 100f).ToString("F1") + " k";
                }
            }
            this.LabelDistance.text = text;
            if (magnitude > 120f)
            {
                this.crossT1.localPosition += Vectors.up * 10000f;
                this.labelT.localPosition = this.crossT2.localPosition;
            }
            else
            {
                this.crossT2.localPosition += Vectors.up * 10000f;
                this.labelT.localPosition = this.crossT1.localPosition;
            }
            this.labelT.localPosition -= new Vector3(0f, 15f, 0f);
            Vector3 vector = new Vector3(0f, 0.4f, 0f);
            vector -= this.baseT.Right() * 0.3f;
            Vector3 vector2 = new Vector3(0f, 0.4f, 0f);
            vector2 += this.baseT.Right() * 0.3f;
            float d = (raycastHit.distance <= 50f) ? (raycastHit.distance * 0.05f) : (raycastHit.distance * 0.3f);
            Vector3 vector3 = raycastHit.point - this.baseT.Right() * d - (this.baseT.position + vector);
            Vector3 vector4 = raycastHit.point + this.baseT.Right() * d - (this.baseT.position + vector2);
            vector3.Normalize();
            vector4.Normalize();
            vector3 *= 1000000f;
            vector4 *= 1000000f;
            if (Physics.Linecast(this.baseT.position + vector, this.baseT.position + vector + vector3, out raycastHit, Layers.EnemyGround.value))
            {
                this.crossL1T.localPosition = IN_GAME_MAIN_CAMERA.BaseCamera.WorldToScreenPoint(raycastHit.point);
                this.crossL1T.localPosition -= new Vector3((float)Screen.width * 0.5f, (float)Screen.height * 0.5f, 0f);
                this.crossL1T.localRotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(this.crossL1T.localPosition.y - (position.y - (float)Screen.height * 0.5f), this.crossL1T.localPosition.x - (position.x - (float)Screen.width * 0.5f)) * 57.29578f + 180f);
                this.crossL2T.localPosition = this.crossL1T.localPosition;
                this.crossL2T.localRotation = this.crossL1T.localRotation;
                (raycastHit.distance > 120f ? crossL1T : crossL2T).localPosition += Vectors.up * 10000f;
            }
            if (Physics.Linecast(this.baseT.position + vector2, this.baseT.position + vector2 + vector4, out raycastHit, Layers.EnemyGround.value))
            {
                this.crossR1T.localPosition = IN_GAME_MAIN_CAMERA.BaseCamera.WorldToScreenPoint(raycastHit.point);
                this.crossR1T.localPosition -= new Vector3((float)Screen.width * 0.5f, (float)Screen.height * 0.5f, 0f);
                this.crossR1T.localRotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(this.crossR1T.localPosition.y - (position.y - Screen.height * 0.5f), this.crossR1T.localPosition.x - (position.x - Screen.width * 0.5f)) * 57.29578f);
                this.crossR2T.localPosition = this.crossR1T.localPosition;
                this.crossR2T.localRotation = this.crossR1T.localRotation;
                (raycastHit.distance > 120f ? crossR1T : crossR2T).localPosition += Vectors.up * 10000f;
            }
        }
    }

    private void ShowBlades()
    {
        float num = this.currentBladeSta / this.totalBladeSta;
        SpriteCache.Find("bladeCL").fillAmount = SpriteCache.Find("bladeCR").fillAmount = num;
        if (num <= 0f)
        {
            SpriteCache.Find("bladel1").color = SpriteCache.Find("blader1").color = Colors.red;
        }
        else if (num <= 0.20f)
        {
            SpriteCache.Find("bladel1").color = SpriteCache.Find("blader1").color = Colors.orange;
        }
        else if (num < 0.40f)
        {
            SpriteCache.Find("bladel1").color = SpriteCache.Find("blader1").color = Colors.yellow;
        }
        else
        {
            SpriteCache.Find("bladel1").color = SpriteCache.Find("blader1").color = Colors.white;
        }
        for (int i = 5; i > 0; i--)
        {
            if (currentBladeNum < i)
            {
                SpriteCache.Find("bladel" + i).enabled = false;
                SpriteCache.Find("blader" + i).enabled = false;
            }
            else
            {

                SpriteCache.Find("bladel" + i).enabled = true;
                SpriteCache.Find("blader" + i).enabled = true;
            }
        }
    }

    private void ShowBullets()
    {
        UpdateLeftMagUI();
        UpdateRightMagUI();
        SpriteCache.Find("bulletL").enabled = leftGunHasBullet;
        SpriteCache.Find("bulletR").enabled = rightGunHasBullet;
    }

    private void ShowFlareCD()
    {
        if (CacheGameObject.Find("UIflare1") != null)
        {
            SpriteCache.Find("UIflare1").fillAmount = (this.flareTotalCD - this.flare1CD) / this.flareTotalCD;
            SpriteCache.Find("UIflare2").fillAmount = (this.flareTotalCD - this.flare2CD) / this.flareTotalCD;
            SpriteCache.Find("UIflare3").fillAmount = (this.flareTotalCD - this.flare3CD) / this.flareTotalCD;
        }
    }

    private void ShowGas()
    {
        float num = this.currentGas / this.totalGas;
        var GasL1 = CacheGameObject.Find("gasL1");
        var GasL2 = CacheGameObject.Find("gasR1");
        if (GasL1 != null)
        {
            UISprite L1 = CacheGameObject.Find("gasL1").GetComponent<UISprite>();
            L1.fillAmount = num;
        }
        if (GasL2 != null)
        {
            UISprite L2 = CacheGameObject.Find("gasR1").GetComponent<UISprite>();
            L2.fillAmount = num;
        }
        var L = SpriteCache.Find("gasL");
        var R = SpriteCache.Find("gasR");
        if (L == null || R == null)
        {
            return;
        }
        if (num <= 0f)
        {
            L.color = R.color = Colors.red;
            return;
        }
        if (num <= 0.1f)
        {
            L.color = R.color = Colors.orange;
            return;
        }
        if (num < 0.3f)
        {
            L.color = R.color = Colors.yellow;
            return;
        }
        L.color = Colors.white;
        R.color = Colors.white;
    }

    [RPC]
    private void showHitDamage()
    {
        GameObject gameObject = CacheGameObject.Find("LabelScore");
        if (!gameObject)
        {
            return;
        }
        this.speed = Mathf.Max(10f, this.speed);
        gameObject.GetComponent<UILabel>().text = this.speed.ToString();
        gameObject.transform.localScale = Vectors.zero;
        this.speed = (float)((int)(this.speed * 0.1f));
        this.speed = Mathf.Clamp(this.speed, 40f, 150f);
        iTween.Stop(gameObject);
        iTween.ScaleTo(gameObject, iTween.Hash(new object[]
        {
            "x",
            this.speed,
            "y",
            this.speed,
            "z",
            this.speed,
            "easetype",
            iTween.EaseType.easeOutElastic,
            "time",
            1f
        }));
        iTween.ScaleTo(gameObject, iTween.Hash(new object[]
        {
            "x",
            0,
            "y",
            0,
            "z",
            0,
            "easetype",
            iTween.EaseType.easeInBounce,
            "time",
            0.5f,
            "delay",
            2f
        }));
    }

    private void showSkillCD()
    {
        if (this.skillCD)
        {
            this.skillCD.GetComponent<UISprite>().fillAmount = (this.skillCDLast - this.skillCDDuration) / this.skillCDLast;
        }
    }

    private void Start()
    {
        FengGameManagerMKII.FGM.AddHero(this);
        if ((FengGameManagerMKII.Level.HorsesEnabled || GameModes.AllowHorses.Enabled) && IN_GAME_MAIN_CAMERA.GameType == GameType.Multi && BasePV.IsMine)
        {
            this.myHorse = Pool.NetworkEnable("horse", baseT.position + Vectors.up * 5f, baseT.rotation, 0);
            this.myHorse.GetComponent<Horse>().myHero = baseG;
            this.myHorse.GetComponent<TITAN_CONTROLLER>().isHorse = true;
        }
        this.sparks = baseT.Find("slideSparks").GetComponent<ParticleSystem>();
        this.smoke_3dmg = baseT.Find("3dmg_smoke").GetComponent<ParticleSystem>();
        baseT.localScale = new Vector3(this.myScale, this.myScale, this.myScale);
        this.facingDirection = baseT.rotation.eulerAngles.y;
        this.targetRotation = Quaternion.Euler(0f, this.facingDirection, 0f);
        this.smoke_3dmg.enableEmission = false;
        this.sparks.enableEmission = false;
        this.speedFXPS = this.speedFX1.GetComponent<ParticleSystem>();
        this.speedFXPS.enableEmission = false;
        List<string> enables = new List<string>(new string[] { "Controller_Body", "AOTTG_HERO 1(Clone)" });
        if (!IsLocal)
        {
            foreach (Collider col in base.GetComponentsInChildren<Collider>())
            {
                if (enables.Contains(col.name))
                    continue;
                col.enabled = false;
            }
        }
        if (IN_GAME_MAIN_CAMERA.GameType == GameType.Multi)
        {
            if (RCManager.heroHash.ContainsKey(BasePV.owner.ID))
            {
                RCManager.heroHash[BasePV.owner.ID] = this;
            }
            else
            {
                RCManager.heroHash.Add(BasePV.owner.ID, this);
            }
            this.myNetWorkName = (GameObject)Instantiate(CacheResources.Load("UI/LabelNameOverHead"));
            this.myNetWorkName.name = "LabelNameOverHead";
            this.myNetWorkName.transform.parent = FengGameManagerMKII.UIRefer.panels[0].transform;
            this.myNetWorkName.transform.localScale = new Vector3(6f, 6f, 6f);
            this.myNetWorkName.GetComponent<UILabel>().text = string.Empty;
            TextMesh txt = myNetWorkName.GetComponent<TextMesh>();
            if (txt == null)
            {
                txt = myNetWorkName.AddComponent<TextMesh>();
            }
            MeshRenderer render = myNetWorkName.GetComponent<MeshRenderer>();
            if (render == null)
            {
                render = myNetWorkName.AddComponent<MeshRenderer>();
            }
            render.material = Labels.Font.material;
            txt.font = Labels.Font;
            txt.fontSize = 20;
            txt.anchor = TextAnchor.MiddleCenter;
            txt.alignment = TextAlignment.Center;
            txt.color = Colors.white;
            txt.text = myNetWorkName.GetComponent<UILabel>().text;
            txt.richText = true;
            txt.gameObject.layer = 5;
            string show = string.Empty;
            myNetWorkName.GetComponent<UILabel>().enabled = false;
            if (BasePV.owner.Team == 2)
            {
                show += "[FF0000]AHSS\n[FFFFFF]";
            }
            if (BasePV.owner.GuildName != string.Empty)
            {
                show += $"[FFFF00]{BasePV.owner.GuildName}\n[FFFFFF]";
            }
            show += BasePV.owner.UIName;
            myNetWorkName.GetComponent<TextMesh>().text = show.ToHTMLFormat();
            GameModes.InfectionOnSpawn(this);
        }
        else
        {
            Minimap.TrackGameObjectOnMinimap(base.gameObject, Color.green, false, true, Minimap.IconStyle.Circle);
        }
        BombImmune = false;
        if (GameModes.BombMode.Enabled)
        {
            BombImmune = true;
            StartCoroutine(stopImmunity());
        }
        if (IsLocal)
        {
            ShowGas();
            if (Gunner)
                ShowBullets();
            else
                ShowBlades();
            if (IN_GAME_MAIN_CAMERA.GameType == GameType.Multi)
            {
                GetComponent<SmoothSyncMovement>().PhotonCamera = true;
                BasePV.RPC("SetMyPhotonCamera", PhotonTargets.OthersBuffered, new object[]
                {
                    Settings.CameraDistance.Value + 0.3f
                });
            }
            spawned = true;
            int viewid = -1;
            if (SkinSettings.HumanSkins.Value > 0 && SkinSettings.HumanSet.Value != "$not define$")
            {
                var set = new HumanSkinPreset(SkinSettings.HumanSet.Value);
                set.Load();
                mySkinURL = string.Join(",", set.ToSkinData(), 0, 13);
            }
            if (myHorse != null)
            {
                viewid = myHorse.GetPhotonView().viewID;
            }
            if (mySkinURL != string.Empty)
            {
                loadskinRPC(viewid, mySkinURL, null);
                if (IN_GAME_MAIN_CAMERA.GameType == GameType.Multi)
                {
                    BasePV.RPC("loadskinRPC", PhotonTargets.Others, new object[] { viewid, mySkinURL });
                }
            }
            Minimap.TrackGameObjectOnMinimap(base.gameObject, Color.green, false, true, Minimap.IconStyle.Circle);
            wLeft.Active = false;
            wRight.Active = false;
            if (AnarchyManager.Pause.Active)
            {
                AnarchyManager.Pause.DisableImmediate();
            }
            return;
        }
        if (IN_GAME_MAIN_CAMERA.DayLight == DayLight.Night)
        {
            GameObject gameObject2 = (GameObject)UnityEngine.Object.Instantiate(CacheResources.Load("flashlight"));
            gameObject2.transform.parent = baseT;
            gameObject2.transform.position = baseT.position + Vectors.up;
            gameObject2.transform.rotation = Quaternion.Euler(353f, 0f, 0f);
        }
        Color mapColor = Colors.blue;
        if (BasePV.owner.RCteam > 0)
        {
            mapColor = BasePV.owner.RCteam == 1 ? Colors.magenta : Colors.cyan;
        }
        if (BasePV.owner.Team == 2)
        {
            mapColor = Colors.red;
        }
        Minimap.TrackGameObjectOnMinimap(base.gameObject, mapColor, false, true, Minimap.IconStyle.Circle);
        this.Setup.Init();
        this.Setup.myCostume = new HeroCostume();
        this.Setup.myCostume = CostumeConeveter.PhotonDataToHeroCostume(BasePV.owner);
        this.Setup.SetCharacterComponent();
        UnityEngine.Object.Destroy(this.checkBoxLeft);
        UnityEngine.Object.Destroy(this.checkBoxRight);
        UnityEngine.Object.Destroy(this.leftbladetrail);
        UnityEngine.Object.Destroy(this.rightbladetrail);
        UnityEngine.Object.Destroy(this.leftbladetrail2);
        UnityEngine.Object.Destroy(this.rightbladetrail2);
        spawned = true;
    }

    private void suicide()
    {
        if (IN_GAME_MAIN_CAMERA.GameType == GameType.Single)
        {
            return;
        }
        this.netDieSpecial(baseR.velocity * 50f, false, -1, User.Suicide.PickRandomString(), true);
        FengGameManagerMKII.FGM.NeedChooseSide = true;
        FengGameManagerMKII.FGM.JustSuicide = true;
    }

    private void throwBlades()
    {
        GameObject obj2 = (GameObject)Instantiate(CacheResources.Load("Character_parts/character_blade_l"), Setup.part_blade_l.transform.position, Setup.part_blade_l.transform.rotation);
        GameObject obj3 = (GameObject)Instantiate(CacheResources.Load("Character_parts/character_blade_r"), this.Setup.part_blade_r.transform.position, this.Setup.part_blade_r.transform.rotation);
        obj2.renderer.material = obj3.renderer.material = CharacterMaterials.Materials[this.Setup.myCostume._3dmg_texture];
        var vec = (baseT.Forward() + baseT.Up() * 2);
        obj2.rigidbody.AddForce(vec - baseT.Right(), ForceMode.Impulse);
        obj3.rigidbody.AddForce(vec + baseT.Right(), ForceMode.Impulse);
        obj2.rigidbody.AddTorque(new Vector3(Random.Range(-100f, 100f), Random.Range(-100f, 100f), Random.Range(-100f, 100f)).normalized);
        obj3.rigidbody.AddTorque(new Vector3(Random.Range(-100f, 100f), Random.Range(-100f, 100f), Random.Range(-100f, 100f)).normalized);
        this.Setup.part_blade_l.SetActive(false);
        this.Setup.part_blade_r.SetActive(false);
        currentBladeNum--;
        currentBladeSta = currentBladeNum == 0 ? 0f : currentBladeSta;
        if (State == HeroState.Attack)
        {
            falseAttack();
        }
        ShowBlades();
    }

    private void unmounted()
    {
        this.myHorse.GetComponent<Horse>().unmounted();
        this.isMounted = false;
    }

    private void UpdateLeftMagUI()
    {
        for (int i = 1; i <= this.bulletMAX; i++)
        {
            SpriteCache.Find("bulletL" + i).enabled = false;
        }
        for (int j = 1; j <= this.leftBulletLeft; j++)
        {
            SpriteCache.Find("bulletL" + j).enabled = true;
        }
    }

    private void UpdateRightMagUI()
    {
        for (int i = 1; i <= this.bulletMAX; i++)
        {
            SpriteCache.Find("bulletR" + i).enabled = false;
        }
        for (int j = 1; j <= this.rightBulletLeft; j++)
        {
            SpriteCache.Find("bulletR" + j).enabled = true;
        }
    }

    private void useGas(float amount = 0f)
    {
        if (amount == 0f)
        {
            amount = this.useGasSpeed;
        }
        if (this.currentGas > 0f)
        {
            this.currentGas -= amount;
            if (this.currentGas < 0f)
            {
                this.currentGas = 0f;
            }
        }
        ShowGas();
    }

    [RPC]
    private void whoIsMyErenTitan(int id)
    {
        this.eren = PhotonView.Find(id).gameObject;
        this.titanForm = true;
    }

    public void attackAccordingToMouse()
    {
        if ((double)Input.mousePosition.x < (double)Screen.width * 0.5)
        {
            this.attackAnimation = "attack2";
        }
        else
        {
            this.attackAnimation = "attack1";
        }
    }

    public void attackAccordingToTarget(Transform a)
    {
        Vector3 vector = a.position - baseT.position;
        float current = -Mathf.Atan2(vector.z, vector.x) * 57.29578f;
        float num = -Mathf.DeltaAngle(current, baseT.rotation.eulerAngles.y - 90f);
        if (Mathf.Abs(num) < 90f && vector.magnitude < 6f && a.position.y <= baseT.position.y + 2f && a.position.y >= baseT.position.y - 5f)
        {
            this.attackAnimation = "attack4";
        }
        else if (num > 0f)
        {
            this.attackAnimation = "attack1";
        }
        else
        {
            this.attackAnimation = "attack2";
        }
    }

    public void backToHuman()
    {
        baseG.GetComponent<SmoothSyncMovement>().Disabled = false;
        baseR.velocity = Vectors.zero;
        this.titanForm = false;
        this.ungrabbed();
        this.falseAttack();
        this.skillCDDuration = this.skillCDLast;
        IN_GAME_MAIN_CAMERA.MainCamera.SetMainObject(this, true, false);
        if (IN_GAME_MAIN_CAMERA.GameType != GameType.Single)
        {
            BasePV.RPC("backToHumanRPC", PhotonTargets.Others, new object[0]);
        }
    }

    [RPC]
    public void badGuyReleaseMe()
    {
        this.hookBySomeOne = false;
        this.badGuy = null;
    }

    [RPC]
    public void blowAway(Vector3 force)
    {
        if (IN_GAME_MAIN_CAMERA.GameType != GameType.Single && !BasePV.IsMine)
        {
            return;
        }
        baseR.AddForce(force, ForceMode.Impulse);
        baseT.LookAt(baseT.position);
    }

    public void ClearPopup()
    {
        FengGameManagerMKII.FGM.ShowHUDInfoCenter(string.Empty);
    }

    public void continueAnimation()
    {
        foreach (object obj in baseA)
        {
            AnimationState animationState = (AnimationState)obj;
            if (animationState.speed == 1f)
            {
                return;
            }
            animationState.speed = 1f;
        }
        this.customAnimationSpeed();
        this.playAnimation(this.currentPlayingClipName());
        if (IN_GAME_MAIN_CAMERA.GameType != GameType.Single && BasePV.IsMine)
        {
            BasePV.RPC("netContinueAnimation", PhotonTargets.Others, new object[0]);
        }
    }

    public void crossFade(string aniName, float time)
    {
        this.currentAnimation = aniName;
        baseA.CrossFade(aniName, time);
        if (!PhotonNetwork.connected)
        {
            return;
        }
        if (BasePV.IsMine)
        {
            BasePV.RPC("netCrossFade", PhotonTargets.Others, new object[]
            {
                aniName,
                time
            });
        }
    }

    public string currentPlayingClipName()
    {
        foreach (object obj in baseA)
        {
            AnimationState animationState = (AnimationState)obj;
            if (baseA.IsPlaying(animationState.name))
            {
                return animationState.name;
            }
        }
        return string.Empty;
    }

    public void die(Vector3 v, bool isBite)
    {
        if (this.invincible > 0f)
        {
            return;
        }
        if (this.titanForm && this.eren != null)
        {
            this.eren.GetComponent<TITAN_EREN>().lifeTime = 0.1f;
        }
        if (this.bulletLeft)
        {
            bulletLeft.RemoveMe();
        }
        if (this.bulletRight)
        {
            bulletRight.RemoveMe();
        }
        this.meatDie.Play();
        if ((IN_GAME_MAIN_CAMERA.GameType == GameType.Single || BasePV.IsMine) && !this.Gunner)
        {
            this.leftbladetrail.Deactivate();
            this.rightbladetrail.Deactivate();
            this.leftbladetrail2.Deactivate();
            this.rightbladetrail2.Deactivate();
        }
        this.breakApart(v, isBite);
        IN_GAME_MAIN_CAMERA.MainCamera.gameOver = true;
        FengGameManagerMKII.FGM.GameLose();
        this.falseAttack();
        this.hasDied = true;
        Transform transform = baseT.Find("audio_die");
        transform.parent = null;
        transform.GetComponent<AudioSource>().Play();
        if (Settings.Snapshots.ToValue())
        {
            IN_GAME_MAIN_CAMERA.MainCamera.startSnapShot(baseT.position, 0, null, 0.02f);
        }
        UnityEngine.Object.Destroy(baseG);
    }

    public void die2(Transform tf)
    {
        if (this.invincible > 0f)
        {
            return;
        }
        if (this.titanForm && this.eren != null)
        {
            this.eren.GetComponent<TITAN_EREN>().lifeTime = 0.1f;
        }
        if (this.bulletLeft)
        {
            bulletLeft.RemoveMe();
        }
        if (this.bulletRight)
        {
            bulletRight.RemoveMe();
        }
        Transform transform = baseT.Find("audio_die");
        transform.parent = null;
        transform.GetComponent<AudioSource>().Play();
        this.meatDie.Play();
        IN_GAME_MAIN_CAMERA.MainCamera.SetMainObject(null, true, false);
        IN_GAME_MAIN_CAMERA.MainCamera.gameOver = true;
        FengGameManagerMKII.FGM.GameLose();
        this.falseAttack();
        this.hasDied = true;
        Pool.Enable("hitMeat2", baseT.position, Quaternion.identity);//(GameObject)UnityEngine.Object.Instantiate(CacheResources.Load("hitMeat2"));
        //gameObject.transform.position = baseT.position;
        UnityEngine.Object.Destroy(baseG);
    }

    public void falseAttack()
    {
        this.attackMove = false;
        if (this.Gunner)
        {
            if (!this.attackReleased)
            {
                this.continueAnimation();
                this.attackReleased = true;
            }
        }
        else
        {
            if (IN_GAME_MAIN_CAMERA.GameType == GameType.Single || BasePV.IsMine)
            {
                this.wLeft.Active = false;
                this.wRight.Active = false;
                this.wLeft.clearHits();
                this.wRight.clearHits();
                this.leftbladetrail.StopSmoothly(0.2f);
                this.rightbladetrail.StopSmoothly(0.2f);
                this.leftbladetrail2.StopSmoothly(0.2f);
                this.rightbladetrail2.StopSmoothly(0.2f);
            }
            this.attackLoop = 0;
            if (!this.attackReleased)
            {
                this.continueAnimation();
                this.attackReleased = true;
            }
        }
    }

    public void fillGas()
    {
        this.currentGas = this.totalGas;
        ShowGas();
        if (Gunner) ShowBullets();
        else ShowBlades();
    }

    public string getDebugInfo()
    {
        string text = "Left:" + this.isLeftHandHooked + " ";
        if (this.isLeftHandHooked && this.bulletLeft != null)
        {
            Vector3 vector = this.bulletLeft.transform.position - baseT.position;
            text += (int)(Mathf.Atan2(vector.x, vector.z) * 57.29578f);
        }
        string text2 = text;
        text = string.Concat(new object[]
        {
            text2,
            "\nRight:",
            this.isRightHandHooked,
            " "
        });
        if (this.isRightHandHooked && this.bulletRight != null)
        {
            Vector3 vector2 = this.bulletRight.transform.position - baseT.position;
            text += (int)(Mathf.Atan2(vector2.x, vector2.z) * 57.29578f);
        }
        text = text + "\nfacingDirection:" + (int)this.facingDirection;
        text = text + "\nActual facingDirection:" + (int)baseT.rotation.eulerAngles.y;
        text = text + "\nState:" + this.State.ToString();
        text += "\n\n\n\n\n";
        if (this.State == HeroState.Attack)
        {
            this.targetRotation = Quaternion.Euler(0f, this.facingDirection, 0f);
        }
        return text;
    }

    public void getSupply()
    {
        if ((baseA.IsPlaying(this.standAnimation) || baseA.IsPlaying("run") || baseA.IsPlaying("run_sasha")) && (this.currentBladeSta != this.totalBladeSta || this.currentBladeNum != this.totalBladeNum || this.currentGas != this.totalGas || this.leftBulletLeft != this.bulletMAX || this.rightBulletLeft != this.bulletMAX))
        {
            this.State = HeroState.FillGas;
            this.crossFade("supply", 0.1f);
        }
    }

    public void grabbed(GameObject titan, bool leftHand)
    {
        if (this.isMounted)
        {
            this.unmounted();
        }
        this.State = HeroState.Grab;
        base.GetComponent<CapsuleCollider>().isTrigger = true;
        this.falseAttack();
        this.titanWhoGrabMe = titan;
        if (this.titanForm && this.eren != null)
        {
            this.eren.GetComponent<TITAN_EREN>().lifeTime = 0.1f;
        }
        if (!this.Gunner && (IN_GAME_MAIN_CAMERA.GameType == GameType.Single || BasePV.IsMine))
        {
            this.leftbladetrail.Deactivate();
            this.rightbladetrail.Deactivate();
            this.leftbladetrail2.Deactivate();
            this.rightbladetrail2.Deactivate();
        }
        this.smoke_3dmg.enableEmission = false;
        this.sparks.enableEmission = false;
    }

    public bool HasDied()
    {
        return this.hasDied || this.IsInvincible();
    }

    public void hookedByHuman(int hooker, Vector3 hookPosition)
    {
        BasePV.RPC("RPCHookedByHuman", BasePV.owner, new object[]
        {
            hooker,
            hookPosition
        });
    }

    [RPC]
    public void hookFail()
    {
        this.hookTarget = null;
        this.hookSomeOne = false;
    }

    public void hookToHuman(GameObject target, Vector3 hookPosition)
    {
        this.releaseIfIHookSb();
        this.hookTarget = target;
        this.hookSomeOne = true;
        if (target.GetComponent<HERO>())
        {
            target.GetComponent<HERO>().hookedByHuman(BasePV.viewID, hookPosition);
        }
        this.launchForce = hookPosition - baseT.position;
        float d = Mathf.Pow(this.launchForce.magnitude, 0.1f);
        if (this.grounded)
        {
            baseR.AddForce(Vectors.up * Mathf.Min(this.launchForce.magnitude * 0.2f, 10f), ForceMode.Impulse);
        }
        baseR.AddForce(this.launchForce * d * 0.1f, ForceMode.Impulse);
    }

    public bool IsGrounded()
    {
        return Physics.Raycast(baseGT.position + Vectors.up * 0.1f, -Vectors.up, 0.3f, Layers.EnemyGround.value);
    }

    public bool IsInvincible()
    {
        return this.invincible > 0f;
    }

    public void lateUpdate()
    {
        if (IN_GAME_MAIN_CAMERA.GameType != GameType.Single && this.myNetWorkName != null)
        {
            if (this.titanForm && this.eren != null)
            {
                this.myNetWorkName.transform.localPosition = Vectors.up * (float)Screen.height * 2f;
            }
            Vector3 vector = new Vector3(baseT.position.x, baseT.position.y + 2f, baseT.position.z);
            if (Vector3.Angle(IN_GAME_MAIN_CAMERA.BaseT.Forward(), vector - IN_GAME_MAIN_CAMERA.BaseT.position) > 90f || Physics.Linecast(vector, IN_GAME_MAIN_CAMERA.BaseT.position, Layers.EnemyGround))
            {
                this.myNetWorkName.transform.localPosition = Vectors.up * (float)Screen.height * 2f;
            }
            else
            {
                Vector2 vector2 = IN_GAME_MAIN_CAMERA.BaseCamera.WorldToScreenPoint(vector);
                this.myNetWorkName.transform.localPosition = new Vector3((float)((int)(vector2.x - (float)Screen.width * 0.5f)), (float)((int)(vector2.y - (float)Screen.height * 0.5f)), 0f);
            }
        }
        if (this.titanForm || isCannon)
        {
            return;
        }
        if (VideoSettings.CameraTilt.Value && IsLocal)
        {
            Vector3 vector3 = Vectors.zero;
            Vector3 vector4 = Vectors.zero;
            if (this.isLaunchLeft && this.bulletLeft != null && bulletLeft.IsHooked())
            {
                vector3 = this.bulletLeft.transform.position;
            }
            if (this.isLaunchRight && this.bulletRight != null && bulletRight.IsHooked())
            {
                vector4 = this.bulletRight.transform.position;
            }
            Vector3 a = Vectors.zero;
            if (vector3.magnitude != 0f && vector4.magnitude == 0f)
            {
                a = vector3;
            }
            else if (vector3.magnitude == 0f && vector4.magnitude != 0f)
            {
                a = vector4;
            }
            else if (vector3.magnitude != 0f && vector4.magnitude != 0f)
            {
                a = (vector3 + vector4) * 0.5f;
            }
            Vector3 vector5 = Vector3.Project(a - baseT.position, IN_GAME_MAIN_CAMERA.BaseCamera.transform.Up());
            Vector3 b = Vector3.Project(a - baseT.position, IN_GAME_MAIN_CAMERA.BaseCamera.transform.Right());
            Quaternion to2;
            if (a.magnitude > 0f)
            {
                Vector3 to = vector5 + b;
                float num = Vector3.Angle(a - baseT.position, baseR.velocity);
                num *= 0.005f;
                to2 = Quaternion.Euler(IN_GAME_MAIN_CAMERA.BaseCamera.transform.rotation.eulerAngles.x, IN_GAME_MAIN_CAMERA.BaseCamera.transform.rotation.eulerAngles.y, ((IN_GAME_MAIN_CAMERA.BaseCamera.transform.right + b.normalized).magnitude >= 1f) ? (-Vector3.Angle(vector5, to) * num) : (Vector3.Angle(vector5, to) * num));
            }
            else
            {
                to2 = Quaternion.Euler(IN_GAME_MAIN_CAMERA.BaseCamera.transform.rotation.eulerAngles.x, IN_GAME_MAIN_CAMERA.BaseCamera.transform.rotation.eulerAngles.y, 0f);
            }
            IN_GAME_MAIN_CAMERA.BaseCamera.transform.rotation = Quaternion.Lerp(IN_GAME_MAIN_CAMERA.BaseCamera.transform.rotation, to2, Time.deltaTime);
        }
        if (this.State == HeroState.Grab && this.titanWhoGrabMe)
        {
            if (this.titanWhoGrabMe.GetComponent<TITAN>())
            {
                baseT.position = this.titanWhoGrabMe.GetComponent<TITAN>().grabTF.transform.position;
                baseT.rotation = this.titanWhoGrabMe.GetComponent<TITAN>().grabTF.transform.rotation;
            }
            else if (this.titanWhoGrabMe.GetComponent<FEMALE_TITAN>())
            {
                baseT.position = this.titanWhoGrabMe.GetComponent<FEMALE_TITAN>().grabTF.transform.position;
                baseT.rotation = this.titanWhoGrabMe.GetComponent<FEMALE_TITAN>().grabTF.transform.rotation;
            }
        }
        if (this.Gunner)
        {
            if (this.leftArmAim || this.rightArmAim)
            {
                Vector3 vector6 = this.gunTarget - baseT.position;
                float current = -Mathf.Atan2(vector6.z, vector6.x) * 57.29578f;
                float num2 = -Mathf.DeltaAngle(current, baseT.rotation.eulerAngles.y - 90f);
                this.headMovement();
                if (!this.isLeftHandHooked && this.leftArmAim && num2 < 40f && num2 > -90f)
                {
                    this.leftArmAimTo(this.gunTarget);
                }
                if (!this.isRightHandHooked && this.rightArmAim && num2 > -40f && num2 < 90f)
                {
                    this.rightArmAimTo(this.gunTarget);
                }
            }
            else if (!this.grounded)
            {
                this.Hand_L.localRotation = Quaternion.Euler(90f, 0f, 0f);
                this.Hand_R.localRotation = Quaternion.Euler(-90f, 0f, 0f);
            }
            if (this.isLeftHandHooked && this.bulletLeft != null)
            {
                this.leftArmAimTo(this.bulletLeft.transform.position);
            }
            if (this.isRightHandHooked && this.bulletRight != null)
            {
                this.rightArmAimTo(this.bulletRight.transform.position);
            }
        }
        if (VideoSettings.BladeTrails.Value && !Gunner && IsLocal)
        {
            this.leftbladetrail.lateUpdate();
            this.rightbladetrail.lateUpdate();
            this.leftbladetrail2.lateUpdate();
            this.rightbladetrail2.lateUpdate();
        }
    }

    public void launch(Vector3 des, bool left = true, bool leviMode = false)
    {
        if (this.isMounted)
        {
            this.unmounted();
        }
        if (this.State != HeroState.Attack)
        {
            this.idle();
        }
        Vector3 a = des - baseT.position;
        if (left)
        {
            this.launchPointLeft = des;
        }
        else
        {
            this.launchPointRight = des;
        }
        a.Normalize();
        a *= 20f;
        if (this.bulletLeft != null && this.bulletRight != null && bulletLeft.IsHooked() && bulletRight.IsHooked())
        {
            a *= 0.8f;
        }
        leviMode = (baseA.IsPlaying("attack5") || baseA.IsPlaying("special_petra"));
        if (!leviMode)
        {
            this.falseAttack();
            this.idle();
            if (this.Gunner)
            {
                this.crossFade("AHSS_hook_forward_both", 0.1f);
            }
            else if (left && !this.isRightHandHooked)
            {
                this.crossFade("air_hook_l_just", 0.1f);
            }
            else if (!left && !this.isLeftHandHooked)
            {
                this.crossFade("air_hook_r_just", 0.1f);
            }
            else
            {
                this.crossFade("dash", 0.1f);
                baseA["dash"].time = 0f;
            }
        }
        if (left)
        {
            this.isLaunchLeft = true;
        }
        if (!left)
        {
            this.isLaunchRight = true;
        }
        this.launchForce = a;
        if (!leviMode)
        {
            if (a.y < 30f)
            {
                this.launchForce += Vectors.up * (30f - a.y);
            }
            if (des.y >= baseT.position.y)
            {
                this.launchForce += Vectors.up * (des.y - baseT.position.y) * 10f;
            }
            baseR.AddForce(this.launchForce);
        }
        this.facingDirection = Mathf.Atan2(this.launchForce.x, this.launchForce.z) * 57.29578f;
        Quaternion quaternion = Quaternion.Euler(0f, this.facingDirection, 0f);
        baseGT.rotation = quaternion;
        baseR.rotation = quaternion;
        this.targetRotation = quaternion;
        if (left)
        {
            this.launchElapsedTimeL = 0f;
        }
        else
        {
            this.launchElapsedTimeR = 0f;
        }
        if (leviMode)
        {
            this.launchElapsedTimeR = -100f;
        }
        if (baseA.IsPlaying("special_petra"))
        {
            this.launchElapsedTimeR = -100f;
            this.launchElapsedTimeL = -100f;
            if (this.bulletRight)
            {
                bulletRight.Disable();
                this.releaseIfIHookSb();
            }
            if (this.bulletLeft)
            {
                bulletLeft.Disable();
                this.releaseIfIHookSb();
            }
        }
        this.sparks.enableEmission = false;
    }


    [RPC]
    public void loadskinRPC(int horse, string urls, PhotonMessageInfo info = null)
    {
        if (info != null && BasePV != null && (BasePV.owner.ID != info.Sender.ID || !Antis.IsValidSkinURL(ref urls, 13, info.Sender.ID)))
        {
            return;
        }
        if (SkinSettings.HumanSkins.Value == 0)
        {
            return;
        }
        if (SkinSettings.HumanSkins.Value > 1 && info != null && !info.Sender.IsLocal)
        {
            return;
        }
        StartCoroutine(loadskinRPCE(horse, urls, info));
    }

    public void markDie()
    {
        this.hasDied = true;
        this.State = HeroState.Die;
    }

    [RPC]
    public void moveToRPC(float x, float y, float z, PhotonMessageInfo info)
    {
        if (info.Sender.IsMasterClient)
        {
            this.baseT.position = new Vector3(x, y, z);
        }
    }

    [RPC]
    public void netDie(Vector3 v, bool isBite, int viewID = -1, string titanName = "", bool killByTitan = true, PhotonMessageInfo info = null)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            onDeathEvent(viewID, killByTitan);
            if (RCManager.heroHash.ContainsKey(viewID))
                RCManager.heroHash.Remove(viewID);
        }
        if (BasePV.IsMine && this.titanForm && this.eren != null)
        {
            this.eren.GetComponent<TITAN_EREN>().lifeTime = 0.1f;
        }
        if (this.bulletLeft)
        {
            bulletLeft.RemoveMe();
        }
        if (this.bulletRight)
        {
            bulletRight.RemoveMe();
        }
        this.meatDie.Play();
        if (!this.Gunner && (IN_GAME_MAIN_CAMERA.GameType == GameType.Single || BasePV.IsMine))
        {
            this.leftbladetrail.Deactivate();
            this.rightbladetrail.Deactivate();
            this.leftbladetrail2.Deactivate();
            this.rightbladetrail2.Deactivate();
        }
        this.falseAttack();
        this.breakApart(v, isBite);
        if (BasePV.IsMine)
        {
            IN_GAME_MAIN_CAMERA.MainCamera.setSpectorMode(false);
            IN_GAME_MAIN_CAMERA.MainCamera.gameOver = true;
            FengGameManagerMKII.FGM.Logic.MyRespawnTime = 0f;
        }
        this.hasDied = true;
        Transform transform = baseT.Find("audio_die");
        transform.parent = null;
        transform.GetComponent<AudioSource>().Play();
        baseG.GetComponent<SmoothSyncMovement>().Disabled = true;
        if (BasePV.IsMine)
        {
            if (myBomb != null)
            {
                myBomb.destroyMe();
            }
            if (this.myCannon != null)
            {
                PhotonNetwork.Destroy(this.myCannon);
            }
            PhotonNetwork.RemoveRPCs(BasePV);
            PhotonNetwork.player.SetCustomProperties(new Hashtable
            {
                {
                    PhotonPlayerProperty.dead,
                    true
                }
            });
            PhotonNetwork.player.SetCustomProperties(new Hashtable
            {
                {
                    PhotonPlayerProperty.deaths,
                    (int)PhotonNetwork.player.Properties[PhotonPlayerProperty.deaths] + 1
                }
            });
            FengGameManagerMKII.FGM.BasePV.RPC("someOneIsDead", PhotonTargets.MasterClient, new object[]
            {
                titanName == string.Empty ? 0 : 1
            });
            if (viewID != -1)
            {
                PhotonView photonView = PhotonView.Find(viewID);
                if (photonView != null)
                {
                    FengGameManagerMKII.FGM.SendKillInfo(killByTitan, User.DeathFormat(info.Sender.ID, info.Sender.UIName) + " ", false, User.DeathName, 0);
                    photonView.owner.SetCustomProperties(new Hashtable
                    {
                        {
                            PhotonPlayerProperty.kills,
                            (int)photonView.owner.Properties[PhotonPlayerProperty.kills] + 1
                        }
                    });
                }
            }
            else
            {
                FengGameManagerMKII.FGM.SendKillInfo(titanName != string.Empty, User.DeathFormat(info.Sender.ID, titanName) + " ", false, User.DeathName, 0);
            }
        }
        if (BasePV.IsMine)
        {
            PhotonNetwork.Destroy(BasePV);
        }
    }

    public void netDieSpecial(Vector3 v, bool isBite, int viewID = -1, string titanName = "", bool killByTitan = true)
    {
        if (this.titanForm && this.eren != null)
        {
            this.eren.GetComponent<TITAN_EREN>().lifeTime = 0.1f;
        }
        if (myBomb != null)
        {
            myBomb.destroyMe();
        }
        if (this.myCannon != null)
        {
            PhotonNetwork.Destroy(this.myCannon);
        }
        if (this.bulletLeft)
        {
            bulletLeft.RemoveMe();
        }
        if (this.bulletRight)
        {
            bulletRight.RemoveMe();
        }
        this.meatDie.Play();
        if (!this.Gunner && (IN_GAME_MAIN_CAMERA.GameType == GameType.Single || BasePV.IsMine))
        {
            this.leftbladetrail.Deactivate();
            this.rightbladetrail.Deactivate();
            this.leftbladetrail2.Deactivate();
            this.rightbladetrail2.Deactivate();
        }
        this.falseAttack();
        this.breakApart(v, isBite);
        IN_GAME_MAIN_CAMERA.MainCamera.setSpectorMode(false);
        IN_GAME_MAIN_CAMERA.MainCamera.gameOver = true;
        FengGameManagerMKII.FGM.Logic.MyRespawnTime = 0f;
        this.hasDied = true;
        Transform transform = baseT.Find("audio_die");
        transform.parent = null;
        transform.GetComponent<AudioSource>().Play();
        baseG.GetComponent<SmoothSyncMovement>().Disabled = true;
        PhotonNetwork.player.Dead = true;
        PhotonNetwork.player.Deaths++;
        FengGameManagerMKII.FGM.BasePV.RPC("someOneIsDead", PhotonTargets.MasterClient, new object[] { 0 });
        FengGameManagerMKII.FGM.SendKillInfo(false, titanName + " ", false, User.DeathName, 0);
        PhotonNetwork.Destroy(BasePV);
        if (PhotonNetwork.IsMasterClient)
        {
            onDeathEvent(viewID, killByTitan);
            if (RCManager.heroHash.ContainsKey(viewID))
                RCManager.heroHash.Remove(viewID);
        }
    }

    public void pauseAnimation()
    {
        foreach (object obj in baseA)
        {
            AnimationState animationState = (AnimationState)obj;
            animationState.speed = 0f;
        }
        if (IN_GAME_MAIN_CAMERA.GameType != GameType.Single && BasePV.IsMine)
        {
            BasePV.RPC("netPauseAnimation", PhotonTargets.Others, new object[0]);
        }
    }

    public void playAnimation(string aniName)
    {
        this.currentAnimation = aniName;
        baseA.Play(aniName);
        if (!PhotonNetwork.connected)
        {
            return;
        }
        if (BasePV.IsMine)
        {
            BasePV.RPC("netPlayAnimation", PhotonTargets.Others, new object[]
            {
                aniName
            });
        }
    }

    public void resetAnimationSpeed()
    {
        foreach (object obj in baseA)
        {
            AnimationState animationState = (AnimationState)obj;
            animationState.speed = 1f;
        }
        this.customAnimationSpeed();
    }

    [RPC]
    public void ReturnFromCannon(PhotonMessageInfo info)
    {
        bool flag = info.Sender == BasePV.owner;
        if (flag)
        {
            this.isCannon = false;
            base.gameObject.GetComponent<SmoothSyncMovement>().Disabled = false;
        }
    }

    [RPC]
    public void SetMyCannon(int viewid, PhotonMessageInfo info)
    {
        bool flag = info.Sender == BasePV.owner;
        if (flag)
        {
            PhotonView photonView = PhotonView.Find(viewid);
            bool flag2 = photonView != null;
            if (flag2)
            {
                this.myCannon = photonView.gameObject;
                bool flag3 = this.myCannon != null;
                if (flag3)
                {
                    this.myCannonBase = this.myCannon.transform;
                    this.myCannonPlayer = this.myCannonBase.Find("PlayerPoint");
                    this.isCannon = true;
                }
            }
        }
    }

    [RPC]
    public void SetMyPhotonCamera(float offset, PhotonMessageInfo info)
    {
        if (BasePV.owner == info.Sender)
        {
            CameraMultiplier = offset;
            GetComponent<SmoothSyncMovement>().PhotonCamera = true;
            IsPhotonCamera = true;
        }
    }
    public void setSkillHUDPosition()
    {
        this.skillCD = CacheGameObject.Find("skill_cd_" + this.skillIDHUD);
        if (this.skillCD != null)
        {
            this.skillCD.transform.localPosition = CacheGameObject.Find("skill_cd_bottom").transform.localPosition;
        }
        if (this.Gunner)
        {
            this.skillCD.transform.localPosition = Vectors.up * 5000f;
        }
    }

    public void setStat()
    {
        this.skillCDLast = 1.5f;
        this.skillID = this.Setup.myCostume.stat.skillID;
        if (this.skillID == "levi")
        {
            this.skillCDLast = 3.5f;
        }
        this.customAnimationSpeed();
        if (this.skillID == "armin")
        {
            this.skillCDLast = 5f;
        }
        if (this.skillID == "marco")
        {
            this.skillCDLast = 10f;
        }
        if (this.skillID == "jean")
        {
            this.skillCDLast = 0.001f;
        }
        if (this.skillID == "eren")
        {
            this.skillCDLast = 120f;
            if (IN_GAME_MAIN_CAMERA.GameType == GameType.Multi)
            {
                if (FengGameManagerMKII.Level.TeamTitan || FengGameManagerMKII.Level.Mode == GameMode.RACING || FengGameManagerMKII.Level.Mode == GameMode.PVP_CAPTURE || FengGameManagerMKII.Level.Mode == GameMode.TROST)
                {
                    this.skillID = "petra";
                    this.skillCDLast = 1f;
                }
                else
                {
                    int num = 0;
                    foreach (PhotonPlayer player in PhotonNetwork.playerList)
                    {
                        if (!player.IsTitan && player.Character.ToUpper() == "EREN")
                        {
                            num++;
                        }
                    }
                    if (num > 1)
                    {
                        this.skillID = "petra";
                        this.skillCDLast = 1f;
                    }
                }
            }
        }
        if (this.skillID == "sasha")
        {
            this.skillCDLast = 20f;
        }
        if (this.skillID == "petra")
        {
            this.skillCDLast = 3.5f;
        }
        bombInit();
        this.speed = (float)this.Setup.myCostume.stat.Spd / 10f;
        this.totalGas = (this.currentGas = (float)this.Setup.myCostume.stat.Gas);
        this.totalBladeSta = (this.currentBladeSta = (float)this.Setup.myCostume.stat.Bla);
        base.rigidbody.mass = 0.5f - (float)(this.Setup.myCostume.stat.Acl - 100) * 0.001f;
        CacheGameObject.Find("skill_cd_bottom").transform.localPosition = new Vector3(0f, (float)(-(float)Screen.height) * 0.5f + 5f, 0f);
        this.skillCD = CacheGameObject.Find("skill_cd_" + this.skillIDHUD);
        this.skillCD.transform.localPosition = CacheGameObject.Find("skill_cd_bottom").transform.localPosition;
        CacheGameObject.Find("GasUI").transform.localPosition = CacheGameObject.Find("skill_cd_bottom").transform.localPosition;
        if (IsLocal)
        {
            SpriteCache.Find("bulletL").enabled = false;
            SpriteCache.Find("bulletR").enabled = false;
            SpriteCache.Find("bulletL1").enabled = false;
            SpriteCache.Find("bulletR1").enabled = false;
            SpriteCache.Find("bulletL2").enabled = false;
            SpriteCache.Find("bulletR2").enabled = false;
            SpriteCache.Find("bulletL3").enabled = false;
            SpriteCache.Find("bulletR3").enabled = false;
            SpriteCache.Find("bulletL4").enabled = false;
            SpriteCache.Find("bulletR4").enabled = false;
            SpriteCache.Find("bulletL5").enabled = false;
            SpriteCache.Find("bulletR5").enabled = false;
            SpriteCache.Find("bulletL6").enabled = false;
            SpriteCache.Find("bulletR6").enabled = false;
            SpriteCache.Find("bulletL7").enabled = false;
            SpriteCache.Find("bulletR7").enabled = false;
        }
        if (this.Setup.myCostume.uniform_type == UNIFORM_TYPE.CasualAHSS)
        {
            this.standAnimation = "AHSS_stand_gun";
            Gunner = true;
            wLeft.Active = false;
            wRight.Active = false;
            this.gunDummy = new GameObject();
            this.gunDummy.name = "gunDummy";
            this.gunDummy.transform.position = this.baseT.position;
            this.gunDummy.transform.rotation = this.baseT.rotation;
            setTeam(2);
            if (IsLocal)
            {
                SpriteCache.Find("bladeCL").enabled = false;
                SpriteCache.Find("bladeCR").enabled = false;
                SpriteCache.Find("bladel1").enabled = false;
                SpriteCache.Find("blader1").enabled = false;
                SpriteCache.Find("bladel2").enabled = false;
                SpriteCache.Find("blader2").enabled = false;
                SpriteCache.Find("bladel3").enabled = false;
                SpriteCache.Find("blader3").enabled = false;
                SpriteCache.Find("bladel4").enabled = false;
                SpriteCache.Find("blader4").enabled = false;
                SpriteCache.Find("bladel5").enabled = false;
                SpriteCache.Find("blader5").enabled = false;
                SpriteCache.Find("bulletL").enabled = true;
                SpriteCache.Find("bulletR").enabled = true;
                SpriteCache.Find("bulletL1").enabled = true;
                SpriteCache.Find("bulletR1").enabled = true;
                SpriteCache.Find("bulletL2").enabled = true;
                SpriteCache.Find("bulletR2").enabled = true;
                SpriteCache.Find("bulletL3").enabled = true;
                SpriteCache.Find("bulletR3").enabled = true;
                SpriteCache.Find("bulletL4").enabled = true;
                SpriteCache.Find("bulletR4").enabled = true;
                SpriteCache.Find("bulletL5").enabled = true;
                SpriteCache.Find("bulletR5").enabled = true;
                SpriteCache.Find("bulletL6").enabled = true;
                SpriteCache.Find("bulletR6").enabled = true;
                SpriteCache.Find("bulletL7").enabled = true;
                SpriteCache.Find("bulletR7").enabled = true;
                this.skillCD.transform.localPosition = Vectors.up * 5000f;
                return;
            }
        }
        else
        {
            if (this.Setup.myCostume.sex == Sex.Female)
            {
                this.standAnimation = "stand";
                this.setTeam(1);
                return;
            }
            this.standAnimation = "stand_levi";
            this.setTeam(1);
        }
    }

    public void setTeam(int team)
    {
        if (IN_GAME_MAIN_CAMERA.GameType == GameType.Multi && BasePV.IsMine)
        {
            BasePV.RPC("setMyTeam", PhotonTargets.AllBuffered, new object[] { team });
            PhotonNetwork.player.Team = team;
        }
        else
        {
            this.setMyTeam(team);
        }
    }

    public void shootFlare(int type)
    {
        bool flag = false;
        if (type == 1 && this.flare1CD == 0f)
        {
            this.flare1CD = this.flareTotalCD;
            flag = true;
        }
        if (type == 2 && this.flare2CD == 0f)
        {
            this.flare2CD = this.flareTotalCD;
            flag = true;
        }
        if (type == 3 && this.flare3CD == 0f)
        {
            this.flare3CD = this.flareTotalCD;
            flag = true;
        }
        if (flag)
        {
            if (IN_GAME_MAIN_CAMERA.GameType == GameType.Single)
            {
                GameObject gameObject = Pool.Enable("FX/flareBullet" + type, baseT.position, baseT.rotation);//(GameObject)UnityEngine.Object.Instantiate(CacheResources.Load("FX/flareBullet" + type), baseT.position, baseT.rotation);
                gameObject.GetComponent<FlareMovement>().dontShowHint();
                //UnityEngine.Object.Destroy(gameObject, 25f);
                gameObject.GetComponent<SelfDestroy>().CountDown = 25f;
            }
            else
            {
                GameObject gameObject2 = Optimization.Caching.Pool.NetworkEnable("FX/flareBullet" + type, baseT.position, baseT.rotation, 0);
                gameObject2.GetComponent<FlareMovement>().dontShowHint();
            }
        }
    }

    [RPC]
    public void SpawnCannonRPC(string settings, PhotonMessageInfo info)
    {
        bool flag = info.Sender.IsMasterClient && IsLocal && this.myCannon == null;
        if (flag)
        {
            bool flag2 = this.myHorse != null && this.isMounted;
            if (flag2)
            {
                this.getOffHorse();
            }
            this.idle();
            bool flag3 = this.bulletLeft != null;
            if (flag3)
            {
                this.bulletLeft.RemoveMe();
            }
            bool flag4 = this.bulletRight != null;
            if (flag4)
            {
                this.bulletRight.RemoveMe();
            }
            bool flag5 = this.smoke_3dmg.enableEmission && IN_GAME_MAIN_CAMERA.GameType == GameType.Multi && IsLocal;
            if (flag5)
            {
                object[] parameters = new object[]
                {
                false
                };
                BasePV.RPC("net3DMGSMOKE", PhotonTargets.Others, parameters);
            }
            this.smoke_3dmg.enableEmission = false;
            base.rigidbody.velocity = Vector3.zero;
            string[] array = settings.Split(new char[]
            {
            ','
            });
            bool flag6 = array.Length > 15;
            if (flag6)
            {
                this.myCannon = Pool.NetworkEnable("RCAsset/" + array[1], new Vector3(Convert.ToSingle(array[12]), Convert.ToSingle(array[13]), Convert.ToSingle(array[14])), new Quaternion(Convert.ToSingle(array[15]), Convert.ToSingle(array[16]), Convert.ToSingle(array[17]), Convert.ToSingle(array[18])), 0);
            }
            else
            {
                this.myCannon = Pool.NetworkEnable("RCAsset/" + array[1], new Vector3(Convert.ToSingle(array[2]), Convert.ToSingle(array[3]), Convert.ToSingle(array[4])), new Quaternion(Convert.ToSingle(array[5]), Convert.ToSingle(array[6]), Convert.ToSingle(array[7]), Convert.ToSingle(array[8])), 0);
            }
            this.myCannonBase = this.myCannon.transform;
            this.myCannonPlayer = this.myCannon.transform.Find("PlayerPoint");
            this.isCannon = true;
            this.myCannon.GetComponent<Cannon>().myHero = this;
            this.myCannonRegion = null;
            IN_GAME_MAIN_CAMERA.MainCamera.SetMainObject(this.myCannon.transform.Find("Barrel").Find("FiringPoint").gameObject, true, false);
            IN_GAME_MAIN_CAMERA.BaseCamera.fieldOfView = 55f;
            BasePV.RPC("SetMyCannon", PhotonTargets.OthersBuffered, new object[]
            {
            this.myCannon.GetPhotonView().viewID
            });
            this.skillCDLastCannon = this.skillCDLast;
            this.skillCDLast = 3.5f;
            this.skillCDDuration = 3.5f;
        }
    }

    public System.Collections.IEnumerator stopImmunity()
    {
        yield return new WaitForSeconds(5f);
        this.BombImmune = false;
        yield break;
    }

    public void ungrabbed()
    {
        this.facingDirection = 0f;
        this.targetRotation = Quaternion.Euler(0f, 0f, 0f);
        baseT.parent = null;
        base.GetComponent<CapsuleCollider>().isTrigger = false;
        this.State = HeroState.Idle;
    }

    public void update()
    {
        if (IN_GAME_MAIN_CAMERA.isPausing || hasDied)
        {
            return;
        }
        float dt = Time.deltaTime;
        if (this.invincible > 0f)
        {
            this.invincible -= dt;
        }
        if (this.titanForm && this.eren != null)
        {
            baseT.position = this.eren.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/neck").position;
            baseG.GetComponent<SmoothSyncMovement>().Disabled = true;
        }
        else if (isCannon && myCannon != null)
        {
            UpdateCannon();
            baseG.GetComponent<SmoothSyncMovement>().Disabled = true;
        }
        if (IN_GAME_MAIN_CAMERA.GameType != GameType.Single && !BasePV.IsMine)
        {
            return;
        }
        if (GameModes.BombMode.Enabled)
        {
            bombUpdate();
        }
        if (this.myCannonRegion != null)
        {
            FengGameManagerMKII.FGM.ShowHUDInfoCenter("Press 'Cannon Mount' key to use Cannon.");
            if (InputManager.IsInputCannonHolding(5))
            {
                this.myCannonRegion.BasePV.RPC("RequestControlRPC", PhotonTargets.MasterClient, new object[] { BasePV.viewID });
            }
        }
        if (this.State == HeroState.Grab && !this.Gunner)
        {
            if (this.skillID == "jean")
            {
                if (this.State != HeroState.Attack && (InputManager.IsInputDown[InputCode.Attack0] || InputManager.IsInputDown[InputCode.Attack1]) && this.escapeTimes > 0 && !baseA.IsPlaying("grabbed_jean"))
                {
                    this.playAnimation("grabbed_jean");
                    baseA["grabbed_jean"].time = 0f;
                    this.escapeTimes--;
                }
                if (baseA.IsPlaying("grabbed_jean") && baseA["grabbed_jean"].normalizedTime > 0.64f && this.titanWhoGrabMe.GetComponent<TITAN>())
                {
                    this.ungrabbed();
                    baseR.velocity = Vectors.up * 30f;
                    if (IN_GAME_MAIN_CAMERA.GameType == GameType.Single)
                    {
                        this.titanWhoGrabMe.GetComponent<TITAN>().grabbedTargetEscape();
                    }
                    else
                    {
                        BasePV.RPC("netSetIsGrabbedFalse", PhotonTargets.All, new object[0]);
                        if (PhotonNetwork.IsMasterClient)
                        {
                            this.titanWhoGrabMe.GetComponent<TITAN>().grabbedTargetEscape();
                        }
                        else
                        {
                            PhotonView.Find(this.titanWhoGrabMeID).RPC("grabbedTargetEscape", PhotonTargets.MasterClient, new object[0]);
                        }
                    }
                }
            }
            else if (this.skillID == "eren")
            {
                this.showSkillCD();
                if (IN_GAME_MAIN_CAMERA.GameType != GameType.Single || (IN_GAME_MAIN_CAMERA.GameType == GameType.Single && !IN_GAME_MAIN_CAMERA.isPausing))
                {
                    this.calcSkillCD();
                    this.calcFlareCD();
                }
                if (InputManager.IsInputDown[InputCode.Attack1])
                {
                    bool flag = false;
                    if (this.skillCDDuration <= 0f && !flag)
                    {
                        this.skillCDDuration = this.skillCDLast;
                        if (this.skillID == "eren" && this.titanWhoGrabMe.GetComponent<TITAN>())
                        {
                            this.ungrabbed();
                            if (IN_GAME_MAIN_CAMERA.GameType == GameType.Single)
                            {
                                this.titanWhoGrabMe.GetComponent<TITAN>().grabbedTargetEscape();
                            }
                            else
                            {
                                BasePV.RPC("netSetIsGrabbedFalse", PhotonTargets.All, new object[0]);
                                if (PhotonNetwork.IsMasterClient)
                                {
                                    this.titanWhoGrabMe.GetComponent<TITAN>().grabbedTargetEscape();
                                }
                                else
                                {
                                    PhotonView.Find(this.titanWhoGrabMeID).BasePV.RPC("grabbedTargetEscape", PhotonTargets.MasterClient, new object[0]);
                                }
                            }
                            this.erenTransform();
                            return;
                        }
                    }
                }
            }
            return;
        }
        if (this.titanForm || isCannon)
        {
            if (isCannon)
            {
                ShowAimUI();
                calcSkillCD();
                showSkillCD();
            }
            return;
        }
        this.bufferUpdate();
        if (!this.grounded && this.State != HeroState.AirDodge)
        {
            this.checkDashDoubleTap();
            if (InputManager.IsInputRebindHolding((int)InputRebinds.GasBurst))
            {
                if (InputManager.IsInput[(int)InputCodes.Forward])
                {
                    dash(0f, 1f);
                }
                else if (InputManager.IsInput[(int)InputCodes.Backward])
                {
                    dash(0f, -1f);
                }
                else if (InputManager.IsInput[(int)InputCodes.Right])
                {
                    dash(1f, 0f);
                }
                else if (InputManager.IsInput[(int)InputCodes.Left])
                {
                    dash(-1f, 0f);
                }
            }
        }
        if (this.grounded && (this.State == HeroState.Idle || this.State == HeroState.Slide))
        {
            if (InputManager.IsInputDown[InputCode.Gas] && !baseA.IsPlaying("jump") && !baseA.IsPlaying("horse_geton"))
            {
                this.idle();
                this.crossFade("jump", 0.1f);
                this.sparks.enableEmission = false;
            }
            if (InputManager.IsInputDown[InputCode.Dodge] && !baseA.IsPlaying("jump") && !baseA.IsPlaying("horse_geton"))
            {
                this.dodge(false);
                return;
            }
        }
        //if (needCheckReelAxis)
        //{
        //    needCheckReelAxis = false;
        //    reelAxis = Input.GetAxis("Mouse ScrollWheel") * 5555f;
        //}
        switch (_state)
        {
            case HeroState.Idle:

                if (InputManager.IsInputDown[InputCode.Flare1])
                {
                    this.shootFlare(1);
                }
                if (InputManager.IsInputDown[InputCode.Flare2])
                {
                    this.shootFlare(2);
                }
                if (InputManager.IsInputDown[InputCode.Flare3])
                {
                    this.shootFlare(3);
                }
                if (InputManager.IsInputDown[InputCode.Restart])
                {
                    this.suicide();
                }
                if (this.myHorse != null && this.isMounted && InputManager.IsInputDown[InputCode.Dodge])
                {
                    this.getOffHorse();
                }
                if ((baseA.IsPlaying(this.standAnimation) || !this.grounded) && InputManager.IsInputDown[InputCode.Reload])
                {
                    if (!Gunner || !GameModes.NoAHSSReload.Enabled)
                    {
                        this.changeBlade();
                        if (Gunner) ShowBullets();
                        else ShowBlades();
                        return;
                    }
                }
                if (baseA.IsPlaying(this.standAnimation) && InputManager.IsInputDown[InputCode.Salute])
                {
                    this.salute();
                    return;
                }
                if (!this.isMounted && (InputManager.IsInputDown[InputCode.Attack0] || InputManager.IsInputDown[InputCode.Attack1]) && !this.Gunner)
                {
                    bool flag2 = false;
                    if (InputManager.IsInputDown[InputCode.Attack1])
                    {
                        if (this.skillCDDuration > 0f || flag2)
                        {
                            flag2 = true;
                        }
                        else
                        {
                            this.skillCDDuration = this.skillCDLast;
                            switch (skillID)
                            {
                                case "eren":
                                    this.erenTransform();
                                    return;

                                case "marco":
                                    if (this.IsGrounded())
                                    {
                                        this.attackAnimation = ((UnityEngine.Random.Range(0, 2) != 0) ? "special_marco_1" : "special_marco_0");
                                        this.playAnimation(this.attackAnimation);
                                    }
                                    else
                                    {
                                        flag2 = true;
                                        this.skillCDDuration = 0f;
                                    }
                                    break;

                                case "armin":
                                    if (this.IsGrounded())
                                    {
                                        this.attackAnimation = "special_armin";
                                        this.playAnimation("special_armin");
                                    }
                                    else
                                    {
                                        flag2 = true;
                                        this.skillCDDuration = 0f;
                                    }
                                    break;

                                case "sasha":
                                    if (this.IsGrounded())
                                    {
                                        this.attackAnimation = "special_sasha";
                                        this.playAnimation("special_sasha");
                                        this.currentBuff = Buff.SpeedUp;
                                        this.buffTime = 10f;
                                    }
                                    else
                                    {
                                        flag2 = true;
                                        this.skillCDDuration = 0f;
                                    }
                                    break;

                                case "mikasa":
                                    this.attackAnimation = "attack3_1";
                                    this.playAnimation("attack3_1");
                                    baseR.velocity = Vectors.up * 10f;
                                    break;

                                case "levi":
                                    this.attackAnimation = "attack5";
                                    this.playAnimation("attack5");
                                    baseR.velocity += Vectors.up * 5f;
                                    Ray ray = IN_GAME_MAIN_CAMERA.BaseCamera.ScreenPointToRay(Input.mousePosition);
                                    RaycastHit hit;
                                    if (Physics.Raycast(ray, out hit, 1E+07f, Layers.EnemyGround.value))
                                    {
                                        if (this.bulletRight)
                                        {
                                            bulletRight.Disable();
                                            this.releaseIfIHookSb();
                                        }
                                        this.dashDirection = hit.point - baseT.position;
                                        this.launchRightRope(hit, true, 1);
                                        this.rope.Play();
                                    }
                                    this.facingDirection = Mathf.Atan2(this.dashDirection.x, this.dashDirection.z) * 57.29578f;
                                    this.targetRotation = Quaternion.Euler(0f, this.facingDirection, 0f);
                                    this.attackLoop = 3;
                                    break;

                                case "petra":
                                    this.attackAnimation = "special_petra";
                                    this.playAnimation("special_petra");
                                    baseR.velocity += Vectors.up * 5f;
                                    Ray ray2 = IN_GAME_MAIN_CAMERA.BaseCamera.ScreenPointToRay(Input.mousePosition);
                                    RaycastHit hit2;
                                    if (Physics.Raycast(ray2, out hit2, 1E+07f, Layers.EnemyGround.value))
                                    {
                                        if (this.bulletRight)
                                        {
                                            bulletRight.Disable();
                                            this.releaseIfIHookSb();
                                        }
                                        if (this.bulletLeft)
                                        {
                                            bulletLeft.Disable();
                                            this.releaseIfIHookSb();
                                        }
                                        this.dashDirection = hit2.point - baseT.position;
                                        this.launchLeftRope(hit2, true, 0);
                                        this.launchRightRope(hit2, true, 0);
                                        this.rope.Play();
                                    }
                                    this.facingDirection = Mathf.Atan2(this.dashDirection.x, this.dashDirection.z) * 57.29578f;
                                    this.targetRotation = Quaternion.Euler(0f, this.facingDirection, 0f);
                                    this.attackLoop = 3;
                                    break;

                                default:
                                    if (this.needLean)
                                    {
                                        if (this.leanLeft)
                                        {
                                            this.attackAnimation = ((UnityEngine.Random.Range(0, 100) >= 50) ? "attack1_hook_l1" : "attack1_hook_l2");
                                        }
                                        else
                                        {
                                            this.attackAnimation = ((UnityEngine.Random.Range(0, 100) >= 50) ? "attack1_hook_r1" : "attack1_hook_r2");
                                        }
                                    }
                                    else
                                    {
                                        this.attackAnimation = "attack1";
                                    }
                                    this.playAnimation(this.attackAnimation);
                                    break;
                            }
                        }
                    }
                    else if (InputManager.IsInputDown[InputCode.Attack0])
                    {
                        if (this.needLean)
                        {
                            if (InputManager.IsInput[InputCode.Left])
                            {
                                this.attackAnimation = ((UnityEngine.Random.Range(0, 100) >= 50) ? "attack1_hook_l1" : "attack1_hook_l2");
                            }
                            else if (InputManager.IsInput[InputCode.Right])
                            {
                                this.attackAnimation = ((UnityEngine.Random.Range(0, 100) >= 50) ? "attack1_hook_r1" : "attack1_hook_r2");
                            }
                            else if (this.leanLeft)
                            {
                                this.attackAnimation = ((UnityEngine.Random.Range(0, 100) >= 50) ? "attack1_hook_l1" : "attack1_hook_l2");
                            }
                            else
                            {
                                this.attackAnimation = ((UnityEngine.Random.Range(0, 100) >= 50) ? "attack1_hook_r1" : "attack1_hook_r2");
                            }
                        }
                        else if (InputManager.IsInput[InputCode.Left])
                        {
                            this.attackAnimation = "attack2";
                        }
                        else if (InputManager.IsInput[InputCode.Right])
                        {
                            this.attackAnimation = "attack1";
                        }
                        else if (this.LastHook != null)
                        {
                            if (LastHook.MyTitan != null)
                            {
                                attackAccordingToTarget(LastHook.MyTitan.Neck);
                            }
                            else
                            {
                                flag2 = true;
                            }
                        }
                        else if ((this.bulletLeft != null) && bulletLeft.MyTitan != null)
                        {
                            attackAccordingToTarget(bulletLeft.MyTitan.Neck);
                        }
                        else if ((this.bulletRight != null) && bulletRight.MyTitan != null)
                        {
                            attackAccordingToTarget(bulletRight.MyTitan.Neck);
                        }
                        else
                        {
                            var obj2 = this.FindNearestTitan();
                            if (obj2 != null)
                            {
                                this.attackAccordingToTarget(obj2.Neck);
                            }
                            else
                            {
                                this.attackAccordingToMouse();
                            }
                        }
                    }
                    if (!flag2)
                    {
                        this.wLeft.clearHits();
                        this.wRight.clearHits();
                        if (this.grounded)
                        {
                            baseR.AddForce(baseGT.forward * 200f);
                        }
                        this.playAnimation(this.attackAnimation);
                        baseA[this.attackAnimation].time = 0f;
                        this.buttonAttackRelease = false;
                        this.State = HeroState.Attack;
                        if (this.grounded || this.attackAnimation == "attack3_1" || this.attackAnimation == "attack5" || this.attackAnimation == "special_petra")
                        {
                            this.attackReleased = true;
                            this.buttonAttackRelease = true;
                        }
                        else
                        {
                            this.attackReleased = false;
                        }
                        this.sparks.enableEmission = false;
                    }
                }
                if (this.Gunner)
                {
                    if (InputManager.IsInput[InputCode.Attack1])
                    {
                        this.leftArmAim = true;
                        this.rightArmAim = true;
                    }
                    else if (InputManager.IsInput[InputCode.Attack0])
                    {
                        if (this.leftGunHasBullet)
                        {
                            this.leftArmAim = true;
                            this.rightArmAim = false;
                        }
                        else
                        {
                            this.leftArmAim = false;
                            if (this.rightGunHasBullet)
                            {
                                this.rightArmAim = true;
                            }
                            else
                            {
                                this.rightArmAim = false;
                            }
                        }
                    }
                    else
                    {
                        this.leftArmAim = false;
                        this.rightArmAim = false;
                    }
                    if (this.leftArmAim || this.rightArmAim)
                    {
                        Ray ray3 = IN_GAME_MAIN_CAMERA.BaseCamera.ScreenPointToRay(Input.mousePosition);
                        RaycastHit raycastHit;
                        if (Physics.Raycast(ray3, out raycastHit, 1E+07f, Layers.EnemyGround.value))
                        {
                            this.gunTarget = raycastHit.point;
                        }
                    }
                    bool flag3 = false;
                    bool flag4 = false;
                    bool flag5 = false;
                    if (InputManager.Settings[InputCode.Attack1].IsUp())
                    {
                        if (this.leftGunHasBullet && this.rightGunHasBullet)
                        {
                            if (this.grounded)
                            {
                                this.attackAnimation = "AHSS_shoot_both";
                            }
                            else
                            {
                                this.attackAnimation = "AHSS_shoot_both_air";
                            }
                            flag3 = true;
                        }
                        else if (!this.leftGunHasBullet && !this.rightGunHasBullet)
                        {
                            flag4 = true;
                        }
                        else
                        {
                            flag5 = true;
                        }
                    }
                    if (flag5 || InputManager.Settings[InputCode.Attack0].IsUp())
                    {
                        if (this.grounded)
                        {
                            if (this.leftGunHasBullet && this.rightGunHasBullet)
                            {
                                if (this.isLeftHandHooked)
                                {
                                    this.attackAnimation = "AHSS_shoot_r";
                                }
                                else
                                {
                                    this.attackAnimation = "AHSS_shoot_l";
                                }
                            }
                            else if (this.leftGunHasBullet)
                            {
                                this.attackAnimation = "AHSS_shoot_l";
                            }
                            else if (this.rightGunHasBullet)
                            {
                                this.attackAnimation = "AHSS_shoot_r";
                            }
                        }
                        else if (this.leftGunHasBullet && this.rightGunHasBullet)
                        {
                            if (this.isLeftHandHooked)
                            {
                                this.attackAnimation = "AHSS_shoot_r_air";
                            }
                            else
                            {
                                this.attackAnimation = "AHSS_shoot_l_air";
                            }
                        }
                        else if (this.leftGunHasBullet)
                        {
                            this.attackAnimation = "AHSS_shoot_l_air";
                        }
                        else if (this.rightGunHasBullet)
                        {
                            this.attackAnimation = "AHSS_shoot_r_air";
                        }
                        if (this.leftGunHasBullet || this.rightGunHasBullet)
                        {
                            flag3 = true;
                        }
                        else
                        {
                            flag4 = true;
                        }
                    }
                    if (flag3)
                    {
                        this.State = HeroState.Attack;
                        this.crossFade(this.attackAnimation, 0.05f);
                        this.gunDummy.transform.position = baseT.position;
                        this.gunDummy.transform.rotation = baseT.rotation;
                        this.gunDummy.transform.LookAt(this.gunTarget);
                        this.attackReleased = false;
                        this.facingDirection = this.gunDummy.transform.rotation.eulerAngles.y;
                        this.targetRotation = Quaternion.Euler(0f, this.facingDirection, 0f);
                        ShowBullets();
                    }
                    else if (flag4 && (this.grounded || FengGameManagerMKII.Level.Mode != GameMode.PVP_AHSS))
                    {
                        this.changeBlade();
                        ShowBullets();
                    }
                }
                break;

            case HeroState.Attack:
                if (!this.Gunner)
                {
                    if (!InputManager.IsInput[InputCode.Attack0])
                    {
                        this.buttonAttackRelease = true;
                    }
                    if (!this.attackReleased)
                    {
                        if (this.buttonAttackRelease)
                        {
                            this.continueAnimation();
                            this.attackReleased = true;
                        }
                        else if (base.baseA[this.attackAnimation].normalizedTime >= 0.32f)
                        {
                            this.pauseAnimation();
                        }
                    }
                    if ((this.attackAnimation == "attack3_1") && (this.currentBladeSta > 0f))
                    {
                        if (base.baseA[this.attackAnimation].normalizedTime >= 0.8f)
                        {
                            if (!wLeft.Active)
                            {
                                wLeft.Active = true;
                                this.leftbladetrail2.Activate();
                                this.rightbladetrail2.Activate();
                                if (QualitySettings.GetQualityLevel() >= 2)
                                {
                                    this.leftbladetrail.Activate();
                                    this.rightbladetrail.Activate();
                                }
                                base.baseR.velocity = (-Vectors.up * 30f);
                            }
                            if (!wRight.Active)
                            {
                                wRight.Active = true;
                                this.slash.Play();
                            }
                        }
                        else if (wLeft.Active)
                        {
                            wLeft.Active = false;
                            wRight.Active = false;
                            wLeft.clearHits();
                            wRight.clearHits();
                            this.leftbladetrail.StopSmoothly(0.1f);
                            this.rightbladetrail.StopSmoothly(0.1f);
                            this.leftbladetrail2.StopSmoothly(0.1f);
                            this.rightbladetrail2.StopSmoothly(0.1f);
                        }
                    }
                    else
                    {
                        float num;
                        float num2;
                        if (this.currentBladeSta == 0f)
                        {
                            num = num2 = -1f;
                        }
                        else
                        {
                            switch (attackAnimation)
                            {
                                case "attack5":
                                    num = 0.35f;
                                    num2 = 0.5f;
                                    break;
                                case "special_petra":
                                    num = 0.35f;
                                    num2 = 0.48f;
                                    break;
                                case "special_armin":
                                    num = 0.25f;
                                    num2 = 0.35f;
                                    break;
                                case "attack4":
                                    num = 0.6f;
                                    num2 = 0.9f;
                                    break;
                                case "special_sasha":
                                    num = num2 = -1f;
                                    break;
                                default:
                                    num = 0.5f;
                                    num2 = 0.85f;
                                    break;
                            }
                        }
                        if ((base.baseA[this.attackAnimation].normalizedTime > num) && (base.baseA[this.attackAnimation].normalizedTime < num2))
                        {
                            if (!wLeft.Active)
                            {
                                wLeft.Active = true;
                                this.slash.Play();
                                this.leftbladetrail2.Activate();
                                this.rightbladetrail2.Activate();
                                if (QualitySettings.GetQualityLevel() >= 2)
                                {
                                    this.leftbladetrail.Activate();
                                    this.rightbladetrail.Activate();
                                }
                            }
                            if (!wRight.Active)
                            {
                                wRight.Active = true;
                            }
                        }
                        else if (wLeft.Active)
                        {
                            wLeft.Active = false;
                            wRight.Active = false;
                            wLeft.clearHits();
                            wRight.clearHits();
                            this.leftbladetrail2.StopSmoothly(0.1f);
                            this.rightbladetrail2.StopSmoothly(0.1f);
                            if (QualitySettings.GetQualityLevel() >= 2)
                            {
                                this.leftbladetrail.StopSmoothly(0.1f);
                                this.rightbladetrail.StopSmoothly(0.1f);
                            }
                        }
                        if ((this.attackLoop > 0) && (base.baseA[this.attackAnimation].normalizedTime > num2))
                        {
                            this.attackLoop--;
                            this.playAnimationAt(this.attackAnimation, num);
                        }
                    }
                    if (base.baseA[this.attackAnimation].normalizedTime >= 1f)
                    {
                        if ((this.attackAnimation == "special_marco_0") || (this.attackAnimation == "special_marco_1"))
                        {
                            if (IN_GAME_MAIN_CAMERA.GameType != GameType.Single)
                            {
                                if (!PhotonNetwork.IsMasterClient)
                                {
                                    object[] parameters = new object[] { 5f, 100f };
                                    base.BasePV.RPC("netTauntAttack", PhotonTargets.MasterClient, parameters);
                                }
                                else
                                {
                                    this.netTauntAttack(5f, 100f);
                                }
                            }
                            else
                            {
                                this.netTauntAttack(5f, 100f);
                            }
                            this.falseAttack();
                            this.idle();
                        }
                        else if (this.attackAnimation == "special_armin")
                        {
                            if (IN_GAME_MAIN_CAMERA.GameType != GameType.Single)
                            {
                                if (!PhotonNetwork.IsMasterClient)
                                {
                                    base.BasePV.RPC("netlaughAttack", PhotonTargets.MasterClient, new object[0]);
                                }
                                else
                                {
                                    this.netlaughAttack();
                                }
                            }
                            else
                            {
                                foreach (GameObject obj3 in GameObject.FindGameObjectsWithTag("titan"))
                                {
                                    if (((Vector3.Distance(obj3.transform.position, base.baseT.position) < 50f) && (Vector3.Angle(obj3.transform.Forward(), base.baseT.position - obj3.transform.position) < 90f)) && (obj3.GetComponent<TITAN>() != null))
                                    {
                                        obj3.GetComponent<TITAN>().beLaughAttacked();
                                    }
                                }
                            }
                            this.falseAttack();
                            this.idle();
                        }
                        else if (this.attackAnimation == "attack3_1")
                        {
                            baseR.velocity -= ((Vectors.up * dt) * 30f);
                        }
                        else
                        {
                            this.falseAttack();
                            this.idle();
                        }
                    }
                    if (base.baseA.IsPlaying("attack3_2") && (base.baseA["attack3_2"].normalizedTime >= 1f))
                    {
                        this.falseAttack();
                        this.idle();
                    }
                }
                else
                {
                    base.baseT.rotation = Quaternion.Lerp(base.baseT.rotation, this.gunDummy.transform.rotation, dt * 30f);
                    if (!this.attackReleased && (base.baseA[this.attackAnimation].normalizedTime > 0.167f))
                    {
                        GameObject obj4;
                        this.attackReleased = true;
                        bool flag6 = false;
                        if ((this.attackAnimation == "AHSS_shoot_both") || (this.attackAnimation == "AHSS_shoot_both_air"))
                        {
                            flag6 = true;
                            this.leftGunHasBullet = false;
                            this.rightGunHasBullet = false;
                            base.baseR.AddForce((Vector3)(-base.baseT.Forward() * 1000f), ForceMode.Acceleration);
                            ShowBullets();
                        }
                        else
                        {
                            if ((this.attackAnimation == "AHSS_shoot_l") || (this.attackAnimation == "AHSS_shoot_l_air"))
                            {
                                this.leftGunHasBullet = false;
                            }
                            else
                            {
                                this.rightGunHasBullet = false;
                            }
                            base.baseR.AddForce((Vector3)(-base.baseT.Forward() * 600f), ForceMode.Acceleration);
                            ShowBullets();
                        }
                        base.baseR.AddForce((Vector3)(Vector3.up * 200f), ForceMode.Acceleration);
                        string prefabName = "FX/shotGun";
                        if (flag6)
                        {
                            prefabName = "FX/shotGun 1";
                        }
                        if ((IN_GAME_MAIN_CAMERA.GameType == GameType.Multi) && base.BasePV.IsMine)
                        {
                            obj4 = Optimization.Caching.Pool.NetworkEnable(prefabName, (Vector3)((base.baseT.position + (base.baseT.Up() * 0.8f)) - (base.baseT.Right() * 0.1f)), base.baseT.rotation, 0);
                            if (obj4.GetComponent<EnemyfxIDcontainer>() != null)
                            {
                                obj4.GetComponent<EnemyfxIDcontainer>().myOwnerViewID = base.BasePV.viewID;
                            }
                        }
                        else
                        {
                            //(GameObject)Instantiate(CacheResources.Load(prefabName), ((base.baseT.position + (base.baseT.Up() * 0.8f)) - (base.baseT.Right() * 0.1f)), base.baseT.rotation);
                            Pool.Enable(prefabName, ((base.baseT.position + (base.baseT.Up() * 0.8f)) - (base.baseT.Right() * 0.1f)), base.baseT.rotation);
                        }
                    }
                    if (base.baseA[this.attackAnimation].normalizedTime >= 1f)
                    {
                        this.falseAttack();
                        this.idle();
                    }
                    if (!base.baseA.IsPlaying(this.attackAnimation))
                    {
                        this.falseAttack();
                        this.idle();
                    }
                }
                break;

            case HeroState.ChangeBlade:
                if (this.Gunner)
                {
                    if (baseA[this.reloadAnimation].normalizedTime > 0.22f)
                    {
                        if (!this.leftGunHasBullet && this.Setup.part_blade_l.activeSelf)
                        {
                            this.Setup.part_blade_l.SetActive(false);
                            Transform transform4 = this.Setup.part_blade_l.transform;
                            GameObject gameObject4 = (GameObject)UnityEngine.Object.Instantiate(CacheResources.Load("Character_parts/character_gun_l"), transform4.position, transform4.rotation);
                            gameObject4.renderer.material = CharacterMaterials.Materials[this.Setup.myCostume._3dmg_texture];
                            Vector3 force = -baseT.Forward() * 10f + baseT.Up() * 5f - baseT.Right();
                            gameObject4.rigidbody.AddForce(force, ForceMode.Impulse);
                            Vector3 torque = new Vector3((float)UnityEngine.Random.Range(-100, 100), (float)UnityEngine.Random.Range(-100, 100), (float)UnityEngine.Random.Range(-100, 100));
                            gameObject4.rigidbody.AddTorque(torque, ForceMode.Acceleration);
                        }
                        if (!this.rightGunHasBullet && this.Setup.part_blade_r.activeSelf)
                        {
                            this.Setup.part_blade_r.SetActive(false);
                            Transform transform5 = this.Setup.part_blade_r.transform;
                            GameObject gameObject5 = (GameObject)UnityEngine.Object.Instantiate(CacheResources.Load("Character_parts/character_gun_r"), transform5.position, transform5.rotation);
                            gameObject5.renderer.material = CharacterMaterials.Materials[this.Setup.myCostume._3dmg_texture];
                            Vector3 force2 = -baseT.Forward() * 10f + baseT.Up() * 5f + baseT.Right();
                            gameObject5.rigidbody.AddForce(force2, ForceMode.Impulse);
                            Vector3 torque2 = new Vector3((float)UnityEngine.Random.Range(-300, 300), (float)UnityEngine.Random.Range(-300, 300), (float)UnityEngine.Random.Range(-300, 300));
                            gameObject5.rigidbody.AddTorque(torque2, ForceMode.Acceleration);
                        }
                    }
                    if (baseA[this.reloadAnimation].normalizedTime > 0.62f && !this.throwedBlades)
                    {
                        this.throwedBlades = true;
                        if (this.leftBulletLeft > 0 && !this.leftGunHasBullet)
                        {
                            this.leftBulletLeft--;
                            this.Setup.part_blade_l.SetActive(true);
                            this.leftGunHasBullet = true;
                            ShowBullets();
                        }
                        if (this.rightBulletLeft > 0 && !this.rightGunHasBullet)
                        {
                            this.Setup.part_blade_r.SetActive(true);
                            this.rightBulletLeft--;
                            this.rightGunHasBullet = true;
                            ShowBullets();
                        }
                        this.UpdateRightMagUI();
                        this.UpdateLeftMagUI();
                        ShowBullets();
                    }
                    if (baseA[this.reloadAnimation].normalizedTime > 1f)
                    {
                        this.idle();
                    }
                }
                else
                {
                    if (!this.grounded)
                    {
                        if (baseA[this.reloadAnimation].normalizedTime >= 0.2f && !this.throwedBlades)
                        {
                            this.throwedBlades = true;
                            if (this.Setup.part_blade_l.activeSelf)
                            {
                                this.throwBlades();
                            }
                        }
                        if (baseA[this.reloadAnimation].normalizedTime >= 0.56f && this.currentBladeNum > 0)
                        {
                            this.Setup.part_blade_l.SetActive(true);
                            this.Setup.part_blade_r.SetActive(true);
                            this.currentBladeSta = this.totalBladeSta;
                            ShowBlades();
                        }
                    }
                    else
                    {
                        if (baseA[this.reloadAnimation].normalizedTime >= 0.13f && !this.throwedBlades)
                        {
                            this.throwedBlades = true;
                            if (this.Setup.part_blade_l.activeSelf)
                            {
                                this.throwBlades();
                            }
                        }
                        if (baseA[this.reloadAnimation].normalizedTime >= 0.37f && this.currentBladeNum > 0)
                        {
                            this.Setup.part_blade_l.SetActive(true);
                            this.Setup.part_blade_r.SetActive(true);
                            this.currentBladeSta = this.totalBladeSta;
                            ShowBlades();
                        }
                    }
                    ShowBlades();
                    if (baseA[this.reloadAnimation].normalizedTime >= 1f)
                    {
                        this.idle();
                    }
                }
                break;

            case HeroState.Salute:
                if (baseA["salute"].normalizedTime >= 1f)
                {
                    this.idle();
                }
                break;

            case HeroState.GroundDodge:
                if (baseA.IsPlaying("dodge"))
                {
                    if (!this.grounded && baseA["dodge"].normalizedTime > 0.6f)
                    {
                        this.idle();
                    }
                    if (baseA["dodge"].normalizedTime >= 1f)
                    {
                        this.idle();
                    }
                }
                break;

            case HeroState.Land:
                if (baseA.IsPlaying("dash_land") && baseA["dash_land"].normalizedTime >= 1f)
                {
                    this.idle();
                }
                break;

            case HeroState.FillGas:
                if (baseA.IsPlaying("supply") && baseA["supply"].normalizedTime >= 1f)
                {
                    this.currentBladeSta = this.totalBladeSta;
                    this.currentBladeNum = this.totalBladeNum;
                    this.currentGas = this.totalGas;
                    ShowGas();
                    if (!this.Gunner)
                    {
                        this.Setup.part_blade_l.SetActive(true);
                        this.Setup.part_blade_r.SetActive(true);
                        ShowBlades();
                    }
                    else
                    {
                        this.leftBulletLeft = (this.rightBulletLeft = this.bulletMAX);
                        this.leftGunHasBullet = (this.rightGunHasBullet = true);
                        this.Setup.part_blade_l.SetActive(true);
                        this.Setup.part_blade_r.SetActive(true);
                        this.UpdateRightMagUI();
                        this.UpdateLeftMagUI();
                        ShowBullets();
                    }
                    this.idle();
                    if (!Gunner) ShowBlades();
                    else ShowBullets();
                }
                break;

            case HeroState.Slide:
                if (!this.grounded)
                {
                    this.idle();
                }
                break;

            case HeroState.AirDodge:
                if (this.dashTime > 0f)
                {
                    this.dashTime -= dt;
                    if (this.currentSpeed > this.originVM)
                    {
                        baseR.AddForce(-baseR.velocity * dt * 1.7f, ForceMode.VelocityChange);
                    }
                }
                else
                {
                    this.dashTime = 0f;
                    this.idle();
                }
                break;
        }
        if (!baseA.IsPlaying("attack3_1") && !baseA.IsPlaying("attack5") && !baseA.IsPlaying("special_petra") && State != HeroState.Grab)
        {
            if (InputManager.IsInput[InputCode.LeftRope])
            {
                if (this.bulletLeft)
                {
                    this.QHold = true;
                }
                else
                {
                    Ray ray4 = IN_GAME_MAIN_CAMERA.BaseCamera.ScreenPointToRay(Input.mousePosition);
                    RaycastHit hit3;
                    if (Physics.Raycast(ray4, out hit3, 10000f, Layers.EnemyGround.value))
                    {
                        this.launchLeftRope(hit3, true, 0);
                        this.rope.Play();
                    }
                }
            }
            else
            {
                this.QHold = false;
            }
            if (InputManager.IsInput[InputCode.RightRope])
            {
                if (this.bulletRight)
                {
                    this.EHold = true;
                }
                else
                {
                    Ray ray5 = IN_GAME_MAIN_CAMERA.BaseCamera.ScreenPointToRay(Input.mousePosition);
                    RaycastHit hit4;
                    if (Physics.Raycast(ray5, out hit4, 10000f, Layers.EnemyGround.value))
                    {
                        this.launchRightRope(hit4, true, 0);
                        this.rope.Play();
                    }
                }
            }
            else
            {
                this.EHold = false;
            }
            if (InputManager.IsInput[InputCode.BothRope])
            {
                this.QHold = true;
                this.EHold = true;
                if (!this.bulletLeft && !this.bulletRight)
                {
                    Ray ray6 = IN_GAME_MAIN_CAMERA.BaseCamera.ScreenPointToRay(Input.mousePosition);
                    RaycastHit hit5;
                    if (Physics.Raycast(ray6, out hit5, 1000000f, Layers.EnemyGround.value))
                    {
                        this.launchLeftRope(hit5, false, 0);
                        this.launchRightRope(hit5, false, 0);
                        this.rope.Play();
                    }
                }
            }
        }
        if (VideoSettings.BladeTrails && !this.Gunner)
        {
            this.leftbladetrail.update();
            this.rightbladetrail.update();
            this.leftbladetrail2.update();
            this.rightbladetrail2.update();
        }
        if (IN_GAME_MAIN_CAMERA.isPausing)
        {
            return;
        }
        this.calcSkillCD();
        this.calcFlareCD();
        this.showSkillCD();
        this.ShowFlareCD();
        this.ShowAimUI();
        float checkAxis = Input.GetAxis("Mouse ScrollWheel");
        if (checkAxis != 0f)
        {
            bool flag2 = false;
            bool flag3 = false;
            if (this.isLaunchLeft && this.bulletLeft != null && bulletLeft.IsHooked())
            {
                this.isLeftHandHooked = true;
                Vector3 vector5 = bulletLeft.baseT.position - baseT.position;
                vector5.Normalize();
                vector5 *= 10f;
                if (!this.isLaunchRight)
                {
                    vector5 *= 2f;
                }
                if (Vector3.Angle(baseR.velocity, vector5) > 90f && InputManager.IsInput[InputCode.Gas])
                {
                    flag2 = true;
                }
            }
            if (this.isLaunchRight && this.bulletRight != null && this.bulletRight.IsHooked())
            {
                this.isRightHandHooked = true;
                Vector3 vector6 = bulletRight.baseT.position - this.baseT.position;
                vector6.Normalize();
                vector6 *= 10f;
                if (!this.isLaunchLeft)
                {
                    vector6 *= 2f;
                }
                if (Vector3.Angle(baseR.velocity, vector6) > 90f && InputManager.IsInput[InputCode.Gas])
                {
                    flag3 = true;
                }
            }
            Vector3 current = Vectors.zero;
            if (flag2 && flag3)
            {
                current = (this.bulletRight.baseT.position + this.bulletLeft.baseT.position) * 0.5f - this.baseT.position;
            }
            else if (flag2 && !flag3)
            {
                current = this.bulletLeft.baseT.position - this.baseT.position;
            }
            else if (flag3 && !flag2)
            {
                current = this.bulletRight.baseT.position - this.baseT.position;
            }
            if (flag2 || flag3)
            {
                this.baseR.AddForce(-this.baseR.velocity, ForceMode.VelocityChange);
                float idk = 1.53938f * (1f + Mathf.Clamp(checkAxis > 0 ? 1f : -1f, -0.8f, 0.8f));
                reelAxis = 0f;
                this.baseR.velocity = Vector3.RotateTowards(current, this.baseR.velocity, idk, idk).normalized * (this.currentSpeed + 0.1f);
            }
        }
    }


    public void UpdateCannon()
    {
        baseT.position = this.myCannonPlayer.position;
        baseT.rotation = this.myCannonBase.rotation;
    }

    public void useBlade(int amount = 0)
    {
        if (amount == 0)
        {
            amount = 1;
        }
        amount *= 2;
        if (this.currentBladeSta > 0f)
        {
            this.currentBladeSta -= amount;
            if (this.currentBladeSta <= 0f)
            {
                if (IsLocal)
                {
                    this.leftbladetrail.Deactivate();
                    this.rightbladetrail.Deactivate();
                    this.leftbladetrail2.Deactivate();
                    this.rightbladetrail2.Deactivate();
                    wLeft.Active = false;
                    wRight.Active = false;
                }
                this.currentBladeSta = 0f;
                this.throwBlades();
            }
        }
        ShowBlades();
    }
}