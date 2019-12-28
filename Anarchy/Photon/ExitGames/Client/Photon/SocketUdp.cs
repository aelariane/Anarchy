using System;
using System.Net;
using System.Net.Sockets;
using System.Security;
using System.Threading;

namespace ExitGames.Client.Photon
{
    internal class SocketUdp : IPhotonSocket, IDisposable
    {
        private Socket sock;

        private readonly object syncer = new object();

        public SocketUdp(PeerBase npeer)
            : base(npeer)
        {
            if (base.ReportDebugOfLevel(DebugLevel.ALL))
            {
                base.Listener.DebugReturn(DebugLevel.ALL, "SocketUdp: UDP, Unity3d.");
            }
            base.PollReceive = false;
        }

        public void Dispose()
        {
            base.State = PhotonSocketState.Disconnecting;
            if (this.sock != null)
            {
                try
                {
                    if (this.sock.Connected)
                    {
                        this.sock.Close();
                    }
                }
                catch (Exception arg)
                {
                    base.EnqueueDebugReturn(DebugLevel.INFO, "Exception in Dispose(): " + arg);
                }
            }
            this.sock = null;
            base.State = PhotonSocketState.Disconnected;
        }

        public override bool Connect()
        {
            object obj = this.syncer;
            lock (obj)
            {
                if (!base.Connect())
                {
                    return false;
                }
                base.State = PhotonSocketState.Connecting;
                Thread thread = new Thread(this.DnsAndConnect);
                thread.IsBackground = true;
                thread.Start();
                return true;
            }
        }

        public override bool Disconnect()
        {
            if (base.ReportDebugOfLevel(DebugLevel.INFO))
            {
                base.EnqueueDebugReturn(DebugLevel.INFO, "SocketUdp.Disconnect()");
            }
            base.State = PhotonSocketState.Disconnecting;
            object obj = this.syncer;
            lock (obj)
            {
                if (this.sock != null)
                {
                    try
                    {
                        this.sock.Close();
                    }
                    catch (Exception arg)
                    {
                        base.EnqueueDebugReturn(DebugLevel.INFO, "Exception in Disconnect(): " + arg);
                    }
                    this.sock = null;
                }
            }
            base.State = PhotonSocketState.Disconnected;
            return true;
        }

        public override PhotonSocketError Send(byte[] data, int length)
        {
            object obj = this.syncer;
            lock (obj)
            {
                if (this.sock == null || !this.sock.Connected)
                {
                    return PhotonSocketError.Skipped;
                }
                try
                {
                    this.sock.Send(data, 0, length, SocketFlags.None);
                }
                catch (Exception ex)
                {
                    if (base.ReportDebugOfLevel(DebugLevel.ERROR))
                    {
                        base.EnqueueDebugReturn(DebugLevel.ERROR, "Cannot send to: " + base.ServerAddress + ". " + ex.Message);
                    }
                    return PhotonSocketError.Exception;
                }
            }
            return PhotonSocketError.Success;
        }

        public override PhotonSocketError Receive(out byte[] data)
        {
            data = null;
            return PhotonSocketError.NoData;
        }

        internal void DnsAndConnect()
        {
            IPAddress iPAddress = null;
            AddressFamily addressFamily;
            try
            {
                iPAddress = IPhotonSocket.GetIpAddress(base.ServerAddress);
                if (iPAddress == null)
                {
                    throw new ArgumentException("Invalid IPAddress. Address: " + base.ServerAddress);
                }
                object obj = this.syncer;
                lock (obj)
                {
                    if (base.State != PhotonSocketState.Disconnecting && base.State != 0)
                    {
                        this.sock = new Socket(iPAddress.AddressFamily, SocketType.Dgram, ProtocolType.Udp);
                        this.sock.Connect(iPAddress, base.ServerPort);
                        base.AddressResolvedAsIpv6 = base.IsIpv6SimpleCheck(iPAddress);
                        base.State = PhotonSocketState.Connected;
                        base.peerBase.OnConnect();
                        goto end_IL_003d;
                    }
                    return;
                end_IL_003d:;
                }
            }
            catch (SecurityException ex)
            {
                if (base.ReportDebugOfLevel(DebugLevel.ERROR))
                {
                    IPhotonPeerListener listener = base.Listener;
                    string[] obj2 = new string[6]
                    {
                    "Connect() to '",
                    base.ServerAddress,
                    "' (",
                    null,
                    null,
                    null
                    };
                    object obj3;
                    if (iPAddress != null)
                    {
                        addressFamily = iPAddress.AddressFamily;
                        obj3 = addressFamily.ToString();
                    }
                    else
                    {
                        obj3 = "";
                    }
                    obj2[3] = (string)obj3;
                    obj2[4] = ") failed: ";
                    obj2[5] = ex.ToString();
                    listener.DebugReturn(DebugLevel.ERROR, string.Concat(obj2));
                }
                base.HandleException(StatusCode.SecurityExceptionOnConnect);
                return;
            }
            catch (Exception ex2)
            {
                if (base.ReportDebugOfLevel(DebugLevel.ERROR))
                {
                    IPhotonPeerListener listener2 = base.Listener;
                    string[] obj4 = new string[6]
                    {
                    "Connect() to '",
                    base.ServerAddress,
                    "' (",
                    null,
                    null,
                    null
                    };
                    object obj5;
                    if (iPAddress != null)
                    {
                        addressFamily = iPAddress.AddressFamily;
                        obj5 = addressFamily.ToString();
                    }
                    else
                    {
                        obj5 = "";
                    }
                    obj4[3] = (string)obj5;
                    obj4[4] = ") failed: ";
                    obj4[5] = ex2.ToString();
                    listener2.DebugReturn(DebugLevel.ERROR, string.Concat(obj4));
                }
                base.HandleException(StatusCode.ExceptionOnConnect);
                return;
            }
            Thread thread = new Thread(this.ReceiveLoop);
            thread.IsBackground = true;
            thread.Start();
        }

        public void ReceiveLoop()
        {
            byte[] array = new byte[base.MTU];
            while (base.State == PhotonSocketState.Connected)
            {
                try
                {
                    int length = this.sock.Receive(array);
                    base.HandleReceivedDatagram(array, length, true);
                }
                catch (SocketException ex)
                {
                    if (base.State != PhotonSocketState.Disconnecting && base.State != 0)
                    {
                        if (base.ReportDebugOfLevel(DebugLevel.ERROR))
                        {
                            base.EnqueueDebugReturn(DebugLevel.ERROR, "Receive issue. State: " + base.State + ". Server: '" + base.ServerAddress + "' ErrorCode: " + ex.ErrorCode + " SocketErrorCode: " + ex.SocketErrorCode + " Message: " + ex.Message + " " + ex);
                        }
                        base.HandleException(StatusCode.ExceptionOnReceive);
                    }
                }
                catch (Exception ex2)
                {
                    if (base.State != PhotonSocketState.Disconnecting && base.State != 0)
                    {
                        if (base.ReportDebugOfLevel(DebugLevel.ERROR))
                        {
                            base.EnqueueDebugReturn(DebugLevel.ERROR, "Receive issue. State: " + base.State + ". Server: '" + base.ServerAddress + "' Message: " + ex2.Message + " Exception: " + ex2);
                        }
                        base.HandleException(StatusCode.ExceptionOnReceive);
                    }
                }
            }
            this.Disconnect();
        }
    }
}
