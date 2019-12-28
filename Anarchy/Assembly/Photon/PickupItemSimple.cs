using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public class PickupItemSimple : Photon.MonoBehaviour
{
    public bool PickupOnCollide;
    public float SecondsBeforeRespawn = 2f;
    public bool SentPickup;

    public void OnTriggerEnter(Collider other)
    {
        PhotonView component = other.GetComponent<PhotonView>();
        if (this.PickupOnCollide && component != null && component.IsMine)
        {
            this.Pickup();
        }
    }

    public void Pickup()
    {
        if (this.SentPickup)
        {
            return;
        }
        this.SentPickup = true;
        BasePV.RPC("PunPickupSimple", PhotonTargets.AllViaServer, new object[0]);
    }

    [RPC]
    public void PunPickupSimple(PhotonMessageInfo msgInfo)
    {
        if (!this.SentPickup || !msgInfo.Sender.IsLocal || base.gameObject.GetActive())
        {
        }
        this.SentPickup = false;
        if (!base.gameObject.GetActive())
        {
            Debug.Log("Ignored PU RPC, cause item is inactive. " + base.gameObject);
            return;
        }
        double num = PhotonNetwork.time - msgInfo.Timestamp;
        float num2 = this.SecondsBeforeRespawn - (float)num;
        if (num2 > 0f)
        {
            base.gameObject.SetActive(false);
            base.Invoke("RespawnAfter", num2);
        }
    }

    public void RespawnAfter()
    {
        if (base.gameObject != null)
        {
            base.gameObject.SetActive(true);
        }
    }
}