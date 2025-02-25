using com.absence.dialoguesystem.runtime.backup.data;
using com.absence.dialoguesystem.runtime.backup;
using System.Collections.Generic;
using UnityEngine;

namespace com.absence.dialoguesystem.internals
{
    /// <summary>
    /// Node which is simply <see cref="StickyNoteNode"/> but bigger.
    /// </summary>
    [HelpURL("https://b1lodhand.github.io/absent-dialogues/api/com.absence.dialoguesystem.internals.TitleNode.html")]
    public sealed class TitleNode : Node
    {
        public static string ParentCreationMenu => "Misc";

        [HideInInspector] public string m_text;

        public override bool DisplayState => false;
        public override bool ShowInMinimap => false;

        public override string GetClassName() => "titleNode";

        public override string Title => "";

        protected override void AddNextNode_Internal(Node nextWillBeAdded, int atPort)
        {
            
        }

        protected override void GetNextNodes_Internal(ref List<(int portIndex, Node node)> result)
        {
            
        }

        protected override void OnPass(DialogueFlowContext context)
        {
            
        }

        protected override void OnReach(DialogueFlowContext context)
        {
            
        }

        protected override void RemoveNextNode_Internal(int atPort)
        {
            
        }

        public override string GetInputPortNameForCreation() => null;
        public override List<string> GetOutputPortNamesForCreation() => new();

        public override void OnExport(NodeData dataToWrite)
        {
            dataToWrite.Data = m_text;
        }

        public override void OnImport(NodeData dataToRead, DialogueImportContext context)
        {
            m_text = dataToRead.Data;
        }
    }

}