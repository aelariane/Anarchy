using Anarchy.Network.Events;
using System.Collections.Generic;

namespace Anarchy.Network
{
    public class NetworkManager
    {
        private static readonly Event101 ev101 = new Event101();
        private static readonly Event226 ev226 = new Event226();
        private static readonly EventCloseConnection ban = new EventCloseConnection();
        private static readonly EventDestroy destroy = new EventDestroy();
        private static readonly EventDestroyAll destroyAll = new EventDestroyAll();
        private static readonly EventDI di = new EventDI();
        private static readonly EventJoin join = new EventJoin();
        private static readonly EventLeave leave = new EventLeave();
        private static readonly EventMCSwitch mcChange = new EventMCSwitch();
        private static readonly EventOSR osr = new EventOSR();
        private static readonly EventPropertires props = new EventPropertires();
        private static readonly EventRoast roast = new EventRoast();
        private static readonly EventRoomListUpdate roomUpdate = new EventRoomListUpdate();
        private static readonly EventRoomListUpdate1 roomUpdate1 = new EventRoomListUpdate1();
        private static readonly EventRPC rpc = new EventRPC();

        static NetworkManager()
        {
            NetworkingPeer.RegisterEvent(PhotonNetworkingMessage.OnJoinedLobby, OnJoinedLobby);
        }

        private static Dictionary<byte, INetworkEvent> events;
        //public static bool NeedRejoin = false;
        //public static Room RejoinRoom;
        //public static string RejoinRegion = "";

        internal static void RegisterEvent(INetworkEvent ev)
        {
            if (events == null)
                events = new Dictionary<byte, INetworkEvent>();
            events.Add(ev.Code, ev);
        }

        public static INetworkEvent GetEvent(byte code)
        {
            if (!events.ContainsKey(code))
                return null;
            return events[code];
        }

        private static void OnJoinedLobby(Optimization.AOTEventArgs args)
        {
            //if (NeedRejoin)
            //{
            //    TryRejoinRoom();
            //    NeedRejoin = false;
            //}
            //RejoinRegion = PhotonNetwork.networkingPeer.MasterServerAddress.Split(':')[0];
        }

        //public static bool TryRejoin()
        //{
        //    if (Settings.Rejoin == 0 || RejoinRoom == null)
        //        return false;
        //    bool result = PhotonNetwork.ConnectToMaster(RejoinRegion, NetworkingPeer.ProtocolToNameServerPort[PhotonNetwork.networkingPeer.TransportProtocol], FengGameManagerMKII.ApplicationId, UIMainReferences.ConnectField);
        //    return result;
        //}

        //public static bool TryRejoinRoom()
        //{
        //    if (Settings.Rejoin == 0 || RejoinRoom == null)
        //    {
        //        return false;
        //    }
        //    if (RejoinRoom.maxPlayers <= RejoinRoom.playerCount)
        //        return false;
        //    return PhotonNetwork.JoinRoom(RejoinRoom.name);
        //}
    }
}
