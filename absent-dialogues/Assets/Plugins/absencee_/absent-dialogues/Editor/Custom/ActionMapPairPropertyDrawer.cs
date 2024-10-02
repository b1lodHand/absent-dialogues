using com.absence.dialoguesystem.internals;
using com.absence.dialoguesystem.runtime;
using UnityEditor;
using UnityEngine;

namespace com.absence.dialoguesystem.editor
{
    [CustomPropertyDrawer(typeof(DialogueActionMapper.ActionMapPair))]
    public class ActionMapPairPropertyDrawer : PropertyDrawer
    {
        static readonly int s_vertical_spacing = 2;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            SerializedProperty foldoutProperty = property.FindPropertyRelative("m_editorProperties").
                FindPropertyRelative("IsFoldout");

            SerializedProperty eventProp = property.FindPropertyRelative("AttachedEvent");

            bool isFoldout = foldoutProperty.boolValue;
            float eventHeight = EditorGUI.GetPropertyHeight(eventProp, new GUIContent("Events: "), true);

            if (isFoldout) return EditorGUI.GetPropertyHeight(property, label, true) + eventHeight + (s_vertical_spacing * 2);
            else return EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty foldoutProperty = property.FindPropertyRelative("m_editorProperties").
                FindPropertyRelative("IsFoldout");

            bool isFoldout = foldoutProperty.boolValue;

            SerializedProperty eventProp = property.FindPropertyRelative("AttachedEvent");
            SerializedProperty enabledProp = property.FindPropertyRelative("Enabled");
            ActionNode targetActionNode = (property.boxedValue as DialogueActionMapper.ActionMapPair).TargetActionNode;

            bool enabled = enabledProp.boolValue;

            float totalHeight = position.height;
            position.height = EditorGUIUtility.singleLineHeight;

            EditorGUI.BeginProperty(position, label, property);

            GUIStyle foldoutLabelStyle = new GUIStyle(EditorStyles.foldout);
            foldoutLabelStyle.richText = true;
            foldoutLabelStyle.fontStyle = FontStyle.Bold;

            string foldoutLabel;
            if (enabled) foldoutLabel = Utilities.Texts.ColorizeString(targetActionNode.UniqueMapperId, 
                Constants.Tooltips.VARIABLE_NAME_HEX);
            else foldoutLabel = Utilities.Texts.ColorizeString($"{targetActionNode.UniqueMapperId} [DISABLED]",
                Constants.Tooltips.AND_HEX);

            isFoldout = EditorGUI.Foldout(position, isFoldout, 
                foldoutLabel,
                true, foldoutLabelStyle);

            foldoutProperty.boolValue = isFoldout;
            if (!isFoldout) return;

            position.y += EditorGUIUtility.singleLineHeight;
            position.y += s_vertical_spacing;

            EditorGUI.PropertyField(position, eventProp, new GUIContent("Events: "), true);

            EditorGUI.EndProperty();
        }
    }
}