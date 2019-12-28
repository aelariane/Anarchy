using Anarchy;
using Optimization.Caching;
using UnityEngine;

public class SpectatorMovement : MonoBehaviour
{
    private float speed = 100f;
    public bool disable;

    private void Update()
    {
        if (this.disable)
        {
            return;
        }
        float num;
        if (InputManager.IsInput[InputCode.Up])
        {
            num = 1f;
        }
        else if (InputManager.IsInput[InputCode.Down])
        {
            num = -1f;
        }
        else
        {
            num = 0f;
        }
        float num2;
        if (InputManager.IsInput[InputCode.Left])
        {
            num2 = -1f;
        }
        else if (InputManager.IsInput[InputCode.Right])
        {
            num2 = 1f;
        }
        else
        {
            num2 = 0f;
        }
        if (num > 0f)
        {
            base.transform.position += base.transform.Forward() * this.speed * Time.deltaTime;
        }
        if (num < 0f)
        {
            base.transform.position -= base.transform.Forward() * this.speed * Time.deltaTime;
        }
        if (num2 > 0f)
        {
            base.transform.position += base.transform.right * this.speed * Time.deltaTime;
        }
        if (num2 < 0f)
        {
            base.transform.position -= base.transform.right * this.speed * Time.deltaTime;
        }
        if (InputManager.IsInput[InputCode.LeftRope])
        {
            base.transform.position -= base.transform.Up() * this.speed * Time.deltaTime;
        }
        if (InputManager.IsInput[InputCode.RightRope])
        {
            base.transform.position += base.transform.Up() * this.speed * Time.deltaTime;
        }
    }
}