using com.absence.dialoguesystem.internals.backup.data;
using UnityEngine;

namespace com.absence.dialoguesystem.editor.internals.backup
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
