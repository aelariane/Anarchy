using System;
using System.IO;

namespace Anarchy.IO
{
    public abstract class File : IDisposable
    {
        private readonly object locker = new object();

        public readonly bool AlwaysOpen = false;
        public readonly string Path;
        protected FileInfo info;
        protected FileStream fileStream;
        protected StreamReader textReader;
        protected StreamWriter textWriter;

        public string DirectoryName => info.DirectoryName;
        public bool Exists => info.Exists;
        public string Name => info.Name;

        public File(string path) : this(path, false, true)
        {
        }

        public File(string path, bool alwaysOpen, bool autocreate)
        {
            Path = path;
            info = new FileInfo(Path);
            if (!info.Directory.Exists)
            {
                Directory.CreateDirectory(info.Directory.FullName);
            }
            if (autocreate)
            {
                if (!info.Exists)
                {
                    info.Create().Close();
                }
            }
            if (alwaysOpen)
            {
                Open();
            }
            AlwaysOpen = alwaysOpen;
        }

        ~File()
        {
        }

        public void Close()
        {
            if (AlwaysOpen)
            {
                return;
            }

            lock (locker)
            {
                Dispose();
            }
        }

        public void Create()
        {
            lock (locker)
            {
                if (info.Exists)
                {
                    info.Delete();
                }

                info.Create();
                using (StreamWriter writer = new StreamWriter(Path, false))
                {
                    writer.Write(string.Empty);
                }
            }
        }

        public void Delete()
        {
            lock (locker)
            {
                Dispose();
                info.Delete();
            }
        }

        public virtual void Dispose()
        {
            lock (locker)
            {
                textReader?.Dispose();
                textWriter?.Dispose();
                fileStream?.Dispose();
            }
        }

        public void Open()
        {
            if ((AlwaysOpen && fileStream != null) || !info.Exists)
            {
                return;
            }

            Dispose();
            fileStream = info.Open(FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read);
            textReader = new StreamReader(fileStream);
            textWriter = new StreamWriter(fileStream);
        }

        public virtual string ReadLine()
        {
            if (!info.Exists)
            {
                return string.Empty;
            }

            if (AlwaysOpen)
            {
                return textReader.ReadLine();
            }
            using (textReader = info.OpenText())
            {
                return textReader.ReadLine();
            }
        }

        public virtual void WriteLine(string line)
        {
            if (!info.Exists)
            {
                return;
            }

            if (AlwaysOpen)
            {
                textWriter.WriteLine(line);
                return;
            }
            using (textWriter = info.AppendText())
            {
                textWriter.WriteLine(line);
            }
        }
    }
}