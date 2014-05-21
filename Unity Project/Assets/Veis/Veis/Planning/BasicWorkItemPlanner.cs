using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Veis.Workflow;
using Veis.Bots;
using Veis.Services.Interfaces;

namespace Veis.Planning
{
    public class BasicWorkItemPlanner : Planner<WorkItem>
    {
        private Dictionary<String, String> _animationMap;    // Map from common action terms to actual animation names
        private readonly ISceneService _sceneService;

        public BasicWorkItemPlanner(ISceneService sceneService)
        {
            _animationMap = InitialiseAnimations();
            _sceneService = sceneService;
        }
        
        public PlanResult MakePlan(WorkItem input)
        {
            PlanResult plan = new PlanResult();

            string onlyText = input.taskName.ToLower().Remove(input.taskName.LastIndexOf("_"));
            string highLevelTask = onlyText.Replace("_", " ").Trim();

            if (highLevelTask.Contains("walk to"))
            {
                string objectName = highLevelTask.Substring("walk to".Count() + 1);
                Veis.Common.Math.Vector3 location = _sceneService.GetPositionOfObject(objectName);
                if (location != null)
                    plan.Tasks.Add(AvailableActions.WALKTO + ":" + location.ToString());
            }
            else if (highLevelTask.Contains("animate"))
            {
                string animationName = highLevelTask.Substring("animate".Count() + 1);
                if (_animationMap.ContainsKey(animationName))
                {
                    AddAnimation(plan, _animationMap[animationName]);
                }
                else
                {
                    AddAnimation(plan, animationName);
                }
                
            }
            else if (highLevelTask.Contains("touch"))
            {
                plan.Tasks.Add(AvailableActions.TOUCH + ":" + highLevelTask.Substring("touch".Count() + 1));
            }
            else if (highLevelTask.Contains("say"))
            {
                plan.Tasks.Add(AvailableActions.SAY + ":" + highLevelTask.Substring("say".Count() + 1));
            }
            else
            {
                if (_animationMap.ContainsKey(highLevelTask))
                {
                    AddAnimation(plan, _animationMap[highLevelTask]);
                }
            }

            return plan;
        }

        public Dictionary<String, String> InitialiseAnimations()
        {
            var animationMap = new Dictionary<String, String>();
            animationMap.Add("laugh", "express_laugh");
            animationMap.Add("dance", "dance1");
            animationMap.Add("wave", "blowkiss");
            animationMap.Add("punch", "punch_onetwo");
            return animationMap;
        }

        private void AddAnimation(PlanResult plan, string animation)
        {
            plan.Tasks.Add(AvailableActions.ANIMATE + ":" + animation);
            plan.Tasks.Add(AvailableActions.WAIT + ":" + 3);
            plan.Tasks.Add(AvailableActions.STOP + ":");
        }
    }
}
