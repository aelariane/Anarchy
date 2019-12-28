using UnityEngine;

public class ParticleScaling : MonoBehaviour
{
    private Transform baseT;
    private Material material;

    private void Awake()
    {
        baseT = transform;
        material = GetComponent<ParticleSystem>().renderer.material;
    }

    private void OnEnable()
    {
        baseT = transform;
        material = GetComponent<ParticleSystem>().renderer.material;
    }

    public void OnWillRenderObject()
    {
        material.SetVector("_Center", baseT.position);
        material.SetVector("_Scaling", baseT.lossyScale);
        Matrix4x4 matrix = IN_GAME_MAIN_CAMERA.BaseCamera.worldToCameraMatrix;
        material.SetMatrix("_Camera", matrix);
        material.SetMatrix("_CameraInv", matrix.inverse);
    }
}