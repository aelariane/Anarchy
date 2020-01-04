using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using Photon.SocketServer.Security;

namespace ExitGames.Client.Photon
{
    public abstract class PeerBase
    {
        internal delegate void MyAction();

        internal PhotonPeer photonPeer;

        public IProtocol SerializationProtocol;

        internal ConnectionProtocol usedTransportProtocol;

        internal IPhotonSocket PhotonSocket;

        internal ConnectionStateValue peerConnectionState;

        internal int ByteCountLastOperation;

        internal int ByteCountCurrentDispatch;

        internal NCommand CommandInCurrentDispatch;

        internal int packetLossByCrc;

        internal int packetLossByChallenge;

        internal readonly Queue<MyAction> ActionQueue = new Queue<MyAction>();

        internal short peerID = -1;

        internal int serverTimeOffset;

        internal bool serverTimeOffsetIsAvailable;

        internal int roundTripTime;

        internal int roundTripTimeVariance;

        internal int lastRoundTripTime;

        internal int lowestRoundTripTime;

        internal int lastRoundTripTimeVariance;

        internal int highestRoundTripTimeVariance;

        internal int timestampOfLastReceive;

        internal static short peerCount;

        internal long bytesOut;

        internal long bytesIn;

        internal object CustomInitData;

        public string AppId;

        internal EventData reusableEventData;

        internal int timeBase;

        internal int timeInt;

        internal int timeoutInt;

        internal int timeLastAckReceive;

        internal int timeLastSendAck;

        internal int timeLastSendOutgoing;

        internal bool ApplicationIsInitialized;

        internal bool isEncryptionAvailable;

        internal int outgoingCommandsInStream = 0;

        protected internal static Queue<StreamBuffer> MessageBufferPool = new Queue<StreamBuffer>(32);

        internal ICryptoProvider CryptoProvider;

        private readonly Random lagRandomizer = new Random();

        internal readonly LinkedList<SimulationItem> NetSimListOutgoing = new LinkedList<SimulationItem>();

        internal readonly LinkedList<SimulationItem> NetSimListIncoming = new LinkedList<SimulationItem>();

        private readonly NetworkSimulationSet networkSimulationSettings = new NetworkSimulationSet();

        internal int TrafficPackageHeaderSize;

        private int commandLogSize;

        internal Queue<CmdLogItem> CommandLog;

        internal Queue<CmdLogItem> InReliableLog;

        internal Type SocketImplementation
        {
            get
            {
                return this.photonPeer.SocketImplementation;
            }
        }

        public string ServerAddress
        {
            get;
            internal set;
        }

        internal IPhotonPeerListener Listener
        {
            get
            {
                return this.photonPeer.Listener;
            }
        }

        internal DebugLevel debugOut
        {
            get
            {
                return this.photonPeer.DebugOut;
            }
        }

        internal int DisconnectTimeout
        {
            get
            {
                return this.photonPeer.DisconnectTimeout;
            }
        }

        internal int timePingInterval
        {
            get
            {
                return this.photonPeer.TimePingInterval;
            }
        }

        internal byte ChannelCount
        {
            get
            {
                return this.photonPeer.ChannelCount;
            }
        }

        internal long BytesOut
        {
            get
            {
                return this.bytesOut;
            }
        }

        internal long BytesIn
        {
            get
            {
                return this.bytesIn;
            }
        }

        internal abstract int QueuedIncomingCommandsCount
        {
            get;
        }

        internal abstract int QueuedOutgoingCommandsCount
        {
            get;
        }

        internal virtual int SentReliableCommandsCount
        {
            get
            {
                return 0;
            }
        }

        public virtual string PeerID
        {
            get
            {
                return ((ushort)this.peerID).ToString();
            }
        }

        internal static int outgoingStreamBufferSize
        {
            get
            {
                return PhotonPeer.OutgoingStreamBufferSize;
            }
        }

        internal bool IsSendingOnlyAcks
        {
            get
            {
                return this.photonPeer.IsSendingOnlyAcks;
            }
        }

        internal int mtu
        {
            get
            {
                return this.photonPeer.MaximumTransferUnit;
            }
        }

        protected internal bool IsIpv6
        {
            get
            {
                return this.PhotonSocket != null && this.PhotonSocket.AddressResolvedAsIpv6;
            }
        }

        public NetworkSimulationSet NetworkSimulationSettings
        {
            get
            {
                return this.networkSimulationSettings;
            }
        }

        internal bool TrafficStatsEnabled
        {
            get
            {
                return this.photonPeer.TrafficStatsEnabled;
            }
        }

        internal TrafficStats TrafficStatsIncoming
        {
            get
            {
                return this.photonPeer.TrafficStatsIncoming;
            }
        }

        internal TrafficStats TrafficStatsOutgoing
        {
            get
            {
                return this.photonPeer.TrafficStatsOutgoing;
            }
        }

        internal TrafficStatsGameLevel TrafficStatsGameLevel
        {
            get
            {
                return this.photonPeer.TrafficStatsGameLevel;
            }
        }

        internal int CommandLogSize
        {
            get
            {
                return this.commandLogSize;
            }
            set
            {
                this.commandLogSize = value;
                this.CommandLogResize();
            }
        }

        protected PeerBase()
        {
            this.networkSimulationSettings.peerBase = this;
            PeerBase.peerCount++;
        }

        public static StreamBuffer MessageBufferPoolGet()
        {
            Queue<StreamBuffer> messageBufferPool = PeerBase.MessageBufferPool;
            lock (messageBufferPool)
            {
                if (PeerBase.MessageBufferPool.Count > 0)
                {
                    return PeerBase.MessageBufferPool.Dequeue();
                }
                return new StreamBuffer(75);
            }
        }

        public static void MessageBufferPoolPut(StreamBuffer buff)
        {
            Queue<StreamBuffer> messageBufferPool = PeerBase.MessageBufferPool;
            lock (messageBufferPool)
            {
                buff.Position = 0;
                buff.SetLength(0L);
                PeerBase.MessageBufferPool.Enqueue(buff);
            }
        }

        internal virtual void InitPeerBase()
        {
            this.SerializationProtocol = SerializationProtocolFactory.Create(this.photonPeer.SerializationProtocolType);
            this.photonPeer.InitializeTrafficStats();
            this.ByteCountLastOperation = 0;
            this.ByteCountCurrentDispatch = 0;
            this.bytesIn = 0L;
            this.bytesOut = 0L;
            this.packetLossByCrc = 0;
            this.packetLossByChallenge = 0;
            this.networkSimulationSettings.LostPackagesIn = 0;
            this.networkSimulationSettings.LostPackagesOut = 0;
            LinkedList<SimulationItem> netSimListOutgoing = this.NetSimListOutgoing;
            lock (netSimListOutgoing)
            {
                this.NetSimListOutgoing.Clear();
            }
            LinkedList<SimulationItem> netSimListIncoming = this.NetSimListIncoming;
            lock (netSimListIncoming)
            {
                this.NetSimListIncoming.Clear();
            }
            this.peerConnectionState = ConnectionStateValue.Disconnected;
            this.timeBase = SupportClass.GetTickCount();
            this.isEncryptionAvailable = false;
            this.ApplicationIsInitialized = false;
            this.roundTripTime = 200;
            this.roundTripTimeVariance = 5;
            this.serverTimeOffsetIsAvailable = false;
            this.serverTimeOffset = 0;
        }

        internal abstract bool Connect(string serverAddress, string appID, object customData = null);

        private string GetHttpKeyValueString(Dictionary<string, string> dic)
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (KeyValuePair<string, string> item in dic)
            {
                stringBuilder.Append(item.Key).Append("=").Append(item.Value)
                    .Append("&");
            }
            return stringBuilder.ToString();
        }

        internal byte[] PrepareConnectData(string serverAddress, string appID, object custom)
        {
            if (this.PhotonSocket == null || !this.PhotonSocket.Connected)
            {
                this.EnqueueDebugReturn(DebugLevel.WARNING, "The peer attempts to prepare an Init-Request but the socket is not connected!?");
            }
            if (custom == null)
            {
                byte[] array = new byte[41];
                byte[] clientVersion = ExitGames.Client.Photon.Version.clientVersion;
                array[0] = 243;
                array[1] = 0;
                array[2] = this.SerializationProtocol.VersionBytes[0];
                array[3] = this.SerializationProtocol.VersionBytes[1];
                array[4] = this.photonPeer.ClientSdkIdShifted;
                array[5] = (byte)((byte)(clientVersion[0] << 4) | clientVersion[1]);
                array[6] = clientVersion[2];
                array[7] = clientVersion[3];
                array[8] = 0;
                if (string.IsNullOrEmpty(appID))
                {
                    appID = "LoadBalancing";
                }
                for (int i = 0; i < 32; i++)
                {
                    array[i + 9] = (byte)((i < appID.Length) ? ((byte)appID[i]) : 0);
                }
                if (this.IsIpv6)
                {
                    array[5] |= 128;
                }
                else
                {
                    array[5] &= 127;
                }
                return array;
            }
            if (custom != null)
            {
                byte[] array2 = null;
                Dictionary<string, string> dictionary = new Dictionary<string, string>();
                dictionary["init"] = null;
                dictionary["app"] = appID;
                dictionary["clientversion"] = this.photonPeer.ClientVersion;
                dictionary["protocol"] = this.SerializationProtocol.ProtocolType;
                dictionary["sid"] = this.photonPeer.ClientSdkIdShifted.ToString();
                byte[] array3 = null;
                int num = 0;
                if (custom != null)
                {
                    array3 = this.SerializationProtocol.Serialize(custom);
                    num += array3.Length;
                }
                string text = this.GetHttpKeyValueString(dictionary);
                if (this.IsIpv6)
                {
                    text += "&IPv6";
                }
                string text2 = string.Format("POST /?{0} HTTP/1.1\r\nHost: {1}\r\nContent-Length: {2}\r\n\r\n", text, serverAddress, num);
                array2 = new byte[text2.Length + num];
                if (array3 != null)
                {
                    Buffer.BlockCopy(array3, 0, array2, text2.Length, array3.Length);
                }
                Buffer.BlockCopy(Encoding.UTF8.GetBytes(text2), 0, array2, 0, text2.Length);
                return array2;
            }
            return null;
        }

        internal string PepareWebSocketUrl(string serverAddress, string appId, object customData)
        {
            StringBuilder stringBuilder = new StringBuilder(1024);
            string empty = string.Empty;
            if (customData != null)
            {
                byte[] array = this.SerializationProtocol.Serialize(customData);
                if (array == null)
                {
                    this.EnqueueDebugReturn(DebugLevel.ERROR, "Can not deserialize custom data");
                    return null;
                }
            }
            stringBuilder.AppendFormat("app={0}&clientver={1}&sid={2}&{3}&initobj={4}", appId, this.photonPeer.ClientVersion, this.photonPeer.ClientSdkIdShifted, this.IsIpv6 ? "IPv6" : string.Empty, empty);
            return stringBuilder.ToString();
        }

        public abstract void OnConnect();

        internal void InitCallback()
        {
            if (this.peerConnectionState == ConnectionStateValue.Connecting)
            {
                this.peerConnectionState = ConnectionStateValue.Connected;
            }
            this.ApplicationIsInitialized = true;
            this.FetchServerTimestamp();
            this.Listener.OnStatusChanged(StatusCode.Connect);
        }

        internal abstract void Disconnect();

        internal abstract void StopConnection();

        internal abstract void FetchServerTimestamp();

        internal abstract bool EnqueueOperation(Dictionary<byte, object> parameters, byte opCode, SendOptions sendParams, EgMessageType messageType = EgMessageType.Operation);

        internal abstract StreamBuffer SerializeOperationToMessage(byte opCode, Dictionary<byte, object> parameters, EgMessageType messageType, bool encrypt);

        internal abstract bool EnqueueMessage(object message, SendOptions sendOptions);

        internal StreamBuffer SerializeMessageToMessage(object message, bool encrypt, byte[] messageHeader, bool writeLength = true)
        {
            StreamBuffer streamBuffer = PeerBase.MessageBufferPoolGet();
            streamBuffer.SetLength(0L);
            if (!encrypt)
            {
                streamBuffer.Write(messageHeader, 0, messageHeader.Length);
            }
            if (message is byte[])
            {
                byte[] array = message as byte[];
                streamBuffer.Write(array, 0, array.Length);
            }
            else
            {
                this.SerializationProtocol.SerializeMessage(streamBuffer, message);
            }
            if (encrypt)
            {
                byte[] array2 = this.CryptoProvider.Encrypt(streamBuffer.GetBuffer(), 0, streamBuffer.Length);
                streamBuffer.SetLength(0L);
                streamBuffer.Write(messageHeader, 0, messageHeader.Length);
                streamBuffer.Write(array2, 0, array2.Length);
            }
            byte[] buffer = streamBuffer.GetBuffer();
            buffer[messageHeader.Length - 1] = (byte)((message is byte[]) ? 9 : 8);
            if (encrypt)
            {
                buffer[messageHeader.Length - 1] = (byte)(buffer[messageHeader.Length - 1] | 0x80);
            }
            if (writeLength)
            {
                int num = 1;
                Protocol.Serialize(streamBuffer.Length, buffer, ref num);
            }
            return streamBuffer;
        }

        internal abstract bool SendOutgoingCommands();

        internal virtual bool SendAcksOnly()
        {
            return false;
        }

        internal abstract void ReceiveIncomingCommands(byte[] inBuff, int dataLength);

        internal abstract bool DispatchIncomingCommands();

        internal virtual bool DeserializeMessageAndCallback(StreamBuffer stream)
        {
            if (stream.Length < 2)
            {
                if ((int)this.debugOut >= 1)
                {
                    this.Listener.DebugReturn(DebugLevel.ERROR, "Incoming UDP data too short! " + stream.Length);
                }
                return false;
            }
            byte b = stream.ReadByte();
            if (b != 243 && b != 253)
            {
                if ((int)this.debugOut >= 1)
                {
                    this.Listener.DebugReturn(DebugLevel.ALL, "No regular operation UDP message: " + b);
                }
                return false;
            }
            byte b2 = stream.ReadByte();
            byte b3 = (byte)(b2 & 0x7F);
            bool flag = (b2 & 0x80) > 0;
            if (b3 != 1)
            {
                try
                {
                    if (flag)
                    {
                        byte[] buf = this.CryptoProvider.Decrypt(stream.GetBuffer(), 2, stream.Length - 2);
                        stream = new StreamBuffer(buf);
                    }
                    else
                    {
                        stream.Seek(2L, SeekOrigin.Begin);
                    }
                }
                catch (Exception ex)
                {
                    if ((int)this.debugOut >= 1)
                    {
                        this.Listener.DebugReturn(DebugLevel.ERROR, "msgType: " + b3 + " exception: " + ex.ToString());
                    }
                    SupportClass.WriteStackTrace(ex);
                    return false;
                }
            }
            int num = 0;
            switch (b3)
            {
                case 1:
                    this.InitCallback();
                    break;

                case 3:
                    {
                        OperationResponse operationResponse = this.SerializationProtocol.DeserializeOperationResponse(stream);
                        if (this.TrafficStatsEnabled)
                        {
                            this.TrafficStatsGameLevel.CountResult(this.ByteCountCurrentDispatch);
                            num = SupportClass.GetTickCount();
                        }
                        this.Listener.OnOperationResponse(operationResponse);
                        if (this.TrafficStatsEnabled)
                        {
                            this.TrafficStatsGameLevel.TimeForResponseCallback(operationResponse.OperationCode, SupportClass.GetTickCount() - num);
                        }
                        break;
                    }

                case 4:
                    {

                        EventData eventData = this.SerializationProtocol.DeserializeEventData(stream, this.reusableEventData);
                        if (this.TrafficStatsEnabled)
                        {
                            this.TrafficStatsGameLevel.CountEvent(this.ByteCountCurrentDispatch);
                            num = SupportClass.GetTickCount();
                        }
                        this.Listener.OnEvent(eventData);
                        if (this.TrafficStatsEnabled)
                        {
                            this.TrafficStatsGameLevel.TimeForEventCallback(eventData.Code, SupportClass.GetTickCount() - num);
                        }
                        if (this.photonPeer.ReuseEventInstance)
                        {
                            this.reusableEventData = eventData;
                        }
                        break;
                    }

                case 7:
                    {
                        OperationResponse operationResponse = this.SerializationProtocol.DeserializeOperationResponse(stream);
                        if (this.TrafficStatsEnabled)
                        {
                            this.TrafficStatsGameLevel.CountResult(this.ByteCountCurrentDispatch);
                            num = SupportClass.GetTickCount();
                        }
                        if (operationResponse.OperationCode == PhotonCodes.InitEncryption)
                        {
                            this.DeriveSharedKey(operationResponse);
                        }
                        else if (operationResponse.OperationCode == PhotonCodes.Ping)
                        {
                            TPeer tPeer = this as TPeer;
                            if (tPeer != null)
                            {
                                tPeer.ReadPingResult(operationResponse);
                            }
                        }
                        else
                        {
                            this.EnqueueDebugReturn(DebugLevel.ERROR, "Received unknown internal operation. " + operationResponse.ToStringFull());
                        }
                        if (this.TrafficStatsEnabled)
                        {
                            this.TrafficStatsGameLevel.TimeForResponseCallback(operationResponse.OperationCode, SupportClass.GetTickCount() - num);
                        }
                        break;
                    }

                case 8:
                    {
                        object obj = this.SerializationProtocol.DeserializeMessage(stream);
                        if (this.TrafficStatsEnabled)
                        {
                            this.TrafficStatsGameLevel.CountEvent(this.ByteCountCurrentDispatch);
                            num = SupportClass.GetTickCount();
                        }
                        if (this.TrafficStatsEnabled)
                        {
                            this.TrafficStatsGameLevel.TimeForMessageCallback(SupportClass.GetTickCount() - num);
                        }
                        break;
                    }

                case 9:
                    {
                        if (this.TrafficStatsEnabled)
                        {
                            this.TrafficStatsGameLevel.CountEvent(this.ByteCountCurrentDispatch);
                            num = SupportClass.GetTickCount();
                        }
                        byte[] array = stream.ToArrayFromPos();
                        if (this.TrafficStatsEnabled)
                        {
                            this.TrafficStatsGameLevel.TimeForRawMessageCallback(SupportClass.GetTickCount() - num);
                        }
                        break;
                    }
                default:
                    this.EnqueueDebugReturn(DebugLevel.ERROR, "unexpected msgType " + b3);
                    break;
            }
            return true;
        }

        internal void UpdateRoundTripTimeAndVariance(int lastRoundtripTime)
        {
            if (lastRoundtripTime >= 0)
            {
                this.roundTripTimeVariance -= this.roundTripTimeVariance / 4;
                if (lastRoundtripTime >= this.roundTripTime)
                {
                    this.roundTripTime += (lastRoundtripTime - this.roundTripTime) / 8;
                    this.roundTripTimeVariance += (lastRoundtripTime - this.roundTripTime) / 4;
                }
                else
                {
                    this.roundTripTime += (lastRoundtripTime - this.roundTripTime) / 8;
                    this.roundTripTimeVariance -= (lastRoundtripTime - this.roundTripTime) / 4;
                }
                if (this.roundTripTime < this.lowestRoundTripTime)
                {
                    this.lowestRoundTripTime = this.roundTripTime;
                }
                if (this.roundTripTimeVariance > this.highestRoundTripTimeVariance)
                {
                    this.highestRoundTripTimeVariance = this.roundTripTimeVariance;
                }
            }
        }

        internal bool ExchangeKeysForEncryption(object lockObject)
        {
            this.isEncryptionAvailable = false;
            if (this.CryptoProvider != null)
            {
                this.CryptoProvider.Dispose();
                this.CryptoProvider = null;
            }
            if (this.CryptoProvider == null)
            {
                this.CryptoProvider = new DiffieHellmanCryptoProvider();
            }
            Dictionary<byte, object> dictionary = new Dictionary<byte, object>(1);
            dictionary[PhotonCodes.ClientKey] = this.CryptoProvider.PublicKey;
            SendOptions sendOptions;
            if (lockObject != null)
            {
                lock (lockObject)
                {
                    sendOptions = default(SendOptions);
                    sendOptions.Channel = 0;
                    sendOptions.Encrypt = false;
                    sendOptions.DeliveryMode = DeliveryMode.Reliable;
                    SendOptions sendParams = sendOptions;
                    return this.EnqueueOperation(dictionary, PhotonCodes.InitEncryption, sendParams, EgMessageType.InternalOperationRequest);
                }
            }
            sendOptions = default(SendOptions);
            sendOptions.Channel = 0;
            sendOptions.Encrypt = false;
            sendOptions.DeliveryMode = DeliveryMode.Reliable;
            SendOptions sendParams2 = sendOptions;
            return this.EnqueueOperation(dictionary, PhotonCodes.InitEncryption, sendParams2, EgMessageType.InternalOperationRequest);
        }

        internal void DeriveSharedKey(OperationResponse operationResponse)
        {
            if (operationResponse.ReturnCode != 0)
            {
                this.EnqueueDebugReturn(DebugLevel.ERROR, "Establishing encryption keys failed. " + operationResponse.ToStringFull());
                this.EnqueueStatusCallback(StatusCode.EncryptionFailedToEstablish);
            }
            else
            {
                byte[] array = (byte[])operationResponse[PhotonCodes.ServerKey];
                if (array == null || array.Length == 0)
                {
                    this.EnqueueDebugReturn(DebugLevel.ERROR, "Establishing encryption keys failed. Server's public key is null or empty. " + operationResponse.ToStringFull());
                    this.EnqueueStatusCallback(StatusCode.EncryptionFailedToEstablish);
                }
                else
                {
                    this.CryptoProvider.DeriveSharedKey(array);
                    this.isEncryptionAvailable = true;
                    this.EnqueueStatusCallback(StatusCode.EncryptionEstablished);
                }
            }
        }

        internal virtual void InitEncryption(byte[] secret)
        {
            if (this.CryptoProvider == null)
            {
                this.CryptoProvider = new DiffieHellmanCryptoProvider(secret);
                this.isEncryptionAvailable = true;
            }
        }

        internal void EnqueueActionForDispatch(MyAction action)
        {
            Queue<MyAction> actionQueue = this.ActionQueue;
            lock (actionQueue)
            {
                this.ActionQueue.Enqueue(action);
            }
        }

        internal void EnqueueDebugReturn(DebugLevel level, string debugReturn)
        {
            Queue<MyAction> actionQueue = this.ActionQueue;
            lock (actionQueue)
            {
                this.ActionQueue.Enqueue(delegate
                {
                    this.Listener.DebugReturn(level, debugReturn);
                });
            }
        }

        internal void EnqueueStatusCallback(StatusCode statusValue)
        {
            Queue<MyAction> actionQueue = this.ActionQueue;
            lock (actionQueue)
            {
                this.ActionQueue.Enqueue(delegate
                {
                    this.Listener.OnStatusChanged(statusValue);
                });
            }
        }

        internal void SendNetworkSimulated(byte[] dataToSend)
        {
            if (!this.NetworkSimulationSettings.IsSimulationEnabled)
            {
                throw new NotImplementedException("SendNetworkSimulated was called, despite NetworkSimulationSettings.IsSimulationEnabled == false.");
            }
            if (this.usedTransportProtocol == ConnectionProtocol.Udp && this.NetworkSimulationSettings.OutgoingLossPercentage > 0 && this.lagRandomizer.Next(101) < this.NetworkSimulationSettings.OutgoingLossPercentage)
            {
                this.networkSimulationSettings.LostPackagesOut++;
            }
            else
            {
                int num = (this.networkSimulationSettings.OutgoingJitter > 0) ? (this.lagRandomizer.Next(this.networkSimulationSettings.OutgoingJitter * 2) - this.networkSimulationSettings.OutgoingJitter) : 0;
                int num2 = this.networkSimulationSettings.OutgoingLag + num;
                int num3 = SupportClass.GetTickCount() + num2;
                SimulationItem value = new SimulationItem
                {
                    DelayedData = dataToSend,
                    TimeToExecute = num3,
                    Delay = num2
                };
                LinkedList<SimulationItem> netSimListOutgoing = this.NetSimListOutgoing;
                lock (netSimListOutgoing)
                {
                    if (this.NetSimListOutgoing.Count == 0 || this.usedTransportProtocol == ConnectionProtocol.Tcp)
                    {
                        this.NetSimListOutgoing.AddLast(value);
                    }
                    else
                    {
                        LinkedListNode<SimulationItem> linkedListNode = this.NetSimListOutgoing.First;
                        while (linkedListNode != null && linkedListNode.Value.TimeToExecute < num3)
                        {
                            linkedListNode = linkedListNode.Next;
                        }
                        if (linkedListNode == null)
                        {
                            this.NetSimListOutgoing.AddLast(value);
                        }
                        else
                        {
                            this.NetSimListOutgoing.AddBefore(linkedListNode, value);
                        }
                    }
                }
            }
        }

        internal void ReceiveNetworkSimulated(byte[] dataReceived)
        {
            if (!this.networkSimulationSettings.IsSimulationEnabled)
            {
                throw new NotImplementedException("ReceiveNetworkSimulated was called, despite NetworkSimulationSettings.IsSimulationEnabled == false.");
            }
            if (this.usedTransportProtocol == ConnectionProtocol.Udp && this.networkSimulationSettings.IncomingLossPercentage > 0 && this.lagRandomizer.Next(101) < this.networkSimulationSettings.IncomingLossPercentage)
            {
                this.networkSimulationSettings.LostPackagesIn++;
            }
            else
            {
                int num = (this.networkSimulationSettings.IncomingJitter > 0) ? (this.lagRandomizer.Next(this.networkSimulationSettings.IncomingJitter * 2) - this.networkSimulationSettings.IncomingJitter) : 0;
                int num2 = this.networkSimulationSettings.IncomingLag + num;
                int num3 = SupportClass.GetTickCount() + num2;
                SimulationItem value = new SimulationItem
                {
                    DelayedData = dataReceived,
                    TimeToExecute = num3,
                    Delay = num2
                };
                LinkedList<SimulationItem> netSimListIncoming = this.NetSimListIncoming;
                lock (netSimListIncoming)
                {
                    if (this.NetSimListIncoming.Count == 0 || this.usedTransportProtocol == ConnectionProtocol.Tcp)
                    {
                        this.NetSimListIncoming.AddLast(value);
                    }
                    else
                    {
                        LinkedListNode<SimulationItem> linkedListNode = this.NetSimListIncoming.First;
                        while (linkedListNode != null && linkedListNode.Value.TimeToExecute < num3)
                        {
                            linkedListNode = linkedListNode.Next;
                        }
                        if (linkedListNode == null)
                        {
                            this.NetSimListIncoming.AddLast(value);
                        }
                        else
                        {
                            this.NetSimListIncoming.AddBefore(linkedListNode, value);
                        }
                    }
                }
            }
        }

        protected internal void NetworkSimRun()
        {
            while (true)
            {
                bool flag = false;
                ManualResetEvent netSimManualResetEvent = this.networkSimulationSettings.NetSimManualResetEvent;
                lock (netSimManualResetEvent)
                {
                    flag = this.networkSimulationSettings.IsSimulationEnabled;
                }
                if (!flag)
                {
                    this.networkSimulationSettings.NetSimManualResetEvent.WaitOne();
                }
                else
                {
                    LinkedList<SimulationItem> netSimListIncoming = this.NetSimListIncoming;
                    lock (netSimListIncoming)
                    {
                        SimulationItem simulationItem = null;
                        while (this.NetSimListIncoming.First != null)
                        {
                            simulationItem = this.NetSimListIncoming.First.Value;
                            if (simulationItem.stopw.ElapsedMilliseconds < simulationItem.Delay)
                            {
                                break;
                            }
                            this.ReceiveIncomingCommands(simulationItem.DelayedData, simulationItem.DelayedData.Length);
                            this.NetSimListIncoming.RemoveFirst();
                        }
                    }
                    LinkedList<SimulationItem> netSimListOutgoing = this.NetSimListOutgoing;
                    lock (netSimListOutgoing)
                    {
                        SimulationItem simulationItem2 = null;
                        while (this.NetSimListOutgoing.First != null)
                        {
                            simulationItem2 = this.NetSimListOutgoing.First.Value;
                            if (simulationItem2.stopw.ElapsedMilliseconds < simulationItem2.Delay)
                            {
                                break;
                            }
                            if (this.PhotonSocket != null && this.PhotonSocket.Connected)
                            {
                                this.PhotonSocket.Send(simulationItem2.DelayedData, simulationItem2.DelayedData.Length);
                            }
                            this.NetSimListOutgoing.RemoveFirst();
                        }
                    }
                    Thread.Sleep(0);
                }
            }
        }

        internal void CommandLogResize()
        {
            if (this.CommandLogSize <= 0)
            {
                this.CommandLog = null;
                this.InReliableLog = null;
            }
            else
            {
                if (this.CommandLog == null || this.InReliableLog == null)
                {
                    this.CommandLogInit();
                }
                while (this.CommandLog.Count > 0 && this.CommandLog.Count > this.CommandLogSize)
                {
                    this.CommandLog.Dequeue();
                }
                while (this.InReliableLog.Count > 0 && this.InReliableLog.Count > this.CommandLogSize)
                {
                    this.InReliableLog.Dequeue();
                }
            }
        }

        internal void CommandLogInit()
        {
            if (this.CommandLogSize <= 0)
            {
                this.CommandLog = null;
                this.InReliableLog = null;
            }
            else if (this.CommandLog == null || this.InReliableLog == null)
            {
                this.CommandLog = new Queue<CmdLogItem>(this.CommandLogSize);
                this.InReliableLog = new Queue<CmdLogItem>(this.CommandLogSize);
            }
            else
            {
                this.CommandLog.Clear();
                this.InReliableLog.Clear();
            }
        }

        public string CommandLogToString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            int num = (this.usedTransportProtocol == ConnectionProtocol.Udp) ? ((EnetPeer)this).reliableCommandsRepeated : 0;
            stringBuilder.AppendFormat("PeerId: {0} Now: {1} Server: {2} State: {3} Total Resends: {4} Received {5}ms ago.\n", this.PeerID, this.timeInt, this.ServerAddress, this.peerConnectionState, num, SupportClass.GetTickCount() - this.timestampOfLastReceive);
            if (this.CommandLog == null)
            {
                return stringBuilder.ToString();
            }
            foreach (CmdLogItem item in this.CommandLog)
            {
                stringBuilder.AppendLine(item.ToString());
            }
            stringBuilder.AppendLine("Received Reliable Log: ");
            foreach (CmdLogItem item2 in this.InReliableLog)
            {
                stringBuilder.AppendLine(item2.ToString());
            }
            return stringBuilder.ToString();
        }
    }
}
