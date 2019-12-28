using Optimization.Caching;
using UnityEngine;

[AddComponentMenu("NGUI/Tween/Spring Position")]
public class SpringPosition : IgnoreTimeScale
{
    private float mThreshold;
    private Transform mTrans;
    public string callWhenFinished;
    public GameObject eventReceiver;
    public bool ignoreTimeScale;
    public SpringPosition.OnFinished onFinished;
    public float strength = 10f;
    public Vector3 target = Vectors.zero;
    public bool worldSpace;

    public delegate void OnFinished(SpringPosition spring);

    private void Start()
    {
        this.mTrans = base.transform;
    }

    private void Update()
    {
        float deltaTime = (!this.ignoreTimeScale) ? Time.deltaTime : base.UpdateRealTimeDelta();
        if (this.worldSpace)
        {
            if (this.mThreshold == 0f)
            {
                this.mThreshold = (this.target - this.mTrans.position).magnitude * 0.001f;
            }
            this.mTrans.position = NGUIMath.SpringLerp(this.mTrans.position, this.target, this.strength, deltaTime);
            if (this.mThreshold >= (this.target - this.mTrans.position).magnitude)
            {
                this.mTrans.position = this.target;
                if (this.onFinished != null)
                {
                    this.onFinished(this);
                }
                if (this.eventReceiver != null && !string.IsNullOrEmpty(this.callWhenFinished))
                {
                    this.eventReceiver.SendMessage(this.callWhenFinished, this, SendMessageOptions.DontRequireReceiver);
                }
                base.enabled = false;
            }
        }
        else
        {
            if (this.mThreshold == 0f)
            {
                this.mThreshold = (this.target - this.mTrans.localPosition).magnitude * 0.001f;
            }
            this.mTrans.localPosition = NGUIMath.SpringLerp(this.mTrans.localPosition, this.target, this.strength, deltaTime);
            if (this.mThreshold >= (this.target - this.mTrans.localPosition).magnitude)
            {
                this.mTrans.localPosition = this.target;
                if (this.onFinished != null)
                {
                    this.onFinished(this);
                }
                if (this.eventReceiver != null && !string.IsNullOrEmpty(this.callWhenFinished))
                {
                    this.eventReceiver.SendMessage(this.callWhenFinished, this, SendMessageOptions.DontRequireReceiver);
                }
                base.enabled = false;
            }
        }
    }

    public static SpringPosition Begin(GameObject go, Vector3 pos, float strength)
    {
        SpringPosition springPosition = go.GetComponent<SpringPosition>();
        if (springPosition == null)
        {
            springPosition = go.AddComponent<SpringPosition>();
        }
        springPosition.target = pos;
        springPosition.strength = strength;
        springPosition.onFinished = null;
        if (!springPosition.enabled)
        {
            springPosition.mThreshold = 0f;
            springPosition.enabled = true;
        }
        return springPosition;
    }
}