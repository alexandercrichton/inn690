using System;
using System.Collections.Generic;
using Veis.Planning.Knowledge;
using Veis.Planning.Resourcing;

namespace Veis.Planning.HTN
{
    public class ComplexHTNPlanner : HTNPlanner
    {
        public const string FirstConstraint = "constraints_1";
        
        public HTNState InitialState { get; set; }
        public HTNState TargetState { get; set; }
        public HTNTaskNetwork TaskNetwork { get; set; }

        public List<HTNState> StateConstraints { get; set; }
        public List<HTNTaskSet> TaskSets { get; set; }

        public string WorkitemID { get; set; }
        public SpecificationKnowledge SpecificationKnowledge { get; set; }

        public ResourceProfileSet ResourceProfileSet { get; set; }

        public Dictionary<string, string> AllocationDictionary { get; set; }

        public ComplexHTNPlanner()
        {
            InitialState = new HTNState();
            TargetState = new HTNState();
            TaskNetwork = new HTNTaskNetwork();

            StateConstraints = new List<HTNState>();
            TaskSets = new List<HTNTaskSet>();
            AllocationDictionary = new Dictionary<string, string>();
            ResourceProfileSet = new ResourceProfileSet();
        }
        
        public ComplexHTNPlanner(SpecificationKnowledge specificationKnowledge) : this()
        {
            SpecificationKnowledge = specificationKnowledge;
        }      

        public override void SetPlannerProperties(
            HTNState initialHTNState,
            HTNState targetHTNState,
            string workitemID,
            SpecificationKnowledge specificationKnowledge,
            ResourceProfileSet resourceProfileSet)
        {
            ResourceProfileSet = resourceProfileSet;
            SpecificationKnowledge = specificationKnowledge;

            StateConstraints.Clear();
            StateConstraints.Add(initialHTNState);
            StateConstraints.Add(targetHTNState);

            InitialState.StateAtoms.Clear();
            InitialState = new HTNState(initialHTNState);
            TargetState = new HTNState(targetHTNState);

            WorkitemID = workitemID;
        }


        private List<HTNTaskSet> RefineTaskSet(HTNTaskSet htnTaskSet)
        {
            ResourceProfileSet.RefreshTheCandidateList();
            List<HTNTaskSet> candidateTaskSetList = new List<HTNTaskSet>();
            List<string> candidateTaskIDList = new List<string>();

            foreach (string taskID in htnTaskSet.TaskIDList)
            {
                candidateTaskIDList.Add(taskID);
            }

            bool doAllocation = true;
            while (doAllocation)
            {
                HTNTaskSet taskSet = new HTNTaskSet();

                for (int i = 0; i < candidateTaskIDList.Count; i++)
                {
                    string taskID = candidateTaskIDList[i];
                    string candidateExecutorName = ResourceProfileSet.FindAnExecutor(taskID);

                    if (!candidateExecutorName.Contains("no_cand"))
                    {
                        taskSet.AddTaskID(taskID);
                        candidateTaskIDList.RemoveAt(i);
                        i = 0;
                    }
                    else
                    {
                        candidateTaskSetList.Add(taskSet);
                    }
                }

                doAllocation = candidateTaskIDList.Count > 0;
            }

            return candidateTaskSetList;
        } 

        private bool AreAllTasksPrimitive(IEnumerable<HTNTaskSet> taskSets)
        {
            foreach (HTNTaskSet htnTaskSet in taskSets)
            {
                foreach (string taskID in htnTaskSet.TaskIDList)
                {
                    if (!SpecificationKnowledge.IsTaskAPrimitive(taskID))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public override List<HTNTaskSet> GeneratePlan()
        {
            //  Select a task network for the workitem
            HTNMethod htnMethod = SelectAMethodForThisWorkitem(WorkitemID, InitialState);
            if (htnMethod == null) {
                Console.WriteLine("Method undefined. Unable to generate a plan.");
                return null;
            }
                
            string taskNetworkID = htnMethod.TaskNetworkID;

            HTNTaskNetwork htnTaskNetwork = SpecificationKnowledge.GetTaskNetwork(taskNetworkID);
            HTNTaskNetworkConstraints constraint = htnTaskNetwork.Constraints[FirstConstraint];

            TaskSets = constraint.HTNTaskConstraints;
            for (int i = 1; i < constraint.StateConstraints.Count - 1; i++)
            {
                StateConstraints.Insert(i, constraint.StateConstraints[i]);
            }

           
            // Perform Decomposition
            bool allPrimitive = false;
            bool allScheduled = false;

            while (!allPrimitive && !allScheduled)
            {
                // extend the current task network, assign some tasks to resources, generate more states
                List<HTNTaskSet> newHTNTaskSetList = new List<HTNTaskSet>();

                for (int i = 0; i < TaskSets.Count; i++)
                {
                    HTNTaskSet htnTaskSet = TaskSets[i];
                    List<HTNTaskSet> newHTNSetFragment = htnTaskSet.
                        AssignAndRefineTaskSet(
                            ResourceProfileSet,
                            StateConstraints[i],
                            StateConstraints[i + 1],
                            SpecificationKnowledge
                            );

                    foreach (HTNTaskSet newHTNTaskSet in newHTNSetFragment)
                    {
                        newHTNTaskSetList.Add(newHTNTaskSet);
                    }
                }  
                TaskSets = newHTNTaskSetList;

                List<HTNState> newHTNStateList = new List<HTNState>();
                newHTNStateList.Add(InitialState);
                newHTNStateList.Add(TargetState);
                HTNState localHTNState = new HTNState(InitialState);

                for (int i = 0; i < TaskSets.Count - 1; i++)
                {
                    HTNState deducedState = localHTNState.DeduceNextStateBasedOnEffects(
                        TaskSets[i].GetAllPostEffects(SpecificationKnowledge));
                    localHTNState = new HTNState(deducedState);
                    newHTNStateList.Insert(i + 1, deducedState);
                }

                StateConstraints = newHTNStateList;

                // Attempt to decompose
                List<HTNTaskSet> newlyDecomposedHTNTaskSetList = new List<HTNTaskSet>();

                for (int i = 0; i < TaskSets.Count; i++)
                {
                    HTNTaskSet htnTaskSet = TaskSets[i];

                    List<HTNTaskSet> partialDecomposedHTNTaskSetList
                        =
                        htnTaskSet.DecomposeTaskSet(
                            ResourceProfileSet,
                            SpecificationKnowledge,
                            this,
                            StateConstraints[i],
                            StateConstraints[i + 1]
                        );

                    foreach (HTNTaskSet taskSet in partialDecomposedHTNTaskSetList)
                    {
                        newlyDecomposedHTNTaskSetList.Add(taskSet);
                    }
                }

                allScheduled = true;

                TaskSets = newlyDecomposedHTNTaskSetList;

                // TODO: Hope this doesn't break. Dammit..
                allPrimitive = AreAllTasksPrimitive(TaskSets);
                
                //foreach (HTNTaskSet htnTaskSet in TaskSets)
                //{
                //    foreach (string taskID in htnTaskSet.TaskIDList)
                //    {
                //        if (!SpecificationKnowledge.IsTaskAPrimitive(taskID))
                //        {
                //            allPrimitive = false;
                //        }
                //    }

                //    if (!allPrimitive)
                //    {
                //        break;
                //    }
                //}

                if (!allPrimitive && allScheduled)
                {
                    newHTNStateList.Clear();
                    newHTNStateList.Add(InitialState);
                    newHTNStateList.Add(TargetState);
                    localHTNState = new HTNState(InitialState);

                    for (int i = 0; i < TaskSets.Count - 1; i++)
                    {
                        HTNState deduced_state = localHTNState.DeduceNextStateBasedOnEffects(TaskSets[i].GetAllPostEffects(SpecificationKnowledge));
                        localHTNState = new HTNState(deduced_state);
                        newHTNStateList.Insert(i + 1, deduced_state);
                    }

                    this.StateConstraints = newHTNStateList;
                    allPrimitive = false;
                }

                allScheduled = true;

                foreach (HTNTaskSet htnTaskSet in TaskSets)
                {
                    if (!htnTaskSet.IsScheduled())
                    {
                        allScheduled = false;
                    }
                }
            }  


            int stateSize = StateConstraints.Count;
            for (int i = 0; i < StateConstraints.Count; i++)
            {
                HTNState htnState = new HTNState(StateConstraints[i]);

                foreach (KeyValuePair<string, string> kvp1 in htnState.States)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write("state_name:   " + kvp1.Key);
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("         state_value:  " + kvp1.Value);
                }

                if (i < stateSize - 1)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;

                    HTNTaskSet htnTaskSet = new HTNTaskSet(TaskSets[i]);

                    foreach (KeyValuePair<string, string> kvp2 in htnTaskSet.TaskExecutors)
                    {
                        Console.WriteLine("task_id:  " + kvp2.Key + "       " + "executor:  " + kvp2.Value);
                    }
                }
            }

            return TaskSets;
        }


        public HTNMethod FindASuitableMethodOrOperator(string taskID, HTNState currentState)
        {
            Dictionary<string, HTNMethod> methodDictionary = SpecificationKnowledge.GetMethodDictionary();
            List<string> candidateMethodList = new List<string>();

            foreach (KeyValuePair<string, HTNMethod> pvMethod in methodDictionary)
            {
                if (pvMethod.Value.SuitsAMethod(taskID, currentState) && pvMethod.Value.CurrentStateSatisifyMethodPrecondition(currentState))
                    candidateMethodList.Add(pvMethod.Key);
            }

            if (candidateMethodList.Count == 0)
                return null;
            return SpecificationKnowledge.GetMethodDictionary()[candidateMethodList[0]];
        }


        private HTNMethod SelectAMethodForThisWorkitem(string key, HTNState currentState)
        {
            List<HTNMethod> suitableMethod = new List<HTNMethod>();

            foreach (KeyValuePair<string, HTNMethod> pv in SpecificationKnowledge.GetMethodDictionary())
            {
                HTNMethod method = pv.Value;
                if (method.SuitsAMethod(key, currentState))
                    suitableMethod.Add(method);
            }

            int count = suitableMethod.Count;

            HTNMethod selectMethod = null;

            if (count >= 1)
            {
                Random randObj = new Random();
                int select = randObj.Next(0, count);
                selectMethod = suitableMethod[select];
            }

            return selectMethod;
        }
    }
}
