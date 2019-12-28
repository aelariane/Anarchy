using UnityEngine;

public class ScaleAffector : Affector
{
    protected float DeltaX;

    protected float DeltaY;

    protected AnimationCurve ScaleXCurve;

    protected AnimationCurve ScaleYCurve;

    protected RSTYPE Type;

    public ScaleAffector(AnimationCurve curveX, AnimationCurve curveY, EffectNode node) : base(node)
    {
        this.Type = RSTYPE.CURVE;
        this.ScaleXCurve = curveX;
        this.ScaleYCurve = curveY;
    }

    public ScaleAffector(float x, float y, EffectNode node) : base(node)
    {
        this.Type = RSTYPE.SIMPLE;
        this.DeltaX = x;
        this.DeltaY = y;
    }

    public override void Update()
    {
        float elapsedTime = this.Node.GetElapsedTime();
        if (this.Type == RSTYPE.CURVE)
        {
            if (this.ScaleXCurve != null)
            {
                this.Node.Scale.x = this.ScaleXCurve.Evaluate(elapsedTime);
            }
            if (this.ScaleYCurve != null)
            {
                this.Node.Scale.y = this.ScaleYCurve.Evaluate(elapsedTime);
            }
        }
        else if (this.Type == RSTYPE.SIMPLE)
        {
            float num = this.Node.Scale.x + this.DeltaX * Time.deltaTime;
            float num2 = this.Node.Scale.y + this.DeltaY * Time.deltaTime;
            if (num * this.Node.Scale.x > 0f)
            {
                this.Node.Scale.x = num;
            }
            if (num2 * this.Node.Scale.y > 0f)
            {
                this.Node.Scale.y = num2;
            }
        }
    }
}