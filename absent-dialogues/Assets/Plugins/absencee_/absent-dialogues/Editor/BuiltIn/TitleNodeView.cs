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

        public TitleNodeView(Node node, DialogueGraphView graph = null) : base(node, graph)
        {
            node.onValidation -= OnNodeValidation;
            node.onValidation += OnNodeValidation;

            m_defaultTextField.RegisterValueChangedCallback(OnTitleValueChange);
        }

        private void OnTitleValueChange(ChangeEvent<string> evt)
        {
            if (string.IsNullOrWhiteSpace(evt.newValue))
            {
                m_defaultTextField.value = Constants.Text.NO_TEXT;
            }
        }

        protected override void OnAfterStylesApplied()
        {
            m_nodeAsTitle = Node as TitleNode;
            m_textInput = m_defaultTextField.Q("unity-text-input");
            m_parent = m_textInput.parent;
        }

        private void OnNodeValidation()
        {
            m_textInput.style.fontSize = m_nodeAsTitle.m_fontSize;
            m_defaultTextFieldTextElement.enableRichText = m_nodeAsTitle.m_richText;

            int index = m_parent.IndexOf(m_textInput);
            m_parent.Remove(m_textInput);
            m_parent.Insert(index, m_textInput);
        }
    }
}
