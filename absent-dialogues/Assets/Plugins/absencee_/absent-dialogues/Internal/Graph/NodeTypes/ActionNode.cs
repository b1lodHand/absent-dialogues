using com.absence.variablesystem;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace com.absence.dialoguesystem.internals
{
    /// <summary>
    /// Node which invokes some actions on the flow.
    /// </summary>
    public class ActionNode : Node, IPerformDelayedClone, IContainVariableManipulators, IPerformEditorRefresh
    {
        public List<FixedVariableSetter> VBActions = new List<FixedVariableSetter>();
        public UnityEvent UnityEvents;

        [HideInInspector] public Node Next;
        protected virtual void CustomAction()
        {

        }

        public override string GetClassName() => "actionNode";
        public override string GetTitle() => "Action";

        protected override void Pass_Inline(params object[] passData)
        {
            if (Next == null) return;

            VBActions.ForEach(action => action.Perform());
            UnityEvents?.Invoke();
            CustomAction();

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

        public void DelayedClone(Dialogue originalDialogue)
        {
            Next = MasterDialogue.AllNodes[originalDialogue.AllNodes.IndexOf(Next)];

            VBActions = VBActions.ConvertAll(action =>
            {
                return action.Clone(Blackboard.Bank);
            });
        }

        public override void Traverse(Action<Node> action)
        {
            action?.Invoke(this);
            Next.Traverse(action);
        }

        public List<FixedVariableComparer> GetComparers() => null;

        public List<FixedVariableSetter> GetSetters() => new(VBActions);

        public void PerformEditorRefresh()
        {
            VBActions.ForEach(setter => setter.SetFixedBank(Blackboard.Bank));
        }
    }

}