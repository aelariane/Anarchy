using Optimization.Caching;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("NGUI/UI/Font")]
public class UIFont : MonoBehaviour
{
    private static CharacterInfo mChar;

    [HideInInspector]
    [SerializeField]
    private UIAtlas mAtlas;

    private List<Color> mColors = new List<Color>();

    [HideInInspector]
    [SerializeField]
    private Font mDynamicFont;

    [SerializeField]
    [HideInInspector]
    private float mDynamicFontOffset;

    [SerializeField]
    [HideInInspector]
    private int mDynamicFontSize = 16;

    [HideInInspector]
    [SerializeField]
    private FontStyle mDynamicFontStyle;

    [HideInInspector]
    [SerializeField]
    private BMFont mFont = new BMFont();

    [HideInInspector]
    [SerializeField]
    private Material mMat;

    [HideInInspector]
    [SerializeField]
    private float mPixelSize = 1f;

    private int mPMA = -1;

    [HideInInspector]
    [SerializeField]
    private UIFont mReplacement;

    [HideInInspector]
    [SerializeField]
    private int mSpacingX;

    [SerializeField]
    [HideInInspector]
    private int mSpacingY;

    private UIAtlas.Sprite mSprite;

    private bool mSpriteSet;

    [HideInInspector]
    [SerializeField]
    private List<BMSymbol> mSymbols = new List<BMSymbol>();

    [SerializeField]
    [HideInInspector]
    private Rect mUVRect = new Rect(0f, 0f, 1f, 1f);

    public enum Alignment
    {
        Left,
        Center,
        Right
    }

    public enum SymbolStyle
    {
        None,
        Uncolored,
        Colored
    }

    private Texture dynamicTexture
    {
        get
        {
            if (this.mReplacement)
            {
                return this.mReplacement.dynamicTexture;
            }
            if (this.isDynamic)
            {
                return this.mDynamicFont.material.mainTexture;
            }
            return null;
        }
    }

    public UIAtlas atlas
    {
        get
        {
            return (!(this.mReplacement != null)) ? this.mAtlas : this.mReplacement.atlas;
        }
        set
        {
            if (this.mReplacement != null)
            {
                this.mReplacement.atlas = value;
            }
            else if (this.mAtlas != value)
            {
                if (value == null)
                {
                    if (this.mAtlas != null)
                    {
                        this.mMat = this.mAtlas.spriteMaterial;
                    }
                    if (this.sprite != null)
                    {
                        this.mUVRect = this.uvRect;
                    }
                }
                this.mPMA = -1;
                this.mAtlas = value;
                this.MarkAsDirty();
            }
        }
    }

    public BMFont bmFont
    {
        get
        {
            return (!(this.mReplacement != null)) ? this.mFont : this.mReplacement.bmFont;
        }
    }

    public Font dynamicFont
    {
        get
        {
            return (!(this.mReplacement != null)) ? this.mDynamicFont : this.mReplacement.dynamicFont;
        }
        set
        {
            if (this.mReplacement != null)
            {
                this.mReplacement.dynamicFont = value;
            }
            else if (this.mDynamicFont != value)
            {
                if (this.mDynamicFont != null)
                {
                    this.material = null;
                }
                this.mDynamicFont = value;
                this.MarkAsDirty();
            }
        }
    }

    public int dynamicFontSize
    {
        get
        {
            return (!(this.mReplacement != null)) ? this.mDynamicFontSize : this.mReplacement.dynamicFontSize;
        }
        set
        {
            if (this.mReplacement != null)
            {
                this.mReplacement.dynamicFontSize = value;
            }
            else
            {
                value = Mathf.Clamp(value, 4, 128);
                if (this.mDynamicFontSize != value)
                {
                    this.mDynamicFontSize = value;
                    this.MarkAsDirty();
                }
            }
        }
    }

    public FontStyle dynamicFontStyle
    {
        get
        {
            return (!(this.mReplacement != null)) ? this.mDynamicFontStyle : this.mReplacement.dynamicFontStyle;
        }
        set
        {
            if (this.mReplacement != null)
            {
                this.mReplacement.dynamicFontStyle = value;
            }
            else if (this.mDynamicFontStyle != value)
            {
                this.mDynamicFontStyle = value;
                this.MarkAsDirty();
            }
        }
    }

    public bool hasSymbols
    {
        get
        {
            return (!(this.mReplacement != null)) ? (this.mSymbols.Count != 0) : this.mReplacement.hasSymbols;
        }
    }

    public int horizontalSpacing
    {
        get
        {
            return (!(this.mReplacement != null)) ? this.mSpacingX : this.mReplacement.horizontalSpacing;
        }
        set
        {
            if (this.mReplacement != null)
            {
                this.mReplacement.horizontalSpacing = value;
            }
            else if (this.mSpacingX != value)
            {
                this.mSpacingX = value;
                this.MarkAsDirty();
            }
        }
    }

    public bool isDynamic
    {
        get
        {
            return this.mDynamicFont != null;
        }
    }

    public bool isValid
    {
        get
        {
            return this.mDynamicFont != null || this.mFont.isValid;
        }
    }

    public Material material
    {
        get
        {
            if (this.mReplacement != null)
            {
                return this.mReplacement.material;
            }
            if (this.mAtlas != null)
            {
                return this.mAtlas.spriteMaterial;
            }
            if (this.mMat != null)
            {
                if (this.mDynamicFont != null && this.mMat != this.mDynamicFont.material)
                {
                    this.mMat.mainTexture = this.mDynamicFont.material.mainTexture;
                }
                return this.mMat;
            }
            if (this.mDynamicFont != null)
            {
                return this.mDynamicFont.material;
            }
            return null;
        }
        set
        {
            if (this.mReplacement != null)
            {
                this.mReplacement.material = value;
            }
            else if (this.mMat != value)
            {
                this.mPMA = -1;
                this.mMat = value;
                this.MarkAsDirty();
            }
        }
    }

    public float pixelSize
    {
        get
        {
            if (this.mReplacement != null)
            {
                return this.mReplacement.pixelSize;
            }
            if (this.mAtlas != null)
            {
                return this.mAtlas.pixelSize;
            }
            return this.mPixelSize;
        }
        set
        {
            if (this.mReplacement != null)
            {
                this.mReplacement.pixelSize = value;
            }
            else if (this.mAtlas != null)
            {
                this.mAtlas.pixelSize = value;
            }
            else
            {
                float num = Mathf.Clamp(value, 0.25f, 4f);
                if (this.mPixelSize != num)
                {
                    this.mPixelSize = num;
                    this.MarkAsDirty();
                }
            }
        }
    }

    public bool premultipliedAlpha
    {
        get
        {
            if (this.mReplacement != null)
            {
                return this.mReplacement.premultipliedAlpha;
            }
            if (this.mAtlas != null)
            {
                return this.mAtlas.premultipliedAlpha;
            }
            if (this.mPMA == -1)
            {
                Material material = this.material;
                this.mPMA = ((!(material != null) || !(material.shader != null) || !material.shader.name.Contains("Premultiplied")) ? 0 : 1);
            }
            return this.mPMA == 1;
        }
    }

    public UIFont replacement
    {
        get
        {
            return this.mReplacement;
        }
        set
        {
            UIFont uifont = value;
            if (uifont == this)
            {
                uifont = null;
            }
            if (this.mReplacement != uifont)
            {
                if (uifont != null && uifont.replacement == this)
                {
                    uifont.replacement = null;
                }
                if (this.mReplacement != null)
                {
                    this.MarkAsDirty();
                }
                this.mReplacement = uifont;
                this.MarkAsDirty();
            }
        }
    }

    public int size
    {
        get
        {
            return (!(this.mReplacement != null)) ? ((!this.isDynamic) ? this.mFont.charSize : this.mDynamicFontSize) : this.mReplacement.size;
        }
    }

    public UIAtlas.Sprite sprite
    {
        get
        {
            if (this.mReplacement != null)
            {
                return this.mReplacement.sprite;
            }
            if (!this.mSpriteSet)
            {
                this.mSprite = null;
            }
            if (this.mSprite == null)
            {
                if (this.mAtlas != null && !string.IsNullOrEmpty(this.mFont.spriteName))
                {
                    this.mSprite = this.mAtlas.GetSprite(this.mFont.spriteName);
                    if (this.mSprite == null)
                    {
                        this.mSprite = this.mAtlas.GetSprite(base.name);
                    }
                    this.mSpriteSet = true;
                    if (this.mSprite == null)
                    {
                        this.mFont.spriteName = null;
                    }
                }
                int i = 0;
                int count = this.mSymbols.Count;
                while (i < count)
                {
                    this.symbols[i].MarkAsDirty();
                    i++;
                }
            }
            return this.mSprite;
        }
    }

    public string spriteName
    {
        get
        {
            return (!(this.mReplacement != null)) ? this.mFont.spriteName : this.mReplacement.spriteName;
        }
        set
        {
            if (this.mReplacement != null)
            {
                this.mReplacement.spriteName = value;
            }
            else if (this.mFont.spriteName != value)
            {
                this.mFont.spriteName = value;
                this.MarkAsDirty();
            }
        }
    }

    public List<BMSymbol> symbols
    {
        get
        {
            return (!(this.mReplacement != null)) ? this.mSymbols : this.mReplacement.symbols;
        }
    }

    public int texHeight
    {
        get
        {
            return (!(this.mReplacement != null)) ? ((this.mFont == null) ? 1 : this.mFont.texHeight) : this.mReplacement.texHeight;
        }
    }

    public Texture2D texture
    {
        get
        {
            if (this.mReplacement != null)
            {
                return this.mReplacement.texture;
            }
            Material material = this.material;
            return (!(material != null)) ? null : (material.mainTexture as Texture2D);
        }
    }

    public int texWidth
    {
        get
        {
            return (!(this.mReplacement != null)) ? ((this.mFont == null) ? 1 : this.mFont.texWidth) : this.mReplacement.texWidth;
        }
    }

    public Rect uvRect
    {
        get
        {
            if (this.mReplacement != null)
            {
                return this.mReplacement.uvRect;
            }
            if (this.mAtlas != null && this.mSprite == null && this.sprite != null)
            {
                Texture texture = this.mAtlas.texture;
                if (texture != null)
                {
                    this.mUVRect = this.mSprite.outer;
                    if (this.mAtlas.coordinates == UIAtlas.Coordinates.Pixels)
                    {
                        this.mUVRect = NGUIMath.ConvertToTexCoords(this.mUVRect, texture.width, texture.height);
                    }
                    if (this.mSprite.hasPadding)
                    {
                        Rect rect = this.mUVRect;
                        this.mUVRect.xMin = rect.xMin - this.mSprite.paddingLeft * rect.width;
                        this.mUVRect.yMin = rect.yMin - this.mSprite.paddingBottom * rect.height;
                        this.mUVRect.xMax = rect.xMax + this.mSprite.paddingRight * rect.width;
                        this.mUVRect.yMax = rect.yMax + this.mSprite.paddingTop * rect.height;
                    }
                    if (this.mSprite.hasPadding)
                    {
                        this.Trim();
                    }
                }
            }
            return this.mUVRect;
        }
        set
        {
            if (this.mReplacement != null)
            {
                this.mReplacement.uvRect = value;
            }
            else if (this.sprite == null && this.mUVRect != value)
            {
                this.mUVRect = value;
                this.MarkAsDirty();
            }
        }
    }

    public int verticalSpacing
    {
        get
        {
            return (!(this.mReplacement != null)) ? this.mSpacingY : this.mReplacement.verticalSpacing;
        }
        set
        {
            if (this.mReplacement != null)
            {
                this.mReplacement.verticalSpacing = value;
            }
            else if (this.mSpacingY != value)
            {
                this.mSpacingY = value;
                this.MarkAsDirty();
            }
        }
    }

    private static void EndLine(ref StringBuilder s)
    {
        int num = s.Length - 1;
        if (num > 0 && s[num] == ' ')
        {
            s[num] = '\n';
        }
        else
        {
            s.Append('\n');
        }
    }

    private void Align(BetterList<Vector3> verts, int indexOffset, UIFont.Alignment alignment, int x, int lineWidth)
    {
        if (alignment != UIFont.Alignment.Left)
        {
            int size = this.size;
            if (size > 0)
            {
                float num;
                if (alignment == UIFont.Alignment.Right)
                {
                    num = (float)Mathf.RoundToInt((float)(lineWidth - x));
                    if (num < 0f)
                    {
                        num = 0f;
                    }
                    num /= (float)this.size;
                }
                else
                {
                    num = (float)Mathf.RoundToInt((float)(lineWidth - x) * 0.5f);
                    if (num < 0f)
                    {
                        num = 0f;
                    }
                    num /= (float)this.size;
                    if ((lineWidth & 1) == 1)
                    {
                        num += 0.5f / (float)size;
                    }
                }
                for (int i = indexOffset; i < verts.size; i++)
                {
                    Vector3 vector = verts.buffer[i];
                    vector.x += num;
                    verts.buffer[i] = vector;
                }
            }
        }
    }

    private BMSymbol GetSymbol(string sequence, bool createIfMissing)
    {
        int i = 0;
        int count = this.mSymbols.Count;
        while (i < count)
        {
            BMSymbol bmsymbol = this.mSymbols[i];
            if (bmsymbol.sequence == sequence)
            {
                return bmsymbol;
            }
            i++;
        }
        if (createIfMissing)
        {
            BMSymbol bmsymbol2 = new BMSymbol();
            bmsymbol2.sequence = sequence;
            this.mSymbols.Add(bmsymbol2);
            return bmsymbol2;
        }
        return null;
    }

    private BMSymbol MatchSymbol(string text, int offset, int textLength)
    {
        int count = this.mSymbols.Count;
        if (count == 0)
        {
            return null;
        }
        textLength -= offset;
        for (int i = 0; i < count; i++)
        {
            BMSymbol bmsymbol = this.mSymbols[i];
            int length = bmsymbol.length;
            if (length != 0 && textLength >= length)
            {
                bool flag = true;
                for (int j = 0; j < length; j++)
                {
                    if (text[offset + j] != bmsymbol.sequence[j])
                    {
                        flag = false;
                        break;
                    }
                }
                if (flag && bmsymbol.Validate(this.atlas))
                {
                    return bmsymbol;
                }
            }
        }
        return null;
    }

    private void OnFontChanged()
    {
        this.MarkAsDirty();
    }

    private bool References(UIFont font)
    {
        return !(font == null) && (font == this || (this.mReplacement != null && this.mReplacement.References(font)));
    }

    private void Trim()
    {
        Texture texture = this.mAtlas.texture;
        if (texture != null && this.mSprite != null)
        {
            Rect rect = NGUIMath.ConvertToPixels(this.mUVRect, this.texture.width, this.texture.height, true);
            Rect rect2 = (this.mAtlas.coordinates != UIAtlas.Coordinates.TexCoords) ? this.mSprite.outer : NGUIMath.ConvertToPixels(this.mSprite.outer, texture.width, texture.height, true);
            int xMin = Mathf.RoundToInt(rect2.xMin - rect.xMin);
            int yMin = Mathf.RoundToInt(rect2.yMin - rect.yMin);
            int xMax = Mathf.RoundToInt(rect2.xMax - rect.xMin);
            int yMax = Mathf.RoundToInt(rect2.yMax - rect.yMin);
            this.mFont.Trim(xMin, yMin, xMax, yMax);
        }
    }

    public static bool CheckIfRelated(UIFont a, UIFont b)
    {
        return !(a == null) && !(b == null) && ((a.isDynamic && b.isDynamic && a.dynamicFont.fontNames[0] == b.dynamicFont.fontNames[0]) || a == b || a.References(b) || b.References(a));
    }

    public void AddSymbol(string sequence, string spriteName)
    {
        BMSymbol symbol = this.GetSymbol(sequence, true);
        symbol.spriteName = spriteName;
        this.MarkAsDirty();
    }

    public Vector2 CalculatePrintedSize(string text, bool encoding, UIFont.SymbolStyle symbolStyle)
    {
        if (this.mReplacement != null)
        {
            return this.mReplacement.CalculatePrintedSize(text, encoding, symbolStyle);
        }
        Vector2 zero = Vectors.v2zero;
        bool isDynamic = this.isDynamic;
        if (isDynamic || (this.mFont != null && this.mFont.isValid && !string.IsNullOrEmpty(text)))
        {
            if (encoding)
            {
                text = NGUITools.StripSymbols(text);
            }
            if (isDynamic)
            {
                this.mDynamicFont.textureRebuildCallback = new Font.FontTextureRebuildCallback(this.OnFontChanged);
                this.mDynamicFont.RequestCharactersInTexture(text, this.mDynamicFontSize, this.mDynamicFontStyle);
                this.mDynamicFont.textureRebuildCallback = null;
            }
            int length = text.Length;
            int num = 0;
            int num2 = 0;
            int num3 = 0;
            int num4 = 0;
            int size = this.size;
            int num5 = size + this.mSpacingY;
            bool flag = encoding && symbolStyle != UIFont.SymbolStyle.None && this.hasSymbols;
            for (int i = 0; i < length; i++)
            {
                char c = text[i];
                if (c == '\n')
                {
                    if (num2 > num)
                    {
                        num = num2;
                    }
                    num2 = 0;
                    num3 += num5;
                    num4 = 0;
                }
                else if (c < ' ')
                {
                    num4 = 0;
                }
                else if (!isDynamic)
                {
                    BMSymbol bmsymbol = (!flag) ? null : this.MatchSymbol(text, i, length);
                    if (bmsymbol == null)
                    {
                        BMGlyph glyph = this.mFont.GetGlyph((int)c);
                        if (glyph != null)
                        {
                            num2 += this.mSpacingX + ((num4 == 0) ? glyph.advance : (glyph.advance + glyph.GetKerning(num4)));
                            num4 = (int)c;
                        }
                    }
                    else
                    {
                        num2 += this.mSpacingX + bmsymbol.width;
                        i += bmsymbol.length - 1;
                        num4 = 0;
                    }
                }
                else if (this.mDynamicFont.GetCharacterInfo(c, out UIFont.mChar, this.mDynamicFontSize, this.mDynamicFontStyle))
                {
                    num2 += (int)((float)this.mSpacingX + UIFont.mChar.width);
                }
            }
            float num6 = (size <= 0) ? 1f : (1f / (float)size);
            zero.x = num6 * (float)((num2 <= num) ? num : num2);
            zero.y = num6 * (float)(num3 + num5);
        }
        return zero;
    }

    public string GetEndOfLineThatFits(string text, float maxWidth, bool encoding, UIFont.SymbolStyle symbolStyle)
    {
        if (this.mReplacement != null)
        {
            return this.mReplacement.GetEndOfLineThatFits(text, maxWidth, encoding, symbolStyle);
        }
        int num = Mathf.RoundToInt(maxWidth * (float)this.size);
        if (num < 1)
        {
            return text;
        }
        int length = text.Length;
        int num2 = num;
        BMGlyph bmglyph = null;
        int num3 = length;
        bool flag = encoding && symbolStyle != UIFont.SymbolStyle.None && this.hasSymbols;
        bool isDynamic = this.isDynamic;
        if (isDynamic)
        {
            this.mDynamicFont.textureRebuildCallback = new Font.FontTextureRebuildCallback(this.OnFontChanged);
            this.mDynamicFont.RequestCharactersInTexture(text, this.mDynamicFontSize, this.mDynamicFontStyle);
            this.mDynamicFont.textureRebuildCallback = null;
        }
        while (num3 > 0 && num2 > 0)
        {
            char c = text[--num3];
            BMSymbol bmsymbol = (!flag) ? null : this.MatchSymbol(text, num3, length);
            int num4 = this.mSpacingX;
            if (!isDynamic)
            {
                if (bmsymbol != null)
                {
                    num4 += bmsymbol.advance;
                }
                else
                {
                    BMGlyph glyph = this.mFont.GetGlyph((int)c);
                    if (glyph == null)
                    {
                        bmglyph = null;
                        continue;
                    }
                    num4 += glyph.advance + ((bmglyph != null) ? bmglyph.GetKerning((int)c) : 0);
                    bmglyph = glyph;
                }
            }
            else if (this.mDynamicFont.GetCharacterInfo(c, out UIFont.mChar, this.mDynamicFontSize, this.mDynamicFontStyle))
            {
                num4 += (int)UIFont.mChar.width;
            }
            num2 -= num4;
        }
        if (num2 < 0)
        {
            num3++;
        }
        return text.Substring(num3, length - num3);
    }

    public void MarkAsDirty()
    {
        if (this.mReplacement != null)
        {
            this.mReplacement.MarkAsDirty();
        }
        this.RecalculateDynamicOffset();
        this.mSprite = null;
        UILabel[] array = NGUITools.FindActive<UILabel>();
        int i = 0;
        int num = array.Length;
        while (i < num)
        {
            UILabel uilabel = array[i];
            if (uilabel.enabled && NGUITools.GetActive(uilabel.gameObject) && UIFont.CheckIfRelated(this, uilabel.font))
            {
                UIFont font = uilabel.font;
                uilabel.font = null;
                uilabel.font = font;
            }
            i++;
        }
        int j = 0;
        int count = this.mSymbols.Count;
        while (j < count)
        {
            this.symbols[j].MarkAsDirty();
            j++;
        }
    }

    public void Print(string text, Color32 color, BetterList<Vector3> verts, BetterList<Vector2> uvs, BetterList<Color32> cols, bool encoding, UIFont.SymbolStyle symbolStyle, UIFont.Alignment alignment, int lineWidth, bool premultiply)
    {
        if (this.mReplacement != null)
        {
            this.mReplacement.Print(text, color, verts, uvs, cols, encoding, symbolStyle, alignment, lineWidth, premultiply);
        }
        else if (text != null)
        {
            if (!this.isValid)
            {
                Debug.LogError("Attempting to print using an invalid font!");
                return;
            }
            bool isDynamic = this.isDynamic;
            if (isDynamic)
            {
                this.mDynamicFont.textureRebuildCallback = new Font.FontTextureRebuildCallback(this.OnFontChanged);
                this.mDynamicFont.RequestCharactersInTexture(text, this.mDynamicFontSize, this.mDynamicFontStyle);
                this.mDynamicFont.textureRebuildCallback = null;
            }
            this.mColors.Clear();
            this.mColors.Add(color);
            int size = this.size;
            Vector2 vector = (size <= 0) ? Vectors.v2one : new Vector2(1f / (float)size, 1f / (float)size);
            int size2 = verts.size;
            int num = 0;
            int num2 = 0;
            int num3 = 0;
            int num4 = 0;
            int num5 = size + this.mSpacingY;
            Vector3 zero = Vectors.zero;
            Vector3 zero2 = Vectors.zero;
            Vector2 zero3 = Vectors.v2zero;
            Vector2 zero4 = Vectors.v2zero;
            float num6 = this.uvRect.width / (float)this.mFont.texWidth;
            float num7 = this.mUVRect.height / (float)this.mFont.texHeight;
            int length = text.Length;
            bool flag = encoding && symbolStyle != UIFont.SymbolStyle.None && this.hasSymbols && this.sprite != null;
            for (int i = 0; i < length; i++)
            {
                char c = text[i];
                if (c == '\n')
                {
                    if (num2 > num)
                    {
                        num = num2;
                    }
                    if (alignment != UIFont.Alignment.Left)
                    {
                        this.Align(verts, size2, alignment, num2, lineWidth);
                        size2 = verts.size;
                    }
                    num2 = 0;
                    num3 += num5;
                    num4 = 0;
                }
                else if (c < ' ')
                {
                    num4 = 0;
                }
                else
                {
                    if (encoding && c == '[')
                    {
                        int num8 = NGUITools.ParseSymbol(text, i, this.mColors, premultiply);
                        if (num8 > 0)
                        {
                            color = this.mColors[this.mColors.Count - 1];
                            i += num8 - 1;
                            goto IL_96C;
                        }
                    }
                    if (!isDynamic)
                    {
                        BMSymbol bmsymbol = (!flag) ? null : this.MatchSymbol(text, i, length);
                        if (bmsymbol == null)
                        {
                            BMGlyph glyph = this.mFont.GetGlyph((int)c);
                            if (glyph == null)
                            {
                                goto IL_96C;
                            }
                            if (num4 != 0)
                            {
                                num2 += glyph.GetKerning(num4);
                            }
                            if (c == ' ')
                            {
                                num2 += this.mSpacingX + glyph.advance;
                                num4 = (int)c;
                                goto IL_96C;
                            }
                            zero.x = vector.x * (float)(num2 + glyph.offsetX);
                            zero.y = -vector.y * (float)(num3 + glyph.offsetY);
                            zero2.x = zero.x + vector.x * (float)glyph.width;
                            zero2.y = zero.y - vector.y * (float)glyph.height;
                            zero3.x = this.mUVRect.xMin + num6 * (float)glyph.x;
                            zero3.y = this.mUVRect.yMax - num7 * (float)glyph.y;
                            zero4.x = zero3.x + num6 * (float)glyph.width;
                            zero4.y = zero3.y - num7 * (float)glyph.height;
                            num2 += this.mSpacingX + glyph.advance;
                            num4 = (int)c;
                            if (glyph.channel == 0 || glyph.channel == 15)
                            {
                                for (int j = 0; j < 4; j++)
                                {
                                    cols.Add(color);
                                }
                            }
                            else
                            {
                                Color color2 = color;
                                color2 *= 0.49f;
                                switch (glyph.channel)
                                {
                                    case 1:
                                        color2.b += 0.51f;
                                        break;

                                    case 2:
                                        color2.g += 0.51f;
                                        break;

                                    case 4:
                                        color2.r += 0.51f;
                                        break;

                                    case 8:
                                        color2.a += 0.51f;
                                        break;
                                }
                                for (int k = 0; k < 4; k++)
                                {
                                    cols.Add(color2);
                                }
                            }
                        }
                        else
                        {
                            zero.x = vector.x * (float)(num2 + bmsymbol.offsetX);
                            zero.y = -vector.y * (float)(num3 + bmsymbol.offsetY);
                            zero2.x = zero.x + vector.x * (float)bmsymbol.width;
                            zero2.y = zero.y - vector.y * (float)bmsymbol.height;
                            Rect uvRect = bmsymbol.uvRect;
                            zero3.x = uvRect.xMin;
                            zero3.y = uvRect.yMax;
                            zero4.x = uvRect.xMax;
                            zero4.y = uvRect.yMin;
                            num2 += this.mSpacingX + bmsymbol.advance;
                            i += bmsymbol.length - 1;
                            num4 = 0;
                            if (symbolStyle == UIFont.SymbolStyle.Colored)
                            {
                                for (int l = 0; l < 4; l++)
                                {
                                    cols.Add(color);
                                }
                            }
                            else
                            {
                                Color32 item = Color.white;
                                item.a = color.a;
                                for (int m = 0; m < 4; m++)
                                {
                                    cols.Add(item);
                                }
                            }
                        }
                        verts.Add(new Vector3(zero2.x, zero.y));
                        verts.Add(new Vector3(zero2.x, zero2.y));
                        verts.Add(new Vector3(zero.x, zero2.y));
                        verts.Add(new Vector3(zero.x, zero.y));
                        uvs.Add(new Vector2(zero4.x, zero3.y));
                        uvs.Add(new Vector2(zero4.x, zero4.y));
                        uvs.Add(new Vector2(zero3.x, zero4.y));
                        uvs.Add(new Vector2(zero3.x, zero3.y));
                    }
                    else if (this.mDynamicFont.GetCharacterInfo(c, out UIFont.mChar, this.mDynamicFontSize, this.mDynamicFontStyle))
                    {
                        zero.x = vector.x * ((float)num2 + UIFont.mChar.vert.xMin);
                        zero.y = -vector.y * ((float)num3 - UIFont.mChar.vert.yMax + this.mDynamicFontOffset);
                        zero2.x = zero.x + vector.x * UIFont.mChar.vert.width;
                        zero2.y = zero.y - vector.y * UIFont.mChar.vert.height;
                        zero3.x = UIFont.mChar.uv.xMin;
                        zero3.y = UIFont.mChar.uv.yMin;
                        zero4.x = UIFont.mChar.uv.xMax;
                        zero4.y = UIFont.mChar.uv.yMax;
                        num2 += this.mSpacingX + (int)UIFont.mChar.width;
                        for (int n = 0; n < 4; n++)
                        {
                            cols.Add(color);
                        }
                        if (UIFont.mChar.flipped)
                        {
                            uvs.Add(new Vector2(zero3.x, zero4.y));
                            uvs.Add(new Vector2(zero3.x, zero3.y));
                            uvs.Add(new Vector2(zero4.x, zero3.y));
                            uvs.Add(new Vector2(zero4.x, zero4.y));
                        }
                        else
                        {
                            uvs.Add(new Vector2(zero4.x, zero3.y));
                            uvs.Add(new Vector2(zero3.x, zero3.y));
                            uvs.Add(new Vector2(zero3.x, zero4.y));
                            uvs.Add(new Vector2(zero4.x, zero4.y));
                        }
                        verts.Add(new Vector3(zero2.x, zero.y));
                        verts.Add(new Vector3(zero.x, zero.y));
                        verts.Add(new Vector3(zero.x, zero2.y));
                        verts.Add(new Vector3(zero2.x, zero2.y));
                    }
                }
                IL_96C:;
            }
            if (alignment != UIFont.Alignment.Left && size2 < verts.size)
            {
                this.Align(verts, size2, alignment, num2, lineWidth);
                size2 = verts.size;
            }
        }
    }

    public bool RecalculateDynamicOffset()
    {
        if (this.mDynamicFont != null)
        {
            this.mDynamicFont.RequestCharactersInTexture("j", this.mDynamicFontSize, this.mDynamicFontStyle);
            CharacterInfo characterInfo;
            this.mDynamicFont.GetCharacterInfo('j', out characterInfo, this.mDynamicFontSize, this.mDynamicFontStyle);
            float num = (float)this.mDynamicFontSize + characterInfo.vert.yMax;
            if (!object.Equals(this.mDynamicFontOffset, num))
            {
                this.mDynamicFontOffset = num;
                return true;
            }
        }
        return false;
    }

    public void RemoveSymbol(string sequence)
    {
        BMSymbol symbol = this.GetSymbol(sequence, false);
        if (symbol != null)
        {
            this.symbols.Remove(symbol);
        }
        this.MarkAsDirty();
    }

    public void RenameSymbol(string before, string after)
    {
        BMSymbol symbol = this.GetSymbol(before, false);
        if (symbol != null)
        {
            symbol.sequence = after;
        }
        this.MarkAsDirty();
    }

    public bool UsesSprite(string s)
    {
        if (!string.IsNullOrEmpty(s))
        {
            if (s.Equals(this.spriteName))
            {
                return true;
            }
            int i = 0;
            int count = this.symbols.Count;
            while (i < count)
            {
                BMSymbol bmsymbol = this.symbols[i];
                if (s.Equals(bmsymbol.spriteName))
                {
                    return true;
                }
                i++;
            }
        }
        return false;
    }

    public string WrapText(string text, float maxWidth, int maxLineCount, bool encoding, UIFont.SymbolStyle symbolStyle)
    {
        if (this.mReplacement != null)
        {
            return this.mReplacement.WrapText(text, maxWidth, maxLineCount, encoding, symbolStyle);
        }
        int num = Mathf.RoundToInt(maxWidth * (float)this.size);
        if (num < 1)
        {
            return text;
        }
        StringBuilder stringBuilder = new StringBuilder();
        int length = text.Length;
        int num2 = num;
        int num3 = 0;
        int num4 = 0;
        int i = 0;
        bool flag = true;
        bool flag2 = maxLineCount != 1;
        int num5 = 1;
        bool flag3 = encoding && symbolStyle != UIFont.SymbolStyle.None && this.hasSymbols;
        bool isDynamic = this.isDynamic;
        if (isDynamic)
        {
            this.mDynamicFont.textureRebuildCallback = new Font.FontTextureRebuildCallback(this.OnFontChanged);
            this.mDynamicFont.RequestCharactersInTexture(text, this.mDynamicFontSize, this.mDynamicFontStyle);
            this.mDynamicFont.textureRebuildCallback = null;
        }
        while (i < length)
        {
            char c = text[i];
            if (c == '\n')
            {
                if (!flag2 || num5 == maxLineCount)
                {
                    break;
                }
                num2 = num;
                if (num4 < i)
                {
                    stringBuilder.Append(text.Substring(num4, i - num4 + 1));
                }
                else
                {
                    stringBuilder.Append(c);
                }
                flag = true;
                num5++;
                num4 = i + 1;
                num3 = 0;
            }
            else
            {
                if (c == ' ' && num3 != 32 && num4 < i)
                {
                    stringBuilder.Append(text.Substring(num4, i - num4 + 1));
                    flag = false;
                    num4 = i + 1;
                    num3 = (int)c;
                }
                if (encoding && c == '[' && i + 2 < length)
                {
                    if (text[i + 1] == '-' && text[i + 2] == ']')
                    {
                        i += 2;
                        goto IL_3E7;
                    }
                    if (i + 7 < length && text[i + 7] == ']' && NGUITools.EncodeColor(NGUITools.ParseColor(text, i + 1)) == text.Substring(i + 1, 6).ToUpper())
                    {
                        i += 7;
                        goto IL_3E7;
                    }
                }
                BMSymbol bmsymbol = (!flag3) ? null : this.MatchSymbol(text, i, length);
                int num6 = this.mSpacingX;
                if (!isDynamic)
                {
                    if (bmsymbol != null)
                    {
                        num6 += bmsymbol.advance;
                    }
                    else
                    {
                        BMGlyph bmglyph = (bmsymbol != null) ? null : this.mFont.GetGlyph((int)c);
                        if (bmglyph == null)
                        {
                            goto IL_3E7;
                        }
                        num6 += ((num3 == 0) ? bmglyph.advance : (bmglyph.advance + bmglyph.GetKerning(num3)));
                    }
                }
                else if (this.mDynamicFont.GetCharacterInfo(c, out UIFont.mChar, this.mDynamicFontSize, this.mDynamicFontStyle))
                {
                    num6 += Mathf.RoundToInt(UIFont.mChar.width);
                }
                num2 -= num6;
                if (num2 < 0)
                {
                    if (flag || !flag2 || num5 == maxLineCount)
                    {
                        stringBuilder.Append(text.Substring(num4, Mathf.Max(0, i - num4)));
                        if (!flag2 || num5 == maxLineCount)
                        {
                            num4 = i;
                            break;
                        }
                        UIFont.EndLine(ref stringBuilder);
                        flag = true;
                        num5++;
                        if (c == ' ')
                        {
                            num4 = i + 1;
                            num2 = num;
                        }
                        else
                        {
                            num4 = i;
                            num2 = num - num6;
                        }
                        num3 = 0;
                    }
                    else
                    {
                        while (num4 < length && text[num4] == ' ')
                        {
                            num4++;
                        }
                        flag = true;
                        num2 = num;
                        i = num4 - 1;
                        num3 = 0;
                        if (!flag2 || num5 == maxLineCount)
                        {
                            break;
                        }
                        num5++;
                        UIFont.EndLine(ref stringBuilder);
                        goto IL_3E7;
                    }
                }
                else
                {
                    num3 = (int)c;
                }
                if (!isDynamic && bmsymbol != null)
                {
                    i += bmsymbol.length - 1;
                    num3 = 0;
                }
            }
            IL_3E7:
            i++;
        }
        if (num4 < i)
        {
            stringBuilder.Append(text.Substring(num4, i - num4));
        }
        return stringBuilder.ToString();
    }

    public string WrapText(string text, float maxWidth, int maxLineCount, bool encoding)
    {
        return this.WrapText(text, maxWidth, maxLineCount, encoding, UIFont.SymbolStyle.None);
    }

    public string WrapText(string text, float maxWidth, int maxLineCount)
    {
        return this.WrapText(text, maxWidth, maxLineCount, false, UIFont.SymbolStyle.None);
    }
}