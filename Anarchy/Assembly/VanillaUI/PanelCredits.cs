using UnityEngine;

public class PanelCredits : MonoBehaviour
{
    private int lang = -1;
    public GameObject label_back;
    public GameObject label_title;

    private void showTxt()
    {
        if (this.lang == Language.type)
        {
            return;
        }
        this.lang = Language.type;
        this.label_title.GetComponent<UILabel>().text = Language.btn_credits[Language.type];
        this.label_back.GetComponent<UILabel>().text = Language.btn_back[Language.type];
    }

    private void Update()
    {
        this.showTxt();
    }
}