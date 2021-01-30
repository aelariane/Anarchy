using System.Collections.Generic;
using UnityEngine;

namespace Optimization.Caching
{
    internal class PoolObject
    {
        private int defaultCount;
        private Dictionary<string, List<GameObject>> cache = new Dictionary<string, List<GameObject>>();
        private string[] keys = null;

        public string[] Keys
        {
            get
            {
                string[] res = new string[keys.Length];
                for (int i = 0; i < keys.Length; i++)
                {
                    res[i] = keys[i];
                }
                return res;
            }
        }

        public PoolObject(int count)
        {
            defaultCount = count;
        }

        public PoolObject(string path, int count = 10) : this(count)
        {
            keys = System.IO.File.ReadAllLines(Application.dataPath + "/" + path);
        }

        public PoolObject(string[] names, int count = 10) : this(count)
        {
            keys = new string[names.Length];
            for (int i = 0; i < names.Length; i++)
            {
                keys[i] = names[i];
            }
        }

        public void Clear()
        {
            cache.Clear();
        }

        private GameObject CreateObject(string name)
        {
            Object resource = name.StartsWith("RCAsset/") ? CacheResources.RCLoad(name) : CacheResources.Load(name);
            if (resource == null)
            {
                throw new System.Exception($"PoolObject.CreateObject(): Cannot find Resource with name \"{name}\". Please check Keys for values.");
            }
            GameObject res = (GameObject)Object.Instantiate(resource);
            res.AddComponent<PoolableObject>();
            //Object.DontDestroyOnLoad(res);
            res.SetActive(false);
            return res;
        }

        public GameObject Enable(string name)
        {
            return Enable(name, Vectors.zero, Quaternion.identity);
        }

        public T Enable<T>(string name) where T : Component
        {
            return Enable<T>(name, Vectors.zero, Quaternion.identity);
        }

        public GameObject Enable(string name, Vector3 position, Quaternion rotation)
        {
            GameObject res = PickObject(name);
            if (res.transform != null)
            {
                res.transform.position = position;
                res.transform.rotation = rotation;
            }
            res.SetActive(true);
            return res;
        }

        public T Enable<T>(string name, Vector3 position, Quaternion rotation) where T : Component
        {
            GameObject go = PickObject(name);
            T result = go.GetComponent<T>();
            if (result == null)
            {
                throw new System.Exception($"PoolObject.Enable<{typeof(T).Name}>(): Required Component does not exists on template with name \"{name}\"");
            }
            if (go.transform != null)
            {
                go.transform.position = position;
                go.transform.rotation = rotation;
            }
            go.SetActive(true);
            return result;
        }

        private void Expand(string name, List<GameObject> list, int count)
        {
            lock (list)
            {
                for (int i = 0; i < count; i++)
                {
                    list.Add(CreateObject(name));
                }
            }
        }

        //public void Initialize(bool empty = false)
        //{
        //    cache = new Dictionary<string, List<GameObject>>();
        //    if (empty)
        //    {
        //        keys = new string[] { "" };
        //        return;
        //    }
        //    for (int i = 0; i < keys.Length; i++)
        //    {
        //        string currentKey = keys[i];
        //        List<GameObject> local = new List<GameObject>();
        //        Expand(currentKey, local, defaultCount);
        //        cache.Add(currentKey, local);
        //    }
        //}

        public GameObject NetworkEnable(string name, Vector3 position, Quaternion rotation, int group = 0, object[] data = null, bool isSceneObject = false)
        {
            if (IN_GAME_MAIN_CAMERA.GameType != GameType.MultiPlayer)
            {
                Debug.LogError($"PoolObject.NetworkEnable(): Failed to NetworkEnable prefab, because GameType is not Multiplayer.");
                return null;
            }
            GameObject go = PickObject(name);
            PhotonView pv = go.GetComponent<PhotonView>();
            if (pv == null)
            {
                throw new System.Exception($"PoolObject.NetworkEnable(): Prefab \"{name}\" has not PhotonView component.");
            }
            PhotonView[] photonViews = go.GetPhotonViewsInChildren();
            int[] viewIDs = new int[photonViews.Length];
            for (int i = 0; i < viewIDs.Length; i++)
            {
                viewIDs[i] = PhotonNetwork.AllocateViewID(PhotonNetwork.player.ID);
            }
            PhotonNetwork.networkingPeer.SendInstantiate(name, position, rotation, group, viewIDs, data, isSceneObject);
            return NetworkInstantiate(name, position, rotation, viewIDs[0], viewIDs, (short)(PhotonNetwork.networkingPeer.currentLevelPrefix > 0 ? PhotonNetwork.networkingPeer.currentLevelPrefix : 0), group, data);

            #region Old version

            //for (int i = 0; i < photonViews.Length; i++)
            //{
            //    photonViews[i].viewID = viewIDs[i];
            //    photonViews[i].prefix = prefix;
            //    photonViews[i].instantiationId = instantiationId;
            //}
            //if (go.transform != null)
            //{
            //    go.transform.position = position;
            //    go.transform.rotation = rotation;
            //}
            //peer.StoreInstantiationData(instantiationId, data);
            //go.SetActive(true);
            //peer.RemoveInstantiationData(instantiationId);
            //if (peer.instantiatedObjects.ContainsKey(instantiationId))
            //{
            //    GameObject gameobj = peer.instantiatedObjects[instantiationId];
            //    string str2 = string.Empty;
            //    if (gameobj != null)
            //    {
            //        foreach (PhotonView view in gameobj.GetPhotonViewsInChildren())
            //        {
            //            if (view != null)
            //            {
            //                str2 = str2 + view.ToString() + ", ";
            //            }
            //        }
            //    }
            //    object[] args = new object[] { gameobj, instantiationId, peer.instantiatedObjects.Count, go, str2, PhotonNetwork.lastUsedViewSubId, PhotonNetwork.lastUsedViewSubIdStatic, NetworkingPeer.photonViewList.Count };
            //    Debug.LogError(string.Format("DoInstantiate re-defines a GameObject. Destroying old entry! New: '{0}' (instantiationID: {1}) Old: {3}. PhotonViews on old: {4}. instantiatedObjects.Count: {2}. PhotonNetwork.lastUsedViewSubId: {5} PhotonNetwork.lastUsedViewSubIdStatic: {6} photonViewList.Count {7}.)", args));
            //    peer.RemoveInstantiatedGO(go, true);
            //}
            //peer.instantiatedObjects.Add(instantiationId, go);
            //return go;

            #endregion Old version
        }

        public GameObject NetworkInstantiate(string name, Vector3 position, Quaternion rotation, int instantioationId, int[] viewIDs, short prefix = 0, int group = 0, object[] data = null)
        {
            GameObject go = PickObject(name);
            if (!go.GetComponent<PhotonView>())
            {
                Debug.LogError($"PoolObject.NetworkInstantiate(): Prefab with name \"{name}\" has not PhotonView component!");
                return null;
            }
            PhotonView[] views = go.GetPhotonViewsInChildren();
            for (int i = 0; i < views.Length; i++)
            {
                views[i].didAwake = false;
                views[i].viewID = 0;
                views[i].prefix = prefix;
                views[i].instantiationId = instantioationId;
                views[i].instantiationData = data;
                views[i].didAwake = true;
                views[i].viewID = viewIDs[i];
            }
            if (go.transform)
            {
                go.transform.position = position;
                go.transform.rotation = rotation;
            }
            go.SetActive(true);
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

        private GameObject PickObject(string name)
        {
            if (!cache.TryGetValue(name, out List<GameObject> list))
            {
                List<GameObject> goList = new List<GameObject>();
                Expand(name, goList, 5);
                cache.Add(name, goList);
                list = cache[name];
            }
            GameObject result = null;
            lock (list)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    if (list[i].activeInHierarchy)
                    {
                        continue;
                    }

                    result = list[i];
                }
                if (result == null)
                {
                    Expand(name, list, 5);
                    result = PickObject(name);
                }
            }
            return result;
        }
    }
}