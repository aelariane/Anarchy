using Anarchy;
using UnityEngine;

public class BTN_ToOption : MonoBehaviour
{
    private void OnClick()
    {
        NGUITools.SetActive(base.transform.parent.gameObject, false);
        NGUITools.SetActive(UIMainReferences.Main.panelOption, true);
        FengCustomInputs.Main.showKeyMap();
        InputManager.MenuOn = true;
    }
}