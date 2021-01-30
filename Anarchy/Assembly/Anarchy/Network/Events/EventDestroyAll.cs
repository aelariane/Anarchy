using ExitGames.Client.Photon;

namespace Anarchy.Network.Events
{
    public class EventDestroyAll : INetworkEvent
    {
        private int key;

        public byte Code => 207;

        public EventDestroyAll()
        {
            NetworkManager.RegisterEvent(this);
        }

        public bool CheckData(EventData data, PhotonPlayer sender, out string reason)
        {
            reason = "";

            //Commented due to PedoRC mod triggering this
            //if (!sender.IsMasterClient)
            //{
            //    reason += UI.Log.GetString("notMC");
            //    return false;
            //}
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
            if (!hash.ContainsKey((byte)0) || !(hash[(byte)0] is int keyi))
            {
                reason += UI.Log.GetString("missOrInvalidKey", "0");
                return false;
            }
            key = keyi;
            return true;
        }

        public bool Handle()
        {
            if (key < 0)
            {
                PhotonNetwork.networkingPeer.DestroyAll(true);
            }
            else
            {
                PhotonNetwork.networkingPeer.DestroyPlayerObjects(key, true);
            }

            return true;
        }

        public void OnFailedHandle()
        {
        }
    }
}