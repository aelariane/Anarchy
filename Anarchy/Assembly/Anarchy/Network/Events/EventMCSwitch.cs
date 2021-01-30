using ExitGames.Client.Photon;

namespace Anarchy.Network.Events
{
    public class EventMCSwitch : INetworkEvent
    {
        private int key;
        public byte Code => 208;

        public EventMCSwitch()
        {
            NetworkManager.RegisterEvent(this);
        }

        public bool CheckData(EventData data, PhotonPlayer sender, out string reason)
        {
            reason = "";
            if (PhotonNetwork.IsMasterClient)
            {
                reason += UI.Log.GetString("mcSteal");
                return false;
            }
            Hashtable hash = data[245] as Hashtable;
            if (hash == null)
            {
                reason += UI.Log.GetString("notHashOrNull");
                return false;
            }
            if (hash.Count != 1)
            {
                reason += UI.Log.GetString("invalidParamsCount", hash.Count.ToString());
                return false;
            }
            if (!hash.ContainsKey((byte)1) || !(hash[(byte)1] is int idk))
            {
                reason += UI.Log.GetString("missOrInvalidKey", "1");
                return false;
            }
            key = idk;
            return true;
        }

        public bool Handle()
        {
            PhotonNetwork.networkingPeer.SetMasterClient(key, false);
            return true;
        }

        public void OnFailedHandle()
        {
        }
    }
}