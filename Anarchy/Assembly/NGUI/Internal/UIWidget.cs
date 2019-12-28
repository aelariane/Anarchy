using Optimization.Caching;
using System;
using UnityEngine;

public abstract class UIWidget : MonoBehaviour
{
    [HideInInspector]
    [SerializeField]
    private Color mColor = Color.white;

    [HideInInspector]
    [SerializeField]
    private int mDepth;

    private bool mForceVisible;

    private UIGeometry mGeom = new UIGeometry();

    private float mLastAlpha;

    private Matrix4x4 mLocalToPanel;

    private Vector3 mOldV0;

    private Vector3 mOldV1;

    [HideInInspector]
    [SerializeField]
    private UIWidget.Pivot mPivot = UIWidget.Pivot.Center;

    private bool mVisibleByPanel = true;

    protected bool mChanged = true;

    protected GameObject mGo;

    [HideInInspector]
    [SerializeField]
    protected Material mMat;

    protected UIPanel mPanel;

    protected bool mPlayMode = true;

    [SerializeField]
    [HideInInspector]
    protected Texture mTex;

    protected Transform mTrans;

    public enum Pivot
    {
        TopLeft,
        Top,
        TopRight,
        Left,
        Center,
        Right,
        BottomLeft,
        Bottom,
        BottomRight
    }

    public float alpha
    {
        get
        {
            return this.mColor.a;
        }
        set
        {
            Color color = this.mColor;
            color.a = value;
            this.color = color;
        }
    }

    public virtual Vector4 border
    {
        get
        {
            return Vector4.zero;
        }
    }

    public GameObject cachedGameObject
    {
        get
        {
            if (this.mGo == null)
            {
                this.mGo = base.gameObject;
            }
            return this.mGo;
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

    public Color color
    {
        get
        {
            return this.mColor;
        }
        set
        {
            if (!this.mColor.Equals(value))
            {
                this.mColor = value;
                this.mChanged = true;
            }
        }
    }

    public int depth
    {
        get
        {
            return this.mDepth;
        }
        set
        {
            if (this.mDepth != value)
            {
                this.mDepth = value;
                if (this.mPanel != null)
                {
                    this.mPanel.MarkMaterialAsChanged(this.material, true);
                }
            }
        }
    }

    public float finalAlpha
    {
        get
        {
            if (this.mPanel == null)
            {
                this.CreatePanel();
            }
            return (!(this.mPanel != null)) ? this.mColor.a : (this.mColor.a * this.mPanel.alpha);
        }
    }

    public bool isVisible
    {
        get
        {
            return this.mVisibleByPanel && this.finalAlpha > 0.001f;
        }
    }

    public virtual bool keepMaterial
    {
        get
        {
            return false;
        }
    }

    public virtual Texture mainTexture
    {
        get
        {
            Material material = this.material;
            if (material != null)
            {
                if (material.mainTexture != null)
                {
                    this.mTex = material.mainTexture;
                }
                else if (this.mTex != null)
                {
                    if (this.mPanel != null)
                    {
                        this.mPanel.RemoveWidget(this);
                    }
                    this.mPanel = null;
                    this.mMat.mainTexture = this.mTex;
                    if (base.enabled)
                    {
                        this.CreatePanel();
                    }
                }
            }
            return this.mTex;
        }
        set
        {
            Material material = this.material;
            if (material == null || material.mainTexture != value)
            {
                if (this.mPanel != null)
                {
                    this.mPanel.RemoveWidget(this);
                }
                this.mPanel = null;
                this.mTex = value;
                material = this.material;
                if (material != null)
                {
                    material.mainTexture = value;
                    if (base.enabled)
                    {
                        this.CreatePanel();
                    }
                }
            }
        }
    }

    public virtual Material material
    {
        get
        {
            return this.mMat;
        }
        set
        {
            if (this.mMat != value)
            {
                if (this.mMat != null && this.mPanel != null)
                {
                    this.mPanel.RemoveWidget(this);
                }
                this.mPanel = null;
                this.mMat = value;
                this.mTex = null;
                if (this.mMat != null)
                {
                    this.CreatePanel();
                }
            }
        }
    }

    public UIPanel panel
    {
        get
        {
            if (this.mPanel == null)
            {
                this.CreatePanel();
            }
            return this.mPanel;
        }
        set
        {
            this.mPanel = value;
        }
    }

    public UIWidget.Pivot pivot
    {
        get
        {
            return this.mPivot;
        }
        set
        {
            if (this.mPivot != value)
            {
                Vector3 vector = NGUIMath.CalculateWidgetCorners(this)[0];
                this.mPivot = value;
                this.mChanged = true;
                Vector3 vector2 = NGUIMath.CalculateWidgetCorners(this)[0];
                Transform cachedTransform = this.cachedTransform;
                Vector3 vector3 = cachedTransform.position;
                float z = cachedTransform.localPosition.z;
                vector3.x += vector.x - vector2.x;
                vector3.y += vector.y - vector2.y;
                this.cachedTransform.position = vector3;
                vector3 = this.cachedTransform.localPosition;
                vector3.x = Mathf.Round(vector3.x);
                vector3.y = Mathf.Round(vector3.y);
                vector3.z = z;
                this.cachedTransform.localPosition = vector3;
            }
        }
    }

    public Vector2 pivotOffset
    {
        get
        {
            Vector2 zero = Vectors.v2zero;
            Vector4 relativePadding = this.relativePadding;
            UIWidget.Pivot pivot = this.pivot;
            if (pivot == UIWidget.Pivot.Top || pivot == UIWidget.Pivot.Center || pivot == UIWidget.Pivot.Bottom)
            {
                zero.x = (relativePadding.x - relativePadding.z - 1f) * 0.5f;
            }
            else if (pivot == UIWidget.Pivot.TopRight || pivot == UIWidget.Pivot.Right || pivot == UIWidget.Pivot.BottomRight)
            {
                zero.x = -1f - relativePadding.z;
            }
            else
            {
                zero.x = relativePadding.x;
            }
            if (pivot == UIWidget.Pivot.Left || pivot == UIWidget.Pivot.Center || pivot == UIWidget.Pivot.Right)
            {
                zero.y = (relativePadding.w - relativePadding.y + 1f) * 0.5f;
            }
            else if (pivot == UIWidget.Pivot.BottomLeft || pivot == UIWidget.Pivot.Bottom || pivot == UIWidget.Pivot.BottomRight)
            {
                zero.y = 1f + relativePadding.w;
            }
            else
            {
                zero.y = -relativePadding.y;
            }
            return zero;
        }
    }

    public virtual bool pixelPerfectAfterResize
    {
        get
        {
            return false;
        }
    }

    public virtual Vector4 relativePadding
    {
        get
        {
            return Vector4.zero;
        }
    }

    public virtual Vector2 relativeSize
    {
        get
        {
            return Vectors.v2one;
        }
    }

    private void OnDestroy()
    {
        if (this.mPanel != null)
        {
            this.mPanel.RemoveWidget(this);
            this.mPanel = null;
        }
    }

    private void OnDisable()
    {
        if (!this.keepMaterial)
        {
            this.material = null;
        }
        else if (this.mPanel != null)
        {
            this.mPanel.RemoveWidget(this);
        }
        this.mPanel = null;
    }

    private void Start()
    {
        this.OnStart();
        this.CreatePanel();
    }

    protected virtual void Awake()
    {
        this.mGo = base.gameObject;
        this.mPlayMode = Application.isPlaying;
    }

    protected virtual void OnEnable()
    {
        this.mChanged = true;
        if (!this.keepMaterial)
        {
            this.mMat = null;
            this.mTex = null;
        }
        this.mPanel = null;
    }

    protected virtual void OnStart()
    {
    }

    public static int CompareFunc(UIWidget left, UIWidget right)
    {
        if (left.mDepth > right.mDepth)
        {
            return 1;
        }
        if (left.mDepth < right.mDepth)
        {
            return -1;
        }
        return 0;
    }

    public static BetterList<UIWidget> Raycast(GameObject root, Vector2 mousePos)
    {
        BetterList<UIWidget> betterList = new BetterList<UIWidget>();
        UICamera uicamera = UICamera.FindCameraForLayer(root.layer);
        if (uicamera != null)
        {
            Camera cachedCamera = uicamera.cachedCamera;
            foreach (UIWidget uiwidget in root.GetComponentsInChildren<UIWidget>())
            {
                Vector3[] worldPoints = NGUIMath.CalculateWidgetCorners(uiwidget);
                if (NGUIMath.DistanceToRectangle(worldPoints, mousePos, cachedCamera) == 0f)
                {
                    betterList.Add(uiwidget);
                }
            }
            betterList.Sort((UIWidget w1, UIWidget w2) => w2.mDepth.CompareTo(w1.mDepth));
        }
        return betterList;
    }

    public void CheckLayer()
    {
        if (this.mPanel != null && this.mPanel.gameObject.layer != base.gameObject.layer)
        {
            Debug.LogWarning("You can't place widgets on a layer different than the UIPanel that manages them.\nIf you want to move widgets to a different layer, parent them to a new panel instead.", this);
            base.gameObject.layer = this.mPanel.gameObject.layer;
        }
    }

    [Obsolete("Use ParentHasChanged() instead")]
    public void CheckParent()
    {
        this.ParentHasChanged();
    }

    public void CreatePanel()
    {
        if (this.mPanel == null && base.enabled && NGUITools.GetActive(base.gameObject) && this.material != null)
        {
            this.mPanel = UIPanel.Find(this.cachedTransform);
            if (this.mPanel != null)
            {
                this.CheckLayer();
                this.mPanel.AddWidget(this);
                this.mChanged = true;
            }
        }
    }

    public virtual void MakePixelPerfect()
    {
        Vector3 localScale = this.cachedTransform.localScale;
        int num = Mathf.RoundToInt(localScale.x);
        int num2 = Mathf.RoundToInt(localScale.y);
        localScale.x = (float)num;
        localScale.y = (float)num2;
        localScale.z = 1f;
        Vector3 localPosition = this.cachedTransform.localPosition;
        localPosition.z = (float)Mathf.RoundToInt(localPosition.z);
        if (num % 2 == 1 && (this.pivot == UIWidget.Pivot.Top || this.pivot == UIWidget.Pivot.Center || this.pivot == UIWidget.Pivot.Bottom))
        {
            localPosition.x = Mathf.Floor(localPosition.x) + 0.5f;
        }
        else
        {
            localPosition.x = Mathf.Round(localPosition.x);
        }
        if (num2 % 2 == 1 && (this.pivot == UIWidget.Pivot.Left || this.pivot == UIWidget.Pivot.Center || this.pivot == UIWidget.Pivot.Right))
        {
            localPosition.y = Mathf.Ceil(localPosition.y) - 0.5f;
        }
        else
        {
            localPosition.y = Mathf.Round(localPosition.y);
        }
        this.cachedTransform.localPosition = localPosition;
        this.cachedTransform.localScale = localScale;
    }

    public virtual void MarkAsChanged()
    {
        this.mChanged = true;
        if (this.mPanel != null && base.enabled && NGUITools.GetActive(base.gameObject) && !Application.isPlaying && this.material != null)
        {
            this.mPanel.AddWidget(this);
            this.CheckLayer();
        }
    }

    public void MarkAsChangedLite()
    {
        this.mChanged = true;
    }

    public virtual void OnFill(BetterList<Vector3> verts, BetterList<Vector2> uvs, BetterList<Color32> cols)
    {
    }

    public void ParentHasChanged()
    {
        if (this.mPanel != null)
        {
            UIPanel y = UIPanel.Find(this.cachedTransform);
            if (this.mPanel != y)
            {
                this.mPanel.RemoveWidget(this);
                if (!this.keepMaterial || Application.isPlaying)
                {
                    this.material = null;
                }
                this.mPanel = null;
                this.CreatePanel();
            }
        }
    }

    public virtual void Update()
    {
        if (this.mPanel == null)
        {
            this.CreatePanel();
        }
    }

    public bool UpdateGeometry(UIPanel p, bool forceVisible)
    {
        if (this.material != null && p != null)
        {
            this.mPanel = p;
            bool flag = false;
            float finalAlpha = this.finalAlpha;
            bool flag2 = finalAlpha > 0.001f;
            bool flag3 = forceVisible || this.mVisibleByPanel;
            if (this.cachedTransform.hasChanged)
            {
                this.mTrans.hasChanged = false;
                if (!this.mPanel.widgetsAreStatic)
                {
                    Vector2 relativeSize = this.relativeSize;
                    Vector2 pivotOffset = this.pivotOffset;
                    Vector4 relativePadding = this.relativePadding;
                    float num = pivotOffset.x * relativeSize.x - relativePadding.x;
                    float num2 = pivotOffset.y * relativeSize.y + relativePadding.y;
                    float x = num + relativeSize.x + relativePadding.x + relativePadding.z;
                    float y = num2 - relativeSize.y - relativePadding.y - relativePadding.w;
                    this.mLocalToPanel = p.worldToLocal * this.cachedTransform.localToWorldMatrix;
                    flag = true;
                    Vector3 vector = new Vector3(num, num2, 0f);
                    Vector3 vector2 = new Vector3(x, y, 0f);
                    vector = this.mLocalToPanel.MultiplyPoint3x4(vector);
                    vector2 = this.mLocalToPanel.MultiplyPoint3x4(vector2);
                    if (Vector3.SqrMagnitude(this.mOldV0 - vector) > 1E-06f || Vector3.SqrMagnitude(this.mOldV1 - vector2) > 1E-06f)
                    {
                        this.mChanged = true;
                        this.mOldV0 = vector;
                        this.mOldV1 = vector2;
                    }
                }
                if (flag2 || this.mForceVisible != forceVisible)
                {
                    this.mForceVisible = forceVisible;
                    flag3 = (forceVisible || this.mPanel.IsVisible(this));
                }
            }
            else if (flag2 && this.mForceVisible != forceVisible)
            {
                this.mForceVisible = forceVisible;
                flag3 = this.mPanel.IsVisible(this);
            }
            if (this.mVisibleByPanel != flag3)
            {
                this.mVisibleByPanel = flag3;
                this.mChanged = true;
            }
            if (this.mVisibleByPanel && this.mLastAlpha != finalAlpha)
            {
                this.mChanged = true;
            }
            this.mLastAlpha = finalAlpha;
            if (this.mChanged)
            {
                this.mChanged = false;
                if (this.isVisible)
                {
                    this.mGeom.Clear();
                    this.OnFill(this.mGeom.verts, this.mGeom.uvs, this.mGeom.cols);
                    if (this.mGeom.hasVertices)
                    {
                        Vector3 pivotOffset2 = this.pivotOffset;
                        Vector2 relativeSize2 = this.relativeSize;
                        pivotOffset2.x *= relativeSize2.x;
                        pivotOffset2.y *= relativeSize2.y;
                        if (!flag)
                        {
                            this.mLocalToPanel = p.worldToLocal * this.cachedTransform.localToWorldMatrix;
                        }
                        this.mGeom.ApplyOffset(pivotOffset2);
                        this.mGeom.ApplyTransform(this.mLocalToPanel, p.generateNormals);
                    }
                    return true;
                }
                if (this.mGeom.hasVertices)
                {
                    this.mGeom.Clear();
                    return true;
                }
            }
        }
        return false;
    }

    public void WriteToBuffers(BetterList<Vector3> v, BetterList<Vector2> u, BetterList<Color32> c, BetterList<Vector3> n, BetterList<Vector4> t)
    {
        this.mGeom.WriteToBuffers(v, u, c, n, t);
    }
}