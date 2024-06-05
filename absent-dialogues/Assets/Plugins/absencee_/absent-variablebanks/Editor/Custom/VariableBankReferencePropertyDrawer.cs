using UnityEditor;
using UnityEngine;

namespace com.absence.variablebanks.editor
{
    /// <summary>
    /// A custom property drawer script for <see cref="VariableBankReferencePropertyDrawer"/>.
    /// </summary>
    [CustomPropertyDrawer(typeof(VariableBankReference), true)]
    public class VariableBankReferencePropertyDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight * 1;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedObject serializedObject = property.serializedObject;

            SerializedProperty targetGuidProp = property.FindPropertyRelative("m_targetGuid");

            string currentGuid = targetGuidProp.stringValue;

            serializedObject.Update();
            EditorGUI.BeginChangeCheck();

            Rect dynamicRect = new Rect(position);

            GUIContent actualLabel = EditorGUI.BeginProperty(dynamicRect, label, property);

            dynamicRect.height = EditorGUIUtility.singleLineHeight;

            if(Application.isPlaying)
            {
                GUI.enabled = false;
            }

            actualLabel.tooltip = $"Guid: {currentGuid}";
            VariableBank editorBank = ((VariableBank)EditorGUI.ObjectField(dynamicRect, actualLabel, VariableBankDatabase.GetBankIfExists(currentGuid), typeof(VariableBank), allowSceneObjects: false));

            if (editorBank != null) currentGuid = editorBank.Guid;
            else currentGuid = string.Empty;

            targetGuidProp.stringValue = currentGuid;

            GUI.enabled = true;

            dynamicRect.y += EditorGUIUtility.singleLineHeight;

            //GUI.enabled = false;

            //EditorGUI.LabelField(dynamicRect, $"Guid: {currentGuid}");

            //string bankName = "null (runtime only)";
            //if (Application.isPlaying && VariableBank.CloningCompleted) bankName = VariableBank.GetClone(currentGuid).name;

            //dynamicRect.y += EditorGUIUtility.singleLineHeight;
            //EditorGUI.LabelField(dynamicRect, $"Bank: {bankName}");

            //GUI.enabled = true;

            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
            }

        }
    }
}