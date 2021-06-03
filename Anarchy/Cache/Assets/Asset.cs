using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Cache.Assets
{
    public class Asset
    {
        private readonly string _path;
        private readonly Dictionary<string, Object> _cache;

        public Asset(string name)
        {
            _path = Application.dataPath + "/Assets/" + name + ".unity3d";
            _cache = new Dictionary<string, Object>();
        }

        private AssetBundle Bundle { get; set; }

        public bool IsLoaded { get; private set; }

        public Object LoadFromBundle(string name)
        {
            if (!IsLoaded)
            {
                throw new Exception($"Can't load element from {GetType().Name} if it is not loaded.");
            }

            if (Bundle == null)
            {
                throw new Exception($"AssetBundle in {GetType().Name} is null.");
            }

            //If we find the asset in the cache.
            if (_cache.TryGetValue(name, out Object result) && result != null)
            {
                return result;
            }

            //If we don't, hard load it and cache it for next access.
            result = Bundle.Load(name);
            if (result != null)
            {
                _cache.Add(name, result);
            }

            return result;
        }

        public IEnumerator Load()
        {
            if (IsLoaded)
            {
                yield break;
            }

            //Create the bundle from the path.
            AssetBundleCreateRequest bundle = AssetBundle.CreateFromMemory(File.ReadAllBytes(_path));
            yield return bundle;

            //Check if we found it.
            if (bundle == null)
            {
                throw new Exception($"Could not load bundle from file, check if {_path} exists.");
            }

            //If we did, load it.
            Bundle = bundle.assetBundle;
            IsLoaded = true;
        }
    }
}
