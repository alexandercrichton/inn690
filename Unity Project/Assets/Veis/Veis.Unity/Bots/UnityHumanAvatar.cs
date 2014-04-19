using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Veis.Bots;

namespace Veis.Unity.Bots
{
    public class UnityHumanAvatar : HumanAvatar
    {
        public UUID UUID { get; set; }
        public string UserName { get; set; }
        public string RoleName { get; set; }

        public UnityHumanAvatar(UUID userId, string userName, string roleName)
        {

        }

        public override void NotifyUser(string message)
        {
            throw new NotImplementedException();
        }
    }
}
