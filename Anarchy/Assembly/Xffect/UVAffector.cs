using Optimization.Caching;
using UnityEngine;

public class UVAffector : Affector
{
    protected float ElapsedTime;

    protected UVAnimation Frames;

    protected float UVTime;

    public UVAffector(UVAnimation frame, float time, EffectNode node) : base(node)
    {
        this.Frames = frame;
        this.UVTime = time;
    }

    public override void Reset()
    {
        this.ElapsedTime = 0f;
        this.Frames.curFrame = 0;
    }

    public override void Update()
    {
        this.ElapsedTime += Time.deltaTime;
        float num;
        if (this.UVTime <= 0f)
        {
            num = this.Node.GetLifeTime() / (float)this.Frames.frames.Length;
        }
        else
        {
            num = this.UVTime / (float)this.Frames.frames.Length;
        }
        if (this.ElapsedTime >= num)
        {
            Vector2 zero = Vectors.v2zero;
            Vector2 zero2 = Vectors.v2zero;
            this.Frames.GetNextFrame(ref zero, ref zero2);
            this.Node.LowerLeftUV = zero;
            this.Node.UVDimensions = zero2;
            this.ElapsedTime -= num;
        }
    }
}