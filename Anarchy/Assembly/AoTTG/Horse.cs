//Temporarily horse rework is stopped.

using Optimization.Caching;
using System.Linq;
using UnityEngine;

public class Horse : Photon.MonoBehaviour
{
    private readonly Vector3 heroOffsetVector = Vectors.up * 1.68f;
    private float awayTimer;
    private Animation baseA;
    private Rigidbody baseR;
    private Transform baseT;
    private TITAN_CONTROLLER controller;
    private ParticleSystem dustParticle;
    private Vector3 setPoint;
    private float speed = 45f;
    private float timeElapsed;
    public GameObject dust;
    public HERO Owner;
    public GameObject myHero;
    public HorseState State = HorseState.Idle;
    private float _idleTime = 0f;

    private void Awake()
    {
        baseA = GetComponent<Animation>();
        baseR = GetComponent<Rigidbody>();
        baseT = GetComponent<Transform>();
        dustParticle = dust.GetComponent<ParticleSystem>();
    }

    private void CrossFade(string aniName, float time)
    {   
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

    private void Followed()
    {
        if (Owner == null)
        {
            return;
        }
        State = HorseState.Follow;
        float[] randoms = new float[2].Select(x => Random.Range(-6f, 6f)).ToArray();
        setPoint = Owner.baseT.position + Vectors.right * randoms[0] + Vectors.forward * randoms[1];
        setPoint.y = GetHeight(setPoint + Vectors.up * 5f);
        awayTimer = 0f;
    }

    private float GetHeight(Vector3 pt)
    {
        RaycastHit raycastHit;
        if (Physics.Raycast(pt, -Vectors.up, out raycastHit, 1000f, Layers.Ground.value))
        {
            return raycastHit.point.y;
        }
        return 0f;
    }

    private void LateUpdate()
    {
        if (Owner == null && BasePV.IsMine)
        {
            PhotonNetwork.Destroy(gameObject);
            return;
        }

        switch (State)
        {
            case HorseState.Mounted:
                {
                    if (this.Owner == null)
                    {
                        this.Unmounted();
                        return;
                    }

                    Owner.baseT.position = baseT.position + heroOffsetVector;
                    Owner.baseT.rotation = baseR.rotation;
                    Owner.baseR.velocity = baseR.velocity;

                    if (controller.targetDirection == -874f)
                    {
                        this.ToIdleAnimation();
                        if (baseR.velocity.magnitude > 15f)
                        {
                            if (!Owner.baseA.IsPlaying("horse_run"))
                            {
                                Owner.CrossFade("horse_run", 0.1f);
                            }
                        }
                        else if (!Owner.baseA.IsPlaying("horse_idle"))
                        {
                            Owner.CrossFade("horse_idle", 0.1f);
                        }
                    }
                    else
                    {
                        base.gameObject.transform.rotation = Quaternion.Lerp(base.gameObject.transform.rotation, Quaternion.Euler(0f, this.controller.targetDirection, 0f), 100f * Time.deltaTime / (base.rigidbody.velocity.magnitude + 20f));
                        if (this.controller.isWALKDown)
                        {
                            baseR.AddForce(baseT.Forward() * this.speed * 0.6f, ForceMode.Acceleration);
                            if (baseR.velocity.magnitude >= this.speed * 0.6f)
                            {
                                baseR.AddForce(-this.speed * 0.6f * baseR.velocity.normalized, ForceMode.Acceleration);
                            }
                        }
                        else
                        {
                            baseR.AddForce(baseT.Forward() * this.speed, ForceMode.Acceleration);
                            if (baseR.velocity.magnitude >= this.speed)
                            {
                                baseR.AddForce(-this.speed * baseR.velocity.normalized, ForceMode.Acceleration);
                            }
                        }
                        if (baseR.velocity.magnitude > 8f)
                        {
                            if (!baseA.IsPlaying("horse_Run"))
                            {
                                this.CrossFade("horse_Run", 0.1f);
                            }
                            if (!this.Owner.baseA.IsPlaying("horse_run"))
                            {
                                this.Owner.CrossFade("horse_run", 0.1f);
                            }
                            if (!this.dustParticle.enableEmission)
                            {
                                this.dustParticle.enableEmission = true;
                                BasePV.RPC("setDust", PhotonTargets.Others, new object[]
                                {
                        true
                                });
                            }
                        }
                        else
                        {
                            if (!baseA.IsPlaying("horse_WALK"))
                            {
                                this.CrossFade("horse_WALK", 0.1f);
                            }
                            if (!this.Owner.baseA.IsPlaying("horse_idle"))
                            {
                                this.Owner.baseA.CrossFade("horse_idle", 0.1f);
                            }
                            if (this.dustParticle.enableEmission)
                            {
                                this.dustParticle.enableEmission = false;
                                BasePV.RPC("setDust", PhotonTargets.Others, new object[]
                                {
                                    false
                                });
                            }
                        }
                    }

                    if ((this.controller.isAttackDown || this.controller.isAttackIIDown) && this.IsGrounded())
                    {
                        baseR.AddForce(Vectors.up * 25f, ForceMode.VelocityChange);
                    }
                }
                break;

            case HorseState.Follow:
                {
                    if (this.Owner == null)
                    {
                        this.Unmounted();
                        return;
                    }

                    if (baseR.velocity.magnitude > 8f)
                    {
                        if (!baseA.IsPlaying("horse_Run"))
                        {
                            this.CrossFade("horse_Run", 0.1f);
                        }
                        if (!this.dustParticle.enableEmission)
                        {
                            this.dustParticle.enableEmission = true;
                            BasePV.RPC("setDust", PhotonTargets.Others, new object[]
                            {
                        true
                            });
                        }
                    }
                    else
                    {
                        if (!baseA.IsPlaying("horse_WALK"))
                        {
                            this.CrossFade("horse_WALK", 0.1f);
                        }
                        if (this.dustParticle.enableEmission)
                        {
                            this.dustParticle.enableEmission = false;
                            BasePV.RPC("setDust", PhotonTargets.Others, new object[]
                            {
                                false
                            });
                        }
                    }

                    float num = -Mathf.DeltaAngle(FengMath.GetHorizontalAngle(baseT.position, this.setPoint), base.gameObject.transform.rotation.eulerAngles.y - 90f);
                    base.gameObject.transform.rotation = Quaternion.Lerp(base.gameObject.transform.rotation, Quaternion.Euler(0f, base.gameObject.transform.rotation.eulerAngles.y + num, 0f), 200f * Time.deltaTime / (baseR.velocity.magnitude + 20f));
                    if (Vector3.Distance(this.setPoint, baseT.position) < 20f)
                    {
                        baseR.AddForce(baseT.Forward() * this.speed * 0.7f, ForceMode.Acceleration);
                        if (baseR.velocity.magnitude >= this.speed)
                        {
                            baseR.AddForce(-this.speed * 0.7f * baseR.velocity.normalized, ForceMode.Acceleration);
                        }
                    }
                    else
                    {
                        baseR.AddForce(base.transform.Forward() * this.speed, ForceMode.Acceleration);
                        if (baseR.velocity.magnitude >= this.speed)
                        {
                            baseR.AddForce(-this.speed * baseR.velocity.normalized, ForceMode.Acceleration);
                        }
                    }
                    this.timeElapsed += Time.deltaTime;
                    if (this.timeElapsed > 0.6f)
                    {
                        this.timeElapsed = 0f;
                        if (Vector3.Distance(this.Owner.baseT.position, this.setPoint) > 20f)
                        {
                            this.Followed();
                        }
                    }
                    if (Vector3.Distance(this.Owner.baseT.position, baseT.position) < 5f)
                    {
                        this.Unmounted();
                    }
                    if (Vector3.Distance(this.setPoint, baseT.position) < 5f)
                    {
                        this.Unmounted();
                    }
                    this.awayTimer += Time.deltaTime;
                    if (this.awayTimer > 6f)
                    {
                        this.awayTimer = 0f;
                        if (Physics.Linecast(baseT.position + Vectors.up, this.Owner.baseT.position + Vectors.up, Layers.Ground.value))
                        {
                            baseT.position = new Vector3(this.Owner.baseT.position.x, this.GetHeight(this.Owner.baseT.position + Vectors.up * 5f), this.Owner.baseT.position.z);
                        }
                    }
                }
                break;

            case HorseState.Idle:
                {
                    this.ToIdleAnimation();
                    if (this.Owner != null && Vector3.Distance(this.Owner.baseT.position, baseT.position) > 20f)
                    {
                        this.Followed();
                    }
                }
                break;
        }
        baseR.AddForce(new Vector3(0f, -50f * baseR.mass, 0f));
    }

    [RPC]
    private void netCrossFade(string aniName, float time)
    {
        baseA.CrossFade(aniName, time);
    }

    [RPC]
    private void netPlayAnimation(string aniName)
    {
        baseA.Play(aniName);
    }

    [RPC]
    private void netPlayAnimationAt(string aniName, float normalizedTime)
    {
        baseA.Play(aniName);
        baseA[aniName].normalizedTime = normalizedTime;
    }

    private void PlayAnimationAt(string aniName, float normalizedTime)
    {
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

    [RPC]
    private void setDust(bool enable)
    {
        if (this.dustParticle.enableEmission)
        {
            this.dustParticle.enableEmission = enable;
        }
    }

    private void Start()
    {
        this.controller = base.gameObject.GetComponent<TITAN_CONTROLLER>();
    }

    private void ToIdleAnimation()
    {
        if (baseR.velocity.magnitude > 0.1f)
        {
            if (baseR.velocity.magnitude > 15f)
            {
                if (!baseA.IsPlaying("horse_Run"))
                {
                    this.CrossFade("horse_Run", 0.1f);
                }
                if (!this.dustParticle.enableEmission)
                {
                    this.dustParticle.enableEmission = true;
                    BasePV.RPC("setDust", PhotonTargets.Others, new object[]
                    {
                        true
                    });
                }
            }
            else
            {
                if (!baseA.IsPlaying("horse_WALK"))
                {
                    this.CrossFade("horse_WALK", 0.1f);
                }
                if (this.dustParticle.enableEmission)
                {
                    this.dustParticle.enableEmission = false;
                    BasePV.RPC("setDust", PhotonTargets.Others, new object[]
                    {
                        false
                    });
                }
            }
        }
        else
        {
            if (_idleTime <= 0f)
            {
                if (base.animation.IsPlaying("horse_idle0"))
                {
                    float num = UnityEngine.Random.Range(0f, 1f);
                    if (num < 0.33f)
                    {
                        CrossFade("horse_idle1", 0.1f);
                    }
                    else if (num < 0.66f)
                    {
                        CrossFade("horse_idle2", 0.1f);
                    }
                    else
                    {
                        CrossFade("horse_idle3", 0.1f);
                    }
                    _idleTime = 1f;
                }
                else
                {
                    CrossFade("horse_idle0", 0.1f);
                    _idleTime = UnityEngine.Random.Range(1f, 4f);
                }
            }
            if (this.dustParticle.enableEmission)
            {
                this.dustParticle.enableEmission = false;
                BasePV.RPC("setDust", PhotonTargets.Others, new object[]
                {
                    false
                });
            }
            _idleTime += Time.deltaTime;
            //base.rigidbody.AddForce(-base.rigidbody.velocity, ForceMode.VelocityChange);
        }
    }

    public bool IsGrounded()
    {
        return Physics.Raycast(base.gameObject.transform.position + Vectors.up * 0.1f, -Vectors.up, 0.3f, Layers.EnemyGround.value);
    }

    public void Mounted()
    {
        this.State = HorseState.Mounted;
        base.gameObject.GetComponent<TITAN_CONTROLLER>().enabled = true;
    }

    public void PlayAnimation(string aniName)
    {
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

    public void Unmounted()
    {
        this.State = HorseState.Idle;
        base.gameObject.GetComponent<TITAN_CONTROLLER>().enabled = false;
    }

    public enum HorseState
    {
        Idle,
        Follow,
        Mounted,
    }
}