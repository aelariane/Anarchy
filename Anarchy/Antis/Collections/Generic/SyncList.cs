using System;
using System.Collections;
using System.Collections.Generic;

namespace Antis.Collections.Generic
{
    public sealed class SyncList<T> : ISyncObject, IList<T>
    {
        private Queue<Action> actionQueue = new Queue<Action>();
        private static readonly T[] Empty = new T[0];
        private object syncer = new object();

        private List<T> items;

        public T this[int index]
        {
            get
            {
                return items[index];
            }
            set
            {
                items[index] = value;
            }
        }

        public int Count => items.Count;

        public bool IsReadOnly => false;

        public bool Locked { get; private set; } = false;

        public SyncList()
        {
            items = new List<T>(Empty);
        }

        public SyncList(int capacity)
        {
            items = new List<T>(capacity);
        }

        public SyncList(IEnumerable<T> collection)
        {
            items = new List<T>(collection);
        }

        public void Add(T item)
        {
            QueueAction(() => items.Add(item));
        }

        public void Clear()
        {
            QueueAction(() => items.Clear());
        }

        public bool Contains(T item)
        {
            return items.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            lock (items)
            {
                items.CopyTo(array, arrayIndex);
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return items.GetEnumerator();
        }

        public int IndexOf(T item)
        {
            lock (syncer)
            {
                return items.IndexOf(item);
            }
        }

        public void Insert(int index, T item)
        {
            QueueAction(() => items.Insert(index, item));
        }

        public void Lock()
        {
            lock (syncer)
            {
                Locked = true;
            }
        }

        public void LockAndExecute(Action action)
        {
            Syncer.Lock(this, action);
        }

        private void QueueAction(Action action)
        {
            if (Locked)
            {
                lock (actionQueue)
                {
                    actionQueue.Enqueue(action);
                }
                return;
            }
            action();
        }

        public bool Remove(T item)
        {
            QueueAction(() => items.Remove(item));
            return !Locked;
        }

        public void RemoveAt(int index)
        {
            QueueAction(() => items.RemoveAt(index));
        }

        public void Unlock()
        {
            lock (syncer)
            {
                Locked = false;
                lock (actionQueue)
                {
                    while (actionQueue.Count > 0)
                    {
                        actionQueue.Dequeue().Invoke();
                    }
                }
            }
        }
    }
}