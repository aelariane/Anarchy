using Optimization.Caching;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public class MoveByKeys : Photon.MonoBehaviour
{
    public float speed = 10f;

    private void Start()
    {
        base.enabled = BasePV.IsMine;
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.A))
        {
            base.transform.position += Vectors.left * (this.speed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.D))
        {
            base.transform.position += Vectors.right * (this.speed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.W))
        {
            base.transform.position += Vectors.forward * (this.speed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.S))
        {
            base.transform.position += Vectors.back * (this.speed * Time.deltaTime);
        }
    }
}