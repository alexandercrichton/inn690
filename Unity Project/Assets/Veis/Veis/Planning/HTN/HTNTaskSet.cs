using System;
using System.Collections.Generic;
using System.Linq;
using Veis.Planning.Knowledge;
using Veis.Planning.Resourcing;

namespace Veis.Planning.HTN
{
    public class HTNTaskSet
    {
        public List<string> TaskIDList { get; set; }

        public Dictionary<string, string> TaskExecutors { get; set; }

        public List<HTNTaskSet> RefinedTaskSet { get; set; }

        public List<HTNState> RefinedStateList { get; set; }

        public HTNTaskSet()
        {
            TaskIDList = new List<string>();
            TaskExecutors = new Dictionary<string, string>();
            RefinedStateList = new List<HTNState>();
            RefinedTaskSet = new List<HTNTaskSet>();
        }

        public HTNTaskSet(HTNTaskSet htnTaskSet) : this()
        {
            foreach (string taskID in htnTaskSet.TaskIDList)
            {
                TaskIDList.Add((string)taskID.Clone());
            }

            foreach (KeyValuePair<string, string> kvp in htnTaskSet.TaskExecutors)
            {
                string key = kvp.Key;
                string value = kvp.Value;
                TaskExecutors.Add(key, value);
            }
        }

        public void AddTaskID(string taskID)
        {
            TaskIDList.Add(taskID);
        }

        public List<HTNEffect> GetAllPostEffects(SpecificationKnowledge specificationKnowledge)
        {
            List<HTNEffect> postEffects = new List<HTNEffect>();

            foreach (string taskID in this.TaskIDList)
            {
                HTNTask htnTask = specificationKnowledge.GetHTNTask(taskID);
                postEffects.AddRange(htnTask.PostConditions);
            }

            return postEffects;
        }

        /// <summary>
        /// Decomposes and schedules the transition between current state and target state.
        /// </summary>
        public List<HTNTaskSet> DecomposeTaskSet(
            ResourceProfileSet resourceProfileSet,
            SpecificationKnowledge specificationKnowledge,
            ComplexHTNPlanner htnPlanner,
            HTNState currentHTNState,
            HTNState targetHTNState)
        {
            List<List<HTNTaskSet>> allNewDecomposedTaskSetList = new List<List<HTNTaskSet>>();
            HTNState currentState = currentHTNState;
            int longestSize = 1;

            // Start to decompose
            foreach (string taskID in TaskIDList)
            {
                // If the task needs to be decomposed, decompose it
                if (!specificationKnowledge.IsTaskAPrimitive(taskID))
                {
                    HTNMethod method = htnPlanner.FindASuitableMethodOrOperator(taskID, currentState);

                    HTNTaskNetwork decomposedTaskNetwork = specificationKnowledge.GetTaskNetwork(method.TaskNetworkID);
                    int constraintsCount = decomposedTaskNetwork.Constraints.Count;

                    Random random = new Random();
                    int selectedConstraint = random.Next(0, constraintsCount);
                    HTNTaskNetworkConstraints htnTaskNetworkConstraints
                        = decomposedTaskNetwork.Constraints.ElementAt(selectedConstraint).Value;

                    List<HTNTaskSet> decomposedTaskSetList = htnTaskNetworkConstraints.HTNTaskConstraints;
                    if (decomposedTaskSetList.Count > longestSize)
                    {
                        longestSize = decomposedTaskSetList.Count;
                    }

                    string resourceID = TaskExecutors[taskID];
                    foreach (HTNTaskSet decomposedHTNTask in decomposedTaskSetList)
                    {
                        foreach (string subTaskID in decomposedHTNTask.TaskIDList)
                        {
                            decomposedHTNTask.TaskExecutors.Add(subTaskID, resourceID);
                        }
                    }

                    allNewDecomposedTaskSetList.Add(decomposedTaskSetList);
                }   
                // The task is primitive so does not need to be decomposed further
                else
                {
                    List<HTNTaskSet> decomposedTaskSetList = new List<HTNTaskSet>();
                    HTNTaskSet htnTaskSet = new HTNTaskSet();

                    string resourceID = TaskExecutors[taskID];

                    htnTaskSet.TaskExecutors.Add(taskID, resourceID);
                    htnTaskSet.TaskIDList.Add(taskID);

                    decomposedTaskSetList.Add(htnTaskSet);
                    allNewDecomposedTaskSetList.Add(decomposedTaskSetList);
                }
            }
            // end of decomposing


            // Schedule all tasks into a single list of task sets

            List<HTNTaskSet> mergeredHTNSetList = new List<HTNTaskSet>();
            for (int i = 0; i < longestSize; i++)
            {
                HTNTaskSet newlyMergeredHTNTaskSet = new HTNTaskSet();

                foreach (List<HTNTaskSet> htnTaskSetList in allNewDecomposedTaskSetList)
                {
                    if (htnTaskSetList.Count > i)
                    {
                        foreach (string taskID in htnTaskSetList[i].TaskIDList)
                        {
                            string resourceID = htnTaskSetList[i].TaskExecutors[taskID];

                            newlyMergeredHTNTaskSet.TaskExecutors.Add(taskID, resourceID);
                            newlyMergeredHTNTaskSet.AddTaskID(taskID);

                        }
                    }
                }
                mergeredHTNSetList.Add(newlyMergeredHTNTaskSet);
            }

            return mergeredHTNSetList;
        }



        public List<HTNTaskSet> AssignAndRefineTaskSet(
            ResourceProfileSet resourceSet,
            HTNState currentHTNState,
            HTNState targetHTNState,
            SpecificationKnowledge specificationKnowledge)
        {
            List<HTNTaskSet> newRefinedHTNTaskSetList = new List<HTNTaskSet>();

            List<string> waitingTaskList = new List<string>();
            foreach (string taskID in this.TaskIDList)
            {
                waitingTaskList.Add(taskID);
            }

            while (waitingTaskList.Count > 0)
            {
                List<string> allocatedResourceIDList = new List<string>();
                List<string> allocatedTaskIDList = new List<string>();
                List<bool> resourceAllocatedIndicator = new List<bool>();

                HTNTaskSet htnTaskSet = new HTNTaskSet();

                for (int i = 0; i < resourceSet.Resources.Count; i++)
                {
                    resourceAllocatedIndicator.Add(true);
                }

                foreach (string taskID in waitingTaskList)
                {
                    for (int i = 0; i < resourceSet.Resources.Count; i++)
                    {
                        ResourceProfile ra = resourceSet.Resources[i];

                        if (resourceAllocatedIndicator[i] && ra.CanExecuteThisTask(taskID))
                        {
                            resourceAllocatedIndicator[i] = false;
                            htnTaskSet.AddTaskID(taskID);
                            htnTaskSet.TaskExecutors.Add(taskID, ra.ID);
                            allocatedTaskIDList.Add(taskID);

                            //  added to prevent the re-added value
                            break;
                        }
                    }
                }

                foreach (string taskID in allocatedTaskIDList)
                {
                    waitingTaskList.Remove(taskID);
                }

                newRefinedHTNTaskSetList.Add(htnTaskSet);
            }
            return newRefinedHTNTaskSetList;
        } 


        private bool DeduceFollowingStates(
            IEnumerable<HTNTaskSet> refinedHTNTaskSetList,
            HTNState currentState,
            HTNState targetState,
            SpecificationKnowledge specificationKnowledge)
        {

            List<HTNState> newlyDeducedHTNStateList = new List<HTNState>();
            newlyDeducedHTNStateList.Add(currentState);

            HTNState localCurrentHTNState = new HTNState(currentState);
            List<List<HTNEffect>> htnEffectList = new List<List<HTNEffect>>();

            foreach (HTNTaskSet htnTaskSet in refinedHTNTaskSetList)
            {
                List<HTNEffect> postEffects = new List<HTNEffect>();

                foreach (string taskID in htnTaskSet.TaskIDList)
                {
                    HTNTask htnTask = specificationKnowledge.GetHTNTask(taskID);
                    postEffects.AddRange(htnTask.PostConditions);
                }
                htnEffectList.Add(postEffects);
            }

            HTNState nextHTNState = null;
            foreach (List<HTNEffect> htnEffects in htnEffectList)
            {
                nextHTNState = new HTNState(localCurrentHTNState.DeduceNextStateBasedOnEffects(htnEffects));
                newlyDeducedHTNStateList.Add(nextHTNState);
            }

            if (nextHTNState != null && nextHTNState.IsEqualTo(targetState))
            {
                newlyDeducedHTNStateList.Remove(nextHTNState);
                RefinedStateList = newlyDeducedHTNStateList;
                return true;
            }

            return false;

        }

        public bool IsScheduled()
        {
            List<string> resourceNameList = new List<string>();

            foreach (KeyValuePair<string, string> kvp in this.TaskExecutors)
            {
                if (!resourceNameList.Contains(kvp.Value))
                {
                    resourceNameList.Add(kvp.Value);
                }
                else
                    return false;
            }

            return true;
        }

        public bool MoreThanOneTask()
        {
            return TaskIDList.Count > 1;
        }
    }
}
