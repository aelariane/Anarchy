using ExitGames.Client.Photon;

namespace Anarchy.Network.Events
{
    public class EventJoin : INetworkEvent
    {
        public EventJoin()
        {
            NetworkManager.RegisterEvent(this);
        }

        public byte Code => 255;

        public bool CheckData(EventData data, PhotonPlayer sender, out string reason)
        {
            if (sender != null && !sender.IsLocal)
            {
                reason = UI.Log.GetString("senderMustBeNull");
                return false;
            }
            reason = "";
            ExitGames.Client.Photon.Hashtable properties = (ExitGames.Client.Photon.Hashtable)data[0xf9];
            int key = (int)data[254];
            if (sender == null)
            {
                bool isLocal = PhotonNetwork.networkingPeer.mLocalActor.ID == key;
                PhotonNetwork.networkingPeer.AddNewPlayer(key, new PhotonPlayer(isLocal, key, properties));
                PhotonNetwork.networkingPeer.ResetPhotonViewsOnSerialize();
            }
            if (key != PhotonNetwork.networkingPeer.mLocalActor.ID)
            {
                object[] parameters = new object[] { NetworkingPeer.mActors[key] };
                NetworkingPeer.SendMonoMessage(PhotonNetworkingMessage.OnPhotonPlayerConnected, parameters);
                PhotonNetwork.SendChek(NetworkingPeer.mActors[key]);
            }
            else
            {
                int[] numArray = (int[])data[0xfc];
                foreach (int num2 in numArray)
                {
                    if ((PhotonNetwork.networkingPeer.mLocalActor.ID != num2) && !NetworkingPeer.mActors.ContainsKey(num2))
                    {
                        PhotonNetwork.networkingPeer.AddNewPlayer(num2, new PhotonPlayer(false, num2, string.Empty));
                    }
                }
                NetworkingPeer.SendMonoMessage(PhotonNetworkingMessage.OnJoinedRoom, new object[0]);
                if ((PhotonNetwork.networkingPeer.mLastJoinType == JoinType.JoinOrCreateOnDemand) && (PhotonNetwork.networkingPeer.mLocalActor.ID == 1))
                {
                    NetworkingPeer.SendMonoMessage(PhotonNetworkingMessage.OnCreatedRoom, new object[0]);
                }
                PhotonNetwork.SendChek();
            }
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