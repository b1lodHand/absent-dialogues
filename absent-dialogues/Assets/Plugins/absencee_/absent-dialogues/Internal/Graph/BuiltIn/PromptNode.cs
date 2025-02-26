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
    /// Node which displays a speech with options.
    /// </summary>
    [HelpURL("https://b1lodhand.github.io/absent-dialogues/api/com.absence.dialoguesystem.internals.DecisionSpeechNode.html")]
    [MovedFrom("DecisionSpeechNode")]
    public sealed class PromptNode : Node
    {
        public static string CreationMenuName => "Prompt";

        [Space(10)]
        
        [HideInInspector, SerializeField, Tooltip("All of the options of this node.")] 
        private List<Option> m_options = new List<Option>();

        [HideInInspector] public string m_text = string.Empty;

        [HideInInspector] public Node NativeNextNode;
        [HideInInspector] public List<Node> GenericOptionLeads; 

        public override bool PersonDependent => true;

        public override string Text { get => m_text; set { m_text = value; } }
        public override List<Option> Options { get => m_options; set { m_options = value; } }

        public bool NoOptionsOverall =>
            m_options.Count == 0 && (GenericOptionLeads == null || GenericOptionLeads.Count == 0);

        public override string Title
        {
            get
            {
                if (NoOptionsOverall) return "Prompt (Optionless)";
                else return "Prompt";
            }
        }

        public override List<string> AdditionalUSSFileLocations => new List<string>()
        {
            "Assets/Plugins/absencee_/absent-dialogues/Editor/BuiltIn/StyleSheets/PromptNodeView.uss"
        };

        protected override Node OnPass(DialogueFlowContext context)
        {
            context.ClearText();
            context.OptionIndexPairs = null;

            int optionSelected = context.OptionIndex;
            int optionCount = m_options.Count;

            if (NoOptionsOverall && optionCount == 0)
                return NativeNextNode;

            if (optionSelected >= optionCount)
                return GenericOptionLeads[optionSelected - optionCount];

            return m_options[optionSelected].LeadsTo;
        }
        protected override void OnReach(DialogueFlowContext context)
        {
            List<OptionHandle> handles = new();
            m_options.ForEach(o =>
            {
                if(!o.IsVisible()) return;

                handles.Add(new OptionHandle(m_options.IndexOf(o), o.Text));
            });

            context.Text = Text;
            if (m_options.Count > 0) context.OptionIndexPairs = handles;
            else context.OptionIndexPairs = null;
        }

        protected override void OnAddOutputConnection(Node nextWillBeAdded, int atPort)
        {
            if (NoOptionsOverall && atPort == 0)
            {
                NativeNextNode = nextWillBeAdded;
                return;
            }

            if (atPort >= m_options.Count)
            {
                atPort -= m_options.Count;
                GenericOptionLeads[atPort] = nextWillBeAdded;
                return;
            }

            m_options[atPort].LeadsTo = nextWillBeAdded;
        }
        protected override void OnRemoveOutputConnection(int atPort)
        {
            if (NoOptionsOverall && atPort == 0)
            {
                NativeNextNode = null;
                return;
            }

            if (atPort >= m_options.Count)
            {
                atPort -= m_options.Count;
                GenericOptionLeads[atPort] = null;
                return;
            }

            m_options[atPort].LeadsTo = null;
        }
        protected override void WriteOutputConnections(ref List<Node> result)
        {
            if (NoOptionsOverall)
            {
                result.Add(NativeNextNode);
                return;
            }

            foreach (Option option in m_options)
            {
                if (option != null) result.Add(option.LeadsTo);
                else result.Add(null);
            }

            foreach (Node target in GenericOptionLeads) 
            { 
                result.Add(target);
            }
        }

        public override List<string> GetDefaultOutputPortNames()
        {
            if (NoOptionsOverall) 
                return new List<string>() { "To" };

            return new List<string>();
        }

        public override void OnCloning(Dialogue originalDialogue, Dialogue clonedDialogue)
        {
            base.OnCloning(originalDialogue, clonedDialogue);

            if (NativeNextNode != null)
                NativeNextNode = clonedDialogue.AllNodes[originalDialogue.AllNodes.IndexOf(NativeNextNode)];

            m_options.ForEach(opt =>
            {
                opt.LeadsTo = clonedDialogue.AllNodes[originalDialogue.AllNodes.IndexOf(opt.LeadsTo)];
            });
        }

        public override List<NodeVariableComparer> Comparers
        {
            get
            {
                List<NodeVariableComparer> result = new();

                m_options.ForEach(option =>
                {
                    option.Visibility.ShowIfList.ForEach(comparer =>
                    {
                        result.Add(comparer);
                    });
                });

                return result;
            }
        }

        public override List<NodeVariableSetter> Setters => null;

        public override void OnImport(NodeData dataToRead, DialogueImportContext context)
        {
            m_text = dataToRead.Data;
            m_options = dataToRead.OptionData.ToList().ConvertAll(optionData => DataReader.ReadOptionData(optionData)).ToList();
        }

        public override void OnExport(NodeData dataToWrite)
        {
            dataToWrite.Data = m_text;
            dataToWrite.OptionData = m_options.ConvertAll(option => DataGenerator.GenerateOptionData(option)).ToArray();
        }
    }
}