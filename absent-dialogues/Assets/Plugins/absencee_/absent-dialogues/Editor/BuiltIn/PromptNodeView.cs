using com.absence.dialoguesystem.internals;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEditor;
using UnityEngine.UIElements;
using Node = com.absence.dialoguesystem.internals.Node;

namespace com.absence.dialoguesystem.editor.internals
{
    [CustomNodeView(typeof(PromptNode))]
    public class PromptNodeView : NodeView
    {
        private PromptNode m_nodeAsPrompt;
        private Button m_createNewOptionButton;
        private List<OptionView> m_optionElems = new List<OptionView>();
        private List<GenericOptionReferenceView> m_genericOptionElems = new List<GenericOptionReferenceView>();

        public PromptNodeView(Node node, DialogueGraphView graph = null) : base(node, graph)
        {
            node.onValidation -= RefreshOptionViews;
            node.onValidation += RefreshOptionViews;

            Graph.m_dialogue.OnValidateAction -= RefreshGenericOptionViews;
            Graph.m_dialogue.OnValidateAction += RefreshGenericOptionViews;

            ApplyHardcodedStyle(EditorSettings.instance);
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
            try
            {
                m_genericOptionElems.ForEach(genericOptionElem =>
                {
                    genericOptionElem.Refresh();
                });
            }

            catch
            {
                return;
            }
        }
        private void RefreshOptionViews()
        {
            try
            {
                m_optionElems.ForEach(optionElem =>
                {
                    optionElem.Refresh();
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
                CreateOptionView(i, optionsProp);
                if (i < lastIndex) optionsProp.Next(false);
            }
        }
        protected virtual void DrawGenericOptions()
        {
            for (int i = 0; i < Node.GenericOptions.Count; i++)
            {
                CreateGenericOptionView(i);
            }
        }

        internal override void ApplyHardcodedStyle(EditorSettings settings)
        {
            for (int i = 0; i < m_optionElems.Count; i++)
            {
                VisualElement optionView = m_optionElems[i];
                //Option target = m_nodeAsPrompt.Options[i];
                optionView.Q<Button>().style.backgroundColor = settings.NegativeColor;
                optionView.Q<Button>().style.color = settings.TextColor;
            }

            for (int i = 0; i < m_genericOptionElems.Count; i++)
            {
                VisualElement genericOptionView = m_genericOptionElems[i];
                GenericOptionReference reference = m_nodeAsPrompt.GenericOptions[i];
                genericOptionView.Q<Button>().style.backgroundColor = reference.Bypass ? 
                    settings.NeutralColor : settings.PositiveColor;
                genericOptionView.Q<Button>().style.color = reference.Bypass ?
                    settings.TextColor : settings.AlternativeTextColor;
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
        protected virtual OptionView CreateOptionView(int index, SerializedProperty optionProp)
        {
            OptionView view = 
                OptionView.Create(m_nodeAsPrompt.Options[index], optionProp, 
                () => InstantiatePort(Orientation.Horizontal, Direction.Output, 
                Port.Capacity.Single, typeof(bool)));

            view.OnRefresh(OnOptionViewRefresh);
            view.OnRemoveButtonClicked(OnOptionViewRemoveButtonClicked);
            view.OnMoveUpButtonClicked(OnOptionViewMoveUpButtonClicked);
            view.OnMoveDownButtonClicked(OnOptionViewMoveDownButtonClicked);

            view.SetEnabledOfMoveUpButton(index > 0);
            view.SetEnabledOfMoveDownButton(index < m_nodeAsPrompt.Options.Count - 1);

            m_optionElems.Add(view);
            Outputs.Add(view.Port);
            mainContainer.Add(view);
            return view;
        }
        protected virtual GenericOptionReferenceView CreateGenericOptionView(int index)
        {
            GenericOptionReferenceView view =
                GenericOptionReferenceView.Create(m_nodeAsPrompt.GenericOptions[index],
                () => InstantiatePort(Orientation.Horizontal, Direction.Output,
                Port.Capacity.Single, typeof(bool)));

            view.OnRefresh(OnGenericOptionRefresh);
            view.OnBypassButtonClicked(OnGenericOptionBypassButtonClicked);

            m_genericOptionElems.Add(view);
            Outputs.Add(view.Port);
            mainContainer.Add(view);
            return view;
        }

        private void OnGenericOptionBypassButtonClicked(GenericOptionReferenceView view)
        {
            GenericOptionReference reference = view.Reference;
            bool hasNoCertainOptions = Node.NoCertainOptions;

            Undo.RegisterCompleteObjectUndo(Node, "Node (Generic Option Bypass Button)");

            reference.Bypass = !reference.Bypass;

            EditorUtility.SetDirty(Node);

            if (hasNoCertainOptions != Node.NoCertainOptions)
            {
                Graph.Refresh();
                return;
            }

            view.Refresh();
        }

        private void OnGenericOptionRefresh(GenericOptionReferenceView view)
        {

        }

        private void OnOptionViewMoveDownButtonClicked(OptionView view)
        {
            Undo.RegisterCompleteObjectUndo(m_nodeAsPrompt, "Prompt Node (Modified)");

            int index = m_optionElems.IndexOf(view);
            int targetIndex = index + 1;

            Option optionToReplace = m_nodeAsPrompt.Options[targetIndex];
            Option self = m_nodeAsPrompt.Options[index];

            m_nodeAsPrompt.Options[index] = optionToReplace;
            m_nodeAsPrompt.Options[targetIndex] = self;

            EditorUtility.SetDirty(m_nodeAsPrompt);
            AssetDatabase.SaveAssetIfDirty(m_assetGuid);

            Graph.Refresh();
        }

        private void OnOptionViewMoveUpButtonClicked(OptionView view)
        {
            Undo.RegisterCompleteObjectUndo(m_nodeAsPrompt, "Prompt Node (Modified)");

            int index = m_optionElems.IndexOf(view);
            int targetIndex = index - 1;

            Option optionToReplace = m_nodeAsPrompt.Options[targetIndex];
            Option self = m_nodeAsPrompt.Options[index];

            m_nodeAsPrompt.Options[index] = optionToReplace;
            m_nodeAsPrompt.Options[targetIndex] = self;

            EditorUtility.SetDirty(m_nodeAsPrompt);
            AssetDatabase.SaveAssetIfDirty(m_assetGuid);

            Graph.Refresh();
        }

        private void OnOptionViewRemoveButtonClicked(OptionView view)
        {
            Undo.RegisterCompleteObjectUndo(m_nodeAsPrompt, "Prompt Node (Modified)");
            m_nodeAsPrompt.RemoveOutputConnection(Outputs.IndexOf(view.Port));
            m_nodeAsPrompt.Options.Remove(view.Target);

            EditorUtility.SetDirty(m_nodeAsPrompt);
            AssetDatabase.SaveAssetIfDirty(m_assetGuid);

            m_optionElems.Remove(view);
            mainContainer.Remove(view);

            Graph.Refresh();
        }

        private void OnOptionViewRefresh(OptionView view)
        {
            
        }
    }
}
