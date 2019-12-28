using UnityEngine;

[AddComponentMenu("NGUI/Interaction/Button")]
public class UIButton : UIButtonColor
{
    public Color disabledColor = Color.grey;

    public bool isEnabled
    {
        get
        {
            Collider collider = base.collider;
            return collider && collider.enabled;
        }
        set
        {
            Collider collider = base.collider;
            if (!collider)
            {
                return;
            }
            if (collider.enabled != value)
            {
                collider.enabled = value;
                this.UpdateColor(value, false);
            }
        }
    }

    protected override void OnEnable()
    {
        if (this.isEnabled)
        {
            base.OnEnable();
        }
        else
        {
            this.UpdateColor(false, true);
        }
    }

    public override void OnHover(bool isOver)
    {
        if (this.isEnabled)
        {
            base.OnHover(isOver);
        }
    }

    public override void OnPress(bool isPressed)
    {
        if (this.isEnabled)
        {
            base.OnPress(isPressed);
        }
    }

    public void UpdateColor(bool shouldBeEnabled, bool immediate)
    {
        if (this.tweenTarget)
        {
            if (!this.mStarted)
            {
                this.mStarted = true;
                base.Init();
            }
            Color color = (!shouldBeEnabled) ? this.disabledColor : base.defaultColor;
            TweenColor tweenColor = TweenColor.Begin(this.tweenTarget, 0.15f, color);
            if (immediate)
            {
                tweenColor.color = color;
                tweenColor.enabled = false;
            }
        }
    }
}