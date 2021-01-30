using System.Collections;

namespace Antis
{
    public static class Extensions
    {
        public static bool CheckKey<T>(this IDictionary dict, object key, out T value, bool remove = false)
        {
            if (dict.Contains(key))
            {
                if (dict[key] is T tValue)
                {
                    value = tValue;
                    return true;
                }
                if (remove)
                {
                    dict.Remove(key);
                }
            }
            value = default;
            return false;
        }

        public static bool CheckType<T>(this object obj, out T value)
        {
            if (obj is T tValue)
            {
                value = tValue;
                return true;
            }
            value = default;
            return false;
        }
    }
}