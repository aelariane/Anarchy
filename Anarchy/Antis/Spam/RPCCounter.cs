using Antis.Collections.Generic;
using Antis.Internal;

namespace Antis.Spam
{
    /// <summary>
    /// Simple RPC counter
    /// </summary>
    public sealed class RPCCounter : AntisThreadExecutable, ICounter<string>
    {
        private SyncDictionary<string, int> rpcList = new SyncDictionary<string, int>();

        private static int checkCount = 30;

        /// <summary>
        /// Amount of object spammed to define it as spam
        /// </summary>
        public static int CheckCount { get => checkCount; set => checkCount = value; }

        /// <summary>
        /// Sender ID
        /// </summary>
        public int Owner { get; }

        public static event SpamDetected<string> OnRPCSpamDetected = (sender, data) => { };

        /// <summary>
        ///
        /// </summary>
        /// <param name="ID">Owner ID</param>
        public RPCCounter(int ID)
        {
            Owner = ID;
            AntisThread.AddExecutable(this);
        }

        ~RPCCounter()
        {
            AntisThread.RemoveExecutable(this);
        }

        void ICounter.Count(object obj)
        {
            Count(obj as string ?? "null");
        }

        public void Count(string value)
        {
            lock (rpcList)
            {
                if (rpcList.TryGetValue(value, out int val))
                {
                    rpcList[value]++;
                    return;
                }
                rpcList.Add(value, 1);
            }
        }

        protected override void OnCheck()
        {
            lock (rpcList)
            {
                rpcList.Lock();
                foreach (var pair in rpcList)
                {
                    if (pair.Value > checkCount)
                    {
                        OnRPCSpamDetected(this, new SpamDetectedArgs<string>(pair.Key, pair.Value, Owner));
                    }
                }
                rpcList.Clear();
                rpcList.Unlock();
            }
        }
    }
}