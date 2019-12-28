using UnityEngine;

public class RotateSample : MonoBehaviour
{
    private void Start()
    {
        iTween.RotateBy(base.gameObject, iTween.Hash(new object[]
        {
            "x",
            0.25,
            "easeType",
            "easeInOutBack",
            "loopType",
            "pingPong",
            "delay",
            0.4
        }));
    }
}