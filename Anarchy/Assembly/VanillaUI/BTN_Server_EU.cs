using UnityEngine;

public class BTN_Server_EU : MonoBehaviour
{
    private void OnClick()
    {
        PhotonNetwork.Disconnect();
        PhotonNetwork.ConnectToMaster("app-eu.exitgamescloud.com", NetworkingPeer.ProtocolToNameServerPort[PhotonNetwork.networkingPeer.UsedProtocol], FengGameManagerMKII.ApplicationId, UIMainReferences.ConnectField);
    }
}