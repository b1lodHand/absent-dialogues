using UnityEditor;

namespace com.absence.dialoguesystem.editor
{
    [CustomEditor(typeof(Dialogue), false)]
    public class DialogueCustomEditor : Editor
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
