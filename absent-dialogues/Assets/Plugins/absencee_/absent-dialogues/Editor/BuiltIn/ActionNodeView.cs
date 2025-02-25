using com.absence.dialoguesystem.internals;
using UnityEngine;

namespace com.absence.dialoguesystem.editor
{
    [CustomNodeView(typeof(ActionNode))]
    public class ActionNodeView : NodeView
    {
        public ActionNodeView(Node node, DialogueGraphView graph = null) : base(node, graph)
        {
            node.onValidation -= RefreshActionMapProps;
            node.onValidation += RefreshActionMapProps;
        }
    }
}
