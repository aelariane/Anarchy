using System.Collections.Generic;
using System.Threading;

namespace Antis.Internal
{
    internal static class AntisThread
    {
        private static List<AntisThreadExecutable> executables;
        private static int nextSleepTime = -1;
        private static int sleepTime = 10;

        public static int SleepTime
        {
            get
            {
                return sleepTime;
            }
            set
            {
                if (value > 0)
                {
                    nextSleepTime = value;
                }
            }
        }

        internal static Thread ThreadInstance { get; private set; }

        static AntisThread()
        {
            if (executables == null)
            {
                executables = new List<AntisThreadExecutable>();
            }
            ThreadInstance = new Thread(Loop) { IsBackground = false, Name = "AntisThread" };
            ThreadInstance.Start();
        }

        internal static void AddExecutable(AntisThreadExecutable execute)
        {
            if (executables == null)
            {
                executables = new List<AntisThreadExecutable>();
            }
            lock (executables)
            {
                if (!executables.Contains(execute))
                {
                    executables.Add(execute);
                }
            }
        }

        internal static void RemoveExecutable(AntisThreadExecutable execute)
        {
            if (executables == null)
            {
                return;
            }
            lock (executables)
            {
                if (executables.Contains(execute))
                {
                    executables.Remove(execute);
                }
            }
        }

        private static void Loop()
        {
            AntisThreadExecutable[] exec = null;
            bool needRemove = false;
            while (true)
            {
                lock (executables)
                {
                    exec = executables.ToArray();
                }
                if (exec.Length > 0)
                {
                    int iterator = 0;
                    while (iterator < exec.Length)
                    {
                        AntisThreadExecutable executable = exec[iterator++];
                        if (executable == null)
                        {
                            needRemove = true;
                            continue;
                        }
                        executable.Check();
                    }
                }
                if (needRemove)
                {
                    lock (executables)
                    {
                        executables.RemoveAll(delegate (AntisThreadExecutable execute) { return execute == null; });
                    }
                    needRemove = false;
                }
                Thread.Sleep(sleepTime);
                if (nextSleepTime > 0)
                {
                    sleepTime = nextSleepTime;
                    nextSleepTime = -1;
                }
            }
        }
    }
}