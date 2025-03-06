using com.absence.dialoguesystem.internals;
using UnityEngine.UIElements;

namespace com.absence.dialoguesystem.editor.internals
{
    [CustomNodeView(typeof(StickyNoteNode))]
    public class StickyNoteNodeView : NodeView
    {
        StickyNoteNode m_nodeAsSticky;
        VisualElement m_textInput;
        VisualElement m_parent;
        TextField m_text;
        TextElement m_textElement;

        public StickyNoteNodeView(Node node, DialogueGraphView graph = null) : base(node, graph)
        {
            m_nodeAsSticky.onValidation -= OnNodeValidation;
            m_nodeAsSticky.onValidation += OnNodeValidation;
        }

        protected override void OnAfterStylesApplied()
        {
            m_nodeAsSticky = Node as StickyNoteNode;
            m_text = this.Q("speech") as TextField;
            m_textInput = m_text.Q("unity-text-input");
            m_textElement = m_textInput.Q<TextElement>();
            m_parent = m_textInput.parent;
        }

        private void OnNodeValidation()
        {
            m_textElement.enableRichText = m_nodeAsSticky.m_richText;

            int index = m_parent.IndexOf(m_textInput);
            m_parent.Remove(m_textInput);
            m_parent.Insert(index, m_textInput);
        }
    }
}
