namespace com.absence.dialoguesystem.runtime.backup.data
{
    [System.Serializable]
    public class OptionData
    {
        public string Speech;
        public bool ShowIfInUse;
        public char ProcessorType;
        public NodeVariableComparerData[] ShowIfData;
    }
}
