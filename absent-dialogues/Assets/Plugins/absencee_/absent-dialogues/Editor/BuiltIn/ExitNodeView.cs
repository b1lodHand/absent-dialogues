using com.absence.dialoguesystem.internals;
using UnityEngine.UIElements;

namespace com.absence.dialoguesystem.editor.internals
{
    [CustomNodeView(typeof(ExitNode))]
    public class ExitNodeView : NodeView
    {
        public ExitNodeView(Node node, DialogueGraphView graph = null) : base(node, graph)
        {
        }

        internal override void ApplyHardcodedStyle(EditorSettings settings)
        {
            this.Q("node-border").style.borderTopColor = settings.ThemeColor;
            this.Q("node-icon").style.unityBackgroundImageTintColor = settings.ThemeColor;
        }
    }
}
