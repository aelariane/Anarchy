using UnityEngine;

[AddComponentMenu("NGUI/Examples/Look At Target")]
public class LookAtTarget : MonoBehaviour
{
    private Transform mTrans;
    public int level;

    public float speed = 8f;
    public Transform target;

    private void LateUpdate()
    {
        if (this.target != null)
        {
            Vector3 forward = this.target.position - this.mTrans.position;
            float magnitude = forward.magnitude;
            if (magnitude > 0.001f)
            {
                Quaternion to = Quaternion.LookRotation(forward);
                this.mTrans.rotation = Quaternion.Slerp(this.mTrans.rotation, to, Mathf.Clamp01(this.speed * Time.deltaTime));
            }
        }
    }

    private void Start()
    {
        this.mTrans = base.transform;
    }
}