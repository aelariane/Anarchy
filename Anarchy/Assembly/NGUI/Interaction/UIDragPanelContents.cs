using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("NGUI/Interaction/Drag Panel Contents")]
public class UIDragPanelContents : MonoBehaviour
{
    [HideInInspector]
    [SerializeField]
    private UIPanel panel;

    public UIDraggablePanel draggablePanel;

    private void Awake()
    {
        if (this.panel != null)
        {
            if (this.draggablePanel == null)
            {
                this.draggablePanel = this.panel.GetComponent<UIDraggablePanel>();
                if (this.draggablePanel == null)
                {
                    this.draggablePanel = this.panel.gameObject.AddComponent<UIDraggablePanel>();
                }
            }
            this.panel = null;
        }
    }

    private void OnDrag(Vector2 delta)
    {
        if (base.enabled && NGUITools.GetActive(base.gameObject) && this.draggablePanel != null)
        {
            this.draggablePanel.Drag();
        }
    }

    private void OnPress(bool pressed)
    {
        if (base.enabled && NGUITools.GetActive(base.gameObject) && this.draggablePanel != null)
        {
            this.draggablePanel.Press(pressed);
        }
    }

    private void OnScroll(float delta)
    {
        if (base.enabled && NGUITools.GetActive(base.gameObject) && this.draggablePanel != null)
        {
            this.draggablePanel.Scroll(delta);
        }
    }

    private void Start()
    {
        if (this.draggablePanel == null)
        {
            this.draggablePanel = NGUITools.FindInParents<UIDraggablePanel>(base.gameObject);
        }
    }
}