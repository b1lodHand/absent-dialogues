using com.absence.dialoguesystem.internals;
using System;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace com.absence.dialoguesystem.editor
{
    public class OptionView : VisualElement
    {
        class StyleSheetPostprocessor : AssetPostprocessor
        {
            static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths, bool didDomainReload)
            {
                if (!didDomainReload)
                    return;

                s_styleSheet =
                    AssetDatabase.LoadAssetAtPath<StyleSheet>
                    ("Assets/Plugins/absencee_/absent-dialogues/Editor/Elems/OptionViewStyle.uss");
            }
        }

        static StyleSheet s_styleSheet;
        public static StyleSheet DefaultStyleSheet => s_styleSheet;

        public static OptionView Create(Option target, SerializedProperty targetProperty, Func<Port> portCreator)
        {
            OptionView root = new OptionView(target, targetProperty, portCreator);
            return root;
        }

        public Option Target => m_target;
        public SerializedProperty TargetProperty => m_targetProperty;
        public Port Port => m_leadingPort;

        public event Action<OptionView> onRefresh;
        public event Action<OptionView> onRemoveButtonClicked;
        public event Action<OptionView> onMoveUpButtonClicked;
        public event Action<OptionView> onMoveDownButtonClicked;

        Option m_target;
        SerializedProperty m_targetProperty;

        Button m_removeButton;
        Button m_moveUpButton;
        Button m_moveDownButton;
        TextField m_textField;
        TextElement m_textFieldTextElement;
        VisualElement m_divider;
        Port m_leadingPort;
        Label m_showIfLabel;

        internal OptionView(Option target, SerializedProperty targetProperty, Func<Port> portCreator)
        {
            m_target = target;
            m_targetProperty = targetProperty;

            this.styleSheets.Add(s_styleSheet);

            VisualElement top = new VisualElement();
            top.AddToClassList("optionBottom");
            top.name = "top";

            VisualElement divider = new VisualElement();
            divider.AddToClassList("optionDivider");

            VisualElement bottom = new VisualElement();
            bottom.AddToClassList("optionBottom");

            var speechProp = m_targetProperty.FindPropertyRelative("Text");

            Button removeButton = new Button(() => onRemoveButtonClicked?.Invoke(this));
            removeButton.RegisterCallback<MouseEnterEvent>(evt => ApplyHardcodedStyle(EditorSettings.instance));
            removeButton.RegisterCallback<MouseOutEvent>(evt => ApplyHardcodedStyle(EditorSettings.instance));

            Button moveUpButton = new Button(() => onMoveUpButtonClicked?.Invoke(this));
            moveUpButton.RegisterCallback<MouseEnterEvent>(evt => ApplyHardcodedStyle(EditorSettings.instance));
            moveUpButton.RegisterCallback<MouseOutEvent>(evt => ApplyHardcodedStyle(EditorSettings.instance));

            Button moveDownButton = new Button(() => onMoveDownButtonClicked?.Invoke(this));
            moveDownButton.RegisterCallback<MouseEnterEvent>(evt => ApplyHardcodedStyle(EditorSettings.instance));
            moveDownButton.RegisterCallback<MouseOutEvent>(evt => ApplyHardcodedStyle(EditorSettings.instance));

            removeButton.text = "×";
            removeButton.AddToClassList("removeOptionButton");
            removeButton.tooltip = "Remove";

            moveUpButton.text = "↑";
            moveUpButton.AddToClassList("moveOptionUpButton");
            moveUpButton.tooltip = "Move up";

            moveDownButton.text = "↓";
            moveDownButton.AddToClassList("moveOptionDownButton");
            moveDownButton.tooltip = "Move down";

            Port port = portCreator.Invoke();
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

            bottom.Add(speechField);
            bottom.Add(port);

            this.Add(divider);
            this.Add(top);
            this.Add(bottom);

            m_divider = divider;
            m_textField = speechField;
            m_textFieldTextElement = m_textField.Q<TextElement>();
            m_removeButton = removeButton;
            m_moveUpButton = moveUpButton;
            m_moveDownButton = moveDownButton;
            m_showIfLabel = showIfLabel;
            m_leadingPort = port;

            Refresh();
        }

        public void ApplyHardcodedStyle(EditorSettings settings)
        {

        }

        public void Refresh()
        {
            m_showIfLabel.visible = m_target.UseShowIf;
            ApplyHardcodedStyle(EditorSettings.instance);
            onRefresh?.Invoke(this);
        }

        public void SetEnabledOfMoveUpButton(bool enabled)
        {
            m_moveUpButton.SetEnabled(enabled);
        }

        public void SetEnabledOfMoveDownButton(bool enabled)
        {
            m_moveDownButton.SetEnabled(enabled);
        }

        public void OnRefresh(Action<OptionView> action)
        {
            onRefresh += action;
        }

        public void OnRemoveButtonClicked(Action<OptionView> action)
        {
            onRemoveButtonClicked += action;
        }

        public void OnMoveUpButtonClicked(Action<OptionView> action)
        {
            onMoveUpButtonClicked += action;
        }

        public void OnMoveDownButtonClicked(Action<OptionView> action)
        {
            onMoveDownButtonClicked += action;
        }
    }
}
