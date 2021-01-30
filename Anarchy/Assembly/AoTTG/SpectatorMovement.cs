using Anarchy;
using Optimization.Caching;
using UnityEngine;

public class SpectatorMovement : MonoBehaviour
{
    private Transform baseT;
    private float speed = 100f;
    public bool disable;

    private void Awake()
    {
        baseT = transform;
    }

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
        float spd = speed;
        if (InputManager.IsInput[InputCode.Gas])
        {
            spd *= 5;
        }
        if (num > 0f)
        {
            baseT.position += baseT.Forward() * spd * Time.deltaTime;
        }
        if (num < 0f)
        {
            baseT.position -= baseT.Forward() * spd * Time.deltaTime;
        }
        if (num2 > 0f)
        {
            baseT.position += baseT.right * spd * Time.deltaTime;
        }
        if (num2 < 0f)
        {
            baseT.position -= baseT.right * spd * Time.deltaTime;
        }
        if (InputManager.IsInput[InputCode.LeftRope])
        {
            baseT.position -= baseT.Up() * spd * Time.deltaTime;
        }
        if (InputManager.IsInput[InputCode.RightRope])
        {
            baseT.position += baseT.Up() * spd * Time.deltaTime;
        }
    }
}