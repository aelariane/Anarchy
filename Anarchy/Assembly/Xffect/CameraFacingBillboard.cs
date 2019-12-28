using Optimization.Caching;
using UnityEngine;

public class CameraFacingBillboard : MonoBehaviour
{
    private Camera referenceCamera;
    public CameraFacingBillboard.Axis axis;
    public bool reverseFace;

    public enum Axis
    {
        up,
        down,
        left,
        right,
        forward,
        back
    }

    private void Awake()
    {
        if (!this.referenceCamera)
        {
            this.referenceCamera = IN_GAME_MAIN_CAMERA.BaseCamera;
        }
    }

    private void Update()
    {
        Vector3 worldPosition = base.transform.position + this.referenceCamera.transform.rotation * ((!this.reverseFace) ? Vectors.back : Vectors.forward);
        Vector3 worldUp = this.referenceCamera.transform.rotation * this.GetAxis(this.axis);
        base.transform.LookAt(worldPosition, worldUp);
    }

    public Vector3 GetAxis(CameraFacingBillboard.Axis refAxis)
    {
        switch (refAxis)
        {
            case CameraFacingBillboard.Axis.down:
                return Vectors.down;

            case CameraFacingBillboard.Axis.left:
                return Vectors.left;

            case CameraFacingBillboard.Axis.right:
                return Vectors.right;

            case CameraFacingBillboard.Axis.forward:
                return Vectors.forward;

            case CameraFacingBillboard.Axis.back:
                return Vectors.back;

            default:
                return Vectors.up;
        }
    }
}