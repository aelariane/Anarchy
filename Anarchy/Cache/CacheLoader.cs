using Cache.Assets;
using System.Collections;
using UnityEngine;

namespace Cache
{
    public class CacheLoader : MonoBehaviour
    {
        public CacheLoader()
        {
            Instance = this;
            StartCoroutine(Load());
        }

        public static CacheLoader Instance { get; private set; }

        private IEnumerator Load()
        {
            yield return StartCoroutine(AssetManager.LoadAll());
        }
    }
}
