using Optimization.Caching;
using UnityEngine;

public class FlareMovement : MonoBehaviour
{
    private GameObject hero;
    private GameObject hint;
    private bool nohint;
    private Vector3 offY;
    private float timer;
    public string color;

    private void Start()
    {
        this.hero = IN_GAME_MAIN_CAMERA.MainObject;
        if (!this.nohint && this.hero != null)
        {
            this.hint = (GameObject)UnityEngine.Object.Instantiate(CacheResources.Load("UI/" + this.color + "FlareHint"));
            if (this.color == "Black")
            {
                this.offY = Vectors.up * 0.4f;
            }
            else
            {
                this.offY = Vectors.up * 0.5f;
            }
            this.hint.transform.parent = base.transform.root;
            this.hint.transform.position = this.hero.transform.position + this.offY;
            Vector3 vector = base.transform.position - this.hint.transform.position;
            float num = Mathf.Atan2(-vector.z, vector.x) * 57.29578f;
            this.hint.transform.rotation = Quaternion.Euler(-90f, num + 180f, 0f);
            this.hint.transform.localScale = Vectors.zero;
            iTween.ScaleTo(this.hint, iTween.Hash(new object[]
            {
                "x",
                1f,
                "y",
                1f,
                "z",
                1f,
                "easetype",
                iTween.EaseType.easeOutElastic,
                "time",
                1f
            }));
            iTween.ScaleTo(this.hint, iTween.Hash(new object[]
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
                2.5f
            }));
        }
    }

    private void OnEnable()
    {
        this.hero = IN_GAME_MAIN_CAMERA.MainObject;
        if (!this.nohint && this.hero != null)
        {
            if (hint == null)
            {
                this.hint = (GameObject)UnityEngine.Object.Instantiate(CacheResources.Load("UI/" + this.color + "FlareHint"));
            }

            if (this.color == "Black")
            {
                this.offY = Vectors.up * 0.4f;
            }
            else
            {
                this.offY = Vectors.up * 0.5f;
            }
            this.hint.transform.parent = base.transform.root;
            this.hint.transform.position = this.hero.transform.position + this.offY;
            Vector3 vector = base.transform.position - this.hint.transform.position;
            float num = Mathf.Atan2(-vector.z, vector.x) * 57.29578f;
            this.hint.transform.rotation = Quaternion.Euler(-90f, num + 180f, 0f);
            this.hint.transform.localScale = Vectors.zero;
            iTween.ScaleTo(this.hint, iTween.Hash(new object[]
            {
                "x",
                1f,
                "y",
                1f,
                "z",
                1f,
                "easetype",
                iTween.EaseType.easeOutElastic,
                "time",
                1f
            }));
            iTween.ScaleTo(this.hint, iTween.Hash(new object[]
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
                2.5f
            }));
        }
        timer = 0f;
    }

    private void Update()
    {
        this.timer += Time.deltaTime;
        if (this.hint != null && hero != null)
        {
            if (this.timer < 3f)
            {
                this.hint.transform.position = this.hero.transform.position + this.offY;
                Vector3 vector = base.transform.position - this.hint.transform.position;
                float num = Mathf.Atan2(-vector.z, vector.x) * 57.29578f;
                this.hint.transform.rotation = Quaternion.Euler(-90f, num + 180f, 0f);
            }
            else if (this.hint != null)
            {
                UnityEngine.Object.Destroy(this.hint);
            }
        }
        if (this.timer < 4f)
        {
            base.rigidbody.AddForce((base.transform.Forward() + base.transform.Up() * 5f) * Time.deltaTime * 5f, ForceMode.VelocityChange);
        }
        else
        {
            base.rigidbody.AddForce(-base.transform.Up() * Time.deltaTime * 7f, ForceMode.Acceleration);
        }
    }

    public void dontShowHint()
    {
        UnityEngine.Object.Destroy(this.hint);
        this.nohint = true;
    }
}