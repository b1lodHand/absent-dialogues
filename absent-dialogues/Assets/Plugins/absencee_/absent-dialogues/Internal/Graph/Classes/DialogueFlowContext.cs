using System.Collections.Generic;

namespace com.absence.dialoguesystem.internals
{
    [System.Serializable]
    public class DialogueFlowContext
    {
        public enum ContextState
        {
            Reach = 0,
            Pass = 1,
        }

        public int OptionIndex;
        public ContextState State;

        public bool InvokeAction;
        public string ActionId;

        public string Text;
        public List<OptionHandle> OptionIndexPairs;
        public NodeCustomDataBase CustomData;

        public bool WillExit { get; set; }
        public bool HasText => Text != null;
        public bool HasOptions => OptionIndexPairs != null && OptionIndexPairs.Count > 0;

        public DialogueFlowContext()
        {
            WillExit = false;
            OptionIndex = -1;
            ActionId = string.Empty;
            InvokeAction = false;
            State = ContextState.Pass;

            OptionIndexPairs = new();
            ClearText();
        }

        public void ClearText()
        {
            Text = null;
            CustomData = null;
            OptionIndexPairs.Clear();
        }
    }
}