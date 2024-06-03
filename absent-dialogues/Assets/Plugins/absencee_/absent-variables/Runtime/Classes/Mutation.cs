namespace com.absence.variablesystem
{
    public class Mutation<T>
    {
        public MutationType MutationType { get; private set; }
        public T MutationValue { get; private set; }

        public Mutation(T mutationValue, MutationType mutationType)
        {
            MutationValue = mutationValue;
            MutationType = mutationType;
        }
    }

    public enum MutationType
    {
        Additive = 0,
        Multiplicative = 1,
    }
}
