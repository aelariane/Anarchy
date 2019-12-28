using UnityEngine;

public class BTN_toSingleSet : MonoBehaviour
{
    private void OnClick()
    {
        NGUITools.SetActive(base.transform.parent.gameObject, false);
        NGUITools.SetActive(UIMainReferences.Main.panelSingleSet, true);
    }

    private void Start()
    {
    }
}