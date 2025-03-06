namespace com.absence.dialoguesystem.internals.backup.data
{
    [System.Serializable]
    public class NodeData
    {
        public string NodeTypeName;
        public float PositionX;
        public float PositionY;
        public string OldGuid;

        public NodeVariableComparerData[] ComparerData;
        public NodeVariableSetterData[] SetterData;

        public char ComparerProcessorType;

        public int PersonIndex;
        public bool HasText;
        public bool HasOptions;
        public bool HasGenericOptions;
        public bool HasComparers;
        public bool HasSetters;

        public int[] IntData;
        public float[] FloatData;
        public string[] StringData;
        public bool[] BoolData;

        public string Text;
        public NodeCustomDataBase CustomData;
        public OptionData[] OptionData;
        public GenericOptionReferenceData[] GenericOptionReferenceData;
    }
}