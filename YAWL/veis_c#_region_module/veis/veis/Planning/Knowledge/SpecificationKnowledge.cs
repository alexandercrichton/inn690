using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Veis.Planning.HTN;

namespace Veis.Planning.Knowledge
{
    /// <summary>
    /// Holds workitem and task information specific 
    /// to a single workflow specification.
    /// </summary>
    public class SpecificationKnowledge
    {
        public string Name { get; set; }
        public string Id { get; set; }
        
        protected Dictionary<string, WorkitemKnowledge> workItems;
        protected Dictionary<string, HTNMethod> methods;
        protected Dictionary<string, HTNOperator> operators;
        protected Dictionary<string, HTNTaskNetwork> taskNetworks;
        protected Dictionary<string, HTNTask> tasks;
        protected Dictionary<string, HTNState> states;

        public SpecificationKnowledge()
        {
            workItems = new Dictionary<string, WorkitemKnowledge>();
            methods = new Dictionary<string, HTNMethod>();
            operators = new Dictionary<string, HTNOperator>();
            taskNetworks = new Dictionary<string, HTNTaskNetwork>();
            tasks = new Dictionary<string, HTNTask>();
            states = new Dictionary<string, HTNState>();
        }

        public bool IsTaskAPrimitive(string taskID)
        {
            HTNTask htnTask = tasks[taskID];

            return htnTask.IsThisWorkPrimitive();
        }

        public WorkitemKnowledge GetWorkitemKnowledge(string workitemID)
        {
            return workItems[workitemID];
        }

        public void AddAWorkitem(string id, WorkitemKnowledge workitem)
        {
            workItems.Add(id, workitem);
        }

        public Dictionary<string, HTNMethod> GetMethodDictionary()
        {
            return methods;
        }

        public void AddAMethod(string id, HTNMethod method)
        {
            methods.Add(id, method);
        }

        public HTNTask GetHTNTask(string taskID)
        {
            return tasks[taskID];
        }
        
        public void AddHTNTask(string id, HTNTask htnTask)
        {
            tasks.Add(id, htnTask);
        }

        public HTNTaskNetwork GetTaskNetwork(string taskNetworkID)
        {
            return taskNetworks[taskNetworkID];
        }

        public void AddATaskNetwork(string id, HTNTaskNetwork taskNetwork)
        {
            taskNetworks.Add(id, taskNetwork);
        }


        public void AddAnOperator(string id, HTNOperator htnOperator)
        {
            operators.Add(id, htnOperator);
        }
    }
}
