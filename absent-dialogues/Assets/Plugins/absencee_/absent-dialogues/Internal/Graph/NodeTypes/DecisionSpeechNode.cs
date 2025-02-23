using com.absence.dialoguesystem.runtime.backup;
using com.absence.dialoguesystem.runtime.backup.data;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace com.absence.dialoguesystem.internals 
{
    /// <summary>
    /// Node which displays a speech with options.
    /// </summary>
    [HelpURL("https://b1lodhand.github.io/absent-dialogues/api/com.absence.dialoguesystem.internals.DecisionSpeechNode.html")]
    public sealed class DecisionSpeechNode : Node, IDialogueNode, IPerformDelayedClone, IContainVariableManipulators
    {
        public static string ParentCreationMenu => "Dialogue";

        [Space(10)]
        
        [HideInInspector, Tooltip("All of the options of this node.")] 
        public List<Option> Options = new List<Option>();

        [HideInInspector] public string m_text;

        [HideInInspector] public Node NativeNextNode;
        [HideInInspector] public List<Node> GenericOptionLeads; 

        public override bool PersonDependent => true;

        public string Text { get => m_text; set { m_text = value; } }
        List<Option> IDialogueNode.Options { get => Options; set { Options = value; } }

        public override string GetClassName() => "decisionSpeechNode";
        public override string GetTitle()
        {
            if (Options.Count > 0) return "Prompt";
            else return "Prompt (Optionless)";
        }

        protected override void OnPass(DialogueFlowContext context)
        {
            context.ClearSpeech();

            var optionSelected = context.OptionIndex;

            if (Options.Count == 0)
            {
                if (NativeNextNode == null) return;

                NativeNextNode.Reach(context);
                return;
            }

            if (Options[optionSelected].LeadsTo == null) return;

            Options[optionSelected].LeadsTo.Reach(context);
        }
        protected override void OnReach(DialogueFlowContext context)
        {
            List<OptionHandle> temp = new();
            Options.ForEach(o =>
            {
                if(!o.IsVisible()) return;

                temp.Add(new OptionHandle(Options.IndexOf(o), o.Text));
            });

            context.Text = Text;
            if (Options.Count > 0) context.OptionIndexPairs = new(temp);
            else context.OptionIndexPairs = null;

            temp.Clear();
            temp = null;
        }

        protected override void AddNextNode_Internal(Node nextWillBeAdded, int atPort)
        {
            if (atPort >= Options.Count)
            {
                atPort -= Options.Count;
                GenericOptionLeads[atPort] = nextWillBeAdded;
                return;
            }

            Options[atPort].LeadsTo = nextWillBeAdded;
        }
        protected override void RemoveNextNode_Internal(int atPort)
        {
            if (atPort >= Options.Count)
            {
                atPort -= Options.Count;
                GenericOptionLeads[atPort] = null;
                return;
            }

            Options[atPort].LeadsTo = null;
        }
        protected override void GetNextNodes_Internal(ref List<(int portIndex, Node node)> result)
        {
            foreach (var o in Options.ToArray())
            {
                if (o != null && o.LeadsTo != null) result.Add((Options.IndexOf(o), o.LeadsTo));
            }

            for (int i = 0; i < GenericOptionLeads.Count; i++) 
            { 
                Node target = GenericOptionLeads[i];
                    
                if (target != null) result.Add((Options.Count + i, target));
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

        public override List<string> GetOutputPortNamesForCreation()
        {
            if (Options.Count == 0 && (GenericOptionLeads == null || GenericOptionLeads.Count == 0)) 
                return new List<string>() { "To" };

            return new List<string>();
        }

        public void DelayedClone(Dialogue originalDialogue, Dialogue clonedDialogue)
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