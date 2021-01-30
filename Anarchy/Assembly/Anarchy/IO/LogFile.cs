using UnityEngine;

namespace Anarchy.IO
{
    public class LogFile : File
    {
        public static readonly string Directory = Application.dataPath + "/Logs/";
        public static readonly string Extension = ".log";

        public LogFile(string fileName) : base(Directory + fileName + Extension, false, true)
        {
            Clear();
        }

        public void Clear()
        {
            using (var writer = new System.IO.StreamWriter(info.FullName, false))
            {
                writer.Write(string.Empty);
            }
        }

        public override string ReadLine()
        {
            throw new System.InvalidOperationException($"You are trying to read something from LogFile. This file should be used only for writing!");
        }

        public override void WriteLine(string line)
        {
            base.WriteLine(line);
        }
    }
}