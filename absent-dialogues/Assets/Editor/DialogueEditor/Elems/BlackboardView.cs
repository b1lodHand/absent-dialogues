using com.absence.dialoguesystem;
using com.absence.dialoguesystem.editor;
using com.absence.variablesystem;
using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class BlackboardView : VisualElement
{
    public new class UxmlFactory : UxmlFactory<BlackboardView, VisualElement.UxmlTraits> { }
    public BlackboardView()
    {

    }

    Vector2 m_blackboardViewScrollPos;
    Editor m_blackboardBankEditor;

    internal void Initialize(Func<SerializedProperty> propertySender)
    {
        Clear();

        SerializedProperty prop = propertySender.Invoke();
        if (prop == null) return;

        IMGUIContainer container = new IMGUIContainer(() =>
        {
            DrawGUI(prop);
        });

        Add(container);
    }

    void DrawGUI(SerializedProperty property)
    {
        if (property == null) return;

        SerializedProperty masterDialogueProp = property.FindPropertyRelative("MasterDialogue");
        Dialogue dialogue = masterDialogueProp.objectReferenceValue as Dialogue;
        SerializedObject dialogueSO = new SerializedObject(dialogue);
        dialogueSO.Update();

        EditorGUILayout.PropertyField(property);

        SerializedProperty bankProp = property.FindPropertyRelative("Bank");

        VariableBank bank = bankProp.objectReferenceValue as VariableBank;
        SerializedObject bankSO = new SerializedObject(bank);

        Undo.RecordObject(dialogue, "Dialogue");

        if (bank == null)
        {
            EditorGUILayout.ObjectField(bankProp);
            EditorGUILayout.HelpBox("There is no bank to edit here. Pick one to continue.", MessageType.Warning);
        }

        dialogueSO.ApplyModifiedProperties();

        if (bank == null) return;

        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("Blackboard Bank: ");

        Undo.RecordObject(bank, "Blackboard Bank");

        m_blackboardViewScrollPos = EditorGUILayout.BeginScrollView(m_blackboardViewScrollPos);

        if (!m_blackboardBankEditor) Editor.CreateCachedEditor(bank, null, ref m_blackboardBankEditor);
        else m_blackboardBankEditor.OnInspectorGUI();

        EditorGUILayout.EndScrollView();

        bankSO.ApplyModifiedProperties();
        dialogueSO.ApplyModifiedProperties();
    }
}
