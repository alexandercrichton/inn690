using System.Collections.Generic;

namespace Veis.Planning.HTN
{
    public class HTNTask
    {
        public string TaskName { get; set; }

        public string ExecutorName { get; set; }

        public List<HTNEffect> PreConditions { get; set; }
        public List<HTNEffect> PostConditions { get; set; }

        protected bool isPrimitive = true;

        public HTNTask()
        {
            PreConditions = new List<HTNEffect>();
            PostConditions = new List<HTNEffect>();
        }

        public HTNTask(HTNTask htnTask) : this()
        {
            TaskName = (string)(htnTask.TaskName.Clone());
            isPrimitive = htnTask.isPrimitive;

            foreach (HTNEffect p in htnTask.PreConditions)
            {
                PreConditions.Add(p);
            }

            foreach (HTNEffect p in htnTask.PostConditions)
            {
                PostConditions.Add(p);
            }
        }

        public void SetPrimitive(bool isPrimitive)
        {
            this.isPrimitive = isPrimitive;
        }

        public bool IsThisWorkPrimitive()
        {
            return isPrimitive;
        }
    }
}
