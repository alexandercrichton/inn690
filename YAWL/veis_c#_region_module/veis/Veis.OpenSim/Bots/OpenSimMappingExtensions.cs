using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Veis.Chat;
using OpenSim.Framework;

namespace Veis.OpenSim.Bots
{
    public static class OpenSimMappingExtensions
    {
        public static Veis.Chat.ChatMessage ToLocal(this OSChatMessage openSimChat)
        {
            return new Veis.Chat.ChatMessage
            {
                Message = openSimChat.Message,
                Position = openSimChat.Position.ToLocal(),
                SenderName = openSimChat.Sender.Name,
                SenderId = openSimChat.SenderUUID.ToString()
            };
        }

        public static Veis.Chat.ChatMessage ToLocal(this GridInstantMessage openSimIM)
        {
            return new Veis.Chat.ChatMessage
            {
                Message = openSimIM.message,
                Position = openSimIM.Position.ToLocal(),
                SenderName = openSimIM.fromAgentName,
                SenderId = openSimIM.fromAgentID.ToString()
            };
        }

        public static OpenMetaverse.Vector3 ToOpenSim(this Veis.Common.Math.Vector3 local)
        {
            return new OpenMetaverse.Vector3(local.X, local.Y, local.Z);
        }

        public static Veis.Common.Math.Vector3 ToLocal(this OpenMetaverse.Vector3 openSim)
        {
            return new Veis.Common.Math.Vector3(openSim.X, openSim.Y, openSim.Z);
        }
    }
}
