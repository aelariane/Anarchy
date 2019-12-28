using UnityEngine;

public class BTN_SetDefault : MonoBehaviour
{
    private void OnClick()
    {
        FengCustomInputs.Main.setToDefault();
        FengCustomInputs.Main.showKeyMap();
    }
}