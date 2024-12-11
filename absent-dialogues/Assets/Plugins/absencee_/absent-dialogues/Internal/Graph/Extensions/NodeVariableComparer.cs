using com.absence.variablesystem.banksystembase;
using System.Text;
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

        public override string ToString()
        {
            return GetConditionString(false);
        }

        public string GetConditionString(bool richText = false)
        {
            if (BlackboardBank == null) return string.Empty;
            if (!BlackboardBank.HasAny(m_targetVariableName)) return string.Empty;

            string realVarName = TrimVariableName(m_targetVariableName);

            if (BlackboardBank.HasBoolean(m_targetVariableName))
            {
                string boolResult = BooleanValue ? realVarName : $"!{realVarName}";

                if (!richText) return boolResult;
                else return Utilities.Texts.ColorizeString(boolResult, Constants.Tooltips.VARIABLE_NAME_HEX);
            }

            StringBuilder sb = new(realVarName);
            sb.Append(" ");
            sb.Append(Utilities.Comparison.GetComparisonTypeIcon(m_comparisonType));
            sb.Append(" ");

            if (BlackboardBank.HasInt(m_targetVariableName)) sb.Append(IntValue);
            else if (BlackboardBank.HasFloat(m_targetVariableName)) sb.Append(FloatValue);
            else if (BlackboardBank.HasString(m_targetVariableName)) sb.Append($"'{StringValue}'");

            if (!richText) return sb.ToString();
            else return Utilities.Texts.ColorizeString(sb.ToString(), Constants.Tooltips.VARIABLE_NAME_HEX);
        }

        string TrimVariableName(string nameToTrim)
        {
            if (!nameToTrim.Contains(':')) return nameToTrim;
            return nameToTrim.Split(':')[1].Trim();
        }
    }
}