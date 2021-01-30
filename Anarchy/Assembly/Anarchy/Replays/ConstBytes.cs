using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Anarchy.Replays
{
    public class ConstBytes
    {
        /// <summary>
        /// After this byte comes position of cursor and camera in frame
        /// </summary>
        public const byte FrameCameraUpdate = 222;
        /// <summary>
        /// Indicates end of frame
        /// </summary>
        public const byte EndOfFrame = 255;
        /// <summary>
        /// Indicates null value
        /// </summary>
        public const byte NullByte = 42;

    }
}
