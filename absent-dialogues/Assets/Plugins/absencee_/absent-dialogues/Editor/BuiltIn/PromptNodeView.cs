using com.absence.dialoguesystem.internals;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using Node = com.absence.dialoguesystem.internals.Node;
using UnityEngine;

namespace com.absence.dialoguesystem.editor
{
    [CustomNodeView(typeof(PromptNode))]
    public sealed class PromptNodeView : NodeView
    {
        private PromptNode m_nodeAsDecisive;
        private Button m_createNewOptionButton;
        private List<VisualElement> m_optionElems = new List<VisualElement>();
        private List<VisualElement> m_genericOptionElems = new List<VisualElement>();
        GUID m_assetGuid;

        public PromptNodeView(Node node, DialogueGraphView graph = null) : base(node, graph)
        {
            node.onValidation -= RefreshOptionViews;
            node.onValidation += RefreshOptionViews;

            Graph.m_dialogue.OnValidateAction -= RefreshGenericOptionViews;
            Graph.m_dialogue.OnValidateAction += RefreshGenericOptionViews;
        }

        protected override void OnBeforeDraw()
        {
            m_nodeAsDecisive = Node as PromptNode;

            if (m_nodeAsDecisive.GenericOptionLeads == null) m_nodeAsDecisive.GenericOptionLeads = new();
            List<Node> temp = new(m_nodeAsDecisive.GenericOptionLeads);
            m_nodeAsDecisive.GenericOptionLeads.Clear();

            for (int i = 0; i < Graph.m_dialogue.GenericOptions.Count; i++)
            {
                Node target = null;

                if (i < temp.Count) target = temp[i];

                m_nodeAsDecisive.GenericOptionLeads.Add(target);
            }

            EditorUtility.SetDirty(Node);
            AssetDatabase.SaveAssetIfDirty(m_assetGuid);
        }

        protected override void Draw()
        {
            m_assetGuid = AssetDatabase.GUIDFromAssetPath(AssetDatabase.GetAssetPath(Node));

            m_createNewOptionButton = new Button(CreateOption);
            m_createNewOptionButton.text = "Add Option";
            m_createNewOptionButton.AddToClassList("addNewOptionButton");
            mainContainer.Add(m_createNewOptionButton);

            Refresh();

            for (int i = 0; i < Graph.m_dialogue.GenericOptions.Count; i++)
            {
                VisualElement elem = Graph.CreateGenericOptionElement(this, i);
                m_genericOptionElems.Add(elem);
                mainContainer.Add(elem);
                Outputs.Add(elem.Q<Port>());
            }
        }

        private void RefreshGenericOptionViews()
        {
            m_genericOptionElems.ForEach(op =>
            {
                Label showIfLabel = op.Q<Label>("show-if-label");
                TextField textField = op.Q<TextField>();

                int index = m_genericOptionElems.IndexOf(op);

                if (index >= Graph.m_dialogue.GenericOptions.Count)
                    return;

                Option target = Graph.m_dialogue.GenericOptions[index];

                showIfLabel.visible = target.UseShowIf;
                showIfLabel.tooltip = target.Visibility.GetConditionString(true);
                textField.SetValueWithoutNotify(target.Text);
            });
        }

        private void RefreshOptionViews()
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

        private void CreateOption()
        {
            Option option = new Option();

            if (m_nodeAsDecisive.Options.Count == 0) m_nodeAsDecisive.NativeNextNode = null;
            Undo.RegisterCompleteObjectUndo(m_nodeAsDecisive, "Prompt Node (Modified)");
            m_nodeAsDecisive.Options.Add(option);

            EditorUtility.SetDirty(m_nodeAsDecisive);
            AssetDatabase.SaveAssetIfDirty(m_assetGuid);

            Graph.Refresh();
        }

        private void Refresh()
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
                Outputs.Add(e.Q<Port>());
            });
        }

        private VisualElement CreateOptionView(int index, SerializedProperty optionProp)
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

                Undo.RegisterCompleteObjectUndo(m_nodeAsDecisive, "Prompt Node (Modified)");
                m_nodeAsDecisive.RemoveOutputConnection(Outputs.IndexOf(optionElem.Q<Port>()));
                m_nodeAsDecisive.Options.Remove(target);

                EditorUtility.SetDirty(m_nodeAsDecisive);
                AssetDatabase.SaveAssetIfDirty(m_assetGuid);

                m_optionElems.Remove(optionElem);
                mainContainer.Remove(optionElem);

                Graph.Refresh();
            });

            Button moveUpButton = new Button(() =>
            {
                Undo.RegisterCompleteObjectUndo(m_nodeAsDecisive, "Prompt Node (Modified)");

                int targetIndex = index - 1;

                Option optionToReplace = m_nodeAsDecisive.Options[targetIndex];
                Option self = m_nodeAsDecisive.Options[index];

                m_nodeAsDecisive.Options[index] = optionToReplace;
                m_nodeAsDecisive.Options[targetIndex] = self;

                EditorUtility.SetDirty(m_nodeAsDecisive);
                AssetDatabase.SaveAssetIfDirty(m_assetGuid);

                Graph.Refresh();
            });

            Button moveDownButton = new Button(() =>
            {
                Undo.RegisterCompleteObjectUndo(m_nodeAsDecisive, "Prompt Node (Modified)");

                int targetIndex = index + 1;

                Option optionToReplace = m_nodeAsDecisive.Options[targetIndex];
                Option self = m_nodeAsDecisive.Options[index];

                m_nodeAsDecisive.Options[index] = optionToReplace;
                m_nodeAsDecisive.Options[targetIndex] = self;

                EditorUtility.SetDirty(m_nodeAsDecisive);
                AssetDatabase.SaveAssetIfDirty(m_assetGuid);

                Graph.Refresh();
            });

            removeButton.text = "x";
            removeButton.AddToClassList("removeOptionButton");

            moveUpButton.text = "↑";
            moveUpButton.AddToClassList("moveOptionUpButton");
            moveUpButton.SetEnabled(index > 0);

            moveDownButton.text = "↓";
            moveDownButton.AddToClassList("moveOptionDownButton");
            moveDownButton.SetEnabled(index < m_nodeAsDecisive.Options.Count - 1);

            Port port = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(bool));
            port.AddToClassList("optionPort");
            port.portName = "";

            TextField speechField = new TextField();
            speechField.AddToClassList("optionField");
            speechField.multiline = true;

            speechField.BindProperty(speechProp);

            removeButton.tooltip = "Remove this option.";

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
