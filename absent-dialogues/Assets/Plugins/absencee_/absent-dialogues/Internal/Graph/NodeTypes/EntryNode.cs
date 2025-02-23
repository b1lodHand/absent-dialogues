using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

namespace com.absence.dialoguesystem.internals
{
    /// <summary>
    /// Node which is essential if you want to have a dialogue graph.
    /// </summary>
    [HelpURL("https://b1lodhand.github.io/absent-dialogues/api/com.absence.dialoguesystem.internals.RootNode.html")]
    [MovedFrom("RootNode")]
    public sealed class EntryNode : Node, IPerformDelayedClone
    {
        public static string ParentCreationMenu => "Initial";

        [HideInInspector] public Node Next;
        public override string GetClassName() => "rootNode";
        public override string GetTitle() => "Entry";

        protected override void OnPass(DialogueFlowContext context)
        {
            if (Next == null) return;

            Next.Reach(context);
        }
        protected override void OnReach(DialogueFlowContext context)
        {

        }

        protected override void AddNextNode_Internal(Node nextWillBeAdded, int atPort)
        {
            Next = nextWillBeAdded;
        }
        protected override void RemoveNextNode_Internal(int atPort)
        {
            Next = null;
        }
        protected override void GetNextNodes_Internal(ref List<(int portIndex, Node node)> result)
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

        public void DelayedClone(Dialogue originalDialogue, Dialogue clonedDialogue)
        {
            if (Next != null) Next = clonedDialogue.AllNodes[originalDialogue.AllNodes.IndexOf(Next)];
        }
    }

}