namespace com.absence.dialoguesystem.internals
{
    [System.Serializable]
    public class OptionHandle 
    {
        public int TargetedIndex { get; private set; }
        public string Text { get; private set; }

        public OptionHandle(int targetIndex, string content)
        {
            TargetedIndex = targetIndex;
            Text = content;
        }
    }
}