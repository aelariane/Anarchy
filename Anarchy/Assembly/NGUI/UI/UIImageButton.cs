using UnityEngine;

[AddComponentMenu("NGUI/UI/Image Button")]
[ExecuteInEditMode]
public class UIImageButton : MonoBehaviour
{
    public string disabledSprite;
    public string hoverSprite;
    public string normalSprite;
    public string pressedSprite;
    public UISprite target;

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
                this.UpdateImage();
            }
        }
    }

    private void Awake()
    {
        if (this.target == null)
        {
            this.target = base.GetComponentInChildren<UISprite>();
        }
    }

    private void OnEnable()
    {
        this.UpdateImage();
    }

    private void OnHover(bool isOver)
    {
        if (this.isEnabled && this.target != null)
        {
            this.target.spriteName = ((!isOver) ? this.normalSprite : this.hoverSprite);
            this.target.MakePixelPerfect();
        }
    }

    private void OnPress(bool pressed)
    {
        if (pressed)
        {
            this.target.spriteName = this.pressedSprite;
            this.target.MakePixelPerfect();
        }
        else
        {
            this.UpdateImage();
        }
    }

    private void UpdateImage()
    {
        if (this.target != null)
        {
            if (this.isEnabled)
            {
                this.target.spriteName = ((!UICamera.IsHighlighted(base.gameObject)) ? this.normalSprite : this.hoverSprite);
            }
            else
            {
                this.target.spriteName = this.disabledSprite;
            }
            this.target.MakePixelPerfect();
        }
    }
}