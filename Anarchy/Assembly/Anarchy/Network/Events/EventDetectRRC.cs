using ExitGames.Client.Photon;
using System;

namespace Anarchy.Network.Events
{
    public class EventDetectRRC : INetworkEvent
    {
        public byte Code => 176;

        public EventDetectRRC()
        {
            NetworkManager.RegisterEvent(this);
        }

        public bool CheckData(EventData data, PhotonPlayer sender, out string reason)
        {
            reason = string.Empty;
            
            string str = string.Empty;
            if(data.Parameters[245] is string == false)
            {
                return false;
            }
            str = data.Parameters[245] as string;

            switch (str.ToLower())
            {
                case "bronze":
                case "silver":
                case "gold":
                case "platin":
                case "diamond":
                case "master":
                case "grandmaster":
                case "top5":
                    sender.ModName = ModNames.RRC;
                    sender.ModLocked = true;
                    break;

                default:
                    UI.Log.AddLineRaw($"Unknown RRC ver ({str}) by ID {sender.ID}");
                    break;
            }

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
