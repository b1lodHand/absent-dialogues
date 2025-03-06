namespace com.absence.dialoguesystem.internals.backup.data
{
    [System.Serializable]
    public class BlackboardData
    {
        public IntPair[] Ints;
        public FloatPair[] Floats;
        public StringPair[] Strings;
        public BooleanPair[] Booleans;
    }

    [System.Serializable]
    public class IntPair
    {
        public string Key;
        public int Value;

        public static implicit operator IntPair(int source)
        {
            return new IntPair() { Value = source };
        }
    }

    [System.Serializable]
    public class FloatPair
    {
        public string Key;
        public float Value;

        public static implicit operator FloatPair(float source)
        {
            return new FloatPair() { Value = source };
        }
    }

    [System.Serializable]
    public class StringPair
    {
        public string Key;
        public string Value;

        public static implicit operator StringPair(string source)
        {
            return new StringPair() { Value = source };
        }
    }

    [System.Serializable]
    public class BooleanPair
    {
        public string Key;
        public bool Value;

        public static implicit operator BooleanPair(bool source)
        {
            return new BooleanPair() { Value = source };
        }
    }
}
