using Anarchy.Skins.Humans;

namespace Anarchy.Network.Events
{
    public class EventLeave : INetworkEvent
    {
        private int key;
        public byte Code { get; }

        public EventLeave()
        {
            Code = 254;
            NetworkManager.RegisterEvent(this);
        }

        public bool CheckData(ExitGames.Client.Photon.EventData data, PhotonPlayer sender, out string reason)
        {
            reason = string.Empty;
            if (sender == null)
            {
                return false;
            }

            key = sender.ID;
            return true;
        }

        public bool Handle()
        {
            PhotonNetwork.networkingPeer.HandleEventLeave(key);
            //ExitGames.Client.Photon.PhotonManager.RemoveID(key);
            if (HumanSkin.Storage.ContainsKey(key))
            {
                HumanSkin.Storage.Remove(key);
            }
            return true;
        }

        public void OnFailedHandle()
        {
        }
    }
}