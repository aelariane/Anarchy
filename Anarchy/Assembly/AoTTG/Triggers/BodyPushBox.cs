using UnityEngine;

public class BodyPushBox : MonoBehaviour
{
    public GameObject parent;

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("bodyCollider"))
        {
            BodyPushBox component = other.gameObject.GetComponent<BodyPushBox>();
            if (component && component.parent)
            {
                Vector3 vector = component.parent.transform.position - this.parent.transform.position;
                float radius = base.gameObject.GetComponent<CapsuleCollider>().radius;
                float radius2 = base.gameObject.GetComponent<CapsuleCollider>().radius;
                vector.y = 0f;
                float num;
                if (vector.magnitude > 0f)
                {
                    num = radius + radius2 - vector.magnitude;
                    vector.Normalize();
                }
                else
                {
                    num = radius + radius2;
                    vector.x = 1f;
                }
                if (num < 0.1f)
                {
                }
            }
        }
    }
}