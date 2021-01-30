using Antis.Collections.Generic;
using Antis.Internal;

namespace Antis.Spam
{
    /// <summary>
    /// Simple counter of objects sent to instantiate
    /// </summary>
    public sealed class InstantiateCounter : AntisThreadExecutable, ICounter<string>
    {
        private SyncDictionary<string, int> instantiatesList = new SyncDictionary<string, int>();

        private static int checkCount = 40;

        /// <summary>
        /// Amount of object spammed to define it as spam
        /// </summary>
        public static int CheckCount { get => checkCount; set => checkCount = value; }

        /// <summary>
        /// Sender ID
        /// </summary>
        public int Owner { get; }

        public static event SpamDetected<string> OnInstantiateSpamDetected = (sender, data) => { };

        /// <summary>
        ///
        /// </summary>
        /// <param name="ID">Owner ID</param>
        public InstantiateCounter(int ID)
        {
            Owner = ID;
            AntisThread.AddExecutable(this);
        }

        ~InstantiateCounter()
        {
            AntisThread.RemoveExecutable(this);
        }

        void ICounter.Count(object obj)
        {
            Count(obj as string ?? "null");
        }

        public void Count(string value)
        {
            lock (instantiatesList)
            {
                if (instantiatesList.TryGetValue(value, out int val))
                {
                    instantiatesList[value]++;
                    return;
                }
                instantiatesList.Add(value, 1);
            }
        }

        protected override void OnCheck()
        {
            lock (instantiatesList)
            {
                instantiatesList.Lock();
                instantiatesList.Clear();
                foreach (var pair in instantiatesList)
                {
                    if (pair.Value > checkCount)
                    {
                        OnInstantiateSpamDetected(this, new SpamDetectedArgs<string>(pair.Key, pair.Value, Owner));
                    }
                }
                instantiatesList.Unlock();
            }
        }
    }
}