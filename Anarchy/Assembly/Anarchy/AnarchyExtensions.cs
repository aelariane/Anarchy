using Optimization.Caching;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Anarchy
{
    public static class AnarchyExtensions
    {
        private static readonly Regex hexCode = new Regex(@"\[([0-9a-f]{6})\]", RegexOptions.IgnoreCase);

        public static int GetCyanPoints() =>
            PhotonNetwork.playerList.Where(player => player.RCteam == 1).Sum(player => player.Kills);

        public static int GetMagentaPoints() =>
            PhotonNetwork.playerList.Where(player => player.RCteam == 2).Sum(player => player.Kills);

        public static string HtmlColor(this string src, string color)
        {
            if (color.Length != 6)
            {
                return src;
            }

            return "<color=#" + color + ">" + src + "</color>";
        }

        public static string AppendLine(this string src, string addition)
        {
            if (src.Length > 0)
            {
                src += Environment.NewLine;
            }

            return src + addition;
        }

        public static bool CheckForInternetConnection()
        {
            try
            {
                using (var client = new WebClient())
                {
                    using (client.OpenRead("http://google.com"))
                    {
                        return true;
                    }
                }
            }
            catch
            {
                return false;
            }
        }

        public static string ColorToString(this Color colorb)
        {
            Color32 color = colorb;
            return color.r.ToString("X2") + color.g.ToString("X2") + color.b.ToString("X2");
        }

        public static string ColorToString(this Color32 color)
        {
            return color.r.ToString("X2") + color.g.ToString("X2") + color.b.ToString("X2");
        }

        public static int CountWords(this string s, string s1)
        {
            return (s.Length - s.Replace(s1, "").Length) / s1.Length;
        }

        public static T Find<T>(this IEnumerable<T> ienum, Func<T, bool> func)
        {
            if (ienum == null)
            {
                return default;
            }

            foreach (var idk in ienum)
            {
                if (func(idk))
                {
                    return idk;
                }
            }

            return default;
        }

        public static float GetFloat(object obj)
        {
            if (obj is float f)
            {
                return f;
            }

            return 0f;
        }

        public static Color HexToColor(this string hex, byte a = 255)
        {
            if (hex.Length != 6)
            {
                return Colors.white;
            }

            byte r = byte.Parse(hex.Substring(0, 2), NumberStyles.HexNumber);
            byte g = byte.Parse(hex.Substring(2, 2), NumberStyles.HexNumber);
            byte b = byte.Parse(hex.Substring(4, 2), NumberStyles.HexNumber);
            return new Color32(r, g, b, a);
        }

        public static bool IsAny<T>(this IEnumerable<T> me, Func<T, bool> state) => me != null && me.All(state);

        public static bool IsNullOrEmpty(this string s) => string.IsNullOrEmpty(s);

        public static bool IsNullOrWhiteSpace(this string s) => string.IsNullOrEmpty(s) || s.All(c => c == ' ');

        public static bool IsImage(this string path)
        {
            return path.EndsWith(".png") || path.EndsWith(".jpg") || path.EndsWith(".jpeg");
        }

        public static Vector3 ParseVector3(this string str)
        {
            if (!str.StartsWith("(") && !str.EndsWith(")"))
            {
                return Vectors.zero;
            }

            str = str.Replace("(", string.Empty).Replace(")", string.Empty);
            string[] toParse = str.Split(',');
            if (toParse.Length != 3)
            {
                return Vectors.zero;
            }

            Vector3 result = new Vector3();
            float.TryParse(toParse[0], out var val);
            result.x = val;
            float.TryParse(toParse[1], out val);
            result.y = val;
            float.TryParse(toParse[2], out val);
            result.z = val;
            return result;
        }

        public static void RemoveAt<T>(ref T[] source, int index)
        {
            bool flag = source.Length == 1;
            if (flag)
            {
                source = new T[0];
            }
            else
            {
                bool flag2 = source.Length > 1;
                if (flag2)
                {
                    T[] array = new T[source.Length - 1];
                    int i = 0;
                    int num = 0;
                    while (i < source.Length)
                    {
                        bool flag3 = i != index;
                        if (flag3)
                        {
                            array[num] = source[i];
                            num++;
                        }

                        i++;
                    }

                    source = array;
                }
            }
        }

        /// <summary>
        /// Limit a string to a specified length.
        /// </summary>
        public static string LimitToLength(this string value, int length) =>
            value.Length <= length ? value : value.Substring(0, length) + "...";

        /// <summary>
        /// Limit a string a specified length (it doesn't count length of HTML and hex colors in the string).
        /// </summary>
        public static string LimitToLengthStripped(this string value, int length) =>
            value.RemoveHex().RemoveHTML().Length <= length ? value : (value.Substring(0, length) + "...");

        /// <summary>
        /// Remove hex colors from a string.
        /// Example: [000000] would be removed
        /// </summary>
        public static string RemoveHex(this string str) =>
            hexCode.Replace(str, string.Empty).Replace("[-]", string.Empty);

        /// <summary>
        /// Remove HTML tags like bold, italic, size and color from a string.
        /// Example: <color=#000000></color> would be removed
        /// </summary>
        public static string RemoveHTML(this string str) => Regex.Replace(str,
            @"((<(\/|)(color(?(?=\=).*?)>|b>|size.*?>|i>)))", "", RegexOptions.IgnoreCase);

        /// <summary>
        /// Remove hex colors and HTML tags like bold, italic, size and color from a string.
        /// Example: [000000] would be removed
        /// </summary>
        public static string RemoveAll(this string x) => Regex.Replace(x,
            @"((\[([0-9a-f]{6})\])|(<(\/|)(color(?(?=\=).*?)>))|(<size=(\\w*)?>?|<\/size>?)|(<\/?[bi]>))", string.Empty, RegexOptions.IgnoreCase);

        /// <summary>
        /// Remove size tags from a string.
        /// Example: <color=#000000></color> would be removed
        /// </summary>
        internal static string RemoveSize(this string str) => Regex.Replace(str, "<size=(\\w*)?>?|<\\/size>?",
            string.Empty, RegexOptions.IgnoreCase);

        /// <summary>
        /// Convert hex color to an HTML code.
        /// Example: [000000] would be changed to <color=#000000></color>
        /// </summary>
        public static string ToHTMLFormat(this string str)
        {
            if (hexCode.IsMatch(str))
            {
                str = str.Contains("[-]")
                    ? hexCode.Replace(str, "<color=#$1>").Replace("[-]", "</color>")
                    : hexCode.Replace(str, "<color=#$1>");
                var c = (short)(str.CountWords("<color=") - str.CountWords("</color>"));
                for (short i = 0; i < c; i++)
                {
                    str += "</color>";
                }
            }

            return str;
        }

        //Credits to Hawk. Took from his Shelter mod
        public static string ValidateUnityTags(string text)
        {
            StringBuilder builder = new StringBuilder(text);

            Stack<Tag> tags = new Stack<Tag>();
            var matches = Regex.Matches(text, @"<(\/?)(\w+)(?:=.+?)?>");
            foreach (Match match in matches)
            {
                var tag = new Tag(
                    match.Groups[2].Value,
                    string.IsNullOrEmpty(match.Groups[1].Value));

                if (tag.IsOpeningTag)
                {
                    tags.Push(tag);
                }
                else
                {
                    if (tags.Count > 0 && tags.Peek().TagName == tag.TagName)
                    {
                        tags.Pop();
                    }
                    else
                    {
                        builder.Remove(match.Index, match.Length);
                    }
                }
            }
            while (tags.Count > 0)
            {
                builder.Append(tags.Pop().ClosingTag);
            }

            return builder.ToString();
        }

        public static string Vector3ToString(this Vector3 vec)
        {
            StringBuilder bld = new StringBuilder();
            bld.Append("(");
            bld.Append(vec.x + ",");
            bld.Append(vec.y + ",");
            bld.Append(vec.z);
            bld.Append(")");
            return bld.ToString();
        }

        private struct Tag
        {
            private readonly string _name;
            private readonly bool _isOpening;

            public Tag(string name, bool isOpening)
            {
                _name = name;
                _isOpening = isOpening;
            }

            public string TagName => _name;
            public bool IsOpeningTag => _isOpening;

            public string ClosingTag => $"</{TagName}>";
        }
    }
}