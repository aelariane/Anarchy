using Optimization.Caching;
using UnityEngine;

public class SupportLogger : MonoBehaviour
{
    public bool LogTrafficStats = true;

    public void Start()
    {
        GameObject gameObject = CacheGameObject.Find("PunSupportLogger");
        if (gameObject == null)
        {
            gameObject = new GameObject("PunSupportLogger");
            UnityEngine.Object.DontDestroyOnLoad(gameObject);
            SupportLogging supportLogging = gameObject.AddComponent<SupportLogging>();
            supportLogging.LogTrafficStats = this.LogTrafficStats;
        }
    }
}