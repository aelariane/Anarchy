using System;
using System.Collections.Generic;
using System.Threading;

namespace ExitGames.Client.Photon
{
    internal class TPeer : PeerBase
    {
        internal const int TCP_HEADER_BYTES = 7;

        internal const int MSG_HEADER_BYTES = 2;

        public const int ALL_HEADER_BYTES = 9;

        private Queue<byte[]> incomingList = new Queue<byte[]>(32);

        internal List<StreamBuffer> outgoingStream;

        private int lastPingResult;

        private byte[] pingRequest = new byte[5]
        {
        240,
        0,
        0,
        0,
        0
        };

        internal static readonly byte[] tcpFramedMessageHead = new byte[9]
        {
        251,
        0,
        0,
        0,
        0,
        0,
        0,
        243,
        2
        };

        internal static readonly byte[] tcpMsgHead = new byte[2]
        {
        243,
        2
        };

        internal byte[] messageHeader;

        protected internal bool DoFraming = true;

        internal override int QueuedIncomingCommandsCount
        {
            get
            {
                return this.incomingList.Count;
            }
        }

        internal override int QueuedOutgoingCommandsCount
        {
            get
            {
                return base.outgoingCommandsInStream;
            }
        }

        internal TPeer()
        {
            base.TrafficPackageHeaderSize = 0;
        }

        internal override void InitPeerBase()
        {
            base.InitPeerBase();
            this.incomingList = new Queue<byte[]>(32);
            base.timestampOfLastReceive = SupportClass.GetTickCount();
        }

        internal override bool Connect(string serverAddress, string appID, object customData = null)
        {
            if (base.peerConnectionState != 0)
            {
                base.Listener.DebugReturn(DebugLevel.WARNING, "Connect() can't be called if peer is not Disconnected. Not connecting.");
                return false;
            }
            if ((int)base.debugOut >= 5)
            {
                base.Listener.DebugReturn(DebugLevel.ALL, "Connect()");
            }
            base.ServerAddress = serverAddress;
            this.InitPeerBase();
            this.outgoingStream = new List<StreamBuffer>();
            if (base.usedTransportProtocol == ConnectionProtocol.WebSocket || base.usedTransportProtocol == ConnectionProtocol.WebSocketSecure)
            {
                serverAddress = base.PepareWebSocketUrl(serverAddress, appID, customData);
            }
            if (base.SocketImplementation != null)
            {
                base.PhotonSocket = (IPhotonSocket)Activator.CreateInstance(base.SocketImplementation, this);
            }
            else
            {
                base.PhotonSocket = new SocketTcp(this);
            }
            if (base.PhotonSocket == null)
            {
                base.Listener.DebugReturn(DebugLevel.ERROR, "Connect() failed, because SocketImplementation or socket was null. Set PhotonPeer.SocketImplementation before Connect(). SocketImplementation: " + base.SocketImplementation);
                return false;
            }
            this.messageHeader = (this.DoFraming ? TPeer.tcpFramedMessageHead : TPeer.tcpMsgHead);
            if (base.PhotonSocket.Connect())
            {
                base.peerConnectionState = ConnectionStateValue.Connecting;
                return true;
            }
            base.peerConnectionState = ConnectionStateValue.Disconnected;
            return false;
        }

        public override void OnConnect()
        {
            this.lastPingResult = SupportClass.GetTickCount();
            byte[] data = base.PrepareConnectData(base.ServerAddress, base.AppId, base.CustomInitData);
            this.EnqueueInit(data);
            this.SendOutgoingCommands();
        }

        internal override void Disconnect()
        {
            if (base.peerConnectionState != 0 && base.peerConnectionState != ConnectionStateValue.Disconnecting)
            {
                if ((int)base.debugOut >= 5)
                {
                    base.Listener.DebugReturn(DebugLevel.ALL, "TPeer.Disconnect()");
                }
                this.StopConnection();
            }
        }

        internal override void StopConnection()
        {
            base.peerConnectionState = ConnectionStateValue.Disconnecting;
            if (base.PhotonSocket != null)
            {
                base.PhotonSocket.Disconnect();
            }
            Queue<byte[]> obj = this.incomingList;
            lock (obj)
            {
                this.incomingList.Clear();
            }
            base.peerConnectionState = ConnectionStateValue.Disconnected;
            base.EnqueueStatusCallback(StatusCode.Disconnect);
        }

        internal override void FetchServerTimestamp()
        {
            if (base.peerConnectionState != ConnectionStateValue.Connected)
            {
                if ((int)base.debugOut >= 3)
                {
                    base.Listener.DebugReturn(DebugLevel.INFO, "FetchServerTimestamp() was skipped, as the client is not connected. Current ConnectionState: " + base.peerConnectionState);
                }
                base.Listener.OnStatusChanged(StatusCode.SendError);
            }
            else
            {
                this.SendPing();
                base.serverTimeOffsetIsAvailable = false;
            }
        }

        private void EnqueueInit(byte[] data)
        {
            if (this.DoFraming)
            {
                StreamBuffer streamBuffer = new StreamBuffer(data.Length + 32);
                byte[] array = new byte[7]
                {
                251,
                0,
                0,
                0,
                0,
                0,
                1
                };
                int num = 1;
                Protocol.Serialize(data.Length + array.Length, array, ref num);
                streamBuffer.Write(array, 0, array.Length);
                streamBuffer.Write(data, 0, data.Length);
                if (base.TrafficStatsEnabled)
                {
                    base.TrafficStatsOutgoing.TotalPacketCount++;
                    base.TrafficStatsOutgoing.TotalCommandsInPackets++;
                    base.TrafficStatsOutgoing.CountControlCommand(streamBuffer.Length);
                }
                this.EnqueueMessageAsPayload(DeliveryMode.Reliable, streamBuffer, 0);
            }
        }

        internal override bool DispatchIncomingCommands()
        {
            if (base.peerConnectionState == ConnectionStateValue.Connected && SupportClass.GetTickCount() - base.timestampOfLastReceive > base.DisconnectTimeout)
            {
                base.EnqueueStatusCallback(StatusCode.TimeoutDisconnect);
                base.EnqueueActionForDispatch(((PeerBase)this).Disconnect);
            }
            while (true)
            {
                Queue<MyAction> actionQueue = base.ActionQueue;     
                MyAction myAction = default(MyAction);
                lock (actionQueue)
                {
                    if (base.ActionQueue.Count > 0)
                    {
                        myAction = base.ActionQueue.Dequeue();
                        goto IL_008a;
                    }
                }
                break;
            IL_008a:
                myAction();
            }
            Queue<byte[]> obj = this.incomingList;         
            byte[] array = default(byte[]);
            lock (obj)
            {
                if (this.incomingList.Count <= 0)
                {
                    return false;
                }
                array = this.incomingList.Dequeue();
            }
            base.ByteCountCurrentDispatch = array.Length + 3;
            return this.DeserializeMessageAndCallback(new StreamBuffer(array));
        }

        internal override bool SendOutgoingCommands()
        {
            if (base.peerConnectionState == ConnectionStateValue.Disconnected)
            {
                return false;
            }
            if (!base.PhotonSocket.Connected)
            {
                return false;
            }
            base.timeInt = SupportClass.GetTickCount() - base.timeBase;
            base.timeLastSendOutgoing = base.timeInt;
            if (base.peerConnectionState == ConnectionStateValue.Connected && Math.Abs(SupportClass.GetTickCount() - this.lastPingResult) > base.timePingInterval)
            {
                this.SendPing();
            }
            List<StreamBuffer> obj = this.outgoingStream;
            lock (obj)
            {
                for (int i = 0; i < this.outgoingStream.Count; i++)
                {
                    StreamBuffer streamBuffer = this.outgoingStream[i];
                    this.SendData(streamBuffer.GetBuffer(), streamBuffer.Length);
                    PeerBase.MessageBufferPoolPut(streamBuffer);
                }
                this.outgoingStream.Clear();
                base.outgoingCommandsInStream = 0;
            }
            return false;
        }

        internal override bool SendAcksOnly()
        {
            if (base.PhotonSocket == null || !base.PhotonSocket.Connected)
            {
                return false;
            }
            base.timeInt = SupportClass.GetTickCount() - base.timeBase;
            if (base.peerConnectionState == ConnectionStateValue.Connected && SupportClass.GetTickCount() - this.lastPingResult > base.timePingInterval)
            {
                this.SendPing();
            }
            return false;
        }

        internal override bool EnqueueOperation(Dictionary<byte, object> parameters, byte opCode, SendOptions sendParams, EgMessageType messageType)
        {
            if (base.peerConnectionState != ConnectionStateValue.Connected)
            {
                if ((int)base.debugOut >= 1)
                {
                    base.Listener.DebugReturn(DebugLevel.ERROR, "Cannot send op: " + opCode + "! Not connected. PeerState: " + base.peerConnectionState);
                }
                base.Listener.OnStatusChanged(StatusCode.SendError);
                return false;
            }
            if (sendParams.Channel >= base.ChannelCount)
            {
                if ((int)base.debugOut >= 1)
                {
                    base.Listener.DebugReturn(DebugLevel.ERROR, "Cannot send op: Selected channel (" + sendParams.Channel + ")>= channelCount (" + base.ChannelCount + ").");
                }
                base.Listener.OnStatusChanged(StatusCode.SendError);
                return false;
            }
            StreamBuffer opMessage = this.SerializeOperationToMessage(opCode, parameters, messageType, sendParams.Encrypt);
            return this.EnqueueMessageAsPayload(sendParams.DeliveryMode, opMessage, sendParams.Channel);
        }

        internal override bool EnqueueMessage(object msg, SendOptions sendOptions)
        {
            if (base.peerConnectionState != ConnectionStateValue.Connected)
            {
                if ((int)base.debugOut >= 1)
                {
                    base.Listener.DebugReturn(DebugLevel.ERROR, "Cannot send message! Not connected. PeerState: " + base.peerConnectionState);
                }
                base.Listener.OnStatusChanged(StatusCode.SendError);
                return false;
            }
            byte channel = sendOptions.Channel;
            if (channel >= base.ChannelCount)
            {
                if ((int)base.debugOut >= 1)
                {
                    base.Listener.DebugReturn(DebugLevel.ERROR, "Cannot send op: Selected channel (" + channel + ")>= channelCount (" + base.ChannelCount + ").");
                }
                base.Listener.OnStatusChanged(StatusCode.SendError);
                return false;
            }
            StreamBuffer opMessage = base.SerializeMessageToMessage(msg, sendOptions.Encrypt, this.messageHeader, true);
            return this.EnqueueMessageAsPayload(sendOptions.DeliveryMode, opMessage, channel);
        }

        internal override StreamBuffer SerializeOperationToMessage(byte opCode, Dictionary<byte, object> parameters, EgMessageType messageType, bool encrypt)
        {
            StreamBuffer streamBuffer = PeerBase.MessageBufferPoolGet();
            streamBuffer.SetLength(0L);
            if (!encrypt)
            {
                streamBuffer.Write(this.messageHeader, 0, this.messageHeader.Length);
            }
            base.SerializationProtocol.SerializeOperationRequest(streamBuffer, opCode, parameters, false);
            if (encrypt)
            {
                byte[] array = base.CryptoProvider.Encrypt(streamBuffer.GetBuffer(), 0, streamBuffer.Length);
                streamBuffer.SetLength(0L);
                streamBuffer.Write(this.messageHeader, 0, this.messageHeader.Length);
                streamBuffer.Write(array, 0, array.Length);
            }
            byte[] buffer = streamBuffer.GetBuffer();
            if (messageType != EgMessageType.Operation)
            {
                buffer[this.messageHeader.Length - 1] = (byte)messageType;
            }
            if (encrypt)
            {
                buffer[this.messageHeader.Length - 1] = (byte)(buffer[this.messageHeader.Length - 1] | 0x80);
            }
            if (this.DoFraming)
            {
                int num = 1;
                Protocol.Serialize(streamBuffer.Length, buffer, ref num);
            }
            return streamBuffer;
        }

        internal bool EnqueueMessageAsPayload(DeliveryMode deliveryMode, StreamBuffer opMessage, byte channelId)
        {
            if (opMessage == null)
            {
                return false;
            }
            if (this.DoFraming)
            {
                byte[] buffer = opMessage.GetBuffer();
                buffer[5] = channelId;
                switch (deliveryMode)
                {
                    case DeliveryMode.Unreliable:
                        buffer[6] = 0;
                        break;
                    case DeliveryMode.Reliable:
                        buffer[6] = 1;
                        break;
                    case DeliveryMode.UnreliableUnsequenced:
                        buffer[6] = 2;
                        break;
                    case DeliveryMode.ReliableUnsequenced:
                        buffer[6] = 3;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException("DeliveryMode", deliveryMode, null);
                }
            }
            List<StreamBuffer> obj = this.outgoingStream;
            lock (obj)
            {
                this.outgoingStream.Add(opMessage);
                base.outgoingCommandsInStream++;
            }
            int num = base.ByteCountLastOperation = opMessage.Length;
            if (base.TrafficStatsEnabled)
            {
                switch (deliveryMode)
                {
                    case DeliveryMode.Unreliable:
                        base.TrafficStatsOutgoing.CountUnreliableOpCommand(num);
                        break;
                    case DeliveryMode.Reliable:
                        base.TrafficStatsOutgoing.CountReliableOpCommand(num);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException("deliveryMode", deliveryMode, null);
                }
                base.TrafficStatsGameLevel.CountOperation(num);
            }
            return true;
        }

        internal void SendPing()
        {
            int num = this.lastPingResult = SupportClass.GetTickCount();
            if (!this.DoFraming)
            {
                SendOptions sendOptions = new SendOptions() { DeliveryMode = DeliveryMode.Reliable };
                StreamBuffer streamBuffer = this.SerializeOperationToMessage(PhotonCodes.Ping, new Dictionary<byte, object>
            {
                {
                    (byte)1,
                    (object)num
                }
            }, EgMessageType.InternalOperationRequest, sendOptions.Encrypt);
                if (base.TrafficStatsEnabled)
                {
                    base.TrafficStatsOutgoing.CountControlCommand(streamBuffer.Length);
                }
                this.SendData(streamBuffer.GetBuffer(), streamBuffer.Length);
                PeerBase.MessageBufferPoolPut(streamBuffer);
            }
            else
            {
                int num2 = 1;
                Protocol.Serialize(num, this.pingRequest, ref num2);
                if (base.TrafficStatsEnabled)
                {
                    base.TrafficStatsOutgoing.CountControlCommand(this.pingRequest.Length);
                }
                this.SendData(this.pingRequest, this.pingRequest.Length);
            }
        }

        internal void SendData(byte[] data, int length)
        {
            try
            {
                base.bytesOut += length;
                if (base.TrafficStatsEnabled)
                {
                    base.TrafficStatsOutgoing.TotalPacketCount++;
                    base.TrafficStatsOutgoing.TotalCommandsInPackets += base.outgoingCommandsInStream;
                }
                if (base.NetworkSimulationSettings.IsSimulationEnabled)
                {
                    byte[] array = new byte[length];
                    Buffer.BlockCopy(data, 0, array, 0, length);
                    base.SendNetworkSimulated(array);
                }
                else
                {
                    base.PhotonSocket.Send(data, length);
                }
            }
            catch (Exception ex)
            {
                if ((int)base.debugOut >= 1)
                {
                    base.Listener.DebugReturn(DebugLevel.ERROR, ex.ToString());
                }
                SupportClass.WriteStackTrace(ex);
            }
        }

        internal override void ReceiveIncomingCommands(byte[] inbuff, int dataLength)
        {
            if (inbuff == null)
            {
                if ((int)base.debugOut >= 1)
                {
                    base.EnqueueDebugReturn(DebugLevel.ERROR, "checkAndQueueIncomingCommands() inBuff: null");
                }
            }
            else
            {
                base.timestampOfLastReceive = SupportClass.GetTickCount();
                base.timeInt = SupportClass.GetTickCount() - base.timeBase;
                base.bytesIn += dataLength + 7;
                if (base.TrafficStatsEnabled)
                {
                    base.TrafficStatsIncoming.TotalPacketCount++;
                    base.TrafficStatsIncoming.TotalCommandsInPackets++;
                }
                if (inbuff[0] == 243 || inbuff[0] == 244)
                {
                    byte[] array = new byte[dataLength];
                    Buffer.BlockCopy(inbuff, 0, array, 0, dataLength);
                    Queue<byte[]> obj = this.incomingList;
                    lock (obj)
                    {
                        this.incomingList.Enqueue(array);
                    }
                }
                else if (inbuff[0] == 240)
                {
                    base.TrafficStatsIncoming.CountControlCommand(inbuff.Length);
                    this.ReadPingResult(inbuff);
                }
                else if ((int)base.debugOut >= 1)
                {
                    base.EnqueueDebugReturn(DebugLevel.ERROR, "receiveIncomingCommands() MagicNumber should be 0xF0, 0xF3 or 0xF4. Is: " + inbuff[0]);
                }
            }
        }

        private void ReadPingResult(byte[] inbuff)
        {
            int num = 0;
            int num2 = 0;
            int num3 = 1;
            Protocol.Deserialize(out num, inbuff, ref num3);
            Protocol.Deserialize(out num2, inbuff, ref num3);
            base.lastRoundTripTime = SupportClass.GetTickCount() - num2;
            if (!base.serverTimeOffsetIsAvailable)
            {
                base.roundTripTime = base.lastRoundTripTime;
            }
            base.UpdateRoundTripTimeAndVariance(base.lastRoundTripTime);
            if (!base.serverTimeOffsetIsAvailable)
            {
                base.serverTimeOffset = num + (base.lastRoundTripTime >> 1) - SupportClass.GetTickCount();
                base.serverTimeOffsetIsAvailable = true;
            }
        }

        protected internal void ReadPingResult(OperationResponse operationResponse)
        {
            int num = (int)operationResponse.Parameters[2];
            int num2 = (int)operationResponse.Parameters[1];
            base.lastRoundTripTime = SupportClass.GetTickCount() - num2;
            if (!base.serverTimeOffsetIsAvailable)
            {
                base.roundTripTime = base.lastRoundTripTime;
            }
            base.UpdateRoundTripTimeAndVariance(base.lastRoundTripTime);
            if (!base.serverTimeOffsetIsAvailable)
            {
                base.serverTimeOffset = num + (base.lastRoundTripTime >> 1) - SupportClass.GetTickCount();
                base.serverTimeOffsetIsAvailable = true;
            }
        }
    }
}
