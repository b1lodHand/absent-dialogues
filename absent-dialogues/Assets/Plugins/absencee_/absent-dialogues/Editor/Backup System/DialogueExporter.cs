using com.absence.dialoguesystem.internals;
using com.absence.dialoguesystem.runtime.backup.data;
using com.absence.dialoguesystem.runtime.backup.internals;
using com.absence.variablesystem;
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

            data.BlackboardData = GenerateBlackboardData(dialogue.Blackboard);

            return data;
        }

        static void WriteNodeList(DialogueData target, Dialogue dialogue)
        {
            int nodeCount = dialogue.AllNodes.Count;

            target.NodeDatas = new NodeData[nodeCount];
            for (int i = 0; i < nodeCount; i++)
            {
                target.NodeDatas[i] = GenerateNodeData(dialogue.AllNodes[i]);
            }
        }
        static void CopyConnections(DialogueData target, Dialogue dialogue)
        {
            List<NodeConnectionData> dynamicData = new();
            dialogue.AllNodes.ForEach(fromNode =>
            {
                List<(int portIndex, Node toNode)> rightSideNodes = fromNode.GetNextNodes();
                rightSideNodes.ForEach(connection =>
                {
                    NodeConnectionData newConnectionData = new();
                    newConnectionData.FromPortIndex = connection.portIndex;
                    newConnectionData.FromGuid = fromNode.Guid;
                    newConnectionData.ToGuid = connection.toNode.Guid;

                    dynamicData.Add(newConnectionData);
                });
            });

            target.ConnectionDatas = dynamicData.ToArray();
        }
        static NodeData GenerateNodeData<T>(T node) where T : Node
        {
            NodeData data = new();
            data.PositionX = node.Position.x;
            data.PositionY = node.Position.y;
            //data.NodeTypeIndicator = DialogueExportSettings.NodeTypeDictionary[node.GetType()];
            data.NodeTypeName = node.GetType().Name;
            data.OldGuid = node.Guid;

            if (node is IContainData speecher)
            {
                data.Text = speecher.Text;

                List<Option> options = speecher.Options;
                if(options != null) data.OptionDatas = options.ConvertAll(option => GenerateOptionData(option)).ToArray();
            }

            //DialogueExportSettings.NodeExportActionDictionary[node.GetType()]?.Invoke(node, data);
            node.OnExport(data);

            return data;
        }
        static BlackboardData GenerateBlackboardData(Blackboard blackboard)
        {
            BlackboardData data = new();
            VariableBank bank = blackboard.Bank;

            int intCount = bank.Ints.Count;
            int floatCount = bank.Floats.Count;
            int stringCount = bank.Strings.Count;
            int booleanCount = bank.Booleans.Count;

            data.Ints = new IntPair[intCount];
            data.Floats = new FloatPair[floatCount];
            data.Strings = new StringPair[stringCount];
            data.Booleans = new BooleanPair[booleanCount];

            for (int i = 0; i < intCount; i++)
            {
                IntPair intPair = new();
                Variable_Integer intVariable = bank.Ints[i];

                intPair.Key = intVariable.Name;
                intPair.Value = intVariable.Value;

                data.Ints[i] = intPair;
            }

            for (int f = 0; f < floatCount; f++)
            {
                FloatPair floatPair = new();
                Variable_Float floatVariable = bank.Floats[f];

                floatPair.Key = floatVariable.Name;
                floatPair.Value = floatVariable.Value;

                data.Floats[f] = floatPair;
            }

            for (int s = 0; s < intCount; s++)
            {
                StringPair stringPair = new();
                Variable_String floatVariable = bank.Strings[s];

                stringPair.Key = floatVariable.Name;
                stringPair.Value = floatVariable.Value;

                data.Strings[s] = stringPair;
            }

            for (int b = 0; b < booleanCount; b++)
            {
                BooleanPair booleanPair = new();
                Variable_Boolean booleanVariable = bank.Booleans[b];

                booleanPair.Key = booleanVariable.Name;
                booleanPair.Value = booleanVariable.Value;

                data.Booleans[b] = booleanPair;
            }

            return data;
        }
        static OptionData GenerateOptionData(Option option)
        {
            OptionData data = new();
            data.ShowIfInUse = option.UseShowIf;
            data.ShowIfData = option.Visibility.ShowIfList.ConvertAll(comparer => DataGenerator.GenerateComparerData(comparer)).ToArray();
            data.Speech = option.Text;
            data.ProcessorType = DialogueExportSettings.ProcessorDictionary[option.Visibility.Processor];

            return data;
        }
    }
}
