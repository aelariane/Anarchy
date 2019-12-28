using UnityEngine;

public class BTN_Server_ASIA : MonoBehaviour
{
    private void OnClick()
    {
        PhotonNetwork.Disconnect();
        PhotonNetwork.ConnectToMaster("app-asia.exitgamescloud.com", 5055, FengGameManagerMKII.ApplicationId, UIMainReferences.ConnectField);
    }
}