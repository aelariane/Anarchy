using UnityEngine;

namespace Optimization.Caching
{
    internal static class Pool
    {

        public static void Create()
        {
        }

        public static void Clear()
        {
        }

        public static void Disable(GameObject go)
        {
            Object.Destroy(go);
        }

        public static GameObject Enable(string name)
        {
            return Enable(name, Vectors.zero, Quaternion.identity);
        }

        public static GameObject Enable(string name, Vector3 position, Quaternion rotation)
        {
            var result = (GameObject)Object.Instantiate(CacheResources.Load(name), position, rotation);
            //if(result.rigidbody != null)
            //{
                //result.rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
            //}
            return result;
        }

        public static GameObject NetworkEnable(string name, Vector3 position, Quaternion rotation, int group = 0)
        {
            return PhotonNetwork.Instantiate(name, position, rotation, group);
        }

        public static GameObject NetworkInstantiate(string name, Vector3 position, Quaternion rotation, int instantioationId, int[] viewIDs, short prefix = 0, int group = 0, object[] data = null)
        {
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
