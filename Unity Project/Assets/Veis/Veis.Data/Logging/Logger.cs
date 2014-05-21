using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Veis.Data.Logging
{
    /*
     * This class has been set up as a way for Veis classes
     * to broadcast messages that can be optionally receieved
     * by any listener(s). It's mainly for debugging purposes.
     */
    public static class Logger
    {
        public static event EventHandler<LogEventArgs> LogMessage;

        public static void BroadcastMessage(object o, LogEventArgs e)
        {
            if (LogMessage != null)
            {
                LogMessage(o, e);
            }
        }

        public static void BroadcastMessage(object o, string message)
        {
            if (LogMessage != null)
            {
                LogMessage(o, new LogEventArgs(o, message));
            }
        }
    }
}
