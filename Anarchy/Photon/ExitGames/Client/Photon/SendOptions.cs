using System;

namespace ExitGames.Client.Photon
{
	public struct SendOptions
	{
		public bool Reliability
		{
			get
			{
				return this.DeliveryMode == DeliveryMode.Reliable;
			}
			set
			{
				this.DeliveryMode = (value ? DeliveryMode.Reliable : DeliveryMode.Unreliable);
			}
		}

		public static readonly SendOptions SendReliable = new SendOptions
		{
			Reliability = true
		};

		public static readonly SendOptions SendUnreliable = new SendOptions
		{
			Reliability = false
		};

		public DeliveryMode DeliveryMode;

		public bool Encrypt;

		public byte Channel;
	}
}
