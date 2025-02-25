using com.absence.attributes;
using com.absence.dialoguesystem.runtime.backup;
using com.absence.dialoguesystem.runtime.backup.data;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

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
    [MovedFrom("ActionNode")]
    public class EventNode : Node, IContainVariableManipulators
    {
        public static string CreationMenuName => "Event";

        protected const string k_none = "None";

        public bool UsedByMapper = false;
        [ShowIf(nameof(UsedByMapper))] public string UniqueMapperId;

        [Space(10)]

        [Tooltip("All of the Blackboard based events of this node.")]
        public List<NodeVariableSetter> BlackboardEvents = new();

        [Space(10)]

        [HideInInspector] public Node Next;

        /// <summary>
        /// Use to define what to do when this action node gets passed on the flow.
        /// </summary>
        protected virtual void CustomAction()
        {

        }

        public override string Title => "Event";

        public override List<string> AdditionalUSSFileLocations => new List<string>()
        {
            "Assets/Plugins/absencee_/absent-dialogues/Editor/BuiltIn/StyleSheets/EventNodeView.uss"
        };

        protected override Node OnPass(DialogueFlowContext context)
        {
            BlackboardEvents.ForEach(action => action.Perform());
            CustomAction();

            return Next;
        }
        protected override void OnReach(DialogueFlowContext context)
        {
            if (UsedByMapper)
            {
                context.InvokeAction = true;
                context.ActionId = UniqueMapperId;
            }

            SetState(FlowState.Current);
        }

        protected override void OnAddOutputConnection(Node nextWillBeAdded, int atPort)
        {
            Next = nextWillBeAdded;
        }
        protected override void OnRemoveOutputConnection(int atPort)
        {
            Next = null;
        }
        protected override void WriteOutputConnections(ref List<Node> result)
        {
            result.Add(Next);
        }

        public override void OnCloning(Dialogue originalDialogue, Dialogue clonedDialogue)
        {
            if (Next != null) Next = clonedDialogue.AllNodes[originalDialogue.AllNodes.IndexOf(Next)];

            BlackboardEvents = BlackboardEvents.ConvertAll(action =>
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

        public List<NodeVariableSetter> GetSetters() => new(BlackboardEvents);

        public override void OnValidate()
        {
            BlackboardEvents.ForEach(setter => setter.SetBlackboardBank(Blackboard.Bank));

            base.OnValidate();
        }

        public override void OnImport(NodeData dataToRead, DialogueImportContext context)
        {
            UsedByMapper = (bool)dataToRead.BoxedData[0];
            UniqueMapperId = dataToRead.Data;
            BlackboardEvents = dataToRead.SetterData.ToList().ConvertAll(setterData => DataReader.ReadSetterData(setterData)).ToList();
        }

        public override void OnExport(NodeData dataToWrite)
        {
            dataToWrite.BoxedData = new object[1];
            dataToWrite.BoxedData[0] = (object)UsedByMapper;

            dataToWrite.Data = UniqueMapperId;
            dataToWrite.SetterData = BlackboardEvents.ConvertAll(setter => DataGenerator.GenerateSetterData(setter)).ToArray();
        }
    }

}