using UnityEditor;
using UnityEngine;

namespace com.absence.variablesystem.editor
{
    [CustomPropertyDrawer(typeof(Variable), true)]
    public class VariableEditor : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            int lineCount = 2;
            return EditorGUIUtility.singleLineHeight * lineCount + EditorGUIUtility.standardVerticalSpacing * (lineCount - 1);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            var nameProp = property.FindPropertyRelative("m_name");
            var valueProp = property.FindPropertyRelative("m_value");

            var horizontalSpacing = 15;
            var sizeX = (position.size.x - horizontalSpacing) / 2;

            Rect dynamicPosition = new Rect(position.x, position.y, sizeX, EditorGUIUtility.singleLineHeight);

            EditorGUI.LabelField(dynamicPosition, $"Name ({label.text})");
            dynamicPosition.x += sizeX;
            dynamicPosition.x += horizontalSpacing;

            EditorGUI.LabelField(dynamicPosition, "Value");
            dynamicPosition.y += EditorGUIUtility.singleLineHeight;
            dynamicPosition.x -= sizeX;
            dynamicPosition.x -= horizontalSpacing;

            nameProp.stringValue = EditorGUI.TextField(dynamicPosition, nameProp.stringValue);
            dynamicPosition.x += sizeX;
            dynamicPosition.x += (horizontalSpacing - 1) / 2;

            dynamicPosition.width = 1;
            dynamicPosition.height -= 2;
            EditorGUI.DrawRect(dynamicPosition, new Color(1, 1, 1, 0.2f));
            dynamicPosition.width = sizeX;
            dynamicPosition.height += 2;

            dynamicPosition.x += (horizontalSpacing - 1) / 2;

            EditorGUI.PropertyField(dynamicPosition, valueProp, new GUIContent() { text = "" });

            EditorGUI.EndProperty();
        }
    }
}
