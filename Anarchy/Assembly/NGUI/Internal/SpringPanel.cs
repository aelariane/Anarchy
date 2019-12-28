using Optimization.Caching;
using UnityEngine;

[AddComponentMenu("NGUI/Internal/Spring Panel")]
[RequireComponent(typeof(UIPanel))]
public class SpringPanel : IgnoreTimeScale
{
    private UIDraggablePanel mDrag;
    private UIPanel mPanel;
    private float mThreshold;
    private Transform mTrans;
    public SpringPanel.OnFinished onFinished;
    public float strength = 10f;
    public Vector3 target = Vectors.zero;

    public delegate void OnFinished();

    private void Start()
    {
        this.mPanel = base.GetComponent<UIPanel>();
        this.mDrag = base.GetComponent<UIDraggablePanel>();
        this.mTrans = base.transform;
    }

    private void Update()
    {
        float deltaTime = base.UpdateRealTimeDelta();
        if (this.mThreshold == 0f)
        {
            this.mThreshold = (this.target - this.mTrans.localPosition).magnitude * 0.005f;
        }
        bool flag = false;
        Vector3 localPosition = this.mTrans.localPosition;
        Vector3 vector = NGUIMath.SpringLerp(this.mTrans.localPosition, this.target, this.strength, deltaTime);
        if (this.mThreshold >= Vector3.Magnitude(vector - this.target))
        {
            vector = this.target;
            base.enabled = false;
            flag = true;
        }
        this.mTrans.localPosition = vector;
        Vector3 vector2 = vector - localPosition;
        Vector4 clipRange = this.mPanel.clipRange;
        clipRange.x -= vector2.x;
        clipRange.y -= vector2.y;
        this.mPanel.clipRange = clipRange;
        if (this.mDrag != null)
        {
            this.mDrag.UpdateScrollbars(false);
        }
        if (flag && this.onFinished != null)
        {
            this.onFinished();
        }
    }

    public static SpringPanel Begin(GameObject go, Vector3 pos, float strength)
    {
        SpringPanel springPanel = go.GetComponent<SpringPanel>();
        if (springPanel == null)
        {
            springPanel = go.AddComponent<SpringPanel>();
        }
        springPanel.target = pos;
        springPanel.strength = strength;
        springPanel.onFinished = null;
        if (!springPanel.enabled)
        {
            springPanel.mThreshold = 0f;
            springPanel.enabled = true;
        }
        return springPanel;
    }
}