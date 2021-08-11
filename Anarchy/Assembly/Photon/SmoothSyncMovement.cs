using Optimization.Caching;
using UnityEngine;

internal class SmoothSyncMovement : Photon.MonoBehaviour, IPunObservable
{
    private float activityTime = 0f;
    private Rigidbody baseR;
    private Transform baseT;
    private Vector3 correctPlayerPos = Vectors.zero;
    private Quaternion correctPlayerRot = Quaternion.identity;
    private Vector3 oldPlayerPos = Vectors.zero;
    private Quaternion oldPlayerRot = Quaternion.identity;
    private Vector3 correctPlayerVelocity = Vectors.zero;
    public Quaternion CorrectCameraRot;
    public bool Disabled;
    private bool noVelocity = true;
    public bool PhotonCamera = false;
    public bool AnarchySync = false;
    public float SmoothingDelay = 5f;

    public void Awake()
    {
        if (BasePV == null || BasePV.observed != this)
        {
            Debug.LogWarning(this + " is not observed by this object's photonView! OnPhotonSerializeView() in this class won't be used.");
        }
        if (IN_GAME_MAIN_CAMERA.GameType == GameType.Single)
        {
            enabled = false;
        }
        baseT = transform;
        correctPlayerPos = baseT.position;
        correctPlayerRot = baseT.rotation;
        if (rigidbody != null)
        {
            noVelocity = false;
            baseR = rigidbody;
            correctPlayerVelocity = baseR.velocity;
        }
    }

    private void OnEnable()
    {
        baseT = transform;
        correctPlayerPos = baseT.position;
        correctPlayerRot = baseT.rotation;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            stream.SendNext(baseT.position);
            stream.SendNext(baseT.rotation);
            if (!noVelocity)
            {
                stream.SendNext(baseR.velocity);
            }
            if (PhotonCamera)
            {
                stream.SendNext(IN_GAME_MAIN_CAMERA.BaseT.rotation);
            }
        }
        else
        {
            this.correctPlayerPos = (Vector3)stream.ReceiveNext();
            this.correctPlayerRot = (Quaternion)stream.ReceiveNext();
            if (PhotonNetwork.IsMasterClient && Anarchy.GameModes.AfkKill.Enabled)
            {
                if (oldPlayerPos != correctPlayerPos)
                {
                    activityTime = 0f;
                    oldPlayerPos = correctPlayerPos;
                }
                if (oldPlayerRot != correctPlayerRot)
                {
                    activityTime = 0f;
                    oldPlayerRot = correctPlayerRot;
                }
            }
            if (!noVelocity)
            {
                this.correctPlayerVelocity = (Vector3)stream.ReceiveNext();
            }
            if (PhotonCamera)
            {
                CorrectCameraRot = (Quaternion)stream.ReceiveNext();
            }
        }
    }

    public void Update()
    {
        if (Disabled || BasePV.IsMine)
        {
            return;
        }
        if (baseT != null)
        {
            float delta = Time.deltaTime * SmoothingDelay;
            baseT.position = Vector3.Lerp(baseT.position, correctPlayerPos, delta);
            baseT.rotation = Quaternion.Lerp(baseT.rotation, correctPlayerRot, delta);
            if (PhotonNetwork.IsMasterClient && IN_GAME_MAIN_CAMERA.GameMode != GameMode.Racing & FengGameManagerMKII.Level.RespawnMode != RespawnMode.DEATHMATCH && !Anarchy.GameModes.EndlessRespawn.Enabled && Anarchy.GameModes.AfkKill.Enabled && oldPlayerPos == correctPlayerPos && oldPlayerRot == correctPlayerRot)
            {
                activityTime += Time.deltaTime;
                if (activityTime >= Anarchy.GameModes.AfkKill.GetInt(0))
                {
                    this.BasePV.RPC("netDie2", PhotonTargets.All, new object[] { -1, "AFK Kill " });
                }
            }
        }
        if (!noVelocity)
        {
            baseR.velocity = this.correctPlayerVelocity;
        }
    }
}