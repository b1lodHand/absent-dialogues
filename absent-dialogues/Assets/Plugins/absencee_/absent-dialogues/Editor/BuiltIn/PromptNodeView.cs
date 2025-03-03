using com.absence.dialoguesystem.internals;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using Node = com.absence.dialoguesystem.internals.Node;

namespace com.absence.dialoguesystem.editor
{
    [CustomNodeView(typeof(PromptNode))]
    public class PromptNodeView : NodeView
    {
        private PromptNode m_nodeAsPrompt;
        private Button m_createNewOptionButton;
        private List<VisualElement> m_optionElems = new List<VisualElement>();
        private List<VisualElement> m_genericOptionElems = new List<VisualElement>();

        public PromptNodeView(Node node, DialogueGraphView graph = null) : base(node, graph)
        {
            node.onValidation -= RefreshOptionViews;
            node.onValidation += RefreshOptionViews;

            Graph.m_dialogue.OnValidateAction -= RefreshGenericOptionViews;
            Graph.m_dialogue.OnValidateAction += RefreshGenericOptionViews;
        }

        protected override void OnAfterStylesApplied()
        {
            m_nodeAsPrompt = Node as PromptNode;
        }

        protected override void OnDraw()
        {
            m_assetGuid = AssetDatabase.GUIDFromAssetPath(AssetDatabase.GetAssetPath(Node));

            m_createNewOptionButton = new Button(CreateOption);
            m_createNewOptionButton.text = "Add Option";
            m_createNewOptionButton.AddToClassList("addNewOptionButton");
            mainContainer.Add(m_createNewOptionButton);

            DrawOptions();
            DrawGenericOptions();
        }

        private void RefreshGenericOptionViews()
        {
            for (int i = 0; i < m_genericOptionElems.Count; i++)
            {
                VisualElement element = m_genericOptionElems[i];
                Label showIfLabel = element.Q<Label>("show-if-label");
                TextField textField = element.Q<TextField>();

                if (i >= Node.GenericOptions.Count)
                    return;

                GenericOptionReference reference = Node.GenericOptions[i];

                bool useShowIf = reference.Target.UseShowIf;
                bool bypass = reference.Bypass;

                showIfLabel.visible = useShowIf || bypass;

                if (reference.Bypass) showIfLabel.text = "Bypassed.";
                else if (reference.Target.UseShowIf) showIfLabel.text = "Conditional visibility active.";

                showIfLabel.tooltip = reference.Target.Visibility.GetConditionString(true);
                textField.SetValueWithoutNotify(reference.Target.Text);
            }
        }
        private void RefreshOptionViews()
        {
            try
            {
                m_optionElems.ForEach(optionElem =>
                {
                    Option targetOption = m_nodeAsPrompt.Options[m_optionElems.IndexOf(optionElem)];
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

        protected virtual void DrawOptions()
        {
            var optionsProp = m_serializedNode.FindProperty("m_options").Copy();

            optionsProp.Next(true);
            optionsProp.Next(true);

            var optionArrayLength = optionsProp.intValue;
            var lastIndex = optionArrayLength - 1;

            optionsProp.Next(true);

            for (int i = 0; i < optionArrayLength; i++)
            {
                m_optionElems.Add(CreateOptionView(i, optionsProp));
                if (i < lastIndex) optionsProp.Next(false);
            }

            m_optionElems.ForEach(e =>
            {
                mainContainer.Add(e);

                Port port = e.Q<Port>("option-direct-port");

                Outputs.Add(port);
            });
        }
        protected virtual void DrawGenericOptions()
        {
            for (int i = 0; i < Node.GenericOptions.Count; i++)
            {
                VisualElement elem = Graph.CreateGenericOptionElement(this, Node.GenericOptions[i]);
                m_genericOptionElems.Add(elem);
                mainContainer.Add(elem);

                Port port = elem.Q<Port>("option-direct-port");

                Outputs.Add(port);
            }
        }

        protected override void OnDisconnectAll(ref HashSet<GraphElement> toDelete)
        {
            foreach (VisualElement option in m_optionElems)
            {
                AddConnectionsToDeleteSet(option, ref toDelete);
            }

            foreach (VisualElement option in m_genericOptionElems)
            {
                AddConnectionsToDeleteSet(option, ref toDelete);
            }
        }

        protected override DropdownMenuAction.Status DisconnectAllStatus(DropdownMenuAction action)
        {
            DropdownMenuAction.Status result = base.DisconnectAllStatus(action);

            if (result == DropdownMenuAction.Status.Normal) 
                return result;

            foreach (VisualElement option in m_optionElems)
            {
                foreach (Port elem in option.Query<Port>().ToList())
                {
                    if (elem.connected)
                    {
                        result = DropdownMenuAction.Status.Normal;
                    }
                }
            }

            foreach (VisualElement option in m_genericOptionElems)
            {
                foreach (Port elem in option.Query<Port>().ToList())
                {
                    if (elem.connected)
                    {
                        result = DropdownMenuAction.Status.Normal;
                    }
                }
            }

            return result;
        }

        protected virtual void CreateOption()
        {
            Option option = new Option();

            if (m_nodeAsPrompt.Options.Count == 0) m_nodeAsPrompt.NativeNextNode = null;
            Undo.RegisterCompleteObjectUndo(m_nodeAsPrompt, "Prompt Node (Modified)");
            m_nodeAsPrompt.Options.Add(option);

            EditorUtility.SetDirty(m_nodeAsPrompt);
            AssetDatabase.SaveAssetIfDirty(m_assetGuid);

            Graph.Refresh();
        }
        protected virtual VisualElement CreateOptionView(int index, SerializedProperty optionProp)
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
                var target = m_nodeAsPrompt.Options[index];

                Undo.RegisterCompleteObjectUndo(m_nodeAsPrompt, "Prompt Node (Modified)");
                m_nodeAsPrompt.RemoveOutputConnection(Outputs.IndexOf(optionElem.Q<Port>()));
                m_nodeAsPrompt.Options.Remove(target);

                EditorUtility.SetDirty(m_nodeAsPrompt);
                AssetDatabase.SaveAssetIfDirty(m_assetGuid);

                m_optionElems.Remove(optionElem);
                mainContainer.Remove(optionElem);

                Graph.Refresh();
            });

            Button moveUpButton = new Button(() =>
            {
                Undo.RegisterCompleteObjectUndo(m_nodeAsPrompt, "Prompt Node (Modified)");

                int targetIndex = index - 1;

                Option optionToReplace = m_nodeAsPrompt.Options[targetIndex];
                Option self = m_nodeAsPrompt.Options[index];

                m_nodeAsPrompt.Options[index] = optionToReplace;
                m_nodeAsPrompt.Options[targetIndex] = self;

                EditorUtility.SetDirty(m_nodeAsPrompt);
                AssetDatabase.SaveAssetIfDirty(m_assetGuid);

                Graph.Refresh();
            });

            Button moveDownButton = new Button(() =>
            {
                Undo.RegisterCompleteObjectUndo(m_nodeAsPrompt, "Prompt Node (Modified)");

                int targetIndex = index + 1;

                Option optionToReplace = m_nodeAsPrompt.Options[targetIndex];
                Option self = m_nodeAsPrompt.Options[index];

                m_nodeAsPrompt.Options[index] = optionToReplace;
                m_nodeAsPrompt.Options[targetIndex] = self;

                EditorUtility.SetDirty(m_nodeAsPrompt);
                AssetDatabase.SaveAssetIfDirty(m_assetGuid);

                Graph.Refresh();
            });

            removeButton.text = "×";
            removeButton.AddToClassList("removeOptionButton");
            removeButton.tooltip = "Remove";

            moveUpButton.text = "↑";
            moveUpButton.AddToClassList("moveOptionUpButton");
            moveUpButton.SetEnabled(index > 0);
            moveUpButton.tooltip = "Move up";

            moveDownButton.text = "↓";
            moveDownButton.AddToClassList("moveOptionDownButton");
            moveDownButton.SetEnabled(index < m_nodeAsPrompt.Options.Count - 1);
            moveDownButton.tooltip = "Move down";

            Port port = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(bool));
            port.AddToClassList("optionPort");
            port.portName = "";
            port.name = "option-direct-port";

            TextField speechField = new TextField();
            speechField.AddToClassList("optionField");
            speechField.multiline = true;

            speechField.BindProperty(speechProp);

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

            void RefreshShowIfLabel()
            {
                showIfLabel.visible = optionProp.FindPropertyRelative("m_useShowIf").boolValue;
            }

            return optionElem;
        }
    }
}
