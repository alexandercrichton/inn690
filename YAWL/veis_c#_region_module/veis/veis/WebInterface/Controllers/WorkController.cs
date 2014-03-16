using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Veis.WebInterface.Models;
using System.Collections;
using Veis.Workflow;
using Veis.Bots;
using Veis.Simulation;
using Veis.Simulation.WorldState;
using Veis.WebInterface.Infrastructure;

namespace Veis.WebInterface.Controllers
{
    public class WorkController : Controller
    {
        // Replace this with avatar manager at some point
        private readonly Simulation.Simulation _simulation;

        public WorkController(Simulation.Simulation simulation)
        {
            _simulation = simulation;
        }

        public Hashtable Show(Hashtable repParams)
        {
            if (repParams.ContainsKey("uuid"))
            {
                var model = new WorklistPersonalModel();
                var user = _simulation.GetParticipantById((string)repParams["uuid"]);
                if (user != null && user is HumanAvatar)
                {
                    model.Bind(user as HumanAvatar, (string)repParams["uuid"]);
                    var page = PageBuilder.Current.Transform("worklistpersonal", model);
                    return FormulateResponse(page, 200);
                }
                else
                    return ShowErrorPage("Couldn't find user");
            }
            else
                return ShowErrorPage("Need to include user id");
        }
    }
}
