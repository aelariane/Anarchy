using Optimization.Caching;
using UnityEngine;

public class ClickToMove : MonoBehaviour
{
    private Vector3 targetPosition;
    public int smooth;

    public void Main()
    {
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Plane plane = new Plane(Vectors.up, base.transform.position);
            Ray ray = IN_GAME_MAIN_CAMERA.BaseCamera.ScreenPointToRay(Input.mousePosition);
            float distance = 0f;
            if (plane.Raycast(ray, out distance))
            {
                Vector3 point = ray.GetPoint(distance);
                this.targetPosition = ray.GetPoint(distance);
                Quaternion rotation = Quaternion.LookRotation(point - base.transform.position);
                base.transform.rotation = rotation;
            }
        }
        base.transform.position = Vector3.Lerp(base.transform.position, this.targetPosition, Time.deltaTime * (float)this.smooth);
    }
}