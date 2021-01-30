using System;

namespace Antis
{
    public class ThreadExecutableException : Exception
    {
        public ThreadExecutableException() : base($"Avoid calling {nameof(AntisThreadExecutable)}.OnCheck outside AntisThread")
        {
        }

        public ThreadExecutableException(string message) : base(message)
        {
        }

        public ThreadExecutableException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}