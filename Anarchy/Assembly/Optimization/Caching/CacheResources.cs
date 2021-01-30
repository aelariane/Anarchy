using System.Collections.Generic;
using UnityEngine;

namespace Optimization.Caching
{
    internal static class CacheResources
    {
        private static Dictionary<string, Object> cache = new Dictionary<string, Object>();
        private static Dictionary<string, GameObject> cacheRC = new Dictionary<string, GameObject>();
        private static Dictionary<string, Component> cacheType = new Dictionary<string, Component>();

        internal static void ClearCache()
        {
            cache = new Dictionary<string, Object>();
            cache.Clear();
            cacheType = new Dictionary<string, Component>();
            cacheType.Clear();
        }

        internal static Object Load(string path)
        {
            cache.TryGetValue(path, out Object obj);
            if (obj != null)
            {
                return obj;
            }

            if (cache.ContainsKey(path))
            {
                cache[path] = Resources.Load(path);
            }
            else
            {
                cache.Add(path, Resources.Load(path));
            }

            return cache[path];
        }

        internal static T Load<T>(string path) where T : Component
        {
            cacheType.TryGetValue(path, out Component obj);
            if (obj != null)
            {
                return obj as T;
            }

            var go = (GameObject)Load(path);
            if (go.GetComponent<T>() != null)
            {
                if (cacheType.ContainsKey(path))
                {
                    cacheType[path] = go.GetComponent<T>();
                }
                else
                {
                    cacheType.Add(path, go.GetComponent<T>());
                }

                return go.GetComponent<T>();
            }
            return default(T);
        }

        public static GameObject RCLoad(string _name)
        {
            string name = _name.StartsWith("RCAsset/") ? _name.Remove(0, 8) : _name;
            if (!cacheRC.ContainsKey(name))
            {
                return cacheRC[name] = (GameObject)RC.RCManager.Asset.Load(name);
            }
            return cacheRC[name];
        }
    }
}