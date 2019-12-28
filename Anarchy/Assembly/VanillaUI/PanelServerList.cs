using UnityEngine;

public class PanelServerList : MonoBehaviour
{
    private int lang = -1;
    public GameObject label_back;
    public GameObject label_create;
    public GameObject label_name;

    public GameObject label_refresh;

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
        this.label_name.GetComponent<UILabel>().text = Language.server_name[Language.type];
        this.label_refresh.GetComponent<UILabel>().text = Language.btn_refresh[Language.type];
        this.label_back.GetComponent<UILabel>().text = Language.btn_back[Language.type];
        this.label_create.GetComponent<UILabel>().text = Language.btn_create_game[Language.type];
    }

    private void Update()
    {
        this.showTxt();
    }
}