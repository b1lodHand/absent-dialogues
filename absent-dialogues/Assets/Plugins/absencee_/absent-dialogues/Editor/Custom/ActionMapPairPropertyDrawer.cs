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
        static readonly int s_vertical_space_count = 1;
        static readonly int s_vertical_base_size = 36;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            SerializedProperty foldoutProperty = property.FindPropertyRelative("m_editorProperties").
                FindPropertyRelative("IsFoldout");

            SerializedProperty eventProp = property.FindPropertyRelative("AttachedEvent");

            bool isFoldout = foldoutProperty.boolValue;
            float eventHeight = EditorGUI.GetPropertyHeight(eventProp, new GUIContent("Events"), true);

            if (isFoldout) return s_vertical_base_size + eventHeight + (s_vertical_spacing * s_vertical_space_count);
            else return EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty foldoutProperty = property.FindPropertyRelative("m_editorProperties").
                FindPropertyRelative("IsFoldout");

            bool isFoldout = foldoutProperty.boolValue;

            SerializedProperty eventProp = property.FindPropertyRelative("AttachedEvent");
            SerializedProperty enabledProp = property.FindPropertyRelative("Enabled");
            SerializedProperty idProp = property.FindPropertyRelative("BackupId");

            string id = idProp.stringValue;
            bool enabled = enabledProp.boolValue;

            float totalHeight = position.height;
            position.height = EditorGUIUtility.singleLineHeight;

            EditorGUI.BeginProperty(position, label, property);

            GUIStyle foldoutLabelStyle = new GUIStyle(EditorStyles.foldout);
            foldoutLabelStyle.richText = true;
            foldoutLabelStyle.fontStyle = FontStyle.Bold;

            string foldoutLabel;
            if (enabled) foldoutLabel = Utilities.Texts.ColorizeString(id, 
                Constants.Tooltips.VARIABLE_NAME_HEX);
            else foldoutLabel = Utilities.Texts.ColorizeString($"{id} [DISABLED]",
                Constants.Tooltips.AND_HEX);

            isFoldout = EditorGUI.Foldout(position, isFoldout, 
                foldoutLabel,
                true, foldoutLabelStyle);

            foldoutProperty.boolValue = isFoldout;
            if (!isFoldout) return;

            position.y += EditorGUIUtility.singleLineHeight;
            position.y += s_vertical_spacing;

            EditorGUI.PropertyField(position, eventProp, new GUIContent("Events"), true);

            EditorGUI.EndProperty();
        }
    }
}