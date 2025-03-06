namespace com.absence.dialoguesystem.internals.backup.data
{
    [System.Serializable]
    public class OptionData
    {
        public string Text;
        public bool ShowIfInUse;
        public char ProcessorType;
        public string OldLeadingNodeGuid;
        public NodeVariableComparerData[] ShowIfData;
    }
}
