using com.absence.dialoguesystem.internals;
using System.Linq;
using UnityEditor;
using UnityEngine.UIElements;

namespace com.absence.dialoguesystem.editor
{
    [CustomNodeView(typeof(GotoNode))]
    public sealed class GotoNodeView : NodeView
    {
        private GotoNode m_nodeAsGoto;

        private DropdownField m_dropdown;

        public GotoNodeView(Node node, DialogueGraphView graph = null) : base(node, graph)
        {
            DialogueEditorWindow.Current.m_inspectorView.OnNodeValidation -= Refresh;
            DialogueEditorWindow.Current.m_inspectorView.OnNodeValidation += Refresh;

            Refresh();
        }

        protected override void OnDraw()
        {
            m_nodeAsGoto = Node as GotoNode;

            DropdownField gotoDropdown = new DropdownField();
            gotoDropdown.name = "goto-dropdown";
            gotoDropdown.AddToClassList("goto-field");
            m_dropdown = gotoDropdown;

            gotoDropdown.RegisterValueChangedCallback(evt =>
            {
                Undo.RecordObject(m_nodeAsGoto, "Node (Person Modified)");

                SectionNode targetNode = Graph.m_dialogue.GetSectionsWithName(evt.newValue).FirstOrDefault();
                if (targetNode != null) m_nodeAsGoto.TargetNode = targetNode;

                EditorUtility.SetDirty(m_nodeAsGoto);
            });

            this.Add(gotoDropdown);
        }

        private void SoftRefresh()
        {
            m_dropdown.SetValueWithoutNotify(m_nodeAsGoto.TargetNode.DialoguePartName);
        }

        private void Refresh()
        {
            m_dropdown.choices.Clear();

            Graph.m_dialogue.GetAllSections().ForEach(dialoguePartNode =>
            {
                m_dropdown.choices.Add(dialoguePartNode.DialoguePartName);
            });

            if (m_dropdown.choices.Count == 0)
            {
                m_dropdown.SetValueWithoutNotify("None");
                return;
            }

            if (Graph.m_dialogue.GetAllSections().Contains(m_nodeAsGoto.TargetNode)) SoftRefresh();
            else m_dropdown.SetValueWithoutNotify("Select a DialoguePartNode.");
        }
    }
}