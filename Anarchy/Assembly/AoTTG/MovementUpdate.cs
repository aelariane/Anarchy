using Optimization.Caching;
using UnityEngine;

public class MovementUpdate : MonoBehaviour
{
    private Vector3 lastPosition;
    private Quaternion lastRotation;
    private Vector3 lastVelocity;
    private Vector3 targetPosition;
    public bool disabled;

    private void Start()
    {
        if (IN_GAME_MAIN_CAMERA.GameType == GameType.Single)
        {
            this.disabled = true;
            base.enabled = false;
        }
        else if (base.networkView.isMine)
        {
            base.networkView.RPC("updateMovement", RPCMode.OthersBuffered, new object[]
            {
                base.transform.position,
                base.transform.rotation,
                base.transform.localScale,
                Vectors.zero
            });
        }
        else
        {
            this.targetPosition = base.transform.position;
        }
    }

    private void Update()
    {
        if (this.disabled)
        {
            return;
        }
        if (Network.peerType == NetworkPeerType.Disconnected)
        {
            return;
        }
        if (Network.peerType == NetworkPeerType.Connecting)
        {
            return;
        }
        if (base.networkView.isMine)
        {
            if (Vector3.Distance(base.transform.position, this.lastPosition) >= 0.5f)
            {
                this.lastPosition = base.transform.position;
                base.networkView.RPC("updateMovement", RPCMode.Others, new object[]
                {
                    base.transform.position,
                    base.transform.rotation,
                    base.transform.localScale,
                    base.rigidbody.velocity
                });
            }
            else if (Vector3.Distance(base.transform.rigidbody.velocity, this.lastVelocity) >= 0.1f)
            {
                this.lastVelocity = base.transform.rigidbody.velocity;
                base.networkView.RPC("updateMovement", RPCMode.Others, new object[]
                {
                    base.transform.position,
                    base.transform.rotation,
                    base.transform.localScale,
                    base.rigidbody.velocity
                });
            }
            else if (Quaternion.Angle(base.transform.rotation, this.lastRotation) >= 1f)
            {
                this.lastRotation = base.transform.rotation;
                base.networkView.RPC("updateMovement", RPCMode.Others, new object[]
                {
                    base.transform.position,
                    base.transform.rotation,
                    base.transform.localScale,
                    base.rigidbody.velocity
                });
            }
        }
        else
        {
            base.transform.position = Vector3.Slerp(base.transform.position, this.targetPosition, Time.deltaTime * 2f);
        }
    }

    [RPC]
    private void updateMovement(Vector3 newPosition, Quaternion newRotation, Vector3 newScale, Vector3 veloctiy)
    {
        this.targetPosition = newPosition;
        base.transform.rotation = newRotation;
        base.transform.localScale = newScale;
        base.rigidbody.velocity = veloctiy;
    }
}