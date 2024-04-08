using com.absence.variablesystem;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace com.absence.dialoguesystem
{
    public class ActionNode : Node
    {
        public List<VariableSetter> GVActions = new List<VariableSetter>();
        public UnityEvent UnityActions;
        public event Action CustomAction;

        [HideInInspector] public Node Next;

        public override string GetClassName() => "actionNode";
        public override string GetTitle() => "Action";

        protected override void Pass_Inline(params object[] passData)
        {
            if (Next == null) return;

            GVActions.ForEach(action => action.Perform());
            UnityActions?.Invoke();
            CustomAction?.Invoke();

            Next.Reach();
            SetState(NodeState.Past);
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
    }

}