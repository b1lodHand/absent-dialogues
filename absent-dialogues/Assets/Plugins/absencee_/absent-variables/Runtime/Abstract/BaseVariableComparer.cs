using System;
using UnityEngine;

namespace com.absence.variablesystem.internals
{
    [System.Serializable]
    public abstract class BaseVariableComparer
    {
        public enum ComparisonType
        {
            [InspectorName("<")] LessThan = 0,
            [InspectorName("≤")] LessOrEqual = 1,
            [InspectorName("=")] EqualsTo = 2,
            [InspectorName("≠")] NotEquals = 3,
            [InspectorName("≥")] GreaterOrEqual = 4,
            [InspectorName(">")] GreaterThan = 5,
        }

        [SerializeField] protected ComparisonType m_comparisonType = ComparisonType.EqualsTo;
        [SerializeField] public string m_targetBankGuid;
        [SerializeField] protected string m_targetVariableName = VariableBank.Null;

        [SerializeField] protected int m_intValue;
        [SerializeField] protected float m_floatValue;
        [SerializeField] protected string m_stringValue;
        [SerializeField] protected bool m_boolValue;

        public abstract bool HasFixedBank { get; }

        public virtual VariableBank GetRuntimeBank() => VariableBank.GetClone(m_targetBankGuid);

        public virtual bool GetResult()
        {
            if (!Application.isPlaying) throw new Exception("You cannot call GetResult() on comparers outside play mode!");

            VariableBank bank = GetRuntimeBank();

            var result = true;

            if (bank == null) return result;
            if (m_targetVariableName == VariableBank.Null) return result;

            if (bank.TryGetString(m_targetVariableName, out string stringValue)) result = (stringValue == m_stringValue);
            else if (bank.TryGetBoolean(m_targetVariableName, out bool boolValue)) result = (boolValue == m_boolValue);
            else if (bank.TryGetInt(m_targetVariableName, out int intValue)) result = CompareNumerics(intValue, m_intValue);
            else if (bank.TryGetFloat(m_targetVariableName, out float floatValue)) result = CompareNumerics(floatValue, m_floatValue);

            return result;
        }

        bool CompareNumerics(int a, int b)
        {
            switch (m_comparisonType)
            {
                case ComparisonType.LessThan:
                    return a < b;
                case ComparisonType.LessOrEqual:
                    return a <= b;
                case ComparisonType.EqualsTo:
                    return a == b;
                case ComparisonType.GreaterOrEqual:
                    return a >= b;
                case ComparisonType.GreaterThan:
                    return a > b;
                default:
                    return false;
            }
        }
        bool CompareNumerics(float a, float b)
        {
            switch (m_comparisonType)
            {
                case ComparisonType.LessThan:
                    return a < b;
                case ComparisonType.LessOrEqual:
                    return a <= b;
                case ComparisonType.EqualsTo:
                    return a == b;
                case ComparisonType.GreaterOrEqual:
                    return a >= b;
                case ComparisonType.GreaterThan:
                    return a > b;
                default:
                    return false;
            }
        }
    }
}
