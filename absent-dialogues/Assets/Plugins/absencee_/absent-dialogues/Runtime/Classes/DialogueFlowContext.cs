using com.absence.dialoguesystem.internals;
using com.absence.variablesystem.banksystembase;
using System.Collections.Generic;

namespace com.absence.dialoguesystem
{
    [System.Serializable]
    public class DialogueFlowContext
    {
        public enum ContextState
        {
            Reach = 0,
            Pass = 1,
        }

        public int SelectedOption;
        public ContextState State;

        public bool InvokeAction;
        public string ActionId;

        public string Text;
        public List<OptionHandle> OptionHandles;
        public NodeCustomDataBase CustomData;
        public NodeCustomDataBase OptionData;

        public IPrimitiveVariableContainer BankHandle;

        public bool WillExit { get; set; }
        public bool HasText => Text != null && (!string.IsNullOrWhiteSpace(Text));
        public bool HasOptions => OptionHandles != null && OptionHandles.Count > 0;

        public DialogueFlowContext()
        {
            OptionHandles = new();
            Clear();
        }

        public void Clear()
        {
            Text = null;
            CustomData = null;
            InvokeAction = false;
            ActionId = string.Empty;
            SelectedOption = -1;
            OptionHandles?.Clear();
        }
    }
}