using System;

namespace Antis.Collections
{
    public static class Syncer
    {
        public static void Lock(ISyncObject subject, Action action)
        {
            subject.Lock();
            try
            {
                action();
            }
            finally
            {
                subject.Unlock();
            }
        }
    }
}