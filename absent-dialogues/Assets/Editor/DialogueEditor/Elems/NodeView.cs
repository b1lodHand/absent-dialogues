using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.UIElements;

namespace com.absence.dialoguesystem.editor
{
    public class NodeView : UnityEditor.Experimental.GraphView.Node
    {
        public Action<NodeView> OnNodeSelected;
        public Node Node;

        public Port input;
        public List<Port> Outputs = new List<Port>();

        private Button m_createNewOptionButton;
        private List<VisualElement> m_optionElems = new List<VisualElement>();

        protected SerializedObject m_serializedNode;
        private DecisionSpeechNode m_nodeAsDecisive;

        public DialogueGraphView Master { get; internal set; }

        public NodeView(Node node) : base("Assets/Editor/DialogueEditor/Elems/NodeView.uxml")
        {
            this.Node = node;
            node.Guid = GUID.Generate().ToString();
            this.viewDataKey = node.Guid;

            style.left = node.Position.x;
            style.top = node.Position.y;

            AddToClassList(node.GetClassName());
            this.title = node.GetTitle();

            SetupNodeForSerialization();

            DrawElems();
            CreateInputPort();
            CreateOutputPorts();

            SetupSpeechFieldIfExists();

            node.OnSetState -= UpdateState;
            node.OnSetState += UpdateState;
        }

        private void SetupNodeForSerialization()
        {
            m_serializedNode = new SerializedObject(Node);
            if (Node is DecisionSpeechNode) m_nodeAsDecisive = Node as DecisionSpeechNode;
        }
        private void SetupSpeechFieldIfExists()
        {
            TextField speechField = this.Q<TextField>("speech");
            speechField.bindingPath = "Speech";
            speechField.Bind(m_serializedNode);
        }

        private void DrawElems()
        {
            if (Node is DecisionSpeechNode) DrawElems_DecisionSpeechNode();
            else if (Node is GotoNode) DrawElems_GotoNode();
            else if (Node is DialoguePartNode) DrawElems_DialogPartNode();
        }
        private void DrawElems_DialogPartNode()
        {
            Label nameLabel = new Label();
            nameLabel.bindingPath = "DialogPartName";
            nameLabel.Bind(m_serializedNode);

            this.Insert(0, nameLabel);
        }
        private void DrawElems_DecisionSpeechNode()
        {
            m_createNewOptionButton = new Button(CreateOption_DialogPartNode);
            m_createNewOptionButton.text = "Add New Option";
            m_createNewOptionButton.AddToClassList("addNewOptionButton");
            mainContainer.Add(m_createNewOptionButton);

            RefreshOptions_DialogPartNode();
        }
        private void DrawElems_GotoNode()
        {
            Label gotoLabel = new Label();
            gotoLabel.bindingPath = "TargetDialogPartName";
            gotoLabel.Bind(m_serializedNode);

            this.Add(gotoLabel);
        }

        private void CreateInputPort()
        {
            if (Node.GetInputPortNameForCreation() == null) return;

            input = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, typeof(bool));
            input.portName = Node.GetInputPortNameForCreation();
            inputContainer.Add(input);
        }
        private void CreateOutputPorts()
        {
            Node.GetOutputPortNamesForCreation().ForEach(portName =>
            {
                var port = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(bool));

                port.portName = portName;
                Outputs.Add(port);
                outputContainer.Add(port);
            });
        }

        private void UpdateState(Node.NodeState state)
        {
            RemoveFromClassList("unreached");
            RemoveFromClassList("current");
            RemoveFromClassList("past");

            switch (state)
            {
                case Node.NodeState.Unreached:
                    AddToClassList("unreached");
                    break;
                case Node.NodeState.Current:
                    AddToClassList("current");
                    break;
                case Node.NodeState.Past:
                    AddToClassList("past");
                    break;
                default:
                    AddToClassList("unreached");
                    break;
            }
        }

        public override void SetPosition(Rect newPos)
        {
            base.SetPosition(newPos);
            Undo.RecordObject(Node, "Dialog (Set Position)");
            Node.Position.x = newPos.xMin;
            Node.Position.y = newPos.yMin;
            EditorUtility.SetDirty(Node);
        }
        public override void OnSelected()
        {
            base.OnSelected();
            OnNodeSelected?.Invoke(this);
        }

        #region Decision Speech Node
        private void CreateOption_DialogPartNode()
        {
            Option option = new Option();
            m_nodeAsDecisive.Options.Add(option);

            Master.Refresh();
        }
        private void RefreshOptions_DialogPartNode()
        {
            m_optionElems.ForEach(v =>
            {
                if (mainContainer.Contains(v)) mainContainer.Remove(v);

                var port = v.Q<Port>();
                if (Outputs.Contains(port)) Outputs.Remove(port);
            });

            m_optionElems.Clear();

            var optionsProp = m_serializedNode.FindProperty("Options").Copy();

            optionsProp.Next(true);
            optionsProp.Next(true);

            var optionArrayLength = optionsProp.intValue;
            var lastIndex = optionArrayLength - 1;

            optionsProp.Next(true);

            for (int i = 0; i < optionArrayLength; i++)
            {
                m_optionElems.Add(CreateOptionElem_DialogPartNode(i, optionsProp));
                if (i < lastIndex) optionsProp.Next(false);
            }

            m_optionElems.ForEach(e =>
            {
                mainContainer.Add(e);
                Outputs.Add(e.Q<Port>());
            });
        }
        private VisualElement CreateOptionElem_DialogPartNode(int index, SerializedProperty optionProp)
        {
            VisualElement optionElem = new VisualElement();

            VisualElement top = new VisualElement();
            top.AddToClassList("optionBottom");

            VisualElement divider = new VisualElement();
            divider.AddToClassList("optionDivider");

            VisualElement bottom = new VisualElement();
            bottom.AddToClassList("optionBottom");

            var useShowIfProp = optionProp.FindPropertyRelative("UseShowIf");
            var showIfProp = optionProp.FindPropertyRelative("ShowIf");
            var speechProp = optionProp.FindPropertyRelative("Speech");

            Toggle showIfToggle = new Toggle();
            showIfToggle.BindProperty(useShowIfProp);

            PropertyField showIfContainer = new PropertyField(showIfProp, "Show If");
            showIfContainer.Bind(m_serializedNode);

            Button removeButton = new Button(() =>
            {
                var target = m_nodeAsDecisive.Options[index];
                m_nodeAsDecisive.RemoveNextNode(Outputs.IndexOf(optionElem.Q<Port>()));
                m_nodeAsDecisive.Options.Remove(target);
                m_optionElems.Remove(optionElem);
                mainContainer.Remove(optionElem);

                Master.Refresh();
            });
            removeButton.text = "X";
            removeButton.AddToClassList("removeOptionButton");

            Port port = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(bool));
            port.AddToClassList("optionPort");
            port.portName = "";

            TextField speechField = new TextField();
            speechField.AddToClassList("optionField");
            speechField.multiline = true;

            speechField.BindProperty(speechProp);

            showIfToggle.RegisterValueChangedCallback(evt =>
            {
                showIfContainer.SetEnabled(evt.newValue);
            });

            showIfContainer.SetEnabled(showIfToggle.value);

            removeButton.tooltip = "Remove this option.";
            showIfToggle.tooltip = "If this checkbox is checked, the show if panel on the right will get enabled. For more details, hover it.";
            showIfContainer.tooltip = "When enabled, the visibility of this option will depend on this condition.";

            top.Add(removeButton);
            top.Add(showIfToggle);
            top.Add(showIfContainer);

            bottom.Add(speechField);
            bottom.Add(port);

            optionElem.Add(divider);
            optionElem.Add(top);
            optionElem.Add(bottom);

            return optionElem;
        }
        #endregion
    }

}