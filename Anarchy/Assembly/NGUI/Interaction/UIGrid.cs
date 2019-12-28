using System;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("NGUI/Interaction/Grid")]
public class UIGrid : MonoBehaviour
{
    private bool mStarted;
    public UIGrid.Arrangement arrangement;

    public float cellHeight = 200f;
    public float cellWidth = 200f;
    public bool hideInactive = true;
    public int maxPerLine;
    public bool repositionNow;

    public bool sorted;

    public enum Arrangement
    {
        Horizontal,
        Vertical
    }

    private void Start()
    {
        this.mStarted = true;
        this.Reposition();
    }

    private void Update()
    {
        if (this.repositionNow)
        {
            this.repositionNow = false;
            this.Reposition();
        }
    }

    public static int SortByName(Transform a, Transform b)
    {
        return string.Compare(a.name, b.name);
    }

    public void Reposition()
    {
        if (!this.mStarted)
        {
            this.repositionNow = true;
            return;
        }
        Transform transform = base.transform;
        int num = 0;
        int num2 = 0;
        if (this.sorted)
        {
            List<Transform> list = new List<Transform>();
            for (int i = 0; i < transform.childCount; i++)
            {
                Transform child = transform.GetChild(i);
                if (child && (!this.hideInactive || NGUITools.GetActive(child.gameObject)))
                {
                    list.Add(child);
                }
            }
            list.Sort(new Comparison<Transform>(UIGrid.SortByName));
            int j = 0;
            int count = list.Count;
            while (j < count)
            {
                Transform transform2 = list[j];
                if (NGUITools.GetActive(transform2.gameObject) || !this.hideInactive)
                {
                    float z = transform2.localPosition.z;
                    transform2.localPosition = ((this.arrangement != UIGrid.Arrangement.Horizontal) ? new Vector3(this.cellWidth * (float)num2, -this.cellHeight * (float)num, z) : new Vector3(this.cellWidth * (float)num, -this.cellHeight * (float)num2, z));
                    if (++num >= this.maxPerLine && this.maxPerLine > 0)
                    {
                        num = 0;
                        num2++;
                    }
                }
                j++;
            }
        }
        else
        {
            for (int k = 0; k < transform.childCount; k++)
            {
                Transform child2 = transform.GetChild(k);
                if (NGUITools.GetActive(child2.gameObject) || !this.hideInactive)
                {
                    float z2 = child2.localPosition.z;
                    child2.localPosition = ((this.arrangement != UIGrid.Arrangement.Horizontal) ? new Vector3(this.cellWidth * (float)num2, -this.cellHeight * (float)num, z2) : new Vector3(this.cellWidth * (float)num, -this.cellHeight * (float)num2, z2));
                    if (++num >= this.maxPerLine && this.maxPerLine > 0)
                    {
                        num = 0;
                        num2++;
                    }
                }
            }
        }
        UIDraggablePanel uidraggablePanel = NGUITools.FindInParents<UIDraggablePanel>(base.gameObject);
        if (uidraggablePanel != null)
        {
            uidraggablePanel.UpdateScrollbars(true);
        }
    }
}