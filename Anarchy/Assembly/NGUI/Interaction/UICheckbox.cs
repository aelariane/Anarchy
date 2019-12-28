using NGUI;
using UnityEngine;

[AddComponentMenu("NGUI/Interaction/Checkbox")]
public class UICheckbox : MonoBehaviour
{
    private bool mChecked = true;
    private bool mStarted;
    private Transform mTrans;

    [HideInInspector]
    [SerializeField]
    private bool option;

    public static UICheckbox current;

    public Animation checkAnimation;
    public UISprite checkSprite;
    public GameObject eventReceiver;
    public string functionName = "OnActivate";
    public bool instantTween;

    public UICheckbox.OnStateChange onStateChange;
    public bool optionCanBeNone;
    public Transform radioButtonRoot;
    public bool startsChecked = true;

    public delegate void OnStateChange(bool state);

    public bool isChecked
    {
        get
        {
            return this.mChecked;
        }
        set
        {
            if (this.radioButtonRoot == null || value || this.optionCanBeNone || !this.mStarted)
            {
                this.Set(value);
            }
        }
    }

    private void Awake()
    {
        this.mTrans = base.transform;
        if (this.checkSprite != null)
        {
            this.checkSprite.alpha = ((!this.startsChecked) ? 0f : 1f);
        }
        if (this.option)
        {
            this.option = false;
            if (this.radioButtonRoot == null)
            {
                this.radioButtonRoot = this.mTrans.parent;
            }
        }
    }

    private void OnClick()
    {
        if (base.enabled)
        {
            this.isChecked = !this.isChecked;
        }
    }

    private void Set(bool state)
    {
        if (!this.mStarted)
        {
            this.mChecked = state;
            this.startsChecked = state;
            if (this.checkSprite != null)
            {
                this.checkSprite.alpha = ((!state) ? 0f : 1f);
            }
        }
        else if (this.mChecked != state)
        {
            if (this.radioButtonRoot != null && state)
            {
                UICheckbox[] componentsInChildren = this.radioButtonRoot.GetComponentsInChildren<UICheckbox>(true);
                int i = 0;
                int num = componentsInChildren.Length;
                while (i < num)
                {
                    UICheckbox uicheckbox = componentsInChildren[i];
                    if (uicheckbox != this && uicheckbox.radioButtonRoot == this.radioButtonRoot)
                    {
                        uicheckbox.Set(false);
                    }
                    i++;
                }
            }
            this.mChecked = state;
            if (this.checkSprite != null)
            {
                if (this.instantTween)
                {
                    this.checkSprite.alpha = ((!this.mChecked) ? 0f : 1f);
                }
                else
                {
                    TweenAlpha.Begin(this.checkSprite.gameObject, 0.15f, (!this.mChecked) ? 0f : 1f);
                }
            }
            UICheckbox.current = this;
            if (this.onStateChange != null)
            {
                this.onStateChange(this.mChecked);
            }
            if (this.eventReceiver != null && !string.IsNullOrEmpty(this.functionName))
            {
                this.eventReceiver.SendMessage(this.functionName, this.mChecked, SendMessageOptions.DontRequireReceiver);
            }
            UICheckbox.current = null;
            if (this.checkAnimation != null)
            {
                ActiveAnimation.Play(this.checkAnimation, (!state) ? Direction.Reverse : Direction.Forward);
            }
        }
    }

    private void Start()
    {
        if (this.eventReceiver == null)
        {
            this.eventReceiver = base.gameObject;
        }
        this.mChecked = !this.startsChecked;
        this.mStarted = true;
        this.Set(this.startsChecked);
    }
}