using com.absence.variablesystem;
using com.absence.variablesystem.internals;
using UnityEngine;

namespace com.absence.dialoguesystem.internals
{
    [System.Serializable]
    public class NodeVariableComparer : BaseVariableComparer
    {
        public VariableBank BlackboardBank { get; set; }

        public override bool HasFixedBank => true;

        public override VariableBank GetRuntimeBank() => BlackboardBank;

        public void SetBlackboardBank(VariableBank originalBlackboardBank)
        {
            if (Application.isPlaying) return;

            BlackboardBank = originalBlackboardBank;
            m_targetBankGuid = BlackboardBank.GUID;
        }

        public NodeVariableComparer Clone(VariableBank clonedBlackboardBank)
        {
            NodeVariableComparer clone = new();

            clone.m_boolValue = m_boolValue;
            clone.m_floatValue = m_floatValue;
            clone.m_intValue = m_intValue;
            clone.m_stringValue = m_stringValue;

            clone.m_comparisonType = m_comparisonType;
            clone.m_targetVariableName = m_targetVariableName;

            clone.BlackboardBank = clonedBlackboardBank;

            return clone;
        }
    }
}