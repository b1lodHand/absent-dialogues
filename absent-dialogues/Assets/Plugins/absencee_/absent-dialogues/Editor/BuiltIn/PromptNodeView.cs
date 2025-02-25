using com.absence.dialoguesystem.internals;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using Node = com.absence.dialoguesystem.internals.Node;

namespace com.absence.dialoguesystem.editor
{
    [CustomNodeView(typeof(DecisionSpeechNode))]
    public class PromptNodeView : NodeView
    {
        private DecisionSpeechNode m_nodeAsDecisive;

        public PromptNodeView(Node node, DialogueGraphView graph = null) : base(node, graph)
        {
            node.onValidation -= RefreshOptionLabels;
            node.onValidation += RefreshOptionLabels;

            Graph.m_dialogue.OnValidateAction -= RefreshGenericOptionElems;
            Graph.m_dialogue.OnValidateAction += RefreshGenericOptionElems;
        }

        protected override void Draw()
        {
            m_createNewOptionButton = new Button(CreateOption_DecisionSpeechNode);
            m_createNewOptionButton.text = "Add Option";
            m_createNewOptionButton.AddToClassList("addNewOptionButton");
            mainContainer.Add(m_createNewOptionButton);

            RefreshOptions_DecisionSpeechNode();

            List<Node> temp = m_nodeAsDecisive.GenericOptionLeads;
            m_nodeAsDecisive.GenericOptionLeads = new List<Node>(Graph.m_dialogue.GenericOptions.Count);

            for (int i = 0; i < temp.Count; i++)
            {
                m_nodeAsDecisive.GenericOptionLeads[i] = temp[i];
            }

            EditorUtility.SetDirty(Node);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            for (int i = 0; i < Graph.m_dialogue.GenericOptions.Count; i++)
            {
                VisualElement elem = Graph.CreateGenericOptionElement(this, i);
                m_genericOptionElems.Add(elem);
                mainContainer.Add(elem);
                Outputs.Add(elem.Q<Port>());
            }
        }

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

        private void CreateOption()
        {
            Option option = new Option();

            Undo.RecordObject(m_nodeAsDecisive, "Decision Node (Modified)");
            m_nodeAsDecisive.Options.Add(option);

            EditorUtility.SetDirty(m_nodeAsDecisive);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Graph.Refresh();
        }

        private void RefreshOptions()
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
                m_optionElems.Add(CreateOptionElem(i, optionsProp));
                if (i < lastIndex) optionsProp.Next(false);
            }

            m_optionElems.ForEach(e =>
            {
                mainContainer.Add(e);
                Outputs.Add(e.Q<Port>());
            });
        }

        private VisualElement CreateOptionElem(int index, SerializedProperty optionProp)
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

                Graph.Refresh();
            });

            Button moveUpButton = new Button(() =>
            {
                Undo.RecordObject(m_nodeAsDecisive, "Decision Node (Modified)");

                int targetIndex = index - 1;

                Option optionToReplace = m_nodeAsDecisive.Options[targetIndex];
                Option self = m_nodeAsDecisive.Options[index];

                m_nodeAsDecisive.Options[index] = optionToReplace;
                m_nodeAsDecisive.Options[targetIndex] = self;

                EditorUtility.SetDirty(m_nodeAsDecisive);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                Graph.Refresh();
            });

            Button moveDownButton = new Button(() =>
            {
                Undo.RecordObject(m_nodeAsDecisive, "Decision Node (Modified)");

                int targetIndex = index + 1;

                Option optionToReplace = m_nodeAsDecisive.Options[targetIndex];
                Option self = m_nodeAsDecisive.Options[index];

                m_nodeAsDecisive.Options[index] = optionToReplace;
                m_nodeAsDecisive.Options[targetIndex] = self;

                EditorUtility.SetDirty(m_nodeAsDecisive);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

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
