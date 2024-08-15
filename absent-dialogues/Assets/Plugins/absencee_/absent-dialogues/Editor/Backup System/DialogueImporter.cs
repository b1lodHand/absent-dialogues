using com.absence.dialoguesystem.editor.backup.data;
using com.absence.dialoguesystem.editor.backup.internals;
using com.absence.dialoguesystem.internals;
using com.absence.variablesystem;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.ProjectWindowCallback;
using UnityEngine;

namespace com.absence.dialoguesystem.editor.backup
{
    public static class DialogueImporter
    {
        public static void Import(DialogueData data, string pathToCreate, Action<Dialogue> onComplete = null)
        {
            ImportDialogueEndNameEditAction create = ScriptableObject.CreateInstance<ImportDialogueEndNameEditAction>().OnComplete(onComplete);
            create.ImportedData = data;

            var icon = EditorGUIUtility.IconContent("d_ScriptableObject Icon").image as Texture2D;

            ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0, create, pathToCreate, icon, null);
        }

        static void ReadInitialDialogueData(DialogueData data, Dialogue target)
        {
            ReadBlackboardData(data.BlackboardData, target.Blackboard);
            ReadNodeList(data, target);
        }
        static void ReadBlackboardData(BlackboardData data, Blackboard target)
        {
            List<Variable_Integer> ints = data.Ints.ToList().ConvertAll(intPair =>
            {
                return new Variable_Integer(intPair.Key, intPair.Value);
            }).ToList();

            List<Variable_Float> floats = data.Floats.ToList().ConvertAll(floatPair =>
            {
                return new Variable_Float(floatPair.Key, floatPair.Value);
            }).ToList();

            List<Variable_String> strings = data.Strings.ToList().ConvertAll(stringPair =>
            {
                return new Variable_String(stringPair.Key, stringPair.Value);
            }).ToList();

            List<Variable_Boolean> booleans = data.Booleans.ToList().ConvertAll(booleanPair =>
            {
                return new Variable_Boolean(booleanPair.Key, booleanPair.Value);
            }).ToList();

            target.Bank.Ints = new(ints);
            target.Bank.Floats = new(floats);
            target.Bank.Strings = new(strings);
            target.Bank.Booleans = new(booleans);
        }
        static void ReadNodeList(DialogueData data, Dialogue target)
        {
            Dictionary<string, Node> oldGuidPairs = new Dictionary<string, Node>();
            int nodeCount = data.NodeDatas.Length;

            for (int i = 0; i < nodeCount; i++)
            {
                NodeData nodeData = data.NodeDatas[i];
                oldGuidPairs.Add(nodeData.OldGuid, ReadNodeData(nodeData, target));
            }

            DialogueImportContext context = new DialogueImportContext()
            {
                Dialogue = target,
                OldGuidPairs = oldGuidPairs,
                DialogueData = data,
            };

            ApplyConnections(context);
            UpdateNodes(context);
        }
        static void ApplyConnections(DialogueImportContext context)
        {
            context.DialogueData.ConnectionDatas.ToList().ForEach(connectionData =>
            {
                Node from = context.OldGuidPairs[connectionData.FromGuid];
                Node to = context.OldGuidPairs[connectionData.ToGuid];

                int portIndex = connectionData.FromPortIndex;

                from.AddNextNode(to, portIndex);
            });
        }
        static Node ReadNodeData(NodeData data, Dialogue targetDialogue)
        {
            Type nodeType = DialogueImportSettings.NodeTypeDictionary[data.NodeTypeIndicator];
            Node node = targetDialogue.CreateNode(nodeType);
            node.Guid = GUID.Generate().ToString();
            node.Position.x = data.PositionX;
            node.Position.y = data.PositionY;
            //node.ExitDialogueAfterwards = data.ExitDialogueAfterwards; deprecated.

            if(node is IContainData speecher)
            {
                speecher.Text = data.Speech;
                List<Option> options = data.OptionDatas.ToList().ConvertAll(optionData => ReadOptionData(optionData)).ToList();
                if(options.Count > 0) speecher.Options = new(options);
            }

            AssetDatabase.AddObjectToAsset(node, targetDialogue);
            return node;
        }
        static void UpdateNodes(DialogueImportContext context)
        {
            for (int i = 0; i < context.Dialogue.AllNodes.Count; i++)
            {
                Node node = context.Dialogue.AllNodes[i];
                NodeData data = context.DialogueData.NodeDatas[i];

                Type nodeType = DialogueImportSettings.NodeTypeDictionary[data.NodeTypeIndicator];

                DialogueImportSettings.NodeImportActionDictionary[nodeType].Invoke(node, data, context);

                if (node is not IContainVariableManipulators manipulator) continue;

                List<NodeVariableComparer> comparers = manipulator.GetComparers();
                List<NodeVariableSetter> setters = manipulator.GetSetters();

                if (comparers != null) comparers.ForEach(comparer => comparer.SetBlackboardBank(context.Dialogue.Blackboard.Bank));
                if (setters != null) setters.ForEach(setter => setter.SetBlackboardBank(context.Dialogue.Blackboard.Bank));
            }
        }

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

        internal class ImportDialogueEndNameEditAction : EndNameEditAction
        {
            public DialogueData ImportedData { get; set; }
            private event Action<Dialogue> m_onCompleteAction = null;

            public override void Action(int instanceId, string pathName, string resourceFile)
            {
                Dialogue dialogueCreated = DialogueCreationHandler.CreateDialogue(pathName);
                ReadInitialDialogueData(ImportedData, dialogueCreated);
                dialogueCreated.RootNode = dialogueCreated.AllNodes.Find(node => node is RootNode) as RootNode;

                dialogueCreated.Rebind();
                dialogueCreated.Initialize();

                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                m_onCompleteAction?.Invoke(dialogueCreated);
            }

            public override void Cancelled(int instanceId, string pathName, string resourceFile)
            {
                Dialogue item = EditorUtility.InstanceIDToObject(instanceId) as Dialogue;
                ScriptableObject.DestroyImmediate(item);
            }

            public ImportDialogueEndNameEditAction OnComplete(Action<Dialogue> action)
            {
                m_onCompleteAction += action;
                return this;
            }
        }
    }
}
