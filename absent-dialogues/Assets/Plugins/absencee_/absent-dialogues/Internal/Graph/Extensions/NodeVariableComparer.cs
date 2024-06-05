using com.absence.variablesystem;
using UnityEngine;

namespace com.absence.dialoguesystem.internals
{
    /// <summary>
    /// The comparer specifically designed for working with dialogue nodes.
    /// </summary>
    [System.Serializable]
    public class NodeVariableComparer : BaseVariableComparer
    {
        /// <summary>
        /// Bank of the blackboard in context.
        /// </summary>
        public VariableBank BlackboardBank { get; set; }

        public override bool HasFixedBank => true;

        protected override VariableBank GetRuntimeBank() => BlackboardBank;

        /// <summary>
        /// Use to set the blackboard bank of this comparer.
        /// </summary>
        /// <param name="originalBlackboardBank">Target bank.</param>
        public void SetBlackboardBank(VariableBank originalBlackboardBank)
        {
            if (Application.isPlaying) return;

            BlackboardBank = originalBlackboardBank;
            m_targetBankGuid = BlackboardBank.Guid;
        }

        /// <summary>
        /// Use to copy this comparer.
        /// </summary>
        /// <param name="clonedBlackboardBank">Cloned blackboard bank.</param>
        /// <returns>The clone.</returns>
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