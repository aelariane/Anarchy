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
            key = (int)data[ParameterCode.TargetActorNr];
            hash = data[ParameterCode.Properties] as Hashtable;
            if (hash == null)
            {
                reason += UI.Log.GetString("notHashOrNull");
                return false;
            }
            return true;
        }

        public bool Handle()
        {
            if (key == 0)
            {
                PropertiesChecker.CheckRoomProperties(hash, sender);
            }
            else
            {
                PropertiesChecker.CheckPlayersProperties(hash, key, sender);
            }
            return true;
        }

        public void OnFailedHandle()
        {
        }
    }
}