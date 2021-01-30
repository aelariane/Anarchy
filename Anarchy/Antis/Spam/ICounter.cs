namespace Antis.Spam
{
    public interface ICounter
    {
        /// <summary>
        /// Sender ID
        /// </summary>
        int Owner { get; }

        /// <summary>
        /// Counts passed object
        /// </summary>
        /// <param name="obj">Object to count</param>
        void Count(object obj);
    }

    public interface ICounter<T> : ICounter
    {
        /// <summary>
        /// Counts passed object
        /// </summary>
        /// <param name="value">Object to count</param>
        void Count(T value);
    }
}