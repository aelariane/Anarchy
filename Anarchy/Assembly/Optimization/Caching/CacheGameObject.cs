using System.Collections.Generic;
using UnityEngine;

namespace Optimization.Caching
{
    internal static class CacheGameObject
    {
        private static Dictionary<string, GameObject> goCache = new Dictionary<string, GameObject>();

        private static Dictionary<string, Component> typeCache = new Dictionary<string, Component>();

        internal static T Find<T>(string name) where T : Component
        {
            typeCache.TryGetValue(name, out Component obj);
            if (obj != null) return obj as T;
            var go = Find(name);
            T value = go == null ? default(T) : go.GetComponent<T>();
            if (value != null)
            {
                if (typeCache.ContainsKey(name)) typeCache[name] = value;
                else typeCache.Add(name, value);
                return value;
            }
            return value;
        }

        internal static GameObject Find(string name)
        {
            switch (name.ToLower())
            {
                case "aottg_hero1":
                case "aottg_hero1(clone)":
                case "colossal_titan":
                case "door":
                case "femaletitan":
                case "female_titan":
                case "crawler":
                case "punk":
                case "abberant":
                case "jumper":
                case "titan":
                case "tree":
                case "cube001":
                    return GameObject.Find(name);

                default:
                    goCache.TryGetValue(name, out GameObject obj);
                    if (obj != null && obj.activeInHierarchy) return obj;
                    var go = GameObject.Find(name);
                    if (go == null) return null;
                    if (!goCache.ContainsKey(name))
                    {
                        goCache.Add(name, go);
                        return go;
                    }
                    return (goCache[name] = go);
            }
        }

        public static void ClearCache()
        {
            goCache = new Dictionary<string, GameObject>();
            typeCache = new Dictionary<string, Component>();
            goCache.Clear();
            typeCache.Clear();
        }

        //Old version
        //internal static T Find<T>(string name) where T : Component
        //{
        //    string key = name + typeof(T).Name;
        //    _typeCache.TryGetValue(key, out Component obj);
        //    if (obj != null) return obj as T;
        //    var go = Find(name);
        //    T value = go == null ? default(T) : go.GetComponent<T>();
        //    if (value != null)
        //    {
        //        if (_typeCache.ContainsKey(key)) _typeCache[key] = value;
        //        else _typeCache.Add(key, value);
        //        return value;
        //    }
        //    return value;
        //}
    }
}