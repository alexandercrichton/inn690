using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Veis.Common.Math;

namespace Veis.Chat
{
    public class ChatMessage
    {
        public string Message { get; set; }
        public Vector3 Position { get; set; }
        public string SenderName { get; set; }
        public string SenderId { get; set; }
    }
}
