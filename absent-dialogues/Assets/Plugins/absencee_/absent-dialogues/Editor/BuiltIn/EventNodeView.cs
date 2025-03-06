using com.absence.dialoguesystem.internals;
using System.Text;
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

        private void Refresh()
        {
            VisualElement icon = this.Q<VisualElement>("node-icon");
            Label title = this.Q<Label>("title-label");

            if (m_nodeAsEvent.UsedByMapper)
            {
                AddToClassList("mapped");
                title.text = m_nodeAsEvent.UniqueMapperId;
            }

            else
            {
                RemoveFromClassList("mapped");
                title.text = m_nodeAsEvent.Title;
            }

            if (m_nodeAsEvent.UsedByMapper)
            {
                StringBuilder sb = new();

                sb.Append(m_nodeAsEvent.UniqueMapperId);
                sb.Append("\n\n");
                sb.Append(m_nodeAsEvent.GenerateIconTooltip());

                icon.tooltip = sb.ToString();
            }

            else
            {
                icon.tooltip = m_nodeAsEvent.GenerateIconTooltip();
            }
        }
    }
}
