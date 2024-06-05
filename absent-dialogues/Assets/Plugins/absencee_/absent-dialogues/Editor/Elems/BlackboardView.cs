using com.absence.variablebanks;
using com.absence.dialoguesystem.internals;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace com.absence.dialoguesystem.editor
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
        Editor m_blackboardBankEditor;

        internal void Initialize(SerializedObject dialogue)
        {
            Clear();

            if (dialogue == null) return;

            IMGUIContainer container = new IMGUIContainer(() =>
            {
                DrawGUI(dialogue);
            });

            Add(container);
        }

        void DrawGUI(SerializedObject dialogue)
        {
            SerializedProperty blackboardProperty = dialogue.FindProperty("Blackboard");
            if (blackboardProperty == null) return;

            dialogue.Update();

            EditorGUILayout.PropertyField(blackboardProperty);

            SerializedProperty bankProp = blackboardProperty.FindPropertyRelative("Bank");

            VariableBank bank = bankProp.objectReferenceValue as VariableBank;
            SerializedObject bankSO = new SerializedObject(bank);

            Undo.RecordObject(dialogue.targetObject, "Dialogue");

            if (bank == null)
            {
                EditorGUILayout.ObjectField(bankProp);
                EditorGUILayout.HelpBox("There is no bank to edit here. Pick one to continue.", MessageType.Warning);
            }

            dialogue.ApplyModifiedProperties();

            if (bank == null) return;

            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Bank: ");

            Undo.RecordObject(bank, "Blackboard Bank");

            m_blackboardViewScrollPos = EditorGUILayout.BeginScrollView(m_blackboardViewScrollPos);

            if (bank == null) return;

            try
            {
                if (m_blackboardBankEditor == null) Editor.CreateCachedEditor(bank, null, ref m_blackboardBankEditor);
                else if (!m_blackboardBankEditor.serializedObject.targetObject.Equals(bank)) Editor.CreateCachedEditor(bank, null, ref m_blackboardBankEditor);
                else m_blackboardBankEditor.OnInspectorGUI();
            }

            catch
            {
                Editor.CreateCachedEditor(bank, null, ref m_blackboardBankEditor);
            }


            EditorGUILayout.EndScrollView();

            bankSO.ApplyModifiedProperties();
            dialogue.ApplyModifiedProperties();
        }
    }
}
