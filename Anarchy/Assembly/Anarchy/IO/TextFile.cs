using UnityEngine;

namespace Anarchy.IO
{
    internal class TextFile : File
    {
        public static readonly string Directory = Application.dataPath + "/TextFiles/";
        public static readonly string Extension = ".txt";

        public TextFile(string name) : base(Directory + name + Extension, false, true)
        {
            Clear();
        }

        public TextFile(string path, string name) : base(path + name, false, true)
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
    }
}