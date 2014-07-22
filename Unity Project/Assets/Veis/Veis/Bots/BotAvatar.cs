using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using Veis.Common.Math;
using Veis.Chat;
using Veis.Common;
//using OpenMetaverse;
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
        public WorkProvider WorkProvider { get; set; }

        public ChatProvider ChatHandle { get; set; } // Chat provider to interpret messages

        public List<ExecutableAction> ExecutableActions { get; set; }

        public BotAvatar()
        {
            ExecutableActions = new List<ExecutableAction>();
        }

        #region Bot Avatar Control Functions

        public abstract void Say(string message);

        //public abstract void SendTextBox(string message, int chatChannel, string objectname, UUID ownerID, string ownerFirstName, string ownerLastName, UUID objectId);

        public abstract void FlyToLocation(Vector3 position);

        public abstract void WalkToLocation(Vector3 position);

        public abstract void Despawn();

        public abstract void WalkTo(string areaName);

        public abstract void PickUp(string objectName);

        public abstract void Drop();

        public abstract void Touch(String objectName);

        public abstract void SitOn(String objectName);

        public abstract void StandUp();

        public abstract void PlayAnimation(string animationName);

        public abstract void StopAnimation();

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

        public void TaskWait(double seconds)
        {
            System.Timers.Timer t = new System.Timers.Timer();
            t.Interval = seconds * 1000;
            t.AutoReset = false;
            t.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            t.Enabled = true;
        }

        private void OnTimedEvent(object sender, ElapsedEventArgs e)
        {
            currentTask = "";
            ProcessNextTask();
        }

        #endregion

        #region Agent Task Queue

        public LinkedList<String> taskQueue = new LinkedList<string>();
        private String currentTask = "";
        private int taskLoopIndex = 0;

        public void AddTaskToQueue(string task)
        {
            lock (taskQueue)
            {
                taskQueue.AddLast(task);
            }

            ProcessNextTask();
        }

        private void ProcessNextTask()
        {
            if (currentTask != "") return;

            lock (taskQueue)
            {
                if (taskQueue.Count > 0)
                {
                    currentTask = taskQueue.First.Value;
                    taskQueue.RemoveFirst();

                    if (taskLoopIndex > 0)
                    {
                        taskQueue.AddLast(currentTask);
                    }
                }
            }

            if (currentTask != "")
            {
                Say(currentTask); // TODO: Remove this because it is a debugging message

                string action = currentTask.Split(':')[0];

                switch (action.ToUpper())
                {
                    case AvailableActions.WAIT:
                        TaskWait(double.Parse(currentTask.Split(':')[1]));
                        break;
                    case AvailableActions.DESPAWN:
                        Despawn();
                        currentTask = "";
                        break;
                    case AvailableActions.ANIMATE:
                        PlayAnimation(currentTask.Split(':')[1]);
                        currentTask = "";
                        break;
                    case AvailableActions.STOP:
                        StopAnimation();
                        currentTask = "";
                        break;
                    case AvailableActions.GOTO:
                        WalkTo(currentTask.Split(':')[1]);
                        currentTask = "";
                        break;
                    case AvailableActions.FLYTO:
                        FlyToLocation(Vector3.Parse(currentTask.Split(':')[1]));
                        currentTask = "";
                        break;
                    case AvailableActions.WALKTO:
                        WalkToLocation(Vector3.Parse(currentTask.Split(':')[1]));
                        currentTask = "";
                        break;
                    case AvailableActions.SAY:
                        Say(currentTask.Split(':')[1]);
                        currentTask = "";
                        break;
                    case AvailableActions.SIT:
                    case AvailableActions.SITON:
                        SitOn(currentTask.Split(':')[1]);
                        currentTask = "";
                        break;
                    case AvailableActions.STAND:
                    case AvailableActions.STANDUP:
                        StandUp();
                        currentTask = "";
                        break;
                    case AvailableActions.TOUCH:
                        Touch(currentTask.Split(':')[1]);
                        currentTask = "";
                        break;
                    case AvailableActions.PICKUP:
                    case AvailableActions.GRAB:
                        PickUp(currentTask.Split(':')[1]);
                        currentTask = "";
                        break;
                    case AvailableActions.DROP:
                        Drop();
                        currentTask = "";
                        break;
                    case AvailableActions.STARTLOOP:
                        taskLoopIndex++;

                        if (currentTask.Split(':').Length > 1)
                        {
                            int loopIndex = int.Parse(currentTask.Split(':')[1]);

                            if (loopIndex != 0)
                            {
                                lock (taskQueue)
                                {
                                    if (taskQueue.Count > 0)
                                    {
                                        taskQueue.AddLast(AvailableActions.STARTLOOP + ":" + (loopIndex - 1));
                                    }
                                }
                            }
                        }
                        else
                        {
                            taskQueue.AddLast(AvailableActions.STARTLOOP);
                        }
                        currentTask = "";
                        break;
                    case AvailableActions.ENDLOOP:
                        taskLoopIndex--;
                        currentTask = "";
                        break;
                    case AvailableActions.STARTWORK:
                        WorkProvider.StartWork(currentTask.Split(':')[1]);
                        currentTask = "";
                        break;
                    case AvailableActions.COMPLETEWORK:
                        WorkProvider.CompleteWork(currentTask.Split(':')[1]);
                        currentTask = "";
                        break;
                    case AvailableActions.EXECUTEACTION:
                        var parts = currentTask.Split(':');
                        ExecuteAction(parts[1], parts[2], parts[3]);
                        currentTask = "";
                        break;
                    default:
                        Say("{ERROR:TASK:UNKNOWN:" + action.ToUpper() + "}");
                        currentTask = "";
                        break;
                }

                if (currentTask == "")
                    ProcessNextTask();
            }
        }

        #endregion

        #region Chat Handling

        public string RecieveChat(ChatMessage message)
        {
            String reply = String.Empty;

            if (message.Message[0] == '@') // '@' indicates a direct command
            {
                if (message.Message.Split(' ')[0].ToLower() == "@here")
                {
                    reply = "Okay, I'll come to you";
                    FlyToLocation(message.Position);
                }
                else
                {
                    if(message.Message.Trim().Length > 1)
                    {
                        reply = "I'll get right onto it: " + message.Message.Substring(1);
                        AddTaskToQueue(message.Message.Substring(1));
                    }
                    else
                    {
                        //Only @ received
                        //May add a list of posible command here for ease of use
                        reply = "Hi there, I'm waiting for your command.";
                    }
                }
            }
            else
            {
                // This one will let NPC chat by AIML
                reply = ChatHandle.ProcessChat(message.Message, message.SenderName,
                        message.SenderId);
            }

            return reply;
        }

        #endregion

    }
}
