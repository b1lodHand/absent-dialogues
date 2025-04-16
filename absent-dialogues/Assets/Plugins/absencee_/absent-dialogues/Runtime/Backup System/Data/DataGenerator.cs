using com.absence.variablesystem.banksystembase;
using com.absence.variablesystem.builtin;
using System.Linq;

namespace com.absence.dialoguesystem.internals.backup.data
{
    public static class DataGenerator
    {
        public static NodeData GenerateNodeData<T>(T node) where T : Node
        {
            NodeData data = new();
#if UNITY_EDITOR
            data.PositionX = node.Position.x;
            data.PositionY = node.Position.y;
#endif
            data.NodeTypeName = node.GetType().Name;
            data.OldGuid = node.Guid;

            node.OnExport(data);
            return data;
        }
        public static OptionData GenerateOptionData(Option option)
        {
            OptionData data = new();
            data.ShowIfInUse = option.UseShowIf;
            data.ShowIfData = option.Visibility.ShowIfList.ConvertAll(comparer => DataGenerator.GenerateComparerData(comparer)).ToArray();
            data.Text = option.Text;
            data.ProcessorType = DialogueExportSettings.ProcessorDictionary[option.Visibility.Processor];
            data.OldLeadingNodeGuid = option.LeadingNode != null ?
                option.LeadingNode.Guid : Node.NaN;

            return data;
        }
        public static BlackboardData GenerateBlackboardData(Blackboard blackboard)
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

            var intList = bank.Ints.ToList();
            for (int i = 0; i < intCount; i++)
            {
                IntPair intPair = new();
                var intVariable = intList[i];

                intPair.Key = intVariable.Key;
                intPair.Value = intVariable.Value.Value;

                data.Ints[i] = intPair;
            }

            var floatlist = bank.Floats.ToList();
            for (int f = 0; f < floatCount; f++)
            {
                FloatPair floatPair = new();
                var floatVariable = floatlist[f];

                floatPair.Key = floatVariable.Key;
                floatPair.Value = floatVariable.Value.Value;

                data.Floats[f] = floatPair;
            }

            var stringList = bank.Strings.ToList();
            for (int s = 0; s < intCount; s++)
            {
                StringPair stringPair = new();
                var floatVariable = stringList[s];

                stringPair.Key = floatVariable.Key;
                stringPair.Value = floatVariable.Value.Value;

                data.Strings[s] = stringPair;
            }

            var booleanList = bank.Booleans.ToList();
            for (int b = 0; b < booleanCount; b++)
            {
                BooleanPair booleanPair = new();
                var booleanVariable = booleanList[b];

                booleanPair.Key = booleanVariable.Key;
                booleanPair.Value = booleanVariable.Value.Value;

                data.Booleans[b] = booleanPair;
            }

            return data;
        }
        public static NodeVariableComparerData GenerateComparerData(NodeVariableComparer comparer)
        {
            NodeVariableComparerData data = new();
            data.TargetVariableName = comparer.TargetVariableName;
            data.ComparisonType = DialogueExportSettings.ComparerDictionary[comparer.TypeOfComparison];
            data.IntValue = comparer.IntValue;
            data.FloatValue = comparer.FloatValue;
            data.StringValue = comparer.StringValue;
            data.BooleanValue = comparer.BooleanValue;

            return data;
        }
        public static NodeVariableSetterData GenerateSetterData(NodeVariableSetter setter)
        {
            NodeVariableSetterData data = new();
            data.TargetVariableName = setter.TargetVariableName;
            data.SetType = DialogueExportSettings.SetterDictionary[setter.TypeOfSet];
            data.IntValue = setter.IntValue;
            data.FloatValue = setter.FloatValue;
            data.StringValue = setter.StringValue;
            data.BooleanValue = setter.BooleanValue;

            return data;
        }
    }
}