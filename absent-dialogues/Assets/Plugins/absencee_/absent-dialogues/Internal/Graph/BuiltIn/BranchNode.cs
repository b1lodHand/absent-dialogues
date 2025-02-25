using com.absence.dialoguesystem.runtime.backup;
using com.absence.dialoguesystem.runtime.backup.data;
using com.absence.dialoguesystem.runtime.backup.internals;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace com.absence.dialoguesystem.internals
{
    /// <summary>
    /// Node which re-routes the flow under some conditions.
    /// </summary>
    [HelpURL("https://b1lodhand.github.io/absent-dialogues/api/com.absence.dialoguesystem.internals.ConditionNode.html")]
    public class BranchNode : Node, IContainVariableManipulators
    {
        public static string CreationMenuName => "Branch";

        [HideInInspector] public Node TrueNext;
        [HideInInspector] public Node FalseNext;

        [Tooltip("Use to declare what to do with the sum of the results of comparers.")] public VBProcessType Processor = VBProcessType.All;
        [Tooltip("All of the comparers this node relies on.")] public List<NodeVariableComparer> Comparers = new();

        public override string Title => "Branch";

        public override List<string> AdditionalUSSFileLocations => new List<string>()
        {
            "Assets/Plugins/absencee_/absent-dialogues/Editor/BuiltIn/StyleSheets/BranchNodeView.uss"
        };

        protected override Node OnPass(DialogueFlowContext context)
        {
            bool result = Process();
            Node targetNext = result ? TrueNext : FalseNext;

            return targetNext;
        }
        protected override void OnReach(DialogueFlowContext context)
        {

        }

        protected override void OnAddOutputConnection(Node nextWillBeAdded, int atPort)
        {
            if (atPort == 0) TrueNext = nextWillBeAdded;
            else if (atPort == 1) FalseNext = nextWillBeAdded;
        }
        protected override void OnRemoveOutputConnection(int atPort)
        {
            if (atPort == 0) TrueNext = null;
            else if (atPort == 1) FalseNext = null;
        }
        protected override void WriteOutputConnections(ref List<Node> result)
        {
            result.Add(TrueNext);
            result.Add(FalseNext);
        }

        public override void OnCloning(Dialogue originalDialogue, Dialogue clonedDialogue)
        {
            if (TrueNext != null) TrueNext = clonedDialogue.AllNodes[originalDialogue.AllNodes.IndexOf(TrueNext)];
            if (FalseNext != null) FalseNext = clonedDialogue.AllNodes[originalDialogue.AllNodes.IndexOf(FalseNext)];

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

        public override List<string> GetDefaultOutputPortNames()
        {
            return new List<string>() { "True", "False" };
        }

        /// <summary>
        /// Use this to override (if you need) the checking result of this node.
        /// </summary>
        /// <returns>Normally returns the sum of the results of node's comparer list in a way declared by <see cref="Processor"/></returns>
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

        public List<NodeVariableComparer> GetComparers() => new(Comparers);
        public List<NodeVariableSetter> GetSetters() => null;

        public override void OnImport(NodeData dataToRead, DialogueImportContext context)
        {
            Processor = DialogueImportSettings.ProcessorDictionary[dataToRead.ComparerProcessorType];
            Comparers = dataToRead.ComparerData.ToList().ConvertAll(comparerData => DataReader.ReadComparerData(comparerData)).ToList();
        }

        public override void OnExport(NodeData dataToWrite)
        {
            dataToWrite.ComparerProcessorType = DialogueExportSettings.ProcessorDictionary[Processor];
            dataToWrite.ComparerData = Comparers.ConvertAll(comparer => DataGenerator.GenerateComparerData(comparer)).ToArray();
        }

        public override void OnValidate()
        {
            Comparers.ForEach(comparer => comparer.SetBlackboardBank(Blackboard.Bank));

            base.OnValidate();
        }

        public string GetConditionString(bool richText = false)
        {
            return Utilities.Comparison.GetConditionString(Comparers, Processor, richText);
        }
    }
}