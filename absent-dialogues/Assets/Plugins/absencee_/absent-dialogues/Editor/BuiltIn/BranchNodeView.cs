using com.absence.dialoguesystem.internals;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace com.absence.dialoguesystem.editor.internals
{
    [CustomNodeView(typeof(BranchNode))]
    public class BranchNodeView : NodeView
    {
        protected BranchNode m_nodeAsBranch;
        protected DropdownField m_dropdown;

        public BranchNodeView(dialoguesystem.internals.Node node, DialogueGraphView graph = null) : base(node, graph)
        {
            node.onValidation -= RefreshTooltip;
            node.onValidation += RefreshTooltip;
        }

        protected override void OnAfterStylesApplied()
        {
            m_nodeAsBranch = Node as BranchNode;
        }

        protected virtual void RefreshTooltip()
        {
            string info = m_nodeAsBranch.GetConditionString(true);
            m_nodeIcon.tooltip = info;
            m_titleText.parent.tooltip = info;

            RefreshTopInfo();
        }

        internal override void ApplyHardcodedStyle(EditorSettings settings)
        {
            //SetPortColor(Outputs[0], settings.PositiveColor, settings.NeutralColor);
            //SetPortColor(Outputs[1], settings.NegativeColor, settings.NeutralColor);
        }

        internal override bool HasTopInfo => true;
        internal override float TopInfoBottomPosition => 89f;
        internal override float TopInfoRightPosition => -12f;
    }
}
