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

namespace com.absence.dialoguesystem.editor.internals
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

        List<Node> m_cutCache = new();
        List<NodeView> views = new();

        [SerializeField] internal bool m_displayDetails;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public DialogueGraphView()
        {
            Insert(0, new GridBackground());
            this.focusable = true;

            AddManipulators();
            AddTopPanel();
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

        protected override bool canCutSelection => false;

        private void ClearCutCache()
        {
            foreach (Node node in m_cutCache)
            {
                Node.DestroyImmediate(node);
            }

            m_cutCache.Clear();
        }

        private string OnCopy(IEnumerable<GraphElement> elements)
        {
            ClearCutCache();

            StringBuilder sb = new(string.Empty);
            foreach (GraphElement node in elements) 
            {
                if (node is not NodeView view)
                    continue;

                if (view.Node is EntryNode)
                    continue;

                m_cutCache.Add(Node.Instantiate(view.Node));

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
                Node nativeNodeFound = m_dialogue.AllNodes.FirstOrDefault(node => node.Guid.Equals(guid));
                if (nativeNodeFound == null) nativeNodeFound = m_cutCache.FirstOrDefault(node => node.Guid.Equals(guid));
                nodesToCopy.Add(nativeNodeFound);
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
                        Undo.RegisterCompleteObjectUndo(node, "Node (Paste Connections)");
                        node.AddOutputConnection(nodesCopied.Where(n => n.Guid.Equals(oldGuidPairs[outputNode.Guid])).First(), i);
                        continue;
                    }

                    Undo.RegisterCompleteObjectUndo(node, "Node (Paste Connections)");
                    node.RemoveOutputConnection(i);
                }

                EditorUtility.SetDirty(node);
                AssetDatabase.SaveAssetIfDirty(node);
            }

            EditorUtility.SetDirty(m_dialogue);
            AssetDatabase.SaveAssetIfDirty(m_dialogue);

            Undo.CollapseUndoOperations(group);

            Refresh();
            ClearSelection();

            foreach (Node node in nodesCopied) 
            {
                NodeView view = FindNodeView(node);
                ISelectable selectable = view.GetFirstOfType<ISelectable>();
                AddToSelection(selectable);
            }

            ClearCutCache();
        }

        public override bool canGrabFocus => true;

        private bool AllowPaste(string data)
        {
            return true;
        }

        private void AddStyleSheets()
        {
            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Plugins/absencee_/absent-dialogues/Editor/DialogueEditorWindow.uss");
            styleSheets.Add(styleSheet);
        }
        private void AddTopPanel()
        {
            VisualElement topPanel = new VisualElement();
            topPanel.style.justifyContent = Justify.SpaceBetween;
            topPanel.style.flexDirection = FlexDirection.Row;
            topPanel.style.backgroundColor = new Color(0f, 0f, 0f, 0.1f);

            var mapFoldout = new Foldout() { focusable = false, value = true, text = "Minimap" };
            var miniMap = new MiniMap() { anchored = true };
            mapFoldout.style.alignSelf = Align.FlexStart;
            mapFoldout.style.alignItems = Align.FlexStart;
            mapFoldout.style.alignContent = Align.FlexStart;
            miniMap.name = "mini-map";

            Toggle toggle = new Toggle("Display Details");
            toggle.RegisterValueChangedCallback(evt =>
            {
                m_displayDetails = evt.newValue;

                if (m_dialogue != null)
                    RefreshTopInfos(m_displayDetails);
            });

            OnPopulateView -= () => toggle.SetValueWithoutNotify(m_displayDetails);
            OnPopulateView += () => toggle.SetValueWithoutNotify(m_displayDetails);

            mapFoldout.Add(miniMap);
            topPanel.Add(mapFoldout);
            topPanel.Add(toggle);

            this.Add(topPanel);
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

            if (m_dialogue == null)
                return;

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

        internal void RefreshTopInfos(bool display)
        {
            foreach (NodeView view in views)
            {
                view.SetTopInfoVisibility(display);
                view.RefreshTopInfo();
            }
        }

        internal void ReapplyHardcodedStyles(EditorSettings settings)
        {
            foreach (NodeView view in views)
            {
                view.ApplyHardcodedStyle(settings);
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
            bool hasMenuProp = menuProp != null;
            string menuPropValue = hasMenuProp ? menuProp.GetValue(null).ToString() : string.Empty;
            bool menuSpecified = hasMenuProp && (!string.IsNullOrWhiteSpace(menuPropValue));

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
            views.Clear();

            graphViewChanged -= OnGraphViewChanged;
            DeleteElements(graphElements);
            graphViewChanged += OnGraphViewChanged;
        }

        internal void PopulateView(Dialogue dialogue)
        {
            Dialogue previousDialogue = m_dialogue;
            m_dialogue = dialogue;

            ClearViewWithoutNotification();

            if (previousDialogue != null)
            {
                previousDialogue.ClearCallbacks();
            }

            if (previousDialogue != m_dialogue) EditorPrefs.SetString("last-node-guid", string.Empty);

            if (m_dialogue == null)
            {
                OnPopulateView?.Invoke();
                return;
            }

            if (m_dialogue.Entry == null)
            {
                m_dialogue.Entry = DialogueSystem.CreateNode(typeof(EntryNode), m_dialogue) 
                    as EntryNode;

                AssetDatabase.AddObjectToAsset(m_dialogue.Entry, m_dialogue);
                EditorUtility.SetDirty(m_dialogue);
                AssetDatabase.SaveAssets();
            }

            dialogue.AllNodes.RemoveAll(n => n == null);

            dialogue.AllNodes.ForEach(n =>
            {
                NodeView view = CreateNodeView(n);
                views.Add(view);
            });

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

            dialogue.OnGenericOptionsChange -= DelayedRefresh;
            dialogue.OnGenericOptionsChange += DelayedRefresh;

            dialogue.OnValidate();
            dialogue.AllNodes.ForEach(n =>
            {
                n.OnValidate();
            });

            OnPopulateView?.Invoke();
        }

        void DelayedRefresh()
        {
            EditorApplication.delayCall -= Refresh;
            EditorApplication.delayCall += Refresh;
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

            Node node;
            if (from == null) node = DialogueSystem.CreateNode(type, m_dialogue);
            else node = DialogueSystem.CreateNode(from, m_dialogue);

            m_dialogue.AllNodes.Remove(node);

            node.Position.x = atPosition.x;
            node.Position.y = atPosition.y;

            AssetDatabase.AddObjectToAsset(node, m_dialogue);
            Undo.RegisterCreatedObjectUndo(node, "Dialogue (Node Asset Created)");

            Undo.RecordObject(m_dialogue, "Dialogue (Node Added to AllNodes)");
            m_dialogue.AllNodes.Add(node);

            EditorUtility.SetDirty(m_dialogue);

            if (from != null && from.CustomData != null)
            {
                NodeCustomDataBase newCustomData = ScriptableObject.Instantiate(from.CustomData);
                newCustomData.name = DialogueSystem.GenerateCustomDataName(node);
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
                        optionCustomData.name = DialogueSystem.GenerateOptionDataName(node);
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

            view.Node.OnRemoveFromDialogue();
            m_dialogue.AllNodes.Remove(view.Node);

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
    }

}