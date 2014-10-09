using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using Veis.Common.Math;
using Veis.Chat;
using Veis.Common;
using Vector3 = Veis.Common.Math.Vector3;


namespace Veis.Bots
{
    /// <summary>
    /// This class provides an abstract interface for a Bot avatar.
    /// It has abstract method signatures for the possible actions a
    /// bot avatar can complete. It also manages the task queue enaction.
    /// </summary>
    public abstract class BotAvatar : Avatar
    {
        public BotWorkEnactor WorkEnactor { get; set; }

        public ChatProvider ChatHandle { get; set; } // Chat provider to interpret messages

        public List<ExecutableAction> ExecutableActions { get; set; }

        public BotAvatar()
        {
            ExecutableActions = new List<ExecutableAction>();
        }

        #region Bot Avatar Control Functions

        public abstract void Say(string message);

        public abstract void Despawn();

        public abstract void WalkTo(string location);

        public abstract void Touch(String objectName);

        public abstract bool IsAt(string location);

        public abstract void DefineTask(string task);

        public void ExecuteAction(string asset, string methodName, string parameterString)
        {
            ExecutableActions.Add(new ExecutableAction
            {
                AssetName = asset,
                MethodName = methodName,
                Parameters = StringFormattingExtensions.DecodeParameterString(parameterString)
            });
        }

        // Retrieves the executable action, and pops it off the list
        public ExecutableAction PopExecutableAction(string assetName)
        {
            var action = ExecutableActions.Where(a => a.AssetName.Equals(assetName, StringComparison.OrdinalIgnoreCase))
                .FirstOrDefault();
            if (action != null)
            {
                ExecutableActions.Remove(action);
            }

            return action;
        }

        #endregion

        #region Agent Task Queue

        public Queue<string> taskQueue = new Queue<string>();
        private string currentTask = "";
        private bool doNextTask = true;

        public void Update()
        {
            if (taskQueue.Count > 0)
            {
                processTasks();
            }
            else if (WorkEnactor.WorkReady())
            {
                taskQueue = WorkEnactor.GetNextTasks();
            }
        }

        private void processTasks()
        {
            if (doNextTask)
            {
                currentTask = taskQueue.Dequeue();
                DefineTask(currentTask);
                if (currentTask != "")
                {
                    Say(currentTask); 
                }
            }

            string action = currentTask.Split(':')[0];

            switch (action.ToUpper())
            {
                case AvailableActions.DESPAWN:
                    Despawn();
                    break;
                case AvailableActions.WALKTO:
                    WalkTo(currentTask.Split(':')[1]);
                    doNextTask = false;
                    if (IsAt(currentTask.Split(':')[1]))
                    {
                        doNextTask = true;
                    }
                    break;
                case AvailableActions.TOUCH:
                    Touch(currentTask.Split(':')[1]);
                    break;
                //case AvailableActions.STARTWORK:
                //    WorkEnactor.StartWork(currentTask.Split(':')[1]);
                //    break;
                case AvailableActions.COMPLETEWORK:
                    WorkEnactor.CompleteWork(currentTask.Split(':')[1]);
                    break;
                case AvailableActions.EXECUTEACTION:
                    var parts = currentTask.Split(':');
                    ExecuteAction(parts[1], parts[2], parts[3]);
                    break;
                case AvailableActions.SAY:
                    Say(currentTask.Split(':')[1]);
                    break;
                default:
                    if (currentTask != "")
                    {
                        Say("{ERROR:TASK:UNKNOWN:" + action.ToUpper() + "}");
                    }
                    break;
            }
        }

        #endregion

    }
}
