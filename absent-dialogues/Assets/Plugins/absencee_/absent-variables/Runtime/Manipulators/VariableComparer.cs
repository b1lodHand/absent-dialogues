using com.absence.variablesystem.internals;

namespace com.absence.variablesystem
{
    [System.Serializable]
    public sealed class VariableComparer : BaseVariableComparer
    {
        public override bool HasFixedBank => false;

        public VariableComparer Clone(string overrideBankGuid)
        {
            VariableComparer clone = new();
            
            clone.m_boolValue = m_boolValue;
            clone.m_floatValue = m_floatValue;
            clone.m_intValue = m_intValue;
            clone.m_stringValue = m_stringValue;

            clone.m_comparisonType = m_comparisonType;
            clone.m_targetVariableName = m_targetVariableName;

            clone.m_targetBankGuid = overrideBankGuid;

            return clone;
        }
        public VariableComparer Clone()
        {
            VariableComparer clone = new();

            clone.m_boolValue = m_boolValue;
            clone.m_floatValue = m_floatValue;
            clone.m_intValue = m_intValue;
            clone.m_stringValue = m_stringValue;

            clone.m_comparisonType = m_comparisonType;
            clone.m_targetVariableName = m_targetVariableName;

            clone.m_targetBankGuid = m_targetBankGuid;

            return clone;
        }
    }
}