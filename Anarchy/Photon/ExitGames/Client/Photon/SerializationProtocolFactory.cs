using System;

namespace ExitGames.Client.Photon
{
	internal static class SerializationProtocolFactory
	{
		internal static IProtocol Create(SerializationProtocol serializationProtocol)
		{
			IProtocol result;
			if (serializationProtocol != SerializationProtocol.GpBinaryV18)
			{
				result = new Protocol16();
			}
			else
			{
				result = new Protocol18();
			}
			return result;
		}
	}
}
