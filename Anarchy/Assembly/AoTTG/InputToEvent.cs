using UnityEngine;

public class InputToEvent : MonoBehaviour
{
    private GameObject lastGo;
    public static Vector3 inputHitPos;
    public bool DetectPointedAtGameObject;
    public static GameObject goPointedAt { get; private set; }

    private void Press(Vector2 screenPos)
    {
        this.lastGo = this.RaycastObject(screenPos);
        if (this.lastGo != null)
        {
            this.lastGo.SendMessage("OnPress", SendMessageOptions.DontRequireReceiver);
        }
    }

    private GameObject RaycastObject(Vector2 screenPos)
    {
        RaycastHit raycastHit;
        if (Physics.Raycast(base.camera.ScreenPointToRay(screenPos), out raycastHit, 200f))
        {
            InputToEvent.inputHitPos = raycastHit.point;
            return raycastHit.collider.gameObject;
        }
        return null;
    }

    private void Release(Vector2 screenPos)
    {
        if (this.lastGo != null)
        {
            GameObject x = this.RaycastObject(screenPos);
            if (x == this.lastGo)
            {
                this.lastGo.SendMessage("OnClick", SendMessageOptions.DontRequireReceiver);
            }
            this.lastGo.SendMessage("OnRelease", SendMessageOptions.DontRequireReceiver);
            this.lastGo = null;
        }
    }

    private void Update()
    {
        if (this.DetectPointedAtGameObject)
        {
            InputToEvent.goPointedAt = this.RaycastObject(Input.mousePosition);
        }
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                this.Press(touch.position);
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                this.Release(touch.position);
            }
            return;
        }
        if (Input.GetMouseButtonDown(0))
        {
            this.Press(Input.mousePosition);
        }
        if (Input.GetMouseButtonUp(0))
        {
            this.Release(Input.mousePosition);
        }
    }
}