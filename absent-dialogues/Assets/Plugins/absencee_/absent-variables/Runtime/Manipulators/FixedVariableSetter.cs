using com.absence.variablesystem.internals;

namespace com.absence.variablesystem
{
    [System.Serializable]
    public sealed class FixedVariableSetter : BaseVariableSetter
    {
        public override bool HasFixedBank => true;

        public void SetFixedBank(string fixedBankGuid)
        {
            if (!HasFixedBank) return;

            m_targetBankGuid = fixedBankGuid;
        }

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
