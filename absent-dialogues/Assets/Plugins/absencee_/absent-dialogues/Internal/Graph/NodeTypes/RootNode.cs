using System;
using System.Collections.Generic;
using UnityEngine;

namespace com.absence.dialoguesystem.internals
{
    /// <summary>
    /// Node which is essential if you want to have a dialogue graph.
    /// </summary>
    [HelpURL("https://b1lodhand.github.io/absent-dialogues/api/com.absence.dialoguesystem.internals.RootNode.html")]
    public sealed class RootNode : Node, IPerformDelayedClone
    {
        [HideInInspector] public Node Next;
        public override bool DisplayState => false;
        public override string GetClassName() => "rootNode";
        public override string GetTitle() => "Root";

        protected override void Pass_Inline(DialogueFlowContext context)
        {
            if (Next == null) return;

            Next.Reach(context);
        }
        protected override void Reach_Inline(DialogueFlowContext context)
        {

        }

        protected override void AddNextNode_Inline(Node nextWillBeAdded, int atPort)
        {
            Next = nextWillBeAdded;
        }
        protected override void RemoveNextNode_Inline(int atPort)
        {
            Next = null;
        }
        protected override void GetNextNodes_Inline(ref List<(int portIndex, Node node)> result)
        {
            if (Next != null) result.Add((0, Next));
        }
        public override void Traverse(Action<Node> action)
        {
            action?.Invoke(this);
            Next.Traverse(action);
        }

        public override string GetInputPortNameForCreation()
        {
            return null;
        }
        public override List<string> GetOutputPortNamesForCreation()
        {
            return new List<string>() { "Start" };
        }

        public void DelayedClone(Dialogue originalDialogue)
        {
            if (Next != null) Next = MasterDialogue.AllNodes[originalDialogue.AllNodes.IndexOf(Next)];
        }
    }

}