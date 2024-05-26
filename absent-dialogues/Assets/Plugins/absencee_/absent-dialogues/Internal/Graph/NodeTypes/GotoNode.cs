using System.Collections.Generic;
using System.Linq;

namespace com.absence.dialoguesystem.internals
{
    /// <summary>
    /// Node which teleports the flow to a specific <see cref="DialoguePartNode"/>.
    /// </summary>
    public sealed class GotoNode : Node
    {
        public string TargetDialogPartName;
        public override string GetClassName() => "gotoNode";
        public override string GetTitle() => "Goto";

        protected override void Pass_Inline(params object[] passData)
        {
            var check = MasterDialogue.GetDialogPartNodesWithName(TargetDialogPartName);
            //if (check.Count == 0 || check.Count > 1) return;

            SetState(NodeState.Past);
            check.FirstOrDefault().Reach();
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
    }

}