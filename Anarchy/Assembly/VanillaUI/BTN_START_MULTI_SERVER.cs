using Optimization.Caching;
using UnityEngine;

public class BTN_START_MULTI_SERVER : MonoBehaviour
{
    private void OnClick()
    {
        string text = CacheGameObject.Find("InputServerName").GetComponent<UIInput>().label.text;
        int maxPlayers = int.Parse(CacheGameObject.Find("InputMaxPlayer").GetComponent<UIInput>().label.text);
        int num = int.Parse(CacheGameObject.Find("InputMaxTime").GetComponent<UIInput>().label.text);
        string selection = CacheGameObject.Find("PopupListMap").GetComponent<UIPopupList>().selection;
        string text2 = (!CacheGameObject.Find("CheckboxHard").GetComponent<UICheckbox>().isChecked) ? ((!CacheGameObject.Find("CheckboxAbnormal").GetComponent<UICheckbox>().isChecked) ? "normal" : "abnormal") : "hard";
        string text3 = string.Empty;
        if (IN_GAME_MAIN_CAMERA.DayLight == DayLight.Day)
        {
            text3 = "day";
        }
        if (IN_GAME_MAIN_CAMERA.DayLight == DayLight.Dawn)
        {
            text3 = "dawn";
        }
        if (IN_GAME_MAIN_CAMERA.DayLight == DayLight.Night)
        {
            text3 = "night";
        }
        string text4 = CacheGameObject.Find("InputStartServerPWD").GetComponent<UIInput>().label.text;
        if (text4.Length > 0)
        {
            SimpleAES simpleAES = new SimpleAES();
            text4 = simpleAES.Encrypt(text4);
        }
        text = string.Concat(new object[]
        {
            text,
            "`",
            selection,
            "`",
            text2,
            "`",
            num,
            "`",
            text3,
            "`",
            text4,
            "`",
            UnityEngine.Random.Range(0, 50000)
        });
        PhotonNetwork.CreateRoom(text, new RoomOptions() { isOpen = true, isVisible = true, maxPlayers = maxPlayers }, null);
    }
}