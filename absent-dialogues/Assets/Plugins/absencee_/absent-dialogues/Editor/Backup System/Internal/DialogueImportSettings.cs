using System;
using com.absence.dialoguesystem.internals;
using System.Collections.Generic;
using com.absence.variablesystem;
using com.absence.dialoguesystem.editor.backup.data;

namespace com.absence.dialoguesystem.editor.backup.internals
{
    public static class DialogueImportSettings
    {
        public static readonly Dictionary<char, Type> NodeTypeDictionary = new Dictionary<char, Type>()
        {
            { 'r', typeof(RootNode) },
            { 'p', typeof(DialoguePartNode) },
            { 'd', typeof(DecisionSpeechNode) },
            { 'f', typeof(FastSpeechNode) },
            { 'g', typeof(GotoNode) },
            { 'a', typeof(ActionNode) },
            { 'c', typeof(ConditionNode) },
            { 't', typeof(TitleNode) },
            { 's', typeof(StickyNoteNode) },
        };

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

        public static readonly Dictionary<Type, Action<Node, NodeData, DialogueImportContext>> NodeImportActionDictionary = new Dictionary<Type, Action<Node, NodeData, DialogueImportContext>>()
        {
            { typeof(RootNode), NodeImportActions.RootNode },
            { typeof(DialoguePartNode), NodeImportActions.DialoguePartNode },
            { typeof(DecisionSpeechNode), NodeImportActions.DecisionSpeechNode },
            { typeof(FastSpeechNode), NodeImportActions.FastSpeechNode },
            { typeof(GotoNode), NodeImportActions.GotoNode },
            { typeof(ActionNode), NodeImportActions.ActionNode },
            { typeof(ConditionNode), NodeImportActions.ConditionNode },
            { typeof(TitleNode), NodeImportActions.TitleNode },
            { typeof(StickyNoteNode), NodeImportActions.StickyNoteNode },
        };
    }
}
