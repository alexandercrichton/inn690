namespace Veis.Planning.HTN
{
    public class HTNEffect
    {
        public string StateAtomName { get; set; }
        public string StateAtomValue { get; set; }

        public HTNEffect(HTNEffect htnEffect)
        {
            StateAtomName = htnEffect.StateAtomName;
            StateAtomValue = htnEffect.StateAtomValue;
        }

        public HTNEffect()
        {
        }
    }
}
