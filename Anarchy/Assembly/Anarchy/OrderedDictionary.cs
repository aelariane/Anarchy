using System;
using System.Collections;
using System.Collections.Generic;

namespace Anarchy
{
    public class OrderedDictionary<TKey, TValue> : IDictionary<TKey, TValue>, ICollection<KeyValuePair<TKey, TValue>>, IEnumerable<KeyValuePair<TKey, TValue>>
    {
        private Dictionary<TKey, LinkedListNode<KeyValuePair<TKey, TValue>>> mDictionary;
        private LinkedList<KeyValuePair<TKey, TValue>> mLinkedList;

        private ValueCollection valueCollection;
        private KeyCollection keyCollection;

        #region Constructors

        public OrderedDictionary()
        {
            mDictionary = new Dictionary<TKey, LinkedListNode<KeyValuePair<TKey, TValue>>>();
            mLinkedList = new LinkedList<KeyValuePair<TKey, TValue>>();
            valueCollection = new ValueCollection(this);
            keyCollection = new KeyCollection(this);
        }

        public OrderedDictionary(int capacity)
        {
            mDictionary = new Dictionary<TKey, LinkedListNode<KeyValuePair<TKey, TValue>>>(capacity);
            mLinkedList = new LinkedList<KeyValuePair<TKey, TValue>>();
            valueCollection = new ValueCollection(this);
            keyCollection = new KeyCollection(this);
        }

        public OrderedDictionary(IEqualityComparer<TKey> comparer)
        {
            mDictionary = new Dictionary<TKey, LinkedListNode<KeyValuePair<TKey, TValue>>>(comparer);
            mLinkedList = new LinkedList<KeyValuePair<TKey, TValue>>();
            valueCollection = new ValueCollection(this);
            keyCollection = new KeyCollection(this);
        }

        #endregion Constructors

        public void Add(TKey key, TValue value)
        {
            LinkedListNode<KeyValuePair<TKey, TValue>> lln = new LinkedListNode<KeyValuePair<TKey, TValue>>(new KeyValuePair<TKey, TValue>(key, value));
            mDictionary.Add(key, lln);
            mLinkedList.AddLast(lln);
        }

        #region IDictionary Generic

        public bool ContainsKey(TKey key)
        {
            return mDictionary.ContainsKey(key);
        }

        public ICollection<TKey> Keys
        {
            get { return keyCollection; }
        }

        public bool Remove(TKey key)
        {
            LinkedListNode<KeyValuePair<TKey, TValue>> lln;
            bool found = mDictionary.TryGetValue(key, out lln);
            if (!found) { return false; }
            mDictionary.Remove(key);
            mLinkedList.Remove(lln);
            return true;
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            LinkedListNode<KeyValuePair<TKey, TValue>> lln;
            bool found = mDictionary.TryGetValue(key, out lln);
            if (!found)
            {
                value = default(TValue);
                return false;
            }
            value = lln.Value.Value;
            return true;
        }

        public ICollection<TValue> Values
        {
            get { return valueCollection; }
        }

        public TValue this[TKey key]
        {
            get
            {
                return mDictionary[key].Value.Value;
            }
            set
            {
                LinkedListNode<KeyValuePair<TKey, TValue>> lln;
                if (mDictionary.ContainsKey(key))
                {
                    lln = mDictionary[key];
                    lln.Value = new KeyValuePair<TKey, TValue>(key, value);
                }
                else
                {
                    lln = new LinkedListNode<KeyValuePair<TKey, TValue>>(new KeyValuePair<TKey, TValue>(key, value));
                    mDictionary.Add(key, lln);
                }
            }
        }

        public void Clear()
        {
            mDictionary.Clear();
            mLinkedList.Clear();
        }

        public int Count
        {
            get { return mLinkedList.Count; }
        }

        public bool IsReadOnly
        {
            get { throw new NotImplementedException(); }
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return mLinkedList.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion IDictionary Generic

        #region Explicit ICollection Generic

        void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
        {
            LinkedListNode<KeyValuePair<TKey, TValue>> lln = new LinkedListNode<KeyValuePair<TKey, TValue>>(item);
            mDictionary.Add(item.Key, lln);
            mLinkedList.AddLast(lln);
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item)
        {
            return mDictionary.ContainsKey(item.Key);
        }

        void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            mLinkedList.CopyTo(array, arrayIndex);
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly
        {
            get { return false; }
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
        {
            return Remove(item.Key);
        }

        #endregion Explicit ICollection Generic

        #region Explicit IEnumerable Generic

        IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion Explicit IEnumerable Generic

        public sealed class KeyCollection : ICollection<TKey>, IEnumerable<TKey>
        {
            private OrderedDictionary<TKey, TValue> parent;

            internal KeyCollection(OrderedDictionary<TKey, TValue> parent)
            {
                this.parent = parent;
            }

            public int Count
            {
                get { return parent.mLinkedList.Count; }
            }

            public void CopyTo(TKey[] array, int arrayIndex)
            {
                if (array == null) { throw new ArgumentNullException("array"); }
                if (arrayIndex < 0) { throw new ArgumentOutOfRangeException("arrayIndex"); }
                if (array.Rank > 1) { throw new ArgumentException("array", "array must only have one dimension"); }
                if ((array.Length - arrayIndex) > parent.mLinkedList.Count) { throw new ArgumentException("array", "array does not have enough space starting at arrayIndex"); }

                int i = arrayIndex;
                foreach (KeyValuePair<TKey, TValue> entry in parent.mLinkedList)
                {
                    array[i] = entry.Key;
                    i++;
                }
            }

            public IEnumerator<TKey> GetEnumerator()
            {
                return new Enumerator(this);
            }

            #region Explicit ICollection Generic

            void ICollection<TKey>.Add(TKey item)
            {
                throw new NotSupportedException();
            }

            void ICollection<TKey>.Clear()
            {
                throw new NotSupportedException();
            }

            bool ICollection<TKey>.Contains(TKey item)
            {
                foreach (KeyValuePair<TKey, TValue> entry in parent.mLinkedList)
                {
                    if (entry.Key.Equals(item))
                    {
                        return true;
                    }
                }

                return false;
            }

            bool ICollection<TKey>.IsReadOnly
            {
                get { return true; }
            }

            bool ICollection<TKey>.Remove(TKey item)
            {
                throw new NotSupportedException();
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return new Enumerator(this);
            }

            #endregion Explicit ICollection Generic

            public struct Enumerator : IEnumerator<TKey>, IDisposable
            {
                private LinkedListNode<KeyValuePair<TKey, TValue>> current;

                private KeyCollection parent;

                internal Enumerator(KeyCollection parent)
                {
                    this.parent = parent;
                    current = null;
                }

                public void Dispose()
                {
                    parent = null;
                    current = null;
                }

                public bool MoveNext()
                {
                    if (current == null)
                    {
                        if (parent.parent.mLinkedList.Count > 0)
                        {
                            current = parent.parent.mLinkedList.First;
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        if (current.Next != null)
                        {
                            current = current.Next;
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }

                public TKey Current
                {
                    get { return current.Value.Key; }
                }

                #region Explicit IEnumerator generic

                object IEnumerator.Current
                {
                    get { return Current; }
                }

                void IEnumerator.Reset()
                {
                    current = null;
                }

                #endregion Explicit IEnumerator generic
            }
        }

        public sealed class ValueCollection : ICollection<TValue>, IEnumerable<TValue>
        {
            private OrderedDictionary<TKey, TValue> parent;

            internal ValueCollection(OrderedDictionary<TKey, TValue> parent)
            {
                this.parent = parent;
            }

            public int Count
            {
                get { return parent.mLinkedList.Count; }
            }

            public void CopyTo(TValue[] array, int arrayIndex)
            {
                if (array == null) { throw new ArgumentNullException("array"); }
                if (arrayIndex < 0) { throw new ArgumentOutOfRangeException("arrayIndex"); }
                if (array.Rank > 1) { throw new ArgumentException("array", "array must only have one dimension"); }
                if ((array.Length - arrayIndex) > parent.mLinkedList.Count) { throw new ArgumentException("array", "array does not have enough space starting at arrayIndex"); }

                int i = arrayIndex;
                foreach (KeyValuePair<TKey, TValue> entry in parent.mLinkedList)
                {
                    array[i] = entry.Value;
                    i++;
                }
            }

            public IEnumerator<TValue> GetEnumerator()
            {
                return new Enumerator(this);
            }

            #region Explicit ICollection Generic

            void ICollection<TValue>.Add(TValue item)
            {
                throw new NotSupportedException();
            }

            void ICollection<TValue>.Clear()
            {
                throw new NotSupportedException();
            }

            bool ICollection<TValue>.Contains(TValue item)
            {
                foreach (KeyValuePair<TKey, TValue> entry in parent.mLinkedList)
                {
                    if (entry.Value.Equals(item))
                    {
                        return true;
                    }
                }

                return false;
            }

            bool ICollection<TValue>.IsReadOnly
            {
                get { return true; }
            }

            bool ICollection<TValue>.Remove(TValue item)
            {
                throw new NotSupportedException();
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return new Enumerator(this);
            }

            #endregion Explicit ICollection Generic

            public struct Enumerator : IEnumerator<TValue>, IDisposable
            {
                private LinkedListNode<KeyValuePair<TKey, TValue>> current;

                private ValueCollection parent;

                internal Enumerator(ValueCollection parent)
                {
                    this.parent = parent;
                    current = null;
                }

                public void Dispose()
                {
                    parent = null;
                    current = null;
                }

                public bool MoveNext()
                {
                    if (current == null)
                    {
                        if (parent.parent.mLinkedList.Count > 0)
                        {
                            current = parent.parent.mLinkedList.First;
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        if (current.Next != null)
                        {
                            current = current.Next;
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }

                public TValue Current
                {
                    get { return current.Value.Value; }
                }

                #region Explicit IEnumerator generic

                object IEnumerator.Current
                {
                    get { return Current; }
                }

                void IEnumerator.Reset()
                {
                    current = null;
                }

                #endregion Explicit IEnumerator generic
            }
        }
    }
}