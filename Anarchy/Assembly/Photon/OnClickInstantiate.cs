using UnityEngine;

public class OnClickInstantiate : MonoBehaviour
{
    private string[] InstantiateTypeNames = new string[]
        {
        "Mine",
        "Scene"
    };

    public int InstantiateType;
    public GameObject Prefab;
    public bool showGui;

    private void OnClick()
    {
        if (PhotonNetwork.connectionStateDetailed != PeerState.Joined)
        {
            return;
        }
        int instantiateType = this.InstantiateType;
        if (instantiateType != 0)
        {
            if (instantiateType == 1)
            {
                PhotonNetwork.InstantiateSceneObject(this.Prefab.name, InputToEvent.inputHitPos + new Vector3(0f, 5f, 0f), Quaternion.identity, 0, null);
            }
        }
        else
        {
            Optimization.Caching.Pool.NetworkEnable(this.Prefab.name, InputToEvent.inputHitPos + new Vector3(0f, 5f, 0f), Quaternion.identity, 0);
        }
    }

    private void OnGUI()
    {
        if (this.showGui)
        {
            GUILayout.BeginArea(new Rect((float)(Screen.width - 180), 0f, 180f, 50f));
            this.InstantiateType = GUILayout.Toolbar(this.InstantiateType, this.InstantiateTypeNames, new GUILayoutOption[0]);
            GUILayout.EndArea();
        }
    }
}