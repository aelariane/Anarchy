using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BMFont
{
    [SerializeField]
    [HideInInspector]
    private int mBase;

    private Dictionary<int, BMGlyph> mDict = new Dictionary<int, BMGlyph>();

    [HideInInspector]
    [SerializeField]
    private int mHeight;

    [SerializeField]
    [HideInInspector]
    private List<BMGlyph> mSaved = new List<BMGlyph>();

    [SerializeField]
    [HideInInspector]
    private int mSize;

    [SerializeField]
    [HideInInspector]
    private string mSpriteName;

    [HideInInspector]
    [SerializeField]
    private int mWidth;

    public int baseOffset
    {
        get
        {
            return this.mBase;
        }
        set
        {
            this.mBase = value;
        }
    }

    public int charSize
    {
        get
        {
            return this.mSize;
        }
        set
        {
            this.mSize = value;
        }
    }

    public int glyphCount
    {
        get
        {
            return (!this.isValid) ? 0 : this.mSaved.Count;
        }
    }

    public bool isValid
    {
        get
        {
            return this.mSaved.Count > 0;
        }
    }

    public string spriteName
    {
        get
        {
            return this.mSpriteName;
        }
        set
        {
            this.mSpriteName = value;
        }
    }

    public int texHeight
    {
        get
        {
            return this.mHeight;
        }
        set
        {
            this.mHeight = value;
        }
    }

    public int texWidth
    {
        get
        {
            return this.mWidth;
        }
        set
        {
            this.mWidth = value;
        }
    }

    public void Clear()
    {
        this.mDict.Clear();
        this.mSaved.Clear();
    }

    public BMGlyph GetGlyph(int index, bool createIfMissing)
    {
        BMGlyph bmglyph = null;
        if (this.mDict.Count == 0)
        {
            int i = 0;
            int count = this.mSaved.Count;
            while (i < count)
            {
                BMGlyph bmglyph2 = this.mSaved[i];
                this.mDict.Add(bmglyph2.index, bmglyph2);
                i++;
            }
        }
        if (!this.mDict.TryGetValue(index, out bmglyph) && createIfMissing)
        {
            bmglyph = new BMGlyph();
            bmglyph.index = index;
            this.mSaved.Add(bmglyph);
            this.mDict.Add(index, bmglyph);
        }
        return bmglyph;
    }

    public BMGlyph GetGlyph(int index)
    {
        return this.GetGlyph(index, false);
    }

    public void Trim(int xMin, int yMin, int xMax, int yMax)
    {
        if (this.isValid)
        {
            int i = 0;
            int count = this.mSaved.Count;
            while (i < count)
            {
                BMGlyph bmglyph = this.mSaved[i];
                if (bmglyph != null)
                {
                    bmglyph.Trim(xMin, yMin, xMax, yMax);
                }
                i++;
            }
        }
    }
}