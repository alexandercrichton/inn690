using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Veis.Planning.Knowledge;
using Veis.Planning.Resourcing;

namespace Veis.Planning.HTN
{
    public abstract class HTNPlanner
    {
        public abstract void SetPlannerProperties(
            HTNState initialHTNState,
            HTNState targetHTNState,
            string workitemID,
            SpecificationKnowledge specificationKnowledge,
            ResourceProfileSet resourceProfileSet);

        public abstract List<HTNTaskSet> GeneratePlan();

    }
}
