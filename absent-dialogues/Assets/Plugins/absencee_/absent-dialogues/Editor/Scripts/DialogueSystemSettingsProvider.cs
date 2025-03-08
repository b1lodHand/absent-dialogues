using System.Collections.Generic;
using UnityEditor;

namespace com.absence.dialoguesystem
{
    public class DialogueSystemSettingsProvider : SettingsProvider
    {
        public DialogueSystemSettingsProvider(string path, SettingsScope scopes, IEnumerable<string> keywords = null) : base(path, scopes, keywords)
        {
        }

        [SettingsProvider]
        private static SettingsProvider GetInstance()
        {
            return new DialogueSystemSettingsProvider("absencee_/absent-dialogues", SettingsScope.Project);
        }
    }
}
