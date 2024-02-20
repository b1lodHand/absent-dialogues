using UnityEditor;
using UnityEngine.UIElements;

namespace com.absence.dialoguesystem.editor
{
    public class InspectorView : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<InspectorView, VisualElement.UxmlTraits> { }

        Editor editor;
        public InspectorView()
        {

        }

        internal void UpdateSelection(NodeView nodeView)
        {
            Clear();

            UnityEngine.Object.DestroyImmediate(editor);
            editor = Editor.CreateEditor(nodeView.Node);
            IMGUIContainer container = new IMGUIContainer(() =>
            {
                if (editor.target == null) return;
                editor.OnInspectorGUI();
                //editor.CreateInspectorGUI();
            });
            Add(container);
        }
    }

}