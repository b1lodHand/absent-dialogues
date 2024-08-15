namespace com.absence.dialoguesystem.runtime.backup.data
{
    [System.Serializable]
    public class DialogueData 
    {
        public string DefaultDialogueName;
        public NodeData[] NodeDatas;
        public NodeConnectionData[] ConnectionDatas;
        public BlackboardData BlackboardData;
    }
}
