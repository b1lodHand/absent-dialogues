using com.absence.dialoguesystem.internals;
using com.absence.dialoguesystem.runtime.backup.internals;
using com.absence.variablesystem.builtin;
using System;
using System.Collections.Generic;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#else
using System.Reflection;
#endif

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
        public static Node ReadNodeData(NodeData data, Dialogue targetDialogue)
        {
#if UNITY_EDITOR
            Type nodeType = TypeCache.GetTypesDerivedFrom(typeof(Node)).Where(t => t.Name.Equals(data.NodeTypeName)).FirstOrDefault();
#else
            Type nodeType = null;
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly assembly in assemblies) 
            {
                foreach (Type type in assembly.GetTypes()) 
                {
                    if (type.Name.Equals(data.NodeTypeName))
                    {
                        nodeType = type;
                        break;
                    }
                }

                if (nodeType != null) 
                    break;
            }
#endif
            if (nodeType == null)
                throw new Exception("Something went wrong while reading node data!");

            Node node = targetDialogue.CreateNode(nodeType);
            node.Guid = Guid.NewGuid().ToString();
            node.name = node.Guid;

            if (node is EntryNode) node.name = "EntryNode";

#if UNITY_EDITOR
            node.Position.x = data.PositionX;
            node.Position.y = data.PositionY;
            AssetDatabase.AddObjectToAsset(node, targetDialogue);
#endif
            return node;
        }
        public static void ReadBlackboardData(BlackboardData data, Blackboard target)
        {
            List<IntegerVariable> ints = data.Ints.ToList().ConvertAll(intPair =>
            {
                return new IntegerVariable(intPair.Key, intPair.Value);
            }).ToList();

            List<FloatVariable> floats = data.Floats.ToList().ConvertAll(floatPair =>
            {
                return new FloatVariable(floatPair.Key, floatPair.Value);
            }).ToList();

            List<StringVariable> strings = data.Strings.ToList().ConvertAll(stringPair =>
            {
                return new StringVariable(stringPair.Key, stringPair.Value);
            }).ToList();

            List<BooleanVariable> booleans = data.Booleans.ToList().ConvertAll(booleanPair =>
            {
                return new BooleanVariable(booleanPair.Key, booleanPair.Value);
            }).ToList();

            target.Bank.Ints = new(ints);
            target.Bank.Floats = new(floats);
            target.Bank.Strings = new(strings);
            target.Bank.Booleans = new(booleans);
        }

        public static T ReadOptionData<T>(OptionData data) where T : Option, new()
        {
            T option = new();
            option.Text = data.Text;
            option.UseShowIf = data.ShowIfInUse;

            option.Visibility = new Option.ShowIf();
            option.Visibility.Processor = DialogueImportSettings.ProcessorDictionary[data.ProcessorType];
            option.Visibility.ShowIfList = data.ShowIfData.ToList().ConvertAll(comparerData => ReadComparerData(comparerData));

            return option;
        }

        public static void ReadGenericOptionData(DialogueData data, Dialogue target)
        {
            target.GenericOptions = new();
            for (int i = 0; i < data.GenericOptionData.Length; i++)
            {
                target.GenericOptions.Add(ReadOptionData<GenericOption>(data.GenericOptionData[i]));
            }
        }
    }
}