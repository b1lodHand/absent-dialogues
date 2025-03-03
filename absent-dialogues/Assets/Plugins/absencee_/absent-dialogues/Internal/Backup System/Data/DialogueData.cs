namespace com.absence.dialoguesystem.runtime.backup.data
{
    [System.Serializable]
    public class DialogueData 
    {
        public string DefaultDialogueName;
        public NodeData[] NodeData;
        public NodeConnectionData[] ConnectionData;
        public OptionData[] GenericOptionData;
        public BlackboardData BlackboardData;
    }
}
