using UnityEngine;

[AddComponentMenu("NGUI/Internal/Event Listener")]
public class UIEventListener : MonoBehaviour
{
    public UIEventListener.VoidDelegate onClick;
    public UIEventListener.VoidDelegate onDoubleClick;
    public UIEventListener.VectorDelegate onDrag;
    public UIEventListener.ObjectDelegate onDrop;
    public UIEventListener.BoolDelegate onHover;
    public UIEventListener.StringDelegate onInput;
    public UIEventListener.KeyCodeDelegate onKey;
    public UIEventListener.BoolDelegate onPress;
    public UIEventListener.FloatDelegate onScroll;
    public UIEventListener.BoolDelegate onSelect;
    public UIEventListener.VoidDelegate onSubmit;
    public object parameter;

    public delegate void BoolDelegate(GameObject go, bool state);

    public delegate void FloatDelegate(GameObject go, float delta);

    public delegate void KeyCodeDelegate(GameObject go, KeyCode key);

    public delegate void ObjectDelegate(GameObject go, GameObject draggedObject);

    public delegate void StringDelegate(GameObject go, string text);

    public delegate void VectorDelegate(GameObject go, Vector2 delta);

    public delegate void VoidDelegate(GameObject go);

    private void OnClick()
    {
        if (this.onClick != null)
        {
            this.onClick(base.gameObject);
        }
    }

    private void OnDoubleClick()
    {
        if (this.onDoubleClick != null)
        {
            this.onDoubleClick(base.gameObject);
        }
    }

    private void OnDrag(Vector2 delta)
    {
        if (this.onDrag != null)
        {
            this.onDrag(base.gameObject, delta);
        }
    }

    private void OnDrop(GameObject go)
    {
        if (this.onDrop != null)
        {
            this.onDrop(base.gameObject, go);
        }
    }

    private void OnHover(bool isOver)
    {
        if (this.onHover != null)
        {
            this.onHover(base.gameObject, isOver);
        }
    }

    private void OnInput(string text)
    {
        if (this.onInput != null)
        {
            this.onInput(base.gameObject, text);
        }
    }

    private void OnKey(KeyCode key)
    {
        if (this.onKey != null)
        {
            this.onKey(base.gameObject, key);
        }
    }

    private void OnPress(bool isPressed)
    {
        if (this.onPress != null)
        {
            this.onPress(base.gameObject, isPressed);
        }
    }

    private void OnScroll(float delta)
    {
        if (this.onScroll != null)
        {
            this.onScroll(base.gameObject, delta);
        }
    }

    private void OnSelect(bool selected)
    {
        if (this.onSelect != null)
        {
            this.onSelect(base.gameObject, selected);
        }
    }

    private void OnSubmit()
    {
        if (this.onSubmit != null)
        {
            this.onSubmit(base.gameObject);
        }
    }

    public static UIEventListener Get(GameObject go)
    {
        UIEventListener uieventListener = go.GetComponent<UIEventListener>();
        if (uieventListener == null)
        {
            uieventListener = go.AddComponent<UIEventListener>();
        }
        return uieventListener;
    }
}