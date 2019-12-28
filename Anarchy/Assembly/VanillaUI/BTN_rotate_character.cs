using Optimization.Caching;
using UnityEngine;

public class BTN_rotate_character : MonoBehaviour
{
    private float distance = 3f;
    private bool isRotate;
    public new GameObject camera;
    public GameObject hero;

    private void OnPress(bool press)
    {
        if (press)
        {
            this.isRotate = true;
        }
        else
        {
            this.isRotate = false;
        }
    }

    private void Update()
    {
        this.distance -= Input.GetAxis("Mouse ScrollWheel") * 0.05f;
        this.distance = Mathf.Clamp(this.distance, 0.8f, 3.5f);
        this.camera.transform.position = this.hero.transform.position;
        this.camera.transform.position += Vectors.up * 1.1f;
        if (this.isRotate)
        {
            float angle = Input.GetAxis("Mouse X") * 2.5f;
            float angle2 = -Input.GetAxis("Mouse Y") * 2.5f;
            this.camera.transform.RotateAround(this.camera.transform.position, Vectors.up, angle);
            this.camera.transform.RotateAround(this.camera.transform.position, this.camera.transform.right, angle2);
        }
        this.camera.transform.position -= this.camera.transform.Forward() * this.distance;
    }
}