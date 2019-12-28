using UnityEngine;

public class RotateAffector : Affector
{
    protected float Delta;

    protected AnimationCurve RotateCurve;

    protected RSTYPE Type;

    public RotateAffector(AnimationCurve curve, EffectNode node) : base(node)
    {
        this.Type = RSTYPE.CURVE;
        this.RotateCurve = curve;
    }

    public RotateAffector(float delta, EffectNode node) : base(node)
    {
        this.Type = RSTYPE.SIMPLE;
        this.Delta = delta;
    }

    public override void Update()
    {
        float elapsedTime = this.Node.GetElapsedTime();
        if (this.Type == RSTYPE.CURVE)
        {
            this.Node.RotateAngle = (float)((int)this.RotateCurve.Evaluate(elapsedTime));
        }
        else if (this.Type == RSTYPE.SIMPLE)
        {
            float rotateAngle = this.Node.RotateAngle + this.Delta * Time.deltaTime;
            this.Node.RotateAngle = rotateAngle;
        }
    }
}