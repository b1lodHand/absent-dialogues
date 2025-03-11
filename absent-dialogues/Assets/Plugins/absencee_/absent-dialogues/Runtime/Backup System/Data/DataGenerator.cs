using com.absence.variablesystem.banksystembase;
using com.absence.variablesystem.builtin;

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

            for (int i = 0; i < intCount; i++)
            {
                IntPair intPair = new();
                var intVariable = bank.Ints[i];

                intPair.Key = intVariable.Name;
                intPair.Value = intVariable.Variable.Value;

                data.Ints[i] = intPair;
            }

            for (int f = 0; f < floatCount; f++)
            {
                FloatPair floatPair = new();
                var floatVariable = bank.Floats[f];

                floatPair.Key = floatVariable.Name;
                floatPair.Value = floatVariable.Variable.Value;

                data.Floats[f] = floatPair;
            }

            for (int s = 0; s < intCount; s++)
            {
                StringPair stringPair = new();
                var floatVariable = bank.Strings[s];

                stringPair.Key = floatVariable.Name;
                stringPair.Value = floatVariable.Variable.Value;

                data.Strings[s] = stringPair;
            }

            for (int b = 0; b < booleanCount; b++)
            {
                BooleanPair booleanPair = new();
                var booleanVariable = bank.Booleans[b];

                booleanPair.Key = booleanVariable.Name;
                booleanPair.Value = booleanVariable.Variable.Value;

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