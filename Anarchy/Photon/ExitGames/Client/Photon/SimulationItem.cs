using System;
using System.Diagnostics;

namespace ExitGames.Client.Photon
{
	internal class SimulationItem
	{
		public SimulationItem()
		{
			this.stopw = new Stopwatch();
			this.stopw.Start();
		}

		public int Delay { get; internal set; }

		internal readonly Stopwatch stopw;

		public int TimeToExecute;

		public byte[] DelayedData;
	}
}
