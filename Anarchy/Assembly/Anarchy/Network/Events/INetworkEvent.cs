using ExitGames.Client.Photon;

namespace Anarchy.Network.Events
{
    /// <summary>
    /// Defines network events
    /// </summary>
    public interface INetworkEvent
    {
        /// <summary>
        /// Event code in byte
        /// </summary>
        byte Code { get; }

        /// <summary>
        /// Checks if sent data is valid
        /// </summary>
        /// <param name="data">Receied data</param>
        /// <param name="sender">Player who sent it</param>
        /// <param name="reason">Gets set if data was invalid, <seealso cref="string.Empty"/> if data is valid</param>
        /// <returns><seealso cref="true"/> if data is valid. <seealso cref="false"/> if data is invalid</returns>
        bool CheckData(EventData data, PhotonPlayer sender, out string reason);

        /// <summary>
        /// Processes the event
        /// </summary>
        /// <returns><see cref="true"/> if esecution was succesful, <see cref="false"/> if something went wrong</returns>
        bool Handle();

        /// <summary>
        /// Supposed to be called if <seealso cref="Handle"/> returned <seealso cref="false"/>
        /// </summary>
        void OnFailedHandle();
    }
}