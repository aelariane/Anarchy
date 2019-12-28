using Optimization.Caching;
using UnityEngine;

public class BTN_Enter_PWD : MonoBehaviour
{
    private void OnClick()
    {
        string text = CacheGameObject.Find("InputEnterPWD").GetComponent<UIInput>().label.text;
        SimpleAES simpleAES = new SimpleAES();
        if (text == simpleAES.Decrypt(PanelMultiJoinPWD.Password))
        {
            PhotonNetwork.JoinRoom(PanelMultiJoinPWD.roomName);
        }
        else
        {
            NGUITools.SetActive(UIMainReferences.Main.PanelMultiPWD, false);
            NGUITools.SetActive(UIMainReferences.Main.panelMultiROOM, true);
            CacheGameObject.Find("PanelMultiROOM").GetComponent<PanelMultiJoin>().refresh();
        }
    }
}