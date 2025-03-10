using com.absence.dialoguesystem.internals;
using UnityEngine;
using UnityEngine.UIElements;

namespace com.absence.dialoguesystem.editor.internals
{
    [CustomNodeView(typeof(BranchNode))]
    public class BranchNodeView : NodeView
    {
        private const float k_topInfoShift = 89f;

        protected BranchNode m_nodeAsBranch;
        protected DropdownField m_dropdown;
        protected VisualElement m_infoBoxElement;
        protected Label m_infoBoxText;

        public BranchNodeView(Node node, DialogueGraphView graph = null) : base(node, graph)
        {
            node.onValidation -= RefreshTooltip;
            node.onValidation += RefreshTooltip;

            m_infoBoxElement = new VisualElement();
            m_infoBoxText = new Label();
            m_infoBoxElement.pickingMode = PickingMode.Ignore;
            m_infoBoxElement.style.position = Position.Absolute;
            m_infoBoxElement.style.maxWidth = this.style.maxWidth;
            m_infoBoxElement.style.minWidth = this.style.minWidth;
            m_infoBoxElement.style.width = this.style.width;
            m_infoBoxText.style.unityTextAlign = TextAnchor.UpperCenter;
            m_infoBoxText.pickingMode = PickingMode.Ignore;
            m_infoBoxText.style.whiteSpace = WhiteSpace.Normal;
            m_infoBoxText.enableRichText = true;

            m_infoBoxElement.Add(m_infoBoxText);
            this.Insert(0, m_infoBoxElement);
        }

        protected override void OnAfterStylesApplied()
        {
            m_nodeAsBranch = Node as BranchNode;
        }

        protected virtual void RefreshTooltip()
        {
            string info = m_nodeAsBranch.GetConditionString(true);
            string verticalInfo = m_nodeAsBranch.GetVerticalConditionString(true);
            m_nodeIcon.tooltip = verticalInfo;
            m_infoBoxElement.style.bottom = k_topInfoShift;
            m_infoBoxText.text = verticalInfo;

            BranchNode.TitleTextMode titleMode = m_nodeAsBranch.TitleMode;

            m_infoBoxElement.style.display = titleMode == BranchNode.TitleTextMode.External ?
                DisplayStyle.Flex : DisplayStyle.None;

            if (titleMode == BranchNode.TitleTextMode.Horizontal ||
                titleMode == BranchNode.TitleTextMode.Vertical)
            {
                m_titleText.text = info;
            }

            else
            {
                m_titleText.text = m_nodeAsBranch.Title;
            }
        }
    }
}
