using UnityEngine;

[RequireComponent(typeof(UILabel))]
[AddComponentMenu("NGUI/Examples/Typewriter Effect")]
public class TypewriterEffect : MonoBehaviour
{
    private UILabel mLabel;
    private float mNextChar;
    private int mOffset;
    private string mText;
    public int charsPerSecond = 40;

    private void Update()
    {
        if (this.mLabel == null)
        {
            this.mLabel = base.GetComponent<UILabel>();
            this.mLabel.supportEncoding = false;
            this.mLabel.symbolStyle = UIFont.SymbolStyle.None;
            this.mText = this.mLabel.font.WrapText(this.mLabel.text, (float)this.mLabel.lineWidth / this.mLabel.cachedTransform.localScale.x, this.mLabel.maxLineCount, false, UIFont.SymbolStyle.None);
        }
        if (this.mOffset < this.mText.Length)
        {
            if (this.mNextChar <= Time.time)
            {
                this.charsPerSecond = Mathf.Max(1, this.charsPerSecond);
                float num = 1f / (float)this.charsPerSecond;
                char c = this.mText[this.mOffset];
                if (c == '.' || c == '\n' || c == '!' || c == '?')
                {
                    num *= 4f;
                }
                this.mNextChar = Time.time + num;
                this.mLabel.text = this.mText.Substring(0, ++this.mOffset);
            }
        }
        else
        {
            UnityEngine.Object.Destroy(this);
        }
    }
}