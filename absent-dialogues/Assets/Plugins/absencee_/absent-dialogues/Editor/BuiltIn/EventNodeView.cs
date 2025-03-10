using com.absence.dialoguesystem.internals;
using System.Text;
using UnityEngine;
using UnityEngine.UIElements;

namespace com.absence.dialoguesystem.editor.internals
{
    [CustomNodeView(typeof(EventNode))]
    public class EventNodeView : NodeView
    {
        EventNode m_nodeAsEvent;

        public EventNodeView(Node node, DialogueGraphView graph = null) : base(node, graph)
        {
            node.onValidation -= Refresh;
            node.onValidation += Refresh;
        }

        protected override void OnAfterStylesApplied()
        {
            m_nodeAsEvent = Node as EventNode;
        }

        internal override void ApplyHardcodedStyle(EditorSettings settings)
        {
            m_nodeIcon.style.unityBackgroundImageTintColor = m_nodeAsEvent.UsedByMapper ?
                settings.ThemeColor : Color.white;
        }

        private void Refresh()
        {
            if (m_nodeAsEvent.UsedByMapper)
            {
                AddToClassList("mapped");
                m_titleText.text = m_nodeAsEvent.UniqueMapperId;
            }

            else
            {
                RemoveFromClassList("mapped");
                m_titleText.text = m_nodeAsEvent.Title;
            }

            if (m_nodeAsEvent.UsedByMapper)
            {
                StringBuilder sb = new();

                sb.Append(m_nodeAsEvent.UniqueMapperId);

                string defaultDescription = m_nodeAsEvent.GenerateIconTooltip();

                if (!string.IsNullOrWhiteSpace(defaultDescription))
                {
                    sb.Append("\n\n");
                    sb.Append(defaultDescription);
                }

                m_nodeIcon.tooltip = sb.ToString();
            }

            else
            {
                m_nodeIcon.tooltip = m_nodeAsEvent.GenerateIconTooltip();
            }

            ApplyHardcodedStyle(EditorSettings.instance);
        }
    }
}
