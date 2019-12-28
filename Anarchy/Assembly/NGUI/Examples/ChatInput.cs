using UnityEngine;

[AddComponentMenu("NGUI/Examples/Chat Input")]
[RequireComponent(typeof(UIInput))]
public class ChatInput : MonoBehaviour
{
    private bool mIgnoreNextEnter;
    private UIInput mInput;
    public bool fillWithDummyData;
    public UITextList textList;

    private void OnSubmit()
    {
        if (this.textList != null)
        {
            string text = NGUITools.StripSymbols(this.mInput.text);
            if (!string.IsNullOrEmpty(text))
            {
                this.textList.Add(text);
                this.mInput.text = string.Empty;
                this.mInput.selected = false;
            }
        }
        this.mIgnoreNextEnter = true;
    }

    private void Start()
    {
        this.mInput = base.GetComponent<UIInput>();
        if (this.fillWithDummyData && this.textList != null)
        {
            for (int i = 0; i < 30; i++)
            {
                this.textList.Add(string.Concat(new object[]
                {
                    (i % 2 != 0) ? "[AAAAAA]" : "[FFFFFF]",
                    "This is an example paragraph for the text list, testing line ",
                    i,
                    "[-]"
                }));
            }
        }
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Return))
        {
            if (!this.mIgnoreNextEnter && !this.mInput.selected)
            {
                this.mInput.selected = true;
            }
            this.mIgnoreNextEnter = false;
        }
    }
}