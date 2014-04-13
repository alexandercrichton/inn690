using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Veis.Unity.Logging
{
    public class Logger
    {
        public static event EventHandler<LogEventArgs> LogMessage;

        public static void OnLogMessage(Object o, LogEventArgs e)
        {
            if (LogMessage != null)
            {
                LogMessage(o, e);
            }
        }
    }
}
