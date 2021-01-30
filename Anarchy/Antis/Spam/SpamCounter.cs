using Antis.Collections.Generic;
using Antis.Internal;

namespace Antis.Spam
{
    /// <summary>
    /// Simple and not so optimized, but easy to use anywhere to check simple spam
    /// </summary>
    public sealed class SpamCounter : AntisThreadExecutable
    {
        private SyncDictionary<string, SpamCounterData> data = new SyncDictionary<string, SpamCounterData>();
        internal static SpamCounter Instance = new SpamCounter();

        public static event SpamDetected OnSpamDetected = (obj, sender) => { };

        /// <summary>
        /// Amount of object spammed to define it as spam
        /// </summary>
        public static int CheckCount { get; set; } = 15;

        internal SpamCounter()
        {
            AntisThread.AddExecutable(this);
        }

        ~SpamCounter()
        {
            AntisThread.RemoveExecutable(this);
        }

        /// <summary>
        /// </summary>
        /// <param name="spam">Spammed object as string</param>
        /// <param name="sender">Sender ID</param>
        /// <exception cref="System.ArgumentNullException">If <paramref name="spam"/> is null</exception>
        public static void Count(string spam, int sender)
        {
            if (spam == null)
            {
                throw new System.ArgumentNullException(nameof(spam));
            }
            Instance.CountInternal(spam, sender);
        }

        internal void CountInternal(string val, int sender)
        {
            string key = val + sender.ToString();
            if (data.TryGetValue(key, out SpamCounterData dat))
            {
                dat.Count++;
                return;
            }
            data.Add(key, new SpamCounterData(sender, val));
        }

        protected override void OnCheck()
        {
            if (System.Threading.Thread.CurrentThread.ManagedThreadId != AntisThread.ThreadInstance.ManagedThreadId)
            {
                throw new ThreadExecutableException();
            }
            data.Lock();
            data.Clear();
            foreach (var pair in data)
            {
                if (pair.Value.Count > CheckCount)
                {
                    OnSpamDetected(this, new SpamDetectedArgs(pair.Value.ResourceName, pair.Value.Count, pair.Value.Sender));
                }
            }
            data.Unlock();
        }

        private class SpamCounterData
        {
            public readonly int Sender = -1;
            public int Count;
            public readonly string ResourceName;

            public SpamCounterData(int sender, string name)
            {
                Count = 1;
                this.Sender = sender;
                ResourceName = name;
            }

            public void Reset()
            {
                Count = 0;
            }
        }
    }
}