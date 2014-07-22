using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Veis.Workflow;
using Veis.Planning.Knowledge;
using Veis.Planning.Resourcing;

namespace Veis.Planning.HTN
{
    public class HTNWorkItemPlanner : Planner<WorkItem>
    {
        private ComplexHTNPlanner _htnPlanner; // The HTN Planner has the logic to make the plan
        private WorkflowKnowledgeBase _knowledgeBase; // The workflow knowledge base has DETAILED knowledge about all the specifications that can be run.

        public HTNWorkItemPlanner()
        {

        }
     
        public PlanResult MakePlan(WorkItem input)
        {
            PlanResult result = new PlanResult();
            return result;
            //string order = SimulationController.getINSTANCE()
            //    .getSimulationDetailsByCaseID(caseId).getOrder();

            //// Get knowlegde base of the workflow specification
            //SpecificationKnowledge specificationKnowledge = _knowledgeBase
            //    .GetSpecificationByDescriptor("Total_Workitems");

            //// Define states for the work item (Move this to be on setup)
            //this.mWorkflowSpecificationBaseKnowledge.Test(); // ???

            //// WHAAAAAAAAT is this?
            //WorkitemStateInstantiationPack stateInstantiation =
            //    this.mWorkflowSpecificationBaseKnowledge
            //    .mWorkitemInstantiator
            //    .GetAWorkitemStateInstantiationPack(workitemName, order, this);

            //WorkitemKnowledge workitemKnowledge = specificationKnowledge.GetWorkitemKnowledge(workitemName);

            //ResourceProfileSet res_prof_set = VisResourceController.GetInstance().GenerateAResourceProfileSet(workitemKnowledge.mResourceList, caseId, workitemName);
            ////workitem_knowledge.m_resource_list, "2");

            //HTNState initalState = stateInstantiation.GetInitialState();
            //HTNState targetState = stateInstantiation.GetTargetState();

            // Get initial and target states (initial state is predicate?) (target state is goal state)
            //HTNState initialState 

           // _htnPlanner.SetPlannerProperties();
            //this.mComplexHTN.set_UMCP(
            //    initial_state,
            //    target_state,
            //    workitemName,
            //    specificationKnowledge,
            //    res_prof_set,
            //    this.mFirstName);

            //List<HTN_TASK_SET> plan = mComplexHTN.generate_a_plan();
        }

        private bool IsAtomicTask(WorkItem input)
        {
            throw new NotImplementedException("IsAtomicTask");
        }

        private List<String> ProcessAtomicTask(String task)
        {
            throw new NotImplementedException("ProcessAtomicTask");
        }
    }
}
