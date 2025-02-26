using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.UIElements;
using System.Linq;
using com.absence.dialoguesystem.internals;
using Node = com.absence.dialoguesystem.internals.Node;
using com.absence.personsystem;

namespace com.absence.dialoguesystem.editor
{
    /// <summary>
    /// The view class responsible for rendering a node's data in the graph.
    /// </summary>
    [HelpURL("https://b1lodhand.github.io/absent-dialogues/api/com.absence.dialoguesystem.editor.NodeView.html")]
    public class NodeView : UnityEditor.Experimental.GraphView.Node
    {

        /// <summary>
        /// The USS class name for person dependent nodes.
        /// </summary>
        public static string K_PERSONDEPENDENT_CLASSNAME = "personDependent";

        public const string DefaultUXMLFileLocation = "Assets/Plugins/absencee_/absent-dialogues/Editor/Elems/NodeView.uxml";

        public virtual List<string> AdditionalUSSFileLocations => null;

        /// <summary>
        /// Action gets invoked when this node gets selected or unselected.
        /// </summary>
        public Action<NodeView> OnSelect;

        /// <summary>
        /// The node this view displays.
        /// </summary>
        public Node Node;

        /// <summary>
        /// The left-hand side port.
        /// </summary>
        public Port Input;

        /// <summary>
        /// A list of right-hand side ports.
        /// </summary>
        public List<Port> Outputs = new List<Port>();

        protected SerializedObject m_serializedNode;

        /// <summary>
        /// The graph we're in.
        /// </summary>
        public DialogueGraphView Graph { get; internal set; }

        /// <summary>
        /// Use to construct a node view from a node.
        /// </summary>
        /// <param name="node">Target node.</param>
        public NodeView(Node node, DialogueGraphView graph = null) : base(DefaultUXMLFileLocation)
        {
            Type nodeType = node.GetType();

            this.Graph = graph;
            this.Node = node;
            this.viewDataKey = node.Guid;
            this.showInMiniMap = node.ShowInMinimap;

            NodeViewStyles.ApplyStyles(this);

            style.left = node.Position.x;
            style.top = node.Position.y;

            if (Node.PersonDependent) AddToClassList(K_PERSONDEPENDENT_CLASSNAME);

            OnBeforeDraw();

            this.title = node.Title ?? "Node";

            SetupNodeForSerialization();
            SetupPersonDropdownIfExists();
            SetupTextFieldIfExists();

            CreateInputPort();
            CreateOutputPorts();

            DoDraw();

            UpdateState(node.State);

            if (node.PersonDependent)
            {
                Graph.m_dialogue.OnValidateAction -= RefreshPersonDropdown;
                Graph.m_dialogue.OnValidateAction += RefreshPersonDropdown;
            }

            node.onSetState -= UpdateState;
            node.onSetState += UpdateState;

            OnAfterDraw();
        }

        private void DoDraw()
        {
            if (Node is IContainVariableManipulators nodeAsManipulator)
                RefreshVariableManipulators(nodeAsManipulator);

            if (Node.PersonDependent)
                RefreshPersonDropdown();

            Draw();
        }

        #region Protected API
        protected virtual void SetupPersonDropdownIfExists()
        {
            if (!Node.PersonDependent) return;

            DropdownField personDropdown = this.Q<DropdownField>("person-field");
            Image personPreview = new Image();
            personPreview.name = "person-icon-preview";
            personPreview.AddToClassList("personPreview");
            personDropdown.parent.Insert(0, personPreview);

            personDropdown.tooltip = "The person who speaks.";

            personDropdown.RegisterValueChangedCallback(evt =>
            {
                Undo.RecordObject(Node, "Node (Person Modified)");

                Person targetPerson = Graph.m_dialogue.People.Where(p => p.Name == evt.newValue).FirstOrDefault();
                Node.PersonIndex = Graph.m_dialogue.People.IndexOf(targetPerson);

                EditorUtility.SetDirty(Node);

                personPreview.sprite = targetPerson.Icon;
            });
        }
        protected virtual void SetupNodeForSerialization()
        {
            m_serializedNode = new SerializedObject(Node);
        }
        protected virtual void SetupTextFieldIfExists()
        {
            TextField textField = this.Q<TextField>("speech");

            if (!Node.HasText)
            {
                textField.style.display = DisplayStyle.None;
                return;
            }

            textField.bindingPath = "m_text";
            textField.Bind(m_serializedNode);
        }
        protected virtual void CreateInputPort()
        {
            if (Node.GetDefaultInputPortName() == null) return;

            Input = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, typeof(bool));
            Input.portName = Node.GetDefaultInputPortName();
            inputContainer.Add(Input);
        }
        protected virtual void CreateOutputPorts()
        {
            Node.GetDefaultOutputPortNames().ForEach(portName =>
            {
                var port = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(bool));

                port.portName = portName;
                Outputs.Add(port);
                outputContainer.Add(port);
            });
        }
        protected virtual void UpdateState(Node.FlowState state)
        {
            if ((!Graph.m_dialogue.IsClone) || !Application.isPlaying) return;

            RemoveFromClassList("unreached");
            RemoveFromClassList("current");
            RemoveFromClassList("past");

            switch (state)
            {
                case Node.FlowState.Unreached:
                    AddToClassList("unreached");
                    break;
                case Node.FlowState.Current:
                    AddToClassList("current");
                    break;
                case Node.FlowState.Past:
                    AddToClassList("past");
                    break;
                default:
                    AddToClassList("unreached");
                    break;
            }
        }
        protected virtual void RefreshVariableManipulators(IContainVariableManipulators nodeAsManipulator)
        {
            List<NodeVariableComparer> comparers = nodeAsManipulator.GetComparers();
            List<NodeVariableSetter> setters = nodeAsManipulator.GetSetters();

            if (comparers != null && comparers.Count > 0) comparers.ForEach(comparer => comparer.BlackboardBank = Node.Blackboard.Bank);
            if (setters != null && setters.Count > 0) setters.ForEach(setter => setter.BlackboardBank = Node.Blackboard.Bank);
        }
        protected virtual void RefreshPersonDropdown()
        {
            DropdownField personDropdown = this.Q<DropdownField>("person-field");

            if (personDropdown == null)
                return;

            List<string> peopleNameList = Graph.m_dialogue.People.ConvertAll(p =>
            {
                if (p) return p.Name;

                return null;
            });

            Image personIconPreview = personDropdown.parent.Q<Image>("person-icon-preview");

            if (peopleNameList.Count == 0)
            {
                personDropdown.choices = new List<string>();
                personDropdown.SetValueWithoutNotify("None");
                if (personIconPreview != null) personIconPreview.style.display = DisplayStyle.None;
                return;
            }

            personDropdown.choices = new List<string>(peopleNameList);

            if (Node.PersonIndex < 0 || Node.PersonIndex > Graph.m_dialogue.People.Count - 1)
            {
                personDropdown.SetValueWithoutNotify("Missing person...");
                if (personIconPreview != null) personIconPreview.style.display = DisplayStyle.None;
                return;
            }

            if (Graph.m_dialogue.People[Node.PersonIndex])
            {
                personDropdown.SetValueWithoutNotify(Graph.m_dialogue.People[Node.PersonIndex].Name);
                if (personIconPreview != null) personIconPreview.style.display = DisplayStyle.Flex;
                if (personIconPreview != null) personIconPreview.sprite = Graph.m_dialogue.People[Node.PersonIndex].Icon;
            }

            else
            {
                personDropdown.SetValueWithoutNotify("Select a person...");
                if (personIconPreview != null) personIconPreview.style.display = DisplayStyle.None;
            }

        }
        protected virtual void OnBeforeDraw()
        {

        }
        protected virtual void Draw()
        {

        }
        protected virtual void OnAfterDraw()
        {

        }
        #endregion

        #region Graph-Based Methods
        public override void SetPosition(Rect newPos)
        {
            base.SetPosition(newPos);
            Undo.RecordObject(Node, "Dialogue (Set Position)");
            Node.Position.x = newPos.xMin;
            Node.Position.y = newPos.yMin;
            EditorUtility.SetDirty(Node);
        }
        public override void OnSelected()
        {
            base.OnSelected();
            OnSelect?.Invoke(this);
        }
        public override void OnUnselected()
        {
            base.OnUnselected();
            if (Graph.selection.Count == 1) OnSelect?.Invoke(null); //??
        }
        #endregion
    }

}