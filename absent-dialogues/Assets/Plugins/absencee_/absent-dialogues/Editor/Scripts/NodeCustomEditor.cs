using com.absence.dialoguesystem.internals;
using UnityEditor;

namespace com.absence.dialoguesystem.editor
{
    [CustomEditor(typeof(Node), true)]
    public class NodeCustomEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            Node node = target as Node;

            EditorGUI.BeginChangeCheck();

            base.OnInspectorGUI();

            if(EditorGUI.EndChangeCheck())
            {
                if (node is IPerformEditorRefresh performer) performer.PerformEditorRefresh();
            }
        }
    }
}