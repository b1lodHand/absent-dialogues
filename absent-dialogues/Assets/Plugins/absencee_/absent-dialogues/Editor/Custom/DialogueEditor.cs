using com.absence.attributes.editor;
using UnityEditor;
using UnityEngine;

namespace com.absence.dialoguesystem.editor.internals
{
    [CustomEditor(typeof(Dialogue))]
    public class DialogueEditor : Editor
    {
        private const float k_buttonWidth = 21f;
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
            Dialogue dialogue = (Dialogue)target;

            serializedObject.Update();

            SerializedProperty optionListProp = serializedObject.FindProperty("m_genericOptions");

            //float height = EditorGUIUtility.singleLineHeight;
            //float spacing = EditorGUIUtility.standardVerticalSpacing;

            bool foldout = optionListProp.isExpanded;

            GUIContent foldoutContent = new GUIContent()
            {
                text = optionListProp.displayName,
                tooltip = optionListProp.tooltip,
            };

            GUIStyle buttonStyle = new GUIStyle(GUI.skin.button)
            {
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter,
            };

            EditorGUI.BeginChangeCheck();

            Undo.RecordObject(target, "Dialogue (Editor)");

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

                int arraySize = optionListProp.arraySize;
                int lastIndex = arraySize > 0 ? arraySize - 1 : 0;
                for (int i = 0; i < arraySize; i++)
                {
                    EditorGUILayout.BeginHorizontal();

                    Color prevColor = GUI.backgroundColor;
                    GUI.backgroundColor = new Color(171f/255f, 68f/255f, 63f/255f, 255f);
                    bool remove = GUILayout.Button("×", buttonStyle, GUILayout.Width(k_buttonWidth));
                    GUI.backgroundColor = prevColor;

                    if (i == 0) GUI.enabled = false;
                    bool moveUp = GUILayout.Button("↑", buttonStyle, GUILayout.Width(k_buttonWidth));
                    if (i == 0) GUI.enabled = true;

                    if (i == lastIndex) GUI.enabled = false;
                    bool moveDown = GUILayout.Button("↓", buttonStyle, GUILayout.Width(k_buttonWidth));
                    if (i == lastIndex) GUI.enabled = true;

                    if (remove)
                    {
                        Undo.SetCurrentGroupName("Dialogue (Generic Option Removed)");
                        int group = Undo.GetCurrentGroup();

                        Undo.RegisterCompleteObjectUndo(dialogue, "Dialogue (Generic Option Removed)");
                        optionListProp.DeleteArrayElementAtIndex(i);

                        serializedObject.ApplyModifiedProperties();

                        EditorUtility.SetDirty(dialogue);
                        AssetDatabase.SaveAssetIfDirty(dialogue);

                        if (DialogueEditorWindow.Current != null)
                        {
                            DialogueGraphView graph = DialogueEditorWindow.Current.m_dialogueGraphView;
                            dialogue.AllNodes.ForEach(node =>
                            {
                                if (!node.UseGenericOptions)
                                    return;

                                graph.FindNodeView(node).OnGenericOptionRemoved(i);
                            });
                        }

                        Undo.CollapseUndoOperations(group);

                        dialogue.InvokeOnGenericOptionRemoved(i);
                    }

                    else if (moveUp)
                    {
                        Undo.SetCurrentGroupName("Dialogue (Generic Options Rearranged)");
                        int group = Undo.GetCurrentGroup();

                        Undo.RegisterCompleteObjectUndo(dialogue, "Dialogue (Generic Options Rearranged)");
                        optionListProp.MoveArrayElement(i, i - 1);

                        serializedObject.ApplyModifiedProperties();

                        EditorUtility.SetDirty(dialogue);
                        AssetDatabase.SaveAssetIfDirty(dialogue);

                        if (DialogueEditorWindow.Current != null)
                        {
                            DialogueGraphView graph = DialogueEditorWindow.Current.m_dialogueGraphView;
                            dialogue.AllNodes.ForEach(node =>
                            {
                                if (!node.UseGenericOptions)
                                    return;

                                graph.FindNodeView(node).OnGenericOptionsRearranged(i, i - 1);
                            });
                        }

                        Undo.CollapseUndoOperations(group);

                        dialogue.InvokeOnGenericOptionsRearranged(i, i - 1);
                    }

                    else if (moveDown)
                    {
                        Undo.SetCurrentGroupName("Dialogue (Generic Options Rearranged)");
                        int group = Undo.GetCurrentGroup();

                        Undo.RegisterCompleteObjectUndo(dialogue, "Dialogue (Generic Options Rearranged)");
                        optionListProp.MoveArrayElement(i, i + 1);

                        serializedObject.ApplyModifiedProperties();

                        EditorUtility.SetDirty(dialogue);
                        AssetDatabase.SaveAssetIfDirty(dialogue);

                        if (DialogueEditorWindow.Current != null)
                        {
                            DialogueGraphView graph = DialogueEditorWindow.Current.m_dialogueGraphView;
                            dialogue.AllNodes.ForEach(node =>
                            {
                                if (!node.UseGenericOptions)
                                    return;

                                graph.FindNodeView(node).OnGenericOptionsRearranged(i, i + 1);
                            });
                        }

                        Undo.CollapseUndoOperations(group);

                        dialogue.InvokeOnGenericOptionsRearranged(i, i + 1);
                    }

                    if (remove || moveUp || moveDown)
                    {
                        EditorGUILayout.EndHorizontal();

                        serializedObject.ApplyModifiedProperties();
                        serializedObject.Update();

                        EditorUtility.SetDirty(dialogue);
                        AssetDatabase.SaveAssetIfDirty(dialogue);

                        dialogue.InvokeOnGenericOptionsChange();
                        return;
                    }

                    EditorGUILayout.PropertyField(optionListProp.GetArrayElementAtIndex(i), true);

                    EditorGUILayout.EndHorizontal();
                }

                bool addNewOne = GUILayout.Button("+", buttonStyle, GUILayout.Width(k_buttonWidth));

                if (addNewOne)
                {
                    int group = Undo.GetCurrentGroup();
                    Undo.SetCurrentGroupName("Dialogue (Generic Option Created)");

                    Undo.RegisterCompleteObjectUndo(dialogue, "Dialogue (Generic Option Created)");
                    optionListProp.InsertArrayElementAtIndex(lastIndex);

                    serializedObject.ApplyModifiedProperties();

                    EditorUtility.SetDirty(dialogue);
                    AssetDatabase.SaveAssetIfDirty(dialogue);

                    int newIndex = lastIndex + 1;

                    if (DialogueEditorWindow.Current != null)
                    {
                        DialogueGraphView graph = DialogueEditorWindow.Current.m_dialogueGraphView;
                        dialogue.AllNodes.ForEach(node =>
                        {
                            if (!node.UseGenericOptions)
                                return;

                            int index = arraySize == 0 ? 0 : newIndex;
                            graph.FindNodeView(node).OnGenericOptionCreated(index);
                        });
                    }

                    Undo.CollapseUndoOperations(group);

                    if (arraySize == 0)
                    {
                        dialogue.InvokeOnGenericOptionCreated(0);
                        dialogue.InvokeOnGenericOptionsChange();
                        return;
                    }

                    serializedObject.Update();

                    SerializedProperty newOptionProp = optionListProp.GetArrayElementAtIndex(newIndex);
                    newOptionProp.FindPropertyRelative("CustomData").objectReferenceValue = null;

                    dialogue.InvokeOnGenericOptionCreated(newIndex);
                    dialogue.InvokeOnGenericOptionsChange();
                }

                EditorGUI.indentLevel--;
            }
        }
    }
}
