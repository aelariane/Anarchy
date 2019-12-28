using UnityEngine;

public class LinearForceAffector : Affector
{
    protected Vector3 Force;

    public LinearForceAffector(Vector3 force, EffectNode node) : base(node)
    {
        this.Force = force;
    }

    public override void Update()
    {
        this.Node.Velocity += this.Force * Time.deltaTime;
    }
}