namespace com.absence.dialoguesystem.editor.backup.data
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
