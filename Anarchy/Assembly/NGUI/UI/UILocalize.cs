using UnityEngine;

[AddComponentMenu("NGUI/UI/Localize")]
[RequireComponent(typeof(UIWidget))]
public class UILocalize : MonoBehaviour
{
    private string mLanguage;
    private bool mStarted;
    public string key;

    private void OnEnable()
    {
        if (this.mStarted && Localization.instance != null)
        {
            this.Localize();
        }
    }

    private void OnLocalize(Localization loc)
    {
        if (this.mLanguage != loc.currentLanguage)
        {
            this.Localize();
        }
    }

    private void Start()
    {
        this.mStarted = true;
        if (Localization.instance != null)
        {
            this.Localize();
        }
    }

    public void Localize()
    {
        Localization instance = Localization.instance;
        UIWidget component = base.GetComponent<UIWidget>();
        UILabel uilabel = component as UILabel;
        UISprite uisprite = component as UISprite;
        if (string.IsNullOrEmpty(this.mLanguage) && string.IsNullOrEmpty(this.key) && uilabel != null)
        {
            this.key = uilabel.text;
        }
        string text = (!string.IsNullOrEmpty(this.key)) ? instance.Get(this.key) : string.Empty;
        if (uilabel != null)
        {
            UIInput uiinput = NGUITools.FindInParents<UIInput>(uilabel.gameObject);
            if (uiinput != null && uiinput.label == uilabel)
            {
                uiinput.defaultText = text;
            }
            else
            {
                uilabel.text = text;
            }
        }
        else if (uisprite != null)
        {
            uisprite.spriteName = text;
            uisprite.MakePixelPerfect();
        }
        this.mLanguage = instance.currentLanguage;
    }
}