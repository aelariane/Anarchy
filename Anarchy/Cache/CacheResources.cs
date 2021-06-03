using Cache.Assets;
using System.Collections.Generic;
using UnityEngine;

namespace Cache
{
    public static class CacheResources
    {
        private static readonly Dictionary<string, Object> _cache;
        private static readonly Dictionary<string, GameObject> _cacheRC;
        private static readonly Dictionary<string, Component> _cacheType;

        static CacheResources()
        {
            _cache = new Dictionary<string, Object>();
            _cacheRC = new Dictionary<string, GameObject>();
            _cacheType = new Dictionary<string, Component>();
        }

        public static void ClearCache()
        {
            _cache.Clear();
            _cacheRC.Clear();
            _cacheType.Clear();
        }

        public static Object Load(string path)
        {
            _cache.TryGetValue(path, out Object obj);
            
            if (obj != null)
            { 
                return obj;
            }
            
            if (_cache.ContainsKey(path))
            {
                _cache[path] = Resources.Load(path);
            }
            else
            {
                _cache.Add(path, Resources.Load(path));
            }
            return _cache[path];
        }

        public static T Load<T>(string path) where T : Component
        {
            _cacheType.TryGetValue(path, out Component obj);
            if (obj != null)
            {
                return obj as T;
            }
            GameObject go = (GameObject)Load(path);
            
            if (go.GetComponent<T>() == null)
            {
                return default;
            }

            if (_cacheType.ContainsKey(path))
            {
                _cacheType[path] = go.GetComponent<T>();
            }
            else
            {
                _cacheType.Add(path, go.GetComponent<T>());
            }
            return go.GetComponent<T>();
        }

        public static GameObject RCLoad(string name)
        {
            name = name.StartsWith("RCAsset/") ? name.Remove(0, 8) : name;
            if (!_cacheRC.ContainsKey(name))
            {
                return _cacheRC[name] = (GameObject)AssetManager.LoadFromAsset(AssetType.RC, name);
            }
            return _cacheRC[name];
        }
    }
}
