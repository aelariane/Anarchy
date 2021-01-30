using System.Collections.Generic;
using UnityEngine;

public static class ClothFactory
{
    public static void ClearClothCache()
    {
        ClothFactory.clothCache.Clear();
    }

    public static void DisposeObject(GameObject cachedObject)
    {
        bool flag = cachedObject != null;
        if (flag)
        {
            ParentFollow component = cachedObject.GetComponent<ParentFollow>();
            bool flag2 = component != null;
            if (flag2)
            {
                bool isActiveInScene = component.isActiveInScene;
                if (isActiveInScene)
                {
                    component.isActiveInScene = false;
                    cachedObject.transform.position = new Vector3(0f, -99999f, 0f);
                    cachedObject.GetComponent<ParentFollow>().RemoveParent();
                }
            }
            else
            {
                UnityEngine.Object.Destroy(cachedObject);
            }
        }
    }

    private static GameObject GenerateCloth(GameObject go, string res)
    {
        bool flag = go.GetComponent<SkinnedMeshRenderer>() == null;
        if (flag)
        {
            go.AddComponent<SkinnedMeshRenderer>();
        }
        Transform[] bones = go.GetComponent<SkinnedMeshRenderer>().bones;
        SkinnedMeshRenderer component = ((GameObject)UnityEngine.Object.Instantiate(Resources.Load(res))).GetComponent<SkinnedMeshRenderer>();
        component.transform.localScale = Vector3.one;
        component.bones = bones;
        component.quality = SkinQuality.Bone4;
        return component.gameObject;
    }

    public static GameObject GetCape(GameObject reference, string name, Material material)
    {
        List<GameObject> list;
        bool flag = ClothFactory.clothCache.TryGetValue(name, out list);
        GameObject result;
        if (flag)
        {
            for (int i = 0; i < list.Count; i++)
            {
                GameObject gameObject = list[i];
                bool flag2 = gameObject == null;
                if (flag2)
                {
                    list.RemoveAt(i);
                    i = Mathf.Max(i - 1, 0);
                }
                else
                {
                    ParentFollow component = gameObject.GetComponent<ParentFollow>();
                    bool flag3 = !component.isActiveInScene;
                    if (flag3)
                    {
                        component.isActiveInScene = true;
                        gameObject.renderer.material = material;
                        gameObject.GetComponent<Cloth>().enabled = true;
                        gameObject.GetComponent<SkinnedMeshRenderer>().enabled = true;
                        gameObject.GetComponent<ParentFollow>().SetParent(reference.transform);
                        ClothFactory.ReapplyClothBones(reference, gameObject);
                        return gameObject;
                    }
                }
            }
            GameObject gameObject2 = ClothFactory.GenerateCloth(reference, name);
            gameObject2.renderer.material = material;
            gameObject2.AddComponent<ParentFollow>().SetParent(reference.transform);
            list.Add(gameObject2);
            ClothFactory.clothCache[name] = list;
            result = gameObject2;
        }
        else
        {
            GameObject gameObject2 = ClothFactory.GenerateCloth(reference, name);
            gameObject2.renderer.material = material;
            gameObject2.AddComponent<ParentFollow>().SetParent(reference.transform);
            list = new List<GameObject>
            {
                gameObject2
            };
            ClothFactory.clothCache.Add(name, list);
            result = gameObject2;
        }
        return result;
    }

    public static string GetDebugInfo()
    {
        int num = 0;
        foreach (KeyValuePair<string, List<GameObject>> keyValuePair in ClothFactory.clothCache)
        {
            num += ClothFactory.clothCache[keyValuePair.Key].Count;
        }
        int num2 = 0;
        foreach (Cloth cloth in UnityEngine.Object.FindObjectsOfType<Cloth>())
        {
            bool enabled = cloth.enabled;
            if (enabled)
            {
                num2++;
            }
        }
        return string.Format("{0} cached cloths, {1} active cloths, {2} types cached", num, num2, ClothFactory.clothCache.Keys.Count);
    }

    public static GameObject GetHair(GameObject reference, string name, Material material, Color color)
    {
        List<GameObject> list;
        bool flag = ClothFactory.clothCache.TryGetValue(name, out list);
        GameObject result;
        if (flag)
        {
            for (int i = 0; i < list.Count; i++)
            {
                GameObject gameObject = list[i];
                bool flag2 = gameObject == null;
                if (flag2)
                {
                    Debug.Log("Hair is null");
                    list.RemoveAt(i);
                    i = Mathf.Max(i - 1, 0);
                }
                else
                {
                    ParentFollow component = gameObject.GetComponent<ParentFollow>();
                    bool flag3 = !component.isActiveInScene;
                    if (flag3)
                    {
                        component.isActiveInScene = true;
                        gameObject.renderer.material = material;
                        gameObject.renderer.material.color = color;
                        gameObject.GetComponent<Cloth>().enabled = true;
                        gameObject.GetComponent<SkinnedMeshRenderer>().enabled = true;
                        gameObject.GetComponent<ParentFollow>().SetParent(reference.transform);
                        ClothFactory.ReapplyClothBones(reference, gameObject);
                        return gameObject;
                    }
                }
            }
            GameObject gameObject2 = ClothFactory.GenerateCloth(reference, name);
            gameObject2.renderer.material = material;
            gameObject2.renderer.material.color = color;
            gameObject2.AddComponent<ParentFollow>().SetParent(reference.transform);
            list.Add(gameObject2);
            ClothFactory.clothCache[name] = list;
            result = gameObject2;
        }
        else
        {
            GameObject gameObject2 = ClothFactory.GenerateCloth(reference, name);
            gameObject2.renderer.material = material;
            gameObject2.renderer.material.color = color;
            gameObject2.AddComponent<ParentFollow>().SetParent(reference.transform);
            list = new List<GameObject>
            {
                gameObject2
            };
            ClothFactory.clothCache.Add(name, list);
            result = gameObject2;
        }
        return result;
    }

    private static void ReapplyClothBones(GameObject reference, GameObject clothObject)
    {
        SkinnedMeshRenderer component = reference.GetComponent<SkinnedMeshRenderer>();
        SkinnedMeshRenderer component2 = clothObject.GetComponent<SkinnedMeshRenderer>();
        component2.bones = component.bones;
        component2.transform.localScale = Vector3.one;
    }

    private static Dictionary<string, List<GameObject>> clothCache = new Dictionary<string, List<GameObject>>(CostumeHair.hairsF.Length);
}