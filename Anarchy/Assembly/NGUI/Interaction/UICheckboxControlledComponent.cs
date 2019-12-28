using System;
using UnityEngine;

[AddComponentMenu("NGUI/Interaction/Checkbox Controlled Component")]
public class UICheckboxControlledComponent : MonoBehaviour
{
    private bool mUsingDelegates;
    public bool inverse;
    public MonoBehaviour target;

    private void OnActivate(bool isActive)
    {
        if (!this.mUsingDelegates)
        {
            this.OnActivateDelegate(isActive);
        }
    }

    private void OnActivateDelegate(bool isActive)
    {
        if (base.enabled && this.target != null)
        {
            this.target.enabled = ((!this.inverse) ? isActive : (!isActive));
        }
    }

    private void Start()
    {
        UICheckbox component = base.GetComponent<UICheckbox>();
        if (component != null)
        {
            this.mUsingDelegates = true;
            UICheckbox uicheckbox = component;
            uicheckbox.onStateChange = (UICheckbox.OnStateChange)Delegate.Combine(uicheckbox.onStateChange, new UICheckbox.OnStateChange(this.OnActivateDelegate));
        }
    }
}