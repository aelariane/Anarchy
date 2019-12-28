using Optimization.Caching;
using UnityEngine;

public class OnJoinedInstantiate : MonoBehaviour
{
    public float PositionOffset = 2f;
    public GameObject[] PrefabsToInstantiate;
    public Transform SpawnPosition;

    public void OnJoinedRoom()
    {
        if (this.PrefabsToInstantiate != null)
        {
            foreach (GameObject gameObject in this.PrefabsToInstantiate)
            {
                Debug.Log("Instantiating: " + gameObject.name);
                Vector3 a = Vectors.up;
                if (this.SpawnPosition != null)
                {
                    a = this.SpawnPosition.position;
                }
                Vector3 a2 = UnityEngine.Random.insideUnitSphere;
                a2.y = 0f;
                a2 = a2.normalized;
                Vector3 position = a + this.PositionOffset * a2;
                Optimization.Caching.Pool.NetworkEnable(gameObject.name, position, Quaternion.identity, 0);
            }
        }
    }
}