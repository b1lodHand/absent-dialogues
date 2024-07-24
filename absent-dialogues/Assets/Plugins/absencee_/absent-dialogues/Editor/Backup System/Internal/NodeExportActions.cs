using com.absence.dialoguesystem.editor.backup.data;
using com.absence.dialoguesystem.internals;

namespace com.absence.dialoguesystem.editor.backup.internals
{
    public static class NodeExportActions
    {
        public static void RootNode(Node node, NodeData data)
        {
            return;
        }

        public static void DialoguePartNode(Node node, NodeData data)
        {
            DialoguePartNode dpNode = node as DialoguePartNode;
            data.DialoguePartName = dpNode.DialoguePartName;
        }

        public static void DecisionSpeechNode(Node node, NodeData data)
        {
            return;
        }

        public static void FastSpeechNode(Node node, NodeData data)
        {
            return;
        }

        public static void ActionNode(Node node, NodeData data)
        {
            ActionNode actionNode = node as ActionNode;
            data.SetterDatas = actionNode.VBActions.ConvertAll(setter => DialogueExporter.GenerateSetterData(setter)).ToArray();
        }

        public static void ConditionNode(Node node, NodeData data)
        {
            ConditionNode conditionNode = node as ConditionNode;
            data.ComparerProcessorType = DialogueExportSettings.ProcessorDictionary[conditionNode.Processor];
            data.ComparerDatas = conditionNode.Comparers.ConvertAll(comparer => DialogueExporter.GenerateComparerData(comparer)).ToArray();
        }

        public static void GotoNode(Node node, NodeData data)
        {
            GotoNode gotoNode = node as GotoNode;
            data.GotoTargetGuid = gotoNode.TargetNode.Guid;
        }

        public static void TitleNode(Node node, NodeData data)
        {
            return;
        }

        public static void StickyNoteNode(Node node, NodeData data)
        {
            return;
        }
    }
}
