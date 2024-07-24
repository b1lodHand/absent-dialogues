using com.absence.dialoguesystem.editor.backup.data;
using com.absence.dialoguesystem.internals;
using System.Linq;
using UnityEngine;

namespace com.absence.dialoguesystem.editor.backup.internals
{
    public static class NodeImportActions
    {
        public static void RootNode(Node node, NodeData data, DialogueImportContext context)
        {
            return;
        }

        public static void DialoguePartNode(Node node, NodeData data, DialogueImportContext context)
        {
            DialoguePartNode dpNode = node as DialoguePartNode;
            dpNode.DialoguePartName = data.DialoguePartName;
            Debug.Log("aye!");
        }

        public static void DecisionSpeechNode(Node node, NodeData data, DialogueImportContext context)
        {
            return;
        }

        public static void FastSpeechNode(Node node, NodeData data, DialogueImportContext context)
        {
            return;
        }

        public static void ActionNode(Node node, NodeData data, DialogueImportContext context)
        {
            ActionNode actionNode = node as ActionNode;
            actionNode.VBActions = data.SetterDatas.ToList().ConvertAll(setterData => DialogueImporter.ReadSetterData(setterData)).ToList();
        }

        public static void ConditionNode(Node node, NodeData data, DialogueImportContext context)
        {
            ConditionNode conditionNode = node as ConditionNode;
            conditionNode.Processor = DialogueImportSettings.ProcessorDictionary[data.ComparerProcessorType];
            conditionNode.Comparers = data.ComparerDatas.ToList().ConvertAll(comparerData => DialogueImporter.ReadComparerData(comparerData)).ToList();
        }

        public static void GotoNode(Node node, NodeData data, DialogueImportContext context)
        {
            GotoNode gotoNode = node as GotoNode;
            gotoNode.TargetNode = context.OldGuidPairs[data.GotoTargetGuid] as DialoguePartNode;
        }

        public static void TitleNode(Node node, NodeData data, DialogueImportContext context)
        {
            return;
        }

        public static void StickyNoteNode(Node node, NodeData data, DialogueImportContext context)
        {
            return;
        }
    }
}
