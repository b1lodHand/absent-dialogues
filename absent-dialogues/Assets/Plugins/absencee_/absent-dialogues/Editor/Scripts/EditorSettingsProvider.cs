using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace com.absence.dialoguesystem.editor
{
    public class EditorSettingsProvider : SettingsProvider
    {
        static List<string> s_keywords = new()
        {
            "Dialogue",
            "DialogueSystem",
            "Graph",
            "System",
            "Prompt",
        };

        public EditorSettingsProvider(string path, SettingsScope scopes, IEnumerable<string> keywords = null) : base(path, scopes, keywords)
        {
        }

        [SettingsProvider]
        private static SettingsProvider GetInstance()
        {
            return new EditorSettingsProvider("absencee_/absent-dialogues", SettingsScope.Project, s_keywords);
        }

        public override void OnGUI(string searchContext)
        {
            EditorSettings settings = EditorSettings.instance;

            bool experimentalCut = settings.ExperimentalCut;

            KeyCode infoKey = settings.InformationKey;
            bool infoKeyHold = settings.InformationKeyHold;
            int infoKeyHoldIndex = infoKeyHold ? 0 : 1;

            Color themeColor = settings.ThemeColor;
            Color positiveColor = settings.PositiveColor;
            Color negativeColor = settings.NegativeColor;
            Color neutralColor = settings.NeutralColor;

            Color textColor = settings.TextColor;
            Color alternativeTextColor = settings.AlternativeTextColor;

            GUIContent themeColorContent = new("Theme Color");
            GUIContent positiveColorContent = new("Positive Color");
            GUIContent negativeColorContent = new("Negative Color");
            GUIContent neutralColorContent = new("Neutral Color");

            GUIContent textColorContent = new("Text Color");
            GUIContent alternativeTextColorContent = new("Alternative Text Color");

            GUIStyle headerStyle = new(EditorStyles.boldLabel);

            GUILayoutOption[] keyHoldPopupOptions =
            {
                GUILayout.Width(150)
            };

            GUIContent experimentalCutContent = new()
            {
                text = "Cut Nodes (Ctrl+X)",
            };

            GUIContent infoKeyContent = new()
            {
                text = "Information Key",
            };

            GUIContent[] holdPopupOptions =
            {
                new("Hold"),
                new("Toggle"),
            };

            EditorGUILayout.Space();

            EditorGUI.BeginChangeCheck();

            EditorGUILayout.BeginVertical(EditorStyles.inspectorDefaultMargins);

            EditorGUILayout.LabelField("Keys", headerStyle);

            EditorGUILayout.BeginHorizontal();

            infoKey = (KeyCode)EditorGUILayout.EnumPopup(infoKeyContent, infoKey);
            infoKeyHoldIndex = EditorGUILayout.Popup(infoKeyHoldIndex, holdPopupOptions, keyHoldPopupOptions);

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Colors", headerStyle);

            themeColor = EditorGUILayout.ColorField(themeColorContent, themeColor);
            positiveColor = EditorGUILayout.ColorField(positiveColorContent, positiveColor);
            negativeColor = EditorGUILayout.ColorField(negativeColorContent, negativeColor);
            neutralColor = EditorGUILayout.ColorField(neutralColorContent, neutralColor);

            textColor = EditorGUILayout.ColorField(textColorContent, textColor);
            alternativeTextColor = EditorGUILayout.ColorField(alternativeTextColorContent, alternativeTextColor);

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Experimental", headerStyle);

            experimentalCut = EditorGUILayout.Toggle(experimentalCutContent, experimentalCut);

            EditorGUILayout.EndVertical();

            if (EditorGUI.EndChangeCheck()) 
            {
                Undo.RecordObject(settings, "Dialogue System (Project Settings)");

                settings.ExperimentalCut = experimentalCut;

                settings.InformationKey = infoKey;
                settings.InformationKeyHold = infoKeyHoldIndex == 1 ? false : true;

                settings.ThemeColor = themeColor;
                settings.PositiveColor = positiveColor;
                settings.NegativeColor = negativeColor;
                settings.NeutralColor = neutralColor;
                settings.TextColor = textColor;
                settings.AlternativeTextColor = alternativeTextColor;

                if (DialogueEditorWindow.Current != null)
                    DialogueEditorWindow.Current.m_dialogueGraphView.ReapplyHardcodedStyles(settings);
            }
        }

        public override void OnTitleBarGUI()
        {
        }

        public override void OnDeactivate()
        {
            EditorSettings settings = EditorSettings.instance;
            settings.Save();
        }

        public override void OnFooterBarGUI()
        {
            EditorSettings settings = EditorSettings.instance;

            if (GUILayout.Button("Reset"))
            {
                Undo.RecordObject(settings, "Dialogue System (Project Settings)");
                settings.Reset();

                if (DialogueEditorWindow.Current != null)
                    DialogueEditorWindow.Current.m_dialogueGraphView.ReapplyHardcodedStyles(settings);
            }

            if (GUILayout.Button("Save"))
            {
                settings.Save();
            }
        }
    }
}
