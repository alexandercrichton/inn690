using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Threading;

namespace Veis.Unity
{
    public class SynchronizeInvoke : ISynchronizeInvoke
    {
        private readonly SynchronizationContext _currentContext = SynchronizationContext.Current;

        private readonly Thread _mainThread = Thread.CurrentThread;

        private readonly object _invokeLocker = new object();

        public IAsyncResult BeginInvoke(Delegate method, object[] args)
        {
            throw new NotImplementedException();
        }

        public object EndInvoke(IAsyncResult result)
        {
            throw new NotImplementedException();
        }

        public object Invoke(Delegate method)
        {
            return Invoke(method, null);
        }

        public object Invoke(Delegate method, object[] args)
        {
            if (method == null)
            {
                throw new ArgumentNullException("method");
            }

            lock (_invokeLocker)
            {
                object objectToGet = null;

                SendOrPostCallback invoker = new SendOrPostCallback(
                delegate(object data)
                {
                    objectToGet = method.DynamicInvoke(args);
                });

                _currentContext.Send(new SendOrPostCallback(invoker), method.Target);

                return objectToGet;
            }
        }

        public bool InvokeRequired
        {
            get
            {
                return Thread.CurrentThread.ManagedThreadId != this._mainThread.ManagedThreadId;
            }
        }
    }
}
