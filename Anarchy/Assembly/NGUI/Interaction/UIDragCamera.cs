using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("NGUI/Interaction/Drag Camera")]
public class UIDragCamera : IgnoreTimeScale
{
    [HideInInspector]
    [SerializeField]
    private Component target;

    public UIDraggableCamera draggableCamera;

    private void Awake()
    {
        if (this.target != null)
        {
            if (this.draggableCamera == null)
            {
                this.draggableCamera = this.target.GetComponent<UIDraggableCamera>();
                if (this.draggableCamera == null)
                {
                    this.draggableCamera = this.target.gameObject.AddComponent<UIDraggableCamera>();
                }
            }
            this.target = null;
        }
        else if (this.draggableCamera == null)
        {
            this.draggableCamera = NGUITools.FindInParents<UIDraggableCamera>(base.gameObject);
        }
    }

    private void OnDrag(Vector2 delta)
    {
        if (base.enabled && NGUITools.GetActive(base.gameObject) && this.draggableCamera != null)
        {
            this.draggableCamera.Drag(delta);
        }
    }

    private void OnPress(bool isPressed)
    {
        if (base.enabled && NGUITools.GetActive(base.gameObject) && this.draggableCamera != null)
        {
            this.draggableCamera.Press(isPressed);
        }
    }

    private void OnScroll(float delta)
    {
        if (base.enabled && NGUITools.GetActive(base.gameObject) && this.draggableCamera != null)
        {
            this.draggableCamera.Scroll(delta);
        }
    }
}