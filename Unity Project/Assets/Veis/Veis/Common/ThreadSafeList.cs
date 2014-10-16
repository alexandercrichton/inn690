using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Veis.Data.Logging;

namespace Veis.Common
{
    public class ThreadSafeList<T> : IList<T>
    {
        protected readonly object _lock = new object();
        protected List<T> _innerList;

        #region Constructors

        public ThreadSafeList()
        {
            lock (_lock)
            {
                _innerList = new List<T>();
            }
        }

        public ThreadSafeList(IEnumerable<T> collection)
        {
            lock (_lock)
            {
                _innerList = new List<T>(collection);
            }
        }

        #endregion

        #region Add/Insert/Remove/Clear

        public T this[int index]
        {
            get
            {
                lock (_lock)
                {
                    return _innerList[index];
                }
            }
            set
            {
                lock (_lock)
                {
                    _innerList[index] = value;
                }
            }
        }

        public void Add(T item)
        {
            Logger.BroadcastMessage(this, "Add");
            lock (_lock)
            {
                _innerList.Add(item);
            }
        }

        public void Insert(int index, T item)
        {
            lock (_lock)
            {
                _innerList.Insert(index, item);
            }
        }

        public bool Remove(T item)
        {
            lock (_lock)
            {
                return _innerList.Remove(item);
            }
        }

        public void RemoveAt(int index)
        {
            lock (_lock)
            {
                _innerList.RemoveAt(index);
            }
        }

        public void Clear()
        {
            lock (_lock)
            {
                _innerList.Clear();
            }
        }

        #endregion

        #region Helpers

        public int IndexOf(T item)
        {
            lock (_lock)
            {
                return _innerList.IndexOf(item);
            }
        }

        public bool Contains(T item)
        {
            lock (_lock)
            {
                return _innerList.Contains(item);
            }
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            lock (_lock)
            {
                _innerList.CopyTo(array, arrayIndex);
            }
        }

        public int Count
        {
            get
            {
                lock (_lock)
                {
                    return _innerList.Count;
                }
            }
        }

        public bool IsReadOnly
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        #endregion

        #region Enumerator Functions

        public IEnumerator<T> GetEnumerator()
        {
            return new ThreadSafeEnumerator<T>(_innerList.GetEnumerator(), _lock);
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return new ThreadSafeEnumerator<T>(_innerList.GetEnumerator(), _lock);
        }

        #endregion

        #region LINQ Substitutes

        public void ForEach(Action<T> action)
        {
            lock (_lock)
            {
                _innerList.ForEach(action);
            }
        }

        #endregion
    }
}
