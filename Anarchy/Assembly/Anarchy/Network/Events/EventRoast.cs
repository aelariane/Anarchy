using ExitGames.Client.Photon;

namespace Anarchy.Network.Events
{
    public class EventRoast : INetworkEvent
    {
        public byte Code { get; }

        public EventRoast()
        {
            Code = 228;
            NetworkManager.RegisterEvent(this);
        }

        public bool CheckData(EventData data, PhotonPlayer sender, out string reason)
        {
            reason = UI.Log.GetString("roastEvent");
            return false;
        }

        public bool Handle() => true;

        public void OnFailedHandle()
        {
        }
    }
}