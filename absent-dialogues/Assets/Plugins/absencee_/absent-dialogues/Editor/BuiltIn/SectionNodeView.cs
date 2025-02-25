using com.absence.dialoguesystem.internals;
using UnityEngine.UIElements;

namespace com.absence.dialoguesystem.editor
{
    [CustomNodeView(typeof(SectionNode))]
    public class SectionNodeView : NodeView
    {
        public SectionNodeView(Node node, DialogueGraphView graph = null) : base(node, graph)
        {
            node.onValidation -= RefreshFinder;
            node.onValidation += RefreshFinder;

            node.onValidation += RefreshTitle;
            node.onValidation += RefreshTitle;
        }

        private void RefreshFinder()
        {
            DialogueEditorWindow.RefreshDialoguePartFinder();
        }

        private void RefreshTitle()
        {
            SectionNode nodeAsDp = Node as SectionNode;
            Label title = this.Q<Label>("title-label");

            string dpName = nodeAsDp.DialoguePartName;

            if (string.IsNullOrWhiteSpace(dpName)) title.text = nodeAsDp.Title;
            else title.text = dpName;
        }
    }
}
