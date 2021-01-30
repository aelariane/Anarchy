namespace Antis
{
    public interface IProtection
    {
        /// <summary>
        /// Retrurns true if data is valid. returns false if invalid.
        /// </summary>
        /// <param name="data"></param>
        bool Check(object data);
    }

    /// <summary>
    /// Extended <see cref="IProtection"/> interface
    /// </summary>
    /// <typeparam name="T">Type of data to check</typeparam>
    public interface IProtection<T> : IProtection
    {
        /// <summary>
        /// Returns true if data is valid. returns false if invalid.
        /// </summary>
        /// <param name="data"></param>
        bool Check(T data);
    }
}