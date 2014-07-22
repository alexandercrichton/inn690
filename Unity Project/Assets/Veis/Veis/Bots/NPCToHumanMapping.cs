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
        public static HumanWorkProvider MapWorkProviderFromNPC(BotAvatar npc, HumanAvatar human, 
            GoalService goalService, IDecompositionService<WorkItem> decompService)
        {   
            HumanWorkProvider worker = new HumanWorkProvider
                (human, npc.WorkProvider.GetWorkAgent(), npc.WorkProvider.GetWorkflowProvider(),
                decompService, goalService);

            // Any currently processing tasks, remove
            npc.WorkProvider.GetWorkAgent().processing.Clear();
            Veis.Data.Logging.Logger.BroadcastMessage(new object(), "here");
            // Now add all the tasks that had been started to the worker
            //foreach (var workItem in npc.WorkProvider.GetWorkAgent().started)
            //{
            //    worker.AddWork(workItem);
            //}
            List<WorkItem> workItems = npc.WorkProvider.GetWorkAgent().started;
            for (int i = 0; i < workItems.Count; i++)
            {
                worker.AddWork(workItems[i]);
            }
                Veis.Data.Logging.Logger.BroadcastMessage(new object(), "here2");

            return worker;
        }
    }
}
