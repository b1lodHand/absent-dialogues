using com.absence.dialoguesystem.runtime.backup;
using com.absence.dialoguesystem.runtime.backup.data;
using com.absence.dialoguesystem.runtime.backup.internals;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace com.absence.dialoguesystem.internals
{
    /// <summary>
    /// Node which re-routes the flow under some conditions.
    /// </summary>
    [HelpURL("https://b1lodhand.github.io/absent-dialogues/api/com.absence.dialoguesystem.internals.ConditionNode.html")]
    public class BranchNode : Node
    {
        public static string CreationMenuName => "Branch";

        [HideInInspector] public Node TrueNext;
        [HideInInspector] public Node FalseNext;

        [Tooltip("Use to declare what to do with the sum of the results of comparers.")] public ConditionProcessMode Mode = ConditionProcessMode.All;
        [SerializeField, Tooltip("All of the comparers this node relies on.")] protected List<NodeVariableComparer> m_conditions = new();

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

        public override List<string> GetDefaultOutputPortNames()
        {
            return new List<string>() { "True", "False" };
        }

        /// <summary>
        /// Use this to override (if you need) the checking result of this node.
        /// </summary>
        /// <returns>Normally returns the sum of the results of node's comparer list in a way declared by <see cref="Mode"/></returns>
        protected virtual bool Process()
        {
            if (m_conditions.Count == 0) return true;

            bool result = true;
            switch (Mode)
            {
                case ConditionProcessMode.All:
                    result = m_conditions.All(c => c.GetResult());
                    break;
                case ConditionProcessMode.Any:
                    result = m_conditions.Any(c => c.GetResult());
                    break;
            }

            return result;
        }

        public override List<NodeVariableComparer> Comparers
        {
            get { return m_conditions; }
            set { m_conditions = value; }
        }

        public override List<NodeVariableSetter> Setters => null;

        public override void OnImport(NodeData dataToRead, DialogueImportContext context)
        {
            base.OnImport(dataToRead, context);

            Mode = DialogueImportSettings.ProcessorDictionary[dataToRead.ComparerProcessorType];
            //m_conditions = dataToRead.ComparerData.ToList().ConvertAll(comparerData => DataReader.ReadComparerData(comparerData)).ToList();
        }

        public override void OnExport(NodeData dataToWrite)
        {
            base.OnExport(dataToWrite);

            dataToWrite.ComparerProcessorType = DialogueExportSettings.ProcessorDictionary[Mode];
            //dataToWrite.ComparerData = m_conditions.ConvertAll(comparer => DataGenerator.GenerateComparerData(comparer)).ToArray();
        }

        public string GetConditionString(bool richText = false)
        {
            return Utilities.Comparison.GetConditionString(m_conditions, Mode, richText);
        }
    }
}