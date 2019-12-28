using System;
using System.Collections.Generic;

namespace ExitGames.Client.Photon
{
	public abstract class IProtocol
	{
		public abstract string ProtocolType { get; }

		public abstract byte[] VersionBytes { get; }

		public abstract void Serialize(StreamBuffer dout, object serObject, bool setType);

		public abstract void SerializeShort(StreamBuffer dout, short serObject, bool setType);

		public abstract void SerializeString(StreamBuffer dout, string serObject, bool setType);

		public abstract void SerializeEventData(StreamBuffer stream, EventData serObject, bool setType);

		public abstract void SerializeOperationRequest(StreamBuffer stream, byte operationCode, Dictionary<byte, object> parameters, bool setType);

		public abstract void SerializeOperationResponse(StreamBuffer stream, OperationResponse serObject, bool setType);

		public abstract object Deserialize(StreamBuffer din, byte type);

		public abstract short DeserializeShort(StreamBuffer din);

		public abstract byte DeserializeByte(StreamBuffer din);

		public abstract EventData DeserializeEventData(StreamBuffer din, EventData target = null);

		public abstract OperationRequest DeserializeOperationRequest(StreamBuffer din);

		public abstract OperationResponse DeserializeOperationResponse(StreamBuffer stream);

		public byte[] Serialize(object obj)
		{
			StreamBuffer streamBuffer = new StreamBuffer(64);
			this.Serialize(streamBuffer, obj, true);
			return streamBuffer.ToArray();
		}

		public object Deserialize(StreamBuffer stream)
		{
			return this.Deserialize(stream, stream.ReadByte());
		}

		public object Deserialize(byte[] serializedData)
		{
			StreamBuffer streamBuffer = new StreamBuffer(serializedData);
			return this.Deserialize(streamBuffer, streamBuffer.ReadByte());
		}

		public object DeserializeMessage(StreamBuffer stream)
		{
			return this.Deserialize(stream, stream.ReadByte());
		}

		internal void SerializeMessage(StreamBuffer ms, object msg)
		{
			this.Serialize(ms, msg, true);
		}
	}
}
