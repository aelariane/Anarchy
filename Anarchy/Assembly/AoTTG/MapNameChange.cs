using Optimization.Caching;
using UnityEngine;

public class MapNameChange : MonoBehaviour
{
    private void OnSelectionChange()
    {
        LevelInfo info = LevelInfo.GetInfo(base.GetComponent<UIPopupList>().selection);
        if (info != null)
        {
            CacheGameObject.Find("LabelLevelInfo").GetComponent<UILabel>().text = info.Description;
        }
    }
}