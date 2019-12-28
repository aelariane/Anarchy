using Optimization.Caching;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XffectCache : MonoBehaviour
{
    private Dictionary<string, ArrayList> ObjectDic = new Dictionary<string, ArrayList>();

    private void Awake()
    {
        foreach (object obj in base.transform)
        {
            Transform transform = (Transform)obj;
            this.ObjectDic[transform.name] = new ArrayList();
            this.ObjectDic[transform.name].Add(transform);
            Xffect component = transform.GetComponent<Xffect>();
            if (component != null)
            {
                component.Initialize();
            }
            transform.gameObject.SetActive(false);
        }
    }

    protected Transform AddObject(string name)
    {
        Transform transform = base.transform.Find(name);
        if (transform == null)
        {
            Debug.Log("object:" + name + "doesn't exist!");
            return null;
        }
        Transform transform2 = UnityEngine.Object.Instantiate(transform, Vectors.zero, Quaternion.identity) as Transform;
        this.ObjectDic[name].Add(transform2);
        transform2.gameObject.SetActive(false);
        Xffect component = transform2.GetComponent<Xffect>();
        if (component != null)
        {
            component.Initialize();
        }
        return transform2;
    }

    public Transform GetObject(string name)
    {
        ArrayList arrayList = this.ObjectDic[name];
        if (arrayList == null)
        {
            Debug.LogError(name + ": cache doesnt exist!");
            return null;
        }
        foreach (object obj in arrayList)
        {
            Transform transform = (Transform)obj;
            if (!transform.gameObject.activeInHierarchy)
            {
                transform.gameObject.SetActive(false);
                return transform;
            }
        }
        return this.AddObject(name);
    }

    public ArrayList GetObjectCache(string name)
    {
        ArrayList arrayList = this.ObjectDic[name];
        if (arrayList == null)
        {
            Debug.LogError(name + ": cache doesnt exist!");
            return null;
        }
        return arrayList;
    }
}