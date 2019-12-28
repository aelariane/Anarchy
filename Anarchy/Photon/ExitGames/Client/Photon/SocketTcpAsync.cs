using System;
using System.Net;
using System.Net.Sockets;
using System.Security;
using System.Threading;

namespace ExitGames.Client.Photon
{
	public class SocketTcpAsync : IPhotonSocket, IDisposable
	{
		public SocketTcpAsync(PeerBase npeer) : base(npeer)
		{
			bool flag = base.ReportDebugOfLevel(DebugLevel.ALL);
			if (flag)
			{
				base.Listener.DebugReturn(DebugLevel.ALL, "SocketTcpAsync: TCP, DotNet, Unity.");
			}
			this.PollReceive = false;
		}

		public override bool Connect()
		{
			bool flag = base.Connect();
			bool flag2 = !flag;
			bool result;
			if (flag2)
			{
				result = false;
			}
			else
			{
				base.State = PhotonSocketState.Connecting;
				new Thread(new ThreadStart(this.DnsAndConnect))
				{
					IsBackground = true
				}.Start();
				result = true;
			}
			return result;
		}

		public void DnsAndConnect()
		{
			try
			{
				IPAddress ipAddress = IPhotonSocket.GetIpAddress(base.ServerAddress);
				bool flag = ipAddress == null;
				if (flag)
				{
					throw new ArgumentException("SocketTcpAsync. Invalid IPAddress. Address: " + base.ServerAddress);
				}
				this.sock = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
				this.sock.NoDelay = true;
				this.sock.ReceiveTimeout = this.peerBase.DisconnectTimeout;
				this.sock.SendTimeout = this.peerBase.DisconnectTimeout;
				this.sock.Connect(ipAddress, base.ServerPort);
				base.AddressResolvedAsIpv6 = base.IsIpv6SimpleCheck(ipAddress);
				base.State = PhotonSocketState.Connected;
				this.peerBase.OnConnect();
			}
			catch (SecurityException ex)
			{
				bool flag2 = base.ReportDebugOfLevel(DebugLevel.ERROR);
				if (flag2)
				{
					base.Listener.DebugReturn(DebugLevel.ERROR, "SocketTcpAsync SecurityException. Connect() to '" + base.ServerAddress + "' failed: " + ex.ToString());
				}
				base.HandleException(StatusCode.SecurityExceptionOnConnect);
				return;
			}
			catch (Exception ex2)
			{
				bool flag3 = base.ReportDebugOfLevel(DebugLevel.ERROR);
				if (flag3)
				{
					base.Listener.DebugReturn(DebugLevel.ERROR, "SocketTcpAsync Exception. Connect() to '" + base.ServerAddress + "' failed: " + ex2.ToString());
				}
				base.HandleException(StatusCode.ExceptionOnConnect);
				return;
			}
			this.ReceiveAsync((ReceiveContext)null);
		}

		public override PhotonSocketError Send(byte[] data, int length)
		{
			bool flag = this.sock == null || !this.sock.Connected;
			PhotonSocketError result;
			if (flag)
			{
				result = PhotonSocketError.Skipped;
			}
			else
			{
				try
				{
					this.sock.Send(data, 0, length, SocketFlags.None);
				}
				catch (Exception ex)
				{
					bool flag2 = base.State != PhotonSocketState.Disconnecting && base.State > PhotonSocketState.Disconnected;
					if (flag2)
					{
						bool flag3 = base.ReportDebugOfLevel(DebugLevel.ERROR);
						if (flag3)
						{
							base.EnqueueDebugReturn(DebugLevel.ERROR, string.Format("SocketTcpAsync.Send Exception: Cannot send to: {0}. Uptime: {1} ms. {2} {3}", new object[]
							{
								base.ServerAddress,
								SupportClass.GetTickCount() - this.peerBase.timeBase,
								base.AddressResolvedAsIpv6 ? " IPv6" : string.Empty,
								ex
							}));
						}
						base.HandleException(StatusCode.Exception);
					}
					return PhotonSocketError.Exception;
				}
				result = PhotonSocketError.Success;
			}
			return result;
		}

		public override PhotonSocketError Receive(out byte[] data)
		{
			data = null;
			return PhotonSocketError.NoData;
		}

		private void ReceiveAsync(SocketTcpAsync.ReceiveContext context = null)
		{
			bool flag = context == null;
			if (flag)
			{
				context = new SocketTcpAsync.ReceiveContext(this.sock, new byte[9], new byte[base.MTU]);
			}
			try
			{
				this.sock.BeginReceive(context.CurrentBuffer, context.CurrentOffset, context.CurrentExpected - context.CurrentOffset, SocketFlags.None, new AsyncCallback(this.ReceiveAsync), context);
			}
			catch (Exception ex)
			{
				bool flag2 = base.State != PhotonSocketState.Disconnecting && base.State > PhotonSocketState.Disconnected;
				if (flag2)
				{
					bool flag3 = base.ReportDebugOfLevel(DebugLevel.ERROR);
					if (flag3)
					{
						base.EnqueueDebugReturn(DebugLevel.ERROR, string.Concat(new object[]
						{
							"SocketTcpAsync.ReceiveAsync Exception. State: ",
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

		private void ReceiveAsync(IAsyncResult ar)
		{
			bool flag = base.State == PhotonSocketState.Disconnecting || base.State == PhotonSocketState.Disconnected;
			if (!flag)
			{
				int num = 0;
				try
				{
					num = this.sock.EndReceive(ar);
					bool flag2 = num == 0;
					if (flag2)
					{
						throw new SocketException(10054);
					}
				}
				catch (SocketException ex)
				{
					bool flag3 = base.State != PhotonSocketState.Disconnecting && base.State > PhotonSocketState.Disconnected;
					if (flag3)
					{
						bool flag4 = base.ReportDebugOfLevel(DebugLevel.ERROR);
						if (flag4)
						{
							base.EnqueueDebugReturn(DebugLevel.ERROR, string.Concat(new object[]
							{
								"SocketTcpAsync.EndReceive SocketException. State: ",
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
						return;
					}
				}
				catch (Exception ex2)
				{
					bool flag5 = base.State != PhotonSocketState.Disconnecting && base.State > PhotonSocketState.Disconnected;
					if (flag5)
					{
						bool flag6 = base.ReportDebugOfLevel(DebugLevel.ERROR);
						if (flag6)
						{
							base.EnqueueDebugReturn(DebugLevel.ERROR, string.Concat(new object[]
							{
								"SocketTcpAsync.EndReceive Exception. State: ",
								base.State,
								". Server: '",
								base.ServerAddress,
								"' Exception: ",
								ex2
							}));
						}
						base.HandleException(StatusCode.ExceptionOnReceive);
						return;
					}
				}
				SocketTcpAsync.ReceiveContext receiveContext = (SocketTcpAsync.ReceiveContext)ar.AsyncState;
				bool flag7 = num + receiveContext.CurrentOffset != receiveContext.CurrentExpected;
				if (flag7)
				{
					bool readingHeader = receiveContext.ReadingHeader;
					if (readingHeader)
					{
						receiveContext.ReceivedHeaderBytes += num;
					}
					else
					{
						receiveContext.ReceivedMessageBytes += num;
					}
					this.ReceiveAsync(receiveContext);
				}
				else
				{
					bool readingHeader2 = receiveContext.ReadingHeader;
					if (readingHeader2)
					{
						byte[] headerBuffer = receiveContext.HeaderBuffer;
						bool flag8 = headerBuffer[0] == 240;
						if (flag8)
						{
							base.HandleReceivedDatagram(headerBuffer, headerBuffer.Length, true);
							receiveContext.Reset();
							this.ReceiveAsync(receiveContext);
						}
						else
						{
							int num2 = (int)headerBuffer[1] << 24 | (int)headerBuffer[2] << 16 | (int)headerBuffer[3] << 8 | (int)headerBuffer[4];
							receiveContext.ExpectedMessageBytes = num2 - 7;
							bool flag9 = receiveContext.ExpectedMessageBytes > receiveContext.MessageBuffer.Length;
							if (flag9)
							{
								receiveContext.MessageBuffer = new byte[receiveContext.ExpectedMessageBytes];
							}
							receiveContext.MessageBuffer[0] = headerBuffer[7];
							receiveContext.MessageBuffer[1] = headerBuffer[8];
							receiveContext.ReceivedMessageBytes = 2;
							this.ReceiveAsync(receiveContext);
						}
					}
					else
					{
						base.HandleReceivedDatagram(receiveContext.MessageBuffer, receiveContext.ExpectedMessageBytes, true);
						receiveContext.Reset();
						this.ReceiveAsync(receiveContext);
					}
				}
			}
		}

		public override bool Disconnect()
		{
			bool flag = base.ReportDebugOfLevel(DebugLevel.INFO);
			if (flag)
			{
				base.EnqueueDebugReturn(DebugLevel.INFO, "SocketTcpAsync.Disconnect()");
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

		private class ReceiveContext
		{
			public ReceiveContext(Socket socket, byte[] headerBuffer, byte[] messageBuffer)
			{
				this.HeaderBuffer = headerBuffer;
				this.MessageBuffer = messageBuffer;
				this.workSocket = socket;
			}

			public bool ReadingHeader
			{
				get
				{
					return this.ExpectedMessageBytes == 0;
				}
			}

			public bool ReadingMessage
			{
				get
				{
					return this.ExpectedMessageBytes != 0;
				}
			}

			public byte[] CurrentBuffer
			{
				get
				{
					return this.ReadingHeader ? this.HeaderBuffer : this.MessageBuffer;
				}
			}

			public int CurrentOffset
			{
				get
				{
					return this.ReadingHeader ? this.ReceivedHeaderBytes : this.ReceivedMessageBytes;
				}
			}

			public int CurrentExpected
			{
				get
				{
					return this.ReadingHeader ? 9 : this.ExpectedMessageBytes;
				}
			}

			public void Reset()
			{
				this.ReceivedHeaderBytes = 0;
				this.ExpectedMessageBytes = 0;
				this.ReceivedMessageBytes = 0;
			}

			public Socket workSocket;

			public int ReceivedHeaderBytes;

			public byte[] HeaderBuffer;

			public int ExpectedMessageBytes;

			public int ReceivedMessageBytes;

			public byte[] MessageBuffer;
		}
	}
}
