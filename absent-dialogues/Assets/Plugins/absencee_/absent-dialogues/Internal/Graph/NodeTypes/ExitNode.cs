using System.Collections.Generic;

namespace com.absence.dialoguesystem.internals
{
    public class ExitNode : Node
    {
        public static string ParentCreationMenu => "Initial";

        public override string GetClassName() => "exitNode";

        public override string GetTitle() => "Exit";

        protected override void OnPass(DialogueFlowContext context)
        {
        }

        protected override void OnReach(DialogueFlowContext context)
        {
            context.WillExit = true;
        }

        protected override void AddNextNode_Internal(Node nextWillBeAdded, int atPort)
        {
            
        }

        protected override void GetNextNodes_Internal(ref List<(int portIndex, Node node)> result)
        {
            
        }

        protected override void RemoveNextNode_Internal(int atPort)
        {
            
        }

        public override string GetInputPortNameForCreation() => "Out";
        public override List<string> GetOutputPortNamesForCreation() => new();
    }
}