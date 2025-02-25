using com.absence.dialoguesystem.internals;

namespace com.absence.dialoguesystem.editor
{
    [CustomNodeView(typeof(DialoguePartNode))]
    public class SectionNodeView : NodeView
    {
        public SectionNodeView(Node node, DialogueGraphView graph = null) : base(node, graph)
        {
            node.onValidation -= RefreshDialoguePartFinder;
            node.onValidation += RefreshDialoguePartFinder;

            node.onValidation += RefreshDialoguePartTitle;
            node.onValidation += RefreshDialoguePartTitle;
        }
    }
}
