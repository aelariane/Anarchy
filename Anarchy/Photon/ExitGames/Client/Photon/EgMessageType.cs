using System;

namespace ExitGames.Client.Photon
{
	internal enum EgMessageType : byte
	{
		Init,
		InitResponse,
		Operation,
		OperationResponse,
		Event,
		InternalOperationRequest = 6,
		InternalOperationResponse,
		Message,
		RawMessage
	}
}
