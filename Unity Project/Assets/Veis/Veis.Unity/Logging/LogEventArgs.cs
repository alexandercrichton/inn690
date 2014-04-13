using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Veis.Unity.Logging
{
    public class LogEventArgs : EventArgs
    {
        public string Message { get; protected set; }

        public LogEventArgs(string message)
        {
            this.Message = message;
        }
    }
}
