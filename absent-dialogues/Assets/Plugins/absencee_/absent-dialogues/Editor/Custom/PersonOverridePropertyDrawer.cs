using com.absence.dialoguesystem.internals;
using com.absence.personsystem;
using UnityEditor;
using UnityEngine;

namespace com.absence.dialoguesystem.editor.internals
{
    [CustomPropertyDrawer(typeof(PersonOverride), true)]
    public class PersonOverridePropertyDrawer : PropertyDrawer
    {
        private const float k_seperatorWidth = 1f;
        private const float k_horizontalSpacing = 4f;
        private const float k_seperatorHeightDecremention = 4f;
        private static readonly Color s_seperatorColor = new Color(1f, 1f, 1f, 0.1f);

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty targetProp = property.FindPropertyRelative("Target");
            SerializedProperty overrideProp = property.FindPropertyRelative("Override");

            GUIContent empty = new("");

            position.height = EditorGUIUtility.singleLineHeight;
            float fieldWidth = (position.width - ((2 * k_horizontalSpacing) + k_seperatorWidth)) / 2;

            position.width = fieldWidth;

            GUI.enabled = false;

            targetProp.objectReferenceValue = EditorGUI.ObjectField(position, empty, targetProp.objectReferenceValue, typeof(Person), false);

            GUI.enabled = true;

            position.x += fieldWidth;
            position.x += k_horizontalSpacing;
            position.width = k_seperatorWidth;
            position.y += k_seperatorHeightDecremention / 2;
            position.height -= k_seperatorHeightDecremention;

            EditorGUI.DrawRect(position, s_seperatorColor);

            position.x += k_seperatorWidth;
            position.x += k_horizontalSpacing;
            position.width = fieldWidth;
            position.y -= k_seperatorHeightDecremention / 2;
            position.height += k_horizontalSpacing;

            overrideProp.objectReferenceValue = EditorGUI.ObjectField(position, empty, overrideProp.objectReferenceValue, typeof(Person), false);
        }
    }
}
