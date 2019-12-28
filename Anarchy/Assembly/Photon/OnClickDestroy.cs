using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public class OnClickDestroy : Photon.MonoBehaviour
{
    public bool DestroyByRpc;

    private void OnClick()
    {
        if (!this.DestroyByRpc)
        {
            PhotonNetwork.Destroy(base.gameObject);
        }
        else
        {
            BasePV.RPC("DestroyRpc", PhotonTargets.AllBuffered, new object[0]);
        }
    }

    [RPC]
    public void DestroyRpc()
    {
        UnityEngine.Object.Destroy(base.gameObject);
        PhotonNetwork.UnAllocateViewID(BasePV.viewID);
    }
}