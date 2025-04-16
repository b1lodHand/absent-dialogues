using com.absence.variablesystem.banksystembase;
using com.absence.variablesystem.builtin;
using System;
using System.Collections.Generic;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
#else
using System.Reflection;
#endif

namespace com.absence.dialoguesystem.internals.backup.data
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

            Node node = DialogueSystem.CreateNode(nodeType, targetDialogue);

#if UNITY_EDITOR
            node.Position.x = data.PositionX;
            node.Position.y = data.PositionY;
            AssetDatabase.AddObjectToAsset(node, targetDialogue);
#endif
            return node;
        }
        public static void ReadBlackboardData(BlackboardData data, Blackboard target)
        {
            var ints = data.Ints
                .ToDictionary(intPair => intPair.Key, 
                intPair => new IntegerVariable(intPair.Value));

            var floats = data.Floats
                .ToDictionary(floatPair => floatPair.Key, 
                floatPair => new FloatVariable(floatPair.Value));

            var strings = data.Strings
                .ToDictionary(stringPair => stringPair.Key, 
                stringPair => new StringVariable(stringPair.Value));

            var booleans = data.Booleans
                .ToDictionary(booleanPair => booleanPair.Key,
                booleanPair => new BooleanVariable(booleanPair.Value));

            target.Bank.Ints = ints;
            target.Bank.Floats = floats;
            target.Bank.Strings = strings;
            target.Bank.Booleans = booleans;
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