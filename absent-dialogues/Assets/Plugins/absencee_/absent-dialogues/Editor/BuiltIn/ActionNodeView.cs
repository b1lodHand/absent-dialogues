using com.absence.dialoguesystem.internals;
using UnityEngine.UIElements;

namespace com.absence.dialoguesystem.editor
{
    [CustomNodeView(typeof(EventNode))]
    public class ActionNodeView : NodeView
    {
        public ActionNodeView(Node node, DialogueGraphView graph = null) : base(node, graph)
        {
            node.onValidation -= Refresh;
            node.onValidation += Refresh;
        }

        private void Refresh()
        {
            EventNode nodeAsAction = Node as EventNode;
            VisualElement icon = this.Q<VisualElement>("node-icon");
            Label title = this.Q<Label>("title-label");

            if (nodeAsAction.UsedByMapper)
            {
                AddToClassList("mapped");
                title.text = nodeAsAction.UniqueMapperId;
            }

            else
            {
                RemoveFromClassList("mapped");
                title.text = nodeAsAction.Title;
            }

            if (nodeAsAction.UsedByMapper) icon.tooltip = nodeAsAction.UniqueMapperId;
            else icon.tooltip = null;
        }
    }
}
