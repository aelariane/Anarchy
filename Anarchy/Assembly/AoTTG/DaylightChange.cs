using UnityEngine;

public class DaylightChange : MonoBehaviour
{
    private void OnSelectionChange()
    {
        if (base.GetComponent<UIPopupList>().selection == "DAY")
        {
            IN_GAME_MAIN_CAMERA.DayLight = DayLight.Day;
        }
        if (base.GetComponent<UIPopupList>().selection == "DAWN")
        {
            IN_GAME_MAIN_CAMERA.DayLight = DayLight.Dawn;
        }
        if (base.GetComponent<UIPopupList>().selection == "NIGHT")
        {
            IN_GAME_MAIN_CAMERA.DayLight = DayLight.Night;
        }
    }
}