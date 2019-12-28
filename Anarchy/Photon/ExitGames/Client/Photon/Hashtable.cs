using System;
using System.Collections;
using System.Collections.Generic;

namespace ExitGames.Client.Photon
{
	public class Hashtable : Dictionary<object, object>
	{
		public Hashtable()
		{
		}

		public Hashtable(int x) : base(x)
		{
		}

		public new object this[object key]
		{
			get
			{
				object result = null;
				base.TryGetValue(key, out result);
				return result;
			}
			set
			{
				base[key] = value;
			}
		}

        public T Get<T>(object key)
        {
            if (ContainsKey(key))
            {
                if(base[key] is T)
                {
                    return (T)base[key];
                }
            }
            return default(T);
        }

		public new IEnumerator<DictionaryEntry> GetEnumerator()
		{
			return new DictionaryEntryEnumerator(((IDictionary)this).GetEnumerator());
		}

		public override string ToString()
		{
			List<string> list = new List<string>();
			foreach (object obj in base.Keys)
			{
				bool flag = obj == null || this[obj] == null;
				if (flag)
				{
					list.Add(obj + "=" + this[obj]);
				}
				else
				{
					list.Add(string.Concat(new object[]
					{
						"(",
						obj.GetType(),
						")",
						obj,
						"=(",
						this[obj].GetType(),
						")",
						this[obj]
					}));
				}
			}
			return string.Join(", ", list.ToArray());
		}

		public object Clone()
		{
			return new Dictionary<object, object>(this);
		}
	}
}
