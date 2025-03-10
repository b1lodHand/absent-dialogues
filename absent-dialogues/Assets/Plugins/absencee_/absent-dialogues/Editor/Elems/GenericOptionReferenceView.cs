using com.absence.dialoguesystem.internals;
using System;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace com.absence.dialoguesystem.editor
{
    public class GenericOptionReferenceView : VisualElement
    {
        class StyleSheetPostprocessor : AssetPostprocessor
        {
            static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths, bool didDomainReload)
            {
                if (!didDomainReload)
                    return;

                s_styleSheet =
                    AssetDatabase.LoadAssetAtPath<StyleSheet>
                    ("Assets/Plugins/absencee_/absent-dialogues/Editor/Elems/GenericOptionReferenceViewStyle.uss");
            }
        }

        static StyleSheet s_styleSheet;
        public static StyleSheet DefaultStyleSheet => s_styleSheet;

        public static GenericOptionReferenceView Create(GenericOptionReference reference, Func<Port> portCreator)
        {
            GenericOptionReferenceView root = new GenericOptionReferenceView(reference, portCreator);
            return root;
        }

        public GenericOptionReference Reference => m_reference;
        public Port Port => m_leadingPort;

        public event Action<GenericOptionReferenceView> onRefresh;
        public event Action<GenericOptionReferenceView> onBypassButtonClicked;

        GenericOptionReference m_reference;
        Button m_bypassButton;
        TextField m_textField;
        TextElement m_textFieldTextElement;
        VisualElement m_divider;
        Port m_leadingPort;
        Label m_showIfLabel;

        internal GenericOptionReferenceView(GenericOptionReference reference, Func<Port> portCreator)
        {
            this.m_reference = reference;

            this.styleSheets.Add(OptionView.DefaultStyleSheet);
            this.styleSheets.Add(s_styleSheet);

            VisualElement top = new VisualElement();
            top.AddToClassList("optionBottom");
            top.name = "top";

            VisualElement divider = new VisualElement();
            divider.AddToClassList("optionDivider");

            VisualElement bottom = new VisualElement();
            bottom.AddToClassList("optionBottom");

            GenericOption target = reference.Target;

            //"◦•✓"
            Button bypassButton = new Button(() => onBypassButtonClicked?.Invoke(this));
            bypassButton.AddToClassList("bypassOptionButton");
            bypassButton.RegisterCallback<MouseEnterEvent>(evt => ApplyHardcodedStyle(EditorSettings.instance));
            bypassButton.RegisterCallback<MouseOutEvent>(evt => ApplyHardcodedStyle(EditorSettings.instance));

            Port port = portCreator.Invoke();
            port.AddToClassList("optionPort");
            port.portName = "";
            port.name = "option-direct-port";

            TextField speechField = new TextField();
            speechField.AddToClassList("optionField");
            speechField.SetValueWithoutNotify(target.Text);
            speechField.SetEnabled(false);
            speechField.multiline = true;

            Label showIfLabel = new Label("Conditional visibility active.");
            showIfLabel.AddToClassList("optionShowIfLabel");
            showIfLabel.name = "show-if-label";
            showIfLabel.tooltip = "NODATA";

            top.Add(bypassButton);
            top.Add(showIfLabel);

            bottom.Add(speechField);
            bottom.Add(port);

            this.Add(divider);
            this.Add(top);
            this.Add(bottom);

            m_divider = divider;
            m_textField = speechField;
            m_textFieldTextElement = m_textField.Q<TextElement>();
            m_bypassButton = bypassButton;
            m_showIfLabel = showIfLabel;
            m_leadingPort = port;

            Refresh();
        }

        public void ApplyHardcodedStyle(EditorSettings settings)
        {
            if (m_reference.Bypass)
            {
                m_bypassButton.style.backgroundColor = settings.NeutralColor;
                m_bypassButton.style.color = settings.TextColor;
                m_bypassButton.text = "—";
                m_bypassButton.RemoveFromClassList("passiveBypassButton");
                m_bypassButton.AddToClassList("activeBypassButton");
            }

            else
            {
                m_bypassButton.style.backgroundColor = settings.PositiveColor;
                m_bypassButton.style.color = settings.AlternativeTextColor;
                m_bypassButton.text = "✓";
                m_bypassButton.AddToClassList("passiveBypassButton");
                m_bypassButton.RemoveFromClassList("activeBypassButton");
            }
        }

        public void Refresh()
        {
            bool useShowIf = m_reference.Target.UseShowIf;
            bool bypass = m_reference.Bypass;

            m_showIfLabel.visible = useShowIf || bypass;

            if (bypass)
            {
                m_showIfLabel.text = "Bypassed.";
                m_showIfLabel.tooltip = "This option won't be displayed.";
            }

            else if (useShowIf)
            {
                m_showIfLabel.text = "Conditional visibility active.";
                m_showIfLabel.tooltip = m_reference.Target.Visibility.GetConditionString(true);
            }

            m_textField.SetValueWithoutNotify(m_reference.Target.Text);

            ApplyHardcodedStyle(EditorSettings.instance);
            onRefresh?.Invoke(this);
        }

        public void OnRefresh(Action<GenericOptionReferenceView> action)
        {
            onRefresh += action;
        }

        public void OnBypassButtonClicked(Action<GenericOptionReferenceView> action)
        {
            onBypassButtonClicked += action;
        }
    }
}
