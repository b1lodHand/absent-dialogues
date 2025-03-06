using com.absence.dialoguesystem.internals;
using UnityEngine.UIElements;

namespace com.absence.dialoguesystem.editor.internals
{
    [CustomNodeView(typeof(BranchNode))]
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
            BranchNode nodeAsCondition = Node as BranchNode;
            VisualElement icon = this.Q<VisualElement>("node-icon");

            icon.tooltip = nodeAsCondition.GetConditionString(true);
        }
    }
}
