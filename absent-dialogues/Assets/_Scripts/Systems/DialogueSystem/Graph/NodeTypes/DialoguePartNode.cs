using System.Collections.Generic;
using UnityEngine;

namespace com.absence.dialoguesystem
{
    public class DialoguePartNode : Node
    {
        [HideInInspector] public Node Next;
        public string DialogPartName;
        public override bool DisplayState => false;
        public override string GetClassName() => "dialogPartNode";
        public override string GetTitle() => $"Dialog Part";

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

        public override Node Clone()
        {
            DialoguePartNode node = Instantiate(this);
            node.Next = Next.Clone();
            return node;
        }
        public override string GetInputPortNameForCreation()
        {
            return null;
        }
    }

}