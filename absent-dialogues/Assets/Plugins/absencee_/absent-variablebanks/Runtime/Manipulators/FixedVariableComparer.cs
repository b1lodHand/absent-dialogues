using com.absence.variablebanks.internals;

namespace com.absence.variablebanks
{
    /// <summary>
    /// Comparer with a fixed bank.
    /// </summary>
    [System.Serializable]
    public sealed class FixedVariableComparer : BaseVariableComparer
    {
        public override bool HasFixedBank => true;

        /// <summary>
        /// Use to set the fixed bank of this fixed comparer.
        /// </summary>
        /// <param name="fixedBankGuid">Guid for the fixed bank.</param>
        public void SetFixedBank(string fixedBankGuid)
        {
            if (!HasFixedBank) return;

            m_targetBankGuid = fixedBankGuid;
        }

        /// <summary>
        /// Use to clone this comparer.
        /// </summary>
        /// <param name="overrideBankGuid">Guid for a new bank.</param>
        /// <returns>The clone.</returns>
        public FixedVariableComparer Clone(string overrideBankGuid)
        {
            FixedVariableComparer clone = new();

            clone.m_boolValue = m_boolValue;
            clone.m_floatValue = m_floatValue;
            clone.m_intValue = m_intValue;
            clone.m_stringValue = m_stringValue;

            clone.m_comparisonType = m_comparisonType;
            clone.m_targetVariableName = m_targetVariableName;

            clone.SetFixedBank(overrideBankGuid);

            return clone;
        }

        /// <summary>
        /// Use to clone this comparer.
        /// </summary>
        /// <returns>The clone.</returns>
        public FixedVariableComparer Clone()
        {
            FixedVariableComparer clone = new();

            clone.m_boolValue = m_boolValue;
            clone.m_floatValue = m_floatValue;
            clone.m_intValue = m_intValue;
            clone.m_stringValue = m_stringValue;

            clone.m_comparisonType = m_comparisonType;
            clone.m_targetVariableName = m_targetVariableName;

            clone.SetFixedBank(m_targetBankGuid);

            return clone;
        }
    }
}
