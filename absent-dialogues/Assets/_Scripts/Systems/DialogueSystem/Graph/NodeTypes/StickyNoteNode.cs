using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.absence.dialoguesystem
{
    public class StickyNoteNode : Node
    {
        [HideInInspector] public string Speech;

        public override bool DisplayState => false;

        public override string GetClassName() => "stickyNoteNode";

        public override string GetTitle() => "Sticky Note";

        protected override void AddNextNode_Inline(Node nextWillBeAdded, int atPort)
        {
            
        }

        protected override void GetNextNodes_Inline(ref List<(int portIndex, Node node)> result)
        {
            
        }

        protected override void Pass_Inline(params object[] passData)
        {
            
        }

        protected override void Reach_Inline()
        {
            
        }

        protected override void RemoveNextNode_Inline(int atPort)
        {
            
        }

        public override string GetInputPortNameForCreation() => null;
        public override List<string> GetOutputPortNamesForCreation() => new List<string>();
    }

}