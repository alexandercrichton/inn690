using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Veis.Common
{
    public class ThreadSafeEnumerator<T> : IEnumerator<T>
    {
        // this is the (thread-unsafe)
        // enumerator of the underlying collection
        private readonly IEnumerator<T> _innerIEnumerator;
        // this is the object we shall lock on. 
        private readonly object _lock;

        public ThreadSafeEnumerator(IEnumerator<T> inner, object _lock)
        {
            _innerIEnumerator = inner;
            this._lock = _lock;
            // entering lock in constructor
            Monitor.Enter(_lock);
        }

        #region Implementation of IDisposable

        public void Dispose()
        {
            // .. and exiting lock on Dispose()
            // This will be called when foreach loop finishes
            Monitor.Exit(_lock);
        }

        #endregion

        #region Implementation of IEnumerator

        // we just delegate actual implementation
        // to the inner enumerator, that actually iterates
        // over some collection

        public bool MoveNext()
        {
            return _innerIEnumerator.MoveNext();
        }

        public void Reset()
        {
            _innerIEnumerator.Reset();
        }

        public T Current
        {
            get { return _innerIEnumerator.Current; }
        }

        object IEnumerator.Current
        {
            get { return Current; }
        }

        #endregion
    }
}
