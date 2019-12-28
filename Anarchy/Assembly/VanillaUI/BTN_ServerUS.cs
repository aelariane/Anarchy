using UnityEngine;

public class BTN_ServerUS : MonoBehaviour
{
    private void OnClick()
    {
        PhotonNetwork.Disconnect();
        PhotonNetwork.ConnectToMaster("app-us.exitgamescloud.com", 5055, FengGameManagerMKII.ApplicationId, UIMainReferences.ConnectField);
    }
}