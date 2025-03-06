using com.absence.attributes.editor;
using com.absence.dialoguesystem.internals;
using UnityEditor;
using UnityEngine;

namespace com.absence.dialoguesystem.editor.internals
{
    [CustomEditor(typeof(PromptNode), true, isFallback = false)]
    public class PromptNodeEditor : Editor
    {
        Editor initialEditor;

        private void OnEnable()
        {
            Editor.CreateCachedEditor(target, typeof(absentEditorExtension), ref initialEditor);
        }

        private void OnDisable()
        {
            Editor.DestroyImmediate(initialEditor);
            initialEditor = null;
        }

        public override void OnInspectorGUI()
        {
            initialEditor.OnInspectorGUI();

            serializedObject.Update();

            SerializedProperty optionListProp = serializedObject.FindProperty("m_options");

            bool foldout = optionListProp.isExpanded;

            GUIContent foldoutContent = new GUIContent()
            {
                text = optionListProp.displayName,
                tooltip = optionListProp.tooltip,
            };

            EditorGUI.BeginChangeCheck();

            Undo.RecordObject(target, "Prompt Node (Editor)");

            DrawOptionList();

            if (EditorGUI.EndChangeCheck())
            {
                optionListProp.isExpanded = foldout;

                serializedObject.ApplyModifiedProperties();
                EditorUtility.SetDirty(target);
            }

            return;

            void DrawOptionList()
            {
                foldout = EditorGUILayout.Foldout(foldout, foldoutContent, true);

                if (!foldout)
                    return;

                EditorGUI.indentLevel++;

                for (int i = 0; i < optionListProp.arraySize; i++) 
                {
                    EditorGUILayout.PropertyField(optionListProp.GetArrayElementAtIndex(i), true);                
                }

                EditorGUI.indentLevel--;
            }
        }
    }

}