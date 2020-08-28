using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Anarchy;
using Anarchy.Commands.Chat;
using Anarchy.Configuration;
using Anarchy.Configuration.Presets;
using Anarchy.InputPos;
using Anarchy.Skins.Humans;
using Anarchy.UI;
using Optimization;
using Optimization.Caching;
using Optimization.Caching.Bases;
using RC;
using UnityEngine;
using Xft;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Random = UnityEngine.Random;

[SuppressMessage("ReSharper", "InconsistentNaming")]
[SuppressMessage("ReSharper", "CompareOfFloatsByEqualityOperator")]
public partial class HERO : HeroBase
{
    private const int TotalBladeNum = 5;

    private static List<TITAN> MyTitans = new List<TITAN>();
    private static string mySkinUrl = "";
    private bool animationStopped = false;
    private bool almostSingleHook;
    private string attackAnimation;
    private int attackLoop;
    private bool attackMove;
    private bool attackReleased;
    public AudioSource audio_ally;
    public AudioSource audio_hitwall;
    private GameObject badGuy;
    public float bombCD;
    public bool BombImmune;
    public float bombRadius;
    public float bombSpeed;
    public float bombTime;
    public float bombTimeMax;
    private float buffTime;
    public Bullet bulletLeft;
    private readonly int bulletMAX = 7;
    public Bullet bulletRight;
    private bool buttonAttackRelease;
    public float CameraMultiplier;
    public bool canJump = true;
    public GameObject checkBoxLeft;
    public GameObject checkBoxRight;
    private GameObject crossL1;
    private Transform crossL1T;
    private GameObject crossL2;
    private Transform crossL2T;
    private GameObject crossR1;
    private Transform crossR1T;
    private GameObject crossR2;
    private Transform crossR2T;
    private Transform crossT1;
    private Transform crossT2;
    public string currentAnimation;
    private int currentBladeNum = 5;
    private float currentBladeSta = 100f;
    private Buff currentBuff;
    public Camera currentCamera;
    private float currentGas = 100f;
    public float currentSpeed;
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
    private readonly float flareTotalCD = 30f;
    private readonly float gravity = 20f;
    private bool grounded;
    private GameObject gunDummy;
    public bool Gunner;
    private Vector3 gunTarget;
    private bool hookBySomeOne = true;
    public GameObject hookRefL1;
    public GameObject hookRefL2;
    public GameObject hookRefR1;
    public GameObject hookRefR2;
    private bool hookSomeOne;
    private GameObject hookTarget;
    private float invincible = 3f;
    public bool isCannon;
    private bool isLaunchLeft;
    private bool isLaunchRight;
    private bool isLeftHandHooked;
    private bool isMounted;
    public bool IsPhotonCamera;
    private bool isRightHandHooked;
    public float jumpHeight = 2f;
    private bool justGrounded;
    private UILabel labelDistance;
    private Transform labelT;
    public Bullet LastHook;
    private float launchElapsedTimeL;
    private float launchElapsedTimeR;
    private Vector3 launchForce;
    private Vector3 launchPointLeft;
    private Vector3 launchPointRight;
    private bool leanLeft;
    private bool leftArmAim;
    public XWeaponTrail leftbladetrail;
    public XWeaponTrail leftbladetrail2;
    private int leftBulletLeft = 7;
    private bool leftGunHasBullet = true;
    private float lTapTime = -1f;
    public float maxVelocityChange = 10f;
    public AudioSource meatDie;
    internal Bomb myBomb;
    public GameObject myCannon;
    public Transform myCannonBase;
    public Transform myCannonPlayer;
    public CannonPropRegion myCannonRegion;
    public Group myGroup;
    private GameObject myHorse;
    private GameObject myNetWorkName;
    public float myScale = 1f;
    public int myTeam = 1;
    private bool needLean;
    private Quaternion oldHeadRotation;
    private float originVM;
    private bool qHold;
    private float reelAxis;
    private string reloadAnimation = string.Empty;
    private bool rightArmAim;
    public XWeaponTrail rightbladetrail;
    public XWeaponTrail rightbladetrail2;
    private int rightBulletLeft = 7;
    private bool rightGunHasBullet = true;
    public AudioSource rope;
    private float rTapTime = -1f;
    public HERO_SETUP Setup;
    private GameObject skillCD;
    public float skillCDDuration;
    public float skillCDLast;
    public float skillCDLastCannon;
    private string skillID;
    private string skillIDHUD;
    internal HumanSkin Skin;
    public string[] SkinData;
    public AudioSource slash;
    public AudioSource slashHit;
    private ParticleSystem smoke3Dmg;
    private ParticleSystem sparks;
    private bool spawned;
    public float speed = 10f;
    public GameObject speedFX;
    public GameObject speedFX1;
    private ParticleSystem speedFXPS;
    private string standAnimation = "stand";
    private HeroState state;
    private Quaternion targetHeadRotation;
    private Quaternion targetRotation;
    private Vector3 targetV;
    private bool throwedBlades;
    public bool titanForm;
    private GameObject titanWhoGrabMe;
    private int titanWhoGrabMeID;
    public float totalBladeSta = 100f;
    public float totalGas = 100f;
    private const float UseGasSpeed = 0.2f;
    private float uTapTime = -1f;
    private bool wallJump;
    private float wallRunTime;
    private TriggerColliderWeapon wLeft;
    private TriggerColliderWeapon wRight;
    private SmoothSyncMovement smoothSync;

    public bool IsDead { get; private set; }

    public bool IsGrabbed => State == HeroState.Grab;

    internal SmoothSyncMovement SmoothSync
    {
        get
        {
            if (smoothSync == null)
            {
                smoothSync = GetComponent<SmoothSyncMovement>();
            }
            return smoothSync;
        }
    }

    private HeroState State
    {
        get => state;
        set
        {
            if (state == HeroState.AirDodge || state == HeroState.GroundDodge) dashTime = 0f;
            state = value;
        }
    }

    private static void ApplyForceToBody(GameObject go, Vector3 v)
    {
        go.rigidbody.AddForce(v);
        go.rigidbody.AddTorque(Random.Range(-10f, 10f), Random.Range(-10f, 10f), Random.Range(-10f, 10f));
    }

    private void Awake()
    {
        base.Cache();
        Setup = baseG.GetComponent<HERO_SETUP>();
        baseR.freezeRotation = true;
        baseR.useGravity = false;
        if (IsLocal)
        {
            wLeft = checkBoxLeft.GetComponent<TriggerColliderWeapon>();
            wRight = checkBoxRight.GetComponent<TriggerColliderWeapon>();
            crossT1 = GameObject.Find("cross1").GetComponent<Transform>();
            crossT2 = GameObject.Find("cross2").GetComponent<Transform>();
            crossL1 = GameObject.Find("crossL1");
            crossL2 = GameObject.Find("crossL2");
            crossR1 = GameObject.Find("crossR1");
            crossR2 = GameObject.Find("crossR2");
            crossL1T = crossL1.transform;
            crossL2T = crossL2.transform;
            crossR1T = crossR1.transform;
            crossR2T = crossR2.transform;
            labelDistance = CacheGameObject.Find<UILabel>("LabelDistance");
            labelT = labelDistance.transform;
        }
        else
        {
            if (IN_GAME_MAIN_CAMERA.GameType == GameType.MultiPlayer)
            {
                baseG.layer = Layers.NetworkObjectN;
                GetComponent<CapsuleCollider>().isTrigger = false;
            }
        }
    }

    private void BodyLean()
    {
        if (IsLocal)
        {
            var z = 0f;
            needLean = false;
            if (!grounded && !Gunner && State == HeroState.Attack && attackAnimation != "attack3_1" &&
                attackAnimation != "attack3_2")
            {
                var velocity = baseR.velocity;
                var y = velocity.y;
                var x = velocity.x;
                var z2 = velocity.z;
                var x2 = Mathf.Sqrt(x * x + z2 * z2);
                var num = Mathf.Atan2(y, x2) * 57.29578f;
                targetRotation = Quaternion.Euler(-num * (1f - Vector3.Angle(baseR.velocity, baseT.Forward()) / 90f),
                    facingDirection, 0f);
                if (isLeftHandHooked && bulletLeft != null || isRightHandHooked && bulletRight != null)
                    baseT.rotation = targetRotation;
                return;
            }

            if (isLeftHandHooked && bulletLeft != null && isRightHandHooked && bulletRight != null)
            {
                if (almostSingleHook)
                {
                    needLean = true;
                    z = GetLeanAngle(bulletRight.baseT.position, true);
                }
            }
            else if (isLeftHandHooked && bulletLeft != null)
            {
                needLean = true;
                z = GetLeanAngle(bulletLeft.baseT.position, true);
            }
            else if (isRightHandHooked && bulletRight != null)
            {
                needLean = true;
                z = GetLeanAngle(bulletRight.baseT.position, false);
            }

            if (needLean)
            {
                var num2 = 0f;
                if (!Gunner && State != HeroState.Attack)
                {
                    num2 = currentSpeed * 0.1f;
                    num2 = Mathf.Min(num2, 20f);
                }

                targetRotation = Quaternion.Euler(-num2, facingDirection, z);
            }
            else if (State != HeroState.Attack)
            {
                targetRotation = Quaternion.Euler(0f, facingDirection, 0f);
            }
        }
    }

    private void BombInit()
    {
        skillCDDuration = skillCDLast;
        skillIDHUD = skillID;
        if (GameModes.BombMode.Enabled)
        {
            var rad = Bomb.MyBombRad.Value;
            var range = Bomb.MyBombRange.Value;
            var bSpeed = Bomb.MyBombSpeed.Value;
            var cd = Bomb.MyBombCD.Value;
            var hash = new Hashtable();
            bombCD = cd * -0.4f + 5f;
            bombTimeMax = (range * 60f + 200f) / (bSpeed * 60f + 200f);
            bombRadius = rad * 4f + 20f;
            bombSpeed = bSpeed * 60f + 200f;
            hash.Add("RCBombR", Bomb.MyBombColorR.ToValue());
            hash.Add("RCBombG", Bomb.MyBombColorG.ToValue());
            hash.Add("RCBombB", Bomb.MyBombColorB.ToValue());
            hash.Add("RCBombA", Bomb.MyBombColorA.ToValue());
            hash.Add("RCBombRadius", bombRadius);
            PhotonNetwork.player.SetCustomProperties(hash);
            skillID = "bomb";
            skillIDHUD = "armin";
            skillCDLast = bombCD;
            skillCDDuration = 10f;
            if (FengGameManagerMKII.FGM.logic.RoundTime > 10f) skillCDDuration = 5f;
        }
    }

    private void BombUpdate()
    {
        if (InputManager.IsInputDown[InputCode.Attack1] && skillCDDuration <= 0f)
        {
            if (!(myBomb == null) && !myBomb.disabled) myBomb.Explode(bombRadius);
            var hash = new Hashtable
            {
                { "RCBombR", Bomb.MyBombColorR.ToValue() },
                { "RCBombG", Bomb.MyBombColorG.ToValue() },
                { "RCBombB", Bomb.MyBombColorB.ToValue() },
                { "RCBombA", Bomb.MyBombColorA.ToValue() }
            };
            PhotonNetwork.player.SetCustomProperties(hash);
            detonate = false;
            skillCDDuration = bombCD;
            var ray = IN_GAME_MAIN_CAMERA.BaseCamera.ScreenPointToRay(Input.mousePosition);
            currentV = baseT.position;
            targetV = currentV + Vectors.forward * 200f;
            if (Physics.Raycast(ray, out var hitInfo, 1000000f, Layers.EnemyGround.value)) targetV = hitInfo.point;
            var vector = Vector3.Normalize(targetV - currentV);
            myBomb = Pool.NetworkEnable("RCAsset/BombMain", currentV + vector * 4f, new Quaternion(0f, 0f, 0f, 1f))
                .GetComponent<Bomb>();
            myBomb.SetMyOwner(this);
            myBomb.rigidbody.velocity = vector * bombSpeed;
            bombTime = 0f;
            return;
        }

        if (myBomb != null && !myBomb.disabled)
        {
            bombTime += Time.deltaTime;
            var flag2 = false;
            if (InputManager.Settings[InputCode.Attack1].IsKeyUp())
            {
                detonate = true;
            }
            else if (InputManager.IsInputDown[InputCode.Attack1] && detonate)
            {
                detonate = false;
                flag2 = true;
            }

            if (bombTime >= bombTimeMax) flag2 = true;
            if (flag2)
            {
                myBomb.Explode(bombRadius);
                detonate = false;
            }
        }
    }

    private void BreakApart(Vector3 v, bool isBite)
    {
        var go = (GameObject) Instantiate(CacheResources.Load("Character_parts/AOTTG_HERO_body"), baseT.position,
            baseT.rotation);
        if (go == null) return;
        go.gameObject.GetComponent<HERO_SETUP>().myCostume = Setup.myCostume;
        go.gameObject.GetComponent<HERO_SETUP>().IsDeadBody = true;
        go.GetComponent<HERO_DEAD_BODY_SETUP>()
            .init(currentAnimation, baseA[currentAnimation].normalizedTime, BodyParts.ARM_R);
        if (!isBite)
        {
            var position = baseT.position;
            var rotation = baseT.rotation;
            var gameObject2 = (GameObject) Instantiate(CacheResources.Load("Character_parts/AOTTG_HERO_body"),
                position, rotation);
            var gameObject3 = (GameObject) Instantiate(CacheResources.Load("Character_parts/AOTTG_HERO_body"),
                position, rotation);
            var gameObject4 = (GameObject) Instantiate(CacheResources.Load("Character_parts/AOTTG_HERO_body"),
                position, rotation);
            gameObject2.gameObject.GetComponent<HERO_SETUP>().myCostume = Setup.myCostume;
            gameObject3.gameObject.GetComponent<HERO_SETUP>().myCostume = Setup.myCostume;
            gameObject4.gameObject.GetComponent<HERO_SETUP>().myCostume = Setup.myCostume;
            gameObject2.gameObject.GetComponent<HERO_SETUP>().IsDeadBody = true;
            gameObject3.gameObject.GetComponent<HERO_SETUP>().IsDeadBody = true;
            gameObject4.gameObject.GetComponent<HERO_SETUP>().IsDeadBody = true;
            gameObject2.GetComponent<HERO_DEAD_BODY_SETUP>().init(currentAnimation,
                baseA[currentAnimation].normalizedTime, BodyParts.UPPER);
            gameObject3.GetComponent<HERO_DEAD_BODY_SETUP>().init(currentAnimation,
                baseA[currentAnimation].normalizedTime, BodyParts.LOWER);
            gameObject4.GetComponent<HERO_DEAD_BODY_SETUP>().init(currentAnimation,
                baseA[currentAnimation].normalizedTime, BodyParts.ARM_L);
            ApplyForceToBody(gameObject2, v);
            ApplyForceToBody(gameObject3, v);
            ApplyForceToBody(gameObject4, v);
            if (IN_GAME_MAIN_CAMERA.GameType == GameType.Single || BasePV.IsMine)
                IN_GAME_MAIN_CAMERA.MainCamera.SetMainObject(gameObject2, false);
        }
        else if (IN_GAME_MAIN_CAMERA.GameType == GameType.Single || BasePV.IsMine)
        {
            IN_GAME_MAIN_CAMERA.MainCamera.SetMainObject(go, false);
        }

        ApplyForceToBody(go, v);
        GameObject gameObject5;
        GameObject gameObject6;
        GameObject gameObject7;
        GameObject gameObject8;
        GameObject gameObject9;
        if (Gunner)
        {
            gameObject5 = (GameObject) Instantiate(CacheResources.Load("Character_parts/character_gun_l"),
                Hand_L.position, Hand_L.rotation);
            gameObject6 = (GameObject) Instantiate(CacheResources.Load("Character_parts/character_gun_r"),
                Hand_R.position, Hand_R.rotation);
            var rotation = baseT.rotation;
            var position = baseT.position;
            gameObject7 = (GameObject) Instantiate(CacheResources.Load("Character_parts/character_3dmg_2"),
                position, rotation);
            gameObject8 = (GameObject) Instantiate(CacheResources.Load("Character_parts/character_gun_mag_l"),
                position, rotation);
            gameObject9 = (GameObject) Instantiate(CacheResources.Load("Character_parts/character_gun_mag_r"),
                position, rotation);
        }
        else
        {
            gameObject5 = (GameObject) Instantiate(CacheResources.Load("Character_parts/character_blade_l"),
                Hand_L.position, Hand_L.rotation);
            gameObject6 = (GameObject) Instantiate(CacheResources.Load("Character_parts/character_blade_r"),
                Hand_R.position, Hand_R.rotation);
            var rotation = baseT.rotation;
            var position = baseT.position;
            gameObject7 = (GameObject) Instantiate(CacheResources.Load("Character_parts/character_3dmg"),
                position, rotation);
            gameObject8 = (GameObject) Instantiate(CacheResources.Load("Character_parts/character_3dmg_gas_l"),
                position, rotation);
            gameObject9 = (GameObject) Instantiate(CacheResources.Load("Character_parts/character_3dmg_gas_r"),
                position, rotation);
        }

        gameObject5.renderer.material = CharacterMaterials.Materials[Setup.myCostume._3dmg_texture];
        gameObject6.renderer.material = CharacterMaterials.Materials[Setup.myCostume._3dmg_texture];
        gameObject7.renderer.material = CharacterMaterials.Materials[Setup.myCostume._3dmg_texture];
        gameObject8.renderer.material = CharacterMaterials.Materials[Setup.myCostume._3dmg_texture];
        gameObject9.renderer.material = CharacterMaterials.Materials[Setup.myCostume._3dmg_texture];
        ApplyForceToBody(gameObject5, v);
        ApplyForceToBody(gameObject6, v);
        ApplyForceToBody(gameObject7, v);
        ApplyForceToBody(gameObject8, v);
        ApplyForceToBody(gameObject9, v);
    }

    private void BufferUpdate()
    {
        if (buffTime > 0f)
        {
            buffTime -= Time.deltaTime;
            if (buffTime <= 0f)
            {
                buffTime = 0f;
                if (currentBuff == Buff.SpeedUp && baseA.IsPlaying("run_sasha")) CrossFade("run", 0.1f);
                currentBuff = Buff.NoBuff;
            }
        }
    }

    private void CalcFlareCd()
    {
        if (flare1CD > 0f)
        {
            flare1CD -= Time.deltaTime;
            if (flare1CD < 0f) flare1CD = 0f;
        }

        if (flare2CD > 0f)
        {
            flare2CD -= Time.deltaTime;
            if (flare2CD < 0f) flare2CD = 0f;
        }

        if (flare3CD > 0f)
        {
            flare3CD -= Time.deltaTime;
            if (flare3CD < 0f) flare3CD = 0f;
        }
    }

    private void CalcSkillCd()
    {
        if (skillCDDuration > 0f)
        {
            skillCDDuration -= Time.deltaTime;
            if (skillCDDuration < 0f) skillCDDuration = 0f;
        }
    }

    private float CalculateJumpVerticalSpeed()
    {
        return Mathf.Sqrt(2f * jumpHeight * gravity);
    }

    private void ChangeBlade()
    {
        if (Gunner && !grounded && FengGameManagerMKII.Level.Mode == GameMode.PVP_AHSS) return;
        State = HeroState.ChangeBlade;
        throwedBlades = false;
        if (Gunner)
        {
            if (!leftGunHasBullet && !rightGunHasBullet)
            {
                if (grounded)
                    reloadAnimation = "AHSS_gun_reload_both";
                else
                    reloadAnimation = "AHSS_gun_reload_both_air";
            }
            else if (!leftGunHasBullet)
            {
                if (grounded)
                    reloadAnimation = "AHSS_gun_reload_l";
                else
                    reloadAnimation = "AHSS_gun_reload_l_air";
            }
            else if (!rightGunHasBullet)
            {
                if (grounded)
                    reloadAnimation = "AHSS_gun_reload_r";
                else
                    reloadAnimation = "AHSS_gun_reload_r_air";
            }
            else
            {
                if (grounded)
                    reloadAnimation = "AHSS_gun_reload_both";
                else
                    reloadAnimation = "AHSS_gun_reload_both_air";
                leftGunHasBullet = rightGunHasBullet = false;
            }

            CrossFade(reloadAnimation, 0.05f);
        }
        else
        {
            if (!grounded)
                reloadAnimation = "changeBlade_air";
            else
                reloadAnimation = "changeBlade";
            CrossFade(reloadAnimation, 0.1f);
        }

        if (IN_GAME_MAIN_CAMERA.GameType == GameType.Single) SingleRunStats.Reload();
    }

    private void CheckDashDoubleTap()
    {
        if (uTapTime >= 0f)
        {
            uTapTime += Time.deltaTime;
            if (uTapTime > 0.2f) uTapTime = -1f;
        }

        if (dTapTime >= 0f)
        {
            dTapTime += Time.deltaTime;
            if (dTapTime > 0.2f) dTapTime = -1f;
        }

        if (lTapTime >= 0f)
        {
            lTapTime += Time.deltaTime;
            if (lTapTime > 0.2f) lTapTime = -1f;
        }

        if (rTapTime >= 0f)
        {
            rTapTime += Time.deltaTime;
            if (rTapTime > 0.2f) rTapTime = -1f;
        }

        if (InputManager.IsInputDown[InputCode.Up])
        {
            if (uTapTime == -1f) uTapTime = 0f;
            if (uTapTime != 0f) Dash(0f, 1f);
        }

        if (InputManager.IsInputDown[InputCode.Down])
        {
            if (dTapTime == -1f) dTapTime = 0f;
            if (dTapTime != 0f) Dash(0f, -1f);
        }

        if (InputManager.IsInputDown[InputCode.Left])
        {
            if (lTapTime == -1f) lTapTime = 0f;
            if (lTapTime != 0f) Dash(-1f, 0f);
        }

        if (InputManager.IsInputDown[InputCode.Right])
        {
            if (rTapTime == -1f) rTapTime = 0f;
            if (rTapTime != 0f) Dash(1f, 0f);
        }
    }

    //private static void CheckTitan()
    //{
    //    var hits = Physics.RaycastAll(IN_GAME_MAIN_CAMERA.BaseCamera.ScreenPointToRay(Input.mousePosition), 180f,
    //        Layers.EnemyGroundPlayerAttack.value);
    //    var currentTitans = new List<TITAN>();
    //    var sortedHits = hits.ToList();
    //    sortedHits.Sort((x, y) => x.distance.CompareTo(y.distance));
    //    var maxDistance = 180f;
    //    for (var i = 0; i < hits.Length; i++)
    //    {
    //        var hitObject = hits[i].collider.gameObject;
    //        if (hitObject.layer == 16)
    //        {
    //            if (hitObject.name.Contains("PlayerDetectorRC") && hits[i].distance < maxDistance)
    //            {
    //                hitObject.transform.root.gameObject.GetComponent<TITAN>().IsLook = true;
    //                maxDistance -= 60f;
    //                if (maxDistance <= 60f) i = sortedHits.Count;
    //                var titan = hitObject.transform.root.gameObject.GetComponent<TITAN>();
    //                if (titan != null) currentTitans.Add(titan);
    //            }
    //        }
    //        else
    //        {
    //            i = sortedHits.Count;
    //        }
    //    }

    //    foreach (var oldTitan in MyTitans.Where(oldTitan => !currentTitans.Contains(oldTitan)))
    //        oldTitan.IsLook = false;
    //    foreach (var newTitan in currentTitans) newTitan.IsLook = true;
    //    MyTitans = currentTitans;
    //}

    public void CheckTitan()
    {
        var hits = Physics.RaycastAll(IN_GAME_MAIN_CAMERA.BaseCamera.ScreenPointToRay(Input.mousePosition), 180f,
            Layers.EnemyGroundPlayerAttack.value);
        List<RaycastHit> sortedHits = new List<RaycastHit>();
        List<TITAN> currentTitans = new List<TITAN>();
        Array.ForEach(hits, delegate (RaycastHit hit)
        {
            sortedHits.Add(hit);
        });
        sortedHits.Sort((RaycastHit x, RaycastHit y) => x.distance.CompareTo(y.distance));
        float num = 180f;
        for (int i = 0; i < sortedHits.Count; i++)
        {
            GameObject gameObject = sortedHits[i].collider.gameObject;
            if (gameObject.layer == 16)
            {
                if (gameObject.name.Contains("PlayerDetectorRC") && sortedHits[i].distance < num)
                {
                    num -= 60f;
                    if (num <= 60f)
                    {
                        i = sortedHits.Count;
                    }
                    TITAN component = gameObject.transform.root.gameObject.GetComponent<TITAN>();
                    if (component != null)
                    {
                        currentTitans.Add(component);
                    }
                }
            }
            else
            {
                i = sortedHits.Count;
            }
        }
        Array.ForEach(MyTitans.ToArray(), delegate (TITAN oldTitan)
        {
            if (!currentTitans.Contains(oldTitan))
            {
                oldTitan.IsLook = false;
            }
        });
        Array.ForEach(currentTitans.ToArray(), delegate (TITAN newTitan)
        {
            newTitan.IsLook = true;
        });
        MyTitans = currentTitans;
    }

    private void CustomAnimationSpeed()
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

    private void Dash(float horizontal, float vertical)
    {
        if (dashTime > 0f) return;
        if (currentGas <= 0f) return;
        if (isMounted) return;
        UseGas(totalGas * 0.04f);
        facingDirection = GetGlobalFacingDirection(horizontal, vertical);
        dashV = GetGlobaleFacingVector3(facingDirection);
        originVM = currentSpeed;
        var rotation = Quaternion.Euler(0f, facingDirection, 0f);
        baseR.rotation = rotation;
        targetRotation = rotation;
        if (IN_GAME_MAIN_CAMERA.GameType == GameType.Single)
            Pool.Enable("FX/boost_smoke", baseT.position, baseT.rotation);
        else
            Pool.NetworkEnable("FX/boost_smoke", baseT.position, baseT.rotation);
        dashTime = 0.5f;
        CrossFade("dash", 0.1f);
        baseA["dash"].time = 0.1f;
        State = HeroState.AirDodge;
        FalseAttack();
        baseR.AddForce(dashV * 40f, ForceMode.VelocityChange);
    }

    private void Dodge(bool offTheWall = false)
    {
        if (myHorse != null && !isMounted && Vector3.Distance(myHorse.transform.position, baseT.position) < 15f)
        {
            getOnHorse();
            return;
        }

        State = HeroState.GroundDodge;
        if (!offTheWall)
        {
            float num;
            if (InputManager.IsInput[InputCode.Up])
                num = 1f;
            else if (InputManager.IsInput[InputCode.Down])
                num = -1f;
            else
                num = 0f;
            float num2;
            if (InputManager.IsInput[InputCode.Left])
                num2 = -1f;
            else if (InputManager.IsInput[InputCode.Right])
                num2 = 1f;
            else
                num2 = 0f;
            var globalFacingDirection = GetGlobalFacingDirection(num2, num);
            if (num2 != 0f || num != 0f)
            {
                facingDirection = globalFacingDirection + 180f;
                targetRotation = Quaternion.Euler(0f, facingDirection, 0f);
            }

            CrossFade("dodge", 0.1f);
        }
        else
        {
            PlayAnimation("dodge");
            PlayAnimationAt("dodge", 0.2f);
        }

        sparks.enableEmission = false;
    }

    private void ErenTransform()
    {
        skillCDDuration = skillCDLast;
        if (bulletLeft) bulletLeft.RemoveMe();
        if (bulletRight) bulletRight.RemoveMe();
        if (IN_GAME_MAIN_CAMERA.GameType == GameType.Single)
            eren = (GameObject) Instantiate(CacheResources.Load("TITAN_EREN"), baseT.position, baseT.rotation);
        else
            eren = Pool.NetworkEnable("TITAN_EREN", baseT.position, baseT.rotation);
        eren.GetComponent<TITAN_EREN>().realBody = baseG;
        IN_GAME_MAIN_CAMERA.MainCamera.flashBlind();
        IN_GAME_MAIN_CAMERA.MainCamera.SetMainObject(eren.GetComponent<TITAN_EREN>());
        eren.GetComponent<TITAN_EREN>().born();
        eren.rigidbody.velocity = baseR.velocity;
        baseR.velocity = Vectors.zero;
        baseT.position = eren.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/neck").position;
        titanForm = true;
        if (IN_GAME_MAIN_CAMERA.GameType != GameType.Single)
            BasePV.RPC("whoIsMyErenTitan", PhotonTargets.Others, eren.GetPhotonView().viewID);
        if (smoke3Dmg.enableEmission && IN_GAME_MAIN_CAMERA.GameType != GameType.Single && BasePV.IsMine)
            BasePV.RPC("net3DMGSMOKE", PhotonTargets.Others, false);
        smoke3Dmg.enableEmission = false;
    }
    
    private TITAN FindNearestTitan()
    {
        TITAN res = null;
        var positiveInfinity = float.PositiveInfinity;
        var position = baseT.position;
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
        if (isCannon || titanForm ||
            IN_GAME_MAIN_CAMERA.isPausing && IN_GAME_MAIN_CAMERA.GameType == GameType.Single) return;
        currentSpeed = baseR.velocity.magnitude;
        if (!IsLocal)
            return;
        if (!baseA.IsPlaying("attack3_2") && !baseA.IsPlaying("attack5") && !baseA.IsPlaying("special_petra"))
            baseR.rotation = Quaternion.Lerp(baseGT.rotation, targetRotation, Time.fixedDeltaTime * 6f);
        if (State == HeroState.Grab)
        {
            baseR.AddForce(-baseR.velocity, ForceMode.VelocityChange);
            return;
        }

        if (IsGrounded())
        {
            if (!grounded) justGrounded = true;
            grounded = true;
        }
        else
        {
            grounded = false;
        }

        if (hookSomeOne)
        {
            if (hookTarget != null)
            {
                var vector = hookTarget.transform.position - baseT.position;
                var magnitude = vector.magnitude;
                if (magnitude > 2f)
                    baseR.AddForce(vector.normalized * Mathf.Pow(magnitude, 0.15f) * 30f - baseR.velocity * 0.95f,
                        ForceMode.VelocityChange);
            }
            else
            {
                hookSomeOne = false;
            }
        }
        else if (hookBySomeOne && badGuy != null)
        {
            if (badGuy != null)
            {
                var vector2 = badGuy.transform.position - baseT.position;
                var magnitude2 = vector2.magnitude;
                if (magnitude2 > 5f)
                    baseR.AddForce(vector2.normalized * Mathf.Pow(magnitude2, 0.15f) * 0.2f, ForceMode.Impulse);
            }
            else
            {
                hookBySomeOne = false;
            }
        }

        var num = 0f;
        var num2 = 0f;
        if (!IN_GAME_MAIN_CAMERA.isPausing && !IN_GAME_MAIN_CAMERA.isTyping)
        {
            if (InputManager.IsInput[InputCode.Up])
                num2 = 1f;
            else if (InputManager.IsInput[InputCode.Down]) num2 = -1f;
            if (InputManager.IsInput[InputCode.Left])
                num = -1f;
            else if (InputManager.IsInput[InputCode.Right]) num = 1f;
        }

        var flag = false;
        var flag2 = false;
        var flag3 = false;
        isLeftHandHooked = false;
        isRightHandHooked = false;
        if (isLaunchLeft)
        {
            if (bulletLeft != null && bulletLeft.IsHooked())
            {
                isLeftHandHooked = true;
                var vector3 = bulletLeft.transform.position - baseT.position;
                vector3.Normalize();
                vector3 *= 10f;
                if (!isLaunchRight) vector3 *= 2f;
                if (Vector3.Angle(baseR.velocity, vector3) > 90f && InputManager.IsInput[InputCode.Gas])
                {
                    flag2 = true;
                    flag = true;
                }

                if (!flag2)
                {
                    baseR.AddForce(vector3);
                    if (Vector3.Angle(baseR.velocity, vector3) > 90f)
                        baseR.AddForce(-baseR.velocity * 2f, ForceMode.Acceleration);
                }
            }

            launchElapsedTimeL += Time.deltaTime;
            if (qHold && currentGas > 0f)
            {
                UseGas(UseGasSpeed * Time.deltaTime);
            }
            else if (launchElapsedTimeL > 0.3f)
            {
                isLaunchLeft = false;
                if (bulletLeft != null)
                {
                    var component = bulletLeft;
                    component.Disable();
                    ReleaseIfIHookSb();
                    bulletLeft = null;
                    flag2 = false;
                }
            }
        }

        if (isLaunchRight)
        {
            if (bulletRight != null && bulletRight.IsHooked())
            {
                isRightHandHooked = true;
                var vector4 = bulletRight.transform.position - baseT.position;
                vector4.Normalize();
                vector4 *= 10f;
                if (!isLaunchLeft) vector4 *= 2f;
                if (Vector3.Angle(baseR.velocity, vector4) > 90f && InputManager.IsInput[InputCode.Gas])
                {
                    flag3 = true;
                    flag = true;
                }

                if (!flag3)
                {
                    baseR.AddForce(vector4);
                    if (Vector3.Angle(baseR.velocity, vector4) > 90f)
                        baseR.AddForce(-baseR.velocity * 2f, ForceMode.Acceleration);
                }
            }

            launchElapsedTimeR += Time.deltaTime;
            if (EHold && currentGas > 0f)
            {
                UseGas(UseGasSpeed * Time.deltaTime);
            }
            else if (launchElapsedTimeR > 0.3f)
            {
                isLaunchRight = false;
                if (bulletRight != null)
                {
                    var component2 = bulletRight;
                    component2.Disable();
                    ReleaseIfIHookSb();
                    bulletRight = null;
                    flag3 = false;
                }
            }
        }

        if (grounded)
        {
            var a = Vectors.zero;
            if (State == HeroState.Attack)
            {
                if (attackAnimation == "attack5")
                {
                    if (baseA[attackAnimation].normalizedTime > 0.4f && baseA[attackAnimation].normalizedTime < 0.61f)
                        baseR.AddForce(baseGT.forward * 200f);
                }
                else if (attackAnimation == "special_petra")
                {
                    if (baseA[attackAnimation].normalizedTime > 0.35f && baseA[attackAnimation].normalizedTime < 0.48f)
                        baseR.AddForce(baseGT.forward * 200f);
                }
                else if (baseA.IsPlaying("attack3_2"))
                {
                    a = Vectors.zero;
                }
                else if (baseA.IsPlaying("attack1") || baseA.IsPlaying("attack2"))
                {
                    baseR.AddForce(baseGT.forward * 200f);
                }

                if (baseA.IsPlaying("attack3_2")) a = Vectors.zero;
            }

            if (justGrounded)
            {
                if (State != HeroState.Attack || attackAnimation != "attack3_1" && attackAnimation != "attack5" &&
                    attackAnimation != "special_petra")
                {
                    if (State != HeroState.Attack && num == 0f && num2 == 0f && !bulletLeft && !bulletRight &&
                        State != HeroState.FillGas)
                    {
                        State = HeroState.Land;
                        CrossFade("dash_land", 0.01f);
                    }
                    else
                    {
                        buttonAttackRelease = true;
                        if (State != HeroState.Attack &&
                            baseR.velocity.x * baseR.velocity.x + baseR.velocity.z * baseR.velocity.z >
                            speed * speed * 1.5f && State != HeroState.FillGas)
                        {
                            State = HeroState.Slide;
                            CrossFade("slide", 0.05f);
                            var velocity1 = baseR.velocity;
                            facingDirection = Mathf.Atan2(velocity1.x, velocity1.z) * 57.29578f;
                            targetRotation = Quaternion.Euler(0f, facingDirection, 0f);
                            sparks.enableEmission = true;
                        }
                    }
                }

                justGrounded = false;
                a = baseR.velocity;
            }

            if (State == HeroState.Attack && attackAnimation == "attack3_1" &&
                baseA[attackAnimation].normalizedTime >= 1f)
            {
                PlayAnimation("attack3_2");
                ResetAnimationSpeed();
                var zero = Vectors.zero;
                baseR.velocity = zero;
                a = zero;
                IN_GAME_MAIN_CAMERA.MainCamera.startShake(0.2f, 0.3f);
            }

            switch (State)
            {
                case HeroState.GroundDodge:
                {
                    if (baseA["dodge"].normalizedTime >= 0.2f && baseA["dodge"].normalizedTime < 0.8f)
                        a = -baseT.Forward() * 2.4f * speed;
                    if (baseA["dodge"].normalizedTime > 0.8f)
                    {
                        a = baseR.velocity;
                        a *= 0.9f;
                    }

                    break;
                }
                case HeroState.Idle:
                {
                    var vector5 = new Vector3(num, 0f, num2);
                    var num3 = GetGlobalFacingDirection(num, num2);
                    a = GetGlobaleFacingVector3(num3);
                    var d = vector5.magnitude <= 0.95f ? vector5.magnitude >= 0.25f ? vector5.magnitude : 0f : 1f;
                    a *= d;
                    a *= speed;
                    if (buffTime > 0f && currentBuff == Buff.SpeedUp) a *= 4f;
                    if (num != 0f || num2 != 0f)
                    {
                        if (!baseA.IsPlaying("run") && !baseA.IsPlaying("jump") && !baseA.IsPlaying("run_sasha") &&
                            (!baseA.IsPlaying("horse_geton") || baseA["horse_geton"].normalizedTime >= 0.5f))
                        {
                            if (buffTime > 0f && currentBuff == Buff.SpeedUp)
                                CrossFade("run_sasha", 0.1f);
                            else
                                CrossFade("run", 0.1f);
                        }
                    }
                    else
                    {
                        if (!baseA.IsPlaying(standAnimation) && State != HeroState.Land && !baseA.IsPlaying("jump") &&
                            !baseA.IsPlaying("horse_geton") && !baseA.IsPlaying("grabbed"))
                        {
                            CrossFade(standAnimation, 0.1f);
                            a *= 0f;
                        }

                        num3 = -874f;
                    }

                    if (num3 != -874f)
                    {
                        facingDirection = num3;
                        targetRotation = Quaternion.Euler(0f, facingDirection, 0f);
                    }

                    break;
                }
                case HeroState.Land:
                    a = baseR.velocity;
                    a *= 0.96f;
                    break;
                case HeroState.Slide:
                {
                    a = baseR.velocity;
                    a *= 0.99f;
                    if (currentSpeed < speed * 1.2f)
                    {
                        idle();
                        sparks.enableEmission = false;
                    }

                    break;
                }
            }

            var velocity = baseR.velocity;
            var vector6 = a - velocity;
            vector6.x = Mathf.Clamp(vector6.x, -maxVelocityChange, maxVelocityChange);
            vector6.z = Mathf.Clamp(vector6.z, -maxVelocityChange, maxVelocityChange);
            vector6.y = 0f;
            if (baseA.IsPlaying("jump") && baseA["jump"].normalizedTime > 0.18f) vector6.y += 8f;
            if (baseA.IsPlaying("horse_geton") && baseA["horse_geton"].normalizedTime > 0.18f &&
                baseA["horse_geton"].normalizedTime < 1f)
            {
                var num4 = 6f;
                vector6 = -baseR.velocity;
                vector6.y = num4;
                var position = myHorse.transform.position;
                var position1 = baseT.position;
                var num5 = Vector3.Distance(position, position1);
                var d2 = 0.6f * gravity * num5 / (2f * num4);
                vector6 += d2 * (position - position1).normalized;
            }

            if (State != HeroState.Attack || !Gunner)
            {
                baseR.AddForce(vector6, ForceMode.VelocityChange);
                baseR.rotation = Quaternion.Lerp(baseGT.rotation, Quaternion.Euler(0f, facingDirection, 0f),
                    Time.deltaTime * 10f);
            }
        }
        else
        {
            if (sparks.enableEmission) sparks.enableEmission = false;
            if (myHorse != null && (baseA.IsPlaying("horse_geton") || baseA.IsPlaying("air_fall")) &&
                baseR.velocity.y < 0f &&
                Vector3.Distance(myHorse.transform.position + Vectors.up * 1.65f, baseT.position) < 0.5f)
            {
                baseT.position = myHorse.transform.position + Vectors.up * 1.65f;
                baseT.rotation = myHorse.transform.rotation;
                isMounted = true;
                CrossFade("horse_idle", 0.1f);
                myHorse.GetComponent<Horse>().mounted();
            }

            if (State == HeroState.Idle && !baseA.IsPlaying("dash") && !baseA.IsPlaying("wallrun") &&
                !baseA.IsPlaying("toRoof") && !baseA.IsPlaying("horse_geton") && !baseA.IsPlaying("horse_getoff") &&
                !baseA.IsPlaying("air_release") && !isMounted &&
                (!baseA.IsPlaying("air_hook_l_just") || baseA["air_hook_l_just"].normalizedTime >= 1f) &&
                (!baseA.IsPlaying("air_hook_r_just") || baseA["air_hook_r_just"].normalizedTime >= 1f) ||
                baseA["dash"].normalizedTime >= 0.99f)
            {
                if (!isLeftHandHooked && !isRightHandHooked &&
                    (baseA.IsPlaying("air_hook_l") || baseA.IsPlaying("air_hook_r") || baseA.IsPlaying("air_hook")) &&
                    baseR.velocity.y > 20f)
                {
                    baseA.CrossFade("air_release");
                }
                else
                {
                    var flag4 = Mathf.Abs(baseR.velocity.x) + Mathf.Abs(baseR.velocity.z) > 25f;
                    var flag5 = baseR.velocity.y < 0f;
                    if (!flag4)
                    {
                        if (flag5)
                        {
                            if (!baseA.IsPlaying("air_fall")) CrossFade("air_fall", 0.2f);
                        }
                        else if (!baseA.IsPlaying("air_rise"))
                        {
                            CrossFade("air_rise", 0.2f);
                        }
                    }
                    else if (!isLeftHandHooked && !isRightHandHooked)
                    {
                        var velocity = baseR.velocity;
                        var cr = -Mathf.Atan2(velocity.z, velocity.x) * 57.29578f;
                        var num6 = -Mathf.DeltaAngle(cr, baseT.rotation.eulerAngles.y - 90f);
                        if (Mathf.Abs(num6) < 45f)
                        {
                            if (!baseA.IsPlaying("air2")) CrossFade("air2", 0.2f);
                        }
                        else if (num6 < 135f && num6 > 0f)
                        {
                            if (!baseA.IsPlaying("air2_right")) CrossFade("air2_right", 0.2f);
                        }
                        else if (num6 > -135f && num6 < 0f)
                        {
                            if (!baseA.IsPlaying("air2_left")) CrossFade("air2_left", 0.2f);
                        }
                        else if (!baseA.IsPlaying("air2_backward"))
                        {
                            CrossFade("air2_backward", 0.2f);
                        }
                    }
                    else if (Gunner)
                    {
                        if (!isRightHandHooked)
                        {
                            if (!baseA.IsPlaying("AHSS_hook_forward_l")) CrossFade("AHSS_hook_forward_l", 0.1f);
                        }
                        else if (!isLeftHandHooked)
                        {
                            if (!baseA.IsPlaying("AHSS_hook_forward_r")) CrossFade("AHSS_hook_forward_r", 0.1f);
                        }
                        else if (!baseA.IsPlaying("AHSS_hook_forward_both"))
                        {
                            CrossFade("AHSS_hook_forward_both", 0.1f);
                        }
                    }
                    else if (!isRightHandHooked)
                    {
                        if (!baseA.IsPlaying("air_hook_l")) CrossFade("air_hook_l", 0.1f);
                    }
                    else if (!isLeftHandHooked)
                    {
                        if (!baseA.IsPlaying("air_hook_r")) CrossFade("air_hook_r", 0.1f);
                    }
                    else if (!baseA.IsPlaying("air_hook"))
                    {
                        CrossFade("air_hook", 0.1f);
                    }
                }
            }

            if (State == HeroState.Idle && baseA.IsPlaying("air_release") && baseA["air_release"].normalizedTime >= 1f)
                CrossFade("air_rise", 0.2f);
            if (baseA.IsPlaying("horse_getoff") && baseA["horse_getoff"].normalizedTime >= 1f)
                CrossFade("air_rise", 0.2f);
            if (baseA.IsPlaying("toRoof"))
            {
                if (baseA["toRoof"].normalizedTime < 0.22f)
                {
                    baseR.velocity = Vectors.zero;
                    baseR.AddForce(new Vector3(0f, gravity * baseR.mass, 0f));
                }
                else
                {
                    if (!wallJump)
                    {
                        wallJump = true;
                        baseR.AddForce(Vectors.up * 8f, ForceMode.Impulse);
                    }

                    baseR.AddForce(baseT.Forward() * 0.05f, ForceMode.Impulse);
                }

                if (baseA["toRoof"].normalizedTime >= 1f) PlayAnimation("air_rise");
            }
            else if (State == HeroState.Idle && IsPressDirectionTowardsHero(num, num2) &&
                     !InputManager.IsInput[InputCode.Gas] && !InputManager.IsInput[InputCode.LeftRope] &&
                     !InputManager.IsInput[InputCode.RightRope] && !InputManager.IsInput[InputCode.BothRope] &&
                     IsFrontGrounded() && !baseA.IsPlaying("wallrun") && !baseA.IsPlaying("dodge"))
            {
                CrossFade("wallrun", 0.1f);
                wallRunTime = 0f;
            }
            else if (baseA.IsPlaying("wallrun"))
            {
                baseR.AddForce(Vectors.up * speed - baseR.velocity, ForceMode.VelocityChange);
                wallRunTime += Time.deltaTime;
                if (wallRunTime > 1f || num2 == 0f && num == 0f)
                {
                    baseR.AddForce(-baseT.Forward() * speed * 0.75f, ForceMode.Impulse);
                    Dodge(true);
                }
                else if (!IsUpFrontGrounded())
                {
                    wallJump = false;
                    CrossFade("toRoof", 0.1f);
                }
                else if (!IsFrontGrounded())
                {
                    CrossFade("air_fall", 0.1f);
                }
            }
            else if (!baseA.IsPlaying("attack5") && !baseA.IsPlaying("special_petra") && !baseA.IsPlaying("dash") &&
                     !baseA.IsPlaying("jump"))
            {
                var vector7 = new Vector3(num, 0f, num2);
                var num7 = GetGlobalFacingDirection(num, num2);
                var vector8 = GetGlobaleFacingVector3(num7);
                var d3 = vector7.magnitude <= 0.95f ? vector7.magnitude >= 0.25f ? vector7.magnitude : 0f : 1f;
                vector8 *= d3;
                vector8 *= Setup.myCostume.stat.Acl / 10f * 2f;
                if (num == 0f && num2 == 0f)
                {
                    if (State == HeroState.Attack) vector8 *= 0f;
                    num7 = -874f;
                }

                if (num7 != -874f)
                {
                    facingDirection = num7;
                    targetRotation = Quaternion.Euler(0f, facingDirection, 0f);
                }

                if (!flag2 && !flag3 && !isMounted && InputManager.IsInput[InputCode.Gas] && currentGas > 0f)
                {
                    if (num != 0f || num2 != 0f)
                        baseR.AddForce(vector8, ForceMode.Acceleration);
                    else
                        baseR.AddForce(baseT.Forward() * vector8.magnitude, ForceMode.Acceleration);
                    flag = true;
                }
            }

            if (baseA.IsPlaying("air_fall") && currentSpeed < 0.2f && IsFrontGrounded()) CrossFade("onWall", 0.3f);
        }

        var current = Vectors.zero;
        if (flag2 && flag3)
            current = (bulletRight.baseT.position + bulletLeft.baseT.position) * 0.5f - baseT.position;
        else if (flag2 && !flag3)
            current = bulletLeft.baseT.position - baseT.position;
        else if (flag3 && !flag2) current = bulletRight.baseT.position - baseT.position;
        if (flag2 || flag3)
        {
            baseR.AddForce(-baseR.velocity, ForceMode.VelocityChange);
            if (InputManager.IsInputRebindHolding((int) InputRebinds.ReelIn))
                reelAxis = -1f;
            else if (InputManager.IsInputRebindHolding((int) InputRebinds.ReelOut))
                reelAxis = 1f;
            else
                reelAxis = Input.GetAxis("Mouse ScrollWheel") * 5555f;
            var idk = 1.53938f * (1f + Mathf.Clamp(reelAxis, -0.8f, 0.8f));
            reelAxis = 0f;
            baseR.velocity = Vector3.RotateTowards(current, baseR.velocity, idk, idk).normalized *
                             (currentSpeed + 0.1f);
        }

        if (State == HeroState.Attack && (attackAnimation == "attack5" || attackAnimation == "special_petra") &&
            baseA[attackAnimation].normalizedTime > 0.4f && !attackMove)
        {
            attackMove = true;
            if (launchPointRight.magnitude > 0f)
            {
                var vector9 = launchPointRight - baseT.position;
                vector9.Normalize();
                vector9 *= 13f;
                baseR.AddForce(vector9, ForceMode.Impulse);
            }

            if (attackAnimation == "special_petra" && launchPointLeft.magnitude > 0f)
            {
                var vector10 = launchPointLeft - baseT.position;
                vector10.Normalize();
                vector10 *= 13f;
                baseR.AddForce(vector10, ForceMode.Impulse);
                if (bulletRight)
                {
                    bulletRight.Disable();
                    ReleaseIfIHookSb();
                }

                if (bulletLeft)
                {
                    bulletLeft.Disable();
                    ReleaseIfIHookSb();
                }
            }

            baseR.AddForce(Vectors.up * 2f, ForceMode.Impulse);
        }

        var flag6 = false;
        if (bulletLeft != null || bulletRight != null)
        {
            if (bulletLeft && bulletLeft.transform.position.y > baseGT.position.y && isLaunchLeft &&
                bulletLeft.IsHooked()) flag6 = true;
            if (bulletRight && bulletRight.transform.position.y > baseGT.position.y && isLaunchRight &&
                bulletRight.IsHooked()) flag6 = true;
        }

        if (flag6)
            baseR.AddForce(new Vector3(0f, -10f * baseR.mass, 0f));
        else
            baseR.AddForce(new Vector3(0f, -gravity * baseR.mass, 0f));
        if (this.currentSpeed > 10f)
        {
            IN_GAME_MAIN_CAMERA.BaseCamera.fieldOfView =
 Mathf.Lerp(IN_GAME_MAIN_CAMERA.BaseCamera.fieldOfView, Mathf.Min(100f, this.currentSpeed + 40f), 0.1f);
        }
        else
        {
            IN_GAME_MAIN_CAMERA.BaseCamera.fieldOfView =
 Mathf.Lerp(IN_GAME_MAIN_CAMERA.BaseCamera.fieldOfView, 50f, 0.1f);
        }
        if (flag)
        {
            UseGas(UseGasSpeed * Time.deltaTime);
            if (!smoke3Dmg.enableEmission && IN_GAME_MAIN_CAMERA.GameType != GameType.Single)
                BasePV.RPC("net3DMGSMOKE", PhotonTargets.Others, true);
            smoke3Dmg.enableEmission = true;
        }
        else
        {
            if (smoke3Dmg.enableEmission && IN_GAME_MAIN_CAMERA.GameType != GameType.Single)
                BasePV.RPC("net3DMGSMOKE", PhotonTargets.Others, false);
            smoke3Dmg.enableEmission = false;
        }

        if (VideoSettings.WindEffect.Value)
        {
            if (currentSpeed > 80f)
            {
                if (!speedFXPS.enableEmission) speedFXPS.enableEmission = true;
                speedFXPS.startSpeed = currentSpeed;
                speedFX.transform.LookAt(baseT.position + baseR.velocity);
            }
            else if (speedFXPS.enableEmission)
            {
                speedFXPS.enableEmission = false;
            }
        }
    }

    private Vector3 GetGlobaleFacingVector3(float horizontal, float vertical)
    {
        var num = -GetGlobalFacingDirection(horizontal, vertical) + 90f;
        var x = Mathf.Cos(num * 0.0174532924f);
        var z = Mathf.Sin(num * 0.0174532924f);
        return new Vector3(x, 0f, z);
    }

    private static Vector3 GetGlobaleFacingVector3(float resultAngle)
    {
        var num = -resultAngle + 90f;
        var x = Mathf.Cos(num * 0.0174532924f);
        var z = Mathf.Sin(num * 0.0174532924f);
        return new Vector3(x, 0f, z);
    }

    private float GetGlobalFacingDirection(float horizontal, float vertical)
    {
        if (vertical == 0f && horizontal == 0f) return baseT.rotation.eulerAngles.y;
        var y = IN_GAME_MAIN_CAMERA.MainCamera.transform.rotation.eulerAngles.y;
        var num = Mathf.Atan2(vertical, horizontal) * 57.29578f;
        num = -num + 90f;
        return y + num;
    }

    private float GetLeanAngle(Vector3 p, bool left)
    {
        if (!Gunner && State == HeroState.Attack) return 0f;
        var position = baseT.position;
        var num = p.y - position.y;
        var num2 = Vector3.Distance(p, position);
        var num3 = Mathf.Acos(num / num2) * 57.29578f;
        num3 *= 0.1f;
        num3 *= 1f + Mathf.Pow(baseR.velocity.magnitude, 0.2f);
        var vector = p - position;
        var current = Mathf.Atan2(vector.x, vector.z) * 57.29578f;
        var velocity = baseR.velocity;
        var target = Mathf.Atan2(velocity.x, velocity.z) * 57.29578f;
        var num4 = Mathf.DeltaAngle(current, target);
        num3 += Mathf.Abs(num4 * 0.5f);
        if (State != HeroState.Attack) num3 = Mathf.Min(num3, 80f);
        if (num4 > 0f)
            leanLeft = true;
        else
            leanLeft = false;
        if (Gunner) return num3 * (num4 >= 0f ? 1 : -1);
        float num5;
        if (left && num4 < 0f || !left && num4 > 0f)
            num5 = 0.1f;
        else
            num5 = 0.5f;
        return num3 * (num4 >= 0f ? num5 : -num5);
    }

    private void GetOffHorse()
    {
        PlayAnimation("horse_getoff");
        baseR.AddForce(Vectors.up * 10f - baseT.Forward() * 2f - baseT.Right() * 1f, ForceMode.VelocityChange);
        Unmounted();
    }

    private void getOnHorse()
    {
        PlayAnimation("horse_geton");
        facingDirection = myHorse.transform.rotation.eulerAngles.y;
        targetRotation = Quaternion.Euler(0f, facingDirection, 0f);
    }

    private void HeadMovement()
    {
        var position = baseT.position;
        var x = Mathf.Sqrt((gunTarget.x - position.x) * (gunTarget.x - position.x) +
                           (gunTarget.z - position.z) * (gunTarget.z - position.z));
        var rotation = Head.rotation;
        targetHeadRotation = rotation;
        var vector = gunTarget - position;
        var current = -Mathf.Atan2(vector.z, vector.x) * 57.29578f;
        var num = -Mathf.DeltaAngle(current, baseT.rotation.eulerAngles.y - 90f);
        num = Mathf.Clamp(num, -40f, 40f);
        var y = Neck.position.y - gunTarget.y;
        var num2 = Mathf.Atan2(y, x) * 57.29578f;
        num2 = Mathf.Clamp(num2, -40f, 30f);
        targetHeadRotation = Quaternion.Euler(rotation.eulerAngles.x + num2, rotation.eulerAngles.y + num,
            rotation.eulerAngles.z);
        oldHeadRotation = Quaternion.Lerp(oldHeadRotation, targetHeadRotation, Time.deltaTime * 60f);
        rotation = oldHeadRotation;
        Head.rotation = rotation;
    }

    private void idle()
    {
        if (State == HeroState.Attack) FalseAttack();
        State = HeroState.Idle;
        CrossFade(standAnimation, 0.1f);
    }

    private bool IsFrontGrounded()
    {
        return Physics.Raycast(baseGT.position + baseGT.up * 1f, baseGT.forward, 1f, Layers.EnemyGround.value);
    }

    private bool IsPressDirectionTowardsHero(float h, float v)
    {
        if (h == 0f && v == 0f) return false;
        var globalFacingDirection = GetGlobalFacingDirection(h, v);
        return Mathf.Abs(Mathf.DeltaAngle(globalFacingDirection, baseT.rotation.eulerAngles.y)) < 45f;
    }

    private bool IsUpFrontGrounded()
    {
        return Physics.Raycast(baseGT.position + baseGT.up * 3f, baseGT.forward, 1.2f, Layers.EnemyGround.value);
    }

    private void LaunchLeftRope(RaycastHit hit, bool single, int mode = 0)
    {
        if (currentGas == 0f) return;
        UseGas();
        if (IN_GAME_MAIN_CAMERA.GameType == GameType.Single)
            bulletLeft = ((GameObject) Instantiate(CacheResources.Load("hook"))).GetComponent<Bullet>();
        else if (BasePV.IsMine)
            bulletLeft = Pool.NetworkEnable("hook", baseT.position, baseT.rotation).GetComponent<Bullet>();
        var go = !Gunner ? hookRefL1 : hookRefL2;
        var launcherRef = !Gunner ? "hookRefL1" : "hookRefL2";
        bulletLeft.transform.position = go.transform.position;
        var component = bulletLeft;
        var d = !single ? hit.distance <= 50f ? hit.distance * 0.05f : hit.distance * 0.3f : 0f;
        var a = hit.point - baseT.Right() * d - bulletLeft.transform.position;
        a.Normalize();
        if (mode == 1)
            component.Launch(a * 3f, baseR.velocity, launcherRef, true, this, true);
        else
            component.Launch(a * 3f, baseR.velocity, launcherRef, true, this);
        launchPointLeft = Vectors.zero;
    }

    private void LaunchRightRope(RaycastHit hit, bool single, int mode = 0)
    {
        if (currentGas == 0f) return;
        UseGas();
        if (IN_GAME_MAIN_CAMERA.GameType == GameType.Single)
            bulletRight = ((GameObject) Instantiate(CacheResources.Load("hook"))).GetComponent<Bullet>();
        else if (BasePV.IsMine)
            bulletRight = Pool.NetworkEnable("hook", baseT.position, baseT.rotation).GetComponent<Bullet>();
        var go = !Gunner ? hookRefR1 : hookRefR2;
        var launcherRef = !Gunner ? "hookRefR1" : "hookRefR2";
        bulletRight.transform.position = go.transform.position;
        var component = bulletRight;
        var d = !single ? hit.distance <= 50f ? hit.distance * 0.05f : hit.distance * 0.3f : 0f;
        var a = hit.point + baseT.Right() * d - bulletRight.transform.position;
        a.Normalize();
        if (mode == 1)
            component.Launch(a * 5f, baseR.velocity, launcherRef, false, this, true);
        else
            component.Launch(a * 3f, baseR.velocity, launcherRef, false, this);
        launchPointRight = Vectors.zero;
    }

    private IEnumerator loadskinRPCE(int horse, string urls, PhotonMessageInfo info = null)
    {
        while (!spawned) yield return null;
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

        if (Skin.NeedReload(SkinData)) yield return StartCoroutine(Skin.Reload(SkinData));
        Skin.Apply();
    }

    private void LeftArmAimTo(Vector3 target)
    {
        var position = Upper_Arm_L.position;
        var num = target.x - position.x;
        var y = target.y - position.y;
        var num2 = target.z - position.z;
        var x = Mathf.Sqrt(num * num + num2 * num2);
        Hand_L.localRotation = Quaternion.Euler(90f, 0f, 0f);
        Forearm_L.localRotation = Quaternion.Euler(-90f, 0f, 0f);
        Upper_Arm_L.rotation =
            Quaternion.Euler(0f, 90f + Mathf.Atan2(num, num2) * 57.29578f, -Mathf.Atan2(y, x) * 57.29578f);
    }

    public void OnDeathEvent(int viewID, bool isTitan)
    {
        if (PhotonNetwork.player.IsMasterClient)
            if (BasePV != null && BasePV.owner != null)
                GameModes.EndlessMode(BasePV.owner.ID);
        if (isTitan)
        {
            if (RCManager.RCEvents.ContainsKey("OnPlayerDieByTitan"))
            {
                var event2 = (RCEvent) RCManager.RCEvents["OnPlayerDieByTitan"];
                var strArray = (string[]) RCManager.RCVariableNames["OnPlayerDieByTitan"];
                if (RCManager.playerVariables.ContainsKey(strArray[0]))
                    RCManager.playerVariables[strArray[0]] = BasePV.owner;
                else
                    RCManager.playerVariables.Add(strArray[0], BasePV.owner);
                if (RCManager.titanVariables.ContainsKey(strArray[1]))
                    RCManager.titanVariables[strArray[1]] = PhotonView.Find(viewID).gameObject.GetComponent<TITAN>();
                else
                    RCManager.titanVariables.Add(strArray[1], PhotonView.Find(viewID).gameObject.GetComponent<TITAN>());
                event2.checkEvent();
            }
        }
        else if (RCManager.RCEvents.ContainsKey("OnPlayerDieByPlayer"))
        {
            var event2 = (RCEvent) RCManager.RCEvents["OnPlayerDieByPlayer"];
            var strArray = (string[]) RCManager.RCVariableNames["OnPlayerDieByPlayer"];
            if (RCManager.playerVariables.ContainsKey(strArray[0]))
                RCManager.playerVariables[strArray[0]] = BasePV.owner;
            else
                RCManager.playerVariables.Add(strArray[0], BasePV.owner);
            if (RCManager.playerVariables.ContainsKey(strArray[1]))
                RCManager.playerVariables[strArray[1]] = PhotonView.Find(viewID).owner;
            else
                RCManager.playerVariables.Add(strArray[1], PhotonView.Find(viewID).owner);
            event2.checkEvent();
        }
    }

    private void OnDestroy()
    {
        if (FengGameManagerMKII.FGM != null) FengGameManagerMKII.FGM.RemoveHero(this);
        if (myNetWorkName != null) Destroy(myNetWorkName);
        if (gunDummy != null) Destroy(gunDummy);
        if (IN_GAME_MAIN_CAMERA.GameType == GameType.MultiPlayer)
        {
            ReleaseIfIHookSb();
            if (BasePV != null && BasePV.owner != null)
            {
                GameModes.AntiReviveAdd(BasePV.owner.ID);
                BasePV.owner.GameObject = null;
            }
        }

        if (Setup.part_cape != null) ClothFactory.DisposeObject(Setup.part_cape);
        if (Setup.part_hair_1 != null) ClothFactory.DisposeObject(Setup.part_hair_1);
        if (Setup.part_hair_2 != null) ClothFactory.DisposeObject(Setup.part_hair_2);
        if (IN_GAME_MAIN_CAMERA.GameType == GameType.MultiPlayer && BasePV != null)
            GameModes.InfectionOnDeath(BasePV.owner);

        if (IsLocal)
        {
            if (leftbladetrail) leftbladetrail.Deactivate();
            if (leftbladetrail2) leftbladetrail2.Deactivate();
            if (rightbladetrail) rightbladetrail.Deactivate();
            if (rightbladetrail2) rightbladetrail2.Deactivate();
            
            if (bulletLeft) Destroy(bulletLeft.gameObject);
            if (bulletRight) Destroy(bulletRight.gameObject);
        }
    }

    private void PlayAnimationAt(string aniName, float normalizedTime)
    {
        currentAnimation = aniName;
        baseA.Play(aniName);
        baseA[aniName].normalizedTime = normalizedTime;
        if (!PhotonNetwork.connected) return;
        if (BasePV.IsMine) BasePV.RPC("netPlayAnimationAt", PhotonTargets.Others, aniName, normalizedTime);
    }

    private void ReleaseIfIHookSb()
    {
        if (hookSomeOne && hookTarget != null)
        {
            hookTarget.GetPhotonView().RPC("badGuyReleaseMe", hookTarget.GetPhotonView().owner);
            hookTarget = null;
            hookSomeOne = false;
        }
    }

    private void RightArmAimTo(Vector3 target)
    {
        var position = Upper_Arm_R.position;
        var num = target.x - position.x;
        var y = target.y - position.y;
        var num2 = target.z - position.z;
        var x = Mathf.Sqrt(num * num + num2 * num2);
        Hand_R.localRotation = Quaternion.Euler(-90f, 0f, 0f);
        Forearm_R.localRotation = Quaternion.Euler(90f, 0f, 0f);
        Upper_Arm_R.rotation =
            Quaternion.Euler(180f, 90f + Mathf.Atan2(num, num2) * 57.29578f, Mathf.Atan2(y, x) * 57.29578f);
    }

    private void Salute()
    {
        State = HeroState.Salute;
        CrossFade("salute", 0.1f);
    }

    private void SetHookedPplDirection()
    {
        almostSingleHook = false;
        if (isRightHandHooked && isLeftHandHooked)
        {
            if (bulletLeft != null && bulletRight != null)
            {
                var vector = bulletLeft.transform.position - bulletRight.transform.position;
                if (vector.sqrMagnitude < 4f)
                {
                    var vector2 = (bulletLeft.transform.position + bulletRight.transform.position) * 0.5f -
                                  baseT.position;
                    facingDirection = Mathf.Atan2(vector2.x, vector2.z) * 57.29578f;
                    if (Gunner && State != HeroState.Attack)
                    {
                        var velocity = baseR.velocity;
                        var current = -Mathf.Atan2(velocity.z, velocity.x) * 57.29578f;
                        var target = -Mathf.Atan2(vector2.z, vector2.x) * 57.29578f;
                        var num = -Mathf.DeltaAngle(current, target);
                        facingDirection += num;
                    }

                    almostSingleHook = true;
                }
                else
                {
                    var position = baseT.position;
                    var position1 = bulletLeft.transform.position;
                    var to = position - position1;
                    var position2 = bulletRight.transform.position;
                    var to2 = position - position2;
                    var vector3 = (position1 + position2) * 0.5f;
                    var from = position - vector3;
                    if (Vector3.Angle(from, to) < 30f && Vector3.Angle(from, to2) < 30f)
                    {
                        almostSingleHook = true;
                        var vector4 = vector3 - baseT.position;
                        facingDirection = Mathf.Atan2(vector4.x, vector4.z) * 57.29578f;
                    }
                    else
                    {
                        almostSingleHook = false;
                        var forward = baseT.Forward();
                        Vector3.OrthoNormalize(ref vector, ref forward);
                        facingDirection = Mathf.Atan2(forward.x, forward.z) * 57.29578f;
                        var current2 = Mathf.Atan2(to.x, to.z) * 57.29578f;
                        var num2 = Mathf.DeltaAngle(current2, facingDirection);
                        if (num2 > 0f) facingDirection += 180f;
                    }
                }
            }
        }
        else
        {
            almostSingleHook = true;
            Vector3 vector5;
            if (isRightHandHooked && bulletRight != null)
            {
                vector5 = bulletRight.transform.position - baseT.position;
            }
            else
            {
                if (!isLeftHandHooked || !(bulletLeft != null)) return;
                vector5 = bulletLeft.transform.position - baseT.position;
            }

            facingDirection = Mathf.Atan2(vector5.x, vector5.z) * 57.29578f;
            if (State != HeroState.Attack)
            {
                var velocity = baseR.velocity;
                var current3 = -Mathf.Atan2(velocity.z, velocity.x) * 57.29578f;
                var target2 = -Mathf.Atan2(vector5.z, vector5.x) * 57.29578f;
                var num3 = -Mathf.DeltaAngle(current3, target2);
                if (Gunner)
                {
                    facingDirection += num3;
                }
                else
                {
                    float num4;
                    if (isLeftHandHooked && num3 < 0f || isRightHandHooked && num3 > 0f)
                        num4 = -0.1f;
                    else
                        num4 = 0.1f;
                    facingDirection += num3 * num4;
                }
            }
        }
    }

    private void ShowAimUI()
    {
        CheckTitan();
        if (Screen.showCursor)
        {
            var localPosition = Vectors.up * 10000f;
            crossT1.localPosition = localPosition;
            crossT2.localPosition = localPosition;
            crossR2T.localPosition = localPosition;
            crossR1T.localPosition = localPosition;
            crossL2T.localPosition = localPosition;
            crossL1T.localPosition = localPosition;
            labelT.localPosition = localPosition;
            return;
        }

        var position = Input.mousePosition;
        var ray = IN_GAME_MAIN_CAMERA.BaseCamera.ScreenPointToRay(position);
        if (Physics.Raycast(ray, out var raycastHit, 1E+07f, Layers.EnemyGround.value))
        {
            crossT1.localPosition = position;
            crossT1.localPosition -= new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0f);
            crossT2.localPosition = crossT1.localPosition;
            var magnitude = (raycastHit.point - baseT.position).magnitude;
            var text = magnitude <= 1000f ? ((int) magnitude).ToString() : "???";
            if (Settings.Speedometer.Value)
            {
                if (Settings.SpeedometerType.Value == 0)
                    text += "\n" + baseR.velocity.magnitude.ToString("F0") + " u/s";
                else
                    text += "\n" + (baseR.velocity.magnitude / 100f).ToString("F1") + " k";
            }

            labelDistance.text = text;
            if (magnitude > 120f)
            {
                crossT1.localPosition += Vectors.up * 10000f;
                labelT.localPosition = crossT2.localPosition;
            }
            else
            {
                crossT2.localPosition += Vectors.up * 10000f;
                labelT.localPosition = crossT1.localPosition;
            }

            labelT.localPosition -= new Vector3(0f, 15f, 0f);
            var vector = new Vector3(0f, 0.4f, 0f);
            vector -= baseT.Right() * 0.3f;
            var vector2 = new Vector3(0f, 0.4f, 0f);
            vector2 += baseT.Right() * 0.3f;
            var d = raycastHit.distance <= 50f ? raycastHit.distance * 0.05f : raycastHit.distance * 0.3f;
            var vector3 = raycastHit.point - baseT.Right() * d - (baseT.position + vector);
            var vector4 = raycastHit.point + baseT.Right() * d - (baseT.position + vector2);
            vector3.Normalize();
            vector4.Normalize();
            vector3 *= 1000000f;
            vector4 *= 1000000f;
            if (Physics.Linecast(baseT.position + vector, baseT.position + vector + vector3, out raycastHit,
                Layers.EnemyGround.value))
            {
                crossL1T.localPosition = IN_GAME_MAIN_CAMERA.BaseCamera.WorldToScreenPoint(raycastHit.point);
                crossL1T.localPosition -= new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0f);
                crossL1T.localRotation = Quaternion.Euler(0f, 0f,
                    Mathf.Atan2(crossL1T.localPosition.y - (position.y - Screen.height * 0.5f),
                        crossL1T.localPosition.x - (position.x - Screen.width * 0.5f)) * 57.29578f + 180f);
                crossL2T.localPosition = crossL1T.localPosition;
                crossL2T.localRotation = crossL1T.localRotation;
                (raycastHit.distance > 120f ? crossL1T : crossL2T).localPosition += Vectors.up * 10000f;
            }

            if (Physics.Linecast(baseT.position + vector2, baseT.position + vector2 + vector4, out raycastHit,
                Layers.EnemyGround.value))
            {
                crossR1T.localPosition = IN_GAME_MAIN_CAMERA.BaseCamera.WorldToScreenPoint(raycastHit.point);
                crossR1T.localPosition -= new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0f);
                crossR1T.localRotation = Quaternion.Euler(0f, 0f,
                    Mathf.Atan2(crossR1T.localPosition.y - (position.y - Screen.height * 0.5f),
                        crossR1T.localPosition.x - (position.x - Screen.width * 0.5f)) * 57.29578f);
                crossR2T.localPosition = crossR1T.localPosition;
                crossR2T.localRotation = crossR1T.localRotation;
                (raycastHit.distance > 120f ? crossR1T : crossR2T).localPosition += Vectors.up * 10000f;
            }
        }
    }

    private void ShowBlades()
    {
        var num = currentBladeSta / totalBladeSta;
        SpriteCache.Find("bladeCL").fillAmount = SpriteCache.Find("bladeCR").fillAmount = num;
        if (num <= 0f)
            SpriteCache.Find("bladel1").color = SpriteCache.Find("blader1").color = Colors.red;
        else if (num <= 0.20f)
            SpriteCache.Find("bladel1").color = SpriteCache.Find("blader1").color = Colors.orange;
        else if (num < 0.40f)
            SpriteCache.Find("bladel1").color = SpriteCache.Find("blader1").color = Colors.yellow;
        else
            SpriteCache.Find("bladel1").color = SpriteCache.Find("blader1").color = Colors.white;
        for (var i = 5; i > 0; i--)
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
            SpriteCache.Find("UIflare1").fillAmount = (flareTotalCD - flare1CD) / flareTotalCD;
            SpriteCache.Find("UIflare2").fillAmount = (flareTotalCD - flare2CD) / flareTotalCD;
            SpriteCache.Find("UIflare3").fillAmount = (flareTotalCD - flare3CD) / flareTotalCD;
        }
    }

    private void ShowGas()
    {
        var num = currentGas / totalGas;
        var gasL1 = CacheGameObject.Find("gasL1");
        var gasL2 = CacheGameObject.Find("gasR1");
        if (gasL1 != null)
        {
            var L1 = CacheGameObject.Find("gasL1").GetComponent<UISprite>();
            L1.fillAmount = num;
        }

        if (gasL2 != null)
        {
            var L2 = CacheGameObject.Find("gasR1").GetComponent<UISprite>();
            L2.fillAmount = num;
        }

        var gasL = SpriteCache.Find("gasL");
        var gasR = SpriteCache.Find("gasR");
        if (gasL == null || gasR == null) return;
        if (num <= 0f)
        {
            gasL.color = gasR.color = Colors.red;
            return;
        }

        if (num <= 0.1f)
        {
            gasL.color = gasR.color = Colors.orange;
            return;
        }

        if (num < 0.3f)
        {
            gasL.color = gasR.color = Colors.yellow;
            return;
        }

        gasL.color = Colors.white;
        gasR.color = Colors.white;
    }

    private void ShowSkillCd()
    {
        if (skillCD) skillCD.GetComponent<UISprite>().fillAmount = (skillCDLast - skillCDDuration) / skillCDLast;
    }

    private void Start()
    {
        FengGameManagerMKII.FGM.AddHero(this);
        switch (IN_GAME_MAIN_CAMERA.GameType)
        {
            case GameType.MultiPlayer:
            {
                if (BasePV != null)
                {
                    BasePV.owner.GameObject = gameObject;
                }

                break;
            }
            case GameType.Single:
                SingleRunStats.SetHERO(this);
                break;
        }

        if ((FengGameManagerMKII.Level.HorsesEnabled || GameModes.AllowHorses.Enabled) &&
            IN_GAME_MAIN_CAMERA.GameType == GameType.MultiPlayer && BasePV.IsMine)
        {
            myHorse = Pool.NetworkEnable("horse", baseT.position + Vectors.up * 5f, baseT.rotation);
            myHorse.GetComponent<Horse>().myHero = baseG;
            myHorse.GetComponent<TITAN_CONTROLLER>().isHorse = true;
        }

        sparks = baseT.Find("slideSparks").GetComponent<ParticleSystem>();
        smoke3Dmg = baseT.Find("3dmg_smoke").GetComponent<ParticleSystem>();
        baseT.localScale = new Vector3(myScale, myScale, myScale);
        facingDirection = baseT.rotation.eulerAngles.y;
        targetRotation = Quaternion.Euler(0f, facingDirection, 0f);
        smoke3Dmg.enableEmission = false;
        sparks.enableEmission = false;
        speedFXPS = speedFX1.GetComponent<ParticleSystem>();
        speedFXPS.enableEmission = false;
        var enables = new List<string>(new[] {"Controller_Body", "AOTTG_HERO 1(Clone)"});
        if (!IsLocal)
            foreach (var col in GetComponentsInChildren<Collider>())
            {
                if (enables.Contains(col.name))
                    continue;
                col.enabled = false;
            }

        if (IN_GAME_MAIN_CAMERA.GameType == GameType.MultiPlayer)
        {
            if (RCManager.heroHash.ContainsKey(BasePV.owner.ID))
                RCManager.heroHash[BasePV.owner.ID] = this;
            else
                RCManager.heroHash.Add(BasePV.owner.ID, this);
            myNetWorkName = (GameObject) Instantiate(CacheResources.Load("UI/LabelNameOverHead"));
                myNetWorkName.name = "LabelNameOverHead";
                myNetWorkName.transform.parent = FengGameManagerMKII.UIRefer.panels[0].transform;
                myNetWorkName.transform.localScale = new Vector3(6f, 6f, 6f);
                myNetWorkName.GetComponent<UILabel>().text = string.Empty;
                var txt = myNetWorkName.GetComponent<TextMesh>();
                if (txt == null) txt = myNetWorkName.AddComponent<TextMesh>();
                var render = myNetWorkName.GetComponent<MeshRenderer>();
                if (render == null) render = myNetWorkName.AddComponent<MeshRenderer>();
                render.material = Labels.Font.material;
                txt.font = Labels.Font;
                txt.fontSize = 20;
                txt.anchor = TextAnchor.MiddleCenter;
                txt.alignment = TextAlignment.Center;
                txt.color = Colors.white;
                txt.text = myNetWorkName.GetComponent<UILabel>().text;
                txt.richText = true;
                txt.gameObject.layer = 5;
                var show = string.Empty;
                myNetWorkName.GetComponent<UILabel>().enabled = false;
                if (BasePV.owner.Team == 2) show += "[FF0000]AHSS\n[FFFFFF]";
                if (BasePV.owner.GuildName != string.Empty) show += $"[FFFF00]{BasePV.owner.GuildName}\n[FFFFFF]";
                show += BasePV.owner.UIName;
                myNetWorkName.GetComponent<TextMesh>().text = show.ToHTMLFormat();
            GameModes.InfectionOnSpawn(this);
        }
        else
        {
            Minimap.TrackGameObjectOnMinimap(gameObject, Color.green, false, true);
        }

        BombImmune = false;
        if (GameModes.BombMode.Enabled)
        {
            BombImmune = true;
            StartCoroutine(StopImmunity());
        }

        if (IsLocal)
        {
            ShowGas();
            if (Gunner)
                ShowBullets();
            else
                ShowBlades();
            if (IN_GAME_MAIN_CAMERA.GameType == GameType.MultiPlayer)
            {
                GetComponent<SmoothSyncMovement>().PhotonCamera = true;
                BasePV.RPC("SetMyPhotonCamera", PhotonTargets.OthersBuffered, new object[]
                {
                    Settings.CameraDistance.Value + 0.3f
                });
            }

            spawned = true;
            var viewid = -1;
            if (SkinSettings.HumanSkins.Value > 0 && SkinSettings.HumanSet.Value != StringSetting.NotDefine)
            {
                var set = new HumanSkinPreset(SkinSettings.HumanSet.Value);
                set.Load();
                mySkinUrl = string.Join(",", set.ToSkinData(), 0, 13);
            }

            if (myHorse != null) viewid = myHorse.GetPhotonView().viewID;
            if (mySkinUrl != string.Empty && SkinSettings.HumanSkins.Value > 0)
            {
                loadskinRPC(viewid, mySkinUrl);
                if (IN_GAME_MAIN_CAMERA.GameType == GameType.MultiPlayer && SkinSettings.HumanSkins.Value != 2)
                    BasePV.RPC("loadskinRPC", PhotonTargets.Others, viewid, mySkinUrl);
            }

            Minimap.TrackGameObjectOnMinimap(gameObject, Color.green, false, true);
            wLeft.Active = false;
            wRight.Active = false;
            if (AnarchyManager.Pause.Active) AnarchyManager.Pause.DisableImmediate();
            return;
        }

        if (IN_GAME_MAIN_CAMERA.DayLight == DayLight.Night)
        {
            var gameObject2 = (GameObject) Instantiate(CacheResources.Load("flashlight"));
            gameObject2.transform.parent = baseT;
            gameObject2.transform.position = baseT.position + Vectors.up;
            gameObject2.transform.rotation = Quaternion.Euler(353f, 0f, 0f);
        }

        var mapColor = Colors.blue;
        if (BasePV.owner.RCteam > 0) mapColor = BasePV.owner.RCteam == 1 ? Colors.magenta : Colors.cyan;
        if (BasePV.owner.Team == 2) mapColor = Colors.red;
        Minimap.TrackGameObjectOnMinimap(gameObject, mapColor, false, true);
        Setup.Init();
        Setup.myCostume = new HeroCostume();
        Setup.myCostume = CostumeConeveter.PhotonDataToHeroCostume(BasePV.owner);
        Setup.SetCharacterComponent();
        Destroy(checkBoxLeft);
        Destroy(checkBoxRight);
        Destroy(leftbladetrail);
        Destroy(rightbladetrail);
        Destroy(leftbladetrail2);
        Destroy(rightbladetrail2);
        spawned = true;
        GameModes.AntiReviveCheck(BasePV.owner.ID, this);
    }

    private void Suicide()
    {
        if (IN_GAME_MAIN_CAMERA.GameType == GameType.Single) return;
        NetDieSpecial(baseR.velocity * 50f, false, -1, User.Suicide.PickRandomString());
        FengGameManagerMKII.FGM.needChooseSide = true;
        FengGameManagerMKII.FGM.justSuicide = true;
    }

    private void ThrowBlades()
    {
        var obj2 = (GameObject) Instantiate(CacheResources.Load("Character_parts/character_blade_l"),
            Setup.part_blade_l.transform.position, Setup.part_blade_l.transform.rotation);
        var obj3 = (GameObject) Instantiate(CacheResources.Load("Character_parts/character_blade_r"),
            Setup.part_blade_r.transform.position, Setup.part_blade_r.transform.rotation);
        obj2.renderer.material = obj3.renderer.material = CharacterMaterials.Materials[Setup.myCostume._3dmg_texture];
        var vec = baseT.Forward() + baseT.Up() * 2;
        obj2.rigidbody.AddForce(vec - baseT.Right(), ForceMode.Impulse);
        obj3.rigidbody.AddForce(vec + baseT.Right(), ForceMode.Impulse);
        obj2.rigidbody.AddTorque(
            new Vector3(Random.Range(-100f, 100f), Random.Range(-100f, 100f), Random.Range(-100f, 100f)).normalized);
        obj3.rigidbody.AddTorque(
            new Vector3(Random.Range(-100f, 100f), Random.Range(-100f, 100f), Random.Range(-100f, 100f)).normalized);
        Setup.part_blade_l.SetActive(false);
        Setup.part_blade_r.SetActive(false);
        currentBladeNum--;
        currentBladeSta = currentBladeNum == 0 ? 0f : currentBladeSta;
        if (State == HeroState.Attack) FalseAttack();
        ShowBlades();
    }

    private void Unmounted()
    {
        myHorse.GetComponent<Horse>().unmounted();
        isMounted = false;
    }

    private void UpdateLeftMagUI()
    {
        for (var i = 1; i <= bulletMAX; i++) SpriteCache.Find("bulletL" + i).enabled = false;
        for (var j = 1; j <= leftBulletLeft; j++) SpriteCache.Find("bulletL" + j).enabled = true;
    }

    private void UpdateRightMagUI()
    {
        for (var i = 1; i <= bulletMAX; i++) SpriteCache.Find("bulletR" + i).enabled = false;
        for (var j = 1; j <= rightBulletLeft; j++) SpriteCache.Find("bulletR" + j).enabled = true;
    }

    private void UseGas(float amount = 0f)
    {
        if (amount == 0f) amount = UseGasSpeed;
        if (currentGas > 0f)
        {
            currentGas -= amount;
            if (currentGas < 0f) currentGas = 0f;
        }

        ShowGas();
    }

    public void AttackAccordingToMouse()
    {
        if (Input.mousePosition.x < Screen.width * 0.5)
            attackAnimation = "attack2";
        else
            attackAnimation = "attack1";
    }

    public void AttackAccordingToTarget(Transform a)
    {
        var vector = a.position - baseT.position;
        var current = -Mathf.Atan2(vector.z, vector.x) * 57.29578f;
        var num = -Mathf.DeltaAngle(current, baseT.rotation.eulerAngles.y - 90f);
        if (Mathf.Abs(num) < 90f && vector.magnitude < 6f && a.position.y <= baseT.position.y + 2f &&
            a.position.y >= baseT.position.y - 5f)
            attackAnimation = "attack4";
        else if (num > 0f)
            attackAnimation = "attack1";
        else
            attackAnimation = "attack2";
    }

    public void BackToHuman()
    {
        baseG.GetComponent<SmoothSyncMovement>().Disabled = false;
        baseR.velocity = Vectors.zero;
        titanForm = false;
        Ungrabbed();
        FalseAttack();
        skillCDDuration = skillCDLast;
        IN_GAME_MAIN_CAMERA.MainCamera.SetMainObject(this);
        if (IN_GAME_MAIN_CAMERA.GameType != GameType.Single) BasePV.RPC("backToHumanRPC", PhotonTargets.Others);
    }

    public void ClearPopup()
    {
        FengGameManagerMKII.FGM.ShowHUDInfoCenter(string.Empty);
    }

    public void ContinueAnimation()
    {
        if (!animationStopped)
        {
            return;
        }
        animationStopped = false;
        foreach (var obj in baseA)
        {
            var animationState = (AnimationState) obj;
            if (animationState.speed == 1f) return;
            animationState.speed = 1f;
        }

        CustomAnimationSpeed();
        PlayAnimation(CurrentPlayingClipName());
        if (IN_GAME_MAIN_CAMERA.GameType != GameType.Single && BasePV.IsMine)
            BasePV.RPC("netContinueAnimation", PhotonTargets.Others);
    }

    public void CrossFade(string aniName, float time)
    {
        currentAnimation = aniName;
        baseA.CrossFade(aniName, time);
        if (!PhotonNetwork.connected) return;
        if (BasePV.IsMine) BasePV.RPC("netCrossFade", PhotonTargets.Others, aniName, time);
    }

    public string CurrentPlayingClipName()
    {
        foreach (var obj in baseA)
        {
            var animationState = (AnimationState) obj;
            if (baseA.IsPlaying(animationState.name)) return animationState.name;
        }

        return string.Empty;
    }

    public void Die(Vector3 v, bool isBite)
    {
        if (invincible > 0f) return;
        if (titanForm && eren != null) eren.GetComponent<TITAN_EREN>().lifeTime = 0.1f;
        if (bulletLeft) bulletLeft.RemoveMe();
        if (bulletRight) bulletRight.RemoveMe();
        meatDie.Play();
        if ((IN_GAME_MAIN_CAMERA.GameType == GameType.Single || BasePV.IsMine) && !Gunner)
        {
            leftbladetrail.Deactivate();
            rightbladetrail.Deactivate();
            leftbladetrail2.Deactivate();
            rightbladetrail2.Deactivate();
        }

        BreakApart(v, isBite);
        IN_GAME_MAIN_CAMERA.MainCamera.gameOver = true;
        if (IN_GAME_MAIN_CAMERA.GameMode != GameMode.RACING) FengGameManagerMKII.FGM.GameLose();
        FalseAttack();
        IsDead = true;
        var audioTransform = baseT.Find("audio_die");
        audioTransform.parent = null;
        audioTransform.GetComponent<AudioSource>().Play();
        if (Settings.Snapshots.ToValue())
            IN_GAME_MAIN_CAMERA.MainCamera.startSnapShot(baseT.position, 0, null, Time.fixedDeltaTime);
        Destroy(baseG);
    }

    public void Die2(Transform tf)
    {
        if (invincible > 0f) return;
        if (titanForm && eren != null) eren.GetComponent<TITAN_EREN>().lifeTime = 0.1f;
        if (bulletLeft) bulletLeft.RemoveMe();
        if (bulletRight) bulletRight.RemoveMe();
        var audioTransform = baseT.Find("audio_die");
        audioTransform.parent = null;
        audioTransform.GetComponent<AudioSource>().Play();
        meatDie.Play();
        IN_GAME_MAIN_CAMERA.MainCamera.SetMainObject(null);
        IN_GAME_MAIN_CAMERA.MainCamera.gameOver = true;
        FengGameManagerMKII.FGM.GameLose();
        FalseAttack();
        IsDead = true;
        Pool.Enable("hitMeat2", baseT.position,
            Quaternion.identity); //(GameObject)UnityEngine.Object.Instantiate(CacheResources.Load("hitMeat2"));
        //gameObject.transform.position = baseT.position;
        Destroy(baseG);
    }

    public void FalseAttack()
    {
        attackMove = false;
        if (Gunner)
        {
            if (!attackReleased)
            {
                ContinueAnimation();
                attackReleased = true;
            }
        }
        else
        {
            if (IN_GAME_MAIN_CAMERA.GameType == GameType.Single || BasePV.IsMine)
            {
                wLeft.Active = false;
                wRight.Active = false;
                wLeft.clearHits();
                wRight.clearHits();
                leftbladetrail.StopSmoothly(0.2f);
                rightbladetrail.StopSmoothly(0.2f);
                leftbladetrail2.StopSmoothly(0.2f);
                rightbladetrail2.StopSmoothly(0.2f);
            }

            attackLoop = 0;
            if (!attackReleased)
            {
                ContinueAnimation();
                attackReleased = true;
            }
        }
    }

    public void FillGas()
    {
        currentGas = totalGas;
        ShowGas();
        if (Gunner) ShowBullets();
        else ShowBlades();
    }

    public string GetDebugInfo()
    {
        var text = "Left:" + isLeftHandHooked + " ";
        if (isLeftHandHooked && bulletLeft != null)
        {
            var vector = bulletLeft.transform.position - baseT.position;
            text += (int) (Mathf.Atan2(vector.x, vector.z) * 57.29578f);
        }

        var text2 = text;
        text = string.Concat(text2, "\nRight:", isRightHandHooked, " ");
        if (isRightHandHooked && bulletRight != null)
        {
            var vector2 = bulletRight.transform.position - baseT.position;
            text += (int) (Mathf.Atan2(vector2.x, vector2.z) * 57.29578f);
        }

        text = text + "\nfacingDirection:" + (int) facingDirection;
        text = text + "\nActual facingDirection:" + (int) baseT.rotation.eulerAngles.y;
        text = text + "\nState:" + State;
        text += "\n\n\n\n\n";
        if (State == HeroState.Attack) targetRotation = Quaternion.Euler(0f, facingDirection, 0f);
        return text;
    }

    public void GetSupply()
    {
        if ((baseA.IsPlaying(standAnimation) || baseA.IsPlaying("run") || baseA.IsPlaying("run_sasha")) &&
            (currentBladeSta != totalBladeSta || currentBladeNum != TotalBladeNum || currentGas != totalGas ||
             leftBulletLeft != bulletMAX || rightBulletLeft != bulletMAX))
        {
            State = HeroState.FillGas;
            CrossFade("supply", 0.1f);
        }
    }

    public void Grabbed(GameObject titan, bool leftHand)
    {
        if (isMounted) Unmounted();
        State = HeroState.Grab;
        GetComponent<CapsuleCollider>().isTrigger = true;
        FalseAttack();
        titanWhoGrabMe = titan;
        if (titanForm && eren != null) eren.GetComponent<TITAN_EREN>().lifeTime = 0.1f;
        if (!Gunner && (IN_GAME_MAIN_CAMERA.GameType == GameType.Single || BasePV.IsMine))
        {
            leftbladetrail.Deactivate();
            rightbladetrail.Deactivate();
            leftbladetrail2.Deactivate();
            rightbladetrail2.Deactivate();
        }

        smoke3Dmg.enableEmission = false;
        sparks.enableEmission = false;
    }

    public bool HasDied()
    {
        return IsDead || IsInvincible();
    }

    public void HookedByHuman(int hooker, Vector3 hookPosition)
    {
        BasePV.RPC("RPCHookedByHuman", BasePV.owner, hooker, hookPosition);
    }

    public void HookToHuman(GameObject target, Vector3 hookPosition)
    {
        ReleaseIfIHookSb();
        hookTarget = target;
        hookSomeOne = true;
        if (target.GetComponent<HERO>()) target.GetComponent<HERO>().HookedByHuman(BasePV.viewID, hookPosition);
        launchForce = hookPosition - baseT.position;
        var d = Mathf.Pow(launchForce.magnitude, 0.1f);
        if (grounded) baseR.AddForce(Vectors.up * Mathf.Min(launchForce.magnitude * 0.2f, 10f), ForceMode.Impulse);
        baseR.AddForce(launchForce * d * 0.1f, ForceMode.Impulse);
    }

    public bool IsGrounded()
    {
        return Physics.Raycast(baseGT.position + Vectors.up * 0.1f, -Vectors.up, 0.3f, Layers.EnemyGround.value);
    }

    public bool IsInvincible()
    {
        return invincible > 0f;
    }

    public void lateUpdate()
    {
        if (IN_GAME_MAIN_CAMERA.GameType != GameType.Single && myNetWorkName != null)
        {
            if (titanForm && eren != null) myNetWorkName.transform.localPosition = Vectors.up * Screen.height * 2f;
            var position = baseT.position;
            var vector = new Vector3(position.x, position.y + 2f, position.z);
            if (Vector3.Angle(IN_GAME_MAIN_CAMERA.BaseT.Forward(), vector - IN_GAME_MAIN_CAMERA.BaseT.position) > 90f ||
                Physics.Linecast(vector, IN_GAME_MAIN_CAMERA.BaseT.position, Layers.EnemyGround))
            {
                myNetWorkName.transform.localPosition = Vectors.up * Screen.height * 2f;
            }
            else
            {
                Vector2 vector2 = IN_GAME_MAIN_CAMERA.BaseCamera.WorldToScreenPoint(vector);
                myNetWorkName.transform.localPosition = new Vector3((int) (vector2.x - Screen.width * 0.5f),
                    (int) (vector2.y - Screen.height * 0.5f), 0f);
            }
        }

        if (titanForm || isCannon) return;
        if (VideoSettings.CameraTilt.Value && IsLocal)
        {
            var vector3 = Vectors.zero;
            var vector4 = Vectors.zero;
            if (isLaunchLeft && bulletLeft != null && bulletLeft.IsHooked()) vector3 = bulletLeft.transform.position;
            if (isLaunchRight && bulletRight != null && bulletRight.IsHooked())
                vector4 = bulletRight.transform.position;
            var a = Vectors.zero;
            if (vector3.magnitude != 0f && vector4.magnitude == 0f)
                a = vector3;
            else if (vector3.magnitude == 0f && vector4.magnitude != 0f)
                a = vector4;
            else if (vector3.magnitude != 0f && vector4.magnitude != 0f) a = (vector3 + vector4) * 0.5f;
            var position = baseT.position;
            var vector5 = Vector3.Project(a - position, IN_GAME_MAIN_CAMERA.BaseCamera.transform.Up());
            var b = Vector3.Project(a - position, IN_GAME_MAIN_CAMERA.BaseCamera.transform.Right());
            Quaternion to2;
            if (a.magnitude > 0f)
            {
                var to = vector5 + b;
                var num = Vector3.Angle(a - baseT.position, baseR.velocity);
                num *= 0.005f;
                var transform1 = IN_GAME_MAIN_CAMERA.BaseCamera.transform;
                var rotation = transform1.rotation;
                to2 = Quaternion.Euler(rotation.eulerAngles.x,
                    rotation.eulerAngles.y,
                    (transform1.right + b.normalized).magnitude >= 1f
                        ? -Vector3.Angle(vector5, to) * num
                        : Vector3.Angle(vector5, to) * num);
            }
            else
            {
                var transform1 = IN_GAME_MAIN_CAMERA.BaseCamera.transform;
                var rotation = transform1.rotation;
                to2 = Quaternion.Euler(rotation.eulerAngles.x,
                    rotation.eulerAngles.y, 0f);
            }

            IN_GAME_MAIN_CAMERA.BaseCamera.transform.rotation =
                Quaternion.Lerp(IN_GAME_MAIN_CAMERA.BaseCamera.transform.rotation, to2, Time.deltaTime);
        }

        if (State == HeroState.Grab && titanWhoGrabMe)
        {
            if (titanWhoGrabMe.GetComponent<TITAN>())
            {
                baseT.position = titanWhoGrabMe.GetComponent<TITAN>().grabTF.transform.position;
                baseT.rotation = titanWhoGrabMe.GetComponent<TITAN>().grabTF.transform.rotation;
            }
            else if (titanWhoGrabMe.GetComponent<FEMALE_TITAN>())
            {
                baseT.position = titanWhoGrabMe.GetComponent<FEMALE_TITAN>().grabTF.transform.position;
                baseT.rotation = titanWhoGrabMe.GetComponent<FEMALE_TITAN>().grabTF.transform.rotation;
            }
        }

        if (Gunner)
        {
            if (leftArmAim || rightArmAim)
            {
                var vector6 = gunTarget - baseT.position;
                var current = -Mathf.Atan2(vector6.z, vector6.x) * 57.29578f;
                var num2 = -Mathf.DeltaAngle(current, baseT.rotation.eulerAngles.y - 90f);
                HeadMovement();
                if (!isLeftHandHooked && leftArmAim && num2 < 40f && num2 > -90f) LeftArmAimTo(gunTarget);
                if (!isRightHandHooked && rightArmAim && num2 > -40f && num2 < 90f) RightArmAimTo(gunTarget);
            }
            else if (!grounded)
            {
                Hand_L.localRotation = Quaternion.Euler(90f, 0f, 0f);
                Hand_R.localRotation = Quaternion.Euler(-90f, 0f, 0f);
            }

            if (isLeftHandHooked && bulletLeft != null) LeftArmAimTo(bulletLeft.transform.position);
            if (isRightHandHooked && bulletRight != null) RightArmAimTo(bulletRight.transform.position);
        }

        SetHookedPplDirection();
        this.BodyLean();
        //if (VideoSettings.BladeTrails.Value && !Gunner && IsLocal)
        //{
        //}
    }

    public void Launch(Vector3 des, bool left = true, bool leviMode = false)
    {
        if (isMounted) Unmounted();
        if (State != HeroState.Attack) idle();
        var a = des - baseT.position;
        if (left)
            launchPointLeft = des;
        else
            launchPointRight = des;
        a.Normalize();
        a *= 20f;
        if (bulletLeft != null && bulletRight != null && bulletLeft.IsHooked() && bulletRight.IsHooked()) a *= 0.8f;
        leviMode = baseA.IsPlaying("attack5") || baseA.IsPlaying("special_petra");
        if (!leviMode)
        {
            FalseAttack();
            idle();
                if (this.Gunner)
                {
                    this.CrossFade("AHSS_hook_forward_both", 0.1f);
                }
                else if (left && !this.isRightHandHooked)
                {
                    this.CrossFade("air_hook_l_just", 0.1f);
                }
                else if (!left && !this.isLeftHandHooked)
                {
                    this.CrossFade("air_hook_r_just", 0.1f);
                }
                else
                {
                    this.CrossFade("dash", 0.1f);
                    baseA["dash"].time = 0f;
                }
        }

        if (left) isLaunchLeft = true;
        if (!left) isLaunchRight = true;
        launchForce = a;
        if (!leviMode)
        {
            if (a.y < 30f) launchForce += Vectors.up * (30f - a.y);
            if (des.y >= baseT.position.y) launchForce += Vectors.up * (des.y - baseT.position.y) * 10f;
            baseR.AddForce(launchForce);
        }

        facingDirection = Mathf.Atan2(launchForce.x, launchForce.z) * 57.29578f;
        var quaternion = Quaternion.Euler(0f, facingDirection, 0f);
        baseGT.rotation = quaternion;
        baseR.rotation = quaternion;
        targetRotation = quaternion;
        if (left)
            launchElapsedTimeL = 0f;
        else
            launchElapsedTimeR = 0f;
        if (leviMode) launchElapsedTimeR = -100f;
        if (baseA.IsPlaying("special_petra"))
        {
            launchElapsedTimeR = -100f;
            launchElapsedTimeL = -100f;
            if (bulletRight)
            {
                bulletRight.Disable();
                ReleaseIfIHookSb();
            }

            if (bulletLeft)
            {
                bulletLeft.Disable();
                ReleaseIfIHookSb();
            }
        }

        sparks.enableEmission = false;
    }

    public void MarkDie()
    {
        IsDead = true;
        State = HeroState.Die;
    }

    public void NetDieSpecial(Vector3 v, bool isBite, int viewID = -1, string titanName = "", bool killByTitan = true)
    {
        if (titanForm && eren != null) eren.GetComponent<TITAN_EREN>().lifeTime = 0.1f;
        if (myBomb != null) myBomb.destroyMe();
        if (myCannon != null) PhotonNetwork.Destroy(myCannon);
        if (bulletLeft) bulletLeft.RemoveMe();
        if (bulletRight) bulletRight.RemoveMe();
        meatDie.Play();
        if (!Gunner && (IN_GAME_MAIN_CAMERA.GameType == GameType.Single || BasePV.IsMine))
        {
            leftbladetrail.Deactivate();
            rightbladetrail.Deactivate();
            leftbladetrail2.Deactivate();
            rightbladetrail2.Deactivate();
        }

        FalseAttack();
        BreakApart(v, isBite);
        IN_GAME_MAIN_CAMERA.MainCamera.setSpectorMode(false);
        IN_GAME_MAIN_CAMERA.MainCamera.gameOver = true;
        FengGameManagerMKII.FGM.logic.MyRespawnTime = 0f;
        IsDead = true;
        var audioTransform = baseT.Find("audio_die");
        audioTransform.parent = null;
        audioTransform.GetComponent<AudioSource>().Play();
        baseG.GetComponent<SmoothSyncMovement>().Disabled = true;
        PhotonNetwork.player.Dead = true;
        PhotonNetwork.player.Deaths++;
        FengGameManagerMKII.FGM.BasePV.RPC("someOneIsDead", PhotonTargets.MasterClient, 0);
        FengGameManagerMKII.FGM.SendKillInfo(false, titanName, false, User.DeathName);
        AnarchyManager.Feed.Kill(titanName, User.DeathName, 0);
        PhotonNetwork.Destroy(BasePV);
        if (PhotonNetwork.IsMasterClient)
        {
            OnDeathEvent(viewID, killByTitan);
            if (RCManager.heroHash.ContainsKey(viewID))
                RCManager.heroHash.Remove(viewID);
        }
    }

    public void PauseAnimation()
    {
        if (animationStopped)
        {
            return;
        }
        foreach (var obj in baseA)
        {
            var animationState = (AnimationState) obj;
            animationState.speed = 0f;
        }

        if (IN_GAME_MAIN_CAMERA.GameType != GameType.Single && BasePV.IsMine)
            BasePV.RPC("netPauseAnimation", PhotonTargets.Others);
        animationStopped = true;
    }

    public void PlayAnimation(string aniName)
    {
        currentAnimation = aniName;
        baseA.Play(aniName);
        if (!PhotonNetwork.connected) return;
        if (BasePV.IsMine) BasePV.RPC("netPlayAnimation", PhotonTargets.Others, aniName);
    }

    public void ResetAnimationSpeed()
    {
        foreach (AnimationState animationState in baseA) animationState.speed = 1f;
        CustomAnimationSpeed();
    }

    public void SetSkillHudPosition()
    {
        skillCD = CacheGameObject.Find("skill_cd_" + skillIDHUD);
        if (skillCD != null)
            skillCD.transform.localPosition = CacheGameObject.Find("skill_cd_bottom").transform.localPosition;
        if (Gunner) skillCD.transform.localPosition = Vectors.up * 5000f;
    }

    public void SetStat()
    {
        skillCDLast = 1.5f;
        skillID = Setup.myCostume.stat.skillID;
        if (skillID == "levi") skillCDLast = 3.5f;
        CustomAnimationSpeed();
        switch (skillID)
        {
            case "armin":
                skillCDLast = 5f;
                break;
            case "marco":
                skillCDLast = 10f;
                break;
            case "jean":
                skillCDLast = 0.001f;
                break;
            case "eren":
            {
                skillCDLast = 120f;
                if (IN_GAME_MAIN_CAMERA.GameType == GameType.MultiPlayer)
                {
                    if (FengGameManagerMKII.Level.TeamTitan || FengGameManagerMKII.Level.Mode == GameMode.RACING ||
                        FengGameManagerMKII.Level.Mode == GameMode.PVP_CAPTURE ||
                        FengGameManagerMKII.Level.Mode == GameMode.TROST)
                    {
                        skillID = "petra";
                        skillCDLast = 1f;
                    }
                    else
                    {
                        var num = PhotonNetwork.playerList.Count(player => !player.IsTitan && player.Character.ToUpper() == "EREN");
                        if (num > 1)
                        {
                            skillID = "petra";
                            skillCDLast = 1f;
                        }
                    }
                }

                break;
            }
        }

        switch (skillID)
        {
            case "sasha":
                skillCDLast = 20f;
                break;
            case "petra":
                skillCDLast = 3.5f;
                break;
        }

        BombInit();
        speed = Setup.myCostume.stat.Spd / 10f;
        totalGas = currentGas = Setup.myCostume.stat.Gas;
        totalBladeSta = currentBladeSta = Setup.myCostume.stat.Bla;
        baseR.mass = 0.5f - (Setup.myCostume.stat.Acl - 100) * 0.001f;
        CacheGameObject.Find("skill_cd_bottom").transform.localPosition =
            new Vector3(0f, -(float) Screen.height * 0.5f + 5f, 0f);
        skillCD = CacheGameObject.Find("skill_cd_" + skillIDHUD);
        skillCD.transform.localPosition = CacheGameObject.Find("skill_cd_bottom").transform.localPosition;
        CacheGameObject.Find("GasUI").transform.localPosition =
            CacheGameObject.Find("skill_cd_bottom").transform.localPosition;
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

        if (Setup.myCostume.uniform_type == UNIFORM_TYPE.CasualAHSS)
        {
            standAnimation = "AHSS_stand_gun";
            Gunner = true;
            wLeft.Active = false;
            wRight.Active = false;
            gunDummy = new GameObject();
            gunDummy.name = "gunDummy";
            gunDummy.transform.position = baseT.position;
            gunDummy.transform.rotation = baseT.rotation;
            SetTeam(2);
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
                skillCD.transform.localPosition = Vectors.up * 5000f;
            }
        }
        else
        {
            if (Setup.myCostume.sex == Sex.Female)
            {
                standAnimation = "stand";
                SetTeam(1);
                return;
            }

            standAnimation = "stand_levi";
            SetTeam(1);
        }
    }

    public void SetTeam(int team)
    {
        if (IN_GAME_MAIN_CAMERA.GameType == GameType.MultiPlayer && BasePV.IsMine)
        {
            BasePV.RPC("setMyTeam", PhotonTargets.AllBuffered, team);
            PhotonNetwork.player.Team = team;
        }
        else
        {
            setMyTeam(team);
        }
    }

    public void ShootFlare(int type)
    {
        var flag = false;
        if (type == 1 && flare1CD == 0f)
        {
            flare1CD = flareTotalCD;
            flag = true;
        }

        if (type == 2 && flare2CD == 0f)
        {
            flare2CD = flareTotalCD;
            flag = true;
        }

        if (type == 3 && flare3CD == 0f)
        {
            flare3CD = flareTotalCD;
            flag = true;
        }

        if (flag)
        {
            if (IN_GAME_MAIN_CAMERA.GameType == GameType.Single)
            {
                var go =
                    Pool.Enable("FX/flareBullet" + type, baseT.position,
                        baseT.rotation); //(GameObject)UnityEngine.Object.Instantiate(CacheResources.Load("FX/flareBullet" + type), baseT.position, baseT.rotation);
                go.GetComponent<FlareMovement>().dontShowHint();
                //UnityEngine.Object.Destroy(gameObject, 25f);
                go.GetComponent<SelfDestroy>().CountDown = 25f;
            }
            else
            {
                var gameObject2 = Pool.NetworkEnable("FX/flareBullet" + type, baseT.position, baseT.rotation);
                gameObject2.GetComponent<FlareMovement>().dontShowHint();
            }
        }
    }

    public IEnumerator StopImmunity()
    {
        yield return new WaitForSeconds(5f);
        BombImmune = false;
    }

    public void Ungrabbed()
    {
        facingDirection = 0f;
        targetRotation = Quaternion.Euler(0f, 0f, 0f);
        baseT.parent = null;
        GetComponent<CapsuleCollider>().isTrigger = false;
        State = HeroState.Idle;
    }

    public void update()
    {
        if (IN_GAME_MAIN_CAMERA.isPausing || IsDead) return;
        var dt = Time.deltaTime;
        if (invincible > 0f) invincible -= dt;
        if (titanForm && eren != null)
        {
            baseT.position = eren.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/neck").position;
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

        if (GameModes.BombMode.Enabled) BombUpdate();
        if (myCannonRegion != null)
        {
            FengGameManagerMKII.FGM.ShowHUDInfoCenter("Press 'Cannon Mount' key to use Cannon.");
            if (InputManager.IsInputCannonDown((int) InputCannon.CannonMount))
                myCannonRegion.BasePV.RPC("RequestControlRPC", PhotonTargets.MasterClient, BasePV.viewID);
        }

        if (State == HeroState.Grab && !Gunner)
        {
            switch (skillID)
            {
                case "jean":
                {
                    if (State != HeroState.Attack &&
                        (InputManager.IsInputDown[InputCode.Attack0] || InputManager.IsInputDown[InputCode.Attack1]) &&
                        escapeTimes > 0 && !baseA.IsPlaying("grabbed_jean"))
                    {
                        PlayAnimation("grabbed_jean");
                        baseA["grabbed_jean"].time = 0f;
                        escapeTimes--;
                    }

                    if (baseA.IsPlaying("grabbed_jean") && baseA["grabbed_jean"].normalizedTime > 0.64f &&
                        titanWhoGrabMe.GetComponent<TITAN>())
                    {
                        Ungrabbed();
                        baseR.velocity = Vectors.up * 30f;
                        if (IN_GAME_MAIN_CAMERA.GameType == GameType.Single)
                        {
                            titanWhoGrabMe.GetComponent<TITAN>().grabbedTargetEscape();
                        }
                        else
                        {
                            BasePV.RPC("netSetIsGrabbedFalse", PhotonTargets.All);
                            if (PhotonNetwork.IsMasterClient)
                                titanWhoGrabMe.GetComponent<TITAN>().grabbedTargetEscape();
                            else
                                PhotonView.Find(titanWhoGrabMeID).RPC("grabbedTargetEscape", PhotonTargets.MasterClient);
                        }
                    }

                    break;
                }
                case "eren":
                {
                    ShowSkillCd();
                    if (IN_GAME_MAIN_CAMERA.GameType != GameType.Single ||
                        IN_GAME_MAIN_CAMERA.GameType == GameType.Single && !IN_GAME_MAIN_CAMERA.isPausing)
                    {
                        CalcSkillCd();
                        CalcFlareCd();
                    }

                    if (InputManager.IsInputDown[InputCode.Attack1])
                    {
                        var flag = false;
                        if (skillCDDuration <= 0f && !flag)
                        {
                            skillCDDuration = skillCDLast;
                            if (skillID == "eren" && titanWhoGrabMe.GetComponent<TITAN>())
                            {
                                Ungrabbed();
                                if (IN_GAME_MAIN_CAMERA.GameType == GameType.Single)
                                {
                                    titanWhoGrabMe.GetComponent<TITAN>().grabbedTargetEscape();
                                }
                                else
                                {
                                    BasePV.RPC("netSetIsGrabbedFalse", PhotonTargets.All);
                                    if (PhotonNetwork.IsMasterClient)
                                        titanWhoGrabMe.GetComponent<TITAN>().grabbedTargetEscape();
                                    else
                                        PhotonView.Find(titanWhoGrabMeID).BasePV.RPC("grabbedTargetEscape",
                                            PhotonTargets.MasterClient);
                                }

                                ErenTransform();
                                return;
                            }
                        }
                    }

                    break;
                }
            }

            return;
        }

        if (titanForm || isCannon)
        {
            if (isCannon)
            {
                ShowAimUI();
                CalcSkillCd();
                ShowSkillCd();
            }

            return;
        }

        BufferUpdate();
        if (!grounded && State != HeroState.AirDodge)
        {
            if (InputManager.IsInputRebind((int) InputRebinds.GasBurst))
            {
                if (InputManager.IsInput[(int) InputCodes.Forward])
                    Dash(0f, 1f);
                else if (InputManager.IsInput[(int) InputCodes.Backward])
                    Dash(0f, -1f);
                else if (InputManager.IsInput[(int) InputCodes.Left])
                    Dash(-1f, 0f);
                else if (InputManager.IsInput[(int) InputCodes.Right]) Dash(1f, 0f);
            }
            else
            {
                if (InputManager.GasBurstType.Value == 0) CheckDashDoubleTap();
            }
        }

        if (grounded && (State == HeroState.Idle || State == HeroState.Slide))
        {
            if (InputManager.IsInputDown[InputCode.Gas] && !baseA.IsPlaying("jump") && !baseA.IsPlaying("horse_geton"))
            {
                idle();
                CrossFade("jump", 0.1f);
                sparks.enableEmission = false;
            }

            if (InputManager.IsInputDown[InputCode.Dodge] && !baseA.IsPlaying("jump") &&
                !baseA.IsPlaying("horse_geton"))
            {
                Dodge();
                return;
            }
        }

        switch (state)
        {
            case HeroState.Idle:

                if (InputManager.IsInputDown[InputCode.Flare1]) ShootFlare(1);
                if (InputManager.IsInputDown[InputCode.Flare2]) ShootFlare(2);
                if (InputManager.IsInputDown[InputCode.Flare3]) ShootFlare(3);
                if (InputManager.IsInputDown[InputCode.Restart]) Suicide();
                if (myHorse != null && isMounted && InputManager.IsInputDown[InputCode.Dodge]) GetOffHorse();
                if ((baseA.IsPlaying(standAnimation) || !grounded) && InputManager.IsInputDown[InputCode.Reload])
                    if (!Gunner || !GameModes.NoAhssReload.Enabled)
                    {
                        ChangeBlade();
                        if (Gunner) ShowBullets();
                        else ShowBlades();
                        return;
                    }

                if (baseA.IsPlaying(standAnimation) && InputManager.IsInputDown[InputCode.Salute])
                {
                    Salute();
                    return;
                }

                if (!isMounted &&
                    (InputManager.IsInputDown[InputCode.Attack0] || InputManager.IsInputDown[InputCode.Attack1]) &&
                    !Gunner)
                {
                    var flag2 = false;
                    if (InputManager.IsInputDown[InputCode.Attack1])
                    {
                        if (skillCDDuration > 0f || flag2)
                        {
                            flag2 = true;
                        }
                        else
                        {
                            skillCDDuration = skillCDLast;
                            switch (skillID)
                            {
                                case "eren":
                                    ErenTransform();
                                    return;

                                case "marco":
                                    if (IsGrounded())
                                    {
                                        attackAnimation = Random.Range(0, 2) != 0
                                            ? "special_marco_1"
                                            : "special_marco_0";
                                        PlayAnimation(attackAnimation);
                                    }
                                    else
                                    {
                                        flag2 = true;
                                        skillCDDuration = 0f;
                                    }

                                    break;

                                case "armin":
                                    if (IsGrounded())
                                    {
                                        attackAnimation = "special_armin";
                                        PlayAnimation("special_armin");
                                    }
                                    else
                                    {
                                        flag2 = true;
                                        skillCDDuration = 0f;
                                    }

                                    break;

                                case "sasha":
                                    if (IsGrounded())
                                    {
                                        attackAnimation = "special_sasha";
                                        PlayAnimation("special_sasha");
                                        currentBuff = Buff.SpeedUp;
                                        buffTime = 10f;
                                    }
                                    else
                                    {
                                        flag2 = true;
                                        skillCDDuration = 0f;
                                    }

                                    break;

                                case "mikasa":
                                    attackAnimation = "attack3_1";
                                    PlayAnimation("attack3_1");
                                    baseR.velocity = Vectors.up * 10f;
                                                                            if (bulletRight)
                                        {
                                            bulletRight.Disable();
                                            ReleaseIfIHookSb();
                                        }

                                        if (bulletLeft)
                                        {
                                            bulletLeft.Disable();
                                            ReleaseIfIHookSb();
                                        }
                                    break;

                                case "levi":
                                    attackAnimation = "attack5";
                                    PlayAnimation("attack5");
                                    baseR.velocity += Vectors.up * 5f;
                                    var ray = IN_GAME_MAIN_CAMERA.BaseCamera.ScreenPointToRay(Input.mousePosition);
                                    RaycastHit hit;
                                    if (Physics.Raycast(ray, out hit, 1E+07f, Layers.EnemyGround.value))
                                    {
                                        if (bulletRight)
                                        {
                                            bulletRight.Disable();
                                            ReleaseIfIHookSb();
                                        }

                                        if (bulletLeft)
                                        {
                                            bulletLeft.Disable();
                                            ReleaseIfIHookSb();
                                        }

                                        dashDirection = hit.point - baseT.position;
                                        LaunchRightRope(hit, true, 1);
                                        rope.Play();
                                    }

                                    facingDirection = Mathf.Atan2(dashDirection.x, dashDirection.z) * 57.29578f;
                                    targetRotation = Quaternion.Euler(0f, facingDirection, 0f);
                                    attackLoop = 3;
                                    break;

                                case "petra":
                                    attackAnimation = "special_petra";
                                    PlayAnimation("special_petra");
                                    baseR.velocity += Vectors.up * 5f;
                                    var ray2 = IN_GAME_MAIN_CAMERA.BaseCamera.ScreenPointToRay(Input.mousePosition);
                                    RaycastHit hit2;
                                    if (Physics.Raycast(ray2, out hit2, 1E+07f, Layers.EnemyGround.value))
                                    {
                                        if (bulletRight)
                                        {
                                            bulletRight.Disable();
                                            ReleaseIfIHookSb();
                                        }

                                        if (bulletLeft)
                                        {
                                            bulletLeft.Disable();
                                            ReleaseIfIHookSb();
                                        }

                                        dashDirection = hit2.point - baseT.position;
                                        LaunchLeftRope(hit2, true);
                                        LaunchRightRope(hit2, true);
                                        rope.Play();
                                    }

                                    facingDirection = Mathf.Atan2(dashDirection.x, dashDirection.z) * 57.29578f;
                                    targetRotation = Quaternion.Euler(0f, facingDirection, 0f);
                                    attackLoop = 3;
                                    break;

                                default:
                                    if (this.needLean)
                                    {
                                        if (InputManager.IsInput[InputCode.Left])
                                            attackAnimation = Random.Range(0, 100) >= 50 ? "attack1_hook_l1" : "attack1_hook_l2";
                                        else if (InputManager.IsInput[InputCode.Right])
                                            attackAnimation = Random.Range(0, 100) >= 50 ? "attack1_hook_r1" : "attack1_hook_r2";
                                        else if (leanLeft)
                                            attackAnimation = Random.Range(0, 100) >= 50 ? "attack1_hook_l1" : "attack1_hook_l2";
                                        else
                                            attackAnimation = Random.Range(0, 100) >= 50 ? "attack1_hook_r1" : "attack1_hook_r2";
                                    }
                                    else
                                    {
                                        attackAnimation = "attack1";
                                    }
                                    PlayAnimation(attackAnimation);
                                    break;
                            }
                        }
                    }
                    else if (InputManager.IsInputDown[InputCode.Attack0])
                    {
                        if (this.needLean)
                        {
                            if (InputManager.IsInput[InputCode.Left])
                                attackAnimation = Random.Range(0, 100) >= 50 ? "attack1_hook_l1" : "attack1_hook_l2";
                            else if (InputManager.IsInput[InputCode.Right])
                                attackAnimation = Random.Range(0, 100) >= 50 ? "attack1_hook_r1" : "attack1_hook_r2";
                            else if (leanLeft)
                                attackAnimation = Random.Range(0, 100) >= 50 ? "attack1_hook_l1" : "attack1_hook_l2";
                            else
                                attackAnimation = Random.Range(0, 100) >= 50 ? "attack1_hook_r1" : "attack1_hook_r2";
                        }
                        else if (InputManager.IsInput[InputCode.Left])
                        {
                            attackAnimation = "attack2";
                        }
                        else if (InputManager.IsInput[InputCode.Right])
                        {
                            attackAnimation = "attack1";
                        }
                        else if (LastHook != null)
                        {
                            if (LastHook.MyTitan != null)
                                AttackAccordingToTarget(LastHook.MyTitan.Neck);
                            else
                            {
                                AttackAccordingToMouse();
                            }
                        }
                        else if (bulletLeft != null && bulletLeft.MyTitan != null)
                        {
                            AttackAccordingToTarget(bulletLeft.MyTitan.Neck);
                        }
                        else if (bulletRight != null && bulletRight.MyTitan != null)
                        {
                            AttackAccordingToTarget(bulletRight.MyTitan.Neck);
                        }
                        else
                        {
                            var obj2 = FindNearestTitan();
                            if (obj2 != null)
                                AttackAccordingToTarget(obj2.Neck);
                            else
                                AttackAccordingToMouse();
                        }
                    }

                    if (!flag2)
                    {
                        wLeft.clearHits();
                        wRight.clearHits();
                        if (grounded) baseR.AddForce(baseGT.forward * 200f);
                        PlayAnimation(attackAnimation);
                        baseA[attackAnimation].time = 0f;
                        buttonAttackRelease = false;
                        State = HeroState.Attack;
                        if (grounded || attackAnimation == "attack3_1" || attackAnimation == "attack5" ||
                            attackAnimation == "special_petra")
                        {
                            attackReleased = true;
                            buttonAttackRelease = true;
                        }
                        else
                        {
                            attackReleased = false;
                        }

                        sparks.enableEmission = false;
                    }
                }

                if (Gunner)
                {
                    if (InputManager.IsInput[InputCode.Attack1])
                    {
                        leftArmAim = true;
                        rightArmAim = true;
                    }
                    else if (InputManager.IsInput[InputCode.Attack0])
                    {
                        if (leftGunHasBullet)
                        {
                            leftArmAim = true;
                            rightArmAim = false;
                        }
                        else
                        {
                            leftArmAim = false;
                            if (rightGunHasBullet)
                                rightArmAim = true;
                            else
                                rightArmAim = false;
                        }
                    }
                    else
                    {
                        leftArmAim = false;
                        rightArmAim = false;
                    }

                    if (leftArmAim || rightArmAim)
                    {
                        var ray3 = IN_GAME_MAIN_CAMERA.BaseCamera.ScreenPointToRay(Input.mousePosition);
                        RaycastHit raycastHit;
                        if (Physics.Raycast(ray3, out raycastHit, 1E+07f, Layers.EnemyGround.value))
                            gunTarget = raycastHit.point;
                    }

                    var flag3 = false;
                    var flag4 = false;
                    var flag5 = false;
                    if (InputManager.Settings[InputCode.Attack1].IsKeyUp())
                    {
                        if (leftGunHasBullet && rightGunHasBullet)
                        {
                            if (grounded)
                                attackAnimation = "AHSS_shoot_both";
                            else
                                attackAnimation = "AHSS_shoot_both_air";
                            flag3 = true;
                        }
                        else if (!leftGunHasBullet && !rightGunHasBullet)
                        {
                            flag4 = true;
                        }
                        else
                        {
                            flag5 = true;
                        }
                    }

                    if (flag5 || InputManager.Settings[InputCode.Attack0].IsKeyUp())
                    {
                        if (grounded)
                        {
                            if (leftGunHasBullet && rightGunHasBullet)
                            {
                                if (isLeftHandHooked)
                                    attackAnimation = "AHSS_shoot_r";
                                else
                                    attackAnimation = "AHSS_shoot_l";
                            }
                            else if (leftGunHasBullet)
                            {
                                attackAnimation = "AHSS_shoot_l";
                            }
                            else if (rightGunHasBullet)
                            {
                                attackAnimation = "AHSS_shoot_r";
                            }
                        }
                        else if (leftGunHasBullet && rightGunHasBullet)
                        {
                            if (isLeftHandHooked)
                                attackAnimation = "AHSS_shoot_r_air";
                            else
                                attackAnimation = "AHSS_shoot_l_air";
                        }
                        else if (leftGunHasBullet)
                        {
                            attackAnimation = "AHSS_shoot_l_air";
                        }
                        else if (rightGunHasBullet)
                        {
                            attackAnimation = "AHSS_shoot_r_air";
                        }

                        if (leftGunHasBullet || rightGunHasBullet)
                            flag3 = true;
                        else
                            flag4 = true;
                    }

                    if (flag3)
                    {
                        State = HeroState.Attack;
                        CrossFade(attackAnimation, 0.05f);
                        gunDummy.transform.position = baseT.position;
                        gunDummy.transform.rotation = baseT.rotation;
                        gunDummy.transform.LookAt(gunTarget);
                        attackReleased = false;
                        facingDirection = gunDummy.transform.rotation.eulerAngles.y;
                        targetRotation = Quaternion.Euler(0f, facingDirection, 0f);
                        ShowBullets();
                    }
                    else if (flag4 && (grounded || FengGameManagerMKII.Level.Mode != GameMode.PVP_AHSS))
                    {
                            ChangeBlade();
                        ShowBullets();
                    }
                }

                break;

            case HeroState.Attack:
                if (!Gunner)
                {
                    if (!InputManager.IsInput[InputCode.Attack0]) buttonAttackRelease = true;
                    if (!attackReleased)
                    {
                        if (buttonAttackRelease)
                        {
                            ContinueAnimation();
                            attackReleased = true;
                        }
                        else if (baseA[attackAnimation].normalizedTime >= 0.32f)
                        {
                            PauseAnimation();
                        }
                    }

                    if (attackAnimation == "attack3_1" && currentBladeSta > 0f)
                    {
                        if (baseA[attackAnimation].normalizedTime >= 0.8f)
                        {
                            if (!wLeft.Active)
                            {
                                wLeft.Active = true;
                                if (VideoSettings.TrailType.Value == 0)
                                {
                                    leftbladetrail.Activate();
                                    rightbladetrail.Activate();
                                }
                                else if (VideoSettings.TrailType.Value == 1)
                                {
                                    leftbladetrail2.Activate();
                                    rightbladetrail2.Activate();
                                }
                                else
                                {
                                    leftbladetrail2.Activate();
                                    rightbladetrail2.Activate();
                                    leftbladetrail.Activate();
                                    rightbladetrail.Activate();
                                }

                                baseR.velocity = -Vectors.up * 30f;
                            }

                            if (!wRight.Active)
                            {
                                wRight.Active = true;
                                slash.Play();
                            }
                        }
                        else if (wLeft.Active)
                        {
                            wLeft.Active = false;
                            wRight.Active = false;
                            wLeft.clearHits();
                            wRight.clearHits();
                            leftbladetrail.StopSmoothly(0.1f);
                            rightbladetrail.StopSmoothly(0.1f);
                            leftbladetrail2.StopSmoothly(0.1f);
                            rightbladetrail2.StopSmoothly(0.1f);
                        }
                    }
                    else
                    {
                        float num;
                        float num2;
                        if (currentBladeSta == 0f)
                            num = num2 = -1f;
                        else
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

                        if (baseA[attackAnimation].normalizedTime > num && baseA[attackAnimation].normalizedTime < num2)
                        {
                            if (!wLeft.Active)
                            {
                                wLeft.Active = true;
                                slash.Play();
                                if (VideoSettings.TrailType.Value == 0)
                                {
                                    leftbladetrail.Activate();
                                    rightbladetrail.Activate();
                                }
                                else if (VideoSettings.TrailType.Value == 1)
                                {
                                    leftbladetrail2.Activate();
                                    rightbladetrail2.Activate();
                                }
                                else
                                {
                                    leftbladetrail2.Activate();
                                    rightbladetrail2.Activate();
                                    leftbladetrail.Activate();
                                    rightbladetrail.Activate();
                                }
                            }

                            if (!wRight.Active) wRight.Active = true;
                        }
                        else if (wLeft.Active)
                        {
                            wLeft.Active = false;
                            wRight.Active = false;
                            wLeft.clearHits();
                            wRight.clearHits();
                            leftbladetrail.StopSmoothly(0.1f);
                            rightbladetrail.StopSmoothly(0.1f);
                            leftbladetrail2.StopSmoothly(0.1f);
                            rightbladetrail2.StopSmoothly(0.1f);
                        }

                        if (attackLoop > 0 && baseA[attackAnimation].normalizedTime > num2)
                        {
                            attackLoop--;
                            PlayAnimationAt(attackAnimation, num);
                        }
                    }

                    if (baseA[attackAnimation].normalizedTime >= 1f)
                    {
                        if (attackAnimation == "special_marco_0" || attackAnimation == "special_marco_1")
                        {
                            if (IN_GAME_MAIN_CAMERA.GameType != GameType.Single)
                            {
                                if (!PhotonNetwork.IsMasterClient)
                                {
                                    object[] parameters = {5f, 100f};
                                    BasePV.RPC("netTauntAttack", PhotonTargets.MasterClient, parameters);
                                }
                                else
                                {
                                    netTauntAttack(5f);
                                }
                            }
                            else
                            {
                                netTauntAttack(5f);
                            }

                            FalseAttack();
                            idle();
                        }
                        else if (attackAnimation == "special_armin")
                        {
                            if (IN_GAME_MAIN_CAMERA.GameType != GameType.Single)
                            {
                                if (!PhotonNetwork.IsMasterClient)
                                    BasePV.RPC("netlaughAttack", PhotonTargets.MasterClient);
                                else
                                    netlaughAttack();
                            }
                            else
                            {
                                foreach (var obj3 in GameObject.FindGameObjectsWithTag("titan"))
                                    if (Vector3.Distance(obj3.transform.position, baseT.position) < 50f &&
                                        Vector3.Angle(obj3.transform.Forward(),
                                            baseT.position - obj3.transform.position) < 90f &&
                                        obj3.GetComponent<TITAN>() != null)
                                        obj3.GetComponent<TITAN>().BeLaughAttacked();
                            }

                            FalseAttack();
                            idle();
                        }
                        else if (attackAnimation == "attack3_1")
                        {
                            baseR.velocity -= Vectors.up * dt * 30f;
                        }
                        else
                        {
                            FalseAttack();
                            idle();
                        }
                    }

                    if (baseA.IsPlaying("attack3_2") && baseA["attack3_2"].normalizedTime >= 1f)
                    {
                        FalseAttack();
                        idle();
                    }
                }
                else
                {
                    baseT.rotation = Quaternion.Lerp(baseT.rotation, gunDummy.transform.rotation, dt * 30f);
                    if (!attackReleased && baseA[attackAnimation].normalizedTime > 0.167f)
                    {
                        GameObject obj4;
                        attackReleased = true;
                        var flag6 = false;
                        if (attackAnimation == "AHSS_shoot_both" || attackAnimation == "AHSS_shoot_both_air")
                        {
                            flag6 = true;
                            {
                                leftGunHasBullet = false;
                                rightGunHasBullet = false;
                            }

                            baseR.AddForce(-baseT.Forward() * 1000f, ForceMode.Acceleration);
                            ShowBullets();
                        }
                        else
                        {
                            {
                                if (attackAnimation == "AHSS_shoot_l" || attackAnimation == "AHSS_shoot_l_air")
                                    leftGunHasBullet = false;
                                else
                                    rightGunHasBullet = false;
                            }

                            baseR.AddForce(-baseT.Forward() * 600f, ForceMode.Acceleration);
                            ShowBullets();
                        }

                        baseR.AddForce(Vector3.up * 200f, ForceMode.Acceleration);
                        var prefabName = "FX/shotGun";
                        if (flag6) prefabName = "FX/shotGun 1";
                        if (IN_GAME_MAIN_CAMERA.GameType == GameType.MultiPlayer && BasePV.IsMine)
                        {
                            obj4 = Pool.NetworkEnable(prefabName,
                                baseT.position + baseT.Up() * 0.8f - baseT.Right() * 0.1f, baseT.rotation);
                            if (obj4.GetComponent<EnemyfxIDcontainer>() != null)
                                obj4.GetComponent<EnemyfxIDcontainer>().myOwnerViewID = BasePV.viewID;
                        }
                        else
                        {
                            Pool.Enable(prefabName, baseT.position + baseT.Up() * 0.8f - baseT.Right() * 0.1f,
                                baseT.rotation);
                        }
                    }

                    if (baseA[attackAnimation].normalizedTime >= 1f)
                    {
                        FalseAttack();
                        idle();
                    }

                    if (!baseA.IsPlaying(attackAnimation))
                    {
                        FalseAttack();
                        idle();
                    }
                }

                break;

            case HeroState.ChangeBlade:
                if (Gunner)
                {
                    if (baseA[reloadAnimation].normalizedTime > 0.22f)
                    {
                        if (!leftGunHasBullet && Setup.part_blade_l.activeSelf)
                        {
                            Setup.part_blade_l.SetActive(false);
                            var transform4 = Setup.part_blade_l.transform;
                            var gameObject4 = (GameObject) Instantiate(
                                CacheResources.Load("Character_parts/character_gun_l"), transform4.position,
                                transform4.rotation);
                            gameObject4.renderer.material = CharacterMaterials.Materials[Setup.myCostume._3dmg_texture];
                            var force = -baseT.Forward() * 10f + baseT.Up() * 5f - baseT.Right();
                            gameObject4.rigidbody.AddForce(force, ForceMode.Impulse);
                            var torque = new Vector3(Random.Range(-100, 100), Random.Range(-100, 100),
                                Random.Range(-100, 100));
                            gameObject4.rigidbody.AddTorque(torque, ForceMode.Acceleration);
                        }

                        if (!rightGunHasBullet && Setup.part_blade_r.activeSelf)
                        {
                            Setup.part_blade_r.SetActive(false);
                            var transform5 = Setup.part_blade_r.transform;
                            var gameObject5 = (GameObject) Instantiate(
                                CacheResources.Load("Character_parts/character_gun_r"), transform5.position,
                                transform5.rotation);
                            gameObject5.renderer.material = CharacterMaterials.Materials[Setup.myCostume._3dmg_texture];
                            var force2 = -baseT.Forward() * 10f + baseT.Up() * 5f + baseT.Right();
                            gameObject5.rigidbody.AddForce(force2, ForceMode.Impulse);
                            var torque2 = new Vector3(Random.Range(-300, 300), Random.Range(-300, 300),
                                Random.Range(-300, 300));
                            gameObject5.rigidbody.AddTorque(torque2, ForceMode.Acceleration);
                        }
                    }

                    if (baseA[reloadAnimation].normalizedTime > 0.62f && !throwedBlades)
                    {
                        throwedBlades = true;
                        if (leftBulletLeft > 0 && !leftGunHasBullet)
                        {
                            leftBulletLeft--;
                            Setup.part_blade_l.SetActive(true);
                            leftGunHasBullet = true;
                            ShowBullets();
                        }

                        if (rightBulletLeft > 0 && !rightGunHasBullet)
                        {
                            Setup.part_blade_r.SetActive(true);
                            rightBulletLeft--;
                            rightGunHasBullet = true;
                            ShowBullets();
                        }

                        UpdateRightMagUI();
                        UpdateLeftMagUI();
                        ShowBullets();
                    }

                    if (baseA[reloadAnimation].normalizedTime > 1f) idle();
                }
                else
                {
                    if (!grounded)
                    {
                        if (baseA[reloadAnimation].normalizedTime >= 0.2f && !throwedBlades)
                        {
                            throwedBlades = true;
                            if (Setup.part_blade_l.activeSelf) ThrowBlades();
                        }

                        if (baseA[reloadAnimation].normalizedTime >= 0.56f && currentBladeNum > 0)
                        {
                            Setup.part_blade_l.SetActive(true);
                            Setup.part_blade_r.SetActive(true);
                            currentBladeSta = totalBladeSta;
                            ShowBlades();
                        }
                    }
                    else
                    {
                        if (baseA[reloadAnimation].normalizedTime >= 0.13f && !throwedBlades)
                        {
                            throwedBlades = true;
                            if (Setup.part_blade_l.activeSelf) ThrowBlades();
                        }

                        if (baseA[reloadAnimation].normalizedTime >= 0.37f && currentBladeNum > 0)
                        {
                            Setup.part_blade_l.SetActive(true);
                            Setup.part_blade_r.SetActive(true);
                            currentBladeSta = totalBladeSta;
                            ShowBlades();
                        }
                    }

                    ShowBlades();
                    if (baseA[reloadAnimation].normalizedTime >= 1f) idle();
                }

                break;

            case HeroState.Salute:
                if (baseA["salute"].normalizedTime >= 1f) idle();
                break;

            case HeroState.GroundDodge:
                if (baseA.IsPlaying("dodge"))
                {
                    if (!grounded && baseA["dodge"].normalizedTime > 0.6f) idle();
                    if (baseA["dodge"].normalizedTime >= 1f) idle();
                }

                break;

            case HeroState.Land:
                if (baseA.IsPlaying("dash_land") && baseA["dash_land"].normalizedTime >= 1f) idle();
                break;

            case HeroState.FillGas:
                if (baseA.IsPlaying("supply") && baseA["supply"].normalizedTime >= 1f)
                {
                    if (IN_GAME_MAIN_CAMERA.GameType == GameType.Single) SingleRunStats.Refill();
                    currentBladeSta = totalBladeSta;
                    currentBladeNum = TotalBladeNum;
                    currentGas = totalGas;
                    ShowGas();
                    if (!Gunner)
                    {
                        Setup.part_blade_l.SetActive(true);
                        Setup.part_blade_r.SetActive(true);
                        ShowBlades();
                    }
                    else
                    {
                        leftBulletLeft = rightBulletLeft = bulletMAX;
                        leftGunHasBullet = rightGunHasBullet = true;
                        Setup.part_blade_l.SetActive(true);
                        Setup.part_blade_r.SetActive(true);
                        UpdateRightMagUI();
                        UpdateLeftMagUI();
                        ShowBullets();
                    }

                    idle();
                    if (!Gunner) ShowBlades();
                    else ShowBullets();
                }

                break;

            case HeroState.Slide:
                if (!grounded) idle();
                break;

            case HeroState.AirDodge:
                if (dashTime > 0f)
                {
                    dashTime -= dt;
                    if (currentSpeed > originVM) baseR.AddForce(-baseR.velocity * dt * 1.7f, ForceMode.VelocityChange);
                }
                else
                {
                    dashTime = 0f;
                    idle();
                }

                break;
        }

        if (!baseA.IsPlaying("attack3_1") && !baseA.IsPlaying("attack5") && !baseA.IsPlaying("special_petra") &&
            State != HeroState.Grab)
        {
            if (InputManager.IsInput[InputCode.LeftRope])
            {
                if (bulletLeft)
                {
                    qHold = true;
                }
                else
                {
                    var ray4 = IN_GAME_MAIN_CAMERA.BaseCamera.ScreenPointToRay(Input.mousePosition);
                    RaycastHit hit3;
                    if (Physics.Raycast(ray4, out hit3, 10000f, Layers.EnemyGround.value))
                    {
                        LaunchLeftRope(hit3, true);
                        rope.Play();
                    }
                }
            }
            else
            {
                qHold = false;
            }

            if (InputManager.IsInput[InputCode.RightRope])
            {
                if (bulletRight)
                {
                    EHold = true;
                }
                else
                {
                    var ray5 = IN_GAME_MAIN_CAMERA.BaseCamera.ScreenPointToRay(Input.mousePosition);
                    RaycastHit hit4;
                    if (Physics.Raycast(ray5, out hit4, 10000f, Layers.EnemyGround.value))
                    {
                        LaunchRightRope(hit4, true);
                        rope.Play();
                    }
                }
            }
            else
            {
                EHold = false;
            }

            if (InputManager.IsInput[InputCode.BothRope])
            {
                qHold = true;
                EHold = true;
                if (!bulletLeft && !bulletRight)
                {
                    var ray6 = IN_GAME_MAIN_CAMERA.BaseCamera.ScreenPointToRay(Input.mousePosition);
                    RaycastHit hit5;
                    if (Physics.Raycast(ray6, out hit5, 1000000f, Layers.EnemyGround.value))
                    {
                        LaunchLeftRope(hit5, false);
                        LaunchRightRope(hit5, false);
                        rope.Play();
                    }
                }
            }
        }

        if (VideoSettings.BladeTrails && !Gunner)
        {
            if (leftbladetrail.gameObject.activeInHierarchy)
            {
                leftbladetrail.update();
                rightbladetrail.update();
            }

            if (leftbladetrail2.gameObject.activeInHierarchy)
            {
                leftbladetrail2.update();
                rightbladetrail2.update();
            }

            if (leftbladetrail.gameObject.activeInHierarchy)
            {
                leftbladetrail.lateUpdate();
                rightbladetrail.lateUpdate();
            }

            if (leftbladetrail2.gameObject.activeInHierarchy)
            {
                leftbladetrail2.lateUpdate();
                rightbladetrail2.lateUpdate();
            }

            //if (VideoSettings.TrailType.Value == 0)
            //{
            //    this.leftbladetrail.update();
            //    this.rightbladetrail.update();
            //    this.leftbladetrail.lateUpdate();
            //    this.rightbladetrail.lateUpdate();
            //}
            //else if(VideoSettings.TrailType.Value == 1)
            //{
            //    this.leftbladetrail2.lateUpdate();
            //    this.rightbladetrail2.lateUpdate();
            //}
            //else
            //{
            //    this.leftbladetrail.update();
            //    this.rightbladetrail.update();
            //    this.leftbladetrail2.update();
            //    this.rightbladetrail2.update();
            //    this.leftbladetrail.lateUpdate();
            //    this.rightbladetrail.lateUpdate();
            //    this.leftbladetrail2.lateUpdate();
            //    this.rightbladetrail2.lateUpdate();
            //}
        }

        if (IN_GAME_MAIN_CAMERA.isPausing) return;
        CalcSkillCd();
        CalcFlareCd();
        ShowSkillCd();
        ShowFlareCD();
        ShowAimUI();
        var checkAxis = Input.GetAxis("Mouse ScrollWheel");
        if (checkAxis != 0f)
        {
            var flag2 = false;
            var flag3 = false;
            if (isLaunchLeft && bulletLeft != null && bulletLeft.IsHooked())
            {
                isLeftHandHooked = true;
                var vector5 = bulletLeft.baseT.position - baseT.position;
                vector5.Normalize();
                vector5 *= 10f;
                if (!isLaunchRight) vector5 *= 2f;
                if (Vector3.Angle(baseR.velocity, vector5) > 90f && InputManager.IsInput[InputCode.Gas]) flag2 = true;
            }

            if (isLaunchRight && bulletRight != null && bulletRight.IsHooked())
            {
                isRightHandHooked = true;
                var vector6 = bulletRight.baseT.position - baseT.position;
                vector6.Normalize();
                vector6 *= 10f;
                if (!isLaunchLeft) vector6 *= 2f;
                if (Vector3.Angle(baseR.velocity, vector6) > 90f && InputManager.IsInput[InputCode.Gas]) flag3 = true;
            }

            var current = Vectors.zero;
            if (flag2 && flag3)
                current = (bulletRight.baseT.position + bulletLeft.baseT.position) * 0.5f - baseT.position;
            else if (flag2 && !flag3)
                current = bulletLeft.baseT.position - baseT.position;
            else if (flag3 && !flag2) current = bulletRight.baseT.position - baseT.position;
            if (flag2 || flag3)
            {
                baseR.AddForce(-baseR.velocity, ForceMode.VelocityChange);
                var idk = 1.53938f * (1f + Mathf.Clamp(checkAxis > 0 ? 1f : -1f, -0.8f, 0.8f));
                reelAxis = 0f;
                baseR.velocity = Vector3.RotateTowards(current, baseR.velocity, idk, idk).normalized *
                                 (currentSpeed + 0.1f);
            }
        }
    }


    public void UpdateCannon()
    {
        baseT.position = myCannonPlayer.position;
        baseT.rotation = myCannonBase.rotation;
    }

    public void useBlade(int amount = 0)
    {
        if (amount == 0) amount = 1;
        amount *= 2;
        if (currentBladeSta > 0f)
        {
            currentBladeSta -= amount;
            if (currentBladeSta <= 0f)
            {
                if (IsLocal)
                {
                    leftbladetrail.Deactivate();
                    rightbladetrail.Deactivate();
                    leftbladetrail2.Deactivate();
                    rightbladetrail2.Deactivate();
                    wLeft.Active = false;
                    wRight.Active = false;
                }

                currentBladeSta = 0f;
                ThrowBlades();
            }
        }

        ShowBlades();
    }
}
