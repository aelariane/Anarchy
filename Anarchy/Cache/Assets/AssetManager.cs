using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Cache.Assets
{
    public static class AssetManager
    {
        private static readonly Dictionary<AssetType, Asset> _assets;

        static AssetManager()
        {
            _assets = new Dictionary<AssetType, Asset>()
            {
                [AssetType.Skin] = new Asset("Skin"),
                [AssetType.General] = new Asset("Assets"),
                [AssetType.RC] = new Asset("RCAssets")
            };
        }

        public static bool AreAllAssetsLoaded()
        {
            return _assets.All(asset => asset.Value.IsLoaded);
        }

        public static bool IsLoaded(AssetType assetType)
        {
            return _assets.TryGetValue(assetType, out Asset asset) && asset.IsLoaded;
        }

        public static IEnumerator Load(AssetType assetType)
        {
            //Check if we find the asset in the asset cache.
            if (_assets.TryGetValue(assetType, out Asset asset))
            {
                yield return CacheLoader.Instance.StartCoroutine(asset.Load());
            }
        }

        public static IEnumerator LoadAll()
        {
            foreach (KeyValuePair<AssetType, Asset> asset in _assets)
            {
                //No need to check if it exists in the cache as it is directly grabbed from it.
                yield return CacheLoader.Instance.StartCoroutine(_assets[asset.Key].Load());
            }
        }

        public static Object LoadFromAsset(AssetType assetType, string objectName)
        {
            //Check if we find asset in cache and if we do, load the object.
            return _assets.TryGetValue(assetType, out Asset asset) ? asset.LoadFromBundle(objectName) : null;
        }
    }
}
