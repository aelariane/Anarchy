using System;

namespace Antis
{
    public static class AntisManager
    {
        private static Response response = (ID, ban, reason) => { };

        /// <summary>
        /// Callback that calls in <see cref="Response(int, string)"/> and <see cref="Response(int, bool, string)"/>
        /// </summary>
        public static event Action<int, bool, string> OnResponseCallback = (ID, ban, reason) => { };

        /// <summary>
        /// Sets response. You can only replace old response with the new one.
        /// </summary>
        public static event Response ResponseAction
        {
            add
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }
                response = value;
            }
            remove
            {
                UnityEngine.Debug.Log($"You can only re-set response. Avoid using {nameof(ResponseAction)}.remove");
            }
        }

        public static void Response(int ID, string reason)
        {
            response(ID, false, reason);
            if (OnResponseCallback != null)
            {
                OnResponseCallback(ID, false, reason);
            }
        }

        public static void Response(int ID, bool banned, string reason)
        {
            response(ID, banned, reason);
            if (OnResponseCallback != null)
            {
                OnResponseCallback(ID, banned, reason);
            }
        }
    }
}