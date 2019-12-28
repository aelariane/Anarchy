using NGUI;
using UnityEngine;

[AddComponentMenu("NGUI/Interaction/Button Tween")]
public class UIButtonTween : MonoBehaviour
{
    private bool mHighlighted;
    private bool mStarted;
    private UITweener[] mTweens;
    public string callWhenFinished;
    public DisableCondition disableWhenFinished;
    public GameObject eventReceiver;
    public EnableCondition ifDisabledOnPlay;
    public bool includeChildren;
    public UITweener.OnFinished onFinished;
    public Direction playDirection = Direction.Forward;
    public bool resetOnPlay;
    public Trigger trigger;
    public int tweenGroup;
    public GameObject tweenTarget;

    private void OnActivate(bool isActive)
    {
        if (base.enabled && (this.trigger == Trigger.OnActivate || (this.trigger == Trigger.OnActivateTrue && isActive) || (this.trigger == Trigger.OnActivateFalse && !isActive)))
        {
            this.Play(isActive);
        }
    }

    private void OnClick()
    {
        if (base.enabled && this.trigger == Trigger.OnClick)
        {
            this.Play(true);
        }
    }

    private void OnDoubleClick()
    {
        if (base.enabled && this.trigger == Trigger.OnDoubleClick)
        {
            this.Play(true);
        }
    }

    private void OnEnable()
    {
        if (this.mStarted && this.mHighlighted)
        {
            this.OnHover(UICamera.IsHighlighted(base.gameObject));
        }
    }

    private void OnHover(bool isOver)
    {
        if (base.enabled)
        {
            if (this.trigger == Trigger.OnHover || (this.trigger == Trigger.OnHoverTrue && isOver) || (this.trigger == Trigger.OnHoverFalse && !isOver))
            {
                this.Play(isOver);
            }
            this.mHighlighted = isOver;
        }
    }

    private void OnPress(bool isPressed)
    {
        if (base.enabled && (this.trigger == Trigger.OnPress || (this.trigger == Trigger.OnPressTrue && isPressed) || (this.trigger == Trigger.OnPressFalse && !isPressed)))
        {
            this.Play(isPressed);
        }
    }

    private void OnSelect(bool isSelected)
    {
        if (base.enabled && (this.trigger == Trigger.OnSelect || (this.trigger == Trigger.OnSelectTrue && isSelected) || (this.trigger == Trigger.OnSelectFalse && !isSelected)))
        {
            this.Play(true);
        }
    }

    private void Start()
    {
        this.mStarted = true;
        if (this.tweenTarget == null)
        {
            this.tweenTarget = base.gameObject;
        }
    }

    private void Update()
    {
        if (this.disableWhenFinished != DisableCondition.DoNotDisable && this.mTweens != null)
        {
            bool flag = true;
            bool flag2 = true;
            int i = 0;
            int num = this.mTweens.Length;
            while (i < num)
            {
                UITweener uitweener = this.mTweens[i];
                if (uitweener.tweenGroup == this.tweenGroup)
                {
                    if (uitweener.enabled)
                    {
                        flag = false;
                        break;
                    }
                    if (uitweener.direction != (Direction)this.disableWhenFinished)
                    {
                        flag2 = false;
                    }
                }
                i++;
            }
            if (flag)
            {
                if (flag2)
                {
                    NGUITools.SetActive(this.tweenTarget, false);
                }
                this.mTweens = null;
            }
        }
    }

    public void Play(bool forward)
    {
        GameObject gameObject = (!(this.tweenTarget == null)) ? this.tweenTarget : base.gameObject;
        if (!NGUITools.GetActive(gameObject))
        {
            if (this.ifDisabledOnPlay != EnableCondition.EnableThenPlay)
            {
                return;
            }
            NGUITools.SetActive(gameObject, true);
        }
        this.mTweens = ((!this.includeChildren) ? gameObject.GetComponents<UITweener>() : gameObject.GetComponentsInChildren<UITweener>());
        if (this.mTweens.Length == 0)
        {
            if (this.disableWhenFinished != DisableCondition.DoNotDisable)
            {
                NGUITools.SetActive(this.tweenTarget, false);
            }
        }
        else
        {
            bool flag = false;
            if (this.playDirection == Direction.Reverse)
            {
                forward = !forward;
            }
            int i = 0;
            int num = this.mTweens.Length;
            while (i < num)
            {
                UITweener uitweener = this.mTweens[i];
                if (uitweener.tweenGroup == this.tweenGroup)
                {
                    if (!flag && !NGUITools.GetActive(gameObject))
                    {
                        flag = true;
                        NGUITools.SetActive(gameObject, true);
                    }
                    if (this.playDirection == Direction.Toggle)
                    {
                        uitweener.Toggle();
                    }
                    else
                    {
                        uitweener.Play(forward);
                    }
                    if (this.resetOnPlay)
                    {
                        uitweener.Reset();
                    }
                    uitweener.onFinished = this.onFinished;
                    if (this.eventReceiver != null && !string.IsNullOrEmpty(this.callWhenFinished))
                    {
                        uitweener.eventReceiver = this.eventReceiver;
                        uitweener.callWhenFinished = this.callWhenFinished;
                    }
                }
                i++;
            }
        }
    }
}