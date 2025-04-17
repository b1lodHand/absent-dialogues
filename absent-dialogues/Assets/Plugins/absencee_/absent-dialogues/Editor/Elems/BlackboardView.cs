using com.absence.dialoguesystem.internals;
using com.absence.variablesystem.banksystembase;
using com.absence.variablesystem.editor;
using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace com.absence.dialoguesystem.editor.internals
{
    /// <summary>
    /// A visual element subtype which is responsible for displaying a <see cref="Blackboard"/>.
    /// </summary>
    [HelpURL("https://b1lodhand.github.io/absent-dialogues/api/com.absence.dialoguesystem.editor.BlackboardView.html")]
    public class BlackboardView : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<BlackboardView, VisualElement.UxmlTraits> { }
        public BlackboardView()
        {

        }

        Vector2 m_blackboardViewScrollPos;
        Dialogue m_dialogue;
        IMGUIContainer m_container;
        Editor m_blackboardBankEditor;

        internal void Initialize(Dialogue dialogue)
        {
            Clear();

            m_dialogue = dialogue;

            if (dialogue == null) return;

            m_container = new IMGUIContainer(() =>
            {
                DrawGUI(dialogue);
            });

            Editor.CreateCachedEditorWithContext(dialogue.Blackboard.Bank, dialogue, null, ref m_blackboardBankEditor);

            Add(m_container);
        }

        void DrawGUI(Dialogue dialogue)
        {
            if (dialogue == null)
            {
                Debug.LogWarning("Associated dialogue graph is somehow deleted.");
                return;
            }

            if (Application.isPlaying) 
                GUI.enabled = false;

            SerializedObject dialogueSO = new SerializedObject(dialogue);

            dialogueSO.Update();
            SerializedProperty blackboardProperty = dialogueSO.FindProperty("Blackboard");
            if (blackboardProperty == null) return;

            EditorGUILayout.PropertyField(blackboardProperty);

            SerializedProperty bankProp = blackboardProperty.FindPropertyRelative("Bank");

            VariableBank bank = bankProp.objectReferenceValue as VariableBank;

            if (bank == null)
            {
                EditorGUILayout.ObjectField(bankProp);
                EditorGUILayout.HelpBox("There is no bank to edit here. Pick one to continue.", MessageType.Warning);
                return;
            }

            m_blackboardViewScrollPos = EditorGUILayout.BeginScrollView(m_blackboardViewScrollPos);

            (m_blackboardBankEditor as VariableBankEditorBase).DrawIMGUI(false);

            EditorGUILayout.EndScrollView();

            dialogueSO.ApplyModifiedProperties();

            if (Application.isPlaying) GUI.enabled = true;
        }
    }
}
