using com.absence.dialoguesystem.internals;
using com.absence.dialoguesystem.runtime.backup.internals;

namespace com.absence.dialoguesystem.runtime.backup.data
{
    public static class DataGenerator
    {
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
