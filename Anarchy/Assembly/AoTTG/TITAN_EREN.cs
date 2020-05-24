using Anarchy;
using Anarchy.Configuration;
using Anarchy.Configuration.Presets;
using Anarchy.Skins.Titans;
using Optimization.Caching;
using System.Collections;
using UnityEngine;
public class TITAN_EREN : Photon.MonoBehaviour
{
    private string attackAnimation;
    private Transform attackBox;
    private bool attackChkOnce;
    private ArrayList checkPoints = new ArrayList();
    private float dieTime;
    private float facingDirection;
    private float gravity = 500f;
    private bool grounded;
    private bool hasDieSteam;
    private string hitAnimation;
    private float hitPause;
    private ArrayList hitTargets;
    private bool isAttack;
    private bool isHitWhileCarryingRock;
    private bool isNextAttack;
    private bool isPlayRoar;
    private bool isROCKMOVE;
    private bool justGrounded;
    private float lifeTimeMax = 9999f;
    private float myR;
    private ErenSkin mySkin;
    private bool needFreshCorePosition;
    private bool needRoar;
    private Vector3 oldCorePosition;
    private bool rockHitGround;
    private int rockPhase;
    private bool spawned;
    private float sqrt2 = Mathf.Sqrt(2f);
    private int stepSoundPhase = 2;
    private Vector3 targetCheckPt;
    private float waitCounter;
    public GameObject bottomObject;
    public bool canJump = true;
    public Camera currentCamera;
    public bool hasDied;
    public bool isHit;
    public float jumpHeight = 2f;
    public float lifeTime = 9999f;
    public float maxVelocityChange = 100f;
    public GameObject realBody;
    public GameObject rock;
    public bool rockLift;
    public float speed = 80f;

    private void crossFade(string aniName, float time)
    {
        base.animation.CrossFade(aniName, time);
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

    [RPC]
    private void endMovingRock()
    {
        this.isROCKMOVE = false;
    }

    private void falseAttack()
    {
        this.isAttack = false;
        this.isNextAttack = false;
        this.hitTargets = new ArrayList();
        this.attackChkOnce = false;
    }

    private void FixedUpdate()
    {
        if (IN_GAME_MAIN_CAMERA.isPausing && IN_GAME_MAIN_CAMERA.GameType == GameType.Single)
        {
            return;
        }
        if (this.rockLift)
        {
            this.RockUpdate();
            return;
        }
        if (IN_GAME_MAIN_CAMERA.GameType != GameType.Single)
        {
            if (!BasePV.IsMine)
            {
                return;
            }
        }
        if (this.hitPause > 0f)
        {
            base.rigidbody.velocity = Vectors.zero;
            return;
        }
        if (this.hasDied)
        {
            base.rigidbody.velocity = Vectors.zero + Vectors.up * base.rigidbody.velocity.y;
            base.rigidbody.AddForce(new Vector3(0f, -this.gravity * base.rigidbody.mass, 0f));
            return;
        }
        if (IN_GAME_MAIN_CAMERA.GameType == GameType.Single || BasePV.IsMine)
        {
            if (base.rigidbody.velocity.magnitude > 50f)
            {
                IN_GAME_MAIN_CAMERA.MainCamera.GetComponent<Camera>().fieldOfView = Mathf.Lerp(IN_GAME_MAIN_CAMERA.MainCamera.GetComponent<Camera>().fieldOfView, Mathf.Min(100f, base.rigidbody.velocity.magnitude), 0.1f);
            }
            else
            {
                IN_GAME_MAIN_CAMERA.MainCamera.GetComponent<Camera>().fieldOfView = Mathf.Lerp(IN_GAME_MAIN_CAMERA.MainCamera.GetComponent<Camera>().fieldOfView, 50f, 0.1f);
            }
            if (this.bottomObject.GetComponent<CheckHitGround>().isGrounded)
            {
                if (!this.grounded)
                {
                    this.justGrounded = true;
                }
                this.grounded = true;
                this.bottomObject.GetComponent<CheckHitGround>().isGrounded = false;
            }
            else
            {
                this.grounded = false;
            }
            float num = 0f;
            float num2 = 0f;
            if (!IN_GAME_MAIN_CAMERA.isTyping)
            {
                if (InputManager.IsInput[InputCode.Up])
                {
                    num2 = 1f;
                }
                else if (InputManager.IsInput[InputCode.Down])
                {
                    num2 = -1f;
                }
                else
                {
                    num2 = 0f;
                }
                if (InputManager.IsInput[InputCode.Left])
                {
                    num = -1f;
                }
                else if (InputManager.IsInput[InputCode.Right])
                {
                    num = 1f;
                }
                else
                {
                    num = 0f;
                }
            }
            if (this.needFreshCorePosition)
            {
                this.oldCorePosition = base.transform.position - base.transform.Find("Amarture/Core").position;
                this.needFreshCorePosition = false;
            }
            if (this.isAttack || this.isHit)
            {
                Vector3 a = base.transform.position - base.transform.Find("Amarture/Core").position - this.oldCorePosition;
                this.oldCorePosition = base.transform.position - base.transform.Find("Amarture/Core").position;
                base.rigidbody.velocity = a / Time.deltaTime + Vectors.up * base.rigidbody.velocity.y;
                base.rigidbody.rotation = Quaternion.Lerp(base.gameObject.transform.rotation, Quaternion.Euler(0f, this.facingDirection, 0f), Time.deltaTime * 10f);
                if (this.justGrounded)
                {
                    this.justGrounded = false;
                }
            }
            else if (this.grounded)
            {
                Vector3 a2 = Vectors.zero;
                if (this.justGrounded)
                {
                    this.justGrounded = false;
                    a2 = base.rigidbody.velocity;
                    if (base.animation.IsPlaying("jump_air"))
                    {
                        GameObject gameObject = Pool.Enable("FX/boom2_eren", base.transform.position, Quaternion.Euler(270f, 0f, 0f));//(GameObject)UnityEngine.Object.Instantiate(CacheResources.Load("FX/boom2_eren"), base.transform.position, Quaternion.Euler(270f, 0f, 0f));
                        gameObject.transform.localScale = Vectors.one * 1.5f;
                        if (this.needRoar)
                        {
                            this.playAnimation("born");
                            this.needRoar = false;
                            this.isPlayRoar = false;
                        }
                        else
                        {
                            this.playAnimation("jump_land");
                        }
                    }
                }
                if (!base.animation.IsPlaying("jump_land") && !this.isAttack && !this.isHit && !base.animation.IsPlaying("born"))
                {
                    Vector3 vector = new Vector3(num, 0f, num2);
                    float y = IN_GAME_MAIN_CAMERA.MainCamera.transform.rotation.eulerAngles.y;
                    float num3 = Mathf.Atan2(num2, num) * 57.29578f;
                    num3 = -num3 + 90f;
                    float num4 = y + num3;
                    float num5 = -num4 + 90f;
                    float x = Mathf.Cos(num5 * 0.0174532924f);
                    float z = Mathf.Sin(num5 * 0.0174532924f);
                    a2 = new Vector3(x, 0f, z);
                    float d = (vector.magnitude <= 0.95f) ? ((vector.magnitude >= 0.25f) ? vector.magnitude : 0f) : 1f;
                    a2 *= d;
                    a2 *= this.speed;
                    if (num != 0f || num2 != 0f)
                    {
                        if (!base.animation.IsPlaying("run") && !base.animation.IsPlaying("jump_start") && !base.animation.IsPlaying("jump_air"))
                        {
                            this.crossFade("run", 0.1f);
                        }
                    }
                    else
                    {
                        if (!base.animation.IsPlaying("idle") && !base.animation.IsPlaying("dash_land") && !base.animation.IsPlaying("dodge") && !base.animation.IsPlaying("jump_start") && !base.animation.IsPlaying("jump_air") && !base.animation.IsPlaying("jump_land"))
                        {
                            this.crossFade("idle", 0.1f);
                            a2 *= 0f;
                        }
                        num4 = -874f;
                    }
                    if (num4 != -874f)
                    {
                        this.facingDirection = num4;
                    }
                }
                Vector3 velocity = base.rigidbody.velocity;
                Vector3 force = a2 - velocity;
                force.x = Mathf.Clamp(force.x, -this.maxVelocityChange, this.maxVelocityChange);
                force.z = Mathf.Clamp(force.z, -this.maxVelocityChange, this.maxVelocityChange);
                force.y = 0f;
                if (base.animation.IsPlaying("jump_start") && base.animation["jump_start"].normalizedTime >= 1f)
                {
                    this.playAnimation("jump_air");
                    force.y += 240f;
                }
                else if (base.animation.IsPlaying("jump_start"))
                {
                    force = -base.rigidbody.velocity;
                }
                base.rigidbody.AddForce(force, ForceMode.VelocityChange);
                base.rigidbody.rotation = Quaternion.Lerp(base.gameObject.transform.rotation, Quaternion.Euler(0f, this.facingDirection, 0f), Time.deltaTime * 10f);
            }
            else
            {
                if (base.animation.IsPlaying("jump_start") && base.animation["jump_start"].normalizedTime >= 1f)
                {
                    this.playAnimation("jump_air");
                    base.rigidbody.AddForce(Vectors.up * 240f, ForceMode.VelocityChange);
                }
                if (!base.animation.IsPlaying("jump") && !this.isHit)
                {
                    Vector3 vector2 = new Vector3(num, 0f, num2);
                    float y2 = IN_GAME_MAIN_CAMERA.MainCamera.transform.rotation.eulerAngles.y;
                    float num6 = Mathf.Atan2(num2, num) * 57.29578f;
                    num6 = -num6 + 90f;
                    float num7 = y2 + num6;
                    float num8 = -num7 + 90f;
                    float x2 = Mathf.Cos(num8 * 0.0174532924f);
                    float z2 = Mathf.Sin(num8 * 0.0174532924f);
                    Vector3 vector3 = new Vector3(x2, 0f, z2);
                    float d2 = (vector2.magnitude <= 0.95f) ? ((vector2.magnitude >= 0.25f) ? vector2.magnitude : 0f) : 1f;
                    vector3 *= d2;
                    vector3 *= this.speed * 2f;
                    if (num != 0f || num2 != 0f)
                    {
                        base.rigidbody.AddForce(vector3, ForceMode.Impulse);
                    }
                    else
                    {
                        num7 = -874f;
                    }
                    if (num7 != -874f)
                    {
                        this.facingDirection = num7;
                    }
                    if (!base.animation.IsPlaying(string.Empty) && !base.animation.IsPlaying("attack3_2") && !base.animation.IsPlaying("attack5"))
                    {
                        base.rigidbody.rotation = Quaternion.Lerp(base.gameObject.transform.rotation, Quaternion.Euler(0f, this.facingDirection, 0f), Time.deltaTime * 6f);
                    }
                }
            }
            base.rigidbody.AddForce(new Vector3(0f, -this.gravity * base.rigidbody.mass, 0f));
            return;
        }
    }

    [RPC]
    private void hitByFTRPC(int phase)
    {
        if (BasePV.IsMine)
        {
            this.hitByFT(phase);
        }
    }

    [RPC]
    private void hitByTitanRPC()
    {
        if (BasePV.IsMine)
        {
            this.hitByTitan();
        }
    }

    private System.Collections.IEnumerator LoadMySkin(string url)
    {
        while (!spawned)
        {
            yield return null;
        }
        mySkin = new ErenSkin(this, url);
        Anarchy.Skins.Skin.Check(mySkin, new string[] { url });
    }

    private void LoadSkin()
    {
        if (SkinSettings.SkinsCheck(SkinSettings.TitanSkins))
        {
            if (SkinSettings.TitanSet.Value != Anarchy.Configuration.StringSetting.NotDefine)
            {
                TitanSkinPreset set = new TitanSkinPreset(SkinSettings.TitanSet.Value);
                set.Load();
                if (set.Eren.IsImage())
                {
                    StartCoroutine(LoadMySkin(set.Eren));
                    if (IN_GAME_MAIN_CAMERA.GameType == GameType.MultiPlayer && PhotonNetwork.IsMasterClient && SkinSettings.TitanSkins.Value != 2)
                    {
                        BasePV.RPC("loadskinRPC", PhotonTargets.OthersBuffered, new object[] { set.Eren });
                    }
                }
            }
        }
    }

    [RPC]
    private void loadskinRPC(string url, PhotonMessageInfo info = null)
    {
        if (SkinSettings.TitanSkins.Value == 1 && info != null && info.Sender.IsMasterClient)
        {
            if (mySkin != null)
            {
                Debug.LogError($"Someone tries to reload existing {GetType().Name} skin");
                return;
            }
            StartCoroutine(LoadMySkin(url));
        }
    }

    [RPC]
    private void netCrossFade(string aniName, float time)
    {
        base.animation.CrossFade(aniName, time);
    }

    [RPC]
    private void netPlayAnimation(string aniName)
    {
        base.animation.Play(aniName);
    }

    [RPC]
    private void netPlayAnimationAt(string aniName, float normalizedTime)
    {
        base.animation.Play(aniName);
        base.animation[aniName].normalizedTime = normalizedTime;
    }

    [RPC]
    private void netTauntAttack(float tauntTime, float distance = 100f)
    {
        GameObject[] array = GameObject.FindGameObjectsWithTag("titan");
        foreach (GameObject gameObject in array)
        {
            if (Vector3.Distance(gameObject.transform.position, base.transform.position) < distance && gameObject.GetComponent<TITAN>())
            {
                gameObject.GetComponent<TITAN>().BeTauntedBy(base.gameObject, tauntTime);
            }
            if (gameObject.GetComponent<FEMALE_TITAN>())
            {
                gameObject.GetComponent<FEMALE_TITAN>().erenIsHere(base.gameObject);
            }
        }
    }

    private void OnDestroy()
    {
        if (FengGameManagerMKII.FGM != null)
        {
            FengGameManagerMKII.FGM.RemoveET(this);
        }
    }

    private void playAnimationAt(string aniName, float normalizedTime)
    {
        base.animation.Play(aniName);
        base.animation[aniName].normalizedTime = normalizedTime;
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

    private void playSound(string sndname)
    {
        this.playsoundRPC(sndname);
        if (IN_GAME_MAIN_CAMERA.GameType != GameType.Single)
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
        Transform transform = base.transform.Find(sndname);
        transform.GetComponent<AudioSource>().Play();
    }

    [RPC]
    private void rockPlayAnimation(string anim)
    {
        if (rock.animation == null)
        {
            return;
        }
        this.rock.animation.Play(anim);
        this.rock.animation[anim].speed = 1f;
    }

    private void RockUpdate()
    {
        if (this.isHitWhileCarryingRock)
        {
            return;
        }
        if (this.isROCKMOVE)
        {
            this.rock.transform.position = base.transform.position;
            this.rock.transform.rotation = base.transform.rotation;
        }
        if (IN_GAME_MAIN_CAMERA.GameType != GameType.Single)
        {
            if (!BasePV.IsMine)
            {
                return;
            }
        }
        if (this.rockPhase == 0)
        {
            base.rigidbody.AddForce(-base.rigidbody.velocity, ForceMode.VelocityChange);
            base.rigidbody.AddForce(new Vector3(0f, -10f * base.rigidbody.mass, 0f));
            this.waitCounter += Time.deltaTime;
            if (this.waitCounter > 20f)
            {
                this.rockPhase++;
                this.crossFade("idle", 1f);
                this.waitCounter = 0f;
                this.setRoute();
            }
        }
        else if (this.rockPhase == 1)
        {
            base.rigidbody.AddForce(-base.rigidbody.velocity, ForceMode.VelocityChange);
            base.rigidbody.AddForce(new Vector3(0f, -this.gravity * base.rigidbody.mass, 0f));
            this.waitCounter += Time.deltaTime;
            if (this.waitCounter > 2f)
            {
                this.rockPhase++;
                this.crossFade("run", 0.2f);
                this.waitCounter = 0f;
            }
        }
        else if (this.rockPhase == 2)
        {
            Vector3 a = base.transform.Forward() * 30f;
            Vector3 velocity = base.rigidbody.velocity;
            Vector3 force = a - velocity;
            force.x = Mathf.Clamp(force.x, -this.maxVelocityChange, this.maxVelocityChange);
            force.z = Mathf.Clamp(force.z, -this.maxVelocityChange, this.maxVelocityChange);
            force.y = 0f;
            base.rigidbody.AddForce(force, ForceMode.VelocityChange);
            if (base.transform.position.z < -238f)
            {
                base.transform.position = new Vector3(base.transform.position.x, 0f, -238f);
                this.rockPhase++;
                this.crossFade("idle", 0.2f);
                this.waitCounter = 0f;
            }
        }
        else if (this.rockPhase == 3)
        {
            base.rigidbody.AddForce(-base.rigidbody.velocity, ForceMode.VelocityChange);
            base.rigidbody.AddForce(new Vector3(0f, -10f * base.rigidbody.mass, 0f));
            this.waitCounter += Time.deltaTime;
            if (this.waitCounter > 1f)
            {
                this.rockPhase++;
                this.crossFade("rock_lift", 0.1f);
                BasePV.RPC("rockPlayAnimation", PhotonTargets.All, new object[]
                {
                    "lift"
                });
                this.waitCounter = 0f;
                this.targetCheckPt = (Vector3)this.checkPoints[0];
            }
        }
        else if (this.rockPhase == 4)
        {
            base.rigidbody.AddForce(-base.rigidbody.velocity, ForceMode.VelocityChange);
            base.rigidbody.AddForce(new Vector3(0f, -this.gravity * base.rigidbody.mass, 0f));
            this.waitCounter += Time.deltaTime;
            if (this.waitCounter > 4.2f)
            {
                this.rockPhase++;
                this.crossFade("rock_walk", 0.1f);
                BasePV.RPC("rockPlayAnimation", PhotonTargets.All, new object[]
                {
                    "move"
                });
                this.rock.animation["move"].normalizedTime = base.animation["rock_walk"].normalizedTime;
                this.waitCounter = 0f;
                BasePV.RPC("startMovingRock", PhotonTargets.All, new object[0]);
            }
        }
        else if (this.rockPhase == 5)
        {
            if (Vector3.Distance(base.transform.position, this.targetCheckPt) < 10f)
            {
                if (this.checkPoints.Count > 0)
                {
                    if (this.checkPoints.Count == 1)
                    {
                        this.rockPhase++;
                    }
                    else
                    {
                        Vector3 vector = (Vector3)this.checkPoints[0];
                        this.targetCheckPt = vector;
                        this.checkPoints.RemoveAt(0);
                        GameObject[] array = GameObject.FindGameObjectsWithTag("titanRespawn2");
                        GameObject gameObject = CacheGameObject.Find("titanRespawnTrost" + (7 - this.checkPoints.Count));
                        if (gameObject != null)
                        {
                            foreach (GameObject gameObject2 in array)
                            {
                                if (gameObject2.transform.parent.gameObject == gameObject)
                                {
                                    var  tit = FengGameManagerMKII.FGM.SpawnTitan(70, gameObject2.transform.position, gameObject2.transform.rotation, false);
                                    tit.isAlarm = true;
                                    tit.chaseDistance = 999999f;
                                }
                            }
                        }
                    }
                }
                else
                {
                    this.rockPhase++;
                }
            }
            if (this.checkPoints.Count > 0 && UnityEngine.Random.Range(0, 3000) < 10 - this.checkPoints.Count)
            {
                Quaternion rotation;
                if (UnityEngine.Random.Range(0, 10) > 5)
                {
                    rotation = base.transform.rotation * Quaternion.Euler(0f, UnityEngine.Random.Range(150f, 210f), 0f);
                }
                else
                {
                    rotation = base.transform.rotation * Quaternion.Euler(0f, UnityEngine.Random.Range(-30f, 30f), 0f);
                }
                Vector3 b = rotation * new Vector3(UnityEngine.Random.Range(100f, 200f), 0f, 0f);
                Vector3 vector2 = base.transform.position + b;
                float d = 0f;
                RaycastHit raycastHit;
                if (Physics.Raycast(vector2 + Vectors.up * 500f, -Vectors.up, out raycastHit, 1000f, Layers.Ground.value))
                {
                    d = raycastHit.point.y;
                }
                vector2 += Vectors.up * d;
                var tit = FengGameManagerMKII.FGM.SpawnTitan(70, vector2, base.transform.rotation, false);
                tit.isAlarm = true;
                tit.chaseDistance = 999999f;
            }
            Vector3 a2 = base.transform.Forward() * 6f;
            Vector3 velocity2 = base.rigidbody.velocity;
            Vector3 force2 = a2 - velocity2;
            force2.x = Mathf.Clamp(force2.x, -this.maxVelocityChange, this.maxVelocityChange);
            force2.z = Mathf.Clamp(force2.z, -this.maxVelocityChange, this.maxVelocityChange);
            force2.y = 0f;
            base.rigidbody.AddForce(force2, ForceMode.VelocityChange);
            base.rigidbody.AddForce(new Vector3(0f, -this.gravity * base.rigidbody.mass, 0f));
            Vector3 vector3 = this.targetCheckPt - base.transform.position;
            float current = -Mathf.Atan2(vector3.z, vector3.x) * 57.29578f;
            float num = -Mathf.DeltaAngle(current, base.gameObject.transform.rotation.eulerAngles.y - 90f);
            base.gameObject.transform.rotation = Quaternion.Lerp(base.gameObject.transform.rotation, Quaternion.Euler(0f, base.gameObject.transform.rotation.eulerAngles.y + num, 0f), 0.8f * Time.deltaTime);
        }
        else if (this.rockPhase == 6)
        {
            base.rigidbody.AddForce(-base.rigidbody.velocity, ForceMode.VelocityChange);
            base.rigidbody.AddForce(new Vector3(0f, -10f * base.rigidbody.mass, 0f));
            base.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
            this.rockPhase++;
            this.crossFade("rock_fix_hole", 0.1f);
            BasePV.RPC("rockPlayAnimation", PhotonTargets.All, new object[]
            {
                "set"
            });
            BasePV.RPC("endMovingRock", PhotonTargets.All, new object[0]);
        }
        else if (this.rockPhase == 7)
        {
            base.rigidbody.AddForce(-base.rigidbody.velocity, ForceMode.VelocityChange);
            base.rigidbody.AddForce(new Vector3(0f, -10f * base.rigidbody.mass, 0f));
            if (base.animation["rock_fix_hole"].normalizedTime >= 1.2f)
            {
                this.crossFade("die", 0.1f);
                this.rockPhase++;
                FengGameManagerMKII.FGM.GameWin();
            }
            if (base.animation["rock_fix_hole"].normalizedTime >= 0.62f && !this.rockHitGround)
            {
                this.rockHitGround = true;
                if (IN_GAME_MAIN_CAMERA.GameType == GameType.MultiPlayer && PhotonNetwork.IsMasterClient)
                {
                    Optimization.Caching.Pool.NetworkEnable("FX/boom1_CT_KICK", new Vector3(0f, 30f, 684f), Quaternion.Euler(270f, 0f, 0f), 0);
                }
                else
                {
                    Pool.Enable("FX/boom1_CT_KICK", new Vector3(0f, 30f, 684f), Quaternion.Euler(270f, 0f, 0f));
                   // UnityEngine.Object.Instantiate(CacheResources.Load("FX/boom1_CT_KICK"), new Vector3(0f, 30f, 684f), Quaternion.Euler(270f, 0f, 0f));
                }
            }
        }
    }

    private void showAimUI()
    {
        Vector3 vector = Vectors.up * 10000f;
        GameObject go = CacheGameObject.Find("cross1");
        go.transform.localPosition = vector;
        CacheGameObject.Find("cross2").transform.localPosition = vector;
        CacheGameObject.Find("crossL1").transform.localPosition = vector;
        CacheGameObject.Find("crossL2").transform.localPosition = vector;
        CacheGameObject.Find("crossR1").transform.localPosition = vector;
        CacheGameObject.Find("crossR2").transform.localPosition = vector;
        CacheGameObject.Find("LabelDistance").transform.localPosition = vector;
        go.transform.localPosition = vector;
    }

    private void showSkillCD()
    {
        CacheGameObject.Find("skill_cd_eren").GetComponent<UISprite>().fillAmount = this.lifeTime / this.lifeTimeMax;
    }

    private void Start()
    {
        if (GameModes.KickEren.Enabled && PhotonNetwork.IsMasterClient && !BasePV.IsMine)
        {
            PhotonNetwork.CloseConnection(BasePV.owner);
            Anarchy.UI.Chat.Add($"Player {BasePV.owner} autobanned  (anti-eren)");
            return;
        }
        FengGameManagerMKII.FGM.AddET(this);
        LoadSkin();
        if (this.rockLift)
        {
            this.rock = CacheGameObject.Find("rock");
            if (rock.animation["lift"] != null)
            {
                this.rock.animation["lift"].speed = 0f;
            }
            return;
        }
        if (transform.Find("Amarture/Core") != null)
        {
            this.oldCorePosition = base.transform.position - base.transform.Find("Amarture/Core").position;
        }
        this.myR = this.sqrt2 * 6f;
        if (animation["hit_annie_1"] != null)
        {
            base.animation["hit_annie_1"].speed = 0.8f;
        }
        if (animation["hit_annie_2"] != null)
        {
            base.animation["hit_annie_2"].speed = 0.7f;
        }
        if (animation["hit_annie_3"] != null)
        {
            base.animation["hit_annie_3"].speed = 0.7f;
        }
        spawned = true;
    }

    [RPC]
    private void startMovingRock()
    {
        this.isROCKMOVE = true;
    }

    public void born()
    {
        GameObject[] array = GameObject.FindGameObjectsWithTag("titan");
        foreach (GameObject gameObject in array)
        {
            if (gameObject.GetComponent<FEMALE_TITAN>())
            {
                gameObject.GetComponent<FEMALE_TITAN>().erenIsHere(base.gameObject);
            }
        }
        if (!this.bottomObject.GetComponent<CheckHitGround>().isGrounded)
        {
            this.playAnimation("jump_air");
            this.needRoar = true;
        }
        else
        {
            this.needRoar = false;
            this.playAnimation("born");
            this.isPlayRoar = false;
        }
        this.playSound("snd_eren_shift");
        if (IN_GAME_MAIN_CAMERA.GameType == GameType.Single)
        {
            Pool.Enable("FX/Thunder", base.transform.position + Vectors.up * 23f, Quaternion.Euler(270f, 0f, 0f));
            //UnityEngine.Object.Instantiate(CacheResources.Load("FX/Thunder"), base.transform.position + Vectors.up * 23f, Quaternion.Euler(270f, 0f, 0f));
        }
        else if (BasePV.IsMine)
        {
            Optimization.Caching.Pool.NetworkEnable("FX/Thunder", base.transform.position + Vectors.up * 23f, Quaternion.Euler(270f, 0f, 0f), 0);
        }
        this.lifeTimeMax = (this.lifeTime = 30f);
    }

    public void hitByFT(int phase)
    {
        if (this.hasDied)
        {
            return;
        }
        this.isHit = true;
        this.hitAnimation = "hit_annie_" + phase;
        this.falseAttack();
        this.playAnimation(this.hitAnimation);
        this.needFreshCorePosition = true;
        if (phase == 3)
        {
            this.hasDied = true;
            Transform transform = base.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/neck");
            GameObject gameObject;
            if (IN_GAME_MAIN_CAMERA.GameType == GameType.MultiPlayer && PhotonNetwork.IsMasterClient)
            {
                gameObject = Optimization.Caching.Pool.NetworkEnable("bloodExplore", transform.position + Vectors.up * 1f * 4f, Quaternion.Euler(270f, 0f, 0f), 0);
            }
            else
            {
                gameObject = Pool.Enable("bloodExplore", transform.position + Vectors.up * 1f * 4f, Quaternion.Euler(270f, 0f, 0f));
                //(GameObject)UnityEngine.Object.Instantiate(CacheResources.Load("bloodExplore"), transform.position + Vectors.up * 1f * 4f, Quaternion.Euler(270f, 0f, 0f));
            }
            gameObject.transform.localScale = base.transform.localScale;
            if (IN_GAME_MAIN_CAMERA.GameType == GameType.MultiPlayer && PhotonNetwork.IsMasterClient)
            {
                gameObject = Optimization.Caching.Pool.NetworkEnable("bloodsplatter", transform.position, Quaternion.Euler(90f + transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z), 0);
            }
            else
            {
                gameObject = Pool.Enable("bloodsplatter", transform.position, Quaternion.Euler(90f + transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z));
                //(GameObject)UnityEngine.Object.Instantiate(CacheResources.Load("bloodsplatter"), transform.position, Quaternion.Euler(90f + transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z));
            }
            gameObject.transform.localScale = base.transform.localScale;
            gameObject.transform.parent = transform;
            if (IN_GAME_MAIN_CAMERA.GameType == GameType.MultiPlayer && PhotonNetwork.IsMasterClient)
            {
                gameObject = Optimization.Caching.Pool.NetworkEnable("FX/justSmoke", transform.position, Quaternion.Euler(270f, 0f, 0f), 0);
            }
            else
            {
                gameObject = Pool.Enable("FX/justSmoke", transform.position, Quaternion.Euler(270f, 0f, 0f));
                //(GameObject)UnityEngine.Object.Instantiate(CacheResources.Load("FX/justSmoke"), transform.position, Quaternion.Euler(270f, 0f, 0f));
            }
            gameObject.transform.parent = transform;
        }
    }

    public void hitByFTByServer(int phase)
    {
        BasePV.RPC("hitByFTRPC", PhotonTargets.All, new object[]
        {
            phase
        });
    }

    public void hitByTitan()
    {
        if (this.isHit)
        {
            return;
        }
        if (this.hasDied)
        {
            return;
        }
        if (base.animation.IsPlaying("born"))
        {
            return;
        }
        if (this.rockLift)
        {
            this.crossFade("die", 0.1f);
            this.isHitWhileCarryingRock = true;
            FengGameManagerMKII.FGM.GameLose();
            BasePV.RPC("rockPlayAnimation", PhotonTargets.All, new object[]
            {
                "set"
            });
            return;
        }
        this.isHit = true;
        this.hitAnimation = "hit_titan";
        this.falseAttack();
        this.playAnimation(this.hitAnimation);
        this.needFreshCorePosition = true;
    }

    public void hitByTitanByServer()
    {
        BasePV.RPC("hitByTitanRPC", PhotonTargets.All, new object[0]);
    }

    public bool IsGrounded()
    {
        return this.bottomObject.GetComponent<CheckHitGround>().isGrounded;
    }

    public void LateUpdate()
    {
        if (IN_GAME_MAIN_CAMERA.isPausing && IN_GAME_MAIN_CAMERA.GameType == GameType.Single)
        {
            return;
        }
        if (this.rockLift)
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
        Quaternion to = Quaternion.Euler(IN_GAME_MAIN_CAMERA.BaseCamera.transform.rotation.eulerAngles.x, IN_GAME_MAIN_CAMERA.BaseCamera.transform.rotation.eulerAngles.y, 0f);
        IN_GAME_MAIN_CAMERA.BaseCamera.transform.rotation = Quaternion.Lerp(IN_GAME_MAIN_CAMERA.BaseCamera.transform.rotation, to, Time.deltaTime * 2f);
    }

    public void playAnimation(string aniName)
    {
        base.animation.Play(aniName);
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

    public void setRoute()
    {
        GameObject gameObject = CacheGameObject.Find("routeTrost");
        this.checkPoints = new ArrayList();
        for (int i = 1; i <= 7; i++)
        {
            this.checkPoints.Add(gameObject.transform.Find("r" + i).position);
        }
        this.checkPoints.Add("end");
    }

    public void Update()
    {
        if (IN_GAME_MAIN_CAMERA.isPausing && IN_GAME_MAIN_CAMERA.GameType == GameType.Single)
        {
            return;
        }
        if (this.rockLift)
        {
            return;
        }
        if (base.animation.IsPlaying("run"))
        {
            if (base.animation["run"].normalizedTime % 1f > 0.3f && base.animation["run"].normalizedTime % 1f < 0.75f && this.stepSoundPhase == 2)
            {
                this.stepSoundPhase = 1;
                Transform transform = base.transform.Find("snd_eren_foot");
                transform.GetComponent<AudioSource>().Stop();
                transform.GetComponent<AudioSource>().Play();
            }
            if (base.animation["run"].normalizedTime % 1f > 0.75f && this.stepSoundPhase == 1)
            {
                this.stepSoundPhase = 2;
                Transform transform2 = base.transform.Find("snd_eren_foot");
                transform2.GetComponent<AudioSource>().Stop();
                transform2.GetComponent<AudioSource>().Play();
            }
        }
        if (IN_GAME_MAIN_CAMERA.GameType != GameType.Single)
        {
            if (!BasePV.IsMine)
            {
                return;
            }
        }
        if (this.hasDied)
        {
            if (base.animation["die"].normalizedTime >= 1f || this.hitAnimation == "hit_annie_3")
            {
                if (this.realBody != null)
                {
                    this.realBody.GetComponent<HERO>().BackToHuman();
                    this.realBody.transform.position = base.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/neck").position + Vectors.up * 2f;
                    this.realBody = null;
                }
                this.dieTime += Time.deltaTime;
                if (this.dieTime > 2f && !this.hasDieSteam)
                {
                    this.hasDieSteam = true;
                    if (IN_GAME_MAIN_CAMERA.GameType == GameType.Single)
                    {
                        GameObject gameObject = Pool.Enable("FX/FXtitanDie1", base.transform.Find("Amarture/Core/Controller_Body/hip").position, Quaternion.identity);//(GameObject)UnityEngine.Object.Instantiate(CacheResources.Load("FX/FXtitanDie1"));
                        //gameObject.transform.position = base.transform.Find("Amarture/Core/Controller_Body/hip").position;
                        gameObject.transform.localScale = base.transform.localScale;
                    }
                    else if (BasePV.IsMine)
                    {
                        GameObject gameObject2 = Optimization.Caching.Pool.NetworkEnable("FX/FXtitanDie1", base.transform.Find("Amarture/Core/Controller_Body/hip").position, Quaternion.Euler(-90f, 0f, 0f), 0);
                        gameObject2.transform.localScale = base.transform.localScale;
                    }
                }
                if (this.dieTime > 5f)
                {
                    if (IN_GAME_MAIN_CAMERA.GameType == GameType.Single)
                    {
                        GameObject gameObject3 = Pool.Enable("FX/FXtitanDie", base.transform.Find("Amarture/Core/Controller_Body/hip").position, Quaternion.identity);//(GameObject)UnityEngine.Object.Instantiate(CacheResources.Load("FX/FXtitanDie"));
                        //gameObject3.transform.position = base.transform.Find("Amarture/Core/Controller_Body/hip").position;
                        gameObject3.transform.localScale = base.transform.localScale;
                        UnityEngine.Object.Destroy(base.gameObject);
                    }
                    else if (BasePV.IsMine)
                    {
                        GameObject gameObject4 = Optimization.Caching.Pool.NetworkEnable("FX/FXtitanDie", base.transform.Find("Amarture/Core/Controller_Body/hip").position, Quaternion.Euler(-90f, 0f, 0f), 0);
                        gameObject4.transform.localScale = base.transform.localScale;
                        PhotonNetwork.Destroy(BasePV);
                    }
                    return;
                }
            }
            return;
        }
        if (IN_GAME_MAIN_CAMERA.GameType != GameType.Single && !BasePV.IsMine)
        {
            return;
        }
        if (!this.isHit)
        {
            if (this.lifeTime > 0f)
            {
                this.lifeTime -= Time.deltaTime;
                if (this.lifeTime <= 0f)
                {
                    this.hasDied = true;
                    this.playAnimation("die");
                    return;
                }
            }
            if (this.grounded && !this.isAttack && !base.animation.IsPlaying("jump_land") && !this.isAttack && !base.animation.IsPlaying("born"))
            {
                if (InputManager.IsInputDown[InputCode.Attack0] || InputManager.IsInputDown[InputCode.Attack1])
                {
                    bool flag = false;
                    if ((IN_GAME_MAIN_CAMERA.CameraMode == CameraType.WOW && InputManager.IsInput[InputCode.Down]) || InputManager.IsInputDown[InputCode.Attack1])
                    {
                        if (IN_GAME_MAIN_CAMERA.CameraMode == CameraType.WOW && InputManager.IsInputDown[InputCode.Attack1] && InputManager.Settings[InputCode.Attack1].Value == KeyCode.Mouse1)
                        {
                            flag = true;
                        }
                        if (flag)
                        {
                            flag = true;
                        }
                        else
                        {
                            this.attackAnimation = "attack_kick";
                        }
                    }
                    else
                    {
                        this.attackAnimation = "attack_combo_001";
                    }
                    if (!flag)
                    {
                        this.playAnimation(this.attackAnimation);
                        base.animation[this.attackAnimation].time = 0f;
                        this.isAttack = true;
                        this.needFreshCorePosition = true;
                        if (this.attackAnimation == "attack_combo_001" || this.attackAnimation == "attack_combo_001")
                        {
                            this.attackBox = base.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/shoulder_R/upper_arm_R/forearm_R/hand_R");
                        }
                        else if (this.attackAnimation == "attack_combo_002")
                        {
                            this.attackBox = base.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/shoulder_L/upper_arm_L/forearm_L/hand_L");
                        }
                        else if (this.attackAnimation == "attack_kick")
                        {
                            this.attackBox = base.transform.Find("Amarture/Core/Controller_Body/hip/thigh_R/shin_R/foot_R");
                        }
                        this.hitTargets = new ArrayList();
                    }
                }
                if (InputManager.IsInputDown[InputCode.Salute])
                {
                    this.crossFade("born", 0.1f);
                    base.animation["born"].normalizedTime = 0.28f;
                    this.isPlayRoar = false;
                }
            }
            if (!this.isAttack)
            {
                if ((this.grounded || base.animation.IsPlaying("idle")) && !base.animation.IsPlaying("jump_start") && !base.animation.IsPlaying("jump_air") && !base.animation.IsPlaying("jump_land") && InputManager.IsInput[InputCode.BothRope])
                {
                    this.crossFade("jump_start", 0.1f);
                }
            }
            else
            {
                if (base.animation[this.attackAnimation].time >= 0.1f && InputManager.IsInputDown[InputCode.Attack0])
                {
                    this.isNextAttack = true;
                }
                float num = 0f;
                string text = string.Empty;
                float num2;
                float num3;
                if (this.attackAnimation == "attack_combo_001")
                {
                    num2 = 0.4f;
                    num3 = 0.5f;
                    num = 0.66f;
                    text = "attack_combo_002";
                }
                else if (this.attackAnimation == "attack_combo_002")
                {
                    num2 = 0.15f;
                    num3 = 0.25f;
                    num = 0.43f;
                    text = "attack_combo_003";
                }
                else if (this.attackAnimation == "attack_combo_003")
                {
                    num = 0f;
                    num2 = 0.31f;
                    num3 = 0.37f;
                }
                else if (this.attackAnimation == "attack_kick")
                {
                    num = 0f;
                    num2 = 0.32f;
                    num3 = 0.38f;
                }
                else
                {
                    num2 = 0.5f;
                    num3 = 0.85f;
                }
                if (this.hitPause > 0f)
                {
                    this.hitPause -= Time.deltaTime;
                    if (this.hitPause <= 0f)
                    {
                        base.animation[this.attackAnimation].speed = 1f;
                        this.hitPause = 0f;
                    }
                }
                if (num > 0f && this.isNextAttack && base.animation[this.attackAnimation].normalizedTime >= num)
                {
                    if (this.hitTargets.Count > 0)
                    {
                        Transform transform3 = (Transform)this.hitTargets[0];
                        if (transform3)
                        {
                            base.transform.rotation = Quaternion.Euler(0f, Quaternion.LookRotation(transform3.position - base.transform.position).eulerAngles.y, 0f);
                            this.facingDirection = base.transform.rotation.eulerAngles.y;
                        }
                    }
                    this.falseAttack();
                    this.attackAnimation = text;
                    this.crossFade(this.attackAnimation, 0.1f);
                    base.animation[this.attackAnimation].time = 0f;
                    base.animation[this.attackAnimation].speed = 1f;
                    this.isAttack = true;
                    this.needFreshCorePosition = true;
                    if (this.attackAnimation == "attack_combo_002")
                    {
                        this.attackBox = base.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/shoulder_L/upper_arm_L/forearm_L/hand_L");
                    }
                    else if (this.attackAnimation == "attack_combo_003")
                    {
                        this.attackBox = base.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/shoulder_R/upper_arm_R/forearm_R/hand_R");
                    }
                    this.hitTargets = new ArrayList();
                }
                if ((base.animation[this.attackAnimation].normalizedTime >= num2 && base.animation[this.attackAnimation].normalizedTime <= num3) || (!this.attackChkOnce && base.animation[this.attackAnimation].normalizedTime >= num2))
                {
                    if (!this.attackChkOnce)
                    {
                        if (this.attackAnimation == "attack_combo_002")
                        {
                            this.playSound("snd_eren_swing2");
                        }
                        else if (this.attackAnimation == "attack_combo_001")
                        {
                            this.playSound("snd_eren_swing1");
                        }
                        else if (this.attackAnimation == "attack_combo_003")
                        {
                            this.playSound("snd_eren_swing3");
                        }
                        this.attackChkOnce = true;
                    }
                    Collider[] array = Physics.OverlapSphere(this.attackBox.transform.position, 8f);
                    for (int i = 0; i < array.Length; i++)
                    {
                        if (array[i].gameObject.transform.root.GetComponent<TITAN>())
                        {
                            bool flag2 = false;
                            for (int j = 0; j < this.hitTargets.Count; j++)
                            {
                                if (array[i].gameObject.transform.root.Equals(hitTargets[j]))
                                {
                                    flag2 = true;
                                    break;
                                }
                            }
                            if (!flag2)
                            {
                                if (!array[i].gameObject.transform.root.GetComponent<TITAN>().hasDie)
                                {
                                    base.animation[this.attackAnimation].speed = 0f;
                                    if (this.attackAnimation == "attack_combo_002")
                                    {
                                        this.hitPause = 0.05f;
                                        array[i].gameObject.transform.root.GetComponent<TITAN>().HitL(base.transform.position, this.hitPause);
                                        IN_GAME_MAIN_CAMERA.MainCamera.startShake(1f, 0.03f, 0.95f);
                                    }
                                    else if (this.attackAnimation == "attack_combo_001")
                                    {
                                        IN_GAME_MAIN_CAMERA.MainCamera.startShake(1.2f, 0.04f, 0.95f);
                                        this.hitPause = 0.08f;
                                        array[i].gameObject.transform.root.GetComponent<TITAN>().HitR(base.transform.position, this.hitPause);
                                    }
                                    else if (this.attackAnimation == "attack_combo_003")
                                    {
                                        IN_GAME_MAIN_CAMERA.MainCamera.startShake(3f, 0.1f, 0.95f);
                                        this.hitPause = 0.3f;
                                        array[i].gameObject.transform.root.GetComponent<TITAN>().DieHeadBlow(base.transform.position, this.hitPause);
                                    }
                                    else if (this.attackAnimation == "attack_kick")
                                    {
                                        IN_GAME_MAIN_CAMERA.MainCamera.startShake(3f, 0.1f, 0.95f);
                                        this.hitPause = 0.2f;
                                        if (array[i].gameObject.transform.root.GetComponent<TITAN>().abnormalType == AbnormalType.Crawler)
                                        {
                                            array[i].gameObject.transform.root.GetComponent<TITAN>().DieBlow(base.transform.position, this.hitPause);
                                        }
                                        else if (array[i].gameObject.transform.root.transform.localScale.x < 2f)
                                        {
                                            array[i].gameObject.transform.root.GetComponent<TITAN>().DieBlow(base.transform.position, this.hitPause);
                                        }
                                        else
                                        {
                                            array[i].gameObject.transform.root.GetComponent<TITAN>().HitR(base.transform.position, this.hitPause);
                                        }
                                    }
                                    this.hitTargets.Add(array[i].gameObject.transform.root);
                                    if (IN_GAME_MAIN_CAMERA.GameType != GameType.Single)
                                    {
                                        Optimization.Caching.Pool.NetworkEnable("hitMeatBIG", (array[i].transform.position + this.attackBox.position) * 0.5f, Quaternion.Euler(270f, 0f, 0f), 0);
                                    }
                                    else
                                    {
                                        Pool.Enable("hitMeatBIG", (array[i].transform.position + this.attackBox.position) * 0.5f, Quaternion.Euler(270f, 0f, 0f));//UnityEngine.Object.Instantiate(CacheResources.Load("hitMeatBIG"), (array[i].transform.position + this.attackBox.position) * 0.5f, Quaternion.Euler(270f, 0f, 0f));
                                    }
                                }
                            }
                        }
                    }
                }
                if (base.animation[this.attackAnimation].normalizedTime >= 1f)
                {
                    this.falseAttack();
                    this.playAnimation("idle");
                }
            }
            if (base.animation.IsPlaying("jump_land") && base.animation["jump_land"].normalizedTime >= 1f)
            {
                this.crossFade("idle", 0.1f);
            }
            if (base.animation.IsPlaying("born"))
            {
                if (base.animation["born"].normalizedTime >= 0.28f && !this.isPlayRoar)
                {
                    this.isPlayRoar = true;
                    this.playSound("snd_eren_roar");
                }
                if (base.animation["born"].normalizedTime >= 0.5f && base.animation["born"].normalizedTime <= 0.7f)
                {
                    IN_GAME_MAIN_CAMERA.MainCamera.startShake(0.5f, 1f, 0.95f);
                }
                if (base.animation["born"].normalizedTime >= 1f)
                {
                    this.crossFade("idle", 0.1f);
                    if (IN_GAME_MAIN_CAMERA.GameType != GameType.Single)
                    {
                        if (PhotonNetwork.IsMasterClient)
                        {
                            BasePV.RPC("netTauntAttack", PhotonTargets.MasterClient, new object[]
                            {
                                10f,
                                500f
                            });
                        }
                        else
                        {
                            this.netTauntAttack(10f, 500f);
                        }
                    }
                    else
                    {
                        this.netTauntAttack(10f, 500f);
                    }
                }
            }
            this.showAimUI();
            this.showSkillCD();
            return;
        }
        if (base.animation[this.hitAnimation].normalizedTime >= 1f)
        {
            this.isHit = false;
            this.falseAttack();
            this.playAnimation("idle");
            return;
        }
    }
}