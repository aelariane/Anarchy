using ExitGames.Client.Photon;

namespace Anarchy.Network.Events
{
    public class EventCloseConnection : INetworkEvent
    {
        public byte Code => 203;

        public EventCloseConnection()
        {
            NetworkManager.RegisterEvent(this);
        }

        public bool CheckData(EventData data, PhotonPlayer sender, out string reason)
        {
            reason = "";
            if (sender == null)
            {
                return false;
            }
            if (!sender.IsMasterClient)
            {
                reason = UI.Log.GetString("notMC");
                return false;
            }
            return true;
        }

        public bool Handle()
        {
            PhotonNetwork.LeaveRoom();
            return true;
        }

        public void OnFailedHandle()
        {
        }
    }
}
