using UnityEngine;

[AddComponentMenu("NGUI/Tween/Color")]
public class TweenColor : UITweener
{
    private Light mLight;
    private Material mMat;
    private UIWidget mWidget;
    public Color from = Color.white;

    public Color to = Color.white;

    public Color color
    {
        get
        {
            if (this.mWidget != null)
            {
                return this.mWidget.color;
            }
            if (this.mLight != null)
            {
                return this.mLight.color;
            }
            if (this.mMat != null)
            {
                return this.mMat.color;
            }
            return Color.black;
        }
        set
        {
            if (this.mWidget != null)
            {
                this.mWidget.color = value;
            }
            if (this.mMat != null)
            {
                this.mMat.color = value;
            }
            if (this.mLight != null)
            {
                this.mLight.color = value;
                this.mLight.enabled = (value.r + value.g + value.b > 0.01f);
            }
        }
    }

    private void Awake()
    {
        this.mWidget = base.GetComponentInChildren<UIWidget>();
        Renderer renderer = base.renderer;
        if (renderer != null)
        {
            this.mMat = renderer.material;
        }
        this.mLight = base.light;
    }

    protected override void OnUpdate(float factor, bool isFinished)
    {
        this.color = Color.Lerp(this.from, this.to, factor);
    }

    public static TweenColor Begin(GameObject go, float duration, Color color)
    {
        TweenColor tweenColor = UITweener.Begin<TweenColor>(go, duration);
        tweenColor.from = tweenColor.color;
        tweenColor.to = color;
        if (duration <= 0f)
        {
            tweenColor.Sample(1f, true);
            tweenColor.enabled = false;
        }
        return tweenColor;
    }
}