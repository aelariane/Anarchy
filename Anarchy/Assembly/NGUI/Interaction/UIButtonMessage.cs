using UnityEngine;

[AddComponentMenu("NGUI/Interaction/Button Message")]
public class UIButtonMessage : MonoBehaviour
{
    private bool mHighlighted;
    private bool mStarted;
    public string functionName;
    public bool includeChildren;
    public GameObject target;
    public UIButtonMessage.Trigger trigger;

    public enum Trigger
    {
        OnClick,
        OnMouseOver,
        OnMouseOut,
        OnPress,
        OnRelease,
        OnDoubleClick
    }

    private void OnClick()
    {
        if (base.enabled && this.trigger == UIButtonMessage.Trigger.OnClick)
        {
            this.Send();
        }
    }

    private void OnDoubleClick()
    {
        if (base.enabled && this.trigger == UIButtonMessage.Trigger.OnDoubleClick)
        {
            this.Send();
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
            if ((isOver && this.trigger == UIButtonMessage.Trigger.OnMouseOver) || (!isOver && this.trigger == UIButtonMessage.Trigger.OnMouseOut))
            {
                this.Send();
            }
            this.mHighlighted = isOver;
        }
    }

    private void OnPress(bool isPressed)
    {
        if (base.enabled && ((isPressed && this.trigger == UIButtonMessage.Trigger.OnPress) || (!isPressed && this.trigger == UIButtonMessage.Trigger.OnRelease)))
        {
            this.Send();
        }
    }

    private void Send()
    {
        if (string.IsNullOrEmpty(this.functionName))
        {
            return;
        }
        if (this.target == null)
        {
            this.target = base.gameObject;
        }
        if (this.includeChildren)
        {
            Transform[] componentsInChildren = this.target.GetComponentsInChildren<Transform>();
            int i = 0;
            int num = componentsInChildren.Length;
            while (i < num)
            {
                Transform transform = componentsInChildren[i];
                transform.gameObject.SendMessage(this.functionName, base.gameObject, SendMessageOptions.DontRequireReceiver);
                i++;
            }
        }
        else
        {
            this.target.SendMessage(this.functionName, base.gameObject, SendMessageOptions.DontRequireReceiver);
        }
    }

    private void Start()
    {
        this.mStarted = true;
    }
}