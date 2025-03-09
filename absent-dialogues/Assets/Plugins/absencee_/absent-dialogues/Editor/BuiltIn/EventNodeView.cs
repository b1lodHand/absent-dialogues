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
            this.Q("node-icon").style.unityBackgroundImageTintColor = m_nodeAsEvent.UsedByMapper ?
                settings.ThemeColor : Color.white;
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

                string defaultDescription = m_nodeAsEvent.GenerateIconTooltip();

                if (!string.IsNullOrWhiteSpace(defaultDescription))
                {
                    sb.Append("\n\n");
                    sb.Append(defaultDescription);
                }

                icon.tooltip = sb.ToString();
            }

            else
            {
                icon.tooltip = m_nodeAsEvent.GenerateIconTooltip();
            }

            ApplyHardcodedStyle(EditorSettings.instance);
        }
    }
}
