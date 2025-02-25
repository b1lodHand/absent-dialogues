using com.absence.dialoguesystem.runtime.backup;
using com.absence.dialoguesystem.runtime.backup.data;
using System.Collections.Generic;
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

        private const string k_none = "None";

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

        public override void OnCloning(Dialogue originalDialogue, Dialogue clonedDialogue)
        {
            if (TargetNode != null) 
                TargetNode = clonedDialogue.AllNodes[originalDialogue.AllNodes.IndexOf(TargetNode)] as SectionNode;
        }
        public override void OnImport(NodeData dataToRead, DialogueImportContext context)
        {
            string data = dataToRead.Data;

            if (data.Equals(k_none))
            {
                TargetNode = null;
                return;
            }

            TargetNode = context.OldGuidPairs[data] as SectionNode;
        }

        public override void OnExport(NodeData dataToWrite)
        {
            if (TargetNode == null)
            {
                dataToWrite.Data = k_none;
                return;
            }

            dataToWrite.Data = TargetNode.Guid;
        }
    }

}