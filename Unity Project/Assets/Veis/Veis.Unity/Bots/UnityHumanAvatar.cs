using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Veis.Bots;
using Veis.Unity.Logging;

namespace Veis.Unity.Bots
{
    public class UnityHumanAvatar : HumanAvatar
    {
        public UUID UUID { get; set; }
        public string UserName { get; set; }
        public string RoleName { get; set; }

        public UnityHumanAvatar(UUID uuid, string userName, string roleName)
        {
            this.UUID = uuid;
            this.UserName = userName;
            this.RoleName = roleName;
        }

        public override void NotifyUser(string message)
        {
            Logger.OnLogMessage(this, new LogEventArgs(message));
        }
    }
}
