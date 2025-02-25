using com.absence.dialoguesystem.internals;
using com.absence.dialoguesystem.runtime.backup.data;
using System.Collections.Generic;

namespace com.absence.dialoguesystem.editor.backup
{
    public static class DialogueExporter
    {
        public static DialogueData Export(Dialogue dialogue)
        {
            DialogueData data = new();
            data.DefaultDialogueName = dialogue.name;
            WriteNodeList(data, dialogue);
            CopyConnections(data, dialogue);

            data.BlackboardData = DataGenerator.GenerateBlackboardData(dialogue.Blackboard);

            return data;
        }

        static void WriteNodeList(DialogueData target, Dialogue dialogue)
        {
            int nodeCount = dialogue.AllNodes.Count;

            target.NodeDatas = new NodeData[nodeCount];
            for (int i = 0; i < nodeCount; i++)
            {
                target.NodeDatas[i] = DataGenerator.GenerateNodeData(dialogue.AllNodes[i]);
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

            target.ConnectionDatas = dynamicData.ToArray();
        }
            
    }
}
