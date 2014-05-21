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

            // Now add all the tasks that had been started to the worker
            foreach (var workItem in npc.WorkProvider.GetWorkAgent().started)
            {
                worker.AddWork(workItem);
            }

            return worker;
        }
    }
}
