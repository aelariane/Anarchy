using Optimization.Caching;
using UnityEngine;

public class Horse : Photon.MonoBehaviour
{
    private float awayTimer;
    private TITAN_CONTROLLER controller;
    private Vector3 setPoint;
    private float speed = 45f;
    private string State = "idle";
    private float timeElapsed;
    public GameObject dust;
    public GameObject myHero;

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

    private void followed()
    {
        if (this.myHero == null)
        {
            return;
        }
        this.State = "follow";
        this.setPoint = this.myHero.transform.position + Vectors.right * (float)UnityEngine.Random.Range(-6, 6) + Vectors.forward * (float)UnityEngine.Random.Range(-6, 6);
        this.setPoint.y = this.getHeight(this.setPoint + Vectors.up * 5f);
        this.awayTimer = 0f;
    }

    private float getHeight(Vector3 pt)
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
        if (this.myHero == null && BasePV.IsMine)
        {
            PhotonNetwork.Destroy(base.gameObject);
        }
        if (this.State == "mounted")
        {
            if (this.myHero == null)
            {
                this.unmounted();
                return;
            }
            this.myHero.transform.position = base.transform.position + Vectors.up * 1.68f;
            this.myHero.transform.rotation = base.transform.rotation;
            this.myHero.rigidbody.velocity = base.rigidbody.velocity;
            if (this.controller.targetDirection != -874f)
            {
                base.gameObject.transform.rotation = Quaternion.Lerp(base.gameObject.transform.rotation, Quaternion.Euler(0f, this.controller.targetDirection, 0f), 100f * Time.deltaTime / (base.rigidbody.velocity.magnitude + 20f));
                if (this.controller.isWALKDown)
                {
                    base.rigidbody.AddForce(base.transform.Forward() * this.speed * 0.6f, ForceMode.Acceleration);
                    if (base.rigidbody.velocity.magnitude >= this.speed * 0.6f)
                    {
                        base.rigidbody.AddForce(-this.speed * 0.6f * base.rigidbody.velocity.normalized, ForceMode.Acceleration);
                    }
                }
                else
                {
                    base.rigidbody.AddForce(base.transform.Forward() * this.speed, ForceMode.Acceleration);
                    if (base.rigidbody.velocity.magnitude >= this.speed)
                    {
                        base.rigidbody.AddForce(-this.speed * base.rigidbody.velocity.normalized, ForceMode.Acceleration);
                    }
                }
                if (base.rigidbody.velocity.magnitude > 8f)
                {
                    if (!base.animation.IsPlaying("horse_Run"))
                    {
                        this.crossFade("horse_Run", 0.1f);
                    }
                    if (!this.myHero.animation.IsPlaying("horse_Run"))
                    {
                        this.myHero.GetComponent<HERO>().crossFade("horse_run", 0.1f);
                    }
                    if (!this.dust.GetComponent<ParticleSystem>().enableEmission)
                    {
                        this.dust.GetComponent<ParticleSystem>().enableEmission = true;
                        BasePV.RPC("setDust", PhotonTargets.Others, new object[]
                        {
                            true
                        });
                    }
                }
                else
                {
                    if (!base.animation.IsPlaying("horse_WALK"))
                    {
                        this.crossFade("horse_WALK", 0.1f);
                    }
                    if (!this.myHero.animation.IsPlaying("horse_idle"))
                    {
                        this.myHero.GetComponent<HERO>().crossFade("horse_idle", 0.1f);
                    }
                    if (this.dust.GetComponent<ParticleSystem>().enableEmission)
                    {
                        this.dust.GetComponent<ParticleSystem>().enableEmission = false;
                        BasePV.RPC("setDust", PhotonTargets.Others, new object[]
                        {
                            false
                        });
                    }
                }
            }
            else
            {
                this.toIdleAnimation();
                if (base.rigidbody.velocity.magnitude > 15f)
                {
                    if (!this.myHero.animation.IsPlaying("horse_Run"))
                    {
                        this.myHero.GetComponent<HERO>().crossFade("horse_run", 0.1f);
                    }
                }
                else if (!this.myHero.animation.IsPlaying("horse_idle"))
                {
                    this.myHero.GetComponent<HERO>().crossFade("horse_idle", 0.1f);
                }
            }
            if ((this.controller.isAttackDown || this.controller.isAttackIIDown) && this.IsGrounded())
            {
                base.rigidbody.AddForce(Vectors.up * 25f, ForceMode.VelocityChange);
            }
        }
        else if (this.State == "follow")
        {
            if (this.myHero == null)
            {
                this.unmounted();
                return;
            }
            if (base.rigidbody.velocity.magnitude > 8f)
            {
                if (!base.animation.IsPlaying("horse_Run"))
                {
                    this.crossFade("horse_Run", 0.1f);
                }
                if (!this.dust.GetComponent<ParticleSystem>().enableEmission)
                {
                    this.dust.GetComponent<ParticleSystem>().enableEmission = true;
                    BasePV.RPC("setDust", PhotonTargets.Others, new object[]
                    {
                        true
                    });
                }
            }
            else
            {
                if (!base.animation.IsPlaying("horse_WALK"))
                {
                    this.crossFade("horse_WALK", 0.1f);
                }
                if (this.dust.GetComponent<ParticleSystem>().enableEmission)
                {
                    this.dust.GetComponent<ParticleSystem>().enableEmission = false;
                    BasePV.RPC("setDust", PhotonTargets.Others, new object[]
                    {
                        false
                    });
                }
            }
            float num = -Mathf.DeltaAngle(FengMath.getHorizontalAngle(base.transform.position, this.setPoint), base.gameObject.transform.rotation.eulerAngles.y - 90f);
            base.gameObject.transform.rotation = Quaternion.Lerp(base.gameObject.transform.rotation, Quaternion.Euler(0f, base.gameObject.transform.rotation.eulerAngles.y + num, 0f), 200f * Time.deltaTime / (base.rigidbody.velocity.magnitude + 20f));
            if (Vector3.Distance(this.setPoint, base.transform.position) < 20f)
            {
                base.rigidbody.AddForce(base.transform.Forward() * this.speed * 0.7f, ForceMode.Acceleration);
                if (base.rigidbody.velocity.magnitude >= this.speed)
                {
                    base.rigidbody.AddForce(-this.speed * 0.7f * base.rigidbody.velocity.normalized, ForceMode.Acceleration);
                }
            }
            else
            {
                base.rigidbody.AddForce(base.transform.Forward() * this.speed, ForceMode.Acceleration);
                if (base.rigidbody.velocity.magnitude >= this.speed)
                {
                    base.rigidbody.AddForce(-this.speed * base.rigidbody.velocity.normalized, ForceMode.Acceleration);
                }
            }
            this.timeElapsed += Time.deltaTime;
            if (this.timeElapsed > 0.6f)
            {
                this.timeElapsed = 0f;
                if (Vector3.Distance(this.myHero.transform.position, this.setPoint) > 20f)
                {
                    this.followed();
                }
            }
            if (Vector3.Distance(this.myHero.transform.position, base.transform.position) < 5f)
            {
                this.unmounted();
            }
            if (Vector3.Distance(this.setPoint, base.transform.position) < 5f)
            {
                this.unmounted();
            }
            this.awayTimer += Time.deltaTime;
            if (this.awayTimer > 6f)
            {
                this.awayTimer = 0f;
                if (Physics.Linecast(base.transform.position + Vectors.up, this.myHero.transform.position + Vectors.up, Layers.Ground.value))
                {
                    base.transform.position = new Vector3(this.myHero.transform.position.x, this.getHeight(this.myHero.transform.position + Vectors.up * 5f), this.myHero.transform.position.z);
                }
            }
        }
        else if (this.State == "idle")
        {
            this.toIdleAnimation();
            if (this.myHero != null && Vector3.Distance(this.myHero.transform.position, base.transform.position) > 20f)
            {
                this.followed();
            }
        }
        base.rigidbody.AddForce(new Vector3(0f, -50f * base.rigidbody.mass, 0f));
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

    [RPC]
    private void setDust(bool enable)
    {
        if (this.dust.GetComponent<ParticleSystem>().enableEmission)
        {
            this.dust.GetComponent<ParticleSystem>().enableEmission = enable;
        }
    }

    private void Start()
    {
        this.controller = base.gameObject.GetComponent<TITAN_CONTROLLER>();
    }

    private void toIdleAnimation()
    {
        if (base.rigidbody.velocity.magnitude > 0.1f)
        {
            if (base.rigidbody.velocity.magnitude > 15f)
            {
                if (!base.animation.IsPlaying("horse_Run"))
                {
                    this.crossFade("horse_Run", 0.1f);
                }
                if (!this.dust.GetComponent<ParticleSystem>().enableEmission)
                {
                    this.dust.GetComponent<ParticleSystem>().enableEmission = true;
                    BasePV.RPC("setDust", PhotonTargets.Others, new object[]
                    {
                        true
                    });
                }
            }
            else
            {
                if (!base.animation.IsPlaying("horse_WALK"))
                {
                    this.crossFade("horse_WALK", 0.1f);
                }
                if (this.dust.GetComponent<ParticleSystem>().enableEmission)
                {
                    this.dust.GetComponent<ParticleSystem>().enableEmission = false;
                    BasePV.RPC("setDust", PhotonTargets.Others, new object[]
                    {
                        false
                    });
                }
            }
        }
        else
        {
            if (base.animation.IsPlaying("horse_idle1") && base.animation["horse_idle1"].normalizedTime >= 1f)
            {
                this.crossFade("horse_idle0", 0.1f);
            }
            if (base.animation.IsPlaying("horse_idle2") && base.animation["horse_idle2"].normalizedTime >= 1f)
            {
                this.crossFade("horse_idle0", 0.1f);
            }
            if (base.animation.IsPlaying("horse_idle3") && base.animation["horse_idle3"].normalizedTime >= 1f)
            {
                this.crossFade("horse_idle0", 0.1f);
            }
            if (!base.animation.IsPlaying("horse_idle0") && !base.animation.IsPlaying("horse_idle1") && !base.animation.IsPlaying("horse_idle2") && !base.animation.IsPlaying("horse_idle3"))
            {
                this.crossFade("horse_idle0", 0.1f);
            }
            if (base.animation.IsPlaying("horse_idle0"))
            {
                int num = UnityEngine.Random.Range(0, 10000);
                if (num < 10)
                {
                    this.crossFade("horse_idle1", 0.1f);
                }
                else if (num < 20)
                {
                    this.crossFade("horse_idle2", 0.1f);
                }
                else if (num < 30)
                {
                    this.crossFade("horse_idle3", 0.1f);
                }
            }
            if (this.dust.GetComponent<ParticleSystem>().enableEmission)
            {
                this.dust.GetComponent<ParticleSystem>().enableEmission = false;
                BasePV.RPC("setDust", PhotonTargets.Others, new object[]
                {
                    false
                });
            }
            base.rigidbody.AddForce(-base.rigidbody.velocity, ForceMode.VelocityChange);
        }
    }

    public bool IsGrounded()
    {
        return Physics.Raycast(base.gameObject.transform.position + Vectors.up * 0.1f, -Vectors.up, 0.3f, Layers.EnemyGround.value);
    }

    public void mounted()
    {
        this.State = "mounted";
        base.gameObject.GetComponent<TITAN_CONTROLLER>().enabled = true;
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

    public void unmounted()
    {
        this.State = "idle";
        base.gameObject.GetComponent<TITAN_CONTROLLER>().enabled = false;
    }
}