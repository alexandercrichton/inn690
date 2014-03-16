using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Veis.Simulation;
using System.Collections;
using Veis.WebInterface.Models;
using Veis.Workflow;
using Veis.WebInterface.Infrastructure;

namespace Veis.WebInterface.Controllers
{
    public class SimulationController : Controller
    {
        private readonly Simulation.Simulation _simulation;
        private readonly WorkflowProvider _workflowProvider;

        public SimulationController(Simulation.Simulation simulation, WorkflowProvider workflowProvider)
        {
            _simulation = simulation;
            _workflowProvider = workflowProvider;
        }

        public Hashtable Manage(Hashtable reqParams)
        {
            if (reqParams.ContainsKey("uuid"))
            {
                
                var model = new SimulationModel();
                model.Bind(_simulation, _workflowProvider, (string)reqParams["uuid"]);
                var page = PageBuilder.Current.Transform("simulationmanagement", model);
                return FormulateResponse(page, 200);
            }
            else
            {
                return ShowErrorPage("Need to include user id");
            }
        }

        public Hashtable Initialise(Hashtable reqParams)
        {
            _simulation.PerformSimulationAction(SimulationActions.Start);
            const string message = "Initialising simulation...";
            return FormulateResponse(message, 200);
        }

        public Hashtable Reset(Hashtable reqParams)
        {
            _simulation.PerformSimulationAction(SimulationActions.Reset);
            const string message = "Resetting simulation...";
            return FormulateResponse(message, 200);
        }

        public Hashtable LaunchCase(Hashtable reqParams)
        {
            const string errorMessage = "Need to specifiy specification to run.";
            const string statusMessage = "Launching case of {0}";
            if (!reqParams.ContainsKey("spec"))
                return FormulateResponse(errorMessage, 404);
            _simulation.RequestLaunchCase(reqParams["spec"].ToString());
            return FormulateResponse(String.Format(statusMessage, reqParams["spec"].ToString()), 200);                
        }

        public Hashtable EndAllCases(Hashtable reqParams)
        {
            _simulation.RequestCancelAllCases();
            const string message = "Ending all cases";
            return FormulateResponse(message, 200);
        }

        public Hashtable RegisterAsParticipant(Hashtable reqParams)
        {
            const string errorMessage = "Must specify both user UUID and role name to register as.";
            const string statusMessage = "Registering you as {0}";
            if (!reqParams.ContainsKey("uuid") ||  !reqParams.ContainsKey("role"))
                return FormulateResponse(errorMessage, 404);

            _simulation.RegisterUser(new UserArgs
            {
                UserAction = UserActions.Register,
                RoleName = reqParams["role"].ToString(),
                UserId = reqParams["uuid"].ToString(),
                UserName = ""
            });
            return FormulateResponse(String.Format(statusMessage, reqParams["role"]), 200);         
        }

       
    }
}
