using com.absence.variablesystem;
using com.absence.variablesystem.internals;

namespace com.absence.dialoguesystem.internals
{
    [System.Serializable]
    public class NodeVariableSetter : BaseVariableSetter
    {
        public VariableBank BlackboardBank { get; set; }

        public override bool HasFixedBank => throw new System.NotImplementedException();

        public override VariableBank GetRuntimeBank() => BlackboardBank;

        public NodeVariableSetter Clone(VariableBank clonedVariableBank)
        {
            NodeVariableSetter clone = new();

            clone.m_boolValue = m_boolValue;
            clone.m_floatValue = m_floatValue;
            clone.m_intValue = m_intValue;
            clone.m_stringValue = m_stringValue;

            clone.m_setType = m_setType;
            clone.m_targetVariableName = m_targetVariableName;

            clone.BlackboardBank = clonedVariableBank;

            return clone;
        }
    }

}