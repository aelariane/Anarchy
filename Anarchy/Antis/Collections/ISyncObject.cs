namespace Antis.Collections
{
    public interface ISyncObject
    {
        bool Locked { get; }

        void Lock();

        void Unlock();
    }
}