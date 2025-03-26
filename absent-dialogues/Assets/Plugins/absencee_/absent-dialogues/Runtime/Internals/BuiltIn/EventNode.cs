using com.absence.attributes;
using com.absence.dialoguesystem.internals.backup;
using com.absence.dialoguesystem.internals.backup.data;
using System.Collections.Generic;
using System.Text;
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
    public class EventNode : Node
    {
        public static string CreationMenuName => "Event";

        protected const string k_none = "None";

        public bool UsedByMapper = false;
        [ShowIf(nameof(UsedByMapper))] public string UniqueMapperId;

        [Space(10)]

        [SerializeField, Tooltip("All of the Blackboard based events of this node.")]
        protected List<NodeVariableSetter> m_blackboardEvents = new();

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
            return Next;
        }
        protected override void OnReach(DialogueFlowContext context)
        {
            m_blackboardEvents.ForEach(action => action.Perform(context.BankHandle));
            CustomAction();

            if (UsedByMapper)
            {
                context.InvokeAction = true;
                context.ActionId = UniqueMapperId;
            }
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

        public override List<NodeVariableComparer> Comparers => null;

        public override List<NodeVariableSetter> Setters
        {
            get
            {
                return m_blackboardEvents;
            }

            set
            {
                m_blackboardEvents = value;
            }
        }

        public override string GenerateTopInfoText()
        {
            return GenerateIconTooltip(true);
        }

        public virtual string GenerateIconTooltip(bool richText = false)
        {
            StringBuilder sb = new();

            m_blackboardEvents.ForEach(setter =>
            {
                sb.Append(setter.GetSettingString(richText));
                sb.Append("\n");
            });

            return sb.ToString().TrimEnd('\n');
        }

        public override void OnImport(NodeData dataToRead, DialogueImportContext context)
        {
            base.OnImport(dataToRead, context);

            UsedByMapper = dataToRead.BoolData[0];
            UniqueMapperId = dataToRead.StringData[0];
        }

        public override void OnExport(NodeData dataToWrite)
        {
            base.OnExport(dataToWrite);

            dataToWrite.BoolData = new bool[1];
            dataToWrite.BoolData[0] = UsedByMapper;

            dataToWrite.StringData = new string[1];
            dataToWrite.StringData[0] = UniqueMapperId;
        }
    }

}