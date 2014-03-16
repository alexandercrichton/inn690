using System.Collections.Generic;

namespace Veis.Planning.HTN
{
    public class HTNMethod
    {
        public string MethodID { get; set; }
        public string TaskID { get; set; }            
        public string TaskNetworkID { get; set; }
        public List<HTNEffect> PreConditions { get; set; }
        public List<HTNEffect> PostConditions { get; set; }

        public HTNMethod()
        {
            PreConditions = new List<HTNEffect>();
            PostConditions = new List<HTNEffect>();
        }
        
        // TODO This appears to not be implemented ffs.
        public bool CurrentStateSatisifyMethodPrecondition(HTNState currentState)
        {
            return true;
        }

        public bool SuitsAMethod(string taskID, HTNState htnState)
        {
            if (TaskID.Equals(taskID) && htnState.IsThisAViolatingEffect(PreConditions))
                return true;
            return false;
        }
    }
}
