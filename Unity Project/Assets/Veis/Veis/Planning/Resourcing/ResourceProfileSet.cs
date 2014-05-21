using System.Collections.Generic;

namespace Veis.Planning.Resourcing
{
    public class ResourceProfileSet
    {
        public List<ResourceProfile> Resources { get; set; }
        protected List<bool> allocationIndicator;

        public Dictionary<string, ResourceProfile> ResourceProfiles { get; set; }

        public ResourceProfileSet()
        {
            Resources = new List<ResourceProfile>();
            allocationIndicator = new List<bool>();
        }

        public void RefreshTheCandidateList()
        {
            for (int i = 0; i < allocationIndicator.Count; i++)
            {
                allocationIndicator[i] = false;
            }
        }

        public void AddResource(ResourceProfile resourceName)
        {
            Resources.Add(resourceName);
            ResourceProfiles.Add(resourceName.ID, resourceName);
        }

        public string FindAnExecutor(string taskID)
        {
            for (int i = 0; i < Resources.Count; i++)
            {
                ResourceProfile r = Resources[i];
                bool a = allocationIndicator[i];
                if (r.CanExecuteThisTask(taskID) && !a)
                    return r.ReturnResourceName();
            }

            return "no_candidate";
        }
    }
}
