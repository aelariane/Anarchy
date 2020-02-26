using System;
using System.Collections;
using System.Collections.Generic;
using Anarchy;
using Anarchy.Configuration;
using Anarchy.Configuration.Presets;
using Anarchy.Skins.Titans;
using Optimization;
using Optimization.Caching;
using RC;
using UnityEngine;

public class TITAN : Optimization.Caching.Bases.TitanBase
{
    private static string[] titanNames = new string[] { "Titan", "Aberrant", "Jumper", "Crawler", "Punk" };

    private Vector3 abnorma_jump_bite_horizon_v;
    private float angle;
    private string attackAnimation;
    private float attackCheckTime;
    private float attackCheckTimeA;
    private float attackCheckTimeB;
    private int attackCount;
    private bool attacked;
    private float attackEndWait;
    private List<Collider> baseColliders;
    private float between2;
    private Transform currentGrabHand;
    private float desDeg;
    private float dieTime;
    private string fxName;
    private Vector3 fxPosition;
    private Quaternion fxRotation;
    private float getdownTime;
    private HERO grabbedTarget;
    private float gravity = 120f;
    private bool grounded;
    private bool hasDieSteam;
    private bool hasExplode;
    private Vector3 headscale = Vectors.one;
    private float healthTime;
    private string hitAnimation;
    private float hitPause;
    private bool isAttackMoveByCore;
    private bool isGrabHandLeft;
    private float lagMax;
    private bool leftHandAttack;
    private float maxStamina = 320f;
    private bool needFreshCorePosition;
    private string nextAttackAnimation;
    private bool nonAIcombo;
    private Vector3 oldCorePosition;
    private Quaternion oldHeadRotation;
    private float random_run_time;
    private float rockInterval;
    private string runAnimation;
    private float sbtime;
    private bool spawned = false;
    private Vector3 spawnPt;
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

    public static float minusDistance = 99999f;
    public static GameObject minusDistanceEnemy;
    public AbnormalType abnormalType;
    public int activeRad = int.MaxValue;
    internal int armor;
    public bool asClientLookTarget;
    public float attackDistance = 13f;
    public float attackWait = 1f;
    public float chaseDistance = 80f;
    public ArrayList checkPoints = new ArrayList();
    public bool ColliderEnabled;
    public TITAN_CONTROLLER controller;
    public GameObject currentCamera;
    public int currentHealth;
    public GameObject grabTF;
    public bool hasDie;
    public bool hasSetLevel;
    public GameObject healthLabel;
    public bool healthLabelEnabled;

    public bool isAlarm;
    public bool IsHooked = false;
    public bool IsLook;
    public GameObject mainMaterial;
    public int maxHealth;
    public float maxVelocityChange = 10f;
    public int myDifficulty;
    public float myDistance;
    public Group myGroup = Group.T;
    public GameObject myHero;

    public float myLevel = 1f;
    public TitanTrigger MyTitanTrigger;
    public bool nonAI;
    public PVPcheckPoint PVPfromCheckPt;
    internal TitanSkin Skin;
    public float speed = 7f;

    public string ShowName { get; private set; }


    private void attack(string type)
    {
        this.state = TitanState.Attack;
        this.attacked = false;
        this.isAlarm = true;
        if (this.attackAnimation == type)
        {
            this.attackAnimation = type;
            this.playAnimationAt("attack_" + type, 0f);
        }
        else
        {
            this.attackAnimation = type;
            this.playAnimationAt("attack_" + type, 0f);
        }
        this.nextAttackAnimation = null;
        this.fxName = null;
        this.isAttackMoveByCore = false;
        this.attackCheckTime = 0f;
        this.attackCheckTimeA = 0f;
        this.attackCheckTimeB = 0f;
        this.attackEndWait = 0f;
        this.fxRotation = Quaternion.Euler(270f, 0f, 0f);
        switch (type)
        {
            case "abnormal_getup":
                this.attackCheckTime = 0f;
                this.fxName = string.Empty;
                break;

            case "abnormal_jump":
                this.nextAttackAnimation = "abnormal_getup";
                if (this.nonAI)
                {
                    this.attackEndWait = 0f;
                }
                else
                {
                    this.attackEndWait = ((this.myDifficulty <= 0) ? UnityEngine.Random.Range(1f, 4f) : UnityEngine.Random.Range(0f, 1f));
                }
                this.attackCheckTime = 0.75f;
                this.fxName = "boom4";
                this.fxRotation = Quaternion.Euler(270f, baseT.rotation.eulerAngles.y, 0f);
                break;

            case "combo_1":
                this.nextAttackAnimation = "combo_2";
                this.attackCheckTimeA = 0.54f;
                this.attackCheckTimeB = 0.76f;
                this.nonAIcombo = false;
                this.isAttackMoveByCore = true;
                this.leftHandAttack = false;
                break;

            case "combo_2":
                if (this.abnormalType != AbnormalType.Punk)
                {
                    this.nextAttackAnimation = "combo_3";
                }
                this.attackCheckTimeA = 0.37f;
                this.attackCheckTimeB = 0.57f;
                this.nonAIcombo = false;
                this.isAttackMoveByCore = true;
                this.leftHandAttack = true;
                break;

            case "combo_3":
                this.nonAIcombo = false;
                this.isAttackMoveByCore = true;
                this.attackCheckTime = 0.21f;
                this.fxName = "boom1";
                break;

            case "front_ground":
                this.fxName = "boom1";
                this.attackCheckTime = 0.45f;
                break;

            case "kick":
                this.fxName = "boom5";
                this.fxRotation = baseT.rotation;
                this.attackCheckTime = 0.43f;
                break;

            case "slap_back":
                this.fxName = "boom3";
                this.attackCheckTime = 0.66f;
                break;

            case "slap_face":
                this.fxName = "boom3";
                this.attackCheckTime = 0.655f;
                break;

            case "stomp":
                this.fxName = "boom2";
                this.attackCheckTime = 0.42f;
                break;

            case "bite":
                this.fxName = "bite";
                this.attackCheckTime = 0.6f;
                break;

            case "bite_l":
                this.fxName = "bite";
                this.attackCheckTime = 0.4f;
                break;

            case "bite_r":
                this.fxName = "bite";
                this.attackCheckTime = 0.4f;
                break;

            case "jumper_0":
                this.abnorma_jump_bite_horizon_v = Vectors.zero;
                break;

            case "crawler_jump_0":
                this.abnorma_jump_bite_horizon_v = Vectors.zero;
                break;

            case "anti_AE_l":
                this.attackCheckTimeA = 0.31f;
                this.attackCheckTimeB = 0.4f;
                this.leftHandAttack = true;
                break;

            case "anti_AE_r":
                this.attackCheckTimeA = 0.31f;
                this.attackCheckTimeB = 0.4f;
                this.leftHandAttack = false;
                break;

            case "anti_AE_low_l":
                this.attackCheckTimeA = 0.31f;
                this.attackCheckTimeB = 0.4f;
                this.leftHandAttack = true;
                break;

            case "anti_AE_low_r":
                this.attackCheckTimeA = 0.31f;
                this.attackCheckTimeB = 0.4f;
                this.leftHandAttack = false;
                break;

            case "quick_turn_l":
                this.attackCheckTimeA = 2f;
                this.attackCheckTimeB = 2f;
                this.isAttackMoveByCore = true;
                break;

            case "quick_turn_r":
                this.attackCheckTimeA = 2f;
                this.attackCheckTimeB = 2f;
                this.isAttackMoveByCore = true;
                break;

            case "throw":
                this.isAlarm = true;
                this.chaseDistance = 99999f;
                break;
        }
        this.needFreshCorePosition = true;
    }

    public void pt()
    {
        if (this.controller.sit)
        {
            this.sitdown();
        }
        if (this.controller.bite)
        {
            this.attack("bite");
        }
        if (this.controller.bitel)
        {
            this.attack("bite_l");
        }
        if (this.controller.biter)
        {
            this.attack("bite_r");
        }
        if (this.controller.chopl)
        {
            this.attack("anti_AE_low_l");
        }
        if (this.controller.chopr)
        {
            this.attack("anti_AE_low_r");
        }
        if (this.controller.choptl)
        {
            this.attack("anti_AE_l");
        }
        if (this.controller.choptr)
        {
            this.attack("anti_AE_r");
        }
        if (this.controller.cover && this.stamina > 75f)
        {
            this.recover();
            this.stamina -= 75f;
        }
        if (this.controller.grabbackl)
        {
            this.grab("ground_back_l");
        }
        if (this.controller.grabbackr)
        {
            this.grab("ground_back_r");
        }
        if (this.controller.grabfrontl)
        {
            this.grab("ground_front_l");
        }
        if (this.controller.grabfrontr)
        {
            this.grab("ground_front_r");
        }
        if (this.controller.grabnapel)
        {
            this.grab("head_back_l");
        }
        if (this.controller.grabnaper)
        {
            this.grab("head_back_r");
        }
    }

    private void Awake()
    {
        base.Cache();
        baseR.freezeRotation = true;
        baseR.useGravity = false;
        this.controller = baseG.GetComponent<TITAN_CONTROLLER>();
        this.baseColliders = new List<Collider>();
        foreach (Collider childCollider in base.GetComponentsInChildren<Collider>())
        {
            if (childCollider.name != "AABB")
            {
                this.baseColliders.Add(childCollider);
            }
        }
        GameObject obj = new GameObject();
        obj.name = "PlayerDetectorRC";
        CapsuleCollider triggerCollider = obj.AddComponent<CapsuleCollider>();
        CapsuleCollider referenceCollider = AABB.GetComponent<CapsuleCollider>();
        triggerCollider.center = referenceCollider.center;
        triggerCollider.radius = Math.Abs(base.Head.position.y - baseT.position.y);
        triggerCollider.height = referenceCollider.height * 1.2f;
        triggerCollider.material = referenceCollider.material;
        triggerCollider.isTrigger = true;
        triggerCollider.name = "PlayerDetectorRC";
        this.MyTitanTrigger = obj.AddComponent<TitanTrigger>();
        this.MyTitanTrigger.IsCollide = false;
        obj.layer = 16;
        obj.transform.parent = AABB;
        obj.transform.localPosition = new Vector3(0f, 0f, 0f);
        this.ColliderEnabled = true;
        this.IsHooked = false;
        this.IsLook = false;
    }

    private void chase()
    {
        this.state = TitanState.Chase;
        this.isAlarm = true;
        this.crossFade(this.runAnimation, 0.5f);
    }

    private GameObject checkIfHitCrawlerMouth(Transform head, float rad)
    {
        float num = rad * this.myLevel;
        GameObject[] array = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject gameObject in array)
        {
            if (!gameObject.GetComponent<TITAN_EREN>())
            {
                if (!gameObject.GetComponent<HERO>() || !gameObject.GetComponent<HERO>().IsInvincible())
                {
                    float num2 = gameObject.GetComponent<CapsuleCollider>().height * 0.5f;
                    if (Vector3.Distance(gameObject.transform.position + Vectors.up * num2, head.position - Vectors.up * 1.5f * this.myLevel) < num + num2)
                    {
                        return gameObject;
                    }
                }
            }
        }
        return null;
    }

    private HERO CheckHitHeadAndCrawlerMouth(float rad)
    {
        float num = rad * myLevel;
        foreach (HERO curr in FengGameManagerMKII.Heroes)
        {
            if (curr.eren != null || !curr.IsInvincible())
            {
                float idk = curr.GetComponent<CapsuleCollider>().height * 0.5f;
                if ((curr.baseT.position + Vectors.up * idk - (this.Head.position - Vectors.up * 1.5f * myLevel)).magnitude < num + idk)
                {
                    return curr;
                }
            }
        }
        return null;
    }

    private HERO CheckIfHitHand(Transform hand)
    {
        float num = 2.4f * myLevel;
        HERO hero = null;
        IEnumerator ienum = Physics.OverlapSphere(hand.position, num + 1f).GetEnumerator();
        while (ienum.MoveNext())
        {
            if ((hero = ((Collider)ienum.Current).transform.root.gameObject.GetComponent<HERO>()) != null)
            {
                if (hero.eren != null)
                {
                    if (!hero.eren.GetComponent<TITAN_EREN>().isHit)
                    {
                        hero.eren.GetComponent<TITAN_EREN>().hitByTitan();
                    }
                }
                if (!hero.IsInvincible())
                {
                    return hero;
                }
            }
        }
        return hero;
    }

    private void crossFade(string aniName, float time)
    {
        baseA.CrossFade(aniName, time);
        if (IN_GAME_MAIN_CAMERA.GameType == GameType.MultiPlayer && BasePV.IsMine)
        {
            BasePV.RPC("netCrossFade", PhotonTargets.Others, new object[]
            {
                aniName,
                time
            });
        }
    }

    private void dieAnimation()
    {
        if (baseA.IsPlaying("sit_idle") || baseA.IsPlaying("sit_hit_eye"))
        {
            this.crossFade("sit_die", 0.1f);
        }
        else if (this.abnormalType == AbnormalType.Crawler)
        {
            this.crossFade("crawler_die", 0.2f);
        }
        else if (this.abnormalType == AbnormalType.Normal)
        {
            this.crossFade("die_front", 0.05f);
        }
        else if ((baseA.IsPlaying("attack_abnormal_jump") && baseA["attack_abnormal_jump"].normalizedTime > 0.7f) || (baseA.IsPlaying("attack_abnormal_getup") && baseA["attack_abnormal_getup"].normalizedTime < 0.7f) || baseA.IsPlaying("tired"))
        {
            this.crossFade("die_ground", 0.2f);
        }
        else
        {
            this.crossFade("die_back", 0.05f);
        }
    }

    [RPC]
    private void dieBlowRPC(Vector3 attacker, float hitPauseTime)
    {
        if (BasePV.IsMine)
        {
            float magnitude = (attacker - baseT.position).magnitude;
            if (magnitude < 80f)
            {
                this.dieBlowFunc(attacker, hitPauseTime);
            }
        }
    }

    [RPC]
    private void dieHeadBlowRPC(Vector3 attacker, float hitPauseTime)
    {
        if (BasePV.IsMine)
        {
            float magnitude = (attacker - baseT.position).magnitude;
            if (magnitude < 80f)
            {
                this.dieHeadBlowFunc(attacker, hitPauseTime);
            }
        }
    }

    private void eat()
    {
        this.state = TitanState.Eat;
        this.attacked = false;
        if (this.isGrabHandLeft)
        {
            this.attackAnimation = "eat_l";
            this.crossFade("eat_l", 0.1f);
        }
        else
        {
            this.attackAnimation = "eat_r";
            this.crossFade("eat_r", 0.1f);
        }
    }

    private void EatSet(HERO hero)
    {
        if (!hero.isGrabbed || !BasePV.IsMine)
        {
            if (this.isGrabHandLeft)
            {
                this.grabToLeft();
            }
            else
            {
                this.grabToRight();
            }
            if (IN_GAME_MAIN_CAMERA.GameType == GameType.MultiPlayer && BasePV.IsMine)
            {
                BasePV.RPC(this.isGrabHandLeft ? "grabToLeft" : "grabToRight", PhotonTargets.Others, new object[0]);
                hero.BasePV.RPC("netPlayAnimation", PhotonTargets.All, new object[]
                {
                "grabbed"
                });
                hero.BasePV.RPC("netGrabbed", PhotonTargets.All, new object[] { this.BasePV.viewID, this.isGrabHandLeft });
                return;
            }
            hero.grabbed(baseG, this.isGrabHandLeft);
            hero.baseA.Play("grabbed");
        }
    }
    private bool executeAttack(string decidedAction)
    {
        switch (decidedAction)
        {
            case "grab_ground_front_l":
                this.grab("ground_front_l");
                return true;

            case "grab_ground_front_r":
                this.grab("ground_front_r");
                return true;

            case "grab_ground_back_l":
                this.grab("ground_back_l");
                return true;

            case "grab_ground_back_r":
                this.grab("ground_back_r");
                return true;

            case "grab_head_front_l":
                this.grab("head_front_l");
                return true;

            case "grab_head_front_r":
                this.grab("head_front_r");
                return true;

            case "grab_head_back_l":
                this.grab("head_back_l");
                return true;

            case "grab_head_back_r":
                this.grab("head_back_r");
                return true;

            case "attack_abnormal_jump":
                this.attack("abnormal_jump");
                return true;

            case "attack_combo":
                this.attack("combo_1");
                return true;

            case "attack_front_ground":
                this.attack("front_ground");
                return true;

            case "attack_kick":
                this.attack("kick");
                return true;

            case "attack_slap_back":
                this.attack("slap_back");
                return true;

            case "attack_slap_face":
                this.attack("slap_face");
                return true;

            case "attack_stomp":
                this.attack("stomp");
                return true;

            case "attack_bite":
                this.attack("bite");
                return true;

            case "attack_bite_l":
                this.attack("bite_l");
                return true;

            case "attack_bite_r":
                this.attack("bite_r");
                return true;
        }
        return false;
    }

    private void Explode()
    {
        if ((!GameModes.ExplodeMode.Enabled || GameModes.ExplodeMode.GetInt(0) <= 0) || !hasDie || dieTime < 1f || hasExplode)
        {
            return;
        }
        float d = this.myLevel * 10f;
        if (abnormalType == AbnormalType.Crawler)
        {
            if (dieTime < 2f)
                return;
            d = 0f;
        }
        else
        {
            this.hasExplode = true;
        }
        Vector3 vector = baseT.position + Vectors.up * d;
        Pool.NetworkEnable("FX/Thunder", vector, Quaternion.Euler(270f, 0f, 0f), 0);
        Pool.NetworkEnable("FX/boom1", vector, Quaternion.Euler(270f, 0f, 0f), 0);
        int rad = GameModes.ExplodeMode.GetInt(0);
        foreach (HERO hero in FengGameManagerMKII.Heroes)
        {
            if (Vector3.Distance(hero.baseT.position, vector) < rad)
            {
                hero.markDie();
                hero.BasePV.RPC("netDie2", PhotonTargets.All, new object[] { -1, "Server" });
            }
        }
    }

    private void FindNearestFacingHero()
    {
        GameObject[] array = GameObject.FindGameObjectsWithTag("Player");
        GameObject x = null;
        float num = float.PositiveInfinity;
        Vector3 position = baseT.position;
        float num2 = (this.abnormalType != AbnormalType.Normal) ? 180f : 100f;
        foreach (GameObject gameObject in array)
        {
            float sqrMagnitude = (gameObject.transform.position - position).sqrMagnitude;
            if (sqrMagnitude < num)
            {
                Vector3 vector = gameObject.transform.position - baseT.position;
                float current = -Mathf.Atan2(vector.z, vector.x) * 57.29578f;
                float f = -Mathf.DeltaAngle(current, baseGT.rotation.eulerAngles.y - 90f);
                if (Mathf.Abs(f) < num2)
                {
                    x = gameObject;
                    num = sqrMagnitude;
                }
            }
        }
        if (x != null)
        {
            GameObject x2 = this.myHero;
            this.myHero = x;
            if (x2 != this.myHero && IN_GAME_MAIN_CAMERA.GameType == GameType.MultiPlayer && PhotonNetwork.IsMasterClient)
            {
                BasePV.RPC("setMyTarget", PhotonTargets.Others, new object[] { myHero == null ? -1 : this.myHero.GetPhotonView().viewID });
            }
            this.tauntTime = 5f;
        }
    }

    private void FindNearestHero()
    {
        GameObject y = this.myHero;
        this.myHero = this.getNearestHero();
        if (this.myHero != y && IN_GAME_MAIN_CAMERA.GameType == GameType.MultiPlayer && PhotonNetwork.IsMasterClient)
        {
            BasePV.RPC("setMyTarget", PhotonTargets.Others, new object[] { myHero == null ? -1 : this.myHero.GetPhotonView().viewID });
        }
        this.oldHeadRotation = this.Head.rotation;
    }

    private void FixedUpdate()
    {
        if (IN_GAME_MAIN_CAMERA.isPausing && IN_GAME_MAIN_CAMERA.GameType == GameType.Single)
        {
            return;
        }
        if (IN_GAME_MAIN_CAMERA.GameType != GameType.Single)
        {
            if (!BasePV.IsMine)
            {
                return;
            }
        }
        baseR.AddForce(new Vector3(0f, -this.gravity * baseR.mass, 0f));
        if (this.needFreshCorePosition)
        {
            this.oldCorePosition = baseT.position - Core.position;
            this.needFreshCorePosition = false;
        }
        if (this.hasDie)
        {
            if (this.hitPause <= 0f)
            {
                if (baseA.IsPlaying("die_headOff"))
                {
                    Vector3 a = baseT.position - Core.position - this.oldCorePosition;
                    baseR.velocity = a / Time.deltaTime + Vectors.up * baseR.velocity.y;
                }
            }
            this.oldCorePosition = baseT.position - Core.position;
        }
        else if ((this.state == TitanState.Attack && this.isAttackMoveByCore) || this.state == TitanState.Hit)
        {
            Vector3 a2 = baseT.position - Core.position - this.oldCorePosition;
            baseR.velocity = a2 / Time.deltaTime + Vectors.up * baseR.velocity.y;
            this.oldCorePosition = baseT.position - Core.position;
        }
        if (this.hasDie)
        {
            if (this.hitPause > 0f)
            {
                this.hitPause -= Time.deltaTime;
                if (this.hitPause <= 0f)
                {
                    baseA[this.hitAnimation].speed = 1f;
                    this.hitPause = 0f;
                }
            }
            else if (baseA.IsPlaying("die_blow"))
            {
                if (baseA["die_blow"].normalizedTime < 0.55f)
                {
                    baseR.velocity = -baseT.Forward() * 300f + Vectors.up * baseR.velocity.y;
                }
                else if (baseA["die_blow"].normalizedTime < 0.83f)
                {
                    baseR.velocity = -baseT.Forward() * 100f + Vectors.up * baseR.velocity.y;
                }
                else
                {
                    baseR.velocity = Vectors.up * baseR.velocity.y;
                }
            }
            return;
        }
        if (this.nonAI && !IN_GAME_MAIN_CAMERA.isPausing && (this.state == TitanState.Idle || (this.state == TitanState.Attack && this.attackAnimation == "jumper_1")))
        {
            Vector3 a3 = Vectors.zero;
            if (this.controller.targetDirection != -874f)
            {
                bool flag = false;
                if (this.stamina < 5f)
                {
                    flag = true;
                }
                else if (this.stamina < 40f)
                {
                    if (!baseA.IsPlaying("run_abnormal") && !baseA.IsPlaying("crawler_run"))
                    {
                        flag = true;
                    }
                }
                if (this.controller.isWALKDown || flag)
                {
                    a3 = baseT.Forward() * this.speed * Mathf.Sqrt(this.myLevel) * 0.2f;
                }
                else
                {
                    a3 = baseT.Forward() * this.speed * Mathf.Sqrt(this.myLevel);
                }
                baseGT.rotation = Quaternion.Lerp(baseGT.rotation, Quaternion.Euler(0f, this.controller.targetDirection, 0f), this.speed * 0.15f * Time.deltaTime);
                if (this.state == TitanState.Idle)
                {
                    if (this.controller.isWALKDown || flag)
                    {
                        if (this.abnormalType == AbnormalType.Crawler)
                        {
                            if (!baseA.IsPlaying("crawler_run"))
                            {
                                this.crossFade("crawler_run", 0.1f);
                            }
                        }
                        else if (!baseA.IsPlaying("run_walk"))
                        {
                            this.crossFade("run_walk", 0.1f);
                        }
                    }
                    else if (this.abnormalType == AbnormalType.Crawler)
                    {
                        if (!baseA.IsPlaying("crawler_run"))
                        {
                            this.crossFade("crawler_run", 0.1f);
                        }
                        var hero = CheckHitHeadAndCrawlerMouth(2.2f);
                        if (hero != null)
                        {
                            Vector3 position = Chest.position;
                            if (IN_GAME_MAIN_CAMERA.GameType == GameType.Single)
                            {
                                hero.die((hero.baseT.position - position) * 15f * this.myLevel, false);
                            }
                            else if (IN_GAME_MAIN_CAMERA.GameType == GameType.MultiPlayer && base.BasePV.IsMine && !hero.HasDied())
                            {
                                hero.markDie();
                                hero.BasePV.RPC("netDie", PhotonTargets.All, new object[] { (hero.baseT.position - position) * 15f * this.myLevel, true, (!this.nonAI) ? -1 : BasePV.viewID, name, true });
                            }
                        }
                    }
                    else if (!baseA.IsPlaying("run_abnormal"))
                    {
                        this.crossFade("run_abnormal", 0.1f);
                    }
                }
            }
            else if (this.state == TitanState.Idle)
            {
                if (this.abnormalType == AbnormalType.Crawler)
                {
                    if (!baseA.IsPlaying("crawler_idle"))
                    {
                        this.crossFade("crawler_idle", 0.1f);
                    }
                }
                else if (!baseA.IsPlaying("idle"))
                {
                    this.crossFade("idle", 0.1f);
                }
                a3 = Vectors.zero;
            }
            if (this.state == TitanState.Idle)
            {
                Vector3 velocity = baseR.velocity;
                Vector3 force = a3 - velocity;
                force.x = Mathf.Clamp(force.x, -this.maxVelocityChange, this.maxVelocityChange);
                force.z = Mathf.Clamp(force.z, -this.maxVelocityChange, this.maxVelocityChange);
                force.y = 0f;
                baseR.AddForce(force, ForceMode.VelocityChange);
            }
            else if (this.state == TitanState.Attack && this.attackAnimation == "jumper_0")
            {
                Vector3 velocity2 = baseR.velocity;
                Vector3 force2 = a3 * 0.8f - velocity2;
                force2.x = Mathf.Clamp(force2.x, -this.maxVelocityChange, this.maxVelocityChange);
                force2.z = Mathf.Clamp(force2.z, -this.maxVelocityChange, this.maxVelocityChange);
                force2.y = 0f;
                baseR.AddForce(force2, ForceMode.VelocityChange);
            }
        }
        if ((this.abnormalType == AbnormalType.Aberrant || this.abnormalType == AbnormalType.Jumper) && !this.nonAI && this.state == TitanState.Attack && this.attackAnimation == "jumper_0")
        {
            Vector3 a4 = baseT.Forward() * this.speed * this.myLevel * 0.5f;
            Vector3 velocity3 = baseR.velocity;
            if (baseA["attack_jumper_0"].normalizedTime <= 0.28f || baseA["attack_jumper_0"].normalizedTime >= 0.8f)
            {
                a4 = Vectors.zero;
            }
            Vector3 force3 = a4 - velocity3;
            force3.x = Mathf.Clamp(force3.x, -this.maxVelocityChange, this.maxVelocityChange);
            force3.z = Mathf.Clamp(force3.z, -this.maxVelocityChange, this.maxVelocityChange);
            force3.y = 0f;
            baseR.AddForce(force3, ForceMode.VelocityChange);
        }
        if (this.state == TitanState.Chase || this.state == TitanState.Wander || this.state == TitanState.To_CheckPoint || this.state == TitanState.To_PVP_PT || this.state == TitanState.Random_Run)
        {
            Vector3 a5 = baseT.Forward() * this.speed;
            Vector3 velocity4 = baseR.velocity;
            Vector3 force4 = a5 - velocity4;
            force4.x = Mathf.Clamp(force4.x, -this.maxVelocityChange, this.maxVelocityChange);
            force4.z = Mathf.Clamp(force4.z, -this.maxVelocityChange, this.maxVelocityChange);
            force4.y = 0f;
            baseR.AddForce(force4, ForceMode.VelocityChange);
            if (!this.stuck && this.abnormalType != AbnormalType.Crawler && !this.nonAI)
            {
                if (baseA.IsPlaying(this.runAnimation) && baseR.velocity.magnitude < this.speed * 0.5f)
                {
                    this.stuck = true;
                    this.stuckTime = 2f;
                    this.stuckTurnAngle = (float)UnityEngine.Random.Range(0, 2) * 140f - 70f;
                }
                if (this.state == TitanState.Chase && this.myHero != null && this.myDistance > this.attackDistance && this.myDistance < 150f)
                {
                    float num = 0.05f;
                    if (this.myDifficulty > 1)
                    {
                        num += 0.05f;
                    }
                    if (this.abnormalType != AbnormalType.Normal)
                    {
                        num += 0.1f;
                    }
                    if (UnityEngine.Random.Range(0f, 1f) < num)
                    {
                        this.stuck = true;
                        this.stuckTime = 1f;
                        float num2 = UnityEngine.Random.Range(20f, 50f);
                        this.stuckTurnAngle = (float)UnityEngine.Random.Range(0, 2) * num2 * 2f - num2;
                    }
                }
            }
            float num3;
            if (this.state == TitanState.Wander)
            {
                num3 = baseT.rotation.eulerAngles.y - 90f;
            }
            else if (this.state == TitanState.To_CheckPoint || this.state == TitanState.To_PVP_PT || this.state == TitanState.Random_Run)
            {
                Vector3 vector = this.targetCheckPt - baseT.position;
                num3 = -Mathf.Atan2(vector.z, vector.x) * 57.29578f;
            }
            else
            {
                if (this.myHero == null)
                {
                    return;
                }
                Vector3 vector2 = this.myHero.transform.position - baseT.position;
                num3 = -Mathf.Atan2(vector2.z, vector2.x) * 57.29578f;
            }
            if (this.stuck)
            {
                this.stuckTime -= Time.deltaTime;
                if (this.stuckTime < 0f)
                {
                    this.stuck = false;
                }
                if (this.stuckTurnAngle > 0f)
                {
                    this.stuckTurnAngle -= Time.deltaTime * 10f;
                }
                else
                {
                    this.stuckTurnAngle += Time.deltaTime * 10f;
                }
                num3 += this.stuckTurnAngle;
            }
            float num4 = -Mathf.DeltaAngle(num3, baseGT.rotation.eulerAngles.y - 90f);
            if (this.abnormalType == AbnormalType.Crawler)
            {
                baseGT.rotation = Quaternion.Lerp(baseGT.rotation, Quaternion.Euler(0f, baseGT.rotation.eulerAngles.y + num4, 0f), this.speed * 0.3f * Time.deltaTime / this.myLevel);
            }
            else
            {
                baseGT.rotation = Quaternion.Lerp(baseGT.rotation, Quaternion.Euler(0f, baseGT.rotation.eulerAngles.y + num4, 0f), this.speed * 0.5f * Time.deltaTime / this.myLevel);
            }
        }
    }

    private string[] GetAttackStrategy()
    {
        string[] array = null;
        if (this.isAlarm || this.myHero.transform.position.y + 3f <= this.Neck.position.y + 10f * this.myLevel)
        {
            if (this.myHero.transform.position.y > this.Neck.position.y - 3f * this.myLevel)
            {
                if (this.myDistance < this.attackDistance * 0.5f)
                {
                    if (Vector3.Distance(this.myHero.transform.position, chkOverHead.position) < 3.6f * this.myLevel)
                    {
                        if (this.between2 > 0f)
                        {
                            array = new string[]
                            {
                                "grab_head_front_r"
                            };
                        }
                        else
                        {
                            array = new string[]
                            {
                                "grab_head_front_l"
                            };
                        }
                    }
                    else if (Mathf.Abs(this.between2) < 90f)
                    {
                        if (Mathf.Abs(this.between2) < 30f)
                        {
                            if (Vector3.Distance(this.myHero.transform.position, chkFront.position) < 2.5f * this.myLevel)
                            {
                                array = new string[]
                                {
                                    "attack_bite",
                                    "attack_bite",
                                    "attack_slap_face"
                                };
                            }
                        }
                        else if (this.between2 > 0f)
                        {
                            if (Vector3.Distance(this.myHero.transform.position, chkFrontRight.position) < 2.5f * this.myLevel)
                            {
                                array = new string[]
                                {
                                    "attack_bite_r"
                                };
                            }
                        }
                        else if (Vector3.Distance(this.myHero.transform.position, chkFrontLeft.position) < 2.5f * this.myLevel)
                        {
                            array = new string[]
                            {
                                "attack_bite_l"
                            };
                        }
                    }
                    else if (this.between2 > 0f)
                    {
                        if (Vector3.Distance(this.myHero.transform.position, chkBackRight.position) < 2.8f * this.myLevel)
                        {
                            array = new string[]
                            {
                                "grab_head_back_r",
                                "grab_head_back_r",
                                "attack_slap_back"
                            };
                        }
                    }
                    else if (Vector3.Distance(this.myHero.transform.position, chkBackLeft.position) < 2.8f * this.myLevel)
                    {
                        array = new string[]
                        {
                            "grab_head_back_l",
                            "grab_head_back_l",
                            "attack_slap_back"
                        };
                    }
                }
                if (array == null)
                {
                    if (this.abnormalType == AbnormalType.Normal || this.abnormalType == AbnormalType.Punk)
                    {
                        if ((this.myDifficulty > 0 || UnityEngine.Random.Range(0, 1000) < 3) && Mathf.Abs(this.between2) < 60f)
                        {
                            array = new string[]
                            {
                                "attack_combo"
                            };
                        }
                    }
                    else if ((this.abnormalType == AbnormalType.Aberrant || this.abnormalType == AbnormalType.Jumper) && (this.myDifficulty > 0 || UnityEngine.Random.Range(0, 100) < 50))
                    {
                        array = new string[]
                        {
                            "attack_abnormal_jump"
                        };
                    }
                }
            }
            else
            {
                int num;
                if (Mathf.Abs(this.between2) < 90f)
                {
                    if (this.between2 > 0f)
                    {
                        num = 1;
                    }
                    else
                    {
                        num = 2;
                    }
                }
                else if (this.between2 > 0f)
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
                        if (this.myDistance < this.attackDistance * 0.25f)
                        {
                            if (this.abnormalType == AbnormalType.Punk)
                            {
                                array = new string[]
                                {
                                "attack_kick",
                                "attack_stomp"
                                };
                            }
                            else if (this.abnormalType == AbnormalType.Normal)
                            {
                                array = new string[]
                                {
                                "attack_front_ground",
                                "attack_stomp"
                                };
                            }
                            else
                            {
                                array = new string[]
                                {
                                "attack_kick"
                                };
                            }
                        }
                        else if (this.myDistance < this.attackDistance * 0.5f)
                        {
                            if (this.abnormalType == AbnormalType.Punk)
                            {
                                array = new string[]
                                {
                                "grab_ground_front_r",
                                "grab_ground_front_r",
                                "attack_abnormal_jump"
                                };
                            }
                            else if (this.abnormalType == AbnormalType.Normal)
                            {
                                array = new string[]
                                {
                                "grab_ground_front_r",
                                "grab_ground_front_r",
                                "attack_stomp"
                                };
                            }
                            else
                            {
                                array = new string[]
                                {
                                "grab_ground_front_r",
                                "grab_ground_front_r",
                                "attack_abnormal_jump"
                                };
                            }
                        }
                        else if (this.abnormalType == AbnormalType.Punk)
                        {
                            array = new string[]
                            {
                            "attack_combo",
                            "attack_combo",
                            "attack_abnormal_jump"
                            };
                        }
                        else if (this.abnormalType == AbnormalType.Normal)
                        {
                            if (this.myDifficulty > 0)
                            {
                                array = new string[]
                                {
                                "attack_front_ground",
                                "attack_combo",
                                "attack_combo"
                                };
                            }
                            else
                            {
                                array = new string[]
                                {
                                "attack_front_ground",
                                "attack_front_ground",
                                "attack_front_ground",
                                "attack_front_ground",
                                "attack_combo"
                                };
                            }
                        }
                        else
                        {
                            array = new string[]
                            {
                            "attack_abnormal_jump"
                            };
                        }
                        break;

                    case 2:
                        if (this.myDistance < this.attackDistance * 0.25f)
                        {
                            if (this.abnormalType == AbnormalType.Punk)
                            {
                                array = new string[]
                                {
                                "attack_kick",
                                "attack_stomp"
                                };
                            }
                            else if (this.abnormalType == AbnormalType.Normal)
                            {
                                array = new string[]
                                {
                                "attack_front_ground",
                                "attack_stomp"
                                };
                            }
                            else
                            {
                                array = new string[]
                                {
                                "attack_kick"
                                };
                            }
                        }
                        else if (this.myDistance < this.attackDistance * 0.5f)
                        {
                            if (this.abnormalType == AbnormalType.Punk)
                            {
                                array = new string[]
                                {
                                "grab_ground_front_l",
                                "grab_ground_front_l",
                                "attack_abnormal_jump"
                                };
                            }
                            else if (this.abnormalType == AbnormalType.Normal)
                            {
                                array = new string[]
                                {
                                "grab_ground_front_l",
                                "grab_ground_front_l",
                                "attack_stomp"
                                };
                            }
                            else
                            {
                                array = new string[]
                                {
                                "grab_ground_front_l",
                                "grab_ground_front_l",
                                "attack_abnormal_jump"
                                };
                            }
                        }
                        else if (this.abnormalType == AbnormalType.Punk)
                        {
                            array = new string[]
                            {
                            "attack_combo",
                            "attack_combo",
                            "attack_abnormal_jump"
                            };
                        }
                        else if (this.abnormalType == AbnormalType.Normal)
                        {
                            if (this.myDifficulty > 0)
                            {
                                array = new string[]
                                {
                                "attack_front_ground",
                                "attack_combo",
                                "attack_combo"
                                };
                            }
                            else
                            {
                                array = new string[]
                                {
                                "attack_front_ground",
                                "attack_front_ground",
                                "attack_front_ground",
                                "attack_front_ground",
                                "attack_combo"
                                };
                            }
                        }
                        else
                        {
                            array = new string[]
                            {
                            "attack_abnormal_jump"
                            };
                        }
                        break;

                    case 3:
                        if (this.myDistance < this.attackDistance * 0.5f)
                        {
                            if (this.abnormalType == AbnormalType.Normal)
                            {
                                array = new string[]
                                {
                                "grab_ground_back_l"
                                };
                            }
                            else
                            {
                                array = new string[]
                                {
                                "grab_ground_back_l"
                                };
                            }
                        }
                        break;

                    case 4:
                        if (this.myDistance < this.attackDistance * 0.5f)
                        {
                            if (this.abnormalType == AbnormalType.Normal)
                            {
                                array = new string[]
                                {
                                "grab_ground_back_r"
                                };
                            }
                            else
                            {
                                array = new string[]
                                {
                                "grab_ground_back_r"
                                };
                            }
                        }
                        break;
                }
            }
        }
        return array;
    }

    private void getDown()
    {
        this.state = TitanState.Down;
        this.isAlarm = true;
        this.playAnimation("sit_hunt_down");
        this.getdownTime = UnityEngine.Random.Range(3f, 5f);
    }

    private GameObject getNearestHero()
    {
        GameObject[] array = GameObject.FindGameObjectsWithTag("Player");
        GameObject result = null;
        float num = float.PositiveInfinity;
        Vector3 position = baseT.position;
        foreach (GameObject gameObject in array)
        {
            float sqrMagnitude = (gameObject.transform.position - position).sqrMagnitude;
            if (sqrMagnitude < num)
            {
                result = gameObject;
                num = sqrMagnitude;
            }
        }
        return result;
    }

    private int getPunkNumber()
    {
        int num = 0;
        GameObject[] array = GameObject.FindGameObjectsWithTag("titan");
        foreach (GameObject gameObject in array)
        {
            if (gameObject.GetComponent<TITAN>() && gameObject.GetComponent<TITAN>().name == "Punk")
            {
                num++;
            }
        }
        return num;
    }

    private void grab(string type)
    {
        this.state = TitanState.Grad;
        this.attacked = false;
        this.isAlarm = true;
        this.attackAnimation = type;
        this.crossFade("grab_" + type, 0.1f);
        this.isGrabHandLeft = true;
        this.grabbedTarget = null;
        switch (type)
        {
            case "ground_back_l":
                this.attackCheckTimeA = 0.34f;
                this.attackCheckTimeB = 0.49f;
                break;

            case "ground_back_r":
                this.attackCheckTimeA = 0.34f;
                this.attackCheckTimeB = 0.49f;
                this.isGrabHandLeft = false;
                break;

            case "ground_front_l":
                this.attackCheckTimeA = 0.37f;
                this.attackCheckTimeB = 0.6f;
                break;

            case "ground_front_r":
                this.attackCheckTimeA = 0.37f;
                this.attackCheckTimeB = 0.6f;
                this.isGrabHandLeft = false;
                break;

            case "head_back_l":
                this.attackCheckTimeA = 0.45f;
                this.attackCheckTimeB = 0.5f;
                this.isGrabHandLeft = false;
                break;

            case "head_back_r":
                this.attackCheckTimeA = 0.45f;
                this.attackCheckTimeB = 0.5f;
                break;

            case "head_front_l":
                this.attackCheckTimeA = 0.38f;
                this.attackCheckTimeB = 0.55f;
                break;

            case "head_front_r":
                this.attackCheckTimeA = 0.38f;
                this.attackCheckTimeB = 0.55f;
                this.isGrabHandLeft = false;
                break;
        }
        if (this.isGrabHandLeft)
        {
            this.currentGrabHand = Hand_L_001;
        }
        else
        {
            this.currentGrabHand = Hand_R_001;
        }
    }

    private void hit(string animationName, Vector3 attacker, float hitPauseTime)
    {
        this.state = TitanState.Hit;
        this.hitAnimation = animationName;
        this.hitPause = hitPauseTime;
        this.playAnimation(this.hitAnimation);
        baseA[this.hitAnimation].time = 0f;
        baseA[this.hitAnimation].speed = 0f;
        baseT.rotation = Quaternion.Euler(0f, Quaternion.LookRotation(attacker - baseT.position).eulerAngles.y, 0f);
        this.needFreshCorePosition = true;
        if (BasePV.IsMine && this.grabbedTarget != null)
        {
            this.grabbedTarget.BasePV.RPC("netUngrabbed", PhotonTargets.All, new object[0]);
        }
    }

    [RPC]
    private void hitLRPC(Vector3 attacker, float hitPauseTime)
    {
        if (BasePV.IsMine)
        {
            float magnitude = (attacker - baseT.position).magnitude;
            if (magnitude < 80f)
            {
                this.hit("hit_eren_L", attacker, hitPauseTime);
            }
        }
    }

    [RPC]
    private void hitRRPC(Vector3 attacker, float hitPauseTime)
    {
        if (BasePV.IsMine)
        {
            if (this.hasDie)
            {
                return;
            }
            float magnitude = (attacker - baseT.position).magnitude;
            if (magnitude < 80f)
            {
                this.hit("hit_eren_R", attacker, hitPauseTime);
            }
        }
    }

    private void idle(float sbtime = 0f)
    {
        this.stuck = false;
        this.sbtime = sbtime;
        if (this.myDifficulty == 2 && (this.abnormalType == AbnormalType.Jumper || this.abnormalType == AbnormalType.Aberrant))
        {
            this.sbtime = UnityEngine.Random.Range(0f, 1.5f);
        }
        else if (this.myDifficulty >= 1)
        {
            this.sbtime = 0f;
        }
        this.sbtime = Mathf.Max(0.5f, this.sbtime);
        if (this.abnormalType == AbnormalType.Punk)
        {
            this.sbtime = 0.1f;
            if (this.myDifficulty == 1)
            {
                this.sbtime += 0.4f;
            }
        }
        this.state = TitanState.Idle;
        if (this.abnormalType == AbnormalType.Crawler)
        {
            this.crossFade("crawler_idle", 0.2f);
        }
        else
        {
            this.crossFade("idle", 0.2f);
        }
    }

    private void JustEatHero(HERO target, Transform hand)
    {
        if (target != null)
        {
            if (IN_GAME_MAIN_CAMERA.GameType == GameType.MultiPlayer && this.BasePV.IsMine)
            {
                if (!target.HasDied())
                {
                    target.markDie();
                    target.BasePV.RPC("netDie2", PhotonTargets.All, new object[]
                    {
                    nonAI ? BasePV.viewID : -1,
                    ShowName
                    });
                    return;
                }
            }
            else if (IN_GAME_MAIN_CAMERA.GameType == GameType.Single)
            {
                target.die2(hand);
            }
        }
    }

    private void justHitEye()
    {
        if (this.state != TitanState.Hit_Eye)
        {
            if (this.state == TitanState.Down || this.state == TitanState.Sit)
            {
                this.playAnimation("sit_hit_eye");
            }
            else
            {
                this.playAnimation("hit_eye");
            }
            this.state = TitanState.Hit_Eye;
        }
    }

    [RPC]
    private void laugh(float sbtime = 0f)
    {
        if (this.state == TitanState.Idle || this.state == TitanState.Turn || this.state == TitanState.Chase)
        {
            this.sbtime = sbtime;
            this.state = TitanState.Laugh;
            this.crossFade("laugh", 0.2f);
        }
    }

    [RPC]
    public void labelRPC(int health, int maxHealth)
    {
        if (health < 0)
        {
            if (healthLabel != null)
            {
                Destroy(this.healthLabel);
            }
        }
        else
        {
            if (healthLabel == null)
            {
                healthLabel = (GameObject)Instantiate(CacheResources.Load("UI/LabelNameOverHead"));
                healthLabel.name = "LabelNameOverHead";
                healthLabel.transform.parent = baseT;
                healthLabel.transform.localPosition = new Vector3(0f, 20f + 1f / this.myLevel, 0f);
                float num = 1f;
                if (myLevel < 1f)
                {
                    num = 1f / myLevel;
                }
                healthLabel.transform.localScale = new Vector3(num, num, num);
                healthLabel.GetComponent<UILabel>().text = string.Empty;
                TextMesh txt = healthLabel.GetComponent<TextMesh>();
                if (txt == null)
                {
                    txt = healthLabel.AddComponent<TextMesh>();
                }
                MeshRenderer render = healthLabel.GetComponent<MeshRenderer>();
                if (render == null)
                {
                    render = healthLabel.AddComponent<MeshRenderer>();
                }
                render.material = Labels.Font.material;
                txt.font = Labels.Font;
                txt.fontSize = 20;
                txt.anchor = TextAnchor.MiddleCenter;
                txt.alignment = TextAlignment.Center;
                txt.color = Colors.white;
                txt.text = healthLabel.GetComponent<UILabel>().text;
                txt.richText = true;
                txt.gameObject.layer = 5;
                if (abnormalType == AbnormalType.Crawler)
                {
                    healthLabel.transform.localPosition = new Vector3(0f, 10f + 1f / this.myLevel, 0f);
                }
                healthLabelEnabled = true;
            }
            string str = "[7FFF00]";
            float num2 = (float)health / (float)maxHealth;
            if (num2 < 0.75f && num2 >= 0.5f)
            {
                str = "[f2b50f]";
            }
            else if (num2 < 0.5f && num2 >= 0.25f)
            {
                str = "[ff8100]";
            }
            else if (num2 < 0.25f)
            {
                str = "[ff3333]";
            }
            this.healthLabel.GetComponent<TextMesh>().text = (str + Convert.ToString(health)).ToHTMLFormat();
        }
    }

    [RPC]
    private void loadskinRPC(string body, string eye, PhotonMessageInfo info = null)
    {
        if(SkinSettings.TitanSkins != 1)
        {
            return;
        }
        if(Skin != null)
        {
            Debug.Log($"Trying to change TITAN.Skin for viewID {BasePV.viewID} by ID {(info == null ? "-1" : info.Sender.ID.ToString())}");
            return;
        }
        //Put antis there
        StartCoroutine(loadskinRPCE(body, eye));
    }

    private IEnumerator loadskinRPCE(string body, string eye)
    {
        while (!spawned)
        {
            yield return null;
        }
        Skin = new TitanSkin(this, body, eye);
        Anarchy.Skins.Skin.Check(Skin, new string[] { body, eye });
    }

    private bool longRangeAttackCheck()
    {
        if (this.abnormalType != AbnormalType.Punk)
        {
            return false;
        }
        if (this.myHero != null && this.myHero.rigidbody != null)
        {
            Vector3 vector = this.myHero.rigidbody.velocity * Time.deltaTime * 30f;
            if (vector.sqrMagnitude > 10f)
            {
                if (this.simpleHitTestLineAndBall(vector, chkAeLeft.position - this.myHero.transform.position, 5f * this.myLevel))
                {
                    this.attack("anti_AE_l");
                    return true;
                }
                if (this.simpleHitTestLineAndBall(vector, chkAeLLeft.position - this.myHero.transform.position, 5f * this.myLevel))
                {
                    this.attack("anti_AE_low_l");
                    return true;
                }
                if (this.simpleHitTestLineAndBall(vector, chkAeRight.position - this.myHero.transform.position, 5f * this.myLevel))
                {
                    this.attack("anti_AE_r");
                    return true;
                }
                if (this.simpleHitTestLineAndBall(vector, chkAeLRight.position - this.myHero.transform.position, 5f * this.myLevel))
                {
                    this.attack("anti_AE_low_r");
                    return true;
                }
            }
            Vector3 vector2 = this.myHero.transform.position - baseT.position;
            float current = -Mathf.Atan2(vector2.z, vector2.x) * 57.29578f;
            float f = -Mathf.DeltaAngle(current, baseGT.rotation.eulerAngles.y - 90f);
            if (this.rockInterval > 0f)
            {
                this.rockInterval -= Time.deltaTime;
            }
            else if (Mathf.Abs(f) < 5f)
            {
                Vector3 a = this.myHero.transform.position + vector;
                float sqrMagnitude = (a - baseT.position).sqrMagnitude;
                if (sqrMagnitude > 8000f && sqrMagnitude < 90000f && !GameModes.NoRocks.Enabled)
                {
                    this.attack("throw");
                    this.rockInterval = 2f;
                    return true;
                }
            }
        }
        return false;
    }

    [RPC]
    private void netCrossFade(string aniName, float time)
    {
        baseA.CrossFade(aniName, time);
    }

    [RPC]
    private void netDie()
    {
        this.asClientLookTarget = false;
        if (this.hasDie)
        {
            return;
        }
        this.hasDie = true;
        if (this.nonAI)
        {
            IN_GAME_MAIN_CAMERA.MainCamera.SetMainObject(null, true, false);
            IN_GAME_MAIN_CAMERA.MainCamera.setSpectorMode(true);
            IN_GAME_MAIN_CAMERA.MainCamera.gameOver = true;
            PhotonNetwork.player.SetCustomProperties(new ExitGames.Client.Photon.Hashtable
            {
                {
                    PhotonPlayerProperty.dead,
                    true
                }
            });
            PhotonNetwork.player.SetCustomProperties(new ExitGames.Client.Photon.Hashtable
            {
                {
                    PhotonPlayerProperty.deaths,
                    (int)PhotonNetwork.player.Properties[PhotonPlayerProperty.deaths] + 1
                }
            });
        }
        this.dieAnimation();
    }

    [RPC]
    private void netPlayAnimation(string aniName, PhotonMessageInfo info = null)
    {
        if (!Antis.Protection.TitanAnimationCheck.Check(aniName))
        {
            if (info != null)
            {
                Anarchy.Network.Antis.Kick(info.Sender, true, "Invalid TITAN anim: " + aniName);
            }
            return;
        }
        baseA.Play(aniName);
    }

    [RPC]
    private void netPlayAnimationAt(string aniName, float normalizedTime, PhotonMessageInfo info = null)
    {
        if (!Antis.Protection.TitanAnimationCheck.Check(aniName))
        {
            if (info != null)
            {
                Anarchy.Network.Antis.Kick(info.Sender, true, "Invalid TITAN anim: " + aniName);
            }
            return;
        }
        baseA.Play(aniName);
        baseA[aniName].normalizedTime = normalizedTime;
    }

    [RPC]
    private void netSetAbnormalType(int type)
    {
        if (type == 0)
        {
            this.abnormalType = AbnormalType.Normal;
            this.runAnimation = "run_walk";
            base.GetComponent<TITAN_SETUP>().setHair();
        }
        else if (type == 1)
        {
            this.abnormalType = AbnormalType.Aberrant;
            this.runAnimation = "run_abnormal";
            base.GetComponent<TITAN_SETUP>().setHair();
        }
        else if (type == 2)
        {
            this.abnormalType = AbnormalType.Jumper;
            this.runAnimation = "run_abnormal";
            base.GetComponent<TITAN_SETUP>().setHair();
        }
        else if (type == 3)
        {
            this.abnormalType = AbnormalType.Crawler;
            this.runAnimation = "crawler_run";
            base.GetComponent<TITAN_SETUP>().setHair();
        }
        else if (type == 4)
        {
            this.abnormalType = AbnormalType.Punk;
            this.runAnimation = "run_abnormal_1";
            base.GetComponent<TITAN_SETUP>().setPunkHair();
        }
        base.name = titanNames[(int)abnormalType];
        ShowName = User.TitanNames[(int)abnormalType].PickRandomString();
        if (this.abnormalType == AbnormalType.Aberrant || this.abnormalType == AbnormalType.Jumper || this.abnormalType == AbnormalType.Punk)
        {
            this.speed = 18f;
            if (this.myLevel > 1f)
            {
                this.speed *= Mathf.Sqrt(this.myLevel);
            }
            if (this.myDifficulty == 1)
            {
                this.speed *= 1.4f;
            }
            if (this.myDifficulty == 2)
            {
                this.speed *= 1.6f;
            }
            baseA["turnaround1"].speed = 2f;
            baseA["turnaround2"].speed = 2f;
        }
        if (this.abnormalType == AbnormalType.Crawler)
        {
            this.chaseDistance += 50f;
            this.speed = 25f;
            if (this.myLevel > 1f)
            {
                this.speed *= Mathf.Sqrt(this.myLevel);
            }
            if (this.myDifficulty == 1)
            {
                this.speed *= 2f;
            }
            if (this.myDifficulty == 2)
            {
                this.speed *= 2.2f;
            }
            AABB.GetComponent<CapsuleCollider>().height = 10f;
            AABB.GetComponent<CapsuleCollider>().radius = 5f;
            AABB.GetComponent<CapsuleCollider>().center = new Vector3(0f, 5.05f, 0f);
        }
        if (this.nonAI)
        {
            if (this.abnormalType == AbnormalType.Crawler)
            {
                this.speed = Mathf.Min(70f, this.speed);
            }
            else
            {
                this.speed = Mathf.Min(60f, this.speed);
            }
            baseA["attack_jumper_0"].speed = 7f;
            baseA["attack_crawler_jump_0"].speed = 4f;
        }
        baseA["attack_combo_1"].speed = 1f;
        baseA["attack_combo_2"].speed = 1f;
        baseA["attack_combo_3"].speed = 1f;
        baseA["attack_quick_turn_l"].speed = 1f;
        baseA["attack_quick_turn_r"].speed = 1f;
        baseA["attack_anti_AE_l"].speed = 1.1f;
        baseA["attack_anti_AE_low_l"].speed = 1.1f;
        baseA["attack_anti_AE_r"].speed = 1.1f;
        baseA["attack_anti_AE_low_r"].speed = 1.1f;
        this.idle(0f);
    }

    [RPC]
    private void netSetLevel(float level, int AI, int skinColor)
    {
        this.setLevel(level, AI, skinColor);
    }

    private void OnCollisionStay()
    {
        this.grounded = true;
    }

    private void OnDestroy()
    {
        if (FengGameManagerMKII.FGM != null)
        {
            FengGameManagerMKII.FGM.RemoveTitan(this);
        }
    }

    private void playAnimation(string aniName)
    {
        baseA.Play(aniName);
        if (IN_GAME_MAIN_CAMERA.GameType == GameType.MultiPlayer && BasePV.IsMine)
        {
            BasePV.RPC("netPlayAnimation", PhotonTargets.Others, new object[]
            {
                aniName
            });
        }
    }

    private void playAnimationAt(string aniName, float normalizedTime)
    {
        baseA.Play(aniName);
        baseA[aniName].normalizedTime = normalizedTime;
        if (IN_GAME_MAIN_CAMERA.GameType == GameType.MultiPlayer && BasePV.IsMine)
        {
            BasePV.RPC("netPlayAnimationAt", PhotonTargets.Others, new object[]
            {
                aniName,
                normalizedTime
            });
        }
    }

    private void playSound(string sndname)
    {
        this.playsoundRPC(sndname);
        if (BasePV.IsMine)
        {
            BasePV.RPC("playsoundRPC", PhotonTargets.Others, new object[]
            {
                sndname
            });
        }
    }

    [RPC]
    private void playsoundRPC(string sndname)
    {
        Transform transform = baseT.Find(sndname);
        transform.GetComponent<AudioSource>().Play();
    }

    private void recover()
    {
        this.state = TitanState.Recover;
        this.playAnimation("idle_recovery");
        this.getdownTime = UnityEngine.Random.Range(2f, 5f);
    }

    private void remainSitdown()
    {
        this.state = TitanState.Sit;
        this.playAnimation("sit_idle");
        this.getdownTime = UnityEngine.Random.Range(10f, 30f);
    }

    [RPC]
    private void setIfLookTarget(bool bo)
    {
        this.asClientLookTarget = bo;
    }

    private void setLevel(float level, int AI, int skinColor)
    {
        this.myLevel = level;
        this.myLevel = Mathf.Clamp(this.myLevel, 0.1f, 50f);
        this.attackWait += UnityEngine.Random.Range(0f, 2f);
        this.chaseDistance += this.myLevel * 10f;
        baseT.localScale = new Vector3(this.myLevel, this.myLevel, this.myLevel);
        float num = Mathf.Pow(2f / this.myLevel, 0.35f);
        num = Mathf.Min(num, 1.25f);
        this.headscale = new Vector3(num, num, num);
        this.Head.localScale = this.headscale;
        if (skinColor != 0)
        {
            this.mainMaterial.GetComponent<SkinnedMeshRenderer>().material.color = ((skinColor != 1) ? ((skinColor != 2) ? FengColor.titanSkin3 : FengColor.titanSkin2) : FengColor.titanSkin1);
        }
        float num2 = 1.4f - (this.myLevel - 0.7f) * 0.15f;
        num2 = Mathf.Clamp(num2, 0.9f, 1.5f);
        foreach (object obj in baseA)
        {
            AnimationState animationState = (AnimationState)obj;
            animationState.speed = num2;
        }
        baseR.mass *= this.myLevel;
        baseR.rotation = Quaternion.Euler(0f, (float)UnityEngine.Random.Range(0, 360), 0f);
        if (this.myLevel > 1f)
        {
            this.speed *= Mathf.Sqrt(this.myLevel);
        }
        this.myDifficulty = AI;
        if (this.myDifficulty == 1 || this.myDifficulty == 2)
        {
            foreach (object obj2 in baseA)
            {
                AnimationState animationState2 = (AnimationState)obj2;
                animationState2.speed = num2 * 1.05f;
            }
            if (this.nonAI)
            {
                this.speed *= 1.1f;
            }
            else
            {
                this.speed *= 1.4f;
            }
            this.chaseDistance *= 1.15f;
        }
        if (this.myDifficulty == 2)
        {
            foreach (object obj3 in baseA)
            {
                AnimationState animationState3 = (AnimationState)obj3;
                animationState3.speed = num2 * 1.05f;
            }
            if (this.nonAI)
            {
                this.speed *= 1.1f;
            }
            else
            {
                this.speed *= 1.5f;
            }
            this.chaseDistance *= 1.3f;
        }
        if (IN_GAME_MAIN_CAMERA.GameMode == GameMode.ENDLESS_TITAN || IN_GAME_MAIN_CAMERA.GameMode == GameMode.SURVIVE_MODE)
        {
            this.chaseDistance = 999999f;
        }
        if (this.nonAI)
        {
            if (this.abnormalType == AbnormalType.Crawler)
            {
                this.speed = Mathf.Min(70f, this.speed);
            }
            else
            {
                this.speed = Mathf.Min(60f, this.speed);
            }
        }
        this.attackDistance = Vector3.Distance(baseT.position, ap_front_ground.position) * 1.65f;
    }

    private void setmyLevel()
    {
        baseA.cullingType = AnimationCullingType.BasedOnRenderers;
        if (IN_GAME_MAIN_CAMERA.GameType != GameType.Single && BasePV.IsMine)
        {
            BasePV.RPC("netSetLevel", PhotonTargets.AllBuffered, new object[]
            {
                this.myLevel,
                FengGameManagerMKII.FGM.Difficulty,
                UnityEngine.Random.Range(0, 4)
            });
            baseA.cullingType = AnimationCullingType.AlwaysAnimate;
        }
        else if (IN_GAME_MAIN_CAMERA.GameType == GameType.Single)
        {
            this.setLevel(this.myLevel, IN_GAME_MAIN_CAMERA.Difficulty, UnityEngine.Random.Range(0, 4));
        }
    }

    [RPC]
    private void setMyTarget(int ID)
    {
        if (ID == -1)
        {
            this.myHero = null;
        }
        PhotonView photonView = PhotonView.Find(ID);
        if (photonView != null)
        {
            this.myHero = photonView.gameObject;
        }
    }

    private bool simpleHitTestLineAndBall(Vector3 line, Vector3 ball, float R)
    {
        Vector3 vector = Vector3.Project(ball, line);
        return (ball - vector).magnitude <= R && Vector3.Dot(line, vector) >= 0f && vector.sqrMagnitude <= line.sqrMagnitude;
    }

    private void sitdown()
    {
        this.state = TitanState.Sit;
        this.playAnimation("sit_down");
        this.getdownTime = UnityEngine.Random.Range(10f, 30f);
    }

    private void Start()
    {
        FengGameManagerMKII.FGM.AddTitan(this);
        Minimap.TrackGameObjectOnMinimap(base.gameObject, Color.yellow, false, true, Minimap.IconStyle.Circle);
        this.runAnimation = "run_walk";
        this.grabTF = new GameObject();
        this.grabTF.name = "titansTmpGrabTF";
        this.oldHeadRotation = this.Head.rotation;
        if (IN_GAME_MAIN_CAMERA.GameType == GameType.MultiPlayer && !BasePV.IsMine)
        {
            return;
        }
        if (!hasSetLevel)
        {
            this.myLevel = UnityEngine.Random.Range(0.7f, 3f);
            if (GameModes.SizeMode.Enabled)
            {
                myLevel = UnityEngine.Random.Range(GameModes.SizeMode.GetFloat(0), GameModes.SizeMode.GetFloat(1));
            }
            hasSetLevel = true;
        }
        this.spawnPt = baseT.position;
        this.setmyLevel();
        spawned = true;
        this.setAbnormalType(this.abnormalType, false);
        if (this.myHero == null)
        {
            this.FindNearestHero();
        }
        if (maxHealth == 0 && GameModes.HealthMode.Enabled)
        {
            int healthLower = GameModes.HealthMode.GetInt(0);
            int healthUpper = GameModes.HealthMode.GetInt(1) + 1;
            if (GameModes.HealthMode.Selection == 1)
            {
                maxHealth = (this.currentHealth = UnityEngine.Random.Range(healthLower, healthUpper));
            }
            else if(GameModes.HealthMode.Selection == 2)
            {
                maxHealth = this.currentHealth = Mathf.Clamp(Mathf.RoundToInt(this.myLevel / 4f * (float)UnityEngine.Random.Range(healthLower, healthUpper)), healthLower, healthUpper);
                Anarchy.UI.Log.AddLineRaw(maxHealth.ToString());
            }
        }
        this.lagMax = 150f + this.myLevel * 3f;
        this.healthTime = Time.time;
        if(currentHealth > 0 && IN_GAME_MAIN_CAMERA.GameType == GameType.MultiPlayer)
        {
            BasePV.RPC("labelRPC", PhotonTargets.AllBuffered, new object[] { currentHealth, maxHealth });
        }
        hasExplode = false;
        ColliderEnabled = true;
        IsHooked = false;
        IsLook = false;
    }

    private void turn(float d)
    {
        if (this.abnormalType == AbnormalType.Crawler)
        {
            if (d > 0f)
            {
                this.turnAnimation = "crawler_turnaround_R";
            }
            else
            {
                this.turnAnimation = "crawler_turnaround_L";
            }
        }
        else if (d > 0f)
        {
            this.turnAnimation = "turnaround2";
        }
        else
        {
            this.turnAnimation = "turnaround1";
        }
        this.playAnimation(this.turnAnimation);
        baseA[this.turnAnimation].time = 0f;
        d = Mathf.Clamp(d, -120f, 120f);
        this.turnDeg = d;
        this.desDeg = baseGT.rotation.eulerAngles.y + this.turnDeg;
        this.state = TitanState.Turn;
    }

    private void wander(float sbtime = 0f)
    {
        this.state = TitanState.Wander;
        this.crossFade(this.runAnimation, 0.5f);
    }

    private void UpdateLabel()
    {
        if (this.healthLabel != null)
        {
            this.healthLabel.transform.LookAt(2f * this.healthLabel.transform.position - IN_GAME_MAIN_CAMERA.BaseT.position);
        }
    }

    public void beLaughAttacked()
    {
        if (this.hasDie)
        {
            return;
        }
        if (this.abnormalType == AbnormalType.Crawler)
        {
            return;
        }
        if (IN_GAME_MAIN_CAMERA.GameType == GameType.MultiPlayer)
        {
            BasePV.RPC("laugh", PhotonTargets.All, new object[]
            {
                0f
            });
        }
        else if (this.state == TitanState.Idle || this.state == TitanState.Turn || this.state == TitanState.Chase)
        {
            this.laugh(0f);
        }
    }

    public void beTauntedBy(GameObject target, float tauntTime)
    {
        this.whoHasTauntMe = target;
        this.tauntTime = tauntTime;
        this.isAlarm = true; 
        if (whoHasTauntMe != null && IN_GAME_MAIN_CAMERA.GameType == GameType.MultiPlayer && PhotonNetwork.IsMasterClient)
        {
            this.myHero = this.whoHasTauntMe;
            BasePV.RPC("setMyTarget", PhotonTargets.Others, new object[] { this.myHero.GetPhotonView().viewID });
        }
    }

    public bool die()
    {
        if (this.hasDie)
        {
            return false;
        }
        this.hasDie = true;
        FengGameManagerMKII.FGM.oneTitanDown(string.Empty, false);
        this.dieAnimation();
        return true;
    }

    public void dieBlow(Vector3 attacker, float hitPauseTime)
    {
        if (IN_GAME_MAIN_CAMERA.GameType == GameType.Single)
        {
            this.dieBlowFunc(attacker, hitPauseTime);
            if (GameObject.FindGameObjectsWithTag("titan").Length <= 1)
            {
                IN_GAME_MAIN_CAMERA.MainCamera.gameOver = true;
            }
        }
        else
        {
            BasePV.RPC("dieBlowRPC", PhotonTargets.All, new object[]
            {
                attacker,
                hitPauseTime
            });
        }
    }

    public void dieBlowFunc(Vector3 attacker, float hitPauseTime)
    {
        if (this.hasDie)
        {
            return;
        }
        baseT.rotation = Quaternion.Euler(0f, Quaternion.LookRotation(attacker - baseT.position).eulerAngles.y, 0f);
        this.hasDie = true;
        this.hitAnimation = "die_blow";
        this.hitPause = hitPauseTime;
        this.playAnimation(this.hitAnimation);
        baseA[this.hitAnimation].time = 0f;
        baseA[this.hitAnimation].speed = 0f;
        this.needFreshCorePosition = true;
        FengGameManagerMKII.FGM.oneTitanDown(string.Empty, false);
        if (BasePV.IsMine)
        {
            if (this.grabbedTarget != null)
            {
                this.grabbedTarget.BasePV.RPC("netUngrabbed", PhotonTargets.All, new object[0]);
            }
            if (this.nonAI)
            {
                IN_GAME_MAIN_CAMERA.MainCamera.SetMainObject(null, true, false);
                IN_GAME_MAIN_CAMERA.MainCamera.setSpectorMode(true);
                IN_GAME_MAIN_CAMERA.MainCamera.gameOver = true;
                PhotonNetwork.player.SetCustomProperties(new ExitGames.Client.Photon.Hashtable
                {
                    {
                        PhotonPlayerProperty.dead,
                        true
                    }
                });
                PhotonNetwork.player.SetCustomProperties(new ExitGames.Client.Photon.Hashtable
                {
                    {
                        PhotonPlayerProperty.deaths,
                        (int)PhotonNetwork.player.Properties[PhotonPlayerProperty.deaths] + 1
                    }
                });
            }
        }
    }

    [RPC]
    public void DieByCannon(int viewID)
    {
        PhotonView view = PhotonView.Find(viewID);
        if (view != null)
        {
            int damage = 0;
            if (PhotonNetwork.IsMasterClient)
            {
                this.OnTitanDie(view);
            }
            if (this.nonAI)
            {
                FengGameManagerMKII.FGM.TitanGetKill(view.owner, damage, PhotonNetwork.player.UIName);
            }
            else
            {
                FengGameManagerMKII.FGM.TitanGetKill(view.owner, damage, ShowName);
            }
        }
        else
        {
            FengGameManagerMKII.FGM.BasePV.RPC("netShowDamage", view.owner, new object[] { this.speed });
        }
    }

    public void dieHeadBlow(Vector3 attacker, float hitPauseTime)
    {
        if (this.abnormalType == AbnormalType.Crawler)
        {
            return;
        }
        if (IN_GAME_MAIN_CAMERA.GameType == GameType.Single)
        {
            this.dieHeadBlowFunc(attacker, hitPauseTime);
            if (GameObject.FindGameObjectsWithTag("titan").Length <= 1)
            {
                IN_GAME_MAIN_CAMERA.MainCamera.gameOver = true;
            }
        }
        else
        {
            BasePV.RPC("dieHeadBlowRPC", PhotonTargets.All, new object[]
            {
                attacker,
                hitPauseTime
            });
        }
    }

    public void dieHeadBlowFunc(Vector3 attacker, float hitPauseTime)
    {
        if (this.hasDie)
        {
            return;
        }
        this.playSound("snd_titan_head_blow");
        baseT.rotation = Quaternion.Euler(0f, Quaternion.LookRotation(attacker - baseT.position).eulerAngles.y, 0f);
        this.hasDie = true;
        this.hitAnimation = "die_headOff";
        this.hitPause = hitPauseTime;
        this.playAnimation(this.hitAnimation);
        baseA[this.hitAnimation].time = 0f;
        baseA[this.hitAnimation].speed = 0f;
        FengGameManagerMKII.FGM.oneTitanDown(string.Empty, false);
        this.needFreshCorePosition = true;
        GameObject gameObject;
        if (IN_GAME_MAIN_CAMERA.GameType == GameType.MultiPlayer && BasePV.IsMine)
        {
            gameObject = Optimization.Caching.Pool.NetworkEnable("bloodExplore", this.Head.position + Vectors.up * 1f * this.myLevel, Quaternion.Euler(270f, 0f, 0f), 0);
        }
        else
        {
            gameObject = Pool.Enable("bloodExplore", this.Head.position + Vectors.up * 1f * this.myLevel, Quaternion.Euler(270f, 0f, 0f));//(GameObject)UnityEngine.Object.Instantiate(CacheResources.Load("bloodExplore"), this.Head.position + Vectors.up * 1f * this.myLevel, Quaternion.Euler(270f, 0f, 0f));
        }
        gameObject.transform.localScale = baseT.localScale;
        if (IN_GAME_MAIN_CAMERA.GameType == GameType.MultiPlayer && BasePV.IsMine)
        {
            gameObject = Optimization.Caching.Pool.NetworkEnable("bloodsplatter", this.Head.position, Quaternion.Euler(270f + this.Neck.rotation.eulerAngles.x, this.Neck.rotation.eulerAngles.y, this.Neck.rotation.eulerAngles.z), 0);
        }
        else
        {
            gameObject = Pool.Enable("bloodsplatter", Head.position, Quaternion.Euler(270f + this.Neck.rotation.eulerAngles.x, this.Neck.rotation.eulerAngles.y, this.Neck.rotation.eulerAngles.z));//(GameObject)UnityEngine.Object.Instantiate(CacheResources.Load("bloodsplatter"), this.Head.position, Quaternion.Euler(270f + this.Neck.rotation.eulerAngles.x, this.Neck.rotation.eulerAngles.y, this.Neck.rotation.eulerAngles.z));
        }
        gameObject.transform.localScale = baseT.localScale;
        gameObject.transform.parent = this.Neck;
        if (IN_GAME_MAIN_CAMERA.GameType == GameType.MultiPlayer && BasePV.IsMine)
        {
            gameObject = Optimization.Caching.Pool.NetworkEnable("FX/justSmoke", this.Neck.position, Quaternion.Euler(270f, 0f, 0f), 0);
        }
        else
        {
            gameObject = Pool.Enable("FX/justSmoke", this.Neck.position, Quaternion.Euler(270f, 0f, 0f));//(GameObject)UnityEngine.Object.Instantiate(CacheResources.Load("FX/justSmoke"), this.Neck.position, Quaternion.Euler(270f, 0f, 0f));
        }
        gameObject.transform.parent = this.Neck;
        if (BasePV.IsMine)
        {
            if (this.grabbedTarget != null)
            {
                this.grabbedTarget.BasePV.RPC("netUngrabbed", PhotonTargets.All, new object[0]);
            }
            if (this.nonAI)
            {
                IN_GAME_MAIN_CAMERA.MainCamera.SetMainObject(null, true, false);
                IN_GAME_MAIN_CAMERA.MainCamera.setSpectorMode(true);
                IN_GAME_MAIN_CAMERA.MainCamera.gameOver = true;
                PhotonNetwork.player.SetCustomProperties(new ExitGames.Client.Photon.Hashtable
                {
                    {
                        PhotonPlayerProperty.dead,
                        true
                    }
                });
                PhotonNetwork.player.SetCustomProperties(new ExitGames.Client.Photon.Hashtable
                {
                    {
                        PhotonPlayerProperty.deaths,
                        (int)PhotonNetwork.player.Properties[PhotonPlayerProperty.deaths] + 1
                    }
                });
            }
        }
    }

    [RPC]
    public void grabbedTargetEscape()
    {
        this.grabbedTarget = null;
    }

    [RPC]
    public void grabToLeft()
    {
        this.grabTF.transform.parent = Hand_L_001;
        this.grabTF.transform.position = Hand_L_001SphereT.position;
        this.grabTF.transform.rotation = Hand_L_001SphereT.rotation;
        this.grabTF.transform.localPosition -= Vectors.right * Hand_L_001Sphere.radius * 0.3f;
        this.grabTF.transform.localPosition -= Vectors.up * Hand_L_001Sphere.radius * 0.51f;
        this.grabTF.transform.localPosition -= Vectors.forward * Hand_L_001Sphere.radius * 0.3f;
        this.grabTF.transform.localRotation = Quaternion.Euler(this.grabTF.transform.localRotation.eulerAngles.x, this.grabTF.transform.localRotation.eulerAngles.y + 180f, this.grabTF.transform.localRotation.eulerAngles.z + 180f);
    }

    [RPC]
    public void grabToRight()
    {
        this.grabTF.transform.parent = Hand_R_001;
        this.grabTF.transform.position = Hand_R_001SphereT.position;
        this.grabTF.transform.rotation = Hand_R_001SphereT.rotation;
        this.grabTF.transform.localPosition -= Vectors.right * Hand_R_001Sphere.radius * 0.3f;
        this.grabTF.transform.localPosition += Vectors.up * Hand_R_001Sphere.radius * 0.51f;
        this.grabTF.transform.localPosition -= Vectors.forward * Hand_R_001Sphere.radius * 0.3f;
        this.grabTF.transform.localRotation = Quaternion.Euler(this.grabTF.transform.localRotation.eulerAngles.x, this.grabTF.transform.localRotation.eulerAngles.y + 180f, this.grabTF.transform.localRotation.eulerAngles.z);
    }

    public void headMovement()
    {
        if (!hasDie)
        {
            if (IN_GAME_MAIN_CAMERA.GameType != GameType.Single)
            {
                bool isMine = BasePV.IsMine;
                if (isMine)
                {
                    this.targetHeadRotation = this.Head.rotation;
                    bool flag2 = false;
                    bool flag6 = this.abnormalType != AbnormalType.Crawler && this.state != TitanState.Attack && this.state != TitanState.Down && this.state != TitanState.Hit && this.state != TitanState.Recover && this.state != TitanState.Eat && this.state != TitanState.Hit_Eye && !this.hasDie && this.myDistance < 100f && this.myHero != null;
                    if (flag6)
                    {
                        Vector3 vector = this.myHero.transform.position - baseT.position;
                        this.angle = -Mathf.Atan2(vector.z, vector.x) * 57.29578f;
                        float num = -Mathf.DeltaAngle(this.angle, baseT.rotation.eulerAngles.y - 90f);
                        num = Mathf.Clamp(num, -40f, 40f);
                        float y = this.Neck.position.y + this.myLevel * 2f - this.myHero.transform.position.y;
                        float num2 = Mathf.Atan2(y, this.myDistance) * 57.29578f;
                        num2 = Mathf.Clamp(num2, -40f, 30f);
                        this.targetHeadRotation = Quaternion.Euler(this.Head.rotation.eulerAngles.x + num2, this.Head.rotation.eulerAngles.y + num, this.Head.rotation.eulerAngles.z);
                        bool flag7 = !this.asClientLookTarget;
                        if (flag7)
                        {
                            this.asClientLookTarget = true;
                            object[] parameters = new object[]
                            {
                                true
                            };
                            BasePV.RPC("setIfLookTarget", PhotonTargets.Others, parameters);
                        }
                        flag2 = true;
                    }
                    bool flag8 = !flag2 && this.asClientLookTarget;
                    if (flag8)
                    {
                        this.asClientLookTarget = false;
                        object[] objArray3 = new object[]
                        {
                            false
                        };
                        BasePV.RPC("setIfLookTarget", PhotonTargets.Others, objArray3);
                    }
                    bool flag9 = this.state == TitanState.Attack || this.state == TitanState.Hit || this.state == TitanState.Hit_Eye;
                    if (flag9)
                    {
                        this.oldHeadRotation = Quaternion.Lerp(this.oldHeadRotation, this.targetHeadRotation, Time.deltaTime * 20f);
                    }
                    else
                    {
                        this.oldHeadRotation = Quaternion.Lerp(this.oldHeadRotation, this.targetHeadRotation, Time.deltaTime * 10f);
                    }
                }
                else
                {
                    bool flag3 = this.myHero != null;
                    bool flag10 = flag3;
                    if (flag10)
                    {
                        this.myDistance = Mathf.Sqrt((this.myHero.transform.position.x - baseT.position.x) * (this.myHero.transform.position.x - baseT.position.x) + (this.myHero.transform.position.z - baseT.position.z) * (this.myHero.transform.position.z - baseT.position.z));
                    }
                    else
                    {
                        this.myDistance = float.MaxValue;
                    }
                    this.targetHeadRotation = this.Head.rotation;
                    bool flag11 = this.asClientLookTarget && flag3 && this.myDistance < 100f;
                    if (flag11)
                    {
                        Vector3 vector2 = this.myHero.transform.position - this.baseT.position;
                        this.angle = -Mathf.Atan2(vector2.z, vector2.x) * 57.29578f;
                        float num3 = -Mathf.DeltaAngle(this.angle, this.baseT.rotation.eulerAngles.y - 90f);
                        num3 = Mathf.Clamp(num3, -40f, 40f);
                        float num4 = this.Neck.position.y + this.myLevel * 2f - this.myHero.transform.position.y;
                        float num5 = Mathf.Atan2(num4, this.myDistance) * 57.29578f;
                        num5 = Mathf.Clamp(num5, -40f, 30f);
                        this.targetHeadRotation = Quaternion.Euler(this.Head.rotation.eulerAngles.x + num5, this.Head.rotation.eulerAngles.y + num3, this.Head.rotation.eulerAngles.z);
                    }
                    bool flag12 = !this.hasDie;
                    if (flag12)
                    {
                        this.oldHeadRotation = Quaternion.Slerp(this.oldHeadRotation, this.targetHeadRotation, Time.deltaTime * 10f);
                    }
                }
            }
            else
            {
                this.targetHeadRotation = this.Head.rotation;
                bool flag13 = this.abnormalType != AbnormalType.Crawler && this.state != TitanState.Attack && this.state != TitanState.Down && this.state != TitanState.Hit && this.state != TitanState.Recover && this.state != TitanState.Hit_Eye && !this.hasDie && this.myDistance < 100f && this.myHero != null;
                if (flag13)
                {
                    Vector3 vector3 = this.myHero.transform.position - baseT.position;
                    this.angle = -Mathf.Atan2(vector3.z, vector3.x) * 57.29578f;
                    float num6 = -Mathf.DeltaAngle(this.angle, baseT.rotation.eulerAngles.y - 90f);
                    num6 = Mathf.Clamp(num6, -40f, 40f);
                    float num7 = this.Neck.position.y + this.myLevel * 2f - this.myHero.transform.position.y;
                    float num8 = Mathf.Atan2(num7, this.myDistance) * 57.29578f;
                    num8 = Mathf.Clamp(num8, -40f, 30f);
                    this.targetHeadRotation = Quaternion.Euler(this.Head.rotation.eulerAngles.x + num8, this.Head.rotation.eulerAngles.y + num6, this.Head.rotation.eulerAngles.z);
                }
                bool flag14 = this.state == TitanState.Attack || this.state == TitanState.Hit || this.state == TitanState.Hit_Eye;
                if (flag14)
                {
                    this.oldHeadRotation = Quaternion.Lerp(this.oldHeadRotation, this.targetHeadRotation, Time.deltaTime * 20f);
                }
                else
                {
                    this.oldHeadRotation = Quaternion.Lerp(this.oldHeadRotation, this.targetHeadRotation, Time.deltaTime * 10f);
                }
            }
            this.Head.rotation = this.oldHeadRotation;
        }
        if (!baseA.IsPlaying("die_headOff"))
        {
            this.Head.localScale = this.headscale;
        }
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

    public void hitAnkle()
    {
        if (this.hasDie)
        {
            return;
        }
        if (this.state == TitanState.Down)
        {
            return;
        }
        if (this.grabbedTarget != null)
        {
            this.grabbedTarget.BasePV.RPC("netUngrabbed", PhotonTargets.All, new object[0]);
        }
        this.getDown();
    }

    [RPC]
    public void hitAnkleRPC(int viewID)
    {
        if (this.hasDie)
        {
            return;
        }
        if (this.state == TitanState.Down)
        {
            return;
        }
        PhotonView photonView = PhotonView.Find(viewID);
        if (photonView == null)
        {
            return;
        }
        float magnitude = (photonView.gameObject.transform.position - baseT.position).magnitude;
        if (magnitude < 20f)
        {
            if (BasePV.IsMine && this.grabbedTarget != null)
            {
                this.grabbedTarget.BasePV.RPC("netUngrabbed", PhotonTargets.All, new object[0]);
            }
            this.getDown();
        }
    }

    public void hitEye()
    {
        if (this.hasDie)
        {
            return;
        }
        this.justHitEye();
    }

    [RPC]
    public void hitEyeRPC(int viewID)
    {
        if (this.hasDie)
        {
            return;
        }
        float magnitude = (PhotonView.Find(viewID).gameObject.transform.position - this.Neck.position).magnitude;
        if (magnitude < 20f)
        {
            if (BasePV.IsMine && this.grabbedTarget != null)
            {
                this.grabbedTarget.BasePV.RPC("netUngrabbed", PhotonTargets.All, new object[0]);
            }
            if (!this.hasDie)
            {
                this.justHitEye();
            }
        }
    }

    public void hitL(Vector3 attacker, float hitPauseTime)
    {
        if (this.abnormalType == AbnormalType.Crawler)
        {
            return;
        }
        if (IN_GAME_MAIN_CAMERA.GameType == GameType.Single)
        {
            this.hit("hit_eren_L", attacker, hitPauseTime);
        }
        else
        {
            BasePV.RPC("hitLRPC", PhotonTargets.All, new object[]
            {
                attacker,
                hitPauseTime
            });
        }
    }

    public void hitR(Vector3 attacker, float hitPauseTime)
    {
        if (this.abnormalType == AbnormalType.Crawler)
        {
            return;
        }
        if (IN_GAME_MAIN_CAMERA.GameType == GameType.Single)
        {
            this.hit("hit_eren_R", attacker, hitPauseTime);
        }
        else
        {
            BasePV.RPC("hitRRPC", PhotonTargets.All, new object[]
            {
                attacker,
                hitPauseTime
            });
        }
    }

    public bool IsGrounded()
    {
        return Physics.Raycast(baseGT.position + Vectors.up * 0.1f, -Vectors.up, 0.3f, Layers.EnemyAABBGround.value);
    }

    public void lateUpdate()
    {
        if (IN_GAME_MAIN_CAMERA.isPausing && IN_GAME_MAIN_CAMERA.GameType == GameType.Single)
        {
            return;
        }
        if (base.baseA.IsPlaying("run_walk"))
        {
            if (base.baseA["run_walk"].normalizedTime % 1f > 0.1f && base.baseA["run_walk"].normalizedTime % 1f < 0.6f && this.stepSoundPhase == 2)
            {
                this.stepSoundPhase = 1;
                FootAudio.Stop();
                FootAudio.Play();
            }
            if (base.baseA["run_walk"].normalizedTime % 1f > 0.6f && this.stepSoundPhase == 1)
            {
                this.stepSoundPhase = 2;
                FootAudio.Stop();
                FootAudio.Play();
            }
        }
        if (base.baseA.IsPlaying("crawler_run"))
        {
            if (base.baseA["crawler_run"].normalizedTime % 1f > 0.1f && base.baseA["crawler_run"].normalizedTime % 1f < 0.56f && this.stepSoundPhase == 2)
            {
                this.stepSoundPhase = 1;
                FootAudio.Stop();
                FootAudio.Play();
            }
            if (base.baseA["crawler_run"].normalizedTime % 1f > 0.56f && this.stepSoundPhase == 1)
            {
                this.stepSoundPhase = 2;
                FootAudio.Stop();
                FootAudio.Play();
            }
        }
        if (base.baseA.IsPlaying("run_abnormal"))
        {
            if (base.baseA["run_abnormal"].normalizedTime % 1f > 0.47f && base.baseA["run_abnormal"].normalizedTime % 1f < 0.95f && this.stepSoundPhase == 2)
            {
                this.stepSoundPhase = 1;
                FootAudio.Stop();
                FootAudio.Play();
            }
            if ((base.baseA["run_abnormal"].normalizedTime % 1f > 0.95f || base.baseA["run_abnormal"].normalizedTime % 1f < 0.47f) && this.stepSoundPhase == 1)
            {
                this.stepSoundPhase = 2;
                FootAudio.Stop();
                FootAudio.Play();
            }
        }
        UpdateCollider();
        UpdateLabel();
        this.headMovement();
        this.grounded = false;
    }

    public void moveTo(float x, float y, float z)
    {
        this.baseT.position = new Vector3(x, y, z);
    }

    [RPC]
    public void moveToRPC(float x, float y, float z, PhotonMessageInfo info)
    {
        if (info.Sender.IsMasterClient)
        {
            this.baseT.position = new Vector3(x, y, z);
        }
    }

    public void OnTitanDie(PhotonView view)
    {
        if (CustomLevel.logicLoaded && RCManager.RCEvents.ContainsKey("OnTitanDie"))
        {
            RCEvent event2 = (RCEvent)RCManager.RCEvents["OnTitanDie"];
            string[] strArray = (string[])RCManager.RCVariableNames["OnTitanDie"];
            if (RCManager.titanVariables.ContainsKey(strArray[0]))
            {
                RCManager.titanVariables[strArray[0]] = this;
            }
            else
            {
                RCManager.titanVariables.Add(strArray[0], this);
            }
            if (RCManager.playerVariables.ContainsKey(strArray[1]))
            {
                RCManager.playerVariables[strArray[1]] = view.owner;
            }
            else
            {
                RCManager.playerVariables.Add(strArray[1], view.owner);
            }
            event2.checkEvent();
        }
    }

    public void randomRun(Vector3 targetPt, float r)
    {
        this.state = TitanState.Random_Run;
        this.targetCheckPt = targetPt;
        this.targetR = r;
        this.random_run_time = UnityEngine.Random.Range(1f, 2f);
        this.crossFade(this.runAnimation, 0.5f);
    }

    public void resetLevel(float level)
    {
        this.myLevel = level;
        this.setmyLevel();
    }

    public void setAbnormalType(AbnormalType type, bool forceCrawler = false)
    {
        int num = 0;
        float num2 = 0.02f * (float)(IN_GAME_MAIN_CAMERA.Difficulty + 1);
        if (IN_GAME_MAIN_CAMERA.GameMode == GameMode.PVP_AHSS)
        {
            num2 = 100f;
        }
        if (type == AbnormalType.Normal)
        {
            if (UnityEngine.Random.Range(0f, 1f) < num2)
            {
                num = 4;
            }
            else
            {
                num = 0;
            }
        }
        else if (type == AbnormalType.Aberrant)
        {
            if (UnityEngine.Random.Range(0f, 1f) < num2)
            {
                num = 4;
            }
            else
            {
                num = 1;
            }
        }
        else if (type == AbnormalType.Jumper)
        {
            if (UnityEngine.Random.Range(0f, 1f) < num2)
            {
                num = 4;
            }
            else
            {
                num = 2;
            }
        }
        else if (type == AbnormalType.Crawler)
        {
            num = 3;
            if (UnityEngine.Random.Range(0, 1000) > 5)
            {
                num = 2;
            }
        }
        else if (type == AbnormalType.Punk)
        {
            num = 4;
        }
        if (forceCrawler)
        {
            num = 3;
        }
        if (num == 4)
        {
            if (!FengGameManagerMKII.Level.PunksEnabled)
            {
                num = 1;
            }
            else
            {
                if (IN_GAME_MAIN_CAMERA.GameType == GameType.Single && this.getPunkNumber() >= 3)
                {
                    num = 1;
                }
                if (IN_GAME_MAIN_CAMERA.GameMode == GameMode.SURVIVE_MODE)
                {
                    int wave = FengGameManagerMKII.FGM.Logic.Round.Wave;
                    if (wave % 5 != 0)
                    {
                        num = 1;
                    }
                }
            }
        }
        if (GameModes.SpawnRate.Enabled && IN_GAME_MAIN_CAMERA.GameType == GameType.MultiPlayer && PhotonNetwork.IsMasterClient)
        {
            num = (int)type;
        }
        if (SkinSettings.SkinsCheck(SkinSettings.TitanSkins))
        {
            if (SkinSettings.TitanSet.Value != Anarchy.Configuration.StringSetting.NotDefine)
            {
                TitanSkinPreset set = new TitanSkinPreset(SkinSettings.TitanSet.Value);
                set.Load();
                int rnd = UnityEngine.Random.Range(0, 5);
                string body = set.Bodies[rnd];
                rnd = set.RandomizePairs ? UnityEngine.Random.Range(0, 5) : rnd;
                string eyes = set.Eyes[rnd];
                bool eye = false;
                if (eyes.EndsWith(".png") || eyes.EndsWith(".jpeg") || eyes.EndsWith(".jpg") || eyes.ToLower().Equals("transparent"))
                {
                    eye = true;
                }
                GetComponent<TITAN_SETUP>().SetVar(rnd, eye);
                StartCoroutine(loadskinRPCE(body, eyes));
                if (IN_GAME_MAIN_CAMERA.GameType == GameType.MultiPlayer && PhotonNetwork.IsMasterClient && SkinSettings.TitanSkins.Value != 2)
                {
                    BasePV.RPC("loadskinRPC", PhotonTargets.OthersBuffered, new object[] { body, eyes });
                }
            }
        }
        if (IN_GAME_MAIN_CAMERA.GameType != GameType.Single && BasePV.IsMine)
        {
            BasePV.RPC("netSetAbnormalType", PhotonTargets.AllBuffered, new object[]
            {
                num
            });
        }
        else if (IN_GAME_MAIN_CAMERA.GameType == GameType.Single)
        {
            this.netSetAbnormalType(num);
        }
    }

    public void setRoute(GameObject route)
    {
        this.checkPoints = new ArrayList();
        for (int i = 1; i <= 10; i++)
        {
            this.checkPoints.Add(route.transform.Find("r" + i).position);
        }
        this.checkPoints.Add("end");
    }

    public void suicide()
    {
        this.netDie();
        if (this.nonAI)
        {
            FengGameManagerMKII.FGM.SendKillInfo(false, string.Empty, true, (string)PhotonNetwork.player.Properties[PhotonPlayerProperty.name], 0);
        }
        FengGameManagerMKII.FGM.NeedChooseSide = true;
        FengGameManagerMKII.FGM.JustSuicide = true;
    }

    [RPC]
    public void titanGetHit(int viewID, int speed)
    {
        PhotonView photonView = PhotonView.Find(viewID);
        if (photonView == null)
        {
            return;
        }
        float magnitude = (photonView.gameObject.transform.position - this.Neck.position).magnitude;
        if (magnitude < lagMax && !this.hasDie && Time.time - this.healthTime > 0.2f)
        {
            this.healthTime = Time.time;
            if (GameModes.DamageMode.Enabled && speed < GameModes.DamageMode.GetInt(0))
            {
                FengGameManagerMKII.FGM.BasePV.RPC("netShowDamage", photonView.owner, new object[] { speed });
                if (maxHealth > 0)
                {
                    BasePV.RPC("labelRPC", PhotonTargets.AllBuffered, new object[] { currentHealth, maxHealth });
                }
                return;
            }
            currentHealth -= speed;
            if (maxHealth > 0)
            {
                BasePV.RPC("labelRPC", PhotonTargets.AllBuffered, new object[] { currentHealth, maxHealth });
            }
            if (currentHealth <= 0)
            {
                OnTitanDie(photonView);
                BasePV.RPC("netDie", PhotonTargets.OthersBuffered, new object[0]);
                if (this.grabbedTarget != null)
                {
                    this.grabbedTarget.BasePV.RPC("netUngrabbed", PhotonTargets.All, new object[0]);
                }
                this.netDie();
                if (this.nonAI)
                {
                    FengGameManagerMKII.FGM.TitanGetKill(photonView.owner, speed, (string)PhotonNetwork.player.Properties[PhotonPlayerProperty.name]);
                }
                else
                {
                    FengGameManagerMKII.FGM.TitanGetKill(photonView.owner, speed, ShowName);
                }
            }
            else
            {
                FengGameManagerMKII.FGM.BasePV.RPC("netShowDamage", photonView.owner, new object[] { speed });
            }
        }
    }

    public void toCheckPoint(Vector3 targetPt, float r)
    {
        this.state = TitanState.To_CheckPoint;
        this.targetCheckPt = targetPt;
        this.targetR = r;
        this.crossFade(this.runAnimation, 0.5f);
    }

    public void toPVPCheckPoint(Vector3 targetPt, float r)
    {
        this.state = TitanState.To_PVP_PT;
        this.targetCheckPt = targetPt;
        this.targetR = r;
        this.crossFade(this.runAnimation, 0.5f);
    }

    public void update()
    {
        if (IN_GAME_MAIN_CAMERA.isPausing && IN_GAME_MAIN_CAMERA.GameType == GameType.Single)
        {
            return;
        }
        if (this.myDifficulty < 0)
        {
            return;
        }
        if (IN_GAME_MAIN_CAMERA.GameType != GameType.Single)
        {
            if (!BasePV.IsMine)
            {
                return;
            }
        }
        float dt = Time.deltaTime;
        Explode();
        if (!this.nonAI)
        {
            if (this.activeRad < 2147483647 && (this.state == TitanState.Idle || this.state == TitanState.Wander || this.state == TitanState.Chase))
            {
                if (this.checkPoints.Count > 1)
                {
                    if (Vector3.Distance((Vector3)this.checkPoints[0], baseT.position) > (float)this.activeRad)
                    {
                        this.toCheckPoint((Vector3)this.checkPoints[0], 10f);
                    }
                }
                else if (Vector3.Distance(this.spawnPt, baseT.position) > (float)this.activeRad)
                {
                    this.toCheckPoint(this.spawnPt, 10f);
                }
            }
            if (this.whoHasTauntMe != null)
            {
                this.tauntTime -= dt;
                if (this.tauntTime <= 0f)
                {
                    this.whoHasTauntMe = null;
                    BasePV.RPC("setMyTarget", PhotonTargets.Others, new object[] { -1 });
                }
            }
        }
        if (!this.hasDie)
        {
            if (this.state == TitanState.Hit)
            {
                if (this.hitPause > 0f)
                {
                    this.hitPause -= dt;
                    if (this.hitPause <= 0f)
                    {
                        baseA[this.hitAnimation].speed = 1f;
                        this.hitPause = 0f;
                    }
                }
                if (baseA[this.hitAnimation].normalizedTime >= 1f)
                {
                    this.idle(0f);
                }
            }
            if (!this.nonAI)
            {
                if (this.myHero == null)
                {
                    this.FindNearestHero();
                }
                if ((this.state == TitanState.Idle || this.state == TitanState.Chase || this.state == TitanState.Wander) && this.whoHasTauntMe == null && UnityEngine.Random.Range(0, 100) < 10)
                {
                    this.FindNearestFacingHero();
                }
                if (this.myHero == null)
                {
                    this.myDistance = float.MaxValue;
                }
                else
                {
                    this.myDistance = Mathf.Sqrt((this.myHero.transform.position.x - baseT.position.x) * (this.myHero.transform.position.x - baseT.position.x) + (this.myHero.transform.position.z - baseT.position.z) * (this.myHero.transform.position.z - baseT.position.z));
                }
            }
            else
            {
                if (this.stamina < this.maxStamina)
                {
                    if (baseA.IsPlaying("idle"))
                    {
                        this.stamina += dt * 30f;
                    }
                    if (baseA.IsPlaying("crawler_idle"))
                    {
                        this.stamina += dt * 35f;
                    }
                    if (baseA.IsPlaying("run_walk"))
                    {
                        this.stamina += dt * 10f;
                    }
                }
                if (baseA.IsPlaying("run_abnormal_1"))
                {
                    this.stamina -= dt * 5f;
                }
                if (baseA.IsPlaying("crawler_run"))
                {
                    this.stamina -= dt * 15f;
                }
                if (this.stamina < 0f)
                {
                    this.stamina = 0f;
                }
                if (!IN_GAME_MAIN_CAMERA.isPausing)
                {
                    CacheGameObject.Find("stamina_titan").transform.localScale = new Vector3(this.stamina, 16f);
                }
            }
            switch (state)
            {
                case TitanState.Laugh:
                    if (baseA["laugh"].normalizedTime >= 1f)
                    {
                        this.idle(2f);
                        return;
                    }
                    break;

                case TitanState.Idle:
                    if (this.nonAI)
                    {
                        if (IN_GAME_MAIN_CAMERA.isPausing)
                        {
                            return;
                        }
                        pt();
                        if (this.abnormalType != AbnormalType.Crawler)
                        {
                            if (this.controller.isAttackDown && this.stamina > 25f)
                            {
                                this.stamina -= 25f;
                                this.attack("combo_1");
                            }
                            else if (this.controller.isAttackIIDown && this.stamina > 50f)
                            {
                                this.stamina -= 50f;
                                this.attack("abnormal_jump");
                            }
                            else if (this.controller.isJumpDown && this.stamina > 15f)
                            {
                                this.stamina -= 15f;
                                this.attack("jumper_0");
                            }
                        }
                        else if (this.controller.isAttackDown && this.stamina > 40f)
                        {
                            this.stamina -= 40f;
                            this.attack("crawler_jump_0");
                        }
                        if (this.controller.isSuicide)
                        {
                            this.suicide();
                        }
                        return;
                    }
                    else
                    {
                        if (this.sbtime > 0f)
                        {
                            this.sbtime -= dt;
                            return;
                        }
                        if (!this.isAlarm)
                        {
                            if (this.abnormalType != AbnormalType.Punk && this.abnormalType != AbnormalType.Crawler && UnityEngine.Random.Range(0f, 1f) < 0.005f)
                            {
                                this.sitdown();
                                return;
                            }
                            if (UnityEngine.Random.Range(0f, 1f) < 0.02f)
                            {
                                this.wander(0f);
                                return;
                            }
                            if (UnityEngine.Random.Range(0f, 1f) < 0.01f)
                            {
                                this.turn((float)UnityEngine.Random.Range(30, 120));
                                return;
                            }
                            if (UnityEngine.Random.Range(0f, 1f) < 0.01f)
                            {
                                this.turn((float)UnityEngine.Random.Range(-30, -120));
                                return;
                            }
                        }
                        this.angle = 0f;
                        this.between2 = 0f;
                        if (this.myDistance < this.chaseDistance || this.whoHasTauntMe != null)
                        {
                            Vector3 vector = this.myHero.transform.position - baseT.position;
                            this.angle = -Mathf.Atan2(vector.z, vector.x) * 57.29578f;
                            this.between2 = -Mathf.DeltaAngle(this.angle, baseGT.rotation.eulerAngles.y - 90f);
                            if (this.myDistance >= this.attackDistance)
                            {
                                if (this.isAlarm || Mathf.Abs(this.between2) < 90f)
                                {
                                    this.chase();
                                    return;
                                }
                                if (!this.isAlarm && this.myDistance < this.chaseDistance * 0.1f)
                                {
                                    this.chase();
                                    return;
                                }
                            }
                        }
                        if (this.longRangeAttackCheck())
                        {
                            return;
                        }
                        if (this.myDistance < this.chaseDistance)
                        {
                            if (this.abnormalType == AbnormalType.Jumper && (this.myDistance > this.attackDistance || this.myHero.transform.position.y > this.Head.position.y + 4f * this.myLevel) && Mathf.Abs(this.between2) < 120f && Vector3.Distance(baseT.position, this.myHero.transform.position) < 1.5f * this.myHero.transform.position.y)
                            {
                                this.attack("jumper_0");
                                return;
                            }
                            if (this.abnormalType == AbnormalType.Crawler && this.myDistance < this.attackDistance * 3f && Mathf.Abs(this.between2) < 90f && this.myHero.transform.position.y < this.Neck.position.y + 30f * this.myLevel && this.myHero.transform.position.y > this.Neck.position.y + 10f * this.myLevel)
                            {
                                this.attack("crawler_jump_0");
                                return;
                            }
                        }
                        if (this.abnormalType == AbnormalType.Punk && this.myDistance < 90f && Mathf.Abs(this.between2) > 90f)
                        {
                            if (UnityEngine.Random.Range(0f, 1f) < 0.4f)
                            {
                                this.randomRun(baseT.position + new Vector3(UnityEngine.Random.Range(-50f, 50f), UnityEngine.Random.Range(-50f, 50f), UnityEngine.Random.Range(-50f, 50f)), 10f);
                            }
                            if (UnityEngine.Random.Range(0f, 1f) < 0.2f)
                            {
                                this.recover();
                            }
                            else if (UnityEngine.Random.Range(0, 2) == 0)
                            {
                                this.attack("quick_turn_l");
                            }
                            else
                            {
                                this.attack("quick_turn_r");
                            }
                            return;
                        }
                        if (this.myDistance < this.attackDistance)
                        {
                            if (this.abnormalType == AbnormalType.Crawler)
                            {
                                if (this.myHero.transform.position.y + 3f <= this.Neck.position.y + 20f * this.myLevel)
                                {
                                    if (UnityEngine.Random.Range(0f, 1f) < 0.1f)
                                    {
                                        this.chase();
                                        return;
                                    }
                                }
                                return;
                            }
                            string text = string.Empty;
                            string[] attackStrategy = this.GetAttackStrategy();
                            if (attackStrategy != null)
                            {
                                text = attackStrategy[UnityEngine.Random.Range(0, attackStrategy.Length)];
                            }
                            if ((this.abnormalType == AbnormalType.Jumper || this.abnormalType == AbnormalType.Aberrant) && Mathf.Abs(this.between2) > 40f)
                            {
                                if (text.Contains("grab") || text.Contains("kick") || text.Contains("slap") || text.Contains("bite"))
                                {
                                    if (UnityEngine.Random.Range(0, 100) < 30)
                                    {
                                        this.turn(this.between2);
                                        return;
                                    }
                                }
                                else if (UnityEngine.Random.Range(0, 100) < 90)
                                {
                                    this.turn(this.between2);
                                    return;
                                }
                            }
                            if (this.executeAttack(text))
                            {
                                return;
                            }
                            if (this.abnormalType == AbnormalType.Normal)
                            {
                                if (UnityEngine.Random.Range(0, 100) < 30 && Mathf.Abs(this.between2) > 45f)
                                {
                                    this.turn(this.between2);
                                    return;
                                }
                            }
                            else if (Mathf.Abs(this.between2) > 45f)
                            {
                                this.turn(this.between2);
                                return;
                            }
                        }
                        if (this.PVPfromCheckPt != null)
                        {
                            if (this.PVPfromCheckPt.state == CheckPointState.Titan)
                            {
                                if (UnityEngine.Random.Range(0, 100) > 48)
                                {
                                    GameObject gameObject = this.PVPfromCheckPt.chkPtNext;
                                    if (gameObject != null && (gameObject.GetComponent<PVPcheckPoint>().state != CheckPointState.Titan || UnityEngine.Random.Range(0, 100) < 20))
                                    {
                                        this.toPVPCheckPoint(gameObject.transform.position, (float)(5 + UnityEngine.Random.Range(0, 10)));
                                        this.PVPfromCheckPt = gameObject.GetComponent<PVPcheckPoint>();
                                    }
                                }
                                else
                                {
                                    GameObject gameObject = this.PVPfromCheckPt.chkPtPrevious;
                                    if (gameObject != null && (gameObject.GetComponent<PVPcheckPoint>().state != CheckPointState.Titan || UnityEngine.Random.Range(0, 100) < 5))
                                    {
                                        this.toPVPCheckPoint(gameObject.transform.position, (float)(5 + UnityEngine.Random.Range(0, 10)));
                                        this.PVPfromCheckPt = gameObject.GetComponent<PVPcheckPoint>();
                                    }
                                }
                            }
                            else
                            {
                                this.toPVPCheckPoint(this.PVPfromCheckPt.transform.position, (float)(5 + UnityEngine.Random.Range(0, 10)));
                            }
                        }
                    }
                    break;

                case TitanState.Attack:
                    if (this.attackAnimation == "combo")
                    {
                        if (this.nonAI)
                        {
                            if (this.controller.isAttackDown)
                            {
                                this.nonAIcombo = true;
                            }
                            if (!this.nonAIcombo && baseA["attack_" + this.attackAnimation].normalizedTime >= 0.385f)
                            {
                                this.idle(0f);
                                return;
                            }
                        }
                        if (baseA["attack_" + this.attackAnimation].normalizedTime >= 0.11f && baseA["attack_" + this.attackAnimation].normalizedTime <= 0.16f)
                        {
                            HERO go = this.CheckIfHitHand(base.Hand_R_001);
                            if (go != null)
                            {
                                Vector3 position = Chest.position;
                                if (IN_GAME_MAIN_CAMERA.GameType == GameType.Single)
                                {
                                    go.die((go.baseT.position - position) * 15f * this.myLevel, false);
                                }
                                else if (IN_GAME_MAIN_CAMERA.GameType == GameType.MultiPlayer && base.BasePV.IsMine && !go.HasDied())
                                {
                                    go.markDie();
                                    go.BasePV.RPC("netDie", PhotonTargets.All, new object[]
                                    {
                                    (go.transform.position - position) * 15f * this.myLevel,
                                    false,
                                    (!this.nonAI) ? -1 : base.BasePV.viewID,
                                    ShowName,
                                    true
                                    });
                                }
                            }
                        }
                        if (baseA["attack_" + this.attackAnimation].normalizedTime >= 0.27f && baseA["attack_" + this.attackAnimation].normalizedTime <= 0.32f)
                        {
                            HERO go = this.CheckIfHitHand(Hand_L_001);
                            if (go != null)
                            {
                                Vector3 position2 = Chest.position;
                                if (IN_GAME_MAIN_CAMERA.GameType == GameType.Single)
                                {
                                    go.die((go.baseT.position - position2) * 15f * myLevel, false);
                                }
                                else if (IN_GAME_MAIN_CAMERA.GameType == GameType.MultiPlayer && base.BasePV.IsMine && !go.HasDied())
                                {
                                    go.markDie();
                                    go.BasePV.RPC("netDie", PhotonTargets.All, new object[]
                                    {
                                    (go.transform.position - position2) * 15f * this.myLevel,
                                    false,
                                    (!this.nonAI) ? -1 : base.BasePV.viewID,
                                    ShowName,
                                    true
                                    });
                                }
                            }
                        }
                    }
                    if (this.attackCheckTimeA != 0f && baseA["attack_" + this.attackAnimation].normalizedTime >= this.attackCheckTimeA && baseA["attack_" + this.attackAnimation].normalizedTime <= this.attackCheckTimeB)
                    {
                        HERO go = this.CheckIfHitHand(leftHandAttack ? Hand_L_001 : Hand_R_001);
                        if (go != null)
                        {
                            Vector3 position3 = Chest.position;
                            if (IN_GAME_MAIN_CAMERA.GameType == GameType.Single)
                            {
                                go.die((go.baseT.position - position3) * 15f * this.myLevel, false);
                            }
                            else if (IN_GAME_MAIN_CAMERA.GameType == GameType.MultiPlayer && base.BasePV.IsMine && !go.HasDied())
                            {
                                go.markDie();
                                go.BasePV.RPC("netDie", PhotonTargets.All, new object[]
                                {
                                    (go.baseT.position - position3) * 15f * this.myLevel,
                                    false,
                                    (!this.nonAI) ? -1 : base.BasePV.viewID,
                                    ShowName,
                                    true
                                });
                            }
                        }
                    }
                    if (!this.attacked && this.attackCheckTime != 0f && baseA["attack_" + this.attackAnimation].normalizedTime >= this.attackCheckTime)
                    {
                        this.attacked = true;
                        this.fxPosition = baseT.Find("ap_" + this.attackAnimation).position;
                        GameObject gameObject6;
                        if (IN_GAME_MAIN_CAMERA.GameType == GameType.MultiPlayer && BasePV.IsMine)
                        {
                            gameObject6 = Optimization.Caching.Pool.NetworkEnable("FX/" + this.fxName, this.fxPosition, this.fxRotation, 0);
                        }
                        else
                        {
                            gameObject6 = Pool.Enable("FX/" + this.fxName, this.fxPosition, this.fxRotation);//(GameObject)UnityEngine.Object.Instantiate(CacheResources.Load("FX/" + this.fxName), this.fxPosition, this.fxRotation);
                        }
                        if (this.nonAI)
                        {
                            gameObject6.transform.localScale = baseT.localScale * 1.5f;
                            if (gameObject6.GetComponent<EnemyfxIDcontainer>())
                            {
                                gameObject6.GetComponent<EnemyfxIDcontainer>().myOwnerViewID = BasePV.viewID;
                            }
                        }
                        else
                        {
                            gameObject6.transform.localScale = baseT.localScale;
                        }
                        if (gameObject6.GetComponent<EnemyfxIDcontainer>())
                        {
                            gameObject6.GetComponent<EnemyfxIDcontainer>().titanName = ShowName;
                        }
                        float num = 1f - Vector3.Distance(IN_GAME_MAIN_CAMERA.MainCamera.transform.position, gameObject6.transform.position) * 0.05f;
                        num = Mathf.Min(1f, num);
                        IN_GAME_MAIN_CAMERA.MainCamera.startShake(num, num, 0.95f);
                    }
                    if (this.attackAnimation == "throw")
                    {
                        if (!this.attacked && baseA["attack_" + this.attackAnimation].normalizedTime >= 0.11f)
                        {
                            this.attacked = true;
                            if (IN_GAME_MAIN_CAMERA.GameType == GameType.MultiPlayer && BasePV.IsMine)
                            {
                                this.throwRock = Optimization.Caching.Pool.NetworkEnable("FX/rockThrow", Hand_R_001.position, Hand_R_001.rotation, 0);
                            }
                            else
                            {
                                this.throwRock = Pool.Enable("FX/rockThrow", Hand_R_001.position, Hand_R_001.rotation);//(GameObject)UnityEngine.Object.Instantiate(CacheResources.Load("FX/rockThrow"), Hand_R_001.position, Hand_R_001.rotation);
                            }
                            this.throwRock.transform.localScale = baseT.localScale;
                            this.throwRock.transform.position -= this.throwRock.transform.Forward() * 2.5f * this.myLevel;
                            if (this.throwRock.GetComponent<EnemyfxIDcontainer>())
                            {
                                if (this.nonAI)
                                {
                                    this.throwRock.GetComponent<EnemyfxIDcontainer>().myOwnerViewID = BasePV.viewID;
                                }
                                this.throwRock.GetComponent<EnemyfxIDcontainer>().titanName = ShowName;
                            }
                            this.throwRock.transform.parent = Hand_R_001;
                            if (IN_GAME_MAIN_CAMERA.GameType == GameType.MultiPlayer && BasePV.IsMine)
                            {
                                this.throwRock.GetPhotonView().RPC("initRPC", PhotonTargets.Others, new object[]
                                {
                                BasePV.viewID,
                                baseT.localScale,
                                this.throwRock.transform.localPosition,
                                this.myLevel
                                });
                            }
                        }
                        if (baseA["attack_" + this.attackAnimation].normalizedTime >= 0.11f)
                        {
                            float y = Mathf.Atan2(this.myHero.transform.position.x - baseT.position.x, this.myHero.transform.position.z - baseT.position.z) * 57.29578f;
                            baseGT.rotation = Quaternion.Euler(0f, y, 0f);
                        }
                        if (this.throwRock != null && baseA["attack_" + this.attackAnimation].normalizedTime >= 0.62f)
                        {
                            float num2 = 1f;
                            float num3 = -20f;
                            Vector3 v;
                            if (this.myHero != null)
                            {
                                v = (this.myHero.transform.position - this.throwRock.transform.position) / num2 + this.myHero.rigidbody.velocity;
                                float num4 = this.myHero.transform.position.y + 2f * this.myLevel;
                                float num5 = num4 - this.throwRock.transform.position.y;
                                v = new Vector3(v.x, num5 / num2 - 0.5f * num3 * num2, v.z);
                            }
                            else
                            {
                                v = baseT.Forward() * 60f + Vectors.up * 10f;
                            }
                            this.throwRock.GetComponent<RockThrow>().launch(v);
                            this.throwRock.transform.parent = null;
                            this.throwRock = null;
                        }
                    }
                    if (this.attackAnimation == "jumper_0" || this.attackAnimation == "crawler_jump_0")
                    {
                        if (!this.attacked)
                        {
                            if (baseA["attack_" + this.attackAnimation].normalizedTime >= 0.68f)
                            {
                                this.attacked = true;
                                if (this.myHero == null || this.nonAI)
                                {
                                    float d = 120f;
                                    Vector3 velocity = baseT.Forward() * this.speed + Vectors.up * d;
                                    if (this.nonAI && this.abnormalType == AbnormalType.Crawler)
                                    {
                                        d = 100f;
                                        float num6 = this.speed * 2.5f;
                                        num6 = Mathf.Min(num6, 100f);
                                        velocity = baseT.Forward() * num6 + Vectors.up * d;
                                    }
                                    baseR.velocity = velocity;
                                }
                                else
                                {
                                    float y2 = this.myHero.rigidbody.velocity.y;
                                    float num7 = -20f;
                                    float num8 = this.gravity;
                                    float y3 = this.Neck.position.y;
                                    float num9 = (num7 - num8) * 0.5f;
                                    float num10 = y2;
                                    float num11 = this.myHero.transform.position.y - y3;
                                    float d2 = Mathf.Abs((Mathf.Sqrt(num10 * num10 - 4f * num9 * num11) - num10) / (2f * num9));
                                    Vector3 a = this.myHero.transform.position + this.myHero.rigidbody.velocity * d2 + Vectors.up * 0.5f * num7 * d2 * d2;
                                    float y4 = a.y;
                                    float num12;
                                    if (num11 < 0f || y4 - y3 < 0f)
                                    {
                                        num12 = 60f;
                                        float num13 = this.speed * 2.5f;
                                        num13 = Mathf.Min(num13, 100f);
                                        Vector3 velocity2 = baseT.Forward() * num13 + Vectors.up * num12;
                                        baseR.velocity = velocity2;
                                        return;
                                    }
                                    float num14 = y4 - y3;
                                    float num15 = Mathf.Sqrt(2f * num14 / this.gravity);
                                    num12 = this.gravity * num15;
                                    num12 = Mathf.Max(30f, num12);
                                    Vector3 vector2 = (a - baseT.position) / d2;
                                    this.abnorma_jump_bite_horizon_v = new Vector3(vector2.x, 0f, vector2.z);
                                    Vector3 velocity3 = baseR.velocity;
                                    Vector3 force = new Vector3(this.abnorma_jump_bite_horizon_v.x, velocity3.y, this.abnorma_jump_bite_horizon_v.z) - velocity3;
                                    baseR.AddForce(force, ForceMode.VelocityChange);
                                    baseR.AddForce(Vectors.up * num12, ForceMode.VelocityChange);
                                    float y5 = Vector2.Angle(new Vector2(baseT.position.x, baseT.position.z), new Vector2(this.myHero.transform.position.x, this.myHero.transform.position.z));
                                    y5 = Mathf.Atan2(this.myHero.transform.position.x - baseT.position.x, this.myHero.transform.position.z - baseT.position.z) * 57.29578f;
                                    baseGT.rotation = Quaternion.Euler(0f, y5, 0f);
                                }
                            }
                            else
                            {
                                baseR.velocity = Vectors.zero;
                            }
                        }
                        if (baseA["attack_" + this.attackAnimation].normalizedTime >= 1f)
                        {
                            HERO hero7 = CheckHitHeadAndCrawlerMouth(3f);
                            if (hero7 != null)
                            {
                                if (IN_GAME_MAIN_CAMERA.GameType == GameType.Single)
                                {
                                    hero7.die((hero7.transform.position - Chest.position) * 15f * this.myLevel, false);
                                }
                                else if (IN_GAME_MAIN_CAMERA.GameType == GameType.MultiPlayer && BasePV.IsMine && !hero7.HasDied())
                                {
                                    hero7.markDie();
                                    hero7.BasePV.RPC("netDie", PhotonTargets.All, new object[]
                                    {
                                    (hero7.transform.position - Chest.position) * 15f * this.myLevel,
                                    true,
                                    (!this.nonAI) ? -1 : BasePV.viewID,
                                    ShowName,
                                    true
                                    });
                                }
                                if (this.abnormalType == AbnormalType.Crawler)
                                {
                                    this.attackAnimation = "crawler_jump_1";
                                }
                                else
                                {
                                    this.attackAnimation = "jumper_1";
                                }
                                this.playAnimation("attack_" + this.attackAnimation);
                            }
                            if (Mathf.Abs(baseR.velocity.y) < 0.5f || baseR.velocity.y < 0f || this.IsGrounded())
                            {
                                if (this.abnormalType == AbnormalType.Crawler)
                                {
                                    this.attackAnimation = "crawler_jump_1";
                                }
                                else
                                {
                                    this.attackAnimation = "jumper_1";
                                }
                                this.playAnimation("attack_" + this.attackAnimation);
                            }
                        }
                    }
                    else if (this.attackAnimation == "jumper_1" || this.attackAnimation == "crawler_jump_1")
                    {
                        if (baseA["attack_" + this.attackAnimation].normalizedTime >= 1f && this.grounded)
                        {
                            if (this.abnormalType == AbnormalType.Crawler)
                            {
                                this.attackAnimation = "crawler_jump_2";
                            }
                            else
                            {
                                this.attackAnimation = "jumper_2";
                            }
                            this.crossFade("attack_" + this.attackAnimation, 0.1f);
                            this.fxPosition = baseT.position;
                            GameObject gameObject8;
                            if (IN_GAME_MAIN_CAMERA.GameType == GameType.MultiPlayer && BasePV.IsMine)
                            {
                                gameObject8 = Optimization.Caching.Pool.NetworkEnable("FX/boom2", this.fxPosition, this.fxRotation, 0);
                            }
                            else
                            {
                                gameObject8 = Pool.Enable("FX/boom2", this.fxPosition, this.fxRotation);//(GameObject)UnityEngine.Object.Instantiate(CacheResources.Load("FX/boom2"), this.fxPosition, this.fxRotation);
                            }
                            gameObject8.transform.localScale = baseT.localScale * 1.6f;
                            float num16 = 1f - Vector3.Distance(IN_GAME_MAIN_CAMERA.MainCamera.transform.position, gameObject8.transform.position) * 0.05f;
                            num16 = Mathf.Min(1f, num16);
                            IN_GAME_MAIN_CAMERA.MainCamera.startShake(num16, num16, 0.95f);
                        }
                    }
                    else if (this.attackAnimation == "jumper_2" || this.attackAnimation == "crawler_jump_2")
                    {
                        if (baseA["attack_" + this.attackAnimation].normalizedTime >= 1f)
                        {
                            this.idle(0f);
                        }
                    }
                    else if (baseA.IsPlaying("tired"))
                    {
                        if (baseA["tired"].normalizedTime >= 1f + Mathf.Max(this.attackEndWait * 2f, 3f))
                        {
                            this.idle(UnityEngine.Random.Range(this.attackWait - 1f, 3f));
                        }
                    }
                    else if (baseA["attack_" + this.attackAnimation].normalizedTime >= 1f + this.attackEndWait)
                    {
                        if (this.nextAttackAnimation != null)
                        {
                            this.attack(this.nextAttackAnimation);
                            return;
                        }
                        if (this.attackAnimation == "quick_turn_l" || this.attackAnimation == "quick_turn_r")
                        {
                            baseT.rotation = Quaternion.Euler(baseT.rotation.eulerAngles.x, baseT.rotation.eulerAngles.y + 180f, baseT.rotation.eulerAngles.z);
                            this.idle(UnityEngine.Random.Range(0.5f, 1f));
                            this.playAnimation("idle");
                            return;
                        }
                        if (this.abnormalType == AbnormalType.Aberrant || this.abnormalType == AbnormalType.Jumper)
                        {
                            this.attackCount++;
                            if (this.attackCount > 3 && this.attackAnimation == "abnormal_getup")
                            {
                                this.attackCount = 0;
                                this.crossFade("tired", 0.5f);
                            }
                            else
                            {
                                this.idle(UnityEngine.Random.Range(this.attackWait - 1f, 3f));
                            }
                        }
                        else
                        {
                            this.idle(UnityEngine.Random.Range(this.attackWait - 1f, 3f));
                        }
                    }
                    break;

                case TitanState.Grad:
                    if (baseA["grab_" + this.attackAnimation].normalizedTime >= this.attackCheckTimeA && baseA["grab_" + this.attackAnimation].normalizedTime <= this.attackCheckTimeB && this.grabbedTarget == null)
                    {
                        HERO gameObject9 = this.CheckIfHitHand(this.currentGrabHand);
                        if (gameObject9 != null)
                        {
                            EatSet(gameObject9);
                            this.grabbedTarget = gameObject9;
                        }
                    }
                    if (baseA["grab_" + this.attackAnimation].normalizedTime >= 1f)
                    {
                        if (this.grabbedTarget)
                        {
                            this.eat();
                        }
                        else
                        {
                            this.idle(UnityEngine.Random.Range(this.attackWait - 1f, 2f));
                        }
                    }
                    break;

                case TitanState.Eat:
                    if (!this.attacked && baseA[this.attackAnimation].normalizedTime >= 0.48f)
                    {
                        this.attacked = true;
                        this.JustEatHero(this.grabbedTarget, this.currentGrabHand);
                    }
                    if (this.grabbedTarget == null)
                    {
                    }
                    if (baseA[this.attackAnimation].normalizedTime >= 1f)
                    {
                        this.idle(0f);
                    }
                    break;

                case TitanState.Chase:

                    if (this.myHero == null)
                    {
                        this.idle(0f);
                        return;
                    }
                    if (this.longRangeAttackCheck())
                    {
                        return;
                    }
                    if (IN_GAME_MAIN_CAMERA.GameMode == GameMode.PVP_CAPTURE && this.PVPfromCheckPt != null && this.myDistance > this.chaseDistance)
                    {
                        this.idle(0f);
                        return;
                    }
                    if (this.abnormalType == AbnormalType.Crawler)
                    {
                        Vector3 vector3 = this.myHero.transform.position - baseT.position;
                        float current = -Mathf.Atan2(vector3.z, vector3.x) * 57.29578f;
                        float f = -Mathf.DeltaAngle(current, baseGT.rotation.eulerAngles.y - 90f);
                        if (this.myDistance < this.attackDistance * 3f && UnityEngine.Random.Range(0f, 1f) < 0.1f && Mathf.Abs(f) < 90f && this.myHero.transform.position.y < this.Neck.position.y + 30f * this.myLevel && this.myHero.transform.position.y > this.Neck.position.y + 10f * this.myLevel)
                        {
                            this.attack("crawler_jump_0");
                            return;
                        }

                        HERO gameObject10 = CheckHitHeadAndCrawlerMouth(2.2f);
                        if (gameObject10 != null)
                        {
                            Vector3 position6 = Chest.position;
                            if (IN_GAME_MAIN_CAMERA.GameType == GameType.Single)
                            {
                                gameObject10.die((gameObject10.transform.position - position6) * 15f * this.myLevel, false);
                            }
                            else if (IN_GAME_MAIN_CAMERA.GameType == GameType.MultiPlayer && base.BasePV.IsMine)
                            {
                                if (gameObject10.eren)
                                {
                                    gameObject10.eren.GetComponent<TITAN_EREN>().hitByTitan();
                                }
                                else if (!gameObject10.HasDied())
                                {
                                    gameObject10.markDie();
                                    gameObject10.BasePV.RPC("netDie", PhotonTargets.All, new object[]
                                    {
                                    (gameObject10.baseT.position - position6) * 15f * this.myLevel,
                                    true,
                                    (!this.nonAI) ? -1 : base.BasePV.viewID,
                                    ShowName,
                                    true
                                    });
                                }
                            }
                        }
                        if (this.myDistance < this.attackDistance && UnityEngine.Random.Range(0f, 1f) < 0.02f)
                        {
                            this.idle(UnityEngine.Random.Range(0.05f, 0.2f));
                        }
                    }
                    else
                    {
                        if (this.abnormalType == AbnormalType.Jumper && ((this.myDistance > this.attackDistance && this.myHero.transform.position.y > this.Head.position.y + 4f * this.myLevel) || this.myHero.transform.position.y > this.Head.position.y + 4f * this.myLevel) && Vector3.Distance(baseT.position, this.myHero.transform.position) < 1.5f * this.myHero.transform.position.y)
                        {
                            this.attack("jumper_0");
                            return;
                        }
                        if (this.myDistance < this.attackDistance)
                        {
                            this.idle(UnityEngine.Random.Range(0.05f, 0.2f));
                        }
                    }
                    break;

                case TitanState.Wander:

                    if (this.myDistance < this.chaseDistance || this.whoHasTauntMe != null)
                    {
                        Vector3 vector4 = this.myHero.transform.position - baseT.position;
                        float current2 = -Mathf.Atan2(vector4.z, vector4.x) * 57.29578f;
                        float f2 = -Mathf.DeltaAngle(current2, baseGT.rotation.eulerAngles.y - 90f);
                        if (this.isAlarm || Mathf.Abs(f2) < 90f)
                        {
                            this.chase();
                            return;
                        }
                        if (!this.isAlarm && this.myDistance < this.chaseDistance * 0.1f)
                        {
                            this.chase();
                            return;
                        }
                    }
                    if (UnityEngine.Random.Range(0f, 1f) < 0.01f)
                    {
                        this.idle(0f);
                    }
                    break;

                case TitanState.Turn:
                    baseGT.rotation = Quaternion.Lerp(baseGT.rotation, Quaternion.Euler(0f, this.desDeg, 0f), dt * Mathf.Abs(this.turnDeg) * 0.015f);
                    if (baseA[this.turnAnimation].normalizedTime >= 1f)
                    {
                        this.idle(0f);
                    }
                    break;

                case TitanState.Hit_Eye:
                    if (baseA.IsPlaying("sit_hit_eye") && baseA["sit_hit_eye"].normalizedTime >= 1f)
                    {
                        this.remainSitdown();
                    }
                    else if (baseA.IsPlaying("hit_eye") && baseA["hit_eye"].normalizedTime >= 1f)
                    {
                        if (this.nonAI)
                        {
                            this.idle(0f);
                        }
                        else
                        {
                            this.attack("combo_1");
                        }
                    }
                    break;

                case TitanState.To_CheckPoint:
                    if (this.checkPoints.Count <= 0)
                    {
                        if (this.myDistance < this.attackDistance)
                        {
                            string decidedAction = string.Empty;
                            string[] attackStrategy2 = this.GetAttackStrategy();
                            if (attackStrategy2 != null)
                            {
                                decidedAction = attackStrategy2[UnityEngine.Random.Range(0, attackStrategy2.Length)];
                            }
                            if (this.executeAttack(decidedAction))
                            {
                                return;
                            }
                        }
                    }
                    if (Vector3.Distance(baseT.position, this.targetCheckPt) < this.targetR)
                    {
                        if (this.checkPoints.Count > 0)
                        {
                            if (this.checkPoints.Count == 1)
                            {
                                if (IN_GAME_MAIN_CAMERA.GameMode == GameMode.BOSS_FIGHT_CT)
                                {
                                    FengGameManagerMKII.FGM.GameLose();
                                    this.checkPoints = new ArrayList();
                                    this.idle(0f);
                                }
                            }
                            else
                            {
                                if (this.checkPoints.Count == 4)
                                {
                                    FengGameManagerMKII.FGM.SendChatContentInfo("<color=#A8FF24>*WARNING!* An abnormal titan is approaching the north gate!</color>");
                                }
                                Vector3 vector5 = (Vector3)this.checkPoints[0];
                                this.targetCheckPt = vector5;
                                this.checkPoints.RemoveAt(0);
                            }
                        }
                        else
                        {
                            this.idle(0f);
                        }
                    }
                    break;

                case TitanState.To_PVP_PT:
                    if (this.myDistance < this.chaseDistance * 0.7f)
                    {
                        this.chase();
                    }
                    if (Vector3.Distance(baseT.position, this.targetCheckPt) < this.targetR)
                    {
                        this.idle(0f);
                    }
                    break;

                case TitanState.Random_Run:
                    this.random_run_time -= dt;
                    if (Vector3.Distance(baseT.position, this.targetCheckPt) < this.targetR || this.random_run_time <= 0f)
                    {
                        this.idle(0f);
                    }
                    break;

                case TitanState.Down:
                    this.getdownTime -= dt;
                    if (baseA.IsPlaying("sit_hunt_down") && baseA["sit_hunt_down"].normalizedTime >= 1f)
                    {
                        this.playAnimation("sit_idle");
                    }
                    if (this.getdownTime <= 0f)
                    {
                        this.crossFade("sit_getup", 0.1f);
                    }
                    if (baseA.IsPlaying("sit_getup") && baseA["sit_getup"].normalizedTime >= 1f)
                    {
                        this.idle(0f);
                    }
                    break;

                case TitanState.Sit:
                    this.getdownTime -= dt;
                    this.angle = 0f;
                    this.between2 = 0f;
                    if (this.myDistance < this.chaseDistance || this.whoHasTauntMe != null)
                    {
                        if (this.myDistance < 50f)
                        {
                            this.isAlarm = true;
                        }
                        else
                        {
                            Vector3 vector6 = this.myHero.transform.position - baseT.position;
                            this.angle = -Mathf.Atan2(vector6.z, vector6.x) * 57.29578f;
                            this.between2 = -Mathf.DeltaAngle(this.angle, baseGT.rotation.eulerAngles.y - 90f);
                            if (Mathf.Abs(this.between2) < 100f)
                            {
                                this.isAlarm = true;
                            }
                        }
                    }
                    if (baseA.IsPlaying("sit_down") && baseA["sit_down"].normalizedTime >= 1f)
                    {
                        this.playAnimation("sit_idle");
                    }
                    if ((this.getdownTime <= 0f || this.isAlarm) && baseA.IsPlaying("sit_idle"))
                    {
                        this.crossFade("sit_getup", 0.1f);
                    }
                    if (baseA.IsPlaying("sit_getup") && baseA["sit_getup"].normalizedTime >= 1f)
                    {
                        this.idle(0f);
                    }
                    break;

                case TitanState.Recover:
                    this.getdownTime -= dt;
                    if (this.getdownTime <= 0f)
                    {
                        this.idle(0f);
                    }
                    if (baseA.IsPlaying("idle_recovery") && baseA["idle_recovery"].normalizedTime >= 1f)
                    {
                        this.idle(0f);
                    }
                    break;
            }
            return;
        }
        this.dieTime += dt;
        if (this.dieTime > 2f && !this.hasDieSteam)
        {
            this.hasDieSteam = true;
            if (IN_GAME_MAIN_CAMERA.GameType == GameType.Single)
            {
                GameObject gameObject11 = Pool.Enable("FX/FXtitanDie1", Hip.position, Quaternion.identity);//(GameObject)UnityEngine.Object.Instantiate(CacheResources.Load("FX/FXtitanDie1"));
                //gameObject11.transform.position = Hip.position;
                gameObject11.transform.localScale = baseT.localScale;
            }
            else if (BasePV.IsMine)
            {
                GameObject gameObject12 = Optimization.Caching.Pool.NetworkEnable("FX/FXtitanDie1", Hip.position, Quaternion.Euler(-90f, 0f, 0f), 0);
                gameObject12.transform.localScale = baseT.localScale;
            }
        }
        if (this.dieTime > 5f)
        {
            if (IN_GAME_MAIN_CAMERA.GameType == GameType.Single)
            {
                GameObject gameObject13 = Pool.Enable("FX/FXtitanDie", Hip.position, Quaternion.identity);//(GameObject)UnityEngine.Object.Instantiate(CacheResources.Load("FX/FXtitanDie"));
                //gameObject13.transform.position = Hip.position;
                gameObject13.transform.localScale = baseT.localScale;
                UnityEngine.Object.Destroy(baseG);
            }
            else if (BasePV.IsMine)
            {
                GameObject gameObject14 = Optimization.Caching.Pool.NetworkEnable("FX/FXtitanDie", Hip.position, Quaternion.Euler(-90f, 0f, 0f), 0);
                gameObject14.transform.localScale = baseT.localScale;
                PhotonNetwork.Destroy(baseG);
                this.myDifficulty = -1;
            }
            return;
        }
    }

    public void UpdateCollider()
    {
        if (this.ColliderEnabled)
        {
            if (!this.IsHooked && !this.MyTitanTrigger.IsCollide && !this.IsLook)
            {
                foreach (Collider collider in this.baseColliders)
                {
                    collider.enabled = false;
                }
                this.ColliderEnabled = false;
            }
        }
        else if (this.IsHooked || this.MyTitanTrigger.IsCollide || this.IsLook)
        {
            foreach (Collider collider in this.baseColliders)
            {
                collider.enabled = true;
            }
            this.ColliderEnabled = true;
        }
    }
}