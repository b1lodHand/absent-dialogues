using com.absence.attributes.editor;
using com.absence.dialoguesystem.internals;
using UnityEditor;
using UnityEngine;

namespace com.absence.dialoguesystem.editor.internals
{
    [CustomPropertyDrawer(typeof(Option), true)]
    public class OptionPropertyDrawer : PropertyDrawer
    {
        const int k_constantLineCount = 3;
        const float k_customDataHeight = 100f;
        const float k_buttonWidth = 40f;
        const float k_majorSpacing = 10f;
        const float k_customDataPadding = 0f;

        protected virtual int NewButtonId => 1803;
        protected virtual int DelButtonId => 1802;

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
                return (k_constantLineCount * (spacing + height));

            int totalLines = k_constantLineCount;

            SerializedProperty visibilityProp = property.FindPropertyRelative("Visibility");
            SerializedProperty showIfArrayProp = visibilityProp.FindPropertyRelative("ShowIfList");

            float addition = 0f;

            if (customData != null && customDataProp.isExpanded) addition += k_customDataHeight + (k_customDataPadding * 2) + spacing + k_majorSpacing;

            int arraySize = showIfArrayProp.arraySize;

            if (showIf) totalLines += 2;
            if (showIf && showIfArrayProp.isExpanded) totalLines += arraySize + 2;
            if (showIf && showIfArrayProp.isExpanded && arraySize == 0) totalLines += 1;

            return (totalLines * (spacing + height)) + addition;
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
            foldout = DrawHeaderFoldout(position, property, label, foldout);

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

            customDataProp.isExpanded = EditorGUI.Foldout(position, customDataProp.isExpanded, "", true, GUI.skin.label);

            EditorGUI.PropertyField(position, customDataProp);

            position.x += normalWidth - k_buttonWidth + spacing;
            position.width = k_buttonWidth - spacing;

            if (customData == null)
            {
                if (GUI.Button(position, "New"))
                {
                    bool success = FieldButtonManager.Invoke(NewButtonId, out object output, property.serializedObject.targetObject, property.boxedValue);
                    if (success)
                    {
                        NodeCustomDataBase realOutput = (NodeCustomDataBase)output;
                        customDataProp.objectReferenceValue = realOutput;
                    }
                }
            }

            else
            {
                if (GUI.Button(position, "Del"))
                {
                    bool success = FieldButtonManager.Invoke(DelButtonId, property.serializedObject.targetObject, property.boxedValue);
                    if (success)
                    {
                        customDataProp.objectReferenceValue = null;
                    }
                }
            }

            position.x = normalX;
            position.width = normalWidth;

            position.y += step;
            position.height = k_customDataHeight;

            if (customData != null && customDataProp.isExpanded)
            {
                position.y -= k_customDataPadding / 2;
                position.height += k_customDataPadding;

                EditorGUI.DrawRect(position, color);

                position.y += k_customDataPadding / 2;
                position.height -= k_customDataPadding;

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
                position.y += k_majorSpacing;
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

        protected virtual bool DrawHeaderFoldout(Rect position, SerializedProperty property, GUIContent label, bool foldout)
        {
            SerializedProperty textProp = property.FindPropertyRelative("Text");

            string text = textProp.stringValue;
            if (string.IsNullOrWhiteSpace(text)) text = $"<color=grey>{Constants.Text.NO_TEXT}</color>";

            GUIStyle foldoutStyle = new GUIStyle(EditorStyles.foldout)
            {
                richText = true,
            };

            return EditorGUI.Foldout(position, foldout, text, true, foldoutStyle);
        }
    }
}
