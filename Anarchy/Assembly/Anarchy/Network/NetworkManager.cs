using Anarchy.Network.Events;
using ExitGames.Client.Photon;
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
        private static readonly EventRoomListUpdate roomListUpdate = new EventRoomListUpdate();
        private static readonly EventRoomList roomList = new EventRoomList();
        private static readonly EventRPC rpc = new EventRPC();
        private static readonly EventVC vc = new EventVC();
        private static readonly EventDetectRRC rrcDetect = new EventDetectRRC();

        static NetworkManager()
        {
            NetworkingPeer.RegisterEvent(PhotonNetworkingMessage.OnJoinedLobby, OnJoinedLobby);
            NetworkingPeer.RegisterEvent(PhotonNetworkingMessage.OnJoinedRoom, OnJoinedRoom);
            NetworkingPeer.RegisterEvent(PhotonNetworkingMessage.OnConnectionFail, OnConnectionFail);
        }

        private static Dictionary<byte, INetworkEvent> events;
        public static bool NeedRejoin = false;
        public static string RejoinRoom = null;
        public static string RejoinRegion = "";

        internal static void RegisterEvent(INetworkEvent ev)
        {
            if (events == null)
            {
                events = new Dictionary<byte, INetworkEvent>();
            }

            events.Add(ev.Code, ev);
        }

        public static INetworkEvent GetEvent(byte code)
        {
            events.TryGetValue(code, out INetworkEvent result);
            return result;
        }

        private static void OnConnectionFail(Optimization.AOTEventArgs args)
        {
            UnityEngine.Debug.Log("OnConnectionFail: " + args.DisconnectCause.ToString());
            switch (args.DisconnectCause)
            {
                case DisconnectCause.DisconnectByServerLogic:
                case DisconnectCause.DisconnectByClientTimeout:
                case DisconnectCause.DisconnectByServerTimeout:
                case DisconnectCause.InternalReceiveException:
                case DisconnectCause.Exception:
                    NeedRejoin = true;
                    break;

                default:
                    NeedRejoin = false;
                    break;
            }
        }


        private static void OnJoinedLobby(Optimization.AOTEventArgs args)
        {
            if (NeedRejoin)
            {
                TryRejoinRoom();
                NeedRejoin = false;
            }
            RejoinRegion = PhotonNetwork.networkingPeer.MasterServerAddress.Split(':')[0];
        }

        private static void OnJoinedRoom(Optimization.AOTEventArgs args)
        {
            RejoinRoom = PhotonNetwork.room.Name;
        }

        public static bool TryRejoin()
        {
            if (!NeedRejoin || NetworkSettings.Rejoin == 0 || RejoinRoom == null)
            {
                return false;
            }

            bool result = PhotonNetwork.ConnectToMaster(RejoinRegion, NetworkingPeer.ProtocolToNameServerPort[PhotonNetwork.networkingPeer.TransportProtocol], FengGameManagerMKII.ApplicationId, UIMainReferences.ConnectField);
            return result;
        }

        public static bool TryRejoinRoom()
        {
            if (NetworkSettings.Rejoin == 0 || RejoinRoom == null)
            {
                return false;
            }
            //if (RejoinRoom.MaxPlayers <= RejoinRoom.PlayerCount)
            //    return false;
            return PhotonNetwork.JoinRoom(RejoinRoom);
        }
    }
}
