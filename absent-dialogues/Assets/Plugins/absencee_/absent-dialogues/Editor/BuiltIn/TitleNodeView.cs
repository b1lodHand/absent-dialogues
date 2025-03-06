using com.absence.dialoguesystem.internals;
using UnityEngine.UIElements;

namespace com.absence.dialoguesystem.editor.internals
{
    [CustomNodeView(typeof(TitleNode))]
    public class TitleNodeView : NodeView
    {
        TitleNode m_nodeAsTitle;
        VisualElement m_textInput;
        VisualElement m_parent;
        TextField m_text;

        public TitleNodeView(Node node, DialogueGraphView graph = null) : base(node, graph)
        {
            node.onValidation -= OnNodeValidation;
            node.onValidation += OnNodeValidation;

            m_text.RegisterValueChangedCallback(OnTitleValueChange);
        }

        private void OnTitleValueChange(ChangeEvent<string> evt)
        {
            if (string.IsNullOrWhiteSpace(evt.newValue))
            {
                m_text.value = Constants.Text.NO_TEXT;
            }
        }

        protected override void OnAfterStylesApplied()
        {
            m_nodeAsTitle = Node as TitleNode;
            m_text = this.Q("speech") as TextField;
            m_textInput = m_text.Q("unity-text-input");
            m_parent = m_textInput.parent;
        }

        private void OnNodeValidation()
        {
            m_textInput.style.fontSize = m_nodeAsTitle.m_fontSize;
            int index = m_parent.IndexOf(m_textInput);
            m_parent.Remove(m_textInput);
            m_parent.Insert(index, m_textInput);
        }
    }
}
