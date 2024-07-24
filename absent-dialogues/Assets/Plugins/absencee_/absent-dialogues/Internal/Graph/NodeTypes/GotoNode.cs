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
        /// <summary>
        /// The node which will get reached when this goto node gets passed.
        /// </summary>
        [HideInInspector] public DialoguePartNode TargetNode;

        public override string GetClassName() => "gotoNode";
        public override string GetTitle() => "Goto";

        protected override void Pass_Inline(params object[] passData)
        {
            if (TargetNode == null) throw new System.Exception("Target node of GotoNode is null!");

            SetState(NodeState.Past);
            TargetNode.Reach();
        }
        protected override void Reach_Inline()
        {

        }

        protected override void AddNextNode_Inline(Node nextWillBeAdded, int atPort)
        {
            // no impl.
        }
        protected override void RemoveNextNode_Inline(int atPort)
        {
            // no impl.
        }
        protected override void GetNextNodes_Inline(ref List<(int portIndex, Node node)> result)
        {

        }

        public override List<string> GetOutputPortNamesForCreation()
        {
            return new List<string>();
        }

        public void DelayedClone(Dialogue originalDialogue)
        {
            if (TargetNode != null) TargetNode = MasterDialogue.AllNodes[originalDialogue.AllNodes.IndexOf(TargetNode)] as DialoguePartNode;
        }
    }

}