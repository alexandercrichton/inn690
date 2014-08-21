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
        //public UUID UUID { get; set; }
        //public string UserName { get; set; }
        //public string RoleName { get; set; }

        public UnityHumanAvatar(string id)
        {
            this.ID = id;
            this.Name = "NoName";
            this.Role = "NoRole";
        }

        public UnityHumanAvatar(string id, string name, string role)
        {
            //this.UUID = uuid;
            this.ID = id;
            this.Name = name;
            this.Role = role;
        }

        public override void NotifyUser(string message)
        {
            UnityLogger.BroadcastMesage(this, message);
        }
    }
}
