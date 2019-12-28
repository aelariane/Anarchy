using UnityEngine;

[AddComponentMenu("NGUI/Tween/Transform")]
public class TweenTransform : UITweener
{
    private Vector3 mPos;
    private Quaternion mRot;
    private Vector3 mScale;
    private Transform mTrans;
    public Transform from;

    public bool parentWhenFinished;
    public Transform to;

    protected override void OnUpdate(float factor, bool isFinished)
    {
        if (this.to != null)
        {
            if (this.mTrans == null)
            {
                this.mTrans = base.transform;
                this.mPos = this.mTrans.position;
                this.mRot = this.mTrans.rotation;
                this.mScale = this.mTrans.localScale;
            }
            if (this.from != null)
            {
                this.mTrans.position = this.from.position * (1f - factor) + this.to.position * factor;
                this.mTrans.localScale = this.from.localScale * (1f - factor) + this.to.localScale * factor;
                this.mTrans.rotation = Quaternion.Slerp(this.from.rotation, this.to.rotation, factor);
            }
            else
            {
                this.mTrans.position = this.mPos * (1f - factor) + this.to.position * factor;
                this.mTrans.localScale = this.mScale * (1f - factor) + this.to.localScale * factor;
                this.mTrans.rotation = Quaternion.Slerp(this.mRot, this.to.rotation, factor);
            }
            if (this.parentWhenFinished && isFinished)
            {
                this.mTrans.parent = this.to;
            }
        }
    }

    public static TweenTransform Begin(GameObject go, float duration, Transform to)
    {
        return TweenTransform.Begin(go, duration, null, to);
    }

    public static TweenTransform Begin(GameObject go, float duration, Transform from, Transform to)
    {
        TweenTransform tweenTransform = UITweener.Begin<TweenTransform>(go, duration);
        tweenTransform.from = from;
        tweenTransform.to = to;
        if (duration <= 0f)
        {
            tweenTransform.Sample(1f, true);
            tweenTransform.enabled = false;
        }
        return tweenTransform;
    }
}