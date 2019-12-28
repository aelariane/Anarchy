using Optimization.Caching;
using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("NGUI/UI/Label")]
public class UILabel : UIWidget
{
    [SerializeField]
    [HideInInspector]
    private Color mEffectColor = Color.black;

    [SerializeField]
    [HideInInspector]
    private Vector2 mEffectDistance = Vectors.v2one;

    [HideInInspector]
    [SerializeField]
    private UILabel.Effect mEffectStyle;

    [SerializeField]
    [HideInInspector]
    private bool mEncoding = true;

    [SerializeField]
    [HideInInspector]
    private UIFont mFont;

    private int mLastCount;

    private UILabel.Effect mLastEffect;

    private bool mLastEncoding = true;

    private bool mLastPass;

    private Vector3 mLastScale = Vectors.one;

    private bool mLastShow;

    private string mLastText = string.Empty;

    private int mLastWidth;

    [SerializeField]
    [HideInInspector]
    private float mLineWidth;

    [SerializeField]
    [HideInInspector]
    private int mMaxLineCount;

    [HideInInspector]
    [SerializeField]
    private int mMaxLineWidth;

    [HideInInspector]
    [SerializeField]
    private bool mMultiline = true;

    [HideInInspector]
    [SerializeField]
    private bool mPassword;

    private bool mPremultiply;

    private string mProcessedText;

    private bool mShouldBeProcessed = true;

    [HideInInspector]
    [SerializeField]
    private bool mShowLastChar;

    [SerializeField]
    [HideInInspector]
    private bool mShrinkToFit;

    private Vector2 mSize = Vectors.v2zero;

    [SerializeField]
    [HideInInspector]
    private UIFont.SymbolStyle mSymbols = UIFont.SymbolStyle.Uncolored;

    [HideInInspector]
    [SerializeField]
    private string mText = string.Empty;

    public enum Effect
    {
        None,
        Shadow,
        Outline
    }

    private bool hasChanged
    {
        get
        {
            return this.mShouldBeProcessed || this.mLastText != this.text || this.mLastWidth != this.mMaxLineWidth || this.mLastEncoding != this.mEncoding || this.mLastCount != this.mMaxLineCount || this.mLastPass != this.mPassword || this.mLastShow != this.mShowLastChar || this.mLastEffect != this.mEffectStyle;
        }
        set
        {
            if (value)
            {
                this.mChanged = true;
                this.mShouldBeProcessed = true;
            }
            else
            {
                this.mShouldBeProcessed = false;
                this.mLastText = this.text;
                this.mLastWidth = this.mMaxLineWidth;
                this.mLastEncoding = this.mEncoding;
                this.mLastCount = this.mMaxLineCount;
                this.mLastPass = this.mPassword;
                this.mLastShow = this.mShowLastChar;
                this.mLastEffect = this.mEffectStyle;
            }
        }
    }

    public Color effectColor
    {
        get
        {
            return this.mEffectColor;
        }
        set
        {
            if (!this.mEffectColor.Equals(value))
            {
                this.mEffectColor = value;
                if (this.mEffectStyle != UILabel.Effect.None)
                {
                    this.hasChanged = true;
                }
            }
        }
    }

    public Vector2 effectDistance
    {
        get
        {
            return this.mEffectDistance;
        }
        set
        {
            if (this.mEffectDistance != value)
            {
                this.mEffectDistance = value;
                this.hasChanged = true;
            }
        }
    }

    public UILabel.Effect effectStyle
    {
        get
        {
            return this.mEffectStyle;
        }
        set
        {
            if (this.mEffectStyle != value)
            {
                this.mEffectStyle = value;
                this.hasChanged = true;
            }
        }
    }

    public UIFont font
    {
        get
        {
            return this.mFont;
        }
        set
        {
            if (this.mFont != value)
            {
                this.mFont = value;
                this.material = ((!(this.mFont != null)) ? null : this.mFont.material);
                this.mChanged = true;
                this.hasChanged = true;
                this.MarkAsChanged();
            }
        }
    }

    public int lineWidth
    {
        get
        {
            return this.mMaxLineWidth;
        }
        set
        {
            if (this.mMaxLineWidth != value)
            {
                this.mMaxLineWidth = value;
                this.hasChanged = true;
                if (this.shrinkToFit)
                {
                    this.MakePixelPerfect();
                }
            }
        }
    }

    public override Material material
    {
        get
        {
            Material material = base.material;
            if (material == null)
            {
                material = ((!(this.mFont != null)) ? null : this.mFont.material);
                this.material = material;
            }
            return material;
        }
    }

    public int maxLineCount
    {
        get
        {
            return this.mMaxLineCount;
        }
        set
        {
            if (this.mMaxLineCount != value)
            {
                this.mMaxLineCount = Mathf.Max(value, 0);
                this.hasChanged = true;
                if (value == 1)
                {
                    this.mPassword = false;
                }
            }
        }
    }

    public bool multiLine
    {
        get
        {
            return this.mMaxLineCount != 1;
        }
        set
        {
            if (this.mMaxLineCount != 1 != value)
            {
                this.mMaxLineCount = ((!value) ? 1 : 0);
                this.hasChanged = true;
                if (value)
                {
                    this.mPassword = false;
                }
            }
        }
    }

    public bool password
    {
        get
        {
            return this.mPassword;
        }
        set
        {
            if (this.mPassword != value)
            {
                if (value)
                {
                    this.mMaxLineCount = 1;
                    this.mEncoding = false;
                }
                this.mPassword = value;
                this.hasChanged = true;
            }
        }
    }

    public string processedText
    {
        get
        {
            if (this.mLastScale != base.cachedTransform.localScale)
            {
                this.mLastScale = base.cachedTransform.localScale;
                this.mShouldBeProcessed = true;
            }
            if (this.hasChanged)
            {
                this.ProcessText();
            }
            return this.mProcessedText;
        }
    }

    public override Vector2 relativeSize
    {
        get
        {
            if (this.mFont == null)
            {
                return Vectors.one;
            }
            if (this.hasChanged)
            {
                this.ProcessText();
            }
            return this.mSize;
        }
    }

    public bool showLastPasswordChar
    {
        get
        {
            return this.mShowLastChar;
        }
        set
        {
            if (this.mShowLastChar != value)
            {
                this.mShowLastChar = value;
                this.hasChanged = true;
            }
        }
    }

    public bool shrinkToFit
    {
        get
        {
            return this.mShrinkToFit;
        }
        set
        {
            if (this.mShrinkToFit != value)
            {
                this.mShrinkToFit = value;
                this.hasChanged = true;
            }
        }
    }

    public bool supportEncoding
    {
        get
        {
            return this.mEncoding;
        }
        set
        {
            if (this.mEncoding != value)
            {
                this.mEncoding = value;
                this.hasChanged = true;
                if (value)
                {
                    this.mPassword = false;
                }
            }
        }
    }

    public UIFont.SymbolStyle symbolStyle
    {
        get
        {
            return this.mSymbols;
        }
        set
        {
            if (this.mSymbols != value)
            {
                this.mSymbols = value;
                this.hasChanged = true;
            }
        }
    }

    public string text
    {
        get
        {
            return this.mText;
        }
        set
        {
            if (string.IsNullOrEmpty(value))
            {
                if (!string.IsNullOrEmpty(this.mText))
                {
                    this.mText = string.Empty;
                }
                this.hasChanged = true;
            }
            else if (this.mText != value)
            {
                this.mText = value;
                this.hasChanged = true;
                if (this.shrinkToFit)
                {
                    this.MakePixelPerfect();
                }
            }
        }
    }

    private void ApplyShadow(BetterList<Vector3> verts, BetterList<Vector2> uvs, BetterList<Color32> cols, int start, int end, float x, float y)
    {
        Color color = this.mEffectColor;
        color.a *= base.alpha * this.mPanel.alpha;
        Color32 color2 = (!this.font.premultipliedAlpha) ? color : NGUITools.ApplyPMA(color);
        for (int i = start; i < end; i++)
        {
            verts.Add(verts.buffer[i]);
            uvs.Add(uvs.buffer[i]);
            cols.Add(cols.buffer[i]);
            Vector3 vector = verts.buffer[i];
            vector.x += x;
            vector.y += y;
            verts.buffer[i] = vector;
            cols.buffer[i] = color2;
        }
    }

    private void ProcessText()
    {
        this.mChanged = true;
        this.hasChanged = false;
        this.mLastText = this.mText;
        float num = Mathf.Abs(base.cachedTransform.localScale.x);
        float num2 = (float)(this.mFont.size * this.mMaxLineCount);
        if (num > 0f)
        {
            do
            {
                if (this.mPassword)
                {
                    this.mProcessedText = string.Empty;
                    if (this.mShowLastChar)
                    {
                        int i = 0;
                        int num3 = this.mText.Length - 1;
                        while (i < num3)
                        {
                            this.mProcessedText += "*";
                            i++;
                        }
                        if (this.mText.Length > 0)
                        {
                            this.mProcessedText += this.mText[this.mText.Length - 1];
                        }
                    }
                    else
                    {
                        int j = 0;
                        int length = this.mText.Length;
                        while (j < length)
                        {
                            this.mProcessedText += "*";
                            j++;
                        }
                    }
                    this.mProcessedText = this.mFont.WrapText(this.mProcessedText, (float)this.mMaxLineWidth / num, this.mMaxLineCount, false, UIFont.SymbolStyle.None);
                }
                else if (this.mMaxLineWidth > 0)
                {
                    this.mProcessedText = this.mFont.WrapText(this.mText, (float)this.mMaxLineWidth / num, (!this.mShrinkToFit) ? this.mMaxLineCount : 0, this.mEncoding, this.mSymbols);
                }
                else if (!this.mShrinkToFit && this.mMaxLineCount > 0)
                {
                    this.mProcessedText = this.mFont.WrapText(this.mText, 100000f, this.mMaxLineCount, this.mEncoding, this.mSymbols);
                }
                else
                {
                    this.mProcessedText = this.mText;
                }
                this.mSize = (string.IsNullOrEmpty(this.mProcessedText) ? Vectors.v2one : this.mFont.CalculatePrintedSize(this.mProcessedText, this.mEncoding, this.mSymbols));
                if (!this.mShrinkToFit)
                {
                    goto IL_2F2;
                }
                if (this.mMaxLineCount <= 0 || this.mSize.y * num <= num2)
                {
                    break;
                }
                num = Mathf.Round(num - 1f);
            }
            while (num > 1f);
            if (this.mMaxLineWidth > 0)
            {
                float num4 = (float)this.mMaxLineWidth / num;
                float a = (this.mSize.x * num <= num4) ? num : (num4 / this.mSize.x * num);
                num = Mathf.Min(a, num);
            }
            num = Mathf.Round(num);
            base.cachedTransform.localScale = new Vector3(num, num, 1f);
            IL_2F2:
            this.mSize.x = Mathf.Max(this.mSize.x, (num <= 0f) ? 1f : ((float)this.lineWidth / num));
        }
        else
        {
            this.mSize.x = 1f;
            num = (float)this.mFont.size;
            base.cachedTransform.localScale = new Vector3(0.01f, 0.01f, 1f);
            this.mProcessedText = string.Empty;
        }
        this.mSize.y = Mathf.Max(this.mSize.y, 1f);
    }

    protected override void OnStart()
    {
        if (this.mLineWidth > 0f)
        {
            this.mMaxLineWidth = Mathf.RoundToInt(this.mLineWidth);
            this.mLineWidth = 0f;
        }
        if (!this.mMultiline)
        {
            this.mMaxLineCount = 1;
            this.mMultiline = true;
        }
        this.mPremultiply = (this.font != null && this.font.material != null && this.font.material.shader.name.Contains("Premultiplied"));
    }

    public override void MakePixelPerfect()
    {
        if (this.mFont != null)
        {
            float pixelSize = this.font.pixelSize;
            Vector3 localScale = base.cachedTransform.localScale;
            localScale.x = (float)this.mFont.size * pixelSize;
            localScale.y = localScale.x;
            localScale.z = 1f;
            Vector3 localPosition = base.cachedTransform.localPosition;
            localPosition.x = (float)(Mathf.CeilToInt(localPosition.x / pixelSize * 4f) >> 2);
            localPosition.y = (float)(Mathf.CeilToInt(localPosition.y / pixelSize * 4f) >> 2);
            localPosition.z = (float)Mathf.RoundToInt(localPosition.z);
            localPosition.x *= pixelSize;
            localPosition.y *= pixelSize;
            base.cachedTransform.localPosition = localPosition;
            base.cachedTransform.localScale = localScale;
            if (this.shrinkToFit)
            {
                this.ProcessText();
            }
        }
        else
        {
            base.MakePixelPerfect();
        }
    }

    public override void MarkAsChanged()
    {
        this.hasChanged = true;
        base.MarkAsChanged();
    }

    public override void OnFill(BetterList<Vector3> verts, BetterList<Vector2> uvs, BetterList<Color32> cols)
    {
        if (this.mFont == null)
        {
            return;
        }
        UIWidget.Pivot pivot = base.pivot;
        int start = verts.size;
        Color c = base.color;
        c.a *= this.mPanel.alpha;
        if (this.font.premultipliedAlpha)
        {
            c = NGUITools.ApplyPMA(c);
        }
        if (pivot == UIWidget.Pivot.Left || pivot == UIWidget.Pivot.TopLeft || pivot == UIWidget.Pivot.BottomLeft)
        {
            this.mFont.Print(this.processedText, c, verts, uvs, cols, this.mEncoding, this.mSymbols, UIFont.Alignment.Left, 0, this.mPremultiply);
        }
        else if (pivot == UIWidget.Pivot.Right || pivot == UIWidget.Pivot.TopRight || pivot == UIWidget.Pivot.BottomRight)
        {
            this.mFont.Print(this.processedText, c, verts, uvs, cols, this.mEncoding, this.mSymbols, UIFont.Alignment.Right, Mathf.RoundToInt(this.relativeSize.x * (float)this.mFont.size), this.mPremultiply);
        }
        else
        {
            this.mFont.Print(this.processedText, c, verts, uvs, cols, this.mEncoding, this.mSymbols, UIFont.Alignment.Center, Mathf.RoundToInt(this.relativeSize.x * (float)this.mFont.size), this.mPremultiply);
        }
        if (this.effectStyle != UILabel.Effect.None)
        {
            int size = verts.size;
            float num = 1f / (float)this.mFont.size;
            float num2 = num * this.mEffectDistance.x;
            float num3 = num * this.mEffectDistance.y;
            this.ApplyShadow(verts, uvs, cols, start, size, num2, -num3);
            if (this.effectStyle == UILabel.Effect.Outline)
            {
                start = size;
                size = verts.size;
                this.ApplyShadow(verts, uvs, cols, start, size, -num2, num3);
                start = size;
                size = verts.size;
                this.ApplyShadow(verts, uvs, cols, start, size, num2, num3);
                start = size;
                size = verts.size;
                this.ApplyShadow(verts, uvs, cols, start, size, -num2, -num3);
            }
        }
    }
}