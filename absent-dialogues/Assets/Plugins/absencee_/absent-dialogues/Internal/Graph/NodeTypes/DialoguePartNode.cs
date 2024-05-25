using System;
using System.Collections.Generic;
using UnityEngine;

namespace com.absence.dialoguesystem.internals
{
    public sealed class DialoguePartNode : Node, IPerformDelayedClone
    {
        [HideInInspector] public Node Next;
        public string DialoguePartName;
        public override bool DisplayState => false;
        public override string GetClassName() => "dialogPartNode";
        public override string GetTitle() => $"Dialogue Part";

        protected override void Pass_Inline(params object[] passData)
        {
            if (Next != null) Next.Reach();
        }
        protected override void Reach_Inline()
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

        public void DelayedClone(Dialogue originalDialogue)
        {
            Next = MasterDialogue.AllNodes[originalDialogue.AllNodes.IndexOf(Next)];
        }
    }

}