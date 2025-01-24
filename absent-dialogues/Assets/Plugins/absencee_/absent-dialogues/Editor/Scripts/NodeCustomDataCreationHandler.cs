using com.absence.attributes;
using com.absence.attributes.editor;
using com.absence.dialoguesystem.internals;
using System;
using UnityEditor;
using UnityEngine;

namespace com.absence.dialoguesystem.editor
{
    public static class NodeCustomDataCreationHandler
    {
        public static Type NodeCustomDataTypeToCreate = typeof(NodeCustomData);

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

        public static NodeCustomDataBase CreateNodeCustomData(Node sender, Type type)
        {
            if (!type.BaseType.Equals(typeof(NodeCustomDataBase)))
            {
                Debug.LogError("Target type must derive from 'NodeCustomDataBase'.");
                return null;
            }

            ScriptableObject createdSO = ScriptableObject.CreateInstance(type);
            createdSO.name = $"{sender.Guid}_CustomData";

            AssetDatabase.AddObjectToAsset(createdSO, sender.MasterDialogue);

            sender.CustomData = (NodeCustomDataBase)createdSO;

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            return null;
        }

        public static void DeleteNodeCustomData(Node sender)
        {
            UnityEngine.Object objectWillGetDeleted = sender.CustomData;

            AssetDatabase.RemoveObjectFromAsset(objectWillGetDeleted);
            ScriptableObject.DestroyImmediate(objectWillGetDeleted);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            sender.CustomData = null;
        }
    }
}
