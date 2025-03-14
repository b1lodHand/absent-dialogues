using com.absence.variablesystem.banksystembase;
using System;
using System.Text;
using UnityEngine;

namespace com.absence.dialoguesystem.internals
{
    /// <summary>
    /// The setter specifically designed for working with dialogue nodes.
    /// </summary>
    [System.Serializable]
    public class NodeVariableSetter : VariableSetterBase
    {
        /// <summary>
        /// Bank of the blackboard in context.
        /// </summary>
        public VariableBank BlackboardBank { get; set; }

        public override bool HasFixedBank => true;

        protected override IPrimitiveVariableContainer GetRuntimeBank() => BlackboardBank;

        /// <summary>
        /// Use to set the blackboard bank of this setter.
        /// </summary>
        /// <param name="originalBlackboardBank">Target bank.</param>
        public void SetBlackboardBank(VariableBank originalBlackboardBank)
        {
            BlackboardBank = originalBlackboardBank;
            m_targetBankGuid = BlackboardBank.Guid;
        }

        public string GetSettingString(bool richText = false)
        {
            if (BlackboardBank == null) return string.Empty;
            if (!BlackboardBank.HasAny(m_targetVariableName)) return string.Empty;

            string realVarName = TrimVariableName(m_targetVariableName);
            string richVarName = Utilities.Text.ColorizeString(realVarName, Constants.Tooltips.VARIABLE_NAME_HEX);
            string bracketHex = TypeOfSet == SetType.SetTo ?
                Constants.Tooltips.AND_HEX : Constants.Tooltips.OR_HEX;

            StringBuilder sb = new();
            sb.Append(richText ? Utilities.Text.ColorizeString("[", bracketHex) : "[");
            sb.Append(richText ? richVarName : realVarName);
            sb.Append(" ");
            sb.Append(richText ? Utilities.Text.ColorizeString(Utilities.Setting.GetSetTypeIcon(m_setType), bracketHex) : Utilities.Setting.GetSetTypeIcon(m_setType));
            sb.Append(" ");

            if (richText) sb.Append($"<color={Constants.Tooltips.VARIABLE_NAME_HEX}>");
            if (BlackboardBank.HasInt(m_targetVariableName)) sb.Append(IntValue);
            else if (BlackboardBank.HasBoolean(m_targetVariableName)) sb.Append(BooleanValue);
            else if (BlackboardBank.HasFloat(m_targetVariableName)) sb.Append(FloatValue);
            else if (BlackboardBank.HasString(m_targetVariableName)) sb.Append($"{StringValue}");
            if (richText) sb.Append("</color>");

            sb.Append(richText ? Utilities.Text.ColorizeString("]", bracketHex) : "]");

            return sb.ToString();
        }

        string TrimVariableName(string nameToTrim)
        {
            if (!nameToTrim.Contains(':')) return nameToTrim;
            return nameToTrim.Split(':')[1].Trim();
        }

        /// <summary>
        /// Use to copy this setter.
        /// </summary>
        /// <param name="clonedBlackboardBank">Cloned blackboard bank.</param>
        /// <returns>The clone.</returns>
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