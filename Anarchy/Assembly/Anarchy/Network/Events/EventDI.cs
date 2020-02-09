using Anarchy.UI;
using ExitGames.Client.Photon;
using Optimization.Caching;
using UnityEngine;

namespace Anarchy.Network.Events
{
    internal class EventDI : INetworkEvent
    {
        private object[] data = null;
        private int instID;
        private int[] instIDs = null;
        private int item;
        private string key = string.Empty;
        private Vector3 position = Vectors.zero;
        private short prefix;
        private Quaternion rotation = Quaternion.identity;
        private PhotonPlayer sender = null;
        private int timeStamp;

        public byte Code => 202;

        public EventDI()
        {
            NetworkManager.RegisterEvent(this);
        }

        public bool CheckData(EventData ev, PhotonPlayer snder, out string reason)
        {
            sender = snder;
            reason = string.Empty;
            Hashtable hash = ev[245] as Hashtable;
            if (hash == null)
            {
                reason += Log.GetString("notHashtable");
                return false;
            }
            if (hash.Count < 3 || hash.Count > 9)
            {
                reason += Log.GetString("invalidParamsCount", hash.Count.ToString());
                return false;
            }
            if (!CheckKey(hash, 0, out key))
            {
                reason += Log.GetString("missOrInvalidKeyArgd", "0", (key ?? "null"));
                return false;
            }
            snder.InstantiateSpam.Count(key);
            if (!CheckKey(hash, 6, out timeStamp))
            {
                reason += Log.GetString("missOrInvalidKey", "6");
                return false;
            }
            if (!CheckKey(hash, 7, out instID))
            {
                reason += Log.GetString("missOrInvalidKey", "7");
                return false;
            }
            //if(!Antis.CheckInstantiate(key, instID, sender, out reason))
            //{
            //    return false;
            //}
            if (!CheckKey(hash, 1, out position, true))
            {
                reason += Log.GetString("invalidKey", "1");
                return false;
            }
            if (!CheckKey(hash, 2, out rotation, true))
            {
                reason += Log.GetString("invalidKey", "2");
                return false;
            }
            if (!CheckKey(hash, 3, out item, true))
            {
                reason = Log.GetString("invalidKey", "3");
                return false;
            }
            if (!CheckKey(hash, 8, out prefix, true))
            {
                reason += Log.GetString("iInvalidKey", "8");
                return false;
            }
            if (!CheckKey(hash, 5, out data, true))
            {
                reason += Log.GetString("invalidKey", "5");
                return false;
            }
            if (hash.ContainsKey((byte)4))
            {
                if ((hash[(byte)4] is int[] numArr))
                    instIDs = numArr;
                else
                {
                    reason = Log.GetString("invalidKey", "4");
                    return false;
                }
            }
            else
                instIDs = new int[] { instID };
            return true;
        }

        private bool CheckKey<T>(Hashtable src, byte kkey, out T val, bool force = false)
        {
            if (src.ContainsKey(kkey))
            {
                if(src[kkey] is T idk)
                {
                    val = idk;
                    return val != null;
                }
                val = default(T);
                return false;
            }
            val = default(T);
            return force;
        }

        public bool Handle()
        {
            GameObject res = key.StartsWith("RCAsset/") ? CacheResources.RCLoad(key) : (GameObject)CacheResources.Load(key);
            if (res == null)
                return false;
            PhotonView[] photonViewsInChildren = res.GetPhotonViewsInChildren();
            if (photonViewsInChildren.Length != instIDs.Length)
            {
                throw new System.Exception("Error in Instantiation! The resource's PhotonView count is not the same as in incoming data.");
            }
            for (int i = 0; i < instIDs.Length; i++)
            {
                photonViewsInChildren[i].viewID = instIDs[i];
                photonViewsInChildren[i].prefix = prefix;
                photonViewsInChildren[i].instantiationId = instID;
            }
            PhotonNetwork.networkingPeer.StoreInstantiationData(instID, data);
            GameObject obj2 = (GameObject)UnityEngine.Object.Instantiate(res, position, rotation);
            for (int j = 0; j < instIDs.Length; j++)
            {
                photonViewsInChildren[j].viewID = 0;
                photonViewsInChildren[j].prefix = -1;
                photonViewsInChildren[j].prefixBackup = -1;
                photonViewsInChildren[j].instantiationId = -1;
            }
            PhotonNetwork.networkingPeer.RemoveInstantiationData(instID);
            if (PhotonNetwork.networkingPeer.instantiatedObjects.ContainsKey(instID))
            {
                GameObject go = PhotonNetwork.networkingPeer.instantiatedObjects[instID];
                string str2 = string.Empty;
                if (go != null)
                {
                    foreach (PhotonView view in go.GetPhotonViewsInChildren())
                    {
                        if (view != null)
                        {
                            str2 = str2 + view.ToString() + ", ";
                        }
                    }
                }
                object[] args = new object[] { obj2, instID, PhotonNetwork.networkingPeer.instantiatedObjects.Count, go, str2, PhotonNetwork.lastUsedViewSubId, PhotonNetwork.lastUsedViewSubIdStatic, NetworkingPeer.photonViewList.Count };
                Debug.LogError(string.Format("DoInstantiate re-defines a GameObject. Destroying old entry! New: '{0}' (instantiationID: {1}) Old: {3}. PhotonViews on old: {4}. instantiatedObjects.Count: {2}. PhotonNetwork.lastUsedViewSubId: {5} PhotonNetwork.lastUsedViewSubIdStatic: {6} photonViewList.Count {7}.)", args));
                PhotonNetwork.networkingPeer.RemoveInstantiatedGO(go, true);
            }
            PhotonNetwork.networkingPeer.instantiatedObjects.Add(instID, obj2);
            obj2.SendMessage(PhotonNetworkingMessage.OnPhotonInstantiate.ToString(), new PhotonMessageInfo(sender, timeStamp, null), SendMessageOptions.DontRequireReceiver);
            return true;
        }

        public void OnFailedHandle()
        {
            Log.AddLine("failedInstantiate", MsgType.Error, key);
        }

    }
}
