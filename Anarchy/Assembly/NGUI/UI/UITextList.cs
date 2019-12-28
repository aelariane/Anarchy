using System.Collections.Generic;
using System.Text;
using UnityEngine;

[AddComponentMenu("NGUI/UI/Text List")]
public class UITextList : MonoBehaviour
{
    protected List<UITextList.Paragraph> mParagraphs = new List<UITextList.Paragraph>();
    protected float mScroll;
    protected bool mSelected;

    protected char[] mSeparator = new char[]
                            {
        '\n'
    };

    protected int mTotalLines;
    public int maxEntries = 50;
    public float maxHeight;
    public float maxWidth;
    public UITextList.Style style;

    public bool supportScrollWheel = true;
    public UILabel textLabel;

    public enum Style
    {
        Text,
        Chat
    }

    private void Awake()
    {
        if (this.textLabel == null)
        {
            this.textLabel = base.GetComponentInChildren<UILabel>();
        }
        if (this.textLabel != null)
        {
            this.textLabel.lineWidth = 0;
        }
        Collider collider = base.collider;
        if (collider != null)
        {
            if (this.maxHeight <= 0f)
            {
                this.maxHeight = collider.bounds.size.y / base.transform.lossyScale.y;
            }
            if (this.maxWidth <= 0f)
            {
                this.maxWidth = collider.bounds.size.x / base.transform.lossyScale.x;
            }
        }
    }

    private void OnScroll(float val)
    {
        if (this.mSelected && this.supportScrollWheel)
        {
            val *= ((this.style != UITextList.Style.Chat) ? -10f : 10f);
            this.mScroll = Mathf.Max(0f, this.mScroll + val);
            this.UpdateVisibleText();
        }
    }

    private void OnSelect(bool selected)
    {
        this.mSelected = selected;
    }

    protected void Add(string text, bool updateVisible)
    {
        UITextList.Paragraph paragraph;
        if (this.mParagraphs.Count < this.maxEntries)
        {
            paragraph = new UITextList.Paragraph();
        }
        else
        {
            paragraph = this.mParagraphs[0];
            this.mParagraphs.RemoveAt(0);
        }
        paragraph.text = text;
        this.mParagraphs.Add(paragraph);
        if (this.textLabel != null && this.textLabel.font != null)
        {
            paragraph.lines = this.textLabel.font.WrapText(paragraph.text, this.maxWidth / this.textLabel.transform.localScale.y, this.textLabel.maxLineCount, this.textLabel.supportEncoding, this.textLabel.symbolStyle).Split(this.mSeparator);
            this.mTotalLines = 0;
            int i = 0;
            int count = this.mParagraphs.Count;
            while (i < count)
            {
                this.mTotalLines += this.mParagraphs[i].lines.Length;
                i++;
            }
        }
        if (updateVisible)
        {
            this.UpdateVisibleText();
        }
    }

    protected void UpdateVisibleText()
    {
        if (this.textLabel != null)
        {
            UIFont font = this.textLabel.font;
            if (font != null)
            {
                int num = 0;
                int num2 = (this.maxHeight <= 0f) ? 100000 : Mathf.FloorToInt(this.maxHeight / this.textLabel.cachedTransform.localScale.y);
                int num3 = Mathf.RoundToInt(this.mScroll);
                if (num2 + num3 > this.mTotalLines)
                {
                    num3 = Mathf.Max(0, this.mTotalLines - num2);
                    this.mScroll = (float)num3;
                }
                if (this.style == UITextList.Style.Chat)
                {
                    num3 = Mathf.Max(0, this.mTotalLines - num2 - num3);
                }
                StringBuilder stringBuilder = new StringBuilder();
                int i = 0;
                int count = this.mParagraphs.Count;
                while (i < count)
                {
                    UITextList.Paragraph paragraph = this.mParagraphs[i];
                    int j = 0;
                    int num4 = paragraph.lines.Length;
                    while (j < num4)
                    {
                        string value = paragraph.lines[j];
                        if (num3 > 0)
                        {
                            num3--;
                        }
                        else
                        {
                            if (stringBuilder.Length > 0)
                            {
                                stringBuilder.Append("\n");
                            }
                            stringBuilder.Append(value);
                            num++;
                            if (num >= num2)
                            {
                                break;
                            }
                        }
                        j++;
                    }
                    if (num >= num2)
                    {
                        break;
                    }
                    i++;
                }
                this.textLabel.text = stringBuilder.ToString();
            }
        }
    }

    public void Add(string text)
    {
        this.Add(text, true);
    }

    public void Clear()
    {
        this.mParagraphs.Clear();
        this.UpdateVisibleText();
    }

    protected class Paragraph
    {
        public string[] lines;
        public string text;
    }
}