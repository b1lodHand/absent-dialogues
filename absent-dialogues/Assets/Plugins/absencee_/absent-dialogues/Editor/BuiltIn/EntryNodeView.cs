using com.absence.dialoguesystem.internals;
using UnityEngine.UIElements;

namespace com.absence.dialoguesystem.editor.internals
{
    [CustomNodeView(typeof(EntryNode))]
    public class EntryNodeView : NodeView
    {
        public EntryNodeView(Node node, DialogueGraphView graph = null) : base(node, graph)
        {
        }

        internal override void ApplyHardcodedStyle(EditorSettings settings)
        {
            m_nodeBorder.style.borderTopColor = settings.ThemeColor;
            m_nodeIcon.style.unityBackgroundImageTintColor = settings.ThemeColor;
        }
    }
}
