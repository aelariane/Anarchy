using UnityEngine;

public class MoveSample : MonoBehaviour
{
    private void Start()
    {
        iTween.MoveBy(base.gameObject, iTween.Hash(new object[]
        {
            "x",
            2,
            "easeType",
            "easeInOutExpo",
            "loopType",
            "pingPong",
            "delay",
            0.1
        }));
    }
}