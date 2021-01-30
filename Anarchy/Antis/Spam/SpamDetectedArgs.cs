using System;

namespace Antis.Spam
{
    public sealed class SpamDetectedArgs : EventArgs
    {
        /// <summary>
        /// Delay between checks in checking Thread
        /// </summary>
        public int CheckDelay => Internal.AntisThread.SleepTime;

        /// <summary>
        /// Amount of how much times <seealso cref="SpammedObject"/> was counted
        /// </summary>
        public int Count { get; }

        /// <summary>
        /// Sender ID
        /// </summary>
        public int Sender { get; }

        /// <summary>
        /// Spammed object
        /// </summary>
        public object SpammedObject { get; }

        public SpamDetectedArgs(object spammed, int count, int sender)
        {
            SpammedObject = spammed;
            Count = count;
            Sender = sender;
        }

        public override string ToString()
        {
            return $"Spammed {SpammedObject.ToString()} x {Count} times (CheckDelay {CheckDelay}ms).{(Sender > 0 ? " Sender : " + Sender.ToString() : string.Empty)}";
        }
    }

    public sealed class SpamDetectedArgs<T> : EventArgs
    {
        /// <summary>
        /// Delay between checks in checking Thread
        /// </summary>
        public int CheckDelay => Internal.AntisThread.SleepTime;

        /// <summary>
        /// Amount of how much times <seealso cref="SpammedObject"/> was counted
        /// </summary>
        public int Count { get; }

        /// <summary>
        /// Sender ID
        /// </summary>
        public int Sender { get; }

        /// <summary>
        /// Spammed object
        /// </summary>
        public T SpammedObject { get; }

        public SpamDetectedArgs(T spammed, int count, int sender)
        {
            SpammedObject = spammed;
            Count = count;
            Sender = sender;
        }

        public override string ToString()
        {
            return $"Spammed {SpammedObject.ToString()} x {Count} times (CheckDelay {CheckDelay}ms).{(Sender > 0 ? " Sender : " + Sender.ToString() : string.Empty)}";
        }
    }
}