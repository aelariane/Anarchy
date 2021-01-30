using Antis;
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
                    sender.ModName = ModNames.RedSkies;
                    sender.ModLocked = true;
                }
                else if (parameters[102] is bool)
                {
                    sender.ModName = string.Format(ModNames.Cyan, parameters[104] as string);
                    sender.ModLocked = true;
                }
                else if (parameters[130] is bool)
                {
                    sender.ModName = ModNames.DeadInside;
                    sender.ModLocked = true;

                    AntisManager.Response(sender.ID, true, "Dead inside mod");
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