using ExitGames.Client.Photon;

namespace Anarchy.Network.Events
{
    public interface INetworkEvent
    {
        byte Code { get; }

        bool CheckData(EventData data, PhotonPlayer sender, out string reason);

        bool Handle();

        void OnFailedHandle();
    }
}