using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Veis.Planning.HTN;

namespace Veis.Planning.Knowledge
{
    public class WorkItemStateTransition
    {
        public string WorkitemID { get; set; }
        protected HTNState initialState;
        protected HTNState targetState;

        public void AddInitialState(HTNState initialState)
        {
            this.initialState = initialState;
        }

        public void AddTargetState(HTNState targetState)
        {
            this.targetState = targetState;
        }

        public HTNState GetInitialState()
        {
            return initialState;
        }

        public HTNState GetTargetState()
        {
            return targetState;
        }
    }
}
