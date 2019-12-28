using Optimization.Caching;
using System;
using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("NGUI/UI/Panel")]
public class UIPanel : MonoBehaviour
{
    private static float[] mTemp = new float[4];

    [SerializeField]
    [HideInInspector]
    private float mAlpha = 1f;

    private Camera mCam;
    private BetterList<Material> mChanged = new BetterList<Material>();
    private UIPanel[] mChildPanels;

    [HideInInspector]
    [SerializeField]
    private UIDrawCall.Clipping mClipping;

    [SerializeField]
    [HideInInspector]
    private Vector4 mClipRange = Vector4.zero;

    [HideInInspector]
    [SerializeField]
    private Vector2 mClipSoftness = new Vector2(40f, 40f);

    private BetterList<Color32> mCols = new BetterList<Color32>();
    private float mCullTime;

    [HideInInspector]
    [SerializeField]
    private UIPanel.DebugInfo mDebugInfo = UIPanel.DebugInfo.Gizmos;

    private bool mDepthChanged;
    private BetterList<UIDrawCall> mDrawCalls = new BetterList<UIDrawCall>();
    private GameObject mGo;
    private int mLayer = -1;
    private float mMatrixTime;
    private Vector2 mMax = Vectors.v2zero;
    private Vector2 mMin = Vectors.v2zero;
    private BetterList<Vector3> mNorms = new BetterList<Vector3>();
    private BetterList<Vector4> mTans = new BetterList<Vector4>();
    private Transform mTrans;
    private float mUpdateTime;
    private BetterList<Vector2> mUvs = new BetterList<Vector2>();
    private BetterList<Vector3> mVerts = new BetterList<Vector3>();
    private BetterList<UIWidget> mWidgets = new BetterList<UIWidget>();
    public bool cullWhileDragging;
    public bool depthPass;
    public bool generateNormals;
    public UIPanel.OnChangeDelegate onChange;

    public bool showInPanelTool = true;
    public bool widgetsAreStatic;

    [HideInInspector]
    public Matrix4x4 worldToLocal = Matrix4x4.identity;

    public delegate void OnChangeDelegate();

    public enum DebugInfo
    {
        None,
        Gizmos,
        Geometry
    }

    public float alpha
    {
        get
        {
            return this.mAlpha;
        }
        set
        {
            float num = Mathf.Clamp01(value);
            if (this.mAlpha != num)
            {
                this.mAlpha = num;
                for (int i = 0; i < this.mDrawCalls.size; i++)
                {
                    UIDrawCall uidrawCall = this.mDrawCalls[i];
                    this.MarkMaterialAsChanged(uidrawCall.material, false);
                }
                for (int j = 0; j < this.mWidgets.size; j++)
                {
                    this.mWidgets[j].MarkAsChangedLite();
                }
            }
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

    public UIDrawCall.Clipping clipping
    {
        get
        {
            return this.mClipping;
        }
        set
        {
            if (this.mClipping != value)
            {
                this.mClipping = value;
                this.mMatrixTime = 0f;
                this.UpdateDrawcalls();
            }
        }
    }

    public Vector4 clipRange
    {
        get
        {
            return this.mClipRange;
        }
        set
        {
            if (this.mClipRange != value)
            {
                this.mCullTime = ((this.mCullTime != 0f) ? (Time.realtimeSinceStartup + 0.15f) : 0.001f);
                this.mClipRange = value;
                this.mMatrixTime = 0f;
                this.UpdateDrawcalls();
            }
        }
    }

    public Vector2 clipSoftness
    {
        get
        {
            return this.mClipSoftness;
        }
        set
        {
            if (this.mClipSoftness != value)
            {
                this.mClipSoftness = value;
                this.UpdateDrawcalls();
            }
        }
    }

    public UIPanel.DebugInfo debugInfo
    {
        get
        {
            return this.mDebugInfo;
        }
        set
        {
            if (this.mDebugInfo != value)
            {
                this.mDebugInfo = value;
                BetterList<UIDrawCall> drawCalls = this.drawCalls;
                HideFlags hideFlags = (this.mDebugInfo != UIPanel.DebugInfo.Geometry) ? HideFlags.HideAndDontSave : (HideFlags.DontSave | HideFlags.NotEditable);
                int i = 0;
                int size = drawCalls.size;
                while (i < size)
                {
                    UIDrawCall uidrawCall = drawCalls[i];
                    GameObject gameObject = uidrawCall.gameObject;
                    NGUITools.SetActiveSelf(gameObject, false);
                    gameObject.hideFlags = hideFlags;
                    NGUITools.SetActiveSelf(gameObject, true);
                    i++;
                }
            }
        }
    }

    public BetterList<UIDrawCall> drawCalls
    {
        get
        {
            int i = this.mDrawCalls.size;
            while (i > 0)
            {
                UIDrawCall x = this.mDrawCalls[--i];
                if (x == null)
                {
                    this.mDrawCalls.RemoveAt(i);
                }
            }
            return this.mDrawCalls;
        }
    }

    public BetterList<UIWidget> widgets
    {
        get
        {
            return this.mWidgets;
        }
    }

    private static void SetChildLayer(Transform t, int layer)
    {
        for (int i = 0; i < t.childCount; i++)
        {
            Transform child = t.GetChild(i);
            if (child.GetComponent<UIPanel>() == null)
            {
                if (child.GetComponent<UIWidget>() != null)
                {
                    child.gameObject.layer = layer;
                }
                UIPanel.SetChildLayer(child, layer);
            }
        }
    }

    private void Awake()
    {
        this.mGo = base.gameObject;
        this.mTrans = base.transform;
    }

    private void Fill(Material mat)
    {
        int i = 0;
        while (i < this.mWidgets.size)
        {
            UIWidget uiwidget = this.mWidgets.buffer[i];
            if (uiwidget == null)
            {
                this.mWidgets.RemoveAt(i);
            }
            else
            {
                if (uiwidget.material == mat && uiwidget.isVisible)
                {
                    if (!(uiwidget.panel == this))
                    {
                        this.mWidgets.RemoveAt(i);
                        continue;
                    }
                    if (this.generateNormals)
                    {
                        uiwidget.WriteToBuffers(this.mVerts, this.mUvs, this.mCols, this.mNorms, this.mTans);
                    }
                    else
                    {
                        uiwidget.WriteToBuffers(this.mVerts, this.mUvs, this.mCols, null, null);
                    }
                }
                i++;
            }
        }
        if (this.mVerts.size > 0)
        {
            UIDrawCall drawCall = this.GetDrawCall(mat, true);
            drawCall.depthPass = (this.depthPass && this.mClipping == UIDrawCall.Clipping.None);
            drawCall.Set(this.mVerts, (!this.generateNormals) ? null : this.mNorms, (!this.generateNormals) ? null : this.mTans, this.mUvs, this.mCols);
        }
        else
        {
            UIDrawCall drawCall2 = this.GetDrawCall(mat, false);
            if (drawCall2 != null)
            {
                this.mDrawCalls.Remove(drawCall2);
                NGUITools.DestroyImmediate(drawCall2.gameObject);
            }
        }
        this.mVerts.Clear();
        this.mNorms.Clear();
        this.mTans.Clear();
        this.mUvs.Clear();
        this.mCols.Clear();
    }

    private UIDrawCall GetDrawCall(Material mat, bool createIfMissing)
    {
        int i = 0;
        int size = this.drawCalls.size;
        while (i < size)
        {
            UIDrawCall uidrawCall = this.drawCalls.buffer[i];
            if (uidrawCall.material == mat)
            {
                return uidrawCall;
            }
            i++;
        }
        UIDrawCall uidrawCall2 = null;
        if (createIfMissing)
        {
            GameObject gameObject = new GameObject("_UIDrawCall [" + mat.name + "]");
            UnityEngine.Object.DontDestroyOnLoad(gameObject);
            gameObject.layer = this.cachedGameObject.layer;
            uidrawCall2 = gameObject.AddComponent<UIDrawCall>();
            uidrawCall2.material = mat;
            this.mDrawCalls.Add(uidrawCall2);
        }
        return uidrawCall2;
    }

    private bool IsVisible(Vector3 a, Vector3 b, Vector3 c, Vector3 d)
    {
        this.UpdateTransformMatrix();
        a = this.worldToLocal.MultiplyPoint3x4(a);
        b = this.worldToLocal.MultiplyPoint3x4(b);
        c = this.worldToLocal.MultiplyPoint3x4(c);
        d = this.worldToLocal.MultiplyPoint3x4(d);
        UIPanel.mTemp[0] = a.x;
        UIPanel.mTemp[1] = b.x;
        UIPanel.mTemp[2] = c.x;
        UIPanel.mTemp[3] = d.x;
        float num = Mathf.Min(UIPanel.mTemp);
        float num2 = Mathf.Max(UIPanel.mTemp);
        UIPanel.mTemp[0] = a.y;
        UIPanel.mTemp[1] = b.y;
        UIPanel.mTemp[2] = c.y;
        UIPanel.mTemp[3] = d.y;
        float num3 = Mathf.Min(UIPanel.mTemp);
        float num4 = Mathf.Max(UIPanel.mTemp);
        return num2 >= this.mMin.x && num4 >= this.mMin.y && num <= this.mMax.x && num3 <= this.mMax.y;
    }

    private void LateUpdate()
    {
        this.mUpdateTime = Time.realtimeSinceStartup;
        this.UpdateTransformMatrix();
        if (this.mLayer != this.cachedGameObject.layer)
        {
            this.mLayer = this.mGo.layer;
            UICamera uicamera = UICamera.FindCameraForLayer(this.mLayer);
            this.mCam = ((!(uicamera != null)) ? NGUITools.FindCameraForLayer(this.mLayer) : uicamera.cachedCamera);
            UIPanel.SetChildLayer(this.cachedTransform, this.mLayer);
            int i = 0;
            int size = this.drawCalls.size;
            while (i < size)
            {
                this.mDrawCalls.buffer[i].gameObject.layer = this.mLayer;
                i++;
            }
        }
        bool forceVisible = !this.cullWhileDragging && (this.clipping == UIDrawCall.Clipping.None || this.mCullTime > this.mUpdateTime);
        int j = 0;
        int size2 = this.mWidgets.size;
        while (j < size2)
        {
            UIWidget uiwidget = this.mWidgets[j];
            if (uiwidget.UpdateGeometry(this, forceVisible) && !this.mChanged.Contains(uiwidget.material))
            {
                this.mChanged.Add(uiwidget.material);
            }
            j++;
        }
        if (this.mChanged.size != 0 && this.onChange != null)
        {
            this.onChange();
        }
        if (this.mDepthChanged)
        {
            this.mDepthChanged = false;
            this.mWidgets.Sort(new Comparison<UIWidget>(UIWidget.CompareFunc));
        }
        int k = 0;
        int size3 = this.mChanged.size;
        while (k < size3)
        {
            this.Fill(this.mChanged.buffer[k]);
            k++;
        }
        this.UpdateDrawcalls();
        this.mChanged.Clear();
    }

    private void OnDisable()
    {
        int i = this.mDrawCalls.size;
        while (i > 0)
        {
            UIDrawCall uidrawCall = this.mDrawCalls.buffer[--i];
            if (uidrawCall != null)
            {
                NGUITools.DestroyImmediate(uidrawCall.gameObject);
            }
        }
        this.mDrawCalls.Clear();
        this.mChanged.Clear();
    }

    private void OnEnable()
    {
        int i = 0;
        while (i < this.mWidgets.size)
        {
            UIWidget uiwidget = this.mWidgets.buffer[i];
            if (uiwidget != null)
            {
                this.MarkMaterialAsChanged(uiwidget.material, true);
                i++;
            }
            else
            {
                this.mWidgets.RemoveAt(i);
            }
        }
    }

    private void Start()
    {
        this.mLayer = this.mGo.layer;
        UICamera uicamera = UICamera.FindCameraForLayer(this.mLayer);
        this.mCam = ((!(uicamera != null)) ? NGUITools.FindCameraForLayer(this.mLayer) : uicamera.cachedCamera);
    }

    private void UpdateTransformMatrix()
    {
        if (this.mUpdateTime == 0f || this.mMatrixTime != this.mUpdateTime)
        {
            this.mMatrixTime = this.mUpdateTime;
            this.worldToLocal = this.cachedTransform.worldToLocalMatrix;
            if (this.mClipping != UIDrawCall.Clipping.None)
            {
                Vector2 a = new Vector2(this.mClipRange.z, this.mClipRange.w);
                if (a.x == 0f)
                {
                    a.x = ((!(this.mCam == null)) ? this.mCam.pixelWidth : ((float)Screen.width));
                }
                if (a.y == 0f)
                {
                    a.y = ((!(this.mCam == null)) ? this.mCam.pixelHeight : ((float)Screen.height));
                }
                a *= 0.5f;
                this.mMin.x = this.mClipRange.x - a.x;
                this.mMin.y = this.mClipRange.y - a.y;
                this.mMax.x = this.mClipRange.x + a.x;
                this.mMax.y = this.mClipRange.y + a.y;
            }
        }
    }

    public static UIPanel Find(Transform trans, bool createIfMissing)
    {
        Transform y = trans;
        UIPanel uipanel = null;
        while (uipanel == null && trans != null)
        {
            uipanel = trans.GetComponent<UIPanel>();
            if (uipanel != null)
            {
                break;
            }
            if (trans.parent == null)
            {
                break;
            }
            trans = trans.parent;
        }
        if (createIfMissing && uipanel == null && trans != y)
        {
            uipanel = trans.gameObject.AddComponent<UIPanel>();
            UIPanel.SetChildLayer(uipanel.cachedTransform, uipanel.cachedGameObject.layer);
        }
        return uipanel;
    }

    public static UIPanel Find(Transform trans)
    {
        return UIPanel.Find(trans, true);
    }

    public void AddWidget(UIWidget w)
    {
        if (w != null && !this.mWidgets.Contains(w))
        {
            this.mWidgets.Add(w);
            if (!this.mChanged.Contains(w.material))
            {
                this.mChanged.Add(w.material);
            }
            this.mDepthChanged = true;
        }
    }

    public Vector3 CalculateConstrainOffset(Vector2 min, Vector2 max)
    {
        float num = this.clipRange.z * 0.5f;
        float num2 = this.clipRange.w * 0.5f;
        Vector2 minRect = new Vector2(min.x, min.y);
        Vector2 maxRect = new Vector2(max.x, max.y);
        Vector2 minArea = new Vector2(this.clipRange.x - num, this.clipRange.y - num2);
        Vector2 maxArea = new Vector2(this.clipRange.x + num, this.clipRange.y + num2);
        if (this.clipping == UIDrawCall.Clipping.SoftClip)
        {
            minArea.x += this.clipSoftness.x;
            minArea.y += this.clipSoftness.y;
            maxArea.x -= this.clipSoftness.x;
            maxArea.y -= this.clipSoftness.y;
        }
        return NGUIMath.ConstrainRect(minRect, maxRect, minArea, maxArea);
    }

    public bool ConstrainTargetToBounds(Transform target, ref Bounds targetBounds, bool immediate)
    {
        Vector3 b = this.CalculateConstrainOffset(targetBounds.min, targetBounds.max);
        if (b.magnitude > 0f)
        {
            if (immediate)
            {
                target.localPosition += b;
                targetBounds.center += b;
                SpringPosition component = target.GetComponent<SpringPosition>();
                if (component != null)
                {
                    component.enabled = false;
                }
            }
            else
            {
                SpringPosition springPosition = SpringPosition.Begin(target.gameObject, target.localPosition + b, 13f);
                springPosition.ignoreTimeScale = true;
                springPosition.worldSpace = false;
            }
            return true;
        }
        return false;
    }

    public bool ConstrainTargetToBounds(Transform target, bool immediate)
    {
        Bounds bounds = NGUIMath.CalculateRelativeWidgetBounds(this.cachedTransform, target);
        return this.ConstrainTargetToBounds(target, ref bounds, immediate);
    }

    public bool IsVisible(Vector3 worldPos)
    {
        if (this.mAlpha < 0.001f)
        {
            return false;
        }
        if (this.mClipping == UIDrawCall.Clipping.None)
        {
            return true;
        }
        this.UpdateTransformMatrix();
        Vector3 vector = this.worldToLocal.MultiplyPoint3x4(worldPos);
        return vector.x >= this.mMin.x && vector.y >= this.mMin.y && vector.x <= this.mMax.x && vector.y <= this.mMax.y;
    }

    public bool IsVisible(UIWidget w)
    {
        if (this.mAlpha < 0.001f)
        {
            return false;
        }
        if (!w.enabled || !NGUITools.GetActive(w.cachedGameObject) || w.alpha < 0.001f)
        {
            return false;
        }
        if (this.mClipping == UIDrawCall.Clipping.None)
        {
            return true;
        }
        Vector2 relativeSize = w.relativeSize;
        Vector2 vector = Vector2.Scale(w.pivotOffset, relativeSize);
        Vector2 v = vector;
        vector.x += relativeSize.x;
        vector.y -= relativeSize.y;
        Transform cachedTransform = w.cachedTransform;
        Vector3 a = cachedTransform.TransformPoint(vector);
        Vector3 b = cachedTransform.TransformPoint(new Vector2(vector.x, v.y));
        Vector3 c = cachedTransform.TransformPoint(new Vector2(v.x, vector.y));
        Vector3 d = cachedTransform.TransformPoint(v);
        return this.IsVisible(a, b, c, d);
    }

    public void MarkMaterialAsChanged(Material mat, bool sort)
    {
        if (mat != null)
        {
            if (sort)
            {
                this.mDepthChanged = true;
            }
            if (!this.mChanged.Contains(mat))
            {
                this.mChanged.Add(mat);
            }
        }
    }

    public void Refresh()
    {
        UIWidget[] componentsInChildren = base.GetComponentsInChildren<UIWidget>();
        int i = 0;
        int num = componentsInChildren.Length;
        while (i < num)
        {
            componentsInChildren[i].Update();
            i++;
        }
        this.LateUpdate();
    }

    public void RemoveWidget(UIWidget w)
    {
        if (w != null && w != null && this.mWidgets.Remove(w) && w.material != null)
        {
            this.mChanged.Add(w.material);
        }
    }

    public void SetAlphaRecursive(float val, bool rebuildList)
    {
        if (rebuildList || this.mChildPanels == null)
        {
            this.mChildPanels = base.GetComponentsInChildren<UIPanel>(true);
        }
        int i = 0;
        int num = this.mChildPanels.Length;
        while (i < num)
        {
            this.mChildPanels[i].alpha = val;
            i++;
        }
    }

    public void UpdateDrawcalls()
    {
        Vector4 zero = Vector4.zero;
        if (this.mClipping != UIDrawCall.Clipping.None)
        {
            zero = new Vector4(this.mClipRange.x, this.mClipRange.y, this.mClipRange.z * 0.5f, this.mClipRange.w * 0.5f);
        }
        if (zero.z == 0f)
        {
            zero.z = (float)Screen.width * 0.5f;
        }
        if (zero.w == 0f)
        {
            zero.w = (float)Screen.height * 0.5f;
        }
        RuntimePlatform platform = Application.platform;
        if (platform == RuntimePlatform.WindowsPlayer || platform == RuntimePlatform.WindowsWebPlayer || platform == RuntimePlatform.WindowsEditor)
        {
            zero.x -= 0.5f;
            zero.y += 0.5f;
        }
        Transform cachedTransform = this.cachedTransform;
        int i = 0;
        int size = this.mDrawCalls.size;
        while (i < size)
        {
            UIDrawCall uidrawCall = this.mDrawCalls.buffer[i];
            uidrawCall.clipping = this.mClipping;
            uidrawCall.clipRange = zero;
            uidrawCall.clipSoftness = this.mClipSoftness;
            uidrawCall.depthPass = (this.depthPass && this.mClipping == UIDrawCall.Clipping.None);
            Transform transform = uidrawCall.transform;
            transform.position = cachedTransform.position;
            transform.rotation = cachedTransform.rotation;
            transform.localScale = cachedTransform.lossyScale;
            i++;
        }
    }
}