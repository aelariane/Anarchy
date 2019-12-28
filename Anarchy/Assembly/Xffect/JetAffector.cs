using UnityEngine;

public class JetAffector : Affector
{
    protected float MaxAcceleration;

    protected float MinAcceleration;

    public JetAffector(float min, float max, EffectNode node) : base(node)
    {
        this.MinAcceleration = min;
        this.MaxAcceleration = max;
    }

    public override void Update()
    {
        if ((double)Mathf.Abs(this.Node.Acceleration) < 1E-06)
        {
            float acceleration = UnityEngine.Random.Range(this.MinAcceleration, this.MaxAcceleration);
            this.Node.Acceleration = acceleration;
        }
    }
}