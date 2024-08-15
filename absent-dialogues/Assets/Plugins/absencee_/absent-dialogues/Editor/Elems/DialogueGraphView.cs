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

namespace com.absence.dialoguesystem.editor
{
    /// <summary>
    /// The graph view responsible for rendering a dialogue's graph elements.
    /// </summary>
    [HelpURL("https://b1lodhand.github.io/absent-dialogues/api/com.absence.dialoguesystem.editor.DialogueGraphView.html")]
    public sealed class DialogueGraphView : GraphView
    {
        public new class UxmlFactory : UxmlFactory<DialogueGraphView, GraphView.UxmlTraits> { }

        internal Dialogue m_dialogue;

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

            Undo.undoRedoPerformed -= OnUndoRedo;
            Undo.undoRedoPerformed += OnUndoRedo;
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
            Refresh();
            AssetDatabase.SaveAssets();
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
                        if (nodeView.Node.Equals(m_dialogue.RootNode)) return;

                        DeleteNode(nodeView);
                        return;
                    }

                    Edge edge = elem as Edge;
                    if (edge != null)
                    {
                        NodeView outputView = edge.output.node as NodeView;
                        NodeView inputView = edge.input.node as NodeView;

                        Undo.RecordObject(outputView.Node, "Dialog (Remove Child)");
                        outputView.Node.RemoveNextNode(outputView.Outputs.IndexOf(edge.output));
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

                    Undo.RecordObject(outputView.Node, "Dialog (Add Child)");
                    outputView.Node.AddNextNode(inputView.Node, outputView.Outputs.IndexOf(edge.output));
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
            foreach (var type in types)
            {
                if (type.Equals(typeof(RootNode))) continue;

                var mousePos = viewTransform.matrix.inverse.MultiplyPoint(evt.localMousePosition);
                evt.menu.AppendAction($"{Helpers.SplitCamelCase(type.Name, " ")}", a =>
                {
                    CreateNode(type, mousePos);
                });
            }
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

            if (m_dialogue.RootNode == null)
            {
                m_dialogue.RootNode = m_dialogue.CreateNode(typeof(RootNode)) as RootNode;

                AssetDatabase.AddObjectToAsset(m_dialogue.RootNode, m_dialogue);
                EditorUtility.SetDirty(m_dialogue);
                AssetDatabase.SaveAssets();
            }

            dialogue.AllNodes.ForEach(n => CreateNodeView(n));

            dialogue.AllNodes.ForEach(n =>
            {
                if (n == null) return;

                var nexts = n.GetNextNodes();
                nexts.ForEach(c =>
                {
                    NodeView startView = FindNodeView(n);
                    NodeView endView = FindNodeView(c.node);

                    Edge edge = startView.Outputs[c.portIndex].ConnectTo(endView.Input);
                    AddElement(edge);
                });
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

        Node CreateNode(System.Type type, Vector2 atPosition)
        {
            Undo.RecordObject(m_dialogue, "Dialog (Create Node)");

            Node node = m_dialogue.CreateNode(type);
            node.Guid = GUID.Generate().ToString();
            node.Position.x = atPosition.x;
            node.Position.y = atPosition.y;

            AssetDatabase.AddObjectToAsset(node, m_dialogue);
            Undo.RegisterCreatedObjectUndo(node, "Dialog (Create Node)");

            AssetDatabase.SaveAssets();

            NodeView viewOfNodeCreated = CreateNodeView(node);

            Refresh();
            SelectNode(node);

            OnNodeCreated?.Invoke(node);

            return node;
        }

        void DeleteNode(NodeView view)
        {
            Undo.RecordObject(m_dialogue, "Dialog (Delete Node)");
            m_dialogue.DeleteNode(view.Node);

            OnBeforeNodeDeleted?.Invoke(view.Node);

            Undo.DestroyObjectImmediate(view.Node);
            AssetDatabase.SaveAssets();
        }

        NodeView CreateNodeView(Node node)
        {
            if(node == null) return null;

            NodeView nodeView = new NodeView(node);
            nodeView.Master = this;
            nodeView.OnNodeSelected = OnNodeSelected;
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