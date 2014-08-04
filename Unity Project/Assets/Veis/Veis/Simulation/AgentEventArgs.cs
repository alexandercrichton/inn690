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

    public class AgentEventArgs
    {
        public string Role { get; set; }
        public string Name { get; set; }
        public string ID { get; set; }
        public UserActions UserAction { get; set; }
    }
}
