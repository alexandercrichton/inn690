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
        public List<BotAvatar> AllBots { get; private set; }

        public List<HumanAvatar> AllHumans { get; private set; }

        public AvatarManager()
        {
            AllBots = new List<BotAvatar>();
            AllHumans = new List<HumanAvatar>();
        }

        public bool AddHuman(bool replaceBot)
        {
            throw new NotImplementedException();
        }

        public bool AddBot(bool replaceHuman)
        {
            throw new NotImplementedException();
        }

        public bool ReplaceHuman()
        {
            throw new NotImplementedException();
        }

        public bool ReplaceBot()
        {
            throw new NotImplementedException();
        }

        public bool RemoveHuman(bool replaceWithBot)
        {
            throw new NotImplementedException();
        }

        public bool RemoveBot(bool replaceWithHuman)
        {
            throw new NotImplementedException();
        }

    }
}
