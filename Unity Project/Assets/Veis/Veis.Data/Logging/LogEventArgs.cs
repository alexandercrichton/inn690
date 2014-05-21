using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Veis.Data.Logging
{
    public class LogEventArgs : EventArgs
    {
        // EventInitiator is for keeping track of the object that
        // originally called the event. This allows LogEventArgs to be
        // passed on by other event handlers without losing track of
        // the initial object.
        public object EventInitiator { get; protected set; }

        public string Message { get; protected set; }

        // For creating a new message to be broadcast
        public LogEventArgs(object eventInitiator, string message)
        {
            this.EventInitiator = eventInitiator;
            this.Message = message;
        }

        // For passing on/re-broadcasting a message. This is useful
        // for external libraries that might want to channel broadcast messages
        // through their own LogEventHandler while preserving the original
        // message data.
        public LogEventArgs(LogEventArgs logEventArgs)
        {
            this.EventInitiator = logEventArgs.EventInitiator;
            this.Message = logEventArgs.Message;
        }
    }
}
