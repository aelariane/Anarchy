using UnityEngine;

public class OnClickLoadSomething : MonoBehaviour
{
    public string ResourceToLoad;
    public OnClickLoadSomething.ResourceTypeOption ResourceTypeToLoad;

    public enum ResourceTypeOption : byte
    {
        Scene,
        Web
    }

    public void OnClick()
    {
        OnClickLoadSomething.ResourceTypeOption resourceTypeToLoad = this.ResourceTypeToLoad;
        if (resourceTypeToLoad != OnClickLoadSomething.ResourceTypeOption.Scene)
        {
            if (resourceTypeToLoad == OnClickLoadSomething.ResourceTypeOption.Web)
            {
                Application.OpenURL(this.ResourceToLoad);
            }
        }
        else
        {
            Application.LoadLevel(this.ResourceToLoad);
        }
    }
}