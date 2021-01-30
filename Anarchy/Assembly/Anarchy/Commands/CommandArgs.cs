using System.Collections.Generic;

namespace Anarchy.Commands
{
    internal class CommandArgs
    {
        public readonly char Separator;
        public const string FLAG_EMPTY = "$FLAG_EMPTY$";
        private Dictionary<string, string> flags = new Dictionary<string, string>();
        private string[] source;
        public string[] RawArgs { get; private set; }
        public int RawArgsCount { get; private set; }
        public int FlagsCount => flags.Count;

        public string this[int key]
        {
            get
            {
                return source[key];
            }
        }

        public CommandArgs(string[] source) : this(source, '=')
        {
        }

        public CommandArgs(string[] source, char separator)
        {
            this.source = new string[source.Length];
            for (int i = 0; i < source.Length; i++)
            {
                this.source[i] = source[i];
            }
            Separator = separator;
            Parse(source);
        }

        private void Parse(string[] src)
        {
            var tmp = new List<string>();
            foreach (string str in src)
            {
                if (str.StartsWith("-"))
                {
                    int rmCount = 1;
                    if (str.StartsWith("--"))
                    {
                        rmCount++;
                    }

                    string[] flag = str.Substring(rmCount).Split(Separator);
                    string val = string.Join(Separator.ToString(), flag, 1, flag.Length - 1);
                    flags.Add(flag[0], flag[1]);
                    continue;
                }
                tmp.Add(str);
            }
            RawArgs = tmp.ToArray();
            RawArgsCount = tmp.Count;
        }

        public bool IsDefined(string key)
        {
            return flags.ContainsKey(key);
        }

        public bool HasValue(string key)
        {
            if (IsDefined(key))
            {
                return flags[key] != FLAG_EMPTY;
            }
            return false;
        }

        public bool TryGetFloat(string key, out float val)
        {
            if (HasValue(key))
            {
                return float.TryParse(flags[key], out val);
            }
            val = default;
            return false;
        }

        public bool TryGetFloat(int rawIndex, out float val)
        {
            return float.TryParse(RawArgs[rawIndex], out val);
        }

        public bool TryGetInt(string key, out int val)
        {
            if (HasValue(key))
            {
                return int.TryParse(flags[key], out val);
            }
            val = default;
            return false;
        }

        public bool TryGetInt(int rawIndex, out int val)
        {
            return int.TryParse(RawArgs[rawIndex], out val);
        }
    }
}