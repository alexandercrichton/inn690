using System.Collections.Generic;

namespace Veis.Planning.HTN
{
    public class HTNTaskNetworkConstraints
    {
        protected string id;
        public List<HTNState> StateConstraints { get; set; }
        public List<HTNTaskSet> HTNTaskConstraints { get; set; }

        public HTNTaskNetworkConstraints()
        {
            HTNTaskConstraints = new List<HTNTaskSet>();
            StateConstraints = new List<HTNState>();
        }

        public HTNTaskNetworkConstraints(HTNTaskNetworkConstraints htnTaskNetworkConstraints) : this()
        {
            id = (string)htnTaskNetworkConstraints.id.Clone();

            foreach (HTNTaskSet taskNameSet in htnTaskNetworkConstraints.HTNTaskConstraints)
            {
                HTNTaskConstraints.Add(new HTNTaskSet(taskNameSet));
            }

            foreach (HTNState htnState in htnTaskNetworkConstraints.StateConstraints)
            {
                StateConstraints.Add(new HTNState(htnState));
            }
        }
    }

    // TODO : Not used
    public class HTNOrderingConstraints
    {
        public List<string> OrderingConstraints { get; set; }
    }

    // TODO: Not used
    public class HTNStateConstrains
    {
        public List<string> StateConstraints { get; set; }
    }
}
