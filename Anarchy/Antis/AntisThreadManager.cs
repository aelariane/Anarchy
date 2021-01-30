using Antis.Internal;

namespace Antis
{
    public static class AntisThreadManager
    {
        /// <summary>
        /// Delay between checks in checking Thread in ms
        /// </summary>
        public static int ThreadSleepTime
        {
            get => AntisThread.SleepTime;
            set => AntisThread.SleepTime = value;
        }

        public static void OnApplicationQuit()
        {
            AntisThread.ThreadInstance.Abort();
        }

        public static void RegisterExecutable(AntisThreadExecutable exec)
        {
            AntisThread.AddExecutable(exec);
        }

        public static void RemoveExecutable(AntisThreadExecutable exec)
        {
            AntisThread.RemoveExecutable(exec);
        }
    }
}