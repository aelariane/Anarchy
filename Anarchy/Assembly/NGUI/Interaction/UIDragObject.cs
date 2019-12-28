using Optimization.Caching;
using UnityEngine;

[AddComponentMenu("NGUI/Interaction/Drag Object")]
public class UIDragObject : IgnoreTimeScale
{
    private Bounds mBounds;
    private Vector3 mLastPos;
    private Vector3 mMomentum = Vectors.zero;
    private UIPanel mPanel;
    private Plane mPlane;
    private bool mPressed;
    private float mScroll;
    public UIDragObject.DragEffect dragEffect = UIDragObject.DragEffect.MomentumAndSpring;
    public float momentumAmount = 35f;
    public bool restrictWithinPanel;
    public Vector3 scale = Vectors.one;
    public float scrollWheelFactor;
    public Transform target;

    public enum DragEffect
    {
        None,
        Momentum,
        MomentumAndSpring
    }

    private void FindPanel()
    {
        this.mPanel = ((!(this.target != null)) ? null : UIPanel.Find(this.target.transform, false));
        if (this.mPanel == null)
        {
            this.restrictWithinPanel = false;
        }
    }

    private void LateUpdate()
    {
        float deltaTime = base.UpdateRealTimeDelta();
        if (this.target == null)
        {
            return;
        }
        if (this.mPressed)
        {
            SpringPosition component = this.target.GetComponent<SpringPosition>();
            if (component != null)
            {
                component.enabled = false;
            }
            this.mScroll = 0f;
        }
        else
        {
            this.mMomentum += this.scale * (-this.mScroll * 0.05f);
            this.mScroll = NGUIMath.SpringLerp(this.mScroll, 0f, 20f, deltaTime);
            if (this.mMomentum.magnitude > 0.0001f)
            {
                if (this.mPanel == null)
                {
                    this.FindPanel();
                }
                if (this.mPanel != null)
                {
                    this.target.position += NGUIMath.SpringDampen(ref this.mMomentum, 9f, deltaTime);
                    if (this.restrictWithinPanel && this.mPanel.clipping != UIDrawCall.Clipping.None)
                    {
                        this.mBounds = NGUIMath.CalculateRelativeWidgetBounds(this.mPanel.cachedTransform, this.target);
                        if (!this.mPanel.ConstrainTargetToBounds(this.target, ref this.mBounds, this.dragEffect == UIDragObject.DragEffect.None))
                        {
                            SpringPosition component2 = this.target.GetComponent<SpringPosition>();
                            if (component2 != null)
                            {
                                component2.enabled = false;
                            }
                        }
                    }
                    return;
                }
            }
            else
            {
                this.mScroll = 0f;
            }
        }
        NGUIMath.SpringDampen(ref this.mMomentum, 9f, deltaTime);
    }

    private void OnDrag(Vector2 delta)
    {
        if (base.enabled && NGUITools.GetActive(base.gameObject) && this.target != null)
        {
            UICamera.currentTouch.clickNotification = UICamera.ClickNotification.BasedOnDelta;
            Ray ray = UICamera.currentCamera.ScreenPointToRay(UICamera.currentTouch.pos);
            float distance = 0f;
            if (this.mPlane.Raycast(ray, out distance))
            {
                Vector3 point = ray.GetPoint(distance);
                Vector3 vector = point - this.mLastPos;
                this.mLastPos = point;
                if (vector.x != 0f || vector.y != 0f)
                {
                    vector = this.target.InverseTransformDirection(vector);
                    vector.Scale(this.scale);
                    vector = this.target.TransformDirection(vector);
                }
                if (this.dragEffect != UIDragObject.DragEffect.None)
                {
                    this.mMomentum = Vector3.Lerp(this.mMomentum, this.mMomentum + vector * (0.01f * this.momentumAmount), 0.67f);
                }
                if (this.restrictWithinPanel)
                {
                    Vector3 localPosition = this.target.localPosition;
                    this.target.position += vector;
                    this.mBounds.center = this.mBounds.center + (this.target.localPosition - localPosition);
                    if (this.dragEffect != UIDragObject.DragEffect.MomentumAndSpring && this.mPanel.clipping != UIDrawCall.Clipping.None && this.mPanel.ConstrainTargetToBounds(this.target, ref this.mBounds, true))
                    {
                        this.mMomentum = Vectors.zero;
                        this.mScroll = 0f;
                    }
                }
                else
                {
                    this.target.position += vector;
                }
            }
        }
    }

    private void OnPress(bool pressed)
    {
        if (base.enabled && NGUITools.GetActive(base.gameObject) && this.target != null)
        {
            this.mPressed = pressed;
            if (pressed)
            {
                if (this.restrictWithinPanel && this.mPanel == null)
                {
                    this.FindPanel();
                }
                if (this.restrictWithinPanel)
                {
                    this.mBounds = NGUIMath.CalculateRelativeWidgetBounds(this.mPanel.cachedTransform, this.target);
                }
                this.mMomentum = Vectors.zero;
                this.mScroll = 0f;
                SpringPosition component = this.target.GetComponent<SpringPosition>();
                if (component != null)
                {
                    component.enabled = false;
                }
                this.mLastPos = UICamera.lastHit.point;
                Transform transform = UICamera.currentCamera.transform;
                this.mPlane = new Plane(((!(this.mPanel != null)) ? transform.rotation : this.mPanel.cachedTransform.rotation) * Vectors.back, this.mLastPos);
            }
            else if (this.restrictWithinPanel && this.mPanel.clipping != UIDrawCall.Clipping.None && this.dragEffect == UIDragObject.DragEffect.MomentumAndSpring)
            {
                this.mPanel.ConstrainTargetToBounds(this.target, ref this.mBounds, false);
            }
        }
    }

    private void OnScroll(float delta)
    {
        if (base.enabled && NGUITools.GetActive(base.gameObject))
        {
            if (Mathf.Sign(this.mScroll) != Mathf.Sign(delta))
            {
                this.mScroll = 0f;
            }
            this.mScroll += delta * this.scrollWheelFactor;
        }
    }
}