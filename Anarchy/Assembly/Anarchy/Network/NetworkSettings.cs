using Anarchy.Configuration;
using ExitGames.Client.Photon;

namespace Anarchy.Network
{
    /// <summary>
    /// Contains Anarchy-Specific network settings
    /// </summary>
    public class NetworkSettings
    {
        public const string AdressString = "app-{0}.exitgamescloud.com";

        /// <summary>
        /// Connection network protocol
        /// </summary>
        /// <remarks>UDP, TCP, WebSocket, WebSocketSecure</remarks>
        public static readonly IntSetting ConnectionProtocol = new IntSetting("ConnectionProtocol", 0);

        /// <summary>
        /// Region preferred by player. Serverlist automatically connects to it
        /// </summary>
        /// <remarks>US, Europe, Asia, Japan</remarks>
        public static readonly IntSetting PreferedRegion = new IntSetting("PrefRegion", 1);
        public static readonly string[] RegionAdresses = new string[] { "us", "eu", "asia", "jp" };

        /// <summary>
        /// If Auto-Rejoin is enabled
        /// </summary>
        /// <remarks>0 - disabled, 1 - Enabled, rejoin by button, 2 - Rejoins instantly and automatically if dc'd</remarks>
        public static readonly IntSetting Rejoin = new IntSetting(nameof(Rejoin), 0);

        public static readonly BoolSetting CustomSettings = new BoolSetting("Network" + nameof(CustomSettings), false);
        public static readonly StringSetting ApplicationId = new StringSetting(nameof(ApplicationId), FengGameManagerMKII.ApplicationId);
        public static readonly StringSetting IPAdress = new StringSetting(nameof(IPAdress), "127.0.0.1");
        public static readonly IntSetting Port = new IntSetting(nameof(Port), 5055);
        public static readonly BoolSetting IsCustomPhotonServer = new BoolSetting(nameof(IsCustomPhotonServer), false);

        /// <summary>
        /// Connection protocol, selected at <seealso cref="ConnectionProtocol"/> setting
        /// </summary>
        public static ConnectionProtocol ConnectProtocol
        {
            get
            {
                int protocol = ConnectionProtocol.Value;
                if (protocol >= 2)
                {
                    protocol += 2;
                }

                return (ConnectionProtocol)protocol;
            }
        }

        public static SerializationProtocol SerializationProtocol
        {
            get
            {
                return ExitGames.Client.Photon.SerializationProtocol.GpBinaryV16;
            }
        }
    }
}
