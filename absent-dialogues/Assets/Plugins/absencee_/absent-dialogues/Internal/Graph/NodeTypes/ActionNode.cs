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
    /// <remarks>
    /// Execution order goes like:
    /// <code>
    /// VBActions.ForEach(action => action.Perform());
    /// UnityEvents?.Invoke();
    /// CustomAction();
    /// </code>
    /// </remarks>
    [HelpURL("https://b1lodhand.github.io/absent-dialogues/api/com.absence.dialoguesystem.internals.ActionNode.html")]
    public class ActionNode : Node, IPerformDelayedClone, IContainVariableManipulators, IPerformEditorRefresh
    {
        [Tooltip("All of the 'VariableBank' based actions of this action node.")] 
        public List<NodeVariableSetter> VBActions = new();

        [Tooltip("All of the unity based events of this action node.")] public UnityEvent UnityEvents;

        [HideInInspector] public Node Next;

        /// <summary>
        /// Use to define what to do when this action node gets passed on the flow.
        /// </summary>
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

        public List<NodeVariableComparer> GetComparers() => null;

        public List<NodeVariableSetter> GetSetters() => new(VBActions);

        public void PerformEditorRefresh()
        {
            VBActions.ForEach(setter => setter.BlackboardBank = Blackboard.Bank);
        }
    }

}