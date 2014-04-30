using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Veis.Data.Logging;
using UnityEngine;

namespace Veis.Unity.Logging
{
    public static class UnityLogger
    {
        public static event EventHandler<LogEventArgs> LogMessage;

        static UnityLogger()
        {
            Logger.LogMessage += OnLogMessage;
        }

        public static void OnLogMessage(object o, LogEventArgs e)
        {
            if (LogMessage != null)
            {
                LogMessage(o, e);
            }
        }

        public static void BroadcastMesage(object o, string message)
        {
            if (LogMessage != null)
            {
                LogMessage(o, new LogEventArgs(o, message));
            }
        }
    }
}
