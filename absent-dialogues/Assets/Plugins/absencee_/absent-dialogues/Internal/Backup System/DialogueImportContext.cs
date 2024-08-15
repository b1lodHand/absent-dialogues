using com.absence.dialoguesystem.internals;
using com.absence.dialoguesystem.runtime.backup.data;
using System.Collections.Generic;

namespace com.absence.dialoguesystem.runtime.backup
{
    [System.Serializable]
    public class DialogueImportContext 
    {
        public Dictionary<string, Node> OldGuidPairs;
        public Dialogue Dialogue;
        public DialogueData DialogueData;
    }
}
