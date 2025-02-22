using com.absence.attributes.editor;
using com.absence.dialoguesystem.internals;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace com.absence.dialoguesystem.editor
{
    [CustomPropertyDrawer(typeof(Option), true)]
    public class OptionPropertyDrawer : PropertyDrawer
    {
        const int k_constantLineCount = 3;
        const float k_customDataHeight = 100f;
        const float k_buttonWidth = 50f;

        Editor lastEditor;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float spacing = EditorGUIUtility.standardVerticalSpacing;
            float height = EditorGUIUtility.singleLineHeight;

            if (!property.isExpanded)
                return spacing + height;

            SerializedProperty useShowIfProp = property.FindPropertyRelative("m_useShowIf");
            SerializedProperty customDataProp = property.FindPropertyRelative("CustomData");

            bool showIf = useShowIfProp.boolValue;
            UnityEngine.Object customData = customDataProp.objectReferenceValue;

            if (customData == null && !showIf)
                return k_constantLineCount * (spacing + height);

            int totalLines = k_constantLineCount;

            SerializedProperty visibilityProp = property.FindPropertyRelative("Visibility");
            SerializedProperty showIfArrayProp = visibilityProp.FindPropertyRelative("ShowIfList");

            float addition = 0f;

            if (customData != null) addition += k_customDataHeight;

            int arraySize = showIfArrayProp.arraySize;

            if (showIf) totalLines += arraySize + 4;

            if (arraySize == 0) totalLines += 1;

            return (totalLines * (spacing + height)) + (addition + spacing);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty scrollProp = property.FindPropertyRelative("m_scroll");
            SerializedProperty textProp = property.FindPropertyRelative("Text");
            SerializedProperty useShowIfProp = property.FindPropertyRelative("m_useShowIf");
            SerializedProperty customDataProp = property.FindPropertyRelative("CustomData");
            SerializedProperty visibilityProp = property.FindPropertyRelative("Visibility");
            SerializedProperty processorProp = visibilityProp.FindPropertyRelative("Processor");
            SerializedProperty showIfArrayProp = visibilityProp.FindPropertyRelative("ShowIfList");

            float spacing = EditorGUIUtility.standardVerticalSpacing;
            float height = EditorGUIUtility.singleLineHeight;
            float step = spacing + height;

            EditorGUI.BeginProperty(position, label, property);

            position.height = EditorGUIUtility.singleLineHeight;

            bool foldout = property.isExpanded;
            foldout = EditorGUI.Foldout(position, foldout, textProp.stringValue, true);
            property.isExpanded = foldout;

            if (!foldout) 
            {
                EditorGUI.EndProperty();
                return; 
            }

            EditorGUI.indentLevel++;

            position = EditorGUI.IndentedRect(position);
            position.y += step;
            
            UnityEngine.Object customData = customDataProp.objectReferenceValue;

            Color color = Color.black;
            color.a = 0.1f;

            if (customData != null) EditorGUI.DrawRect(position, color);

            float normalX = position.x;
            float normalWidth = position.width;
            position.width -= k_buttonWidth;

            EditorGUI.PropertyField(position, customDataProp);

            position.x += normalWidth - k_buttonWidth + spacing;
            position.width = k_buttonWidth - spacing;

            if (customData == null)
            {
                if (GUI.Button(position, "New"))
                {
                    FieldButtonManager.Invoke(1803, property.serializedObject.targetObject, property.boxedValue);
                }
            }

            else
            {
                if (GUI.Button(position, "Del"))
                {
                    FieldButtonManager.Invoke(1802, property.serializedObject.targetObject, property.boxedValue);
                }
            }

            position.x = normalX;
            position.width = normalWidth;

            position.y += step;
            position.height = k_customDataHeight;

            if (customData != null)
            {
                EditorGUI.DrawRect(position, color);

                Vector2 scroll = scrollProp.vector2Value;

                SerializedObject so = new SerializedObject(customData);

                SerializedProperty iterator = so.GetIterator();
                Rect total = position;
                total.height = 0f;
                bool enterChildren = true;
                while (iterator.NextVisible(enterChildren))
                {
                    using (new EditorGUI.DisabledScope("m_Script" == iterator.propertyPath))
                    {
                        total.height += EditorGUI.GetPropertyHeight(iterator, true);
                    }

                    enterChildren = false;
                }

                using (GUI.ScrollViewScope scope = new GUI.ScrollViewScope(position, scroll, total))
                {
                    DoDrawDefaultInspector(position, so);
                    scrollProp.vector2Value = scope.scrollPosition;
                }

                position.y += k_customDataHeight;
            }

            position.height = height;

            GUIContent toggleContent = new GUIContent()
            {
                text = "Conditional Visibility",
                tooltip = visibilityProp.tooltip,
            };

            bool showIf = useShowIfProp.boolValue;
            showIf = EditorGUI.ToggleLeft(position, toggleContent, showIf);
            useShowIfProp.boolValue = showIf;

            if (!showIf)
            {
                EditorGUI.indentLevel--;
                EditorGUI.EndProperty();
                return;
            }

            position.y += step;

            EditorGUI.PropertyField(position, processorProp);

            position.y += step;

            EditorGUI.PropertyField(position, showIfArrayProp);

            EditorGUI.indentLevel--;
            EditorGUI.EndProperty();
        }

        private static bool DoDrawDefaultInspector(Rect position, SerializedObject obj)
        {
            position.height = EditorGUIUtility.singleLineHeight;

            EditorGUI.BeginChangeCheck();
            obj.UpdateIfRequiredOrScript();
            SerializedProperty iterator = obj.GetIterator();
            bool enterChildren = true;
            while (iterator.NextVisible(enterChildren))
            {
                using (new EditorGUI.DisabledScope("m_Script" == iterator.propertyPath))
                {
                    EditorGUI.PropertyField(position, iterator, true);
                    position.y += EditorGUI.GetPropertyHeight(iterator, true);
                    position.y += EditorGUIUtility.standardVerticalSpacing;
                }

                enterChildren = false;
            }

            obj.ApplyModifiedProperties();
            return EditorGUI.EndChangeCheck();
        }
    }
}
