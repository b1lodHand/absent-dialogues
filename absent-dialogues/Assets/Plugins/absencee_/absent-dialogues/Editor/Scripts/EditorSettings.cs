using UnityEditor;
using UnityEngine;

namespace com.absence.dialoguesystem
{
    [FilePath("ProjectSettings/absent-dialogues-settings.asset", FilePathAttribute.Location.ProjectFolder)]
    public class EditorSettings : ScriptableSingleton<EditorSettings>
    {
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

        public void Reset()
        {
            ThemeColor = s_defaultThemeColor;
            PositiveColor = s_defaultPositiveColor;
            NegativeColor = s_defaultNegativeColor;
            NeutralColor = s_defaultNeutralColor;

            TextColor = s_defaultTextColor;
            AlternativeTextColor = s_defaultAlternativeTextColor;

            Save();
        }

        public void Save()
        {
            Save(true);
        }
    }
}
