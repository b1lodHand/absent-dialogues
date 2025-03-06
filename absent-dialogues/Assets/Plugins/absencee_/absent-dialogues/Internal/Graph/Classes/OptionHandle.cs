namespace com.absence.dialoguesystem.internals
{
    [System.Serializable]
    public class OptionHandle 
    {
        public int TargetedIndex { get; private set; }
        public string Text { get; private set; }
        public NodeCustomDataBase CustomData { get; private set; }

        public OptionHandle(int targetIndex, string content, NodeCustomDataBase customData)
        {
            TargetedIndex = targetIndex;
            Text = content;
            CustomData = customData;
        }
    }
}