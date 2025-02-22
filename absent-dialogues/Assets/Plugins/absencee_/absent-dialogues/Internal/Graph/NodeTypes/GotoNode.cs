using com.absence.dialoguesystem.runtime.backup;
using com.absence.dialoguesystem.runtime.backup.data;
using System.Collections.Generic;
using UnityEngine;

namespace com.absence.dialoguesystem.internals
{
    /// <summary>
    /// Node which teleports the flow to a specific <see cref="DialoguePartNode"/>.
    /// </summary>
    [HelpURL("https://b1lodhand.github.io/absent-dialogues/api/com.absence.dialoguesystem.internals.GotoNode.html")]
    public sealed class GotoNode : Node, IPerformDelayedClone
    {
        public static string ParentCreationMenu => "Grouping";

        /// <summary>
        /// The node which will get reached when this goto node gets passed.
        /// </summary>
        [HideInInspector] public DialoguePartNode TargetNode;

        public override string GetClassName() => "gotoNode";
        public override string GetTitle() => "Goto";

        protected override void OnPass(DialogueFlowContext context)
        {
            if (TargetNode == null) throw new System.Exception("Target node of GotoNode is null!");

            TargetNode.Reach(context);
        }
        protected override void OnReach(DialogueFlowContext context)
        {

        }

        protected override void AddNextNode_Internal(Node nextWillBeAdded, int atPort)
        {
            // no impl.
        }
        protected override void RemoveNextNode_Internal(int atPort)
        {
            // no impl.
        }
        protected override void GetNextNodes_Internal(ref List<(int portIndex, Node node)> result)
        {

        }

        public override List<string> GetOutputPortNamesForCreation()
        {
            return new List<string>();
        }

        public void DelayedClone(Dialogue originalDialogue, Dialogue clonedDialogue)
        {
            if (TargetNode != null) TargetNode = clonedDialogue.AllNodes[originalDialogue.AllNodes.IndexOf(TargetNode)] as DialoguePartNode;
        }

        public override void OnImport(NodeData dataToRead, DialogueImportContext context)
        {
            TargetNode = context.OldGuidPairs[dataToRead.GotoTargetGuid] as DialoguePartNode;
        }

        public override void OnExport(NodeData dataToWrite)
        {
            dataToWrite.GotoTargetGuid = TargetNode.Guid;
        }
    }

}