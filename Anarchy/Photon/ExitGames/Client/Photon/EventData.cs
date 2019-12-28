using System;
using System.Collections.Generic;

namespace ExitGames.Client.Photon
{
	public class EventData
	{
		public object this[byte key]
		{
			get
			{
				bool flag = this.Parameters == null;
				object result;
				if (flag)
				{
					result = null;
				}
				else
				{
					object obj;
					this.Parameters.TryGetValue(key, out obj);
					result = obj;
				}
				return result;
			}
			internal set
			{
				bool flag = this.Parameters == null;
				if (flag)
				{
					this.Parameters = new Dictionary<byte, object>();
				}
				this.Parameters[key] = value;
			}
		}

		public int Sender
		{
			get
			{
				bool flag = this.sender == -1;
				if (flag)
				{
					object obj = this[this.SenderKey];
					this.sender = ((obj != null) ? ((int)obj) : -1);
				}
				return this.sender;
			}
			internal set
			{
				this.sender = value;
			}
		}

		public object CustomData
		{
			get
			{
				bool flag = this.customData == null;
				if (flag)
				{
					this.customData = this[this.CustomDataKey];
				}
				return this.customData;
			}
			internal set
			{
				this.customData = value;
			}
		}

		internal void Reset()
		{
			this.Code = 0;
			this.Parameters.Clear();
			this.sender = -1;
			this.customData = null;
		}

		public override string ToString()
		{
			return string.Format("Event {0}.", this.Code.ToString());
		}

		public string ToStringFull()
		{
			return string.Format("Event {0}: {1}", this.Code, SupportClass.DictionaryToString(this.Parameters));
		}

		public byte Code;

		public Dictionary<byte, object> Parameters;

		public byte SenderKey = 254;

		private int sender = -1;

		public byte CustomDataKey = 245;

		private object customData;
	}
}
