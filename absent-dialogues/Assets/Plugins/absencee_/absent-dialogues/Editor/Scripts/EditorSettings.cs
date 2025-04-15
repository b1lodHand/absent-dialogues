using UnityEditor;
using UnityEngine;

namespace com.absence.dialoguesystem
{
    [FilePath("ProjectSettings/absent-dialogues-settings.asset", FilePathAttribute.Location.ProjectFolder)]
    public class EditorSettings : ScriptableSingleton<EditorSettings>
    {
        // window color
        // window label color
        // edge color -> idle
        // edge color -> selected
        // port color
        // label color
        // inactive label color
        // button color
        // node background color
        // node border color
        // text field background color
        // divider color
        // alternative divider color
        // grid background color
        // grid line color
        // grid thick line color
        // option remove button color
        // option remove button text color
        // generic option bypass button color -> passive
        // generic option bypass button color -> active
        // generic option bypass button text color -> passive
        // generic option bypass button text color -> active

        const KeyCode INFORMATION_KEY_DEFAULT = KeyCode.LeftAlt;
        const bool INFORMATION_KEY_HOLD_DEFAULT = true;
        const bool EXPERIMENTALS_DEFAULT_VALUE = false;

        static readonly Color s_defaultThemeColor = new(63f / 255f, 166f / 255f, 171f / 255f);
        static readonly Color s_defaultPositiveColor = new(112f / 255f, 171f / 255f, 63f / 255f);
        static readonly Color s_defaultNegativeColor = new(171f/255f, 68f/255f, 63f/255f);
        static readonly Color s_defaultNeutralColor = new(88f / 255f, 88f / 255f, 88f / 255f);

        static readonly Color s_defaultTextColor = new(1f, 1f, 1f);
        static readonly Color s_defaultAlternativeTextColor = new(1f, 1f, 1f);

        public Color ThemeColor = s_defaultThemeColor;
        public Color PositiveColor = s_defaultPositiveColor;
        public Color NegativeColor = s_defaultNegativeColor;
        public Color NeutralColor = s_defaultNeutralColor;

        public Color TextColor = s_defaultTextColor;
        public Color AlternativeTextColor = s_defaultAlternativeTextColor;

        public KeyCode InformationKey = INFORMATION_KEY_DEFAULT;
        public bool InformationKeyHold = INFORMATION_KEY_HOLD_DEFAULT;

        public bool ExperimentalCut = EXPERIMENTALS_DEFAULT_VALUE;

        public void Reset()
        {
            InformationKey = INFORMATION_KEY_DEFAULT;
            InformationKeyHold = INFORMATION_KEY_HOLD_DEFAULT;

            ThemeColor = s_defaultThemeColor;
            PositiveColor = s_defaultPositiveColor;
            NegativeColor = s_defaultNegativeColor;
            NeutralColor = s_defaultNeutralColor;

            TextColor = s_defaultTextColor;
            AlternativeTextColor = s_defaultAlternativeTextColor;

            ExperimentalCut = EXPERIMENTALS_DEFAULT_VALUE;

            Save();
        }

        public void Save()
        {
            Save(true);
        }
    }
}
