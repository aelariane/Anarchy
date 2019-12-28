using UnityEngine;

[AddComponentMenu("NGUI/Tween/Rotation")]
public class TweenRotation : UITweener
{
    private Transform mTrans;
    public Vector3 from;

    public Vector3 to;

    public Transform cachedTransform
    {
        get
        {
            if (this.mTrans == null)
            {
                this.mTrans = base.transform;
            }
            return this.mTrans;
        }
    }

    public Quaternion rotation
    {
        get
        {
            return this.cachedTransform.localRotation;
        }
        set
        {
            this.cachedTransform.localRotation = value;
        }
    }

    protected override void OnUpdate(float factor, bool isFinished)
    {
        this.cachedTransform.localRotation = Quaternion.Slerp(Quaternion.Euler(this.from), Quaternion.Euler(this.to), factor);
    }

    public static TweenRotation Begin(GameObject go, float duration, Quaternion rot)
    {
        TweenRotation tweenRotation = UITweener.Begin<TweenRotation>(go, duration);
        tweenRotation.from = tweenRotation.rotation.eulerAngles;
        tweenRotation.to = rot.eulerAngles;
        if (duration <= 0f)
        {
            tweenRotation.Sample(1f, true);
            tweenRotation.enabled = false;
        }
        return tweenRotation;
    }
}