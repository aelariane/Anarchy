using Optimization.Caching;
using UnityEngine;

[AddComponentMenu("NGUI/Examples/Pan With Mouse")]
public class PanWithMouse : IgnoreTimeScale
{
    private Vector2 mRot = Vectors.v2zero;
    private Quaternion mStart;
    private Transform mTrans;
    public Vector2 degrees = new Vector2(5f, 3f);

    public float range = 1f;

    private void Start()
    {
        this.mTrans = base.transform;
        this.mStart = this.mTrans.localRotation;
    }

    private void Update()
    {
        float num = base.UpdateRealTimeDelta();
        Vector3 mousePosition = Input.mousePosition;
        float num2 = (float)Screen.width * 0.5f;
        float num3 = (float)Screen.height * 0.5f;
        if (this.range < 0.1f)
        {
            this.range = 0.1f;
        }
        float x = Mathf.Clamp((mousePosition.x - num2) / num2 / this.range, -1f, 1f);
        float y = Mathf.Clamp((mousePosition.y - num3) / num3 / this.range, -1f, 1f);
        this.mRot = Vector2.Lerp(this.mRot, new Vector2(x, y), num * 5f);
        this.mTrans.localRotation = this.mStart * Quaternion.Euler(-this.mRot.y * this.degrees.y, this.mRot.x * this.degrees.x, 0f);
    }
}