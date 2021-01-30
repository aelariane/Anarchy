using Antis.Internal;
using System.Collections.Generic;

namespace Antis.Spam
{
    /// <summary>
    /// Simple events counter
    /// </summary>
    public class EventsCounter : AntisThreadExecutable, ICounter<byte>
    {
        private List<byte> receivedEvents = new List<byte>();
        private int[] events = new int[256];
        private static int checkCount = 25;

        /// <summary>
        /// Amount of object spammed to define it as spam
        /// </summary>
        public static int CheckCount { get => checkCount; set => checkCount = value; }

        /// <summary>
        /// Sender ID
        /// </summary>
        public int Owner { get; }

        public static event SpamDetected<byte> OnEventsSpamDetected = (sender, data) => { };

        /// <summary>
        ///
        /// </summary>
        /// <param name="ID">Owner ID</param>
        public EventsCounter(int ID)
        {
            Owner = ID;
            AntisThread.AddExecutable(this);
        }

        ~EventsCounter()
        {
            AntisThread.RemoveExecutable(this);
        }

        void ICounter.Count(object obj)
        {
            if (obj is byte value)
            {
                Count(value);
            }
        }

        public void Count(byte eventCode)
        {
            lock (events)
            {
                events[eventCode]++;
                if (!receivedEvents.Contains(eventCode))
                {
                    receivedEvents.Add(eventCode);
                }
            }
        }

        protected override void OnCheck()
        {
            lock (events)
            {
                for (int i = 0; i < receivedEvents.Count; i++)
                {
                    int eventCode = receivedEvents[i];
                    if (events[eventCode] > checkCount)
                    {
                        OnEventsSpamDetected(this, new SpamDetectedArgs<byte>((byte)eventCode, events[eventCode], Owner));
                    }
                    events[eventCode] = 0;
                }
                receivedEvents.Clear();
            }
        }
    }
}