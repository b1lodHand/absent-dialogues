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

        /// <summary>
        /// Action gets invoked when this node gets selected or unselected.
        /// </summary>
        public Action<NodeView> OnNodeSelected;

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

        private Button m_createNewOptionButton;
        private DropdownField m_gotoDropdown;

        private List<VisualElement> m_optionElems = new List<VisualElement>();

        private SerializedObject m_serializedNode;

        private DecisionSpeechNode m_nodeAsDecisive;
        private GotoNode m_nodeAsGoto;

        /// <summary>
        /// The graph we're in.
        /// </summary>
        public DialogueGraphView Master { get; internal set; }

        /// <summary>
        /// Use to construct a node view from a node.
        /// </summary>
        /// <param name="node">Target node.</param>
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

            SetupTextFieldIfExists();

            node.OnSetState -= UpdateState;
            node.OnSetState += UpdateState;

            UpdateState(node.State);

            if (node.PersonDependent)
            {
                node.MasterDialogue.OnValidateAction -= RefreshPersonDropdown;
                node.MasterDialogue.OnValidateAction += RefreshPersonDropdown;
            }

            if (node is DecisionSpeechNode)
            {
                node.OnValidation -= RefreshOptionLabels;
                node.OnValidation += RefreshOptionLabels;
            }
            else if (node is DialoguePartNode)
            {
                node.OnValidation -= RefreshDialoguePartFinder;
                node.OnValidation += RefreshDialoguePartFinder;

                node.OnValidation += RefreshDialoguePartTitle;
                node.OnValidation += RefreshDialoguePartTitle;
            }
            else if (node is ActionNode)
            {
                node.OnValidation -= RefreshActionMapProps;
                node.OnValidation += RefreshActionMapProps;
            }
            else if (node is GotoNode)
            {
                DialogueEditorWindow.m_inspectorView.OnNodeValidation -= RefreshGotoDropdown;
                DialogueEditorWindow.m_inspectorView.OnNodeValidation += RefreshGotoDropdown;
            }
            else if (node is ConditionNode)
            {
                node.OnValidation -= RefreshConditionTooltip;
                node.OnValidation += RefreshConditionTooltip;
            }
        }

        private void RefreshConditionTooltip()
        {
            ConditionNode nodeAsCondition = Node as ConditionNode;
            VisualElement icon = this.Q<VisualElement>("node-icon");

            icon.tooltip = nodeAsCondition.GetConditionString(true);
        }

        private void RefreshDialoguePartTitle()
        {
            DialoguePartNode nodeAsDp = Node as DialoguePartNode;
            Label title = this.Q<Label>("title-label");

            string dpName = nodeAsDp.DialoguePartName;

            if (string.IsNullOrWhiteSpace(dpName)) title.text = nodeAsDp.GetTitle();
            else title.text = dpName;
        }

        private void RefreshActionMapProps()
        {
            ActionNode nodeAsAction = Node as ActionNode;
            VisualElement icon = this.Q<VisualElement>("node-icon");
            Label title = this.Q<Label>("title-label");

            if (nodeAsAction.UsedByMapper)
            {
                AddToClassList("mapped");
                title.text = nodeAsAction.UniqueMapperId;
            }

            else
            {
                RemoveFromClassList("mapped");
                title.text = nodeAsAction.GetTitle();
            }

            if (nodeAsAction.UsedByMapper) icon.tooltip = nodeAsAction.UniqueMapperId;
            else icon.tooltip = null;
        }

        private void RefreshDialoguePartFinder()
        {
            DialogueEditorWindow.RefreshDialoguePartFinder();
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
            if (Node is DecisionSpeechNode decisiveNode) m_nodeAsDecisive = decisiveNode;
            else if (Node is GotoNode gotoNode) m_nodeAsGoto = gotoNode;
        }
        private void SetupTextFieldIfExists()
        {
            TextField textField = this.Q<TextField>("speech");
            textField.bindingPath = "m_text";
            textField.Bind(m_serializedNode);
        }

        private void DrawElems()
        {
            if (Node is DecisionSpeechNode) DrawElems_DecisionSpeechNode();
            else if (Node is GotoNode) DrawElems_GotoNode();

            if (Node.PersonDependent) RefreshPersonDropdown();
            if (Node is IContainVariableManipulators) RefreshVariableManipulators();
        }

        private void RefreshVariableManipulators()
        {
            IContainVariableManipulators nodeAsManipulator = Node as IContainVariableManipulators;
            List<NodeVariableComparer> comparers = nodeAsManipulator.GetComparers();
            List<NodeVariableSetter> setters = nodeAsManipulator.GetSetters();

            if (comparers != null && comparers.Count > 0) comparers.ForEach(comparer => comparer.BlackboardBank = Node.Blackboard.Bank);
            if (setters != null && setters.Count > 0) setters.ForEach(setter => setter.BlackboardBank = Node.Blackboard.Bank);
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

            if (Node.PersonIndex < 0 || Node.PersonIndex > Node.MasterDialogue.People.Count - 1)
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

        private void RefreshGotoDropdown()
        {
            m_gotoDropdown.choices.Clear();

            Node.MasterDialogue.GetAllDialogueParts().ForEach(dialoguePartNode =>
            {
                m_gotoDropdown.choices.Add(dialoguePartNode.DialoguePartName);
            });

            if (m_gotoDropdown.choices.Count == 0)
            {
                m_gotoDropdown.SetValueWithoutNotify("There are no DialoguePartNode.");
                return;
            }

            if (Node.MasterDialogue.GetAllDialogueParts().Contains(m_nodeAsGoto.TargetNode)) SoftRefreshGotoLabel();
            else m_gotoDropdown.SetValueWithoutNotify("Select a DialoguePartNode.");
        }
        private void SoftRefreshGotoLabel()
        {
            m_gotoDropdown.SetValueWithoutNotify(m_nodeAsGoto.TargetNode.DialoguePartName);
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
            DropdownField gotoDropdown = new DropdownField();
            gotoDropdown.name = "goto-dropdown";
            gotoDropdown.AddToClassList("goto-field");
            m_gotoDropdown = gotoDropdown;

            gotoDropdown.RegisterValueChangedCallback(evt =>
            {
                Undo.RecordObject(m_nodeAsGoto, "Node (Person Modified)");

                DialoguePartNode targetNode = Node.MasterDialogue.GetDialoguePartNodesWithName(evt.newValue).FirstOrDefault();
                if (targetNode != null) m_nodeAsGoto.TargetNode = targetNode;

                EditorUtility.SetDirty(m_nodeAsGoto);
            });

            RefreshGotoDropdown();
            this.Add(gotoDropdown);
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
            if ((!Node.MasterDialogue.IsClone) || !Application.isPlaying) return;

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
            if (Master.selection.Count == 1) OnNodeSelected?.Invoke(null); //??
        }

        #region Decision Speech Node
        private void RefreshOptionLabels()
        {
            try
            {
                m_optionElems.ForEach(optionElem =>
                {
                    Option targetOption = m_nodeAsDecisive.Options[m_optionElems.IndexOf(optionElem)];
                    Label showIfLabel = optionElem.Q<VisualElement>("top").Q<Label>("show-if-label");

                    showIfLabel.visible = targetOption.UseShowIf;

                    if (!showIfLabel.visible) return;

                    showIfLabel.tooltip = targetOption.Visibility.GetConditionString(true);
                });
            }

            catch
            {
                return;
            }
        }
        private void CreateOption_DecisionSpeechNode()
        {
            Option option = new Option();

            Undo.RecordObject(m_nodeAsDecisive, "Decision Node (Modified)");
            m_nodeAsDecisive.Options.Add(option);

            EditorUtility.SetDirty(m_nodeAsDecisive);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

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

            var speechProp = optionProp.FindPropertyRelative("Text");

            Button removeButton = new Button(() =>
            {
                var target = m_nodeAsDecisive.Options[index];

                Undo.RecordObject(m_nodeAsDecisive, "Decision Node (Modified)");
                m_nodeAsDecisive.RemoveNextNode(Outputs.IndexOf(optionElem.Q<Port>()));
                m_nodeAsDecisive.Options.Remove(target);

                EditorUtility.SetDirty(m_nodeAsDecisive);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

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
            showIfLabel.tooltip = "NODATA";

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