using System;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("NGUI/UI/Atlas")]
public class UIAtlas : MonoBehaviour
{
    [HideInInspector]
    [SerializeField]
    private Material material;

    [SerializeField]
    [HideInInspector]
    private UIAtlas.Coordinates mCoordinates;

    [HideInInspector]
    [SerializeField]
    private float mPixelSize = 1f;

    private int mPMA = -1;

    [HideInInspector]
    [SerializeField]
    private UIAtlas mReplacement;

    [HideInInspector]
    [SerializeField]
    private List<UIAtlas.Sprite> sprites = new List<UIAtlas.Sprite>();

    public enum Coordinates
    {
        Pixels,
        TexCoords
    }

    public UIAtlas.Coordinates coordinates
    {
        get
        {
            return (!(this.mReplacement != null)) ? this.mCoordinates : this.mReplacement.coordinates;
        }
        set
        {
            if (this.mReplacement != null)
            {
                this.mReplacement.coordinates = value;
            }
            else if (this.mCoordinates != value)
            {
                if (this.material == null || this.material.mainTexture == null)
                {
                    Debug.LogError("Can't switch coordinates until the atlas material has a valid texture");
                    return;
                }
                this.mCoordinates = value;
                Texture mainTexture = this.material.mainTexture;
                int i = 0;
                int count = this.sprites.Count;
                while (i < count)
                {
                    UIAtlas.Sprite sprite = this.sprites[i];
                    if (this.mCoordinates == UIAtlas.Coordinates.TexCoords)
                    {
                        sprite.outer = NGUIMath.ConvertToTexCoords(sprite.outer, mainTexture.width, mainTexture.height);
                        sprite.inner = NGUIMath.ConvertToTexCoords(sprite.inner, mainTexture.width, mainTexture.height);
                    }
                    else
                    {
                        sprite.outer = NGUIMath.ConvertToPixels(sprite.outer, mainTexture.width, mainTexture.height, true);
                        sprite.inner = NGUIMath.ConvertToPixels(sprite.inner, mainTexture.width, mainTexture.height, true);
                    }
                    i++;
                }
            }
        }
    }

    public float pixelSize
    {
        get
        {
            return (!(this.mReplacement != null)) ? this.mPixelSize : this.mReplacement.pixelSize;
        }
        set
        {
            if (this.mReplacement != null)
            {
                this.mReplacement.pixelSize = value;
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
            if (this.mPMA == -1)
            {
                Material spriteMaterial = this.spriteMaterial;
                this.mPMA = ((!(spriteMaterial != null) || !(spriteMaterial.shader != null) || !spriteMaterial.shader.name.Contains("Premultiplied")) ? 0 : 1);
            }
            return this.mPMA == 1;
        }
    }

    public UIAtlas replacement
    {
        get
        {
            return this.mReplacement;
        }
        set
        {
            UIAtlas uiatlas = value;
            if (uiatlas == this)
            {
                uiatlas = null;
            }
            if (this.mReplacement != uiatlas)
            {
                if (uiatlas != null && uiatlas.replacement == this)
                {
                    uiatlas.replacement = null;
                }
                if (this.mReplacement != null)
                {
                    this.MarkAsDirty();
                }
                this.mReplacement = uiatlas;
                this.MarkAsDirty();
            }
        }
    }

    public List<UIAtlas.Sprite> spriteList
    {
        get
        {
            return (!(this.mReplacement != null)) ? this.sprites : this.mReplacement.spriteList;
        }
        set
        {
            if (this.mReplacement != null)
            {
                this.mReplacement.spriteList = value;
            }
            else
            {
                this.sprites = value;
            }
        }
    }

    public Material spriteMaterial
    {
        get
        {
            return (!(this.mReplacement != null)) ? this.material : this.mReplacement.spriteMaterial;
        }
        set
        {
            if (this.mReplacement != null)
            {
                this.mReplacement.spriteMaterial = value;
            }
            else if (this.material == null)
            {
                this.mPMA = 0;
                this.material = value;
            }
            else
            {
                this.MarkAsDirty();
                this.mPMA = -1;
                this.material = value;
                this.MarkAsDirty();
            }
        }
    }

    public Texture texture
    {
        get
        {
            return (!(this.mReplacement != null)) ? ((!(this.material != null)) ? null : this.material.mainTexture) : this.mReplacement.texture;
        }
    }

    private static int CompareString(string a, string b)
    {
        return a.CompareTo(b);
    }

    private bool References(UIAtlas atlas)
    {
        return !(atlas == null) && (atlas == this || (this.mReplacement != null && this.mReplacement.References(atlas)));
    }

    public static bool CheckIfRelated(UIAtlas a, UIAtlas b)
    {
        return !(a == null) && !(b == null) && (a == b || a.References(b) || b.References(a));
    }

    public BetterList<string> GetListOfSprites()
    {
        if (this.mReplacement != null)
        {
            return this.mReplacement.GetListOfSprites();
        }
        BetterList<string> betterList = new BetterList<string>();
        int i = 0;
        int count = this.sprites.Count;
        while (i < count)
        {
            UIAtlas.Sprite sprite = this.sprites[i];
            if (sprite != null && !string.IsNullOrEmpty(sprite.name))
            {
                betterList.Add(sprite.name);
            }
            i++;
        }
        return betterList;
    }

    public BetterList<string> GetListOfSprites(string match)
    {
        if (this.mReplacement != null)
        {
            return this.mReplacement.GetListOfSprites(match);
        }
        if (string.IsNullOrEmpty(match))
        {
            return this.GetListOfSprites();
        }
        BetterList<string> betterList = new BetterList<string>();
        int i = 0;
        int count = this.sprites.Count;
        while (i < count)
        {
            UIAtlas.Sprite sprite = this.sprites[i];
            if (sprite != null && !string.IsNullOrEmpty(sprite.name) && string.Equals(match, sprite.name, StringComparison.OrdinalIgnoreCase))
            {
                betterList.Add(sprite.name);
                return betterList;
            }
            i++;
        }
        string[] array = match.Split(new char[]
        {
            ' '
        }, StringSplitOptions.RemoveEmptyEntries);
        for (int j = 0; j < array.Length; j++)
        {
            array[j] = array[j].ToLower();
        }
        int k = 0;
        int count2 = this.sprites.Count;
        while (k < count2)
        {
            UIAtlas.Sprite sprite2 = this.sprites[k];
            if (sprite2 != null && !string.IsNullOrEmpty(sprite2.name))
            {
                string text = sprite2.name.ToLower();
                int num = 0;
                for (int l = 0; l < array.Length; l++)
                {
                    if (text.Contains(array[l]))
                    {
                        num++;
                    }
                }
                if (num == array.Length)
                {
                    betterList.Add(sprite2.name);
                }
            }
            k++;
        }
        return betterList;
    }

    public UIAtlas.Sprite GetSprite(string name)
    {
        if (this.mReplacement != null)
        {
            return this.mReplacement.GetSprite(name);
        }
        if (!string.IsNullOrEmpty(name))
        {
            int i = 0;
            int count = this.sprites.Count;
            while (i < count)
            {
                UIAtlas.Sprite sprite = this.sprites[i];
                if (!string.IsNullOrEmpty(sprite.name) && name == sprite.name)
                {
                    return sprite;
                }
                i++;
            }
        }
        return null;
    }

    public void MarkAsDirty()
    {
        if (this.mReplacement != null)
        {
            this.mReplacement.MarkAsDirty();
        }
        UISprite[] array = NGUITools.FindActive<UISprite>();
        int i = 0;
        int num = array.Length;
        while (i < num)
        {
            UISprite uisprite = array[i];
            if (UIAtlas.CheckIfRelated(this, uisprite.atlas))
            {
                UIAtlas atlas = uisprite.atlas;
                uisprite.atlas = null;
                uisprite.atlas = atlas;
            }
            i++;
        }
        UIFont[] array2 = Resources.FindObjectsOfTypeAll(typeof(UIFont)) as UIFont[];
        int j = 0;
        int num2 = array2.Length;
        while (j < num2)
        {
            UIFont uifont = array2[j];
            if (UIAtlas.CheckIfRelated(this, uifont.atlas))
            {
                UIAtlas atlas2 = uifont.atlas;
                uifont.atlas = null;
                uifont.atlas = atlas2;
            }
            j++;
        }
        UILabel[] array3 = NGUITools.FindActive<UILabel>();
        int k = 0;
        int num3 = array3.Length;
        while (k < num3)
        {
            UILabel uilabel = array3[k];
            if (uilabel.font != null && UIAtlas.CheckIfRelated(this, uilabel.font.atlas))
            {
                UIFont font = uilabel.font;
                uilabel.font = null;
                uilabel.font = font;
            }
            k++;
        }
    }

    [Serializable]
    public class Sprite
    {
        public Rect inner = new Rect(0f, 0f, 1f, 1f);
        public string name = "Unity Bug";

        public Rect outer = new Rect(0f, 0f, 1f, 1f);
        public float paddingBottom;
        public float paddingLeft;
        public float paddingRight;
        public float paddingTop;
        public bool rotated;

        public bool hasPadding
        {
            get
            {
                return this.paddingLeft != 0f || this.paddingRight != 0f || this.paddingTop != 0f || this.paddingBottom != 0f;
            }
        }
    }
}