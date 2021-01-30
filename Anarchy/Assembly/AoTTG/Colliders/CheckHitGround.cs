using Optimization.Caching;
using UnityEngine;

public class CheckHitGround : MonoBehaviour
{
    public bool isGrounded;

    private void OnTriggerEnter(Collider other)
    {
        switch (other.gameObject.layer)
        {
            case Layers.GroundN:
            case Layers.EnemyAABBN:
                isGrounded = true;
                break;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        switch (other.gameObject.layer)
        {
            case Layers.GroundN:
            case Layers.EnemyAABBN:
                this.isGrounded = true;
                break;
        }
    }
}