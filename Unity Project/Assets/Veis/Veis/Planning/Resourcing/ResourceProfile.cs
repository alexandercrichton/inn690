using System.Collections.Generic;

namespace Veis.Planning.Resourcing
{
    public class ResourceProfile
    {
        protected string resourceName;
        protected List<string> competentTaskList;
        public string ID { get; set; }

        public ResourceProfile()
        {
            competentTaskList = new List<string>();
        }
        
        public string ReturnResourceName()
        {
            return resourceName;
        }

        public bool CanExecuteThisTask(string taskID)
        {
            return competentTaskList.Contains(taskID);
        }

        public void SetResourceName(string name)
        {
            ID = name;
        }

        public void AddACompetentTask(string taskID)
        {
            competentTaskList.Add(taskID);
        }
    }
}
