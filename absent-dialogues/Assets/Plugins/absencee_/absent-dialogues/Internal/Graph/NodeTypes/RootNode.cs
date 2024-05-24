using System;
using System.Collections.Generic;
using UnityEngine;

namespace com.absence.dialoguesystem.internals
{
    public sealed class RootNode : Node
    {
        [HideInInspector] public Node Next;
        public override bool DisplayState => false;
        public override string GetClassName() => "rootNode";
        public override string GetTitle() => "Root";

        protected override void Pass_Inline(params object[] passData)
        {
            if (Next == null) return;

            Next.Reach();
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

        public override Node Clone()
        {
            RootNode node = Instantiate(this);
            node.Next = Next.Clone();
            return node;
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
    }

}