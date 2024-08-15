using com.absence.dialoguesystem.internals;
using System.Collections.Generic;
using com.absence.variablesystem;

namespace com.absence.dialoguesystem.runtime.backup.internals
{
    public static class DialogueImportSettings
    {
        public static readonly Dictionary<char, BaseVariableComparer.ComparisonType> ComparerDictionary = new Dictionary<char, BaseVariableComparer.ComparisonType>()
        {
            { 'L', BaseVariableComparer.ComparisonType.LessThan },
            { 'l', BaseVariableComparer.ComparisonType.LessOrEqual },
            { 'e', BaseVariableComparer.ComparisonType.EqualsTo },
            { 'n', BaseVariableComparer.ComparisonType.NotEquals },
            { 'g', BaseVariableComparer.ComparisonType.GreaterOrEqual },
            { 'G', BaseVariableComparer.ComparisonType.GreaterThan },
        };

        public static readonly Dictionary<char, BaseVariableSetter.SetType> SetterDictionary = new Dictionary<char, BaseVariableSetter.SetType>()
        {
            { 's', BaseVariableSetter.SetType.SetTo },
            { 'i', BaseVariableSetter.SetType.IncrementBy },
            { 'l', BaseVariableSetter.SetType.DecrementBy },
            { 'm', BaseVariableSetter.SetType.MultipltyBy },
            { 'd', BaseVariableSetter.SetType.DivideBy },
        };

        public static readonly Dictionary<char, VBProcessType> ProcessorDictionary = new Dictionary<char, VBProcessType>()
        {
            { 'A', VBProcessType.All },
            { 'V', VBProcessType.Any },
        };
    }
}
