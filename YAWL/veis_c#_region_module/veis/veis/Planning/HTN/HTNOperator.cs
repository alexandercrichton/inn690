using System.Collections.Generic;

namespace Veis.Planning.HTN
{
    public class HTNOperator
    {
        public string OperatorID { get; set; }
        public string TaskID { get; set; }

        public List<HTNEffect> PreConditions { get; set; }
        public List<HTNEffect> PostConditions { get; set; }

        public HTNOperator()
        {
            PreConditions = new List<HTNEffect>();
            PreConditions = new List<HTNEffect>();
        }

        public HTNOperator(HTNOperator htnOpperator) : this()
        {
            OperatorID = htnOpperator.OperatorID;
            TaskID = htnOpperator.TaskID;

            foreach (HTNEffect htnEffect in htnOpperator.PreConditions)
            {
                HTNEffect newHTNEffect = new HTNEffect(htnEffect);
                PreConditions.Add(newHTNEffect);
            }

            foreach (HTNEffect htnEffect in htnOpperator.PostConditions)
            {
                HTNEffect newHTNEffect = new HTNEffect(htnEffect);
                PostConditions.Add(newHTNEffect);
            }
        }
    }
}
