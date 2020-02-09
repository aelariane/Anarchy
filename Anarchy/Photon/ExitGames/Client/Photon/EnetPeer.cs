using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

namespace ExitGames.Client.Photon
{
    internal class EnetPeer : PeerBase
    {
        private const int CRC_LENGTH = 4;

        private static readonly int HMAC_SIZE = 32;

        private static readonly int BLOCK_SIZE = 16;

        private static readonly int IV_SIZE = 16;

        private const int EncryptedDataGramHeaderSize = 7;

        private const int EncryptedHeaderSize = 5;

        private List<NCommand> sentReliableCommands = new List<NCommand>();

        private StreamBuffer outgoingAcknowledgementsPool;

        internal const int UnsequencedWindowSize = 128;

        internal readonly int[] unsequencedWindow = new int[4];

        internal int outgoingUnsequencedGroupNumber;

        internal int incomingUnsequencedGroupNumber;

        private byte udpCommandCount;

        private byte[] udpBuffer;

        private int udpBufferIndex;

        private int udpBufferLength;

        private byte[] bufferForEncryption;

        private int commandBufferSize = 100;

        internal int challenge;

        internal int reliableCommandsRepeated;

        internal int reliableCommandsSent;

        internal int serverSentTime;

        internal static readonly byte[] udpHeader0xF3 = new byte[2]
        {
        243,
        2
        };

        internal static readonly byte[] messageHeader = EnetPeer.udpHeader0xF3;

        protected bool datagramEncryptedConnection;

        private EnetChannel[] channelArray = new EnetChannel[0];

        private const byte ControlChannelNumber = 255;

        protected internal const short PeerIdForConnect = -1;

        protected internal const short PeerIdForConnectTrace = -2;

        private Queue<int> commandsToRemove = new Queue<int>();

        private int fragmentLength = 0;

        private int fragmentLengthDatagramEncrypt = 0;

        private int fragmentLengthMtuValue = 0;

        private Queue<NCommand> commandsToResend = new Queue<NCommand>();

        private Queue<NCommand> CommandQueue = new Queue<NCommand>();

        internal override int QueuedIncomingCommandsCount
        {
            get
            {
                int num = 0;
                EnetChannel[] obj = this.channelArray;
                lock (obj)
                {
                    for (int i = 0; i < this.channelArray.Length; i++)
                    {
                        EnetChannel enetChannel = this.channelArray[i];
                        num += enetChannel.incomingReliableCommandsList.Count;
                        num += enetChannel.incomingUnreliableCommandsList.Count;
                    }
                }
                return num;
            }
        }

        internal override int QueuedOutgoingCommandsCount
        {
            get
            {
                int num = 0;
                EnetChannel[] obj = this.channelArray;
                lock (obj)
                {
                    for (int i = 0; i < this.channelArray.Length; i++)
                    {
                        EnetChannel enetChannel = this.channelArray[i];
                        num += enetChannel.outgoingReliableCommandsList.Count;
                        num += enetChannel.outgoingUnreliableCommandsList.Count;
                    }
                }
                return num;
            }
        }

        internal override int SentReliableCommandsCount
        {
            get
            {
                return this.sentReliableCommands.Count;
            }
        }

        internal EnetPeer()
        {
            base.TrafficPackageHeaderSize = 12;
        }

        internal override void InitPeerBase()
        {
            base.InitPeerBase();
            if (base.photonPeer.PayloadEncryptionSecret != null && base.usedTransportProtocol == ConnectionProtocol.Udp)
            {
                this.InitEncryption(base.photonPeer.PayloadEncryptionSecret);
            }
            if (base.photonPeer.DgramEncryptor != null && base.photonPeer.DgramDecryptor != null)
            {
                base.isEncryptionAvailable = true;
            }
            base.peerID = (short)(base.photonPeer.EnableServerTracing ? (-2) : (-1));
            this.challenge = SupportClass.ThreadSafeRandom.Next();
            if (this.udpBuffer == null || this.udpBuffer.Length != base.mtu)
            {
                this.udpBuffer = new byte[base.mtu];
            }
            this.reliableCommandsSent = 0;
            this.reliableCommandsRepeated = 0;
            EnetChannel[] obj = this.channelArray;
            lock (obj)
            {
                EnetChannel[] array = this.channelArray;
                if (array.Length != base.ChannelCount + 1)
                {
                    array = new EnetChannel[base.ChannelCount + 1];
                }
                for (byte b = 0; b < base.ChannelCount; b = (byte)(b + 1))
                {
                    array[b] = new EnetChannel(b, this.commandBufferSize);
                }
                array[base.ChannelCount] = new EnetChannel(255, this.commandBufferSize);
                this.channelArray = array;
            }
            List<NCommand> obj2 = this.sentReliableCommands;
            lock (obj2)
            {
                this.sentReliableCommands = new List<NCommand>(this.commandBufferSize);
            }
            this.outgoingAcknowledgementsPool = new StreamBuffer(0);
            base.CommandLogInit();
        }

        internal void ApplyRandomizedSequenceNumbers()
        {
            if (base.photonPeer.RandomizeSequenceNumbers)
            {
                EnetChannel[] obj = this.channelArray;
                lock (obj)
                {
                    EnetChannel[] array = this.channelArray;
                    foreach (EnetChannel enetChannel in array)
                    {
                        int num = base.photonPeer.RandomizedSequenceNumbers[(int)enetChannel.ChannelNumber % base.photonPeer.RandomizedSequenceNumbers.Length];
                        string debugReturn = string.Format("Channel {0} seqNr in: {1} out: {2}. randomize value: {3}", enetChannel.ChannelNumber, enetChannel.incomingReliableSequenceNumber, enetChannel.outgoingReliableSequenceNumber, num);
                        base.EnqueueDebugReturn(DebugLevel.INFO, debugReturn);
                        enetChannel.incomingReliableSequenceNumber = num;
                        enetChannel.outgoingReliableSequenceNumber = num;
                        enetChannel.outgoingReliableUnsequencedNumber = num;
                    }
                }
            }
        }

        internal override bool Connect(string ipport, string appID, object custom = null)
        {
            if (base.peerConnectionState != 0)
            {
                base.Listener.DebugReturn(DebugLevel.WARNING, "Connect() can't be called if peer is not Disconnected. Not connecting. peerConnectionState: " + base.peerConnectionState);
                return false;
            }
            if ((int)base.debugOut >= 5)
            {
                base.Listener.DebugReturn(DebugLevel.ALL, "Connect()");
            }
            base.ServerAddress = ipport;
            this.InitPeerBase();
            if (base.SocketImplementation != null)
            {
                base.PhotonSocket = (IPhotonSocket)Activator.CreateInstance(base.SocketImplementation, this);
            }
            else
            {
                base.PhotonSocket = new SocketUdp(this);
            }
            if (base.PhotonSocket == null)
            {
                base.Listener.DebugReturn(DebugLevel.ERROR, "Connect() failed, because SocketImplementation or socket was null. Set PhotonPeer.SocketImplementation before Connect().");
                return false;
            }
            if (base.PhotonSocket.Connect())
            {
                if (base.TrafficStatsEnabled)
                {
                    base.TrafficStatsOutgoing.ControlCommandBytes += 44;
                    base.TrafficStatsOutgoing.ControlCommandCount++;
                }
                base.peerConnectionState = ConnectionStateValue.Connecting;
                return true;
            }
            return false;
        }

        public override void OnConnect()
        {
            this.QueueOutgoingReliableCommand(new NCommand(this, 2, null, 255));
        }

        internal override void Disconnect()
        {
            if (base.peerConnectionState != 0 && base.peerConnectionState != ConnectionStateValue.Disconnecting)
            {
                if (this.sentReliableCommands != null)
                {
                    List<NCommand> obj = this.sentReliableCommands;
                    lock (obj)
                    {
                        this.sentReliableCommands.Clear();
                    }
                }
                EnetChannel[] obj2 = this.channelArray;
                lock (obj2)
                {
                    EnetChannel[] array = this.channelArray;
                    foreach (EnetChannel enetChannel in array)
                    {
                        enetChannel.clearAll();
                    }
                }
                bool isSimulationEnabled = base.NetworkSimulationSettings.IsSimulationEnabled;
                base.NetworkSimulationSettings.IsSimulationEnabled = false;
                NCommand nCommand = new NCommand(this, 4, null, 255);
                this.QueueOutgoingReliableCommand(nCommand);
                this.SendOutgoingCommands();
                if (base.TrafficStatsEnabled)
                {
                    base.TrafficStatsOutgoing.CountControlCommand(nCommand.Size);
                }
                base.NetworkSimulationSettings.IsSimulationEnabled = isSimulationEnabled;
                base.PhotonSocket.Disconnect();
                base.peerConnectionState = ConnectionStateValue.Disconnected;
                base.EnqueueStatusCallback(StatusCode.Disconnect);
                this.datagramEncryptedConnection = false;
            }
        }

        internal override void StopConnection()
        {
            if (base.PhotonSocket != null)
            {
                base.PhotonSocket.Disconnect();
            }
            base.peerConnectionState = ConnectionStateValue.Disconnected;
            if (base.Listener != null)
            {
                base.Listener.OnStatusChanged(StatusCode.Disconnect);
            }
        }

        internal override void FetchServerTimestamp()
        {
            if (base.peerConnectionState != ConnectionStateValue.Connected || !base.ApplicationIsInitialized)
            {
                if ((int)base.debugOut >= 3)
                {
                    base.EnqueueDebugReturn(DebugLevel.INFO, "FetchServerTimestamp() was skipped, as the client is not connected. Current ConnectionState: " + base.peerConnectionState);
                }
            }
            else
            {
                this.CreateAndEnqueueCommand(12, null, 255);
            }
        }

        internal override bool DispatchIncomingCommands()
        {
            int count = this.CommandQueue.Count;
            if (count > 0)
            {
                for (int i = 0; i < count; i++)
                {
                    Queue<NCommand> commandQueue = this.CommandQueue;
                    lock (commandQueue)
                    {
                        NCommand command = this.CommandQueue.Dequeue();
                        this.ExecuteCommand(command);
                    }
                }
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
                        goto IL_00a8;
                    }
                }
                break;
            IL_00a8:
                myAction();
            }
            NCommand nCommand = null;
            EnetChannel[] obj = this.channelArray;
            lock (obj)
            {
                for (int j = 0; j < this.channelArray.Length; j++)
                {
                    EnetChannel enetChannel = this.channelArray[j];
                    if (enetChannel.incomingUnsequencedCommandsList.Count > 0)
                    {
                        nCommand = enetChannel.incomingUnsequencedCommandsList.Dequeue();
                        break;
                    }
                    if (enetChannel.incomingUnreliableCommandsList.Count > 0)
                    {
                        if (!enetChannel.incomingUnreliableCommandsList.TryGetValue(enetChannel.incomingUnreliableSequenceNumber + 1, out nCommand))
                        {
                            int num = 2147483647;
                            foreach (int key in enetChannel.incomingUnreliableCommandsList.Keys)
                            {
                                NCommand nCommand2 = enetChannel.incomingUnreliableCommandsList[key];
                                if (key < enetChannel.incomingUnreliableSequenceNumber || nCommand2.reliableSequenceNumber < enetChannel.incomingReliableSequenceNumber)
                                {
                                    this.commandsToRemove.Enqueue(key);
                                }
                                else if (key < num && nCommand2.reliableSequenceNumber <= enetChannel.incomingReliableSequenceNumber)
                                {
                                    num = key;
                                }
                            }
                            while (this.commandsToRemove.Count > 0)
                            {
                                enetChannel.incomingUnreliableCommandsList.Remove(this.commandsToRemove.Dequeue());
                            }
                            if (num < 2147483647)
                            {
                                nCommand = enetChannel.incomingUnreliableCommandsList[num];
                            }
                        }
                        if (nCommand != null)
                        {
                            enetChannel.incomingUnreliableCommandsList.Remove(nCommand.unreliableSequenceNumber);
                            enetChannel.incomingUnreliableSequenceNumber = nCommand.unreliableSequenceNumber;
                            break;
                        }
                    }
                    if (nCommand == null && enetChannel.incomingReliableCommandsList.Count > 0)
                    {
                        enetChannel.incomingReliableCommandsList.TryGetValue(enetChannel.incomingReliableSequenceNumber + 1, out nCommand);
                        if (nCommand != null)
                        {
                            if (nCommand.commandType != 8)
                            {
                                enetChannel.incomingReliableSequenceNumber = nCommand.reliableSequenceNumber;
                                enetChannel.incomingReliableCommandsList.Remove(nCommand.reliableSequenceNumber);
                            }
                            else if (nCommand.fragmentsRemaining > 0)
                            {
                                nCommand = null;
                            }
                            else
                            {
                                enetChannel.incomingReliableSequenceNumber = nCommand.reliableSequenceNumber + nCommand.fragmentCount - 1;
                                enetChannel.incomingReliableCommandsList.Remove(nCommand.reliableSequenceNumber);
                            }
                            break;
                        }
                    }
                }
            }
            if (nCommand != null && nCommand.Payload != null)
            {
                base.ByteCountCurrentDispatch = nCommand.Size;
                base.CommandInCurrentDispatch = nCommand;
                if (this.DeserializeMessageAndCallback(nCommand.Payload))
                {
                    nCommand.FreePayload();
                    base.CommandInCurrentDispatch = null;
                    return true;
                }
                base.CommandInCurrentDispatch = null;
            }
            return false;
        }

        private int GetFragmentLength()
        {
            if (this.fragmentLength == 0 || base.mtu != this.fragmentLengthMtuValue)
            {
                this.fragmentLengthMtuValue = base.mtu;
                this.fragmentLength = base.mtu - 12 - 36;
                int mtu = base.mtu;
                mtu = mtu - 7 - EnetPeer.HMAC_SIZE - EnetPeer.IV_SIZE;
                mtu = mtu / EnetPeer.BLOCK_SIZE * EnetPeer.BLOCK_SIZE;
                mtu = (this.fragmentLengthDatagramEncrypt = mtu - 5 - 36);
            }
            return this.datagramEncryptedConnection ? this.fragmentLengthDatagramEncrypt : this.fragmentLength;
        }

        private int CalculateBufferLen()
        {
            int mtu = base.mtu;
            if (this.datagramEncryptedConnection)
            {
                mtu = mtu - 7 - EnetPeer.HMAC_SIZE - EnetPeer.IV_SIZE;
                mtu = mtu / EnetPeer.BLOCK_SIZE * EnetPeer.BLOCK_SIZE;
                return mtu - 1;
            }
            return mtu;
        }

        private int CalculateInitialOffset()
        {
            if (this.datagramEncryptedConnection)
            {
                return 5;
            }
            int num = 12;
            if (base.photonPeer.CrcEnabled)
            {
                num += 4;
            }
            return num;
        }

        internal override bool SendAcksOnly()
        {
            if (base.peerConnectionState == ConnectionStateValue.Disconnected)
            {
                return false;
            }
            if (base.PhotonSocket == null || !base.PhotonSocket.Connected)
            {
                return false;
            }
            byte[] obj = this.udpBuffer;
            lock (obj)
            {
                int num = 0;
                this.udpBufferIndex = this.CalculateInitialOffset();
                this.udpBufferLength = this.CalculateBufferLen();
                this.udpCommandCount = 0;
                base.timeInt = SupportClass.GetTickCount() - base.timeBase;
                StreamBuffer obj2 = this.outgoingAcknowledgementsPool;
                lock (obj2)
                {
                    num = this.SerializeAckToBuffer();
                    base.timeLastSendAck = base.timeInt;
                }
                if (base.timeInt > base.timeoutInt && this.sentReliableCommands.Count > 0)
                {
                    List<NCommand> obj3 = this.sentReliableCommands;
                    lock (obj3)
                    {
                        foreach (NCommand sentReliableCommand in this.sentReliableCommands)
                        {
                            if (sentReliableCommand != null && sentReliableCommand.roundTripTimeout != 0 && base.timeInt - sentReliableCommand.commandSentTime > sentReliableCommand.roundTripTimeout)
                            {
                                sentReliableCommand.commandSentCount = 1;
                                sentReliableCommand.roundTripTimeout = 0;
                                sentReliableCommand.timeoutTime = 2147483647;
                                sentReliableCommand.commandSentTime = base.timeInt;
                            }
                        }
                    }
                }
                if (this.udpCommandCount <= 0)
                {
                    return false;
                }
                if (base.TrafficStatsEnabled)
                {
                    base.TrafficStatsOutgoing.TotalPacketCount++;
                    base.TrafficStatsOutgoing.TotalCommandsInPackets += this.udpCommandCount;
                }
                this.SendData(this.udpBuffer, this.udpBufferIndex);
                return num > 0;
            }
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
            byte[] obj = this.udpBuffer;
            lock (obj)
            {
                int num = 0;
                this.udpBufferIndex = this.CalculateInitialOffset();
                this.udpBufferLength = this.CalculateBufferLen();
                this.udpCommandCount = 0;
                base.timeInt = SupportClass.GetTickCount() - base.timeBase;
                base.timeLastSendOutgoing = base.timeInt;
                StreamBuffer obj2 = this.outgoingAcknowledgementsPool;
                lock (obj2)
                {
                    if (this.outgoingAcknowledgementsPool.IntLength > 0)
                    {
                        num = this.SerializeAckToBuffer();
                        base.timeLastSendAck = base.timeInt;
                    }
                }
                if (!base.IsSendingOnlyAcks && base.timeInt > base.timeoutInt && this.sentReliableCommands.Count > 0)
                {
                    List<NCommand> obj3 = this.sentReliableCommands;
                    lock (obj3)
                    {
                        this.commandsToResend.Clear();
                        for (int i = 0; i < this.sentReliableCommands.Count; i++)
                        {
                            NCommand nCommand = this.sentReliableCommands[i];
                            if (nCommand != null && base.timeInt - nCommand.commandSentTime > nCommand.roundTripTimeout)
                            {
                                if (nCommand.commandSentCount > base.photonPeer.SentCountAllowance || base.timeInt > nCommand.timeoutTime)
                                {
                                    if ((int)base.debugOut >= 2)
                                    {
                                        base.Listener.DebugReturn(DebugLevel.WARNING, "Timeout-disconnect! Command: " + nCommand + " now: " + base.timeInt + " challenge: " + Convert.ToString(this.challenge, 16));
                                    }
                                    if (base.CommandLog != null)
                                    {
                                        base.CommandLog.Enqueue(new CmdLogSentReliable(nCommand, base.timeInt, base.roundTripTime, base.roundTripTimeVariance, true));
                                        base.CommandLogResize();
                                    }
                                    base.peerConnectionState = ConnectionStateValue.Zombie;
                                    base.EnqueueStatusCallback(StatusCode.TimeoutDisconnect);
                                    this.Disconnect();
                                    return false;
                                }
                                this.commandsToResend.Enqueue(nCommand);
                            }
                        }
                        while (this.commandsToResend.Count > 0)
                        {
                            NCommand nCommand2 = this.commandsToResend.Dequeue();
                            this.QueueOutgoingReliableCommand(nCommand2);
                            this.sentReliableCommands.Remove(nCommand2);
                            this.reliableCommandsRepeated++;
                            if ((int)base.debugOut >= 3)
                            {
                                base.Listener.DebugReturn(DebugLevel.INFO, string.Format("Resending: {0}. times out after: {1} sent: {3} now: {2} rtt/var: {4}/{5} last recv: {6}", nCommand2, nCommand2.roundTripTimeout, base.timeInt, nCommand2.commandSentTime, base.roundTripTime, base.roundTripTimeVariance, SupportClass.GetTickCount() - base.timestampOfLastReceive));
                            }
                        }
                    }
                }
                if (!base.IsSendingOnlyAcks && base.peerConnectionState == ConnectionStateValue.Connected && base.timePingInterval > 0 && this.sentReliableCommands.Count == 0 && base.timeInt - base.timeLastAckReceive > base.timePingInterval && !this.AreReliableCommandsInTransit() && this.udpBufferIndex + 12 < this.udpBufferLength)
                {
                    NCommand nCommand3 = new NCommand(this, 5, null, 255);
                    this.QueueOutgoingReliableCommand(nCommand3);
                    if (base.TrafficStatsEnabled)
                    {
                        base.TrafficStatsOutgoing.CountControlCommand(nCommand3.Size);
                    }
                }
                if (!base.IsSendingOnlyAcks)
                {
                    EnetChannel[] obj4 = this.channelArray;
                    lock (obj4)
                    {
                        for (int j = 0; j < this.channelArray.Length; j++)
                        {
                            EnetChannel enetChannel = this.channelArray[j];
                            num += this.SerializeToBuffer(enetChannel.outgoingReliableCommandsList);
                            num += this.SerializeToBuffer(enetChannel.outgoingUnreliableCommandsList);
                        }
                    }
                }
                if (this.udpCommandCount <= 0)
                {
                    return false;
                }
                if (base.TrafficStatsEnabled)
                {
                    base.TrafficStatsOutgoing.TotalPacketCount++;
                    base.TrafficStatsOutgoing.TotalCommandsInPackets += this.udpCommandCount;
                }
                this.SendData(this.udpBuffer, this.udpBufferIndex);
                return num > 0;
            }
        }

        private bool AreReliableCommandsInTransit()
        {
            EnetChannel[] obj = this.channelArray;
            lock (obj)
            {
                for (int i = 0; i < this.channelArray.Length; i++)
                {
                    EnetChannel enetChannel = this.channelArray[i];
                    if (enetChannel.outgoingReliableCommandsList.Count > 0)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        internal override bool EnqueueOperation(Dictionary<byte, object> parameters, byte opCode, SendOptions sendParams, EgMessageType messageType = EgMessageType.Operation)
        {
            if (base.peerConnectionState != ConnectionStateValue.Connected)
            {
                if ((int)base.debugOut >= 1)
                {
                    base.Listener.DebugReturn(DebugLevel.ERROR, "Cannot send op: " + opCode + " Not connected. PeerState: " + base.peerConnectionState);
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
            byte commandType = 7;
            if (sendParams.DeliveryMode == DeliveryMode.UnreliableUnsequenced)
            {
                commandType = 11;
            }
            else if (sendParams.DeliveryMode == DeliveryMode.ReliableUnsequenced)
            {
                commandType = 14;
            }
            else if (sendParams.DeliveryMode == DeliveryMode.Reliable)
            {
                commandType = 6;
            }
            StreamBuffer payload = this.SerializeOperationToMessage(opCode, parameters, messageType, sendParams.Encrypt);
            return this.CreateAndEnqueueCommand(commandType, payload, sendParams.Channel);
        }

        internal override bool EnqueueMessage(object message, SendOptions sendOptions)
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
            byte commandType = 7;
            if (sendOptions.DeliveryMode == DeliveryMode.UnreliableUnsequenced)
            {
                commandType = 11;
            }
            else if (sendOptions.DeliveryMode == DeliveryMode.ReliableUnsequenced)
            {
                commandType = 14;
            }
            else if (sendOptions.DeliveryMode == DeliveryMode.Reliable)
            {
                commandType = 6;
            }
            StreamBuffer payload = base.SerializeMessageToMessage(message, sendOptions.Encrypt, EnetPeer.messageHeader, false);
            return this.CreateAndEnqueueCommand(commandType, payload, channel);
        }

        private EnetChannel GetChannel(byte channelNumber)
        {
            return (channelNumber == 255) ? this.channelArray[this.channelArray.Length - 1] : this.channelArray[channelNumber];
        }

        internal bool CreateAndEnqueueCommand(byte commandType, StreamBuffer payload, byte channelNumber)
        {
            EnetChannel channel = this.GetChannel(channelNumber);
            base.ByteCountLastOperation = 0;
            int num = this.GetFragmentLength();
            if (payload == null || payload.IntLength <= num)
            {
                NCommand nCommand = new NCommand(this, commandType, payload, channel.ChannelNumber);
                if (nCommand.IsFlaggedReliable)
                {
                    this.QueueOutgoingReliableCommand(nCommand);
                    base.ByteCountLastOperation = nCommand.Size;
                    if (base.TrafficStatsEnabled)
                    {
                        base.TrafficStatsOutgoing.CountReliableOpCommand(nCommand.Size);
                        base.TrafficStatsGameLevel.CountOperation(nCommand.Size);
                    }
                }
                else
                {
                    this.QueueOutgoingUnreliableCommand(nCommand);
                    base.ByteCountLastOperation = nCommand.Size;
                    if (base.TrafficStatsEnabled)
                    {
                        base.TrafficStatsOutgoing.CountUnreliableOpCommand(nCommand.Size);
                        base.TrafficStatsGameLevel.CountOperation(nCommand.Size);
                    }
                }
            }
            else
            {
                bool flag = commandType == 14 || commandType == 11;
                int fragmentCount = (payload.IntLength + num - 1) / num;
                int startSequenceNumber = (flag ? channel.outgoingReliableUnsequencedNumber : channel.outgoingReliableSequenceNumber) + 1;
                byte[] buffer = payload.GetBuffer();
                int num2 = 0;
                for (int i = 0; i < payload.IntLength; i += num)
                {
                    if (payload.IntLength - i < num)
                    {
                        num = payload.IntLength - i;
                    }
                    StreamBuffer streamBuffer = PeerBase.MessageBufferPoolGet();
                    streamBuffer.Write(buffer, i, num);
                    NCommand nCommand2 = new NCommand(this, (byte)(flag ? 15 : 8), streamBuffer, channel.ChannelNumber);
                    nCommand2.fragmentNumber = num2;
                    nCommand2.startSequenceNumber = startSequenceNumber;
                    nCommand2.fragmentCount = fragmentCount;
                    nCommand2.totalLength = payload.IntLength;
                    nCommand2.fragmentOffset = i;
                    this.QueueOutgoingReliableCommand(nCommand2);
                    base.ByteCountLastOperation += nCommand2.Size;
                    if (base.TrafficStatsEnabled)
                    {
                        base.TrafficStatsOutgoing.CountFragmentOpCommand(nCommand2.Size);
                        base.TrafficStatsGameLevel.CountOperation(nCommand2.Size);
                    }
                    num2++;
                }
            }
            return true;
        }

        internal override StreamBuffer SerializeOperationToMessage(byte opCode, Dictionary<byte, object> parameters, EgMessageType messageType, bool encrypt)
        {
            encrypt = (encrypt && !this.datagramEncryptedConnection);
            StreamBuffer streamBuffer = PeerBase.MessageBufferPoolGet();
            streamBuffer.SetLength(0L);
            if (!encrypt)
            {
                streamBuffer.Write(EnetPeer.messageHeader, 0, EnetPeer.messageHeader.Length);
            }
            base.SerializationProtocol.SerializeOperationRequest(streamBuffer, opCode, parameters, false);
            if (encrypt)
            {
                byte[] array = base.CryptoProvider.Encrypt(streamBuffer.GetBuffer(), 0, streamBuffer.IntLength);
                streamBuffer.SetLength(0L);
                streamBuffer.Write(EnetPeer.messageHeader, 0, EnetPeer.messageHeader.Length);
                streamBuffer.Write(array, 0, array.Length);
            }
            byte[] buffer = streamBuffer.GetBuffer();
            if (messageType != EgMessageType.Operation)
            {
                buffer[EnetPeer.messageHeader.Length - 1] = (byte)messageType;
            }
            if (encrypt)
            {
                buffer[EnetPeer.messageHeader.Length - 1] = (byte)(buffer[EnetPeer.messageHeader.Length - 1] | 0x80);
            }
            return streamBuffer;
        }

        internal int SerializeAckToBuffer()
        {
            this.outgoingAcknowledgementsPool.Seek(0L, SeekOrigin.Begin);
            while (this.outgoingAcknowledgementsPool.IntPosition + 20 <= this.outgoingAcknowledgementsPool.IntLength)
            {
                if (this.udpBufferIndex + 20 <= this.udpBufferLength)
                {
                    int srcOffset = default(int);
                    byte[] bufferAndAdvance = this.outgoingAcknowledgementsPool.GetBufferAndAdvance(20, out srcOffset);
                    Buffer.BlockCopy(bufferAndAdvance, srcOffset, this.udpBuffer, this.udpBufferIndex, 20);
                    this.udpBufferIndex += 20;
                    this.udpCommandCount++;
                    continue;
                }
                if ((int)base.debugOut < 3)
                {
                    break;
                }
                base.Listener.DebugReturn(DebugLevel.INFO, "UDP package is full. Commands in Package: " + this.udpCommandCount + ". bytes left in queue: " + this.outgoingAcknowledgementsPool.IntPosition);
                break;
            }
            this.outgoingAcknowledgementsPool.Compact();
            this.outgoingAcknowledgementsPool.IntPosition = this.outgoingAcknowledgementsPool.IntLength;
            return this.outgoingAcknowledgementsPool.IntLength / 20;
        }

        internal int SerializeToBuffer(Queue<NCommand> commandList)
        {
            while (commandList.Count > 0)
            {
                NCommand nCommand = commandList.Peek();
                if (nCommand == null)
                {
                    commandList.Dequeue();
                }
                else
                {
                    if (this.udpBufferIndex + nCommand.Size > this.udpBufferLength)
                    {
                        if ((int)base.debugOut >= 3)
                        {
                            base.Listener.DebugReturn(DebugLevel.INFO, "UDP package is full. Commands in Package: " + this.udpCommandCount + ". Commands left in queue: " + commandList.Count);
                        }
                        break;
                    }
                    nCommand.SerializeHeader(this.udpBuffer, ref this.udpBufferIndex);
                    if (nCommand.SizeOfPayload > 0)
                    {
                        Buffer.BlockCopy(nCommand.Serialize(), 0, this.udpBuffer, this.udpBufferIndex, nCommand.SizeOfPayload);
                        this.udpBufferIndex += nCommand.SizeOfPayload;
                    }
                    this.udpCommandCount++;
                    if (nCommand.IsFlaggedReliable)
                    {
                        this.QueueSentCommand(nCommand);
                        if (base.CommandLog != null)
                        {
                            base.CommandLog.Enqueue(new CmdLogSentReliable(nCommand, base.timeInt, base.roundTripTime, base.roundTripTimeVariance, false));
                            base.CommandLogResize();
                        }
                    }
                    else
                    {
                        nCommand.FreePayload();
                    }
                    commandList.Dequeue();
                }
            }
            return commandList.Count;
        }

        internal void SendData(byte[] data, int length)
        {
            try
            {
                if (this.datagramEncryptedConnection)
                {
                    this.SendDataEncrypted(data, length);
                }
                else
                {
                    int num = 0;
                    Protocol.Serialize(base.peerID, data, ref num);
                    data[2] = (byte)(base.photonPeer.CrcEnabled ? 204 : 0);
                    data[3] = this.udpCommandCount;
                    num = 4;
                    Protocol.Serialize(base.timeInt, data, ref num);
                    Protocol.Serialize(this.challenge, data, ref num);
                    if (base.photonPeer.CrcEnabled)
                    {
                        Protocol.Serialize(0, data, ref num);
                        uint value = SupportClass.CalculateCrc(data, length);
                        num -= 4;
                        Protocol.Serialize((int)value, data, ref num);
                    }
                    base.bytesOut += length;
                    this.SendToSocket(data, length);
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

        private void SendToSocket(byte[] data, int length)
        {
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

        private void SendDataEncrypted(byte[] data, int length)
        {
            if (this.bufferForEncryption == null || this.bufferForEncryption.Length != base.mtu)
            {
                this.bufferForEncryption = new byte[base.mtu];
            }
            byte[] array = this.bufferForEncryption;
            int num = 0;
            Protocol.Serialize(base.peerID, array, ref num);
            array[2] = 1;
            num++;
            Protocol.Serialize(this.challenge, array, ref num);
            data[0] = this.udpCommandCount;
            int num2 = 1;
            Protocol.Serialize(base.timeInt, data, ref num2);
            base.photonPeer.DgramEncryptor.Encrypt(data, length, array, ref num);
            Buffer.BlockCopy(base.photonPeer.DgramEncryptor.FinishHMAC(array, 0, num), 0, array, num, EnetPeer.HMAC_SIZE);
            this.SendToSocket(array, num + EnetPeer.HMAC_SIZE);
        }

        internal void QueueSentCommand(NCommand command)
        {
            command.commandSentTime = base.timeInt;
            command.commandSentCount++;
            if (command.roundTripTimeout == 0)
            {
                command.roundTripTimeout = base.roundTripTime + 4 * base.roundTripTimeVariance;
                command.timeoutTime = base.timeInt + base.DisconnectTimeout;
            }
            else if (command.commandSentCount > base.photonPeer.QuickResendAttempts + 1)
            {
                command.roundTripTimeout *= 2;
            }
            List<NCommand> obj = this.sentReliableCommands;
            lock (obj)
            {
                if (this.sentReliableCommands.Count == 0)
                {
                    int num = command.commandSentTime + command.roundTripTimeout;
                    if (num < base.timeoutInt)
                    {
                        base.timeoutInt = num;
                    }
                }
                this.reliableCommandsSent++;
                this.sentReliableCommands.Add(command);
            }
        }

        internal void QueueOutgoingReliableCommand(NCommand command)
        {
            EnetChannel channel = this.GetChannel(command.commandChannelID);
            EnetChannel obj = channel;
            lock (obj)
            {
                if (command.reliableSequenceNumber == 0)
                {
                    if (command.IsFlaggedUnsequenced)
                    {
                        command.reliableSequenceNumber = ++channel.outgoingReliableUnsequencedNumber;
                    }
                    else
                    {
                        command.reliableSequenceNumber = ++channel.outgoingReliableSequenceNumber;
                    }
                }
                channel.outgoingReliableCommandsList.Enqueue(command);
            }
        }

        internal void QueueOutgoingUnreliableCommand(NCommand command)
        {
            EnetChannel channel = this.GetChannel(command.commandChannelID);
            EnetChannel obj = channel;
            lock (obj)
            {
                if (command.IsFlaggedUnsequenced)
                {
                    command.reliableSequenceNumber = 0;
                    command.unsequencedGroupNumber = ++this.outgoingUnsequencedGroupNumber;
                }
                else
                {
                    command.reliableSequenceNumber = channel.outgoingReliableSequenceNumber;
                    command.unreliableSequenceNumber = ++channel.outgoingUnreliableSequenceNumber;
                }
                channel.outgoingUnreliableCommandsList.Enqueue(command);
            }
        }

        internal void QueueOutgoingAcknowledgement(NCommand readCommand, int sendTime)
        {
            StreamBuffer obj = this.outgoingAcknowledgementsPool;
            lock (obj)
            {
                int offset = default(int);
                byte[] bufferAndAdvance = this.outgoingAcknowledgementsPool.GetBufferAndAdvance(20, out offset);
                NCommand.CreateAck(bufferAndAdvance, offset, readCommand, sendTime);
            }
        }

        internal override void ReceiveIncomingCommands(byte[] inBuff, int dataLength)
        {
            base.timestampOfLastReceive = SupportClass.GetTickCount();
            try
            {
                int num = 0;
                short num2 = default(short);
                Protocol.Deserialize(out num2, inBuff, ref num);
                byte b = inBuff[num++];
                int num4 = default(int);
                byte b2;
                if (b == 1)
                {
                    if (base.photonPeer.DgramDecryptor == null)
                    {
                        base.EnqueueDebugReturn(DebugLevel.ERROR, "Got encrypted packet, but encryption is not set up. Packet ignored");
                        goto end_IL_000c;
                    }
                    this.datagramEncryptedConnection = true;
                    if (!base.photonPeer.DgramDecryptor.CheckHMAC(inBuff, dataLength))
                    {
                        base.packetLossByCrc++;
                        if (base.peerConnectionState != 0 && (int)base.debugOut >= 3)
                        {
                            base.EnqueueDebugReturn(DebugLevel.INFO, "Ignored package due to wrong HMAC.");
                        }
                        goto end_IL_000c;
                    }
                    Protocol.Deserialize(out num4, inBuff, ref num);
                    inBuff = base.photonPeer.DgramDecryptor.DecryptBufferWithIV(inBuff, num, dataLength - num - EnetPeer.HMAC_SIZE, out dataLength);
                    dataLength = inBuff.Length;
                    num = 0;
                    b2 = inBuff[num++];
                    Protocol.Deserialize(out this.serverSentTime, inBuff, ref num);
                    base.bytesIn += 12 + EnetPeer.IV_SIZE + EnetPeer.HMAC_SIZE + dataLength + (EnetPeer.BLOCK_SIZE - dataLength % EnetPeer.BLOCK_SIZE);
                }
                else
                {
                    if (this.datagramEncryptedConnection)
                    {
                        base.EnqueueDebugReturn(DebugLevel.WARNING, "Got not encrypted packet, but expected only encrypted. Packet ignored");
                        goto end_IL_000c;
                    }
                    b2 = inBuff[num++];
                    Protocol.Deserialize(out this.serverSentTime, inBuff, ref num);
                    Protocol.Deserialize(out num4, inBuff, ref num);
                    if (b == 204)
                    {
                        int num7 = default(int);
                        Protocol.Deserialize(out num7, inBuff, ref num);
                        base.bytesIn += 4L;
                        num -= 4;
                        Protocol.Serialize(0, inBuff, ref num);
                        uint num8 = SupportClass.CalculateCrc(inBuff, dataLength);
                        if (num7 != (int)num8)
                        {
                            base.packetLossByCrc++;
                            if (base.peerConnectionState != 0 && (int)base.debugOut >= 3)
                            {
                                base.EnqueueDebugReturn(DebugLevel.INFO, string.Format("Ignored package due to wrong CRC. Incoming:  {0:X} Local: {1:X}", (uint)num7, num8));
                            }
                            goto end_IL_000c;
                        }
                    }
                    base.bytesIn += 12L;
                }
                if (base.TrafficStatsEnabled)
                {
                    base.TrafficStatsIncoming.TotalPacketCount++;
                    base.TrafficStatsIncoming.TotalCommandsInPackets += b2;
                }
                if (b2 > this.commandBufferSize || b2 <= 0)
                {
                    base.EnqueueDebugReturn(DebugLevel.ERROR, "too many/few incoming commands in package: " + b2 + " > " + this.commandBufferSize);
                }
                if (num4 != this.challenge)
                {
                    base.packetLossByChallenge++;
                    if (base.peerConnectionState != 0 && (int)base.debugOut >= 5)
                    {
                        base.EnqueueDebugReturn(DebugLevel.ALL, "Info: Ignoring received package due to wrong challenge. Challenge in-package!=local:" + num4 + "!=" + this.challenge + " Commands in it: " + b2);
                    }
                }
                else
                {
                    base.timeInt = SupportClass.GetTickCount() - base.timeBase;
                    for (int i = 0; i < b2; i++)
                    {
                        NCommand nCommand = new NCommand(this, inBuff, ref num);
                        if (nCommand.commandType != 1 && nCommand.commandType != 16)
                        {
                            Queue<NCommand> commandQueue = this.CommandQueue;
                            lock (commandQueue)
                            {
                                this.CommandQueue.Enqueue(nCommand);
                            }
                        }
                        else
                        {
                            this.ExecuteCommand(nCommand);
                        }
                        if (nCommand.IsFlaggedReliable)
                        {
                            if (base.InReliableLog != null)
                            {
                                base.InReliableLog.Enqueue(new CmdLogReceivedReliable(nCommand, base.timeInt, base.roundTripTime, base.roundTripTimeVariance, base.timeInt - base.timeLastSendOutgoing, base.timeInt - base.timeLastSendAck));
                                base.CommandLogResize();
                            }
                            this.QueueOutgoingAcknowledgement(nCommand, this.serverSentTime);
                            if (base.TrafficStatsEnabled)
                            {
                                base.TrafficStatsIncoming.TimestampOfLastReliableCommand = SupportClass.GetTickCount();
                                base.TrafficStatsOutgoing.CountControlCommand(20);
                            }
                        }
                    }
                }
            end_IL_000c:;
            }
            catch (Exception ex)
            {
                if ((int)base.debugOut >= 1)
                {
                    base.EnqueueDebugReturn(DebugLevel.ERROR, string.Format("Exception while reading commands from incoming data: {0}", ex));
                }
                SupportClass.WriteStackTrace(ex);
            }
        }

        internal bool ExecuteCommand(NCommand command)
        {
            bool result = true;
            switch (command.commandType)
            {
                case 2:
                case 5:
                    if (base.TrafficStatsEnabled)
                    {
                        base.TrafficStatsIncoming.CountControlCommand(command.Size);
                    }
                    break;
                case 4:
                    {
                        if (base.TrafficStatsEnabled)
                        {
                            base.TrafficStatsIncoming.CountControlCommand(command.Size);
                        }
                        StatusCode statusValue = StatusCode.DisconnectByServerReasonUnknown;
                        if (command.reservedByte == 1)
                        {
                            statusValue = StatusCode.DisconnectByServerLogic;
                        }
                        else if (command.reservedByte == 2)
                        {
                            statusValue = StatusCode.DisconnectByServerTimeout;
                        }
                        else if (command.reservedByte == 3)
                        {
                            statusValue = StatusCode.DisconnectByServerUserLimit;
                        }
                        if ((int)base.debugOut >= 3)
                        {
                            base.Listener.DebugReturn(DebugLevel.INFO, "Server " + base.ServerAddress + " sent disconnect. PeerId: " + (ushort)base.peerID + " RTT/Variance:" + base.roundTripTime + "/" + base.roundTripTimeVariance + " reason byte: " + command.reservedByte);
                        }
                        base.EnqueueStatusCallback(statusValue);
                        this.Disconnect();
                        break;
                    }
                case 1:
                case 16:
                    {
                        if (base.TrafficStatsEnabled)
                        {
                            base.TrafficStatsIncoming.TimestampOfLastAck = SupportClass.GetTickCount();
                            base.TrafficStatsIncoming.CountControlCommand(command.Size);
                        }
                        base.timeLastAckReceive = base.timeInt;
                        base.lastRoundTripTime = base.timeInt - command.ackReceivedSentTime;
                        if (base.lastRoundTripTime < 0 || base.lastRoundTripTime > 10000)
                        {
                            if ((int)base.debugOut >= 3)
                            {
                                base.EnqueueDebugReturn(DebugLevel.INFO, "Measured lastRoundtripTime is suspicious: " + base.lastRoundTripTime + " for command: " + command);
                            }
                            base.lastRoundTripTime = base.roundTripTime * 4;
                        }
                        NCommand nCommand4 = this.RemoveSentReliableCommand(command.ackReceivedReliableSequenceNumber, command.commandChannelID, command.commandType == 16);
                        if (base.CommandLog != null)
                        {
                            base.CommandLog.Enqueue(new CmdLogReceivedAck(command, base.timeInt, base.roundTripTime, base.roundTripTimeVariance));
                            base.CommandLogResize();
                        }
                        if (nCommand4 != null)
                        {
                            nCommand4.FreePayload();
                            if (nCommand4.commandType == 12)
                            {
                                if (base.lastRoundTripTime <= base.roundTripTime)
                                {
                                    base.serverTimeOffset = this.serverSentTime + (base.lastRoundTripTime >> 1) - SupportClass.GetTickCount();
                                    base.serverTimeOffsetIsAvailable = true;
                                }
                                else
                                {
                                    this.FetchServerTimestamp();
                                }
                            }
                            else
                            {
                                base.UpdateRoundTripTimeAndVariance(base.lastRoundTripTime);
                                if (nCommand4.commandType == 4 && base.peerConnectionState == ConnectionStateValue.Disconnecting)
                                {
                                    if ((int)base.debugOut >= 3)
                                    {
                                        base.EnqueueDebugReturn(DebugLevel.INFO, "Received disconnect ACK by server");
                                    }
                                    base.EnqueueActionForDispatch(delegate
                                    {
                                        base.PhotonSocket.Disconnect();
                                    });
                                }
                                else if (nCommand4.commandType == 2 && base.lastRoundTripTime >= 0)
                                {
                                    if (base.lastRoundTripTime <= 15)
                                    {
                                        base.roundTripTime = 15;
                                        base.roundTripTimeVariance = 5;
                                    }
                                    else
                                    {
                                        base.roundTripTime = base.lastRoundTripTime;
                                    }
                                }
                            }
                        }
                        break;
                    }
                case 6:
                    if (base.TrafficStatsEnabled)
                    {
                        base.TrafficStatsIncoming.CountReliableOpCommand(command.Size);
                    }
                    if (base.peerConnectionState == ConnectionStateValue.Connected)
                    {
                        result = this.QueueIncomingCommand(command);
                    }
                    break;
                case 14:
                    if (base.TrafficStatsEnabled)
                    {
                        base.TrafficStatsIncoming.CountReliableOpCommand(command.Size);
                    }
                    if (base.peerConnectionState == ConnectionStateValue.Connected)
                    {
                        result = this.QueueIncomingCommand(command);
                    }
                    break;
                case 7:
                    if (base.TrafficStatsEnabled)
                    {
                        base.TrafficStatsIncoming.CountUnreliableOpCommand(command.Size);
                    }
                    if (base.peerConnectionState == ConnectionStateValue.Connected)
                    {
                        result = this.QueueIncomingCommand(command);
                    }
                    break;
                case 11:
                    if (base.TrafficStatsEnabled)
                    {
                        base.TrafficStatsIncoming.CountUnreliableOpCommand(command.Size);
                    }
                    if (base.peerConnectionState == ConnectionStateValue.Connected)
                    {
                        result = this.QueueIncomingCommand(command);
                    }
                    break;
                case 8:
                case 15:
                    if (base.peerConnectionState == ConnectionStateValue.Connected)
                    {
                        if (base.TrafficStatsEnabled)
                        {
                            base.TrafficStatsIncoming.CountFragmentOpCommand(command.Size);
                        }
                        if (command.fragmentNumber > command.fragmentCount || command.fragmentOffset >= command.totalLength || command.fragmentOffset + command.Payload.IntLength > command.totalLength)
                        {
                            if ((int)base.debugOut >= 1)
                            {
                                base.Listener.DebugReturn(DebugLevel.ERROR, "Received fragment has bad size: " + command);
                            }
                        }
                        else
                        {
                            bool flag = command.commandType == 8;
                            EnetChannel channel = this.GetChannel(command.commandChannelID);
                            NCommand nCommand = null;
                            bool flag2 = channel.TryGetFragment(command.startSequenceNumber, flag, out nCommand);
                            if ((!flag2 || nCommand.fragmentsRemaining > 0) && this.QueueIncomingCommand(command))
                            {
                                if (command.reliableSequenceNumber != command.startSequenceNumber)
                                {
                                    if (flag2)
                                    {
                                        nCommand.fragmentsRemaining--;
                                    }
                                }
                                else
                                {
                                    nCommand = command;
                                    nCommand.fragmentsRemaining--;
                                    NCommand nCommand2 = null;
                                    int num = command.startSequenceNumber + 1;
                                    while (nCommand.fragmentsRemaining > 0 && num < nCommand.startSequenceNumber + nCommand.fragmentCount)
                                    {
                                        if (channel.TryGetFragment(num++, flag, out nCommand2))
                                        {
                                            nCommand.fragmentsRemaining--;
                                        }
                                    }
                                }
                                if (nCommand != null && nCommand.fragmentsRemaining <= 0)
                                {
                                    byte[] array = new byte[nCommand.totalLength];
                                    int num3 = nCommand.startSequenceNumber;
                                    while (num3 < nCommand.startSequenceNumber + nCommand.fragmentCount)
                                    {
                                        NCommand nCommand3 = default(NCommand);
                                        if (channel.TryGetFragment(num3, flag, out nCommand3))
                                        {
                                            Buffer.BlockCopy(nCommand3.Payload.GetBuffer(), 0, array, nCommand3.fragmentOffset, nCommand3.Payload.IntLength);
                                            nCommand3.FreePayload();
                                            channel.RemoveFragment(nCommand3.reliableSequenceNumber, flag);
                                            num3++;
                                            continue;
                                        }
                                        throw new Exception("startCommand.fragmentsRemaining was 0 but not all fragments were found to be combined!");
                                    }
                                    nCommand.Payload = new StreamBuffer(array);
                                    nCommand.Size = 12 * nCommand.fragmentCount + nCommand.totalLength;
                                    if (!flag)
                                    {
                                        channel.incomingUnsequencedCommandsList.Enqueue(nCommand);
                                    }
                                    else
                                    {
                                        channel.incomingReliableCommandsList.Add(nCommand.startSequenceNumber, nCommand);
                                    }
                                }
                            }
                        }
                    }
                    break;
                case 3:
                    if (base.TrafficStatsEnabled)
                    {
                        base.TrafficStatsIncoming.CountControlCommand(command.Size);
                    }
                    if (base.peerConnectionState == ConnectionStateValue.Connecting)
                    {
                        byte[] buf = base.PrepareConnectData(base.ServerAddress, base.AppId, base.CustomInitData);
                        this.CreateAndEnqueueCommand(6, new StreamBuffer(buf), 0);
                        if (base.photonPeer.RandomizeSequenceNumbers)
                        {
                            this.ApplyRandomizedSequenceNumbers();
                        }
                        base.peerConnectionState = ConnectionStateValue.Connected;
                    }
                    break;
            }
            return result;
        }

        internal bool QueueIncomingCommand(NCommand command)
        {
            EnetChannel channel = this.GetChannel(command.commandChannelID);
            if (channel == null)
            {
                if ((int)base.debugOut >= 1)
                {
                    base.Listener.DebugReturn(DebugLevel.ERROR, "Received command for non-existing channel: " + command.commandChannelID);
                }
                return false;
            }
            if ((int)base.debugOut >= 5)
            {
                base.Listener.DebugReturn(DebugLevel.ALL, "queueIncomingCommand() " + command + " channel seq# r/u: " + channel.incomingReliableSequenceNumber + "/" + channel.incomingUnreliableSequenceNumber);
            }
            if (command.IsFlaggedReliable)
            {
                if (command.IsFlaggedUnsequenced)
                {
                    return channel.QueueIncomingReliableUnsequenced(command);
                }
                if (command.reliableSequenceNumber <= channel.incomingReliableSequenceNumber)
                {
                    if ((int)base.debugOut >= 3)
                    {
                        base.Listener.DebugReturn(DebugLevel.INFO, "incoming command " + command + " is old (not saving it). Dispatched incomingReliableSequenceNumber: " + channel.incomingReliableSequenceNumber);
                    }
                    return false;
                }
                if (channel.ContainsReliableSequenceNumber(command.reliableSequenceNumber))
                {
                    if ((int)base.debugOut >= 3)
                    {
                        base.Listener.DebugReturn(DebugLevel.INFO, "Info: command was received before! Old/New: " + channel.FetchReliableSequenceNumber(command.reliableSequenceNumber) + "/" + command + " inReliableSeq#: " + channel.incomingReliableSequenceNumber);
                    }
                    return false;
                }
                channel.incomingReliableCommandsList.Add(command.reliableSequenceNumber, command);
                return true;
            }
            if (command.commandFlags == 0)
            {
                if (command.reliableSequenceNumber < channel.incomingReliableSequenceNumber)
                {
                    if ((int)base.debugOut >= 3)
                    {
                        base.Listener.DebugReturn(DebugLevel.INFO, "incoming reliable-seq# < Dispatched-rel-seq#. not saved.");
                    }
                    return true;
                }
                if (command.unreliableSequenceNumber <= channel.incomingUnreliableSequenceNumber)
                {
                    if ((int)base.debugOut >= 3)
                    {
                        base.Listener.DebugReturn(DebugLevel.INFO, "incoming unreliable-seq# < Dispatched-unrel-seq#. not saved.");
                    }
                    return true;
                }
                if (channel.ContainsUnreliableSequenceNumber(command.unreliableSequenceNumber))
                {
                    if ((int)base.debugOut >= 3)
                    {
                        base.Listener.DebugReturn(DebugLevel.INFO, "command was received before! Old/New: " + channel.incomingUnreliableCommandsList[command.unreliableSequenceNumber] + "/" + command);
                    }
                    return false;
                }
                channel.incomingUnreliableCommandsList.Add(command.unreliableSequenceNumber, command);
                return true;
            }
            if (command.commandFlags == 2)
            {
                int unsequencedGroupNumber = command.unsequencedGroupNumber;
                int num = command.unsequencedGroupNumber % 128;
                if (unsequencedGroupNumber >= this.incomingUnsequencedGroupNumber + 128)
                {
                    this.incomingUnsequencedGroupNumber = unsequencedGroupNumber - num;
                    for (int i = 0; i < this.unsequencedWindow.Length; i++)
                    {
                        this.unsequencedWindow[i] = 0;
                    }
                }
                else if (unsequencedGroupNumber < this.incomingUnsequencedGroupNumber || (this.unsequencedWindow[num / 32] & 1 << num % 32) != 0)
                {
                    return false;
                }
                this.unsequencedWindow[num / 32] |= 1 << num % 32;
                channel.incomingUnsequencedCommandsList.Enqueue(command);
                return true;
            }
            return false;
        }

        internal NCommand RemoveSentReliableCommand(int ackReceivedReliableSequenceNumber, int ackReceivedChannel, bool isUnsequenced)
        {
            NCommand nCommand = null;
            List<NCommand> obj = this.sentReliableCommands;
            lock (obj)
            {
                foreach (NCommand sentReliableCommand in this.sentReliableCommands)
                {
                    if (sentReliableCommand != null && sentReliableCommand.reliableSequenceNumber == ackReceivedReliableSequenceNumber && sentReliableCommand.IsFlaggedUnsequenced == isUnsequenced && sentReliableCommand.commandChannelID == ackReceivedChannel)
                    {
                        nCommand = sentReliableCommand;
                        break;
                    }
                }
                if (nCommand != null)
                {
                    this.sentReliableCommands.Remove(nCommand);
                    if (this.sentReliableCommands.Count > 0)
                    {
                        base.timeoutInt = base.timeInt + 25;
                    }
                }
                else if ((int)base.debugOut >= 5 && base.peerConnectionState != ConnectionStateValue.Connected && base.peerConnectionState != ConnectionStateValue.Disconnecting)
                {
                    base.EnqueueDebugReturn(DebugLevel.ALL, string.Format("No sent command for ACK (Ch: {0} Sq#: {1}). PeerState: {2}.", ackReceivedReliableSequenceNumber, ackReceivedChannel, base.peerConnectionState));
                }
            }
            return nCommand;
        }

        internal string CommandListToString(NCommand[] list)
        {
            if ((int)base.debugOut < 5)
            {
                return string.Empty;
            }
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i < list.Length; i++)
            {
                stringBuilder.Append(i + "=");
                stringBuilder.Append(list[i]);
                stringBuilder.Append(" # ");
            }
            return stringBuilder.ToString();
        }
    }
}
