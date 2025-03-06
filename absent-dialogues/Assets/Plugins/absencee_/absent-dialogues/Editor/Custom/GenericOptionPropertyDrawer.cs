using com.absence.dialoguesystem.internals;
using UnityEditor;
using UnityEngine;

namespace com.absence.dialoguesystem.editor.internals
{
    [CustomPropertyDrawer(typeof(GenericOption), true)]
    public class GenericOptionPropertyDrawer : OptionPropertyDrawer
    {
        private const float k_minFieldWidth = 18f;

        protected override int NewButtonId => 1805;
        protected override int DelButtonId => 1804;

        protected override bool DrawHeaderFoldout(Rect position, SerializedProperty property, GUIContent label, bool foldout)
        {
            SerializedProperty textProp = property.FindPropertyRelative("Text");

            float spacing = EditorGUIUtility.standardVerticalSpacing;
            float height = EditorGUIUtility.singleLineHeight;

            GUIStyle foldoutStyle = new GUIStyle(EditorStyles.foldout)
            {
                richText = true,
            };

            string text = textProp.stringValue;
            if (string.IsNullOrWhiteSpace(text)) text = Constants.Text.NO_TEXT;

            const bool kToggleOnRemainderClick = true;

            GUIStyle normalStyle = new GUIStyle(EditorStyles.label)
            {
                richText = true
            };

            GUIStyle focusedStyle = new GUIStyle(EditorStyles.textField)
            {
                richText = true
            };

            GUIContent content = new GUIContent()
            {
                text = text,
                tooltip = textProp.tooltip,
            };

            Vector2 size = normalStyle.CalcSize(content);
            float width = Mathf.Min(size.x + k_minFieldWidth, position.width);

            Rect foldoutFirstHalfPosition = position;
            Rect foldoutSecondHalfPosition = position;
            foldoutFirstHalfPosition.width = spacing + height;
            foldoutSecondHalfPosition.width = position.width - (foldoutFirstHalfPosition.width + width);
            foldoutSecondHalfPosition.x += foldoutFirstHalfPosition.width + width;

            foldout = EditorGUI.Foldout(foldoutFirstHalfPosition, foldout, "", false, foldoutStyle);
            if (kToggleOnRemainderClick) foldout = EditorGUI.Foldout(foldoutSecondHalfPosition, foldout, "", true, normalStyle);

            float originalWidth = position.width;

            position.width = width;

            textProp.stringValue = EditorGUI.TextField(position, text, normalStyle);

            return foldout;
        }
    }
}
