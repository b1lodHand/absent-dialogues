using com.absence.variablesystem.internals;

namespace com.absence.variablesystem
{
    [System.Serializable]
    public sealed class FixedVariableComparer : BaseVariableComparer
    {
        public override bool HasFixedBank => true;

        public void SetFixedBank(string fixedBankGuid)
        {
            if (!HasFixedBank) return;

            m_targetBankGuid = fixedBankGuid;
        }

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
