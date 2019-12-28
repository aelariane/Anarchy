using UnityEngine;

public class PanelResult : MonoBehaviour
{
    private int lang = -1;
    public GameObject label_quit;

    private void OnEnable()
    {
    }

    private void showTxt()
    {
        if (this.lang == Language.type)
        {
            return;
        }
        this.lang = Language.type;
        this.label_quit.GetComponent<UILabel>().text = Language.btn_quit[Language.type];
    }

    private void Update()
    {
        this.showTxt();
    }
}