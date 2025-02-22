using com.absence.attributes;
using com.absence.dialoguesystem.internals;
using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace com.absence.dialoguesystem.editor
{
    public static class NodeCustomDataCreationHandler
    {
        public static Type NodeCustomDataTypeToCreate = typeof(NodeCustomData);
        public static Type OptionCustomDataTypeToCreate = typeof(NodeCustomData);

        [FieldButtonId(1801, priority = int.MaxValue)]
        static NodeCustomDataBase CreateNodeCustomData_FieldButton(object sender)
        {
            return CreateNodeCustomData(sender as Node, NodeCustomDataTypeToCreate);
        }

        [FieldButtonId(1800, priority = int.MaxValue)]
        static void DeleteNodeCustomData_FieldButton(object sender)
        {
            DeleteNodeCustomData(sender as Node);
        }

        [FieldButtonId(1803, priority = int.MaxValue)]
        static NodeCustomDataBase CreateOptionCustomData_FieldButton(object sender, object option)
        {
            return CreateOptionCustomData(sender as Node, option as Option, OptionCustomDataTypeToCreate);
        }

        [FieldButtonId(1802, priority = int.MaxValue)]
        static void DeleteOptionCustomData_FieldButton(object sender, object option)
        {
            DeleteOptionCustomData(sender as Node, option as Option);
        }

        public static NodeCustomDataBase CreateNodeCustomData(Node sender, Type type)
        {
            if (!type.BaseType.Equals(typeof(NodeCustomDataBase)))
            {
                Debug.LogError("Target type must derive from 'NodeCustomDataBase'.");
                return null;
            }

            ScriptableObject createdSO = ScriptableObject.CreateInstance(type);
            createdSO.name = $"{sender.Guid}_CustomData";

            AssetDatabase.AddObjectToAsset(createdSO, sender);

            sender.CustomData = (NodeCustomDataBase)createdSO;

            Undo.RegisterCreatedObjectUndo(createdSO, "Node (Create Custom Data)");

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            return null;
        }

        public static void DeleteNodeCustomData(Node sender)
        {
            Undo.RecordObject(sender, "Node (Delete Custom Data)");

            UnityEngine.Object objectWillGetDeleted = sender.CustomData;

            sender.CustomData = null;

            //AssetDatabase.RemoveObjectFromAsset(objectWillGetDeleted);
            if (objectWillGetDeleted == null)
                return;

            Undo.DestroyObjectImmediate(objectWillGetDeleted);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        public static NodeCustomDataBase CreateOptionCustomData(Node sender, Option context, Type type)
        {
            if (sender is not IDialogueNode dialogueNode)
            {
                Debug.LogError("Target node must implement the interface 'IDialogueNode'.");
                return null;
            }

            if (!type.BaseType.Equals(typeof(NodeCustomDataBase)))
            {
                Debug.LogError("Target type must derive from 'NodeCustomDataBase'.");
                return null;
            }

            Option realContext = dialogueNode.Options.First(op => op.Text.Equals(context.Text));

            if (realContext == null)
                return null;

            ScriptableObject createdSO = ScriptableObject.CreateInstance(type);
            createdSO.name = $"{sender.Guid}_OptionData";

            AssetDatabase.AddObjectToAsset(createdSO, sender);

            realContext.CustomData = (NodeCustomDataBase)createdSO;

            Undo.RegisterCreatedObjectUndo(createdSO, "Node (Create Option Data)");

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            return null;
        }

        public static void DeleteOptionCustomData(Node sender, Option context)
        {
            if (sender is not IDialogueNode dialogueNode)
            {
                Debug.LogError("Target node must implement the interface 'IDialogueNode'.");
                return;
            }

            Option realContext = dialogueNode.Options.First(op => op.Text.Equals(context.Text));

            if (realContext == null)
                return;

            Undo.RecordObject(sender, "Node (Delete Option Data)");

            UnityEngine.Object objectWillGetDeleted = realContext.CustomData;

            realContext.CustomData = null;

            //AssetDatabase.RemoveObjectFromAsset(objectWillGetDeleted);
            if (objectWillGetDeleted == null)
                return;

            Undo.DestroyObjectImmediate(objectWillGetDeleted);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}
