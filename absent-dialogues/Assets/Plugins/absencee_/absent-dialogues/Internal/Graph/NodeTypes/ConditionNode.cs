using com.absence.variablesystem;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace com.absence.dialoguesystem.internals
{
    /// <summary>
    /// Node which re-routes the flow under some conditions.
    /// </summary>
    public class ConditionNode : Node, IPerformDelayedClone, IContainVariableManipulators, IPerformEditorRefresh
    {
        [HideInInspector] public Node TrueNext;
        [HideInInspector] public Node FalseNext;
        public VBProcessType Processor = VBProcessType.All;
        public List<FixedVariableComparer> Comparers = new List<FixedVariableComparer>();

        public override string GetClassName() => "conditionNode";
        public override string GetTitle() => "Condition";

        protected override void Pass_Inline(params object[] passData)
        {
            var targetNext = Process() ? TrueNext : FalseNext;
            if (targetNext != null) targetNext.Reach();
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

        public void DelayedClone(Dialogue originalDialogue)
        {
            TrueNext = MasterDialogue.AllNodes[originalDialogue.AllNodes.IndexOf(TrueNext)];
            FalseNext = MasterDialogue.AllNodes[originalDialogue.AllNodes.IndexOf(FalseNext)];

            Comparers = Comparers.ConvertAll(comparer =>
            {
                return comparer.Clone(Blackboard.Bank);
            });
        }

        public override void Traverse(Action<Node> action)
        {
            action?.Invoke(this);
            TrueNext.Traverse(action);
            FalseNext.Traverse(action);
        }

        public override List<string> GetOutputPortNamesForCreation()
        {
            return new List<string>() { "True", "False" };
        }

        protected virtual bool Process()
        {
            if (Comparers.Count == 0) return true;

            bool result = true;
            switch (Processor)
            {
                case VBProcessType.All:
                    result = Comparers.All(c => c.GetResult());
                    break;
                case VBProcessType.Any:
                    result = Comparers.Any(c => c.GetResult());
                    break;
            }

            return result;
        }

        public List<FixedVariableComparer> GetComparers() => new(Comparers);
        public List<FixedVariableSetter> GetSetters() => null;

        public void PerformEditorRefresh()
        {
            Comparers.ForEach(comparer => comparer.SetFixedBank(Blackboard.Bank));
        }
    }

    public enum VBProcessType
    {
        All = 0,
        Any = 1,
    }
}