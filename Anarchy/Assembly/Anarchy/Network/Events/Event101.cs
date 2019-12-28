using ExitGames.Client.Photon;

namespace Anarchy.Network.Events
{
    public class Event101 : INetworkEvent
    {
        public Event101()
        {
            NetworkManager.RegisterEvent(this);
        }

        public byte Code => 101;

        public bool CheckData(EventData data, PhotonPlayer sender, out string reason)
        {
            reason = "";
            return true;
        }

        public bool Handle()
        {
            return true;
        }

        public void OnFailedHandle()
        {
        }
    }
}
