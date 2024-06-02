using UnityEditor;
using UnityEngine;

namespace com.absence.dialoguesystem.editor
{
    /// <summary>
    /// Custom editor script for dialogues.
    /// </summary>
    [CustomEditor(typeof(Dialogue), false)]
    [HelpURL("https://b1lodhand.github.io/absent-dialogues/api/com.absence.dialoguesystem.editor.DialogueCustomEditor.html")]
    public sealed class DialogueCustomEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            Dialogue dialogue = target as Dialogue;

            EditorGUI.BeginChangeCheck();

            base.OnInspectorGUI();

            if(EditorGUI.EndChangeCheck())
            {
                dialogue.PerformEditorRefresh();
            }
        }
    }
}
