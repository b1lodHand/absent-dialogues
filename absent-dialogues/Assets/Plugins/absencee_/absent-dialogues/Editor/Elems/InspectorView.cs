using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace com.absence.dialoguesystem.editor
{
    /// <summary>
    /// A visual element subtype which is responsible for rendering a node's inspector properties when selected.
    /// </summary>
    [HelpURL("https://b1lodhand.github.io/absent-dialogues/api/com.absence.dialoguesystem.editor.InspectorView.html")]
    public class InspectorView : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<InspectorView, VisualElement.UxmlTraits> { }

        Editor editor;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public InspectorView()
        {

        }

        internal void UpdateSelection(NodeView nodeView)
        {
            Clear();
            UnityEngine.Object.DestroyImmediate(editor);

            if (nodeView == null) return;

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