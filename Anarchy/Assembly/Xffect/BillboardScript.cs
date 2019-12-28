using Optimization.Caching;
using UnityEngine;

public class BillboardScript : MonoBehaviour
{
    public void Main()
    {
    }

    public void Update()
    {
        base.transform.LookAt(IN_GAME_MAIN_CAMERA.BaseCamera.transform.position);
        base.transform.Rotate(Vectors.left * -90f);
    }
}