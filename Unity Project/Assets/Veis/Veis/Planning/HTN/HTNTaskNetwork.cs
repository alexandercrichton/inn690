using System.Collections.Generic;

namespace Veis.Planning.HTN
{
    public class HTNTaskNetwork
    {
        public Dictionary<string, string> HTNTasks { get; set; }

        public List<string> PrimitiveTasks { get; set; }

        public Dictionary<string, HTNTaskNetworkConstraints> Constraints { get; set; }

        public HTNTaskNetwork()
        {
            HTNTasks = new Dictionary<string, string>();
            PrimitiveTasks = new List<string>();
            Constraints = new Dictionary<string, HTNTaskNetworkConstraints>();
        }

        public HTNTaskNetwork(HTNTaskNetwork htnTaskNetwork) : this()
        {
            foreach (string key in htnTaskNetwork.PrimitiveTasks)
            {
                PrimitiveTasks.Add((string)key.Clone());
            }

            foreach (KeyValuePair<string, string> pv in htnTaskNetwork.HTNTasks)
            {
                string id = (string)pv.Key.Clone();
                string name = (string)pv.Value.Clone();
                HTNTasks.Add(id, name);
            }

            foreach (KeyValuePair<string, HTNTaskNetworkConstraints> pv in htnTaskNetwork.Constraints)
            {
                // TODO .....uh? Is this on purpose?
            }
        }

        // TODO: Not used
        public bool IsPrimitiveTaskNetwork()
        {
            // TODO: LOL WAT
            
            bool is_primitive = true;

            this.PrimitiveTasks.Clear();
            /*
            foreach (KeyValuePair<string,HTN_TASK> vp in this.m_htn_task_dic)
            {
                if (vp.Value is HTN_NON_PRIMITIVE_TASK) {
                    is_primitive = false;
                    this.PrimitiveTasks.Add(vp.Key);
                } 
            }
            */
            return is_primitive;
        }



    }
}
