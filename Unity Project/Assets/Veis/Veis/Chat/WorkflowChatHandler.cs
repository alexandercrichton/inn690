using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Veis.Workflow;


namespace Veis.Chat
{
    public class WorkflowChatHandler : ChatHandler
    {
        private readonly WorkAgent _workAgent;
        private readonly WorkflowProvider _workflow;

        public WorkflowChatHandler(WorkAgent workAgent, WorkflowProvider workProvider)
        {
            _workAgent = workAgent;
            _workflow = workProvider;
        }

        public override bool CanHandleMessage(string message)
        {
            bool can = message.Split(':')[0] == "YAWL";
            //System.Console.WriteLine("Can handle message: " + can);
            return can;
        }

        protected override string ProcessMessage(string answer, string pre, string post)
        {
            //System.Console.WriteLine("Answer: " + answer);

            string output = "I can't do this \"" + answer + "\" that you ask of me.";

            // TODO When YAWL talking is sorted out
            switch (answer.Split(':')[1])
            {
                case "CURRENTWORK":
                    output = "Doing_amazing_thing...";
                    break;
                case "COMPLETEWORKITEM":
                    if (answer.Split(':').Length == 3)
                    {
                        WorkItem item = _workflow.AllWorkItems.FirstOrDefault(w => w.TaskName == answer.Split(':')[2]);
                        if (_workAgent.started.Contains(item) && _workAgent.processing.Contains(item))
                        {
                            _workAgent.Complete(item, _workflow);
                            output = pre + "Completed " + item.TaskName + post;
                        }
                        else
                        {
                            output = pre + "{PROC:ERROR:COMPLETE:UNKNOWNERROR}" + post;
                        }
                    }
                    else
                    {
                        while (_workAgent.processing.Count > 0)
                        {
                            _workAgent.Complete(_workAgent.processing[0], _workflow);
                        }
                    }
                    break;
                case "DELEGATEWORKITEM":
                    if (answer.Split(':').Length == 4)
                    {
                        WorkItem item = _workflow.AllWorkItems.FirstOrDefault(w => w.TaskName == answer.Split(':')[2]);
                        WorkAgent other = _workflow.AllWorkAgents.FirstOrDefault(a => a.FirstName == answer.Split(':')[3]);

                        if (item != null && _workAgent.started.Contains(item) && other != null)
                        {
                            _workAgent.Delegate(item, other, _workflow);
                            output = pre + "Delegated " + item.TaskName + " to " + other.FirstName + post;
                        }
                        else if (item == null)
                        {
                            output = pre + "Sorry; No Work Item With That Name Could Be Found" + post;
                        }
                        else if (other == null)
                        {
                            output = pre + "Sorry; That agent could not be found" + post;
                        }
                        else
                        {
                            output = pre + "{PROC:YAWL:DELEGATE:UNKNOWNERROR}" + post;
                        }
                    }
                    break;
                case "SUSPENDWORKITEM":
                    if (answer.Split(':').Length == 3)
                    {
                        WorkItem item = _workflow.AllWorkItems.FirstOrDefault(w => w.TaskName == answer.Split(':')[2]);
                        if (_workAgent.started.Contains(item))
                        {
                            _workAgent.Suspend(item, _workflow);
                            output = pre + "Suspended " + item.TaskName + post;
                        }
                        else
                        {
                            output = pre + "{PROC:ERROR:SUSPEND:UNKNOWNERROR}" + post;
                        }
                    }
                    else
                    {
                        while (_workAgent.processing.Count > 0)
                        {
                            _workAgent.Suspend(_workAgent.processing[0], _workflow);
                        }
                    }
                    break;
                case "UNSUSPENDWORKITEM":
                    if (answer.Split(':').Length == 3)
                    {
                        WorkItem item = _workflow.AllWorkItems.FirstOrDefault(w => w.TaskName == answer.Split(':')[2]);
                        if (_workAgent.suspended.Contains(item))
                        {
                            _workAgent.Unsuspend(item, _workflow);
                            output = pre + "Unsuspended " + item.TaskName + post;
                        }
                        else
                        {
                            output = pre + "{PROC:ERROR:UNSUSPEND:UNKNOWNERROR}" + post;
                        }
                    }
                    else
                    {
                        while (_workAgent.processing.Count > 0)
                        {
                            _workAgent.Unsuspend(_workAgent.processing[0], _workflow);
                        }
                    }
                    break;
            }

            return output;
        }
    }
}
