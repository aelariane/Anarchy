using UnityEngine;

public class CatchDestroy : MonoBehaviour
{
    public GameObject target;
    private bool destroyed = false;

    private void OnDisable()
    {
        destroyed = true;
    }

    private void OnDestroy()
    {
        if (target != null && !destroyed)
        {
            Optimization.Caching.Pool.Disable(target);
        }
    }

    private void OnEnable()
    {
        destroyed = false;
    }
}