using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Anarchy.Custom.Level
{
    //TODO: Anarchy maps in binary? + Compiler for them to conver from RC's scripts?
    public class CustomLevelData
    {
        public Dictionary<short, string> Objects { get; }
        public Dictionary<byte, string> Textures { get; }
        public int GlobalObjectsCount { get; }

        public CustomLevelData(string path)
        {

        }

        public CustomLevelData(byte[] rawData)
        {

        }

        
    }
}
