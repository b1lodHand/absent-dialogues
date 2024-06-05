using com.absence.variablebanks.internals;

namespace com.absence.variablebanks
{
    /// <summary>
    /// Comparer with a dynamic bank you select in editor.
    /// </summary>
    [System.Serializable]
    public sealed class VariableComparer : BaseVariableComparer
    {
        public override bool HasFixedBank => false;

        /// <summary>
        /// Set this comparer's target bank Guid.
        /// </summary>
        /// <param name="newBankGuid">New Guid.</param>
        public void SetBankGuid(string newBankGuid)
        {
            m_targetBankGuid = newBankGuid;
        }

        /// <summary>
        /// Use to clone this comparer.
        /// </summary>
        /// <param name="overrideBankGuid">Guid for a new bank.</param>
        /// <returns>The clone.</returns>
        public VariableComparer Clone(string overrideBankGuid)
        {
            VariableComparer clone = new();
            
            clone.m_boolValue = m_boolValue;
            clone.m_floatValue = m_floatValue;
            clone.m_intValue = m_intValue;
            clone.m_stringValue = m_stringValue;

            clone.m_comparisonType = m_comparisonType;
            clone.m_targetVariableName = m_targetVariableName;

            clone.SetBankGuid(overrideBankGuid);

            return clone;
        }

        /// <summary>
        /// Use to clone this comparer.
        /// </summary>
        /// <returns>The clone.</returns>
        public VariableComparer Clone()
        {
            VariableComparer clone = new();

            clone.m_boolValue = m_boolValue;
            clone.m_floatValue = m_floatValue;
            clone.m_intValue = m_intValue;
            clone.m_stringValue = m_stringValue;

            clone.m_comparisonType = m_comparisonType;
            clone.m_targetVariableName = m_targetVariableName;

            clone.SetBankGuid(m_targetBankGuid);

            return clone;
        }
    }
}