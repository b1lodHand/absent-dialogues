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

        public string Text;
        public List<OptionHandle> OptionIndexPairs;
        public ExtraDialogueData ExtraData;

        public bool WillExit { get; set; }
        public bool HasText => Text != null;
        public bool HasOptions => OptionIndexPairs.Count > 0;

        public DialogueFlowContext()
        {
            WillExit = false;
            OptionIndex = -1;
            State = ContextState.Reach;

            OptionIndexPairs = new();
            ClearSpeech();
        }

        public void ClearSpeech()
        {
            Text = null;
            ExtraData = null;
            OptionIndexPairs.Clear();
        }
    }
}