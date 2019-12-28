using System;
using ExitGames.Client.Photon;

namespace Anarchy.Network.Events
{
    public class EventPropertires : INetworkEvent
    {
        private Hashtable hash;
        private int key;
        private PhotonPlayer sender;


        public EventPropertires()
        {
            NetworkManager.RegisterEvent(this);
        }

        public byte Code => 253;

        public bool CheckData(EventData data, PhotonPlayer sender, out string reason)
        {
            reason = "";
            this.sender = sender;
            key = (int)data[0xfd];
            hash = data[0xfb] as Hashtable;
            if(hash == null)
            {
                reason += UI.Log.GetString("notHashOrNull");
                return false;
            }
            return true;
        }

        public bool Handle()
        {
            if (key == 0)
                PhotonNetwork.networkingPeer.ReadoutProperties(hash, null, -1);
                //Antis.ReadoutPropertiesRoom(hash, sender);
            else
            {
                PhotonNetwork.networkingPeer.ReadoutProperties(null, hash, key);
                //Antis.ReadoutProperties(hash, key, sender);
                //try
                //{
                //    Antis.ReadoutProperties(hash, key, sender);
                //}
                //catch
                //{
                //    FileLogger.AddLine("Exception: " + ExitGames.Client.Photon.SupportClass.DictionaryToString(hash) + "\nKey: " + key + "\nSender: " + sender);
                //}
            }
            return true;
        }

        public void OnFailedHandle() { }
    }
}
