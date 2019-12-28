using Optimization.Caching;
using System;
using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("NGUI/Interaction/Scroll Bar")]
public class UIScrollBar : MonoBehaviour
{
    [SerializeField]
    [HideInInspector]
    private UISprite mBG;

    private Camera mCam;

    [HideInInspector]
    [SerializeField]
    private UIScrollBar.Direction mDir;

    [HideInInspector]
    [SerializeField]
    private UISprite mFG;

    [HideInInspector]
    [SerializeField]
    private bool mInverted;

    private bool mIsDirty;
    private Vector2 mScreenPos = Vectors.v2zero;

    [HideInInspector]
    [SerializeField]
    private float mScroll;

    [SerializeField]
    [HideInInspector]
    private float mSize = 1f;

    private Transform mTrans;
    public UIScrollBar.OnScrollBarChange onChange;

    public UIScrollBar.OnDragFinished onDragFinished;

    public delegate void OnDragFinished();

    public delegate void OnScrollBarChange(UIScrollBar sb);

    public enum Direction
    {
        Horizontal,
        Vertical
    }

    public float alpha
    {
        get
        {
            if (this.mFG != null)
            {
                return this.mFG.alpha;
            }
            if (this.mBG != null)
            {
                return this.mBG.alpha;
            }
            return 0f;
        }
        set
        {
            if (this.mFG != null)
            {
                this.mFG.alpha = value;
                NGUITools.SetActiveSelf(this.mFG.gameObject, this.mFG.alpha > 0.001f);
            }
            if (this.mBG != null)
            {
                this.mBG.alpha = value;
                NGUITools.SetActiveSelf(this.mBG.gameObject, this.mBG.alpha > 0.001f);
            }
        }
    }

    public UISprite background
    {
        get
        {
            return this.mBG;
        }
        set
        {
            if (this.mBG != value)
            {
                this.mBG = value;
                this.mIsDirty = true;
            }
        }
    }

    public float barSize
    {
        get
        {
            return this.mSize;
        }
        set
        {
            float num = Mathf.Clamp01(value);
            if (this.mSize != num)
            {
                this.mSize = num;
                this.mIsDirty = true;
                if (this.onChange != null)
                {
                    this.onChange(this);
                }
            }
        }
    }

    public Camera cachedCamera
    {
        get
        {
            if (this.mCam == null)
            {
                this.mCam = NGUITools.FindCameraForLayer(base.gameObject.layer);
            }
            return this.mCam;
        }
    }

    public Transform cachedTransform
    {
        get
        {
            if (this.mTrans == null)
            {
                this.mTrans = base.transform;
            }
            return this.mTrans;
        }
    }

    public UIScrollBar.Direction direction
    {
        get
        {
            return this.mDir;
        }
        set
        {
            if (this.mDir != value)
            {
                this.mDir = value;
                this.mIsDirty = true;
                if (this.mBG != null)
                {
                    Transform cachedTransform = this.mBG.cachedTransform;
                    Vector3 localScale = cachedTransform.localScale;
                    if ((this.mDir == UIScrollBar.Direction.Vertical && localScale.x > localScale.y) || (this.mDir == UIScrollBar.Direction.Horizontal && localScale.x < localScale.y))
                    {
                        float x = localScale.x;
                        localScale.x = localScale.y;
                        localScale.y = x;
                        cachedTransform.localScale = localScale;
                        this.ForceUpdate();
                        if (this.mBG.collider != null)
                        {
                            NGUITools.AddWidgetCollider(this.mBG.gameObject);
                        }
                        if (this.mFG.collider != null)
                        {
                            NGUITools.AddWidgetCollider(this.mFG.gameObject);
                        }
                    }
                }
            }
        }
    }

    public UISprite foreground
    {
        get
        {
            return this.mFG;
        }
        set
        {
            if (this.mFG != value)
            {
                this.mFG = value;
                this.mIsDirty = true;
            }
        }
    }

    public bool inverted
    {
        get
        {
            return this.mInverted;
        }
        set
        {
            if (this.mInverted != value)
            {
                this.mInverted = value;
                this.mIsDirty = true;
            }
        }
    }

    public float scrollValue
    {
        get
        {
            return this.mScroll;
        }
        set
        {
            float num = Mathf.Clamp01(value);
            if (this.mScroll != num)
            {
                this.mScroll = num;
                this.mIsDirty = true;
                if (this.onChange != null)
                {
                    this.onChange(this);
                }
            }
        }
    }

    private void CenterOnPos(Vector2 localPos)
    {
        if (this.mBG == null || this.mFG == null)
        {
            return;
        }
        Bounds bounds = NGUIMath.CalculateRelativeInnerBounds(this.cachedTransform, this.mBG);
        Bounds bounds2 = NGUIMath.CalculateRelativeInnerBounds(this.cachedTransform, this.mFG);
        if (this.mDir == UIScrollBar.Direction.Horizontal)
        {
            float num = bounds.size.x - bounds2.size.x;
            float num2 = num * 0.5f;
            float num3 = bounds.center.x - num2;
            float num4 = (num <= 0f) ? 0f : ((localPos.x - num3) / num);
            this.scrollValue = ((!this.mInverted) ? num4 : (1f - num4));
        }
        else
        {
            float num5 = bounds.size.y - bounds2.size.y;
            float num6 = num5 * 0.5f;
            float num7 = bounds.center.y - num6;
            float num8 = (num5 <= 0f) ? 0f : (1f - (localPos.y - num7) / num5);
            this.scrollValue = ((!this.mInverted) ? num8 : (1f - num8));
        }
    }

    private void OnDragBackground(GameObject go, Vector2 delta)
    {
        this.mCam = UICamera.currentCamera;
        this.Reposition(UICamera.lastTouchPosition);
    }

    private void OnDragForeground(GameObject go, Vector2 delta)
    {
        this.mCam = UICamera.currentCamera;
        this.Reposition(this.mScreenPos + UICamera.currentTouch.totalDelta);
    }

    private void OnPressBackground(GameObject go, bool isPressed)
    {
        this.mCam = UICamera.currentCamera;
        this.Reposition(UICamera.lastTouchPosition);
        if (!isPressed && this.onDragFinished != null)
        {
            this.onDragFinished();
        }
    }

    private void OnPressForeground(GameObject go, bool isPressed)
    {
        if (isPressed)
        {
            this.mCam = UICamera.currentCamera;
            Bounds bounds = NGUIMath.CalculateAbsoluteWidgetBounds(this.mFG.cachedTransform);
            this.mScreenPos = this.mCam.WorldToScreenPoint(bounds.center);
        }
        else if (this.onDragFinished != null)
        {
            this.onDragFinished();
        }
    }

    private void Reposition(Vector2 screenPos)
    {
        Transform cachedTransform = this.cachedTransform;
        Plane plane = new Plane(cachedTransform.rotation * Vectors.back, cachedTransform.position);
        Ray ray = this.cachedCamera.ScreenPointToRay(screenPos);
        float distance;
        if (!plane.Raycast(ray, out distance))
        {
            return;
        }
        this.CenterOnPos(cachedTransform.InverseTransformPoint(ray.GetPoint(distance)));
    }

    private void Start()
    {
        if (this.background != null && this.background.collider != null)
        {
            UIEventListener uieventListener = UIEventListener.Get(this.background.gameObject);
            UIEventListener uieventListener2 = uieventListener;
            uieventListener2.onPress = (UIEventListener.BoolDelegate)Delegate.Combine(uieventListener2.onPress, new UIEventListener.BoolDelegate(this.OnPressBackground));
            UIEventListener uieventListener3 = uieventListener;
            uieventListener3.onDrag = (UIEventListener.VectorDelegate)Delegate.Combine(uieventListener3.onDrag, new UIEventListener.VectorDelegate(this.OnDragBackground));
        }
        if (this.foreground != null && this.foreground.collider != null)
        {
            UIEventListener uieventListener4 = UIEventListener.Get(this.foreground.gameObject);
            UIEventListener uieventListener5 = uieventListener4;
            uieventListener5.onPress = (UIEventListener.BoolDelegate)Delegate.Combine(uieventListener5.onPress, new UIEventListener.BoolDelegate(this.OnPressForeground));
            UIEventListener uieventListener6 = uieventListener4;
            uieventListener6.onDrag = (UIEventListener.VectorDelegate)Delegate.Combine(uieventListener6.onDrag, new UIEventListener.VectorDelegate(this.OnDragForeground));
        }
        this.ForceUpdate();
    }

    private void Update()
    {
        if (this.mIsDirty)
        {
            this.ForceUpdate();
        }
    }

    public void ForceUpdate()
    {
        this.mIsDirty = false;
        if (this.mBG != null && this.mFG != null)
        {
            this.mSize = Mathf.Clamp01(this.mSize);
            this.mScroll = Mathf.Clamp01(this.mScroll);
            Vector4 border = this.mBG.border;
            Vector4 border2 = this.mFG.border;
            Vector2 vector = new Vector2(Mathf.Max(0f, this.mBG.cachedTransform.localScale.x - border.x - border.z), Mathf.Max(0f, this.mBG.cachedTransform.localScale.y - border.y - border.w));
            float num = (!this.mInverted) ? this.mScroll : (1f - this.mScroll);
            if (this.mDir == UIScrollBar.Direction.Horizontal)
            {
                Vector2 vector2 = new Vector2(vector.x * this.mSize, vector.y);
                this.mFG.pivot = UIWidget.Pivot.Left;
                this.mBG.pivot = UIWidget.Pivot.Left;
                this.mBG.cachedTransform.localPosition = Vectors.zero;
                this.mFG.cachedTransform.localPosition = new Vector3(border.x - border2.x + (vector.x - vector2.x) * num, 0f, 0f);
                this.mFG.cachedTransform.localScale = new Vector3(vector2.x + border2.x + border2.z, vector2.y + border2.y + border2.w, 1f);
                if (num < 0.999f && num > 0.001f)
                {
                    this.mFG.MakePixelPerfect();
                }
            }
            else
            {
                Vector2 vector3 = new Vector2(vector.x, vector.y * this.mSize);
                this.mFG.pivot = UIWidget.Pivot.Top;
                this.mBG.pivot = UIWidget.Pivot.Top;
                this.mBG.cachedTransform.localPosition = Vectors.zero;
                this.mFG.cachedTransform.localPosition = new Vector3(0f, -border.y + border2.y - (vector.y - vector3.y) * num, 0f);
                this.mFG.cachedTransform.localScale = new Vector3(vector3.x + border2.x + border2.z, vector3.y + border2.y + border2.w, 1f);
                if (num < 0.999f && num > 0.001f)
                {
                    this.mFG.MakePixelPerfect();
                }
            }
        }
    }
}