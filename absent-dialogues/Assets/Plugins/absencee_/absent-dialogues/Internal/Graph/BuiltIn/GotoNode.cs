using com.absence.dialoguesystem.runtime.backup;
using com.absence.dialoguesystem.runtime.backup.data;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace com.absence.dialoguesystem.internals
{
    /// <summary>
    /// Node which teleports the flow to a specific <see cref="SectionNode"/>.
    /// </summary>
    [HelpURL("https://b1lodhand.github.io/absent-dialogues/api/com.absence.dialoguesystem.internals.GotoNode.html")]
    public sealed class GotoNode : Node
    {
        public static string CreationMenuName => "Goto";

        /// <summary>
        /// The node which will get reached when this goto node gets passed.
        /// </summary>
        [HideInInspector] public SectionNode TargetNode;

        public override string Title => "Goto";

        public override List<string> AdditionalUSSFileLocations => new List<string>()
        {
            "Assets/Plugins/absencee_/absent-dialogues/Editor/BuiltIn/StyleSheets/GotoNodeView.uss"
        };

        protected override Node OnPass(DialogueFlowContext context)
        {
            if (TargetNode == null) 
                throw new System.Exception("Target node of GotoNode is null!");

            return TargetNode;
        }
        protected override void OnReach(DialogueFlowContext context)
        {

        }

        protected override void OnAddOutputConnection(Node nextWillBeAdded, int atPort)
        {
            // no impl.
        }
        protected override void OnRemoveOutputConnection(int atPort)
        {
            // no impl.
        }
        protected override void WriteOutputConnections(ref List<Node> result)
        {

        }

        public override List<string> GetDefaultOutputPortNames()
        {
            return new List<string>();
        }

        public override void OnImport(NodeData dataToRead, DialogueImportContext context)
        {
            base.OnImport(dataToRead, context); 

            string data = dataToRead.Text;

            if (data.Equals(NaN))
            {
                TargetNode = null;
                return;
            }

            TargetNode = context.OldGuidPairs[data] as SectionNode;
        }

        public override void OnExport(NodeData dataToWrite)
        {
            base.OnExport(dataToWrite); 

            if (TargetNode == null)
            {
                dataToWrite.Text = NaN;
                return;
            }

            dataToWrite.Text = TargetNode.Guid;
        }

        public override void OnCloning(Dialogue originalDialogue, Dialogue cloneDialogue)
        {
            base.OnCloning(originalDialogue, cloneDialogue);

            if (TargetNode != null) 
                TargetNode = cloneDialogue.AllNodes.First(nd => nd.Guid.Equals(TargetNode.Guid)) as SectionNode;
        }
    }

}