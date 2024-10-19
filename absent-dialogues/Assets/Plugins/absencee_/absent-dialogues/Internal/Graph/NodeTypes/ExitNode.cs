using System.Collections.Generic;

namespace com.absence.dialoguesystem.internals
{
    public class ExitNode : Node
    {
        public static string ParentCreationMenu => "Initial";

        public override string GetClassName() => "exitNode";

        public override string GetTitle() => "Exit";

        protected override void Pass_Inline(DialogueFlowContext context)
        {
        }

        protected override void Reach_Inline(DialogueFlowContext context)
        {
            context.WillExit = true;
        }

        protected override void AddNextNode_Inline(Node nextWillBeAdded, int atPort)
        {
            
        }

        protected override void GetNextNodes_Inline(ref List<(int portIndex, Node node)> result)
        {
            
        }

        protected override void RemoveNextNode_Inline(int atPort)
        {
            
        }

        public override string GetInputPortNameForCreation() => "Out";
        public override List<string> GetOutputPortNamesForCreation() => new();
    }
}