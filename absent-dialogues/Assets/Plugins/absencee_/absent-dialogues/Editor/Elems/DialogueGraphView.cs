using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using com.absence.dialoguesystem.internals;
using Node = com.absence.dialoguesystem.internals.Node;
using com.absence.utilities;
using System.Text;
using System.Reflection;

namespace com.absence.dialoguesystem.editor
{
    /// <summary>
    /// The graph view responsible for rendering a dialogue's graph elements.
    /// </summary>
    [HelpURL("https://b1lodhand.github.io/absent-dialogues/api/com.absence.dialoguesystem.editor.DialogueGraphView.html")]
    public sealed class DialogueGraphView : GraphView
    {
        public new class UxmlFactory : UxmlFactory<DialogueGraphView, GraphView.UxmlTraits> { }

        [SerializeField] internal Dialogue m_dialogue;

        /// <summary>
        /// Gets invoked when a node gets selected.
        /// </summary>
        public event Action<NodeView> OnNodeSelected = null;

        /// <summary>
        /// Gets invoked when a dialogue gets displayed.
        /// </summary>
        public event Action OnPopulateView = null;

        public event Action<Node> OnNodeCreated = null;
        public event Action<Node> OnBeforeNodeDeleted = null;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public DialogueGraphView()
        {
            Insert(0, new GridBackground());

            AddManipulators();
            AddMiniMap();
            AddStyleSheets();
            SubscribeToEvents();
        }

        private void SubscribeToEvents()
        {
            canPasteSerializedData += AllowPaste;
            serializeGraphElements += OnCopy;
            unserializeAndPaste += OnPaste;

            Undo.undoRedoPerformed -= OnUndoRedo;
            Undo.undoRedoPerformed += OnUndoRedo;
        }

        private string OnCopy(IEnumerable<GraphElement> elements)
        {
            StringBuilder sb = new(string.Empty);
            foreach (GraphElement node in elements) 
            {
                if (node is not NodeView view)
                    continue;

                if (view.Node is EntryNode)
                    continue;

                sb.Append(view.Node.Guid);
                sb.Append("\n");
            }

            return sb.ToString();
        }

        private void OnPaste(string operationName, string data)
        {
            EditorWindow window = DialogueEditorWindow.focusedWindow;

            if (window == null)
                return;

            Vector2 center = window.position.center;
            Vector2 worldMousePosition = window.rootVisualElement.
                ChangeCoordinatesTo(window.rootVisualElement.parent, center - window.position.position);
            Vector2 localMousePosition = contentViewContainer.WorldToLocal(worldMousePosition);

            string[] guids = data.TrimEnd('\n').Split('\n');
            List<Node> nodesToCopy = new List<Node>();

            foreach (string guid in guids)
            {
                nodesToCopy.Add(m_dialogue.AllNodes.First(node => node.Guid.Equals(guid)));
            }

            if (nodesToCopy.Count == 0)
                return;

            Undo.SetCurrentGroupName("Dialogue (Paste)");
            int group = Undo.GetCurrentGroup();

            List<Node> nodesCopied = new();
            Dictionary<string, string> oldGuidPairs = new();
            bool isFirst = true;
            Node firstCopiedNode = null;
            Node firstNodeToCopy = nodesToCopy[0];
            foreach (Node node in nodesToCopy) 
            {
                Vector2 position = localMousePosition;
                if (firstCopiedNode != null) position += (node.Position - firstNodeToCopy.Position);

                Node nodeCreated = DoCreateNode(node.GetType(), position, node);
                nodesCopied.Add(nodeCreated);
                oldGuidPairs.Add(node.Guid, nodeCreated.Guid);

                if (isFirst) firstCopiedNode = nodeCreated;

                isFirst = false;
            }

            foreach (Node node in nodesCopied)
            {
                List<Node> nodesConnected = node.GetOutputConnections();
                for (int i = 0; i < nodesConnected.Count; ++i)
                {
                    Node outputNode = nodesConnected[i];
                    if (nodesToCopy.Contains(outputNode))
                    {
                        Undo.RegisterCompleteObjectUndo(node, "Node (Paste Reconnections)");
                        node.AddOutputConnection(nodesCopied.Where(n => n.Guid.Equals(oldGuidPairs[outputNode.Guid])).First(), i);
                        continue;
                    }

                    Undo.RegisterCompleteObjectUndo(node, "Node (Paste Reconnections)");
                    node.RemoveOutputConnection(i);
                }

                EditorUtility.SetDirty(node);
                AssetDatabase.SaveAssetIfDirty(node);
            }

            EditorUtility.SetDirty(m_dialogue);
            AssetDatabase.SaveAssetIfDirty(m_dialogue);

            Undo.CollapseUndoOperations(group);

            Refresh();
        }

        private bool AllowPaste(string data)
        {
            return true;
        }

        private void AddStyleSheets()
        {
            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Plugins/absencee_/absent-dialogues/Editor/DialogueEditorWindow.uss");
            styleSheets.Add(styleSheet);
        }
        private void AddMiniMap()
        {
            var mapFoldout = new Foldout() { focusable = false, value = true, text = "Minimap" };
            var miniMap = new MiniMap() { anchored = true };
            miniMap.name = "mini-map";

            mapFoldout.Add(miniMap);
            this.Add(mapFoldout);

            miniMap.SetPosition(new Rect(0, 0, 192, 108));
        }
        private void AddManipulators()
        {
            this.AddManipulator(new ContentZoomer());
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());
        }

        private void OnUndoRedo()
        {
            if (Application.isPlaying)
                return;

            Refresh();
            AssetDatabase.SaveAssetIfDirty(m_dialogue);
        }

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            return ports.ToList().Where(endPort =>
            {
                var check1 = endPort.direction != startPort.direction && endPort.node != startPort.node;
                return check1;

            }).ToList();
        }
        private GraphViewChange OnGraphViewChanged(GraphViewChange graphViewChange)
        {
            if (graphViewChange.elementsToRemove != null)
            {
                graphViewChange.elementsToRemove.ForEach(elem =>
                {
                    NodeView nodeView = elem as NodeView;
                    if (nodeView != null)
                    {
                        if (nodeView.Node.Equals(m_dialogue.Entry)) return;

                        DeleteNode(nodeView);
                        return;
                    }

                    Edge edge = elem as Edge;
                    if (edge != null)
                    {
                        NodeView outputView = edge.output.node as NodeView;
                        NodeView inputView = edge.input.node as NodeView;

                        Undo.RegisterCompleteObjectUndo(outputView.Node, "Dialogue (Remove Output Connection)");
                        outputView.Node.RemoveOutputConnection(outputView.Outputs.IndexOf(edge.output));
                        EditorUtility.SetDirty(outputView.Node);
                    }
                });

                Refresh();
            }

            if (graphViewChange.edgesToCreate != null)
            {
                graphViewChange.edgesToCreate.ForEach(edge =>
                {
                    NodeView outputView = edge.output.node as NodeView;
                    NodeView inputView = edge.input.node as NodeView;

                    Undo.RegisterCompleteObjectUndo(outputView.Node, "Dialogue (Add Output Connection)");
                    outputView.Node.AddOutputConnection(inputView.Node, outputView.Outputs.IndexOf(edge.output));
                    EditorUtility.SetDirty(outputView.Node);
                });

            }

            return graphViewChange;
        }

        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            evt.menu.AppendAction("Refresh", v => Refresh());
            evt.menu.AppendSeparator();

            var types = TypeCache.GetTypesDerivedFrom<Node>();
            List<Type> redrawList = new();
            foreach (Type type in types)
            {
                bool needsRedraw = CreateMenuItem(type, evt, false);
                if (needsRedraw) redrawList.Add(type);
            }

            foreach (Type type in redrawList) 
            { 
                CreateMenuItem(type, evt, true);
            }
        }

        protected override void CollectCopyableGraphElements(IEnumerable<GraphElement> elements, HashSet<GraphElement> elementsToCopySet)
        {
            base.CollectCopyableGraphElements(elements, elementsToCopySet);
        }

        private bool CreateMenuItem(Type type, ContextualMenuPopulateEvent evt, bool force = false)
        {
            DropdownMenuAction.Status status = DropdownMenuAction.Status.Normal;
            PropertyInfo menuProp = type.GetProperty("CreationMenuName");
            string menuPropValue = menuProp.GetValue(null).ToString();
            bool menuSpecified = menuProp != null && (!string.IsNullOrWhiteSpace(menuPropValue));

            if (menuSpecified && menuPropValue.Equals(Node.NaN))
                return false;

            if (menuSpecified && (!force) && menuPropValue.Contains("Misc/"))
                return true;

            var mousePos = viewTransform.matrix.inverse.MultiplyPoint(evt.localMousePosition);
            string context = menuSpecified ? menuPropValue : Helpers.SplitCamelCase(type.Name, " ");

            evt.menu.AppendAction(context, a =>
            {
                CreateNode(type, mousePos);
            }, status);

            return false;
        }

        internal void ClearViewWithoutNotification()
        {
            graphViewChanged -= OnGraphViewChanged;
            DeleteElements(graphElements);
            graphViewChanged += OnGraphViewChanged;
        }

        internal void PopulateView(Dialogue dialogue)
        {
            Dialogue previousDialogue = m_dialogue;
            m_dialogue = dialogue;

            ClearViewWithoutNotification();

            if (previousDialogue != m_dialogue) EditorPrefs.SetString("last-node-guid", string.Empty);

            if (m_dialogue == null) return;

            if (m_dialogue.Entry == null)
            {
                m_dialogue.Entry = m_dialogue.CreateNode(typeof(EntryNode)) as EntryNode;

                AssetDatabase.AddObjectToAsset(m_dialogue.Entry, m_dialogue);
                EditorUtility.SetDirty(m_dialogue);
                AssetDatabase.SaveAssets();
            }

            dialogue.AllNodes.RemoveAll(n => n == null);

            dialogue.AllNodes.ForEach(n => CreateNodeView(n));

            dialogue.AllNodes.ForEach(n =>
            {
                if (n == null) return;

                List<Node> nexts = n.GetOutputConnections();

                for (int i = 0; i < nexts.Count; i++)
                {
                    Node n2 = nexts[i];

                    if (n2 == null)
                        continue;

                    NodeView startView = FindNodeView(n);
                    NodeView endView = FindNodeView(n2);

                    Edge edge = startView.Outputs[i].ConnectTo(endView.Input);
                    AddElement(edge);
                }
            });

            dialogue.OnValidate();
            dialogue.AllNodes.ForEach(n =>
            {
                n.OnValidate();
            });

            OnPopulateView?.Invoke();
        }

        /// <summary>
        /// Use to refresh the current graph view.
        /// </summary>
        public void Refresh()
        {
            if (m_dialogue == null) return;
            PopulateView(m_dialogue);

            string lastNodeGuid = EditorPrefs.GetString("last-node-guid", string.Empty);
            if (string.IsNullOrWhiteSpace(lastNodeGuid)) return;

            SelectNode(m_dialogue.AllNodes.Where(node => node.Guid == lastNodeGuid).FirstOrDefault());
        }

        /// <summary>
        /// Use to find the view of a node.
        /// </summary>
        /// <param name="node">Target node.</param>
        /// <returns>Returns the view of the target node.</returns>
        public NodeView FindNodeView(Node node)
        {
            return GetNodeByGuid(node.Guid) as NodeView;
        }

        Node CreateNode(System.Type type, Vector2 atPosition, Node from = null)
        {
            Node node = DoCreateNode(type, atPosition, from);

            Refresh();
            SelectNode(node);

            OnNodeCreated?.Invoke(node);

            return node;
        }

        Node DoCreateNode(System.Type type, Vector2 atPosition, Node from = null)
        {
            Undo.SetCurrentGroupName("Dialogue (Node Created)");
            int group = Undo.GetCurrentGroup();
            Undo.IncrementCurrentGroup();

            Undo.RegisterCompleteObjectUndo(m_dialogue, "Dialogue (Node List)");

            Node node = m_dialogue.CreateNode(type, from);
            m_dialogue.AllNodes.Remove(node);

            node.Guid = GUID.Generate().ToString();
            node.name = node.Guid;
            node.Position.x = atPosition.x;
            node.Position.y = atPosition.y;

            AssetDatabase.AddObjectToAsset(node, m_dialogue);
            Undo.RegisterCreatedObjectUndo(node, "Dialogue (Node Asset Created)");

            EditorUtility.SetDirty(m_dialogue);

            Undo.RecordObject(m_dialogue, "Dialogue (Node Added to AllNodes)");
            m_dialogue.AllNodes.Add(node);

            if (from != null && from.CustomData != null)
            {
                NodeCustomDataBase newCustomData = ScriptableObject.Instantiate(from.CustomData);
                newCustomData.name = $"{node.Guid}_CustomData";
                AssetDatabase.AddObjectToAsset(newCustomData, m_dialogue);
                Undo.RegisterCreatedObjectUndo(newCustomData, "Dialogue (Create Custom Data)");

                Undo.RecordObject(node, "Node (Creation)");
                node.CustomData = newCustomData;
            }

            if (from != null && from.HasOptions)
            {
                List<NodeCustomDataBase> customDatas = new();
                foreach (Option option in from.Options)
                {
                    NodeCustomDataBase optionCustomData = option.CustomData ? ScriptableObject.Instantiate(option.CustomData) : null;
                    if (optionCustomData != null)
                    {
                        optionCustomData.name = $"{node.Guid}_OptionData";
                        AssetDatabase.AddObjectToAsset(optionCustomData, m_dialogue);
                        Undo.RegisterCreatedObjectUndo(optionCustomData, "Dialogue (Create Option Data)");
                    }
                    customDatas.Add(optionCustomData);
                }

                Undo.RecordObject(node, "Dialogue (Create Option Data)");

                for (int i = 0; i < customDatas.Count; i++)
                {
                    node.Options[i].CustomData = customDatas[i];
                }
            }

            EditorUtility.SetDirty(node);

            Undo.CollapseUndoOperations(group);

            AssetDatabase.SaveAssets();

            return node;
        }


        void DeleteNode(NodeView view)
        {
            OnBeforeNodeDeleted?.Invoke(view.Node);

            NodeCustomDataCreationHandler.DeleteNodeCustomData(view.Node);
            if (view.Node.HasOptions)
            {
                foreach (Option option in view.Node.Options)
                {
                    if (option.CustomData == null)
                        continue;

                    NodeCustomDataCreationHandler.DeleteOptionCustomData(view.Node, option);
                }
            }

            Undo.RegisterCompleteObjectUndo(m_dialogue, "Dialog (Delete Node)");

            m_dialogue.DeleteNode(view.Node);

            Undo.DestroyObjectImmediate(view.Node);

            EditorUtility.SetDirty(m_dialogue);
            AssetDatabase.SaveAssetIfDirty(m_dialogue);
        }

        NodeView CreateNodeView(Node node)
        {
            if(node == null) return null;

            NodeView nodeView = NodeViewCreationHandler.CreateNodeView(node.GetType(), node, this);
            nodeView.OnSelect = OnNodeSelected;
            AddElement(nodeView);

            return nodeView;
        }

        internal void SelectNode(Node node)
        {
            if (node == null) return;

            NodeView view = FindNodeView(node);
            ISelectable selectableNode = view.GetFirstOfType<ISelectable>();

            if (selectableNode == null) return;

            ClearSelection();
            AddToSelection(selectableNode);
        }

        internal VisualElement CreateGenericOptionElement(NodeView sender, int index)
        {
            VisualElement optionElem = new VisualElement();

            VisualElement top = new VisualElement();
            top.AddToClassList("optionBottom");
            top.name = "top";

            VisualElement divider = new VisualElement();
            divider.AddToClassList("optionDivider");

            VisualElement bottom = new VisualElement();
            bottom.AddToClassList("optionBottom");

            GenericOption target = m_dialogue.GenericOptions[index];

            Button removeButton = new Button(() =>
            {
            });

            Button moveUpButton = new Button(() =>
            {
            });

            Button moveDownButton = new Button(() =>
            {
            });

            removeButton.text = "x";
            removeButton.AddToClassList("removeOptionButton");
            removeButton.SetEnabled(false);

            moveUpButton.text = "↑";
            moveUpButton.AddToClassList("moveOptionUpButton");
            moveUpButton.SetEnabled(false);

            moveDownButton.text = "↓";
            moveDownButton.AddToClassList("moveOptionDownButton");
            moveDownButton.SetEnabled(false);

            Port port = sender.InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(bool));
            port.AddToClassList("optionPort");
            port.portName = "";
            port.name = "option-direct-port";

            TextField speechField = new TextField();
            speechField.AddToClassList("optionField");
            speechField.multiline = true;
            speechField.SetValueWithoutNotify(target.Text);
            speechField.SetEnabled(false);

            Label showIfLabel = new Label("Conditional visibility active.");
            showIfLabel.AddToClassList("optionShowIfLabel");
            showIfLabel.name = "show-if-label";
            showIfLabel.tooltip = "NODATA";

            top.Add(removeButton);
            top.Add(moveUpButton);
            top.Add(moveDownButton);
            top.Add(showIfLabel);
            RefreshShowIfLabel();

            bottom.Add(speechField);
            bottom.Add(port);

            optionElem.Add(divider);
            optionElem.Add(top);
            optionElem.Add(bottom);

            return optionElem;

            void RefreshShowIfLabel()
            {
                showIfLabel.visible = target.UseShowIf;
            }
        }
    }

}