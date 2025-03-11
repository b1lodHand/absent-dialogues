using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.UIElements;
using System.Linq;
using Node = com.absence.dialoguesystem.internals.Node;
using com.absence.personsystem;
using com.absence.dialoguesystem.internals;
using com.absence.dialoguesystem.editor.internals;

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
        public static string PersonDependentClassName = "personDependent";

        public const string DEFAULT_XML_LOCATION = "Assets/Plugins/absencee_/absent-dialogues/Editor/Elems/NodeView.uxml";

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

        protected GUID m_assetGuid;
        protected SerializedObject m_serializedNode;
        protected Label m_titleText;
        protected TextField m_defaultTextField;
        protected TextElement m_defaultTextFieldTextElement;
        protected VisualElement m_nodeIcon;
        protected VisualElement m_nodeBorder;
        protected VisualElement m_selectionBorder;
        protected VisualElement m_stateBorder;
        protected Label m_personDropdownLabel;
        protected VisualElement m_personDropdown;
        protected VisualElement m_infoBoxElement;
        protected Label m_infoBoxText;

        /// <summary>
        /// The graph we're in.
        /// </summary>
        public DialogueGraphView Graph { get; internal set; }

        /// <summary>
        /// Use to construct a node view from a node.
        /// </summary>
        /// <param name="node">Target node.</param>
        public NodeView(Node node, DialogueGraphView graph = null) : base(DEFAULT_XML_LOCATION)
        {
            Type nodeType = node.GetType();
            m_assetGuid = AssetDatabase.GUIDFromAssetPath(AssetDatabase.GetAssetPath(node));

            this.Graph = graph;
            this.Node = node;
            this.viewDataKey = node.Guid;
            this.showInMiniMap = node.ShowInMinimap;

            CreateDynamicElements();
            FindDefaultElements();
            FetchGenericOptions();

            NodeViewStyles.ApplyStyles(this);

            style.left = node.Position.x;
            style.top = node.Position.y;

            if (Node.PersonDependent) AddToClassList(PersonDependentClassName);

            OnAfterStylesApplied();

            CreateInputPort();
            CreateOutputPorts();

            SetupNodeForSerialization();
            SetupPersonDropdownIfExists();
            SetupTextFieldIfExists();

            SetTopInfoVisibility(Graph.m_displayDetails);
            RefreshTopInfo();
            ApplyHardcodedStyle(EditorSettings.instance);

            this.title = node.Title ?? "Node";

            Node.UpdateManipulators();

            if (Node.PersonDependent) RefreshPersonDropdown();

            OnDraw();

            UpdateState(node.State);

            if (node.PersonDependent)
            {
                Graph.m_dialogue.OnValidateAction -= RefreshPersonDropdown;
                Graph.m_dialogue.OnValidateAction += RefreshPersonDropdown;
            }

            Node.onSetState -= UpdateState;
            Node.onSetState += UpdateState;
        }

        #region Protected API
        protected void SetPortColor(Port target, Color connectedColor, Color notConnectedColor)
        {
            VisualElement connector = target.Q("connector");
            VisualElement cap = connector.Q("cap");
            
            Color color = target.connected ? connectedColor : notConnectedColor;

            cap.style.backgroundColor = color;
            connector.style.borderTopColor = connectedColor;
            connector.style.borderRightColor = connectedColor;
            connector.style.borderBottomColor = connectedColor;
            connector.style.borderLeftColor = connectedColor;
        }
        protected void SetPortLabelColor(Port target, Color color)
        {
            VisualElement label = target.Q("type");

            label.style.color = color;
        }
        protected virtual void CreateDynamicElements()
        {
            m_infoBoxElement = new VisualElement();
            m_infoBoxText = new Label();
            m_infoBoxElement.pickingMode = PickingMode.Ignore;
            m_infoBoxElement.style.position = Position.Absolute;
            m_infoBoxElement.style.maxWidth = this.style.maxWidth;
            m_infoBoxElement.style.minWidth = this.style.minWidth;
            m_infoBoxElement.style.width = this.style.width;
            m_infoBoxText.style.unityTextAlign = TextAnchor.UpperCenter;
            m_infoBoxText.pickingMode = PickingMode.Ignore;
            m_infoBoxText.style.whiteSpace = WhiteSpace.Normal;
            m_infoBoxText.enableRichText = true;

            m_infoBoxElement.Add(m_infoBoxText);
            this.Insert(0, m_infoBoxElement);
        }
        protected virtual void FindDefaultElements()
        {
            m_nodeBorder = this.Q("node-border");
            m_selectionBorder = this.Q("selection-border");
            m_stateBorder = this.Q("state-border");
            m_nodeIcon = this.Q("node-icon");
            m_titleText = this.Q("title").Q<Label>("title-label");
            m_defaultTextField = this.Q<TextField>("speech");
            m_defaultTextFieldTextElement = m_defaultTextField.Q("unity-text-input").Q<TextElement>();
        }
        protected void FetchGenericOptions()
        {
            if (!Node.UseGenericOptions)
                return;

            List<GenericOption> genericOptions = Graph.m_dialogue.GenericOptions;

            for (int i = 0; i < genericOptions.Count; i++)
            {
                Node.GenericOptions[i].Target = genericOptions[i];
            }
        }
        internal void OnGenericOptionCreated(int at)
        {
            List<GenericOption> genericOptions = Graph.m_dialogue.GenericOptions;

            Undo.RegisterCompleteObjectUndo(Node, "Node (Generic Option Created)");

            GenericOptionReference reference = new(genericOptions[at]);
            Node.GenericOptions.Insert(at, reference);

            FetchGenericOptions();

            EditorUtility.SetDirty(Node);
            AssetDatabase.SaveAssetIfDirty(m_assetGuid);
        }

        internal void OnGenericOptionRemoved(int at)
        {
            Undo.RegisterCompleteObjectUndo(Node, "Node (Generic Option Removed)");
            Node.GenericOptions.RemoveAt(at);

            FetchGenericOptions();

            EditorUtility.SetDirty(Node);
            AssetDatabase.SaveAssetIfDirty(m_assetGuid);
        }
        internal void OnGenericOptionsRearranged(int replacer, int replaced)
        {
            Undo.RegisterCompleteObjectUndo(Node, "Node (Generic Option Removed)");

            GenericOptionReference replacerReference = Node.GenericOptions[replacer];
            GenericOptionReference replacedReference = Node.GenericOptions[replaced];

            bool replacedBypass = replacedReference.Bypass;
            Node replacedLead = replacedReference.LeadingNode;

            Node.GenericOptions[replaced].Bypass = replacerReference.Bypass;
            Node.GenericOptions[replaced].LeadingNode = replacerReference.LeadingNode;

            Node.GenericOptions[replacer].Bypass = replacedBypass;
            Node.GenericOptions[replacer].LeadingNode = replacedLead;

            FetchGenericOptions();

            EditorUtility.SetDirty(Node);
            AssetDatabase.SaveAssetIfDirty(m_assetGuid);
        }
        protected void SetupPersonDropdownIfExists()
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
        protected void SetupNodeForSerialization()
        {
            m_serializedNode = new SerializedObject(Node);
        }
        protected void SetupTextFieldIfExists()
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
        protected virtual void OnAfterStylesApplied()
        {

        }
        protected virtual void OnDraw()
        {

        }
        protected virtual void OnDisconnectAll(ref HashSet<GraphElement> toDelete)
        {

        }
        internal virtual void ApplyHardcodedStyle(EditorSettings settings)
        {

        }
        internal virtual void RefreshTopInfo()
        {
            if (!HasTopInfo)
            {
                SetTopInfoVisibility(false); 
                return;
            }

            string info = Node.GenerateTopInfoText();
            m_infoBoxElement.style.bottom = TopInfoBottomPosition;
            m_infoBoxElement.style.right = TopInfoRightPosition;
            m_infoBoxText.text = info;
        }
        internal virtual void SetTopInfoVisibility(bool visibility)
        {
            m_infoBoxElement.style.display = visibility ? 
                DisplayStyle.Flex : DisplayStyle.None;
        }

        internal virtual bool HasTopInfo => false;
        internal virtual float TopInfoRightPosition => 0f;
        internal virtual float TopInfoBottomPosition => 0f;
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
        public override bool IsCopiable() => true;
        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            if (evt.target is NodeView)
            {
                evt.menu.AppendAction("Disconnect all", DisconnectAll, DisconnectAllStatus);
                evt.menu.AppendSeparator();
            }
        }

        protected virtual DropdownMenuAction.Status DisconnectAllStatus(DropdownMenuAction action)
        {
            VisualElement[] array = new VisualElement[2] { inputContainer, outputContainer };
            VisualElement[] array2 = array;
            foreach (VisualElement e in array2)
            {
                List<Port> list = e.Query<Port>().ToList();
                foreach (Port item in list)
                {
                    if (item.connected)
                    {
                        return DropdownMenuAction.Status.Normal;
                    }
                }
            }

            return DropdownMenuAction.Status.Disabled;
        }
        protected void DisconnectAll(DropdownMenuAction action)
        {
            HashSet<GraphElement> toDelete = new HashSet<GraphElement>();
            AddConnectionsToDeleteSet(inputContainer, ref toDelete);
            AddConnectionsToDeleteSet(outputContainer, ref toDelete);
            OnDisconnectAll(ref toDelete);
            toDelete.Remove(null);
            if (Graph != null)
            {
                Graph.DeleteElements(toDelete);
            }
            else
            {
                Debug.Log("Disconnecting nodes that are not in a GraphView will not work.");
            }
        }

        protected void AddConnectionsToDeleteSet(VisualElement container, ref HashSet<GraphElement> toDelete)
        {
            List<GraphElement> toDeleteList = new List<GraphElement>();
            container.Query<Port>().ForEach(delegate (Port elem)
            {
                if (elem.connected)
                {
                    foreach (Edge connection in elem.connections)
                    {
                        if ((connection.capabilities & Capabilities.Deletable) != 0)
                        {
                            toDeleteList.Add(connection);
                        }
                    }
                }
            });
            toDelete.UnionWith(toDeleteList);
        }
        #endregion
    }

}