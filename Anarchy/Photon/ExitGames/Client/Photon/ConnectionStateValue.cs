using System;

namespace ExitGames.Client.Photon
{
	public enum ConnectionStateValue : byte
	{
		Disconnected,
		Connecting,
		Connected = 3,
		Disconnecting,
		AcknowledgingDisconnect,
		Zombie
	}
}
