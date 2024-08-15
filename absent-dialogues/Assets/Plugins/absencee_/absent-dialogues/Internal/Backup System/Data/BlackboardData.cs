namespace com.absence.dialoguesystem.runtime.backup.data
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
    }

    [System.Serializable]
    public class FloatPair
    {
        public string Key;
        public float Value;
    }

    [System.Serializable]
    public class StringPair
    {
        public string Key;
        public string Value;
    }

    [System.Serializable]
    public class BooleanPair
    {
        public string Key;
        public bool Value;
    }
}
