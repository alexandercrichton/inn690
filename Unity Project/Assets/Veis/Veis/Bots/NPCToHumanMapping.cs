using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Veis.Simulation.WorldState;
using Veis.Planning;
using Veis.Workflow;

namespace Veis.Bots
{
    public class NPCToHumanMapping
    {
        public static HumanWorkEnactor MapWorkProviderFromNPC(BotAvatar npc, HumanAvatar human, 
            GoalService goalService, IDecompositionService<WorkItem> decompService)
        {   
            HumanWorkEnactor worker = new HumanWorkEnactor
                (human, npc.WorkEnactor.GetWorkAgent(), npc.WorkEnactor.GetWorkflowProvider(),
                decompService, goalService);

            // Any currently processing tasks, remove
            npc.WorkEnactor.GetWorkAgent().processing.Clear();
            Veis.Data.Logging.Logger.BroadcastMessage(new object(), "here");
            // Now add all the tasks that had been started to the worker
            //foreach (var workItem in npc.WorkProvider.GetWorkAgent().started)
            //{
            //    worker.AddWork(workItem);
            //}
            List<WorkItem> workItems = new List<WorkItem>(npc.WorkEnactor.GetWorkAgent().started);
            for (int i = 0; i < workItems.Count; i++)
            {
                worker.AddWorkItem(workItems[i]);
            }
                Veis.Data.Logging.Logger.BroadcastMessage(new object(), "here2");

            return worker;
        }
    }
}
