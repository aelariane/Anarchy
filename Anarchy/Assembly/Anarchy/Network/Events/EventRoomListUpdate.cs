using ExitGames.Client.Photon;
using System;
using System.Collections;
using System.Collections.Generic;
using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace Anarchy.Network.Events
{
    public class EventRoomListUpdate : INetworkEvent
    {
        private Hashtable roomList;
        public byte Code => 229;

        public EventRoomListUpdate()
        {
            NetworkManager.RegisterEvent(this);
        }

        public bool CheckData(EventData data, PhotonPlayer sender, out string reason)
        {
            reason = "";
            if (sender != null)
            {
                reason = UI.Log.GetString("senderMustBeNull");
                return false;
            }
            roomList = data[0xde] as Hashtable;
            return true;
        }

        public bool Handle()
        {
            IEnumerator<DictionaryEntry> enumerator2 = roomList.GetEnumerator();
            try
            {
                while (enumerator2.MoveNext())
                {
                    DictionaryEntry current = enumerator2.Current;
                    string roomName = (string)current.Key;
                    RoomInfo info = new RoomInfo(roomName, (ExitGames.Client.Photon.Hashtable)current.Value);
                    if (info.RemovedFromList)
                    {
                        NetworkingPeer.mGameList.Remove(roomName);
                    }
                    else
                    {
                        NetworkingPeer.mGameList[roomName] = info;
                    }
                }
            }
            finally
            {
                if (enumerator2 == null)
                {
                }
                enumerator2.Dispose();
            }
            PhotonNetwork.networkingPeer.mGameListCopy = new RoomInfo[NetworkingPeer.mGameList.Count];
            NetworkingPeer.mGameList.Values.CopyTo(PhotonNetwork.networkingPeer.mGameListCopy, 0);
            NetworkingPeer.SendMonoMessage(PhotonNetworkingMessage.OnReceivedRoomListUpdate, new object[0]);
            return true;
        }

        public void OnFailedHandle()
        {
            throw new NotImplementedException();
        }
    }
}