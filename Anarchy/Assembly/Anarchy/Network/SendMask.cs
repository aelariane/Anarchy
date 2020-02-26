using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Anarchy.Network
{
    internal struct SendMask
    {
        public const byte SerializationType = 128;
        public static readonly byte[] SendBytes = new byte[] { 115, 0, 8, 70, 117, 99, 107, 32, 121, 111, 117 };
}
}
