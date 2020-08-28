using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Anarchy;
using Anarchy.Configuration;
using Anarchy.Configuration.Presets;
using Anarchy.Skins.Titans;
using Anarchy.UI;
using Optimization;
using Optimization.Caching;
using Optimization.Caching.Bases;
using RC;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Random = UnityEngine.Random;

[SuppressMessage("ReSharper", "InconsistentNaming")]
public partial class TITAN : TitanBase
{
    private static readonly string[] titanNames = {"Titan", "Aberrant", "Jumper", "Crawler", "Punk"};

    public static float minusDistance = 99999f;
    public static GameObject minusDistanceEnemy;
    private const float Gravity = 120f;
    private const float MaxStamina = 320f;

    private Vector3 abnorma_jump_bite_horizon_v;
    public AbnormalType abnormalType;
    public int activeRad = int.MaxValue;
    private float angle;
    internal int armor;
    public bool asClientLookTarget;
    private string attackAnimation;
    private float attackCheckTime;
    private float attackCheckTimeA;
    private float attackCheckTimeB;
    private int attackCount;
    public float attackDistance = 13f;
    private bool attacked;
    private float attackEndWait;
    public float attackWait = 1f;
    private List<Collider> baseColliders;
    private float between2;
    public float chaseDistance = 80f;
    public ArrayList checkPoints = new ArrayList();
    public bool ColliderEnabled;
    public TITAN_CONTROLLER controller;
    public GameObject currentCamera;
    private Transform currentGrabHand;
    public int currentHealth;
    private float desDeg;
    private float dieTime;
    private string fxName;
    private Vector3 fxPosition;
    private Quaternion fxRotation;
    private float getdownTime;
    private HERO grabbedTarget;
    public GameObject grabTF;
    private bool grounded;
    public bool hasDie;
    private bool hasDieSteam;
    private bool hasExplode;
    public bool hasSetLevel;
    private Vector3 headscale = Vectors.one;
    public GameObject healthLabel;
    public bool healthLabelEnabled;
    private float healthTime;
    private string hitAnimation;
    private float hitPause;

    public bool isAlarm;
    private bool isAttackMoveByCore;
    private bool isGrabHandLeft;
    public bool IsHooked;
    public bool IsLook;
    private float lagMax;
    private bool leftHandAttack;
    public GameObject mainMaterial;
    public int maxHealth;
    public float maxVelocityChange = 10f;
    public int myDifficulty;
    public float myDistance;
    public Group myGroup = Group.T;
    public GameObject myHero;

    public float myLevel = 1f;
    public TitanTrigger MyTitanTrigger;
    private bool needFreshCorePosition;
    private string nextAttackAnimation;
    public bool nonAI;
    private bool nonAIcombo;
    private Vector3 oldCorePosition;
    private Quaternion oldHeadRotation;
    public PVPcheckPoint PVPfromCheckPt;
    private float random_run_time;
    private float rockInterval;
    private string runAnimation;
    private float sbtime;
    internal TitanSkin Skin;
    private bool spawned;
    private Vector3 spawnPt;
    public float speed = 7f;
    private float stamina = 320f;
    private TitanState state;
    private int stepSoundPhase = 2;
    private bool stuck;
    private float stuckTime;
    private float stuckTurnAngle;
    private Vector3 targetCheckPt;
    private Quaternion targetHeadRotation;
    private float targetR;
    private float tauntTime;
    private GameObject throwRock;
    private string turnAnimation;
    private float turnDeg;
    private GameObject whoHasTauntMe;

    public string ShowName { get; private set; }


    private void Attack(string type)
    {
        state = TitanState.Attack;
        attacked = false;
        isAlarm = true;
        if (attackAnimation == type)
        {
            attackAnimation = type;
            PlayAnimationAt("attack_" + type, 0f);
        }
        else
        {
            attackAnimation = type;
            PlayAnimationAt("attack_" + type, 0f);
        }

        nextAttackAnimation = null;
        fxName = null;
        isAttackMoveByCore = false;
        attackCheckTime = 0f;
        attackCheckTimeA = 0f;
        attackCheckTimeB = 0f;
        attackEndWait = 0f;
        fxRotation = Quaternion.Euler(270f, 0f, 0f);
        switch (type)
        {
            case "abnormal_getup":
                attackCheckTime = 0f;
                fxName = string.Empty;
                break;

            case "abnormal_jump":
                nextAttackAnimation = "abnormal_getup";
                if (nonAI)
                    attackEndWait = 0f;
                else
                    attackEndWait = myDifficulty <= 0 ? Random.Range(1f, 4f) : Random.Range(0f, 1f);
                attackCheckTime = 0.75f;
                fxName = "boom4";
                fxRotation = Quaternion.Euler(270f, baseT.rotation.eulerAngles.y, 0f);
                break;

            case "combo_1":
                nextAttackAnimation = "combo_2";
                attackCheckTimeA = 0.54f;
                attackCheckTimeB = 0.76f;
                nonAIcombo = false;
                isAttackMoveByCore = true;
                leftHandAttack = false;
                break;

            case "combo_2":
                if (abnormalType != AbnormalType.Punk) nextAttackAnimation = "combo_3";
                attackCheckTimeA = 0.37f;
                attackCheckTimeB = 0.57f;
                nonAIcombo = false;
                isAttackMoveByCore = true;
                leftHandAttack = true;
                break;

            case "combo_3":
                nonAIcombo = false;
                isAttackMoveByCore = true;
                attackCheckTime = 0.21f;
                fxName = "boom1";
                break;

            case "front_ground":
                fxName = "boom1";
                attackCheckTime = 0.45f;
                break;

            case "kick":
                fxName = "boom5";
                fxRotation = baseT.rotation;
                attackCheckTime = 0.43f;
                break;

            case "slap_back":
                fxName = "boom3";
                attackCheckTime = 0.66f;
                break;

            case "slap_face":
                fxName = "boom3";
                attackCheckTime = 0.655f;
                break;

            case "stomp":
                fxName = "boom2";
                attackCheckTime = 0.42f;
                break;

            case "bite":
                fxName = "bite";
                attackCheckTime = 0.6f;
                break;

            case "bite_l":
                fxName = "bite";
                attackCheckTime = 0.4f;
                break;

            case "bite_r":
                fxName = "bite";
                attackCheckTime = 0.4f;
                break;

            case "jumper_0":
                abnorma_jump_bite_horizon_v = Vectors.zero;
                break;

            case "crawler_jump_0":
                abnorma_jump_bite_horizon_v = Vectors.zero;
                break;

            case "anti_AE_l":
                attackCheckTimeA = 0.31f;
                attackCheckTimeB = 0.4f;
                leftHandAttack = true;
                break;

            case "anti_AE_r":
                attackCheckTimeA = 0.31f;
                attackCheckTimeB = 0.4f;
                leftHandAttack = false;
                break;

            case "anti_AE_low_l":
                attackCheckTimeA = 0.31f;
                attackCheckTimeB = 0.4f;
                leftHandAttack = true;
                break;

            case "anti_AE_low_r":
                attackCheckTimeA = 0.31f;
                attackCheckTimeB = 0.4f;
                leftHandAttack = false;
                break;

            case "quick_turn_l":
                attackCheckTimeA = 2f;
                attackCheckTimeB = 2f;
                isAttackMoveByCore = true;
                break;

            case "quick_turn_r":
                attackCheckTimeA = 2f;
                attackCheckTimeB = 2f;
                isAttackMoveByCore = true;
                break;

            case "throw":
                isAlarm = true;
                chaseDistance = 99999f;
                break;
        }

        needFreshCorePosition = true;
    }

    public void Pt()
    {
        if (controller.sit) SitDown();
        if (controller.bite) Attack("bite");
        if (controller.bitel) Attack("bite_l");
        if (controller.biter) Attack("bite_r");
        if (controller.chopl) Attack("anti_AE_low_l");
        if (controller.chopr) Attack("anti_AE_low_r");
        if (controller.choptl) Attack("anti_AE_l");
        if (controller.choptr) Attack("anti_AE_r");
        if (controller.cover && stamina > 75f)
        {
            Recover();
            stamina -= 75f;
        }

        if (controller.grabbackl) Grab("ground_back_l");
        if (controller.grabbackr) Grab("ground_back_r");
        if (controller.grabfrontl) Grab("ground_front_l");
        if (controller.grabfrontr) Grab("ground_front_r");
        if (controller.grabnapel) Grab("head_back_l");
        if (controller.grabnaper) Grab("head_back_r");
    }

    private void Awake()
    {
        base.Cache();
        baseR.freezeRotation = true;
        baseR.useGravity = false;
        controller = baseG.GetComponent<TITAN_CONTROLLER>();
        baseColliders = new List<Collider>();
        foreach (var childCollider in GetComponentsInChildren<Collider>())
            if (childCollider.name != "AABB")
                baseColliders.Add(childCollider);
        var obj = new GameObject();
        obj.name = "PlayerDetectorRC";
        var triggerCollider = obj.AddComponent<CapsuleCollider>();
        var referenceCollider = AABB.GetComponent<CapsuleCollider>();
        triggerCollider.center = referenceCollider.center;
        triggerCollider.radius = Math.Abs(Head.position.y - baseT.position.y);
        triggerCollider.height = referenceCollider.height * 1.2f;
        triggerCollider.material = referenceCollider.material;
        triggerCollider.isTrigger = true;
        triggerCollider.name = "PlayerDetectorRC";
        MyTitanTrigger = obj.AddComponent<TitanTrigger>();
        MyTitanTrigger.IsCollide = false;
        obj.layer = 16;
        obj.transform.parent = AABB;
        obj.transform.localPosition = new Vector3(0f, 0f, 0f);
        ColliderEnabled = true;
        IsHooked = false;
        IsLook = false;
    }

    private void Chase()
    {
        state = TitanState.Chase;
        isAlarm = true;
        CrossFade(runAnimation, 0.5f);
    }

    private GameObject CheckIfHitCrawlerMouth(Transform head, float rad)
    {
        var num = rad * myLevel;
        var array = GameObject.FindGameObjectsWithTag("Player");
        return (from gameObject in array
            where !gameObject.GetComponent<TITAN_EREN>()
            where !gameObject.GetComponent<HERO>() || !gameObject.GetComponent<HERO>().IsInvincible()
            let num2 = gameObject.GetComponent<CapsuleCollider>().height * 0.5f
            where Vector3.Distance(gameObject.transform.position + Vectors.up * num2,
                head.position - Vectors.up * 1.5f * myLevel) < num + num2
            select gameObject).FirstOrDefault();
    }

    private HERO CheckHitHeadAndCrawlerMouth(float rad)
    {
        var num = rad * myLevel;
        return (from curr in FengGameManagerMKII.Heroes
            where curr.eren != null || !curr.IsInvincible()
            let idk = curr.GetComponent<CapsuleCollider>().height * 0.5f
            where (curr.baseT.position + Vectors.up * idk - (Head.position - Vectors.up * 1.5f * myLevel)).magnitude <
                  num + idk
            select curr).FirstOrDefault();
    }

    private HERO CheckIfHitHand(Transform hand)
    {
        var num = 2.4f * myLevel;
        HERO hero = null;
        var ienum = Physics.OverlapSphere(hand.position, num + 1f).GetEnumerator();
        while (ienum.MoveNext())
            if ((hero = ((Collider) ienum.Current)?.transform.root.gameObject.GetComponent<HERO>()) != null)
            {
                if (hero.eren != null)
                    if (!hero.eren.GetComponent<TITAN_EREN>().isHit)
                        hero.eren.GetComponent<TITAN_EREN>().hitByTitan();
                if (!hero.IsInvincible()) return hero;
            }

        return hero;
    }

    private void CrossFade(string aniName, float time)
    {
        baseA.CrossFade(aniName, time);
        if (IN_GAME_MAIN_CAMERA.GameType == GameType.MultiPlayer && BasePV.IsMine)
            BasePV.RPC("netCrossFade", PhotonTargets.Others, aniName, time);
    }

    private void DieAnimation()
    {
        if (baseA.IsPlaying("sit_idle") || baseA.IsPlaying("sit_hit_eye"))
            CrossFade("sit_die", 0.1f);
        else if (abnormalType == AbnormalType.Crawler)
            CrossFade("crawler_die", 0.2f);
        else if (abnormalType == AbnormalType.Normal)
            CrossFade("die_front", 0.05f);
        else if (baseA.IsPlaying("attack_abnormal_jump") && baseA["attack_abnormal_jump"].normalizedTime > 0.7f ||
                 baseA.IsPlaying("attack_abnormal_getup") && baseA["attack_abnormal_getup"].normalizedTime < 0.7f ||
                 baseA.IsPlaying("tired"))
            CrossFade("die_ground", 0.2f);
        else
            CrossFade("die_back", 0.05f);
    }

    private void Eat()
    {
        state = TitanState.Eat;
        attacked = false;
        if (isGrabHandLeft)
        {
            attackAnimation = "eat_l";
            CrossFade("eat_l", 0.1f);
        }
        else
        {
            attackAnimation = "eat_r";
            CrossFade("eat_r", 0.1f);
        }
    }

    private void EatSet(HERO hero)
    {
        if (!hero.IsGrabbed || !BasePV.IsMine)
        {
            if (isGrabHandLeft)
                grabToLeft();
            else
                grabToRight();
            if (IN_GAME_MAIN_CAMERA.GameType == GameType.MultiPlayer && BasePV.IsMine)
            {
                BasePV.RPC(isGrabHandLeft ? "grabToLeft" : "grabToRight", PhotonTargets.Others);
                hero.BasePV.RPC("netPlayAnimation", PhotonTargets.All, "grabbed");
                hero.BasePV.RPC("netGrabbed", PhotonTargets.All, BasePV.viewID, isGrabHandLeft);
                return;
            }

            hero.Grabbed(baseG, isGrabHandLeft);
            hero.baseA.Play("grabbed");
        }
    }

    private bool ExecuteAttack(string decidedAction)
    {
        switch (decidedAction)
        {
            case "grab_ground_front_l":
                Grab("ground_front_l");
                return true;

            case "grab_ground_front_r":
                Grab("ground_front_r");
                return true;

            case "grab_ground_back_l":
                Grab("ground_back_l");
                return true;

            case "grab_ground_back_r":
                Grab("ground_back_r");
                return true;

            case "grab_head_front_l":
                Grab("head_front_l");
                return true;

            case "grab_head_front_r":
                Grab("head_front_r");
                return true;

            case "grab_head_back_l":
                Grab("head_back_l");
                return true;

            case "grab_head_back_r":
                Grab("head_back_r");
                return true;

            case "attack_abnormal_jump":
                Attack("abnormal_jump");
                return true;

            case "attack_combo":
                Attack("combo_1");
                return true;

            case "attack_front_ground":
                Attack("front_ground");
                return true;

            case "attack_kick":
                Attack("kick");
                return true;

            case "attack_slap_back":
                Attack("slap_back");
                return true;

            case "attack_slap_face":
                Attack("slap_face");
                return true;

            case "attack_stomp":
                Attack("stomp");
                return true;

            case "attack_bite":
                Attack("bite");
                return true;

            case "attack_bite_l":
                Attack("bite_l");
                return true;

            case "attack_bite_r":
                Attack("bite_r");
                return true;
        }

        return false;
    }

    private void Explode()
    {
        if (!GameModes.ExplodeMode.Enabled || GameModes.ExplodeMode.GetInt(0) <= 0 || !hasDie || dieTime < 1f ||
            hasExplode) return;
        var d = myLevel * 10f;
        if (abnormalType == AbnormalType.Crawler)
        {
            if (dieTime < 2f)
                return;
            d = 0f;
        }
        else
        {
            hasExplode = true;
        }

        var vector = baseT.position + Vectors.up * d;
        Pool.NetworkEnable("FX/Thunder", vector, Quaternion.Euler(270f, 0f, 0f));
        Pool.NetworkEnable("FX/boom1", vector, Quaternion.Euler(270f, 0f, 0f));
        var rad = GameModes.ExplodeMode.GetInt(0);
        foreach (var hero in FengGameManagerMKII.Heroes.Where(hero =>
            Vector3.Distance(hero.baseT.position, vector) < rad))
        {
            hero.MarkDie();
            hero.BasePV.RPC("netDie2", PhotonTargets.All, -1, "Server");
        }
    }

    private void FindNearestFacingHero()
    {
        var array = GameObject.FindGameObjectsWithTag("Player");
        GameObject x = null;
        var num = float.PositiveInfinity;
        var position = baseT.position;
        var num2 = abnormalType != AbnormalType.Normal ? 180f : 100f;
        foreach (var go in array)
        {
            var sqrMagnitude = (go.transform.position - position).sqrMagnitude;
            if (sqrMagnitude < num)
            {
                var vector = go.transform.position - baseT.position;
                var current = -Mathf.Atan2(vector.z, vector.x) * 57.29578f;
                var f = -Mathf.DeltaAngle(current, baseGT.rotation.eulerAngles.y - 90f);
                if (Mathf.Abs(f) < num2)
                {
                    x = go;
                    num = sqrMagnitude;
                }
            }
        }

        if (x != null)
        {
            var x2 = myHero;
            myHero = x;
            if (x2 != myHero && IN_GAME_MAIN_CAMERA.GameType == GameType.MultiPlayer && PhotonNetwork.IsMasterClient)
                BasePV.RPC("setMyTarget", PhotonTargets.Others, myHero == null ? -1 : myHero.GetPhotonView().viewID);
            tauntTime = 5f;
        }
    }

    private void FindNearestHero()
    {
        var y = myHero;
        myHero = GetNearestHero();
        if (myHero != y && IN_GAME_MAIN_CAMERA.GameType == GameType.MultiPlayer && PhotonNetwork.IsMasterClient)
            BasePV.RPC("setMyTarget", PhotonTargets.Others, myHero == null ? -1 : myHero.GetPhotonView().viewID);
        oldHeadRotation = Head.rotation;
    }

    private void FixedUpdate()
    {
        if (IN_GAME_MAIN_CAMERA.isPausing && IN_GAME_MAIN_CAMERA.GameType == GameType.Single) return;
        if (IN_GAME_MAIN_CAMERA.GameType != GameType.Single)
            if (!BasePV.IsMine)
                return;
        baseR.AddForce(new Vector3(0f, -Gravity * baseR.mass, 0f));
        if (needFreshCorePosition)
        {
            oldCorePosition = baseT.position - Core.position;
            needFreshCorePosition = false;
        }

        if (hasDie)
        {
            if (hitPause <= 0f)
                if (baseA.IsPlaying("die_headOff"))
                {
                    var a = baseT.position - Core.position - oldCorePosition;
                    baseR.velocity = a / Time.deltaTime + Vectors.up * baseR.velocity.y;
                }

            oldCorePosition = baseT.position - Core.position;
        }
        else if (state == TitanState.Attack && isAttackMoveByCore || state == TitanState.Hit)
        {
            var position = baseT.position;
            var position1 = Core.position;
            var a2 = position - position1 - oldCorePosition;
            baseR.velocity = a2 / Time.deltaTime + Vectors.up * baseR.velocity.y;
            oldCorePosition = position - position1;
        }

        if (hasDie)
        {
            if (hitPause > 0f)
            {
                hitPause -= Time.deltaTime;
                if (hitPause <= 0f)
                {
                    baseA[hitAnimation].speed = 1f;
                    hitPause = 0f;
                }
            }
            else if (baseA.IsPlaying("die_blow"))
            {
                if (baseA["die_blow"].normalizedTime < 0.55f)
                    baseR.velocity = -baseT.Forward() * 300f + Vectors.up * baseR.velocity.y;
                else if (baseA["die_blow"].normalizedTime < 0.83f)
                    baseR.velocity = -baseT.Forward() * 100f + Vectors.up * baseR.velocity.y;
                else
                    baseR.velocity = Vectors.up * baseR.velocity.y;
            }

            return;
        }

        if (nonAI && !IN_GAME_MAIN_CAMERA.isPausing &&
            (state == TitanState.Idle || state == TitanState.Attack && attackAnimation == "jumper_1"))
        {
            var a3 = Vectors.zero;
            if (controller.targetDirection != -874f)
            {
                var flag = false;
                if (stamina < 5f)
                    flag = true;
                else if (stamina < 40f)
                    if (!baseA.IsPlaying("run_abnormal") && !baseA.IsPlaying("crawler_run"))
                        flag = true;
                if (controller.isWALKDown || flag)
                    a3 = baseT.Forward() * speed * Mathf.Sqrt(myLevel) * 0.2f;
                else
                    a3 = baseT.Forward() * speed * Mathf.Sqrt(myLevel);
                baseGT.rotation = Quaternion.Lerp(baseGT.rotation, Quaternion.Euler(0f, controller.targetDirection, 0f),
                    speed * 0.15f * Time.deltaTime);
                if (state == TitanState.Idle)
                {
                    if (controller.isWALKDown || flag)
                    {
                        if (abnormalType == AbnormalType.Crawler)
                        {
                            if (!baseA.IsPlaying("crawler_run")) CrossFade("crawler_run", 0.1f);
                        }
                        else if (!baseA.IsPlaying("run_walk"))
                        {
                            CrossFade("run_walk", 0.1f);
                        }
                    }
                    else if (abnormalType == AbnormalType.Crawler)
                    {
                        if (!baseA.IsPlaying("crawler_run")) CrossFade("crawler_run", 0.1f);
                        var hero = CheckHitHeadAndCrawlerMouth(2.2f);
                        if (hero != null)
                        {
                            var position = Chest.position;
                            if (IN_GAME_MAIN_CAMERA.GameType == GameType.Single)
                            {
                                hero.Die((hero.baseT.position - position) * 15f * myLevel, false);
                            }
                            else if (IN_GAME_MAIN_CAMERA.GameType == GameType.MultiPlayer && BasePV.IsMine &&
                                     !hero.HasDied())
                            {
                                hero.MarkDie();
                                hero.BasePV.RPC("netDie", PhotonTargets.All,
                                    (hero.baseT.position - position) * 15f * myLevel, true, !nonAI ? -1 : BasePV.viewID,
                                    name, true);
                            }
                        }
                    }
                    else if (!baseA.IsPlaying("run_abnormal"))
                    {
                        CrossFade("run_abnormal", 0.1f);
                    }
                }
            }
            else if (state == TitanState.Idle)
            {
                if (abnormalType == AbnormalType.Crawler)
                {
                    if (!baseA.IsPlaying("crawler_idle")) CrossFade("crawler_idle", 0.1f);
                }
                else if (!baseA.IsPlaying("idle"))
                {
                    CrossFade("idle", 0.1f);
                }

                a3 = Vectors.zero;
            }

            switch (state)
            {
                case TitanState.Idle:
                {
                    var velocity = baseR.velocity;
                    var force = a3 - velocity;
                    force.x = Mathf.Clamp(force.x, -maxVelocityChange, maxVelocityChange);
                    force.z = Mathf.Clamp(force.z, -maxVelocityChange, maxVelocityChange);
                    force.y = 0f;
                    baseR.AddForce(force, ForceMode.VelocityChange);
                    break;
                }
                case TitanState.Attack when attackAnimation == "jumper_0":
                {
                    var velocity2 = baseR.velocity;
                    var force2 = a3 * 0.8f - velocity2;
                    force2.x = Mathf.Clamp(force2.x, -maxVelocityChange, maxVelocityChange);
                    force2.z = Mathf.Clamp(force2.z, -maxVelocityChange, maxVelocityChange);
                    force2.y = 0f;
                    baseR.AddForce(force2, ForceMode.VelocityChange);
                    break;
                }
            }
        }

        if ((abnormalType == AbnormalType.Aberrant || abnormalType == AbnormalType.Jumper) && !nonAI &&
            state == TitanState.Attack && attackAnimation == "jumper_0")
        {
            var a4 = baseT.Forward() * speed * myLevel * 0.5f;
            var velocity3 = baseR.velocity;
            if (baseA["attack_jumper_0"].normalizedTime <= 0.28f || baseA["attack_jumper_0"].normalizedTime >= 0.8f)
                a4 = Vectors.zero;
            var force3 = a4 - velocity3;
            force3.x = Mathf.Clamp(force3.x, -maxVelocityChange, maxVelocityChange);
            force3.z = Mathf.Clamp(force3.z, -maxVelocityChange, maxVelocityChange);
            force3.y = 0f;
            baseR.AddForce(force3, ForceMode.VelocityChange);
        }

        if (state == TitanState.Chase || state == TitanState.Wander || state == TitanState.To_CheckPoint ||
            state == TitanState.To_PVP_PT || state == TitanState.Random_Run)
        {
            var a5 = baseT.Forward() * speed;
            var velocity4 = baseR.velocity;
            var force4 = a5 - velocity4;
            force4.x = Mathf.Clamp(force4.x, -maxVelocityChange, maxVelocityChange);
            force4.z = Mathf.Clamp(force4.z, -maxVelocityChange, maxVelocityChange);
            force4.y = 0f;
            baseR.AddForce(force4, ForceMode.VelocityChange);
            if (!stuck && abnormalType != AbnormalType.Crawler && !nonAI)
            {
                if (baseA.IsPlaying(runAnimation) && baseR.velocity.magnitude < speed * 0.5f)
                {
                    stuck = true;
                    stuckTime = 2f;
                    stuckTurnAngle = Random.Range(0, 2) * 140f - 70f;
                }

                if (state == TitanState.Chase && myHero != null && myDistance > attackDistance && myDistance < 150f)
                {
                    var num = 0.05f;
                    if (myDifficulty > 1) num += 0.05f;
                    if (abnormalType != AbnormalType.Normal) num += 0.1f;
                    if (Random.Range(0f, 1f) < num)
                    {
                        stuck = true;
                        stuckTime = 1f;
                        var num2 = Random.Range(20f, 50f);
                        stuckTurnAngle = Random.Range(0, 2) * num2 * 2f - num2;
                    }
                }
            }

            float num3;
            if (state == TitanState.Wander)
            {
                num3 = baseT.rotation.eulerAngles.y - 90f;
            }
            else if (state == TitanState.To_CheckPoint || state == TitanState.To_PVP_PT ||
                     state == TitanState.Random_Run)
            {
                var vector = targetCheckPt - baseT.position;
                num3 = -Mathf.Atan2(vector.z, vector.x) * 57.29578f;
            }
            else
            {
                if (myHero == null) return;
                var vector2 = myHero.transform.position - baseT.position;
                num3 = -Mathf.Atan2(vector2.z, vector2.x) * 57.29578f;
            }

            if (stuck)
            {
                stuckTime -= Time.deltaTime;
                if (stuckTime < 0f) stuck = false;
                if (stuckTurnAngle > 0f)
                    stuckTurnAngle -= Time.deltaTime * 10f;
                else
                    stuckTurnAngle += Time.deltaTime * 10f;
                num3 += stuckTurnAngle;
            }

            var num4 = -Mathf.DeltaAngle(num3, baseGT.rotation.eulerAngles.y - 90f);
            if (abnormalType == AbnormalType.Crawler)
                baseGT.rotation = Quaternion.Lerp(baseGT.rotation,
                    Quaternion.Euler(0f, baseGT.rotation.eulerAngles.y + num4, 0f),
                    speed * 0.3f * Time.deltaTime / myLevel);
            else
                baseGT.rotation = Quaternion.Lerp(baseGT.rotation,
                    Quaternion.Euler(0f, baseGT.rotation.eulerAngles.y + num4, 0f),
                    speed * 0.5f * Time.deltaTime / myLevel);
        }
    }

    private string[] GetAttackStrategy()
    {
        string[] array = null;
        if (isAlarm || myHero.transform.position.y + 3f <= Neck.position.y + 10f * myLevel)
        {
            if (myHero.transform.position.y > Neck.position.y - 3f * myLevel)
            {
                if (myDistance < attackDistance * 0.5f)
                {
                    if (Vector3.Distance(myHero.transform.position, chkOverHead.position) < 3.6f * myLevel)
                    {
                        if (between2 > 0f)
                            array = new[]
                            {
                                "grab_head_front_r"
                            };
                        else
                            array = new[]
                            {
                                "grab_head_front_l"
                            };
                    }
                    else if (Mathf.Abs(between2) < 90f)
                    {
                        if (Mathf.Abs(between2) < 30f)
                        {
                            if (Vector3.Distance(myHero.transform.position, chkFront.position) < 2.5f * myLevel)
                                array = new[]
                                {
                                    "attack_bite",
                                    "attack_bite",
                                    "attack_slap_face"
                                };
                        }
                        else if (between2 > 0f)
                        {
                            if (Vector3.Distance(myHero.transform.position, chkFrontRight.position) < 2.5f * myLevel)
                                array = new[]
                                {
                                    "attack_bite_r"
                                };
                        }
                        else if (Vector3.Distance(myHero.transform.position, chkFrontLeft.position) < 2.5f * myLevel)
                        {
                            array = new[]
                            {
                                "attack_bite_l"
                            };
                        }
                    }
                    else if (between2 > 0f)
                    {
                        if (Vector3.Distance(myHero.transform.position, chkBackRight.position) < 2.8f * myLevel)
                            array = new[]
                            {
                                "grab_head_back_r",
                                "grab_head_back_r",
                                "attack_slap_back"
                            };
                    }
                    else if (Vector3.Distance(myHero.transform.position, chkBackLeft.position) < 2.8f * myLevel)
                    {
                        array = new[]
                        {
                            "grab_head_back_l",
                            "grab_head_back_l",
                            "attack_slap_back"
                        };
                    }
                }

                if (array == null)
                {
                    if (abnormalType == AbnormalType.Normal || abnormalType == AbnormalType.Punk)
                    {
                        if ((myDifficulty > 0 || Random.Range(0, 1000) < 3) && Mathf.Abs(between2) < 60f)
                            array = new[]
                            {
                                "attack_combo"
                            };
                    }
                    else if ((abnormalType == AbnormalType.Aberrant || abnormalType == AbnormalType.Jumper) &&
                             (myDifficulty > 0 || Random.Range(0, 100) < 50))
                    {
                        array = new[]
                        {
                            "attack_abnormal_jump"
                        };
                    }
                }
            }
            else
            {
                int num;
                if (Mathf.Abs(between2) < 90f)
                {
                    if (between2 > 0f)
                        num = 1;
                    else
                        num = 2;
                }
                else if (between2 > 0f)
                {
                    num = 4;
                }
                else
                {
                    num = 3;
                }

                switch (num)
                {
                    case 1:
                        if (myDistance < attackDistance * 0.25f)
                            switch (abnormalType)
                            {
                                case AbnormalType.Punk:
                                    array = new[]
                                    {
                                        "attack_kick",
                                        "attack_stomp"
                                    };
                                    break;
                                case AbnormalType.Normal:
                                    array = new[]
                                    {
                                        "attack_front_ground",
                                        "attack_stomp"
                                    };
                                    break;
                                default:
                                    array = new[]
                                    {
                                        "attack_kick"
                                    };
                                    break;
                            }
                        else if (myDistance < attackDistance * 0.5f)
                            switch (abnormalType)
                            {
                                case AbnormalType.Punk:
                                    array = new[]
                                    {
                                        "grab_ground_front_r",
                                        "grab_ground_front_r",
                                        "attack_abnormal_jump"
                                    };
                                    break;
                                case AbnormalType.Normal:
                                    array = new[]
                                    {
                                        "grab_ground_front_r",
                                        "grab_ground_front_r",
                                        "attack_stomp"
                                    };
                                    break;
                                default:
                                    array = new[]
                                    {
                                        "grab_ground_front_r",
                                        "grab_ground_front_r",
                                        "attack_abnormal_jump"
                                    };
                                    break;
                            }
                        else
                            switch (abnormalType)
                            {
                                case AbnormalType.Punk:
                                    array = new[]
                                    {
                                        "attack_combo",
                                        "attack_combo",
                                        "attack_abnormal_jump"
                                    };
                                    break;
                                case AbnormalType.Normal when myDifficulty > 0:
                                    array = new[]
                                    {
                                        "attack_front_ground",
                                        "attack_combo",
                                        "attack_combo"
                                    };
                                    break;
                                case AbnormalType.Normal:
                                    array = new[]
                                    {
                                        "attack_front_ground",
                                        "attack_front_ground",
                                        "attack_front_ground",
                                        "attack_front_ground",
                                        "attack_combo"
                                    };
                                    break;
                                default:
                                    array = new[]
                                    {
                                        "attack_abnormal_jump"
                                    };
                                    break;
                            }

                        break;

                    case 2:
                        if (myDistance < attackDistance * 0.25f)
                        {
                            if (abnormalType == AbnormalType.Punk)
                                array = new[]
                                {
                                    "attack_kick",
                                    "attack_stomp"
                                };
                            else if (abnormalType == AbnormalType.Normal)
                                array = new[]
                                {
                                    "attack_front_ground",
                                    "attack_stomp"
                                };
                            else
                                array = new[]
                                {
                                    "attack_kick"
                                };
                        }
                        else if (myDistance < attackDistance * 0.5f)
                        {
                            switch (abnormalType)
                            {
                                case AbnormalType.Punk:
                                    array = new[]
                                    {
                                        "grab_ground_front_l",
                                        "grab_ground_front_l",
                                        "attack_abnormal_jump"
                                    };
                                    break;
                                case AbnormalType.Normal:
                                    array = new[]
                                    {
                                        "grab_ground_front_l",
                                        "grab_ground_front_l",
                                        "attack_stomp"
                                    };
                                    break;
                                default:
                                    array = new[]
                                    {
                                        "grab_ground_front_l",
                                        "grab_ground_front_l",
                                        "attack_abnormal_jump"
                                    };
                                    break;
                            }
                        }
                        else
                        {
                            switch (abnormalType)
                            {
                                case AbnormalType.Punk:
                                    array = new[]
                                    {
                                        "attack_combo",
                                        "attack_combo",
                                        "attack_abnormal_jump"
                                    };
                                    break;
                                case AbnormalType.Normal when myDifficulty > 0:
                                    array = new[]
                                    {
                                        "attack_front_ground",
                                        "attack_combo",
                                        "attack_combo"
                                    };
                                    break;
                                case AbnormalType.Normal:
                                    array = new[]
                                    {
                                        "attack_front_ground",
                                        "attack_front_ground",
                                        "attack_front_ground",
                                        "attack_front_ground",
                                        "attack_combo"
                                    };
                                    break;
                                default:
                                    array = new[]
                                    {
                                        "attack_abnormal_jump"
                                    };
                                    break;
                            }
                        }

                        break;

                    case 3:
                        if (myDistance < attackDistance * 0.5f)
                        {
                            if (abnormalType == AbnormalType.Normal)
                                array = new[]
                                {
                                    "grab_ground_back_l"
                                };
                            else
                                array = new[]
                                {
                                    "grab_ground_back_l"
                                };
                        }

                        break;

                    case 4:
                        if (myDistance < attackDistance * 0.5f)
                        {
                            if (abnormalType == AbnormalType.Normal)
                                array = new[]
                                {
                                    "grab_ground_back_r"
                                };
                            else
                                array = new[]
                                {
                                    "grab_ground_back_r"
                                };
                        }

                        break;
                }
            }
        }

        return array;
    }

    private void GetDown()
    {
        state = TitanState.Down;
        isAlarm = true;
        PlayAnimation("sit_hunt_down");
        getdownTime = Random.Range(3f, 5f);
    }

    private GameObject GetNearestHero()
    {
        var array = GameObject.FindGameObjectsWithTag("Player");
        GameObject result = null;
        var num = float.PositiveInfinity;
        var position = baseT.position;
        foreach (var go in array)
        {
            var sqrMagnitude = (go.transform.position - position).sqrMagnitude;
            if (sqrMagnitude < num)
            {
                result = go;
                num = sqrMagnitude;
            }
        }

        return result;
    }

    private int GetPunkNumber()
    {
        var array = GameObject.FindGameObjectsWithTag("titan");
        return array.Count(go => go.GetComponent<TITAN>() && go.GetComponent<TITAN>().name == "Punk");
    }

    private void Grab(string type)
    {
        state = TitanState.Grad;
        attacked = false;
        isAlarm = true;
        attackAnimation = type;
        CrossFade("grab_" + type, 0.1f);
        isGrabHandLeft = true;
        grabbedTarget = null;
        switch (type)
        {
            case "ground_back_l":
                attackCheckTimeA = 0.34f;
                attackCheckTimeB = 0.49f;
                break;

            case "ground_back_r":
                attackCheckTimeA = 0.34f;
                attackCheckTimeB = 0.49f;
                isGrabHandLeft = false;
                break;

            case "ground_front_l":
                attackCheckTimeA = 0.37f;
                attackCheckTimeB = 0.6f;
                break;

            case "ground_front_r":
                attackCheckTimeA = 0.37f;
                attackCheckTimeB = 0.6f;
                isGrabHandLeft = false;
                break;

            case "head_back_l":
                attackCheckTimeA = 0.45f;
                attackCheckTimeB = 0.5f;
                isGrabHandLeft = false;
                break;

            case "head_back_r":
                attackCheckTimeA = 0.45f;
                attackCheckTimeB = 0.5f;
                break;

            case "head_front_l":
                attackCheckTimeA = 0.38f;
                attackCheckTimeB = 0.55f;
                break;

            case "head_front_r":
                attackCheckTimeA = 0.38f;
                attackCheckTimeB = 0.55f;
                isGrabHandLeft = false;
                break;
        }

        if (isGrabHandLeft)
            currentGrabHand = Hand_L_001;
        else
            currentGrabHand = Hand_R_001;
    }

    private void Hit(string animationName, Vector3 attacker, float hitPauseTime)
    {
        state = TitanState.Hit;
        hitAnimation = animationName;
        hitPause = hitPauseTime;
        PlayAnimation(hitAnimation);
        baseA[hitAnimation].time = 0f;
        baseA[hitAnimation].speed = 0f;
        baseT.rotation = Quaternion.Euler(0f, Quaternion.LookRotation(attacker - baseT.position).eulerAngles.y, 0f);
        needFreshCorePosition = true;
        if (BasePV.IsMine && grabbedTarget != null) grabbedTarget.BasePV.RPC("netUngrabbed", PhotonTargets.All);
    }

    private void Idle(float sbTime = 0f)
    {
        stuck = false;
        sbtime = sbTime;
        if (myDifficulty == 2 && (abnormalType == AbnormalType.Jumper || abnormalType == AbnormalType.Aberrant))
            sbtime = Random.Range(0f, 1.5f);
        else if (myDifficulty >= 1) sbtime = 0f;
        sbtime = Mathf.Max(0.5f, sbtime);
        if (abnormalType == AbnormalType.Punk)
        {
            sbtime = 0.1f;
            if (myDifficulty == 1) sbtime += 0.4f;
        }

        state = TitanState.Idle;
        if (abnormalType == AbnormalType.Crawler)
            CrossFade("crawler_idle", 0.2f);
        else
            CrossFade("idle", 0.2f);
    }

    private void JustEatHero(HERO target, Transform hand)
    {
        if (target != null)
            switch (IN_GAME_MAIN_CAMERA.GameType)
            {
                case GameType.MultiPlayer when BasePV.IsMine:
                {
                    if (!target.HasDied())
                    {
                        target.MarkDie();
                        target.BasePV.RPC("netDie2", PhotonTargets.All, nonAI ? BasePV.viewID : -1, ShowName);
                    }

                    break;
                }
                case GameType.Single:
                    target.Die2(hand);
                    break;
            }
    }

    private void JustHitEye()
    {
        if (state != TitanState.Hit_Eye)
        {
            if (state == TitanState.Down || state == TitanState.Sit)
                PlayAnimation("sit_hit_eye");
            else
                PlayAnimation("hit_eye");
            state = TitanState.Hit_Eye;
        }
    }

    private IEnumerator loadskinRPCE(string body, string eye)
    {
        while (!spawned) yield return null;
        Skin = new TitanSkin(this, body, eye);
        Anarchy.Skins.Skin.Check(Skin, new[] {body, eye});
    }

    private bool LongRangeAttackCheck()
    {
        if (abnormalType != AbnormalType.Punk) return false;
        if (myHero != null && myHero.rigidbody != null)
        {
            var vector = myHero.rigidbody.velocity * Time.deltaTime * 30f;
            if (vector.sqrMagnitude > 10f)
            {
                if (SimpleHitTestLineAndBall(vector, chkAeLeft.position - myHero.transform.position, 5f * myLevel))
                {
                    Attack("anti_AE_l");
                    return true;
                }

                if (SimpleHitTestLineAndBall(vector, chkAeLLeft.position - myHero.transform.position, 5f * myLevel))
                {
                    Attack("anti_AE_low_l");
                    return true;
                }

                if (SimpleHitTestLineAndBall(vector, chkAeRight.position - myHero.transform.position, 5f * myLevel))
                {
                    Attack("anti_AE_r");
                    return true;
                }

                if (SimpleHitTestLineAndBall(vector, chkAeLRight.position - myHero.transform.position, 5f * myLevel))
                {
                    Attack("anti_AE_low_r");
                    return true;
                }
            }

            var vector2 = myHero.transform.position - baseT.position;
            var current = -Mathf.Atan2(vector2.z, vector2.x) * 57.29578f;
            var f = -Mathf.DeltaAngle(current, baseGT.rotation.eulerAngles.y - 90f);
            if (rockInterval > 0f)
            {
                rockInterval -= Time.deltaTime;
            }
            else if (Mathf.Abs(f) < 5f)
            {
                var a = myHero.transform.position + vector;
                var sqrMagnitude = (a - baseT.position).sqrMagnitude;
                if (sqrMagnitude > 8000f && sqrMagnitude < 90000f && !GameModes.NoRocks.Enabled)
                {
                    Attack("throw");
                    rockInterval = 2f;
                    return true;
                }
            }
        }

        return false;
    }

    private void OnCollisionStay()
    {
        grounded = true;
    }

    private void OnDestroy()
    {
        if (FengGameManagerMKII.FGM != null) FengGameManagerMKII.FGM.RemoveTitan(this);
    }

    private void PlayAnimation(string aniName)
    {
        baseA.Play(aniName);
        if (IN_GAME_MAIN_CAMERA.GameType == GameType.MultiPlayer && BasePV.IsMine)
            BasePV.RPC("netPlayAnimation", PhotonTargets.Others, aniName);
    }

    private void PlayAnimationAt(string aniName, float normalizedTime)
    {
        baseA.Play(aniName);
        baseA[aniName].normalizedTime = normalizedTime;
        if (IN_GAME_MAIN_CAMERA.GameType == GameType.MultiPlayer && BasePV.IsMine)
            BasePV.RPC("netPlayAnimationAt", PhotonTargets.Others, aniName, normalizedTime);
    }

    private void PlaySound(string sndname)
    {
        playsoundRPC(sndname);
        if (BasePV.IsMine) BasePV.RPC("playsoundRPC", PhotonTargets.Others, sndname);
    }

    private void Recover()
    {
        state = TitanState.Recover;
        PlayAnimation("idle_recovery");
        getdownTime = Random.Range(2f, 5f);
    }

    private void RemainSitdown()
    {
        state = TitanState.Sit;
        PlayAnimation("sit_idle");
        getdownTime = Random.Range(10f, 30f);
    }

    private void SetLevel(float level, int AI, int skinColor)
    {
        myLevel = level;
        myLevel = Mathf.Clamp(myLevel, 0.1f, 50f);
        attackWait += Random.Range(0f, 2f);
        chaseDistance += myLevel * 10f;
        baseT.localScale = new Vector3(myLevel, myLevel, myLevel);
        var num = Mathf.Pow(2f / myLevel, 0.35f);
        num = Mathf.Min(num, 1.25f);
        headscale = new Vector3(num, num, num);
        Head.localScale = headscale;
        if (skinColor != 0)
            mainMaterial.GetComponent<SkinnedMeshRenderer>().material.color = skinColor != 1
                ? skinColor != 2 ? FengColor.titanSkin3 : FengColor.titanSkin2
                : FengColor.titanSkin1;
        var num2 = 1.4f - (myLevel - 0.7f) * 0.15f;
        num2 = Mathf.Clamp(num2, 0.9f, 1.5f);
        foreach (var obj in baseA)
        {
            var animationState = (AnimationState) obj;
            animationState.speed = num2;
        }

        baseR.mass *= myLevel;
        baseR.rotation = Quaternion.Euler(0f, Random.Range(0, 360), 0f);
        if (myLevel > 1f) speed *= Mathf.Sqrt(myLevel);
        myDifficulty = AI;
        if (myDifficulty == 1 || myDifficulty == 2)
        {
            foreach (var obj2 in baseA)
            {
                var animationState2 = (AnimationState) obj2;
                animationState2.speed = num2 * 1.05f;
            }

            if (nonAI)
                speed *= 1.1f;
            else
                speed *= 1.4f;
            chaseDistance *= 1.15f;
        }

        if (myDifficulty == 2)
        {
            foreach (var obj3 in baseA)
            {
                var animationState3 = (AnimationState) obj3;
                animationState3.speed = num2 * 1.05f;
            }

            if (nonAI)
                speed *= 1.1f;
            else
                speed *= 1.5f;
            chaseDistance *= 1.3f;
        }

        if (IN_GAME_MAIN_CAMERA.GameMode == GameMode.ENDLESS_TITAN ||
            IN_GAME_MAIN_CAMERA.GameMode == GameMode.SURVIVE_MODE) chaseDistance = 999999f;
        if (nonAI)
        {
            if (abnormalType == AbnormalType.Crawler)
                speed = Mathf.Min(70f, speed);
            else
                speed = Mathf.Min(60f, speed);
        }

        attackDistance = Vector3.Distance(baseT.position, ap_front_ground.position) * 1.65f;
    }

    private void SetMyLevel()
    {
        baseA.cullingType = AnimationCullingType.BasedOnRenderers;
        if (IN_GAME_MAIN_CAMERA.GameType != GameType.Single && BasePV.IsMine)
        {
            BasePV.RPC("netSetLevel", PhotonTargets.AllBuffered, myLevel, FengGameManagerMKII.FGM.difficulty,
                Random.Range(0, 4));
            baseA.cullingType = AnimationCullingType.AlwaysAnimate;
        }
        else if (IN_GAME_MAIN_CAMERA.GameType == GameType.Single)
        {
            SetLevel(myLevel, IN_GAME_MAIN_CAMERA.Difficulty, Random.Range(0, 4));
        }
    }

    private static bool SimpleHitTestLineAndBall(Vector3 line, Vector3 ball, float R)
    {
        var vector = Vector3.Project(ball, line);
        return (ball - vector).magnitude <= R && Vector3.Dot(line, vector) >= 0f &&
               vector.sqrMagnitude <= line.sqrMagnitude;
    }

    private void SitDown()
    {
        state = TitanState.Sit;
        PlayAnimation("sit_down");
        getdownTime = Random.Range(10f, 30f);
    }

    private void Start()
    {
        FengGameManagerMKII.FGM.AddTitan(this);
        Minimap.TrackGameObjectOnMinimap(gameObject, Color.yellow, false, true);
        runAnimation = "run_walk";
        grabTF = new GameObject {name = "titansTmpGrabTF"};
        oldHeadRotation = Head.rotation;
        if (IN_GAME_MAIN_CAMERA.GameType == GameType.MultiPlayer && !BasePV.IsMine) return;
        if (!hasSetLevel)
        {
            myLevel = Random.Range(0.7f, 3f);
            if (GameModes.SizeMode.Enabled)
                myLevel = Random.Range(GameModes.SizeMode.GetFloat(0), GameModes.SizeMode.GetFloat(1));
            hasSetLevel = true;
        }

        spawnPt = baseT.position;
        SetMyLevel();
        spawned = true;
        SetAbnormalType(abnormalType);
        if (myHero == null) FindNearestHero();
        if (maxHealth == 0 && GameModes.HealthMode.Enabled)
        {
            var healthLower = GameModes.HealthMode.GetInt(0);
            var healthUpper = GameModes.HealthMode.GetInt(1) + 1;
            switch (GameModes.HealthMode.Selection)
            {
                case 1:
                    maxHealth = currentHealth = Random.Range(healthLower, healthUpper);
                    break;
                case 2:
                    maxHealth = currentHealth =
                        Mathf.Clamp(Mathf.RoundToInt(myLevel / 4f * Random.Range(healthLower, healthUpper)),
                            healthLower,
                            healthUpper);
                    Log.AddLineRaw(maxHealth.ToString());
                    break;
            }
        }

        lagMax = 150f + myLevel * 3f;
        healthTime = Time.time;
        if (currentHealth > 0 && IN_GAME_MAIN_CAMERA.GameType == GameType.MultiPlayer)
            BasePV.RPC("labelRPC", PhotonTargets.AllBuffered, currentHealth, maxHealth);
        hasExplode = false;
        ColliderEnabled = true;
        IsHooked = false;
        IsLook = false;
    }

    private void Turn(float d)
    {
        if (abnormalType == AbnormalType.Crawler)
            turnAnimation = d > 0f ? "crawler_turnaround_R" : "crawler_turnaround_L";
        else if (d > 0f)
            turnAnimation = "turnaround2";
        else
            turnAnimation = "turnaround1";

        PlayAnimation(turnAnimation);
        baseA[turnAnimation].time = 0f;
        d = Mathf.Clamp(d, -120f, 120f);
        turnDeg = d;
        desDeg = baseGT.rotation.eulerAngles.y + turnDeg;
        state = TitanState.Turn;
    }

    private void Wander()
    {
        state = TitanState.Wander;
        CrossFade(runAnimation, 0.5f);
    }

    private void UpdateLabel()
    {
        if (healthLabel != null)
            healthLabel.transform.LookAt(2f * healthLabel.transform.position - IN_GAME_MAIN_CAMERA.BaseT.position);
    }

    public void BeLaughAttacked()
    {
        if (hasDie) return;
        if (abnormalType == AbnormalType.Crawler) return;
        if (IN_GAME_MAIN_CAMERA.GameType == GameType.MultiPlayer)
            BasePV.RPC("laugh", PhotonTargets.All, 0f);
        else if (state == TitanState.Idle || state == TitanState.Turn || state == TitanState.Chase) laugh();
    }

    public void BeTauntedBy(GameObject target, float time)
    {
        whoHasTauntMe = target;
        tauntTime = time;
        isAlarm = true;
        if (whoHasTauntMe != null && IN_GAME_MAIN_CAMERA.GameType == GameType.MultiPlayer &&
            PhotonNetwork.IsMasterClient)
        {
            myHero = whoHasTauntMe;
            BasePV.RPC("setMyTarget", PhotonTargets.Others, myHero.GetPhotonView().viewID);
        }
    }

    public bool Die()
    {
        if (hasDie) return false;
        hasDie = true;
        FengGameManagerMKII.FGM.oneTitanDown(string.Empty);
        DieAnimation();
        return true;
    }

    public void DieBlow(Vector3 attacker, float hitPauseTime)
    {
        if (IN_GAME_MAIN_CAMERA.GameType == GameType.Single)
        {
            DieBlowFunc(attacker, hitPauseTime);
            if (GameObject.FindGameObjectsWithTag("titan").Length <= 1) IN_GAME_MAIN_CAMERA.MainCamera.gameOver = true;
        }
        else
        {
            BasePV.RPC("dieBlowRPC", PhotonTargets.All, attacker, hitPauseTime);
        }
    }

    public void DieBlowFunc(Vector3 attacker, float hitPauseTime)
    {
        if (hasDie) return;
        baseT.rotation = Quaternion.Euler(0f, Quaternion.LookRotation(attacker - baseT.position).eulerAngles.y, 0f);
        hasDie = true;
        hitAnimation = "die_blow";
        hitPause = hitPauseTime;
        PlayAnimation(hitAnimation);
        baseA[hitAnimation].time = 0f;
        baseA[hitAnimation].speed = 0f;
        needFreshCorePosition = true;
        FengGameManagerMKII.FGM.oneTitanDown(string.Empty);
        if (BasePV.IsMine)
        {
            if (grabbedTarget != null) grabbedTarget.BasePV.RPC("netUngrabbed", PhotonTargets.All);
            if (nonAI)
            {
                IN_GAME_MAIN_CAMERA.MainCamera.SetMainObject(null);
                IN_GAME_MAIN_CAMERA.MainCamera.setSpectorMode(true);
                IN_GAME_MAIN_CAMERA.MainCamera.gameOver = true;
                PhotonNetwork.player.Dead = true;
                PhotonNetwork.player.Deaths++;
            }
        }
    }

    public void DieHeadBlow(Vector3 attacker, float hitPauseTime)
    {
        if (abnormalType == AbnormalType.Crawler) return;
        if (IN_GAME_MAIN_CAMERA.GameType == GameType.Single)
        {
            DieHeadBlowFunc(attacker, hitPauseTime);
            if (GameObject.FindGameObjectsWithTag("titan").Length <= 1) IN_GAME_MAIN_CAMERA.MainCamera.gameOver = true;
        }
        else
        {
            BasePV.RPC("dieHeadBlowRPC", PhotonTargets.All, attacker, hitPauseTime);
        }
    }

    public void DieHeadBlowFunc(Vector3 attacker, float hitPauseTime)
    {
        if (hasDie) return;
        PlaySound("snd_titan_head_blow");
        baseT.rotation = Quaternion.Euler(0f, Quaternion.LookRotation(attacker - baseT.position).eulerAngles.y, 0f);
        hasDie = true;
        hitAnimation = "die_headOff";
        hitPause = hitPauseTime;
        PlayAnimation(hitAnimation);
        baseA[hitAnimation].time = 0f;
        baseA[hitAnimation].speed = 0f;
        FengGameManagerMKII.FGM.oneTitanDown(string.Empty);
        needFreshCorePosition = true;
        GameObject go;
        if (IN_GAME_MAIN_CAMERA.GameType == GameType.MultiPlayer && BasePV.IsMine)
            go = Pool.NetworkEnable("bloodExplore", Head.position + Vectors.up * 1f * myLevel,
                Quaternion.Euler(270f, 0f, 0f));
        else
            go = Pool.Enable("bloodExplore", Head.position + Vectors.up * 1f * myLevel,
                Quaternion.Euler(270f, 0f,
                    0f)); //(GameObject)UnityEngine.Object.Instantiate(CacheResources.Load("bloodExplore"), this.Head.position + Vectors.up * 1f * this.myLevel, Quaternion.Euler(270f, 0f, 0f));
        go.transform.localScale = baseT.localScale;
        if (IN_GAME_MAIN_CAMERA.GameType == GameType.MultiPlayer && BasePV.IsMine)
        {
            var rotation = Neck.rotation;
            go = Pool.NetworkEnable("bloodsplatter", Head.position,
                Quaternion.Euler(270f + rotation.eulerAngles.x, rotation.eulerAngles.y,
                    rotation.eulerAngles.z));
        }
        else
        {
            var rotation = Neck.rotation;
            go = Pool.Enable("bloodsplatter", Head.position,
                Quaternion.Euler(270f + rotation.eulerAngles.x, rotation.eulerAngles.y,
                    rotation.eulerAngles
                        .z)); //(GameObject)UnityEngine.Object.Instantiate(CacheResources.Load("bloodsplatter"), this.Head.position, Quaternion.Euler(270f + this.Neck.rotation.eulerAngles.x, this.Neck.rotation.eulerAngles.y, this.Neck.rotation.eulerAngles.z));
        }

        go.transform.localScale = baseT.localScale;
        go.transform.parent = Neck;
        if (IN_GAME_MAIN_CAMERA.GameType == GameType.MultiPlayer && BasePV.IsMine)
            go = Pool.NetworkEnable("FX/justSmoke", Neck.position, Quaternion.Euler(270f, 0f, 0f));
        else
            go =
                Pool.Enable("FX/justSmoke", Neck.position,
                    Quaternion.Euler(270f, 0f,
                        0f)); //(GameObject)UnityEngine.Object.Instantiate(CacheResources.Load("FX/justSmoke"), this.Neck.position, Quaternion.Euler(270f, 0f, 0f));
        go.transform.parent = Neck;
        if (BasePV.IsMine)
        {
            if (grabbedTarget != null) grabbedTarget.BasePV.RPC("netUngrabbed", PhotonTargets.All);
            if (nonAI)
            {
                IN_GAME_MAIN_CAMERA.MainCamera.SetMainObject(null);
                IN_GAME_MAIN_CAMERA.MainCamera.setSpectorMode(true);
                IN_GAME_MAIN_CAMERA.MainCamera.gameOver = true;
                PhotonNetwork.player.Dead = true;
                PhotonNetwork.player.Deaths++;
            }
        }
    }

    public void HeadMovement()
    {
        if (!hasDie)
        {
            if (IN_GAME_MAIN_CAMERA.GameType != GameType.Single)
            {
                var isMine = BasePV.IsMine;
                if (isMine)
                {
                    targetHeadRotation = Head.rotation;
                    var flag2 = false;
                    var flag6 = abnormalType != AbnormalType.Crawler && state != TitanState.Attack &&
                                state != TitanState.Down && state != TitanState.Hit && state != TitanState.Recover &&
                                state != TitanState.Eat && state != TitanState.Hit_Eye && !hasDie &&
                                myDistance < 100f && myHero != null;
                    if (flag6)
                    {
                        var position = myHero.transform.position;
                        var vector = position - baseT.position;
                        angle = -Mathf.Atan2(vector.z, vector.x) * 57.29578f;
                        var num = -Mathf.DeltaAngle(angle, baseT.rotation.eulerAngles.y - 90f);
                        num = Mathf.Clamp(num, -40f, 40f);
                        var y = Neck.position.y + myLevel * 2f - position.y;
                        var num2 = Mathf.Atan2(y, myDistance) * 57.29578f;
                        num2 = Mathf.Clamp(num2, -40f, 30f);
                        var rotation = Head.rotation;
                        targetHeadRotation = Quaternion.Euler(rotation.eulerAngles.x + num2,
                            rotation.eulerAngles.y + num, rotation.eulerAngles.z);
                        var flag7 = !asClientLookTarget;
                        if (flag7)
                        {
                            asClientLookTarget = true;
                            object[] parameters =
                            {
                                true
                            };
                            BasePV.RPC("setIfLookTarget", PhotonTargets.Others, parameters);
                        }

                        flag2 = true;
                    }

                    var flag8 = !flag2 && asClientLookTarget;
                    if (flag8)
                    {
                        asClientLookTarget = false;
                        object[] objArray3 =
                        {
                            false
                        };
                        BasePV.RPC("setIfLookTarget", PhotonTargets.Others, objArray3);
                    }

                    var flag9 = state == TitanState.Attack || state == TitanState.Hit || state == TitanState.Hit_Eye;
                    oldHeadRotation = flag9
                        ? Quaternion.Lerp(oldHeadRotation, targetHeadRotation, Time.deltaTime * 20f)
                        : Quaternion.Lerp(oldHeadRotation, targetHeadRotation, Time.deltaTime * 10f);
                }
                else
                {
                    var flag3 = myHero != null;
                    var flag10 = flag3;
                    if (flag10)
                    {
                        var position = myHero.transform.position;
                        var position1 = baseT.position;
                        myDistance =
                            Mathf.Sqrt(
                                (position.x - position1.x) *
                                (position.x - position1.x) +
                                (position.z - position1.z) *
                                (position.z - position1.z));
                    }
                    else
                    {
                        myDistance = float.MaxValue;
                    }

                    targetHeadRotation = Head.rotation;
                    var flag11 = asClientLookTarget && flag3 && myDistance < 100f;
                    if (flag11)
                    {
                        var position = myHero.transform.position;
                        var vector2 = position - baseT.position;
                        angle = -Mathf.Atan2(vector2.z, vector2.x) * 57.29578f;
                        var num3 = -Mathf.DeltaAngle(angle, baseT.rotation.eulerAngles.y - 90f);
                        num3 = Mathf.Clamp(num3, -40f, 40f);
                        var num4 = Neck.position.y + myLevel * 2f - position.y;
                        var num5 = Mathf.Atan2(num4, myDistance) * 57.29578f;
                        num5 = Mathf.Clamp(num5, -40f, 30f);
                        var rotation = Head.rotation;
                        targetHeadRotation = Quaternion.Euler(rotation.eulerAngles.x + num5,
                            rotation.eulerAngles.y + num3, rotation.eulerAngles.z);
                    }

                    var flag12 = !hasDie;
                    if (flag12)
                        oldHeadRotation = Quaternion.Slerp(oldHeadRotation, targetHeadRotation, Time.deltaTime * 10f);
                }
            }
            else
            {
                targetHeadRotation = Head.rotation;
                var flag13 = abnormalType != AbnormalType.Crawler && state != TitanState.Attack &&
                             state != TitanState.Down && state != TitanState.Hit && state != TitanState.Recover &&
                             state != TitanState.Hit_Eye && !hasDie && myDistance < 100f && myHero != null;
                if (flag13)
                {
                    var position = myHero.transform.position;
                    var vector3 = position - baseT.position;
                    angle = -Mathf.Atan2(vector3.z, vector3.x) * 57.29578f;
                    var num6 = -Mathf.DeltaAngle(angle, baseT.rotation.eulerAngles.y - 90f);
                    num6 = Mathf.Clamp(num6, -40f, 40f);
                    var num7 = Neck.position.y + myLevel * 2f - position.y;
                    var num8 = Mathf.Atan2(num7, myDistance) * 57.29578f;
                    num8 = Mathf.Clamp(num8, -40f, 30f);
                    var rotation = Head.rotation;
                    targetHeadRotation = Quaternion.Euler(rotation.eulerAngles.x + num8,
                        rotation.eulerAngles.y + num6, rotation.eulerAngles.z);
                }

                var flag14 = state == TitanState.Attack || state == TitanState.Hit || state == TitanState.Hit_Eye;
                if (flag14)
                    oldHeadRotation = Quaternion.Lerp(oldHeadRotation, targetHeadRotation, Time.deltaTime * 20f);
                else
                    oldHeadRotation = Quaternion.Lerp(oldHeadRotation, targetHeadRotation, Time.deltaTime * 10f);
            }

            Head.rotation = oldHeadRotation;
        }

        if (!baseA.IsPlaying("die_headOff")) Head.localScale = headscale;

        #region Vanilla version

        //if (!this.hasDie)
        //{
        //    if (IN_GAME_MAIN_CAMERA.GameType != GameType.Single)
        //    {
        //        if (BasePV.IsMine)
        //        {
        //            this.targetHeadRotation = this.Head.rotation;
        //            bool flag = false;
        //            if (this.abnormalType != AbnormalType.Crawler && this.state != TitanState.attack && this.state != TitanState.down && this.state != TitanState.hit && this.state != TitanState.recover && this.state != TitanState.eat && this.state != TitanState.hit_eye && !this.hasDie && this.myDistance < 100f && this.myHero != null)
        //            {
        //                Vector3 vector = this.myHero.transform.position - baseT.position;
        //                this.angle = -Mathf.Atan2(vector.z, vector.x) * 57.29578f;
        //                float num = -Mathf.DeltaAngle(this.angle, baseT.rotation.eulerAngles.y - 90f);
        //                num = Mathf.Clamp(num, -40f, 40f);
        //                float y = this.Neck.position.y + this.myLevel * 2f - this.myHero.transform.position.y;
        //                float num2 = Mathf.Atan2(y, this.myDistance) * 57.29578f;
        //                num2 = Mathf.Clamp(num2, -40f, 30f);
        //                this.targetHeadRotation = Quaternion.Euler(this.Head.rotation.eulerAngles.x + num2, this.Head.rotation.eulerAngles.y + num, this.Head.rotation.eulerAngles.z);
        //                if (!this.asClientLookTarget)
        //                {
        //                    this.asClientLookTarget = true;
        //                    BasePV.RPC("setIfLookTarget", PhotonTargets.Others, new object[]
        //                    {
        //                        true
        //                    });
        //                }
        //                flag = true;
        //            }
        //            if (!flag && this.asClientLookTarget)
        //            {
        //                this.asClientLookTarget = false;
        //                BasePV.RPC("setIfLookTarget", PhotonTargets.Others, new object[]
        //                {
        //                    false
        //                });
        //            }
        //            if (this.state == TitanState.attack || this.state == TitanState.hit || this.state == TitanState.hit_eye)
        //            {
        //                this.oldHeadRotation = Quaternion.Lerp(this.oldHeadRotation, this.targetHeadRotation, Time.deltaTime * 20f);
        //            }
        //            else
        //            {
        //                this.oldHeadRotation = Quaternion.Lerp(this.oldHeadRotation, this.targetHeadRotation, Time.deltaTime * 10f);
        //            }
        //        }
        //        else
        //        {
        //            this.targetHeadRotation = this.Head.rotation;
        //            if (this.asClientLookTarget && this.myHero != null)
        //            {
        //                Vector3 vector2 = this.myHero.transform.position - baseT.position;
        //                this.angle = -Mathf.Atan2(vector2.z, vector2.x) * 57.29578f;
        //                float num3 = -Mathf.DeltaAngle(this.angle, baseT.rotation.eulerAngles.y - 90f);
        //                num3 = Mathf.Clamp(num3, -40f, 40f);
        //                float y2 = this.Neck.position.y + this.myLevel * 2f - this.myHero.transform.position.y;
        //                float num4 = Mathf.Atan2(y2, this.myDistance) * 57.29578f;
        //                num4 = Mathf.Clamp(num4, -40f, 30f);
        //                this.targetHeadRotation = Quaternion.Euler(this.Head.rotation.eulerAngles.x + num4, this.Head.rotation.eulerAngles.y + num3, this.Head.rotation.eulerAngles.z);
        //            }
        //            if (!this.hasDie)
        //            {
        //                this.oldHeadRotation = Quaternion.Lerp(this.oldHeadRotation, this.targetHeadRotation, Time.deltaTime * 10f);
        //            }
        //        }
        //    }
        //    else
        //    {
        //        this.targetHeadRotation = this.Head.rotation;
        //        if (this.abnormalType != AbnormalType.Crawler && this.state != TitanState.attack && this.state != TitanState.down && this.state != TitanState.hit && this.state != TitanState.recover && this.state != TitanState.hit_eye && !this.hasDie && this.myDistance < 100f && this.myHero != null)
        //        {
        //            Vector3 vector3 = this.myHero.transform.position - baseT.position;
        //            this.angle = -Mathf.Atan2(vector3.z, vector3.x) * 57.29578f;
        //            float num5 = -Mathf.DeltaAngle(this.angle, baseT.rotation.eulerAngles.y - 90f);
        //            num5 = Mathf.Clamp(num5, -40f, 40f);
        //            float y3 = this.Neck.position.y + this.myLevel * 2f - this.myHero.transform.position.y;
        //            float num6 = Mathf.Atan2(y3, this.myDistance) * 57.29578f;
        //            num6 = Mathf.Clamp(num6, -40f, 30f);
        //            this.targetHeadRotation = Quaternion.Euler(this.Head.rotation.eulerAngles.x + num6, this.Head.rotation.eulerAngles.y + num5, this.Head.rotation.eulerAngles.z);
        //        }
        //        if (this.state == TitanState.attack || this.state == TitanState.hit || this.state == TitanState.hit_eye)
        //        {
        //            this.oldHeadRotation = Quaternion.Lerp(this.oldHeadRotation, this.targetHeadRotation, Time.deltaTime * 20f);
        //        }
        //        else
        //        {
        //            this.oldHeadRotation = Quaternion.Lerp(this.oldHeadRotation, this.targetHeadRotation, Time.deltaTime * 10f);
        //        }
        //    }
        //    this.Head.rotation = this.oldHeadRotation;
        //}
        //if (!baseA.IsPlaying("die_headOff"))
        //{
        //    this.Head.localScale = this.headscale;
        //}

        #endregion
    }

    public void HitAnkle()
    {
        if (hasDie) return;
        if (state == TitanState.Down) return;
        if (grabbedTarget != null) grabbedTarget.BasePV.RPC("netUngrabbed", PhotonTargets.All);
        GetDown();
    }

    public void HitEye()
    {
        if (hasDie) return;
        JustHitEye();
    }

    public void HitL(Vector3 attacker, float hitPauseTime)
    {
        if (abnormalType == AbnormalType.Crawler) return;
        if (IN_GAME_MAIN_CAMERA.GameType == GameType.Single)
            Hit("hit_eren_L", attacker, hitPauseTime);
        else
            BasePV.RPC("hitLRPC", PhotonTargets.All, attacker, hitPauseTime);
    }

    public void HitR(Vector3 attacker, float hitPauseTime)
    {
        if (abnormalType == AbnormalType.Crawler) return;
        if (IN_GAME_MAIN_CAMERA.GameType == GameType.Single)
            Hit("hit_eren_R", attacker, hitPauseTime);
        else
            BasePV.RPC("hitRRPC", PhotonTargets.All, attacker, hitPauseTime);
    }

    public bool IsGrounded()
    {
        return Physics.Raycast(baseGT.position + Vectors.up * 0.1f, -Vectors.up, 0.3f, Layers.EnemyAABBGround.value);
    }

    public void lateUpdate()
    {
        if (IN_GAME_MAIN_CAMERA.isPausing && IN_GAME_MAIN_CAMERA.GameType == GameType.Single) return;
        if (baseA.IsPlaying("run_walk"))
        {
            if (baseA["run_walk"].normalizedTime % 1f > 0.1f && baseA["run_walk"].normalizedTime % 1f < 0.6f &&
                stepSoundPhase == 2)
            {
                stepSoundPhase = 1;
                FootAudio.Stop();
                FootAudio.Play();
            }

            if (baseA["run_walk"].normalizedTime % 1f > 0.6f && stepSoundPhase == 1)
            {
                stepSoundPhase = 2;
                FootAudio.Stop();
                FootAudio.Play();
            }
        }

        if (baseA.IsPlaying("crawler_run"))
        {
            if (baseA["crawler_run"].normalizedTime % 1f > 0.1f && baseA["crawler_run"].normalizedTime % 1f < 0.56f &&
                stepSoundPhase == 2)
            {
                stepSoundPhase = 1;
                FootAudio.Stop();
                FootAudio.Play();
            }

            if (baseA["crawler_run"].normalizedTime % 1f > 0.56f && stepSoundPhase == 1)
            {
                stepSoundPhase = 2;
                FootAudio.Stop();
                FootAudio.Play();
            }
        }

        if (baseA.IsPlaying("run_abnormal"))
        {
            if (baseA["run_abnormal"].normalizedTime % 1f > 0.47f &&
                baseA["run_abnormal"].normalizedTime % 1f < 0.95f && stepSoundPhase == 2)
            {
                stepSoundPhase = 1;
                FootAudio.Stop();
                FootAudio.Play();
            }

            if ((baseA["run_abnormal"].normalizedTime % 1f > 0.95f ||
                 baseA["run_abnormal"].normalizedTime % 1f < 0.47f) && stepSoundPhase == 1)
            {
                stepSoundPhase = 2;
                FootAudio.Stop();
                FootAudio.Play();
            }
        }

        UpdateCollider();
        UpdateLabel();
        HeadMovement();
        grounded = false;
    }

    public void MoveTo(float x, float y, float z)
    {
        baseT.position = new Vector3(x, y, z);
    }

    public void OnTitanDie(PhotonView view)
    {
        if (CustomLevel.logicLoaded && RCManager.RCEvents.ContainsKey("OnTitanDie"))
        {
            var event2 = (RCEvent) RCManager.RCEvents["OnTitanDie"];
            var strArray = (string[]) RCManager.RCVariableNames["OnTitanDie"];
            if (RCManager.titanVariables.ContainsKey(strArray[0]))
                RCManager.titanVariables[strArray[0]] = this;
            else
                RCManager.titanVariables.Add(strArray[0], this);
            if (RCManager.playerVariables.ContainsKey(strArray[1]))
                RCManager.playerVariables[strArray[1]] = view.owner;
            else
                RCManager.playerVariables.Add(strArray[1], view.owner);
            event2.checkEvent();
        }
    }

    public void RandomRun(Vector3 targetPt, float r)
    {
        state = TitanState.Random_Run;
        targetCheckPt = targetPt;
        targetR = r;
        random_run_time = Random.Range(1f, 2f);
        CrossFade(runAnimation, 0.5f);
    }

    public void ResetLevel(float level)
    {
        myLevel = level;
        SetMyLevel();
    }

    public void SetAbnormalType(AbnormalType type, bool forceCrawler = false)
    {
        var num = 0;
        var num2 = 0.02f * (IN_GAME_MAIN_CAMERA.Difficulty + 1);
        if (IN_GAME_MAIN_CAMERA.GameMode == GameMode.PVP_AHSS) num2 = 100f;
        switch (type)
        {
            case AbnormalType.Normal when Random.Range(0f, 1f) < num2:
                num = 4;
                break;
            case AbnormalType.Normal:
                num = 0;
                break;
            case AbnormalType.Aberrant when Random.Range(0f, 1f) < num2:
                num = 4;
                break;
            case AbnormalType.Aberrant:
                num = 1;
                break;
            case AbnormalType.Jumper when Random.Range(0f, 1f) < num2:
                num = 4;
                break;
            case AbnormalType.Jumper:
                num = 2;
                break;
            case AbnormalType.Crawler:
            {
                num = 3;
                if (Random.Range(0, 1000) > 5) num = 2;
                break;
            }
            case AbnormalType.Punk:
                num = 4;
                break;
        }

        if (forceCrawler) num = 3;
        if (num == 4)
        {
            if (!FengGameManagerMKII.Level.PunksEnabled)
            {
                num = 1;
            }
            else
            {
                if (IN_GAME_MAIN_CAMERA.GameType == GameType.Single && GetPunkNumber() >= 3) num = 1;
                if (IN_GAME_MAIN_CAMERA.GameMode == GameMode.SURVIVE_MODE)
                {
                    var wave = FengGameManagerMKII.FGM.logic.Round.Wave;
                    if (wave % 5 != 0) num = 1;
                }
            }
        }

        if (GameModes.SpawnRate.Enabled && IN_GAME_MAIN_CAMERA.GameType == GameType.MultiPlayer &&
            PhotonNetwork.IsMasterClient) num = (int) type;
        if (SkinSettings.SkinsCheck(SkinSettings.TitanSkins))
            if (SkinSettings.TitanSet.Value != StringSetting.NotDefine)
            {
                var set = new TitanSkinPreset(SkinSettings.TitanSet.Value);
                set.Load();
                var rnd = Random.Range(0, 5);
                var body = set.Bodies[rnd];
                rnd = set.RandomizePairs ? Random.Range(0, 5) : rnd;
                var eyes = set.Eyes[rnd];
                var eye = eyes.EndsWith(".png") || eyes.EndsWith(".jpeg") || eyes.EndsWith(".jpg") ||
                          eyes.ToLower().Equals("transparent");
                GetComponent<TITAN_SETUP>().SetVar(rnd, eye);
                StartCoroutine(loadskinRPCE(body, eyes));
                if (IN_GAME_MAIN_CAMERA.GameType == GameType.MultiPlayer && PhotonNetwork.IsMasterClient &&
                    SkinSettings.TitanSkins.Value != 2)
                    BasePV.RPC("loadskinRPC", PhotonTargets.OthersBuffered, body, eyes);
            }

        if (IN_GAME_MAIN_CAMERA.GameType != GameType.Single && BasePV.IsMine)
            BasePV.RPC("netSetAbnormalType", PhotonTargets.AllBuffered, num);
        else if (IN_GAME_MAIN_CAMERA.GameType == GameType.Single) netSetAbnormalType(num);
    }

    public void SetRoute(GameObject route)
    {
        checkPoints = new ArrayList();
        for (var i = 1; i <= 10; i++) checkPoints.Add(route.transform.Find("r" + i).position);
        checkPoints.Add("end");
    }

    public void Suicide()
    {
        netDie();
        if (nonAI)
        {
            FengGameManagerMKII.FGM.SendKillInfo(false, string.Empty, true, User.DeathName);
            AnarchyManager.Feed.Kill(string.Empty, User.DeathName, 0);
        }

        FengGameManagerMKII.FGM.needChooseSide = true;
        FengGameManagerMKII.FGM.justSuicide = true;
    }

    public void ToCheckPoint(Vector3 targetPt, float r)
    {
        state = TitanState.To_CheckPoint;
        targetCheckPt = targetPt;
        targetR = r;
        CrossFade(runAnimation, 0.5f);
    }

    public void ToPvpCheckPoint(Vector3 targetPt, float r)
    {
        state = TitanState.To_PVP_PT;
        targetCheckPt = targetPt;
        targetR = r;
        CrossFade(runAnimation, 0.5f);
    }

    public void update()
    {
        if (IN_GAME_MAIN_CAMERA.isPausing && IN_GAME_MAIN_CAMERA.GameType == GameType.Single) return;
        if (myDifficulty < 0) return;
        if (IN_GAME_MAIN_CAMERA.GameType != GameType.Single)
            if (!BasePV.IsMine)
                return;
        var dt = Time.deltaTime;
        Explode();
        if (!nonAI)
        {
            if (activeRad < 2147483647 &&
                (state == TitanState.Idle || state == TitanState.Wander || state == TitanState.Chase))
            {
                if (checkPoints.Count > 1)
                {
                    if (Vector3.Distance((Vector3) checkPoints[0], baseT.position) > activeRad)
                        ToCheckPoint((Vector3) checkPoints[0], 10f);
                }
                else if (Vector3.Distance(spawnPt, baseT.position) > activeRad)
                {
                    ToCheckPoint(spawnPt, 10f);
                }
            }

            if (whoHasTauntMe != null)
            {
                tauntTime -= dt;
                if (tauntTime <= 0f)
                {
                    whoHasTauntMe = null;
                    BasePV.RPC("setMyTarget", PhotonTargets.Others, -1);
                }
            }
        }

        if (!hasDie)
        {
            if (state == TitanState.Hit)
            {
                if (hitPause > 0f)
                {
                    hitPause -= dt;
                    if (hitPause <= 0f)
                    {
                        baseA[hitAnimation].speed = 1f;
                        hitPause = 0f;
                    }
                }

                if (baseA[hitAnimation].normalizedTime >= 1f) Idle();
            }

            if (!nonAI)
            {
                if (myHero == null) FindNearestHero();
                if ((state == TitanState.Idle || state == TitanState.Chase || state == TitanState.Wander) &&
                    whoHasTauntMe == null && Random.Range(0, 100) < 10) FindNearestFacingHero();
                if (myHero == null)
                {
                    myDistance = float.MaxValue;
                }
                else
                {
                    var position = myHero.transform.position;
                    var position1 = baseT.position;
                    myDistance =
                        Mathf.Sqrt(
                            (position.x - position1.x) *
                            (position.x - position1.x) +
                            (position.z - position1.z) *
                            (position.z - position1.z));
                }
            }
            else
            {
                if (stamina < MaxStamina)
                {
                    if (baseA.IsPlaying("idle")) stamina += dt * 30f;
                    if (baseA.IsPlaying("crawler_idle")) stamina += dt * 35f;
                    if (baseA.IsPlaying("run_walk")) stamina += dt * 10f;
                }

                if (baseA.IsPlaying("run_abnormal_1")) stamina -= dt * 5f;
                if (baseA.IsPlaying("crawler_run")) stamina -= dt * 15f;
                if (stamina < 0f) stamina = 0f;
                if (!IN_GAME_MAIN_CAMERA.isPausing)
                    CacheGameObject.Find("stamina_titan").transform.localScale = new Vector3(stamina, 16f);
            }

            switch (state)
            {
                case TitanState.Laugh:
                    if (baseA["laugh"].normalizedTime >= 1f)
                    {
                        Idle(2f);
                        return;
                    }

                    break;

                case TitanState.Idle:
                    if (nonAI)
                    {
                        if (IN_GAME_MAIN_CAMERA.isPausing) return;
                        Pt();
                        if (abnormalType != AbnormalType.Crawler)
                        {
                            if (controller.isAttackDown && stamina > 25f)
                            {
                                stamina -= 25f;
                                Attack("combo_1");
                            }
                            else if (controller.isAttackIIDown && stamina > 50f)
                            {
                                stamina -= 50f;
                                Attack("abnormal_jump");
                            }
                            else if (controller.isJumpDown && stamina > 15f)
                            {
                                stamina -= 15f;
                                Attack("jumper_0");
                            }
                        }
                        else if (controller.isAttackDown && stamina > 40f)
                        {
                            stamina -= 40f;
                            Attack("crawler_jump_0");
                        }

                        if (controller.isSuicide) Suicide();
                        return;
                    }
                    else
                    {
                        if (sbtime > 0f)
                        {
                            sbtime -= dt;
                            return;
                        }

                        if (!isAlarm)
                        {
                            if (abnormalType != AbnormalType.Punk && abnormalType != AbnormalType.Crawler &&
                                Random.Range(0f, 1f) < 0.005f)
                            {
                                SitDown();
                                return;
                            }

                            if (Random.Range(0f, 1f) < 0.02f)
                            {
                                Wander();
                                return;
                            }

                            if (Random.Range(0f, 1f) < 0.01f)
                            {
                                Turn(Random.Range(30, 120));
                                return;
                            }

                            if (Random.Range(0f, 1f) < 0.01f)
                            {
                                Turn(Random.Range(-30, -120));
                                return;
                            }
                        }

                        angle = 0f;
                        between2 = 0f;
                        if (myDistance < chaseDistance || whoHasTauntMe != null)
                        {
                            var vector = myHero.transform.position - baseT.position;
                            angle = -Mathf.Atan2(vector.z, vector.x) * 57.29578f;
                            between2 = -Mathf.DeltaAngle(angle, baseGT.rotation.eulerAngles.y - 90f);
                            if (myDistance >= attackDistance)
                            {
                                if (isAlarm || Mathf.Abs(between2) < 90f)
                                {
                                    Chase();
                                    return;
                                }

                                if (!isAlarm && myDistance < chaseDistance * 0.1f)
                                {
                                    Chase();
                                    return;
                                }
                            }
                        }

                        if (LongRangeAttackCheck()) return;
                        if (myDistance < chaseDistance)
                        {
                            var position = myHero.transform.position;
                            switch (abnormalType)
                            {
                                case AbnormalType.Jumper when (myDistance > attackDistance ||
                                                               position.y > Head.position.y + 4f * myLevel) &&
                                                              Mathf.Abs(between2) < 120f &&
                                                              Vector3.Distance(baseT.position, position) <
                                                              1.5f * position.y:
                                    Attack("jumper_0");
                                    return;
                                case AbnormalType.Crawler when myDistance < attackDistance * 3f &&
                                                               Mathf.Abs(between2) < 90f &&
                                                               myHero.transform.position.y <
                                                               Neck.position.y + 30f * myLevel &&
                                                               myHero.transform.position.y >
                                                               Neck.position.y + 10f * myLevel:
                                    Attack("crawler_jump_0");
                                    return;
                            }
                        }

                        if (abnormalType == AbnormalType.Punk && myDistance < 90f && Mathf.Abs(between2) > 90f)
                        {
                            if (Random.Range(0f, 1f) < 0.4f)
                                RandomRun(
                                    baseT.position + new Vector3(Random.Range(-50f, 50f), Random.Range(-50f, 50f),
                                        Random.Range(-50f, 50f)), 10f);
                            if (Random.Range(0f, 1f) < 0.2f)
                                Recover();
                            else if (Random.Range(0, 2) == 0)
                                Attack("quick_turn_l");
                            else
                                Attack("quick_turn_r");
                            return;
                        }

                        if (myDistance < attackDistance)
                        {
                            if (abnormalType == AbnormalType.Crawler)
                            {
                                if (myHero.transform.position.y + 3f <= Neck.position.y + 20f * myLevel)
                                    if (Random.Range(0f, 1f) < 0.1f)
                                    {
                                        Chase();
                                        return;
                                    }

                                return;
                            }

                            var text = string.Empty;
                            var attackStrategy = GetAttackStrategy();
                            if (attackStrategy != null) text = attackStrategy[Random.Range(0, attackStrategy.Length)];
                            if ((abnormalType == AbnormalType.Jumper || abnormalType == AbnormalType.Aberrant) &&
                                Mathf.Abs(between2) > 40f)
                            {
                                if (text.Contains("grab") || text.Contains("kick") || text.Contains("slap") ||
                                    text.Contains("bite"))
                                {
                                    if (Random.Range(0, 100) < 30)
                                    {
                                        Turn(between2);
                                        return;
                                    }
                                }
                                else if (Random.Range(0, 100) < 90)
                                {
                                    Turn(between2);
                                    return;
                                }
                            }

                            if (ExecuteAttack(text)) return;
                            if (abnormalType == AbnormalType.Normal)
                            {
                                if (Random.Range(0, 100) < 30 && Mathf.Abs(between2) > 45f)
                                {
                                    Turn(between2);
                                    return;
                                }
                            }
                            else if (Mathf.Abs(between2) > 45f)
                            {
                                Turn(between2);
                                return;
                            }
                        }

                        if (PVPfromCheckPt != null)
                        {
                            if (PVPfromCheckPt.state == CheckPointState.Titan)
                            {
                                if (Random.Range(0, 100) > 48)
                                {
                                    var gameObject = PVPfromCheckPt.chkPtNext;
                                    if (gameObject != null &&
                                        (gameObject.GetComponent<PVPcheckPoint>().state != CheckPointState.Titan ||
                                         Random.Range(0, 100) < 20))
                                    {
                                        ToPvpCheckPoint(gameObject.transform.position, 5 + Random.Range(0, 10));
                                        PVPfromCheckPt = gameObject.GetComponent<PVPcheckPoint>();
                                    }
                                }
                                else
                                {
                                    var go = PVPfromCheckPt.chkPtPrevious;
                                    if (go != null &&
                                        (go.GetComponent<PVPcheckPoint>().state != CheckPointState.Titan ||
                                         Random.Range(0, 100) < 5))
                                    {
                                        ToPvpCheckPoint(go.transform.position, 5 + Random.Range(0, 10));
                                        PVPfromCheckPt = go.GetComponent<PVPcheckPoint>();
                                    }
                                }
                            }
                            else
                            {
                                ToPvpCheckPoint(PVPfromCheckPt.transform.position, 5 + Random.Range(0, 10));
                            }
                        }
                    }

                    break;

                case TitanState.Attack:
                    if (attackAnimation == "combo")
                    {
                        if (nonAI)
                        {
                            if (controller.isAttackDown) nonAIcombo = true;
                            if (!nonAIcombo && baseA["attack_" + attackAnimation].normalizedTime >= 0.385f)
                            {
                                Idle();
                                return;
                            }
                        }

                        if (baseA["attack_" + attackAnimation].normalizedTime >= 0.11f &&
                            baseA["attack_" + attackAnimation].normalizedTime <= 0.16f)
                        {
                            var go = CheckIfHitHand(Hand_R_001);
                            if (go != null)
                            {
                                var position = Chest.position;
                                if (IN_GAME_MAIN_CAMERA.GameType == GameType.Single)
                                {
                                    go.Die((go.baseT.position - position) * 15f * myLevel, false);
                                }
                                else if (IN_GAME_MAIN_CAMERA.GameType == GameType.MultiPlayer && BasePV.IsMine &&
                                         !go.HasDied())
                                {
                                    go.MarkDie();
                                    go.BasePV.RPC("netDie", PhotonTargets.All,
                                        (go.transform.position - position) * 15f * myLevel, false,
                                        !nonAI ? -1 : BasePV.viewID, ShowName, true);
                                }
                            }
                        }

                        if (baseA["attack_" + attackAnimation].normalizedTime >= 0.27f &&
                            baseA["attack_" + attackAnimation].normalizedTime <= 0.32f)
                        {
                            var go = CheckIfHitHand(Hand_L_001);
                            if (go != null)
                            {
                                var position2 = Chest.position;
                                switch (IN_GAME_MAIN_CAMERA.GameType)
                                {
                                    case GameType.Single:
                                        go.Die((go.baseT.position - position2) * 15f * myLevel, false);
                                        break;
                                    case GameType.MultiPlayer when BasePV.IsMine && !go.HasDied():
                                        go.MarkDie();
                                        go.BasePV.RPC("netDie", PhotonTargets.All,
                                            (go.transform.position - position2) * 15f * myLevel, false,
                                            !nonAI ? -1 : BasePV.viewID, ShowName, true);
                                        break;
                                }
                            }
                        }
                    }

                    if (attackCheckTimeA != 0f &&
                        baseA["attack_" + attackAnimation].normalizedTime >= attackCheckTimeA &&
                        baseA["attack_" + attackAnimation].normalizedTime <= attackCheckTimeB)
                    {
                        var go = CheckIfHitHand(leftHandAttack ? Hand_L_001 : Hand_R_001);
                        if (go != null)
                        {
                            var position3 = Chest.position;
                            switch (IN_GAME_MAIN_CAMERA.GameType)
                            {
                                case GameType.Single:
                                    go.Die((go.baseT.position - position3) * 15f * myLevel, false);
                                    break;
                                case GameType.MultiPlayer when BasePV.IsMine && !go.HasDied():
                                    go.MarkDie();
                                    go.BasePV.RPC("netDie", PhotonTargets.All,
                                        (go.baseT.position - position3) * 15f * myLevel, false,
                                        !nonAI ? -1 : BasePV.viewID,
                                        ShowName, true);
                                    break;
                            }
                        }
                    }

                    if (!attacked && attackCheckTime != 0f &&
                        baseA["attack_" + attackAnimation].normalizedTime >= attackCheckTime)
                    {
                        attacked = true;
                        fxPosition = baseT.Find("ap_" + attackAnimation).position;
                        GameObject gameObject6;
                        if (IN_GAME_MAIN_CAMERA.GameType == GameType.MultiPlayer && BasePV.IsMine)
                            gameObject6 = Pool.NetworkEnable("FX/" + fxName, fxPosition, fxRotation);
                        else
                            gameObject6 =
                                Pool.Enable("FX/" + fxName, fxPosition,
                                    fxRotation); //(GameObject)UnityEngine.Object.Instantiate(CacheResources.Load("FX/" + this.fxName), this.fxPosition, this.fxRotation);
                        if (nonAI)
                        {
                            gameObject6.transform.localScale = baseT.localScale * 1.5f;
                            if (gameObject6.GetComponent<EnemyfxIDcontainer>())
                                gameObject6.GetComponent<EnemyfxIDcontainer>().myOwnerViewID = BasePV.viewID;
                        }
                        else
                        {
                            gameObject6.transform.localScale = baseT.localScale;
                        }

                        if (gameObject6.GetComponent<EnemyfxIDcontainer>())
                            gameObject6.GetComponent<EnemyfxIDcontainer>().titanName = ShowName;
                        var num = 1f - Vector3.Distance(IN_GAME_MAIN_CAMERA.MainCamera.transform.position,
                            gameObject6.transform.position) * 0.05f;
                        num = Mathf.Min(1f, num);
                        IN_GAME_MAIN_CAMERA.MainCamera.startShake(num, num);
                    }

                    if (attackAnimation == "throw")
                    {
                        if (!attacked && baseA["attack_" + attackAnimation].normalizedTime >= 0.11f)
                        {
                            attacked = true;
                            if (IN_GAME_MAIN_CAMERA.GameType == GameType.MultiPlayer && BasePV.IsMine)
                                throwRock = Pool.NetworkEnable("FX/rockThrow", Hand_R_001.position,
                                    Hand_R_001.rotation);
                            else
                                throwRock = Pool.Enable("FX/rockThrow", Hand_R_001.position,
                                    Hand_R_001
                                        .rotation); //(GameObject)UnityEngine.Object.Instantiate(CacheResources.Load("FX/rockThrow"), Hand_R_001.position, Hand_R_001.rotation);
                            throwRock.transform.localScale = baseT.localScale;
                            throwRock.transform.position -= throwRock.transform.Forward() * 2.5f * myLevel;
                            if (throwRock.GetComponent<EnemyfxIDcontainer>())
                            {
                                if (nonAI) throwRock.GetComponent<EnemyfxIDcontainer>().myOwnerViewID = BasePV.viewID;
                                throwRock.GetComponent<EnemyfxIDcontainer>().titanName = ShowName;
                            }

                            throwRock.transform.parent = Hand_R_001;
                            if (IN_GAME_MAIN_CAMERA.GameType == GameType.MultiPlayer && BasePV.IsMine)
                                throwRock.GetPhotonView().RPC("initRPC", PhotonTargets.Others, BasePV.viewID,
                                    baseT.localScale, throwRock.transform.localPosition, myLevel);
                        }

                        if (baseA["attack_" + attackAnimation].normalizedTime >= 0.11f)
                        {
                            var y = Mathf.Atan2(myHero.transform.position.x - baseT.position.x,
                                myHero.transform.position.z - baseT.position.z) * 57.29578f;
                            baseGT.rotation = Quaternion.Euler(0f, y, 0f);
                        }

                        if (throwRock != null && baseA["attack_" + attackAnimation].normalizedTime >= 0.62f)
                        {
                            const float num2 = 1f;
                            const float num3 = -20f;
                            Vector3 v;
                            if (myHero != null)
                            {
                                var position = throwRock.transform.position;
                                var position1 = myHero.transform.position;
                                v = (position1 - position) / num2 +
                                    myHero.rigidbody.velocity;
                                var num4 = position1.y + 2f * myLevel;
                                var num5 = num4 - position.y;
                                v = new Vector3(v.x, num5 / num2 - 0.5f * num3 * num2, v.z);
                            }
                            else
                            {
                                v = baseT.Forward() * 60f + Vectors.up * 10f;
                            }

                            throwRock.GetComponent<RockThrow>().launch(v);
                            throwRock.transform.parent = null;
                            throwRock = null;
                        }
                    }

                    switch (attackAnimation)
                    {
                        case "jumper_0":
                        case "crawler_jump_0":
                        {
                            if (!attacked)
                            {
                                if (baseA["attack_" + attackAnimation].normalizedTime >= 0.68f)
                                {
                                    attacked = true;
                                    if (myHero == null || nonAI)
                                    {
                                        var d = 120f;
                                        var velocity = baseT.Forward() * speed + Vectors.up * d;
                                        if (nonAI && abnormalType == AbnormalType.Crawler)
                                        {
                                            d = 100f;
                                            var num6 = speed * 2.5f;
                                            num6 = Mathf.Min(num6, 100f);
                                            velocity = baseT.Forward() * num6 + Vectors.up * d;
                                        }

                                        baseR.velocity = velocity;
                                    }
                                    else
                                    {
                                        var y2 = myHero.rigidbody.velocity.y;
                                        var num7 = -20f;
                                        var num8 = Gravity;
                                        var y3 = Neck.position.y;
                                        var num9 = (num7 - num8) * 0.5f;
                                        var num10 = y2;
                                        var position = myHero.transform.position;
                                        var num11 = position.y - y3;
                                        var d2 = Mathf.Abs((Mathf.Sqrt(num10 * num10 - 4f * num9 * num11) - num10) /
                                                           (2f * num9));
                                        var a = position + myHero.rigidbody.velocity * d2 +
                                                Vectors.up * 0.5f * num7 * d2 * d2;
                                        var y4 = a.y;
                                        float num12;
                                        if (num11 < 0f || y4 - y3 < 0f)
                                        {
                                            num12 = 60f;
                                            var num13 = speed * 2.5f;
                                            num13 = Mathf.Min(num13, 100f);
                                            var velocity2 = baseT.Forward() * num13 + Vectors.up * num12;
                                            baseR.velocity = velocity2;
                                            return;
                                        }

                                        var num14 = y4 - y3;
                                        var num15 = Mathf.Sqrt(2f * num14 / Gravity);
                                        num12 = Gravity * num15;
                                        num12 = Mathf.Max(30f, num12);
                                        var position2 = baseT.position;
                                        var vector2 = (a - position2) / d2;
                                        abnorma_jump_bite_horizon_v = new Vector3(vector2.x, 0f, vector2.z);
                                        var velocity3 = baseR.velocity;
                                        var force = new Vector3(abnorma_jump_bite_horizon_v.x, velocity3.y,
                                            abnorma_jump_bite_horizon_v.z) - velocity3;
                                        baseR.AddForce(force, ForceMode.VelocityChange);
                                        baseR.AddForce(Vectors.up * num12, ForceMode.VelocityChange);
                                        var position1 = myHero.transform.position;
                                        var y5 = Mathf.Atan2(position1.x - position2.x,
                                            position1.z - position2.z) * 57.29578f;
                                        baseGT.rotation = Quaternion.Euler(0f, y5, 0f);
                                    }
                                }
                                else
                                {
                                    baseR.velocity = Vectors.zero;
                                }
                            }

                            if (baseA["attack_" + attackAnimation].normalizedTime >= 1f)
                            {
                                var hero7 = CheckHitHeadAndCrawlerMouth(3f);
                                if (hero7 != null)
                                {
                                    if (IN_GAME_MAIN_CAMERA.GameType == GameType.Single)
                                    {
                                        hero7.Die((hero7.transform.position - Chest.position) * 15f * myLevel, false);
                                    }
                                    else if (IN_GAME_MAIN_CAMERA.GameType == GameType.MultiPlayer && BasePV.IsMine &&
                                             !hero7.HasDied())
                                    {
                                        hero7.MarkDie();
                                        hero7.BasePV.RPC("netDie", PhotonTargets.All,
                                            (hero7.transform.position - Chest.position) * 15f * myLevel, true,
                                            !nonAI ? -1 : BasePV.viewID, ShowName, true);
                                    }

                                    if (abnormalType == AbnormalType.Crawler)
                                        attackAnimation = "crawler_jump_1";
                                    else
                                        attackAnimation = "jumper_1";
                                    PlayAnimation("attack_" + attackAnimation);
                                }

                                if (Mathf.Abs(baseR.velocity.y) < 0.5f || baseR.velocity.y < 0f || IsGrounded())
                                {
                                    if (abnormalType == AbnormalType.Crawler)
                                        attackAnimation = "crawler_jump_1";
                                    else
                                        attackAnimation = "jumper_1";
                                    PlayAnimation("attack_" + attackAnimation);
                                }
                            }

                            break;
                        }
                        case "jumper_1":
                        case "crawler_jump_1":
                        {
                            if (baseA["attack_" + attackAnimation].normalizedTime >= 1f && grounded)
                            {
                                if (abnormalType == AbnormalType.Crawler)
                                    attackAnimation = "crawler_jump_2";
                                else
                                    attackAnimation = "jumper_2";
                                CrossFade("attack_" + attackAnimation, 0.1f);
                                fxPosition = baseT.position;
                                GameObject gameObject8;
                                if (IN_GAME_MAIN_CAMERA.GameType == GameType.MultiPlayer && BasePV.IsMine)
                                    gameObject8 = Pool.NetworkEnable("FX/boom2", fxPosition, fxRotation);
                                else
                                    gameObject8 =
                                        Pool.Enable("FX/boom2", fxPosition,
                                            fxRotation); //(GameObject)UnityEngine.Object.Instantiate(CacheResources.Load("FX/boom2"), this.fxPosition, this.fxRotation);
                                gameObject8.transform.localScale = baseT.localScale * 1.6f;
                                var num16 = 1f - Vector3.Distance(IN_GAME_MAIN_CAMERA.MainCamera.transform.position,
                                    gameObject8.transform.position) * 0.05f;
                                num16 = Mathf.Min(1f, num16);
                                IN_GAME_MAIN_CAMERA.MainCamera.startShake(num16, num16);
                            }

                            break;
                        }
                        case "jumper_2":
                        case "crawler_jump_2":
                        {
                            if (baseA["attack_" + attackAnimation].normalizedTime >= 1f) Idle();
                            break;
                        }
                        default:
                        {
                            if (baseA.IsPlaying("tired"))
                            {
                                if (baseA["tired"].normalizedTime >= 1f + Mathf.Max(attackEndWait * 2f, 3f))
                                    Idle(Random.Range(attackWait - 1f, 3f));
                            }
                            else if (baseA["attack_" + attackAnimation].normalizedTime >= 1f + attackEndWait)
                            {
                                if (nextAttackAnimation != null)
                                {
                                    Attack(nextAttackAnimation);
                                    return;
                                }

                                if (attackAnimation == "quick_turn_l" || attackAnimation == "quick_turn_r")
                                {
                                    var rotation = baseT.rotation;
                                    rotation = Quaternion.Euler(rotation.eulerAngles.x,
                                        rotation.eulerAngles.y + 180f, rotation.eulerAngles.z);
                                    baseT.rotation = rotation;
                                    Idle(Random.Range(0.5f, 1f));
                                    PlayAnimation("idle");
                                    return;
                                }

                                if (abnormalType == AbnormalType.Aberrant || abnormalType == AbnormalType.Jumper)
                                {
                                    attackCount++;
                                    if (attackCount > 3 && attackAnimation == "abnormal_getup")
                                    {
                                        attackCount = 0;
                                        CrossFade("tired", 0.5f);
                                    }
                                    else
                                    {
                                        Idle(Random.Range(attackWait - 1f, 3f));
                                    }
                                }
                                else
                                {
                                    Idle(Random.Range(attackWait - 1f, 3f));
                                }
                            }

                            break;
                        }
                    }

                    break;

                case TitanState.Grad:
                    if (baseA["grab_" + attackAnimation].normalizedTime >= attackCheckTimeA &&
                        baseA["grab_" + attackAnimation].normalizedTime <= attackCheckTimeB && grabbedTarget == null)
                    {
                        var gameObject9 = CheckIfHitHand(currentGrabHand);
                        if (gameObject9 != null)
                        {
                            EatSet(gameObject9);
                            grabbedTarget = gameObject9;
                        }
                    }

                    if (baseA["grab_" + attackAnimation].normalizedTime >= 1f)
                    {
                        if (grabbedTarget)
                            Eat();
                        else
                            Idle(Random.Range(attackWait - 1f, 2f));
                    }

                    break;

                case TitanState.Eat:
                    if (!attacked && baseA[attackAnimation].normalizedTime >= 0.48f)
                    {
                        attacked = true;
                        JustEatHero(grabbedTarget, currentGrabHand);
                    }

                    if (grabbedTarget == null)
                    {
                    }

                    if (baseA[attackAnimation].normalizedTime >= 1f) Idle();
                    break;

                case TitanState.Chase:

                    if (myHero == null)
                    {
                        Idle();
                        return;
                    }

                    if (LongRangeAttackCheck()) return;
                    if (IN_GAME_MAIN_CAMERA.GameMode == GameMode.PVP_CAPTURE && PVPfromCheckPt != null &&
                        myDistance > chaseDistance)
                    {
                        Idle();
                        return;
                    }

                    if (abnormalType == AbnormalType.Crawler)
                    {
                        var vector3 = myHero.transform.position - baseT.position;
                        var current = -Mathf.Atan2(vector3.z, vector3.x) * 57.29578f;
                        var f = -Mathf.DeltaAngle(current, baseGT.rotation.eulerAngles.y - 90f);
                        if (myDistance < attackDistance * 3f && Random.Range(0f, 1f) < 0.1f && Mathf.Abs(f) < 90f &&
                            myHero.transform.position.y < Neck.position.y + 30f * myLevel &&
                            myHero.transform.position.y > Neck.position.y + 10f * myLevel)
                        {
                            Attack("crawler_jump_0");
                            return;
                        }

                        var gameObject10 = CheckHitHeadAndCrawlerMouth(2.2f);
                        if (gameObject10 != null)
                        {
                            var position6 = Chest.position;
                            switch (IN_GAME_MAIN_CAMERA.GameType)
                            {
                                case GameType.Single:
                                    gameObject10.Die((gameObject10.transform.position - position6) * 15f * myLevel,
                                        false);
                                    break;
                                case GameType.MultiPlayer when BasePV.IsMine:
                                {
                                    if (gameObject10.eren)
                                    {
                                        gameObject10.eren.GetComponent<TITAN_EREN>().hitByTitan();
                                    }
                                    else if (!gameObject10.HasDied())
                                    {
                                        gameObject10.MarkDie();
                                        gameObject10.BasePV.RPC("netDie", PhotonTargets.All,
                                            (gameObject10.baseT.position - position6) * 15f * myLevel, true,
                                            !nonAI ? -1 : BasePV.viewID, ShowName, true);
                                    }

                                    break;
                                }
                            }
                        }

                        if (myDistance < attackDistance && Random.Range(0f, 1f) < 0.02f)
                            Idle(Random.Range(0.05f, 0.2f));
                    }
                    else
                    {
                        if (abnormalType == AbnormalType.Jumper &&
                            (myDistance > attackDistance &&
                             myHero.transform.position.y > Head.position.y + 4f * myLevel ||
                             myHero.transform.position.y > Head.position.y + 4f * myLevel) &&
                            Vector3.Distance(baseT.position, myHero.transform.position) <
                            1.5f * myHero.transform.position.y)
                        {
                            Attack("jumper_0");
                            return;
                        }

                        if (myDistance < attackDistance) Idle(Random.Range(0.05f, 0.2f));
                    }

                    break;

                case TitanState.Wander:

                    if (myDistance < chaseDistance || whoHasTauntMe != null)
                    {
                        var vector4 = myHero.transform.position - baseT.position;
                        var current2 = -Mathf.Atan2(vector4.z, vector4.x) * 57.29578f;
                        var f2 = -Mathf.DeltaAngle(current2, baseGT.rotation.eulerAngles.y - 90f);
                        if (isAlarm || Mathf.Abs(f2) < 90f)
                        {
                            Chase();
                            return;
                        }

                        if (!isAlarm && myDistance < chaseDistance * 0.1f)
                        {
                            Chase();
                            return;
                        }
                    }

                    if (Random.Range(0f, 1f) < 0.01f) Idle();
                    break;

                case TitanState.Turn:
                    baseGT.rotation = Quaternion.Lerp(baseGT.rotation, Quaternion.Euler(0f, desDeg, 0f),
                        dt * Mathf.Abs(turnDeg) * 0.015f);
                    if (baseA[turnAnimation].normalizedTime >= 1f) Idle();
                    break;

                case TitanState.Hit_Eye:
                    if (baseA.IsPlaying("sit_hit_eye") && baseA["sit_hit_eye"].normalizedTime >= 1f)
                    {
                        RemainSitdown();
                    }
                    else if (baseA.IsPlaying("hit_eye") && baseA["hit_eye"].normalizedTime >= 1f)
                    {
                        if (nonAI)
                            Idle();
                        else
                            Attack("combo_1");
                    }

                    break;

                case TitanState.To_CheckPoint:
                    if (checkPoints.Count <= 0)
                        if (myDistance < attackDistance)
                        {
                            var decidedAction = string.Empty;
                            var attackStrategy2 = GetAttackStrategy();
                            if (attackStrategy2 != null)
                                decidedAction = attackStrategy2[Random.Range(0, attackStrategy2.Length)];
                            if (ExecuteAttack(decidedAction)) return;
                        }

                    if (Vector3.Distance(baseT.position, targetCheckPt) < targetR)
                    {
                        if (checkPoints.Count > 0)
                        {
                            if (checkPoints.Count == 1)
                            {
                                if (IN_GAME_MAIN_CAMERA.GameMode == GameMode.BOSS_FIGHT_CT)
                                {
                                    FengGameManagerMKII.FGM.GameLose();
                                    checkPoints = new ArrayList();
                                    Idle();
                                }
                            }
                            else
                            {
                                if (checkPoints.Count == 4)
                                    FengGameManagerMKII.FGM.SendChatContentInfo(
                                        "<color=#A8FF24>*WARNING!* An abnormal titan is approaching the north gate!</color>");
                                var vector5 = (Vector3) checkPoints[0];
                                targetCheckPt = vector5;
                                checkPoints.RemoveAt(0);
                            }
                        }
                        else
                        {
                            Idle();
                        }
                    }

                    break;

                case TitanState.To_PVP_PT:
                    if (myDistance < chaseDistance * 0.7f) Chase();
                    if (Vector3.Distance(baseT.position, targetCheckPt) < targetR) Idle();
                    break;

                case TitanState.Random_Run:
                    random_run_time -= dt;
                    if (Vector3.Distance(baseT.position, targetCheckPt) < targetR || random_run_time <= 0f) Idle();
                    break;

                case TitanState.Down:
                    getdownTime -= dt;
                    if (baseA.IsPlaying("sit_hunt_down") && baseA["sit_hunt_down"].normalizedTime >= 1f)
                        PlayAnimation("sit_idle");
                    if (getdownTime <= 0f) CrossFade("sit_getup", 0.1f);
                    if (baseA.IsPlaying("sit_getup") && baseA["sit_getup"].normalizedTime >= 1f) Idle();
                    break;

                case TitanState.Sit:
                    getdownTime -= dt;
                    angle = 0f;
                    between2 = 0f;
                    if (myDistance < chaseDistance || whoHasTauntMe != null)
                    {
                        if (myDistance < 50f)
                        {
                            isAlarm = true;
                        }
                        else
                        {
                            var vector6 = myHero.transform.position - baseT.position;
                            angle = -Mathf.Atan2(vector6.z, vector6.x) * 57.29578f;
                            between2 = -Mathf.DeltaAngle(angle, baseGT.rotation.eulerAngles.y - 90f);
                            if (Mathf.Abs(between2) < 100f) isAlarm = true;
                        }
                    }

                    if (baseA.IsPlaying("sit_down") && baseA["sit_down"].normalizedTime >= 1f)
                        PlayAnimation("sit_idle");
                    if ((getdownTime <= 0f || isAlarm) && baseA.IsPlaying("sit_idle")) CrossFade("sit_getup", 0.1f);
                    if (baseA.IsPlaying("sit_getup") && baseA["sit_getup"].normalizedTime >= 1f) Idle();
                    break;

                case TitanState.Recover:
                    getdownTime -= dt;
                    if (getdownTime <= 0f) Idle();
                    if (baseA.IsPlaying("idle_recovery") && baseA["idle_recovery"].normalizedTime >= 1f) Idle();
                    break;
            }

            return;
        }

        dieTime += dt;
        if (dieTime > 2f && !hasDieSteam)
        {
            hasDieSteam = true;
            if (IN_GAME_MAIN_CAMERA.GameType == GameType.Single)
            {
                var gameObject11 =
                    Pool.Enable("FX/FXtitanDie1", Hip.position,
                        Quaternion
                            .identity); //(GameObject)UnityEngine.Object.Instantiate(CacheResources.Load("FX/FXtitanDie1"));
                //gameObject11.transform.position = Hip.position;
                gameObject11.transform.localScale = baseT.localScale;
            }
            else if (BasePV.IsMine)
            {
                var gameObject12 = Pool.NetworkEnable("FX/FXtitanDie1", Hip.position, Quaternion.Euler(-90f, 0f, 0f));
                gameObject12.transform.localScale = baseT.localScale;
            }
        }

        if (dieTime > 5f)
        {
            if (IN_GAME_MAIN_CAMERA.GameType == GameType.Single)
            {
                var gameObject13 =
                    Pool.Enable("FX/FXtitanDie", Hip.position,
                        Quaternion
                            .identity); //(GameObject)UnityEngine.Object.Instantiate(CacheResources.Load("FX/FXtitanDie"));
                //gameObject13.transform.position = Hip.position;
                gameObject13.transform.localScale = baseT.localScale;
                Destroy(baseG);
            }
            else if (BasePV.IsMine)
            {
                var gameObject14 = Pool.NetworkEnable("FX/FXtitanDie", Hip.position, Quaternion.Euler(-90f, 0f, 0f));
                gameObject14.transform.localScale = baseT.localScale;
                PhotonNetwork.Destroy(baseG);
                myDifficulty = -1;
            }
        }
    }

    public void UpdateCollider()
    {
        if (ColliderEnabled)
        {
            if (!IsHooked && !MyTitanTrigger.IsCollide && !IsLook)
            {   
                foreach (var col in baseColliders) col.enabled = false;
                ColliderEnabled = false;
            }
        }
        else if (IsHooked || MyTitanTrigger.IsCollide || IsLook)
        {
            foreach (var col in baseColliders) col.enabled = true;
            ColliderEnabled = true;
        }
    }
}