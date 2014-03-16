using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using OpenSim.Framework.Servers;
using Veis.Workflow;
using Veis.Simulation;
using Veis.OpenSim.Simulation;
using Veis.WebInterface.Controllers;
using Veis.Bots;
using System.Reflection;
using OpenSim.Framework.Servers.HttpServer;

namespace Veis.WebInterface
{
    public class WebInterfaceModule
    {
        protected readonly OpenSimYAWLSimulation _simulation;
        protected WorkController _workController;
        protected SimulationController _simulationController;
        private Dictionary<string, GenericHTTPMethod> _routes;
        private bool isInitialised;

        public WebInterfaceModule(OpenSimYAWLSimulation simulation)
        {
            _simulation = simulation;
            _routes = new Dictionary<string, GenericHTTPMethod>();
            isInitialised = false;
        }

        // Sets up all routes
        public void Initialise(WorkflowProvider workflowProvider)
        {
            if (isInitialised) return;
            _workController = new WorkController(_simulation);
            _simulationController = new SimulationController(_simulation, workflowProvider);

            RegisterRoutes(GetRoutes(_workController));
            RegisterRoutes(GetRoutes(_simulationController));
            
            isInitialised = true;
        }

        private void RegisterRoutes(Dictionary<string, GenericHTTPMethod> routes)
        {
            foreach (var route in routes)
            {
                MainServer.Instance.AddHTTPHandler(route.Key, route.Value);
            }
        }

        private Dictionary<string, GenericHTTPMethod> GetRoutes(Controller controller)
        {
            const string Suffix = "Controller";
            const int SuffixLength = 10;

            var routes = new Dictionary<string, GenericHTTPMethod>();

            var type = controller.GetType();
            var typeName = type.Name;
            if (!typeName.EndsWith(Suffix)) return new Dictionary<string, GenericHTTPMethod>();
            var controllerName = typeName.Remove(typeName.Length - SuffixLength);

            var publicMethods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            var delegates = publicMethods
                .Where(m => m.ReturnType == typeof(Hashtable) && 
                            m.GetParameters().Count() == 1 && 
                            m.GetParameters()[0].ParameterType == typeof(Hashtable))
                .Select(m => (GenericHTTPMethod)Delegate.CreateDelegate
                        (typeof(GenericHTTPMethod), controller, m.Name)).ToList();

            foreach (var handler in delegates)
            {
                routes.Add("/" + controllerName + "/" + handler.Method.Name, handler);
            }

            return routes;
        }



    }
}
