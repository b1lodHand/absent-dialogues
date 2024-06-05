using com.absence.variablebanks.internals;

namespace com.absence.variablebanks
{
    /// <summary>
    /// Setter with a fixed bank.
    /// </summary>
    [System.Serializable]
    public sealed class FixedVariableSetter : BaseVariableSetter
    {
        public override bool HasFixedBank => true;

        /// <summary>
        /// Use to set the fixed bank of this fixed setter.
        /// </summary>
        /// <param name="fixedBankGuid">Guid for the fixed bank.</param>
        public void SetFixedBank(string fixedBankGuid)
        {
            if (!HasFixedBank) return;

            m_targetBankGuid = fixedBankGuid;
        }

        /// <summary>
        /// Use to clone this setter.
        /// </summary>
        /// <param name="overrideBankGuid">Guid for a new bank.</param>
        /// <returns>The clone.</returns>
        public FixedVariableSetter Clone(string overrideBankGuid)
        {
            FixedVariableSetter clone = new();

            clone.m_boolValue = m_boolValue;
            clone.m_floatValue = m_floatValue;
            clone.m_intValue = m_intValue;
            clone.m_stringValue = m_stringValue;

            clone.m_setType = m_setType;
            clone.m_targetVariableName = m_targetVariableName;

            clone.SetFixedBank(overrideBankGuid);

            return clone;
        }

        /// <summary>
        /// Use to clone this setter.
        /// </summary>
        /// <returns>The clone.</returns>
        public FixedVariableSetter Clone()
        {
            FixedVariableSetter clone = new();

            clone.m_boolValue = m_boolValue;
            clone.m_floatValue = m_floatValue;
            clone.m_intValue = m_intValue;
            clone.m_stringValue = m_stringValue;

            clone.m_setType = m_setType;
            clone.m_targetVariableName = m_targetVariableName;

            clone.SetFixedBank(m_targetBankGuid);

            return clone;
        }
    }
}
