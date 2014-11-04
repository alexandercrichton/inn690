using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using Veis.Common.Math;
using Veis.Chat;
using Veis.Common;
using Vector3 = Veis.Common.Math.Vector3;
using Veis.Data.Services;
using Veis.Data;
using Veis.Data.Entities;
using Veis.Data.Repositories;
using Veis.Simulation.WorldState;


namespace Veis.Bots
{
    /// <summary>
    /// This class provides an abstract interface for a Bot avatar.
    /// It has abstract method signatures for the possible actions a
    /// bot avatar can complete. It also manages the task queue enaction.
    /// 
    /// Note that this class doesn't support concurrency. It is set up
    /// to not be accessed from anywhere other than one dedicated thread.
    /// </summary>
    public abstract class BotAvatar : Avatar
    {
        public BotWorkEnactor WorkEnactor { get; set; }

        public ChatProvider ChatHandle { get; set; } // Chat provider to interpret messages

        private readonly IActivityMethodService _methodService;
        private readonly WorldStateRepository _worldState;
        private readonly ServiceRoutineService _routineService;

        public BotAvatar(IActivityMethodService methodService,
            WorldStateRepository worldState, ServiceRoutineService routineService)
        {
            _methodService = methodService;
            _worldState = worldState;
            _routineService = routineService;
        }

        #region Bot Avatar Control Functions

        public abstract void Say(string message);

        public abstract void Despawn();

        public abstract void WalkTo(string location);

        public abstract void Touch(String objectName);

        public abstract bool IsAt(string location);

        public abstract void DefineTask(string task);

        protected void doAssetInteraction(string assetName, string predicate, string value)
        {
            Veis.Unity.Logging.UnityLogger.BroadcastMesage(this,
                string.Format("doAssetInteraction() assetName: {0}, predicate: {1}, value: {2}",
                assetName, predicate, value));

            _worldState.Update(assetName, predicate, value);

          
        }

        protected void doServiceRoutine(string assetName, string methodName, string value)
        {

            Veis.Unity.Logging.UnityLogger.BroadcastMesage(this,
                string.Format("doServiceRoutine() assetName: {0}, methodName: {1}, value: {2}",
                assetName, methodName, value));

            if (methodName.Length >= "Move".Length
                && string.Equals(methodName.Substring(0, "Move".Length), "Move", StringComparison.OrdinalIgnoreCase))
            {
                _routineService.AddServiceRoutine(new AssetServiceRoutine()
                {
                    Priority = 1,
                    AssetKey = assetName,
                    Id = 1,
                    ServiceRoutine = "Move:" + value
                });
            }  
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
            else if (WorkEnactor.IsWorkAvailable())
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
                    if (doNextTask)
                    {
                        WalkTo(currentTask.Split(':')[1]);
                        doNextTask = false;
                    }
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
                case AvailableActions.ASSETINTERACTION:
                    var iParts = currentTask.Split(':');
                    doAssetInteraction(iParts[1], iParts[2], iParts[3]);
                    break;
                case AvailableActions.ASSETSERVICEROUTINE:
                    var sParts = currentTask.Split(':');
                    doServiceRoutine(sParts[1], sParts[2], sParts[3]);
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
