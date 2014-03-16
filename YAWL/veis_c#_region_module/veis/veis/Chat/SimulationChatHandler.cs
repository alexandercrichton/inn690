using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Veis.Workflow.YAWL;

namespace Veis.Chat
{
    /// <summary>
    /// This chat handler processes chat requests that relate to users jumping in and out of
    /// the running process.
    /// </summary>
    public class SimulationChatHandler : ChatHandler
    {
        private YAWLWorkflowProvider _yawlProvider;

        public SimulationChatHandler(YAWLWorkflowProvider provider)
        {
            _yawlProvider = provider;
        }
        
        public override bool CanHandleMessage(string message)
        {
            return message.Split(':')[0] == "YAWL";
        }

        protected override string ProcessMessage(string message, string pre, string post)
        {
            // All agents that do not have a null work provider

            //if (message.Split(':')[1] == "CURRENTAGENTS")
            //{
            //    string agents = "";

            //    foreach (KeyValuePair<string, string> kvp in _yawlProvider.YawlToWorker)
            //    {
            //        ScenePresence sp;
            //        if (_scene.TryGetScenePresence(kvp.Value, out sp))
            //        {
            //            agents += sp.Name + ", ";
            //        }
            //    }

            //    output = pre + agents + post;
            //}
             return "";
        }
    }
}
