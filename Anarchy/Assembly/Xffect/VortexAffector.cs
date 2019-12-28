using Optimization.Caching;
using UnityEngine;

public class VortexAffector : Affector
{
    private float Magnitude;

    private bool UseCurve;

    private AnimationCurve VortexCurve;

    protected Vector3 Direction;

    public VortexAffector(AnimationCurve vortexCurve, Vector3 dir, EffectNode node) : base(node)
    {
        this.VortexCurve = vortexCurve;
        this.Direction = dir;
        this.UseCurve = true;
    }

    public VortexAffector(float mag, Vector3 dir, EffectNode node) : base(node)
    {
        this.Magnitude = mag;
        this.Direction = dir;
        this.UseCurve = false;
    }

    public override void Update()
    {
        Vector3 vector = this.Node.GetLocalPosition() - this.Node.Owner.EmitPoint;
        float magnitude = vector.magnitude;
        if (magnitude == 0f)
        {
            return;
        }
        float d = Vector3.Dot(this.Direction, vector);
        vector -= d * this.Direction;
        Vector3 vector2 = Vectors.zero;
        if (vector == Vectors.zero)
        {
            vector2 = vector;
        }
        else
        {
            vector2 = Vector3.Cross(this.Direction, vector).normalized;
        }
        float elapsedTime = this.Node.GetElapsedTime();
        float num;
        if (this.UseCurve)
        {
            num = this.VortexCurve.Evaluate(elapsedTime);
        }
        else
        {
            num = this.Magnitude;
        }
        vector2 *= num * Time.deltaTime;
        this.Node.Position += vector2;
    }
}