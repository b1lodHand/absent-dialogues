using com.absence.dialoguesystem.internals;
using com.absence.dialoguesystem.runtime.backup;
using com.absence.dialoguesystem.runtime.backup.data;
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
            DataReader.ReadBlackboardData(data.BlackboardData, target.Blackboard);
            ReadNodeList(data, target);
        }
        static void ReadNodeList(DialogueData data, Dialogue target)
        {
            Dictionary<string, Node> oldGuidPairs = new Dictionary<string, Node>();
            int nodeCount = data.NodeDatas.Length;

            for (int i = 0; i < nodeCount; i++)
            {
                NodeData nodeData = data.NodeDatas[i];
                oldGuidPairs.Add(nodeData.OldGuid, DataReader.ReadNodeData(nodeData, target));
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

                from.AddOutputConnection(to, portIndex);
            });
        }
        static void UpdateNodes(DialogueImportContext context)
        {
            for (int i = 0; i < context.Dialogue.AllNodes.Count; i++)
            {
                Node node = context.Dialogue.AllNodes[i];
                NodeData data = context.DialogueData.NodeDatas[i];

                //Type nodeType = DialogueImportSettings.NodeTypeDictionary[data.NodeTypeName];
                Type nodeType = TypeCache.GetTypesDerivedFrom(typeof(Node)).Where(t => t.Name.Equals(data.NodeTypeName)).FirstOrDefault();

                //DialogueImportSettings.NodeImportActionDictionary[nodeType].Invoke(node, data, context);
                node.OnImport(data, context);

                if (node is not IContainVariableManipulators manipulator) continue;

                List<NodeVariableComparer> comparers = manipulator.GetComparers();
                List<NodeVariableSetter> setters = manipulator.GetSetters();

                if (comparers != null) comparers.ForEach(comparer => comparer.SetBlackboardBank(context.Dialogue.Blackboard.Bank));
                if (setters != null) setters.ForEach(setter => setter.SetBlackboardBank(context.Dialogue.Blackboard.Bank));
            }
        }

        internal class ImportDialogueEndNameEditAction : EndNameEditAction
        {
            public DialogueData ImportedData { get; set; }
            private event Action<Dialogue> m_onCompleteAction = null;

            public override void Action(int instanceId, string pathName, string resourceFile)
            {
                Dialogue dialogueCreated = DialogueCreationHandler.CreateDialogue(pathName);
                ReadInitialDialogueData(ImportedData, dialogueCreated);
                dialogueCreated.Entry = dialogueCreated.AllNodes.Find(node => node is EntryNode) as EntryNode;

                dialogueCreated.ValidateNodes();
                dialogueCreated.ResetNodeStates();

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
