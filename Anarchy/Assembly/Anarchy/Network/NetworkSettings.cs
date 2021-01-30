using Anarchy.Configuration;
using ExitGames.Client.Photon;

namespace Anarchy.Network
{
    public class NetworkSettings
    {
        public const string AdressString = "app-{0}.exitgamescloud.com";

        public static readonly IntSetting ConnectionProtocol = new IntSetting("ConnectionProtocol", 0);
        public static readonly IntSetting PreferedRegion = new IntSetting("PrefRegion", 1);
        public static readonly string[] RegionAdresses = new string[] { "us", "eu", "asia", "jp" };
        public static readonly IntSetting Rejoin = new IntSetting(nameof(Rejoin), 0);

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
