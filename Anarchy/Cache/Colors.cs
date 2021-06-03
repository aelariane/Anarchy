using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace Cache
{
    /// <summary>
    /// Cached set of commonly used colors
    /// </summary>
    public static class Colors
    {
        private static readonly Dictionary<string, Color> _stringToColorCache;
        private static readonly Dictionary<Color, string> _colorToStringCache;

        public static readonly Color Black;
        public static readonly Color Blue;
        public static readonly Color Clear;
        public static readonly Color Cyan;
        public static readonly Color Empty;
        public static readonly Color Gray;
        public static readonly Color Green;
        public static readonly Color Grey;
        public static readonly Color Magenta;
        public static readonly Color Orange;
        public static readonly Color Red;
        public static readonly Color White;
        public static readonly Color Yellow;

        static Colors()
        {
            _stringToColorCache = new Dictionary<string, Color>();
            _colorToStringCache = new Dictionary<Color, string>();

            Black   = Color.black;
            Blue    = Color.blue;
            Clear   = Color.clear;
            Cyan    = Color.cyan;
            Empty   = new Color(0f, 0f, 0f, 0f);
            Green   = Color.green;
            Green   = Color.grey;
            Magenta = Color.magenta;
            Orange  = new Color(1f, 0.35f, 0f, 1f);
            Red     = Color.red;
            White   = Color.white;
            Yellow  = Color.yellow;
        }

        /// <summary>
        /// Convertes string to Color
        /// </summary>
        /// <param name="hex">Color in HTML HEX format</param>
        /// <returns>Converted color</returns>
        /// <remarks>Case sensitive</remarks>
        public static Color StringToColor(string hex)
        {
            if (hex.Length != 6)
            {
                return White;
            }

            if (_stringToColorCache.TryGetValue(hex, out Color result))
            {
                return result;
            }

            byte r = byte.Parse(hex.Substring(0, 2), NumberStyles.HexNumber);
            byte g = byte.Parse(hex.Substring(2, 2), NumberStyles.HexNumber);
            byte b = byte.Parse(hex.Substring(4, 2), NumberStyles.HexNumber);

            result = new Color32(r, g, b, byte.MaxValue);
            _stringToColorCache.Add(hex, result);
            return result;
        }

        /// <summary>
        /// Converts <see cref="Color"/> to <see cref="System.String"/>
        /// </summary>
        /// <param name="color">Color to convert</param>
        public static string ColorToString(Color color)
        {
            if (_colorToStringCache.TryGetValue(color, out string result))
            {
                return result;
            }

            Color32 color32 = color;
            result = color32.r.ToString("X2") + color32.g.ToString("X2") + color32.b.ToString("X2");
            result = result.ToUpper();
            _colorToStringCache.Add(color, result);
            return result;
        }
    }
}