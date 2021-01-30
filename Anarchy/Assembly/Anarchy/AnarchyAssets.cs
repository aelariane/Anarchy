using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Anarchy
{
    public static class AnarchyAssets
    {
        //TODO: Updating assets & downloading them

        public const string AssetVersion = "1.0.0.0";
        public const string DownloadPath = "https://www.dropbox.com/s/t5cbgch4n12p2cy/anarchyassets.unity3d?dl=1";
        public const string UpdatePath = "https://www.dropbox.com/s/fsy5g998t6f0r2p/assetversion.txt?dl=1";

        private static readonly Dictionary<string, Object> cache = new Dictionary<string, Object>();

        public static readonly string BundlePath = Application.dataPath + "/Resources/AnarchyAssets.unity3d";

        public static readonly string[] FontNames = new string[]
        {
            "Consola-Bold", "Consola-BoldItalic", "Consola-Italic", "Consola-Regular",
            "Hack-Bold", "Hack-BoldItalic", "Hack-Italic", "Hack-Regular",
            "FantasqueSans-Bold", "FantasqueSans-BoldItalic", "FantasqueSans-Italic", "FantasqueSans-Regular",
            "Snowstorm-Black", "Snowstorm-Bold", "Snowstorm-Inline", "Snowstorm-Kraft", "Snowstorm-Light",
            "Snowstorm-Regular",
            "Mono-Bold", "Mono-BoldItalic", "Mono-Italic", "Mono-Regular",
            "Inconsolata", "Mandatory", "Prototype", "Russian", "Tahoma", "WhiteRabbit"
        };

        private static AssetBundle Bundle { get; set; }

        public static Object Load(string name)
        {
            if (Bundle == null)
            {
                Debug.Log("AnarchyAssets bundle is null");
                return null;
            }

            if (cache.TryGetValue(name, out Object result) && result != null)
            {
                return result;
            }

            result = Bundle.Load(name);
            if (result != null)
            {
                cache.Add(name, result);
            }

            return result;
        }

        public static T Load<T>(string name) where T : Object
        {
            return Load(name) as T;
        }

        public static System.Collections.IEnumerator LoadAssetBundle()
        {
            AssetBundleCreateRequest bundle = AssetBundle.CreateFromMemory(File.ReadAllBytes(BundlePath));
            yield return bundle;
            if (bundle == null)
            {
                Debug.LogError($"Error while loading AnarchyAssets. Make sure that file \"{BundlePath}\" exists.");
                yield break;
            }

            Bundle = bundle.assetBundle;
        }
    }
}