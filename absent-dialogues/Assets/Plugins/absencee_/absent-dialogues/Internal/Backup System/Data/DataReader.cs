using com.absence.dialoguesystem.internals;
using com.absence.dialoguesystem.runtime.backup.internals;
using System.Linq;

namespace com.absence.dialoguesystem.runtime.backup.data
{
    public static class DataReader
    {
        public static NodeVariableComparer ReadComparerData(NodeVariableComparerData data)
        {
            NodeVariableComparer comparer = new();
            comparer.TargetVariableName = data.TargetVariableName;
            comparer.TypeOfComparison = DialogueImportSettings.ComparerDictionary[data.ComparisonType];
            comparer.IntValue = data.IntValue;
            comparer.FloatValue = data.FloatValue;
            comparer.StringValue = data.StringValue;
            comparer.BooleanValue = data.BooleanValue;

            return comparer;
        }
        public static NodeVariableSetter ReadSetterData(NodeVariableSetterData data)
        {
            NodeVariableSetter setter = new();
            setter.TargetVariableName = data.TargetVariableName;
            setter.TypeOfSet = DialogueImportSettings.SetterDictionary[data.SetType];
            setter.IntValue = data.IntValue;
            setter.FloatValue = data.FloatValue;
            setter.StringValue = data.StringValue;
            setter.BooleanValue = data.BooleanValue;

            return setter;
        }
        public static Option ReadOptionData(OptionData data)
        {
            Option option = new();
            option.Text = data.Speech;
            option.UseShowIf = data.ShowIfInUse;

            option.Visibility = new Option.ShowIf();
            option.Visibility.Processor = DialogueImportSettings.ProcessorDictionary[data.ProcessorType];
            option.Visibility.ShowIfList = data.ShowIfData.ToList().ConvertAll(comparerData => ReadComparerData(comparerData));

            return option;
        }
    }
}