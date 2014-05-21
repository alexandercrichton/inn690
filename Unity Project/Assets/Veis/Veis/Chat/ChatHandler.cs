using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Veis.Chat
{
    public abstract class ChatHandler
    {
        public abstract bool CanHandleMessage(string message);
        protected abstract string ProcessMessage(string message, string pre, string post);
        
        public string HandleMessage(string message)
        {
            return HandleMessage(message, String.Empty, String.Empty);            
        }

        public string HandleMessage(string message, string pre, string post)
        {
            return CanHandleMessage(message) ? ProcessMessage(message, pre, post) : String.Empty;
        }
    }
}
