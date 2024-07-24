using com.absence.dialoguesystem.editor.backup.data;
using com.absence.dialoguesystem.internals;
using com.absence.variablesystem;
using System;
using System.Collections.Generic;
using System.Linq;

namespace com.absence.dialoguesystem.editor.backup.internals
{
    public static class DialogueExportSettings
    {
        public static readonly Dictionary<Type, char> NodeTypeDictionary = DialogueImportSettings.NodeTypeDictionary.ToDictionary((i) => i.Value, (i) => i.Key);

        public static readonly Dictionary<BaseVariableComparer.ComparisonType, char> ComparerDictionary = DialogueImportSettings.ComparerDictionary.ToDictionary((i) => i.Value, (i) => i.Key);

        public static readonly Dictionary<BaseVariableSetter.SetType, char> SetterDictionary = DialogueImportSettings.SetterDictionary.ToDictionary((i) => i.Value, (i) => i.Key);

        public static readonly Dictionary<VBProcessType, char> ProcessorDictionary = DialogueImportSettings.ProcessorDictionary.ToDictionary((i) => i.Value, (i) => i.Key);

        public static readonly Dictionary<Type, Action<Node, NodeData>> NodeExportActionDictionary = new Dictionary<Type, Action<Node, NodeData>>()
        {
            { typeof(RootNode), NodeExportActions.RootNode },
            { typeof(DialoguePartNode), NodeExportActions.DialoguePartNode },
            { typeof(DecisionSpeechNode), NodeExportActions.DecisionSpeechNode },
            { typeof(FastSpeechNode), NodeExportActions.FastSpeechNode },
            { typeof(GotoNode), NodeExportActions.GotoNode },
            { typeof(ActionNode), NodeExportActions.ActionNode },
            { typeof(ConditionNode), NodeExportActions.ConditionNode },
            { typeof(TitleNode), NodeExportActions.TitleNode },
            { typeof(StickyNoteNode), NodeExportActions.StickyNoteNode },
        };
    }

}