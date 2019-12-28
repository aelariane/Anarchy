using UnityEngine;

public class RCGeneralEffect : Photon.MonoBehaviour
{
    private bool destroyed;

    private void Awake()
    {
        destroyed = false;
        StartCoroutine(WaitAndDestroy());
    }

    private void OnDisable()
    {
        destroyed = true;
    }

    private void OnEnable()
    {
        destroyed = false;
    }

    private System.Collections.IEnumerator WaitAndDestroy()
    {
        yield return new WaitForSeconds(1.5f);
        if (!destroyed)
        {
            Optimization.Caching.Pool.Disable(gameObject);
        }
    }
}