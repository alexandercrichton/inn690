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

        public LinkedList<String> taskQueue = new LinkedList<string>();
        private String currentTask = "";
        protected bool isDoingTask = false;

        public void Update()
        {
            processTasks();
        }

        public void AddTaskToQueue(string task)
        {
            lock (taskQueue)
            {
                taskQueue.AddLast(task);
            }
            //processTasks();
        }

        private void processTasks()
		{
            if (currentTask == "")
            {
                lock (taskQueue)
                {
                    if (taskQueue.Count > 0)
                    {
						
						DefineTask(taskQueue.First.Value);
                        currentTask = taskQueue.First.Value;
                        taskQueue.RemoveFirst();
                    }
                }
            }

                Say(currentTask); // TODO: Remove this because it is a debugging message

                string action = currentTask.Split(':')[0];

                switch (action.ToUpper())
                {
                    case AvailableActions.DESPAWN:
                        Despawn();
                        currentTask = "";
                        break;
                    case AvailableActions.WALKTO:
                        WalkTo(currentTask.Split(':')[1]);
                        if (IsAt(currentTask.Split(':')[1]))
                        {
                            currentTask = "";
                        }
                        break;
                    case AvailableActions.TOUCH:
                        Touch(currentTask.Split(':')[1]);
                        currentTask = "";
                        break;
                    case AvailableActions.STARTWORK:
                        WorkEnactor.StartWork(currentTask.Split(':')[1]);
                        currentTask = "";
                        break;
                    case AvailableActions.COMPLETEWORK:
                        WorkEnactor.CompleteWork(currentTask.Split(':')[1]);
                        currentTask = "";
                        break;
                    case AvailableActions.EXECUTEACTION:
                        var parts = currentTask.Split(':');
                        ExecuteAction(parts[1], parts[2], parts[3]);
                        currentTask = "";
                        break;
					case AvailableActions.SAY:
						 Say (currentTask.Split (':')[1]);
					     currentTask = "";
					     break;
				/*
					case AvailableActions.ANIMATE:
						 Animate(parts[1],part[2],parts[3]);
						 break;
						 */
                    default:
                        Say("{ERROR:TASK:UNKNOWN:" + action.ToUpper() + "}");
                        currentTask = "";
                        break;
                }

                //if (currentTask == "")
                //    processTasks();
            
        }



        protected void completeCurrentTask()
        {
            currentTask = "";
            processTasks();
        }

        #endregion

    }
}
