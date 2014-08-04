using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Veis.Bots;

namespace Veis.Simulation.AvatarManagement
{
    /// <summary>
    /// This class tracks the avatars that are currently operating in this simulation.
    /// It has functionality for adding bots and humans, replacing bots with humans,
    /// replacing humans with bots, and removing bots and humans altogether
    /// </summary>
    public class AvatarManager
    {
        public List<BotAvatar> Bots { get; private set; }

        public List<HumanAvatar> Humans { get; private set; }

        public AvatarManager()
        {
            Bots = new List<BotAvatar>();
            Humans = new List<HumanAvatar>();
        }

        public void AddHuman(HumanAvatar human)
        {

        }

        public void Clear()
        {
            Bots.Clear();
            Humans.Clear();
        }
    }
}
