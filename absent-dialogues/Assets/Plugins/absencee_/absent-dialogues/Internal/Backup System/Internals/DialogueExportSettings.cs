using com.absence.dialoguesystem.internals;
using com.absence.variablesystem;
using System;
using System.Collections.Generic;
using System.Linq;

namespace com.absence.dialoguesystem.runtime.backup.internals
{
    public static class DialogueExportSettings
    {
        public static readonly Dictionary<BaseVariableComparer.ComparisonType, char> ComparerDictionary = DialogueImportSettings.ComparerDictionary.ToDictionary((i) => i.Value, (i) => i.Key);

        public static readonly Dictionary<BaseVariableSetter.SetType, char> SetterDictionary = DialogueImportSettings.SetterDictionary.ToDictionary((i) => i.Value, (i) => i.Key);

        public static readonly Dictionary<VBProcessType, char> ProcessorDictionary = DialogueImportSettings.ProcessorDictionary.ToDictionary((i) => i.Value, (i) => i.Key);
    }

}