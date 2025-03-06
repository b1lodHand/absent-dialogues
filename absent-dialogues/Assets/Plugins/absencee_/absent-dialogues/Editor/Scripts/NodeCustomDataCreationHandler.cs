using com.absence.attributes;
using com.absence.dialoguesystem.examples;
using com.absence.dialoguesystem.internals;
using System;
using UnityEditor;
using UnityEngine;

namespace com.absence.dialoguesystem.editor
{
    public static class NodeCustomDataCreationHandler
    {
        public static Type DefaultDataType = typeof(NodeCustomData);

        public static Type NodeCustomDataTypeToCreate = DefaultDataType;
        public static Type OptionCustomDataTypeToCreate = DefaultDataType;
        public static Type GenericOptionCustomDataTypeToCreate = DefaultDataType;

        public static bool BypassUndo = false;

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

        [FieldButtonId(1805, priority = int.MaxValue)]
        static NodeCustomDataBase CreateGenericOptionCustomData_FieldButton(object sender, object option)
        {
            return CreateGenericOptionCustomData(sender as Dialogue, option as Option, GenericOptionCustomDataTypeToCreate);
        }

        [FieldButtonId(1804, priority = int.MaxValue)]
        static void DeleteGenericOptionCustomData_FieldButton(object sender, object option)
        {
            DeleteGenericOptionCustomData(sender as Dialogue, option as Option);
        }

        public static NodeCustomDataBase CreateNodeCustomData(Node sender, Type type)
        {
            if (!type.BaseType.Equals(typeof(NodeCustomDataBase)))
            {
                Debug.LogError("Target type must derive from 'NodeCustomDataBase'.");
                return null;
            }

            ScriptableObject createdSO = ScriptableObject.CreateInstance(type);
            createdSO.name = DialogueSystem.GenerateCustomDataName(sender);

            AssetDatabase.AddObjectToAsset(createdSO, sender);

            sender.CustomData = (NodeCustomDataBase)createdSO;

            if (!BypassUndo) Undo.RegisterCreatedObjectUndo(createdSO, "Node (Create Custom Data)");

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            return (NodeCustomDataBase)createdSO;
        }

        public static void DeleteNodeCustomData(Node sender)
        {
            Undo.RecordObject(sender, "Node (Delete Custom Data)");

            UnityEngine.Object objectWillGetDeleted = sender.CustomData;

            sender.CustomData = null;

            //AssetDatabase.RemoveObjectFromAsset(objectWillGetDeleted);
            if (objectWillGetDeleted == null)
                return;

            if (!BypassUndo) Undo.DestroyObjectImmediate(objectWillGetDeleted);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        public static NodeCustomDataBase CreateOptionCustomData(Node sender, Option context, Type type)
        {
            if (!type.BaseType.Equals(typeof(NodeCustomDataBase)))
            {
                Debug.LogError("Target type must derive from 'NodeCustomDataBase'.");
                return null;
            }

            ScriptableObject createdSO = ScriptableObject.CreateInstance(type);
            createdSO.name = DialogueSystem.GenerateOptionDataName(sender);

            AssetDatabase.AddObjectToAsset(createdSO, sender);

            if (!BypassUndo) Undo.RegisterCreatedObjectUndo(createdSO, "Node (Create Option Data)");

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            return (NodeCustomDataBase)createdSO;
        }

        public static void DeleteOptionCustomData(Node sender, Option context)
        {
            if (context == null)
                return;

            Undo.RecordObject(sender, "Node (Delete Option Data)");

            UnityEngine.Object objectWillGetDeleted = context.CustomData;

            //AssetDatabase.RemoveObjectFromAsset(objectWillGetDeleted);
            if (objectWillGetDeleted == null)
                return;

            if (!BypassUndo) Undo.DestroyObjectImmediate(objectWillGetDeleted);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        public static NodeCustomDataBase CreateGenericOptionCustomData(Dialogue sender, Option context, Type type)
        {
            if (!type.BaseType.Equals(typeof(NodeCustomDataBase)))
            {
                Debug.LogError("Target type must derive from 'NodeCustomDataBase'.");
                return null;
            }

            ScriptableObject createdSO = ScriptableObject.CreateInstance(type);
            createdSO.name = DialogueSystem.GenerateGenericOptionDataName(sender);

            AssetDatabase.AddObjectToAsset(createdSO, sender);

            if (!BypassUndo) Undo.RegisterCreatedObjectUndo(createdSO, "Dialogue (Create Generic Option Data)");

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            return (NodeCustomDataBase)createdSO;
        }

        public static void DeleteGenericOptionCustomData(Dialogue sender, Option context)
        {
            if (context == null)
                return;

            Undo.RecordObject(sender, "Dialogue (Delete Generic Option Data)");

            UnityEngine.Object objectWillGetDeleted = context.CustomData;

            //AssetDatabase.RemoveObjectFromAsset(objectWillGetDeleted);
            if (objectWillGetDeleted == null)
                return;

            if (!BypassUndo) Undo.DestroyObjectImmediate(objectWillGetDeleted);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}
