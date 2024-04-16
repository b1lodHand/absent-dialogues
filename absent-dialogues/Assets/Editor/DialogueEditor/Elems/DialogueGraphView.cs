using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using com.absence.dialoguesystem.internals;
using Node = com.absence.dialoguesystem.internals.Node;
using System.Text.RegularExpressions;
using com.absence.utilities;

namespace com.absence.dialoguesystem.editor
{
    public class DialogueGraphView : GraphView
    {
        public new class UxmlFactory : UxmlFactory<DialogueGraphView, GraphView.UxmlTraits> { }

        Dialogue m_dialogue;
        public event Action<NodeView> OnNodeSelected;
        public event Action OnPopulateView;

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
            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Editor/DialogueEditor/DialogueEditorWindow.uss");
            styleSheets.Add(styleSheet);
        }
        private void AddMiniMap()
        {
            var mapFoldout = new Foldout() { focusable = false, value = true, text = "MiniMap" };
            var miniMap = new MiniMap() { anchored = true };

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

                //Refresh();
            }

            //if(graphViewChange.movedElements != null && graphViewChange.movedElements.Any(e => e is NodeView))
            //{
            //    Refresh();
            //}

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

                evt.menu.AppendAction($"{Helpers.SplitCamelCase(type.Name, " ")}", a => CreateNode(type, evt.originalMousePosition));
            }
        }

        internal void ClearViewWithoutNotification()
        {
            graphViewChanged -= OnGraphViewChanged;
            DeleteElements(graphElements);
            graphViewChanged += OnGraphViewChanged;
        }

        internal void PopulateView(Dialogue dialog)
        {
            this.m_dialogue = dialog;

            ClearViewWithoutNotification();

            if (m_dialogue == null) return;
            if (m_dialogue.RootNode == null)
            {
                m_dialogue.RootNode = m_dialogue.CreateNode(typeof(RootNode)) as RootNode;
                EditorUtility.SetDirty(m_dialogue);
                AssetDatabase.SaveAssets();
            }

            dialog.AllNodes.ForEach(n => CreateNodeView(n));

            dialog.AllNodes.ForEach(n =>
            {
                var nexts = n.GetNextNodes();
                nexts.ForEach(c =>
                {
                    NodeView startView = FindNodeView(n);
                    NodeView endView = FindNodeView(c.node);

                    Edge edge = startView.Outputs[c.portIndex].ConnectTo(endView.input);
                    AddElement(edge);
                });
            });

            OnPopulateView?.Invoke();
        }
        public void Refresh()
        {
            if (m_dialogue == null) return;
            PopulateView(m_dialogue);
        }

        public NodeView FindNodeView(Node node)
        {
            return GetNodeByGuid(node.Guid) as NodeView;
        }
        Node CreateNode(System.Type type, Vector2 atPosition)
        {
            Undo.RecordObject(m_dialogue, "Dialog (Create Node)");

            Node node = m_dialogue.CreateNode(type);
            node.Guid = GUID.Generate().ToString();

            AssetDatabase.AddObjectToAsset(node, m_dialogue);
            Undo.RegisterCreatedObjectUndo(node, "Dialog (Create Node)");

            AssetDatabase.SaveAssets();

            node.Position = atPosition;
            NodeView viewOfNodeCreated = CreateNodeView(node);

            Refresh();
            ClearSelection();
            AddToSelection(viewOfNodeCreated);

            return node;
        }

        void DeleteNode(NodeView view)
        {
            Undo.RecordObject(m_dialogue, "Dialog (Delete Node)");
            m_dialogue.DeleteNode(view.Node);
            Undo.DestroyObjectImmediate(view.Node);
            AssetDatabase.SaveAssets();
        }

        NodeView CreateNodeView(Node node)
        {
            NodeView nodeView = new NodeView(node);
            nodeView.Master = this;
            nodeView.OnNodeSelected = OnNodeSelected;
            AddElement(nodeView);

            return nodeView;
        }
    }

}