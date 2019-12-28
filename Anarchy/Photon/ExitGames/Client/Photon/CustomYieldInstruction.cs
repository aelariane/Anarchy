using System.Collections;

namespace UnityEngine
{
    public abstract class CustomYieldInstruction : IEnumerator
    {
        public abstract bool keepWaiting { get; }

        public object Current
        {
            get
            {
                return (object)null;
            }
        }

        public bool MoveNext()
        {
            return this.keepWaiting;
        }

        public void Reset()
        {
        }
    }
}