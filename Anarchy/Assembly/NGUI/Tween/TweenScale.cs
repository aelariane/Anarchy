using Optimization.Caching;
using UnityEngine;

[AddComponentMenu("NGUI/Tween/Scale")]
public class TweenScale : UITweener
{
    private UITable mTable;
    private Transform mTrans;
    public Vector3 from = Vectors.one;

    public Vector3 to = Vectors.one;

    public bool updateTable;

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

    public Vector3 scale
    {
        get
        {
            return this.cachedTransform.localScale;
        }
        set
        {
            this.cachedTransform.localScale = value;
        }
    }

    protected override void OnUpdate(float factor, bool isFinished)
    {
        this.cachedTransform.localScale = this.from * (1f - factor) + this.to * factor;
        if (this.updateTable)
        {
            if (this.mTable == null)
            {
                this.mTable = NGUITools.FindInParents<UITable>(base.gameObject);
                if (this.mTable == null)
                {
                    this.updateTable = false;
                    return;
                }
            }
            this.mTable.repositionNow = true;
        }
    }

    public static TweenScale Begin(GameObject go, float duration, Vector3 scale)
    {
        TweenScale tweenScale = UITweener.Begin<TweenScale>(go, duration);
        tweenScale.from = tweenScale.scale;
        tweenScale.to = scale;
        if (duration <= 0f)
        {
            tweenScale.Sample(1f, true);
            tweenScale.enabled = false;
        }
        return tweenScale;
    }
}