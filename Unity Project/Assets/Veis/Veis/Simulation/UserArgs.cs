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
        public string Role { get; set; }
        public string Name { get; set; }
        public string ID { get; set; }
        public UserActions UserAction { get; set; }
    }
}
