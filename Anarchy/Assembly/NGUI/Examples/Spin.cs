using UnityEngine;

[AddComponentMenu("NGUI/Examples/Spin")]
public class Spin : MonoBehaviour
{
    private Rigidbody mRb;
    private Transform mTrans;
    public Vector3 rotationsPerSecond = new Vector3(0f, 0.1f, 0f);

    private void FixedUpdate()
    {
        if (this.mRb != null)
        {
            this.ApplyDelta(Time.deltaTime);
        }
    }

    private void Start()
    {
        this.mTrans = base.transform;
        this.mRb = base.rigidbody;
    }

    private void Update()
    {
        if (this.mRb == null)
        {
            this.ApplyDelta(Time.deltaTime);
        }
    }

    public void ApplyDelta(float delta)
    {
        delta *= 360f;
        Quaternion rhs = Quaternion.Euler(this.rotationsPerSecond * delta);
        if (this.mRb == null)
        {
            this.mTrans.rotation = this.mTrans.rotation * rhs;
        }
        else
        {
            this.mRb.MoveRotation(this.mRb.rotation * rhs);
        }
    }
}