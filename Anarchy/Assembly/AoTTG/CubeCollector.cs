using UnityEngine;

public class CubeCollector : MonoBehaviour
{
    public int type;

    private void Start()
    {
    }

    private void Update()
    {
        if (GameObject.FindGameObjectWithTag("Player") == null)
        {
            return;
        }
        GameObject gameObject = GameObject.FindGameObjectWithTag("Player");
        if (Vector3.Distance(gameObject.transform.position, base.transform.position) < 8f)
        {
            UnityEngine.Object.Destroy(base.gameObject);
        }
    }
}