using Anarchy.Configuration;
using Optimization.Caching;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace RC
{
    internal static class RCManager
    {
        public static readonly string CachePath = Application.dataPath + "/Resources/RCAssets.unity3d";
        public const string DownloadPath = "https://www.dropbox.com/s/13kg10qxy8u6sob/rcassets.unity3d?dl=1";

        public static Dictionary<int, CannonValues> allowedToCannon;
        public static AssetBundle Asset;
        public static ExitGames.Client.Photon.Hashtable boolVariables = new ExitGames.Client.Photon.Hashtable();
        public static ExitGames.Client.Photon.Hashtable floatVariables = new ExitGames.Client.Photon.Hashtable();

        //This is gametype. Change it in selection grid to switch between racing, killing, etc...
        public static IntSetting GameType = new IntSetting("customGameType", 3);

        public static ExitGames.Client.Photon.Hashtable heroHash = new ExitGames.Client.Photon.Hashtable();
        public static ExitGames.Client.Photon.Hashtable intVariables = new ExitGames.Client.Photon.Hashtable();
        public static bool Loaded { get; private set; } = false;
        public static ExitGames.Client.Photon.Hashtable playerVariables = new ExitGames.Client.Photon.Hashtable();
        public static ExitGames.Client.Photon.Hashtable stringVariables = new ExitGames.Client.Photon.Hashtable();
        public static ExitGames.Client.Photon.Hashtable titanVariables = new ExitGames.Client.Photon.Hashtable();
        public static Vector3 racingSpawnPoint = Vectors.zero;
        public static Quaternion racingSpawnPointRotation;
        public static bool racingSpawnPointSet;
        public static ExitGames.Client.Photon.Hashtable RCEvents = new ExitGames.Client.Photon.Hashtable();
        public static ExitGames.Client.Photon.Hashtable RCRegions = new ExitGames.Client.Photon.Hashtable();
        public static ExitGames.Client.Photon.Hashtable RCRegionTriggers = new ExitGames.Client.Photon.Hashtable();
        public static ExitGames.Client.Photon.Hashtable RCVariableNames = new ExitGames.Client.Photon.Hashtable();

        //Also changable variable. Use it to change titans amount on a custom map
        public static Anarchy.Configuration.IntSetting SpawnCapCustom = new Anarchy.Configuration.IntSetting("titanSpawnCap", 1);

        public static void ClearAll()
        {
            ClearVariables();
            heroHash.Clear();
            RCEvents.Clear();
            RCRegions.Clear();
            RCRegionTriggers.Clear();
            RCVariableNames.Clear();
        }

        public static void ClearVariables()
        {
            boolVariables.Clear();
            floatVariables.Clear();
            intVariables.Clear();
            playerVariables.Clear();
            stringVariables.Clear();
            titanVariables.Clear();
        }

        public static IEnumerator DownloadAssets()
        {
            if (Loaded)
            {
                yield break;
            }
            if (File.Exists(CachePath))
            {
                var req = AssetBundle.CreateFromMemory(File.ReadAllBytes(CachePath));
                yield return req;
                if (req == null || req.assetBundle == null) { }
                else
                {
                    Asset = req.assetBundle;
                    Loaded = true;
                    yield break;
                }
            }
            WWW www = new WWW(DownloadPath);
            yield return www;
            if (www.assetBundle != null)
            {
                Asset = www.assetBundle;
                File.WriteAllBytes(CachePath, www.bytes);
                Loaded = true;
            }
        }

        public static GameObject Instantiate(string name)
        {
            return CacheResources.RCLoad(name);
        }

        public static UnityEngine.Object Load(string res)
        {
            return Asset.Load(res);
        }
    }
}