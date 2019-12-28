using System;
using System.Net;
using System.Net.Sockets;
using System.Security;
using System.Threading;

namespace ExitGames.Client.Photon
{
	public class SocketUdpAsync : IPhotonSocket, IDisposable
	{
		public SocketUdpAsync(PeerBase npeer) : base(npeer)
		{
			bool flag = base.ReportDebugOfLevel(DebugLevel.INFO);
			if (flag)
			{
				base.Listener.DebugReturn(DebugLevel.INFO, "SocketUdpAsync: UDP, Unity3d.");
			}
			this.PollReceive = false;
		}

		public override bool Connect()
		{
			object obj = this.syncer;
			lock (obj)
			{
				bool flag = base.Connect();
				bool flag2 = !flag;
				if (flag2)
				{
					return false;
				}
			}
			base.State = PhotonSocketState.Connecting;
			new Thread(new ThreadStart(this.DnsAndConnect))
			{
				IsBackground = true
			}.Start();
			return true;
		}

		internal void DnsAndConnect()
		{
			IPAddress ipaddress = null;
			try
			{
				ipaddress = IPhotonSocket.GetIpAddress(base.ServerAddress);
				bool flag = ipaddress == null;
				if (flag)
				{
					throw new ArgumentException("Invalid IPAddress. Address: " + base.ServerAddress);
				}
				object obj = this.syncer;
				lock (obj)
				{
					bool flag2 = base.State == PhotonSocketState.Disconnecting || base.State == PhotonSocketState.Disconnected;
					if (flag2)
					{
						return;
					}
					bool flag3 = ipaddress.AddressFamily != AddressFamily.InterNetwork && ipaddress.AddressFamily != AddressFamily.InterNetworkV6;
					if (flag3)
					{
						throw new ArgumentException(string.Concat(new object[]
						{
							"AddressFamily '",
							ipaddress.AddressFamily,
							"' not supported. Address: ",
							base.ServerAddress
						}));
					}
					this.sock = new Socket(ipaddress.AddressFamily, SocketType.Dgram, ProtocolType.Udp);
					this.sock.Connect(ipaddress, base.ServerPort);
					base.AddressResolvedAsIpv6 = base.IsIpv6SimpleCheck(ipaddress);
					base.State = PhotonSocketState.Connected;
					this.peerBase.OnConnect();
				}
			}
			catch (SecurityException ex)
			{
				bool flag4 = base.ReportDebugOfLevel(DebugLevel.ERROR);
				if (flag4)
				{
					base.Listener.DebugReturn(DebugLevel.ERROR, string.Concat(new string[]
					{
						"Connect() to '",
						base.ServerAddress,
						"' (",
						(ipaddress == null) ? "" : ipaddress.AddressFamily.ToString(),
						") failed: ",
						ex.ToString()
					}));
				}
				base.HandleException(StatusCode.SecurityExceptionOnConnect);
				return;
			}
			catch (Exception ex2)
			{
				bool flag5 = base.ReportDebugOfLevel(DebugLevel.ERROR);
				if (flag5)
				{
					base.Listener.DebugReturn(DebugLevel.ERROR, string.Concat(new string[]
					{
						"Connect() to '",
						base.ServerAddress,
						"' (",
						(ipaddress == null) ? "" : ipaddress.AddressFamily.ToString(),
						") failed: ",
						ex2.ToString()
					}));
				}
				base.HandleException(StatusCode.ExceptionOnConnect);
				return;
			}
			this.StartReceive();
		}

		public override PhotonSocketError Send(byte[] data, int length)
		{
			object obj = this.syncer;
			lock (obj)
			{
				bool flag = this.sock == null || !this.sock.Connected;
				if (flag)
				{
					return PhotonSocketError.Skipped;
				}
				try
				{
					this.sock.Send(data, 0, length, SocketFlags.None);
				}
				catch (Exception ex)
				{
					bool flag2 = base.ReportDebugOfLevel(DebugLevel.ERROR);
					if (flag2)
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

		public void StartReceive()
		{
			byte[] array = new byte[base.MTU];
			try
			{
				this.sock.BeginReceive(array, 0, array.Length, SocketFlags.None, new AsyncCallback(this.OnReceive), array);
			}
			catch (Exception ex)
			{
				bool flag = base.State != PhotonSocketState.Disconnecting && base.State > PhotonSocketState.Disconnected;
				if (flag)
				{
					bool flag2 = base.ReportDebugOfLevel(DebugLevel.ERROR);
					if (flag2)
					{
						base.EnqueueDebugReturn(DebugLevel.ERROR, string.Concat(new object[]
						{
							"Receive issue. State: ",
							base.State,
							". Server: '",
							base.ServerAddress,
							"' Exception: ",
							ex
						}));
					}
					base.HandleException(StatusCode.ExceptionOnReceive);
				}
			}
		}

		private void OnReceive(IAsyncResult ar)
		{
			bool flag = base.State == PhotonSocketState.Disconnecting || base.State == PhotonSocketState.Disconnected;
			if (!flag)
			{
				int length = 0;
				try
				{
					length = this.sock.EndReceive(ar);
				}
				catch (SocketException ex)
				{
					bool flag2 = base.State != PhotonSocketState.Disconnecting && base.State > PhotonSocketState.Disconnected;
					if (flag2)
					{
						bool flag3 = base.ReportDebugOfLevel(DebugLevel.ERROR);
						if (flag3)
						{
							base.EnqueueDebugReturn(DebugLevel.ERROR, string.Concat(new object[]
							{
								"SocketException in EndReceive. State: ",
								base.State,
								". Server: '",
								base.ServerAddress,
								"' ErrorCode: ",
								ex.ErrorCode,
								" SocketErrorCode: ",
								ex.SocketErrorCode,
								" Message: ",
								ex.Message,
								" ",
								ex
							}));
						}
						base.HandleException(StatusCode.ExceptionOnReceive);
					}
				}
				catch (Exception ex2)
				{
					bool flag4 = base.State != PhotonSocketState.Disconnecting && base.State > PhotonSocketState.Disconnected;
					if (flag4)
					{
						bool flag5 = base.ReportDebugOfLevel(DebugLevel.ERROR);
						if (flag5)
						{
							base.EnqueueDebugReturn(DebugLevel.ERROR, string.Concat(new object[]
							{
								"Exception in EndReceive. State: ",
								base.State,
								". Server: '",
								base.ServerAddress,
								"' Exception: ",
								ex2
							}));
						}
						base.HandleException(StatusCode.ExceptionOnReceive);
					}
				}
				bool flag6 = base.State == PhotonSocketState.Disconnecting || base.State == PhotonSocketState.Disconnected;
				if (!flag6)
				{
					byte[] array = (byte[])ar.AsyncState;
					base.HandleReceivedDatagram(array, length, true);
					try
					{
						this.sock.BeginReceive(array, 0, array.Length, SocketFlags.None, new AsyncCallback(this.OnReceive), array);
					}
					catch (SocketException ex3)
					{
						bool flag7 = base.State != PhotonSocketState.Disconnecting && base.State > PhotonSocketState.Disconnected;
						if (flag7)
						{
							bool flag8 = base.ReportDebugOfLevel(DebugLevel.ERROR);
							if (flag8)
							{
								base.EnqueueDebugReturn(DebugLevel.ERROR, string.Concat(new object[]
								{
									"SocketException in BeginReceive. State: ",
									base.State,
									". Server: '",
									base.ServerAddress,
									"' ErrorCode: ",
									ex3.ErrorCode,
									" SocketErrorCode: ",
									ex3.SocketErrorCode,
									" Message: ",
									ex3.Message,
									" ",
									ex3
								}));
							}
							base.HandleException(StatusCode.ExceptionOnReceive);
						}
					}
					catch (Exception ex4)
					{
						bool flag9 = base.State != PhotonSocketState.Disconnecting && base.State > PhotonSocketState.Disconnected;
						if (flag9)
						{
							bool flag10 = base.ReportDebugOfLevel(DebugLevel.ERROR);
							if (flag10)
							{
								base.EnqueueDebugReturn(DebugLevel.ERROR, string.Concat(new object[]
								{
									"Exception in BeginReceive. State: ",
									base.State,
									". Server: '",
									base.ServerAddress,
									"' Exception: ",
									ex4
								}));
							}
							base.HandleException(StatusCode.ExceptionOnReceive);
						}
					}
				}
			}
		}

		public override bool Disconnect()
		{
			bool flag = base.ReportDebugOfLevel(DebugLevel.INFO);
			if (flag)
			{
				base.EnqueueDebugReturn(DebugLevel.INFO, "SocketUdpAsync.Disconnect()");
			}
			base.State = PhotonSocketState.Disconnecting;
			object obj = this.syncer;
			lock (obj)
			{
				bool flag2 = this.sock != null;
				if (flag2)
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

		public void Dispose()
		{
			base.State = PhotonSocketState.Disconnecting;
			bool flag = this.sock != null;
			if (flag)
			{
				try
				{
					bool connected = this.sock.Connected;
					if (connected)
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

		private Socket sock;

		private readonly object syncer = new object();
	}
}
