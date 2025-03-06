using com.absence.dialoguesystem.internals.backup;
using com.absence.dialoguesystem.internals.backup.data;
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
        public static string CreationMenuName => "Misc/Title";

        [HideInInspector] public string m_text = "TITLE";

        [SerializeField] internal uint m_fontSize = 30;

        public override bool DisplayState => false;
        public override bool ShowInMinimap => false;

        public override string Title => "";
        public override string Text { get => m_text; set => m_text = value; }

        public override List<string> AdditionalUSSFileLocations => new List<string>()
        {
            "Assets/Plugins/absencee_/absent-dialogues/Editor/BuiltIn/StyleSheets/TitleNodeView.uss"
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

        public override void OnExport(NodeData dataToWrite)
        {
            base.OnExport(dataToWrite);

            dataToWrite.IntData = new int[1];
            dataToWrite.IntData[0] = (int)m_fontSize;
        }

        public override void OnImport(NodeData dataToRead, DialogueImportContext context)
        {
            base.OnImport(dataToRead, context);

            m_fontSize = (uint)dataToRead.IntData[0];
        }

        public override string GetDefaultInputPortName() => null;
        public override List<string> GetDefaultOutputPortNames() => new();
    }

}