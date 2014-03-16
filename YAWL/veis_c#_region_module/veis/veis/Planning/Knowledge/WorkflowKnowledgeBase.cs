using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Veis.Planning.Knowledge
{  
    
    /// <summary>
    /// This class holds the entire domain knowledge base divvied up into specifications
    /// </summary>
    public class WorkflowKnowledgeBase
    {
        private Dictionary<string, SpecificationKnowledge> _specifications; // Specifications are stored against a given descriptor
        //public WorkitemInstantiator WorkitemInstantiator { get; set; }

        public WorkflowKnowledgeBase()
        {
            _specifications = new Dictionary<string, SpecificationKnowledge>();
            //WorkitemInstantiator = new WorkitemInstantiator();
        }

        #region Getters

        public SpecificationKnowledge GetSpecificationByDescriptor(string descriptor)
        {
            if (_specifications.ContainsKey(descriptor))
            {
                return _specifications[descriptor];
            }
            return null;
        }

        public SpecificationKnowledge GetSpecificationByName(string name)
        {
            return _specifications.Values.FirstOrDefault(spec => spec.Name == name);
        }

        public SpecificationKnowledge GetSpecificationById(string id)
        {
            return _specifications.Values.FirstOrDefault(spec => spec.Id == id);
        }

        public IList<SpecificationKnowledge> GetAllSpecifications()
        {
            return _specifications.Values.ToList();
        }

        #endregion

        #region Loading from file

        public bool InstantiateFromFile(string filepath)
        {
            throw new NotImplementedException("WorkflowKnowledgeBase: InstiantiateFromFile");
        }

        #endregion
    }
}
