using Optimization.Caching;
using UnityEngine;

public class BTN_ToJoin : MonoBehaviour
{
    private void OnClick()
    {
        NGUITools.SetActive(base.transform.parent.gameObject, false);
        NGUITools.SetActive(UIMainReferences.Main.PanelMultiJoinPrivate, true);
        CacheGameObject.Find("LabelJoinInfo").GetComponent<UILabel>().text = string.Empty;
        if (PlayerPrefs.HasKey("lastIP"))
        {
            CacheGameObject.Find("InputIP").GetComponent<UIInput>().label.text = PlayerPrefs.GetString("lastIP");
        }
        if (PlayerPrefs.HasKey("lastPort"))
        {
            CacheGameObject.Find("InputPort").GetComponent<UIInput>().label.text = PlayerPrefs.GetString("lastPort");
        }
    }

    private void Start()
    {
        base.gameObject.GetComponent<UIButton>().isEnabled = false;
    }
}