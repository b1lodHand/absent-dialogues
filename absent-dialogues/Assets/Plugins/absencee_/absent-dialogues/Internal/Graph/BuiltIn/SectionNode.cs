using com.absence.dialoguesystem.internals.backup;
using com.absence.dialoguesystem.internals.backup.data;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

namespace com.absence.dialoguesystem.internals
{
    /// <summary>
    /// Node which let's you create more and seperate routes.
    /// </summary>
    [HelpURL("https://b1lodhand.github.io/absent-dialogues/api/com.absence.dialoguesystem.internals.DialoguePartNode.html")]
    [MovedFrom("DialoguePartNode")]
    public sealed class SectionNode : Node
    {
        public static string CreationMenuName => "Section";

        [HideInInspector] public Node Next;
        public string SectionName;

        public override string Title => "Section";

        public override List<string> AdditionalUSSFileLocations => new List<string>()
        {
            "Assets/Plugins/absencee_/absent-dialogues/Editor/BuiltIn/StyleSheets/SectionNodeView.uss"
        };

        protected override Node OnPass(DialogueFlowContext context)
        {
            return Next;
        }
        protected override void OnReach(DialogueFlowContext context)
        {

        }

        protected override void OnAddOutputConnection(Node nextWillBeAdded, int atPort)
        {
            Next = nextWillBeAdded;
        }
        protected override void OnRemoveOutputConnection(int atPort)
        {
            Next = null;
        }
        protected override void WriteOutputConnections(ref List<Node> result)
        {
            result.Add(Next);
        }

        public override string GetDefaultInputPortName()
        {
            return null;
        }

        public override void OnImport(NodeData dataToRead, DialogueImportContext context)
        {
            base.OnImport(dataToRead, context);

            SectionName = dataToRead.Text;
        }

        public override void OnExport(NodeData dataToWrite)
        {
            base.OnExport(dataToWrite);

            dataToWrite.Text = SectionName;
        }
    }

}