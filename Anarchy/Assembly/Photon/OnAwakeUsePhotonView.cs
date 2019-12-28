using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public class OnAwakeUsePhotonView : Photon.MonoBehaviour
{
    private void Awake()
    {
        if (!BasePV.IsMine)
        {
            return;
        }
        BasePV.RPC("OnAwakeRPC", PhotonTargets.All, new object[0]);
    }

    private void Start()
    {
        if (!BasePV.IsMine)
        {
            return;
        }
        BasePV.RPC("OnAwakeRPC", PhotonTargets.All, new object[]
        {
            1
        });
    }

    [RPC]
    public void OnAwakeRPC()
    {
        Debug.Log("RPC: 'OnAwakeRPC' PhotonView: " + BasePV);
    }

    [RPC]
    public void OnAwakeRPC(byte myParameter)
    {
        Debug.Log(string.Concat(new object[]
        {
            "RPC: 'OnAwakeRPC' Parameter: ",
            myParameter,
            " PhotonView: ",
            BasePV
        }));
    }
}