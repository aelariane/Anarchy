using UnityEngine;

public class ParentFollow : MonoBehaviour
{
    private void Awake()
    {
        this.bTransform = base.transform;
        this.isActiveInScene = true;
    }

    public void RemoveParent()
    {
        this.parent = null;
    }

    public void SetParent(Transform transform)
    {
        this.parent = transform;
        this.bTransform.rotation = transform.rotation;
    }

    private void Update()
    {
        bool flag = this.isActiveInScene && this.parent != null;
        if (flag)
        {
            this.bTransform.position = this.parent.position;
        }
    }

    private Transform bTransform;

    public bool isActiveInScene;

    private Transform parent;
}