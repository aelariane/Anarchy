using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;

namespace ExitGames.Client.Photon
{
	public class SupportClass
	{
		public static List<MethodInfo> GetMethods(Type type, Type attribute)
		{
			List<MethodInfo> list = new List<MethodInfo>();
			bool flag = type == null;
			List<MethodInfo> result;
			if (flag)
			{
				result = list;
			}
			else
			{
				MethodInfo[] methods = type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				foreach (MethodInfo methodInfo in methods)
				{
					bool flag2 = attribute == null || methodInfo.IsDefined(attribute, false);
					if (flag2)
					{
						list.Add(methodInfo);
					}
				}
				result = list;
			}
			return result;
		}

		public static int GetTickCount()
		{
			return SupportClass.IntegerMilliseconds();
		}

		[Obsolete("Use StartBackgroundCalls() instead. It works with StopBackgroundCalls().")]
		public static byte CallInBackground(Func<bool> myThread, int millisecondsInterval = 100, string taskName = "")
		{
			return SupportClass.StartBackgroundCalls(myThread, millisecondsInterval, null);
		}

		public static byte StartBackgroundCalls(Func<bool> myThread, int millisecondsInterval = 100, string taskName = "")
		{
			bool flag = SupportClass.threadList == null;
			if (flag)
			{
				SupportClass.threadList = new List<Thread>();
			}
			Thread thread = new Thread(delegate()
			{
				while (myThread())
				{
					Thread.Sleep(millisecondsInterval);
				}
			});
			bool flag2 = !string.IsNullOrEmpty(taskName);
			if (flag2)
			{
				thread.Name = taskName;
			}
			thread.IsBackground = true;
			thread.Start();
			SupportClass.threadList.Add(thread);
			return (byte)(SupportClass.threadList.Count - 1);
		}

		public static bool StopBackgroundCalls(byte id)
		{
			bool flag = SupportClass.threadList == null || (int)id > SupportClass.threadList.Count || SupportClass.threadList[(int)id] == null;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				SupportClass.threadList[(int)id].Abort();
				result = true;
			}
			return result;
		}

		public static bool StopAllBackgroundCalls()
		{
			bool flag = SupportClass.threadList == null;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				foreach (Thread thread in SupportClass.threadList)
				{
					thread.Abort();
				}
				result = true;
			}
			return result;
		}

		public static void WriteStackTrace(Exception throwable, TextWriter stream)
		{
			bool flag = stream != null;
			if (flag)
			{
				stream.WriteLine(throwable.ToString());
				stream.WriteLine(throwable.StackTrace);
				stream.Flush();
			}
			else
			{
				Debug.WriteLine(throwable.ToString());
				Debug.WriteLine(throwable.StackTrace);
			}
		}

		public static void WriteStackTrace(Exception throwable)
		{
			SupportClass.WriteStackTrace(throwable, null);
		}

		public static string DictionaryToString(IDictionary dictionary)
		{
			return SupportClass.DictionaryToString(dictionary, true);
		}

		public static string DictionaryToString(IDictionary dictionary, bool includeTypes)
		{
			bool flag = dictionary == null;
			string result;
			if (flag)
			{
				result = "null";
			}
			else
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append("{");
				foreach (object obj in dictionary.Keys)
				{
					bool flag2 = stringBuilder.Length > 1;
					if (flag2)
					{
						stringBuilder.Append(", ");
					}
					bool flag3 = dictionary[obj] == null;
					Type type;
					string text;
					if (flag3)
					{
						type = typeof(object);
						text = "null";
					}
					else
					{
						type = dictionary[obj].GetType();
						text = dictionary[obj].ToString();
					}
					bool flag4 = typeof(IDictionary) == type || typeof(Hashtable) == type;
					if (flag4)
					{
						text = SupportClass.DictionaryToString((IDictionary)dictionary[obj]);
					}
					bool flag5 = typeof(string[]) == type;
					if (flag5)
					{
						text = string.Format("{{{0}}}", string.Join(",", (string[])dictionary[obj]));
					}
					bool flag6 = typeof(byte[]) == type;
					if (flag6)
					{
						text = string.Format("byte[{0}]", ((byte[])dictionary[obj]).Length);
					}
					if (includeTypes)
					{
						stringBuilder.AppendFormat("({0}){1}=({2}){3}", new object[]
						{
							obj.GetType().Name,
							obj,
							type.Name,
							text
						});
					}
					else
					{
						stringBuilder.AppendFormat("{0}={1}", obj, text);
					}
				}
				stringBuilder.Append("}");
				result = stringBuilder.ToString();
			}
			return result;
		}

		[Obsolete("Use DictionaryToString() instead.")]
		public static string HashtableToString(Hashtable hash)
		{
			return SupportClass.DictionaryToString(hash);
		}

        public static string StreamBufferToString(StreamBuffer buff)
        {
            StringBuilder bld = new StringBuilder();
            bld.Append("StreamBuffer with length: " + buff.IntLength + "\n");
            int length = buff.IntLength;
            byte[] buffer = buff.GetBuffer();
            for(int i =0; i < length; i++)
            {
                bld.Append(buffer[i].ToString() + "\n");
            }
            return bld.ToString();
        }

		public static string ByteArrayToString(byte[] list)
		{
			bool flag = list == null;
			string result;
			if (flag)
			{
				result = string.Empty;
			}
			else
			{
				result = BitConverter.ToString(list);
			}
			return result;
		}

		private static uint[] InitializeTable(uint polynomial)
		{
			uint[] array = new uint[256];
			for (int i = 0; i < 256; i++)
			{
				uint num = (uint)i;
				for (int j = 0; j < 8; j++)
				{
					bool flag = (num & 1u) == 1u;
					if (flag)
					{
						num = (num >> 1 ^ polynomial);
					}
					else
					{
						num >>= 1;
					}
				}
				array[i] = num;
			}
			return array;
		}

		public static uint CalculateCrc(byte[] buffer, int length)
		{
			uint num = uint.MaxValue;
			uint polynomial = 3988292384u;
			bool flag = SupportClass.crcLookupTable == null;
			if (flag)
			{
				SupportClass.crcLookupTable = SupportClass.InitializeTable(polynomial);
			}
			for (int i = 0; i < length; i++)
			{
				num = (num >> 8 ^ SupportClass.crcLookupTable[(int)((uint)buffer[i] ^ (num & 255u))]);
			}
			return num;
		}

		private static List<Thread> threadList;

		protected internal static SupportClass.IntegerMillisecondsDelegate IntegerMilliseconds = () => Environment.TickCount;

		private static uint[] crcLookupTable;

		public delegate int IntegerMillisecondsDelegate();

		public class ThreadSafeRandom
		{
			public static int Next()
			{
				Random r = SupportClass.ThreadSafeRandom._r;
				int result;
				lock (r)
				{
					result = SupportClass.ThreadSafeRandom._r.Next();
				}
				return result;
			}

			private static readonly Random _r = new Random();
		}
	}
}
