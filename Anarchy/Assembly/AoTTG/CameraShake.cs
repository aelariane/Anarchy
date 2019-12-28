using Optimization.Caching;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    private float decay;

    private float duration;

    private bool flip;

    private float R;

    private void FixedUpdate()
    {
    }

    private void shakeUpdate()
    {
        if (this.duration > 0f)
        {
            this.duration -= Time.deltaTime;
            if (this.flip)
            {
                base.gameObject.transform.position += Vectors.up * this.R;
            }
            else
            {
                base.gameObject.transform.position -= Vectors.up * this.R;
            }
            this.flip = !this.flip;
            this.R *= this.decay;
        }
    }

    private void Start()
    {
    }

    private void Update()
    {
    }

    public void startShake(float R, float duration, float decay = 0.95f)
    {
        if (this.duration < duration)
        {
            this.R = R;
            this.duration = duration;
            this.decay = decay;
        }
    }
}