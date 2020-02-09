

namespace ExitGames.Client.Photon
{
	public static class SerializationProtocolFactory
	{
		public static IProtocol Create(SerializationProtocol serializationProtocol)
		{
			IProtocol result = null;
            switch(serializationProtocol)
            {
                case SerializationProtocol.GpBinaryV16:
                    result = new RpcProtocols.GpBinaryV16.Protocol16();
                    break;

                case SerializationProtocol.GpBinaryV18:
                    result = new RpcProtocols.GpBinaryV18.Protocol18();
                    break;

                default:
                    result = new RpcProtocols.GpBinaryV16.Protocol16();
                    break;
            }
            return result;
		}
	}
}
