using com.absence.variablesystem.banksystembase;
using System.Collections.Generic;
using System.Linq;

namespace com.absence.dialoguesystem.internals.backup
{
    public static class DialogueExportSettings
    {
        public static readonly Dictionary<VariableComparerBase.ComparisonType, char> ComparerDictionary = DialogueImportSettings.ComparerDictionary.ToDictionary((i) => i.Value, (i) => i.Key);

        public static readonly Dictionary<VariableSetterBase.SetType, char> SetterDictionary = DialogueImportSettings.SetterDictionary.ToDictionary((i) => i.Value, (i) => i.Key);

        public static readonly Dictionary<ConditionProcessMode, char> ProcessorDictionary = DialogueImportSettings.ProcessorDictionary.ToDictionary((i) => i.Value, (i) => i.Key);
    }

}