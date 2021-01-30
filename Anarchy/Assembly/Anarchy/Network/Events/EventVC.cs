using ExitGames.Client.Photon;
using System;

namespace Anarchy.Network.Events
{
    public class EventVC : INetworkEvent
    {
        public byte Code => 173;

        public EventVC()
        {
            NetworkManager.RegisterEvent(this);
        }

        public bool CheckData(EventData data, PhotonPlayer sender, out string reason)
        {
            reason = string.Empty;
            return true;
        }

        public bool Handle()
        {
            return true;
        }

        public void OnFailedHandle()
        {
            throw new NotImplementedException();
        }
    }
}