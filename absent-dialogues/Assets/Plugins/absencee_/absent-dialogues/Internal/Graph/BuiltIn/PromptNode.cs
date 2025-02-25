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
    public sealed class PromptNode : Node, IDialogueNode, IContainVariableManipulators
    {
        public static string CreationMenuName => "Prompt";

        [Space(10)]
        
        [HideInInspector, Tooltip("All of the options of this node.")] 
        public List<Option> Options = new List<Option>();

        [HideInInspector] public string m_text;

        [HideInInspector] public Node NativeNextNode;
        [HideInInspector] public List<Node> GenericOptionLeads; 

        public override bool PersonDependent => true;

        public string Text { get => m_text; set { m_text = value; } }
        List<Option> IDialogueNode.Options { get => Options; set { Options = value; } }

        public bool NoOptionsOverall => 
            Options.Count == 0 && (GenericOptionLeads == null || GenericOptionLeads.Count == 0);

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
            int optionCount = Options.Count;

            if (NoOptionsOverall && optionCount == 0)
                return NativeNextNode;

            if (optionSelected >= optionCount)
                return GenericOptionLeads[optionSelected - optionCount];

            return Options[optionSelected].LeadsTo;
        }
        protected override void OnReach(DialogueFlowContext context)
        {
            List<OptionHandle> handles = new();
            Options.ForEach(o =>
            {
                if(!o.IsVisible()) return;

                handles.Add(new OptionHandle(Options.IndexOf(o), o.Text));
            });

            context.Text = Text;
            if (Options.Count > 0) context.OptionIndexPairs = handles;
            else context.OptionIndexPairs = null;
        }

        protected override void OnAddOutputConnection(Node nextWillBeAdded, int atPort)
        {
            if (NoOptionsOverall && atPort == 0)
            {
                NativeNextNode = nextWillBeAdded;
                return;
            }

            if (atPort >= Options.Count)
            {
                atPort -= Options.Count;
                GenericOptionLeads[atPort] = nextWillBeAdded;
                return;
            }

            Options[atPort].LeadsTo = nextWillBeAdded;
        }
        protected override void OnRemoveOutputConnection(int atPort)
        {
            if (NoOptionsOverall && atPort == 0)
            {
                NativeNextNode = null;
                return;
            }

            if (atPort >= Options.Count)
            {
                atPort -= Options.Count;
                GenericOptionLeads[atPort] = null;
                return;
            }

            Options[atPort].LeadsTo = null;
        }
        protected override void WriteOutputConnections(ref List<Node> result)
        {
            if (NoOptionsOverall)
            {
                result.Add(NativeNextNode);
                return;
            }

            foreach (Option option in Options)
            {
                if (option != null) result.Add(option.LeadsTo);
                else result.Add(null);
            }

            foreach (Node target in GenericOptionLeads) 
            { 
                result.Add(target);
            }
        }

        public override void Traverse(Action<Node> action)
        {
            action?.Invoke(this);
            Options.ForEach(option =>
            {
                option.LeadsTo.Traverse(action);
            });
        }

        public override List<string> GetDefaultOutputPortNames()
        {
            if (NoOptionsOverall) 
                return new List<string>() { "To" };

            return new List<string>();
        }

        public override void OnCloning(Dialogue originalDialogue, Dialogue clonedDialogue)
        {
            Options = Options.ConvertAll(opt => opt.Clone(Blackboard.Bank));
            if (NativeNextNode != null)
                NativeNextNode = clonedDialogue.AllNodes[originalDialogue.AllNodes.IndexOf(NativeNextNode)];

            Options.ForEach(opt =>
            {
                if (opt.LeadsTo == null) return;

                opt.LeadsTo = clonedDialogue.AllNodes[originalDialogue.AllNodes.IndexOf(opt.LeadsTo)];
            });
        }

        public List<NodeVariableComparer> GetComparers()
        {
            List<NodeVariableComparer> result = new();

            Options.ForEach(option =>
            {
                option.Visibility.ShowIfList.ForEach(comparer =>
                {
                    result.Add(comparer);
                });
            });

            return result;
        }

        public List<NodeVariableSetter> GetSetters() => null;

        public override void OnImport(NodeData dataToRead, DialogueImportContext context)
        {
            m_text = dataToRead.Data;
            Options = dataToRead.OptionData.ToList().ConvertAll(optionData => DataReader.ReadOptionData(optionData)).ToList();
        }

        public override void OnExport(NodeData dataToWrite)
        {
            dataToWrite.Data = m_text;
            dataToWrite.OptionData = Options.ConvertAll(option => DataGenerator.GenerateOptionData(option)).ToArray();
        }

        public override void OnValidate()
        {
            Options.ForEach(option =>
            {
                option.Visibility.ShowIfList.ForEach(comparer => comparer.SetBlackboardBank(Blackboard.Bank));
            });

            base.OnValidate();
        }
    }
}