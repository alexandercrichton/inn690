using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Veis.Simulation
{
    public enum UserActions
    {
        Register,
        Login,
        Logout
    }

    public class UserArgs
    {
        public string RoleName { get; set; }
        public string UserName { get; set; }
        public string UserId { get; set; }
        public UserActions UserAction { get; set; }
    }
}
