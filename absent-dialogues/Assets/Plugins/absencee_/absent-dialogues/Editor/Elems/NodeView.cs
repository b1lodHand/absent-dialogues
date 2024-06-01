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
using com.absence.variablesystem;

namespace com.absence.dialoguesystem.editor
{
    public class NodeView : UnityEditor.Experimental.GraphView.Node
    {
        public static string K_PERSONDEPENDENT_CLASSNAME = "personDependent";

        public Action<NodeView> OnNodeSelected;
        public Node Node;

        public Port Input;
        public List<Port> Outputs = new List<Port>();

        private Button m_createNewOptionButton;
        private List<VisualElement> m_optionElems = new List<VisualElement>();

        protected SerializedObject m_serializedNode;
        private DecisionSpeechNode m_nodeAsDecisive;

        public DialogueGraphView Master { get; internal set; }

        public NodeView(Node node) : base("Assets/Plugins/absencee_/absent-dialogues/Editor/Elems/NodeView.uxml")
        {
            this.Node = node;
            this.viewDataKey = node.Guid;
            this.showInMiniMap = node.ShowInMinimap;

            style.left = node.Position.x;
            style.top = node.Position.y;

            if (node.GetClassName() != null) AddToClassList(node.GetClassName());
            if (Node.PersonDependent) AddToClassList(K_PERSONDEPENDENT_CLASSNAME);

            this.title = node.GetTitle();

            SetupNodeForSerialization();
            SetupPersonDropdownIfExists();

            DrawElems();
            CreateInputPort();
            CreateOutputPorts();

            SetupSpeechFieldIfExists();
            RefreshNodeIcon();

            node.OnSetState -= UpdateState;
            node.OnSetState += UpdateState;

            node.OnValidation -= RefreshNodeIcon;
            node.OnValidation += RefreshNodeIcon;

            UpdateState(node.State);
            RefreshNodeIcon();

            if(node.PersonDependent)
            {
                node.MasterDialogue.OnEditorRefresh -= RefreshPersonDropdown;
                node.MasterDialogue.OnEditorRefresh += RefreshPersonDropdown;
            }

            if(node is DecisionSpeechNode)
            {
                node.OnValidation -= RefreshOptionLabels;
                node.OnValidation += RefreshOptionLabels;
            }
        }

        private void RefreshNodeIcon()
        {
            if (Node.ExitDialogAfterwards) AddToClassList("exit");
            else RemoveFromClassList("exit");
        }

        private void SetupPersonDropdownIfExists()
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

                Person targetPerson = Node.MasterDialogue.People.Where(p => p.Name == evt.newValue).FirstOrDefault();
                Node.PersonIndex = Node.MasterDialogue.People.IndexOf(targetPerson);

                EditorUtility.SetDirty(Node);

                personPreview.sprite = targetPerson.Icon;
            });
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
            else if (Node is DialoguePartNode) DrawElems_DialoguePartNode();

            if (Node.PersonDependent) RefreshPersonDropdown();
            if (Node is IContainVariableManipulators) RefreshVariableManipulators();
        }

        private void RefreshVariableManipulators()
        {
            IContainVariableManipulators nodeAsManipulator = Node as IContainVariableManipulators;
            List<FixedVariableComparer> comparers = nodeAsManipulator.GetComparers();
            List<FixedVariableSetter> setters = nodeAsManipulator.GetSetters();

            if (comparers != null && comparers.Count > 0) comparers.ForEach(comparer => comparer.SetFixedBank(Node.Blackboard.Bank));
            if (setters != null && setters.Count > 0) setters.ForEach(setter => setter.SetFixedBank(Node.Blackboard.Bank));
        }
        private void RefreshPersonDropdown()
        {
            DropdownField personDropdown = this.Q<DropdownField>("person-field");

            List<string> peopleNameList = Node.MasterDialogue.People.ConvertAll(p =>
            {
                if (p) return p.Name;

                return null;
            });

            if (peopleNameList.Count == 0)
            {
                personDropdown.choices = new List<string>();
                personDropdown.SetValueWithoutNotify("No person exists in this dialogue.");
                return;
            }

            personDropdown.choices = new List<string>(peopleNameList);

            if(Node.PersonIndex < 0 || Node.PersonIndex > Node.MasterDialogue.People.Count - 1)
            {
                personDropdown.SetValueWithoutNotify("Missing person...");
                return;
            }

            if (Node.MasterDialogue.People[Node.PersonIndex])
            {
                personDropdown.SetValueWithoutNotify(Node.MasterDialogue.People[Node.PersonIndex].Name);
                Image personIconPreview = personDropdown.parent.Q<Image>("person-icon-preview");
                personIconPreview.sprite = Node.MasterDialogue.People[Node.PersonIndex].Icon;
            }
            else
                personDropdown.SetValueWithoutNotify("Select a person...");

        }

        private void DrawElems_DialoguePartNode()
        {
            Label nameLabel = new Label();
            nameLabel.bindingPath = "DialoguePartName";
            nameLabel.Bind(m_serializedNode);

            this.Insert(0, nameLabel);
        }
        private void DrawElems_DecisionSpeechNode()
        {
            m_createNewOptionButton = new Button(CreateOption_DecisionSpeechNode);
            m_createNewOptionButton.text = "Add New Option";
            m_createNewOptionButton.AddToClassList("addNewOptionButton");
            mainContainer.Add(m_createNewOptionButton);

            RefreshOptions_DecisionSpeechNode();
        }
        private void DrawElems_GotoNode()
        {
            Label gotoLabel = new Label();
            gotoLabel.bindingPath = "TargetDialoguePartName";
            gotoLabel.Bind(m_serializedNode);

            this.Add(gotoLabel);
        }

        private void CreateInputPort()
        {
            if (Node.GetInputPortNameForCreation() == null) return;

            Input = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, typeof(bool));
            Input.portName = Node.GetInputPortNameForCreation();
            inputContainer.Add(Input);
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
            if (!Application.isPlaying) return;
            
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

        public override void OnUnselected()
        {
            base.OnUnselected();
            OnNodeSelected?.Invoke(null);
        }

        #region Decision Speech Node
        private void RefreshOptionLabels()
        {
            m_optionElems.ForEach(optionElem =>
            {
                Label showIfLabel = optionElem.Q<VisualElement>("top").Q<Label>("show-if-label");

                showIfLabel.visible = m_nodeAsDecisive.Options[m_optionElems.IndexOf(optionElem)].UseShowIf;
            });
        }
        private void CreateOption_DecisionSpeechNode()
        {
            Option option = new Option();

            Undo.RecordObject(m_nodeAsDecisive, "Decision Node (Modified)");
            m_nodeAsDecisive.Options.Add(option);

            Master.Refresh();
        }
        private void RefreshOptions_DecisionSpeechNode()
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
            top.name = "top";

            VisualElement divider = new VisualElement();
            divider.AddToClassList("optionDivider");

            VisualElement bottom = new VisualElement();
            bottom.AddToClassList("optionBottom");

            var speechProp = optionProp.FindPropertyRelative("Speech");

            Button removeButton = new Button(() =>
            {
                var target = m_nodeAsDecisive.Options[index];

                Undo.RecordObject(m_nodeAsDecisive, "Decision Node (Modified)");
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

            removeButton.tooltip = "Remove this option.";

            Label showIfLabel = new Label("Show if in use.");
            showIfLabel.AddToClassList("optionShowIfLabel");
            showIfLabel.name = "show-if-label";

            top.Add(removeButton);
            top.Add(showIfLabel);
            RefreshShowIfLabel();

            bottom.Add(speechField);
            bottom.Add(port);

            optionElem.Add(divider);
            optionElem.Add(top);
            optionElem.Add(bottom);

            void RefreshShowIfLabel()
            {
                showIfLabel.visible = optionProp.FindPropertyRelative("m_useShowIf").boolValue;
            }

            return optionElem;
        }
        #endregion
    }

}