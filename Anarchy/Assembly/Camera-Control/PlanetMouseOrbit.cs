using UnityEngine;

[AddComponentMenu("Camera-Control/Mouse Orbit")]
public class PlanetMouseOrbit : MonoBehaviour
{
    private float x;
    private float y;
    public float distance = 10f;
    public Transform target;
    public float xSpeed = 250f;
    public int yMaxLimit = 80;
    public int yMinLimit = -20;
    public float ySpeed = 120f;
    public int zoomRate = 25;

    public static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360f)
        {
            angle += 360f;
        }
        if (angle > 360f)
        {
            angle -= 360f;
        }
        return Mathf.Clamp(angle, min, max);
    }

    public void Main()
    {
    }

    public void Start()
    {
        Vector3 eulerAngles = base.transform.eulerAngles;
        this.x = eulerAngles.y;
        this.y = eulerAngles.x;
    }

    public void Update()
    {
        if (this.target != null)
        {
            this.x += Input.GetAxis("Mouse X") * this.xSpeed * 0.02f;
            this.y -= Input.GetAxis("Mouse Y") * this.ySpeed * 0.02f;
            this.distance += -(Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime) * (float)this.zoomRate * Mathf.Abs(this.distance);
            this.y = PlanetMouseOrbit.ClampAngle(this.y, (float)this.yMinLimit, (float)this.yMaxLimit);
            Quaternion rotation = Quaternion.Euler(this.y, this.x, 0f);
            Vector3 position = rotation * new Vector3(0f, 0f, -this.distance) + this.target.position;
            base.transform.rotation = rotation;
            base.transform.position = position;
        }
    }
}