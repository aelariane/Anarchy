namespace Antis.Spam
{
    /// <summary>
    /// Empty implementation of <see cref="ICounter{T}"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class EmptyCounter<T> : ICounter<T>
    {
        public int Owner { get; }

        public EmptyCounter(int sender)
        {
            Owner = sender;
        }

        void ICounter.Count(object value)
        {
        }

        public void Count(T value)
        {
        }
    }
}