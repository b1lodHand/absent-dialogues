using com.absence.variablebanks.internals;

namespace com.absence.variablebanks
{
    /// <summary>
    /// Setter with a dynamic bank you select in the editor.
    /// </summary>
    [System.Serializable]
    public sealed class VariableSetter : BaseVariableSetter
    {
        public override bool HasFixedBank => false;

        /// <summary>
        /// Set this setter's target bank Guid.
        /// </summary>
        /// <param name="newBankGuid">New Guid.</param>
        public void SetBankGuid(string newBankGuid)
        {
            m_targetBankGuid = newBankGuid;
        }

        /// <summary>
        /// Use to clone this setter.
        /// </summary>
        /// <param name="overrideBankGuid">Guid for a new bank.</param>
        /// <returns>The clone.</returns>
        public VariableSetter Clone(string overrideBankGuid)
        {
            VariableSetter clone = new();

            clone.m_boolValue = m_boolValue;
            clone.m_floatValue = m_floatValue;
            clone.m_intValue = m_intValue;
            clone.m_stringValue = m_stringValue;

            clone.m_setType = m_setType;
            clone.m_targetVariableName = m_targetVariableName;

            clone.SetBankGuid(overrideBankGuid);

            return clone;
        }

        /// <summary>
        /// Use to clone this setter.
        /// </summary>
        /// <returns>The clone.</returns>
        public VariableSetter Clone()
        {
            VariableSetter clone = new();

            clone.m_boolValue = m_boolValue;
            clone.m_floatValue = m_floatValue;
            clone.m_intValue = m_intValue;
            clone.m_stringValue = m_stringValue;

            clone.m_setType = m_setType;
            clone.m_targetVariableName = m_targetVariableName;

            clone.SetBankGuid(m_targetBankGuid);

            return clone;
        }
    }

}