using Optimization.Caching;
using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("NGUI/UI/Stretch")]
public class UIStretch : MonoBehaviour
{
    private Animation mAnim;
    private Rect mRect;
    private UIRoot mRoot;
    private Transform mTrans;
    public Vector2 initialSize = Vectors.v2one;
    public UIPanel panelContainer;
    public Vector2 relativeSize = Vectors.v2one;
    public UIStretch.Style style;
    public Camera uiCamera;

    public UIWidget widgetContainer;

    public enum Style
    {
        None,
        Horizontal,
        Vertical,
        Both,
        BasedOnHeight,
        FillKeepingRatio,
        FitInternalKeepingRatio
    }

    private void Awake()
    {
        this.mAnim = base.animation;
        this.mRect = default(Rect);
        this.mTrans = base.transform;
    }

    private void Start()
    {
        if (this.uiCamera == null)
        {
            this.uiCamera = NGUITools.FindCameraForLayer(base.gameObject.layer);
        }
        this.mRoot = NGUITools.FindInParents<UIRoot>(base.gameObject);
    }

    private void Update()
    {
        if (this.mAnim != null && this.mAnim.isPlaying)
        {
            return;
        }
        if (this.style != UIStretch.Style.None)
        {
            float num = 1f;
            if (this.panelContainer != null)
            {
                if (this.panelContainer.clipping == UIDrawCall.Clipping.None)
                {
                    this.mRect.xMin = (float)(-(float)Screen.width) * 0.5f;
                    this.mRect.yMin = (float)(-(float)Screen.height) * 0.5f;
                    this.mRect.xMax = -this.mRect.xMin;
                    this.mRect.yMax = -this.mRect.yMin;
                }
                else
                {
                    Vector4 clipRange = this.panelContainer.clipRange;
                    this.mRect.x = clipRange.x - clipRange.z * 0.5f;
                    this.mRect.y = clipRange.y - clipRange.w * 0.5f;
                    this.mRect.width = clipRange.z;
                    this.mRect.height = clipRange.w;
                }
            }
            else if (this.widgetContainer != null)
            {
                Transform cachedTransform = this.widgetContainer.cachedTransform;
                Vector3 localScale = cachedTransform.localScale;
                Vector3 localPosition = cachedTransform.localPosition;
                Vector3 vector = this.widgetContainer.relativeSize;
                Vector3 vector2 = this.widgetContainer.pivotOffset;
                vector2.y -= 1f;
                vector2.x *= this.widgetContainer.relativeSize.x * localScale.x;
                vector2.y *= this.widgetContainer.relativeSize.y * localScale.y;
                this.mRect.x = localPosition.x + vector2.x;
                this.mRect.y = localPosition.y + vector2.y;
                this.mRect.width = vector.x * localScale.x;
                this.mRect.height = vector.y * localScale.y;
            }
            else
            {
                if (!(this.uiCamera != null))
                {
                    return;
                }
                this.mRect = this.uiCamera.pixelRect;
                if (this.mRoot != null)
                {
                    num = this.mRoot.pixelSizeAdjustment;
                }
            }
            float num2 = this.mRect.width;
            float num3 = this.mRect.height;
            if (num != 1f && num3 > 1f)
            {
                float num4 = (float)this.mRoot.activeHeight / num3;
                num2 *= num4;
                num3 *= num4;
            }
            Vector3 localScale2 = this.mTrans.localScale;
            if (this.style == UIStretch.Style.BasedOnHeight)
            {
                localScale2.x = this.relativeSize.x * num3;
                localScale2.y = this.relativeSize.y * num3;
            }
            else if (this.style == UIStretch.Style.FillKeepingRatio)
            {
                float num5 = num2 / num3;
                float num6 = this.initialSize.x / this.initialSize.y;
                if (num6 < num5)
                {
                    float num7 = num2 / this.initialSize.x;
                    localScale2.x = num2;
                    localScale2.y = this.initialSize.y * num7;
                }
                else
                {
                    float num8 = num3 / this.initialSize.y;
                    localScale2.x = this.initialSize.x * num8;
                    localScale2.y = num3;
                }
            }
            else if (this.style == UIStretch.Style.FitInternalKeepingRatio)
            {
                float num9 = num2 / num3;
                float num10 = this.initialSize.x / this.initialSize.y;
                if (num10 > num9)
                {
                    float num11 = num2 / this.initialSize.x;
                    localScale2.x = num2;
                    localScale2.y = this.initialSize.y * num11;
                }
                else
                {
                    float num12 = num3 / this.initialSize.y;
                    localScale2.x = this.initialSize.x * num12;
                    localScale2.y = num3;
                }
            }
            else
            {
                if (this.style == UIStretch.Style.Both || this.style == UIStretch.Style.Horizontal)
                {
                    localScale2.x = this.relativeSize.x * num2;
                }
                if (this.style == UIStretch.Style.Both || this.style == UIStretch.Style.Vertical)
                {
                    localScale2.y = this.relativeSize.y * num3;
                }
            }
            if (this.mTrans.localScale != localScale2)
            {
                this.mTrans.localScale = localScale2;
            }
        }
    }
}