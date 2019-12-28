using UnityEngine;

public class ColorAffector : Affector
{
    protected Color[] ColorArr;

    protected float ElapsedTime;

    protected float GradualLen;

    protected bool IsNodeLife;

    protected COLOR_GRADUAL_TYPE Type;

    public ColorAffector(Color[] colorArr, float gradualLen, COLOR_GRADUAL_TYPE type, EffectNode node) : base(node)
    {
        this.ColorArr = colorArr;
        this.Type = type;
        this.GradualLen = gradualLen;
        if (this.GradualLen < 0f)
        {
            this.IsNodeLife = true;
        }
    }

    public override void Reset()
    {
        this.ElapsedTime = 0f;
    }

    public override void Update()
    {
        this.ElapsedTime += Time.deltaTime;
        if (this.IsNodeLife)
        {
            this.GradualLen = this.Node.GetLifeTime();
        }
        if (this.GradualLen <= 0f)
        {
            return;
        }
        if (this.ElapsedTime <= this.GradualLen)
        {
            int num = (int)((float)(this.ColorArr.Length - 1) * (this.ElapsedTime / this.GradualLen));
            if (num == this.ColorArr.Length - 1)
            {
                num--;
            }
            int num2 = num + 1;
            float num3 = this.GradualLen / (float)(this.ColorArr.Length - 1);
            float t = (this.ElapsedTime - num3 * (float)num) / num3;
            this.Node.Color = Color.Lerp(this.ColorArr[num], this.ColorArr[num2], t);
            return;
        }
        if (this.Type == COLOR_GRADUAL_TYPE.CLAMP)
        {
            return;
        }
        if (this.Type == COLOR_GRADUAL_TYPE.LOOP)
        {
            this.ElapsedTime = 0f;
            return;
        }
        Color[] array = new Color[this.ColorArr.Length];
        this.ColorArr.CopyTo(array, 0);
        for (int i = 0; i < array.Length / 2; i++)
        {
            this.ColorArr[array.Length - i - 1] = array[i];
            this.ColorArr[i] = array[array.Length - i - 1];
        }
        this.ElapsedTime = 0f;
    }
}