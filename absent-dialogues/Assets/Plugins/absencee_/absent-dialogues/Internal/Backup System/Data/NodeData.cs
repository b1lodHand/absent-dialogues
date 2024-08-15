namespace com.absence.dialoguesystem.runtime.backup.data
{
    [System.Serializable]
    public class NodeData
    {
        public string NodeTypeName;
        public float PositionX;
        public float PositionY;
        public string OldGuid;

        public string GotoTargetGuid;
        public string DialoguePartName;

        public NodeVariableComparerData[] ComparerDatas;
        public NodeVariableSetterData[] SetterDatas;

        public char ComparerProcessorType;

        public string Text;
        public OptionData[] OptionDatas;
    }
}