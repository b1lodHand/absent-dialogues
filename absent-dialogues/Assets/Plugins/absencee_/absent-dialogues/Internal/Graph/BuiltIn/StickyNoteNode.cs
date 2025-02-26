using com.absence.dialoguesystem.runtime.backup;
using com.absence.dialoguesystem.runtime.backup.data;
using System.Collections.Generic;
using UnityEngine;

namespace com.absence.dialoguesystem.internals
{
    /// <summary>
    /// Node which contains a user defined string.
    /// </summary>
    [HelpURL("https://b1lodhand.github.io/absent-dialogues/api/com.absence.dialoguesystem.internals.StickyNoteNode.html")]
    public sealed class StickyNoteNode : Node
    {
        public static string CreationMenuName => "Sticky Note";

        [HideInInspector] public string m_text;

        public override bool DisplayState => false;
        public override bool ShowInMinimap => false;

        public override string Title => "Sticky Note";
        public override string Text { get => m_text; set => m_text = value; }

        public override List<string> AdditionalUSSFileLocations => new List<string>()
        {
            "Assets/Plugins/absencee_/absent-dialogues/Editor/BuiltIn/StyleSheets/StickyNoteNodeView.uss"
        };

        protected override void OnAddOutputConnection(Node nextWillBeAdded, int atPort)
        {
            
        }

        protected override void WriteOutputConnections(ref List<Node> result)
        {
            
        }

        protected override Node OnPass(DialogueFlowContext context)
        {
            return null;
        }

        protected override void OnReach(DialogueFlowContext context)
        {
            
        }

        protected override void OnRemoveOutputConnection(int atPort)
        {
            
        }

        public override string GetDefaultInputPortName() => null;
        public override List<string> GetDefaultOutputPortNames() => new();

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