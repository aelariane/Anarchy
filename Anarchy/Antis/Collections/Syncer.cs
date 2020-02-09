using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Antis.Collections
{
    public static class Syncer
    {
        public static void Lock(ISyncObject subject, Action action)
        {
            subject.Lock();
            action();
            subject.Unlock();
        }
    }
}
