using com.absence.dialoguesystem.editor.backup.data;
using com.absence.dialoguesystem.internals;
using System.Collections.Generic;

namespace com.absence.dialoguesystem.editor.backup.internals    
{
    [System.Serializable]
    public class DialogueImportContext 
    {
        public Dictionary<string, Node> OldGuidPairs;
        public Dialogue Dialogue;
        public DialogueData DialogueData;
    }
}
