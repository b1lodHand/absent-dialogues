using com.absence.dialoguesystem.internals;
using com.absence.dialoguesystem.internals.backup.data;
using System.Collections.Generic;

namespace com.absence.dialoguesystem.editor.internals.backup
{
    public static class DialogueExporter
    {
        public static DialogueData Export(Dialogue dialogue)
        {
            DialogueData data = new();
            data.DefaultDialogueName = dialogue.name;
            WriteNodeList(data, dialogue);
            WriteGenericOptions(data, dialogue);
            CopyConnections(data, dialogue);

            data.BlackboardData = DataGenerator.GenerateBlackboardData(dialogue.Blackboard);

            return data;
        }

        private static void WriteGenericOptions(DialogueData target, Dialogue dialogue)
        {
            int optionCount = dialogue.GenericOptions.Count;

            target.GenericOptionData = new OptionData[optionCount];
            for (int i = 0; i < dialogue.GenericOptions.Count; i++)
            {
                target.GenericOptionData[i] = DataGenerator.GenerateOptionData(dialogue.GenericOptions[i]);
            }
        }

        static void WriteNodeList(DialogueData target, Dialogue dialogue)
        {
            int nodeCount = dialogue.AllNodes.Count;

            target.NodeData = new NodeData[nodeCount];
            for (int i = 0; i < nodeCount; i++)
            {
                target.NodeData[i] = DataGenerator.GenerateNodeData(dialogue.AllNodes[i]);
            }
        }
        static void CopyConnections(DialogueData target, Dialogue dialogue)
        {
            List<NodeConnectionData> dynamicData = new();
            dialogue.AllNodes.ForEach(node =>
            {
                List<Node> rightSideNodes = node.GetOutputConnections();
                for (int i = 0; i < rightSideNodes.Count; i++)
                {
                    Node rightSideTarget = rightSideNodes[i];

                    if (rightSideTarget == null)
                        continue;

                    NodeConnectionData newConnectionData = new();
                    newConnectionData.FromPortIndex = i;
                    newConnectionData.FromGuid = node.Guid;
                    newConnectionData.ToGuid = rightSideTarget.Guid;

                    dynamicData.Add(newConnectionData);
                }
            });

            target.ConnectionData = dynamicData.ToArray();
        }
            
    }
}
