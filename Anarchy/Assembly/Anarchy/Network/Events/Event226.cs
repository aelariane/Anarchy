using ExitGames.Client.Photon;
using System;

namespace Anarchy.Network.Events
{
    public class Event226 : INetworkEvent
    {
        public Event226()
        {
            NetworkManager.RegisterEvent(this);
        }

        public byte Code => 226;

        public bool CheckData(EventData data, PhotonPlayer sender, out string reason)
        {
            if (sender != null)
            {
                reason = UI.Log.GetString("senderMustBeNull");
                return false;
            }
            PhotonNetwork.networkingPeer.mPlayersInRoomsCount = (int)data[0xe5];
            PhotonNetwork.networkingPeer.mPlayersOnMasterCount = (int)data[0xe3];
            PhotonNetwork.networkingPeer.mGameCount = (int)data[0xe4];
            reason = "";
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