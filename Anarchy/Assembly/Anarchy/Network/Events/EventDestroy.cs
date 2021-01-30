using ExitGames.Client.Photon;
using UnityEngine;

namespace Anarchy.Network.Events
{
    public class EventDestroy : INetworkEvent
    {
        private int key;

        public EventDestroy()
        {
            NetworkManager.RegisterEvent(this);
        }

        public byte Code => 204;

        public bool CheckData(EventData data, PhotonPlayer sender, out string reason)
        {
            reason = "";
            if (sender == null)
            {
                return false;
            }
            Hashtable hash = data[245] as Hashtable;
            if (hash == null)
            {
                reason = UI.Log.GetString("notHashOrNull");
                return false;
            }
            if (!hash.ContainsKey((byte)0))
            {
                reason = UI.Log.GetString("missingKey", "0");
                return false;
            }
            if (hash[(byte)0] is int mkey)
            {
                key = mkey;
                return true;
            }
            reason = UI.Log.GetString("invalidKey", "0");
            return false;
        }

        public bool Handle()
        {
            PhotonNetwork.networkingPeer.instantiatedObjects.TryGetValue(key, out GameObject obj2);
            if (obj2 != null)
            {
                PhotonNetwork.networkingPeer.RemoveInstantiatedGO(obj2, true);
            }
            return true;
        }

        public void OnFailedHandle()
        {
        }
    }
}