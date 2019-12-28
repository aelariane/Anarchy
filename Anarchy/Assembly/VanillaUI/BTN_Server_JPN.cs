using UnityEngine;

public class BTN_Server_JPN : MonoBehaviour
{
    private void OnClick()
    {
        PhotonNetwork.Disconnect();
        PhotonNetwork.ConnectToMaster("app-jp.exitgamescloud.com", 5055, FengGameManagerMKII.ApplicationId, UIMainReferences.ConnectField);
    }
}