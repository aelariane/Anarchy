using System.Collections.Generic;
using UnityEngine;

namespace Optimization.Caching
{
    internal static class SpriteCache
    {
        private static Dictionary<string, UISprite> cache = new Dictionary<string, UISprite>();

        public static UISprite Find(string name)
        {
            if (cache.TryGetValue(name, out UISprite res) && res != null)
            {
                return res;
            }
            GameObject go = GameObject.Find(name);
            if (go != null)
            {
                UISprite sprite = go.GetComponent<UISprite>();
                if (sprite != null)
                {
                    if (cache.ContainsKey(name))
                    {
                        cache[name] = sprite;
                    }
                    else
                    {
                        cache.Add(name, sprite);
                    }
                    return sprite;
                }
            }
            return null;
        }
    }
}