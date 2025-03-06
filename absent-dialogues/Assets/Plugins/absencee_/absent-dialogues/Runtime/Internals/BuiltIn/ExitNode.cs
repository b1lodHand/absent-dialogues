using System.Collections.Generic;

namespace com.absence.dialoguesystem.internals
{
    public class ExitNode : Node
    {
        public static string CreationMenuName => "Exit";

        public override string Title => "Exit";

        public override List<string> AdditionalUSSFileLocations => new List<string>()
        {
            "Assets/Plugins/absencee_/absent-dialogues/Editor/BuiltIn/StyleSheets/ExitNodeView.uss"
        };

        protected override Node OnPass(DialogueFlowContext context)
        {
            context.WillExit = true;
            return null;
        }

        protected override void OnReach(DialogueFlowContext context)
        {
            
        }

        protected override void OnAddOutputConnection(Node nextWillBeAdded, int atPort)
        {
            
        }

        protected override void WriteOutputConnections(ref List<Node> result)
        {
            
        }

        protected override void OnRemoveOutputConnection(int atPort)
        {
            
        }

        public override string GetDefaultInputPortName() => "Out";
        public override List<string> GetDefaultOutputPortNames() => new();
    }
}