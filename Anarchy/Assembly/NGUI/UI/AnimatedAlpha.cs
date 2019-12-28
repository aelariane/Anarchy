using UnityEngine;

public class AnimatedAlpha : MonoBehaviour
{
    private UIPanel mPanel;
    private UIWidget mWidget;
    public float alpha = 1f;

    private void Awake()
    {
        this.mWidget = base.GetComponent<UIWidget>();
        this.mPanel = base.GetComponent<UIPanel>();
        this.Update();
    }

    private void Update()
    {
        if (this.mWidget != null)
        {
            this.mWidget.alpha = this.alpha;
        }
        if (this.mPanel != null)
        {
            this.mPanel.alpha = this.alpha;
        }
    }
}