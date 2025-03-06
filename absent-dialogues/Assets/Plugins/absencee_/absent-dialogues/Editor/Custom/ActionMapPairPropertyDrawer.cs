using com.absence.dialoguesystem.examples;
using com.absence.dialoguesystem.internals;
using UnityEditor;
using UnityEngine;

namespace com.absence.dialoguesystem.editor.examples
{
    [CustomPropertyDrawer(typeof(DialogueActionMapper.ActionMapPair))]
    public class ActionMapPairPropertyDrawer : PropertyDrawer
    {
        static readonly int s_vertical_spacing = 2;
        static readonly int s_vertical_space_count = 1;
        static readonly int s_vertical_base_size = 36;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            SerializedProperty eventProp = property.FindPropertyRelative("AttachedEvent");

            bool isFoldout = property.isExpanded;
            float eventHeight = EditorGUI.GetPropertyHeight(eventProp, new GUIContent("Events"), true);

            if (isFoldout) return s_vertical_base_size + eventHeight + (s_vertical_spacing * s_vertical_space_count);
            else return EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            bool isFoldout = property.isExpanded;

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
            if (enabled) foldoutLabel = Utilities.Text.ColorizeString(id, 
                Constants.Tooltips.VARIABLE_NAME_HEX);
            else foldoutLabel = Utilities.Text.ColorizeString($"{id} [DISABLED]",
                Constants.Tooltips.AND_HEX);

            isFoldout = EditorGUI.Foldout(position, isFoldout, 
                foldoutLabel,
                true, foldoutLabelStyle);

            property.isExpanded = isFoldout;
            if (!isFoldout) return;

            position.y += EditorGUIUtility.singleLineHeight;
            position.y += s_vertical_spacing;

            EditorGUI.PropertyField(position, eventProp, new GUIContent("Events"), true);

            EditorGUI.EndProperty();
        }
    }
}