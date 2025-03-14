using com.absence.dialoguesystem.internals;
using System.Collections.Generic;
using com.absence.variablesystem.banksystembase;

namespace com.absence.dialoguesystem.internals.backup
{
    public static class DialogueImportSettings
    {
        public static readonly Dictionary<char, VariableComparerBase.ComparisonType> ComparerDictionary = new()
        {
            { 'L', VariableComparerBase.ComparisonType.LessThan },
            { 'l', VariableComparerBase.ComparisonType.LessOrEqual },
            { 'e', VariableComparerBase.ComparisonType.EqualsTo },
            { 'n', VariableComparerBase.ComparisonType.NotEquals },
            { 'g', VariableComparerBase.ComparisonType.GreaterOrEqual },
            { 'G', VariableComparerBase.ComparisonType.GreaterThan },
        };

        public static readonly Dictionary<char, VariableSetterBase.SetType> SetterDictionary = new()
        {
            { 's', VariableSetterBase.SetType.SetTo },
            { 'i', VariableSetterBase.SetType.IncrementBy },
            { 'l', VariableSetterBase.SetType.DecrementBy },
            { 'm', VariableSetterBase.SetType.MultipltyBy },
            { 'd', VariableSetterBase.SetType.DivideBy },
        };

        public static readonly Dictionary<char, ConditionProcessMode> ProcessorDictionary = new()
        {
            { 'A', ConditionProcessMode.All },
            { 'V', ConditionProcessMode.Any },
        };
    }
}
