using NGUI;
using UnityEngine;

public abstract class UITweener : IgnoreTimeScale
{
    private float mAmountPerDelta = 1f;

    private float mDuration;

    private float mFactor;

    private bool mStarted;

    private float mStartTime;

    public AnimationCurve animationCurve = new AnimationCurve(new Keyframe[]
                        {
        new Keyframe(0f, 0f, 0f, 1f),
        new Keyframe(1f, 1f, 1f, 0f)
    });

    public string callWhenFinished;
    public float delay;
    public float duration = 1f;
    public GameObject eventReceiver;
    public bool ignoreTimeScale = true;
    public UITweener.Method method;
    public UITweener.OnFinished onFinished;
    public bool steeperCurves;
    public UITweener.Style style;
    public int tweenGroup;

    public delegate void OnFinished(UITweener tween);

    public enum Method
    {
        Linear,
        EaseIn,
        EaseOut,
        EaseInOut,
        BounceIn,
        BounceOut
    }

    public enum Style
    {
        Once,
        Loop,
        PingPong
    }

    public float amountPerDelta
    {
        get
        {
            if (this.mDuration != this.duration)
            {
                this.mDuration = this.duration;
                this.mAmountPerDelta = Mathf.Abs((this.duration <= 0f) ? 1000f : (1f / this.duration));
            }
            return this.mAmountPerDelta;
        }
    }

    public Direction direction
    {
        get
        {
            return (this.mAmountPerDelta >= 0f) ? Direction.Forward : Direction.Reverse;
        }
    }

    public float tweenFactor
    {
        get
        {
            return this.mFactor;
        }
    }

    private float BounceLogic(float val)
    {
        if (val < 0.363636f)
        {
            val = 7.5685f * val * val;
        }
        else if (val < 0.727272f)
        {
            val = 7.5625f * (val -= 0.545454f) * val + 0.75f;
        }
        else if (val < 0.90909f)
        {
            val = 7.5625f * (val -= 0.818181f) * val + 0.9375f;
        }
        else
        {
            val = 7.5625f * (val -= 0.9545454f) * val + 0.984375f;
        }
        return val;
    }

    private void OnDisable()
    {
        this.mStarted = false;
    }

    private void Start()
    {
        this.Update();
    }

    private void Update()
    {
        float num = (!this.ignoreTimeScale) ? Time.deltaTime : base.UpdateRealTimeDelta();
        float num2 = (!this.ignoreTimeScale) ? Time.time : base.realTime;
        if (!this.mStarted)
        {
            this.mStarted = true;
            this.mStartTime = num2 + this.delay;
        }
        if (num2 < this.mStartTime)
        {
            return;
        }
        this.mFactor += this.amountPerDelta * num;
        if (this.style == UITweener.Style.Loop)
        {
            if (this.mFactor > 1f)
            {
                this.mFactor -= Mathf.Floor(this.mFactor);
            }
        }
        else if (this.style == UITweener.Style.PingPong)
        {
            if (this.mFactor > 1f)
            {
                this.mFactor = 1f - (this.mFactor - Mathf.Floor(this.mFactor));
                this.mAmountPerDelta = -this.mAmountPerDelta;
            }
            else if (this.mFactor < 0f)
            {
                this.mFactor = -this.mFactor;
                this.mFactor -= Mathf.Floor(this.mFactor);
                this.mAmountPerDelta = -this.mAmountPerDelta;
            }
        }
        if (this.style == UITweener.Style.Once && (this.mFactor > 1f || this.mFactor < 0f))
        {
            this.mFactor = Mathf.Clamp01(this.mFactor);
            this.Sample(this.mFactor, true);
            if (this.onFinished != null)
            {
                this.onFinished(this);
            }
            if (this.eventReceiver != null && !string.IsNullOrEmpty(this.callWhenFinished))
            {
                this.eventReceiver.SendMessage(this.callWhenFinished, this, SendMessageOptions.DontRequireReceiver);
            }
            if ((this.mFactor == 1f && this.mAmountPerDelta > 0f) || (this.mFactor == 0f && this.mAmountPerDelta < 0f))
            {
                base.enabled = false;
            }
        }
        else
        {
            this.Sample(this.mFactor, false);
        }
    }

    protected abstract void OnUpdate(float factor, bool isFinished);

    public static T Begin<T>(GameObject go, float duration) where T : UITweener
    {
        T t = go.GetComponent<T>();
        if (t == null)
        {
            t = go.AddComponent<T>();
        }
        t.mStarted = false;
        t.duration = duration;
        t.mFactor = 0f;
        t.mAmountPerDelta = Mathf.Abs(t.mAmountPerDelta);
        t.style = UITweener.Style.Once;
        t.animationCurve = new AnimationCurve(new Keyframe[]
        {
            new Keyframe(0f, 0f, 0f, 1f),
            new Keyframe(1f, 1f, 1f, 0f)
        });
        t.eventReceiver = null;
        t.callWhenFinished = null;
        t.onFinished = null;
        t.enabled = true;
        return t;
    }

    public void Play(bool forward)
    {
        this.mAmountPerDelta = Mathf.Abs(this.amountPerDelta);
        if (!forward)
        {
            this.mAmountPerDelta = -this.mAmountPerDelta;
        }
        base.enabled = true;
    }

    public void Reset()
    {
        this.mStarted = false;
        this.mFactor = ((this.mAmountPerDelta >= 0f) ? 0f : 1f);
        this.Sample(this.mFactor, false);
    }

    public void Sample(float factor, bool isFinished)
    {
        float num = Mathf.Clamp01(factor);
        if (this.method == UITweener.Method.EaseIn)
        {
            num = 1f - Mathf.Sin(1.57079637f * (1f - num));
            if (this.steeperCurves)
            {
                num *= num;
            }
        }
        else if (this.method == UITweener.Method.EaseOut)
        {
            num = Mathf.Sin(1.57079637f * num);
            if (this.steeperCurves)
            {
                num = 1f - num;
                num = 1f - num * num;
            }
        }
        else if (this.method == UITweener.Method.EaseInOut)
        {
            num -= Mathf.Sin(num * 6.28318548f) / 6.28318548f;
            if (this.steeperCurves)
            {
                num = num * 2f - 1f;
                float num2 = Mathf.Sign(num);
                num = 1f - Mathf.Abs(num);
                num = 1f - num * num;
                num = num2 * num * 0.5f + 0.5f;
            }
        }
        else if (this.method == UITweener.Method.BounceIn)
        {
            num = this.BounceLogic(num);
        }
        else if (this.method == UITweener.Method.BounceOut)
        {
            num = 1f - this.BounceLogic(1f - num);
        }
        this.OnUpdate((this.animationCurve == null) ? num : this.animationCurve.Evaluate(num), isFinished);
    }

    public void Toggle()
    {
        if (this.mFactor > 0f)
        {
            this.mAmountPerDelta = -this.amountPerDelta;
        }
        else
        {
            this.mAmountPerDelta = Mathf.Abs(this.amountPerDelta);
        }
        base.enabled = true;
    }
}