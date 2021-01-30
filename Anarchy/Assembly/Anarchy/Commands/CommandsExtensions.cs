using System.Collections.Generic;

namespace Anarchy.Commands
{
    internal static class CommandsExtensions
    {
        public static string ParseStringWithSpaces(string[] src, ref int index)
        {
            string sym = '"'.ToString();
            if (src[index].StartsWith(sym))
            {
                int startIndex = index;
                bool found = false;
                List<string> tmp = new List<string>();
                tmp.Add(src[index++].Substring(1));
                while (index < src.Length)
                {
                    if (src[index].EndsWith(sym))
                    {
                        found = true;
                        tmp.Add(src[index].Remove(src[index].Length - 1, 1));
                        break;
                    }
                    tmp.Add(src[index++]);
                }
                if (found)
                {
                    return string.Join(" ", tmp.ToArray());
                }
                index = startIndex;
            }
            return src[index];
        }

        public static string[] ParseCommandArgs(string src, int startIndex = 0)
        {
            string[] arr = src.Split(' ');
            var tmp = new List<string>();
            for (int i = startIndex; i < arr.Length; i++)
            {
                tmp.Add(ParseStringWithSpaces(arr, ref i));
            }
            return tmp.ToArray();
        }

        public static KeyValuePair<string, string> ParseStringPair(string src, char separator)
        {
            string[] split = src.Split(separator);
            string key = split[0];
            string value = split[1];
            if (split.Length > 2)
            {
                for (int i = 2; i < split.Length; i++)
                {
                    value += (separator + split[i]);
                }
            }
            return new KeyValuePair<string, string>(key, value);
        }

        public static bool TryParseStringPair(string src, char separator, out KeyValuePair<string, string> result)
        {
            //Lazy way
            try
            {
                result = ParseStringPair(src, separator);
                return true;
            }
            catch
            {
                result = default;
                return false;
            }
        }
    }
}