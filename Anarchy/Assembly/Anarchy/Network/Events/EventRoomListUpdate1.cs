using System;
using System.Collections.Generic;
using System.Collections;
using ExitGames.Client.Photon;
using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace Anarchy.Network.Events
{
    public class EventRoomListUpdate1 : INetworkEvent
    {
        private Hashtable roomList;

        public byte Code => 230;

        public EventRoomListUpdate1()
        {
            NetworkManager.RegisterEvent(this);
        }


        public bool CheckData(EventData data, PhotonPlayer sender, out string reason)
        {
            reason = "";
            if(sender != null)
            {
                reason = UI.Log.GetString("senderMustBeNull");
                return false;
            }
            roomList = data[0xde] as Hashtable;
            return true;
        }

        public bool Handle()
        {
            NetworkingPeer.mGameList = new Dictionary<string, RoomInfo>();
            foreach(var pair in roomList)
            {
                string key = (string)pair.Key;
                NetworkingPeer.mGameList[key] = new RoomInfo(key, (Hashtable)pair.Value);
            }
            PhotonNetwork.networkingPeer.mGameListCopy = new RoomInfo[NetworkingPeer.mGameList.Count];
            int i = 0; 
            foreach(RoomInfo info in NetworkingPeer.mGameList.Values)
            {
                PhotonNetwork.networkingPeer.mGameListCopy[i++] = info;
            }
            NetworkingPeer.SendMonoMessage(PhotonNetworkingMessage.OnReceivedRoomListUpdate, new object[0]);
            return true;
        }

        public void OnFailedHandle()
        {
            throw new NotImplementedException();
        }
    }
}
