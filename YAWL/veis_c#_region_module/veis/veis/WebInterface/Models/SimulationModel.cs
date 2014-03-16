using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Veis.Bots;

namespace Veis.WebInterface.Models
{
    public class SimulationModel
    {
        // Running case
        // Not intialised
        // Case not running

        public string UUID { get; set; }

        public string Role { get; set; }

        public bool IsInitialised { get; set; }

        public bool IsRunningCases { get; set; }

        public IList<CaseModel> RunningCases { get; set; }

        public IEnumerable<string> AvailableSpecifications { get; set;} // NEED TO GET THIS INFO FROM JAVA

        public IEnumerable<string> AvailableRoles { get; set; }

        public SimulationModel()
        {
            RunningCases = new List<CaseModel>();
            AvailableSpecifications = new List<string>();
            AvailableRoles = new List<string>();
            IsInitialised = false;
            IsRunningCases = true;
        }

        public void Bind(Simulation.Simulation simulation, Workflow.WorkflowProvider workflowProvider, string uuid)
        {
            
            UUID = uuid;
            var participant = simulation.GetParticipantById(uuid);
            if (participant != null && participant is HumanAvatar)
            {
                Role = ((HumanAvatar)participant).ActingName;
            }
            IsInitialised = simulation.IsInitialised();
            if (IsInitialised && workflowProvider != null && workflowProvider.IsConnected)
            {
                AvailableRoles = workflowProvider.AllParticipants
                        .Select(p => p.Value.FirstName + " " + p.Value.LastName).ToList();
                AvailableSpecifications = workflowProvider.AllSpecifications.Select(s => s.Value).ToList();
                RunningCases = workflowProvider.StartedCases.Select(c => new CaseModel
                {
                    SpecificationName = c.SpecificationName,
                    CaseNumber = c.CaseId,
                    State = CaseModelStateEnum.Running

                }).ToList();
                if (RunningCases.Count() == 0)
                    IsRunningCases = false;
                else
                    IsRunningCases = true;
            }
        }
    }

    public class CaseModel
    {
        public string SpecificationName { get; set; }
        public string CaseNumber { get; set; }
        public CaseModelStateEnum State { get; set; }
    }

    public enum CaseModelStateEnum
    {
        NotStarted,
        Running,
        Completed
    }
}
