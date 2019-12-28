using Optimization.Caching;
using UnityEngine;

public class BTN_START_SINGLE_GAMEPLAY : MonoBehaviour
{
    private void OnClick()
    {
        string selection = CacheGameObject.Find("PopupListMap").GetComponent<UIPopupList>().selection;
        string selection2 = CacheGameObject.Find("PopupListCharacter").GetComponent<UIPopupList>().selection;
        int difficulty = (!CacheGameObject.Find("CheckboxHard").GetComponent<UICheckbox>().isChecked) ? ((!CacheGameObject.Find("CheckboxAbnormal").GetComponent<UICheckbox>().isChecked) ? 0 : 2) : 1;
        IN_GAME_MAIN_CAMERA.Difficulty = difficulty;
        IN_GAME_MAIN_CAMERA.GameType = GameType.Single;
        IN_GAME_MAIN_CAMERA.singleCharacter = selection2.ToUpper();
        if (IN_GAME_MAIN_CAMERA.CameraMode == CameraType.TPS)
        {
            Screen.lockCursor = true;
        }
        Screen.showCursor = false;
        if (selection == "trainning_0")
        {
            IN_GAME_MAIN_CAMERA.Difficulty = -1;
        }
        FengGameManagerMKII.Level = LevelInfo.GetInfo(selection);
        Application.LoadLevel(LevelInfo.GetInfo(selection).MapName);
    }
}