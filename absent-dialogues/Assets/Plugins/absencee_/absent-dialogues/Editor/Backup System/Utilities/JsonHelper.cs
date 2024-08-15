using com.absence.dialoguesystem.runtime.backup.data;
using UnityEngine;

namespace com.absence.dialoguesystem.editor.backup.utilities
{
    public static class JsonHelper
    {
        public static string GenerateJsonFrom(DialogueData data)

        {
            return JsonUtility.ToJson(data, true);
        }

        public static DialogueData ReadFromJson(string jsonText)
        {
            return JsonUtility.FromJson<DialogueData>(jsonText);
        }
    }
}
