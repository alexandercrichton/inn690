using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Veis.Bots
{
    public abstract class HumanAvatar : Avatar
    {
        public string Name { get; set; } // Human user has a (real) name
        public string ActingName { get; set; } // If this human user is taking the role of another person

        // Humans need a (special) way to be provided with work
        public HumanWorkProvider WorkProvider { get; set; }
        public string WorkId { get; set; }
        
        public abstract void NotifyUser(String message);
    }
}
