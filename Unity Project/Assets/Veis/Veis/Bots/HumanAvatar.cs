using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Veis.Bots
{
    public abstract class HumanAvatar : Avatar
    {
        // Humans need a (special) way to be provided with work
        public HumanWorkEnactor WorkEnactor { get; set; }
        //public string WorkId { get; set; }
        
        public abstract void NotifyUser(String message);
    }
}
