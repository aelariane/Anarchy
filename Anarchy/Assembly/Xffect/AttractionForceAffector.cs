using UnityEngine;

public class AttractionForceAffector : Affector
{
    private AnimationCurve AttractionCurve;

    private float Magnitude;

    private bool UseCurve;

    protected Vector3 Position;

    public AttractionForceAffector(AnimationCurve curve, Vector3 pos, EffectNode node) : base(node)
    {
        this.AttractionCurve = curve;
        this.Position = pos;
        this.UseCurve = true;
    }

    public AttractionForceAffector(float magnitude, Vector3 pos, EffectNode node) : base(node)
    {
        this.Magnitude = magnitude;
        this.Position = pos;
        this.UseCurve = false;
    }

    public override void Update()
    {
        Vector3 vector;
        if (this.Node.SyncClient)
        {
            vector = this.Position - this.Node.GetLocalPosition();
        }
        else
        {
            vector = this.Node.ClientTrans.position + this.Position - this.Node.GetLocalPosition();
        }
        float elapsedTime = this.Node.GetElapsedTime();
        float num;
        if (this.UseCurve)
        {
            num = this.AttractionCurve.Evaluate(elapsedTime);
        }
        else
        {
            num = this.Magnitude;
        }
        float d = num;
        this.Node.Velocity += vector.normalized * d * Time.deltaTime;
    }
}