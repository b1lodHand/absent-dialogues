using com.absence.dialoguesystem.internals;
using UnityEngine.UIElements;

namespace com.absence.dialoguesystem.editor
{
    [CustomNodeView(typeof(ConditionNode))]
    public class BranchNodeView : NodeView
    {
        protected DropdownField m_dropdown;

        public BranchNodeView(Node node, DialogueGraphView graph = null) : base(node, graph)
        {
            node.onValidation -= RefreshTooltip;
            node.onValidation += RefreshTooltip;
        }

        protected virtual void RefreshTooltip()
        {
            ConditionNode nodeAsCondition = Node as ConditionNode;
            VisualElement icon = this.Q<VisualElement>("node-icon");

            icon.tooltip = nodeAsCondition.GetConditionString(true);
        }
    }
}
