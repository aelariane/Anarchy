using System;
using UnityEngine;

[Serializable]
public class BMSymbol
{
    private int mAdvance;
    private int mHeight;
    private bool mIsValid;
    private int mLength;
    private int mOffsetX;
    private int mOffsetY;
    private UIAtlas.Sprite mSprite;
    private Rect mUV;
    private int mWidth;
    public string sequence;

    public string spriteName;

    public int advance
    {
        get
        {
            return this.mAdvance;
        }
    }

    public int height
    {
        get
        {
            return this.mHeight;
        }
    }

    public int length
    {
        get
        {
            if (this.mLength == 0)
            {
                this.mLength = this.sequence.Length;
            }
            return this.mLength;
        }
    }

    public int offsetX
    {
        get
        {
            return this.mOffsetX;
        }
    }

    public int offsetY
    {
        get
        {
            return this.mOffsetY;
        }
    }

    public Rect uvRect
    {
        get
        {
            return this.mUV;
        }
    }

    public int width
    {
        get
        {
            return this.mWidth;
        }
    }

    public void MarkAsDirty()
    {
        this.mIsValid = false;
    }

    public bool Validate(UIAtlas atlas)
    {
        if (atlas == null)
        {
            return false;
        }
        if (!this.mIsValid)
        {
            if (string.IsNullOrEmpty(this.spriteName))
            {
                return false;
            }
            this.mSprite = ((!(atlas != null)) ? null : atlas.GetSprite(this.spriteName));
            if (this.mSprite != null)
            {
                Texture texture = atlas.texture;
                if (texture == null)
                {
                    this.mSprite = null;
                }
                else
                {
                    Rect rect = this.mSprite.outer;
                    this.mUV = rect;
                    if (atlas.coordinates == UIAtlas.Coordinates.Pixels)
                    {
                        this.mUV = NGUIMath.ConvertToTexCoords(this.mUV, texture.width, texture.height);
                    }
                    else
                    {
                        rect = NGUIMath.ConvertToPixels(rect, texture.width, texture.height, true);
                    }
                    this.mOffsetX = Mathf.RoundToInt(this.mSprite.paddingLeft * rect.width);
                    this.mOffsetY = Mathf.RoundToInt(this.mSprite.paddingTop * rect.width);
                    this.mWidth = Mathf.RoundToInt(rect.width);
                    this.mHeight = Mathf.RoundToInt(rect.height);
                    this.mAdvance = Mathf.RoundToInt(rect.width + (this.mSprite.paddingRight + this.mSprite.paddingLeft) * rect.width);
                    this.mIsValid = true;
                }
            }
        }
        return this.mSprite != null;
    }
}