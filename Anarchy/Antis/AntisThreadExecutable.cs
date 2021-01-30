using Antis.Internal;

namespace Antis
{
    /// <summary>
    /// Represents an object that can be executed in AntisThread.
    /// </summary>
    public abstract class AntisThreadExecutable
    {
        internal void Check()
        {
            if (System.Threading.Thread.CurrentThread.ManagedThreadId != AntisThread.ThreadInstance.ManagedThreadId)
            {
                throw new ThreadExecutableException();
            }
            OnCheck();
        }

        /// <summary>
        /// Calls in checking Thread once per <seealso cref="AntisThreadManager.ThreadSleepTime"/>
        /// </summary>
        protected abstract void OnCheck();
    }
}