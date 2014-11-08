using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Veis.Bots;
using Veis.Common;
using Veis.Workflow;

namespace Veis.Bots.AvatarManagement
{
    /// <summary>
    /// This class tracks the avatars that are currently operating in this simulation.
    /// It has functionality for adding bots and humans, replacing bots with humans,
    /// replacing humans with bots, and removing bots and humans altogether
    /// </summary>
    public class AvatarManager
    {
        public ThreadSafeList<BotAvatar> Bots { get; private set; }

        // There is the possibility at some stage of having multiple human
        // users, hence the list
        public ThreadSafeList<HumanAvatar> Humans { get; private set; }

        protected Dictionary<HumanAvatar, BotAvatar> possessedBots = new Dictionary<HumanAvatar,BotAvatar>();

        public AvatarManager()
        {
            Bots = new ThreadSafeList<BotAvatar>();
            Humans = new ThreadSafeList<HumanAvatar>();
        }

        public void Clear()
        {
            Bots.Clear();
            Humans.Clear();
            possessedBots.Clear();
        }

        public void PossessBot(HumanAvatar human, BotAvatar bot) 
        {
            Veis.Unity.Logging.UnityLogger.BroadcastMesage(this, "PossessBot human: " + human.WorkEnactor.WorkAgent.AgentID);
            Veis.Unity.Logging.UnityLogger.BroadcastMesage(this, "bot: " + bot.WorkEnactor.WorkAgent.AgentID);
            RelinquishAnyBots(human);
            possessedBots.Add(human, bot);
            swapWorkEnactors(bot.WorkEnactor, human.WorkEnactor);
        }

        public void RelinquishAnyBots(HumanAvatar human)
        {
            foreach (HumanAvatar key in possessedBots.Keys)
            {
                if (key == human)
                {
                    swapWorkEnactors(key.WorkEnactor, possessedBots[key].WorkEnactor);
                }
            }
            while (possessedBots.ContainsKey(human))
            {
                possessedBots.Remove(human);
            }
        }

        protected void swapWorkEnactors(WorkEnactor from, WorkEnactor to)
        {
            // Exchange work agent IDs so the To work enactor receives future communications with YAWL
            to.WorkAgent.AgentID = from.WorkAgent.AgentID;

            // Set the From work agent to be ignored by YAWL
            from.WorkAgent.AgentID = WorkAgent.WORKFLOW_IGNORE_ID;

            to.ClearAll();
            foreach (var workItem in from.WorkAgent.started)
            {
                to.AddWorkItem(workItem);
            }
            from.ClearAll();
        }
    }
}
