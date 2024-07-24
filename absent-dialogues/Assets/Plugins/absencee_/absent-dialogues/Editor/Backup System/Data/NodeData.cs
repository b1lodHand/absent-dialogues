namespace com.absence.dialoguesystem.editor.backup.data
{
    [System.Serializable]
    public class NodeData
    {
        public char NodeTypeIndicator;
        public float PositionX;
        public float PositionY;
        public string OldGuid;

        public bool ExitDialogueAfterwards;

        public string GotoTargetGuid;
        public string DialoguePartName;

        public NodeVariableComparerData[] ComparerDatas;
        public NodeVariableSetterData[] SetterDatas;

        public char ComparerProcessorType;

        public string Speech;
        public OptionData[] OptionDatas;
    }
}