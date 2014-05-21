using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Veis.Planning
{
    public interface Planner<T>
    {
        PlanResult MakePlan(T input);
    }
}
