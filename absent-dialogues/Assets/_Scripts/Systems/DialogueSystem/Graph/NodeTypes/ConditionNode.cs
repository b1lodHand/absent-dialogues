using com.absence.variablesystem;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace com.absence.dialoguesystem.internals
{
    public class ConditionNode : Node
    {
        public enum ProcessType
        {
            All = 0,
            Any = 1,
        }

        [HideInInspector] public Node TrueNext;
        [HideInInspector] public Node FalseNext;
        public ProcessType Processor = ProcessType.All;
        public List<VariableComparer> Comparers = new List<VariableComparer>();

        public override string GetClassName() => "conditionNode";
        public override string GetTitle() => "Condition";

        protected override void Pass_Inline(params object[] passData)
        {
            var targetNext = Process() ? TrueNext : FalseNext;
            if (targetNext != null) targetNext.Reach();
            SetState(NodeState.Past);
        }
        protected override void Reach_Inline()
        {

        }

        protected override void AddNextNode_Inline(Node nextWillBeAdded, int atPort)
        {
            if (atPort == 0) TrueNext = nextWillBeAdded;
            else if (atPort == 1) FalseNext = nextWillBeAdded;
        }
        protected override void RemoveNextNode_Inline(int atPort)
        {
            if (atPort == 0) TrueNext = null;
            else if (atPort == 1) FalseNext = null;
        }
        protected override void GetNextNodes_Inline(ref List<(int portIndex, Node node)> result)
        {
            if (TrueNext != null) result.Add((0, TrueNext));
            if (FalseNext != null) result.Add((1, FalseNext));
        }

        public override Node Clone()
        {
            ConditionNode node = Instantiate(this);
            node.TrueNext = TrueNext.Clone();
            node.FalseNext = FalseNext.Clone();
            return node;
        }
        public override List<string> GetOutputPortNamesForCreation()
        {
            return new List<string>() { "True", "False" };
        }

        private bool Process()
        {
            if (Comparers.Count == 0) return true;

            bool result = true;
            switch (Processor)
            {
                case ProcessType.All:
                    result = Comparers.All(c => c.GetResult());
                    break;
                case ProcessType.Any:
                    result = Comparers.Any(c => c.GetResult());
                    break;
            }

            return result;
        }
    }
}