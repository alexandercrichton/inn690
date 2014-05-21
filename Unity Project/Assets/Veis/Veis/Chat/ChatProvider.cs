using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Veis.Chat
{
    public abstract class ChatProvider
    {
        protected List<ChatHandler> chatHandlers;

        protected ChatProvider()
        {
            chatHandlers = new List<ChatHandler>();
        }

        public void AddChatHandler(ChatHandler handler)
        {
            if (!chatHandlers.Contains(handler))
            {
                chatHandlers.Add(handler);
            }
        }

        public void RemoveChatHandler(ChatHandler handler)
        {
            if (chatHandlers.Contains(handler))
            {
                chatHandlers.Remove(handler);
            }
        }

        public void RemoveAllChatHandlersOfType(Type handlerType)
        {
            chatHandlers = chatHandlers.Where(c => c.GetType() != handlerType).ToList();
        }

        public abstract string ProcessChat(string message, string fromName, string fromId);
    }
}
