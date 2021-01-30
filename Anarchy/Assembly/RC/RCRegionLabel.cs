using UnityEngine;

public class RCRegionLabel : MonoBehaviour
{
    public GameObject myLabel;
    private UILabel label;

    private void Awake()
    {
        label = myLabel.GetComponent<UILabel>();
    }

    private void Update()
    {
        if (label != null && label.isVisible)
        {
            this.myLabel.transform.LookAt(2f * this.myLabel.transform.position - IN_GAME_MAIN_CAMERA.MainT.position);
        }
    }
}