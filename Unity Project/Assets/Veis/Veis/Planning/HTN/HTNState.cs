using System.Collections.Generic;
using Veis.Planning.Knowledge;

namespace Veis.Planning.HTN
{
    public class HTNState
    {
        public List<string> StateAtoms { get; set; }

        public Dictionary<string, string> States { get; set; }

        public string PlanningDomain { get; set; }

        public HTNState()
        {
            StateAtoms = new List<string>();
            States = new Dictionary<string, string>();
        }

        public HTNState(HTNState htnState) : this()
        {          
            PlanningDomain = htnState.PlanningDomain;

            foreach (KeyValuePair<string, string> kvp in this.States)
            {
                States.Remove(kvp.Key);
            }

            foreach (KeyValuePair<string, string> kvp in htnState.States)
            {
                string key = new string(kvp.Key.ToCharArray());
                string value = new string(kvp.Value.ToCharArray());
                States.Add(key, value);
            }
        }

        public bool IsThisAViolatingEffect(List<HTNEffect> effectList)
        {
            int thisSize = States.Count;
            int effectSize = effectList.Count;

            if (thisSize < effectSize)
                return false;

            foreach (HTNEffect htnEffect in effectList)
            {
                if (States.ContainsKey(htnEffect.StateAtomName))
                {
                    string valueInState = States[htnEffect.StateAtomName];
                    if (!valueInState.Equals(htnEffect.StateAtomValue))
                    {
                        return false;
                    }
                }
                else
                    return false;
            }

            return true;
        }

        public void AddStateAtom(string name)
        {
            States.Add(name, "not");
        }

        public void AddStateAtomPair(string name, string value)
        {
            States.Add(name, value);
        }

        public void AddStateAtomList(List<string> stateAtomList)
        {
            foreach (string stateAtom in stateAtomList)
            {
                int index = stateAtom.LastIndexOf("_", System.StringComparison.Ordinal);
                int length = stateAtom.Length;
                string refiniedStateAtomPartOne = stateAtom.Substring(0, index);
                string refiniedStateAtomPartTwo = stateAtom.Substring(index, length - index);

                for (int i = 0; i < StateAtoms.Count; i++)
                {
                    if (StateAtoms[i].Contains(refiniedStateAtomPartOne))
                    {
                        if (StateAtoms[i].Contains(refiniedStateAtomPartTwo))
                        {
                            continue;
                        }

                        StateAtoms[i] = stateAtom;
                    }
                }
            }
        }


        public HTNState DeduceNextStateBasedOnEffects(List<HTNEffect> effectList)
        {
            HTNState nextState = new HTNState(this);

            foreach (HTNEffect htnEffect in effectList)
            {
                nextState.States[htnEffect.StateAtomName] = htnEffect.StateAtomValue;
            }

            return nextState;
        }


        public HTNState ConjectureOneState(List<string> htnTaskIDList, SpecificationKnowledge wsk)
        {
            //List<string> postConditionAtoms = new List<string>();

            List<HTNTask> htnTaskList = new List<HTNTask>();

            foreach (string taskID in htnTaskIDList)
            {
                htnTaskList.Add(wsk.GetHTNTask(taskID));
            }

            // TODO: WHAT IS THIS SUPPOSED TO BE. FFFFFFF
            //foreach (HTNTask htnTask in htnTaskList)
            //{

            //}

            return new HTNState();
        }

        // TODO: SOLID, FINISHED WORK.
        public List<HTNState> ConjectureAStateChain(List<string> htnTaskIDList)
        {
            return null;
        }

        // TODO: JUST HAD A  BRILLIANT IDEA FOR A METHOD GUYS
        public bool ContainsThisState(HTNState htnState)
        {
            return false;
        }

        public bool IsEqualTo(HTNState antherState)
        {
            if (antherState.StateAtoms.Count > this.StateAtoms.Count
                || this.StateAtoms.Count > antherState.StateAtoms.Count)
            {
                return false;
            }

            foreach (string state in antherState.StateAtoms)
            {
                if (!this.StateAtoms.Contains(state))
                    return false;
            }

            return true;
        }

        public bool HasSameStates(HTNState inputState)
        {
            int lengthOfThis = this.StateAtoms.Count;
            int lengthOfInput = inputState.StateAtoms.Count;

            List<string> baseStateSet;
            List<string> otherStateSet;

            if (lengthOfThis > lengthOfInput)
            {
                baseStateSet = this.StateAtoms;
                otherStateSet = inputState.StateAtoms;
            }
            else
            {
                baseStateSet = inputState.StateAtoms;
                otherStateSet = this.StateAtoms;
            }

            foreach (string state in otherStateSet)
            {
                if (!baseStateSet.Contains(state))
                {
                    return false;
                }
            }
            return true;
        }

    }
}
