using com.absence.dialoguesystem.internals.backup.data;
using System.Collections.Generic;

namespace com.absence.dialoguesystem.internals.backup
{
    [System.Serializable]
    public class DialogueImportContext 
    {
        public Dictionary<string, Node> OldGuidPairs;
        public Dialogue Dialogue;
        public DialogueData DialogueData;
    }
}
