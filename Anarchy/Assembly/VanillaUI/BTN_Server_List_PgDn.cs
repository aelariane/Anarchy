using Optimization.Caching;
using UnityEngine;

public class BTN_Server_List_PgDn : MonoBehaviour
{
    private void OnClick()
    {
        CacheGameObject.Find("PanelMultiROOM").GetComponent<PanelMultiJoin>().pageDown();
    }
}