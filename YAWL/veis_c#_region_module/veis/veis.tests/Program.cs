using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Veis.Chat;
using Veis.Simulation.WorldState.StateSources;
using Veis.Data.Repositories;
using Veis.Data;
using MySql.Data.MySqlClient;
using System.Data.Common;
using Veis.Data.Entities;
using Veis.Workflow;
using Veis.Planning;
using Veis.Data.Services;

namespace Veis.Tests
{
    class Program
    {
        static void Main(string[] args)
        {
            DbProviderFactory provider = new MySql.Data.MySqlClient.MySqlClientFactory();
            
            IDataAccess stateDataAccess = new WorldStateDataAccess(provider);
            IDataAccess knowledgeDataAccess = new WorldKnowledgeDataAccess(provider);

            WorldStateRepository worldState = new WorldStateRepository(stateDataAccess);
            AccessRecordRepository accessRecord = new AccessRecordRepository(stateDataAccess);
            AssetServiceRoutineRepository assetServiceRoutine = new AssetServiceRoutineRepository(stateDataAccess);
            MethodRepository method = new MethodRepository(knowledgeDataAccess);

            Console.WriteLine("Affected " + assetServiceRoutine.Delete(new AssetServiceRoutine { Id = 178 }) + " rows");
            
            assetServiceRoutine.Find(new AssetServiceRoutineRepository.ByIdSpecification(178)).ToList()
                .ForEach(x => Console.WriteLine(String.Format("{0} {1} {2} {3}", x.Id, x.Priority, x.AssetKey, x.ServiceRoutine)));
            
            PolledDatabaseStateSource polling = new PolledDatabaseStateSource(2000, worldState, accessRecord);
            //polling.StateUpdated += ShowUpdate;

            var taskVariables = new Dictionary<string, string>();
            taskVariables.Add("Goals", "Bed_1;At;Bay_10");
            WorkItem testItem = new WorkItem { taskVariables = taskVariables };

            ActivityMethodService methodService = new ActivityMethodService(method);
            SimpleWorkItemDecomposition decomp = new SimpleWorkItemDecomposition();
            GoalBasedWorkItemPlanner planner = new GoalBasedWorkItemPlanner(decomp, methodService, worldState, new TestSceneService());

            var plan = planner.MakePlan(testItem);

            foreach (var task in plan.Tasks)
            {
                Console.WriteLine(task);
            }

            Console.ReadLine();
        }

        static void ShowUpdate()
        {
            Console.WriteLine("Updated!");
        }
    }
}
