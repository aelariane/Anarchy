namespace Antis.Protections
{
    /// <summary>
    /// Empty implementation of <see cref="IProtection{T}"/> and <see cref="IProtection"/>. Check always returns true.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class EmptyProtection<T> : IProtection<T>, IProtection
    {
        bool IProtection.Check(object data)
        {
            return true;
        }

        public bool Check(T data)
        {
            return true;
        }
    }
}