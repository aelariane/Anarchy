using System;
using System.Collections;
using System.Collections.Generic;

namespace Antis.Collections.Generic
{
    public sealed class SyncDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISyncObject, IDictionary<TKey, TValue>, IDictionary
    {
        private Queue<Action> actionsQueue = new Queue<Action>();
        private IDictionary<TKey, TValue> genericLink;
        private IDictionary link;
        private object syncer = new object();

        public bool Locked { get; private set; }

        object IDictionary.this[object key]
        {
            get
            {
                return genericLink[(TKey)key];
            }
            set
            {
                genericLink[(TKey)key] = (TValue)value;
            }
        }

        TValue IDictionary<TKey, TValue>.this[TKey key]
        {
            get
            {
                return base[key];
            }
            set
            {
            }
        }

        public new TValue this[TKey key]
        {
            get
            {
                return base[key];
            }
            set
            {
                QueueAction(() => base[key] = value);
            }
        }

        public SyncDictionary() : base()
        {
            genericLink = this as IDictionary<TKey, TValue>;
            link = this as IDictionary;
        }

        public SyncDictionary(IDictionary<TKey, TValue> dict) : base(dict)
        {
            genericLink = this as IDictionary<TKey, TValue>;
            link = this as IDictionary;
        }

        public SyncDictionary(int size) : base(size)
        {
            genericLink = this as IDictionary<TKey, TValue>;
            link = this as IDictionary;
        }

        void IDictionary.Add(object key, object value)
        {
            QueueAction(() => base.Add((TKey)key, (TValue)value));
        }

        void IDictionary<TKey, TValue>.Add(TKey key, TValue value)
        {
            QueueAction(() => base.Add(key, value));
        }

        public new void Add(TKey key, TValue value)
        {
            genericLink.Add(key, value);
        }

        void IDictionary.Clear()
        {
            QueueAction(() => base.Clear());
        }

        public new void Clear()
        {
            link.Clear();
        }

        public void Lock()
        {
            lock (syncer)
            {
                Locked = true;
            }
        }

        private void QueueAction(Action act)
        {
            if (Locked)
            {
                lock (actionsQueue)
                {
                    actionsQueue.Enqueue(act);
                    return;
                }
            }
            act();
        }

        void IDictionary.Remove(object key)
        {
            QueueAction(() => base.Remove((TKey)key));
        }

        bool IDictionary<TKey, TValue>.Remove(TKey key)
        {
            if (Locked)
            {
                lock (actionsQueue)
                {
                    actionsQueue.Enqueue(() => base.Remove(key));
                }
                return false;
            }
            return base.Remove(key);
        }

        public new bool Remove(TKey key)
        {
            return genericLink.Remove(key);
        }

        public void Unlock()
        {
            lock (syncer)
            {
                Locked = false;
                lock (actionsQueue)
                {
                    while (actionsQueue.Count > 0)
                    {
                        actionsQueue.Dequeue().Invoke();
                    }
                }
            }
        }
    }
}