using ExitGames.Client.Photon;

namespace Anarchy.Network.Events
{
    public class Event101 : INetworkEvent
    {
        public Event101()
        {
            NetworkManager.RegisterEvent(this);
        }

        public byte Code => 101;

        public bool CheckData(EventData data, PhotonPlayer sender, out string reason)
        {
            Hashtable parameters = data[245] as Hashtable;
            if (parameters != null)
            {
                if (parameters[101] is bool)
                {
                    sender.ModName = "[[000000]RedSkies[-]]";
                }
                else if (parameters[102] is bool)
                {
                    sender.ModName = $"[00FFFF][Cyan[CCCCDD]({parameters[104] as string})[-][-]]";
                }
            }
            reason = "";
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
