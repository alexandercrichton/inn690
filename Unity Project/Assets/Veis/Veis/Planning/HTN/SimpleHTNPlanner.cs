using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Veis.Planning.Knowledge;
using Veis.Planning.Resourcing;

namespace Veis.Planning.HTN
{
    public class SimpleHTNPlanner : HTNPlanner
    {
        public override void SetPlannerProperties(HTNState initialHTNState, HTNState targetHTNState, string workitemID, SpecificationKnowledge specificationKnowledge, ResourceProfileSet resourceProfileSet)
        {
            throw new NotImplementedException();
        }

        public override List<HTNTaskSet> GeneratePlan()
        {
            throw new NotImplementedException();
        }
    }
}
