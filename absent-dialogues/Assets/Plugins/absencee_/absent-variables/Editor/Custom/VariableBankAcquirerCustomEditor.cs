using UnityEditor;
using UnityEngine;

namespace com.absence.variablesystem.editor
{
    [CustomEditor(typeof(VariableBankAcquirer), false)]
    public class VariableBankAcquirerCustomEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            var guidProp = serializedObject.FindProperty("m_targetGuid");

            if (VariableBankDatabase.NoBanks)
            {
                EditorGUILayout.HelpBox("There are no VariableBanks in your project. Create one to continue", MessageType.Warning);
                return;
            }

            if (Application.isPlaying) GUI.enabled = false;

            string currentGuid = guidProp.stringValue;
            int selectedIndex = VariableBankDatabase.GetIndexOf(currentGuid);
            if (selectedIndex == -1) selectedIndex = 0;

            selectedIndex = EditorGUILayout.Popup(selectedIndex, VariableBankDatabase.GetBankNameList().ToArray());
            GUI.enabled = true;

            string bankName = "null (runtime only)";
            EditorGUILayout.LabelField("Guid: " + currentGuid);

            if(Application.isPlaying && VariableBank.CloningCompleted) bankName = VariableBank.GetClone(currentGuid).name;

            EditorGUILayout.LabelField("Bank: " + bankName);

            guidProp.stringValue = VariableBankDatabase.BanksInAssets[selectedIndex].GUID;

            serializedObject.ApplyModifiedProperties();
        }
    }

}