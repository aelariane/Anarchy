using Optimization.Caching;
using UnityEngine;

public class SmoothSyncMovement3 : Photon.MonoBehaviour, IPunObservable
{
    private Transform baseT;
    private Vector3 correctPlayerPos = Vectors.zero;
    private Quaternion correctPlayerRot = Quaternion.identity;
    public bool disabled;
    public float SmoothingDelay = 10f;

    public void Awake()
    {
        if (BasePV == null || BasePV.observed != this)
        {
            Debug.LogWarning(this + " is not observed by this object's photonView! OnPhotonSerializeView() in this class won't be used.");
        }
        if (IN_GAME_MAIN_CAMERA.GameType == GameType.Single)
        {
            base.enabled = false;
        }
        baseT = transform;
        correctPlayerPos = baseT.position;
        correctPlayerRot = baseT.rotation;
    }

    private void OnEnable()
    {
        correctPlayerPos = baseT.position;
        correctPlayerRot = baseT.rotation;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            stream.SendNext(baseT.position);
            stream.SendNext(baseT.rotation);
        }
        else
        {
            this.correctPlayerPos = (Vector3)stream.ReceiveNext();
            this.correctPlayerRot = (Quaternion)stream.ReceiveNext();
        }
    }

    public void Update()
    {
        if (this.disabled)
        {
            return;
        }
        if (!BasePV.IsMine)
        {
            float delta = Time.deltaTime * this.SmoothingDelay;
            baseT.position = Vector3.Lerp(baseT.position, this.correctPlayerPos, delta);
            baseT.rotation = Quaternion.Lerp(baseT.rotation, this.correctPlayerRot, delta);
        }
    }
}