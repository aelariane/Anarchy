using UnityEngine;

namespace Optimization.Caching
{
    internal static class Pool
    {
        //Change this to enable/disable object pooling
        public const bool UsePooling = true;

        public static bool Initialized = false;
        private static PoolObject Effects;
        private static PoolObject RC;

        public static void Create()
        {
            if (UsePooling)
            {
                Effects = new PoolObject(new string[]
                {
                    "bloodExplore", "bloodsplatter", "FX/bite", "FX/boom1",  "FX/boom1_CT_KICK",  "FX/boom2", "FX/boom2_eren", "FX/boom3", "FX/boom4", "FX/boom5", "FX/boom6", "FX/boost_smoke", "FX/colossal_steam",
                    "FX/colossal_steam_dmg", "FX/flareBullet1", "FX/flareBullet2", "FX/flareBullet3", "FX/FXtitanDie", "FX/FXtitanDie1", "FX/FXtitanSpawn", "FX/justSmoke", "FX/rockThrow", "FX/shotGun",  "FX/shotGun 1", "FX/Thunder", "FX/ThunderCT", "hitMeat", "hitMeat2",
                    "hitMeatBIG", "redCross", "redCross1", "titanNapeMeat"
                });
                RC = new PoolObject(new string[] { "RCAsset/BombMain", "RCAsset/BombExplodeMain" });
            }
            Initialized = true;
        }

        public static void Clear()
        {
            if (UsePooling)
            {
                if (Initialized)
                {
                    Effects.Clear();
                    RC.Clear();
                }
            }
        }

        public static void Disable(GameObject go)
        {
            if (UsePooling)
            {
                if (go != null)
                {
                    if (go.GetComponent<PoolableObject>() != null)
                    {
                        go.SetActive(false);
                        return;
                    }
                    Object.Destroy(go);
                }
            }
            else
            {
                Object.Destroy(go);
            }
        }

        public static GameObject Enable(string name)
        {
            return Enable(name, Vectors.zero, Quaternion.identity);
        }

        public static GameObject Enable(string name, Vector3 position, Quaternion rotation)
        {
            if (UsePooling)
            {
                if (!Initialized)
                {
                    return (GameObject)Object.Instantiate(CacheResources.Load(name), position, rotation);
                }
                if (name.StartsWith("FX/"))
                {
                    return Effects.Enable(name, position, rotation);
                }
                switch (name)
                {
                    case "bloodExplore":
                    case "bloodsplatter":
                    case "hitMeat":
                    case "hitMeat2":
                    case "hitMeatBIG":
                    case "redCross":
                    case "redCross1":
                    case "titanNapeMeat":
                        return Effects.Enable(name, position, rotation);

                    default:
                        return (GameObject)Object.Instantiate(CacheResources.Load(name), position, rotation);
                }
            }
            else
            {
                return (GameObject)Object.Instantiate(CacheResources.Load(name), position, rotation);
            }
        }

        public static GameObject NetworkEnable(string name, Vector3 position, Quaternion rotation, int group = 0)
        {
            if (UsePooling)
            {
                if (name.StartsWith("FX/"))
                {
                    return Effects.NetworkEnable(name, position, rotation);
                }
                switch (name)
                {
                    case "bloodExplore":
                    case "bloodsplatter":
                    case "hitMeat":
                    case "hitMeat2":
                    case "hitMeatBIG":
                    case "redCross":
                    case "redCross1":
                    case "titanNapeMeat":
                        return Effects.NetworkEnable(name, position, rotation, group);

                    case "RCAsset/BombMain":
                    case "RCAsset/BombExplodeMain":
                        return RC.NetworkEnable(name, position, rotation, group);

                    default:
                        return PhotonNetwork.Instantiate(name, position, rotation, group);
                }
            }
            else
            {
                return PhotonNetwork.Instantiate(name, position, rotation, group);
            }
        }

        public static GameObject NetworkInstantiate(string name, Vector3 position, Quaternion rotation, int instantioationId, int[] viewIDs, short prefix = 0, int group = 0, object[] data = null)
        {
            if (UsePooling)
            {
                if (name.StartsWith("FX/"))
                {
                    return Effects.NetworkInstantiate(name, position, rotation, instantioationId, viewIDs, prefix, group, data);
                }
                switch (name)
                {
                    case "bloodExplore":
                    case "bloodsplatter":
                    case "hitMeat":
                    case "hitMeat2":
                    case "hitMeatBIG":
                    case "redCross":
                    case "redCross1":
                    case "titanNapeMeat":
                        return Effects.NetworkInstantiate(name, position, rotation, instantioationId, viewIDs, prefix, group, data);

                    case "RCAsset/BombMain":
                    case "RCAsset/BombExplodeMain":
                        return RC.NetworkInstantiate(name, position, rotation, instantioationId, viewIDs, prefix, group, data);

                    default:
                        break;
                }
            }
            GameObject res = (name.StartsWith("RCAsset/") ? CacheResources.RCLoad(name) : CacheResources.Load(name)) as GameObject;
            if (res == null)
            {
                Debug.LogError($"Pool.NetworkInstantiate(): Cannot fint prefab with name \"{name}\".");
                return null;
            }
            PhotonView[] views = res.GetPhotonViewsInChildren();
            if (views.Length != viewIDs.Length)
            {
                throw new System.Exception($"Pool.NetworkInstantiate(): Error in Instantiation(\"{name}\")! The resource's PhotonView count is not the same as in incoming data. {views.Length} != {viewIDs.Length}");
            }
            for (int i = 0; i < views.Length; i++)
            {
                views[i].viewID = viewIDs[i];
                views[i].prefix = prefix;
                views[i].instantiationId = instantioationId;
            }
            PhotonNetwork.networkingPeer.StoreInstantiationData(instantioationId, data);
            GameObject go = (GameObject)Object.Instantiate(res, position, rotation);
            for (int i = 0; i < views.Length; i++)
            {
                views[i].viewID = 0;
                views[i].prefix = -1;
                views[i].instantiationId = -1;
                views[i].prefixBackup = -1;
            }
            PhotonNetwork.networkingPeer.RemoveInstantiationData(instantioationId);
            if (PhotonNetwork.networkingPeer.instantiatedObjects.ContainsKey(instantioationId))
            {
                GameObject gameobj = PhotonNetwork.networkingPeer.instantiatedObjects[instantioationId];
                string str2 = string.Empty;
                if (gameobj != null)
                {
                    foreach (PhotonView view in gameobj.GetPhotonViewsInChildren())
                    {
                        if (view != null)
                        {
                            str2 = str2 + view.ToString() + ", ";
                        }
                    }
                }
                object[] args = new object[] { gameobj, instantioationId, PhotonNetwork.networkingPeer.instantiatedObjects.Count, go, str2, PhotonNetwork.lastUsedViewSubId, PhotonNetwork.lastUsedViewSubIdStatic, NetworkingPeer.photonViewList.Count };
                Debug.LogError(string.Format("DoInstantiate re-defines a GameObject. Destroying old entry! New: '{0}' (instantiationID: {1}) Old: {3}. PhotonViews on old: {4}. instantiatedObjects.Count: {2}. PhotonNetwork.lastUsedViewSubId: {5} PhotonNetwork.lastUsedViewSubIdStatic: {6} photonViewList.Count {7}.)", args));
                PhotonNetwork.networkingPeer.RemoveInstantiatedGO(go, true);
            }
            PhotonNetwork.networkingPeer.instantiatedObjects.Add(instantioationId, go);
            return go;
        }
    }
}
