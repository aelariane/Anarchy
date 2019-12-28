using System;
using System.Collections.Generic;
using System.Threading;

namespace ExitGames.Client.Photon
{
	public class NetworkSimulationSet
	{
		protected internal bool IsSimulationEnabled
		{
			get
			{
				return this.isSimulationEnabled;
			}
			set
			{
				ManualResetEvent netSimManualResetEvent = this.NetSimManualResetEvent;
				lock (netSimManualResetEvent)
				{
					bool flag = value == this.isSimulationEnabled;
					if (!flag)
					{
						bool flag2 = !value;
						if (flag2)
						{
							LinkedList<SimulationItem> netSimListIncoming = this.peerBase.NetSimListIncoming;
							lock (netSimListIncoming)
							{
								foreach (SimulationItem simulationItem in this.peerBase.NetSimListIncoming)
								{
									bool flag3 = this.peerBase.PhotonSocket != null && this.peerBase.PhotonSocket.Connected;
									if (flag3)
									{
										this.peerBase.ReceiveIncomingCommands(simulationItem.DelayedData, simulationItem.DelayedData.Length);
									}
								}
								this.peerBase.NetSimListIncoming.Clear();
							}
							LinkedList<SimulationItem> netSimListOutgoing = this.peerBase.NetSimListOutgoing;
							lock (netSimListOutgoing)
							{
								foreach (SimulationItem simulationItem2 in this.peerBase.NetSimListOutgoing)
								{
									bool flag4 = this.peerBase.PhotonSocket != null && this.peerBase.PhotonSocket.Connected;
									if (flag4)
									{
										this.peerBase.PhotonSocket.Send(simulationItem2.DelayedData, simulationItem2.DelayedData.Length);
									}
								}
								this.peerBase.NetSimListOutgoing.Clear();
							}
						}
						this.isSimulationEnabled = value;
						bool flag5 = this.isSimulationEnabled;
						if (flag5)
						{
							bool flag6 = this.netSimThread == null;
							if (flag6)
							{
								this.netSimThread = new Thread(new ThreadStart(this.peerBase.NetworkSimRun));
								this.netSimThread.IsBackground = true;
								this.netSimThread.Name = "netSim" + SupportClass.GetTickCount();
								this.netSimThread.Start();
							}
							this.NetSimManualResetEvent.Set();
						}
						else
						{
							this.NetSimManualResetEvent.Reset();
						}
					}
				}
			}
		}

		public int OutgoingLag
		{
			get
			{
				return this.outgoingLag;
			}
			set
			{
				this.outgoingLag = value;
			}
		}

		public int OutgoingJitter
		{
			get
			{
				return this.outgoingJitter;
			}
			set
			{
				this.outgoingJitter = value;
			}
		}

		public int OutgoingLossPercentage
		{
			get
			{
				return this.outgoingLossPercentage;
			}
			set
			{
				this.outgoingLossPercentage = value;
			}
		}

		public int IncomingLag
		{
			get
			{
				return this.incomingLag;
			}
			set
			{
				this.incomingLag = value;
			}
		}

		public int IncomingJitter
		{
			get
			{
				return this.incomingJitter;
			}
			set
			{
				this.incomingJitter = value;
			}
		}

		public int IncomingLossPercentage
		{
			get
			{
				return this.incomingLossPercentage;
			}
			set
			{
				this.incomingLossPercentage = value;
			}
		}

		public int LostPackagesOut { get; internal set; }

		public int LostPackagesIn { get; internal set; }

		public override string ToString()
		{
			return string.Format("NetworkSimulationSet {6}.  Lag in={0} out={1}. Jitter in={2} out={3}. Loss in={4} out={5}.", new object[]
			{
				this.incomingLag,
				this.outgoingLag,
				this.incomingJitter,
				this.outgoingJitter,
				this.incomingLossPercentage,
				this.outgoingLossPercentage,
				this.IsSimulationEnabled
			});
		}

		private bool isSimulationEnabled = false;

		private int outgoingLag = 100;

		private int outgoingJitter = 0;

		private int outgoingLossPercentage = 1;

		private int incomingLag = 100;

		private int incomingJitter = 0;

		private int incomingLossPercentage = 1;

		internal PeerBase peerBase;

		private Thread netSimThread;

		public readonly ManualResetEvent NetSimManualResetEvent = new ManualResetEvent(false);
	}
}
